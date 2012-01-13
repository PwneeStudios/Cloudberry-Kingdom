using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;

namespace CloudberryKingdom.Levels
{
    public class NormalBlock_Parameters : AutoGen_Parameters
    {
        public Param KeepUnused;

        /// <summary>
        /// Whether to make normal blocks or not. Doesn't affect other fill weights.
        /// </summary>
        public bool Make = true;

        public struct _Special
        {
            /// <summary>
            /// Create a long, narrow hallway
            /// </summary>
            public bool Hallway;
        }
        public _Special Special;

        public float HallwayWidth = Unset.Float;

        public Wall MyWall;
        public Wall SetWall(LevelGeometry geometry)
        {
            MyWall = Wall.MakeWall(geometry);
            return MyWall;
        }

        public void SetHallway(LevelGeometry geometry)
        {
            Special.Hallway = true;
            if (geometry == LevelGeometry.Right)
            {
                DoStage1Fill = DoInitialPlats = DoFinalPlats = false;
                SetVal(ref HallwayWidth, 450);
            }
            else
                SetVal(ref HallwayWidth, 75);
        }

        /// <summary>
        /// Whether to do the standard Stage1Fill. False when certain special fills are set.
        /// </summary>
        public bool DoStage1Fill = true, DoInitialPlats = true, DoFinalPlats = true;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            KeepUnused = new Param(PieceSeed);
            if (level.DefaultHeroType is BobPhsxSpaceship)
            {
                //KeepUnused.SetVal(u => BobPhsxSpaceship.KeepUnused(u[Upgrade.]));
                KeepUnused.SetVal(u => .7f);
            }

            FillWeight = new Param(PieceSeed);
            FillWeight.SetVal(u =>
            {
                return 1; // u[Upgrade.Fall];
            });
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

        void SetHallwayBlockProperties(Block block)
        {
            block.StampAsUsed(0);
            block.BlockCore.NonTopUsed = true;
        }

        void Hallway(Level level, Vector2 BL, Vector2 TR)
        {
            if (level.CurMakeData.LevelSeed.MyGeometry == LevelGeometry.Right)
                HorizontalHallway(level, BL, TR);
            else
                VerticalHallway(level, BL, TR);
        }

        NormalBlock_Parameters GetParams(Level level)
        {
            return (NormalBlock_Parameters)level.Style.FindParams(NormalBlock_AutoGen.Instance);
        }

        void VerticalHallway(Level level, Vector2 BL, Vector2 TR)
        {
            var Params = GetParams(level);

            Vector2 Pos = BL;

            float MinRadius = Params.HallwayWidth;
            Block LeftMostBlock = Tools.ArgMax(level.Blocks.FindAll(match => match.Box.GetTR().X < -MinRadius), element => element.Core.Data.Position.X);
            Block RightMostBlock = Tools.ArgMin(level.Blocks.FindAll(match => match.Box.GetBL().X > MinRadius), element => element.Core.Data.Position.X);

            Vector2 pos1 = new Vector2(0, BL.Y - 700);
            Vector2 pos2 = new Vector2(0, TR.Y + 700);
            Vector2 right = new Vector2(RightMostBlock.Box.GetTR().X, 0);
            Vector2 left = new Vector2(LeftMostBlock.Box.GetBL().X, 0);
            NormalBlock block;

            // Right block
            block = NormalBlock_AutoGen.Instance.CreateCementBlockLine(level, pos1 + right, pos2 + right, true);
            SetHallwayBlockProperties(block);

            level.AddBlock(block);

            // Left block
            block = NormalBlock_AutoGen.Instance.CreateCementBlockLine(level, pos1 + left, pos2 + left, false);
            SetHallwayBlockProperties(block);

            level.AddBlock(block);
        }

