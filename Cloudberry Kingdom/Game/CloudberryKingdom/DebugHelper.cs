﻿#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace CloudberryKingdom
{
    partial class CloudberryKingdomGame
    {
        bool ShowFPS = false;

#if DEBUG
        /// <summary>
        /// Extra functions that allow a user to better debug/test/
        /// </summary>
        /// <returns>Return true if the calling method should return.</returns>
        private bool DebugModePhsx()
        {
#if WINDOWS
            if (!KeyboardExtension.Freeze)
            {
                // Test title screen
                if (Tools.Keyboard.IsKeyDown(Keys.G) && !Tools.PrevKeyboard.IsKeyDown(Keys.G))
                {
                    //TitleGameFactory = TitleGameData_Intense.Factory;
                    TitleGameFactory = TitleGameData_MW.Factory;
                    //TitleGameFactory = TitleGameData_Forest.Factory;

                    Tools.SongWad.Stop();
                    Tools.CurGameData = CloudberryKingdomGame.TitleGameFactory.Make();
                    return true;
                }

                if (Tools.Keyboard.IsKeyDown(Keys.J) && !Tools.PrevKeyboard.IsKeyDown(Keys.J))
                {
                    Tools.CurGameData.FadeToBlack();
                }
            }

            //// Give award
            //if (Tools.keybState.IsKeyDown(Keys.S) && !Tools.PrevKeyboardState.IsKeyDown(Keys.S))
            //{
            //    Awardments.GiveAward(Awardments.UnlockHeroRush2);
            //}

            if (!KeyboardExtension.Freeze && Tools.Keyboard.IsKeyDownCustom(Keys.F) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.F))
                ShowFPS = !ShowFPS;
#endif

#if PC_DEBUG
            if (Tools.FreeCam)
            {
                Vector2 pos = Tools.CurLevel.MainCamera.Data.Position;
                if (Tools.Keyboard.IsKeyDownCustom(Keys.Right)) pos.X += 130;
                if (Tools.Keyboard.IsKeyDownCustom(Keys.Left)) pos.X -= 130;
                if (Tools.Keyboard.IsKeyDownCustom(Keys.Up)) pos.Y += 130;
                if (Tools.Keyboard.IsKeyDownCustom(Keys.Down)) pos.Y -= 130;
                Tools.CurLevel.MainCamera.EffectivePos += pos - Tools.CurLevel.MainCamera.Data.Position;
                Tools.CurLevel.MainCamera.Data.Position = Tools.CurLevel.MainCamera.Target = pos;
                Tools.CurLevel.MainCamera.Update();
            }
#endif

            // Reload some dynamic data (tileset info, animation specifications).
            if (Tools.Keyboard.IsKeyDownCustom(Keys.X) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.X))
            {
#if INCLUDE_EDITOR
                if (LoadDynamic)
                {
                    ////Tools.TextureWad.LoadAllDynamic(Content, EzTextureWad.WhatToLoad.Art);
                    ////Tools.TextureWad.LoadAllDynamic(Content, EzTextureWad.WhatToLoad.Backgrounds);
                    //Tools.TextureWad.LoadAllDynamic(Content, EzTextureWad.WhatToLoad.Tilesets);
                    //Tools.TextureWad.LoadAllDynamic(Content, EzTextureWad.WhatToLoad.Animations);
                    TileSets.LoadSpriteEffects();
                    TileSets.LoadCode();
                }
#endif

                // Make blocks in the current level reset their art to reflect possible changes in the reloaded tileset info.
                foreach (BlockBase block in Tools.CurLevel.Blocks)
                {
                    NormalBlock nblock = block as NormalBlock;
                    if (null != nblock) nblock.ResetPieces();

                    MovingBlock mblock = block as MovingBlock;
                    if (null != mblock) mblock.ResetPieces();
                }
            }

#if DEBUG
            // Reload ALL dynamic data (tileset info, animation specifications, dynamic art, backgrounds).
            if (Tools.Keyboard.IsKeyDownCustom(Keys.Z) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.Z))
            {
                foreach (var hero in Bob.HeroTypes)
                    hero.ResetInfo();

                //Tools.TextureWad.LoadAllDynamic(Tools.GameClass.Content, EzTextureWad.WhatToLoad.Animations);

                //LoadInfo();

                //// Reset blocks
                //foreach (BlockBase block in Tools.CurLevel.Blocks)
                //{
                //    NormalBlock nblock = block as NormalBlock;
                //    if (null != nblock) nblock.ResetPieces();

                //    MovingBlock mblock = block as MovingBlock;
                //    if (null != mblock) mblock.ResetPieces();
                //}
            }
