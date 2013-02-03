using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;

namespace CloudberryKingdom
{
    public class SaveGroup
    {
        static List<SaveLoad> ThingsToSave = new List<SaveLoad>();

        public static void Initialize()
        {
            ScoreDatabase.Initialize();

            // Player data
            PlayerManager.SavePlayerData = new _SavePlayerData();
            PlayerManager.SavePlayerData.ContainerName = "PlayerData";
            PlayerManager.SavePlayerData.FileName = "PlayerData.hsc";
            Add(PlayerManager.SavePlayerData);

#if PC_VERSION
            PlayerManager.Player.ContainerName = "PlayerData";
            PlayerManager.Player.FileName = "MainPlayer";
            Add(PlayerManager.Player);
#endif

            LoadAll();

            //PlayerManager.Player.LifetimeStats.Coins += 1000;
            //PlayerManager.Player.Awardments += 4;
            //PlayerManager.Player.Awardments += 7;
        }

        /// <summary>
        /// Wait until nothing is trying to be saved or loaded.
        /// </summary>
        public static void Wait()
        {
            while (true)
            {
                lock (Count)
                {
                    if (Count.MyInt == 0) return;
                }

                Thread.Sleep(1);
            }
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
            if (CloudberryKingdomGame.IsDemo) return;

            foreach (SaveLoad ThingToSave in ThingsToSave)
            {
                Incr();
                ThingToSave.Save();
#if XDK
                return; Tools.Warning();
#endif
                Wait();
            }

#if NOT_PC
            // Save each player's info
            foreach (PlayerData player in PlayerManager.LoggedInPlayers)
            {
                Incr();
                player.ContainerName = "Gamers";
                player.FileName = "___" + warning, should use something other than name here, that can't change player.GetName();
                player.Save();
                Wait();
            }
#endif
        }

#if NOT_PC
        public static PlayerData LoadGamer(string GamerName, PlayerData Data)
        {
            Data.ContainerName = "Gamers";
            Data.FileName = "___" + GamerName;

            Incr();
            Data.Load();
            Wait();

            return Data;
        }
#endif

        /// <summary>
        /// Load every item.
        /// </summary>
        public static void LoadAll()
        {
            foreach (SaveLoad ThingToLoad in ThingsToSave)
            {
                Incr();
                ThingToLoad.Load();
#if XDK
                return; Tools.Warning();
#endif
                Wait();
            }
        }

        static WrappedInt Count = new WrappedInt(0);
        static void Incr() { lock (Count) { Count.MyInt++; } }
        public static void Decr() { lock (Count) { Count.MyInt--; } }
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

        public void Save()
        {
            if (Changed || AlwaysSave)
            {
                EzStorage.Save(ActualContainerName, FileName, writer =>
                    {
                        Serialize(writer);
                        Changed = false;
                        SaveGroup.Decr();
                    },
                    () =>
                    {
                        SaveGroup.Decr();
                        Changed = false;
                    });
            }
            else
                SaveGroup.Decr();
        }

        public void Load()
        {
            EzStorage.Load(ActualContainerName, FileName,
                reader =>
                {
                    Deserialize(reader);
                    Changed = false;
                    SaveGroup.Decr();
                },
                () =>
                {
                    FailLoad();
                    SaveGroup.Decr();
                    Changed = false;
                });
        }

        protected virtual void Serialize(BinaryWriter writer) { }
        protected virtual void Deserialize(byte[] Data) { }
        protected virtual void FailLoad() { }
    }
    
    public static class EzStorage
    {
        static StorageDevice Device;
        static WrappedBool InUse = new WrappedBool(false);

        public static bool DeviceOK()
        {
            return Device != null && Device.IsConnected;
        }

#if XDK
        static void GetDeviceCallback(IAsyncResult result)
        {
            Tools.Write("!");
        }

        public static void GetDevice()
        {
            IAsyncResult result = StorageDevice.BeginShowSelector(GetDeviceCallback, null);
            //result.AsyncWaitHandle.WaitOne();

            //Device = StorageDevice.EndShowSelector(result);

            //result.AsyncWaitHandle.Close();
        }
#endif