        void HorizontalHallway(Level level, Vector2 BL, Vector2 TR)
        {
            NormalBlock_Parameters Params = (NormalBlock_Parameters)level.Style.FindParams(NormalBlock_AutoGen.Instance);

            Vector2 Pos = BL;

            Vector2 pos1 = new Vector2(BL.X - 900, BL.Y + 700);
            Vector2 pos2 = new Vector2(TR.X + 900, BL.Y + 700);
            Vector2 shift = new Vector2(0, Params.HallwayWidth);
            NormalBlock block;

            // Top block
            block = NormalBlock_AutoGen.Instance.CreateCementBlockLine(level, pos1 + shift, pos2 + shift);
            SetHallwayBlockProperties(block);

            level.AddBlock(block);

            // Bottom block
            block = NormalBlock_AutoGen.Instance.CreateCementBlockLine(level, pos1, pos2);
            SetHallwayBlockProperties(block);

            level.AddBlock(block);

            // Start door
            Vector2 pos = level.CurPiece.StartData[0].Position;
            Door door = level.PlaceDoorOnBlock(pos, block, true);

            door.MyBackblock.Extend(Side.Right, pos.X + 350);
            door.MyBackblock.Extend(Side.Left, pos.X - 800);
            door.MyBackblock.Core.MyTileSetType = level.CurMakeData.LevelSeed.MyTileSet;

            // Shift start position
            /*for (int i = 0; i < level.CurMakeData.NumInitialBobs; i++)
            {
                level.CurPiece.StartData[i].Position = door.Core.Data.Position;
            }*/
            Level.SpreadStartPositions(level.CurPiece, level.CurMakeData, door.Core.Data.Position, new Vector2(50, 0));

            // End door
            pos = new Vector2(TR.X, 0);
            door = level.PlaceDoorOnBlock(pos, block, true);
            MakeFinalDoor.SetFinalDoor(door, level, pos);

            door.MyBackblock.Extend(Side.Right, pos.X + 800);
            door.MyBackblock.Extend(Side.Left, pos.X - 350);
            door.MyBackblock.Core.MyTileSetType = level.CurMakeData.LevelSeed.MyTileSet;
            

            //    if (Geometry == LevelGeometry.Right)
            //    {
            //        pos.Y = (i == 0 ? level.MainCamera.BL.Y + shift : level.MainCamera.TR.Y - shift);
            //        for (pos.X = BL.X; pos.X < TR.X; pos.X += 200)
            //            inner();
            //    }
            //    else
            //    {
            //        pos.X = (i == 0 ? level.MainCamera.BL.X + shift : level.MainCamera.TR.X - shift);
            //        for (pos.Y = BL.Y; pos.Y < TR.Y; pos.Y += 200)
            //            inner();
            //    }
            //}
        }

