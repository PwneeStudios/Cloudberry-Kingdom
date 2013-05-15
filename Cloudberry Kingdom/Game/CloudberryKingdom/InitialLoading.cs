using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    partial class CloudberryKingdomGame
    {
        bool DoInnerLogoPhsx = true;
        void LogoPhsx()
        {
            LoadingScreen.PhsxStep();

			ButtonCheck.UpdateControllerAndKeyboard_StartOfStep();
			ButtonCheck.UpdateControllerAndKeyboard_EndOfStep(Tools.TheGame.Resolution);

            if (!DoInnerLogoPhsx)
            {
                if (LoadingScreen.IsDone)
                    LogoScreenUp = false;

                return;
            }

            if (!Resources.LoadingResources.MyBool)
            {
                if (LoadingScreen.IsDone || !Resources.LoadingResources.MyBool)
                {
                    DoInnerLogoPhsx = false;
                    if (LoadingScreen.IsDone)
                        LogoScreenUp = false;

                    DrawCount = PhsxCount = 0;


					// Warning, should not exist, but need to test without.
					// PlayerManager.Get(0).IsAlive = PlayerManager.Get(0).Exists = true;


                    // Now that everything is loaded, start the real game, dependent on the command line arguments.
                    if (StartAsBackgroundEditor)
                    {
#if DEBUG
                        MakeEmptyLevel();
#endif
#if INCLUDE_EDITOR
                        Tools.background_viewer = new Viewer.BackgroundViewer();
                        Tools.background_viewer.Show();
#endif
                        return;
                    }
                    else if (StartAsTestLevel)
                    {
#if DEBUG
                        MakeTestLevel();
#endif
                        return;
                    }
                    else if (StartAsFreeplay)
                    {
                        Tools.CurGameData = CloudberryKingdomGame.TitleGameFactory();
                        return;
                    }

#if XBOX
                    EzStorage.StartAsyncUpdate();
#endif

#if DEBUG
                    // Start at Title Screen
                    // WARNING
                    ScreenSaver Intro = new ScreenSaver(); Intro.Init(); return;
                    //Tools.CurGameData = CloudberryKingdomGame.TitleGameFactory(); return;
#else
                    // Start at Screen Saver
                    ScreenSaver Intro = new ScreenSaver(); Intro.Init(); return;
#endif
                }
            }
        }
    }
}
