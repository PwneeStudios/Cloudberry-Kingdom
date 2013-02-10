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
        public const int EntriesPerPage = 20;

        public static void WriteToLeaderboard(ScoreEntry score)
        {
#if XDK
            if (Gamer.SignedInGamers.Count == 0) return;

            // Write
            NetworkSession networkSession = NetworkSession.Create(
                NetworkSessionType.LocalWithLeaderboards, 4, 4);

            networkSession.StartGame();
            while (networkSession.SessionState == NetworkSessionState.Lobby)
                networkSession.Update();

            foreach (LocalNetworkGamer localNetworkGamer in networkSession.LocalGamers)
            {
                if (localNetworkGamer.SignedInGamer == null || !localNetworkGamer.SignedInGamer.IsSignedInToLive) continue;

                LeaderboardWriter leaderboardWriter =
                    localNetworkGamer.LeaderboardWriter;

                if (leaderboardWriter != null)
                {
                    LeaderboardEntry leaderboardEntry =
                        leaderboardWriter.GetLeaderboard(LeaderboardIdentity.Create(LeaderboardKey.BestScoreLifeTime, GetLeaderboardId(score.GameId)));
                    leaderboardEntry.Rating = score.Value;
                }
            }

            networkSession.EndGame();
            networkSession.Update();
#endif
        }

        public static int GetLeaderboardId(int game_id)
        {
            return game_id;
        }

        int MyId;
        List<ScoreEntry> Above, Below;

        public ScoreEntry GetEntry(int index)
        {
            if (index > 0)
                return Below[index];
            else
                return Above[index];
        }

        IAsyncResult result;
        public Leaderboard(int game_id)
        {
            MyId = GetLeaderboardId(game_id);
            Above = new List<ScoreEntry>();
            Below = new List<ScoreEntry>();

            LeaderboardIdentity Identity = GetIdentity(MyId);
            //leaderboardReader
            result = LeaderboardReader.BeginRead(Identity, 0, 10, OnInfo, null);
            
            //leaderboardReader.BeginPageDown(AsyncCallback, null);
        }

        void OnInfo(IAsyncResult ar)
        {
            Tools.Warning();
        }

        void AsyncCallback(IAsyncResult ar)
        {
            leaderboardReader.EndPageDown(ar);
        }

        void Update()
        {
            if (result == null || !result.IsCompleted) return;

            result = null;

            foreach (LeaderboardEntry entry in leaderboardReader.Entries)
            {
                int val = (int)entry.Rating;
                Gamer gamer = entry.Gamer;
                
                //entry.Columns["key"];
            }
        }

        LeaderboardReader leaderboardReader;
        LeaderboardIdentity GetIdentity(int id)
        {
            return LeaderboardIdentity.Create(LeaderboardKey.BestScoreLifeTime, id);
        }
    }
}