#endif

            // Turn on a simple green screen background.
            if (Tools.Keyboard.IsKeyDownCustom(Keys.D9) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.D9))
                Background.GreenScreen = !Background.GreenScreen;

            Tools.ModNums();

            // Load a test level.
            if (Tools.Keyboard.IsKeyDownCustom(Keys.D5) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.D5))
            {
                GameData.LockLevelStart = false;
                LevelSeedData.ForcedReturnEarly = 0;
                MakeTestLevel(); return true;
            }

            // Hide the GUI. Used for video capture.
            if (ButtonCheck.State(Keys.D8).Pressed) HideGui = !HideGui;

            // Hide the foreground. Used for video capture of backgrounds.
            if (ButtonCheck.State(Keys.D7).Pressed) HideForeground = !HideForeground;

            // Turn on/off immortality.
            if (Tools.Keyboard.IsKeyDownCustom(Keys.O) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.O))
            {
                foreach (Bob bob in Tools.CurLevel.Bobs)
                {
                    bob.Immortal = !bob.Immortal;
                }
            }

            // Turn on/off graphics.
            if (Tools.Keyboard.IsKeyDownCustom(Keys.Q) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.Q))
                Tools.DrawGraphics = !Tools.DrawGraphics;
            // Turn on/off drawing of collision detection boxes.
            if (Tools.Keyboard.IsKeyDownCustom(Keys.W) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.W))
                Tools.DrawBoxes = !Tools.DrawBoxes;
            // Turn on/off step control. When activated, this allows you to step forward in the game by pressing <Enter>.
            if (Tools.Keyboard.IsKeyDownCustom(Keys.E) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.E))
                Tools.StepControl = !Tools.StepControl;
            // Modify the speed of the game.
            if (Tools.Keyboard.IsKeyDownCustom(Keys.R) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.R))
            {
                Tools.IncrPhsxSpeed();
            }

            // Don't do any of the following if a control box is up.
            if (!KeyboardExtension.Freeze)
            {
                // Watch the computer make a level during Stage 1 of construction.
                if (Tools.Keyboard.IsKeyDownCustom(Keys.D3) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.D3))
                {
                    GameData.LockLevelStart = false;
                    LevelSeedData.ForcedReturnEarly = 1;
                    MakeTestLevel(); return true;
                }

                // Watch the computer make a level during Stage 2 of construction.
                if (Tools.Keyboard.IsKeyDownCustom(Keys.D4) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.D4))
                {
                    GameData.LockLevelStart = false;
                    LevelSeedData.ForcedReturnEarly = 2;
                    MakeTestLevel(); return true;
                }

                // Zoom in and out.
                if (Tools.Keyboard.IsKeyDownCustom(Keys.OemComma))
                {
                    Tools.CurLevel.MainCamera.Zoom *= .99f;
                    Tools.CurLevel.MainCamera.EffectiveZoom *= .99f;
                }
                if (Tools.Keyboard.IsKeyDownCustom(Keys.OemPeriod))
                {
                    Tools.CurLevel.MainCamera.Zoom /= .99f;
                    Tools.CurLevel.MainCamera.EffectiveZoom /= .99f;
                }

                // Turn on/off FreeCam, which allows the user to pan the camera through the level freely.
                if (Tools.Keyboard.IsKeyDownCustom(Keys.P) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.P))
                    Tools.FreeCam = !Tools.FreeCam;
            }

            // Allow Back to exit the game if we are in test mode
#if DEBUG
            if (ButtonCheck.State(ControllerButtons.Back, -1).Down)
            {
                Exit();
                return true;
            }
#endif

            return false;
        }
