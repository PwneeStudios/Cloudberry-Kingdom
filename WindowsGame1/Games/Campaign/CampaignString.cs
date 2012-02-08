using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using Drawing;
using System.Linq;
using System.Collections.Generic;
using CloudberryKingdom.Goombas;

namespace CloudberryKingdom
{
    public class Campaign_String : WorldMap
    {
        public override void Release()
        {
            base.Release();
        }

        public static Campaign_String MainString;

        int Index = 0;
        int DelayPhsx = 0;
        bool ShouldIncr = false;
        public override void PhsxStep()
        {
            base.PhsxStep();

            // Make sure the previous game gets released
            if (PrevGame != null && !PrevGame.Released && PrevGame is WorldMap) PrevGame.Release();

            // Skip a few frames to allow for a load to take hold
            if (DelayPhsx > 0) { DelayPhsx--; return; }
            if (Tools.ShowLoadingScreen) return;

            // Do not go to the next level if we didn't beat the current one
            if (Tools.CurrentAftermath != null && Tools.CurrentAftermath.Success == false)
            {
                if (Tools.CurrentAftermath.Retry)
                    ShouldIncr = false;
                else
                {
#if DEBUG
                    //ShouldIncr = false;
                    //ShouldIncr = true;
                    EndGame(false);
#else
                    EndGame(false);
#endif
                }
            }
            if (ShouldIncr) Index++;

            // Stats
            PlayerData p = PlayerManager.Get(0);
            string str3 = string.Format("Total {0}, {1}, {2}, {3}, {4}", p.TempStats.TotalCoins, p.LevelStats.TotalCoins, p.GameStats.TotalCoins, p.CampaignStats.TotalCoins, Campaign.TotalCoins);
            PlayerManager.AbsorbTempStats();
            PlayerManager.AbsorbLevelStats();
            PlayerManager.AbsorbGameStats();

            // Load the next level
            PreventDoorActions = false;
            Seeds[Index].Do();
            DelayPhsx = 2;
            PhsxStepsToDo += 3;

            ShouldIncr = true;
        }

        public override void ReturnTo(int code)
        {
            base.ReturnTo(code);
        }

        public Campaign_String()
            : base(false)
        {
            MainString = this;
            Data = Campaign.Data;
            WorldName = "CampaignString";

            DoDoorProcessing = false;
            DoorDelay_ToLevel = 0;

            Init("Doom\\SimpleCastle_Simple.lvl");

            MakeCenteredCamZone(.72f);
            MakeBackground(BackgroundType.Outside);

            // Players
            SetHeroType(BobPhsxNormal.Instance);
            MakePlayers();

            // Menu
            //AddGameObject(InGameStartMenu_Campaign.MakeListener());

            MakeList();
        }

        List<Door> Seeds = new List<Door>();
        void Add(LevelSeedData d)
        {
            //d.NoMusicStart = true;

            Door door = new Door(false);
            door.SuppressSound = true;
            door.Core.EditorCode1 = Seeds.Count.ToString();
            ProcessDoor(door);
            MakeDoorAction_Level(door, d);
            Seeds.Add(door);
            MyLevel.AddObject(door);
        }
        void AddSubGame(Func<GameData> MakeGame)
        {
            string title = LevelSeedData.GetTitle();
            Add(() => LoadAsSubGame(MakeGame(), title));
        }
        void Add(Action act)
        {
            Door door = new Door(false);
            door.SuppressSound = true;
            door.Core.EditorCode1 = Seeds.Count.ToString();
            ProcessDoor(door);
            MakeDoorAction_Level(door, act);
            Seeds.Add(door);
            MyLevel.AddObject(door);
        }

        void QuickSpawnHint()
        {
            Tools.CurrentLoadingScreen.AddHint(HintGiver.QuickSpawnHint, 169);
        }

        void PowerupHint()
        {
            Tools.CurrentLoadingScreen.AddHint(HintGiver.PowerupHint, 169);
        }

