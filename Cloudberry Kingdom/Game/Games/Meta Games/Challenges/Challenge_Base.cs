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
        public static BobPhsx ChosenHero;
        const int LevelMask = 10000;

        public int[] StartLevels = { 1, 50, 100, 150 };

        public string Name, MenuName;
        
        public int GameId_Score, GameId_Level;
        protected int GameTypeId;

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