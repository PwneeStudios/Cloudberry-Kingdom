using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;

namespace CloudberryKingdom.Levels
{
    public class MovingBlock_Parameters : AutoGen_Parameters
    {
        public Param Range, Period, KeepUnused, Size;

        public int[] MotionLevel = { 0,        1,          2,  3,     4,        5,       6   };
        public enum MotionType     { Vertical, Horizontal, AA, Cross, Straight, Cirlces, All };
        public MotionType Motion;

        public float[] AspectTypeRatio = { 1f, 1f, 1f };
        public enum AspectType { Square, Thin, Tall };
        public AspectType Aspect;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            float MovingBlockLevel = PieceSeed.MyUpgrades1[Upgrade.MovingBlock];

            // If this is a multi-path level then always use thin blocks
            if (PieceSeed.Paths > 1)
                Aspect = AspectType.Thin;
            // otherwise randomize the aspect ratio
            else
                Aspect = (AspectType)level.Rnd.Choose(AspectTypeRatio);

            // No tall blocks on vertical levels
            if (PieceSeed.GeometryType == LevelGeometry.Up || PieceSeed.GeometryType == LevelGeometry.Down)
                if (Aspect == AspectType.Tall)
                    Aspect = AspectType.Thin;

            Size = new Param(PieceSeed);
            Size.SetVal(u =>
            {
                return 230 - (230 - 50)/10f * u[Upgrade.MovingBlock];
            });

            Motion = (MotionType)level.Rnd.Choose(MotionLevel, (int)MovingBlockLevel);

            KeepUnused = new Param(PieceSeed);
            if (level.DefaultHeroType is BobPhsxSpaceship)
            {
                KeepUnused.SetVal(u => BobPhsxSpaceship.KeepUnused(u[Upgrade.MovingBlock]));
            }

            FillWeight = new Param(PieceSeed);
            FillWeight.SetVal(u =>
            {
                return u[Upgrade.MovingBlock];
            });

            Range = new Param(PieceSeed);
            Range.SetVal(u =>
            {
                return Tools.DifficultyLerp(240, 600, .5f * (u[Upgrade.Jump] + u[Upgrade.MovingBlock]));
            });

