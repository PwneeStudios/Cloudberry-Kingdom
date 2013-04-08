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

    public class Challenge_StoryMode : Challenge
    {
        static readonly Challenge_StoryMode instance = new Challenge_StoryMode();
        public static Challenge_StoryMode Instance { get { return instance; } }

        protected Challenge_StoryMode()
        {
            GameId_Level = GameTypeId = 7777;
            MenuName = Name = Localization.Words.StoryMode;
        }

        public override LevelSeedData GetSeed(int Index)
        {
            return null;
        }
    }

    public class CampaignSequence : LevelSequence
    {
        static readonly CampaignSequence instance = new CampaignSequence();
        public static CampaignSequence Instance { get { return instance; } }

		public static Localization.Words[] ChapterName = {	Localization.Words.Chapter1, Localization.Words.Chapter2, Localization.Words.Chapter3,
															Localization.Words.Chapter4, Localization.Words.Chapter5, Localization.Words.Chapter6,
															Localization.Words.TheMasochist };

        public Dictionary<int, int> ChapterStart = new Dictionary<int, int>();
        public Dictionary<int, int> ChapterEnd = new Dictionary<int, int>();
        Dictionary<int, Tuple<string, string>> SpecialLevel = new Dictionary<int, Tuple<string, string>>();

        static PerfectScoreObject MyPerfectScoreObject;

        public static void OnChapterFinished(int chapter)
        {
            // Didn't die during the chapter?
            foreach (PlayerData player in PlayerManager.AlivePlayers)
                Awardments.CheckForAward_NoDeath(player);

            // Give beat chapter award
            Awardment award = null;
            switch (chapter)
            {
                case 2: award = Awardments.Award_Campaign1; break;
                case 4: award = Awardments.Award_Campaign2; break;
                case 6: award = Awardments.Award_Campaign3; break;
                case 7: award = Awardments.Award_Campaign4; break;
                default: break;
            }
            Awardments.GiveAward(award);

			// Clean campaign stats
            for (int i = 0; i < 4; i++)
            {
                var player = PlayerManager.Players[i];
                if (player == null || !player.Exists) continue;

                player.CampaignStats.Clean();
            }
        }

        int StartLevel = 0;
        public override void Start(int Chapter)
        {
            CloudberryKingdomGame.PromptForDeviceIfNoneSelected();

			CloudberryKingdomGame.SetPresence(CloudberryKingdomGame.Presence.Campaign);

			MusicStarted = false;

            MyPerfectScoreObject = new PerfectScoreObject(false, true, true);

			// Continue at last level reached.
			if (Chapter < 0)
			{
				//int NextChapterStart = ChapterStart.ContainsKey(Chapter + 1) ? ChapterStart[Chapter + 1] : StartLevel + 100000;
				//int MaxLevelAttained = PlayerManager.MaxPlayerTotalCampaignIndex() + 1;

				//if (MaxLevelAttained > StartLevel && MaxLevelAttained < NextChapterStart)
				//    StartLevel = MaxLevelAttained;

				StartLevel = PlayerManager.MinPlayerTotalCampaignIndex() + 1;

				//StartLevel = 225;
			}
			else
			{
				StartLevel = ChapterStart[Chapter];
			}

            base.Start(StartLevel);
        }

        static int ChapterFinishing;
        public static void CheckForFinishedChapter()
        {
            if (ChapterFinishing >= 0)
            {
                OnChapterFinished(ChapterFinishing);
                ChapterFinishing = -1;
            }
        }

        protected override bool OnLevelBegin(Level level)
        {
			HelpMenu.CostMultiplier = 1;

			if (level.MyLevelSeed != null)
			{
				int Num = level.MyLevelSeed.LevelNum;
				if (Num >= 0)
				{
					foreach (var pair in ChapterStart)
					{
						if (pair.Value < Num)
						{
							HelpMenu.CostMultiplier = Math.Max(pair.Key + 1, HelpMenu.CostMultiplier);
						}
					}
				}
			}

            // Base OnLevelBegin
            if (base.OnLevelBegin(level)) return true;

            CheckForFinishedChapter();

            //level.MyGame.AddGameObject(InGameStartMenu.MakeListener());
            level.MyGame.AddGameObject(HelpMenu.MakeListener());
            return false;
        }

        protected override void AdditionalPreStart()
        {
            MyStringWorld.OnSwapToFirstLevel += new Action<LevelSeedData>(MyStringWorld_OnSwapToFirstLevel);
        }

        void MyStringWorld_OnSwapToFirstLevel(LevelSeedData data)
        {
            MyPerfectScoreObject.PreventRelease = true;
            data.MyGame.AddGameObject(MyPerfectScoreObject);
        }

        protected override void MakeSeedList()
        {
            int LastRealLevelIndex = -1;
            int LastSetChapter = -1;
            ChapterStart = new Dictionary<int, int>();
            ChapterEnd = new Dictionary<int, int>();

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
                        // Mark end of chapter
                        if (LastRealLevelIndex > 0)
                        {
                            ChapterEnd.AddOrOverwrite(LastSetChapter, LastRealLevelIndex);
                            LastRealLevelIndex = -1;
                        }

                        var chapter = int.Parse(data);
                        ChapterStart.AddOrOverwrite(chapter, count);
                        LastSetChapter = chapter;
                            
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
                        seed += string.Format("level:{0};index:{1};", level, count);
                        LastRealLevelIndex = level;

                        Seeds.Add(seed);
                        count++; level++;

                        break;
                }

                line = reader.ReadLine();
            }

            // Mark end of last chapter
            if (LastRealLevelIndex > 0)
            {
                ChapterEnd.AddOrOverwrite(LastSetChapter, LastRealLevelIndex);
                LastRealLevelIndex = -1;
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

		static bool MusicStarted;

        static void PostMakeCampaign(Level level)
        {
            if (level.MyLevelSeed.MyGameType == ActionGameData.Factory) return;

			if (level.MyLevelSeed.MySong == null)
			{
				if (!MusicStarted)
				{
					Tools.SongWad.SetPlayList(Tools.SongList_Standard);
					Tools.SongWad.Shuffle();
					LevelSeedData.WaitThenPlay(level.MyGame, 40, null);
				}
			}
			else
			{
				Tools.SongWad.SuppressNextInfoDisplay = true;
			}
			MusicStarted = true;

            level.MyGame.OnCoinGrab += OnCoinGrab;
            level.MyGame.OnCompleteLevel += OnCompleteLevel;

            // Level Title only
            //var title = new LevelTitle(string.Format("{1} {0}", level.MyLevelSeed.LevelNum, Localization.WordString(Localization.Words.Level)));
            
            // Level Title plus Hero Name
            if (!level.MyLevelSeed.NewHero && !level.MyLevelSeed.ShowChapterName)
            {
                var title = new LevelTitle(string.Format("{1} {0}", level.MyLevelSeed.LevelNum, Localization.WordString(Localization.Words.Level)));
                title.Shift(new Vector2(0, -45));
                level.MyGame.AddGameObject(title);
            }

            //if (!level.MyLevelSeed.NewHero)
            //{
            //    var hero_title = new LevelTitle(string.Format("{0}", Localization.WordString(level.DefaultHeroType.Name)));
            //    hero_title.Shift(new Vector2(0, -180));
            //    level.MyGame.AddGameObject(hero_title);
            //}

            if (level.MyLevelSeed.Darkness)
            {
                GameData.UseBobLighting(level, 0);
                Background.AddDarkLayer(level.MyBackground);
            }

			//var CScore = new GUI_CampaignScore();
			//CScore.PreventRelease = false;
			//level.MyGame.AddGameObject(CScore);
			//var CLevel = new GUI_Level(level.MyLevelSeed.LevelNum);
			var CLevel = new GUI_Level(level.MyLevelSeed.LevelNum);
			EzText _t;
			_t = CLevel.MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(0f, 0f); _t.Scale = 0.55f; }
			CLevel.MyPile.Pos = new Vector2(1590.556f, 856.0002f);

            CLevel.PreventRelease = false;
			level.MyGame.AddGameObject(CLevel);

            level.MyGame.MyBankType = GameData.BankType.Campaign;
        }

        static void OnCoinGrab(ObjectBase obj)
        {
            for (int i = 0; i < 4; i++)
            {
                var player = PlayerManager.Players[i];
                if (player == null || !player.Exists) continue;

                player.CampaignCoins++;
            }
        }

        static void OnCompleteLevel(Level level)
        {
			MarkProgress(level);

            if (CampaignProgressMade)
            {
                Tools.AddToDo(SaveGroup.SaveAll);
                //SaveGroup.SaveAll();
            }

            // Check for end of chapter
            foreach (KeyValuePair<int, int> key in Instance.ChapterEnd)
                if (key.Value == level.MyLevelSeed.LevelNum)
                {
                    ChapterFinishing = key.Key;
                    if (ChapterFinishing == 0) ChapterFinishing = -1;
                    break;
                }
        }

        public static bool CampaignProgressMade = false;
		public static void MarkProgress(Level level)
		{
            for (int i = 0; i < 4; i++)
            {
                var player = PlayerManager.Players[i];
                if (player == null || !player.Exists) continue;

                if (player.CampaignLevel < level.MyLevelSeed.LevelNum ||
                    player.CampaignIndex < level.MyLevelSeed.LevelIndex)
                {
                    CampaignProgressMade = true;
                }

				player.CampaignLevel = Math.Max(player.CampaignLevel, level.MyLevelSeed.LevelNum);
				player.CampaignIndex = Math.Max(player.CampaignIndex, level.MyLevelSeed.LevelIndex);
				player.Changed = true;
			}
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

            CheckForFinishedChapter();
        }

        protected CampaignSequence()
        {
			MusicStarted = false;
            ChapterFinishing = -1;
        }
    }
}