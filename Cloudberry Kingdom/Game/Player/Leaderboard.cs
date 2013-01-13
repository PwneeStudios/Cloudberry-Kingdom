using CloudberryKingdom.Bobs;
using System;
using System.IO;

using Microsoft.Xna.Framework.GamerServices;

namespace CloudberryKingdom
{
    public class Leaderboard
    {
        public static void WriteToLeaderboard(ScoreEntry score)
        {
            if (CloudberryKingdomGame.IsDemo) return;

#if XDK
            // Write
            NetworkSession networkSession = NetworkSession.Create(
                NetworkSessionType.LocalWithLeaderboards, 4, 4);

            networkSession.StartGame();
            while (networkSession.SessionState == NetworkSessionState.Lobby)
                networkSession.Update();

            foreach (LocalNetworkGamer localNetworkGamer in networkSession.LocalGamers)
            {
                LeaderboardWriter leaderboardWriter =
                    localNetworkGamer.LeaderboardWriter;

                if (leaderboardWriter != null)
                {
                    LeaderboardEntry leaderboardEntry =
                        leaderboardWriter.GetLeaderboard(LeaderboardIdentity.Create(LeaderboardKey.BestScoreLifeTime, 23)); // FIX ID
                    leaderboardEntry.Rating = 100; // FIX SCORE
                }
            }

            networkSession.EndGame();
            networkSession.Update(); networkSession.EndGame();
            networkSession.Update();
#endif
        }

        /*
        // Read
        LeaderboardReader leaderboardReader =
            LeaderboardReader.Read(LeaderboardIdentity.Create(LeaderboardKey.BestScoreLifeTime, 23), 0, 10); // FIX ID

        foreach (LeaderboardEntry entry in leaderboardReader.Entries)
        {
            entry.Rating;
            entry.Gamer.Gamertag;
            entry.Columns["key"]; // FIX KEY
        }*/
    }
}