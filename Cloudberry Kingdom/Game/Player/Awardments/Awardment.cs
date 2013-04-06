using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

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

        public string Key;

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

        public Awardment(int Guid, string Key, Localization.Words Name, Localization.Words Description)
        {
            Official = true;

			if (CloudberryKingdomGame.FakeAwardments)
				ShowWhenAwarded = true;
			else
				ShowWhenAwarded = false;

            this.Key = Key;
            this.Name = Name;
            this.Description = Description;
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
			
			ShowWhenAwarded = false;

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

#if XBOX
        static void GiveAchievementCallback(IAsyncResult result)
        {
            SignedInGamer gamer = result.AsyncState as SignedInGamer;
            if (null == gamer) return;

            try
            {
                gamer.EndAwardAchievement(result);
            }
            catch (Exception e)
            {
                Tools.Write(e.Message);
            }
        }
#endif

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
                    var list = new List<PlayerData>(PlayerManager.ExistingPlayers);
                    foreach (PlayerData p in list)
                        p.Awardments += award.Guid;
                }
                else
                    player.Awardments += award.Guid;
				
#if XDK
                if (award.Official)
                {
                    foreach (var gamer in Gamer.SignedInGamers)
                    {
						if (PlayerManager.Players[(int)gamer.PlayerIndex] != null
							&& PlayerManager.Players[(int)gamer.PlayerIndex].Exists			   // Check player exists
							&& (player == null || gamer.PlayerIndex == player.MyPlayerIndex))  // Check this is the right player
						{
							SignedInGamer AwardedGamer = gamer;
							gamer.BeginAwardAchievement(award.Key, GiveAchievementCallback, AwardedGamer);
						}
                    }
                }
