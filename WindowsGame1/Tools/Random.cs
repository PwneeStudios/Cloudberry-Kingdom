using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class Rand
    {
        public Rand(int seed)
        {
            Rnd = new Random(seed);
        }

        public Random Rnd;

        public Vector2 RndPos(Vector2 BL, Vector2 TR)
        {
            return new Vector2(RndFloat(BL.X, TR.X), RndFloat(BL.Y, TR.Y));
        }

        public Vector2 RndVector2(float Width)
        {
            return new Vector2(RndFloat(-Width / 2, Width / 2), RndFloat(-Width / 2, Width / 2));
        }

        /// <summary>
        /// Randomly returns true or false
        /// </summary>
        public bool RndBool() { return RndBool(.5f); }
        public bool RndBool(float Chance)
        {
            float f = RndFloat();
            return f < Chance;

            //return RndFloat() < Chance;
        }

        /// <summary>
        /// Randomly chooses between 1 and -1
        /// </summary>
        /// <returns></returns>
        public int RndBit()
        {
            if (Rnd.NextDouble() > .5) return 1;
            else return -1;
        }

        /// <summary>
        /// Returns a random number between a and b, inclusive
        /// </summary>
        /// <param name="a">Lower bound</param>
        /// <param name="b">Upper bound</param>
        /// <returns></returns>
        public int RndInt(int a, int b)
        {
            return (int)Math.Min(b, RndFloat(a, b + 1));
        }

        /// <summary>
        /// Returns a random float between 0 and 1.
        /// </summary>
        public float RndFloat()
        {
            return RndFloat(0, 1);
        }

        public float RndFloat(float a, float b)
        {
            float dif = b - a;
            return (float)Rnd.NextDouble() * dif + a;
        }

        public float RndFloat(float a, float b, float spacing)
        {
            if (a > b) { float temp = a; a = b; b = temp; }

            int intervals = (int)((b - a) / spacing + .5f);
            spacing = (b - a) / intervals;

            return a + RndInt(0, intervals) * spacing;
        }

        public float RndFloat(Vector2 range)
        {
            return RndFloat(range.X, range.Y);
        }

        /// <summary>
        /// Returns a random direction with random magnitude between 0 and Length
        /// </summary>
        /// <param name="Length"></param>
        /// <returns></returns>
        public Vector2 RndDir(float Length)
        {
            return RndDir() * RndFloat(0, Length);
        }

        /// <summary>
        /// Returns a random direction of length 1
        /// </summary>
        /// <returns></returns>
        public Vector2 RndDir()
        {
            double Angle = Rnd.NextDouble() * 2 * Math.PI;
            return new Vector2((float)Math.Cos(Angle), (float)Math.Sin(Angle));
        }

        /// <summary>
        /// Choose a random element from the list
        /// </summary>
        public T Choose<T>(this List<T> list)
        {
            //if (list == null || list.Count == 0) return null;
            return list[RndInt(0, list.Count - 1)];
        }

        public int RandomSnap(int Range, int SnapPoints)
        {
            return RndInt(0, SnapPoints) * (Range / SnapPoints);
        }

        public int RndEnum<T>() where T : struct
        {
            return RndInt(0, Tools.Length<T>() - 1);
        }

        /// <summary>
        /// Scrambles the ordering of the given list in place.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public void Scramble<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int j = RndInt(0, list.Count - 1);

                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        public T RandomItem<T>(T[] list)
        {
            return list[RndInt(0, list.Length - 1)];
        }
        public T RandomItem<T>(List<T> list)
        {
            return list[RndInt(0, list.Count - 1)];
        }

        public int[] RndIndex(int Length, int NumIndices, bool[] Valid)
        {
            int[] Indices = new int[NumIndices];
            bool[] Taken = new bool[Length];

            for (int i = 0; i < NumIndices; i++)
                Indices[i] = -1;

            for (int i = 0; i < NumIndices; i++)
            {
                int Index = 0;
                int Count = Rnd.Next(100, 1000);
                int j;
                for (j = 0; j < Count; j++)
                {
                    Index = (Index + 1) % Length;
                    while ((Taken[Index] || !Valid[Index]) && !(_AllTaken(Taken, Valid, Length)))
                        Index = (Index + 1) % Length;
                }

                Taken[Index] = true;
                Indices[i] = Index;
            }

            return Indices;
        }

        /// <summary>
        /// Creates a new list with the same elements in a shuffled order;
        /// </summary>
        public List<T> Shuffle<T>(List<T> list)
        {
            List<T> shuffled = new List<T>();
            List<T> copy = new List<T>(list);

            for (int i = 0; i < list.Count; i++)
            {
                // Choose an element to add to the shuffled list
                int Index = RndInt(0, copy.Count - 1);
                shuffled.Add(copy[Index]);
                copy.Remove(copy[Index]);
            }

            return shuffled;
        }

        /// <summary>
        /// Returns a randomly chosen item from the items given
        /// </summary>
        public T ChooseOne<T>(params T[] choices)
        {
            return choices[RndInt(0, choices.Length - 1)];
        }

        /// <summary>
        /// Choose n elements from a list.
        /// </summary>
        public List<T> Choose<T>(List<T> list, int n)
        {
            List<T> chosen = new List<T>(list);
            for (int i = 0; i < list.Count - n; i++)
                chosen.RemoveAt(RndInt(0, chosen.Count - 1));

            return chosen;
        }
        public T Choose<T>(T[] list)
        {
            return Choose(new List<T>(list), 1)[0];
        }
        public T Choose<T>(List<T> list)
        {
            return Choose(list, 1)[0];
        }


        public int Choose(int[] LevelCutoff, int Level)
        {
            float[] Weights = new float[LevelCutoff.Length];
            for (int i = 0; i < LevelCutoff.Length; i++)
            {
                if (Level >= LevelCutoff[i])
                    Weights[i] = 1;
                else
                    Weights[i] = 0;
            }

            return Choose(Weights);
        }

        public int Choose(float[] Weights)
        {
            double rnd = Rnd.NextDouble() * Weights.Sum() * .999f;// .98f;
            float AddedWeight = 0;
            for (int i = 0; i < Weights.Length; i++)
            {
                AddedWeight += Weights[i];
                if (rnd < AddedWeight) return i;
            }

            return Weights.Length - 1;
        }

        public int ChooseNew(int Cur, int WeightA, int WeightB)
        {
            if (Cur == 0 && WeightB <= 0) return 0;
            if (Cur == 1 && WeightA <= 0) return 1;

            if (Cur == 0) return 1;
            if (Cur == 1) return 0;

            return 0;
        }

        public int ChooseNew(int Cur, int WeightA, int WeightB, int WeightC)
        {
            if (Cur == 0 && WeightB <= 0 && WeightC <= 0) return 0;
            if (Cur == 1 && WeightA <= 0 && WeightC <= 0) return 1;
            if (Cur == 2 && WeightA <= 0 && WeightB <= 0) return 2;

            if (Cur == 0 && WeightB == 0) return 2;
            if (Cur == 0 && WeightC == 0) return 1;
            if (Cur == 1 && WeightA == 0) return 2;
            if (Cur == 1 && WeightC == 0) return 0;
            if (Cur == 2 && WeightA == 0) return 1;
            if (Cur == 2 && WeightB == 0) return 0;


            if (Cur == 0) return Choose2(WeightB, 1, WeightC, 2);
            if (Cur == 1) return Choose2(WeightA, 0, WeightC, 2);
            if (Cur == 2) return Choose2(WeightA, 0, WeightB, 1);

            return -1;
        }

        public int Choose2(int WeightA, int ATag, int WeightB, int BTag)
        {
            int choice = Choose(WeightA, WeightB);
            if (choice == 0) return ATag;
            if (choice == 1) return BTag;

            return -1;
        }

        public int Choose(int WeightA, int WeightB)
        {
            int n = Rnd.Next(0, WeightA + WeightB);
            if (n < WeightA) return 0;
            else return 1;
        }

        public int Choose(int WeightA, int WeightB, int WeightC)
        {
            int n = Rnd.Next(0, WeightA + WeightB + WeightC);
            if (n < WeightA) return 0;
            else if (n < WeightA + WeightB) return 1;
            else return 2;
        }

        public int Choose(int WeightA, int WeightB, int WeightC, int WeightD)
        {
            int n = Rnd.Next(0, WeightA + WeightB + WeightC + WeightD);
            if (n < WeightA) return 0;
            else if (n < WeightA + WeightB) return 1;
            else if (n < WeightA + WeightB + WeightC) return 2;
            else return 3;
        }
    }
}