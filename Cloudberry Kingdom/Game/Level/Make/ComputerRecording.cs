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
		public int[] BoxCenter_BL;
		public int[] BoxCenter_TR;
        public bool[] AutoOnGround;
        public byte[] Anim;
        public int[] t;
        public bool[] Alive;

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
            for (int i = 0; i < BoxCenter_BL.Length; i++) BoxCenter_BL[i] = 0;
			for (int i = 0; i < BoxCenter_TR.Length; i++) BoxCenter_TR[i] = 0;
            for (int i = 0; i < AutoOnGround.Length; i++) AutoOnGround[i] = false;
            for (int i = 0; i < Anim.Length; i++) Anim[i] = 0;
            for (int i = 0; i < t.Length; i++) t[i] = 0;
            for (int i = 0; i < Alive.Length; i++) Alive[i] = false;
        }

        public bool GetAlive(int Step)
        {
            //if (!SuperSparse)
                return Alive[Step];
            //else
            //    return Alive[Step / PareDivider];
        }

        public byte GetAnim(int Step)
        {
            if (!SuperSparse)
                return Anim[Step];
            else
                return Anim[Step / PareDivider];
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
			return Bob.UnpackIntIntoVector(BoxCenter_BL[Step]);

            //if (!SuperSparse)
			//if (true)
				//return BoxCenter[Step];
			//else
			//{
			//    //return BoxCenter[Step / PareDivider];

			//    int i1 = Step / PareDivider;
			//    int i2 = i1 + 1;
			//    if (i2 >= BoxCenter.Length) i2 = i1;

			//    Vector2 p1 = BoxCenter[i1];
			//    Vector2 p2 = BoxCenter[i2];

			//    float t = (float)(i1 * PareDivider - Step) / (float)PareDivider;
			//    return Vector2.Lerp(p2, p1, t);
			//}
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

            Alive = null;
            Anim = null;
            AutoJump = null;
            AutoLocs = null;
            AutoOnGround = null;
            AutoVel = null;
            BoxCenter_TR = null;
			BoxCenter_BL = null;
            Input = null;
            t = null;
        }

        public void Init(int length) { Init(length, false); }
        public void Init(int length, bool Sparse)
        {
            this.Sparse = Sparse;

            BoxCenter_BL = new int[length];
			BoxCenter_TR = new int[length];
            AutoLocs = new Vector2[length];
            AutoVel = new Vector2[length];
            Input = new BobInput[length];

            Anim = new byte[length];
            t = new int[length];

            Alive = new bool[length];

            if (!Sparse)
            {                
                AutoJump = new int[length];                
                AutoOnGround = new bool[length];
            }
        }
    }
}