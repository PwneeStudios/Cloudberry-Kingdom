using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#if PC_VERSION
#elif XDKX || XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
#endif

using CoreEngine;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class PlayerData : SaveLoad
    {
        public SavedSeeds MySavedSeeds;

        public PlayerIndex MyPlayerIndex;
        public bool Exists, IsAlive;

        public Set<int> Purchases;
        public Set<int> Awardments;

        public Dictionary<int, ScoreEntry> HighScores;

        public int CampaignLevel = 0, CampaignCoins = 0, CampaignIndex = 0;

        public int MyIndex;

        int RandomNameIndex;

        /// <summary>
        /// If this player data is or was associated with a gamer tag, this is the name of the gamer tag.
        /// </summary>
        public string StoredName = "";

        /// <summary>
        /// If true the player used a keyboard instead of a gamepad for the last movement input.
        /// </summary>
        public bool KeyboardUsedLast = false;

        public ColorScheme ColorScheme;
        public ColorScheme CustomColorScheme;
        public int ColorSchemeIndex = Unset.Int;

        public PlayerStats LifetimeStats, GameStats, LevelStats, TempStats;
        public PlayerStats CampaignStats;

#if PC_VERSION
#elif XBOX || XBOX_SIGNIN
        public SignedInGamer _MyGamer;
        public SignedInGamer MyGamer { get { return CheckForMatchingGamer(); } }
#endif

        public PlayerData()
        {
            AlwaysSave = true;
            FailLoad();
        }

        #region WriteRead
        public override void Serialize(BinaryWriter writer)
        {
            // Color scheme
            CustomColorScheme.WriteChunk_0(writer);
            Chunk.WriteSingle(writer, 1, ColorSchemeIndex);

            // High Scores
            foreach (var HighScore in HighScores.Values)
                HighScore.WriteChunk_1000(writer);

            // Awardments
            foreach (int guid in Awardments)
                Chunk.WriteSingle(writer, 2, guid);

            // Purchases
            foreach (int guid in Purchases)
                Chunk.WriteSingle(writer, 3, guid);

            // Stats
            LifetimeStats.WriteChunk_4(writer);

            // Save seed
            MySavedSeeds.WriteChunk_5(writer);

            // Campaign (Chunks 100 and up)
            Chunk.WriteSingle(writer, 100, CampaignCoins);
            Chunk.WriteSingle(writer, 101, CampaignLevel);
            Chunk.WriteSingle(writer, 102, CampaignIndex);

            // Player Level
            Chunk.WriteSingle(writer, 23023, LastPlayerLevelUpload);

            // Sound settings
            Chunk.WriteSingle(writer, 84001, Tools.MusicVolume.Val);
            Chunk.WriteSingle(writer, 84002, Tools.SoundVolume.Val);
            Chunk.WriteSingle(writer, 84003, (int)Localization.CurrentLanguage.MyLanguage);

            // Player ID
            if (MyGamer != null)
            {
#if XDK
                ulong xuid = MyGamer.GetXuid();
                string gamertag = MyGamer.Gamertag;
#else
				ulong xuid = 0;
				string gamertag = "";
#endif
                Tools.Write("Saving xuid " + xuid);
                Tools.Write("Saving gamertag " + gamertag);

                Chunk.WriteSingle(writer, 12001, xuid);
                Chunk.WriteSingle(writer, 12002, gamertag);
            }
            else
            {
                // This should never happen
                Tools.Nothing();
            }

#if CAFE
#else
			ScoreDatabase.Instance.Serialize(writer);
			PlayerManager.SavePlayerData.Serialize(writer);
#endif
        }

        protected override void FailLoad()
        {
            MySavedSeeds = new SavedSeeds();
            
            HighScores = new Dictionary<int, ScoreEntry>();
            
            Purchases = new Set<int>();
            Awardments = new Set<int>();
            
            CampaignIndex = CampaignLevel = CampaignCoins = 0;

            if (TempStats != null) TempStats.Clean();
            if (LevelStats != null) LevelStats.Clean();
            if (GameStats != null) GameStats.Clean();
            if (LifetimeStats != null) LifetimeStats.Clean();
            if (CampaignStats != null) CampaignStats.Clean();
        }

        public override void Deserialize(byte[] Data)
        {
			// Clear everything before loading
			MySavedSeeds.SeedStrings.Clear();
			HighScores.Clear();

            foreach (Chunk chunk in Chunks.Get(Data))
            {
				ProcessChunk(chunk);

#if CAFE
#else
				ScoreDatabase.ProcessChunk(chunk);
				_SavePlayerData.ProcessChunk(chunk);
#endif
            }

            // If this loaded data belongs to another user, silently reset the data to defaults.
            if (BelongsToAnotherPlayer())
            {
                FailLoad();
            }

            //// Cheat: Give all unlocks to player
            //int[] l = new int[] { 7777, 9999, 10000, 10100, 11500, 10200, 10400, 10500, 11000, 10300, 11100, 10900, 11200, 11300, 11400, 10001, 10101, 11501, 10201, 10401, 10501, 11001, 10301, 11101, 10901, 11201, 11301, 11401, 10002, 10003 };
            //foreach (int id in l)
            //{
            //    AddHighScore(new ScoreEntry(MyGamer.Gamertag, id, 151, 151, 151, 151, 151, 151));
            //}

            //CampaignIndex = 326;
            //CampaignLevel = 321;
            //CampaignCoins = 10000;
            //Awardments += 100;
            //Awardments += 101;
            //Awardments += 102;
        }

        bool BelongsToAnotherPlayer()
        {
            if (MyGamer == null)
            {
                // Should never happen
                Tools.Nothing();
                return false;
            }
            else
            {
#if XDK
                ulong Actual_Xuid = MyGamer.GetXuid();
                string Actual_Gamertag = MyGamer.Gamertag;
#else
                ulong Actual_Xuid = 0;
                string Actual_Gamertag = "";
#endif

                // If both Xuid and Gamertag are different, assume this data belongs to another user.
                if (Actual_Xuid != LoadedId_Xuid && Actual_Gamertag != LoadedId_Gamertag)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

		private void ProcessChunk(Chunk chunk)
		{
			switch (chunk.Type)
			{
				// Color scheme
				case 0:
					CustomColorScheme.ReadChunk_0(chunk);
					ColorScheme = CustomColorScheme;
					break;

				case 1:
					chunk.ReadSingle(ref ColorSchemeIndex);

					if (ColorSchemeIndex == Unset.Int) ColorSchemeIndex = 0;
					if (ColorSchemeIndex >= 0)
						ColorScheme = ColorSchemeManager.ColorSchemes[ColorSchemeIndex];

					break;

				// High Scores
				case 1000: var score = new ScoreEntry(); score.ReadChunk_1000(chunk); AddHighScore(score); break;

				// Awardments
				case 2: Awardments += chunk.ReadInt(); break;

				// Purchases
				case 3: Purchases += chunk.ReadInt(); break;

				// Stats
				case 4: LifetimeStats.ReadChunk_4(chunk); break;

				// Saved Seeds
				case 5: MySavedSeeds.ReadChunk_5(chunk); break;

				// Campaign (Chunks 100 and up)
				case 100: CampaignCoins = chunk.ReadInt(); break;
				case 101: CampaignLevel = chunk.ReadInt(); break;
				case 102: CampaignIndex = chunk.ReadInt(); break;

                 // Player Level
                case 23023: LastPlayerLevelUpload = chunk.ReadInt(); break;

                // Sound settings
                case 84001: Tools.MusicVolume.Val = chunk.ReadFloat(); break;
                case 84002: Tools.SoundVolume.Val = chunk.ReadFloat(); break;
                case 84003:
                    int _language = chunk.ReadInt();
                    Localization.Language language = (Localization.Language)_language;
					// // This sets the language upon load, which causes a lot of confusion.
                    //Tools.AddToDo(() => Localization.SetLanguage(language));
                    break;

                case 12001: LoadedId_Xuid = chunk.ReadULong(); break;
                case 12002: LoadedId_Gamertag = chunk.ReadString(); break;
			}
		}

        ulong LoadedId_Xuid = 0;
        string LoadedId_Gamertag = null;

        #endregion

        public int LastPlayerLevelUpload = -1;

        public int GetTotalLevel()
        {
            return GetTotalArcadeLevel() + GetTotalCampaignLevel() + 1;
        }

        public int GetTotalCampaignLevel()
        {
            return CampaignLevel;
        }

        public int GetTotalCampaignIndex()
        {
            return CampaignIndex;
        }

        public int GetTotalArcadeLevel()
        {
            int level = 0;
            int id;

            foreach (var game in ArcadeMenu.HeroArcadeList)
            {
                id = Challenge_Escalation.Instance.CalcGameId_Level(game.Item1);
                if (HighScores.ContainsKey(id))
                    level += HighScores[id].Value;

                id = Challenge_TimeCrisis.Instance.CalcGameId_Level(game.Item1);
                if (HighScores.ContainsKey(id))
                    level += HighScores[id].Value;

                id = Challenge_HeroRush.Instance.CalcGameId_Level(game.Item1);
                if (HighScores.ContainsKey(id))
                    level += HighScores[id].Value;

                id = Challenge_HeroRush2.Instance.CalcGameId_Level(game.Item1);
                if (HighScores.ContainsKey(id))
                    level += HighScores[id].Value;
            }

            return level;
        }

        public int GetHighScore(int GameId)
        {
            if (HighScores.ContainsKey(GameId))
                return HighScores[GameId].Value;
            else
                return 0;
        }

        public void AddHighScore(ScoreEntry score)
        {
            if (HighScores.ContainsKey(score.GameId) && score.Value < HighScores[score.GameId].Value)
                return;

            HighScores.AddOrOverwrite(score.GameId, score);

            // Mark this object as changed, so that it will be saved to disk.
            Changed = true;
        }

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

        public int Bank()
        {
            return LifetimeStats.Coins - LifetimeStats.CoinsSpentAtShop;
        }

#if XBOX || XBOX_SIGNIN
		public SignedInGamer CheckForMatchingGamer()
        {
            _MyGamer = null;

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

        public String GetName()
        {
#if XBOX || XBOX_SIGNIN
            if (MyGamer != null)
                return MyGamer.Gamertag;
            else
#endif
            if (StoredName.Length > 0)
                return StoredName;
            else
            {
				//if (RandomNameIndex == -1)
				//    RandomNameIndex = Tools.GlobalRnd.RndInt(0, PlayerManager.RandomNames.Length - 1);
				//return PlayerManager.RandomNames[RandomNameIndex];
				switch ( MyIndex )
				{
					case 0: return "Player 1";
					case 1: return "Player 2";
					case 2: return "Player 3";
					case 3: return "Player 4";
					default: return "Players";
				}
            }
        }

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
        }
    }
}