using System;
using System.Collections.Generic;

using Steamworks;

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
            return SteamWrapper.SteamCore.RestartViaSteamIfNecessary(SteamId);
        }

		public static bool Initialize()
		{
			return SteamWrapper.SteamCore.Initialize();			
		}

		public static void Shutdown()
		{
			SteamWrapper.SteamCore.Shutdown();
		}

		public static void Update()
		{
			SteamWrapper.SteamCore.Update();
		}

		public static string PlayerName()
		{
			return SteamWrapper.SteamCore.PlayerName();
		}

		public static UInt64 SteamID()
		{
			try
			{
				UInt64 id = SteamWrapper.SteamCore.PlayerId();
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
	}

	public static class SteamTextInput
	{
		public static bool OverlayActive = false;

		public static string GetText()
		{
            //var pchText = SteamWrapper.SteamTextInput.GetText();
            //string str = HelperClass.CharArray_To_Utf8String(pchText);
            //return str;
            return null;
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
                //bool result = SteamWrapper.SteamTextInput.ShowGamepadTextInput(Description, MaxCharacters, OnGamepadInputEnd);
                //OverlayActive = result;
                //return result;
                return false;
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
		public SteamWrapper.LeaderboardHandle Handle;

		public LeaderboardHandle(SteamWrapper.LeaderboardHandle Handle)
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
			//bool r = SteamWrapper.SteamStats.GiveAchievement(AchievementApiName);
		}

		public static int NumEntriesFound()
		{
            //return SteamWrapper.SteamStats.NumEntriesFound();
            return 0;
		}

		public static int NumEntries(LeaderboardHandle Handle)
		{
            //return SteamWrapper.SteamStats.NumEntries(Handle.Handle);
            return 0;
		}

		public static void FindLeaderboard(String LeaderboardName, Action<LeaderboardHandle, bool> OnFind)
		{
			//SteamWrapper.SteamStats.FindLeaderboard(LeaderboardName, (h, b) => OnFind(new LeaderboardHandle(h), b));
		}

		public static void UploadScores(LeaderboardHandle Handle, int Value)
		{
			//SteamWrapper.SteamStats.UploadScore(Handle.Handle, Value);
		}

		public static void RequestEntries(LeaderboardHandle Handle, LeaderboardDataRequestType RequestType, int Start, int End, Action<bool> OnDownload)
		{
			//SteamWrapper.SteamStats.RequestEntries(Handle.Handle, (int)RequestType, Start, End, OnDownload);
		}

		public static int Results_GetScore(int Index)
		{
            //return SteamWrapper.SteamStats.Results_GetScore(Index);
            return 0;
		}

		public static int Results_GetRank(int Index)
		{
            //return SteamWrapper.SteamStats.Results_GetRank(Index);
            return 0;
		}

		public static string Results_GetName(int Index)
		{
            //var pchName = SteamWrapper.SteamStats.Results_GetPlayer(Index);
            //var s = HelperClass.CharArray_To_Utf8String(pchName);
            //return s;
            return null;
		}

		public static int Results_GetId(int Index)
		{
            //int id = SteamWrapper.SteamStats.Results_GetId(Index);
            //return id;
            return 0;
		}
	}
}

namespace SteamWrapper
{
    public class CallbackClass
    {
        public static CallbackClass instance = new CallbackClass();
        public CallbackClass() { }

        public void OnFindLeaderboard(LeaderboardFindResult_t pResult, bool bIOFailure)
        {
            LeaderboardHandle handle = new LeaderboardHandle(pResult.m_hSteamLeaderboard);
            SteamStats.s_OnFind.Invoke(handle, bIOFailure);
        }

        public void OnFindLobbies(LobbyMatchList_t pLobbyMatchList, bool bIOFailure)
        {
            if (!bIOFailure)
            {
                SteamMatches.s_nLobbiesFound = (int)pLobbyMatchList.m_nLobbiesMatching;
            }
            else
            {
                SteamMatches.s_nLobbiesFound = 0;
            }

            SteamMatches.s_OnFindLobbies.Invoke(bIOFailure);
        }