#endif

        public static string debugstring = "";
        StringBuilder MainString = new StringBuilder(100, 100);

        /// <summary>
        /// Method for drawing various debug information to the screen.
        /// </summary>
        void DrawDebugInfo()
        {
            Tools.StartSpriteBatch();

            if (Tools.ShowNums)
            {
                string nums = Tools.Num_0_to_2 + "\n\n" + Tools.Num_0_to_360;

                Tools.StartSpriteBatch();
                Tools.Render.MySpriteBatch.DrawString(Resources.LilFont.Font,
                        nums,
                        new Vector2(0, 100),
                        Color.Orange, 0, Vector2.Zero, .4f, SpriteEffects.None, 0);
                Tools.Render.EndSpriteBatch();
                return;
            }

#if WINDOWS
            // Grace period for falling
            //string str = "";
            //if (Tools.CurLevel != null && Tools.CurLevel.Bobs.Count > 0)
            //{
            //    //var phsx = Tools.CurLevel.Bobs[0].MyPhsx as BobPhsxNormal;
            //    //if (null != phsx) str = phsx.FallingCount.ToString();

            //    var phsx = Tools.CurLevel.Bobs[0].MyPhsx as BobPhsxMeat;
            //    //if (null != phsx) str = phsx.WallJumpCount.ToString();
            //    if (null != phsx) str = phsx.StepsSinceSide.ToString();
            //}

            // Mouse
            //string str = string.Format("Mouse delta: {0}", Tools.DeltaMouse);
            //string str = string.Format("Mouse in window: {0}", Tools.MouseInWindow);

            // VPlayer
            //string str = "";
            //if (VPlayer1 != null)
            //{
            //    str = VPlayer1.PlayPosition.Ticks.ToString();
            //    //Console.WriteLine(str);
            //}


            // GC
            //string str = GC.CollectionCount(0).ToString() + " " + fps.ToString() + "\n"
            //    + (RunningSlowly ? "SLOW" : "____ FAST") + "\n"
            //    + debugstring;
            string str = debugstring;

            // Phsx count
            //string str  = string.Format("CurLevel PhsxStep: {0}\n", Tools.CurLevel.CurPhsxStep);

            // Score
            //PlayerData p = PlayerManager.Get(0);
            //string str = string.Format("Death {0}, {1}, {2}, {3}, {4}", p.TempStats.TotalDeaths, p.LevelStats.TotalDeaths, p.GameStats.TotalDeaths, p.CampaignStats.TotalDeaths, Campaign.Attempts);
            //Campaign.CalcScore();
            //string str2 = string.Format("Coins {0}, {1}, {2}, {3}, {4}", p.TempStats.Coins, p.LevelStats.Coins, p.GameStats.Coins, p.CampaignStats.Coins, Campaign.Coins);
            //str += "\n\n" + str2;
            //string str3 = string.Format("Total {0}, {1}, {2}, {3}, {4}", p.TempStats.TotalCoins, p.LevelStats.TotalCoins, p.GameStats.TotalCoins, p.CampaignStats.TotalCoins, Campaign.TotalCoins);
            //str += "\n\n" + str3;
            //string str4 = string.Format("Total {0}, {1}, {2}, {3}", p.TempStats.TotalBlobs, p.LevelStats.TotalBlobs, p.GameStats.TotalBlobs, p.CampaignStats.TotalBlobs);
            //str += "\n" + str4;
            //string str5 = string.Format(" {0}, {1}, {2}, {3}", p.TempStats.Blobs, p.LevelStats.Blobs, p.GameStats.Blobs, p.CampaignStats.Blobs);
            //str += "\n" + str5;

            // Coins
            //string str = string.Format("{0}, {1}, {2}, {3}", p.TempStats.Coins, p.LevelStats.Coins, p.GameStats.Coins, p.CampaignStats.Coins);
            //string str2 = string.Format("{0}, {1}, {2}, {3}", p.TempStats.TotalCoins, p.LevelStats.TotalCoins, p.GameStats.TotalCoins, p.CampaignStats.TotalCoins);
            //str += "\n" + str2;
#else
            string str = debugstring;
#endif

            //str = string.Format("{0,-5} {1,-5} {2,-5} {3,-5} {4,-5}", Level.Pre1, Level.Step1, Level.Pre2, Level.Step2, Level.Post);


            Tools.Render.MySpriteBatch.DrawString(Resources.LilFont.Font,
                    str,
                    new Vector2(0, 100),
                    Color.Orange, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            Tools.Render.EndSpriteBatch();
        }
    }
}
#endif