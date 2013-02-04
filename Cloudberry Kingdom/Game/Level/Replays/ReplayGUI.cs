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
            MyPile.Add(new QuadClass(ButtonTexture.Go, 140, "Button_Go"));
            Play = new EzText(Localization.Words.Play, ItemFont, true);
            Play.Name = "Play";
            SetGrayHeaderProperties(Play);
#else
            MyPile.Add(new QuadClass(ButtonTexture.Go, 90, "Button_Go"));
            Play = new EzText(Localization.Words.Play, ItemFont, true);
            Play.MyFloatColor = new Color(67, 198, 48, 255).ToVector4();
            Play.Name = "Play";
            SetHeaderProperties(Play);
#endif

#if PC_VERSION
            AdditionalAdd = new Vector2(-2, 0);
            MyPile.Add(new QuadClass(ButtonTexture.Back, 140, "Button_Back"));
            End = new EzText(Localization.Words.Done, ItemFont, true);
            End.Name = "Back";
            SetGrayHeaderProperties(End);
#else
            MyPile.Add(new QuadClass(ButtonTexture.Back, 85, "Button_Back"));
            End = new EzText(Localization.Words.Done, ItemFont, true);
            End.MyFloatColor = new Color(239, 41, 41, 255).ToVector4();
            End.Name = "Back";
            SetHeaderProperties(End);
#endif

            if (Type == ReplayGUIType.Replay)
            {
                MyPile.Add(new QuadClass(ButtonTexture.X, 90, "Button_X"));
                Toggle = new EzText(Localization.Words.Single, ItemFont, true);
                Toggle.Name = "Toggle";
#if PC_VERSION
                SetGrayHeaderProperties(Toggle);
#else
                SetHeaderProperties(Toggle);
                Toggle.MyFloatColor = new Color(0, 0, 255, 255).ToVector4();
#endif
                SetToggleText();
            }

            MyPile.Add(new QuadClass(ButtonTexture.LeftRight, 85, "Button_LR"));
            Speed = new EzText(Localization.Words.Speed, ItemFont);
            Speed.Name = "Speed";
            SetGrayHeaderProperties(Speed);

            if (Type == ReplayGUIType.Computer)
            {
                MyPile.Add(new QuadClass(ButtonTexture.LeftBumper, 85, "Button_LB"));
                LB = new EzText(Localization.Words.Reset, ItemFont, true);
                LB.Name = "Reset";
                SetGrayHeaderProperties(LB);
            }
            else
            {
                MyPile.Add(new QuadClass(ButtonTexture.LeftBumper, 85, "Button_LB"));
                LB = new EzText(Localization.Words.Previous, ItemFont, true);
                LB.Name = "Prev";
                SetGrayHeaderProperties(LB);

                MyPile.Add(new QuadClass(ButtonTexture.RightBumper, 85, "Button_RB"));
                RB = new EzText(Localization.Words.Next, ItemFont, true);
                RB.Name = "Next";
                SetGrayHeaderProperties(RB);
            }
            SetSpeed();

            BigPaused = new QuadClass();
            BigPaused.SetToDefault();
            BigPaused.Quad.MyTexture = Tools.TextureWad.FindByName("Replay_GUI\\Paused");
            BigPaused.ScaleYToMatchRatio(355);
            MyPile.Add(BigPaused);
            BigPaused.Pos = new Vector2(1210.557f, 791.1111f);

            BigEnd = new QuadClass();
            BigEnd.SetToDefault();
            BigEnd.Quad.MyTexture = Tools.TextureWad.FindByName("Replay_GUI\\End");
            BigEnd.ScaleYToMatchRatio(255);
            BigPaused.ScaleYToMatchRatio(300);
            MyPile.Add(BigEnd);
            BigEnd.Pos = new Vector2(1277.222f, 774.4444f);

            SetPlayText();








            if (Type == ReplayGUIType.Computer)
            {
if (ButtonCheck.ControllerInUse)
{
#if XBOX || PC_VERSION
                EzText _t;
                _t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-721.1783f, -832.2222f); _t.Scale = 0.44f; }
                _t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-120.0003f, -832.2223f); _t.Scale = 0.44f; }
                _t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(264.0002f, -832.2222f); _t.Scale = 0.44f; }
                _t = MyPile.FindEzText("Reset"); if (_t != null) { _t.Pos = new Vector2(954.3328f, -846.1111f); _t.Scale = 0.3934999f; }

                QuadClass _q;
                _q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(55.55542f, -2058.333f); _q.Size = new Vector2(1230.664f, 1230.664f); }
                _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(52.77765f, -2058.333f); _q.Size = new Vector2(1219.997f, 1219.997f); }
                _q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-930.5562f, -911.1113f); _q.Size = new Vector2(69.64276f, 69.64276f); }
                _q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-347.2227f, -911.1112f); _q.Size = new Vector2(71.76664f, 71.76664f); }
                _q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(186.1109f, -911.1112f); _q.Size = new Vector2(77.89992f, 77.89992f); }
                _q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(655.555f, -908.3333f); _q.Size = new Vector2(152.0833f, 152.0833f); }

                MyPile.Pos = new Vector2(0f, 0f);
