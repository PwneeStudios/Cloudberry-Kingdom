using CloudberryKingdom.Bobs;
using System;
using System.IO;
using System.Reflection;

using CoreEngine;

namespace CloudberryKingdom
{
    public enum StatGroup { Lifetime, Game, Level, Temp, Campaign };
    public class PlayerStats
    {
        #region WriteRead
        public void WriteChunk_4(BinaryWriter writer)
        {
            var chunk = new Chunk();
            chunk.Type = 4;

            chunk.WriteSingle(0, Score);
            chunk.WriteSingle(1, Coins);
            chunk.WriteSingle(2, Blobs);
            chunk.WriteSingle(3, CoinsSpentAtShop);
            chunk.WriteSingle(4, TotalCoins);
            chunk.WriteSingle(5, TotalBlobs);
            chunk.WriteSingle(6, Levels);
            chunk.WriteSingle(7, Checkpoints);
            chunk.WriteSingle(8, Jumps);
            chunk.WriteSingle(10, TimeAlive);

            for (int i = 0; i < DeathsBy.Length; i++)
                WriteDeathChunk_9(chunk, i);

            chunk.Finish(writer);
        }

        public void ReadChunk_4(Chunk ParentChunk)
        {
            foreach (Chunk chunk in ParentChunk)
            {
                switch (chunk.Type)
                {
                    case 0: chunk.ReadSingle(ref Score); break;
                    case 1: chunk.ReadSingle(ref Coins); break;
                    case 2: chunk.ReadSingle(ref Blobs); break;
                    case 3: chunk.ReadSingle(ref CoinsSpentAtShop); break;
                    case 4: chunk.ReadSingle(ref TotalCoins); break;
                    case 5: chunk.ReadSingle(ref TotalBlobs); break;
                    case 6: chunk.ReadSingle(ref Levels); break;
                    case 7: chunk.ReadSingle(ref Checkpoints); break;
                    case 8: chunk.ReadSingle(ref Jumps); break;
                    case 10: chunk.ReadSingle(ref TimeAlive); break;

                    case 9: ReadDeathChunk_9(chunk); break;
                }
            }
        }

        void WriteDeathChunk_9(Chunk ParentChunk, int Index)
        {
            var chunk = new Chunk();
            chunk.Type = 9;

            chunk.Write(Index);
            chunk.Write(DeathsBy[Index]);

            chunk.Finish(ParentChunk);
        }

        void ReadDeathChunk_9(Chunk chunk)
        {
            int Index = chunk.ReadInt();
            if (Index >= 0 && Index < DeathsBy.Length)
                DeathsBy[Index] = chunk.ReadInt();
        }
        #endregion

        #region Persistent Stats
        public int Score, Coins, Blobs, CoinsSpentAtShop;
        public int TotalCoins, TotalBlobs;

        public int Levels, Checkpoints, Jumps, Berries;

        public int[] DeathsBy;

        public int TimeAlive;
        #endregion

        // Add all fields and elements of arrays to the corresponding field in this instance.
        public PlayerStats Absorb(PlayerStats stats)
        {
            foreach (FieldInfo info in GetType().GetFields())
            {
                if (info.FieldType == typeof(int))
                    info.SetValue(this, (int)info.GetValue(this) + (int)info.GetValue(stats));
            }

            for (int i = 0; i < DeathsBy.Length; i++)
                DeathsBy[i] += stats.DeathsBy[i];

            return this;
        }

        // Set all fields and all elements of arrays to 0.
        public void Clean()
        {
            foreach (FieldInfo info in GetType().GetFields())
            {
                if (info.FieldType == typeof(int)) info.SetValue(this, 0);
            }

            for (int i = 0; i < DeathsBy.Length; i++)
                DeathsBy[i] = 0;
        }

        /// <summary>
        /// Total time and time spent moving for the final attempt.
        /// </summary>
        public int FinalTimeSpentNotMoving, FinalTimeSpent;

        #region DerivedStats
        public string LifeExpectancy
        {
            get
            {
                TimeSpan time = new TimeSpan(0, 0, LifeExpectancy_Frames / 60);
                if (time.Hours == 0)
                    return string.Format("{0}:{1:00}", time.Minutes, time.Seconds);
                else
                    return string.Format("{0}:{1}:{2:00}", time.Hours, time.Minutes, time.Seconds);
            }
        }

        public int LifeExpectancy_Frames
        {
            get
            {
                if (DeathsBy[(int)Bob.BobDeathType.Total] <= 0)
                    return TimeAlive;
                else
                    return (int)((float)TimeAlive / (float)(1 + DeathsBy[(int)Bob.BobDeathType.Total]));
            }
        }

        public int TotalDeaths { get { return DeathsBy[(int)Bob.BobDeathType.Total]; } }
        public int CoinPercentGotten
        {
            get
            {
                if (TotalCoins == 0) return 100;
                else
                    return (int)(100f * (float)Coins / (float)TotalCoins);
            }
        }
        #endregion

        public PlayerStats()
        {
            DeathsBy = new int[Tools.Length<Bob.BobDeathType>()];
            
            Clean();
        }
    }
}