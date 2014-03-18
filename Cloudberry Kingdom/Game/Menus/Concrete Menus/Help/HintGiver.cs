using Microsoft.Xna.Framework;
using input = Microsoft.Xna.Framework.Input;

using CloudberryKingdom.Levels;

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

                MyGame.WaitThenDo(5, () =>
                {
                    HintBlurb hint = new HintBlurb();
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
#if PC_VERSION
				if (ButtonCheck.ControllerInUse)
				{
					return string.Format(Localization.WordString(Localization.Words.RespawnNoteGamepad), ButtonString.LeftBumper(150), ButtonString.RightBumper(150));
				}
				else
				{
					return string.Format(Localization.WordString(Localization.Words.RespawnNoteKeyboard), ButtonString.KeyStr(input.Keys.Space, 85));
				}
#else
                return string.Format(Localization.WordString(Localization.Words.RespawnNoteGamepad), ButtonString.LeftBumper(150), ButtonString.RightBumper(150));
#endif
            }
        }

        public static string PowerupHint
        {
            get
            {
#if PC_VERSION
				if (ButtonCheck.ControllerInUse)
				{
					return string.Format(Localization.WordString(Localization.Words.PowerupNote), ButtonString.Y(85));
				}
				else
				{
					return string.Format(Localization.WordString(Localization.Words.PowerupNote), ButtonString.KeyStr(input.Keys.Enter, 85));
				}
#else
                return string.Format(Localization.WordString(Localization.Words.PowerupNote), ButtonString.Y(85));
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
            if (level.CurPhsxStep == 30 && level.PieceAttempts > FirstHint && Hints.YForHelpNum == 0 ||
                level.CurPhsxStep == 30 && level.PieceAttempts > SecondHint && Hints.YForHelpNum == 1 ||
                level.CurPhsxStep == 30 && level.PieceAttempts > ThirdHint && Hints.YForHelpNum == 2)
            {
                Hints.IncrYForHelpNum();

                //MyGame.ToDoOnReset.Add(() =>
                MyGame.WaitThenDo(5, () =>
                {
                    if (Hints.YForHelpNum > 10) return;

                    HintBlurb hint = new HintBlurb();
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