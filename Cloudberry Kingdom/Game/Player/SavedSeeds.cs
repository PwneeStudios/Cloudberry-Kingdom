using CloudberryKingdom.Bobs;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using CoreEngine;

namespace CloudberryKingdom
{
    public class SavedSeeds
    {
        public List<string> SeedStrings = new List<string>(50);

        public void SaveSeed(string seed, string name)
        {
            if (IsSeedValue(seed))
            {
                Awardments.CheckForAward_Save();

				// Prevent strings from being too long
				if (name.Length > 36)
				{
					name = name.Substring(0, 36);
				}

                SeedStrings.Add(seed + "name:" + name + ";");
                SaveGroup.SaveAll();
            }
        }

        /// <summary>
        /// A rough heuristic for determining if a string is a seed.
        /// Heuristic has no false negatives, but many false positives.
        /// </summary>
        /// <param name="seed">The string to check.</param>
        /// <returns>Whether the string is a seed (heuristically).</returns>
        public bool IsSeedValue(string seed)
        {
            if (!seed.Contains(";")) return false;
            if (!seed.Contains(":")) return false;
            return true;
        }

        #region WriteRead
        public void WriteChunk_5(BinaryWriter writer)
        {
            var chunk = new Chunk();
            chunk.Type = 5;

            foreach (string seed in SeedStrings)
                chunk.WriteSingle(0, seed);

            chunk.Finish(writer);
        }

        public void ReadChunk_5(Chunk ParentChunk)
        {
            foreach (Chunk chunk in ParentChunk)
            {
                switch (chunk.Type)
                {
                    case 0: SeedStrings.Add(chunk.ReadString()); break;
                }
            }
        }
        #endregion
    }
}