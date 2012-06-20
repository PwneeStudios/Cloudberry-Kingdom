using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Challenge_ObstacleTraining : VanillaStringWorld
    {
        public static void StartLevel()
        {
            new Challenge_ObstacleTraining().Start();
        }

        protected override void AdditionalPreStart()
        {
            base.AdditionalPreStart();

            // Tutorial
            //MyStringWorld.OnSwapToFirstLevel += data => data.MyGame.AddGameObject(new ObstacleTrainingTutorial(this));

            // When a new level is swapped to...
            //MyStringWorld.OnSwapToLevel += levelindex => ();
        }

        public override List<LevelSeedData> GetSeeds()
        {
            List<LevelSeedData> list = new List<LevelSeedData>();

            list.Add(Level1());
            list.Add(Level2());
            //list.Add(Level3());

            ProcessList(list);

            return list;
        }

        public static LevelSeedData Level1()
        {
            LevelSeedData data = new LevelSeedData();

            data.SetTileSet(TileSets.Dungeon);
            data.DefaultHeroType = BobPhsxSmall.Instance;
            data.MyGameType = NormalGameData.Factory;

            data.MyGameType = PlaceGameData.Factory;
            data.PlaceObjectType = PlaceTypes.FlyingBlob;

            data.PieceLength = 3000;
            data.NumPieces = 1;

            data.StandardInit((p, u) =>
            {
                p.Style.MyModParams = (level, piece) =>
                {
                    var CParams = (Ceiling_Parameters)piece[Ceiling_AutoGen.Instance];
                    CParams.SetLongCeiling();
                };
            });

            data.PostMake += level =>
            {
                // Title
                level.MyGame.WaitThenDo_Pausable(33, () =>
                    WorldMap.AddTitle(level.MyGame, "Obstacle\n   Training", 165));
            };

            return data;
        }
        public static LevelSeedData Level2()
        {
            LevelSeedData data = new LevelSeedData();

            data.SetTileSet(TileSets.Dungeon);
            data.DefaultHeroType = BobPhsxNormal.Instance;
            data.MyGameType = NormalGameData.Factory;

            data.MyGameType = PlaceGameData.Factory;
            data.PlaceObjectType = PlaceTypes.SuperBouncyBlock;

            data.PieceLength = 3000;
            data.NumPieces = 1;

            data.StandardInit((p, u) =>
            {
                u[Upgrade.FlyBlob] = D(1, 2.5, 5, 9, 10);
                u[Upgrade.Jump] = D(0, 1.5, 5, 9, 10);
                u[Upgrade.Speed] = D(0, 1.5, 4, 7, 10);

                p.Style.MyModParams = (level, piece) =>
                {
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