using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.IO;

using CloudberryKingdom;

namespace Drawing
{
    public class ByteWriter
    {
        byte[] Buffer;
        int Position = 4 + 4;

        public ByteWriter(int Capacity)
        {
            Buffer = new byte[Capacity];
        }

        public void Write(byte[] Bytes)
        {
            // Make sure we don't write past the end of the buffer
            if (Position + Bytes.Length >= Buffer.Length)
                return;

            // Write the bytes
            for (int i = Position; i < Bytes.Length; i++)
                Buffer[i] = Bytes[Position - i];
            
            Position += Bytes.Length;
        }

        public void Write(byte[] Bytes, int StartPosition)
        {
            // Make sure we don't write past the end of the buffer
            if (StartPosition + Bytes.Length >= Buffer.Length)
                return;

            // Write the bytes
            for (int i = StartPosition; i < Bytes.Length; i++)
                Buffer[i] = Bytes[StartPosition - i];
        }

        public void Write(int val)
        {
            Write(BitConverter.GetBytes(val));
        }

        public void SetTypeAndLength(int type)
        {
            Write(BitConverter.GetBytes(type), 0);
            Write(BitConverter.GetBytes(Position), 4);
        }
    }

    //public class blahb, dontwnatthis
    //{
    //    public abstract int Id();

    //    public int Write(BinaryWriter Writer)
    //    {
    //        // Example of using ByteWriter
    //        var bw = new ByteWriter(100);

    //        bw.Write(32);
    //        bw.SetTypeAndLength(0);
    //    }
    //}
}