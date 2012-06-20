﻿using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CloudberryKingdom
{
    public static class KeyboardExtension
    {
        static bool Freeze = false;
        public static void FreezeInput() { Freeze = true; }
        public static void UnfreezeInput() { Freeze = false; }

        public static bool IsKeyDownCustom(this KeyboardState keyboard, Keys key)
        {
            if (Freeze) return false;

            return keyboard.IsKeyDown(key);
        }
    }

    public class ButtonStatistics
    {
        int[] _DownCount = new int[Tools.Length<ControllerButtons>()];

        public int DownCount(ControllerButtons button)
        {
            return _DownCount[(int)button];
        }

        /// <summary>
        /// If Incr is true then the increment is increased by 1, otherwise it is set to 0.
        /// </summary>
        public void IncrCount(ControllerButtons button, bool Incr)
        {
            if (Incr) IncrCount(button);
            else SetCount(button, 0);
        }

        public void IncrCount(ControllerButtons button)
        {
            _DownCount[(int)button]++;
        }

        public void SetCount(ControllerButtons button, int count)
        {
            _DownCount[(int)button] = count;
        }
    }

    public static class ButtonStats
    {
        public static ButtonStatistics[] Controller;
        public static ButtonStatistics All;

        static void Init()
        {
            if (Controller == null)
            {
                Controller = new ButtonStatistics[4];
                All = new ButtonStatistics();

                for (int i = 0; i < 4; i++)
                    Controller[i] = new ButtonStatistics();
            }
        }

        public static void Update()
        {
            Init();

            bool Incr = false;
            for (int i = 0; i < 4; i++)
            {
                if (Tools.padState[i].Buttons.A == ButtonState.Pressed)
                {
                    Controller[i].IncrCount(ControllerButtons.A);
                    Incr = true;
                }
                else
                    Controller[i].SetCount(ControllerButtons.A, 0);

                Vector2 dir = ButtonCheck.GetDir(i);
                Controller[i].IncrCount(ControllerButtons.Right, dir.X > .7f);
                Controller[i].IncrCount(ControllerButtons.Left, dir.X < -.7f);
                Controller[i].IncrCount(ControllerButtons.Up, dir.Y > .7f);
                Controller[i].IncrCount(ControllerButtons.Down, dir.Y < -.7f);
            }

            if (Incr)
                All.IncrCount(ControllerButtons.A);
            else
                All.SetCount(ControllerButtons.A, 0);

            ControllerButtons[] buttons = { ControllerButtons.Left, ControllerButtons.Right, ControllerButtons.Up, ControllerButtons.Down };
            foreach (ControllerButtons button in buttons)
            {
                Incr = false;
                for (int i = 0; i < 4; i++)
                    if (Controller[i].DownCount(button) > 0)
                        Incr = true;

                All.IncrCount(button, Incr);
            }
        }
    }

    public class ButtonClass
    {
        public ControllerButtons ControllerButton = ControllerButtons.None;
        public Keys KeyboardKey = Keys.None;
        public bool IsKeyboard = true;

        public void Set(Keys key)
        {
            KeyboardKey = key;
            IsKeyboard = true;
        }
        public void Set(ControllerButtons key)
        {
            ControllerButton = key;
            IsKeyboard = false;
        }
    }

    public enum ControllerButtons { A, B, X, Y, RS, LS, RT, LT, RJ, RJButton, LJ, LJButton, DPad, Start, Back, Left, Right, Up, Down, Enter, None };

    public enum MashType { Hold, Tap, Alternate, HoldDir };

    public struct ButtonData { public bool Down; public bool Pressed; public bool Released; public Vector2 Dir; public float Squeeze; }
    public class ButtonCheck
    {
        public static void KillSecondary()
        {
            Help_KeyboardKey.Set(Keys.None);
            Quickspawn_KeyboardKey.Set(Keys.None);
            Start_Secondary = Go_Secondary =
                Back_Secondary = ReplayPrev_Secondary = ReplayNext_Secondary = SlowMoToggle_Secondary =
                Left_Secondary = Right_Secondary = Up_Secondary = Down_Secondary = Keys.None;
        }

        public static ButtonClass Quickspawn_KeyboardKey = new ButtonClass(), Help_KeyboardKey = new ButtonClass(),
                                  QuickReset_KeyboardKey = new ButtonClass();
        public static Keys Start_Secondary, Go_Secondary,
                Back_Secondary,
                ReplayPrev_Secondary, ReplayNext_Secondary, ReplayToggle_Secondary,
                SlowMoToggle_Secondary,
                Left_Secondary, Right_Secondary, Up_Secondary, Down_Secondary;

        public static void Reset()
        {
            QuickReset_KeyboardKey.Set(Keys.F);
            Quickspawn_KeyboardKey.Set(Keys.Space);
            Help_KeyboardKey.Set(Keys.Enter);

            Start_Secondary = Keys.None;
            Go_Secondary = Keys.None;
            Back_Secondary = Keys.None;
            
            ReplayPrev_Secondary = Keys.N;
            ReplayNext_Secondary = Keys.M;
            ReplayToggle_Secondary = Keys.L;
            
            SlowMoToggle_Secondary = Keys.C;
            
            Left_Secondary = Keys.A;
            Right_Secondary = Keys.D;
            Up_Secondary = Keys.W;
            Down_Secondary = Keys.S;
        }

        public static float ThresholdSensitivity = .715f;//.75f;

        public ControllerButtons MyButton1, MyButton2;
        public int MyPlayerIndex;
        public MashType MyType;
        ButtonData Current, Previous;

        public int GapCount, GapAllowance;
        public int Dir;

        public bool Satisfied;

        static public int Direction(Vector2 Dir)
        {
            if (Dir.Length() < .25f) return -1;
            if (Dir.Y < Dir.X && Dir.Y > -Dir.X) return 0;
            if (Dir.Y > Dir.X && Dir.Y > -Dir.X) return 1;
            if (Dir.Y < -Dir.X && Dir.Y > Dir.X) return 2;
            if (Dir.Y < Dir.X && Dir.Y < -Dir.X) return 3;
            return -1;
        }

        static public Vector2 GetDir(int Control)
        {
            Vector2 Dir = ButtonCheck.State(ControllerButtons.DPad, Control).Dir;
            Vector2 HoldDir = ButtonCheck.State(ControllerButtons.LJ, Control).Dir;
            if (Math.Abs(HoldDir.X) > Math.Abs(Dir.X)) Dir.X = HoldDir.X;
            if (Math.Abs(HoldDir.Y) > Math.Abs(Dir.Y)) Dir.Y = HoldDir.Y;

            return Dir;
        }

        static public Vector2 GetMaxDir() { return GetMaxDir(false); }
        static public Vector2 GetMaxDir(int Control) { return GetMaxDir(Control == -1); }
        static public Vector2 GetMaxDir(bool MustExist)
        {
            if (PlayerManager.Players == null ||
                PlayerManager.Players[0] == null ||
                PlayerManager.Players[1] == null ||
                PlayerManager.Players[2] == null ||
                PlayerManager.Players[3] == null)
                return Vector2.Zero;

            Vector2 Dir = Vector2.Zero;
            for (int i = 0; i < 4; i++)
            {
                if (PlayerManager.Get(i).Exists || !MustExist)
                {
                    Vector2 HoldDir = ButtonCheck.State(ControllerButtons.LJ, i).Dir;
                    if (Math.Abs(HoldDir.X) > Math.Abs(Dir.X)) Dir.X = HoldDir.X;
                    if (Math.Abs(HoldDir.Y) > Math.Abs(Dir.Y)) Dir.Y = HoldDir.Y;

                    HoldDir = ButtonCheck.State(ControllerButtons.DPad, i).Dir;
                    if (Math.Abs(HoldDir.X) > Math.Abs(Dir.X)) Dir.X = HoldDir.X;
                    if (Math.Abs(HoldDir.Y) > Math.Abs(Dir.Y)) Dir.Y = HoldDir.Y;
                }
            }

            return Dir;
        }


        static public bool PreventNextInput = false;
        static public int PreventTimeStamp;
        static public void PreventInput()
        {
#if MIRROR_ALL
            return;
#endif

            PreventNextInput = true;
            PreventTimeStamp = Tools.TheGame.DrawCount;
        }

        /// <summary>
        /// Check for a standard back input (B) or (Esc)
        /// </summary>
        /// <returns></returns>
        public static bool Back(int Control)
        {
            if (ButtonCheck.State(ControllerButtons.B, Control, false).Pressed 
#if WINDOWS
                || ButtonCheck.State(Keys.Escape).Pressed
                || ButtonCheck.State(Keys.Back).Pressed)
#else
                )
#endif
                return true;
            else
                return false;
        }

        static public ButtonData State(Keys Key)
        {
            ButtonData Data = new ButtonData();
#if WINDOWS
            Data.Down = Tools.keybState.IsKeyDownCustom(Key);
            Data.Pressed = Data.Down && !Tools.PrevKeyboardState.IsKeyDownCustom(Key);
            Data.Released = !Data.Down && Tools.PrevKeyboardState.IsKeyDownCustom(Key);
#endif
            return Data;
        }

        static public ButtonData AllState(int iPlayerIndex)
        {
            return State(iPlayerIndex, ControllerButtons.A, ControllerButtons.B,
                                       ControllerButtons.X, ControllerButtons.Y,
                                       ControllerButtons.LS, ControllerButtons.RS);
        }

#if WINDOWS
        /// <summary>
        /// Returns true if a keyboard go key is pressed (Enter, Z, Space)
        /// </summary>
        public static bool KeyboardGo()
        {
            return Tools.keybState.IsKeyDownCustom(Keys.Enter)
                || Tools.keybState.IsKeyDownCustom(Keys.Space)
                || Tools.keybState.IsKeyDownCustom(Go_Secondary);
        }
#endif

        static public ButtonData State(int iPlayerIndex, params ControllerButtons[] ButtonList)
        {
            ButtonData data = new ButtonData();
            foreach (ControllerButtons button in ButtonList)
            {
                ButtonData newdata = State(button, iPlayerIndex);

                data.Down |= newdata.Down;
                data.Pressed |= newdata.Pressed;
                data.Released |= newdata.Released;
                data.Squeeze = Math.Max(data.Squeeze, newdata.Squeeze);
            }

            return data;
        }

        /// <summary>
        /// When true it is assumed that at least one player is officially logged in.
        /// When false a player index of -1 (any signed in player) will be interpreted as -1 (any player).
        /// </summary>
        public static bool PreLogIn = true;

        static public ButtonData State(ButtonClass Button, PlayerIndex Index)
        {
            return State(Button, -2);
        }
        static public ButtonData State(ButtonClass Button, int iPlayerIndex)
        {
            if (Button == null)
                return State(ControllerButtons.None, iPlayerIndex);

            if (Button.IsKeyboard)
                return GetState(Button.KeyboardKey, false);
            else
                return State(Button.ControllerButton, iPlayerIndex);
        }
        static ButtonData GetState(Keys Key, bool Prev)
        {
            ButtonData Data = new ButtonData();
            if (Key == Keys.None) return Data;

            if (PreventNextInput)
            {
                Data.Pressed = Data.Down = false;

                if (Tools.TheGame.DrawCount > PreventTimeStamp + 3)
                    PreventNextInput = false;

                return Data;
            }

#if WINDOWS
            KeyboardState keyboard;
            if (Prev)
                keyboard = Tools.PrevKeyboardState;
            else
                keyboard = Tools.keybState;

            Data.Down = keyboard.IsKeyDownCustom(Key);
#endif

            // Get previous data to calculate Pressed and Released
            if (!Prev)
            {
                ButtonData prevdata = GetState(Key, true);

                // Pressed == true if the previous state was not pressed but the current is
                if (Data.Down && !prevdata.Down)
                    Data.Pressed = true;

                // Released == true if the previous state was not Released but the current is
                if (!Data.Down && prevdata.Down)
                    Data.Released = true;
            }

            return Data;
        }




        static public ButtonData State(ControllerButtons Button, PlayerIndex Index) { return GetState(Button, (int)Index, false, true); }
        static public ButtonData State(ControllerButtons Button, int iPlayerIndex) { return GetState(Button, iPlayerIndex, false, true); }
        static public ButtonData State(ControllerButtons Button, int iPlayerIndex, bool UseKeyboardMapping) { return GetState(Button, iPlayerIndex, false, UseKeyboardMapping); }
        static ButtonData GetState(ControllerButtons Button, int iPlayerIndex, bool Prev, bool UseKeyboardMapping)
        {
            // Debug tool: Use this to set the keyboard for use by player 1/2/3/4
            bool SingleOutPlayer = false;
            int ThisPlayerOnly = 1;

            if (PreLogIn && iPlayerIndex == -1) iPlayerIndex = -2;

#if MIRROR_ALL
            iPlayerIndex = 0;
#endif
            ButtonData Data = new ButtonData();
            if (Button == ControllerButtons.None) return Data;
#if XBOX
            if (Button == ControllerButtons.Enter) return Data;
#endif

            if (SingleOutPlayer && iPlayerIndex >= 0 && iPlayerIndex != ThisPlayerOnly) return Data;

            if (PreventNextInput)
            {
                Data.Pressed = Data.Down = false;

                if (Tools.TheGame.DrawCount > PreventTimeStamp + 3)
                    PreventNextInput = false;

                return Data;
            }

            if (Tools.AutoLoop)
            {
                if (Button == ControllerButtons.A)
                {
                    Data.Down = Prev;
                }
                return Data;
            }

            if (iPlayerIndex == -1 || iPlayerIndex == -2)
            {
                if (!Prev)
                {
                    bool NoneExist = PlayerManager.GetNumPlayers() == 0;

                    for (int i = 0; i < 4; i++)
                        if (NoneExist || PlayerManager.Get(i).Exists || iPlayerIndex == -2)
                        {
                            ButtonData data = GetState(Button, i, false, UseKeyboardMapping);
                            
                            Data.Down = Data.Down || data.Down;
                            Data.Pressed = Data.Pressed || data.Pressed;
                            Data.Released = Data.Released || data.Released;

                            if (data.Dir.LengthSquared() > Data.Dir.LengthSquared())
                                Data.Dir = data.Dir;
                        }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                        if (PlayerManager.Get(i).Exists || iPlayerIndex == -2)
                        {
                            ButtonData data = GetState(Button, i, true, UseKeyboardMapping);
                            Data.Down = Data.Down || data.Down;
                            Data.Pressed = Data.Pressed || data.Pressed;
                            Data.Released = Data.Released || data.Released;
                        }
                }
                return Data;
            }
#if WINDOWS

            Keys key = Keys.None;
            Keys SecondaryKey = Keys.None;
            Keys TertiaryKey = Keys.None;

            if (
                (SingleOutPlayer && iPlayerIndex == ThisPlayerOnly)
                ||
                (UseKeyboardMapping && (iPlayerIndex == 0 || PlayerManager.Get(iPlayerIndex).Exists))
                )
            {
//#if PC_VERSION
                if (Button == ControllerButtons.Enter) key = Keys.Enter;

                if (Button == ControllerButtons.Start)
                {
                    key = Keys.Escape;
                    TertiaryKey = Start_Secondary;
                }
                if (Button == ControllerButtons.A)
                {
                    TertiaryKey = Go_Secondary;
                }
                if (Button == ControllerButtons.B)
                {
                    key = Keys.Escape;
                    TertiaryKey = Back_Secondary;
                }
                if (Button == ControllerButtons.X)
                    TertiaryKey = SlowMoToggle_Secondary;
                //if (Button == ControllerButtons.Y)
                //    TertiaryKey = Help_Secondary;
                if (Button == ControllerButtons.LS)
                    TertiaryKey = ReplayPrev_Secondary;
                if (Button == ControllerButtons.RS)
                    TertiaryKey = ReplayNext_Secondary;


                if (Button == ControllerButtons.X) key = Keys.None;// Keys.C;
                if (Button == ControllerButtons.Y) key = Keys.None;// Keys.V;
                if (Button == ControllerButtons.RT) key = Keys.OemPeriod;
                if (Button == ControllerButtons.LT) key = Keys.OemComma;
                if (Button == ControllerButtons.LS) key = Keys.A;
                if (Button == ControllerButtons.RS) key = Keys.D;
//#else
//                if (Button == ControllerButtons.Start) key = Keys.S;
//                if (Button == ControllerButtons.Back) key = Keys.Escape;
//                if (Button == ControllerButtons.A) key = Keys.Z;
//                if (Button == ControllerButtons.B) key = Keys.X;
//                if (Button == ControllerButtons.X) key = Keys.C;
//                if (Button == ControllerButtons.Y) key = Keys.V;
//                if (Button == ControllerButtons.RT) key = Keys.OemPeriod;
//                if (Button == ControllerButtons.LT) key = Keys.OemComma;
//#endif

                if (Button == ControllerButtons.Start) SecondaryKey = Keys.Back;
                //if (Button == ControllerButtons.Back) SecondaryKey = Keys.Back;
                if (Button == ControllerButtons.B) SecondaryKey = Keys.Back;
            }

            KeyboardState keyboard;
            if (Prev)
                keyboard = Tools.PrevKeyboardState;
            else
                keyboard = Tools.keybState;

#endif
            //#else
            GamePadState Pad;
            if (Prev) Pad = Tools.PrevpadState[iPlayerIndex];
            else Pad = Tools.padState[iPlayerIndex];

            switch (Button)
            {
                case ControllerButtons.Start: Data.Down = (Pad.Buttons.Start == ButtonState.Pressed); break;
                case ControllerButtons.Back: Data.Down = (Pad.Buttons.Back == ButtonState.Pressed); break;
                case ControllerButtons.A: Data.Down = (Pad.Buttons.A == ButtonState.Pressed); break;
                case ControllerButtons.B: Data.Down = (Pad.Buttons.B == ButtonState.Pressed); break;
                case ControllerButtons.X: Data.Down = (Pad.Buttons.X == ButtonState.Pressed); break;
                case ControllerButtons.Y: Data.Down = (Pad.Buttons.Y == ButtonState.Pressed); break;
                case ControllerButtons.LJButton: Data.Down = (Pad.Buttons.LeftStick == ButtonState.Pressed); break;
                case ControllerButtons.RJButton: Data.Down = (Pad.Buttons.RightStick == ButtonState.Pressed); break;
                case ControllerButtons.LS: Data.Down = (Pad.Buttons.LeftShoulder == ButtonState.Pressed); break;
                case ControllerButtons.RS: Data.Down = (Pad.Buttons.RightShoulder == ButtonState.Pressed); break;
                case ControllerButtons.LT:
                    {
                        Data.Down = (Pad.Triggers.Left > .5f);
                        Data.Squeeze = Pad.Triggers.Left;
                        break;
                    }
                case ControllerButtons.RT:
                    {
                        Data.Down = (Pad.Triggers.Right > .5f);
                        Data.Squeeze = Pad.Triggers.Right;
                        break;
                    }
                case ControllerButtons.LJ: Data.Dir = Pad.ThumbSticks.Left; break;
                case ControllerButtons.RJ: Data.Dir = Pad.ThumbSticks.Right; break;
                case ControllerButtons.DPad:
                    {
                        Data.Dir = Vector2.Zero;
                        if (Pad.DPad.Right == ButtonState.Pressed) Data.Dir = new Vector2(1, 0);
                        if (Pad.DPad.Up == ButtonState.Pressed) Data.Dir = new Vector2(0, 1);
                        if (Pad.DPad.Left == ButtonState.Pressed) Data.Dir = new Vector2(-1, 0);
                        if (Pad.DPad.Down == ButtonState.Pressed) Data.Dir = new Vector2(0, -1);
                    }
                    break;
            }
            //#endif

#if WINDOWS
if (
    (SingleOutPlayer && iPlayerIndex == ThisPlayerOnly)
    ||
    (UseKeyboardMapping && iPlayerIndex == 0)
    )
//if (UseKeyboardMapping && iPlayerIndex == 0)
{
            if (Button == ControllerButtons.A)
            {
                if (Prev)
                    Data.Down |= Tools.CurMouseState.LeftButton == ButtonState.Pressed;
                else
                    Data.Down |= Tools.PrevMouseState.LeftButton == ButtonState.Pressed;
            }
            else
                Data.Down |= keyboard.IsKeyDownCustom(key);

            if (SecondaryKey != Keys.None)
                Data.Down |= keyboard.IsKeyDownCustom(SecondaryKey);

            if (TertiaryKey != Keys.None)
                Data.Down |= keyboard.IsKeyDownCustom(TertiaryKey);

            key = Keys.Escape;
    
            //if (Button == ControllerButtons.A) key = Keys.Z;
            //if (Button == ControllerButtons.B) key = Keys.X;

            if (Button == ControllerButtons.A) key = Go_Secondary;
            if (Button == ControllerButtons.B) key = Back_Secondary;

            if (key != Keys.Escape)
                Data.Down |= keyboard.IsKeyDownCustom(key);

            if (Button == ControllerButtons.A)
                Data.Down |= keyboard.IsKeyDownCustom(Keys.Enter)
                          || keyboard.IsKeyDownCustom(Keys.Space);
//#else
//    Data.Down |= keyboard.IsKeyDownCustom(key);
//#endif

            if (Button == ControllerButtons.LJ)
            {
                Vector2 KeyboardDir = Vector2.Zero;
                if (keyboard.IsKeyDownCustom(Keys.Left)) KeyboardDir.X = -1;
                if (keyboard.IsKeyDownCustom(Keys.Right)) KeyboardDir.X = 1;
                if (keyboard.IsKeyDownCustom(Keys.Up)) KeyboardDir.Y = 1;
                if (keyboard.IsKeyDownCustom(Keys.Down)) KeyboardDir.Y = -1;
                if (keyboard.IsKeyDownCustom(Left_Secondary)) KeyboardDir.X = -1;
                if (keyboard.IsKeyDownCustom(Right_Secondary)) KeyboardDir.X = 1;
                if (keyboard.IsKeyDownCustom(Up_Secondary)) KeyboardDir.Y = 1;
                if (keyboard.IsKeyDownCustom(Down_Secondary)) KeyboardDir.Y = -1;

                if (KeyboardDir.LengthSquared() > Data.Dir.LengthSquared())
                {
                    // Use the keyboard direction instead of the gamepad direction 
                    // and note that the keyboard was used.
                    if (iPlayerIndex >= 0) PlayerManager.Get(iPlayerIndex).KeyboardUsedLast = true;
                    Data.Dir = KeyboardDir;
                }
                else
                    if (iPlayerIndex >= 0) PlayerManager.Get(iPlayerIndex).KeyboardUsedLast = false;
            }
}
#endif

            // Get previous data to calculate Pressed and Released
            if (!Prev)
            {
                ButtonData prevdata = GetState(Button, iPlayerIndex, true, UseKeyboardMapping);

                // Pressed == true if the previous state was not pressed but the current is
                if (Data.Down && !prevdata.Down)
                    Data.Pressed = true;

                // Released == true if the previous state was not Released but the current is
                if (!Data.Down && prevdata.Down)
                    Data.Released = true;
            }

            return Data;
        }

        public ButtonCheck() { }

        public String GetString()
        {
            String s;

            switch (MyType)
            {
                case MashType.Hold: s = "Hold {p"; break;
                case MashType.Tap: s = "Tap {p"; break;
                case MashType.HoldDir: s = "Hold {p"; break;
                default: s = ""; break;
            }

            s += "Big_Button_" + Tools.ButtonNames[(int)MyButton1] + ",75,75}";

            switch (MyType)
            {
                case MashType.HoldDir: s += " " + Tools.DirNames[Dir]; break;
                default: break;
            }

            return s;
        }

        public void Phsx()
        {
            Satisfied = false;

            Previous = Current;
            Current = ButtonCheck.State(MyButton1, MyPlayerIndex);

            switch (MyType)
            {
                case MashType.Hold:
                    if (Current.Down)
                        Satisfied = true;

                    break;

                case MashType.Tap:
                    if (Current.Down != Previous.Down)
                        GapCount = 0;
                    else
                        GapCount++;
                    if (GapCount < GapAllowance)
                        Satisfied = true;

                    break;

                case MashType.HoldDir:
                    if (Direction(Current.Dir) == Dir)
                        Satisfied = true;
                    break;
            }
        }
    }
}