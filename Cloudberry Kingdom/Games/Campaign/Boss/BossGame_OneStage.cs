using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using Drawing;
using System.Collections.Generic;
using System.Linq;

namespace CloudberryKingdom
{
    public class Campaign_Boss : WorldMap
    {
        public static bool Beaten = false;

        public Campaign_Boss(bool FromEntrance)
            : base(false)
        {
            Data = Campaign.Data;
            WorldName = "Boss";

            Init("Doom\\BossLevel.lvl");

            MakeCenteredCamZone(.72f);
            MakeBackground(BackgroundType.Outside);
            
            // Players
            //SetHeroType(BobPhsxNormal.Instance);
            SetHeroType(BobPhsxJetman.Instance);
            MakePlayers();
            
            // Menu
            AddGameObject(InGameStartMenu_Campaign.MakeListener());

            // Enter
            if (FromEntrance)
            {
                Doors["Exit"].SetLock(false, true, false);
                EnterFromAndClose(Doors["Enter"], 60);
                SetDeathTime(DeathTime.Slow);
            }
            else
            {
                Doors["Enter"].SetLock(false, true, false);
                EnterFrom(Doors["Exit"]);
            }

            if (!Beaten)
                Start();
        }

        protected override void MakePlayers()
        {
            base.MakePlayers();

            MyLevel.Bobs.ForEach(bob => bob.Immortal = false);
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
                boss.Init(MyLevel.FindBlock("BossCenter").Pos, false);

                // Shut the exit and start music when boss hits the ground
                boss.OnLand += () => {
                    WaitThenDo(13, () => Doors["Exit"].SetLock(true, true, true, false));
                    WaitThenDo(6, AddGUI);

                    // Start the music
                    if (!StartedMusic)
                        WaitThenDo(0, () =>
                        {
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

            if (Beaten)
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
                Start();
            }
        }

        protected override void AssignDoors()
        {
            MakeDoorAction(Doors["Exit"], () =>
            {
                Beaten = true;
            });

            // Return to previous tower
            if (Beaten)
                MakeEntranceAction();
        }

        void MakeEntranceAction()
        {
            //MakeDoorAction(Doors["Enter"], () => Load(new Campaign_World2(false)));
        }

        void KillBoss()
        {
            // Kill the boss
            boss.Die();

            // Unlock the exit
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

                // Open the entrance door if someone survived
                WaitThenDo(110, () =>
                {
                    // Make sure someone has survived
                    if (PlayerManager.AllDead()) return;

                    // Mark boss as beaten
                    Beaten = true;

                    // Remove the blob counter
                    if (gui != null)
                    {
                        gui.PreventRelease = false;
                        RemoveAllGameObjects(obj => obj is GUI_BlobQuota);
                    }

                    // Open the entrance door
                    Doors["Enter"].SetLock(false, true, true, false);
                    MakeEntranceAction();
                }, "", true, true);
            };
        }

        bool GUI_Added = false;
        GUI_BlobQuota gui;
        void AddGUI()
        {
            if (!GUI_Added)
            {
                gui = new GUI_BlobQuota(Campaign.BossQuota[Campaign.Index]);
                gui.OnQuotaMet = me => KillBoss();
                AddGameObject(gui);
                GUI_Added = true;
            }
        }
    }
}