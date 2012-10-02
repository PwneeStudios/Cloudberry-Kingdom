using System;
using System.Collections.Generic;
using System.IO;

namespace CloudberryKingdom
{
    public class ScoreEntry
    {
        public static string DefaultName
        {
            get { return PlayerManager.GetGroupGamerTag(18); }
        }

        public int Score;
        public string GamerTag;
        public bool Fake = false;

        public ScoreEntry() { }

        public ScoreEntry(int Score)
        {
            this.Score = Score;
            this.GamerTag = DefaultName;
        }

        public ScoreEntry(int Score, string GamerTag)
        {
            this.Score = Score;

            if (GamerTag == null) GamerTag = DefaultName;
            this.GamerTag = GamerTag;
        }

        public ScoreEntry(int Score, string GamerTag, bool Fake)
        {
            this.Score = Score;

            if (GamerTag == null) GamerTag = DefaultName;
            this.GamerTag = GamerTag;

            this.Fake = Fake;
        }

        public ScoreEntry(BinaryReader reader)
        {
            Fake = reader.ReadBoolean();
            Score = reader.ReadInt32();
            GamerTag = reader.ReadString();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Fake);
            writer.Write(Score);
            writer.Write(GamerTag);
        }

        public void WriteChunk(BinaryWriter writer)
        {
            writer.Write(0);
            writer.Write(1 + 4 + GamerTag.Length);

            writer.Write(Fake);
            writer.Write(Score);
            writer.Write(GamerTag);
        }

        public static void ReadChunk_0(BinaryReader reader)
        {
            var entry = new ScoreEntry();

            //entry.Fake = reader.ReadBoolean(
            //reader.Read(Fake);
        }

        public override string ToString()
        {
            return ToString(0);
        }

        public enum Format { Score, Time, Attempts };
        public Format MyFormat = Format.Score;
        public string _ToString()
        {
            return _ToString(MyFormat);
        }

        public string _ToString(Format format)
        {
            if (Fake) return "";

            switch (format)
            {
                case Format.Score: return ScoreToString();
                case Format.Attempts: return AttemptsToString();
                case Format.Time: return TimeToString();
            }

            return "";
        }

        string ScoreToString()
        {
            //return Score.ToString();
            return string.Format("{0:n0}", Score);
        }

        string TimeToString()
        {
            return CoreMath.Time(Score);
        }

        string AttemptsToString()
        {
            return Score.ToString();
        }

        public string ToString(int Length)
        {
            string score = _ToString();
            string tag = GamerTag.ToString();

            int NumDots = Length - score.Length - tag.Length;
            if (NumDots < 0) NumDots = 0;

            for (int i = 0; i < NumDots; i++) score += ".";
            return score + tag;
        }

        /// <summary>
        /// Returns a string of the form "{root}........{score}"
        /// </summary>
        /// <param name="Length">The desired length of the string</param>
        /// <param name="MinDots">The minimum number of dots seperating the root from the score</param>
        /// <returns></returns>
        public static string DottedScore(string root, int score, int Length, int MinDots)
        {
            string scorestr = score.ToString();

            int NumDots = Math.Max(MinDots, Length - scorestr.Length - root.Length);

            for (int i = 0; i < NumDots; i++) root += ".";
            return root + scorestr;
        }
    }

    public struct CampaignList
    {
        public ScoreList Score, Attempts, Time;

        int Difficulty;
        public CampaignList(int Difficulty)
        {
            this.Difficulty = Difficulty;
            string Suffix = Difficulty.ToString() + ".hsc";
            string Prefix = "Campaign";

            Score = new ScoreList();
            Score.FileName = Prefix + "Scores" + Suffix;
            SaveGroup.Add(Score);

            Attempts = new ScoreList(1000000);
            Attempts.SmallerIsBetter = true;
            Attempts.MyFormat = ScoreEntry.Format.Attempts;
            Attempts.Header = "Fewest Lives";
            Attempts.Prefix = "";
            Attempts.FileName = Prefix + "Lives" + Suffix;
            SaveGroup.Add(Attempts);

            Time = new ScoreList(62*60*100000);
            Time.SmallerIsBetter = true;
            Time.MyFormat = ScoreEntry.Format.Time;
            Time.Header = "Best Time";
            Time.Prefix = "";
            Time.FileName = Prefix + "Time" + Suffix;
            SaveGroup.Add(Time);
        }
    }

    public class ScoreList : SaveLoad
    {
        public string Header = "High Scores", Prefix = "";

        public int Capacity = 10;
        public ScoreEntry.Format MyFormat = ScoreEntry.Format.Score;

        public List<ScoreEntry> Scores;
        bool _SmallerIsBetter = false;
        int Mult = 1;
        public bool SmallerIsBetter
        {
            get { return _SmallerIsBetter; }
            set { _SmallerIsBetter = value; Mult = SmallerIsBetter ? -1 : 1; }
        }

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
            this.ContainerName = "HighScores";

            this.Capacity = Capacity;

            Scores = new List<ScoreEntry>(Capacity);

            for (int i = 0; i < Capacity; i++)
                Add(new ScoreEntry(DefaultValue, "", true));
        }

        /// <summary>
        /// Get the value of the top score
        /// </summary>
        public int Top {
            get {
                if (Scores == null) return 0;
                if (Scores.Count == 0) return 0;

                return Scores[0].Score;
            }
        }

        /// <summary>
        /// Get the value of the bottom score
        /// </summary>
        public int Bottom
        {
            get
            {
                return Min().Score;
            }
        }

        protected override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(Capacity);
            writer.Write(DefaultValue);

            writer.Write(Scores.Count);
            foreach (ScoreEntry score in Scores)
                score.Serialize(writer);
        }

        protected override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            int capacity = reader.ReadInt32();
            int defaultvalue = reader.ReadInt32();
            Init(capacity, defaultvalue);

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                Add(new ScoreEntry(reader));

            MostRecent = null;
        }

        /// <summary>
        /// The most recently added score.
        /// </summary>
        public ScoreEntry MostRecent;

        /// <summary>
        /// Whether the given score qualifies for the high score list
        /// </summary>
        public bool Qualifies(int score)
        {
            return Scores.Count < Capacity ||
                    Mult * score > Mult * Min().Score ||
                    Min().Fake;
        }

        public void Add(ScoreEntry score)
        {
            MostRecent = null;

            if (!Qualifies(score.Score))
                return;

            MostRecent = score;

            Scores.Add(score);
            Sort();
            TrimExcess();

            // Mark this object as changed, so that it will be saved to disk.
            Changed = true;
        }

        /// <summary>
        /// Remove excess entries, if the list is over capacity.
        /// </summary>
        void TrimExcess()
        {
            if (Scores.Count > Capacity)
                Scores.RemoveRange(Scores.Count - 1, Scores.Count - Capacity);
        }

        /// <summary>
        /// Return the score with the smallest value.
        /// </summary>
        public ScoreEntry Min()
        {
            if (Scores.Count == 0)
                return new ScoreEntry(0, "noone");
            else
                return Scores[Scores.Count - 1];
        }

        /// <summary>
        /// Sort the list by value.
        /// </summary>
        void Sort()
        {
            Scores.Sort((score1, score2) => Mult * (score2.Score - score1.Score));
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
    }
}