using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;

namespace CloudberryKingdom.Levels
{
    public class Ceiling_Parameters : AutoGen_Parameters
    {
        /// <summary>
        /// Whether to make the ceiling or not.
        /// </summary>
        public bool Make = true;

        public Param BufferSize;
        public VectorParam HeightRange, WidthRange;

        public struct _Special
        {
            /// <summary>
            /// One straight cement block for the ceiling.
            /// </summary>
            public bool CementCeiling;

            /// <summary>
            /// One straight block for the ceiling.
            /// </summary>
            public bool LongCeiling;
        }
        public _Special Special;

        public void SetLongCeiling() { Special.LongCeiling = true; Make = false; }
        public void SetCementCeiling() { Special.CementCeiling = true; Make = false; }

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            HeightRange = new VectorParam(PieceSeed, u =>
                Tools.DifficultyLerpRestrict19(new Vector2(100, 900), new Vector2(500, 1300), u[Upgrade.Ceiling]));

            WidthRange = new VectorParam(PieceSeed, u =>
                Tools.DifficultyLerpRestrict19(new Vector2(450, 1450), new Vector2(80, 80), u[Upgrade.Ceiling]));

            BufferSize = new Param(PieceSeed, u =>
                Tools.DifficultyLerpRestrict19(150, 10, u[Upgrade.Ceiling]));
        }
    }

    public sealed class Ceiling_AutoGen : AutoGen
    {
        static readonly Ceiling_AutoGen instance = new Ceiling_AutoGen();
        public static Ceiling_AutoGen Instance { get { return instance; } }

        static Ceiling_AutoGen() { }
        Ceiling_AutoGen()
        {
            Do_PreFill_1 = true;
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            Ceiling_Parameters Params = new Ceiling_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public void MakeLongCeiling(Level level, Vector2 BL, Vector2 TR)
        {
            NormalBlock cblock = (NormalBlock)level.Recycle[ObjectType.NormalBlock, true];
            Vector2 size = TR - BL + new Vector2(4000, 0);
            Vector2 pos = (TR + BL) / 2 + new Vector2(0, 500);
            cblock.Init(pos, size, level.MyTileSetInfo);

            cblock.Extend(Side.Bottom, TR.Y - 200);
            cblock.StampAsFullyUsed(0);
            cblock.BlockCore.CeilingDraw = true;

            level.AddBlock(cblock);
        }

        public void MakeCementCeiling(Level level, Vector2 BL, Vector2 TR)
        {
            Vector2 pos1 = new Vector2(BL.X - 800, TR.Y - 260);
            Vector2 pos2 = new Vector2(TR.X + 1600, TR.Y + 200);
            
            NormalBlock ceiling = NormalBlock_AutoGen.Instance.CreateCementBlockLine(level, pos1, pos2);
            ceiling.StampAsFullyUsed(0);

            ceiling.BlockCore.Finalized = true;

            level.AddBlock(ceiling);
        }

        public override void PreFill_1(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);

            Ceiling_Parameters Params = (Ceiling_Parameters)level.Style.FindParams(Ceiling_AutoGen.Instance);

            if (Params.Special.CementCeiling)
                MakeCementCeiling(level, BL, TR);

            if (Params.Special.LongCeiling)
                MakeLongCeiling(level, BL, TR);

            float MaxStartY = -100000;
            for (int i = 0; i < level.CurMakeData.NumInitialBobs; i++)
                MaxStartY = Math.Max(MaxStartY, level.CurMakeData.Start[i].Position.Y);

            level.MakeCeiling(BL, TR, MaxStartY);
        }
    }

    public partial class Level
    {
        public void MakeCeiling(Vector2 BL, Vector2 TR, float MaxStartY)
        {
            if (!TileSets.Get(MyTileSet).HasCeiling) return;

            Ceiling_Parameters Params = (Ceiling_Parameters)Style.FindParams(Ceiling_AutoGen.Instance);

            if (!Params.Make) return;

            // Ceiling
            Vector2 Pos = new Vector2(BL.X, TR.Y);
            NormalBlock cblock, lastblock = null;
            while (Pos.X < TR.X)
            {
                Vector2 size = new Vector2(
                        Params.WidthRange.RndFloat(Pos, Rnd),
                        //new Vector2(MyLevel.Rnd.RndFloat(
                        //Params.MinWidth.GetVal(Pos),
                        //Params.MaxWidth.GetVal(Pos)),
                        //MyLevel.Rnd.Rnd.Next(100, 900));
                        Params.HeightRange.RndFloat(Pos, Rnd));
                Vector2 offset = new Vector2(Rnd.Rnd.Next(0, 0) + size.X, -size.Y + 85);

                if (this.MyTileSet == TileSet.Castle)
                    size.X += 25;

                cblock = (NormalBlock)Recycle.GetObject(ObjectType.NormalBlock, true);
                // Initialize the size, make sure to modify it's width, since it's a ceiling block
                cblock.Init(Pos + offset, size - NormalBlockDraw.ModCeilingSize, MyTileSetInfo);
                cblock.Core.GenData.RemoveIfUnused = false;
                cblock.BlockCore.CeilingDraw = cblock.BlockCore.Ceiling = true;
                cblock.BlockCore.BlobsOnTop = false;
                cblock.BlockCore.TopLeftNeighbor = lastblock;
                lastblock = cblock;

                if (Pos.X < BL.X + 900)
                    cblock.Extend(Side.Bottom, Math.Max(cblock.Box.Current.BL.Y, MaxStartY + 250));
                cblock.Extend(Side.Left, Math.Max(cblock.Box.Current.BL.X, BL.X));
                cblock.Extend(Side.Right, Math.Min(cblock.Box.Current.TR.X, TR.X));
                cblock.Extend(Side.Top, TR.Y + 600 + CurMakeData.PieceSeed.ExtraBlockLength);
                
                if (cblock.Box.Current.Size.X > 35)
                    AddBlock(cblock);

                Pos.X += 2 * size.X;
            }
        }
    }
}
