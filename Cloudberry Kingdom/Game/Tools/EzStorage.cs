using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;

#if PC_VERSION
#elif XDKX || XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif

using EasyStorage;

namespace CloudberryKingdom
{
    public class SaveGroup
    {
        static List<SaveLoad> ThingsToSave = new List<SaveLoad>();

        public static void Initialize()
        {
            ScoreDatabase.Initialize();
            PlayerManager.SavePlayerData = new _SavePlayerData();

#if PC_VERSION
            PlayerManager.Player.ContainerName = "PlayerData";
            PlayerManager.Player.FileName = "MainPlayer";
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

        /// <summary>
        /// Save every item that has been changed.
        /// </summary>
        public static void SaveAll()
        {
            if (!CloudberryKingdomGame.CanSave()) return;

            foreach (SaveLoad ThingToSave in ThingsToSave)
            {
                ThingToSave.Save(PlayerIndex.One);
            }

#if PC_VERSION
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

#if NOT_PC
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
					var data = PlayerManager.Players[(int)gamer.PlayerIndex] = new PlayerData();
                    data.Init((int)gamer.PlayerIndex);
					LoadGamer(PlayerManager.Players[(int)gamer.PlayerIndex]);
				}
			}

			//for (int i = 0; i < 4; i++)
			//    if (PlayerManager.Players[i].MyGamer != null)
			//        LoadGamer(PlayerManager.Players[i]);
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

                // prompt for a device on the first Update we can
                d.PromptForDevice();

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
        }
    }

    public class SaveLoad
    {
        public bool AlwaysSave = false;

        public bool Changed = false;
        public string ContainerName, FileName;

        string ActualContainerName
        {
            get
            {
#if PC_VERSION
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
                EzStorage.Save(index, ActualContainerName, FileName, writer =>
                    {
						var ms = new MemoryStream();

						using (BinaryWriter buffer_writer = new BinaryWriter(ms))
						{
							Serialize(buffer_writer);

							byte[] bytes = ms.GetBuffer();

							int length = (int)ms.Length;
                            writer.Write(BitConverter.GetBytes(length));
							writer.Write(BitConverter.GetBytes(Checksum(bytes, length)));
							writer.Write(bytes, 0, length);

							ms.Dispose();
						}

                        Changed = false;
                    },
                    () =>
                    {
                        Changed = false;
                    });
            }
        }

		public void Load(PlayerIndex index)
        {
            EzStorage.Load(index, ActualContainerName, FileName,
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

        public static void StartAsyncUpdate()
        {
            Tools.EasyThread(5, "Async SaveDevice Updates", AsyncUpdate);
        }

        public static PlayerSaveDevice[] Device = new PlayerSaveDevice[4];

		public static void Save(PlayerIndex index, string ContainerName, string FileName, Action<BinaryWriter> SaveLogic, Action Fail)
        {
            lock (AsyncUpdateLock)
            {
                if (!Device[(int)index].IsReady) return;

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
                        Fail();
                    }
                });
            }
        }

        public static void Load(PlayerIndex index, string ContainerName, string FileName, Action<byte[]> LoadLogic, Action Fail)
        {
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
                        Fail();
                        CloudberryKingdomGame.ShowError_LoadError();
                    }
                });
            }
        }
    }
}