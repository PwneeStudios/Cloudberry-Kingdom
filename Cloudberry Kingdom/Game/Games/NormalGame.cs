using CoreEngine;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using System.Threading;

namespace CloudberryKingdom
{    
    public class NormalGameData : GameData
    {
        public override void SetCreatedBobParameters(Bob bob)
        {
            bob.Immortal = false;
            bob.ScreenWrap = false;
            bob.ScreenWrapToCenter = false;
        }

        public virtual void SetAdditionalLevelParameters()
        {
            MyLevel.AllowRecording = true;
        }

        public static new GameData Factory(LevelSeedData data, bool MakeInBackground)
        {
            return new NormalGameData(data, MakeInBackground);
        }

        public NormalGameData() { }

        public NormalGameData(LevelSeedData LevelSeed, bool MakeInBackground)
        {
            Init(LevelSeed, MakeInBackground);
        }

        public virtual void Init(LevelSeedData LevelSeed, bool MakeInBackground)
        {
            base.Init();

            AllowQuickJoin = true;
            DefaultHeroType = LevelSeed.DefaultHeroType;

            if (!MakeInBackground)
                Tools.CurGameData = this;

            //Tools.Recycle.Empty();
            Loading = true;

            if (!MakeInBackground)
            {
                Tools.BeginLoadingScreen(!LevelSeed.NoMusicStart);
                if (LevelSeed.OnBeginLoad != null)
                    LevelSeed.OnBeginLoad();
            }

            Thread MakeThread = new Thread(
                new ThreadStart(
                    delegate
                    {
#if XBOX
                        Thread.CurrentThread.SetProcessorAffinity(new[] { 3 });
#endif
                        Tools.TheGame.Exiting += KillThread;
                        
                        MyLevel = LevelSeed.MakeLevel(this);

                        if (MyLevel.ReturnedEarly)
                        {
                            Tools.CurLevel = MyLevel;
                            Tools.CurGameData = this;
                        }

                        Tools.LevelIsFinished();

                        if (!MakeInBackground)
                            Tools.CurLevel = MyLevel;
                        MyLevel.MyGame = this;

                        if (MyLevel.SetToWatchMake) { Loading = false; Tools.EndLoadingScreen(); return; }

                        MyLevel.CanWatchComputer = MyLevel.CanWatchReplay = true;

                        if (MyLevel.SetToWatchMake) return;

                        MakeBobs(MyLevel);
                        SetAdditionalBobParameters(MyLevel.Bobs);

                        MyLevel.Name = LevelSeed.Name;
                        SetAdditionalLevelParameters();

                        // Post process the level
                        if (LevelSeed.PostMake != null)
                            LevelSeed.PostMake(MyLevel);

                        // Final level reset
                        MyLevel.PlayMode = 0;
                        MyLevel.ResetAll(false, !MakeInBackground);

                        // Mark the level as loaded
                        lock (LevelSeed.Loaded)
                        {
                            Loading = false;
                            LevelSeed.Loaded.val = true;
                            LevelSeed.MyGame = this;
                        }

                        // End the loading screen
                        if (!MakeInBackground)
                        {
                            Tools.EndLoadingScreen();
                        }

                        Tools.TheGame.Exiting -= KillThread;

                        if (LevelSeed.ReleaseWhenLoaded)
                        {
                            Release();
                            LevelSeed.Release();
                        }
                    }))
            {
                Name = "MakeLevelThread",
#if WINDOWS
                Priority = ThreadPriority.Normal,
#endif
            };

            MakeThread.Start();
        }




        public override void PhsxStep()
        {
            base.PhsxStep();

            if (Tools.AutoLoop)
            {
                Tools.AutoLoopDelay++;
                if (Tools.AutoLoopDelay == 20 || Tools.AutoLoopDelay == 40)
                    foreach (EzSound snd in Tools.SoundWad.SoundList)
                        snd.Play();
                if (Tools.AutoLoopDelay > 60)
                {
                    Tools.AutoLoopDelay = 0;
                    Tools.WorldMap.SetToReturnTo(0);
                }
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();
        }

        public override void AdditionalReset()
        {
            base.AdditionalReset();

            if (!MyLevel.Watching && MyLevel.Recording && !(MyLevel.PlayMode != 0 || MyLevel.Watching))
                MyLevel.StartRecording();
        }

        public override void BobDie(Level level, Bob bob)
        {
            base.BobDie(level, bob);
        }

        public override void BobDoneDying(Level level, Bob bob)
        {
            base.BobDoneDying(level, bob);
        }
   }
}