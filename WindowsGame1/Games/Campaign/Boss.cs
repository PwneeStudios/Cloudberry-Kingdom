using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using Drawing;
using System.Collections.Generic;
using System.Linq;
using CloudberryKingdom.Goombas;

namespace CloudberryKingdom
{
    public class Campaign_BossNew : WorldMap
    {
        public static void CampaignReset()
        {
            MyState = State.HasPrincess;
        }

        public enum State { HasPrincess, Boss, BossAgain, Beaten };
        public static State MyState = State.HasPrincess;

        bool ForTrailer = false;

        PrincessBubble Princess;

        public Campaign_BossNew(bool FromEntrance)
            : base(false)
        {
            Data = Campaign.Data;
            WorldName = "Boss";

            Init("Doom\\BossLevel.lvl");
            Tools.BeginLoadingScreen_Fake(130);

            MakeCenteredCamZone(.72f);
            MakeBackground(BackgroundType.Rain);

            // Music
            WaitThenDo(20, () =>
            {
                Tools.SongWad.SuppressNextInfoDisplay = true;
                Tools.SongWad.LoopSong(Tools.Song_Ripcurl);
            });

            // Players
            SetHeroType(BobPhsxNormal.Instance);
            MakePlayers();
            
            // Princess
            Princess = new PrincessBubble(Vector2.Zero);
            MyLevel.AddObject(Princess);
            Princess.PickUp(MvpBob);

            // Menu
            AddGameObject(InGameStartMenu_Campaign.MakeListener());

            // Enter
            if (FromEntrance)
            {
                //Doors["Exit"].SetLock(false, true, false);
                Doors["Exit"].SetLock(true, true, false);
                EnterFromAndClose(Doors["Enter"], 40);
                SetDeathTime(DeathTime.Slow);
            }
            else
            {
                Doors["Enter"].SetLock(false, true, false);
                EnterFrom(Doors["Exit"]);
            }


            if (MyState != State.Beaten)
                BlobSwarm();
                //Start();

            // Start recording
            if (ForTrailer)
                WaitThenDo(1, () =>
                    Tools.TheGame.SetToBringSaveVideoDialog = true);
                    //Tools.TheGame.SetToRecordInput = true);
        }

        void BlobSwarm()
        {
            Step = 0;
            Princess.MyState = PrincessBubble.State.None;
            Princess.PickUp(MvpBob);
            DoMakeBlobs = true;
        }

        int Step = 0;
        bool DoMakeBlobs = true;
        public override void PhsxStep()
        {
            base.PhsxStep();
            if (Loading || Tools.ShowLoadingScreen) return;

            // Reset the random seed
            if (Step == 0)
            {
                MyLevel.Rnd.Rnd = new Random(0);
                //Tools.Write("Rnd Test");
                //Tools.Write(MyLevel.Rnd.RndInt(0, 10000));
            }

            Step++;

            if (MyState == State.HasPrincess)
            {
                Princess.Core.RemoveOnReset = false;

                if (DoMakeBlobs)
                    BlobGuy.RandomBlobs(MyLevel, 3, 1, Goomba.BlobColor.Green);

                if (Step == 42)
                {
                    //Campaign.SpecifiedBlobPos = new Vector2(-3000, 3500);
                    Campaign.SpecifiedBlobPos = new Vector2(3500, 3500);
                    Campaign.SpecifiedVel = new Vector2(0, 23);
                    Campaign.GrabPrincess(this, Princess, Campaign.PrincessPos.Specified,
                        blob =>
                        {
                            blob.SetColor(Goomba.BlobColor.Pink);
                            blob.MaxAcc = 20;
                            blob.DistAccMod *= .5f;
                        },
                        () =>
                        {
                            DoMakeBlobs = false;
                            //CinematicToDo(150, (Action)AddJetpack);
                            CinematicToDo(205, (Action)AddJetpack);
                        });
                }
            }
        }

        void AddJetpack()
        {
            Vector2 Pos = Cam.Pos + new Vector2(400, 2300);
            AddPowerups(BobPhsxJetman.Instance, null, Pos, new Vector2(0, 0), new Vector2(-8.5f, 0), new Vector2(1.2f, 0), OnGrabPowerup, p => { p.Friction = .99f; p.Core.RemoveOnReset = true; });
        }

        void OnGrabPowerup()
        {
            //WaitThenDo(40, () =>
            WaitThenDo(53, () =>
            {
                MyLevel.PreventReset = true;
                MyState = State.Boss;
                //MyState = State.BossAgain;
                Tools.SongWad.FadeOut();
                Tools.Sound("MonsterGrowl").Play();

                // Bring the boss
                //WaitThenDo(62 * 6, Start);
                
                //WaitThenDo(62 * 4, Start);
                WaitThenDo(212, Start);
            });
        }



        protected override void MakePlayers()
        {
            base.MakePlayers();

            MyLevel.Bobs.ForEach(bob => bob.Immortal = false);

            // Player control via loaded recording
            if (ForTrailer)
                LoadRecording("BossTrailer.rec");
        }

        bool StartedMusic = false;

