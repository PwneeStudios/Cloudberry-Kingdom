
using System;

namespace CloudberryKingdom
{
    public partial class Level
    {
        /// <summary>
        /// Whether a replay is available to be watched.
        /// </summary>
        public bool ReplayAvailable { get { return MySwarmBundle != null; } }

        public SwarmBundle MySwarmBundle;
        public Recording CurrentRecording;
        public bool Recording;
        ReplayGUI MyReplayGUI;

        public bool SingleOnly = false;

        public bool NoCameraChange = false;
        void SaveCamera()
        {
            if (NoCameraChange) return;

            HoldCamera = MainCamera;
            MainCamera = new Camera(MainCamera);
        }
        void RestoreCamera()
        {
            if (NoCameraChange) return;

            // Destroy the temporary replay camera and
            // start using the previous camera once again
            MainCamera.Release();
            MainCamera = HoldCamera;
        }

        public void SetReplay()
        {
            int NumBobs = Bobs.Count;
            Bobs.Clear();
            //for (int i = 0; i < CurrentRecording.NumBobs; i++)
            for (int i = 0; i < NumBobs; i++)
            {
                if (MySwarmBundle.CurrentSwarm.MainRecord.Recordings.Length <= i)
                    break;

                //Bob Comp = new Bob(Prototypes.bob[DefaultHeroType], false);
                Bob Comp = new Bob(DefaultHeroType, false);
                Comp.SetColorScheme(PlayerManager.Get(i).ColorScheme);

                //Comp.MyRecord = CurrentRecording.Recordings[i];
                Comp.MyRecord = MySwarmBundle.CurrentSwarm.MainRecord.Recordings[i];
                Comp.CompControl = true;
                Comp.Immortal = true;
                AddBob(Comp);
            }
        }

        public void WatchReplay(bool SaveCurInfo)
        {
            Tools.PhsxSpeed = 1;

            SuppressCheckpoints = true;
            GhostCheckpoints = true;

            MyReplayGUI = new ReplayGUI();
            MyReplayGUI.Type = ReplayGUIType.Replay;
            MyGame.AddGameObject(MyReplayGUI);

            MyReplayGUI.StartUp();

            if (Recording) StopRecording();

            if (!MySwarmBundle.Initialized)
                MySwarmBundle.Init(this);

            if (SaveCurInfo)
            {
                HoldPlayerBobs.Clear();
                HoldPlayerBobs.AddRange(Bobs);
                HoldCamPos = MainCamera.Data.Position;
                SaveCamera();
            }

            // Select the first swarm in the bundle to start with
            MySwarmBundle.SetSwarm(this, 0);

            PreventReset = false;
            FreezeCamera = false;
            Watching = true;
            Replay = true;
            ReplayPaused = false;
            //            MainReplayOnly = true;


            SetReplay();

            SetToReset = true;
        }

        public Lambda OnWatchComputer;

        public void WatchComputer() { WatchComputer(true); }
        public void WatchComputer(bool GUI)
        {
            if (Watching) return;

            Tools.PhsxSpeed = 1;

            // Consider the reset free if the players are close to the start
            FreeReset = CloseToStart();
            if (!FreeReset)
                CountReset();

            SaveCamera();

            if (GUI)
            {
                // Create the GUI
                MyReplayGUI = new ReplayGUI();
                MyReplayGUI.Type = ReplayGUIType.Computer;
                MyGame.AddGameObject(MyReplayGUI);

                MyReplayGUI.StartUp();
            }

            Watching = true;
            SuppressCheckpoints = true;
            GhostCheckpoints = true;

            // Swap the player Bobs for computer Bobs
            HoldPlayerBobs.Clear();
            HoldPlayerBobs.AddRange(Bobs);

            Bobs.Clear();
            for (int i = 0; i < CurPiece.NumBobs; i++)
            {
                //Bob Comp = new Bob(Prototypes.bob[DefaultHeroType], false);
                Bob Comp = new Bob(DefaultHeroType, false);

                Comp.MyPiece = CurPiece;
                Comp.MyPieceIndex = i;
                Comp.MyRecord = CurPiece.Recording[i];
                Comp.CompControl = true;
                Comp.Immortal = true;
                AddBob(Comp);

                /*
                if (CloudberryKingdomGame.SimpleAiColors)
                {
                    int index = (new int[] { 0, 3, 5, 1 })[i];
                    Comp.SetColorScheme(ColorSchemeManager.ComputerColorSchemes[index]);
                }
                else
                    Comp.SetColorScheme(Tools.GlobalRnd.RandomItem<ColorScheme>(ColorSchemeManager.ComputerColorSchemes));
                */
                Comp.SetColorScheme(ColorSchemeManager.ComputerColorSchemes[0]);

                Comp.MoveData = CurPiece.MyMakeData.MoveData[i];
                int Copy = Comp.MoveData.Copy;
                if (Copy >= 0)
                    Comp.SetColorScheme(Bobs[Copy].MyColorScheme);
            }

            // Set special Bob parameters
            MySourceGame.SetAdditionalBobParameters(Bobs);

            // Additional actions
            if (OnWatchComputer != null) OnWatchComputer.Apply();

            SetToReset = true;
        }

        public bool EndOfReplay()
        {
            return CurPhsxStep >= CurPiece.PieceLength;
        }

        public Lambda OnEndReplay;
        public void EndReplay()
        {
            SuppressCheckpoints = false;
            GhostCheckpoints = false;

            RestoreCamera();

            Replay = Watching = false;
            Recording = false;
            ReplayPaused = false;

            MainCamera.Data.Position = HoldCamPos;
            //FreezeCamera = true;
            MainCamera.Update();

            Bobs.Clear();
            Bobs.AddRange(HoldPlayerBobs);
            foreach (Bob bob in Bobs)
            {
                bob.PlayerObject.AnimQueue.Clear();
                bob.PlayerObject.EnqueueAnimation(0, 0, true);
                bob.PlayerObject.DequeueTransfers();
            }

            if (OnEndReplay != null) OnEndReplay.Apply();
        }


        public void EndComputerWatch()
        {
            RestoreCamera();

            ReplayPaused = false;
            StartPlayerPlay();

            if (OnEndReplay != null) OnEndReplay.Apply();
        }
    }
}
