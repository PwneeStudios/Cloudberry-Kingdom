using System;
using System.Collections.Generic;

namespace CloudberryKingdom
{
    public class AftermathData
    {
        public bool Success;
        public bool EarlyExit;
        public bool Retry = false;
    }

    public abstract class Challenge
    {
        public static int Coins;
        public static int CurrentScore;

        public static void OnCoinGrab(ObjectBase obj)
        {
            Coins++;
        }

        public static int CurrentId;

		public static int LeaderboardIndex = 0;
        public static BobPhsx ChosenHero;
        public const int LevelMask = 10000;

        public int[] StartLevels = { 1, 50, 100, 150 };

        public Localization.Words Name, MenuName;
        
        public int GameId_Score, GameId_Level;
        protected int GameTypeId;

        public int CalcTopGameLevel(BobPhsx hero)
        {
            int id = CalcGameId_Level(hero);
            return PlayerManager.MaxPlayerHighScore(id);
        }

        public int CalcGameId_Score(BobPhsx hero)
        {
            int HeroId = hero == null ? 0 : hero.Id;

			//GameId_Level = 100 * HeroId + GameTypeId;
			//return GameId_Level;
			return 100 * HeroId + GameTypeId;
        }

        public int CalcGameId_Level(BobPhsx hero)
        {
            int HeroId = hero == null ? 0 : hero.Id;

			//GameId_Level = 100 * HeroId + GameTypeId + LevelMask;
			//return GameId_Level;
			return 100 * HeroId + GameTypeId + LevelMask;
        }

        public int SetGameId()
        {
            int HeroId = Challenge.ChosenHero == null ? 0 : Challenge.ChosenHero.Id;

            GameId_Score = 100 * HeroId + GameTypeId;
            GameId_Level = 100 * HeroId + GameTypeId + LevelMask;
            return GameId_Score;
        }

        protected StringWorldGameData StringWorld { get { return (StringWorldGameData)Tools.WorldMap; } }

        /// <summary>
        /// Get the top score that anyone on this machine has ever gotten.
        /// </summary>
        public int TopScore()
        {
            SetGameId();
            return ScoreDatabase.Max(GameId_Score).Score;
        }

        /// <summary>
        /// Get the highest level that anyone on this machine has ever gotten.
        /// </summary>
        public int TopLevel()
        {
            SetGameId();
            return ScoreDatabase.Max(GameId_Level).Level;
        }

        /// <summary>
        /// Get the top score that anyone playing has ever gotten.
        /// </summary>
        public int TopPlayerScore()
        {
            SetGameId();
            return PlayerManager.MaxPlayerHighScore(GameId_Score);
        }

        /// <summary>
        /// Get the top score that anyone playing has ever gotten.
        /// </summary>
        public int TopPlayerScore(BobPhsx hero)
        {
            SetGameId();
            return PlayerManager.MaxPlayerHighScore(GameId_Score);
        }

        /// <summary>
        /// Get the highest level that anyone playing has ever gotten.
        /// </summary>
        public int TopPlayerLevel()
        {
            SetGameId();
            return PlayerManager.MaxPlayerHighScore(GameId_Level);
        }

        protected virtual void ShowEndScreen()
        {
            var MyGameOverPanel = new GameOverPanel(GameId_Score, GameId_Level);
            MyGameOverPanel.Levels = StringWorld.CurLevelIndex + 1;
            
            Tools.CurGameData.AddGameObject(MyGameOverPanel);
        }

        /// <summary>
        /// If true then this meta-game is not part of the campaign.
        /// </summary>
        public bool NonCampaign = true;
        public virtual void Start(int Difficulty)
        {
            CloudberryKingdomGame.PromptForDeviceIfNoneSelected();

			HelpMenu.CostMultiplier = 1;
			HelpMenu.SetCostGrowthType(HelpMenu.CostGrowthTypes.None);

            CurrentId = this.GameId_Level;
            CurrentScore = 0;

            PlayerManager.PartiallyInvisible = true;
            PlayerManager.TotallyInvisible = true;

            Coins = 0;

            if (NonCampaign)
                PlayerManager.CoinsSpent = 0;

            DifficultySelected = Difficulty;
        }

        /// <summary>
        /// The difficulty selected for this challenge.
        /// </summary>
        public int DifficultySelected;

        /// <summary>
        /// Called immediately after the end of the challenge.
        /// </summary>
        public void Aftermath()
        {
            CurrentId = -1;
            CurrentScore = -1;
            AftermathData data = Tools.CurrentAftermath;
        }
        
        protected virtual void SetGameParent(GameData game)
        {
            game.ParentGame = Tools.CurGameData;
            Tools.WorldMap = Tools.CurGameData = game;
            Tools.CurLevel = game.MyLevel;
        }

        public abstract LevelSeedData GetSeed(int Index);
    }
}