using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Challenge_PlaceTraining : VanillaStringWorld
    {
        public static void StartLevel()
        {
            new Challenge_PlaceTraining().Start();
        }

        protected override void AdditionalPreStart()
        {
            base.AdditionalPreStart();

            // Tutorial
            //MyStringWorld.OnSwapToFirstLevel += data => data.MyGame.AddGameObject(new PlaceTrainingTutorial(this));

            // When a new level is swapped to...
            //MyStringWorld.OnSwapToLevel += levelindex => ();
        }

        public override List<LevelSeedData> GetSeeds()
        {
            List<LevelSeedData> list = new List<LevelSeedData>();

            WorldMap world = Tools.WorldMap as WorldMap;

            list.Add(Campaign.HeroLevel(Campaign.Difficulty, BobPhsxNormal.Instance, 3000, 1, LevelGeometry.Right, TileSet._NightSky));
            //list.Add(new LevelSeedData(() => world.Load(new Campaign_World2(true))));
            list.Add(Level1());
            list.Add(Level2());

            ProcessList(list);

            return list;
        }

        public static LevelSeedData Level1()
        {
            LevelSeedData data = new LevelSeedData();

            data.SetBackground(BackgroundType.Dungeon);
            data.DefaultHeroType = BobPhsxNormal.Instance;
            data.MyGameType = NormalGameData.Factory;

            data.MyGameType = PlaceGameData.Factory;
            data.PlaceObjectType = PlaceTypes.FallingBlock;

            data.PieceLength = 5000;
            data.NumPieces = 1;

            data.StandardInit((p, u) =>
            {
                u[Upgrade.SpikeyGuy] = D(0, 1.5, 5, 8, 10);
                u[Upgrade.SpikeyLine] = D(0, 1.5, 5, 8, 10);
                u[Upgrade.Speed] = D(0, 1.5, 3, 6, 10);

                p.Style.MyModParams = (level, piece) =>
                {
                    var Params = (NormalBlock_Parameters)piece[NormalBlock_AutoGen.Instance];
                    Params.Make = false;
                    Params.FillWeight = 5f;
                };
            });

            //data.PostMake += level =>
            //{
            //    // Title
            //    level.MyGame.WaitThenDo_Pausable(33, () =>
            //        WorldMap.AddTitle(level.MyGame, "Place\n   Training", 165));
            //};

            return data;
        }
        public static LevelSeedData Level2()
        {
            LevelSeedData data = new LevelSeedData();

            data.SetBackground(BackgroundType.Dungeon);
            data.DefaultHeroType = BobPhsxNormal.Instance;
            data.MyGameType = NormalGameData.Factory;

            data.MyGameType = PlaceGameData.Factory;
            data.PlaceObjectType = PlaceTypes.SuperBouncyBlock;

            data.PieceLength = 6000;
            data.NumPieces = 1;

            data.StandardInit((p, u) =>
            {
                u[Upgrade.FlyBlob] = D(1, 2.5, 5, 9, 10);
                u[Upgrade.Jump] = D(0, 1.5, 5, 9, 10);
                u[Upgrade.Laser] = D(0, 1.5, 5, 8, 10);
                u[Upgrade.Speed] = D(0, 1.5, 4, 6, 10);

                p.Style.MyModParams = (level, piece) =>
                {
                    var NParams = (NormalBlock_Parameters)piece[NormalBlock_AutoGen.Instance];
                    NParams.Make = false;
                    NParams.FillWeight = 5f;

                    var Params = (Goomba_Parameters)piece[Goomba_AutoGen.Instance];
                    Params.FillWeight = D(1, 4, 9, 9, 10);

                    var CParams = (Ceiling_Parameters)piece[Ceiling_AutoGen.Instance];
                    CParams.SetLongCeiling();
                };
            });

            return data;
        }
    }
}