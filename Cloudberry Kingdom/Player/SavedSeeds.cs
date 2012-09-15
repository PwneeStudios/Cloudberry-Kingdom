using CloudberryKingdom.Bobs;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace CloudberryKingdom
{
    public class SavedSeeds
    {
        public static int version = 1;

        public List<string> SeedStrings = new List<string>(50);

        public void SaveSeed(string seed, string name)
        {
            if (IsSeedValue(seed))
            {
                SeedStrings.Add(seed + "name:" + name + ";");
                SaveGroup.SaveAll();
            }
        }

        public bool IsSeedValue(string seed)
        {
            if (!seed.Contains(";")) return false;
            if (!seed.Contains(":")) return false;
            return true;
        }

        public void Write(BinaryWriter writer)
        {
            // Version
            writer.Write(version);

            writer.Write(SeedStrings.Count);
            for (int i = 0; i < SeedStrings.Count; i++)
                writer.Write(SeedStrings[i]);
        }

        public void Read(BinaryReader reader)
        {
            // Version
            int LoadedVersion = reader.ReadInt32();

            int n = reader.ReadInt32();
            SeedStrings.Clear();
            for (int i = 0; i < n; i++)
                SeedStrings.Add(reader.ReadString());
        }
    }
}