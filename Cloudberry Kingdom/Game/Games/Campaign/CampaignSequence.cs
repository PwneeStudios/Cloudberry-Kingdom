using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
#if XBOX
    public class Tuple<T1, T2> 
    { 
       public T1 Item1 { get; set; } 
       public T2 Item2 { get; set; } 

       public Tuple(T1 item1, T2 item2) 
       { 
          Item1 = item1; 
          Item2 = item2; 
       } 
    }
#endif

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
            FileStream stream = File.Open("Content\\Campaign\\CampaignList.txt", FileMode.Open, FileAccess.Read, FileShare.None);
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

                int space = line.IndexOf(' ');
                string identifier, data;
                if (space > 0)
                {
                    identifier = line.Substring(0, space);
                    data = line.Substring(space + 1);
                }
                else
                {
                    identifier = line;
                    data = "";
                }

                switch (identifier)
                {
                    case "chapter":
                        var chapter = int.Parse(data);
                        ChapterStart.AddOrOverwrite(chapter, count);
                        break;

                    case "movie":
                        SpecialLevel.AddOrOverwrite(count, new Tuple<string,string>(identifier, data));
                        Seeds.Add(null);
                        count++;

                        break;

                    case "end":
                        SpecialLevel.AddOrOverwrite(count, new Tuple<string, string>(identifier, null));
                        Seeds.Add(null);
                        count++;

                        break;

                    case "seed":
                        var seed = data;
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
            {
                var seed = base.GetSeed(Index);

                seed.PostMake += PostMakeCampaign;

                return seed;
            }
        }

        static void PostMakeCampaign(Level level)
        {
            if (level.MyLevelSeed.MyGameType == ActionGameData.Factory) return;

            level.MyGame.OnCoinGrab += OnCoinGrab;
            level.MyGame.OnCompleteLevel += OnCompleteLevel;

            // Level Title only
            //var title = new LevelTitle(string.Format("{1} {0}", level.MyLevelSeed.LevelNum, Localization.WordString(Localization.Words.Level)));
            
            // Level Title plus Hero Name
            var title = new LevelTitle(string.Format("{1} {0}", level.MyLevelSeed.LevelNum, Localization.WordString(Localization.Words.Level)));
            level.MyGame.AddGameObject(title);

            if (!level.MyLevelSeed.NewHero)
            {
                var hero_title = new LevelTitle(string.Format("{0}", Localization.WordString(level.DefaultHeroType.Name)));
                hero_title.Shift(new Vector2(0, -180));
                level.MyGame.AddGameObject(hero_title);
            }

            level.MyGame.AddGameObject(new GUI_CampaignScore(), new GUI_Level(level.MyLevelSeed.LevelNum));

            level.MyGame.MyBankType = GameData.BankType.Campaign;
        }

        static void OnCoinGrab(ObjectBase obj)
        {
            foreach (var player in PlayerManager.ExistingPlayers)
                player.CampaignCoins++;
        }

        static void OnCompleteLevel(Level level)
        {
            foreach (var player in PlayerManager.ExistingPlayers)
                player.CampaignLevel = Math.Max(player.CampaignLevel, level.MyLevelSeed.LevelNum);
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