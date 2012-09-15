using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Awards;

namespace CloudberryKingdom
{
    public class Awardment
    {
        public string Name, Description;
        public Hat Unlockable;
        public int Guid;

        public Awardment(int Guid, string Name, string Description, Hat Unlockable)
        {
            this.Name = Name;
            this.Description = Description;
            this.Unlockable = Unlockable;
            this.Guid = Guid;

            if (this.Unlockable != null)
                this.Unlockable.AssociatedAward = this;

            Awardments.Awards.Add(this);
        }
    }

    public class Awardments
    {
        public static List<Awardment> Awards = new List<Awardment>();
        public static Dictionary<int, Awardment> AwardsDict = new Dictionary<int, Awardment>();

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

        public static void CheckForAward_HoldForward()
        {
            bool Ran = 
                PlayerManager.ExistingPlayers.Exists(p =>
                {
                    PlayerStats stats = p.GetStats(StatGroup.Level);
                    float ratio = stats.FinalTimeSpentNotMoving / (float)stats.FinalTimeSpent;
                    if (ratio < .05f && stats.FinalTimeSpentNotMoving < 35)
                        return true;
                    else
                        return false;
                });

            bool Reqs = CustomLevel_GUI.IsMaxLength && CustomLevel_GUI.Difficulty >= 1 &&
                        PlayerManager.PlayerSum(p => p.GetStats(StatGroup.Level).Checkpoints) == 0 &&
                        Tools.CurGameData.Freeplay && Tools.CurGameData.DefaultHeroType == BobPhsxNormal.Instance;

            if (Ran && Reqs)
                GiveAward(HoldForwardFreeplay);
        }

        public static void CheckForAward_NoCoins()
        {
            bool NoCoins = PlayerManager.PlayerSum(p => p.GetStats(StatGroup.Level).Coins) == 0 &&
                           PlayerManager.PlayerMax(p => p.GetStats(StatGroup.Level).TotalCoins) > 5;

            bool Reqs = CustomLevel_GUI.IsMaxLength && CustomLevel_GUI.Difficulty >= 1 &&
                        PlayerManager.PlayerSum(p => p.GetStats(StatGroup.Level).Checkpoints) == 0 &&
                        Tools.CurGameData.Freeplay && Tools.CurGameData.DefaultHeroType == BobPhsxNormal.Instance;

            if (NoCoins && Reqs)
                GiveAward(NoCoinFreeplay);
        }

        static int HeroRushScore = 500000;
        public static void CheckForAward_HeroRush_Score(int Score)
        {
            if (Score > HeroRushScore)
                GiveAward(HeroRush_Score);
        }

        static int HeroRush2Score = 500000;
        public static void CheckForAward_HeroRush2_Score(int Score)
        {
            if (Score > HeroRush2Score)
                GiveAward(HeroRush2_Score);
        }

        static int HeroRush2_LevelUnlock = 35;
        public static void CheckForAward_HeroRush2Unlock(int Level)
        {
            //if (Level >= 3)
            if (Level >= HeroRush2_LevelUnlock)
                GiveAward(UnlockHeroRush2);
        }

        public static void CheckForAward_Escalation_Level(int Level)
        {
            if (Level >= 26.2f - 1)
                GiveAward(Escalation_Levels);
        }

        public static void CheckForAward_BeatCampaign(int Index)
        {
            GiveAward(BeatCampaign[Index]);
        }

        //static int FastCampaign_Minutes = 5;
        static int FastCampaign_Minutes = 20;
        public static void CheckForAward_FastCampaign(int Index)
        {
            if (Index != 2) return;
            
            if (Campaign.Time < FastCampaign_Minutes * 60 * 62)
                GiveAward(FastCampaign2);
        }

        public static void CheckForAward_EbenezerAbusiveCastle(int Index)
        {
            if (Index < 2) return;

            if (Campaign.Coins == Campaign.TotalCoins)
                GiveAward(AllCoinsAbusiveCastle);
        }

        public static void CheckForAward_PerfectEasyCastle(int Index)
        {
            if (Index < 0) return;

            if (Campaign.Coins == Campaign.TotalCoins && Campaign.Attempts == 0)
                GiveAward(PerfectEasyCastle);
        }

        public static void CheckForAward_NoDeathNormalCastle(int Index)
        {
            if (Index < 0) return;

            if (Campaign.Attempts == 0)
                GiveAward(NoDeathsNormalCastle);
        }

        public static void CheckForAward_PartiallyInvisible(int Index)
        {
            if (Index < 2) return;

            if (Campaign.PartiallyInvisible)
                GiveAward(PartiallyInvisibleCampaign);
        }

