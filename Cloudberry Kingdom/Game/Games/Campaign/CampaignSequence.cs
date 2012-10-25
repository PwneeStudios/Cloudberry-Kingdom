using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class CampaignSequence : LevelSequence
    {
        static readonly CampaignSequence instance = new CampaignSequence();
        public static CampaignSequence Instance { get { return instance; } }

        Dictionary<int, int> ChapterStart = new Dictionary<int, int>();
        Dictionary<int, Tuple<string, string>> SpecialLevel = new Dictionary<int, Tuple<string, string>>();

        public override void Start(int Chapter)
        {
            int StartLevel = ChapterStart[Chapter];

            base.Start(StartLevel);
        }

        protected override void MakeSeedList()
        {
            Seeds.Add(null);

            Tools.UseInvariantCulture();
            FileStream stream = File.Open("Content\\Campaign\\Campaign.txt", FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader reader = new StreamReader(stream);

            String line;

            int level = 1, count = 1;

            line = reader.ReadLine();
            while (line != null)
            {
                line = Tools.RemoveComment_DashStyle(line);
                if (line == null || line.Length <= 1)
                {
                    line = reader.ReadLine();
                    continue;
                }

                var bits = line.Split(' ');
                switch (bits[0])
                {
                    case "chapter":
                        var chapter = int.Parse(bits[1]);
                        ChapterStart.AddOrOverwrite(chapter, count);
                        break;

                    case "movie":
                        SpecialLevel.AddOrOverwrite(count, new Tuple<string,string>(bits[0], bits[1]));
                        Seeds.Add(null);
                        count++;

                        break;

                    case "end":
                        SpecialLevel.AddOrOverwrite(count, new Tuple<string, string>(bits[0], null));
                        Seeds.Add(null);
                        count++;

                        break;

                    case "seed":
                        var seed = bits[1];
                        seed += string.Format("level:{0};", level);

                        Seeds.Add(seed);
                        count++; level++;

                        break;
                }

                line = reader.ReadLine();
            }
            
            reader.Close();
            stream.Close();            
        }

        static LevelSeedData MakeActionSeed(Action<Level> SeedAction)
        {
            var seed = new LevelSeedData();
            seed.MyGameType = ActionGameData.Factory;

            seed.PostMake = SeedAction;

            return seed;
        }

        public override LevelSeedData GetSeed(int Index)
        {
            if (SpecialLevel.ContainsKey(Index))
            {
                var data = SpecialLevel[Index];
                switch (data.Item1)
                {
                    case "end":
                        return MakeActionSeed(EndAction);

                    case "movie":
                        return MakeActionSeed(MakeWatchMovieAction(data.Item2));

                    default:
                        return null;
                }
            }
            else
                return base.GetSeed(Index);
        }

        static Action<Level> MakeWatchMovieAction(string movie)
        {
            return level =>
            {
                MainVideo.StartVideo_CanSkipIfWatched_OrCanSkipAfterXseconds(movie, 1.5f);

                ((ActionGameData)level.MyGame).Done = true;
            };
        }

        static void EndAction(Level level)
        {
            level.MyGame.EndGame(false);
        }

        protected CampaignSequence()
        {
        }
    }
}