        public void OnJoinLobby(LobbyEnter_t pCallback, bool bIOFailure)
        {
            if (!bIOFailure && !SteamCore.InOfflineMode())
            {
                SteamMatches.s_CurrentLobby = new SteamLobby(pCallback.m_ulSteamIDLobby);
            }

            SteamMatches.s_OnJoinLobby.Invoke(bIOFailure);
        }

        public void OnChatUpdate(LobbyChatUpdate_t pCallback)
        {
            if (SteamMatches.s_OnChatUpdate != null)
            {
                int ChatMemberStateChange = 0;
                var state = pCallback.m_rgfChatMemberStateChange;
                if ((state & (uint)EChatMemberStateChange.k_EChatMemberStateChangeBanned) != 0)
                    ChatMemberStateChange = SteamMatches.ChatMember_Banned;
                else if ((state & (uint)EChatMemberStateChange.k_EChatMemberStateChangeDisconnected) != 0)
                    ChatMemberStateChange = SteamMatches.ChatMember_Disconnected;
                else if ((state & (uint)EChatMemberStateChange.k_EChatMemberStateChangeEntered) != 0)
                    ChatMemberStateChange = SteamMatches.ChatMember_Entered;
                else if ((state & (uint)EChatMemberStateChange.k_EChatMemberStateChangeKicked) != 0)
                    ChatMemberStateChange = SteamMatches.ChatMember_Kicked;
                else if ((state & (uint)EChatMemberStateChange.k_EChatMemberStateChangeLeft) != 0)
                    ChatMemberStateChange = SteamMatches.ChatMember_Left;

                SteamMatches.s_OnChatUpdate.Invoke(ChatMemberStateChange, pCallback.m_ulSteamIDUserChanged);
            }
        }

        public void OnChatMsg(LobbyChatMsg_t pCallback)
        {
            CSteamID sender;
            EChatEntryType entryType;

            byte[] pvData = new byte[4096];

            SteamMatchmaking.GetLobbyChatEntry(
                (CSteamID)pCallback.m_ulSteamIDLobby,
                (int)pCallback.m_iChatID,
                out sender,
                pvData,
                pvData.Length,
                out entryType
            );

            if (SteamMatches.s_OnChatMsg != null)
            {
                string msg = StringHelper.GetString(pvData);
                var id = sender.m_SteamID;
                string name = SteamFriends.GetFriendPersonaName(sender);

                SteamMatches.s_OnChatMsg.Invoke(msg, id, name);
            }
        }

        public void OnDataUpdate(LobbyDataUpdate_t pCallback)
        {
            if (SteamMatches.s_OnDataUpdate != null)
            {
                SteamMatches.s_OnDataUpdate.Invoke();
            }
        }

        public void OnLeaderboardDownloadedEntries(LeaderboardScoresDownloaded_t pLeaderboardScoresDownloaded, bool bIOFailure)
        {
            if (!bIOFailure)
            {
                SteamStats.s_nLeaderboardEntriesFound = Math.Min(pLeaderboardScoresDownloaded.m_cEntryCount, 1000);

                for (int index = 0; index < SteamStats.s_nLeaderboardEntriesFound; index++)
                {
                    SteamUserStats.GetDownloadedLeaderboardEntry(
                        pLeaderboardScoresDownloaded.m_hSteamLeaderboardEntries, index, out SteamStats.m_leaderboardEntries[index], null, 0);
                }
            }

            SteamStats.s_OnDownload.Invoke(bIOFailure);
        }

        public void OnLobbyCreated(LobbyCreated_t pCallback, bool bIOFailure)
        {
            if (!bIOFailure)
            {
                SteamMatches.s_CurrentLobby = new SteamLobby(pCallback.m_ulSteamIDLobby);
            }

            SteamMatches.s_OnCreateLobby.Invoke(bIOFailure);
        }


        //public STEAM_CALLBACK(CallbackClass, OnChatMsg, LobbyChatMsg_t, m_OnChatMsg)
        //{
        //    //
        //}

        // There are like a dozen more STEAM_CALLBACK things
    }

