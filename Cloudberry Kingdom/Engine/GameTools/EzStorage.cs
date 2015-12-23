using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using CoreEngine;

#if PC
#elif XDKX || XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
using EasyStorage;
#endif

namespace CloudberryKingdom
{
    public class SaveGroup
    {
        static List<SaveLoad> ThingsToSave = new List<SaveLoad>();

        public static void Initialize()
        {
            ScoreDatabase.Initialize();
            PlayerManager.SavePlayerData = new _SavePlayerData();

#if PC
            PlayerManager.Player.ContainerName		= "PlayerData";
			if (CloudberryKingdomGame.UsingSteam)
			{
				UInt64 id = SteamManager.SteamCore.SteamID();

				if (id == UInt64.MaxValue)
				{
					PlayerManager.Player.FileName = "Player Data";

					try
					{
						// Couldn't find Steam ID, see if there are any Steam ID save files
						var files = Directory.EnumerateFiles(EzStorage.SaveDir(), "Player Data *", SearchOption.TopDirectoryOnly);
						foreach (var file in files)
						{
							PlayerManager.Player.FileName = file;
							break;
						}
					}
					catch
					{
					}
				}
				else
				{
					PlayerManager.Player.FileName		= "Player Data " + id;
				}
			}
			else
			{
				PlayerManager.Player.FileName		= "Player Data";
			}

			PlayerManager.Player.AlternateFileName	= "Player Data" ;
            Add(PlayerManager.Player);

			LoadAll();
#endif
        }

        /// <summary>
        /// Add an item to group. The item will be saved whenever the group is saved.
        /// </summary>
        public static void Add(SaveLoad ThingToSave)
        {
            ThingsToSave.Add(ThingToSave);
        }

        public static void SynchronizeAll()
	    {
		    int _max_CampaignLevel = 0, _max_CampaignCoins = 0, _max_CampaignIndex = 0;
		    int _max_LastPlayerLevelUpload = 0;

		    // Find the maxes
		    for ( int i = 0; i < 4; ++i )
		    {
			    if ( PlayerManager.Players[ i ] == null ) continue;
			    PlayerData p = PlayerManager.Players[ i ];

			    _max_CampaignLevel = Math.Max( _max_CampaignLevel, p.CampaignLevel );
			    _max_CampaignCoins = Math.Max( _max_CampaignCoins, p.CampaignCoins );
			    _max_CampaignIndex = Math.Max( _max_CampaignIndex, p.CampaignIndex );
			    _max_LastPlayerLevelUpload = Math.Max( _max_LastPlayerLevelUpload, p.LastPlayerLevelUpload );
		    }

		    // Push maxes to everyone
		    for ( int i = 0; i < 4; ++i )
		    {
			    if ( PlayerManager.Players[ i ] == null ) continue;
			    PlayerData p = PlayerManager.Players[ i ];

			    p.CampaignLevel = _max_CampaignLevel;
			    p.CampaignCoins = _max_CampaignCoins;
			    p.CampaignIndex = _max_CampaignIndex;
			    p.LastPlayerLevelUpload = _max_LastPlayerLevelUpload;
		    }

		    // Make master awardment set
		    Set<int> awardments = new Set<int>();
		    for ( int i = 0; i < 4; ++i )
		    {
			    if ( PlayerManager.Players[ i ] == null ) continue;
			    PlayerData p = PlayerManager.Players[ i ];

			    foreach (var guid in p.Awardments)
				    awardments += guid;
		    }

		    // Push awardments to everyone
		    for ( int i = 0; i < 4; ++i )
		    {
			    if ( PlayerManager.Players[ i ] == null ) continue;
			    PlayerData p = PlayerManager.Players[ i ];

			    foreach (var guid in awardments)
				    p.Awardments += guid;
		    }

		    // Calculate max highscores
		    Dictionary<int, ScoreEntry> MaxHighScores = new Dictionary<int,ScoreEntry>();
		    for ( int i = 0; i < 4; ++i )
		    {
			    if ( PlayerManager.Players[ i ] == null ) continue;
			    PlayerData p = PlayerManager.Players[ i ];

                foreach (var HighScore in p.HighScores)
			    {
				    if ( MaxHighScores.ContainsKey( HighScore.Key ) )
				    {
						MaxHighScores[HighScore.Key] = new ScoreEntry(
						    "",
						    HighScore.Value.GameId,
						    Math.Max( HighScore.Value.Value, MaxHighScores[ HighScore.Key ].Value ),
						    Math.Max( HighScore.Value.Score, MaxHighScores[ HighScore.Key ].Score ),
						    Math.Max( HighScore.Value.Level, MaxHighScores[ HighScore.Key ].Level ),
						    Math.Min( HighScore.Value.Attempts, MaxHighScores[ HighScore.Key ].Attempts ),
						    Math.Min( HighScore.Value.Time, MaxHighScores[ HighScore.Key ].Time ),
						    Math.Max( HighScore.Value.Date, MaxHighScores[ HighScore.Key ].Date ) );
				    }
				    else
				    {
					    MaxHighScores.Add( HighScore.Key, HighScore.Value );
				    }
			    }
		    }

		    // Push highscores to every player
		    for ( int i = 0; i < 4; ++i )
		    {
			    if ( PlayerManager.Players[ i ] == null ) continue;
			    PlayerData p = PlayerManager.Players[ i ];

			    foreach ( var HighScore in MaxHighScores )
			    {
                    p.HighScores.AddOrOverwrite(HighScore.Key, new ScoreEntry(
					    "",
					    HighScore.Value.GameId,
					    HighScore.Value.Value,
					    HighScore.Value.Score,
					    HighScore.Value.Level,
					    HighScore.Value.Attempts,
					    HighScore.Value.Time,
					    HighScore.Value.Date));
			    }
		    }

		    // Fuck these stats
		    //boost.shared_ptr<PlayerStats> LifetimeStats, GameStats, LevelStats, TempStats;
		    //boost.shared_ptr<PlayerStats> CampaignStats;

			// Players 2, 3, 4 copy Player 1's seeds
			for (int i = 1; i < 4; i++)
			{
				if (PlayerManager.Players[i] == null) continue;

				PlayerManager.Players[i].MySavedSeeds.SeedStrings = PlayerManager.Players[0].MySavedSeeds.SeedStrings;
			}
	    }

