using Microsoft.Xna.Framework;

using System.IO;

using CloudberryKingdom;

namespace Drawing
{
    public static class WriteReadTools
    {
        public static Vector2 ReadVector2(this BinaryReader reader)
        {
            Vector2 vec = Vector2.Zero;
            ReadVector2(reader, ref vec);
            return vec;
        }

        /// <summary>
        /// Writes the index, ensuring non-negativity.
        /// </summary>
        public static void WriteSafeIndex(this BinaryWriter writer, int index)
        {
            if (index < 0) index = 0;
            writer.Write(index);
        }

        public static void Write(this BinaryWriter writer, Vector2 v)
        {
            WriteVector2(writer, v);
        }

        public static void WriteOneAnim(BinaryWriter writer, OneAnim anim)
        {
            if (anim.Data == null) writer.Write(-1);
            else
            {
                writer.Write(anim.Data.Length);
                for (int i = 0; i < anim.Data.Length; i++)
                    WriteVector2(writer, anim.Data[i]);
            }
        }

        public static void ReadOneAnim(BinaryReader reader, ref OneAnim anim)
        {
            int length = reader.ReadInt32();
            if (length == -1) anim.Data = null;
            else
            {
                anim.Data = new Vector2[length];
                for (int i = 0; i < length; i++)
                    ReadVector2(reader, ref anim.Data[i]);
            }
        }

        public static void WriteOneAnim(BinaryWriter writer, OneAnim_Texture anim)
        {
            if (anim.Data == null) writer.Write(-1);
            else
            {
                writer.Write(anim.Data.Length);
                for (int i = 0; i < anim.Data.Length; i++)
                {
                    writer.Write(anim.Data[i].Name);
                    writer.Write(Vector2.Zero); // Dummy
                    writer.Write(Vector2.Zero); // Dummy
                }
            }
        }

        public static void ReadOneAnim(BinaryReader reader, ref OneAnim_Texture anim)
        {
            int length = reader.ReadInt32();
            if (length == -1) anim.Data = null;
            else
            {
                anim.Data = new EzTexture[length];
                for (int i = 0; i < length; i++)
                {
                    string s = reader.ReadString();
                    anim.Data[i] = Tools.Texture(s);
                    reader.ReadVector2(); // Dummy
                    reader.ReadVector2(); // Dummy
                }
            }
        }


        public static void WriteVector2(BinaryWriter writer, Vector2 vec)
        {
            writer.Write(vec.X);
            writer.Write(vec.Y);
        }

        public static void ReadVector3(BinaryReader reader, ref Vector3 vec)
        {
            vec.X = reader.ReadSingle();
            vec.Y = reader.ReadSingle();
            vec.Z = reader.ReadSingle();
        }

        public static void WriteVector3(BinaryWriter writer, Vector3 vec)
        {
            writer.Write(vec.X);
            writer.Write(vec.Y);
            writer.Write(vec.Z);
        }

        public static void ReadVector2(BinaryReader reader, ref Vector2 vec)
        {
            vec.X = reader.ReadSingle();
            vec.Y = reader.ReadSingle();
        }

        public static void WriteColor(BinaryWriter writer, Color clr)
        {
            writer.Write(clr.R);
            writer.Write(clr.G);
            writer.Write(clr.B);
            writer.Write(clr.A);
        }

        public static void ReadColor(BinaryReader reader, ref Color clr)
        {
            clr.R = reader.ReadByte();
            clr.G = reader.ReadByte();
            clr.B = reader.ReadByte();
            clr.A = reader.ReadByte();
        }

        public static void WriteVertex(BinaryWriter writer, MyOwnVertexFormat ver)
        {
            WriteVector2(writer, ver.xy);
            WriteVector2(writer, ver.uv);
            WriteVector3(writer, new Vector3(0, 0, 0));
            WriteColor(writer, ver.Color);
        }

        public static void ReadVertex(BinaryReader reader, ref MyOwnVertexFormat ver)
        {
            ReadVector2(reader, ref ver.xy);
            ReadVector2(reader, ref ver.uv);
            Vector3 Trash = new Vector3(0, 0, 0);
            ReadVector3(reader, ref Trash);
            ReadColor(reader, ref ver.Color);
        }

        public static void WritePhsxData(BinaryWriter writer, PhsxData data)
        {
            WriteVector2(writer, data.Position);
            WriteVector2(writer, data.Velocity);
            WriteVector2(writer, data.Acceleration);
        }

        public static void ReadPhsxData(BinaryReader reader, ref PhsxData data)
        {
            ReadVector2(reader, ref data.Position);
            ReadVector2(reader, ref data.Velocity);
            ReadVector2(reader, ref data.Acceleration);
        }
    }
}