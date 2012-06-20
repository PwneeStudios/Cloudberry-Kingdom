using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.IO;
using CloudberryKingdom;

namespace Drawing
{    
    public struct MyOwnVertexFormat : IVertexType, IReadWrite
    {
        public Vector2 xy;
        public Vector2 uv;
        public Color Color;

        public MyOwnVertexFormat(Vector2 XY, Vector2 UV, Color color, Vector3 depth)
        {
            this.xy = XY;
            this.uv = UV;
            this.Color = color;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 2, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 4, VertexElementFormat.Color, VertexElementUsage.Color, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }

        public void Write(StreamWriter writer)
        {
            Tools.WriteFields(this, writer, "xy", "uv", "Color");
        }
        public void Read(StreamReader reader)
        {
            Tools.ReadFields(this, reader);
        }
    }
}
