using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
#if MONO
	public class LeaderboardIdentity
	{
	}
#endif

    public class Leaderboard
    {
        public static SignedInGamer LeaderboardGamer;
        public static List<Gamer> LeaderboardFriends;

        public const int EntriesPerPage = 20;
        //public const int EntriesPerPage = 4;

        static NetworkSession WritingNetworkSession;
        static ScoreEntry ScoreToWrite;
        static ScoreEntry[] ScoreToWrite_Separate;
        public static void WriteToLeaderboard(ScoreEntry[] scores)
        {
            _WriteToLeaderboard(null, scores);
        }
        public static void WriteToLeaderboard(ScoreEntry score)
        {
			ScoreEntry copy = new ScoreEntry(score.GamerTag, score.GameId, score.Value, score.Score, score.Level, score.Attempts, score.Time, score.Date);
            
			copy.GameId += Challenge.LevelMask;
            _WriteToLeaderboard(copy, null);
        }
        static void _WriteToLeaderboard(ScoreEntry score, ScoreEntry[] scores)
        {
            if (!CloudberryKingdomGame.OnlineFunctionalityAvailable()) return;

            if (WritingNetworkSession != null) return;
            if (WritingInProgress) return;

#if XDK
            if (Gamer.SignedInGamers.Count == 0) return;

            ScoreToWrite = score;
            ScoreToWrite_Separate = scores;

            // Async write
            WritingInProgress = true;
            NetworkSession.BeginCreate(NetworkSessionType.LocalWithLeaderboards, 4, 4, OnSessionCreate, null);
#endif
        }

        static bool WritingInProgress = false;
        static void OnSessionCreate(IAsyncResult ar)
        {
            try
            {
                WritingNetworkSession = NetworkSession.EndCreate(ar);
            }
            catch
            {
                return;
            }

            if (WritingNetworkSession == null) { WritingInProgress = false; return; }

            WritingNetworkSession.StartGame();
            while (WritingNetworkSession.SessionState == NetworkSessionState.Lobby)
                WritingNetworkSession.Update();

            foreach (LocalNetworkGamer localNetworkGamer in WritingNetworkSession.LocalGamers)
            {
                if (localNetworkGamer.SignedInGamer == null || !localNetworkGamer.SignedInGamer.IsSignedInToLive) continue;

                // Make sure the gamer is in the game
                if (PlayerManager.Players[(int)localNetworkGamer.SignedInGamer.PlayerIndex] == null) return;
                if (!PlayerManager.Players[(int)localNetworkGamer.SignedInGamer.PlayerIndex].Exists) continue;

                // Write the score
                LeaderboardWriter leaderboardWriter = localNetworkGamer.LeaderboardWriter;

                if (leaderboardWriter != null)
                {
                    ScoreEntry score = ScoreToWrite;
                    if (ScoreToWrite_Separate != null)
                    {
                        score = ScoreToWrite_Separate[(int)localNetworkGamer.SignedInGamer.PlayerIndex];
                    }

                    if (score == null) continue;

                    LeaderboardEntry leaderboardEntry = leaderboardWriter.GetLeaderboard(GetIdentity(score.GameId));
                    leaderboardEntry.Rating = score.Value;
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
            catch (Exception e)
            {
                Tools.Write(e.Message);
                Tools.Write(e.InnerException);
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

        public int gamer_rank = -1;

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
                foreach (LeaderboardEntry entry in reader.Entries)
                {
                    int rank = entry.GetRank();
                    int val = (int)entry.Rating;
                    Gamer gamer = entry.Gamer;
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

                    //if (gamer_rank == -1 || gamer.Gamertag == LeaderboardGamer.Gamertag)
                    if (gamer_rank == -1 && gamer.Gamertag == LeaderboardGamer.Gamertag && rank > 0)
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
            string key;
            switch (id)
            {
                case 7777: key = "StoryMode"; break;
                case 9999: key = "PlayerLevel"; break;
                case 10000: key = "Escalation_Normal"; break;
                case 10100: key = "Escalation_Big"; break;
                case 11500: key = "Escalation_Rocketbox"; break;
                case 10200: key = "Escalation_Inverse"; break;
                case 10400: key = "Escalation_Jetman"; break;
                case 10500: key = "Escalation_Bouncy"; break;
                case 11000: key = "Escalation_Spaceship"; break;
                case 10300: key = "Escalation_Double"; break;
                case 11100: key = "Escalation_Wheel"; break;
                case 10900: key = "Escalation_Small"; break;
                case 11200: key = "Escalation_JetpackWheelie"; break;
                case 11300: key = "Escalation_BigBouncy"; break;
                case 11400: key = "Escalation_Ultimate"; break;
                case 10001: key = "TimeCrisis_Normal"; break;
                case 10101: key = "TimeCrisis_Big"; break;
                case 11501: key = "TimeCrisis_Rocketbox"; break;
                case 10201: key = "TimeCrisis_Invert"; break;
                case 10401: key = "TimeCrisis_Jetman"; break;
                case 10501: key = "TimeCrisis_Bouncy"; break;
                case 11001: key = "TimeCrisis_Spaceship"; break;
                case 10301: key = "TimeCrisis_Double"; break;
                case 11101: key = "TimeCrisis_Wheel"; break;
                case 10901: key = "TimeCrisis_Small"; break;
                case 11201: key = "TimeCrisis_JetpackWheelie"; break;
                case 11301: key = "TimeCrisis_BigBouncy"; break;
                case 11401: key = "TimeCrisis_Ultimate"; break;
                case 10002: key = "HeroRush"; break;
                case 10003: key = "HeroRush2"; break;
                default: key = "PlayerLevel"; break;
            }

            LeaderboardIdentity LID = new LeaderboardIdentity() { Key = key };

            return LID;
        }
    }
}