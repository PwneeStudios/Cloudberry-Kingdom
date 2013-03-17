using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CoreEngine;

namespace CloudberryKingdom.Levels
{
    public class ComputerRecording
    {
        static Stack<ComputerRecording> Pool;

		bool Released;
        bool IsFromPool;

        static void FillPool()
        {
            ComputerRecording record = new ComputerRecording();
            record.Init(7000, false);
            record.IsFromPool = true;
            Pool.Push(record);
        }

        public static void InitPool()
        {
            Pool = new Stack<ComputerRecording>();
            for (int i = 0; i < 20; i++)
                FillPool();
        }

        public static ComputerRecording FromPool()
        {
            if (Pool.Count == 0)
                FillPool();

            ComputerRecording popped = Pool.Pop();
            popped.Clean();

            return popped; 
        }

        public static void ToPool(ComputerRecording record)
        {
			if (record.Released) return;

			record.Released = true;
            Pool.Push(record);
        }




        public bool Sparse, SuperSparse;

        public BobInput[] Input;
        public int[] AutoJump;
        public Vector2[] AutoLocs, AutoVel;
		public uint[] Box_BL, Box_Size;
        public bool[] AutoOnGround;
        public int[] t;

        public void Shift(Vector2 shift)
        {
            if (AutoLocs == null) return;

            for (int i = 0; i < AutoLocs.Length; i++)
                AutoLocs[i] += shift;
        }

        public static int PareDivider = 4;

        public void Write(BinaryWriter writer, int Length)
        {
            for (int frame = 0; frame < Length; frame++)
            {
                writer.Write(AutoLocs[frame]);
                //writer.Write(BoxCenter[frame]);
                //writer.Write(AutoOnGround[frame]);
                Input[frame].Write(writer);
            }
        }

        public void Read(BinaryReader reader, int Length)
        {
            for (int frame = 0; frame < Length; frame++)
            {
                AutoLocs[frame] = reader.ReadVector2();
                //BoxCenter[frame] = reader.ReadVector2();
                //AutoOnGround[frame] = reader.ReadBoolean();
                Input[frame].Read(reader);
            }
        }

        public void Clean()
        {
			Released = false;

            for (int i = 0; i < Input.Length; i++) Input[i].Clean();
            for (int i = 0; i < AutoJump.Length; i++) AutoJump[i] = 0;
            for (int i = 0; i < AutoLocs.Length; i++) AutoLocs[i] = Vector2.Zero;
            for (int i = 0; i < AutoVel.Length; i++) AutoVel[i] = Vector2.Zero;
            for (int i = 0; i < Box_BL.Length; i++) Box_BL[i] = 0;
			for (int i = 0; i < Box_Size.Length; i++) Box_Size[i] = 0;
            for (int i = 0; i < AutoOnGround.Length; i++) AutoOnGround[i] = false;
            for (int i = 0; i < t.Length; i++) t[i] = 0;
        }

        public int Gett(int Step)
        {
            if (!SuperSparse)
                return t[Step];
            else
                return t[Step / PareDivider];
        }

        public Vector2 GetBoxCenter(int Step)
        {
			return Bob.UnpackIntIntoVector_Pos(Box_BL[Step]) + GetBoxSize(Step) / 2.0f;
        }

		public Vector2 GetBoxSize(int Step)
		{
			return Bob.UnpackIntIntoVector_Size(Box_Size[Step]);
		}

        public T[] PareDown<T>(T[] SourceArray)
        {
            int n = SourceArray.Length;
            int m = n / PareDivider;
            T[] ParedArray = new T[m];

            for (int i = 0; i < m; i++)
                ParedArray[i] = SourceArray[i * PareDivider];

            return ParedArray;
        }

        public void ConvertToSuperSparse()
        {
            Input = null;
            AutoJump = null;
            AutoLocs = null;
            AutoVel = null;
            AutoOnGround = null;

            if (!SuperSparse)
            {
				////BoxCenter = PareDown<Vector2>(BoxCenter);
				////Alive = PareDown<bool>(Alive);
				//t = PareDown<int>(t);
				//Anim = PareDown<byte>(Anim);

				//SuperSparse = true;
            }
        }

        public void Release()
        {
            if (IsFromPool)
            {
                ToPool(this);
                return;
            }

            AutoJump = null;
            AutoLocs = null;
            AutoOnGround = null;
            AutoVel = null;
            Box_Size = null;
			Box_BL = null;
            Input = null;
            t = null;
        }

        public void Init(int length) { Init(length, false); }
        public void Init(int length, bool Sparse)
        {
            this.Sparse = Sparse;

            Box_BL = new uint[length];
			Box_Size = new uint[length];
            AutoLocs = new Vector2[length];
            AutoVel = new Vector2[length];
            Input = new BobInput[length];

            t = new int[length];

            if (!Sparse)
            {                
                AutoJump = new int[length];                
                AutoOnGround = new bool[length];
            }
        }
    }
}