        public static void GetDevice()
        {
            IAsyncResult result = StorageDevice.BeginShowSelector(null, null);
            result.AsyncWaitHandle.WaitOne();

            Device = StorageDevice.EndShowSelector(result);

            result.AsyncWaitHandle.Close();
        }

        public static void Save(string ContainerName, string FileName, Action<BinaryWriter> SaveLogic, Action Fail)
        {
            // FIXME WARNING DOES NOT WORK ON XBOX
#if XDK
            Tools.Warning();
            if (Fail != null) Fail(); return;
#endif

            if (!DeviceOK())
                GetDevice();

            if (!DeviceOK())
            {
                if (Fail != null) Fail();
                return;
            }

            // Check if we're already trying to use the device
            int count = 0;
            while (InUse.MyBool && count++ < 100)
            {
                Thread.Sleep(1);
            }
            if (InUse.MyBool) { if (Fail != null) Fail(); return; }

            lock (InUse)
            {
                InUse.MyBool = true;
            }

            // Device is hooked up and ready for us to save to

            // Open a container
            IAsyncResult result = Device.BeginOpenContainer(ContainerName,
                ContainerResult =>
                {
                    if (!ContainerResult.IsCompleted) { if (Fail != null) Fail(); return; }

                    StorageContainer container = Device.EndOpenContainer(ContainerResult);
                    ContainerResult.AsyncWaitHandle.Close();

                    if (SaveLogic != null)
                        SaveToContainer(container, FileName, SaveLogic);
                }, null);
        }

        static void SaveToContainer(StorageContainer container, string FileName, Action<BinaryWriter> SaveLogic) 
        {
            // Check to see whether the save exists.
            if (container.FileExists(FileName))
                // Delete it so that we can create one fresh.
                container.DeleteFile(FileName);

            // Create the file.
            Stream stream = container.CreateFile(FileName);

            // Save the data
            if (SaveLogic != null)
            {
                BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8);
                SaveLogic(writer);
                writer.Close();
            }

            // Close the file.
            stream.Close();

            // Dispose the container, to commit changes.
            container.Dispose();

            lock (InUse)
            {
                InUse.MyBool = false;
            }
        }

        public static void Load(string ContainerName, string FileName, Action<byte[]> LoadLogic, Action Fail)
        {
            // FIXME WARNING DOES NOT WORK ON XBOX
            Tools.Warning();

            if (!DeviceOK())
                GetDevice();

#if XDK
            if (Fail != null) Fail(); return;
#endif

            if (!DeviceOK())
            {
                if (Fail != null) Fail();
                return;
            }

            // Check if we're already trying to use the device
            int count = 0;
            while (InUse.MyBool && count++ < 100)
            {
                Thread.Sleep(1);
            }
            if (InUse.MyBool) { if (Fail != null) Fail(); return; }

            lock (InUse)
            {
                InUse.MyBool = true;
            }

            // Device is hooked up and ready for us to load from

            // Open a container
            IAsyncResult result = Device.BeginOpenContainer(ContainerName,
                ContainerResult =>
                {
                    if (!ContainerResult.IsCompleted) { if (Fail != null) Fail(); return; }
                    //if (Fail != null) Fail(); return;

                    StorageContainer container = Device.EndOpenContainer(ContainerResult);
                    ContainerResult.AsyncWaitHandle.Close();

                    if (LoadLogic != null)
                        LoadFromContainer(container, FileName, LoadLogic, Fail);
                }, null);
        }

        static void LoadFromContainer(StorageContainer container, string FileName, Action<byte[]> LoadLogic, Action FailLogic)
        {
            // Fallback action if file doesn't exist
            if (!container.FileExists(FileName))
            {
                container.Dispose();

                lock (InUse)
                {
                    InUse.MyBool = false;
                }

                if (FailLogic != null)
                    FailLogic();

                return;
            }

            // Load and process the data
            if (LoadLogic != null)
            {
                try
                {
                    Stream s = container.OpenFile(FileName, FileMode.Open);
                    byte[] Data = new byte[s.Length];
                    s.Read(Data, 0, (int)s.Length);

                    LoadLogic(Data);
                }
                catch
                {
                    if (FailLogic != null)
                        FailLogic();
                }
            }

            // Dispose the container, to commit changes.
            container.Dispose();

            lock (InUse)
            {
                InUse.MyBool = false;
            }
        }
    }
}