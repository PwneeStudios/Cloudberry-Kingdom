using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Storage;

namespace CloudberryKingdom
{
    public class SaveGroup
    {
        static List<SaveLoad> ThingsToSave = new List<SaveLoad>();

        public static ScoreList ConstructHighScore, ConstructHighLevel;
        public static ScoreList UpUpHighScore, UpUpHighLevel;
        public static ScoreList WheelieHighScore, WheelieHighLevel;
        public static ScoreList EscalationHighScore, EscalationHighLevel;
        public static ScoreList HeroFactoryEscalationHighScore, HeroFactoryEscalationHighLevel;
        public static ScoreList HeroRushHighScore, HeroRushHighLevel;
        public static ScoreList HeroRush2HighScore, HeroRush2HighLevel;

        public static ScoreList TimeCrisisHighScore, TimeCrisisHighLevel;

        public static CampaignList[] CampaignScores;

        static void InitRushScore(string name, ref ScoreList scorelist, ref ScoreList lvllist)
        {
            scorelist = new ScoreList();
            scorelist.FileName = name + ".hsc";
            Add(scorelist);

            lvllist = new ScoreList();
            lvllist.Header = "High Levels";
            lvllist.Prefix = "Level ";
            lvllist.FileName = name + "Level.hsc";
            Add(lvllist);
        }

        public static void Initialize()
        {
            // Escalation Scores
            InitRushScore("Escalation", ref EscalationHighScore, ref EscalationHighLevel);

            // Escalation Scores
            InitRushScore("Hero Factor", ref HeroFactoryEscalationHighScore, ref HeroFactoryEscalationHighLevel);
            
            // Wheelie Scores
            InitRushScore("Wheelie", ref WheelieHighScore, ref WheelieHighLevel);

            // Construct Scores
            InitRushScore("Construct", ref ConstructHighScore, ref ConstructHighLevel);

            // UpUp Scores
            InitRushScore("UpUp", ref UpUpHighScore, ref UpUpHighLevel);

            // Hero Rush Scores
            InitRushScore("HeroRush", ref HeroRushHighScore, ref HeroRushHighLevel);

            // Hero Rush 2 Scores
            InitRushScore("HeroRush2", ref HeroRush2HighScore, ref HeroRush2HighLevel);

            // Time Crisis Scores
            InitRushScore("TimeCrisis", ref TimeCrisisHighScore, ref TimeCrisisHighLevel);


            // Campaign Scores
            CampaignScores = new CampaignList[Campaign.NumDifficulties];
            for (int i = 0; i < Campaign.NumDifficulties; i++)
                CampaignScores[i] = new CampaignList(i);

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

            LockManager.Instance.ContainerName = "LockData";
            LockManager.Instance.FileName = "LockData.shazaam";
            Add(LockManager.Instance);

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
            foreach (SaveLoad ThingToSave in ThingsToSave)
            {
                Incr();
                ThingToSave.Save();
                Wait();
            }

#if NOT_PC
            // Save each player's info
            foreach (PlayerData player in PlayerManager.LoggedInPlayers)
            {
                Incr();
                player.ContainerName = "Gamers";
                player.FileName = "___" + player.GetName();
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

        //public static void LoadRes()
        //{
        //    Incr();
        //    PlayerManager.SavePlayerData = new _SavePlayerData();
        //    PlayerManager.SavePlayerData.Load();
        //    Wait();
        //}

        /// <summary>
        /// Load every item.
        /// </summary>
        public static void LoadAll()
        {
            foreach (SaveLoad ThingToLoad in ThingsToSave)
            {
                Incr();
                ThingToLoad.Load();
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
                    SaveGroup.Decr();
                    Changed = false;
                });
        }

        protected virtual void Serialize(BinaryWriter writer) { }
        protected virtual void Deserialize(BinaryReader reader) { }
    }
    
    public static class EzStorage
    {
        static StorageDevice Device;
        static WrappedBool InUse = new WrappedBool(false);

        public static bool DeviceOK()
        {
            return Device != null && Device.IsConnected;
        }

        public static void GetDevice()
        {
            IAsyncResult result = StorageDevice.BeginShowSelector(null, null);
            result.AsyncWaitHandle.WaitOne();

            Device = StorageDevice.EndShowSelector(result);

            result.AsyncWaitHandle.Close();
        }

        public static void Save(string ContainerName, string FileName, Action<BinaryWriter> SaveLogic, Action Fail)
        {
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
            BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8);
            SaveLogic(writer);
            writer.Close();

            // Close the file.
            stream.Close();

            // Dispose the container, to commit changes.
            container.Dispose();

            lock (InUse)
            {
                InUse.MyBool = false;
            }
        }

        public static void Load(string ContainerName, string FileName, Action<BinaryReader> LoadLogic, Action Fail)
        {
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

            // Device is hooked up and ready for us to load from

            // Open a container
            IAsyncResult result = Device.BeginOpenContainer(ContainerName,
                ContainerResult =>
                {
                    if (!ContainerResult.IsCompleted) { if (Fail != null) Fail(); return; }
                    //if (Fail != null) Fail(); return;

                    StorageContainer container = Device.EndOpenContainer(ContainerResult);
                    ContainerResult.AsyncWaitHandle.Close();

                    LoadFromContainer(container, FileName, LoadLogic, Fail);
                }, null);
        }

        static void LoadFromContainer(StorageContainer container, string FileName, Action<BinaryReader> LoadLogic, Action DoesNotExist)
        {
            // Fallback action if file doesn't exist
            if (!container.FileExists(FileName))
            {
                container.Dispose();

                lock (InUse)
                {
                    InUse.MyBool = false;
                }

                if (DoesNotExist != null)
                    DoesNotExist();

                return;
            }

            // Get the file.
            Stream stream = container.OpenFile(FileName, FileMode.Open);

            // Load the data
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);
            LoadLogic(reader);
            reader.Close();

            // Close the file.
            stream.Close();

            // Dispose the container, to commit changes.
            container.Dispose();

            lock (InUse)
            {
                InUse.MyBool = false;
            }
        }
    }
}