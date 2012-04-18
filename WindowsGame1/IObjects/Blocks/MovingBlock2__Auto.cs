using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;

namespace CloudberryKingdom.Levels
{
    public class MovingBlock2_Parameters : AutoGen_Parameters
    {
        public Param Range, Period, KeepUnused, Size;

        public int[] MotionLevel = { 0,        1,          2,  3,     4,        5,       6   };
        public enum MotionType     { Vertical, Horizontal, AA, Cross, Straight, Cirlces, All };
        public MotionType Motion;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            float lvl = PieceSeed.MyUpgrades1[Upgrade.MovingBlock2];

            Size = new Param(PieceSeed);
            Size.SetVal(u => 230 - (230 - 50)/10f * u[Upgrade.MovingBlock2]);

            Motion = (MotionType)level.Rnd.Choose(MotionLevel, (int)lvl);

            KeepUnused = new Param(PieceSeed);
            if (level.DefaultHeroType is BobPhsxSpaceship)
            {
                KeepUnused.SetVal(u => BobPhsxSpaceship.KeepUnused(u[Upgrade.MovingBlock2]));
            }

            FillWeight = new Param(PieceSeed);
            FillWeight.SetVal(u => u[Upgrade.MovingBlock2]);

            Range = new Param(PieceSeed);
            Range.SetVal(u => Tools.DifficultyLerp(240, 600, .5f * (u[Upgrade.Jump] + u[Upgrade.MovingBlock2])));

            Period = new Param(PieceSeed);
            Period.SetVal(u =>
            {
                float speed = 280 - 32 * u[Upgrade.Speed] + 40 * .5f * (u[Upgrade.Jump] + u[Upgrade.MovingBlock2]);
                return Tools.Restrict(40, 1000, speed);
            });
        }
    }

    public sealed class MovingBlock2_AutoGen : AutoGen
    {
        static readonly MovingBlock2_AutoGen instance = new MovingBlock2_AutoGen();
        public static MovingBlock2_AutoGen Instance { get { return instance; } }

        static MovingBlock2_AutoGen() { }
        MovingBlock2_AutoGen()
        {
            Do_WeightedPreFill_1 = true;
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            MovingBlock2_Parameters Params = new MovingBlock2_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public void SetMoveType(MovingBlock2 mblock, float Displacement, MovingBlock2_Parameters.MotionType mtype, Rand Rnd)
        {
            switch (mtype)
            {
                //case MovingBlock2_Parameters.MotionType.Vertical:
                default:
                    mblock.MoveType = MovingBlock2MoveType.Line;
                    mblock.Displacement = new Vector2(0, .5f * Displacement);

                    mblock.Points[0] = mblock.Pos + new Vector2(0, Displacement);
                    mblock.Points[1] = mblock.Pos + new Vector2(0, -Displacement);
                    mblock.NumPoints = 2;

                    var d = Displacement;
                    mblock.Points[0] = mblock.Pos + new Vector2(-d / 2, d);
                    mblock.Points[1] = mblock.Pos + new Vector2(d / 2, 0);
                    mblock.Points[2] = mblock.Pos + new Vector2(-d / 2, -d);
                    mblock.NumPoints = 3;

                    mblock.Points[0] = mblock.Pos + new Vector2(-d,  d);
                    mblock.Points[1] = mblock.Pos + new Vector2(d,   d);
                    mblock.Points[2] = mblock.Pos + new Vector2(d,  -d);
                    mblock.Points[3] = mblock.Pos + new Vector2(-d, -d);
                    mblock.Points[4] = mblock.Pos + new Vector2(-d,  d);
                    mblock.NumPoints = 5;
                    
                    break;

            }
        }

        public override ObjectBase CreateAt(Level level, Vector2 pos, Vector2 BL, Vector2 TR)
        {
            base.CreateAt(level, pos, BL, TR);

            StyleData Style = level.Style;
            RichLevelGenData GenData = level.CurMakeData.GenData;
            PieceSeedData piece = level.CurMakeData.PieceSeed;

            // Get MovingBlock2 parameters
            MovingBlock2_Parameters Params = (MovingBlock2_Parameters)level.Style.FindParams(MovingBlock2_AutoGen.Instance);

            Vector2 size = new Vector2(Params.Size.GetVal(pos), 40);

            Vector2 offset = Vector2.Zero;

            if (level.Style.BlockFillType == StyleData._BlockFillType.Spaceship)
            {
                offset += new Vector2(level.Rnd.Rnd.Next(0, 100), level.Rnd.Rnd.Next(0, 100));

                if (pos.X > level.CurMakeData.PieceSeed.End.X - 400) offset.X -= pos.X - level.CurMakeData.PieceSeed.End.X + 400;
                if (pos.X < level.CurMakeData.PieceSeed.Start.X + 400) offset.X += level.CurMakeData.PieceSeed.Start.X - pos.X + 400;

                TR.X -= 350;
                BL.X += 200;
            }

            MovingBlock2 mblock = (MovingBlock2)level.Recycle.GetObject(ObjectType.MovingBlock2, true);
            mblock.Init(pos + offset, size, level.DefaultHeroType is BobPhsxSpaceship ? false : true);

            mblock.Period = (int)Params.Period.GetVal(pos);

            mblock.Offset = level.Style.GetOffset(mblock.Period, pos,
                                                        level.Style.MovingBlock2OffsetType);


            float Displacement = Params.Range.GetVal(pos);
            SetMoveType(mblock, Displacement, Params.Motion, level.Rnd);

            mblock.BlockCore.Decide_RemoveIfUnused(Params.KeepUnused.GetVal(pos), level.Rnd);
            mblock.BlockCore.GenData.EdgeSafety = GenData.Get(DifficultyParam.EdgeSafety, pos);

            if (level.Style.RemoveBlockOnOverlap)
                mblock.BlockCore.GenData.RemoveIfOverlap = true;

            Tools.EnsureBounds_X(mblock, TR, BL);
            
            level.AddBlock(mblock);

            return mblock;
        }
    }
}