        /// <summary>
        /// Save every item that has been changed.
        /// </summary>
        public static void SaveAll()
        {
            CampaignSequence.CampaignProgressMade = false;
            Tools.EasyThread(5, "Saving", _SaveAll_Thread);
        }

        static bool Saving = false;
        static void _SaveAll_Thread()
        {
            if (!CloudberryKingdomGame.CanSave()) return;

            if (Saving) return;
            Saving = true;

            try
            {
                foreach (SaveLoad ThingToSave in ThingsToSave)
                {
                    ThingToSave.Save(PlayerIndex.One);
                }

#if PC
                SynchronizeAll();
    			PlayerManager.Player.Save(PlayerIndex.One);
#else

                // Save each player's info
                List<PlayerData> player_list;

                // If we're on the title screen save all players who are signed in
                if (CloudberryKingdomGame.CurrentPresence == CloudberryKingdomGame.Presence.TitleScreen)
                {
                    player_list = new List<PlayerData>(PlayerManager.Players);
                }
                // otherwise only logged in players
                else
                {
                    player_list = PlayerManager.LoggedInPlayers;
                }

                foreach (PlayerData player in player_list)
                {
                    var g = player.MyGamer;
                    if (g == null) continue;

                    player.ContainerName = "SaveData";
                    player.FileName = "SaveData.bam";
                    player.Save(player.MyPlayerIndex);
                }
#endif
            }
            catch (Exception e)
            {
                Tools.Write(e.Message);
            }

            Saving = false;
        }

#if NOT_PC
        public static bool SkipPlayerDataInit = false;
		public static void LoadGamers()
		{
			for (int i = 0; i < 4; i++)
			{
				if (PlayerManager.Players[i] != null)
					PlayerManager.Players[i].Exists = false;
			}

			foreach (SignedInGamer gamer in Gamer.SignedInGamers)
			{
				if (EzStorage.Device[(int)gamer.PlayerIndex] == null)
				{
                    if (SkipPlayerDataInit && PlayerManager.Players[(int)gamer.PlayerIndex] != null)
                    {
                        // We just unlocked the demo and should keep this progress.
                        PlayerManager.Players[(int)gamer.PlayerIndex].SkipLoad = true;
                        LoadGamer(PlayerManager.Players[(int)gamer.PlayerIndex]);
                    }
                    else
                    {
                        // Create a new player data.
                        var data = PlayerManager.Players[(int)gamer.PlayerIndex] = new PlayerData();
                        data.Init((int)gamer.PlayerIndex);
                        LoadGamer(PlayerManager.Players[(int)gamer.PlayerIndex]);
                    }
				}
			}

			//for (int i = 0; i < 4; i++)
			//    if (PlayerManager.Players[i].MyGamer != null)
			//        LoadGamer(PlayerManager.Players[i]);

            SkipPlayerDataInit = false;
		}

