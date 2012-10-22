using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using CloudberryKingdom.Awards;
using System.IO;
using CoreEngine;

#if PC_VERSION
#elif XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public static class UserPowers
    {
        /// <summary>
        /// Whether the user can skip the beginning of the screen saver.
        /// </summary>
        public static bool CanSkipScreensaver = false;

        /// <summary>
        /// Whether the user can skip the logo salad.
        /// </summary>
        public static bool CanSkipLogo = false;

        /// <summary>
        /// Set the value of a variable and make sure the variable is persisted to disk.
        /// </summary>
        public static void Set(ref bool variable, bool value)
        {
            variable = value;
            PlayerManager.SavePlayerData.Changed = true;
        }
    }

    public class _SavePlayerData : SaveLoad
    {
        public _SavePlayerData()
        {
#if PC_VERSION
            AlwaysSave = true;
#endif
        }

        /// <summary>
        /// When true the user has specified a preference for resolution (and fullscreen-ness)
        /// </summary>
        public bool ResolutionPreferenceSet = false;

        protected override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            
            Chunk.WriteSingle(writer, 0, UserPowers.CanSkipScreensaver);
            Chunk.WriteSingle(writer, 1, HeroRush_Tutorial.HasWatchedOnce);
            Chunk.WriteSingle(writer, 2, Hints.QuickSpawnNum);
            Chunk.WriteSingle(writer, 3, Hints.YForHelpNum);
            Chunk.WriteSingle(writer, 4, UserPowers.CanSkipLogo);
        }

        protected override void Deserialize(byte[] Data)
        {
            foreach (Chunk chunk in Chunks.Get(Data))
            {
                switch (chunk.Type)
                {
                    case 0: chunk.ReadSingle(ref UserPowers.CanSkipScreensaver); break;
                    case 1: chunk.ReadSingle(ref HeroRush_Tutorial.HasWatchedOnce); break;
                    case 2: chunk.ReadSingle(ref Hints.QuickSpawnNum); break;
                    case 3: chunk.ReadSingle(ref Hints.YForHelpNum); break;
                    case 4: chunk.ReadSingle(ref UserPowers.CanSkipLogo); break;
                }
            }
        }
    }

    public struct PlayerManager
    {
#if PC_VERSION || WINDOWS
        public struct RezData { public bool Custom, Fullscreen; public int Width, Height; }
#endif
#if PC_VERSION
        public static void SaveRezAndKeys()
        {
            EzStorage.Save("Settings", "Custom", _SaveRezAndKeys, null);
        }

        static void _SaveRezAndKeys(BinaryWriter writer)
        {
            Chunk.WriteSingle(writer, 0, SavePlayerData.ResolutionPreferenceSet);
            
            // Fullscreen
            Chunk.WriteSingle(writer, 1, Tools.Fullscreen);

            // Resolution
            if (ResolutionGroup.LastSetMode == null)
            {
                Chunk.WriteSingle(writer, 2, Tools.TheGame.MyGraphicsDeviceManager.PreferredBackBufferWidth);
                Chunk.WriteSingle(writer, 3, Tools.TheGame.MyGraphicsDeviceManager.PreferredBackBufferHeight);
            }
            else
            {
                Chunk.WriteSingle(writer, 2, ResolutionGroup.LastSetMode.Width);
                Chunk.WriteSingle(writer, 3, ResolutionGroup.LastSetMode.Height);
            }

            // Secondary keys
            Chunk.WriteSingle(writer, 4, ButtonCheck.Quickspawn_KeyboardKey.KeyboardKey);
            Chunk.WriteSingle(writer, 5, ButtonCheck.Start_Secondary);
            Chunk.WriteSingle(writer, 6, ButtonCheck.Go_Secondary);
            Chunk.WriteSingle(writer, 7, ButtonCheck.Back_Secondary);
            Chunk.WriteSingle(writer, 8, ButtonCheck.ReplayPrev_Secondary);
            Chunk.WriteSingle(writer, 9, ButtonCheck.ReplayNext_Secondary);
            Chunk.WriteSingle(writer,10, ButtonCheck.SlowMoToggle_Secondary);
            Chunk.WriteSingle(writer,11, ButtonCheck.Left_Secondary);
            Chunk.WriteSingle(writer,12, ButtonCheck.Right_Secondary);
            Chunk.WriteSingle(writer,13, ButtonCheck.Up_Secondary);
            Chunk.WriteSingle(writer,14, ButtonCheck.Down_Secondary);

            // Volume
            Chunk.WriteSingle(writer, 15, Tools.MusicVolume.Val);
            Chunk.WriteSingle(writer, 16, Tools.SoundVolume.Val);

            // Fixed time step setting
            Chunk.WriteSingle(writer, 17, Tools.FixedTimeStep);

            // Bordered window
            Chunk.WriteSingle(writer, 18, Tools.WindowBorder);
        }

        static RezData d;
        public static RezData LoadRezAndKeys()
        {
            EzStorage.Load("Settings", "Custom", _LoadRezAndKeys, null);

            return d;
        }

        static void _LoadRezAndKeys(byte[] Data)
        {
            d = new RezData();

            foreach (Chunk chunk in Chunks.Get(Data))
            {
                switch (chunk.Type)
                {
                    case 0: chunk.ReadSingle(ref d.Custom); break;
                    
                    // Fullscreen
                    case 1: chunk.ReadSingle(ref d.Fullscreen); break;
                    
                    // Resolution
                    case 2: chunk.ReadSingle(ref d.Width); break;
                    case 3: chunk.ReadSingle(ref d.Height); break;

                    // Secondary keys
                    case 4: chunk.ReadSingle(ref ButtonCheck.Quickspawn_KeyboardKey.KeyboardKey); break;
                    case 5: chunk.ReadSingle(ref ButtonCheck.Start_Secondary); break;
                    case 6: chunk.ReadSingle(ref ButtonCheck.Go_Secondary); break;
                    case 7: chunk.ReadSingle(ref ButtonCheck.Back_Secondary); break;
                    case 8: chunk.ReadSingle(ref ButtonCheck.ReplayPrev_Secondary); break;
                    case 9: chunk.ReadSingle(ref ButtonCheck.ReplayNext_Secondary); break;
                    case 10: chunk.ReadSingle(ref ButtonCheck.SlowMoToggle_Secondary); break;
                    case 11: chunk.ReadSingle(ref ButtonCheck.Left_Secondary); break;
                    case 12: chunk.ReadSingle(ref ButtonCheck.Right_Secondary); break;
                    case 13: chunk.ReadSingle(ref ButtonCheck.Up_Secondary); break;
                    case 14: chunk.ReadSingle(ref ButtonCheck.Down_Secondary); break;

                    // Volume
                    case 15: Tools.MusicVolume.Val = chunk.ReadFloat(); break;
                    case 16: Tools.SoundVolume.Val = chunk.ReadFloat(); break;

                    // Fixed time step setting
                    case 17: chunk.ReadSingle(ref Tools.FixedTimeStep); break;

                    // Bordered window
                    case 18: chunk.ReadSingle(ref Tools.WindowBorder); break;
                }
            }
        }
#endif
        public static bool PartiallyInvisible, TotallyInvisible;

        static int _CoinsSpent;
        public static int CoinsSpent { get { return _CoinsSpent; } set { _CoinsSpent = value; } }

        public static _SavePlayerData SavePlayerData;
#if PC_VERSION
        static string _DefaultName;
        public static string DefaultName
        {
            get { return _DefaultName; }
            set
            {
                if (value.CompareTo(_DefaultName) != 0) SavePlayerData.Changed = true;
                _DefaultName = value;
            }
        }
#endif

        public static void CleanTempStats()
        {
            for (int i = 0; i < 4; i++)
                Get(i).TempStats.Clean();
        }

        public static void AbsorbTempStats()
        {
            for (int i = 0; i < 4; i++)
            {
                Get(i).LevelStats.Absorb(Get(i).TempStats);
                Get(i).TempStats.Clean();
            }
        }

        public static void AbsorbLevelStats()
        {
            for (int i = 0; i < 4; i++)
            {
                Get(i).GameStats.Absorb(Get(i).LevelStats);
                Get(i).LevelStats.Clean();
            }
        }

        public static void AbsorbGameStats()
        {
            for (int i = 0; i < 4; i++)
            {
                Get(i).LifetimeStats.Absorb(Get(i).GameStats);
                Get(i).CampaignStats.Absorb(Get(i).GameStats);
                Get(i).GameStats.Clean();
            }
        }

        // Random names
        public static string[] RandomNames = { "Honky Tonk", "Bosco", "Nuh Guck", "Short-shorts", "Itsy-bitsy", "Low Ball", "Cowboy Stu", "Capsaicin", "Hoity-toity", "Ram Bam", "King Kong", "Upsilon", "Omega", "Peristaltic Pump", "Jeebers", "Sugar Cane", "See-Saw", "Ink Blot", "Glottal Stop", "Olive Oil", "Cod Fish", "Flax", "Tahini", "Cotton Ball", "Sweet Justice", "Ham Sandwich", "Liverwurst", "Cumulus", "Oyster", "Klein", "Hippopotamus", "Bonobo", "Homo Erectus", "Australopithecine", "Quetzalcoatl", "Balogna", "Ceraunoscopy", "Shirley", "Susie", "Sally", "Sue", "Tyrannosaur", "Stick Man Chu", "Paragon", "Woodchuck", "Laissez Faire", "Ipso Facto", "Leviticus", "Berrylicious", "Elderberry", "Currant", "Blackberry", "Blueberry", "Strawberry", "Gooseberry", "Honeysuckle", "Nannyberry", "Hackberry", "Boysenberry", "Cloudberry", "Thimbleberry", "Huckleberry", "Bilberry", "Bearberry", "Mulberry", "Wolfberry", "Raisin", "Samson" };

        public static int FirstPlayer = 0;
        public static bool HaveFirstPlayer;
        public static int GetFirstPlayer()
        {
            HaveFirstPlayer = true;
            if (Players[FirstPlayer].Exists) return FirstPlayer;
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (Players[i].Exists)
                    {
                        FirstPlayer = i;
                        return FirstPlayer;
                    }
                }
            }

            HaveFirstPlayer = false;
            return 0;
        }

        public static int NumPlayers = 1;
        public static PlayerData[] Players;

        /// <summary>
        /// Return a string representing the names of all players playing
        /// </summary>
        /// <returns></returns>
        public static string GetGroupGamerTag(int MaxLength)
        {
            List<PlayerData> players = LoggedInPlayers;
            if (players.Count == 0)
                players = ExistingPlayers;

            int N = players.Count;
            int CharLength = MaxLength - (N - 1); // The max number of characters, exlucing slashes

            // Get a list of all names
            List<StringBuilder> names = new List<StringBuilder>();
            players.ForEach(player => names.Add(new StringBuilder(player.GetName())));

            // A function to calculate the length of all names combined
            Func<int> length = () =>
                {
                    int count = 0;
                    names.ForEach(name => count += name.Length);
                    return count;
                };

            // Remove one character from the longest name until the total length is small enough
            while (length() > CharLength)
            {
                StringBuilder str = Tools.ArgMax(names, name => name.Length);
                str.Remove(str.Length - 1, 1);
            }

            // Concatenate the names together
            string GroupTag = "";
            //foreach (StringBuilder str in names)
            for (int i = 0; i < names.Count; i++)
            {
                StringBuilder str = names[i];
                PlayerData player = players[i];

                //string clr = EzText.ColorToCode(new Color(player.GetTextColor()));

                // Add '/' between player names
                if (GroupTag.Length > 0)
                    GroupTag += '/';

                string name = str.ToString();
                //GroupTag += clr + name;
                GroupTag += name;
            }

            return GroupTag;
        }

        public static int MaxPlayerHighScore(int GameId)
        {
            int max = 0;
            foreach (PlayerData player in ExistingPlayers)
                max = Math.Max(max, player.GetHighScore(GameId));

            return max;
        }

        /// <summary>
        /// Returns true if any of the current players has been awarded the specified awardment.
        /// </summary>
        public static bool Awarded(Awardment award)
        {
            return ExistingPlayers.Any(player => player.Awardments[award.Guid]);
        }

        /// <summary>
        /// Returns true if any of the current players has been bought the specified hat.
        /// </summary>
        public static bool Bought(Buyable item)
        {
            return ExistingPlayers.Any(player => player.Purchases[item.GetGuid()]);
        }

        /// <summary>
        /// Returns true if any of the current players has been bought the specified hat, or it's free.
        /// </summary>
        public static bool BoughtOrFree(Buyable item)
        {
            return item.GetPrice() == 0 || ExistingPlayers.Any(player => player.Purchases[item.GetGuid()]);
        }

        /// <summary>
        /// The combined bank accounts of all current players.
        /// </summary>
        public static int CombinedBank()
        {
            return PlayerSum(p => p.Bank());
        }

        public static void DeductCost(int Cost)
        {
            if (Cost > CombinedBank()) return;

            int SafetyCounter = 0;

            int PlayerIndex = 0;
            while (Cost > 0)
            {
                SafetyCounter++; if (SafetyCounter > 1000000) return;

                // Deduct one coin from each player at a time, so long as they can afford it.
                PlayerData p = Players[PlayerIndex];
                if (p.Exists && p.Bank() > 0)
                {
                    p.LifetimeStats.CoinsSpentAtShop++;
                    Cost--;
                }

                PlayerIndex++; if (PlayerIndex > 3) PlayerIndex = 0;
            }
        }

        public static void GiveBoughtItem(Buyable buyable)
        {
            if (buyable == null) return;

            // Give the hat to each player
            foreach (PlayerData p in ExistingPlayers)
                p.Purchases += buyable.GetGuid();

            SavePlayerData.Changed = true;
        }

        /// <summary>
        /// Returns true if any of the current players has NOT been awarded the specified awardment.
        /// </summary>
        public static bool NotAllAwarded(Awardment award)
        {
            return ExistingPlayers.Any(player => !player.Awardments[award.Guid]);
        }

        /// <summary>
        /// Returns the sum of all player's current game score.
        /// </summary>
        public static int GetGameScore()
        {
            int score = 0;
            foreach (PlayerData player in ExistingPlayers)
                score += player.GetGameScore();

            return score;
        }

        /// <summary>
        /// Returns the sum of all player's current game score.
        /// </summary>
        public static int GetGameScore_WithTemporary()
        {
            int score = 0;
            foreach (PlayerData player in ExistingPlayers)
                score += player.GetGameScore() + player.TempStats.Score;

            return score;
        }

        public static int PlayerSum(Func<PlayerData, int> f)
        {
            int sum = 0;
            foreach (PlayerData player in ExistingPlayers)
                sum += f(player);

            return sum;
        }

        public static int PlayerMax(Func<PlayerData, int> f)
        {
            int max = int.MinValue;
            foreach (PlayerData player in ExistingPlayers)
                max = Math.Max(max, f(player));

            return max;
        }

        /// <summary>
        /// Returns the total coins gotten in a level by all players.
        /// </summary>
        public static int GetLevelCoins()
        {
            int coins = 0;
            foreach (PlayerData player in ExistingPlayers)
                coins += player.GetLevelCoins();

            return coins;
        }

        /// <summary>
        /// A list of all players that exist and are logged in.
        /// </summary>
        public static List<PlayerData> LoggedInPlayers
        {
            get
            {
#if PC_VERSION
                return ExistingPlayers;
#elif XBOX || XBOX_SIGNIN
                return ExistingPlayers.FindAll(player => player.MyGamer != null || player.StoredName.Length > 0);
#else
                return ExistingPlayers;
#endif
            }
        }

        /// <summary>
        /// A list of all players currently existing.
        /// </summary>
        public static List<PlayerData> ExistingPlayers
        {
            get
            {
                _ExistingPlayers.Clear();
                foreach (PlayerData data in Players)
                    if (data.Exists)
                        _ExistingPlayers.Add(data);

                return _ExistingPlayers;
            }
        }
        public static List<PlayerData> _ExistingPlayers = new List<PlayerData>();

        /// <summary>
        /// A list of all players currently alive.
        /// </summary>
        public static List<PlayerData> AlivePlayers
        {
            get
            {
                _AlivePlayers.Clear();
                foreach (PlayerData data in Players)
                    if (data.Exists && data.IsAlive)
                        _AlivePlayers.Add(data);

                return _AlivePlayers;
            }
        }
        public static List<PlayerData> _AlivePlayers = new List<PlayerData>();

