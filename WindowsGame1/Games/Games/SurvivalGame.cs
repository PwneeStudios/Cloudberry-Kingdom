using System.Collections.Generic;

using Drawing;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using System.Threading;

namespace CloudberryKingdom
{    
    public class SurvivalGameData : GameData
    {
        public List<Bob> HoldBobs;

        public override void SetCreatedBobParameters(Bob bob)
        {
            bob.Immortal = false;
            bob.ScreenWrap = false;
            bob.ScreenWrapToCenter = false;
        }

        public static new GameData Factory(LevelSeedData data, bool MakeInBackground)
        {
            return new SurvivalGameData(data, MakeInBackground);
        }

        public SurvivalGameData(LevelSeedData LevelSeed, bool MakeInBackground)
        {
            Init(LevelSeed, MakeInBackground);
        }

        public void Init(LevelSeedData LevelSeed, bool MakeInBackground)
        {
            base.Init();

            AllowQuickJoin = true;

            if (!MakeInBackground)
                Tools.CurGameData = this;

            //Tools.Recycle.Empty();
            Loading = true;

            if (!MakeInBackground)
                Tools.BeginLoadingScreen(!LevelSeed.NoMusicStart);

            Thread MakeThread = new Thread(
                new ThreadStart(
                    delegate
                    {
#if XBOX
                        Thread.CurrentThread.SetProcessorAffinity(new[] { 3 });
#endif
                        Tools.TheGame.Exiting += KillThread;
                        
                        // Finish loading all the art
                        //Tools.TextureWad.LoadAll(1f, Tools.TheGame.Content);

                        MyLevel = LevelSeed.MakeLevel(this);
                        
                        Tools.LevelIsFinished();

                        if (!MakeInBackground)
                            Tools.CurLevel = MyLevel;
                        MyLevel.MyGame = this;

                        if (MyLevel.SetToWatchMake) { Loading = false; Tools.EndLoadingScreen(); return; }

                        MyLevel.CanWatchComputer = MyLevel.CanWatchReplay = true;

                        if (MyLevel.SetToWatchMake) return;

                        MakeBobs(MyLevel);
                        SetAdditionalBobParameters(MyLevel.Bobs);

                        MyLevel.AllowRecording = true;

                        MyLevel.PlayMode = 0;
                        MyLevel.ResetAll(false, !MakeInBackground);


                        // Post process the level
                        if (LevelSeed.PostMake != null)
                            LevelSeed.PostMake(MyLevel);

                        lock (LevelSeed.Loaded)
                        {
                            Loading = false;
                            LevelSeed.Loaded.val = true;
                            LevelSeed.MyGame = this;
                        }


                        Tools.Write("Game made");

                        if (!MakeInBackground)
                        {
                            Tools.EndLoadingScreen();
                        }

                        Tools.TheGame.Exiting -= KillThread;
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