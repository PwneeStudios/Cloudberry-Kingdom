#if PC_VERSION
using System;
using System.Text;
using System.Runtime.InteropServices;

using SW = SteamWrapper;

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
            return SW.SteamCore.RestartViaSteamIfNecessary(SteamId);
        }

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

		public static unsafe string PlayerName()
		{
			var pchName = SW.SteamCore.PlayerName();
			var s = new String(pchName);

			return s;
		}

		public static UInt64 SteamID()
		{
			try
			{
				UInt64 id = SW.SteamCore.SteamID();
				return id;
			}
			catch
			{
				return UInt64.MaxValue;
			}
		}
	}

	public static class HelperClass
	{
		public static string Utf8_to_Utf16(int[] s)
		{
			string utf16 = "";

			for (int i = 0; i < s.Length; i++)
			{
				if ((s[i] & 0x80) == 0x80 /* 1000000 */ )
				{
					int BytesToRead = 0;
					int FirstMask;

					if ((s[i] & 0xF0) == 0xF0 /* 11110000 */ ) { BytesToRead = 4; FirstMask = 0x0F; /* 00001111 */ }
					else if ((s[i] & 0xE0) == 0xE0 /* 11100000 */ ) { BytesToRead = 3; FirstMask = 0x1F; /* 00011111 */ }
					else if ((s[i] & 0xC0) == 0xC0 /* 11000000 */ ) { BytesToRead = 2; FirstMask = 0x3F; /* 00111111 */ }
					else { return ""; /* not a valid UTF-8 string */ }

					int val = s[i] & FirstMask;
					int shift = 0;
					for (int j = 0; j < BytesToRead - 1; ++j)
					{
						++i;

						val = val << 6;
						val += (int)(s[i] & 0x3F /* 00111111 */ );
					}
					utf16 += (char)val;
				}
				else
				{
					if (s[i] != 13)
						utf16 += (char)s[i];
				}
			}

			return utf16;
		}

		public static unsafe string CharArray_To_Utf8String(sbyte* pch)
		{
			string raw_string = new String(pch);

			int[] data = new int[raw_string.Length];
			for (int i = 0; i < raw_string.Length; i++)
			{
				data[i] = (int)raw_string[i];
			}

			string str = Utf8_to_Utf16(data);

			return str;
		}
	}

	public static class SteamTextInput
	{
		public static bool OverlayActive = false;

		public static unsafe string GetText()
		{
			var pchText = SW.SteamTextInput.GetText();

			string str = HelperClass.CharArray_To_Utf8String(pchText);

			return str;
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

			try
			{
				bool result = SW.SteamTextInput.ShowGamepadTextInput(Description, MaxCharacters, OnGamepadInputEnd);

				OverlayActive = result;

				return result;
			}
			catch
			{
				OverlayActive = false;
				return false;
			}
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

		public static void GiveAchievement(string AchievementApiName)
		{
			bool r = SW.SteamStats.GiveAchievement(AchievementApiName);
			Console.Write(r);
		}

		public static int NumEntriesFound()
		{
			return SW.SteamStats.NumEntriesFound();
		}

		public static int NumEntries(LeaderboardHandle Handle)
		{
			return SW.SteamStats.NumEntries(Handle.Handle);
		}

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

		public static unsafe string Results_GetName(int Index)
		{
			var pchName = SW.SteamStats.Results_GetPlayer(Index);
			
			var s = HelperClass.CharArray_To_Utf8String(pchName);

			return s;
		}

		public static int Results_GetId(int Index)
		{
			int id = SW.SteamStats.Results_GetId(Index);
			return id;
		}
	}
}

#elif MONO

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
			return "";
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
			Console.Write(AchievementApiName);
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
			return "";
		}

		public static int Results_GetId(int Index)
		{
			return 0;
		}
	}
}

#endif