using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Awards;

namespace CloudberryKingdom
{
    public class Awardment
    {
        public static string NewGameMode = "New Game Mode";
        public static string Default = "Awardment";

        public Localization.Words Name, Description;
        public string TitleType;
        public Hat Unlockable;
        public int Guid;

        /// <summary>
        /// An associated integer, usually representing a number the player must surpass to achieve the awardment.
        /// </summary>
        public int MyInt;

        /// <summary>
        /// Whether this award is a legitimate Achievement/Trophy/Awardment/etc, as defined by the relevant console.
        /// Unofficial awards are used to track progress without giving an official award to the player.
        /// </summary>
        public bool Official;

        /// <summary>
        /// Whether to show a custom message when the award is given.
        /// On Xbox there is a system level message for official awards, hence no additional message is needed.
        /// </summary>
        public bool ShowWhenAwarded;

        public Awardment(int Guid, Localization.Words Name, Localization.Words Description)
        {
            Official = true;
#if XBOX
            ShowWhenAwarded = false;
#else
            ShowWhenAwarded = true;
#endif

            this.Name = Name;
            this.Description = Description;
            this.Unlockable = Unlockable;
            this.Guid = Guid;
            this.TitleType = Default;

            if (this.Unlockable != null)
                this.Unlockable.AssociatedAward = this;

            Awardments.Awards.Add(this);
        }

        //public Awardment(int Guid, Localization.Words Name, Localization.Words Description, string TitleType, bool ShowWhenAwarded)
        public Awardment(int Guid, string Name, string Description, string TitleType, bool ShowWhenAwarded)
        {
            Official = false;

            //this.Name = Name;
            //this.Description = Description;
            this.Name = Localization.Words.None;
            this.Description = Localization.Words.None;

            this.Unlockable = Unlockable;
            this.Guid = Guid;
            this.TitleType = TitleType;
            this.ShowWhenAwarded = ShowWhenAwarded;

            if (this.Unlockable != null)
                this.Unlockable.AssociatedAward = this;

            Awardments.Awards.Add(this);
        }
    }

    public class Awardments
    {
        public static List<Awardment> Awards = new List<Awardment>();
        public static Dictionary<int, Awardment> AwardsDict = new Dictionary<int, Awardment>();

        #region Award Functionality
        /// <summary>
        /// Whether there is an awardment message on the screen currently.
        /// </summary>
        public static bool MessageOnScreen()
        {
            return Tools.CurGameData.MyGameObjects.Exists(obj => obj is AwardmentMessage);
        }

        /// <summary>
        /// If there is an awardment message on the screen currently,
        /// then return a positive amount to delay by.
        /// </summary>
        /// <returns>The amount to delay by.</returns>
        public static int AwardDelay()
        {
            if (MessageOnScreen())
                return 160;
            else
                return 0;
        }

        public static float CurShift = 0, Shift = 520;
        public static void GiveAward(Awardment award)
        {
            GiveAward(award, null);
        }
        public static void GiveAward(Awardment award, PlayerData player)
        {
            if (CloudberryKingdomGame.IsDemo) return;

            if (award == null) return;

            if (player == null && PlayerManager.NotAllAwarded(award) ||
                player != null && !player.Awardments[award.Guid])
            {
                // Give the award to each player, or to the specified player
                if (player == null)
                {
                    foreach (PlayerData p in PlayerManager.ExistingPlayers)
                        p.Awardments += award.Guid;
                }
                else
                    player.Awardments += award.Guid;

                // Show a note saying the reward was given
                if (award.ShowWhenAwarded)
                {
                    AwardmentMessage msg = new AwardmentMessage(award);
                    Tools.CurGameData.AddGameObject(msg);

                    // Remove all other hints
                    foreach (GameObject obj in Tools.CurGameData.MyGameObjects)
                    {
                        if (obj == msg) continue;

                        AwardmentMessage _msg = obj as AwardmentMessage;
                        if (null != _msg)
                            _msg.MyPile.Pos += new Vector2(0, Shift);
                    }
                }

                PlayerManager.SavePlayerData.Changed = true;
            }
        }
        #endregion

