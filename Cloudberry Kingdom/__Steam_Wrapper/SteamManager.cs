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

public static class SteamManager
{
    public static bool isInitialized = false;
    public static bool overlayIsUp = false;
    public static bool statsInit = false;


    // file read flags 
    // prevents reloading files from steam cloud
    // which can cause un-catchable DLL exceptions
    public static bool achievementsLoaded = false;
    public static bool SPDataLoaded = false;
    public static bool MPDataLoaded = false;
    public static bool weapon1Loaded = false;
    public static bool weapon2Loaded = false;
    public static bool weapon3Loaded = false;
    public static bool bankWeapon1Loaded = false;
    public static bool bankWeapon2Loaded = false;
    public static bool bankWeapon3Loaded = false;
    public static bool cgNEATSettingsLoaded = false;
    public static bool gameSettingsLoaded = false;
    public static bool keyMapLoaded = false;


    // steam ID and nickname
    public static UInt64 steamID = 0;
    public static string steamName = "";


    //--------------------------------------------------------------------
    // SteamInitialize()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    [return: MarshalAsAttribute(UnmanagedType.I1)]
    public static extern bool SteamInitialize();


    //--------------------------------------------------------------------
    // Shutdown()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    public static extern void SteamShutdown();


    //--------------------------------------------------------------------
    // GetSteamName()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    public static extern IntPtr GetSteamName();


    //--------------------------------------------------------------------
    // GetSteamID()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    public static extern UInt64 GetSteamID();


    //--------------------------------------------------------------------
    // GetCurrentStats()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    [return: MarshalAsAttribute(UnmanagedType.I1)]
    public static extern bool RequestCurrentStats();


    //--------------------------------------------------------------------
    // GetStatINT()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    public static extern int GetStatINT([MarshalAsAttribute(UnmanagedType.LPStr)] string statName);


    //--------------------------------------------------------------------
    // GetStatFLOAT()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    public static extern float GetStatFLOAT([MarshalAsAttribute(UnmanagedType.LPStr)] string statName);


    //--------------------------------------------------------------------
    // SetStatFLOAT()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    [return: MarshalAsAttribute(UnmanagedType.I1)]
    public static extern bool SetStatFLOAT([MarshalAsAttribute(UnmanagedType.LPStr)] string statName, float statValue);


    //--------------------------------------------------------------------
    // SetStatINT()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    [return: MarshalAsAttribute(UnmanagedType.I1)]
    public static extern bool SetStatINT([MarshalAsAttribute(UnmanagedType.LPStr)] string statName, int statValue);


    //--------------------------------------------------------------------
    // UpdateAverageStat()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    [return: MarshalAsAttribute(UnmanagedType.I1)]
    public static extern bool UpdateAverageStat([MarshalAsAttribute(UnmanagedType.LPStr)] string statName, float statValue, float timeInSecs);


    //--------------------------------------------------------------------
    // SetAchievement()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    [return: MarshalAsAttribute(UnmanagedType.I1)]
    public static extern bool SetAchievement([MarshalAsAttribute(UnmanagedType.LPStr)] string Achievementname);


    //--------------------------------------------------------------------
    // GetAchievement()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    [return: MarshalAsAttribute(UnmanagedType.I1)]
    public static extern bool GetAchievement([MarshalAsAttribute(UnmanagedType.LPStr)] string pchName, ref bool pbAchieved);


    //--------------------------------------------------------------------
    // SaveAllStatAndAchievementChanges()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    [return: MarshalAsAttribute(UnmanagedType.I1)]
    public static extern bool SaveAllStatAndAchievementChanges();


    //--------------------------------------------------------------------
    // FileExists()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    [return: MarshalAsAttribute(UnmanagedType.I1)]
    public static extern bool FileExists([MarshalAsAttribute(UnmanagedType.LPStr)] string pchFileName);


    //--------------------------------------------------------------------
    // GetFileSize()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    public static extern int GetFileSize([MarshalAs(UnmanagedType.LPStr)] string fileName);


    //--------------------------------------------------------------------
    // SaveFileOnRemoteStorage() - public
    //--------------------------------------------------------------------
    public static bool SaveFileOnRemoteStorage(string fileName, string fileContents)
    {
        System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
        byte[] saveStrBytes = encoder.GetBytes(fileContents);
        return SaveFileOnRemoteStorage(fileName, saveStrBytes, saveStrBytes.Length);
    }


    //--------------------------------------------------------------------
    // SaveFileOnRemoteStorage() - private
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    [return: MarshalAsAttribute(UnmanagedType.I1)]
    static extern bool SaveFileOnRemoteStorage([MarshalAs(UnmanagedType.LPStr)] string fileName,
                                               [MarshalAs(UnmanagedType.LPArray)] byte[] data,
                                               int bytesToWrite);


    //--------------------------------------------------------------------
    // GetFileOnRemoteStorage() - public
    //--------------------------------------------------------------------
    public static string GetFileOnRemoteStorage(string fileName)
    {
        int fileSize = SteamManager.GetFileSize(fileName);
        byte[] fileBytes = new byte[fileSize];
        SteamManager.GetFileOnRemoteStorage(fileName, fileBytes, fileSize);
        System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
        return encoder.GetString(fileBytes);
    }