        public static void LoadGamer(PlayerData player)
        {
            if (EzStorage.Device[(int)player.MyPlayerIndex] == null)
            {
                var d = EzStorage.Device[(int)player.MyPlayerIndex] = new PlayerSaveDevice(player.MyPlayerIndex);
                Tools.GameClass.Components.Add(d);

                // hook two event handlers to force the user to choose a new device if they cancel the
                // device selector or if they disconnect the storage device after selecting it
                d.DeviceSelectorCanceled +=
                    (s, e) => e.Response = SaveDeviceEventResponse.Prompt;
                d.DeviceDisconnected +=
                    (s, e) => e.Response = SaveDeviceEventResponse.Prompt;

                // Prompt for a device on the first Update we can, assuming we are past the IIS "Press Start" phase.
                if (CloudberryKingdomGame.PastPressStart)
                {
                    d.PromptForDevice();
                }

                d.DeviceSelected +=
                    (s, e) =>
                    {
                        player.ContainerName = "SaveData";
                        player.FileName = "SaveData.bam";

                        player.Load(player.MyPlayerIndex);
                    };
            }
        }
#endif

        /// <summary>
        /// Load every item.
        /// </summary>
        public static void LoadAll()
        {
            foreach (SaveLoad ThingToLoad in ThingsToSave)
            {
                ThingToLoad.Load(PlayerIndex.One);
            }

#if PC
			try
			{
				SynchronizeAll();
			}
			catch
			{
			}
#endif
        }
    }

    public class SaveLoad
    {
        public bool AlwaysSave = false;

        public bool Changed = false;
        public string ContainerName, FileName, AlternateFileName;

        string ActualContainerName
        {
            get
            {
#if PC
                return ContainerName;
#else
#if WINDOWS
                return ContainerName + "_XboxVersion";
#else
                return ContainerName;
#endif
#endif
            }
        }

		public static int Checksum(byte[] buffer, int length)
		{
			int val = 0;
			for (int i = 0; i < length; i++)
			{
				val += ((int)buffer[i] + 13) * (i + 10);
			}

			return val;
		}

		public void Save(PlayerIndex index)
        {
            if (Changed || AlwaysSave)
            {
				EzStorage.SaveWithMetaData(index, ActualContainerName, FileName, writer =>
				{
					Serialize(writer);

					Changed = false;
				},
				() =>
				{
					Changed = false;
				});
            }
        }

        /// <summary>
        /// When true the gamer will never load anything.
        /// This is used to prevent progress from being overwritten after a user unlocks the demo.
        /// </summary>
        public bool SkipLoad = false;

		public void Load(PlayerIndex index)
        {
            if (SkipLoad) { Changed = false; SkipLoad = false; return; }

            EzStorage.Load(index, ActualContainerName, FileName, AlternateFileName,
                reader =>
                {
                    Deserialize(reader);
                    Changed = false;
                },
                () =>
                {
                    FailLoad();
                    Changed = false;
                });
        }

        public virtual void Serialize(BinaryWriter writer) { }
        public virtual void Deserialize(byte[] Data) { }
        protected virtual void FailLoad() { }
    }
    
    public static class EzStorage
    {
#if XBOX
        static WrappedBool AsyncUpdateLock = new WrappedBool(false);
        public static void AsyncUpdate()
        {
            while (true)
            {
                if (Guide.IsVisible) { Thread.Sleep(50); continue; }

                for (int i = 0; i < 4; i++)
                {
                    lock (AsyncUpdateLock)
                    {
                        if (Device[i] != null)
                        {
                            Device[i].AysnchUpdate();
                        }
                    }

                    Thread.Sleep(50);
                }
            }
        }

        static bool AsyncUpdateStarted = false;
        public static void StartAsyncUpdate()
        {
            if (AsyncUpdateStarted)
            {
                return;
            }
            else
            {
                AsyncUpdateStarted = true;
                Tools.EasyThread(5, "Async SaveDevice Updates", AsyncUpdate);
            }
        }

        public static PlayerSaveDevice[] Device = new PlayerSaveDevice[4];
#endif

