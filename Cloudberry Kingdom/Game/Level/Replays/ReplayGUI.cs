using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public enum ReplayGUIType { Replay, Computer };
    public class ReplayGUI : CkBaseMenu
    {
        public override void ReturnToCaller()
        {
            InGameStartMenu.PreventMenu = false;
            base.ReturnToCaller();
        }

        bool SkipPhsxStep;

        EzText Play, Toggle, End, Speed, LB, RB;

        QuadClass BigPaused, BigEnd;

        public ReplayGUIType Type;

        public ReplayGUI()
        {
        }

        protected void SetGrayHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.Shadow = false;

            text.Scale = FontScale;
            MyPile.Add(text);
        }

        protected override void SetHeaderProperties(EzText text)
        {
            text.Scale = FontScale;
            MyPile.Add(text);
        }

        public override void Init()
        {
            base.Init();

            SlideInFrom = SlideOutTo = PresetPos.Bottom;
            //SlideOutLength = 0;
        }

        public override void OnAdd()
        {
            base.OnAdd();

            InGameStartMenu.PreventMenu = true;

            FontScale = .5f;

            MyPile = new DrawPile();

            // Backrop
            QuadClass backdrop2 = new QuadClass("White", 1500);
            backdrop2.Quad.SetColor(ColorHelper.GrayColor(.1f));
            backdrop2.Alpha = .45f;
            MyPile.Add(backdrop2, "Backdrop2");

            QuadClass backdrop = new QuadClass("White", 1500);
            backdrop.Quad.SetColor(ColorHelper.GrayColor(.25f));
            backdrop.Alpha = .35f;
            MyPile.Add(backdrop, "Backdrop");

            Vector2 AdditionalAdd = Vector2.Zero;
#if PC_VERSION
            AdditionalAdd = new Vector2(-2, 0);
            Play = new EzText(ButtonString.Enter(140) + " Play", ItemFont, true);
            Play.Name = "Play";
            SetGrayHeaderProperties(Play);
#else
            Play = new EzText(ButtonString.Go(90) + " Play", ItemFont, true);
            Play.MyFloatColor = new Color(67, 198, 48, 255).ToVector4();
            Play.Name = "Play";
            SetHeaderProperties(Play);
#endif

#if PC_VERSION
            AdditionalAdd = new Vector2(-2, 0);
            End = new EzText(ButtonString.Backspace(140) + " Done", ItemFont, true);
            End.Name = "Back";
            SetGrayHeaderProperties(End);
#else
            End = new EzText(ButtonString.Back(85) + " Done", ItemFont, true);
            End.MyFloatColor = new Color(239, 41, 41, 255).ToVector4();
            End.Name = "Back";
            SetHeaderProperties(End);
#endif

            if (Type == ReplayGUIType.Replay)
            {
                Toggle = new EzText(ButtonString.X(90) + " Solo", ItemFont, true);
                Toggle.Name = "Toggle";
#if PC_VERSION
                SetGrayHeaderProperties(Toggle);
#else
                SetHeaderProperties(Toggle);
                Toggle.MyFloatColor = new Color(0, 0, 255, 255).ToVector4();
#endif
                SetToggleText();
            }

            Speed = new EzText(ButtonString.LeftRight(85) + " Speed x 1", ItemFont, true);
            Speed.Name = "Speed";
            SetGrayHeaderProperties(Speed);

            if (Type == ReplayGUIType.Computer)
            {
                LB = new EzText(ButtonString.LeftBumper(85) + " Reset", ItemFont, true);
                LB.Name = "Reset";
                SetGrayHeaderProperties(LB);
            }
            else
            {
                LB = new EzText(ButtonString.LeftBumper(85) + " Prev", ItemFont, true);
                LB.Name = "Prev";
                SetGrayHeaderProperties(LB);

                RB = new EzText(ButtonString.RightBumper(85) + " Next", ItemFont, true);
                RB.Name = "Next";
                SetGrayHeaderProperties(RB);
            }

            BigPaused = new QuadClass();
            BigPaused.SetToDefault();
            BigPaused.Quad.MyTexture = Tools.TextureWad.FindByName("Replay_GUI\\Paused");
            BigPaused.ScaleYToMatchRatio(355);
            MyPile.Add(BigPaused);
            BigPaused.Pos = new Vector2(1210.557f, 791.1111f);

            BigEnd = new QuadClass();
            BigEnd.SetToDefault();
            BigEnd.Quad.MyTexture = Tools.TextureWad.FindByName("Replay_GUI\\End");
            BigEnd.ScaleToTextureSize();
            BigPaused.ScaleYToMatchRatio(300);
            MyPile.Add(BigEnd);
            BigEnd.Pos = new Vector2(1277.222f, 774.4444f);

            SetPlayText();








            if (Type == ReplayGUIType.Computer)
            {
                EzText _t;
                _t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-773.9557f, -832.2222f); _t.Scale = 0.44f; }
                _t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-192.2225f, -829.4445f); _t.Scale = 0.44f; }
                _t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(366.7781f, -835f); _t.Scale = 0.44f; }
                _t = MyPile.FindEzText("Reset"); if (_t != null) { _t.Pos = new Vector2(934.8892f, -835f); _t.Scale = 0.44f; }

                QuadClass _q;
                _q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(55.55542f, -2058.333f); _q.Size = new Vector2(1230.664f, 1230.664f); }
                _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(52.77765f, -2058.333f); _q.Size = new Vector2(1219.997f, 1219.997f); }

                MyPile.Pos = new Vector2(0f, 0f);
            }
            else
            {
                EzText _t;
                _t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-1083.334f, -827.778f); _t.Scale = 0.4145834f; }
                _t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-549.9999f, -830.5555f); _t.Scale = 0.4147499f; }
                _t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(-138.8893f, -830.5555f); _t.Scale = 0.4139166f; }
                _t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(419.4443f, -836.1107f); _t.Scale = 0.3873335f; }
                _t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(933.3335f, -827.7777f); _t.Scale = 0.4208333f; }
                _t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1341.666f, -824.9999f); _t.Scale = 0.4238334f; }

                QuadClass _q;
                _q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(108.3328f, -2327.78f); _q.Size = new Vector2(1517.832f, 1517.832f); }
                _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(108.3335f, -2330.556f); _q.Size = new Vector2(1500f, 1500f); }

                MyPile.Pos = new Vector2(0f, 0f);
            }
        }

        public void StartUp()
        {
            SkipPhsxStep = true;
        }

        void ResetReplay(Level level)
        {
            if (Type == ReplayGUIType.Replay)
                level.SetReplay();

            level.SetToReset = true;
            level.ReplayPaused = false;
            level.FreeReset = true;
        }

        void SetToggleText()
        {
            if (MyGame.MyLevel.SingleOnly)
                Toggle.SubstituteText(" All");
            else
                Toggle.SubstituteText(" Single");
        }

        void SetPlayText()
        {
            if (StepControl)
                Play.SubstituteText(" Step");
            else
            {
                if (PauseSelected)
                    Play.SubstituteText(" Play");
                else
                    Play.SubstituteText(" Pause");
            }
        }

        void SetSpeed()
        {
            switch (SpeedVal)
            {
                case 0:
                    StepControl = true; // Start step control
                    Tools.PhsxSpeed = 1; Speed.SubstituteText(" Speed x 0");
                    break;
                case 1: Tools.PhsxSpeed = 0; Speed.SubstituteText(" Speed x .5"); break;
                case 2: Tools.PhsxSpeed = 1; Speed.SubstituteText(" Speed x 1"); break;
                case 3: Tools.PhsxSpeed = 2; Speed.SubstituteText(" Speed x 2"); break;
                case 4: Tools.PhsxSpeed = 3; Speed.SubstituteText(" Speed x 4"); break;
            }

            // Ensure the game is unpaused if we aren't step controlling and we aren't soliciting a pause
            // We set this to false elsewhere in this case, but not soon enough for the game to realize
            // we aren't paused and set the PhsxSpeed to 1
            if (!PauseSelected && !StepControl) PauseGame = false;
        }

        bool StepControl = false;

        int SpeedVal = 2;
        int Delay = 10;
        int PrevDir = 0;
        public void ProcessInput()
        {
            Level level = MyGame.MyLevel;

            if (SkipPhsxStep) { SkipPhsxStep = false; return; }

            if (ButtonCheck.State(ControllerButtons.A, -1).Pressed)
            {
                if (level.ReplayPaused && ReplayIsOver())
                {
                    PauseSelected = false;
                    SetPlayText();

                    level.ReplayPaused = false;

                    // Reset
                    ResetReplay(level);
                    PauseGame = false;
                }
            }

            if (Type == ReplayGUIType.Computer)
            {
                // Check for reset
                if (ButtonCheck.State(ControllerButtons.LS, -1).Pressed)
                {
                    ResetReplay(level);
                    PauseGame = false;
                }
            }
            else
            {
                // Check for switching
                int SwarmIndex = level.MySwarmBundle.SwarmIndex;
                if (ButtonCheck.State(ControllerButtons.LS, -1).Pressed)
                {
                    if (StepControl && level.CurPhsxStep < 36 ||
                        Tools.DrawCount - TimeStamp < 36)
                        level.MySwarmBundle.SetSwarm(level, SwarmIndex - 1);
                    ResetReplay(level);
                    PauseGame = false;
                }
                if (SwarmIndex < level.MySwarmBundle.NumSwarms - 1 && ButtonCheck.State(ControllerButtons.RS, -1).Pressed)
                {
                    level.MySwarmBundle.SetSwarm(level, SwarmIndex + 1);
                    ResetReplay(level);
                    PauseGame = false;
                }
            }

            float Dir = ButtonCheck.State(ControllerButtons.LJ, -1).Dir.X;
            if (PrevDir != Math.Sign(Dir)) Delay = 0;
            if (Delay == 0)
            {
                bool Change = false;
                if (Dir < -ButtonCheck.ThresholdSensitivity)
                {
                    SpeedVal--;
                    Change = true;
                }
                else if (Dir > ButtonCheck.ThresholdSensitivity)
                {
                    SpeedVal++;
                    Change = true;
                }

                if (Change)
                {
                    SpeedVal = CoreMath.Restrict(0, 4, SpeedVal);

                    //if (SpeedVal == 1) Delay = 30;
                    //else if (SpeedVal == 2) Delay = 30;
                    //else Delay = 15;
                    Delay = 1000;

                    StepControl = false;

                    SetSpeed();

                    SetPlayText();

                    //if (!StepControl) PauseGame = false;
                }
            }
            else
            {
                Delay--;
            }
            PrevDir = Math.Sign(Dir);

            // Switch between swarm and single view (Toggle)
            if (Type == ReplayGUIType.Replay)
            {
                if (ButtonCheck.State(ControllerButtons.X, -1).Pressed)
                {
                    level.SingleOnly = !level.SingleOnly;
                    level.MainReplayOnly = level.SingleOnly;
                    SetToggleText();
                }
            }

            // End the replay
            bool EndReplay = false;
#if PC_VERSION
            if (Tools.keybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Escape))
                EndReplay = true;
