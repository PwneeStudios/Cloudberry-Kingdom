using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using CoreEngine.Random;

namespace CloudberryKingdom
{
    public class BlockGroup
    {
        public Dictionary<int, List<PieceQuad>> Dict;
        public int[] Widths;

        public BlockGroup()
        {
            Dict = new Dictionary<int, List<PieceQuad>>();
        }

        public void Add(PieceQuad piece)
        {
            Add(piece.Pillar_Width, piece);
        }

        public void Add(int width, PieceQuad piece)
        {
            if (!Dict.ContainsKey(width))
                Dict.Add(width, new List<PieceQuad>());

            Dict[width].Add(piece);
        }

        public PieceQuad Choose(int width)
        {
            return Dict[SnapWidthUp(width)][0];
        }

        public PieceQuad Choose(int width, Rand rnd)
        {
            return Dict[width].Choose(rnd);
        }

        public void SortWidths()
        {
            var list = Dict.Keys.ToList();
            list.Sort();
            Widths = list.ToArray();
        }

        public int SnapWidthUp(float width)
        {
            return SnapWidthUp(width, Widths);
        }
        public void SnapWidthUp(ref Vector2 size)
        {
            size.X = SnapWidthUp(size.X, Widths);
        }

		public int SnapHeightUp(float Height)
		{
			return SnapWidthUp(Height, Widths);
		}
		public void SnapHeightUp(ref Vector2 size)
		{
			size.Y = SnapWidthUp(size.Y, Widths);
		}

        public static int SnapWidthUp(float width, int[] Widths)
        {
            if (Widths.Length == 0) return (int)width;

            int int_width = 0;

            int_width = Widths[Widths.Length - 1];
            for (int i = 0; i < Widths.Length; i++)
            {
                if (width < Widths[i])
                {
                    int_width = Widths[i];
                    break;
                }
            }

            width = int_width;

            return int_width;
        }
    }
}