        public override void PreFill_1(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_1(level, BL, TR);

            // Get NormalBlock parameters
            NormalBlock_Parameters Params = (NormalBlock_Parameters)level.Style.FindParams(NormalBlock_AutoGen.Instance);

            if (Params.Special.Hallway)
                Hallway(level, BL, TR);

            if (Params.MyWall != null)
                MakeWall(level);
        }

        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);
            level.AutoNormalBlocks();
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
            level.CleanupNormalBlocks(BL, TR);
        }

        public NormalBlock CreateCementBlock(Level level, Vector2 pos)
        {
            return CreateCementBlock(level, pos, new Vector2(75, 75));
        }

        public NormalBlock CreateCementBlockLine(Level level, Vector2 pos1, Vector2 pos2)
        {
            return CreateCementBlockLine(level, pos1, pos2, true);
        }

        /// <summary>
        /// Create a line of cement blocks.
        /// </summary>
        /// <param name="ShiftRight">When centering the blocks, whether to shift right or left.</param>
        public NormalBlock CreateCementBlockLine(Level level, Vector2 pos1, Vector2 pos2, bool ShiftRight)
        {
            Vector2 dif = (pos2 - pos1) / 150f;
            dif.X = (int)dif.X; dif.Y = (int)dif.Y;
            if (dif.X < 1) dif.X = 1;
            if (dif.Y < 1) dif.Y = 1;

            dif *= 150;

            Vector2 shift = dif / 2;
            if (!ShiftRight) shift.X *= -1;

            return CreateCementBlock(level, pos1 + shift, dif / 2);
        }
        public NormalBlock CreateCementBlock(Level level, Vector2 pos, Vector2 size)
        {
            NormalBlock block = (NormalBlock)level.Recycle.GetObject(ObjectType.NormalBlock, true);
            block.Init(pos, size);

            block.Core.MyTileSetType = TileSet.Cement;

            block.BlockCore.DeleteIfTopOnly = true;

            return block;
        }

        public override IObject CreateAt(Level level, Vector2 pos)
        {
            base.CreateAt(level, pos);

            StyleData style = level.Style;

            NormalBlock block = (NormalBlock)level.Recycle.GetObject(ObjectType.NormalBlock, true);
            BlockData core = block.BlockCore;
            block.Init(pos, new Vector2(50, 50));

            core.GenData.RemoveIfUnused = true;
            core.BlobsOnTop = true;

            if (style.RemoveBlockOnCol)
                core.GenData.RemoveIfUsed = true;

            if (style.RemoveBlockOnOverlap)
                core.GenData.RemoveIfOverlap = true;

            level.AddBlock(block);

            return block;
        }
        public override IObject CreateAt(Level level, Vector2 pos, Vector2 BL, Vector2 TR)
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

            switch (Style.BlockFillType)
            {
                case StyleData._BlockFillType.Regular:
                    size = new Vector2(
                        Tools.Rnd.Next(
                        GenData.Get(DifficultyParam.MinBoxSizeX, pos),
                        GenData.Get(DifficultyParam.MaxBoxSizeX, pos)),
                        Tools.Rnd.Next(
                        GenData.Get(DifficultyParam.MinBoxSizeY, pos),
                        GenData.Get(DifficultyParam.MaxBoxSizeY, pos)));
                    if (-2 * size.Y + pos.Y < BL.Y - 100) size.Y = (pos.Y - BL.Y + 100) / 2;

                    offset = new Vector2(Tools.Rnd.Next(0, 0), Tools.Rnd.Next(0, 0) - size.Y);

                    if (pos.X - size.X < BL.X) offset.X += BL.X - (pos.X - size.X);

                    block = (NormalBlock)level.Recycle.GetObject(ObjectType.NormalBlock, true);
                    block.Init(pos + offset, size);
                    block.Extend(Side.Bottom, block.Box.BL.Y - level.CurMakeData.PieceSeed.ExtraBlockLength);

                    break;

                case StyleData._BlockFillType.Spaceship:
                    size = new Vector2(100 * Tools.Rnd.Next(1, 4), 100 * Tools.Rnd.Next(1, 4));
                    offset = new Vector2(Tools.Rnd.Next(0, 0), Tools.Rnd.Next(0, 0) - size.Y);
                    if (pos.X > piece.End.X - 400) offset.X -= pos.X - piece.End.X + 400;
                    if (pos.X < piece.Start.X + 400) offset.X += piece.Start.X - pos.X + 400;

                    if (pos.X - size.X < BL.X) offset.X += BL.X - (pos.X - size.X);

                    block = CreateCementBlockLine(level, pos + offset - size, pos + offset + size);

                    break;

                default:
                    block = null;
                    break;
            }

            block.BlockCore.Decide_RemoveIfUnused(Params.KeepUnused.GetVal(pos));

            block.BlockCore.GenData.EdgeSafety = GenData.Get(DifficultyParam.EdgeSafety, pos);

            if (Style.RemoveBlockOnCol)
                block.BlockCore.GenData.RemoveIfUsed = true;

            if (Style.RemoveBlockOnOverlap)
                block.BlockCore.GenData.RemoveIfOverlap = true;

            level.AddBlock(block);

            return block;
        }
    }

    public partial class Level
    {
        public void CleanupNormalBlocks(Vector2 BL, Vector2 TR)
        {
            // Get NormalBlock parameters
            NormalBlock_Parameters Params = (NormalBlock_Parameters)Style.FindParams(NormalBlock_AutoGen.Instance);
        }
        public void AutoNormalBlocks()
        {
            // Get NormalBlock parameters
            NormalBlock_Parameters Params = (NormalBlock_Parameters)Style.FindParams(NormalBlock_AutoGen.Instance);
        }
    }
}