#endif

            if (ButtonCheck.State(ControllerButtons.B, -1).Pressed)
                EndReplay = true;

            if (EndReplay)
            {
                PauseGame = false;
                Tools.PhsxSpeed = 1;

                ReturnToCaller();

                level.FreeReset = true;
                if (Type == ReplayGUIType.Replay)
                {                    
                    level.EndReplay();
                }
                else
                    level.EndComputerWatch();
            }
        }

        public override void Reset(bool BoxesOnly)
        {
            base.Reset(BoxesOnly);

            TimeStamp = Tools.DrawCount;
        }

        bool PauseSelected = false;

        int TimeStamp = 0;
        protected override void MyPhsxStep()
        {
            Level level = MyGame.MyLevel;

            if (level.SuppressReplayButtons)
                return;

            base.MyPhsxStep();

            if (!Active)
                return;

            InGameStartMenu.PreventMenu = true;

            if (StepControl && !PauseSelected)
            {
                if (ButtonCheck.State(ControllerButtons.A, -1).Pressed ||
                    ButtonStats.All.DownCount(ControllerButtons.A) > 30 && Tools.DrawCount % 2 == 0)
                    PauseGame = false;
                else
                    PauseGame = true;
            }
            else
            {
                if (ButtonCheck.State(ControllerButtons.A, -1).Pressed)
                {
                    PauseSelected = !PauseSelected;
                    SetPlayText();

                    SetSpeed();
                }

                PauseGame = PauseSelected;
            }

            ProcessInput();
        }

        protected override void MyDraw()
        {
            if (MyGame == null) { Release(); return; }

            Level level = MyGame.MyLevel;

            if (level.SuppressReplayButtons)
                return;

            base.MyDraw();
           
            BigEnd.Show = BigPaused.Show = false;
            if (level.ReplayPaused)
            {
                if (ReplayIsOver())
                    BigEnd.Show = true;
            }
            else
            {
                if (PauseSelected)
                    BigPaused.Show = true;
            }
        }

        bool ReplayIsOver()
        {
            Level level = MyGame.MyLevel;

            if (Type == ReplayGUIType.Computer && level.EndOfReplay())
                return true;

            if (Type == ReplayGUIType.Replay)
                if (level.MySwarmBundle.EndCheck(level) && !level.MySwarmBundle.GetNextSwarm(level))
                    return true;

            return false;
        }
    }
}