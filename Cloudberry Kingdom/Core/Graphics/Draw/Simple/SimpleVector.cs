using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;

namespace CloudberryKingdom
{
    public struct SimpleVector : IReadWrite
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

        static string[] _bits_to_save = new string[] { "Pos", "Vertex" };
        public void WriteCode(string prefix, StreamWriter writer)
        {
            Tools.WriteFieldsToCode(this, prefix, writer, _bits_to_save);
        }
        public void Write(StreamWriter writer)
        {
            //Tools.WriteFields(this, writer, _bits_to_save);
        }
        public void Read(StreamReader reader)
        {
            //this = (SimpleVector)Tools.ReadFields(this, reader);
        }
    }
}