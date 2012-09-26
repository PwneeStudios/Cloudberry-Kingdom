using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom.Levels
{
    public class BouncyBlock_Parameters : AutoGen_Parameters
    {
        public Param Speed, Size, SideDampening, KeepUnused, EdgeSafety;

        public struct _Special
        {
            /// <summary>
            /// A special fill type. Creates a ceiling and ground of bouncy blocks.
            /// </summary>
            public bool Hallway;
        }
        public _Special Special;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            KeepUnused = new Param(PieceSeed);
            if (level.DefaultHeroType is BobPhsxSpaceship)
            {
                KeepUnused.SetVal(u => BobPhsxSpaceship.KeepUnused(u[Upgrade.BouncyBlock]));
            }

            FillWeight = new Param(PieceSeed, u => u[Upgrade.BouncyBlock]);

            Speed = new Param(PieceSeed);
            Speed.SetVal(u =>
            {
                return DifficultyHelper.Interp(45, 60, u[Upgrade.BouncyBlock]);
                //return DifficultyHelper.Interp(45, 70, u[Upgrade.BouncyBlock]);
            });

            SideDampening = new Param(PieceSeed);
            SideDampening.SetVal(u =>
            {
                return DifficultyHelper.Interp159(.55f, .83f, 1.2f, u[Upgrade.BouncyBlock]);
            });

            Size = new Param(PieceSeed);
            Size.SetVal(u =>
            {
                return Math.Max(75, 105 - 1.85f * u[Upgrade.BouncyBlock]);
            });

            EdgeSafety = new Param(PieceSeed);
            EdgeSafety.SetVal(u =>
            {
                return Math.Max(.01f, DifficultyHelper.Interp159(.4f, .3f, .05f, u[Upgrade.BouncyBlock]));
            });
        }
    }

    public sealed class BouncyBlock_AutoGen : AutoGen
    {
        static readonly BouncyBlock_AutoGen instance = new BouncyBlock_AutoGen();
        public static BouncyBlock_AutoGen Instance { get { return instance; } }

        static BouncyBlock_AutoGen() { }
        BouncyBlock_AutoGen()
        {
            Do_PreFill_1 = true;

            Do_WeightedPreFill_1 = true;
            //Generators.AddGenerator(this);
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            BouncyBlock_Parameters Params = new BouncyBlock_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        void SetHallwaysBlockProperties(BouncyBlock block, Level level)
        {
            block.Core.GenData.Used = true;

            block.Init(block.Core.Data.Position, new Vector2(150, 150), 80, level);
        }
        void Hallway(Level level, Vector2 BL, Vector2 TR)
        {
            BL.X = level.FillBL.X;

            float x = BL.X;

            while (x < TR.X)
            {
                BouncyBlock block;
                block = (BouncyBlock)CreateAt(level, new Vector2(x, TR.Y - 300));
                SetHallwaysBlockProperties(block, level);
                block = (BouncyBlock)CreateAt(level, new Vector2(x, BL.Y + 300));
                SetHallwaysBlockProperties(block, level);

                x += 2 * block.Box.Current.Size.X;
            }
        }

        public override void PreFill_1(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_1(level, BL, TR);

            BouncyBlock_Parameters Params = (BouncyBlock_Parameters)level.Style.FindParams(BouncyBlock_AutoGen.Instance);

            if (Params.Special.Hallway)
                Hallway(level, BL, TR);
        }

        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);
            level.AutoBouncyBlocks();
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
            level.CleanupBouncyBlocks(BL, TR);
        }

        public override ObjectBase CreateAt(Level level, Vector2 pos, Vector2 BL, Vector2 TR)        
        {
            base.CreateAt(level, pos, BL, TR);

            return CreateAt(level, pos);
        }

        public override ObjectBase CreateAt(Level level, Vector2 pos)        
        {
            base.CreateAt(level, pos);

            StyleData Style = level.Style;
            RichLevelGenData GenData = level.CurMakeData.GenData;
            PieceSeedData piece = level.CurMakeData.PieceSeed;

            // Get BouncyBlock parameters
            BouncyBlock_Parameters Params = (BouncyBlock_Parameters)piece.Style.FindParams(BouncyBlock_AutoGen.Instance);

            BouncyBlock bblock;
            float Width = Params.Size.GetVal(pos);
            Vector2 size = new Vector2(Width, Width);
            Vector2 offset = new Vector2(level.Rnd.Rnd.Next(0, 0), level.Rnd.Rnd.Next(0, 0) - size.Y);
            float speed = Params.Speed.GetVal(pos);
            float SideDampening = Params.SideDampening.GetVal(pos);

            if (Style.BlockFillType == StyleData._BlockFillType.Spaceship)
            {
                offset += new Vector2(level.Rnd.Rnd.Next(0, 100), level.Rnd.Rnd.Next(0, 100));
                if (pos.X > piece.End.X - 400) offset.X -= pos.X - piece.End.X + 400;
                if (pos.X < piece.Start.X + 400) offset.X += piece.Start.X - pos.X + 400;
            }

            bblock = (BouncyBlock)level.Recycle.GetObject(ObjectType.BouncyBlock, true);
            bblock.Init(pos + offset, size, speed, level);
            bblock.SideDampening = SideDampening;
            bblock.BlockCore.BlobsOnTop = true;

            bblock.BlockCore.Decide_RemoveIfUnused(Params.KeepUnused.GetVal(pos), level.Rnd);
            bblock.BlockCore.GenData.EdgeSafety = Params.EdgeSafety.GetVal(pos) * size.X;

            if (piece.Style.RemoveBlockOnOverlap)
                bblock.BlockCore.GenData.RemoveIfOverlap = true;

            level.AddBlock(bblock);

            return bblock;
        }
    }

    public partial class Level
    {
        public void CleanupBouncyBlocks(Vector2 BL, Vector2 TR)
        {
            // Get BouncyBlock parameters
            BouncyBlock_Parameters Params = (BouncyBlock_Parameters)Style.FindParams(BouncyBlock_AutoGen.Instance);
        }
        public void AutoBouncyBlocks()
        {
            // Get BouncyBlock parameters
            BouncyBlock_Parameters Params = (BouncyBlock_Parameters)Style.FindParams(BouncyBlock_AutoGen.Instance);
        }
    }
}
