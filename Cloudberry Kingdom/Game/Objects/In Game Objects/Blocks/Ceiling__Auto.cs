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

        public enum Style { Normal, SkipOne, Sparse, Random, Length };
        public Style MyStyle;

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

			//MyStyle = (Style)level.Rnd.RndEnum<Style>();
			MyStyle = (Style)level.Rnd.RndInt(0, (int)Style.Length - 1);

            if (PieceSeed.u[Upgrade.Ceiling] <= 0)
                Make = false;

            HeightRange = new VectorParam(PieceSeed, u =>
                DifficultyHelper.InterpRestrict19(new Vector2(100, 900), new Vector2(500, 1300), u[Upgrade.Ceiling]));

            WidthRange = new VectorParam(PieceSeed, u =>
                DifficultyHelper.InterpRestrict19(new Vector2(450, 1450), new Vector2(80, 80), u[Upgrade.Ceiling]));

            BufferSize = new Param(PieceSeed, u =>
                DifficultyHelper.InterpRestrict19(150, 10, u[Upgrade.Ceiling]));
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
            //NormalBlock cblock = (NormalBlock)level.Recycle[ObjectType.NormalBlock, true];
            NormalBlock cblock = (NormalBlock)level.Recycle.GetObject(ObjectType.NormalBlock, true);
            Vector2 size = TR - BL + new Vector2(4000, 0);
            Vector2 pos = (TR + BL) / 2 + new Vector2(0, 500);
            cblock.Init(pos, size, level.MyTileSetInfo);

            cblock.Extend(Side.Bottom, TR.Y - 200);
            cblock.StampAsFullyUsed(0);
            cblock.BlockCore.CeilingDraw = true;

            level.AddBlock(cblock);
        }

        public override void PreFill_1(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);

            Ceiling_Parameters Params = (Ceiling_Parameters)level.Style.FindParams(Ceiling_AutoGen.Instance);

            float MaxStartY = -100000;
            for (int i = 0; i < level.CurMakeData.NumInitialBobs; i++)
                MaxStartY = Math.Max(MaxStartY, level.CurMakeData.Start[i].Position.Y);

            MakeCeiling(level, BL, TR, MaxStartY);
        }

        public void MakeCeiling(Level level, Vector2 BL, Vector2 TR, float MaxStartY)
        {
            if (!level.MyTileSet.HasCeiling) return;

            Ceiling_Parameters Params = (Ceiling_Parameters)level.Style.FindParams(Ceiling_AutoGen.Instance);

            if (!Params.Make) return;

            // Ceiling
            Vector2 Pos = new Vector2(BL.X, TR.Y);
            NormalBlock cblock, lastblock = null;
            while (Pos.X < TR.X)
            {
                Vector2 size = new Vector2(
                        Params.WidthRange.RndFloat(Pos, level.Rnd),
                        Params.HeightRange.RndFloat(Pos, level.Rnd));

                cblock = (NormalBlock)level.Recycle.GetObject(ObjectType.NormalBlock, true);
                cblock.Init(Vector2.Zero, size, level.MyTileSetInfo);
                size = cblock.Box.Current.Size;

                Vector2 offset = new Vector2(size.X, 0);

                // Make sure the ceiling block isn't past the left or right edges of the level.
                if (Pos.X + offset.X + size.X > TR.X)
                    size.X = TR.X - Pos.X - offset.X;
                if (Pos.X + offset.X - size.X < BL.X)
                    size.X = Pos.X + offset.X - BL.X;
                if (size.X < 50) { Pos.X += 100; continue; }
                offset = new Vector2(size.X, 0);

                // Initialize the size, make sure to modify it's width, since it's a ceiling block
                cblock.Init(Pos + offset, size, level.MyTileSetInfo);
                cblock.Core.GenData.RemoveIfUnused = false;
                cblock.Core.GenData.KeepIfUnused = true;
                cblock.BlockCore.CeilingDraw = cblock.BlockCore.Ceiling = true;
                cblock.BlockCore.BlobsOnTop = false;
                cblock.BlockCore.TopLeftNeighbor = lastblock;
                cblock.BlockCore.CeilingDraw = true;
                lastblock = cblock;

                if (Pos.X < BL.X + 900)
                    cblock.Extend(Side.Bottom, Math.Max(cblock.Box.Current.BL.Y, MaxStartY + 250));
                if (cblock.Box.Current.Size.X < 40) { Pos.X += 100; continue; }
                cblock.Extend(Side.Top, TR.Y + 600 + level.CurMakeData.PieceSeed.ExtraBlockLength + 1000);

                if (cblock.Box.Current.Size.X > 35)
                    level.AddBlock(cblock);

                switch (Params.MyStyle)
                {
                    case Ceiling_Parameters.Style.Normal:
                        Pos.X += 2 * cblock.Box.Current.Size.X;
                        break;
                    case Ceiling_Parameters.Style.SkipOne:
                        Pos.X += 2 * 2 * cblock.Box.Current.Size.X;
                        break;
                    case Ceiling_Parameters.Style.Sparse:
                        Pos.X += level.Rnd.RndInt(7, 8) * cblock.Box.Current.Size.X;
                        break;
                    case Ceiling_Parameters.Style.Random:
                        Pos.X += level.Rnd.RndInt(2, 8) * cblock.Box.Current.Size.X;
                        break;
                }
                Pos.X += 20;
            }
        }
    }
}