#else
                EzText _t;
                _t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-721.1783f, -832.2222f); _t.Scale = 0.44f; }
                _t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-120.0003f, -832.2223f); _t.Scale = 0.44f; }
                _t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(264.0002f, -832.2222f); _t.Scale = 0.44f; }
                _t = MyPile.FindEzText("Reset"); if (_t != null) { _t.Pos = new Vector2(1001.555f, -843.3333f); _t.Scale = 0.3934999f; }

                QuadClass _q;
                _q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(55.55542f, -2058.333f); _q.Size = new Vector2(1230.664f, 1230.664f); }
                _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(52.77765f, -2058.333f); _q.Size = new Vector2(1219.997f, 1219.997f); }
                _q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-930.5562f, -911.1113f); _q.Size = new Vector2(69.64276f, 69.64276f); }
                _q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-347.2227f, -911.1112f); _q.Size = new Vector2(71.76664f, 71.76664f); }
                _q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(186.1109f, -911.1112f); _q.Size = new Vector2(77.89992f, 77.89992f); }
                _q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(613.8885f, -916.6666f); _q.Size = new Vector2(91.65893f, 61.58334f); }

                MyPile.Pos = new Vector2(0f, 0f);
#endif
}
else
{
                EzText _t;
                _t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-662.845f, -832.2222f); _t.Scale = 0.44f; }
                _t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-103.3335f, -835.0001f); _t.Scale = 0.44f; }
                _t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(344.5559f, -832.2222f); _t.Scale = 0.44f; }
                _t = MyPile.FindEzText("Reset"); if (_t != null) { _t.Pos = new Vector2(1051.556f, -840.5555f); _t.Scale = 0.44f; }

                QuadClass _q;
                _q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(55.55542f, -2058.333f); _q.Size = new Vector2(1230.664f, 1230.664f); }
                _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(52.77765f, -2058.333f); _q.Size = new Vector2(1219.997f, 1219.997f); }
                _q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-930.5562f, -911.1113f); _q.Size = new Vector2(130.9643f, 62.80939f); }
                _q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-325.0003f, -911.1112f); _q.Size = new Vector2(73.5106f, 69.09996f); }
                _q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(244.4444f, -913.8889f); _q.Size = new Vector2(73.20911f, 68.81656f); }
                _q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(783.3331f, -913.8888f); _q.Size = new Vector2(76.22305f, 71.64967f); }

                MyPile.Pos = new Vector2(0f, 0f);
}
            }
            else
            {
if (ButtonCheck.ControllerInUse)
{
				EzText _t;
				_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-991.6671f, -827.778f); _t.Scale = 0.4145834f; }
				_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-472.2223f, -838.8888f); _t.Scale = 0.4147499f; }
				_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(11.11071f, -836.1111f); _t.Scale = 0.4139166f; }
				_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(436.1108f, -836.1108f); _t.Scale = 0.3873335f; }
				_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(941.667f, -830.5555f); _t.Scale = 0.4208333f; }
				_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1361.11f, -833.3332f); _t.Scale = 0.4238334f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(108.3328f, -2327.78f); _q.Size = new Vector2(1517.832f, 1517.832f); }
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(108.3335f, -2330.556f); _q.Size = new Vector2(1500f, 1500f); }
				_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-1200f, -905.5555f); _q.Size = new Vector2(64.55943f, 64.55943f); }
				_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-688.8887f, -911.111f); _q.Size = new Vector2(67.34993f, 67.34993f); }
				_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(-213.8887f, -908.3332f); _q.Size = new Vector2(65.58324f, 65.58324f); }
				_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(352.7776f, -905.5555f); _q.Size = new Vector2(69.04251f, 64.89996f); }
				_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(727.7779f, -902.7776f); _q.Size = new Vector2(103.1499f, 103.1499f); }
				_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1155.556f, -902.7778f); _q.Size = new Vector2(107.5665f, 107.5665f); }
				_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1210.557f, 791.1111f); _q.Size = new Vector2(300f, 105f); }
				_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

				MyPile.Pos = new Vector2(0f, 0f);
}
else
{
                EzText _t;
                _t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-991.6671f, -827.778f); _t.Scale = 0.4145834f; }
                _t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-472.2223f, -838.8888f); _t.Scale = 0.4147499f; }
                _t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(11.11071f, -836.1111f); _t.Scale = 0.4139166f; }
                _t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(436.1108f, -836.1108f); _t.Scale = 0.3873335f; }
                _t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(941.667f, -830.5555f); _t.Scale = 0.4208333f; }
                _t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1361.11f, -833.3332f); _t.Scale = 0.4238334f; }

                QuadClass _q;
                _q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(108.3328f, -2327.78f); _q.Size = new Vector2(1517.832f, 1517.832f); }
                _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(108.3335f, -2330.556f); _q.Size = new Vector2(1500f, 1500f); }
                _q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-1244.444f, -908.3333f); _q.Size = new Vector2(116.7162f, 55.97613f); }
                _q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-688.8887f, -911.111f); _q.Size = new Vector2(66.06374f, 62.09991f); }
                _q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(-213.8887f, -908.3332f); _q.Size = new Vector2(64.18431f, 60.33325f); }
                _q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(352.7776f, -905.5555f); _q.Size = new Vector2(69.04251f, 64.89996f); }
                _q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(727.7779f, -902.7776f); _q.Size = new Vector2(63.90063f, 60.0666f); }
                _q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1155.556f, -902.7778f); _q.Size = new Vector2(64.96447f, 61.0666f); }

                MyPile.Pos = new Vector2(0f, 0f);
}
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
                Toggle.SubstituteText(Localization.Words.All);
            else
                Toggle.SubstituteText(Localization.Words.Single);
        }

        void SetPlayText()
        {
            if (StepControl)
                Play.SubstituteText(Localization.Words.Step);
            else
            {
                if (PauseSelected)
                    Play.SubstituteText(Localization.Words.Play);
                else
                    Play.SubstituteText(Localization.Words.Pause);
            }
        }

        void SetSpeed()
        {
            switch (SpeedVal)
            {
                case 0:
                    StepControl = true; // Start step control
                    Tools.PhsxSpeed = 1; Speed.SubstituteText("x 0");
                    break;
                case 1: Tools.PhsxSpeed = 0; Speed.SubstituteText("x .5"); break;
                case 2: Tools.PhsxSpeed = 1; Speed.SubstituteText("x 1"); break;
                case 3: Tools.PhsxSpeed = 2; Speed.SubstituteText("x 2"); break;
                case 4: Tools.PhsxSpeed = 3; Speed.SubstituteText("x 4"); break;
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

            float Dir = ButtonCheck.GetDir(-1).X;// ButtonCheck.State(ControllerButtons.LJ, -1).Dir.X;
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
            if (Tools.Keyboard.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Escape))
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