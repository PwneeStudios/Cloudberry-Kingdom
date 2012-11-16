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

                    PlayerManager.Get(0).IsAlive = PlayerManager.Get(0).Exists = true;


                    // Now that everything is loaded, start the real game, dependent on the command line arguments.
                    if (StartAsBackgroundEditor)
                    {
#if DEBUG
                        MakeEmptyLevel();
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
                        Tools.CurGameData = CloudberryKingdomGame.TitleGameFactory.Make();
                        return;
                    }

#if DEBUG
                    // Start at Title Screen
                    Tools.CurGameData = CloudberryKingdomGame.TitleGameFactory.Make(); return;
#else
                    // Start at Screen Saver
                    ScreenSaver Intro = new ScreenSaver(); Intro.Init(); return;
#endif
                }
            }
        }
    }
}
