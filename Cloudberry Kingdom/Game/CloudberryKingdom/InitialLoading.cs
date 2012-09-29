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
            if (!DoInnerLogoPhsx)
            {
                if (LoadingScreen.IsDone || SimpleLoad)
                    LogoScreenUp = false;

                return;
            }

            if (!LoadingResources.MyBool)
            {
                Tools.Write("+++++++++++++++++++ Resources all loaded!");

                //if (false)
                if (LoadingScreen.IsDone || SimpleLoad || !LoadingResources.MyBool)
                {
                    Tools.Write("+++++++++++++++++++ Resources all loaded!");

                    DoInnerLogoPhsx = false;
                    if (LoadingScreen.IsDone || SimpleLoad)
                        LogoScreenUp = false;

                    DrawCount = PhsxCount = 0;

                    PlayerManager.Get(0).IsAlive = PlayerManager.Get(0).Exists = true;


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

#if DEBUG
                    if (SimpleLoad)
                    {
                        Tools.CurGameData = CloudberryKingdomGame.TitleGameFactory(); return;
                    }
                    else
                    {
                        ScreenSaver Intro = new ScreenSaver(); Intro.Init(); return;
                    }
#else
                    // Full Game
                    ScreenSaver Intro = new ScreenSaver(); Intro.Init(); return;
#endif
                }
            }
        }
    }
}
