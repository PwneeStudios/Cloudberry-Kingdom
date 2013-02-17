using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Leaderboard
    {
        public static SignedInGamer LeaderboardGamer;
        public static List<Gamer> LeaderboardFriends;

        //public const int EntriesPerPage = 20;
        public const int EntriesPerPage = 4;

        static NetworkSession WritingNetworkSession;
        static ScoreEntry ScoreToWrite;
        public static void WriteToLeaderboard(ScoreEntry score)
        {
            if (WritingNetworkSession != null) return;
            if (WritingInProgress) return;

#if XDK
            if (Gamer.SignedInGamers.Count == 0) return;

            ScoreToWrite = score;

            // Async write
            WritingInProgress = true;
            NetworkSession.BeginCreate(NetworkSessionType.LocalWithLeaderboards, 4, 4, OnSessionCreate, null);
#endif
        }

        static bool WritingInProgress = true;
        static void OnSessionCreate(IAsyncResult ar)
        {
            WritingNetworkSession = NetworkSession.EndCreate(ar);

            if (WritingNetworkSession == null) { WritingInProgress = false; return; }

            WritingNetworkSession.StartGame();
            while (WritingNetworkSession.SessionState == NetworkSessionState.Lobby)
                WritingNetworkSession.Update();

            foreach (LocalNetworkGamer localNetworkGamer in WritingNetworkSession.LocalGamers)
            {
                if (localNetworkGamer.SignedInGamer == null || !localNetworkGamer.SignedInGamer.IsSignedInToLive) continue;

                LeaderboardWriter leaderboardWriter = localNetworkGamer.LeaderboardWriter;

                if (leaderboardWriter != null)
                {
                    LeaderboardEntry leaderboardEntry = leaderboardWriter.GetLeaderboard(GetIdentity(ScoreToWrite.GameId));
                    leaderboardEntry.Rating = ScoreToWrite.Value;
                }
            }

            WritingNetworkSession.EndGame();
            WritingNetworkSession.Update();
            WritingNetworkSession.Dispose();
            
            WritingNetworkSession = null;
            WritingInProgress = false;
        }
        

        public static int GetLeaderboardId(int game_id)
        {
            return game_id;
        }

        int MyId;

        static IAsyncResult result;
        public LeaderboardGUI.LeaderboardType MySortType;
        public Leaderboard(int game_id)
        {
#if XDK
            Items = new Dictionary<int, LeaderboardItem>();
            FriendItems = new List<LeaderboardItem>();

            MyId = GetLeaderboardId(game_id);
#else
			// Test
			Items = new Dictionary<int, LeaderboardItem>();
			FriendItems = new List<LeaderboardItem>();
			MyId = GetLeaderboardId(game_id);
			MoreRequested = false;
			Updated = true;
			result = null;
			TotalSize = 10000;
			StartIndex = 1;
#endif
        }

        public void SetType(LeaderboardGUI.LeaderboardType type)
        {
            MySortType = type;

            if (result != null) return;

#if XDK
            LeaderboardIdentity Identity = GetIdentity(MyId);

            try
            {
                switch (MySortType)
                {
                    case LeaderboardGUI.LeaderboardType.TopScores:
                        result = LeaderboardReader.BeginRead(Identity, 0, EntriesPerPage, OnInfo_TopScores, null);
                        break;

                    case LeaderboardGUI.LeaderboardType.MyScores:
                        result = LeaderboardReader.BeginRead(Identity, LeaderboardGamer, EntriesPerPage, OnInfo_MyScores, null);
                        break;

                    case LeaderboardGUI.LeaderboardType.FriendsScores:
                        if (FriendItems.Count == 0)
                        {
                            result = LeaderboardReader.BeginRead(Identity, LeaderboardFriends, LeaderboardGamer, 100, OnInfo_FriendScores, null);
                        }
                        break;
                }
            }
            catch
            {
                Tools.Warning();
            }
#endif
        }

        void OnInfo_TopScores(IAsyncResult ar)
        {
            Update(LeaderboardGUI.LeaderboardType.TopScores, ar);
        }

        void OnInfo_MyScores(IAsyncResult ar)
        {
            Update(LeaderboardGUI.LeaderboardType.MyScores, ar);
        }

        void OnInfo_FriendScores(IAsyncResult ar)
        {
            Update(LeaderboardGUI.LeaderboardType.FriendsScores, ar);
        }

        bool MoreRequested = false;
        int RequestPage;
        public void RequestMore(int RequestPage)
        {
            if (MoreRequested || result != null) return;

            this.MoreRequested = true;
            this.RequestPage = RequestPage;

            LeaderboardIdentity Identity = GetIdentity(MyId);
#if XDK
			result = LeaderboardReader.BeginRead(Identity, RequestPage, EntriesPerPage, OnInfo_TopScores, null);
#endif
        }

        public bool Updated = false;
        public int StartIndex = -1;
        public int TotalSize = -1;

        void Update(LeaderboardGUI.LeaderboardType Type, IAsyncResult ar)
        {
#if XDK
            //var reader = LeaderboardReader.EndRead(result);
            LeaderboardReader reader;
            try
            {
                reader = LeaderboardReader.EndRead(ar);
            }
            catch
            {
                result = null; return;
            }

            //if (result == null || !result.IsCompleted) return;
            if (reader == null) { result = null; return; }

            TotalSize = reader.TotalLeaderboardSize;

            lock (Items)
            {
                int gamer_rank = -1;
                foreach (LeaderboardEntry entry in reader.Entries)
                {
                    int rank = entry.GetRank();
                    int val = (int)entry.Rating;
                    Gamer gamer = entry.Gamer;
                    var item = new LeaderboardItem(gamer, val, rank);

                    if (Type == LeaderboardGUI.LeaderboardType.FriendsScores)
                    {
                        FriendItems.Add(item);
                    }
                    else
                    {
                        Items.AddOrOverwrite(rank, item);
                    }

                    if (gamer_rank == -1 || gamer.Gamertag == LeaderboardGamer.Gamertag)
                        gamer_rank = rank;
                }

                MoreRequested = false;

                Updated = true;

                result = null;

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
#endif
        }

        public Dictionary<int, LeaderboardItem> Items;
        public List<LeaderboardItem> FriendItems;

        static LeaderboardIdentity GetIdentity(int id)
        {
            LeaderboardIdentity LID = new LeaderboardIdentity() { Key = "Escalation_Classic" };

            return LID;
        }
    }
}