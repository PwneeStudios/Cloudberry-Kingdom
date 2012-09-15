using System.IO;
using Microsoft.Xna.Framework;

using Drawing;

namespace CloudberryKingdom
{
    public class FloatRectangle
    {
        public Vector2 TR, BL, Center, Size;

        public FloatRectangle() { }

        public FloatRectangle(Vector2 center, Vector2 size)
        {
            Set(center, size);
        }

        public void Clone(FloatRectangle A)
        {
            Center = A.Center;
            Size = A.Size;
            TR = A.TR;
            BL = A.BL;
        }

        public void Write(BinaryWriter writer)
        {
            WriteReadTools.WriteVector2(writer, TR);
            WriteReadTools.WriteVector2(writer, BL);
            WriteReadTools.WriteVector2(writer, Center);
            WriteReadTools.WriteVector2(writer, Size);

            // Trash =(
            WriteReadTools.WriteVector2(writer, Vector2.Zero);
            WriteReadTools.WriteVector2(writer, Vector2.Zero);
            writer.Write(false);
            writer.Write(false);
        }
        public void Read(BinaryReader reader)
        {
            WriteReadTools.ReadVector2(reader, ref TR);
            WriteReadTools.ReadVector2(reader, ref BL);
            WriteReadTools.ReadVector2(reader, ref Center);
            WriteReadTools.ReadVector2(reader, ref Size);

            // Trash =(
            Vector2 trash = Vector2.Zero;
            WriteReadTools.ReadVector2(reader, ref trash);
            WriteReadTools.ReadVector2(reader, ref trash);
            reader.ReadBoolean();
            reader.ReadBoolean();
        }

        public float BoxSize()
        {
            return Tools.BoxSize(TR, BL);
        }

        public Vector2 TL() { return new Vector2(BL.X, TR.Y); }
        public Vector2 BR() { return new Vector2(TR.X, BL.Y); }

        public Rectangle GetIntRectangle()
        {
            return new Rectangle((int)(Center.X - Size.X), (int)(Center.Y - Size.Y), 2 * (int)Size.X, 2 * (int)Size.Y);
        }

        public void Set(Vector2 center, Vector2 size)
        {
            Center = center;
            Size = size;

            CalcBounds();
        }

        public void CalcBounds()
        {
            TR = Center + Size;
            BL = Center - Size;
        }

        public void FromBounds()
        {
            Center = (TR + BL) / 2;
            Size = (TR - BL) / 2;
        }

        public void Scale(float scale)
        {
            Size *= scale;
            CalcBounds();
        }        
    }
}