        public static void CheckForAward_TotallyInvisible(int Index)
        {
            if (Index < 0) return;

            if (Campaign.TotallyInvisible)
                GiveAward(TotallyInvisibleCampaign);
        }

        public const int LotsOfJumps = 10000;
        public static void CheckForAward_JumpAlot(Bob bob)
        {
            int jumps = bob.MyStats.Jumps + PlayerManager.Get(bob).GameStats.Jumps + PlayerManager.Get(bob).LifetimeStats.Jumps;
            if (jumps >= LotsOfJumps)
                GiveAward(JumpAlot);
        }

        public static float CurShift = 0, Shift = 520;
        public static void GiveAward(Awardment award)
        {
            if (award == null) return;

            if (PlayerManager.NotAllAwarded(award))
            {
                // Give the award to each player
                foreach (PlayerData p in PlayerManager.ExistingPlayers)
                    p.Awardments += award.Guid;

                // Show a note saying the reward was given
                //AwardmentMessage.Style = (AwardmentMessage.Style + 1) % 2;
                AwardmentMessage.Style = 0;
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

                PlayerManager.SavePlayerData.Changed = true;
            }
        }

        static string BeatStr = "Beat a classic castle\non";
        public static Awardment[] BeatCampaign = new Awardment[] {
            null,
            new Awardment(1, "Italian Plumbing", BeatStr + CampaignMenu.GetName(2), Hat.Toad),
            new Awardment(2, "Bubbly Bop", BeatStr + CampaignMenu.GetName(3), Hat.BubbleBobble),
            new Awardment(3, "Ghouls n' stickmen", BeatStr + CampaignMenu.GetName(4), Hat.Knight),
            new Awardment(4, "Gosu Master", BeatStr + CampaignMenu.GetName(5), Hat.Gosu) };
        public static Awardment JumpAlot = new Awardment(5, "Jumple-upagus", "Jump " + LotsOfJumps.ToString() + " times.", Hat.Bubble);

        public static Awardment HoldForwardFreeplay = new Awardment(6, "White Rabbit", "Beat a max length " + CampaignMenu.GetName(2) + "level, always holding forward. Classic hero. No checkpoints.", Hat.Cloud);
        public static Awardment HeroRush_Score = new Awardment(7, "Locked IN", "Score " + HeroRushScore + " points in Hero Rush.", Hat.FallingBlockHead);
        public static Awardment Escalation_Levels = new Awardment(8, "Iron Man", "Beat 26.2 levels in Escalation.", Hat.FireHead);

        public static Awardment FastCampaign2 = new Awardment(10, "Minute man", "Beat an" + CampaignMenu.GetName(3) + "castle in under " + FastCampaign_Minutes.ToString() + " minutes.", Hat.Pink);

        public static Awardment HeroRush2_Score = new Awardment(12, "Jack of all sticks", "Score " + HeroRush2Score + " points\nin Hero Rush 2:\n{c255,10,10,255}Revenge of the Double Jump.", Hat.Fedora);
        public static Awardment PartiallyInvisibleCampaign = new Awardment(13, "I HAVE NO FEET", "Beat an" + CampaignMenu.GetName(3) + "castle while invisible. Cape and hat recommended.", Hat.Ghost);
        public static Awardment TotallyInvisibleCampaign = new Awardment(14, "Mind Games", "Beat a" + CampaignMenu.GetName(1) + "castle while invisible, with no hat and no cape.", Hat.Brain);

        public static Awardment NoCoinFreeplay = new Awardment(15, "Chromotephobia", "Beat a max length " + CampaignMenu.GetName(2) + "level without grabbing a single coin. Classic hero. No checkpoints.", Hat.CheckpointHead);
        
        public static Awardment AllCoinsAbusiveCastle = new Awardment(16, "Ebenezer", "Grab every coin in\nan" + CampaignMenu.GetName(3) + "castle.", Hat.TopHat);
        public static Awardment NoDeathsNormalCastle = new Awardment(17, "Untouchable", "Beat an" + CampaignMenu.GetName(2) + "castle without dying once.", Hat.Afro);
        public static Awardment PerfectEasyCastle = new Awardment(18, "Perfection", "Grab every coin in a" + CampaignMenu.GetName(1) + "castle without dying once. Image is everything.", Hat.Halo);

        public static Awardment UnlockHeroRush2 = new Awardment(100, "Hero Rush 2 unlocked!",
            string.Format("{0}Required:{1}\n   Level {3} in {2}Hero Rush", EzText.ColorToMarkup(new Color(205, 10, 10)), EzText.ColorToMarkup(Color.White), EzText.ColorToMarkup(new Color(26, 178, 231)), HeroRush2_LevelUnlock),
            null);

        public static void Init()
        {
            foreach (Awardment award in Awards)
                AwardsDict.Add(award.Guid, award);
        }
    }
}
