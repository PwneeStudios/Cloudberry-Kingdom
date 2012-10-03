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
        public void WriteChunk_4(BinaryWriter writer)
        {
            var chunk = new Chunk();
            chunk.Type = 0;

            chunk.Finish(writer);
        }

        public void ReadChunk_4(Chunk chunk)
        {
        }

        /*
        public void Write(BinaryWriter writer)
        {
            // Stats
            foreach (FieldInfo info in GetType().GetFields())
            {
                if (info.FieldType == typeof(int))
                    writer.Write((int)info.GetValue(this));
            }

            // Deaths
            for (int i = 0; i < DeathsBy.Length; i++)
                writer.Write(DeathsBy[i]);
        }

        public void Read(BinaryReader reader)
        {
            // Version
            int LoadedVersion = reader.ReadInt32();

            // Stats
            foreach (FieldInfo info in GetType().GetFields())
            {
                if (info.FieldType == typeof(int))
                    info.SetValue(this, reader.ReadInt32());
            }

            // Deaths
            for (int i = 0; i < DeathsBy.Length; i++)
                DeathsBy[i] = reader.ReadInt32();
        }*/

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

        public int Score, Coins, Blobs, CoinsSpentAtShop;
        public int TotalCoins, TotalBlobs;

        public int CoinPercentGotten
        {
            get
            {
                if (TotalCoins == 0) return 100;
                else
                    return (int)(100f * (float)Coins / (float)TotalCoins);
            }
        }

        public int Levels, Checkpoints, Jumps, Berries;
        
        public int[] DeathsBy;

        public int TotalDeaths { get { return DeathsBy[(int)Bob.BobDeathType.Total]; } }
        
        /// <summary>
        /// Total time and time spent moving for the final attempt.
        /// </summary>
        public int FinalTimeSpentNotMoving, FinalTimeSpent;
        
        public int TimeAlive;

        public string LifeExpectancy
        {
            get
            {
                TimeSpan time = new TimeSpan(0, 0, LifeExpectancy_Frames / 61);
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

        public PlayerStats()
        {
            DeathsBy = new int[Tools.Length<Bob.BobDeathType>()];
            
            Clean();
        }
    }
}