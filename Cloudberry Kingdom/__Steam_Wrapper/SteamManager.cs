#if PC_VERSION
//----------------------------------------------------------------
// SteamManager - Steam interface
//
// by: Derick Janssen <zorakbane@yahoo.com>
//     Erin Hastings <ejhastings@gmail.com>
//
// thanks to: Matt Enright <menright@cadenzainteractive>
//            for help on the cloud storage routines         
//----------------------------------------------------------------
using System;
using System.Runtime.InteropServices;

using SW = SteamWrapper;

namespace SteamManager
{
	public class Gamer
	{
		public string Gamertag;
	}

	public static class SteamCore
	{
		public static bool Initialize()
		{
			return SW.SteamCore.Initialize();
		}

		public static void Shutdown()
		{
			SW.SteamCore.Shutdown();
		}

		public static void Update()
		{
			SW.SteamCore.Update();
		}
	}

	public class LeaderboardHandle
	{
		public SW.LeaderboardHandle Handle;

		public LeaderboardHandle(SW.LeaderboardHandle Handle)
		{
			this.Handle = Handle;
		}
	}

	public static class SteamStats
	{
		public enum LeaderboardDataRequestType
		{
			Global = 0,
			GlobalAroundUser = 1,
			Friends = 2,
			Users = 3
		};

		public static void FindLeaderboard(String LeaderboardName, Action<LeaderboardHandle, bool> OnFind)
		{
			SW.SteamStats.FindLeaderboard(LeaderboardName, (h, b) => OnFind(new LeaderboardHandle(h), b));
		}

		public static void UploadScores(LeaderboardHandle Handle, int Value)
		{
			SW.SteamStats.UploadScore(Handle.Handle, Value);
		}

		public static void RequestEntries(LeaderboardHandle Handle, LeaderboardDataRequestType RequestType, int Start, int End, Action<bool> OnDownload)
		{
			SW.SteamStats.RequestEntries(Handle.Handle, (int)RequestType, Start, End, OnDownload);
		}

		public static int Results_GetScore(int Index)
		{
			return SW.SteamStats.Results_GetScore(Index);
		}

		public static int Results_GetRank(int Index)
		{
			return SW.SteamStats.Results_GetRank(Index);
		}
	}
}

#endif