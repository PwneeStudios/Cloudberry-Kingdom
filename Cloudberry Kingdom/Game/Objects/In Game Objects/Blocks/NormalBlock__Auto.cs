using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.InGameObjects;

namespace CloudberryKingdom.Levels
{
    public class NormalBlock_Parameters : AutoGen_Parameters
    {
        public Param KeepUnused;

        /// <summary>
        /// Whether to make normal blocks or not. Doesn't affect other fill weights.
        /// </summary>
        public bool Make = true;

        public bool CustomWeight = false;

        public Wall MyWall;
        public Wall SetWall(LevelGeometry geometry)
        {
            MyWall = Wall.MakeWall(geometry);
            return MyWall;
        }

        /// <summary>
        /// Whether to do the standard Stage1Fill. False when certain special fills are set.
        /// </summary>
        public bool DoStage1Fill = true, DoInitialPlats = true, DoFinalPlats = true;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            var u = PieceSeed.u;

            KeepUnused = new Param(PieceSeed);
            if (level.DefaultHeroType is BobPhsxSpaceship)
            {
                KeepUnused.SetVal(.7f);
            }

            FillWeight = new Param(PieceSeed);
            FillWeight.SetVal(1);
        }
    }

    public sealed class NormalBlock_AutoGen : AutoGen
    {
        static readonly NormalBlock_AutoGen instance = new NormalBlock_AutoGen();
        public static NormalBlock_AutoGen Instance { get { return instance; } }

        static NormalBlock_AutoGen() { }
        NormalBlock_AutoGen()
        {
            Do_PreFill_1 = true;
            Do_WeightedPreFill_1 = true;
            //Generators.AddGenerator(this);
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            NormalBlock_Parameters Params = new NormalBlock_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        void MakeWall(Level level)
        {
            var Params = GetParams(level);

            level.AddBlock(Params.MyWall);
            Params.MyWall = null;
        }

        NormalBlock_Parameters GetParams(Level level)
        {
            return (NormalBlock_Parameters)level.Style.FindParams(NormalBlock_AutoGen.Instance);
        }

        public override void PreFill_1(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_1(level, BL, TR);

            // Get NormalBlock parameters
            NormalBlock_Parameters Params = (NormalBlock_Parameters)level.Style.FindParams(NormalBlock_AutoGen.Instance);

            if (Params.MyWall != null)
                MakeWall(level);
        }

        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
        }

        public override ObjectBase CreateAt(Level level, Vector2 pos)
        {
            base.CreateAt(level, pos);

            StyleData style = level.Style;

            // Get NormalBlock parameters
            NormalBlock_Parameters Params = (NormalBlock_Parameters)style.FindParams(NormalBlock_AutoGen.Instance);
            if (!Params.Make) return null;

            NormalBlock block = (NormalBlock)level.Recycle.GetObject(ObjectType.NormalBlock, true);
            BlockData core = block.BlockCore;
            block.Init(pos, new Vector2(50, 50), level.MyTileSetInfo);

            core.GenData.RemoveIfUnused = true;
            core.BlobsOnTop = true;

            if (style.RemoveBlockOnCol)
                core.GenData.RemoveIfUsed = true;

            if (style.RemoveBlockOnOverlap)
                core.GenData.RemoveIfOverlap = true;

            level.AddBlock(block);

            return block;
        }
        public override ObjectBase CreateAt(Level level, Vector2 pos, Vector2 BL, Vector2 TR)
        {
            base.CreateAt(level, pos, BL, TR);

            StyleData Style = level.Style;
            RichLevelGenData GenData = level.CurMakeData.GenData;
            PieceSeedData piece = level.CurMakeData.PieceSeed;

            // Get NormalBlock parameters
            NormalBlock_Parameters Params = (NormalBlock_Parameters)Style.FindParams(NormalBlock_AutoGen.Instance);

            if (!Params.Make) return null;

            NormalBlock block;
            Vector2 size = Vector2.Zero;
            Vector2 offset = Vector2.Zero;

            bool EnsureBoundsAfter = true;
            switch (Style.BlockFillType)
            {
                case StyleData._BlockFillType.Regular:
                    size = new Vector2(
                        level.Rnd.Rnd.Next(
                        GenData.Get(DifficultyParam.MinBoxSizeX, pos),
                        GenData.Get(DifficultyParam.MaxBoxSizeX, pos)),
                        level.Rnd.Rnd.Next(
                        GenData.Get(DifficultyParam.MinBoxSizeY, pos),
                        GenData.Get(DifficultyParam.MaxBoxSizeY, pos)));
                    if (-2 * size.Y + pos.Y < BL.Y - 100) size.Y = (pos.Y - BL.Y + 100) / 2;

                    offset = new Vector2(level.Rnd.Rnd.Next(0, 0), level.Rnd.Rnd.Next(0, 0) - size.Y);

                    if (pos.X - size.X < BL.X) offset.X += BL.X - (pos.X - size.X);

                    block = (NormalBlock)level.Recycle.GetObject(ObjectType.NormalBlock, true);
                    block.Init(pos + offset, size, level.MyTileSetInfo);
                    block.Extend(Side.Bottom, block.Box.BL.Y - level.CurMakeData.PieceSeed.ExtraBlockLength);

                    block.Core.GenData.MyOverlapPreference = GenerationData.OverlapPreference.RemoveLowerThanMe;

                    break;

                case StyleData._BlockFillType.TopOnly:
                    size = new Vector2(
                        level.Rnd.Rnd.Next(
                        GenData.Get(DifficultyParam.MinBoxSizeX, pos),
                        GenData.Get(DifficultyParam.MaxBoxSizeX, pos)),
                        50);

                    if (pos.X - size.X < BL.X) offset.X += BL.X - (pos.X - size.X);

                    block = (NormalBlock)level.Recycle.GetObject(ObjectType.NormalBlock, true);
                    block.Init(pos + offset, size, level.MyTileSetInfo);
                    block.MakeTopOnly();

                    break;

                case StyleData._BlockFillType.Invertable:
                    if (pos.Y < level.MainCamera.Pos.Y - 0)
                    {
                        size = new Vector2(
                            level.Rnd.Rnd.Next(
                            GenData.Get(DifficultyParam.MinBoxSizeX, pos),
                            GenData.Get(DifficultyParam.MaxBoxSizeX, pos)),
                            level.Rnd.Rnd.Next(
                            GenData.Get(DifficultyParam.MinBoxSizeY, pos),
                            GenData.Get(DifficultyParam.MaxBoxSizeY, pos)));
                        if (-2 * size.Y + pos.Y < BL.Y - 100) size.Y = (pos.Y - BL.Y + 100) / 2;

                        offset = new Vector2(level.Rnd.Rnd.Next(0, 0), level.Rnd.Rnd.Next(0, 0) - size.Y);

                        if (pos.X - size.X < BL.X) offset.X += BL.X - (pos.X - size.X);

                        block = (NormalBlock)level.Recycle.GetObject(ObjectType.NormalBlock, true);
                        block.Init(pos + offset, size, level.MyTileSetInfo);
                        block.Extend(Side.Bottom, block.Box.BL.Y - level.CurMakeData.PieceSeed.ExtraBlockLength);
                        
                        block.Core.GenData.MyOverlapPreference = GenerationData.OverlapPreference.RemoveLowerThanMe;
                    }
                    else
                    {
                        size = new Vector2(
                            level.Rnd.Rnd.Next(
                            GenData.Get(DifficultyParam.MinBoxSizeX, pos),
                            GenData.Get(DifficultyParam.MaxBoxSizeX, pos)),
                            level.Rnd.Rnd.Next(
                            GenData.Get(DifficultyParam.MinBoxSizeY, pos),
                            GenData.Get(DifficultyParam.MaxBoxSizeY, pos)));
                        if (2 * size.Y + pos.Y > TR.Y + 100) size.Y = (TR.Y - pos.Y + 100) / 2;

                        offset = new Vector2(level.Rnd.Rnd.Next(0, 0), level.Rnd.Rnd.Next(0, 0) + size.Y);

                        if (pos.X - size.X < BL.X) offset.X += BL.X - (pos.X - size.X);

                        block = (NormalBlock)level.Recycle.GetObject(ObjectType.NormalBlock, true);
                        block.Init(pos + offset, size, level.MyTileSetInfo);
                        block.Extend(Side.Top, block.Box.TR.Y + level.CurMakeData.PieceSeed.ExtraBlockLength);

                        block.Invert = true;
                        block.BlockCore.BlobsOnTop = false;

                        block.Core.GenData.MyOverlapPreference = GenerationData.OverlapPreference.RemoveHigherThanMe;
                    }

                    break;

                case StyleData._BlockFillType.Sideways:
                    if (pos.X < level.MainCamera.Pos.X - 0)
                    {
                        size = new Vector2(
                            level.Rnd.Rnd.Next(
                            GenData.Get(DifficultyParam.MinBoxSizeY, pos),
                            GenData.Get(DifficultyParam.MaxBoxSizeY, pos)),
                            level.Rnd.Rnd.Next(
                            GenData.Get(DifficultyParam.MinBoxSizeX, pos),
                            GenData.Get(DifficultyParam.MaxBoxSizeX, pos)));

                        offset = new Vector2(level.Rnd.Rnd.Next(0, 0) - size.X, level.Rnd.Rnd.Next(0, 0));

                        block = (NormalBlock)level.Recycle.GetObject(ObjectType.NormalBlock, true);
                        block.BlockCore.MyOrientation = PieceQuad.Orientation.RotateRight;
                        block.Init(pos + offset, size, level.MyTileSetInfo);
                        block.Extend(Side.Left, block.Box.BL.X - level.CurMakeData.PieceSeed.ExtraBlockLength);
                    }
                    else
                    {
                        size = new Vector2(
                            level.Rnd.Rnd.Next(
                            GenData.Get(DifficultyParam.MinBoxSizeY, pos),
                            GenData.Get(DifficultyParam.MaxBoxSizeY, pos)),
                            level.Rnd.Rnd.Next(
                            GenData.Get(DifficultyParam.MinBoxSizeX, pos),
                            GenData.Get(DifficultyParam.MaxBoxSizeX, pos)));

                        offset = new Vector2(level.Rnd.Rnd.Next(0, 0) + size.X, level.Rnd.Rnd.Next(0, 0));

                        block = (NormalBlock)level.Recycle.GetObject(ObjectType.NormalBlock, true);
                        block.BlockCore.MyOrientation = PieceQuad.Orientation.RotateLeft;
                        block.Init(pos + offset, size, level.MyTileSetInfo);
                        block.Extend(Side.Right, block.Box.TR.X + level.CurMakeData.PieceSeed.ExtraBlockLength);
                    }
                    block.Core.GenData.NoBottomShift = block.Core.GenData.NoMakingTopOnly = true;
                    EnsureBoundsAfter = false;
                    break;

                default:
                    block = null;
                    break;
            }

            block.BlockCore.Decide_RemoveIfUnused(Params.KeepUnused.GetVal(pos), level.Rnd);
            if (block.Invert && block.BlockCore.BlobsOnTop) block.BlockCore.BlobsOnTop = false;


            block.BlockCore.GenData.EdgeSafety = GenData.Get(DifficultyParam.EdgeSafety, pos);

            if (level.CurMakeData.BlocksAsIs)
            {
                block.Core.GenData.NoMakingTopOnly = true;
                block.Core.GenData.NoBottomShift = true;
            }

            // Ensure bounds
            if (EnsureBoundsAfter)
            {
                float CurTrX = block.Box.GetTR().X;
                if (CurTrX > TR.X + 250)
                    block.Move(new Vector2(TR.X + 250 - CurTrX, 0));
            }

            if (Style.RemoveBlockOnCol)
                block.BlockCore.GenData.RemoveIfUsed = true;

            if (Style.RemoveBlockOnOverlap)
                block.BlockCore.GenData.RemoveIfOverlap = true;

            level.AddBlock(block);

            return block;
        }
    }
}
