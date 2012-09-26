using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;

namespace CloudberryKingdom.Levels
{
    public class Pendulum_Parameters : AutoGen_Parameters
    {
        public Param Period, MaxAngle, KeepUnused, Size;

        public int[] MotionLevel = { 0,        1,          2,  3,     4,        5,       6   };
        public enum MotionType     { Vertical, Horizontal, AA, Cross, Straight, Cirlces, All };
        public MotionType Motion;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            float lvl = PieceSeed.MyUpgrades1[Upgrade.Pendulum];

            Size = new Param(PieceSeed);
            Size.SetVal(u => 230 - (230 - 50)/10f * u[Upgrade.Pendulum]);

            Motion = (MotionType)level.Rnd.Choose(MotionLevel, (int)lvl);

            KeepUnused = new Param(PieceSeed);
            if (level.DefaultHeroType is BobPhsxSpaceship)
            {
                KeepUnused.SetVal(u => BobPhsxSpaceship.KeepUnused(u[Upgrade.Pendulum]));
            }

            FillWeight = new Param(PieceSeed);
            FillWeight.SetVal(u => u[Upgrade.Pendulum]);

            Period = new Param(PieceSeed);
            Period.SetVal(u =>
            {
                float speed = 300 - 30 * u[Upgrade.Speed] + 45 * .5f * (u[Upgrade.Jump] + u[Upgrade.Pendulum]);
                return CoreMath.Restrict(40, 1000, speed);
            });

            MaxAngle = new Param(PieceSeed, u => Math.Min(750, 30 + 64 * .5f * (u[Upgrade.Jump] + u[Upgrade.Pendulum])));
        }
    }

    public sealed class Pendulum_AutoGen : AutoGen
    {
        static readonly Pendulum_AutoGen instance = new Pendulum_AutoGen();
        public static Pendulum_AutoGen Instance { get { return instance; } }

        static Pendulum_AutoGen() { }
        Pendulum_AutoGen()
        {
            Do_WeightedPreFill_1 = true;
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            Pendulum_Parameters Params = new Pendulum_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public override ObjectBase CreateAt(Level level, Vector2 pos, Vector2 BL, Vector2 TR)
        {
            base.CreateAt(level, pos, BL, TR);

            StyleData Style = level.Style;
            RichLevelGenData GenData = level.CurMakeData.GenData;
            PieceSeedData piece = level.CurMakeData.PieceSeed;

            // Get Pendulum parameters
            Pendulum_Parameters Params = (Pendulum_Parameters)level.Style.FindParams(Pendulum_AutoGen.Instance);

            Vector2 size = new Vector2(Params.Size.GetVal(pos), 40);

            //Vector2 offset = Vector2.Zero;
            Vector2 offset = new Vector2(0, -300);

            if (level.Style.BlockFillType == StyleData._BlockFillType.Spaceship)
            {
                offset += new Vector2(level.Rnd.Rnd.Next(0, 100), level.Rnd.Rnd.Next(0, 100));

                if (pos.X > level.CurMakeData.PieceSeed.End.X - 400) offset.X -= pos.X - level.CurMakeData.PieceSeed.End.X + 400;
                if (pos.X < level.CurMakeData.PieceSeed.Start.X + 400) offset.X += level.CurMakeData.PieceSeed.Start.X - pos.X + 400;

                TR.X -= 350;
                BL.X += 200;
            }

            Pendulum p = (Pendulum)level.Recycle.GetObject(ObjectType.Pendulum, true);
            p.Init(pos + offset, size, level);

            if (level.PieceSeed.GeometryType == LevelGeometry.Right)
                //p.PivotPoint.Y = level.MainCamera.TR.Y + 160;
                p.PivotPoint.Y = p.Pos.Y + 2000;
                //p.PivotPoint.Y = p.Pos.Y - 2000;
            else
            {
                p.PivotPoint.X = level.MainCamera.BL.X - 160;
                p.AddAngle = CoreMath.Radians(90);
                //mblock.PivotLocationType = Floater.PivotLocationTypes.LeftRight;
            }

            p.Period = (int)Params.Period.GetVal(pos);
            p.MaxAngle = (int)Params.MaxAngle.GetVal(pos);
            p.MaxAngle *= .001f;
            p.CalculateLength();

            p.Offset = level.Style.GetOffset(p.Period, pos,
                                                        level.Style.PendulumOffsetType);

            p.BlockCore.Decide_RemoveIfUnused(Params.KeepUnused.GetVal(pos), level.Rnd);
            p.BlockCore.GenData.EdgeSafety = GenData.Get(DifficultyParam.EdgeSafety, pos);

            if (level.Style.RemoveBlockOnOverlap)
                p.BlockCore.GenData.RemoveIfOverlap = true;

            Tools.EnsureBounds_X(p, TR, BL);
            
            level.AddBlock(p);

            return p;
        }
    }
}