#if PC_VERSION
        public static PlayerData Player { get { return Players[0]; } }
#endif

        public static PlayerData Get(int i) { return Players[i]; }
        public static PlayerData Get(PlayerIndex Index) { return Players[(int)Index]; }
        public static PlayerData Get(Bob bob) { return Players[(int)bob.MyPlayerIndex]; }

        public static int Score_Blobs, Score_Coins, Score_Attempts, Score_Time;
        public static void CalcScore(StatGroup group)
        {
            if (group == StatGroup.Level)
                AbsorbTempStats();
            else if (group == StatGroup.Game)
            {
                AbsorbTempStats();
                AbsorbLevelStats();
            }

            Score_Attempts = Score_Blobs = Score_Coins = Score_Time = 0;

            for (int i = 0; i < 4; i++)
                if (PlayerManager.Get(i).Exists)
                {
                    PlayerStats stats = Get(i).GetStats(group);

                    Score_Coins += stats.Coins;
                    Score_Blobs += stats.Blobs;
                    Score_Attempts += stats.DeathsBy[(int)Bob.BobDeathType.Total];
                    Score_Time = Math.Max(Score_Time, stats.TimeAlive);
                }
        }

        public static bool Showed_ShouldCheckOutWorlds = false;
        public static int Showed_ShouldLeaveLevel = 0, Showed_ShouldWatchComputer = 0;


        public static int GetNumPlayers()
        {
            NumPlayers = 0;
            for (int i = 0; i < 4; i++)
                if (Players[i].Exists)
                    NumPlayers++;

            return NumPlayers;
        }

        /// <summary>
        /// Whether all the players are dead.
        /// </summary>
        public static bool AllDead()
        {
            bool All = true;
            for (int i = 0; i < 4; i++)
                All = All && (!Players[i].IsAlive || !Players[i].Exists);

            return All;
        }

        /// <summary>
        /// Whether all the players are off the screen
        /// </summary>
        public static bool AllOffscreen()
        {
            bool All = true;
            foreach (Bob bob in Tools.CurLevel.Bobs)
                All = All && !Tools.CurLevel.MainCamera.OnScreen(bob.Core.Data.Position);

            return All;
        }

        public static bool IsAlive(PlayerIndex PIndex)
        {
            int Index = GetIndexFromPlayerIndex(PIndex);

            return Players[Index].IsAlive;
        }

        public static int GetIndexFromPlayerIndex(PlayerIndex PIndex)
        {
            for (int i = 0; i < 4; i++)
                if (Players[i].MyPlayerIndex == PIndex)
                    return i;

            throw(new Exception("PlayerIndex not found!"));
        }

        public static void KillPlayer(PlayerIndex PIndex)
        {
            int Index = GetIndexFromPlayerIndex(PIndex);

            Players[Index].IsAlive = false;
        }

        public static void ReviveBob(Bob bob)
        {
            bob.Dead = bob.Dying = false;

            RevivePlayer(bob.MyPlayerIndex);
        }

        public static void RevivePlayer(PlayerIndex PIndex)
        {
            int Index = GetIndexFromPlayerIndex(PIndex);

            Players[Index].IsAlive = true;
        }

        public static void Init()
        {
#if PC_VERSION
            _DefaultName = PlayerManager.RandomNames.Choose(Tools.GlobalRnd);
#endif

            Players = new PlayerData[4];
            ColorSchemeManager.InitColorSchemes();

            // Player templates
            for (int i = 0; i < 4; i++)
            {
                Players[i] = new PlayerData();
                Players[i].Init(i);
            }
        }
    }
}