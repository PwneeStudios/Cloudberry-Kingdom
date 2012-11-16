using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public static class Hints
    {
        public static int YForHelpNum = 0;
        public static void SetYForHelpNum(int val) { YForHelpNum = val; PlayerManager.SavePlayerData.Changed = true; }
        public static void IncrYForHelpNum() { YForHelpNum++; PlayerManager.SavePlayerData.Changed = true; }

        public static int QuickSpawnNum = 0;
        public static void SetQuickSpawnNum(int val) { QuickSpawnNum = val; PlayerManager.SavePlayerData.Changed = true; }
        public static void IncrQuickSpawnNum() { QuickSpawnNum++; PlayerManager.SavePlayerData.Changed = true; }

        public static HintGiver CurrentGiver;
    }

    public class HintGiver : GUI_Panel
    {
        public HintGiver()
        {
            Active = true;
            PauseOnPause = true;

            Hints.CurrentGiver = this;
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            if (Hints.CurrentGiver == this)
                Hints.CurrentGiver = null;
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            Check_YForHelp();
            Check_QuickSpawn();
        }

        class Check_QuickSpawnHelper : Lambda
        {
            HintGiver hg;

            public Check_QuickSpawnHelper(HintGiver hg)
            {
                this.hg = hg;
            }

            public void Apply()
            {
                HintBlurb hint = new HintBlurb();
                hint.SetText(HintGiver.QuickSpawnHint);

                /*
                hint.MyPile.BackdropShift = new Vector2(166.6668f, -152.2223f);
                hint.MyPile.Backdrop.CalcQuads(new Vector2(855, 330));
                */

                hg.Call(hint);
            }
        }

        public void Check_QuickSpawn()
        {
            Level level = Core.MyLevel;

            // "Quickspawn"
            //int FirstHint = 12, SecondHint = 36, ThirdHint = 70;
            int FirstHint = 9, SecondHint = 20, ThirdHint = 40;
            //int FirstHint = 2, SecondHint = 4, ThirdHint = 6;
            if (level.CurPhsxStep == 20 && level.PieceAttempts > FirstHint && Hints.QuickSpawnNum == 0 ||
                level.CurPhsxStep == 20 && level.PieceAttempts > SecondHint && Hints.QuickSpawnNum == 1 ||
                level.CurPhsxStep == 20 && level.PieceAttempts > ThirdHint && Hints.QuickSpawnNum == 2)
            {
                Hints.IncrQuickSpawnNum();

                MyGame.WaitThenDo(5, new Check_QuickSpawnHelper(this));
            }
        }

        public static string QuickSpawnHint
        {
            get
            {
                Tools.Warning();
#if NOT_PC
                return "Hold " + ButtonString.LeftBumper(85) + " and " + ButtonString.RightBumper(85) + " to respawn quickly!";
#else
                return "Press " + ButtonString.KeyStr(ButtonCheck.Quickspawn_KeyboardKey.KeyboardKey, 85) + " or " +
                    ButtonString.KeyStr(Microsoft.Xna.Framework.Input.Keys.Space, 85) + " to respawn quickly!";
#endif
            }
        }

        public static string PowerupHint
        {
            get
            {
                Tools.Warning();
#if NOT_PC
                return "Press " + ButtonString.Y(85) + " for powerups!";
#else
                return "Press " + ButtonString.Y(85) + " or " +
                    ButtonString.KeyStr(Microsoft.Xna.Framework.Input.Keys.Enter, 85) + " for powerups!";
#endif
            }
        }

        class Check_YForHelpHelper : Lambda
        {
            HintGiver hg;

            public Check_YForHelpHelper(HintGiver hg)
            {
                this.hg = hg;
            }

            public void Apply()
            {
                if (Hints.YForHelpNum > 10) return;

                HintBlurb hint = new HintBlurb();
                hint.SetText(HintGiver.PowerupHint);

                /*
                hint.MyPile.BackdropShift = new Vector2(169.4445f, -102.2223f);
                hint.MyPile.Backdrop.CalcQuads(new Vector2(855, 230));
                */

                hg.Call(hint);
            }
        }

        public void Check_YForHelp()
        {
            Level level = Core.MyLevel;

            // "Press (Y) for help"
            //int FirstHint = 24, SecondHint = 50, ThirdHint = 90;
            //int FirstHint = 18, SecondHint = 50, ThirdHint = 90;
            int FirstHint = 14, SecondHint = 27, ThirdHint = 60;
            if (level.CurPhsxStep == 30 && level.PieceAttempts > FirstHint && Hints.YForHelpNum == 0 ||
                level.CurPhsxStep == 30 && level.PieceAttempts > SecondHint && Hints.YForHelpNum == 1 ||
                level.CurPhsxStep == 30 && level.PieceAttempts > ThirdHint && Hints.YForHelpNum == 2)
            {
                Hints.IncrYForHelpNum();

                MyGame.WaitThenDo(5, new Check_YForHelpHelper(this));
            }
        }
    }
}