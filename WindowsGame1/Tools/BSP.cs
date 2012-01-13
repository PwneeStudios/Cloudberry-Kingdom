#define EMBEDDEDLOAD

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class BSP_Line
    {
        public List<List<IObject>> Grid;

        Vector2 BL, TR, Size;
        int N;

        public BSP_Line(List<IObject> list)
        {
            // Calculate limits
            BL = list[0].Core.Data.Position;
            TR = list[0].Core.Data.Position;

            foreach (IObject obj in list)
            {
                Vector2 pos = obj.Core.Data.Position;

                TR = Vector2.Max(TR, pos);
                BL = Vector2.Min(BL, pos);
            }

            Size = TR - BL;
            N = (int)(Size.X / 2000);

            // Allocate the grid
            Grid = new List<List<IObject>>(N + 1);

            for (int i = 0; i < N + 1; i++)
                Grid.Add(new List<IObject>(200));

            // Fill the grid
            foreach (IObject obj in list)
            {
                Vector2 pos = obj.Core.Data.Position;

                int GridIndex = GetIndex(pos);

                Grid[GridIndex].Add(obj);
            }
        }

        public int GetIndex(Vector2 pos)
        {
            return (int)(N * (pos.X - BL.X) / Size.X);
        }

        public void GetIndexBounds(Vector2 pos, ref int LowerIndex, ref int UppderIndex)
        {
            int Index = GetIndex(pos);

            LowerIndex = Math.Max(0, Index - 1);
            UppderIndex = Math.Min(N, Index + 1);
        }
    }
}