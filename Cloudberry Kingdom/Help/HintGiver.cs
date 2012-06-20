using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public static class Hints
    {
        public static int YForHelp { get { return _YForHelp; } set { _YForHelp = value; PlayerManager.SavePlayerData.Changed = true; } }
        static int _YForHelp = 0;

        public static int QuickSpawn { get { return _QuickSpawn; } set { _QuickSpawn = value; PlayerManager.SavePlayerData.Changed = true; } }
        static int _QuickSpawn = 0;

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

        public void Check_QuickSpawn()
        {
            Level level = Core.MyLevel;

            // "Quickspawn"
            //int FirstHint = 12, SecondHint = 36, ThirdHint = 70;
            int FirstHint = 9, SecondHint = 20, ThirdHint = 40;
            //int FirstHint = 2, SecondHint = 4, ThirdHint = 6;
            if (level.CurPhsxStep == 20 && level.PieceAttempts > FirstHint && Hints.QuickSpawn == 0 ||
                level.CurPhsxStep == 20 && level.PieceAttempts > SecondHint && Hints.QuickSpawn == 1 ||
                level.CurPhsxStep == 20 && level.PieceAttempts > ThirdHint && Hints.QuickSpawn == 2)
            {
                Hints.QuickSpawn++;

                MyGame.WaitThenDo(5, () =>
                {
                    HintBlurb2 hint = new HintBlurb2();
                    hint.SetText(QuickSpawnHint);

                    /*
                    hint.MyPile.BackdropShift = new Vector2(166.6668f, -152.2223f);
                    hint.MyPile.Backdrop.CalcQuads(new Vector2(855, 330));
                */

                    Call(hint);
                });
            }
        }

        public static string QuickSpawnHint
        {
            get
            {
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
#if NOT_PC
                return "Press " + ButtonString.Y(85) + " for powerups!";
#else
                return "Press " + ButtonString.Y(85) + " or " +
                    ButtonString.KeyStr(Microsoft.Xna.Framework.Input.Keys.Enter, 85) + " for powerups!";
#endif
            }
        }

        public void Check_YForHelp()
        {
            Level level = Core.MyLevel;

            // "Press (Y) for help"
            //int FirstHint = 24, SecondHint = 50, ThirdHint = 90;
            //int FirstHint = 18, SecondHint = 50, ThirdHint = 90;
            int FirstHint = 14, SecondHint = 27, ThirdHint = 60;
            if (level.CurPhsxStep == 30 && level.PieceAttempts > FirstHint && Hints.YForHelp == 0 ||
                level.CurPhsxStep == 30 && level.PieceAttempts > SecondHint && Hints.YForHelp == 1 ||
                level.CurPhsxStep == 30 && level.PieceAttempts > ThirdHint && Hints.YForHelp == 2)
            {
                Hints.YForHelp++;

                //MyGame.ToDoOnReset.Add(() =>
                MyGame.WaitThenDo(5, () =>
                {
                    if (Hints.YForHelp > 10) return;

                    HintBlurb2 hint = new HintBlurb2();
                    hint.SetText(PowerupHint);

                    /*
                    hint.MyPile.BackdropShift = new Vector2(169.4445f, -102.2223f);
                    hint.MyPile.Backdrop.CalcQuads(new Vector2(855, 230));
                    */

                    Call(hint);
                });
            }
        }
    }
}