        public static Action<GUI_Level> ModTitle;
        protected void MakeList()
        {
            LevelSeedData d;

            // DEBUG: make the first door the victory door
            if (DEBUG_FirstDoorVictory)
                Add(CampaignEnd);

            //d = Campaign.HeroLevel(Campaign.Difficulty, BobPhsxNormal.Instance, 4000, 1);
            //d.SetBackground(TileSet.Terrace);
            //d.PieceSeeds[0].Style.MyInitialPlatsType = StyleData.InitialPlatsType.CastleToTerrace;
            //d.PieceSeeds[0].Style.MyFinalDoorStyle = StyleData.FinalDoorStyle.TerraceToCastle;
            //d.PostMake += lvl => Campaign.LevelWithPrincess(lvl, true, Campaign.PrincessPos.CenterToRight);
            //Add(d);

            // Start music
            //LevelSeedData.BOL_StartMusic();

            //for (int i = 0; i < 3; i++)
            //    PlayerManager.Get(i).IsAlive = PlayerManager.Get(i).Exists = true;


            // Test credits
            //Add(() => LoadAsSubGame(new Campaign_PrincessRoom()));
            //Add(() => LoadAsSubGame(new Campaign_Boss(true)));


            // new up level intro
            //d = Campaign.HeroLevel(Campaign.Difficulty + .2f, BobPhsxDouble.Instance, 4000, 1, LevelGeometry.Up, TileSet.Dungeon);
            //d.PostMake += lvl => Campaign_UpExplosion.UpExplosion(lvl);
            //d.SetBackFirstAttemp(Campaign_UpExplosion.SetBack);
            //d.SetToShowLevelTitle(false);
            //d.SetToStartSong(Tools.Song_TidyUp);
            //Add(d);

            // new down level
            //d = Campaign.HeroLevel(Campaign.Difficulty + .2f, BobPhsxBox.Instance, 4500, 1, LevelGeometry.Down);
            //d.PostMake += Campaign_PrincessDown.PrincessDown;
            //d.SetToShowLevelTitle(false);
            //d.MyGeometry = LevelGeometry.Down;
            //d.PieceSeeds[0].Style.MyFinalPlatsType = StyleData.FinalPlatsType.DarkBottom;
            //d.SetBackground(BackgroundType.Outside);
            //d.SetToStartSong(Tools.Song_GetaGrip);
            //Add(d);

            //AddSubGame(() => new Campaign_GetJetpack(BackgroundType.Night, Tools.Song_BlueChair));
            //AddSubGame(() => new Campaign_GetCart(Tools.Song_Evidence));
            //AddSubGame(() => new Campaign_CartToBox(Tools.Song_FatInFire));
            
            //Add(() => Challenge_SavePrincessRush.Instance.Start(Campaign.Index));
            //Add(() => LoadAsSubGame(new Campaign_PrincessOverLava_Get()));


            //d = Campaign.HeroLevel(Campaign.Difficulty, BobPhsxRocketbox.Instance, 15500, 1, LevelGeometry.Right, TileSet.Island);
            //d.PieceSeeds[0].Style.MyInitialPlatsType = StyleData.InitialPlatsType.CastleToTerrace;
            //d.PieceSeeds[0].Style.MyFinalDoorStyle = StyleData.FinalDoorStyle.TerraceToCastle;
            //d.SetToShowLevelTitle();
            //d.NoMusicStart = true;
            //Add(d);

            // Test boss
            //Add(() => LoadAsSubGame(new Campaign_BossNew(true), false));

            // Hardcore intro
            if (Campaign.Difficulty > 2)
            //if (Campaign.Difficulty > 1 && !Campaign_Chaos.WatchedOnce)
            {
                AddSubGame(() => new Campaign_Chaos(null));
            }


            // World titles
            LevelSeedData.FirstWorld();
            Tools.SongWad.SuppressNextInfoDisplay = true;

            // First level, princess, dramatic entrance
            d = Campaign.HeroLevel(Campaign.Difficulty, BobPhsxNormal.Instance, 7000, 2);
            d.SetBackground(TileSet.Terrace);
            //d.PieceSeeds[0].Style.MyInitialPlatsType = StyleData.InitialPlatsType.CastleToTerrace;
            d.PieceSeeds[1].Style.MyFinalDoorStyle = StyleData.FinalDoorStyle.TerraceToCastle;
            d.PostMake += lvl => Campaign.LevelWithPrincess(lvl, true, Campaign.PrincessPos.CenterToRight, true);
            var style = d.PieceSeeds[0].Style as SingleData;
            style.InitialDoorYRange = new Vector2(-400);
            //d.SetToShowLevelTitle(240);
            d.SetToShowLevelTitle(40);
            d.SetBackFirstAttemp(Campaign.LvlWithPrincess_SetBack);
            d.SetToStartSong(Tools.Song_BlueChair);
            //d.PostMake += lvl =>
            //{
            //    // Slide in score/level gui before level starts
            //    GameData g = lvl.MyGame;
            //    foreach (var obj in g.MyGameObjects)
            //    {
            //        if (obj is GUI_Level || obj is GUI_Score)
            //            ((GUI_Panel)obj).SlideIn(0);
            //    }
            //};
            Add(d);


            // Level 2, Wall
            d = Campaign.WallLevel(Campaign.Difficulty, BobPhsxNormal.Instance, 1.15f);
            d.SetBackground(BackgroundType.Outside);
            d.SetToShowLevelTitle();
            d.DelayEntrance();
            d.SetToStartSong(Tools.Song_Ripcurl);
            Add(d);

            // Get the cart
            AddSubGame(() => new Campaign_GetCart(Tools.Song_Nero));

            d = Campaign.HeroLevel(Campaign.Difficulty, BobPhsxRocketbox.Instance, 15500, 1, LevelGeometry.Right, TileSet.Island);
            d.PieceSeeds[0].Style.MyInitialPlatsType = StyleData.InitialPlatsType.CastleToTerrace;
            d.PieceSeeds[0].Style.MyFinalDoorStyle = StyleData.FinalDoorStyle.TerraceToCastle;
            d.SetToShowLevelTitle();
            d.NoMusicStart = true;
            Add(d);

            // Cart-to-box
            AddSubGame(() => new Campaign_CartToBox(Tools.Song_BlueChair));

            d = Campaign.HeroLevel(Campaign.Difficulty, BobPhsxBox.Instance, 6500, 1, LevelGeometry.Right, TileSet.Castle);
            d.SetToShowLevelTitle();
            d.NoMusicStart = true;
            d.OnBeginLoad += QuickSpawnHint;
            Add(d);

            //d = Campaign.HeroLevel(Campaign.Difficulty + .2f, BobPhsxBox.Instance, 9500, 1, LevelGeometry.Down);
            //d.SetToShowLevelTitle(false);
            //d.MyGeometry = LevelGeometry.Down;
            //d.PieceSeeds[0].Style.MyFinalPlatsType = StyleData.FinalPlatsType.DarkBottom;
            //d.SetBackground(BackgroundType.Outside);
            //d.DelayEntrance();
            //d.SetToStartSong(Tools.Song_GetaGrip);
            //Add(d);
            d = Campaign.HeroLevel(Campaign.Difficulty + .2f, BobPhsxBox.Instance, 9500, 1, LevelGeometry.Down);
            d.PostMake += Campaign_PrincessDown.PrincessDown;
            d.SetToShowLevelTitle(false);
            d.MyGeometry = LevelGeometry.Down;
            d.PieceSeeds[0].Style.MyFinalPlatsType = StyleData.FinalPlatsType.DarkBottom;
            d.SetBackground(BackgroundType.Outside);
            d.SetToStartSong(Tools.Song_GetaGrip);
            d.SetBackFirstAttemp(Campaign_PrincessDown.SetBack);
            Add(d);

            // NEW WORLD
            LevelSeedData.NewWorld();

            // To dungeon, from above (Become tiny)
            AddSubGame(() => new Campaign_DungeonFromAbove(Tools.Song_FatInFire));

            d = Campaign.HeroLevel(Campaign.Difficulty, BobPhsxSmall.Instance, 8500, 2, LevelGeometry.Right, TileSet.Dungeon);
            d.SetToShowLevelTitle();
            d.DelayEntrance();
            d.NoMusicStart = true;
            d.OnBeginLoad += PowerupHint;
            Add(d);
            d = Campaign.HeroLevel(Campaign.Difficulty, BobPhsxSmall.Instance, 8500, 2, LevelGeometry.Right, TileSet.Dungeon);
            d.SetToShowLevelTitle();
            d.DelayEntrance();
            d.SetToStartSong(Tools.Song_House);
            Add(d);

            // Get double jump
            AddSubGame(() => new Campaign_GetDouble(Tools.Song_Evidence));

            d = Campaign.HeroLevel(Campaign.Difficulty + .25f, BobPhsxDouble.Instance, 9000, 2, LevelGeometry.Right, TileSet.Dungeon);
            d.SetToShowLevelTitle();
            d.DelayEntrance();
            d.NoMusicStart = true;
            Add(d);
            //d = Campaign.HeroLevel(Campaign.Difficulty + .2f, BobPhsxDouble.Instance, 7500, 1, LevelGeometry.Up, TileSet.Dungeon);
            //d.SetToShowLevelTitle(false);
            //d.DelayEntrance();
            //d.SetToStartSong(Tools.Song_TidyUp);
            //Add(d);
            d = Campaign.HeroLevel(Campaign.Difficulty + .2f, BobPhsxDouble.Instance, 7500, 1, LevelGeometry.Up, TileSet.Dungeon);
            d.PostMake += lvl => Campaign_UpExplosion.UpExplosion(lvl);
            d.SetBackFirstAttemp(Campaign_UpExplosion.SetBack);
            d.SetToShowLevelTitle(false);
            d.SetToStartSong(Tools.Song_TidyUp);
            Add(d);

            // NEW WORLD
            LevelSeedData.NewWorld();

            // Get wheelie
            AddSubGame(() => new Campaign_GetWheelie(Tools.Song_WritersBlock));

            d = Campaign.HeroLevel(Campaign.Difficulty, BobPhsxWheel.Instance, 9500, 2, LevelGeometry.Right);
            d.SetBackground(BackgroundType.Night);
            d.SetTileSet(TileSet.Dungeon);
            d.SetToShowLevelTitle();
            d.DelayEntrance();
            d.NoMusicStart = true;
            Add(d);

            // Spaceship
            d = Campaign.HeroLevel(Campaign.Difficulty, BobPhsxSpaceship.Instance, 9000, 2, LevelGeometry.Right);
            d.SetToShowLevelTitle(false);
            d.SetBackground(BackgroundType.Castle);
            d.NoMusicStart = true;
            d.PostMake += lvl => lvl.MyGame.AddGameObject(new PreSpaceship());
            Add(d);

            // Up level, from ground level, get jetpack
            AddSubGame(() => new Campaign_GetJetpack(BackgroundType.Night, Tools.Song_BlueChair));

            d = Campaign.HeroLevel(Campaign.Difficulty + .2f, BobPhsxJetman.Instance, 7500, 1, LevelGeometry.Up);
            d.SetBackground(BackgroundType.Night);
            d.PieceSeeds[0].Style.MyInitialPlatsType = StyleData.InitialPlatsType.Door;
            //d.PostMake += lvl => lvl.Move(new Vector2(0, 1000), false);
            //d.SetToShowLevelTitle();
            d.SetToShowLevelTitle(false);
            d.DelayEntrance();
            d.NoMusicStart = true;
            Add(d);

            d = Campaign.HeroLevel(Campaign.Difficulty + .2f, BobPhsxJetman.Instance, 9500, 2, LevelGeometry.Right, TileSet._NightSky);
            d.PieceSeeds[0].Style.MyInitialPlatsType = StyleData.InitialPlatsType.CastleToTerrace;
            d.SetToShowLevelTitle();
            d.DelayEntrance();
            d.SetToStartSong(Tools.Song_Evidence);
            Add(d);

            // Get bouncy
            AddSubGame(() => new Campaign_GetBouncy(Tools.Song_FatInFire));

            d = Campaign.HeroLevel(Campaign.Difficulty, BobPhsxBouncy.Instance, 7250, 2, LevelGeometry.Right);
            d.SetBackground(BackgroundType.Night);
            d.SetTileSet(TileSet.Dungeon);
            d.SetToShowLevelTitle();
            d.DelayEntrance();
            d.NoMusicStart = true;
            Add(d);


            // FINAL

            // Dramatic scene, princess over lava, followed by Rush
            int NumFloors = Challenge_SavePrincessRush.NumLevels[Campaign.Index] + 1;
            ModTitle = t =>
            {
                CampaignMenu.HardcoreColor(t.LevelText);
                t.MyPile.Pos += new Vector2(40, 0);
            };
            Add(() => LoadAsSubGame(new Campaign_DarkTower(), "The Dark Tower", ModTitle));
            Add(() => Challenge_SavePrincessRush.Instance.Start(Campaign.Index));
            Add(() => LoadAsSubGame(new Campaign_PrincessOverLava_Get(), "Floor " + NumFloors.ToString()));

            
            // NEW WORLD
            LevelSeedData.NewWorld();

            // Rain, carrying princess with RUMBLE
            d = Campaign.HeroLevel(Campaign.Difficulty + .3f, BobPhsxNormal.Instance, 7000, 2);
            //d.SetToShowLevelTitle(false);
            d.SetToShowLevelTitle();
            d.SetBackground(TileSet.Rain);
            d.PieceSeeds[0].Style.MyInitialPlatsType = StyleData.InitialPlatsType.CastleToTerrace;
            d.PieceSeeds[1].Style.MyFinalDoorStyle = StyleData.FinalDoorStyle.TerraceToCastle;

            Campaign.CarryPrinces(d);
            bool RumbleAdded = false;
            d.PostMake += lvl => lvl.MyGame.OnCheckpointGrab +=
                (cp =>
                {
                    if (!RumbleAdded)
                        lvl.MyGame.WaitThenDo(85, () => lvl.MyGame.AddGameObject(new Rumble()));
                    RumbleAdded = true;
                });
            d.LavaMake = LevelSeedData.LavaMakeTypes.NeverMake;
            d.DelayEntrance();
            d.SetToStartSong(Tools.Song_BlueChair);
            Add(d);

            // Boss
            Add(() => LoadAsSubGame(new Campaign_BossNew(true), false));
        }

        public override void SetToReturnTo(int code)
        {
            base.SetToReturnTo(code);
        }
    }
}