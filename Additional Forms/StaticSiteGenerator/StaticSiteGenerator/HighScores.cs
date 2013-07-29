using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using System.IO;

using System.Diagnostics;

using System.Data;
using System.Data.OleDb;

using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Spreadsheets;

using SteamManager;

namespace StaticSiteGenerator
{
	public class FeatureItem
	{
		public string Channel;
		public string Embed;
	}

    public class HighScores
    {
		public class YouTubeGamer
		{
			public string Name, Channel, Country, SteamNickname, YouTubeLink;
			public bool Confirmed = false;

			public string Flag = "none";

			public void Init()
			{
				this.Flag = CountryCodeLookup.FullNameToIsoCode(this.Country);
			}
		}

		public class BoardData
		{
			public static List<FeatureItem> GetFeatureList()
			{
				var Cells = GetGoogleData("Infinity Cup Youtube Coverage");
				if (Cells == null) return null;

				var list = new List<FeatureItem>();

				for (int j = 2; j < 300; j++)
				{
					var item = new FeatureItem();

					try
					{
						item.Channel = Cells[j, 1];
						item.Embed = Cells[j, 2];
					}
					catch (Exception e)
					{
						continue;
					}

					if (item.Embed != null)
					{
						list.Add(item);
					}
				}

				return list;
			}

			public static Dictionary<string, YouTubeGamer> YouTubeDict = null;

			public static string[,] GetGoogleData(string SpreadsheetName)
			{
				string USERNAME = "jordan@pwnee.com";
				string PASSWORD = "heartenergysmile";

				SpreadsheetsService service = new SpreadsheetsService("MySpreadsheetIntegration-v1");
				service.setUserCredentials(USERNAME, PASSWORD);

				SpreadsheetQuery query = new SpreadsheetQuery();
				SpreadsheetFeed feed = service.Query(query);

				if (feed.Entries.Count == 0)
				{
					return null;
				}

				// 
				string downloadUrl = null;
				AtomEntry sheet = null;
				foreach (var entry in feed.Entries)
				{
					if (entry.Title.Text == SpreadsheetName)
					{
						sheet = entry;
						//downloadUrl = entry.Content.Src.Content;
					}
				};

				SpreadsheetEntry spreadsheet = sheet as SpreadsheetEntry;

				if (spreadsheet == null) return null;

				// Get the first worksheet of the first spreadsheet.
				// TODO: Choose a worksheet more intelligently based on your
				// app's needs.
				WorksheetFeed wsFeed = spreadsheet.Worksheets;
				WorksheetEntry worksheet = (WorksheetEntry)wsFeed.Entries[0];

				// Fetch the cell feed of the worksheet.
				CellQuery cellQuery = new CellQuery(worksheet.CellFeedLink);
				CellFeed cellFeed = service.Query(cellQuery);

				string[,] Cells = new string[1000, 1000]; 
				foreach (CellEntry curCell in cellFeed.Entries)
				{
					//Console.WriteLine("Row {0}, column {1}: {2}", curCell.Cell.Row, curCell.Cell.Column, curCell.Cell.Value);
					Cells[curCell.Cell.Row, curCell.Cell.Column] = curCell.Cell.Value;
				}

				return Cells;
			}





			public static void GetData()
			{
				var Cells = GetGoogleData("Youtuber Invite List");
				if (Cells == null) return;

				YouTubeDict = new Dictionary<string, YouTubeGamer>();

				for (int j = 0; j < 300; j++)
				{
					var gamer = new YouTubeGamer();

					try
					{
						//if (Cells[j, 8] == null) continue;

						gamer.Confirmed = (int.Parse(Cells[j, 9]) == 1);
						if (!gamer.Confirmed) continue;

						gamer.Name = Cells[j, 1];
						gamer.Channel = Cells[j, 2];
						gamer.Country = Cells[j, 3];
						gamer.SteamNickname = Cells[j, 12];
						gamer.YouTubeLink = Cells[j, 13];

						if (!gamer.YouTubeLink.Contains("http"))
						{
							gamer.YouTubeLink = "http://" + gamer.YouTubeLink;
						}

						gamer.Init();
					}
					catch (Exception e)
					{
						continue;
					}

					if (gamer.SteamNickname != null)
					{
						YouTubeDict.Add(gamer.SteamNickname.ToLowerInvariant(), gamer);
					}
				}
			}

			/*
			public static Dictionary<string, YouTubeGamer> GetData()
			{
				YouTubeDict = new Dictionary<string, YouTubeGamer>();

				// Get texture asset list
				string proj = @"Youtuber Invite List.xlsx";
				string connection = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=Excel 12.0;", proj);

				var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connection);
				var ds = new DataSet();

				adapter.Fill(ds);

				var table = ds.Tables["Table"];
				var data = table.AsEnumerable();

				int skip = 5;
				foreach (var d in data)
				{
					skip--;
					if (skip > 0) continue;

					var gamer = new YouTubeGamer();

					try
					{
						gamer.Confirmed = ((int)(double)(d.ItemArray[8]) == 1);
						if (!gamer.Confirmed) continue;

						gamer.Name = (string)d.ItemArray[0];
						gamer.Channel = (string)d.ItemArray[1];
						gamer.Country = (string)d.ItemArray[2];
						gamer.SteamNickname = (string)d.ItemArray[11];
						gamer.YouTubeLink = (string)d.ItemArray[12];

						if (!gamer.YouTubeLink.Contains("http"))
						{
							gamer.YouTubeLink = "http://" + gamer.YouTubeLink;
						}

						gamer.Init();
					}
					catch (Exception e)
					{
						continue;
					}

					YouTubeDict.Add(gamer.SteamNickname, gamer);
				}

				return YouTubeDict;
			}
			*/
		}

        public class LeaderboardEntry
        {
            public int Rank;
            public int Score;
            public string Name;

            public LeaderboardEntry(int Rank, int Score, string Name)
            {
                this.Rank = Rank;
                this.Score = Score;
                this.Name = Name;
            }
        }

		public static List<LeaderboardEntry> GetCountryBoard()
		{
			var l = new List<LeaderboardEntry>();

			Dictionary<string, LeaderboardEntry> BestCountryScore = new Dictionary<string, LeaderboardEntry>();

			foreach (var s in Entries)
			{
				string flag = BoardData.YouTubeDict[s.Name.ToLowerInvariant()].Flag;

				if (BestCountryScore.ContainsKey(flag))
				{
					if (s.Score > BestCountryScore[flag].Score)
						BestCountryScore[flag] = s;
				}
				else
				{
					BestCountryScore.Add(flag, s);
				}
			}

			foreach (var s in BestCountryScore.Values)
			{
				l.Add(s);
			}

			return l;
		}

        public static int TotalSize = 0;
        public static List<LeaderboardEntry> Entries = null;
		public static List<LeaderboardEntry> BetaEntries = null;

        public static void GetHighScore()
        {
			Entries = new List<LeaderboardEntry>();
			BetaEntries = new List<LeaderboardEntry>();

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

				if (gamer.Gamertag == "Guunnz") continue;

				if (BoardData.YouTubeDict.ContainsKey(gamer.Gamertag.ToLowerInvariant()))
				{
					Entries.Add(new LeaderboardEntry(rank, val, gamer.Gamertag));
				}
				else
				{
					BetaEntries.Add(new LeaderboardEntry(rank, val, gamer.Gamertag));
				}
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
