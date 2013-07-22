using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SteamManager;

namespace StaticSiteGenerator
{
    public class HighScores
    {
        public class LeaderboardEntry
        {
            public int Rank;
            public int Score;
            public string Name;
			public string Flag = "ca";

            public LeaderboardEntry(int Rank, int Score, string Name)
            {
                this.Rank = Rank;
                this.Score = Score;
                this.Name = Name;

				switch ((int)(10 * Math.Sqrt(Score)) % 5)
				{
					case 0: this.Flag = "ca"; break;
					case 1: this.Flag = "eh"; break;
					case 2: this.Flag = "cn"; break;
					case 3: this.Flag = "bf"; break;
					case 4: this.Flag = "au"; break;
				}
            }
        }

		public static List<LeaderboardEntry> GetCountryBoard()
		{
			var l = new List<LeaderboardEntry>();

			Dictionary<string, LeaderboardEntry> BestCountryScore = new Dictionary<string, LeaderboardEntry>();

			foreach (var s in Entries)
			{
				if (BestCountryScore.ContainsKey(s.Flag))
				{
					if (s.Score > BestCountryScore[s.Flag].Score)
						BestCountryScore[s.Flag] = s;
				}
				else
				{
					BestCountryScore.Add(s.Flag, s);
				}
			}

			foreach (var s in BestCountryScore.Values)
			{
				l.Add(s);
			}

			return l;
		}

        public static int TotalSize = 0;
        public static List<LeaderboardEntry> Entries = new List<LeaderboardEntry>();

        public static void GetHighScore()
        {
            bool SteamInitialized = SteamCore.Initialize();
            if (!SteamInitialized)
            {
                return;
            }

            var id = GetIdentity(10000);
            SteamStats.FindLeaderboard(id, OnFindLeaderboard_Read);
            ReadingInProgress = true;

            while (ReadingInProgress)
            {
                SteamCore.Update();
            }

            SteamCore.Shutdown();
        }

        static bool ReadingInProgress = false;
        static void OnFindLeaderboard_Read(LeaderboardHandle Handle, bool failed)
        {
            Console.WriteLine("Find Leaderboard to read from. Failed? : {0}", failed);

            if (failed)
            {
                ReadingInProgress = false;
            }
            else
            {
				SteamStats.RequestEntries(Handle, SteamStats.LeaderboardDataRequestType.Global, 0, 1000,
					b => OnInfo(Handle, b));
            }
        }

        static void OnInfo(LeaderboardHandle Handle, bool failed)
        {
            if (failed)
            {
                ReadingInProgress = false;

                return;
            }
            
            TotalSize = SteamStats.NumEntries(Handle);

            int NumEntriesFound = SteamStats.NumEntriesFound();

            for (int i = 0; i < NumEntriesFound; i++)
            {
                int rank = SteamStats.Results_GetRank(i);
                int val = SteamStats.Results_GetScore(i);
                Gamer gamer = new Gamer(SteamStats.Results_GetName(i), SteamStats.Results_GetId(i));

                Entries.Add(new LeaderboardEntry(rank, val, gamer.Gamertag));
            }

            ReadingInProgress = false;
        }

		static string GetIdentity(int id)
		{
			switch (id)
			{
				case 7777: return "Story Mode";
				case 9999: return "Player Level";
				case 10000: return "Escalation, Classic";
				case 10100: return "Escalation, Fat Bob";
				case 11500: return "Escalation, Rocketbox";
				case 10200: return "Escalation, Gravity Bob";
				case 10400: return "Escalation, Jetman";
				case 10500: return "Escalation, Bouncy";
				case 11000: return "Escalation, Spaceship";
				case 10300: return "Escalation, Double Jump";
				case 11100: return "Escalation, Wheelie";
				case 10900: return "Escalation, Tiny Bob";
				case 11200: return "Escalation, Jetpack Wheelie";
				case 11300: return "Escalation, Hero";
				case 11400: return "Escalation, The Masochist";
				case 10001: return "Time Crisis, Classic";
				case 10101: return "Time Crisis, Fat Bob";
				case 11501: return "Time Crisis, Rocketbox";
				case 10201: return "Time Crisis, Gravity Bob";
				case 10401: return "Time Crisis, Jetman";
				case 10501: return "Time Crisis, Bouncy";
				case 11001: return "Time Crisis, Spaceship";
				case 10301: return "Time Crisis, Double Jump";
				case 11101: return "Time Crisis, Wheelie";
				case 10901: return "Time Crisis, Tiny Bob";
				case 11201: return "Time Crisis, Jetpack Wheelie";
				case 11301: return "Time Crisis, Hero";
				case 11401: return "Time Crisis, The Masochist";
				case 10002: return "Hero Rush";
				case 10003: return "Hybrid Rush";
				default: return "Player Level";
			}
		}
    }
}
