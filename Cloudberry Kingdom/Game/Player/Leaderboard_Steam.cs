#if FALSE && PC_VERSION
using System;
using System.IO;
using System.Collections.Generic;

//using SteamManager;

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Leaderboard
    {
		public const int EntriesPerPage = 20;

		LeaderboardHandle LHandle = null;
		static bool ReadingInProgress = false;

		void OnInfo(LeaderboardHandle Handle, LeaderboardGUI.LeaderboardType Type, bool failed)
		{
			if (failed)
			{
				ReadingInProgress = false;

				return;
			}

			Update(Handle, Type);

			ReadingInProgress = false;
		}

		void Update(LeaderboardHandle Handle, LeaderboardGUI.LeaderboardType Type)
		{
			TotalSize = SteamStats.NumEntries(Handle);

			lock (Items)
			{
				int NumEntriesFound = SteamStats.NumEntriesFound();
				
				for (int i = 0; i < NumEntriesFound; i++)
				{
					int rank = SteamStats.Results_GetRank(i);
					int val = SteamStats.Results_GetScore(i);
					Gamer gamer = new Gamer(SteamStats.Results_GetName(i), SteamStats.Results_GetId(i));
					var item = new LeaderboardItem(gamer, val, rank);

					if (Type == LeaderboardGUI.LeaderboardType.FriendsScores)
					{
						if (rank > 0 && val > 0)
							FriendItems.Add(item);
					}
					else
					{
						Items.AddOrOverwrite(rank, item);
					}

					if (gamer_rank == -1 && gamer.Gamertag == SteamCore.PlayerName() && rank > 0)
						gamer_rank = rank;
				}

				Updated = true;

				switch (Type)
				{
					case LeaderboardGUI.LeaderboardType.TopScores:
						StartIndex = 1;
						break;
					case LeaderboardGUI.LeaderboardType.MyScores:
						StartIndex = gamer_rank;
						break;
					case LeaderboardGUI.LeaderboardType.FriendsScores:
						StartIndex = 1;
						TotalSize = FriendItems.Count;
						break;
					default: break;
				}
			}
		}

		static void OnFindLeaderboard_Write(ScoreEntry Entry, LeaderboardHandle Handle, bool failed)
		{
			Tools.Write("Find Leaderboard to write to. Failed? : {0}", failed);

			if (!failed)
			{
				SteamStats.UploadScores(Handle, Entry.Score);
			}

			WritingInProgress = false;
		}

        public static void WriteToLeaderboard(ScoreEntry[] scores)
        {
            _WriteToLeaderboard(null, scores);
        }
        public static void WriteToLeaderboard(ScoreEntry score)
        {
			ScoreEntry copy = new ScoreEntry(score.GamerTag, score.GameId, score.Value, score.Score, score.Level, score.Attempts, score.Time, score.Date);
            
			//copy.GameId += Challenge.LevelMask;
            _WriteToLeaderboard(copy, null);
        }
        static void _WriteToLeaderboard(ScoreEntry score, ScoreEntry[] scores)
        {
            if (!CloudberryKingdomGame.OnlineFunctionalityAvailable()) return;

			if (WritingInProgress) return;
			
			WritingInProgress = true;

			var id = GetIdentity(score.GameId);
			SteamStats.FindLeaderboard(id, (h, b) => OnFindLeaderboard_Write(score, h, b));
		}

        static bool WritingInProgress = false;

        public static int GetLeaderboardId(int game_id)
        {
            return game_id;
        }

        int MyId;

        public LeaderboardGUI.LeaderboardType MySortType;
        public Leaderboard(int game_id)
        {
            Items = new Dictionary<int, LeaderboardItem>();
            FriendItems = new List<LeaderboardItem>();

            MyId = GetLeaderboardId(game_id);

			var id = GetIdentity(game_id);
			SteamStats.FindLeaderboard(id, OnFindLeaderboard_Read);
        }

		void OnFindLeaderboard_Read(LeaderboardHandle Handle, bool failed)
		{
			Tools.Write("Find Leaderboard to read from. Failed? : {0}", failed);

			if (!failed)
			{
				LHandle = Handle;
			}
		}

        public void SetType(LeaderboardGUI.LeaderboardType type)
        {
            MySortType = type;

			if (LHandle == null) return;
			if (ReadingInProgress) return;

			try
			{
				switch (MySortType)
				{
					case LeaderboardGUI.LeaderboardType.TopScores:
						ReadingInProgress = true;
						SteamStats.RequestEntries(LHandle, SteamStats.LeaderboardDataRequestType.Global, 0, EntriesPerPage,
							b => OnInfo(LHandle, LeaderboardGUI.LeaderboardType.TopScores, b));
						break;

					case LeaderboardGUI.LeaderboardType.MyScores:
						ReadingInProgress = true;
						SteamStats.RequestEntries(LHandle, SteamStats.LeaderboardDataRequestType.GlobalAroundUser, -EntriesPerPage, EntriesPerPage,
							b => OnInfo(LHandle, LeaderboardGUI.LeaderboardType.MyScores, b));
						break;

					case LeaderboardGUI.LeaderboardType.FriendsScores:
						if (FriendItems.Count == 0)
						{
							ReadingInProgress = true;
							SteamStats.RequestEntries(LHandle, SteamStats.LeaderboardDataRequestType.Friends, 0, 0,
								b => OnInfo(LHandle, LeaderboardGUI.LeaderboardType.FriendsScores, b));
						}
						break;
				}
			}
			catch (Exception e)
			{
				Tools.Write(e.Message);
				Tools.Write(e.InnerException);
				Tools.Warning();
			}
		}

        public void RequestMore(int RequestPage)
        {
            if (ReadingInProgress || LHandle == null) return;

			ReadingInProgress = true;

			SteamStats.RequestEntries(LHandle, SteamStats.LeaderboardDataRequestType.Global, RequestPage, RequestPage + EntriesPerPage,
				b => OnInfo(LHandle, LeaderboardGUI.LeaderboardType.TopScores, b));
		}

        public bool Updated = false;
        public int StartIndex = -1;
        public int TotalSize = -1;

        public int gamer_rank = -1;

		public Dictionary<int, LeaderboardItem> Items;
        public List<LeaderboardItem> FriendItems;

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
#endif