#endif

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
			{
				GiveAward(UnlockTimeCrisis, player);
				//Tools.CurGameData.AddGameObject(new HeroUnlockedMessage());
			}
        }

        public static void CheckForAward_HeroRushUnlock(int Level, PlayerData player)
        {
			if (Level >= UnlockHeroRush.MyInt)
			{
				GiveAward(UnlockHeroRush, player);
				//Tools.CurGameData.AddGameObject(new HeroUnlockedMessage());
			}
        }

        public static void CheckForAward_HeroRush2Unlock(int Level, PlayerData player)
        {
            if (Level >= UnlockHeroRush2.MyInt)
            {
                GiveAward(UnlockHeroRush2, player);

				// Give "The End of Infinity" award immediately if Hero Rush 2 is unlocked.
				GiveAward(Award_UnlockAllArcade, player);


				// Only give "The End of Infinity" award if player unlocks all heroes as well.
				/*
				int id, level;

				// Check we've gotten all Escalation heroes
				id = Challenge_Escalation.Instance.CalcGameId_Level(ArcadeMenu.HighestHero);
				level = PlayerManager.MaxPlayerHighScore(id);
				bool escalation_complete = level >= ArcadeMenu.HighestLevelNeeded;

				id = Challenge_Escalation.Instance.CalcGameId_Level(BobPhsxBouncy.Instance);
				level = PlayerManager.MaxPlayerHighScore(id);
				escalation_complete &= level >= 25;

				id = Challenge_Escalation.Instance.CalcGameId_Level(BobPhsxSmall.Instance);
				level = PlayerManager.MaxPlayerHighScore(id);
				escalation_complete &= level >= 50;

				id = Challenge_Escalation.Instance.CalcGameId_Level(BobPhsxWheel.Instance);
				level = PlayerManager.MaxPlayerHighScore(id);
				escalation_complete &= level >= 75;

				// Check we've gotten all Time Crisis heroes
				id = Challenge_TimeCrisis.Instance.CalcGameId_Level(ArcadeMenu.HighestHero);
				level = PlayerManager.MaxPlayerHighScore(id);
				bool timecrisis_complete = level >= ArcadeMenu.HighestLevelNeeded;

				id = Challenge_TimeCrisis.Instance.CalcGameId_Level(BobPhsxBouncy.Instance);
				level = PlayerManager.MaxPlayerHighScore(id);
				timecrisis_complete &= level >= 25;

				id = Challenge_TimeCrisis.Instance.CalcGameId_Level(BobPhsxSmall.Instance);
				level = PlayerManager.MaxPlayerHighScore(id);
				timecrisis_complete &= level >= 50;

				id = Challenge_TimeCrisis.Instance.CalcGameId_Level(BobPhsxWheel.Instance);
				level = PlayerManager.MaxPlayerHighScore(id);
				timecrisis_complete &= level >= 75;

				// Give award for unlocking everything
				if (escalation_complete && timecrisis_complete)
					GiveAward(Award_UnlockAllArcade, player);
				 */
                
				//CheckForAward_UnlockAllArcade();
				//Tools.CurGameData.AddGameObject(new HeroUnlockedMessage());
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
                GiveAward(Award_Die, bob.MyPlayerData);
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
                GiveAward(Award_Obstacles, bob.MyPlayerData);
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

            if (game.MyLevel.MyLevelSeed.MyGameFlags.IsTethered &&
                PlayerManager.NumPlayers > 3)
            {
                int AliveCount = 0;
                foreach (Bob bob in game.MyLevel.Bobs)
                {
                    if (!bob.Dead && !bob.Dying)
                        AliveCount++;
                }

                if (AliveCount != 1) return;

                foreach (Bob bob in game.MyLevel.Bobs)
                    if (!bob.Dead && !bob.Dying)
                        GiveAward(Award_Bungee, bob.MyPlayerData);
            }
        }

        #endregion

        public static Awardment Award_Campaign1 = new Awardment(1, "Award_Campaign1", Localization.Words.AwardTitle_Campaign1, Localization.Words.AwardText_Campaign1);
        public static Awardment Award_ArcadeHighScore = new Awardment(2, "Award_ArcadeHighScore", Localization.Words.AwardTitle_ArcadeHighScore, Localization.Words.AwardText_ArcadeHighScore);
        public static Awardment Award_Bungee = new Awardment(3, "Award_Bungee", Localization.Words.AwardTitle_Bungee, Localization.Words.AwardText_Bungee);
        public static Awardment Award_ArcadeHighScore2 = new Awardment(4, "Award_ArcadeHighScore2", Localization.Words.AwardTitle_ArcadeHighScore2, Localization.Words.AwardText_ArcadeHighScore2);
        public static Awardment Award_Die = new Awardment(5, "Award_Die", Localization.Words.AwardTitle_Die, Localization.Words.AwardText_Die);
        public static Awardment Award_Campaign3 = new Awardment(6, "Award_Campaign3", Localization.Words.AwardTitle_Campaign3, Localization.Words.AwardText_Campaign3);
        public static Awardment Award_Invisible = new Awardment(7, "Award_Invisible", Localization.Words.AwardTitle_Invisible, Localization.Words.AwardText_Invisible);
        public static Awardment Award_Hats = new Awardment(8, "Award_Hats", Localization.Words.AwardTitle_Hats, Localization.Words.AwardText_Hats);
        public static Awardment Award_Campaign2 = new Awardment(9, "Award_Campaign2", Localization.Words.AwardTitle_Campaign2, Localization.Words.AwardText_Campaign2);
        public static Awardment Award_UnlockAllArcade = new Awardment(10, "Award_UnlockAllArcade", Localization.Words.AwardTitle_UnlockAllArcade, Localization.Words.AwardText_UnlockAllArcade);
        public static Awardment Award_NoDeath = new Awardment(11, "Award_NoDeath", Localization.Words.AwardTitle_NoDeath, Localization.Words.AwardText_NoDeath);
        public static Awardment Award_Save = new Awardment(12, "Award_Save", Localization.Words.AwardTitle_Save, Localization.Words.AwardText_Save);
        public static Awardment Award_Obstacles = new Awardment(13, "Award_Obstacles", Localization.Words.AwardTitle_Obstacles, Localization.Words.AwardText_Obstacles);
        public static Awardment Award_Buy = new Awardment(14, "Award_Buy", Localization.Words.AwardTitle_Buy, Localization.Words.AwardText_Buy);
        public static Awardment Award_Campaign4 = new Awardment(15, "Award_Campaign4", Localization.Words.AwardTitle_Campaign4, Localization.Words.AwardText_Campaign4);
        public static Awardment Award_BuyHat = new Awardment(16, "Award_BuyHat", Localization.Words.AwardTitle_BuyHat, Localization.Words.AwardText_BuyHat);
        public static Awardment Award_HeroRush2Level = new Awardment(17, "Award_HeroRush2Level", Localization.Words.AwardTitle_HeroRush2Level, Localization.Words.AwardText_HeroRush2Level);
        public static Awardment Award_Replay = new Awardment(18, "Award_Replay", Localization.Words.AwardTitle_Replay, Localization.Words.AwardText_Replay);
        
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