    public struct LeaderboardHandle
    {
        public SteamLeaderboard_t m_handle;

        public LeaderboardHandle(SteamLeaderboard_t handle)
        {
            m_handle = handle;
        }
    }

    public static class SteamCore
    {
        static bool s_bOffline;

        public static bool Initialize()
        {
            if (!SteamAPI.Init())
                return false;

            if (!SteamStats.Initialize())
                return false;

            return true;
        }

        public static bool InOfflineMode()
        {
            return s_bOffline;
        }

        public static UInt64 PlayerId()
        {
            if (InOfflineMode() || !SteamIsConnected())
            {
                // Return consistent but invalid Steam ID, for use throughout the application.
                return 12345;
            }

            return (UInt64)SteamUser.GetSteamID();
        }

        public static String PlayerName()
        {
            InteropHelp.TestIfAvailableClient();
            return SteamFriends.GetPersonaName();
        }

        public static bool RestartViaSteamIfNecessary(uint AppId)
        {
            bool result = SteamAPI.RestartAppIfNecessary((AppId_t)AppId);
            return result;
        }

        public static void SetOfflineMode(bool Offline)
        {
            s_bOffline = Offline;
        }

        public static void Shutdown()
        {
            SteamAPI.Shutdown();
        }

        public static bool SteamIsConnected()
        {
            return SteamIsRunning() && SteamUser.BLoggedOn();
        }

        public static bool SteamIsRunning()
        {
            return SteamAPI.IsSteamRunning();
        }

        public static void Update()
        {
            SteamAPI.RunCallbacks();
        }
    }

    public struct SteamLobby
    {
        public CSteamID m_handle;

        public SteamLobby(CSteamID handle)
        {
            m_handle = handle;
        }

        public SteamLobby(ulong handle)
        {
            m_handle = (CSteamID)handle;
        }
    }

    public static class StringHelper
    {
        public static byte[] GetBytes(string str)
        {
            int msgLength = str.Length * sizeof(char);

            byte[] bytes = new byte[msgLength + 4];
            var length = BitConverter.GetBytes(msgLength);

            System.Buffer.BlockCopy(length, 0, bytes, 0, 4);
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 4, msgLength);

            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            int length = BitConverter.ToInt32(bytes, 0);

