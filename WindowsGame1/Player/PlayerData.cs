using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#if PC_VERSION
#elif XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
#if PC_VERSION
    public class PlayerData
    {
#else
    public class PlayerData : SaveLoad
    {
        public PlayerData()
        {
            AlwaysSave = true;
        }

        protected override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            Write(writer);
        }
        protected override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            Read(reader);
        }
#endif
        public static int version = 1;

        public void Write(BinaryWriter writer)
        {
            // Version
            writer.Write(version);

            // Color scheme
            CustomColorScheme.Write(writer);
            writer.Write(ColorSchemeIndex);

            // Awardments
            writer.Write(Awardments.Count);
            foreach (int guid in Awardments)
                writer.Write(guid);

            // Purchases
            writer.Write(Purchases.Count);
            foreach (int guid in Purchases)
                writer.Write(guid);

            // Stats
            LifetimeStats.Write(writer);
        }

        public void Read(BinaryReader reader)
        {
            return;

            // Version
            int LoadedVersion = reader.ReadInt32();

            // Color scheme
            CustomColorScheme.Read(reader);
            ColorScheme = CustomColorScheme;
            ColorSchemeIndex = reader.ReadInt32();
            if (ColorSchemeIndex == Unset.Int) ColorSchemeIndex = 0;
            if (ColorSchemeIndex >= 0)
                ColorScheme = ColorSchemeManager.ColorSchemes[ColorSchemeIndex];

            // Awardments
            int n = reader.ReadInt32();
            for (int i = 0; i < n; i++)
                Awardments += reader.ReadInt32();

            // Purchases
            n = reader.ReadInt32();
            for (int i = 0; i < n; i++)
                Purchases += reader.ReadInt32();

            // Stats
            LifetimeStats.Read(reader);
        }

        public int Bank()
        {
            return LifetimeStats.Coins - LifetimeStats.CoinsSpentAtShop;
        }

        public PlayerIndex MyPlayerIndex;
        public bool Exists, IsAlive;

        /// <summary>
        /// If true the player used a keyboard instead of a gamepad for the last movement input.
        /// </summary>
        public bool KeyboardUsedLast = false;

#if PC_VERSION
#elif XBOX_SIGNIN
        public Gamer _MyGamer;
        public Gamer MyGamer { get { return CheckForMatchingGamer(); } }
#endif

        public ColorScheme ColorScheme;
        public ColorScheme CustomColorScheme;
        public int ColorSchemeIndex = Unset.Int;

        public PlayerStats LifetimeStats, GameStats, LevelStats, TempStats;

        public PlayerStats CampaignStats;
        
        public PlayerStats Stats { get { return LevelStats; } }
        public PlayerStats GetStats(StatGroup group)
        {
            switch (group) {
                case StatGroup.Temp: return TempStats;
                case StatGroup.Level: return LevelStats;
                case StatGroup.Game: return GameStats;
                case StatGroup.Lifetime: return LifetimeStats;
                case StatGroup.Campaign: return CampaignStats;
                default: return null; }
        }

        public PlayerStats GetSummedStats(StatGroup group)
        {
            if (group == StatGroup.Lifetime) return SumStats(StatGroup.Temp, StatGroup.Level, StatGroup.Game, StatGroup.Lifetime);
            if (group == StatGroup.Campaign) return SumStats(StatGroup.Temp, StatGroup.Level, StatGroup.Game, StatGroup.Campaign);
            if (group == StatGroup.Game) return SumStats(StatGroup.Temp, StatGroup.Level, StatGroup.Game);
            if (group == StatGroup.Level) return SumStats(StatGroup.Temp, StatGroup.Level);
            if (group == StatGroup.Temp) return SumStats(StatGroup.Temp);

            return null;
        }

        PlayerStats SumStats(params StatGroup[] group)
        {
            PlayerStats StatSum = new PlayerStats();
            
            StatSum.Clean();
            foreach (StatGroup g in group)
                StatSum.Absorb(GetStats(g));

            return StatSum;
        }

        /// <summary>
        /// Get the the players current score for the game,
        /// adding up the current level's score with the current game score.
        /// </summary>
        public int GetGameScore()
        {
            return GameStats.Score + LevelStats.Score;
        }

        /// <summary>
        /// Returns the total coins gotten in a level
        /// </summary>
        public int GetLevelCoins()
        {
            return LevelStats.Coins + TempStats.Coins;
        }

        public int RunningCampaignScore()
        {
            return CampaignStats.Score + GameStats.Score + LevelStats.Score + TempStats.Score;
        }

        public Set<int> Purchases = new Set<int>();
        public Set<int> Awardments = new Set<int>();

        public Dictionary<Guid, int> ChallengeStars = new Dictionary<Guid, int>();

        public int MyIndex;

        int RandomNameIndex;

#if PC_VERSION
        public String GetName()
        {
            return "Stickman";
        }
#else
#if XBOX_SIGNIN
        public Gamer CheckForMatchingGamer()
        {
            _MyGamer = null;
            //return _MyGamer;

            foreach (SignedInGamer gamer in Gamer.SignedInGamers)
                if ((int)gamer.PlayerIndex == MyIndex)
                    _MyGamer = gamer;

            if (_MyGamer != null)
                StoredName = _MyGamer.Gamertag;

            return _MyGamer;
        }
#endif
        /// <summary>
        /// Gets the color that the player's name should be drawn with.
        /// </summary>
        public Vector4 GetTextColor()
        {
            Vector4 clr = ColorScheme.SkinColor.DetailColor.ToVector4();
            if (clr.W == 0)
                clr = new Vector4(1, 1, 1, 1);
            return clr;
        }

        /// <summary>
        /// Sets the color of text to match the color of the player.
        /// </summary>
        public void SetNameText(EzText text)
        {
            //text.ShadowOffset = new Vector2(9, 9);
            //text.ShadowColor = Generic.PlayerColorSchemes[PlayerIndex].OutlineColor.Clr;
            text.MyFloatColor = GetTextColor();
            //text.OutlineColor = Generic.PlayerColorSchemes[PlayerIndex].OutlineColor.Clr.ToVector4();
        }

        /// <summary>
        /// If this player data is or was associated with a gamer tag, this is the name of the gamer tag.
        /// </summary>
        public string StoredName = "";

        public String GetName()
        {
#if XBOX_SIGNIN
            if (MyGamer != null)
                return MyGamer.Gamertag;
            else
#endif
            if (StoredName.Length > 0)
                return StoredName;
            else
            {
                if (RandomNameIndex == -1)
                    RandomNameIndex = Tools.GlobalRnd.RndInt(0, PlayerManager.RandomNames.Length - 1);
                return PlayerManager.RandomNames[RandomNameIndex];
            }
        }
#endif
        public void Init(int Index)
        {
            Init();

            MyIndex = Index;
            MyPlayerIndex = (PlayerIndex)MyIndex;
            ColorScheme = ColorSchemeManager.ColorSchemes[Index];
        }

        public void Init()
        {            
            RandomNameIndex = -1;

            TempStats = new PlayerStats();
            LevelStats = new PlayerStats();
            GameStats = new PlayerStats();
            LifetimeStats = new PlayerStats();

            CampaignStats = new PlayerStats();

        //    foreach (Challenge challenge in ChallengeList.Challenges)
        //        if (challenge.ID != Guid.Empty)
        //            ChallengeStars.Add(challenge.ID, 0);
        }
    }
}