    //--------------------------------------------------------------------
    // GetFileOnRemoteStorage() - private
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    static extern IntPtr GetFileOnRemoteStorage([MarshalAs(UnmanagedType.LPStr)] string fileName,
                                                [MarshalAs(UnmanagedType.LPArray)] byte[] data,
                                                int bytesToRead);


    //--------------------------------------------------------------------
    // ActivateOverlayFriends()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    public static extern void ActivateOverlayFriends();


    //--------------------------------------------------------------------
    // ActivateOverlayCommunity()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    public static extern void ActivateOverlayCommunity();


    //--------------------------------------------------------------------
    // ActivateOverlayPlayers()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    public static extern void ActivateOverlayPlayers();


    //--------------------------------------------------------------------
    // ActivateOverlaySettings()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    public static extern void ActivateOverlaySettings();


    //--------------------------------------------------------------------
    // ActivateOverlayOfficialGameGroup()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    public static extern void ActivateOverlayOfficialGameGroup();


    //--------------------------------------------------------------------
    // ActivateOverlayAchievements()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    public static extern void ActivateOverlayAchievements();


    //--------------------------------------------------------------------
    // ActivateOverlayWebPage()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    public static extern void ActivateOverlayWebPage([MarshalAsAttribute(UnmanagedType.LPStr)] string url);


    //--------------------------------------------------------------------
    // SetOverlayPosition()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    public static extern void SetOverlayPosition(int corner);


    //--------------------------------------------------------------------
    // ActivateOverlayToGARStore()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    public static extern void ActivateOverlayToGARStore();


    //--------------------------------------------------------------------
    // IsOverlayEnabled()
    //--------------------------------------------------------------------
    [DllImportAttribute("GARSteamManager")]
    [return: MarshalAsAttribute(UnmanagedType.I1)]
    public static extern bool IsOverlayEnabled();


    //----------------------------------------------------------------
    // SaveAllGARPlayerStats()
    //----------------------------------------------------------------
    static string creditsStr = "credits";
    static string neuraliumStr = "neuralium";
    static string alienKillsStr = "alienKills";
    static string pirateKillsStr = "pirateKills";
    static string superEliteKillsStr = "superEliteKills";
    static string blobKillsStr = "blobKills";
    static string playerKillsStr = "playerKills";
    static string playerDeathsStr = "deaths";
    static string totalWeaponsPickedUpStr = "totalWeaponsPickedUp";
    static string totalWeaponsDroppedStr = "totalWeaponsDropped";
    static string totalShotsFiredStr = "totalShotsFired";
    static string highDefenseScore = "highDefenseScore";
    public static bool SaveAllGARPlayerStats()
    {
        bool ok = true;
        /*
        ok = SteamManager.SetStatINT(creditsStr, Globals.player.credits);
        ok = SteamManager.SetStatINT(neuraliumStr, Globals.player.neuralium);
        ok = SteamManager.SetStatINT(alienKillsStr, Globals.player.alienKills);
        ok = SteamManager.SetStatINT(pirateKillsStr, Globals.player.pirateKills);
        ok = SteamManager.SetStatINT(superEliteKillsStr, Globals.player.superEliteKills);
        ok = SteamManager.SetStatINT(blobKillsStr, Globals.player.blobKills);
        ok = SteamManager.SetStatINT(playerKillsStr, Globals.player.playerKills);
        ok = SteamManager.SetStatINT(playerDeathsStr, Globals.player.deaths);
        ok = SteamManager.SetStatINT(totalWeaponsPickedUpStr, (int)Globals.player.totalWeaponsPickedUp);
        ok = SteamManager.SetStatINT(totalWeaponsDroppedStr, (int)Globals.player.totalWeaponsDropped);
        ok = SteamManager.SetStatINT(totalShotsFiredStr, (int)Globals.player.totalShotsFired);
        ok = SteamManager.SetStatINT(highDefenseScore, (int)Globals.player.highScore);
        ok = SaveAllStatAndAchievementChanges();
         * */
        return ok;
    }


    //----------------------------------------------------------------
    // LoadAllAchievements() - load all achievements from Steam
    //----------------------------------------------------------------
    public static bool LoadAllAchievements()
    {
        /*
        if (SteamManager.achievementsLoaded) return true;
        else
        {
            try
            {
                AchievementManager.Clear();
                AchievementObjective ao;
                for (int i = 0; i < AchievementManager.maxAchievements; i++)
                {
                    //this function needs to take in the achievement variable name, not the descriptive user name
                    //the achievement name can be found on steamworks panel in the acheivements section
                    //i kept it aligned with the achievementObjective.tostring()
                    ao = (AchievementObjective)(i);
                    bool success = GetAchievement(ao.ToString(), ref AchievementManager.achievements[i]);
                }
                achievementsLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                MainMenuScreen.statusMsgString = ex.ToString();
                return false;
            }
        }*/
        return true;
    }
}
#endif