            char[] chars = new char[length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 4, chars, 0, length);
            return new string(chars);
        }
    }

    public static class SteamMatches
    {
        public static Action<bool> s_OnFindLobbies;
        public static Action<bool> s_OnJoinLobby;
        public static Action<bool> s_OnCreateLobby;
        public static Action<int, UInt64> s_OnChatUpdate;
        public static Action<String, UInt64, String> s_OnChatMsg;
        public static Action s_OnDataUpdate;

        public static int s_nLobbiesFound = 0;
        public static int s_nFriendLobbiesFound = 0;

        public static SteamLobby s_CurrentLobby;
        public static CallResult<LeaderboardFindResult_t> g_CallResultFindLeaderboard;
        public static CallResult<LeaderboardScoresDownloaded_t> g_CallResultDownloadEntries;
        public static CallResult<LobbyMatchList_t> g_CallResultLobbyMatchList;
        public static CallResult<LobbyEnter_t> g_CallResultJoinLobby;
        public static CallResult<LobbyCreated_t> g_CallResultLobbyCreated;

        public static Callback<LobbyChatMsg_t> g_CallResultChatMsg;
        public static Callback<LobbyDataUpdate_t> g_CallResultDataUpdate;
        public static Callback<LobbyChatUpdate_t> g_CallResultChatUpdate;
        public static Callback<P2PSessionRequest_t> g_CallResultP2PSessionRequest;
        public static Callback<P2PSessionConnectFail_t> g_CallResultP2PSessionConnectFail;

        public static Dictionary<String, String> s_LocalLobbyData;
        const int nMaxFriendLobbies = 1000;
        static CSteamID[] m_friendLobbies = new CSteamID[nMaxFriendLobbies];

        public const int
            LobbyType_Public = 0,
            LobbyType_FriendsOnly = 1,
            LobbyType_Private = 2;

        public const int
            ChatMember_Entered = 1,      // This user has joined or is joining the chat room
            ChatMember_Left = 2,         // This user has left or is leaving the chat room
            ChatMember_Disconnected = 3, // User disconnected without leaving the chat first
            ChatMember_Kicked = 4,       // User kicked
            ChatMember_Banned = 5;       // User kicked and banned

        public static bool InLobby()
        {
            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return false;
            return false;
        }

        public static void FindLobbies(Action<bool> OnFind)
        {
            s_OnFindLobbies = OnFind;
            s_nLobbiesFound = 0;
            s_nFriendLobbiesFound = 0;

            //if ( SteamMatchmaking() == 0 ) return;
            InteropHelp.TestIfAvailableClient();

            var hSteamAPICall = SteamMatchmaking.RequestLobbyList();
            g_CallResultLobbyMatchList = new CallResult<LobbyMatchList_t>(CallbackClass.instance.OnFindLobbies);
            g_CallResultLobbyMatchList.Set(hSteamAPICall); //( hSteamAPICall, SteamStats.g_CallbackClassInstance, CallbackClass.OnFindLobbies );
        }

        public static void FindFriendLobbies(Action<bool> OnFind)
        {
            s_OnFindLobbies = OnFind;
            s_nLobbiesFound = 0;
            s_nFriendLobbiesFound = 0;

            //if ( SteamMatchmaking() == 0 ) return;
            InteropHelp.TestIfAvailableClient();

            int cFriends = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
            for (int i = 0; i < cFriends; i++)
            {
                FriendGameInfo_t friendGameInfo;
                CSteamID steamIDFriend = (CSteamID)SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
                if (SteamFriends.GetFriendGamePlayed(steamIDFriend, out friendGameInfo) && friendGameInfo.m_steamIDLobby.IsValid())
                {
                    m_friendLobbies[s_nFriendLobbiesFound++] = friendGameInfo.m_steamIDLobby;
                    SteamMatchmaking.RequestLobbyData(friendGameInfo.m_steamIDLobby);
                    //int cap = SteamMatchmaking().GetLobbyMemberLimit( friendGameInfo.m_steamIDLobby );
                    //Console.WriteLine("Found friend lobby with capacity {0}", cap);
                }
            }

            if (s_OnFindLobbies != null)
            {
                s_OnFindLobbies(false);
            }
        }

        public static int NumLobbies()
        {
            if (s_nFriendLobbiesFound > 0)
            {
                return s_nFriendLobbiesFound;
            }
            else
            {
                return s_nLobbiesFound;
            }
        }

        public static CSteamID GetLobby(int Index)
        {
            if (s_nFriendLobbiesFound > 0)
            {
                return m_friendLobbies[Index];
            }
            else
            {
                return (CSteamID)SteamMatchmaking.GetLobbyByIndex(Index);
            }
        }

        public static string GetLobbyData(int Index, string Key)
        {
            CSteamID steamIDLobby = GetLobby(Index);
            return SteamMatchmaking.GetLobbyData(steamIDLobby, Key);
        }

        public static void JoinCreatedLobby(
            Action<bool> OnJoinLobby,
            Action<int, UInt64> OnChatUpdate,
            Action<String, UInt64, String> OnChatMsg,
            Action OnDataUpdate)
        {
            if (SteamCore.InOfflineMode())
            {
                SetLobbyCallbacks(OnJoinLobby, OnChatUpdate, OnChatMsg, OnDataUpdate);
                s_OnJoinLobby.Invoke(false);
                return;
            }

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return;

            JoinLobby(s_CurrentLobby.m_handle, OnJoinLobby, OnChatUpdate, OnChatMsg, OnDataUpdate);
        }

        public static void JoinLobby(int Index,
            Action<bool> OnJoinLobby,
            Action<int, UInt64> OnChatUpdate,
            Action<String, UInt64, String> OnChatMsg,
            Action OnDataUpdate)
        {
            CSteamID steamIDLobby = GetLobby(Index);
            JoinLobby(steamIDLobby, OnJoinLobby, OnChatUpdate, OnChatMsg, OnDataUpdate);
        }

        public static void JoinLobby(CSteamID LobbyID,
            Action<bool> OnJoinLobby,
            Action<int, UInt64> OnChatUpdate,
            Action<String, UInt64, String> OnChatMsg,
            Action OnDataUpdate)
        {
            SetLobbyCallbacks(OnJoinLobby, OnChatUpdate, OnChatMsg, OnDataUpdate);

            SteamAPICall_t hSteamAPICall = (SteamAPICall_t)SteamMatchmaking.JoinLobby(LobbyID);
            g_CallResultJoinLobby.Set(hSteamAPICall);
        }

        public static void SetLobbyCallbacks(
            Action<bool> OnJoinLobby,
            Action<int, UInt64> OnChatUpdate,
            Action<String, UInt64, String> OnChatMsg,
            Action OnDataUpdate)
        {
            s_OnJoinLobby = OnJoinLobby;
            s_OnChatUpdate = OnChatUpdate;
            s_OnChatMsg = OnChatMsg;
            s_OnDataUpdate = OnDataUpdate;

            g_CallResultJoinLobby = new CallResult<LobbyEnter_t>(CallbackClass.instance.OnJoinLobby);
            g_CallResultChatUpdate = new Callback<LobbyChatUpdate_t>(CallbackClass.instance.OnChatUpdate);
            g_CallResultChatMsg = new Callback<LobbyChatMsg_t>(CallbackClass.instance.OnChatMsg);
            g_CallResultDataUpdate = new Callback<LobbyDataUpdate_t>(CallbackClass.instance.OnDataUpdate);
        }

        public static ELobbyType IntToLobbyType(int LobbyType)
        {
            ELobbyType type = ELobbyType.k_ELobbyTypePublic;

            switch (LobbyType)
            {
                case LobbyType_Public: type = ELobbyType.k_ELobbyTypePublic; break;
                case LobbyType_FriendsOnly: type = ELobbyType.k_ELobbyTypeFriendsOnly; break;
                case LobbyType_Private: type = ELobbyType.k_ELobbyTypePrivate; break;
            }

            return type;
        }

        public static void CreateLobby(Action<bool> OnCreateLobby, int LobbyType)
        {
            if (SteamCore.InOfflineMode())
            {
                s_LocalLobbyData = new Dictionary<String, String>();

                OnCreateLobby(false);

                return;
            }

            s_OnCreateLobby = OnCreateLobby;

            ELobbyType type = IntToLobbyType(LobbyType);

            InteropHelp.TestIfAvailableClient();
            SteamAPICall_t hSteamAPICall = (SteamAPICall_t)SteamMatchmaking.CreateLobby(type, 4);
            g_CallResultLobbyCreated = new CallResult<LobbyCreated_t>(CallbackClass.instance.OnLobbyCreated);
            g_CallResultLobbyCreated.Set(hSteamAPICall); //, SteamStats.g_CallbackClassInstance, CallbackClass.OnLobbyCreated );
        }

        public static void SetLobbyData(string Key, string Value)
        {
            if (SteamCore.InOfflineMode())
            {
                s_LocalLobbyData[Key] = Value;
                //REMOVE//SteamStats.g_CallbackClassInstance.OnDataUpdate(null);
                return;
            }

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return;

            SteamMatchmaking.SetLobbyData(s_CurrentLobby.m_handle, Key, Value);
        }

        public static string GetLobbyData(string Key)
        {
            if (SteamCore.InOfflineMode())
            {
                try
                {
                    return s_LocalLobbyData[Key];
                }
                catch (Exception e)
                {
                    return e.ToString();
                }
            }

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return "";

            return SteamMatchmaking.GetLobbyData(s_CurrentLobby.m_handle, Key);
        }

        public static int GetLobbyMemberCount(int Index)
        {
            CSteamID steamIDLobby = GetLobby(Index);
            return SteamMatchmaking.GetNumLobbyMembers(steamIDLobby);
        }

        public static int GetLobbyCapacity(int Index)
        {
            CSteamID steamIDLobby = GetLobby(Index);
            return SteamMatchmaking.GetLobbyMemberLimit(steamIDLobby);
        }

        public static bool SetLobbyMemberLimit(int MaxMembers)
        {
            if (SteamCore.InOfflineMode()) return false;
            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return false;

            return SteamMatchmaking.SetLobbyMemberLimit(s_CurrentLobby.m_handle, MaxMembers);
        }

        public static void SetLobbyType(int LobbyType)
        {
            if (SteamCore.InOfflineMode()) return;
            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return;

            ELobbyType type = IntToLobbyType(LobbyType);
            SteamMatchmaking.SetLobbyType(s_CurrentLobby.m_handle, type);
        }

        public static void SendChatMsg(string Msg)
        {
            if (SteamCore.InOfflineMode())
            {
                s_OnChatMsg.Invoke(Msg, SteamCore.PlayerId(), "player");
                return;
            }

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return;

            var bytes = StringHelper.GetBytes(Msg);
            SteamMatchmaking.SendLobbyChatMsg(s_CurrentLobby.m_handle, bytes, bytes.Length);
        }

        public static int GetLobbyMemberCount()
        {
            if (SteamCore.InOfflineMode())
            {
                return 1;
            }

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return -1;
            return SteamMatchmaking.GetNumLobbyMembers(s_CurrentLobby.m_handle);
        }

        public static String GetMemberName(int Index)
        {
            if (SteamCore.InOfflineMode())
            {
                return "Local player";
            }

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return "";

            CSteamID steamIDLobbyMember = (CSteamID)SteamMatchmaking.GetLobbyMemberByIndex(s_CurrentLobby.m_handle, Index);

            return SteamFriends.GetFriendPersonaName(steamIDLobbyMember);
        }

        public static UInt64 GetMemberId(int Index)
        {
            if (SteamCore.InOfflineMode())
            {
                return SteamCore.PlayerId();
            }

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return 0;

            CSteamID steamIDLobbyMember = (CSteamID)SteamMatchmaking.GetLobbyMemberByIndex(s_CurrentLobby.m_handle, Index);

            return (UInt64)steamIDLobbyMember;
        }

        public static bool IsLobbyOwner()
        {
            if (SteamCore.InOfflineMode()) return true;

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return false;
            return (CSteamID)SteamUser.GetSteamID() == (CSteamID)SteamMatchmaking.GetLobbyOwner(s_CurrentLobby.m_handle);
        }

        public static void LeaveLobby()
        {
            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return;

            SteamMatchmaking.LeaveLobby(s_CurrentLobby.m_handle);
            s_CurrentLobby.m_handle.Clear();
        }

        public static void SetLobbyJoinable(bool Joinable)
        {
            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return;

            SteamMatchmaking.SetLobbyJoinable(s_CurrentLobby.m_handle, Joinable);
        }
    }

    public static class SteamP2P
    {
        public static Action<UInt64> OnRequest;
        public static Action<UInt64> OnConnectionFail;

        public static void SendMessage(SteamPlayer User, String Message)
        {
            SendMessage(User.m_handle, Message);
        }

        public static void SendMessage(CSteamID User, String Message)
        {
            var bytes = StringHelper.GetBytes(Message);

            SteamGameServerNetworking.SendP2PPacket(User, bytes, (uint)bytes.Length, EP2PSend.k_EP2PSendReliable, 0);
        }

        public static void SendBytes(SteamPlayer User, byte[] Bytes)
        {
            SendBytes(User.m_handle, Bytes);
        }

        public static void SendBytes(CSteamID User, byte[] Bytes)
        {
            uint len = (uint)Bytes.Length;

            byte[] pchMsg = new byte[len];
            for (int i = 0; i < len; i++)
            {
                pchMsg[i] = Bytes[i];
            }

            //int count = 0;
            //byte   pchMsg = new byte[len];
            //for (int i = 0; i < len; i++)
            //{
            //    if (Bytes[i] == 0) continue;

            //    pchMsg[count] = Bytes[i];
            //    count++;
            //}
            //len = count;

            SteamGameServerNetworking.SendP2PPacket(User, pchMsg, len, EP2PSend.k_EP2PSendReliable, 0);
        }

        public static bool MessageAvailable()
        {
            uint msgSize = 0;
            bool result = SteamNetworking.IsP2PPacketAvailable(out msgSize);

            return result;
        }

        public static Tuple<UInt64, String> ReadMessage()
        {
            UInt32 msgSize = 0;
            bool result = SteamGameServerNetworking.IsP2PPacketAvailable(out msgSize, 0);

            if (!result)
            {
                return null;
            }

            var packet = new byte[msgSize];
            string msg = "";
            CSteamID steamIDRemote;
            UInt32 bytesRead = 0;

            if (SteamGameServerNetworking.ReadP2PPacket(packet, msgSize, out bytesRead, out steamIDRemote, 0))
            {
                msg = packet.ToString();
            }

            return new Tuple<UInt64, String>((ulong)steamIDRemote.GetAccountID(), msg);
        }

        public static Tuple<UInt64, byte[]> ReadBytes()
        {
            UInt32 msgSize = 0;
            bool result = SteamGameServerNetworking.IsP2PPacketAvailable(out msgSize, 0);

            if (!result)
            {
                return null;
            }

            var packet = new byte[msgSize];
            CSteamID steamIDRemote;
            UInt32 bytesRead = 0;

            byte[] Bytes = new byte[msgSize];
            if (SteamGameServerNetworking.ReadP2PPacket(packet, msgSize, out bytesRead, out steamIDRemote, 0))
            {
                for (int i = 0; i < msgSize; i++)
                {
                    Bytes[i] = packet[i];
                }
            }

            return new Tuple<UInt64, byte[]>((UInt64)steamIDRemote, Bytes);
        }

        public static void SetOnP2PSessionRequest(Action<UInt64> OnRequest)
        {
            SteamP2P.OnRequest = OnRequest;
        }

        public static void SetOnP2PSessionConnectFail(Action<UInt64> OnConnectionFail)
        {
            SteamP2P.OnConnectionFail = OnConnectionFail;
        }

        public static void AcceptP2PSessionWithPlayer(SteamPlayer Player)
        {
            SteamGameServerNetworking.AcceptP2PSessionWithUser(Player.m_handle);
        }
    }

    public class SteamPlayer
    {
        public CSteamID m_handle;

        public SteamPlayer(CSteamID handle)
        {
            m_handle = handle;
        }

        public SteamPlayer(UInt64 handle)
        {
            m_handle = new CSteamID(handle);
        }
        public UInt64 Id()
        {
            return (UInt64)m_handle;
        }

        public String Name()
        {
            InteropHelp.TestIfAvailableClient();
            return SteamFriends.GetFriendPersonaName(m_handle);
        }
    }

    public class SteamStats
    {

        public static int s_nLeaderboardEntriesFound = 0;
        public static Action<LeaderboardHandle, bool> s_OnFind;
        public static Action<bool> s_OnDownload;

        const int nMaxLeaderboardEntries = 1000;
        static public LeaderboardEntry_t[] m_leaderboardEntries = new LeaderboardEntry_t[nMaxLeaderboardEntries];
        public static CallbackClass g_CallbackClassInstance;

        public static bool Initialize()
        {
            //REMOVE//g_CallbackClassInstance = new CallbackClass();

            // Is Steam loaded? If not we can't get stats.
            InteropHelp.TestIfAvailableClient();
            SteamAPI.Init();
            //if ( SteamUserStats() == 0 )
            ////if ( SteamUserStats() == 0 || SteamUser() == 0 )
            //{
            //    return false;
            //}
            // Is the user logged on?  If not we can't get stats.
            //if ( !SteamUser().BLoggedOn() )
            //{
            //    return false;
            //}
            // Request user stats.
            return SteamUserStats.RequestCurrentStats();
        }

        public bool GiveAchievement(string AchievementApiName)
        {
            return SteamUserStats.SetAchievement(AchievementApiName);
        }

        public int NumEntries(SteamLeaderboard_t hSteamLeaderboard)
        {
            return SteamUserStats.GetLeaderboardEntryCount(hSteamLeaderboard);
        }

        public int NumEntriesFound()
        {
            return s_nLeaderboardEntriesFound;
        }

        public void FindLeaderboard(string LeaderboardName, Action<LeaderboardHandle, bool> OnFind)
        {
            //marshal_context context;
            // char pchLeaderboardName = context.marshal_as<  char >( LeaderboardName );

            s_OnFind = OnFind;

            InteropHelp.TestIfAvailableClient();

            //if( SteamUserStats() == 0 )
            //{
            //    return;
            //}

            SteamAPICall_t hSteamAPICall = (SteamAPICall_t)SteamUserStats.FindLeaderboard(LeaderboardName);
            SteamMatches.g_CallResultFindLeaderboard = new CallResult<LeaderboardFindResult_t>(CallbackClass.instance.OnFindLeaderboard);
            SteamMatches.g_CallResultFindLeaderboard.Set(hSteamAPICall); //, SteamStats.g_CallbackClassInstance, CallbackClass.OnFindLeaderboard);
        }

        public void UploadScore(LeaderboardHandle Handle, int Value)
        {
            SteamAPICall_t hSteamAPICall = (SteamAPICall_t)SteamUserStats.UploadLeaderboardScore(Handle.m_handle, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, Value, null, 0);
        }

        public void RequestEntries(LeaderboardHandle Handle, int RequestType, int Start, int End, Action<bool> OnDownload)
        {
            s_OnDownload = OnDownload;

            // Request the specified leaderboard data.
            SteamAPICall_t hSteamAPICall = (SteamAPICall_t)SteamUserStats.DownloadLeaderboardEntries(
                Handle.m_handle, (ELeaderboardDataRequest)(RequestType), Start, End);

            // Register for the async callback
            SteamMatches.g_CallResultDownloadEntries = new CallResult<LeaderboardScoresDownloaded_t>(CallbackClass.instance.OnLeaderboardDownloadedEntries);
            SteamMatches.g_CallResultDownloadEntries.Set(hSteamAPICall); //, SteamStats.g_CallbackClassInstance, CallbackClass.OnLeaderboardDownloadedEntries);
        }

        public int Results_GetScore(int Index)
        {
            return m_leaderboardEntries[Index].m_nScore;
        }

        public int Results_GetRank(int Index)
        {
            return m_leaderboardEntries[Index].m_nGlobalRank;
        }

        public string Results_GetPlayer(int Index)
        {
            return SteamFriends.GetFriendPersonaName(m_leaderboardEntries[Index].m_steamIDUser);
        }

        public int Results_GetId(int Index)
        {
            return (int)m_leaderboardEntries[Index].m_steamIDUser.GetAccountID().m_AccountID;
        }
    }

    public class SteamTextInput
    {
        public static Action<bool> s_OnGamepadInputEnd;
        public string GetText()
        {
            uint cchText = SteamUtils.GetEnteredGamepadTextLength();

            string pchText;
            SteamUtils.GetEnteredGamepadTextInput(out pchText, cchText);

            return pchText;
        }

        public bool ShowGamepadTextInput(string Description, string InitialText, uint MaxCharacters, Action<bool> OnGamepadInputEnd)
        {
            s_OnGamepadInputEnd = OnGamepadInputEnd;

            //marshal_context context;
            // char pchDescription = context.marshal_as<  char >( Description );
            // char pchInitialText = context.marshal_as<  char >( InitialText );

            UInt64 unCharMax = (UInt64)(MaxCharacters);

            using (var pchDescription = new InteropHelp.UTF8StringHandle(Description))
            using (var pchInitialText = new InteropHelp.UTF8StringHandle(InitialText))
            {
                bool val = false; // delete this line after the REMOVE is taken out
                                  //REMOVE//bool val = SteamGameServerUtils_ShowGamepadTextInput(EGamepadTextInputMode.k_EGamepadTextInputModeNormal, EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine, pchDescription, unCharMax, pchInitialText);

                return val;
            }
        }
    }
}