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
        int DataLength;

        public ChunkEnumerator(byte[] Data)
        {
            this.Data = Data;
            DataLength = Data.Length;
        }

        public ChunkEnumerator(byte[] Data, int Start, int DataLength)
        {
            this.Data = Data;
            this.Position = Start;
            this.DataLength = DataLength;
        }

        Chunk _Current;
        public Chunk Current { get { return _Current; } }

        public bool MoveNext()
        {
            if (Position >= DataLength) return false;

            int Type = BitConverter.ToInt32(Data, Position);
            int Length = BitConverter.ToInt32(Data, Position + 4);

            if (Type < 0) throw new Exception("Chunk type must be non-negative. Are you loading a non-chunked file?");
            if (Length <= 0) throw new Exception("Chunk length must be strictly positive. Are you loading a non-chunked file?");

            _Current = new Chunk(Length);
            _Current.Copy(Data, Position, Length);
            _Current.Type = Type;
            _Current.Length = Length;

            Position += Length;

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

    public class Chunk : IEnumerable<Chunk>
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

        public IEnumerator<Chunk> GetEnumerator()
        {
            return new ChunkEnumerator(Buffer, 8, Length);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ChunkEnumerator(Buffer, 8, Length);
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
        public void Finish(Chunk ParentChunk)
        {
            SetTypeAndLength();
            ParentChunk.Write(Buffer, 0, Position);
        }

        void EnsureRoom(int Size)
        {
            // Make sure we don't write past the end of the buffer
            while (Position + Size >= Buffer.Length)
                Expand();
        }

        public void Write(byte[] Bytes)
        {
            EnsureRoom(Bytes.Length);

            // Write the bytes
            for (int i = Position; i < Position + Bytes.Length; i++)
                Buffer[i] = Bytes[i - Position];
            
            Position += Bytes.Length;
        }
        public void Write(byte[] Bytes, int StartIndex, int BytesToCopy)
        {
            EnsureRoom(BytesToCopy);

            for (int i = StartIndex; i < StartIndex + BytesToCopy; i++)
                Buffer[Position + i - StartIndex] = Bytes[i];

            Position += BytesToCopy;
        }

        /// <summary>
        /// Copy bytes from another chunk. This will overwrite the Type and Length bytes for this chunk.
        /// </summary>
        /// <param name="Bytes">Bytes from the other chunk</param>
        /// <param name="StartIndex">StartIndex in the other chunk's buffer.</param>
        /// <param name="BytesToCopy">Number of bytes to copy from the other chunk's buffer.</param>
        public void Copy(byte[] Bytes, int StartIndex, int BytesToCopy)
        {
            Position = 0;
            EnsureRoom(BytesToCopy);

            Position = 8;

            for (int i = StartIndex; i < StartIndex + BytesToCopy; i++)
                Buffer[i - StartIndex] = Bytes[i];
        }

        public void Write(bool val)
        {
            Write(BitConverter.GetBytes(val));
        }
        public void Write(int val)
        {
            Write(BitConverter.GetBytes(val));
        }
        public void Write(float val)
        {
            Write(BitConverter.GetBytes(val));
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

        public void WriteSingle(int type, int val)
        {
            var chunk = new Chunk();
            chunk.Type = type;

            chunk.Write(val);

            chunk.Finish(this);
        }
        public void WriteSingle(int type, bool val)
        {
            var chunk = new Chunk();
            chunk.Type = type;

            chunk.Write(val);

            chunk.Finish(this);
        }
        public void WriteSingle(int type, float val)
        {
            var chunk = new Chunk();
            chunk.Type = type;

            chunk.Write(val);

            chunk.Finish(this);
        }
        public void WriteSingle(int type, string val)
        {
            var chunk = new Chunk();
            chunk.Type = type;

            chunk.Write(val);

            chunk.Finish(this);
        }
    }
}