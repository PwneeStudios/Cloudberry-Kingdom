using Microsoft.Xna.Framework;

using CloudberryKingdom.Clouds;

namespace CloudberryKingdom.Levels
{
    public class Cloud_Parameters : AutoGen_Parameters
    {
        public Param Size, Shiftiness;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            FillWeight = new Param(PieceSeed);
            FillWeight.SetVal(u =>
            {
                return .62f * u[Upgrade.Cloud];
            });

            Shiftiness = new Param(PieceSeed);
            Shiftiness.SetVal(u =>
            {
                return 1 + .33f * u[Upgrade.Cloud];
            });

            Size = new Param(PieceSeed);
            Size.SetVal(u =>
            {
                return 2f - .1f * u[Upgrade.Cloud];
            });
        }
    }

    public sealed class Cloud_AutoGen : AutoGen
    {
        static readonly Cloud_AutoGen instance = new Cloud_AutoGen();
        public static Cloud_AutoGen Instance { get { return instance; } }

        static Cloud_AutoGen() { }
        Cloud_AutoGen()
        {
            Do_WeightedPreFill_1 = true;
            //Generators.AddGenerator(this);
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            Cloud_Parameters Params = new Cloud_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);
            level.AutoClouds();
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
            level.CleanupClouds(BL, TR);
        }

        public override ObjectBase CreateAt(Level level, Vector2 pos, Vector2 BL, Vector2 TR)
        {
            base.CreateAt(level, pos, BL, TR);

            // Get Cloud parameters
            Cloud_Parameters Params = (Cloud_Parameters)level.Style.FindParams(Cloud_AutoGen.Instance);

            // Make the new cloud
            pos += new Vector2(level.Rnd.Rnd.Next(0, 70), level.Rnd.Rnd.Next(0, 70));
            Cloud NewCloud = (Cloud)level.MySourceGame.Recycle.GetObject(ObjectType.Cloud, true);
            float size = Params.Size.GetVal(pos);

            TileSet tileset = level.MyTileSet;

            if (level.MyLevelSeed.MyBackgroundType == BackgroundType.Sky)
                tileset = TileSet.Terrace;
            NewCloud.Init(new Vector2(120f * size, 50), TileSets.Get(tileset));

            NewCloud.Core.Data.Position = NewCloud.Core.StartData.Position = pos;

            NewCloud.Shiftiness = Params.Shiftiness.GetVal(pos);

            /*
            if (MyLevel.Rnd.Rnd.NextDouble() < .01f * level.CurMakeData.GenData.Get(DifficultyType.HeadDanger, pos)
                * level.Style.KeepUnusedFlyingCloud)
                NewCloud.Core.GenData.RemoveIfUnused = false;
            else*/
                NewCloud.Core.GenData.RemoveIfUnused = true;

            if (level.Style.RemoveBlockOnOverlap)
                NewCloud.Core.GenData.RemoveIfOverlap = true;

            level.AddObject(NewCloud);

            return NewCloud;
        }
    }

    public partial class Level
    {
        public void CleanupClouds(Vector2 BL, Vector2 TR)
        {
            // Get Cloud parameters
            Cloud_Parameters Params = (Cloud_Parameters)Style.FindParams(Cloud_AutoGen.Instance);
        }
        public void AutoClouds()
        {
            // Get Cloud parameters
            Cloud_Parameters Params = (Cloud_Parameters)Style.FindParams(Cloud_AutoGen.Instance);
        }
    }
}
