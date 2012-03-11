using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom.Levels
{
    public class FlyingBlock_Parameters : AutoGen_Parameters
    {
        public Param Width, KeepUnused, Period;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            KeepUnused = new Param(PieceSeed);
            if (level.DefaultHeroType is BobPhsxSpaceship)
            {
                KeepUnused.SetVal(u => BobPhsxSpaceship.KeepUnused(u[Upgrade.FlyingBlock]));
            }

            FillWeight = new Param(PieceSeed, u => u[Upgrade.FlyingBlock]);

            Period = new Param(PieceSeed, u =>
            {
                return Tools.DifficultyLerp(217, 125, u[Upgrade.Speed]) *
                    Tools.DifficultyLerp(1.275f, .275f, u[Upgrade.FlyingBlock]);
            });

            Width = new Param(PieceSeed, u =>
            {
                return Math.Max(40, 93 - 2 * u[Upgrade.FlyingBlock]);
            });
        }
    }

    public sealed class FlyingBlock_AutoGen : AutoGen
    {
        static readonly FlyingBlock_AutoGen instance = new FlyingBlock_AutoGen();
        public static FlyingBlock_AutoGen Instance { get { return instance; } }

        static FlyingBlock_AutoGen() { }
        FlyingBlock_AutoGen()
        {
            Do_WeightedPreFill_1 = true;
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            FlyingBlock_Parameters Params = new FlyingBlock_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);
            level.AutoFlyingBlocks();
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
            level.CleanupFlyingBlocks(BL, TR);
        }

        public override IObject CreateAt(Level level, Vector2 pos, Vector2 BL, Vector2 TR)
        {
            base.CreateAt(level, pos, BL, TR);

            StyleData Style = level.Style;
            RichLevelGenData GenData = level.CurMakeData.GenData;
            PieceSeedData piece = level.CurMakeData.PieceSeed;

            // Get FlyingBlock parameters
            FlyingBlock_Parameters Params = (FlyingBlock_Parameters)level.Style.FindParams(FlyingBlock_AutoGen.Instance);

            int Period = (int)Params.Period.GetVal(pos);
            int Offset = level.Rnd.Rnd.Next(Period);


            int Width = (int)Params.Width.GetVal(pos);
            Vector2 size = new Vector2(Width, Width);

            FlyingBlock flyblock = (FlyingBlock)level.Recycle.GetObject(ObjectType.FlyingBlock, false);
            Vector2 Orbit = new Vector2(pos.X, pos.Y - 1200);//level.FillBL.Y - 300);

            flyblock.Init(Orbit, size);
            flyblock.Radii.Y = 1200;// pos.Y - Orbit.Y;
            flyblock.Period = Period;
            flyblock.Offset = Offset;

            flyblock.BlockCore.BlobsOnTop = true;

            flyblock.BlockCore.Decide_RemoveIfUnused(Params.KeepUnused.GetVal(pos), level.Rnd);
            flyblock.BlockCore.GenData.EdgeSafety = GenData.Get(DifficultyParam.EdgeSafety, pos);

            if (level.Style.RemoveBlockOnOverlap)
                flyblock.BlockCore.GenData.RemoveIfOverlap = true;

            level.AddBlock(flyblock);

            return flyblock;
        }
    }

    public partial class Level
    {
        public void CleanupFlyingBlocks(Vector2 BL, Vector2 TR)
        {
            // Get FlyingBlock parameters
            FlyingBlock_Parameters Params = (FlyingBlock_Parameters)Style.FindParams(FlyingBlock_AutoGen.Instance);
        }
        public void AutoFlyingBlocks()
        {
            // Get FlyingBlock parameters
            FlyingBlock_Parameters Params = (FlyingBlock_Parameters)Style.FindParams(FlyingBlock_AutoGen.Instance);
        }
    }
}
