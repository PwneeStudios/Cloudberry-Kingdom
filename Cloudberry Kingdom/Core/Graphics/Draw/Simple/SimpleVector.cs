using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;

namespace CloudberryKingdom
{
    public struct SimpleVector
    {
        public AnimationData AnimData;
        public MyOwnVertexFormat Vertex;
        public Vector2 Pos;

        public void RotateRight()
        {
            Tools.Swap(ref Pos.X, ref Pos.Y);
            Tools.Swap(ref Vertex.uv.X, ref Vertex.uv.X);
        }

        public void RotateLeft()
        {
            RotateRight();
            Pos.X *= -1;
            Vertex.uv.X *= -1;
        }

        public void Release()
        {
            AnimData.Release();
        }
    }
}