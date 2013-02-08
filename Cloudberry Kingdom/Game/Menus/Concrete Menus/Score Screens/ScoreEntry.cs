using System;
using System.Collections.Generic;
using System.IO;
using CoreEngine;

namespace CloudberryKingdom
{
    public class ScoreEntry
    {
        public static string DefaultName
        {
            get { return PlayerManager.GetGroupGamerTag(18); }
        }

        public int GameId;
        public int Value, Score, Level, Attempts, Time, Date;
        public string GamerTag;
        public bool Fake = false;

        public ScoreEntry() { }

        public ScoreEntry(int Score)
        {
            this.Value = Score;
            this.GamerTag = "";
            this.Fake = true;
        }

        public ScoreEntry(string GamerTag, int Game, int Value, int Score, int Level, int Attempts, int Time, int Date)
        {
            if (GamerTag == null) GamerTag = DefaultName;
            this.GamerTag = GamerTag;

            this.GamerTag = GamerTag;
            this.GameId = Game;
            this.Value = Value;
            this.Score = Score;
            this.Level = Level;
            this.Attempts = Attempts;
            this.Time = Time;
            this.Date = Date;
        }

        public void WriteChunk_1000(BinaryWriter writer)
        {
            var chunk = new Chunk();
            chunk.Type = 1000;

			WriteMeat(chunk);
            
            chunk.Finish(writer);
        }

		public void WriteChunk_2000(BinaryWriter writer)
		{
			var chunk = new Chunk();
			chunk.Type = 2000;

			WriteMeat(chunk);

			chunk.Finish(writer);
		}

		private void WriteMeat(Chunk chunk)
		{
			chunk.Write(Fake);
			chunk.Write(GamerTag);
			chunk.Write(GameId);
			chunk.Write(Value);
			chunk.Write(Score);
			chunk.Write(Level);
			chunk.Write(Attempts);
			chunk.Write(Time);
			chunk.Write(Date);
		}

        public void ReadChunk_1000(Chunk chunk)
        {
            Fake = chunk.ReadBool();
            GamerTag = chunk.ReadString();
            GameId = chunk.ReadInt();
            Value = chunk.ReadInt();
            Score = chunk.ReadInt();
            Level = chunk.ReadInt();
            Attempts = chunk.ReadInt();
            Time = chunk.ReadInt();
            Date = chunk.ReadInt();
        }

        public override string ToString()
        {
            return ToString(0);
        }

        public enum Format { Score, Level, Attempts, Time };
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
                case Format.Level: return LevelToString();
                case Format.Attempts: return AttemptsToString();
                case Format.Time: return TimeToString();
            }

            return "";
        }

        string ScoreToString()
        {
            return string.Format("{0:n0}", Score);
        }

        string TimeToString()
        {
            return CoreMath.Time(Time);
        }

        string AttemptsToString()
        {
            return Attempts.ToString();
        }

        string LevelToString()
        {
            return Level.ToString();
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
}