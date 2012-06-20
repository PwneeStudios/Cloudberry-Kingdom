using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Awards;
using System.IO;

#if PC_VERSION
#elif XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
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

            writer.Write(ScreenSaver.WatchedIntro);
            writer.Write(HeroRush_Tutorial.WatchedOnce);
            writer.Write(Campaign_Chaos.WatchedOnce);

            writer.Write(Hints.QuickSpawn);
            writer.Write(Hints.YForHelp);

            /*
#if PC_VERSION
            writer.Write(PlayerManager.DefaultName);

            PlayerManager.Player.Write(writer);
#endif
             */

            // Locks
            writer.Write(Challenge_HeroRush.Instance.GetGoalMet());
            writer.Write(Challenge_HeroRush2.Instance.GetGoalMet());
            writer.Write(Challenge_Escalation.Instance.GetGoalMet());
            writer.Write(Challenge_Wheelie.Instance.GetGoalMet());
            writer.Write(Challenge_UpUp.Instance.GetGoalMet());
            writer.Write(Challenge_Construct.Instance.GetGoalMet());


            /*
            // Resolution information
#if WINDOWS
            writer.Write(ResolutionPreferenceSet);
            writer.Write(Tools.Fullscreen);

            if (ResolutionGroup.LastSetMode == null)
            {
                writer.Write(Tools.TheGame.graphics.PreferredBackBufferWidth);
                writer.Write(Tools.TheGame.graphics.PreferredBackBufferHeight);
            }
            else
            {
                writer.Write(ResolutionGroup.LastSetMode.Width);
                writer.Write(ResolutionGroup.LastSetMode.Height);
            }
#endif
             */
        }

        protected override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            ScreenSaver.WatchedIntro = reader.ReadBoolean();
            HeroRush_Tutorial.WatchedOnce = reader.ReadBoolean();
            if (CloudberryKingdomGame.AlwaysGiveTutorials)
                HeroRush_Tutorial.WatchedOnce = false;

            Campaign_Chaos.WatchedOnce = true;//reader.ReadBoolean();

            Hints.QuickSpawn = reader.ReadInt32();
            Hints.YForHelp = reader.ReadInt32();

            /*
#if PC_VERSION
            PlayerManager.DefaultName = reader.ReadString();
            if (PlayerManager.DefaultName.Length < 1)
                PlayerManager.DefaultName = "Stickman";

            PlayerManager.Player.Read(reader);
#endif
             */

            // Locks
            Challenge_HeroRush.Instance.SetGoalMet(reader.ReadBoolean());
            Challenge_HeroRush2.Instance.SetGoalMet(reader.ReadBoolean());
            Challenge_Escalation.Instance.SetGoalMet(reader.ReadBoolean());
            Challenge_Wheelie.Instance.SetGoalMet(reader.ReadBoolean());
            Challenge_UpUp.Instance.SetGoalMet(reader.ReadBoolean());
            Challenge_Construct.Instance.SetGoalMet(reader.ReadBoolean());

            /*
            // Resolution information
#if WINDOWS
            ResolutionPreferenceSet = reader.ReadBoolean();
            bool Fullscreen = reader.ReadBoolean();
            int Width = reader.ReadInt32();
            int Height = reader.ReadInt32();
#endif
             */
            //if (ResolutionPreferenceSet)
            //{
            //    Tools.TheGame.ToDo.Add(() =>
            //    {
            //        Tools.Fullscreen = Fullscreen;
            //        ResolutionGroup.Use(Width, Height);
            //    });
            //}
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
            //string fullpath = Tools.TheGame.Content.RootDirectory;
            //string fullpath = Path.GetDirectoryName(Globals.ContentDirectory);

            using (StreamWriter writer = new StreamWriter("Custom"))
            {
                //FileStream stream = File.Open(fullpath, FileMode.Create, FileAccess.Write, FileShare.None);
                //BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8);

                // Resolution information
                writer.WriteLine("// Custom resolution?");
                writer.WriteLine(SavePlayerData.ResolutionPreferenceSet);
                writer.WriteLine("// Full screen");
                writer.WriteLine(Tools.Fullscreen);
                if (ResolutionGroup.LastSetMode == null)
                {
                    writer.WriteLine("// Width");
                    writer.WriteLine(Tools.TheGame.graphics.PreferredBackBufferWidth);
                    writer.WriteLine("// Height");
                    writer.WriteLine(Tools.TheGame.graphics.PreferredBackBufferHeight);
                }
                else
                {
                    writer.WriteLine("// Width");
                    writer.WriteLine(ResolutionGroup.LastSetMode.Width);
                    writer.WriteLine("// Height");
                    writer.WriteLine(ResolutionGroup.LastSetMode.Height);
                }

                writer.WriteLine();

                // Secondary keys
                writer.WriteLine("// Quickspawn =");
                writer.WriteLine(ButtonString.KeyToString[ButtonCheck.Quickspawn_KeyboardKey.KeyboardKey]);
                writer.WriteLine();

                writer.WriteLine("// Start/Menu =");
                writer.WriteLine(ButtonString.KeyToString[ButtonCheck.Start_Secondary]);
                writer.WriteLine();

                writer.WriteLine("// Go/Select =");
                writer.WriteLine(ButtonString.KeyToString[ButtonCheck.Go_Secondary]);
                writer.WriteLine();

                writer.WriteLine("// Back/Cancel =");
                writer.WriteLine(ButtonString.KeyToString[ButtonCheck.Back_Secondary]);
                writer.WriteLine();

                writer.WriteLine("// Replay, Previous part =");
                writer.WriteLine(ButtonString.KeyToString[ButtonCheck.ReplayPrev_Secondary]);
                writer.WriteLine();

                writer.WriteLine("// Replay, Next part =");
                writer.WriteLine(ButtonString.KeyToString[ButtonCheck.ReplayNext_Secondary]);
                writer.WriteLine();

                writer.WriteLine("// Toggle (Replay, single/multi) (Slow-mo, toggles if activated) =");
                writer.WriteLine(ButtonString.KeyToString[ButtonCheck.SlowMoToggle_Secondary]);
                writer.WriteLine();

                writer.WriteLine("// Left =");
                writer.WriteLine(ButtonString.KeyToString[ButtonCheck.Left_Secondary]);
                writer.WriteLine();

                writer.WriteLine("// Right =");
                writer.WriteLine(ButtonString.KeyToString[ButtonCheck.Right_Secondary]);
                writer.WriteLine();

                writer.WriteLine("// Up =");
                writer.WriteLine(ButtonString.KeyToString[ButtonCheck.Up_Secondary]);
                writer.WriteLine();

                writer.WriteLine("// Down =");
                writer.WriteLine(ButtonString.KeyToString[ButtonCheck.Down_Secondary]);
            }
            //writer.Close();
            //stream.Close();
        }

        public static RezData LoadRezAndKeys()
        {
            //string fullpath = Tools.TheGame.Content.RootDirectory;

            RezData d;

            using (StreamReader reader = new StreamReader("Custom"))
            {
                // Resolution information
                d = new RezData();
                reader.ReadLine();
                d.Custom = bool.Parse(reader.ReadLine());
                reader.ReadLine();
                d.Fullscreen = bool.Parse(reader.ReadLine());
                reader.ReadLine();
                d.Width = int.Parse(reader.ReadLine());
                reader.ReadLine();
                d.Height = int.Parse(reader.ReadLine());

                reader.ReadLine();
                // Secondary keys
                reader.ReadLine();
                ButtonString.SetKeyFromString(ref ButtonCheck.Quickspawn_KeyboardKey.KeyboardKey, reader.ReadLine());
                reader.ReadLine();

                reader.ReadLine();
                ButtonString.SetKeyFromString(ref ButtonCheck.Start_Secondary, reader.ReadLine());
                reader.ReadLine();

                reader.ReadLine();
                ButtonString.SetKeyFromString(ref ButtonCheck.Go_Secondary, reader.ReadLine());
                reader.ReadLine();

                reader.ReadLine();
                ButtonString.SetKeyFromString(ref ButtonCheck.Back_Secondary, reader.ReadLine());
                reader.ReadLine();

                reader.ReadLine();
                ButtonString.SetKeyFromString(ref ButtonCheck.ReplayPrev_Secondary, reader.ReadLine());
                reader.ReadLine();

                reader.ReadLine();
                ButtonString.SetKeyFromString(ref ButtonCheck.ReplayNext_Secondary, reader.ReadLine());
                reader.ReadLine();

                reader.ReadLine();
                ButtonString.SetKeyFromString(ref ButtonCheck.SlowMoToggle_Secondary, reader.ReadLine());
                reader.ReadLine();

                reader.ReadLine();
                ButtonString.SetKeyFromString(ref ButtonCheck.Left_Secondary, reader.ReadLine());
                reader.ReadLine();

                reader.ReadLine();
                ButtonString.SetKeyFromString(ref ButtonCheck.Right_Secondary, reader.ReadLine());
                reader.ReadLine();

                reader.ReadLine();
                ButtonString.SetKeyFromString(ref ButtonCheck.Up_Secondary, reader.ReadLine());
                reader.ReadLine();

                reader.ReadLine();
                ButtonString.SetKeyFromString(ref ButtonCheck.Down_Secondary, reader.ReadLine());
            }

            return d;
        }
