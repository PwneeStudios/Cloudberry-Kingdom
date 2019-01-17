using System;

namespace SteamManager
{
	public class Gamer
	{
		public string Gamertag;
		public int Id;

		public Gamer(string Gamertag, int Id)
		{
			this.Gamertag = Gamertag;
			this.Id = Id;
		}
	}

	public static class SteamCore
	{
		public static bool RestartViaSteamIfNecessary(uint SteamId)
		{
			return false;
		}

		public static bool Initialize()
		{
			return false;
		}

		public static void Shutdown()
		{
		}

		public static void Update()
		{
		}

		public static string PlayerName()
		{
			return "SteamPlayer";
		}

		public static UInt64 SteamID()
		{
			return UInt64.MaxValue;
		}
	}

	public static class SteamTextInput
	{
		public static bool OverlayActive = false;

		public static string GetText()
		{
			return "";
		}

		static void OnGamepadInputEnd(bool result)
		{
			OverlayActive = false;

			if (GamepadInputEndCallback != null)
			{
				GamepadInputEndCallback(result);
			}
		}

		static Action<bool> GamepadInputEndCallback;
		public static bool ShowGamepadTextInput(string Description, uint MaxCharacters, Action<bool> GamepadInputEndCallback)
		{
			SteamTextInput.GamepadInputEndCallback = GamepadInputEndCallback;

			OverlayActive = false;
			return false;
		}
	}

	public class LeaderboardHandle
	{
		public object Handle;

		public LeaderboardHandle(object Handle)
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

		public static void GiveAchievement(string AchievementApiName)
		{
			Console.WriteLine(AchievementApiName);
		}

		public static int NumEntriesFound()
		{
			return 0;
		}

		public static int NumEntries(LeaderboardHandle Handle)
		{
			return 0;
		}

		public static void FindLeaderboard(String LeaderboardName, Action<LeaderboardHandle, bool> OnFind)
		{

		}

		public static void UploadScores(LeaderboardHandle Handle, int Value)
		{

		}

		public static void RequestEntries(LeaderboardHandle Handle, LeaderboardDataRequestType RequestType, int Start, int End, Action<bool> OnDownload)
		{

		}

		public static int Results_GetScore(int Index)
		{
			return 0;
		}

		public static int Results_GetRank(int Index)
		{
			return 0;
		}

		public static string Results_GetName(int Index)
		{
			return "SteamPlayer";
		}

		public static int Results_GetId(int Index)
		{
			return 0;
		}
	}
}
