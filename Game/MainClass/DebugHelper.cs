#if DEBUG
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    partial class CloudberryKingdomGame
    {
        bool ShowFPS = false;

#if PC_DEBUG || (WINDOWS && DEBUG) || INCLUDE_EDITOR
        /// <summary>
        /// Extra functions that allow a user to better debug/test/
        /// </summary>
        /// <returns>Return true if the calling method should return.</returns>
        private bool DebugModePhsx()
        {
			//return false;

#if WINDOWS && !MONO && !SDL2
            if (!Tools.ViewerIsUp && !KeyboardExtension.Freeze)
            {
                // Test title screen
                if (Tools.Keyboard.IsKeyDown(Keys.G) && !Tools.PrevKeyboard.IsKeyDown(Keys.G))
                {
                    //TitleGameFactory = TitleGameData_Intense.Factory;
                    //TitleGameFactory = TitleGameData_MW.Factory;
                    TitleGameFactory = TitleGameData_Clouds.Factory;
                    //TitleGameFactory = TitleGameData_Forest.Factory;

                    Tools.SongWad.Stop();
                    Tools.CurGameData = CloudberryKingdomGame.TitleGameFactory();
                    return true;
                }

				//if (Tools.Keyboard.IsKeyDown(Keys.J) && !Tools.PrevKeyboard.IsKeyDown(Keys.J))
				//{
				//    Tools.CurGameData.FadeToBlack();
				//}
            }

            //// Give award
            //if (Tools.keybState.IsKeyDown(Keys.S) && !Tools.PrevKeyboardState.IsKeyDown(Keys.S))
            //{
            //    Awardments.GiveAward(Awardments.UnlockHeroRush2);
            //}

            // Game Obj Viewer
            if (!Tools.ViewerIsUp && (!KeyboardExtension.Freeze || Tools.CntrlDown()) && (Tools.gameobj_viewer == null || Tools.gameobj_viewer.IsDisposed)
                && Tools.Keyboard.IsKeyDown(Keys.B) && !Tools.PrevKeyboard.IsKeyDown(Keys.B))
            {
                Tools.gameobj_viewer = new Viewer.GameObjViewer();
                Tools.gameobj_viewer.Show();
            }
            if (Tools.gameobj_viewer != null)
            {
                if (Tools.gameobj_viewer.IsDisposed)
                    Tools.gameobj_viewer = null;
                else
                    Tools.gameobj_viewer.Input();
            }

            // Background viewer
            if (!Tools.ViewerIsUp && !KeyboardExtension.Freeze && (Tools.background_viewer == null || Tools.background_viewer.IsDisposed)
                && Tools.Keyboard.IsKeyDown(Keys.V) && !Tools.PrevKeyboard.IsKeyDown(Keys.V))
            {
                Tools.background_viewer = new Viewer.BackgroundViewer();
                Tools.background_viewer.Show();
            }
            if (Tools.background_viewer != null)
            {
                if (Tools.background_viewer.IsDisposed)
                    Tools.background_viewer = null;
                else
                    Tools.background_viewer.Input();
            }

			if (Tools.background_viewer != null)
				return false;

            if (!Tools.ViewerIsUp && !KeyboardExtension.Freeze && Tools.Keyboard.IsKeyDownCustom(Keys.F) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.F))
                ShowFPS = !ShowFPS;
#endif

            // Reload some dynamic data (tileset info, animation specifications).
            if (Tools.Keyboard.IsKeyDownCustom(Keys.X) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.X))
            {
#if INCLUDE_EDITOR
                if (LoadDynamic)
                {
                    //Tools.TextureWad.LoadAllDynamic(Content, CoreTextureWad.WhatToLoad.Art);
                    //Tools.TextureWad.LoadAllDynamic(Content, CoreTextureWad.WhatToLoad.Backgrounds);
                    //Tools.TextureWad.LoadAllDynamic(Content, CoreTextureWad.WhatToLoad.Tilesets);
                    Tools.TextureWad.LoadAllDynamic(Tools.GameClass.Content, CoreEngine.CoreTextureWad.WhatToLoad.Animations);
                    //TileSets.LoadSpriteEffects();
                    //TileSets.LoadCode();
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
            }
#endif

            // Turn on a simple green screen background.
            if (Tools.Keyboard.IsKeyDownCustom(Keys.D9) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.D9)) Background.GreenScreen = !Background.GreenScreen;

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
            if (!GodMode)
            if (Tools.Keyboard.IsKeyDownCustom(Keys.O) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.O))
            {
                foreach (Bob bob in Tools.CurLevel.Bobs)
                {
                    bob.Immortal = !bob.Immortal;
                }
            }

            if (CloudberryKingdomGame.LoadResources)
            {
                // Turn on/off graphics.
                if (Tools.Keyboard.IsKeyDownCustom(Keys.Q) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.Q)) Tools.DrawGraphics = !Tools.DrawGraphics;

                // Turn on/off drawing of collision detection boxes.
                if (Tools.Keyboard.IsKeyDownCustom(Keys.W) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.W)) Tools.DrawBoxes = !Tools.DrawBoxes;
            }

            // Turn on/off step control. When activated, this allows you to step forward in the game by pressing <Enter>.
            if (Tools.Keyboard.IsKeyDownCustom(Keys.E) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.E)) Tools.StepControl = !Tools.StepControl;
            
            // Modify the speed of the game.
            if (Tools.Keyboard.IsKeyDownCustom(Keys.R) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.R)) Tools.IncrPhsxSpeed();

            // Don't do any of the following if a control box is up.
            if (!Tools.ViewerIsUp && !KeyboardExtension.Freeze)
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
            }

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
			if (Tools.CurCamera == null) return;

            var str = "Debug info test";

            Tools.QDrawer.DrawString(Resources.Font_Grobold42.HFont, str, Tools.CurCamera.Pos + new Vector2(-1300, 800), Color.Orange.ToVector4(), new Vector2(2));
			Tools.QDrawer.Flush();
        }
    }
}
#endif