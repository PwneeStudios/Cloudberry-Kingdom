using System;
using System.Collections.Generic;
using System.IO;
using CoreEngine;

namespace CloudberryKingdom
{
    public class ScoreDatabase : SaveLoad
    {
        public static ScoreDatabase Instance;

        public static int MostRecentScoreDate = 0;
        public static int CurrentDate()
        {
            TimeSpan t = (DateTime.Now - new DateTime(2000, 1, 1));
            int minutes = (int)t.TotalMinutes;
            
            return minutes;
        }

        public static int Capacity = 20;

        private static Dictionary<int, List<ScoreEntry>> Games;

        public static void Initialize()
        {
            Instance = new ScoreDatabase();
            Instance.ContainerName = "HighScores";
            Instance.FileName = "HighScores";
            Instance.FailLoad();

            MostRecentScoreDate = CurrentDate();

			if (CloudberryKingdomGame.SimpleLeaderboards)
				SaveGroup.Add(Instance);
        }

        #region SaveLoad
        public override void Serialize(BinaryWriter writer)
        {
            foreach (var list in Games)
                foreach (var score in list.Value)
                    score.WriteChunk_2000(writer);
        }
        protected override void FailLoad()
        {
            Games = new Dictionary<int, List<ScoreEntry>>();
        }

        public  override void Deserialize(byte[] Data)
        {
            foreach (Chunk chunk in Chunks.Get(Data))
            {
				ProcessChunk(chunk);
            }
        }

		public static void ProcessChunk(Chunk chunk)
		{
			switch (chunk.Type)
			{
				case 2000: var score = new ScoreEntry(); score.ReadChunk_1000(chunk); Add(score); break;
			}
		}
        #endregion

        public static void EnsureList(int Game)
        {
            if (Games.ContainsKey(Game)) return;

            Games[Game] = new List<ScoreEntry>();
        }

        public static ScoreList GetList(int Game)
        {
            EnsureList(Game);

            var ScoreList = new ScoreList(10, 0);
            var Scores = Games[Game];

            int Count = 0;
            foreach (var score in Scores)
            {
                ScoreList.Add(score);
                Count++; if (Count >= 10) break;
            }

            return ScoreList;
        }

        /// <summary>
        /// Whether the given score qualifies for the high score list
        /// </summary>
        public static bool Qualifies(int Game, int Score)
        {
            EnsureList(Game);

            var Scores = Games[Game];
            return Scores.Count < Capacity ||
                    Score > Min(Scores).Value ||
                    Min(Scores).Fake;
        }

        /// <summary>
        /// Return the score with the smallest value.
        /// </summary>
        static ScoreEntry Max(List<ScoreEntry> Scores)
        {
            if (Scores.Count == 0)
                return new ScoreEntry(0);
            else
                return Scores[0];
        }
        
        public static ScoreEntry Max(int GameId)
        {
            EnsureList(GameId);
            return Max(Games[GameId]);
        }

        /// <summary>
        /// Return the score with the smallest value.
        /// </summary>
        public static ScoreEntry Min(List<ScoreEntry> Scores)
        {
            if (Scores.Count == 0)
                return new ScoreEntry(0);
            else
                return Scores[Scores.Count - 1];
        }

        public static ScoreEntry Min(int GameId)
        {
            EnsureList(GameId);
            return Min(Games[GameId]);
        }

        public static void Add(ScoreEntry score)
        {
            for (int i = 0; i < 4; i++)
            {
                var player = PlayerManager.Players[i];
                if (player == null || !player.Exists) continue;

                player.AddHighScore(score);
            }

            EnsureList(score.GameId);

            if (!Qualifies(score.GameId, score.Value))
                return;

            var list = Games[score.GameId];

            list.Add(score);
            Sort(list);
            TrimExcess(list);

            // Mark this object as changed, so that it will be saved to disk.
            Instance.Changed = true;
        }

        /// <summary>
        /// Remove excess entries, if the list is over capacity.
        /// </summary>
        static void TrimExcess(List<ScoreEntry> Scores)
        {
            if (Scores.Count > Capacity)
                Scores.RemoveRange(Scores.Count - 1, Scores.Count - Capacity);
        }

        /// <summary>
        /// Sort the list by value.
        /// </summary>
        static void Sort(List<ScoreEntry> Scores)
        {
            Scores.Sort((score1, score2) => (score2.Value - score1.Value));
        }
    }
}