		static void _SaveWithMetaData(BinaryWriter writer, Action<BinaryWriter> SaveLogic)
		{
			var ms = new MemoryStream();

			using (BinaryWriter buffer_writer = new BinaryWriter(ms))
			{
				SaveLogic(buffer_writer);

				byte[] bytes = ms.GetBuffer();

				int length = (int)ms.Length;
				writer.Write(BitConverter.GetBytes(length));
				writer.Write(BitConverter.GetBytes(SaveLoad.Checksum(bytes, length)));
				writer.Write(bytes, 0, length);
				
				ms.Dispose();
			}
		}

		public static void SaveWithMetaData(PlayerIndex index, string ContainerName, string FileName, Action<BinaryWriter> SaveLogic, Action Fail)
		{
			Save(index, ContainerName, FileName, writer => _SaveWithMetaData(writer, SaveLogic), Fail);
		}

		public static string SaveDir()
		{
			string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			dir = Path.Combine(dir, "Cloudberry Kingdom");

			return dir;
		}

		public static void Save(PlayerIndex index, string ContainerName, string FileName, Action<BinaryWriter> SaveLogic, Action Fail)
        {
#if XBOX
            lock (AsyncUpdateLock)
            {
                if (!Device[(int)index].IsReady) return;

                CloudberryKingdomGame.ShowSaving();

                Device[(int)index].Save(ContainerName, FileName, stream =>
                {
                    try
                    {
                        using (BinaryWriter w = new BinaryWriter(stream))
                        {
                            SaveLogic(w);
                        }
                    }
                    catch
                    {
						if (Fail != null)
						{
							Fail();
						}
                    }
                });
            }
#else
			CloudberryKingdomGame.ShowSaving();

			try
			{
				string dir = SaveDir();
				Directory.CreateDirectory(dir);

				using (var stream = File.Create(Path.Combine(dir, FileName)))
				{
					using (BinaryWriter w = new BinaryWriter(stream))
					{
						SaveLogic(w);
					}
				}
			}
			catch (Exception e)
			{
				if (Fail != null)
				{
					Fail();
				}
			}
#endif
		}

        public static void Load(PlayerIndex index, string ContainerName, string FileName, string AlternateFileName, Action<byte[]> LoadLogic, Action Fail)
        {
#if XBOX
            lock (AsyncUpdateLock)
            {
                Device[(int)index].Load(ContainerName, FileName, stream =>
                {
                    bool Failed = false;

                    try
                    {
                        // Get all the bytes
                        byte[] RawData = new byte[stream.Length];
                        stream.Read(RawData, 0, (int)stream.Length);

                        if (RawData.Length <= 8)
                        {
                            Failed = true;
                        }
                        else
                        {
                            byte[] Data = RawData.Range(8, RawData.Length);

                            // Get the length and checksum (first and second integers encoded in byte stream)
                            int length = BitConverter.ToInt32(RawData, 0);
                            int checksum = BitConverter.ToInt32(RawData, 4);

                            int actual_checksum = SaveLoad.Checksum(Data, Data.Length);

                            // Check for errors
                            if (length != Data.Length || checksum != actual_checksum)
                            {
                                Failed = true;
                            }
                            else
                            {
                                LoadLogic(Data);
                            }
                        }
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Tools.Write(e.Message);
#endif
                        Failed = true;
                    }

                    if (Failed)
                    {
						if (Fail != null)
						{
							Fail();
						}

                        CloudberryKingdomGame.ShowError_LoadError();
                    }
                });
            }
#else
			bool Failed = false;

			try
			{
				// Get all the bytes
				string dir = SaveDir();

				byte[] RawData = null;
				try
				{
					RawData = File.ReadAllBytes(Path.Combine(dir, FileName));
				}
				catch
				{
					RawData = File.ReadAllBytes(Path.Combine(dir, AlternateFileName));
				}

				if (RawData.Length <= 8)
				{
					Failed = true;
				}
				else
				{
					byte[] Data = RawData.Range(8, RawData.Length);

					// Get the length and checksum (first and second integers encoded in byte stream)
					int length = BitConverter.ToInt32(RawData, 0);
					int checksum = BitConverter.ToInt32(RawData, 4);

					int actual_checksum = SaveLoad.Checksum(Data, Data.Length);

					// Check for errors
					if (length != Data.Length || checksum != actual_checksum)
					{
						Failed = true;
					}
					else
					{
						LoadLogic(Data);
					}
				}
			}
			catch (Exception e)
			{
#if DEBUG
				Tools.Write(e.Message);
#endif
				Failed = true;
			}

			if (Failed)
			{
				if (Failed != null)
				{
					Fail();
				}

				CloudberryKingdomGame.ShowError_LoadError();
			}
#endif
		}
    }
}