        BlobGuy boss;
        void Start()
        {
            // Kill the music
            if (!StartedMusic)
                Tools.SongWad.FadeOut();
            else
                if (!Tools.SongWad.IsPlaying() || Tools.SongWad.Fading)
                    StartedMusic = false;

            // Blob Guy
            WaitThenDo(80, () =>
            {
                // Boss object
                boss = new BlobGuy();
                MyLevel.AddObject(boss);
                boss.Init(MyLevel.FindBlock("BossCenter").Pos, MyState == State.BossAgain);

                // Shut the exit and start music when boss hits the ground
                boss.OnLand += () => {
                    //WaitThenDo(13, () => Doors["Exit"].SetLock(true, true, true, false));
                    WaitThenDo(6, AddGUI);
                    Tools.Sound("Bash").Play();

                    // Start the music
                    if (!StartedMusic)
                        WaitThenDo(0, () =>
                        {
                            MyLevel.PreventReset = false;

                            StartedMusic = true;
                            Tools.SongWad.SuppressNextInfoDisplay = true;
                            Tools.SongWad.SetPlayList(Tools.Song_140mph);
                            Tools.SongWad.Restart(true);
                        });
                };
            }, "AddBoss", true, true);
        }

        public override void AdditionalReset()
        {
            base.AdditionalReset();

            Step = 0;

            if (MyState == State.Beaten)
            {
                if (gui != null)
                    gui.Release();
            }
            else
            {
                PhsxStepsToDo += 2;
                WaitThenDo(1, () => EnterFrom(Doors["Enter"], 0));
                WaitThenDo(25, () => Doors["Enter"].SetLock(true, true, true));
                Doors["Exit"].SetLock(true, true, false, false);
                SetDeathTime(DeathTime.Normal);

                if (MyState == State.HasPrincess)
                    BlobSwarm();
                else
                    Start();
            }
        }

        protected override void AssignDoors()
        {
            MakeDoorAction(Doors["Exit"], () =>
            {
                MyState = State.Beaten;

                if (boss == null || boss.Exploded)
                    WaitThenDo(20, () => Load(new Campaign_PrincessRoom()));
                else
                    boss.OnExplode += () => WaitThenDo(63, () => Load(new Campaign_PrincessRoom()));
            });

            // Return to previous tower
            if (MyState == State.Beaten)
                //MakeEntranceAction();
                Doors["Enter"].SetLock(true, true, false);
        }

        void MakeEntranceAction()
        {
            MakeDoorAction(Doors["Enter"], () => Load(new Campaign_World2(false)));
        }

        void KillBoss()
        {
            // Kill the boss
            boss.Die();

            // Unlock the exit
            if (Campaign.Index < 3 || MyState == State.BossAgain)
                Doors["Exit"].SetLock(false, true, true);

            // Longer death time so that you can see the full death of the boss
            SetDeathTime(DeathTime.FOREVER);
            boss.OnExplode += () =>
            {
                // Reset if everyone is dead
                WaitThenDo(56, () =>
                {
                    if (PlayerManager.AllDead()) Reset();
                    SetDeathTime(DeathTime.Slow);
                }, "", true, true);

                BossDefeated();
            };
        }

        
        bool GUI_Added = false;
        GUI_BlobQuota gui;
        void AddGUI()
        {
            if (ForTrailer) return;

            if (!GUI_Added)
            {
                gui = new GUI_BlobQuota(Campaign.BossQuota[Campaign.Index]);
                gui.OnQuotaMet = me => KillBoss();
                AddGameObject(gui);
                GUI_Added = true;
            }
        }

        void BossDefeated()
        {
            // Open the entrance door if someone survived
            WaitThenDo(110, () =>
            {
                // Make sure someone has survived
                if (PlayerManager.AllDead()) return;

                if (MyState == State.Boss)
                {
                    if (Campaign.Index >= 3)
                    {
                        // Re-add the blob counter
                        WaitThenDo(26, () =>
                        {
                            if (boss != null)
                            {
                                boss.CollectSelf();
                                foreach (IObject obj in MyLevel.Objects)
                                    if (obj is Goomba)
                                        obj.CollectSelf();
                            }

                            GUI_Added = false;
                            WaitThenDo(3, () =>
                            {
                                PlayerManager.AbsorbTempStats();
                                AddGUI();
                            });
                        });

                        // Start the music again
                        WaitThenDo(76, () =>
                            {
                                StartedMusic = true;
                                Tools.SongWad.SuppressNextInfoDisplay = true;
                                Tools.SongWad.SetPlayList(Tools.Song_Ripcurl);
                                Tools.SongWad.Restart(true);
                            }, "StartMusicAgain", true, false);

                        MyState = State.BossAgain;
                        Start();

                        // Remove the blob counter
                        if (gui != null)
                        {
                            gui.PreventRelease = false;
                            RemoveAllGameObjects(obj => obj is GUI_BlobQuota);
                        }

                        return;
                    }
                }

                // Mark boss as beaten
                MyState = State.Beaten;

                // Remove the blob counter
                if (gui != null)
                {
                    gui.PreventRelease = false;
                    RemoveAllGameObjects(obj => obj is GUI_BlobQuota);
                }

                // Open the entrance door
                //Doors["Enter"].SetLock(false, true, true, false);
                //MakeEntranceAction();
            }, "", true, true);
        }
    }
}