        #region Awardment function checks
        public static void CheckForAward_TimeCrisisUnlock(int Level, PlayerData player)
        {
            if (Level >= UnlockTimeCrisis.MyInt)
                GiveAward(UnlockTimeCrisis, player);
        }

        public static void CheckForAward_HeroRushUnlock(int Level, PlayerData player)
        {
            if (Level >= UnlockHeroRush.MyInt)
                GiveAward(UnlockHeroRush, player);
        }

        public static void CheckForAward_HeroRush2Unlock(int Level, PlayerData player)
        {
            if (Level >= UnlockHeroRush2.MyInt)
            {
                GiveAward(UnlockHeroRush2, player);
                CheckForAward_UnlockAllArcade();
            }
        }

        public static void CheckForAward_HeroRush2_Level(int Level)
        {
            if (Level >= 50)
                GiveAward(Awardments.Award_HeroRush2Level);
        }

        public static void CheckForAward_ArcadeScore(int Score)
        {
            if (Score >= 500000)
                GiveAward(Awardments.Award_ArcadeHighScore);
        }

        public static void CheckForAward_ArcadeScore2(int Score)
        {
            if (Score >= 1500000)
                GiveAward(Awardments.Award_ArcadeHighScore2);
        }

        public static void CheckForAward_Invisible(int Level)
        {
            if (Level < 20) return;

            if (PlayerManager.TotallyInvisible)
                GiveAward(Award_Invisible);
        }

        public static void CheckForAward_Die(Bob bob)
        {
            int deaths = bob.MyStats.TotalDeaths + PlayerManager.Get(bob).GameStats.TotalDeaths + PlayerManager.Get(bob).LifetimeStats.TotalDeaths;
            if (deaths >= 1337)
                GiveAward(Award_Die);
        }

        public static void CheckForAward_NoDeath(PlayerData player)
        {
            int deaths = player.CampaignStats.TotalDeaths;
            if (deaths == 0)
                GiveAward(Award_NoDeath, player);
        }

        public static void CheckForAward_UnlockAllArcade()
        {
            GiveAward(Award_UnlockAllArcade);
        }

        public static void CheckForAward_Save()
        {
            GiveAward(Award_Save);
        }

        public static void CheckForAward_Obstacles(Bob bob)
        {
            int obstacles = bob.MyStats.ObstaclesSeen + PlayerManager.Get(bob).GameStats.ObstaclesSeen + PlayerManager.Get(bob).LifetimeStats.ObstaclesSeen;

            if (obstacles >= 1000)
                GiveAward(Award_Obstacles);
        }

        public static void CheckForAward_Buy()
        {
            GiveAward(Award_Buy);
        }

        public static void CheckForAward_Replay(int Attempts)
        {
            if (Attempts >= 50)
                GiveAward(Award_Replay);
        }

        public static void CheckForAward_Bungee(GameData game)
        {
            if (game == null) return;
            if (game.MyLevel == null) return;
            if (game.MyLevel.MyLevelSeed == null) return;

            if (game.MyLevel.MyLevelSeed.MyGameFlags.IsTethered && PlayerManager.NumPlayers > 3)
            {
                foreach (PlayerData player in PlayerManager.AlivePlayers)
                    GiveAward(Award_Bungee, player);
            }
        }

        #endregion

