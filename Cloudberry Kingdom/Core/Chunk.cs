using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

using CloudberryKingdom;

namespace CoreEngine
{
    public class Chunks : IEnumerable<Chunk>
    {
        public static Chunks Get(byte[] Data)
        {
            return new Chunks(Data);
        }

        byte[] Data;
        public Chunks(byte[] Data)
        {
            this.Data = Data;
        }

        public IEnumerator<Chunk> GetEnumerator()
        {
            return new ChunkEnumerator(Data);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ChunkEnumerator(Data);
        }
    }

    public class ChunkEnumerator : IEnumerator<Chunk>
    {
        byte[] Data;
        int Position = 0;

        public ChunkEnumerator(byte[] Data)
        {
            this.Data = Data;
        }

        Chunk _Current;
        public Chunk Current { get { return _Current; } }

        public bool MoveNext()
        {
            if (Position >= Data.Length) return false;

            int Type = BitConverter.ToInt32(Data, Position);
            int Length = BitConverter.ToInt32(Data, Position + 4);

            var _Current = new Chunk(Length + 8);
            _Current.Write(Data, Position, Length + 8);

            return true;
        }

        public void Reset()
        {
            Position = 0;
            _Current = null;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public void Dispose()
        {
            return;
        }
    }

    public class Chunk
    {
        public int Type = -1;
        public int Length = 0;

        byte[] Buffer;
        int Position = 4 + 4;

        public Chunk()
        {
            Initialize(100);
        }
        
        public Chunk(int Capacity)
        {
            Initialize(Capacity);
        }

        void Initialize(int Capacity)
        {
            Buffer = new byte[Capacity];
            Length = Capacity;
        }

        /// <summary>
        /// Double the size of the buffer without losing any data.
        /// </summary>
        public void Expand()
        {
            var OldBuffer = Buffer;
            Buffer = new byte[OldBuffer.Length * 2];

            for (int i = 0; i < Position; i++)
                Buffer[i] = OldBuffer[i];

            Length = Buffer.Length;
        }

        void SetTypeAndLength()
        {
            int Size = Position;
            Position = 0;

            Write(BitConverter.GetBytes(Type));
            Write(BitConverter.GetBytes(Size));

            Position = Size;
        }

        public void Finish(BinaryWriter writer)
        {
            SetTypeAndLength();
            writer.Write(Buffer, 0, Position);
        }

        public void Write(byte[] Bytes)
        {
            // Make sure we don't write past the end of the buffer
            while (Position + Bytes.Length >= Buffer.Length)
                Expand();

            // Write the bytes
            for (int i = Position; i < Bytes.Length; i++)
                Buffer[i] = Bytes[Position - i];
            
            Position += Bytes.Length;
        }
        public void Write(byte[] Bytes, int StartIndex, int BytesToCopy)
        {
            for (int i = StartIndex; i < StartIndex + BytesToCopy; i++)
                Buffer[Position + i] = Bytes[i];

            Position += BytesToCopy;
        }

        public void Write(bool val)
        {
            Write(BitConverter.GetBytes(val));
            Position += 1;
        }
        public void Write(int val)
        {
            Write(BitConverter.GetBytes(val));
            Position += 4;
        }
        public void Write(float val)
        {
            Write(BitConverter.GetBytes(val));
            Position += 4;
        }
        public void Write(string val)
        {
            int StringLength = System.Text.Encoding.ASCII.GetByteCount(val);
            
            Write(StringLength);
            Write(System.Text.Encoding.ASCII.GetBytes(val));
        }

        public bool ReadBool()
        {
            bool val = BitConverter.ToBoolean(Buffer, Position);
            Position += 1;
            
            return val;
        }
        public int ReadInt()
        {
            int val = BitConverter.ToInt32(Buffer, Position);
            Position += 4;

            return val;
        }
        public float ReadFloat()
        {
            float val = BitConverter.ToSingle(Buffer, Position);
            Position += 4;

            return val;
        }
        public string ReadString()
        {
            int StringLength = ReadInt();
            var val = System.Text.Encoding.ASCII.GetString(Buffer, Position, StringLength);

            Position += StringLength;

            return val;
        }

        public void ReadSingle(ref bool val)
        {
            val = ReadBool();
        }
        public void ReadSingle(ref int val)
        {
            val = ReadInt();
        }
        public void ReadSingle(ref float val)
        {
            val = ReadFloat();
        }
        public void ReadSingle(ref string val)
        {
            val = ReadString();
        }
        public void ReadSingle(ref Keys val)
        {
            val = (Keys)ReadInt();
        }

        public static void WriteSingle(BinaryWriter writer, int type, int val)
        {
            var chunk = new Chunk();
            chunk.Type = type;
            
            chunk.Write(val);

            chunk.Finish(writer);
        }
        public static void WriteSingle(BinaryWriter writer, int type, bool val)
        {
            var chunk = new Chunk();
            chunk.Type = type;

            chunk.Write(val);

            chunk.Finish(writer);
        }
        public static void WriteSingle(BinaryWriter writer, int type, float val)
        {
            var chunk = new Chunk();
            chunk.Type = type;

            chunk.Write(val);

            chunk.Finish(writer);
        }
        public static void WriteSingle(BinaryWriter writer, int type, string val)
        {
            var chunk = new Chunk();
            chunk.Type = type;

            chunk.Write(val);

            chunk.Finish(writer);
        }
        public static void WriteSingle(BinaryWriter writer, int type, Keys val)
        {
            var chunk = new Chunk();
            chunk.Type = type;

            chunk.Write((int)val);

            chunk.Finish(writer);
        }
    }
}