            Period = new Param(PieceSeed);
            Period.SetVal(u =>
            {
                float speed = 280 - 32 * u[Upgrade.Speed] + 40 * .5f * (u[Upgrade.Jump] + u[Upgrade.MovingBlock]);
                return Tools.Restrict(40, 1000, speed);
            });
        }
    }

    public sealed class MovingBlock_AutoGen : AutoGen
    {
        static readonly MovingBlock_AutoGen instance = new MovingBlock_AutoGen();
        public static MovingBlock_AutoGen Instance { get { return instance; } }

        static MovingBlock_AutoGen() { }
        MovingBlock_AutoGen()
        {
            Do_WeightedPreFill_1 = true;
            //Generators.AddGenerator(this);
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            MovingBlock_Parameters Params = new MovingBlock_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);
            level.AutoMovingBlocks();
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
            level.CleanupMovingBlocks(BL, TR);
        }

        public void SetMoveType(MovingBlock mblock, float Displacement, MovingBlock_Parameters.MotionType mtype, Rand Rnd)
        {
            switch (mtype)
            {
                case MovingBlock_Parameters.MotionType.Vertical:
                    mblock.MoveType = MovingBlockMoveType.Line;
                    mblock.Displacement = new Vector2(0, .5f * Displacement);
                    break;

                case MovingBlock_Parameters.MotionType.Horizontal:
                    mblock.MoveType = MovingBlockMoveType.Line;
                    mblock.Displacement = new Vector2(Displacement, 0);
                    break;

                case MovingBlock_Parameters.MotionType.Cross:
                    mblock.MoveType = MovingBlockMoveType.Line;
                    if (Rnd.Rnd.NextDouble() > .5)
                        mblock.Displacement = new Vector2(Displacement, .5f * Displacement);
                    else
                        mblock.Displacement = new Vector2(-Displacement, .5f * Displacement);
                    break;

                case MovingBlock_Parameters.MotionType.Cirlces:
                    mblock.MoveType = MovingBlockMoveType.Circle;
                    mblock.Displacement = new Vector2(Displacement * .5f, Displacement * .5f);
                    mblock.Displacement.X *= Rnd.Rnd.Next(0, 2) * 2 - 1;
                    break;

                case MovingBlock_Parameters.MotionType.AA:
                    if (Rnd.Rnd.NextDouble() > .5)
                        SetMoveType(mblock, Displacement, MovingBlock_Parameters.MotionType.Vertical, Rnd);
                    else
                        SetMoveType(mblock, Displacement, MovingBlock_Parameters.MotionType.Horizontal, Rnd);
                    break;

                case MovingBlock_Parameters.MotionType.Straight:
                    if (Rnd.Rnd.NextDouble() > .5)
                        SetMoveType(mblock, Displacement, MovingBlock_Parameters.MotionType.Cross, Rnd);
                    else
                        SetMoveType(mblock, Displacement, MovingBlock_Parameters.MotionType.AA, Rnd);
                    break;

                case MovingBlock_Parameters.MotionType.All:
                    if (Rnd.Rnd.NextDouble() > .5)
                        SetMoveType(mblock, Displacement, MovingBlock_Parameters.MotionType.Straight, Rnd);
                    else
                        SetMoveType(mblock, Displacement, MovingBlock_Parameters.MotionType.Cirlces, Rnd);
                    break;
            }
        }

        public override ObjectBase CreateAt(Level level, Vector2 pos, Vector2 BL, Vector2 TR)
        {
            base.CreateAt(level, pos, BL, TR);

            StyleData Style = level.Style;
            RichLevelGenData GenData = level.CurMakeData.GenData;
            PieceSeedData piece = level.CurMakeData.PieceSeed;

            // Get MovingBlock parameters
            MovingBlock_Parameters Params = (MovingBlock_Parameters)level.Style.FindParams(MovingBlock_AutoGen.Instance);

            Vector2 size = new Vector2(Params.Size.GetVal(pos), 0);
            switch (Params.Aspect)
            {
                case MovingBlock_Parameters.AspectType.Square: size.Y = size.X; break;
                case MovingBlock_Parameters.AspectType.Thin: size.Y = 30; break;
                case MovingBlock_Parameters.AspectType.Tall: size.Y = pos.Y - BL.Y + 200; break;
            }

            Vector2 offset = new Vector2(level.Rnd.Rnd.Next(0, 0), level.Rnd.Rnd.Next(0, 0) - size.Y);

            if (level.Style.BlockFillType == StyleData._BlockFillType.Spaceship)
            {
                offset += new Vector2(level.Rnd.Rnd.Next(0, 100), level.Rnd.Rnd.Next(0, 100));

                if (pos.X > level.CurMakeData.PieceSeed.End.X - 400) offset.X -= pos.X - level.CurMakeData.PieceSeed.End.X + 400;
                if (pos.X < level.CurMakeData.PieceSeed.Start.X + 400) offset.X += level.CurMakeData.PieceSeed.Start.X - pos.X + 400;

                TR.X -= 350;
                BL.X += 200;
            }

            MovingBlock mblock = (MovingBlock)level.Recycle.GetObject(ObjectType.MovingBlock, true);
            mblock.Init(pos + offset, size);

            mblock.Period = (int)Params.Period.GetVal(pos);

            mblock.Offset = level.Style.GetOffset(mblock.Period, pos,
                                                        level.Style.MovingBlock2OffsetType);


            float Displacement = Params.Range.GetVal(pos);
            SetMoveType(mblock, Displacement, Params.Motion, level.Rnd);

            // If the block is too low make sure it's path is horizontal
            if (pos.Y < BL.Y + 400)
                SetMoveType(mblock, Displacement, MovingBlock_Parameters.MotionType.Horizontal, level.Rnd);

            mblock.BlockCore.Decide_RemoveIfUnused(Params.KeepUnused.GetVal(pos), level.Rnd);
            mblock.BlockCore.GenData.EdgeSafety = GenData.Get(DifficultyParam.EdgeSafety, pos);

            if (level.Style.RemoveBlockOnOverlap)
                mblock.BlockCore.GenData.RemoveIfOverlap = true;

            Tools.EnsureBounds_X(mblock, TR, BL);
            
            level.AddBlock(mblock);

            return mblock;
        }
    }

    public partial class Level
    {
        public void CleanupMovingBlocks(Vector2 BL, Vector2 TR)
        {
            // Get MovingBlock parameters
            MovingBlock_Parameters Params = (MovingBlock_Parameters)Style.FindParams(MovingBlock_AutoGen.Instance);
        }
        public void AutoMovingBlocks()
        {
            // Get MovingBlock parameters
            MovingBlock_Parameters Params = (MovingBlock_Parameters)Style.FindParams(MovingBlock_AutoGen.Instance);
        }
    }
}
