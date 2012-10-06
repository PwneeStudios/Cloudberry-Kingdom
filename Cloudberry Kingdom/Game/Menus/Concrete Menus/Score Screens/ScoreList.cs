using System;
using System.Collections.Generic;
using System.IO;
using CoreEngine;

namespace CloudberryKingdom
{
    public class ScoreList
    {
        public int Capacity = 20;

        public string Header = "High Scores", Prefix = "";
        public ScoreEntry.Format MyFormat = ScoreEntry.Format.Score;

        public List<ScoreEntry> Scores;

        public ScoreList()
        {
            Init(10, 0);
        }
        public ScoreList(int DefaultValue)
        {
            Init(Capacity, DefaultValue);
        }
        public ScoreList(int Capacity, int DefaultValue)
        {
            Init(Capacity, 0);
        }

        public int DefaultValue;
        void Init(int Capacity, int DefaultValue)
        {
            this.DefaultValue = DefaultValue;
            this.Capacity = Capacity;

            Scores = new List<ScoreEntry>(Capacity);

            for (int i = 0; i < Capacity; i++)
                Scores.Add(new ScoreEntry(DefaultValue));
        }

        /// <summary>
        /// Whether the given score qualifies for the high score list
        /// </summary>
        public bool Qualifies(ScoreEntry Entry)
        {
            return Scores.Count < Capacity ||
                    Entry.Value > Bottom;
        }

        /// <summary>
        /// Get the value of the bottom score
        /// </summary>
        public int Bottom
        {
            get
            {
                if (Scores == null) return 0;
                if (Scores.Count == 0) return 0;

                return Scores[Scores.Count - 1].Value;
            }
        }

        public string ScoreString(ScoreEntry score, int Length)
        {
            score.MyFormat = MyFormat;

            int Rank = Scores.IndexOf(score) + 1;
            string RankStr = Rank.ToString() + ". ";

            while (RankStr.Length < 4) RankStr += " ";

            if (!score.Fake)
                RankStr += Prefix;

            string ScoreStr = string.Format("{0:n}", score);
            return RankStr + score.ToString(Length - RankStr.Length);
        }

        public void Add(ScoreEntry score)
        {
            Scores.Add(score);
        }
    }
}