#endif
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
                if (Campaign.CurData != null)
                    Campaign.CurData.Score += Get(i).GameStats.Score;
                Get(i).LifetimeStats.Absorb(Get(i).GameStats);
                Get(i).CampaignStats.Absorb(Get(i).GameStats);
                Get(i).GameStats.Clean();
            }
        }

        // Random names
        public static string[] RandomNames = { "Honky Tonk", "Bosco", "Nuh Guck", "Short-shorts", "Itsy-bitsy", "Low Ball", "Cowboy Stu", "Capsaicin", "Hoity-toity", "Ram Bam", "King Kong", "Upsilon", "Omega", "Peristaltic Pump", "Jeebers", "Sugar Cane", "See-Saw", "Ink Blot", "Glottal Stop", "Olive Oil", "Cod Fish", "Flax", "Tahini", "Cotton Ball", "Sweet Justice", "Ham Sandwich", "Liverwurst", "Cumulus", "Oyster", "Klein", "Hippopotamus", "Bonobo", "Homo Erectus", "Australopithecine", "Quetzalcoatl", "Balogna", "Ceraunoscopy", "Shirley", "Susie", "Sally", "Sue", "Tyrannosaur", "Stick Man Chu", "Paragon", "Woodchuck", "Laissez Faire", "Ipso Facto", "Leviticus", "Berrylicious", "Elderberry", "Currant", "Blackberry", "Blueberry", "Strawberry", "Gooseberry", "Honeysuckle", "Nannyberry", "Hackberry", "Boysenberry", "Cloudberry", "Thimbleberry", "Huckleberry", "Bilberry", "Bearberry", "Mulberry", "Wolfberry", "Raisin", "Samson" };
        //"{s15,15}!{s15,15}", "{pHeart,60,?}", "{pStar,85,?}", "{pTree_large,55,?}", "{pController_X_big,40,?}" };

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
#elif XBOX_SIGNIN
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

        public static int Score_Blobs, Score_Coins, Score_Attempts;
        public static void CalcScore(StatGroup group)
        {
            if (group == StatGroup.Level)
                AbsorbTempStats();
            else if (group == StatGroup.Game)
            {
                AbsorbTempStats();
                AbsorbLevelStats();
            }

            Score_Attempts = Score_Blobs = Score_Coins = 0;

            for (int i = 0; i < 4; i++)
                if (PlayerManager.Get(i).Exists)
                {
                    PlayerStats stats = Get(i).GetStats(group);

                    Score_Coins += stats.Coins;
                    Score_Blobs += stats.Blobs;
                    Score_Attempts += stats.DeathsBy[(int)Bob.BobDeathType.Total];
                }
            if (group == StatGroup.Campaign)
                Score_Attempts = Campaign.Attempts;

            //AbsorbLevelStats();
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