        public static Awardment Award_Campaign1 = new Awardment(1, Localization.Words.AwardTitle_Campaign1, Localization.Words.AwardText_Campaign1);
        public static Awardment Award_ArcadeHighScore = new Awardment(2, Localization.Words.AwardTitle_ArcadeHighScore, Localization.Words.AwardText_ArcadeHighScore);
        public static Awardment Award_Bungee = new Awardment(3, Localization.Words.AwardTitle_Bungee, Localization.Words.AwardText_Bungee);
        public static Awardment Award_ArcadeHighScore2 = new Awardment(4, Localization.Words.AwardTitle_ArcadeHighScore2, Localization.Words.AwardText_ArcadeHighScore2);
        public static Awardment Award_Die = new Awardment(5, Localization.Words.AwardTitle_Die, Localization.Words.AwardText_Die);
        public static Awardment Award_Campaign3 = new Awardment(6, Localization.Words.AwardTitle_Campaign3, Localization.Words.AwardText_Campaign3);
        public static Awardment Award_Invisible = new Awardment(7, Localization.Words.AwardTitle_Invisible, Localization.Words.AwardText_Invisible);
        public static Awardment Award_Hats = new Awardment(8, Localization.Words.AwardTitle_Hats, Localization.Words.AwardText_Hats);
        public static Awardment Award_Campaign2 = new Awardment(9, Localization.Words.AwardTitle_Campaign2, Localization.Words.AwardText_Campaign2);
        public static Awardment Award_UnlockAllArcade = new Awardment(10, Localization.Words.AwardTitle_UnlockAllArcade, Localization.Words.AwardText_UnlockAllArcade);
        public static Awardment Award_NoDeath = new Awardment(11, Localization.Words.AwardTitle_NoDeath, Localization.Words.AwardText_NoDeath);
        public static Awardment Award_Save = new Awardment(12, Localization.Words.AwardTitle_Save, Localization.Words.AwardText_Save);
        public static Awardment Award_Obstacles = new Awardment(13, Localization.Words.AwardTitle_Obstacles, Localization.Words.AwardText_Obstacles);
        public static Awardment Award_Buy = new Awardment(14, Localization.Words.AwardTitle_Buy, Localization.Words.AwardText_Buy);
        public static Awardment Award_Campaign4 = new Awardment(15, Localization.Words.AwardTitle_Campaign4, Localization.Words.AwardText_Campaign4);
        public static Awardment Award_BuyHat = new Awardment(16, Localization.Words.AwardTitle_BuyHat, Localization.Words.AwardText_BuyHat);
        public static Awardment Award_HeroRush2Level = new Awardment(17, Localization.Words.AwardTitle_HeroRush2Level, Localization.Words.AwardText_HeroRush2Level);
        public static Awardment Award_Replay = new Awardment(18, Localization.Words.AwardTitle_Replay, Localization.Words.AwardText_Replay);
        public static Awardment Award_Campaign5 = new Awardment(19, Localization.Words.AwardTitle_Campaign5, Localization.Words.AwardText_Campaign5);
        
        // Arcade Unlocks
        public static Awardment UnlockTimeCrisis = new Awardment(100, "Time Crisis Unlocked!", 
            string.Format("{0} Levels in Escalation", TimeCrisis_LevelUnlock),
            Awardment.NewGameMode, false);

        public static Awardment UnlockHeroRush = new Awardment(101, "Hero Rush unlocked!",
            string.Format("Level {0} in Hero Rush", HeroRush_LevelUnlock),
            Awardment.NewGameMode, false);

        public static Awardment UnlockHeroRush2 = new Awardment(102, "Hero Rush 2 unlocked!",
            string.Format("Level {0} in Hero Rush 2", HeroRush2_LevelUnlock),
            Awardment.NewGameMode, false);

        const int TimeCrisis_LevelUnlock = 50;
        const int HeroRush_LevelUnlock = 250;
        const int HeroRush2_LevelUnlock = 500;

        public static void Init()
        {
            UnlockTimeCrisis.MyInt = TimeCrisis_LevelUnlock;
            UnlockHeroRush.MyInt = HeroRush_LevelUnlock;
            UnlockHeroRush2.MyInt = HeroRush2_LevelUnlock;

            foreach (Awardment award in Awards)
                AwardsDict.Add(award.Guid, award);
        }
    }
}
