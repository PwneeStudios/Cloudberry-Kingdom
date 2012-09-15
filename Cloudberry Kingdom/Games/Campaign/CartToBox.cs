using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using Drawing;
using CloudberryKingdom.Coins;

namespace CloudberryKingdom
{
    public class Campaign_CartToBox : WorldMap
    {
        int Wait_KillMomentum, Wait_PopWheelsOff, Wait_Title, Wait_QM;

        void SetParams()
        {
            Wait_KillMomentum = 166;
            Wait_PopWheelsOff = 245;
            Wait_Title = 300;
            Wait_QM = 200;

            Wait_KillMomentum = 166;
            Wait_QM = 215;
            Wait_PopWheelsOff = 275;
            Wait_Title = 330;

            Wait_KillMomentum = 166;
            Wait_QM = 216;// 215;
            Wait_PopWheelsOff = Wait_QM + 60;
            Wait_Title = Wait_PopWheelsOff + 55;
        }

        public Campaign_CartToBox(EzSong song)
            : base(false)
        {
            SetParams();
             
            Data = Campaign.Data;
            WorldName = "CartToBox";

            Init("Doom\\GetCart.lvl");
            MyLevel.PreventReset = true;

            MyLevel.EraseNonDoodadCoins();

            MakeCenteredCamZone(1f, "Center", "CameraEnd");
            MakeBackground(BackgroundType.Outside);
            //MakeBackground(BackgroundType.Dungeon);

            // Players
            SetHeroType(BobPhsxRocketbox.Instance);

            MakePlayers();

            // Position players
            EnterFrom(FindDoor("Enter"));

            if (song != null)
                WaitThenDo(45, () => Tools.SongWad.LoopSong(song));


            CinematicToDo(Wait_QM, (Action)QM);

            CinematicToDo(Wait_KillMomentum, (Action)KillMomentum);
            CinematicToDo(Wait_PopWheelsOff, (Action)PopWheelsOff);
            CinematicToDo(Wait_Title, () => AddGameObject(LevelTitle.HeroTitle("Hero in a Box")));

            // Exit sign
            if (MyLevel.Info.Doors.ShowSign)
            {
                Sign sign = new Sign(false, MyLevel);
                sign.PlaceAt(Doors["Exit"].GetTop());
                MyLevel.AddObject(sign);
            }

            // Exit door
            Doors["Exit"].SetLock(false, true, false);
            Doors["Exit"].NoNote = true;
            Doors["Exit"].OnOpen += ReturnToWorldMap;

            // Menu
            AddGameObject(InGameStartMenu_Campaign.MakeListener());
        }

        int DelayIncr = 25;
        void PopWheelsOff()
        {
            BobPhsx hero = BobPhsxBox.Instance;

            int Delay = 0;
            foreach (Bob bob in MyLevel.Bobs)
            {
                Bob b = bob;
                CinematicToDo(Delay, () => PopWheelsOffBob(hero, b));
                Delay += DelayIncr;
            }
        }

        private void PopWheelsOffBob(BobPhsx hero, Bob bob)
        {
            Tools.SoundWad.FindByName("Piece Explosion Small").Play(1f);

            Quad q; Particles.Particle p;

            q = bob.PlayerObject.FindQuad("Wheel_Left") as Quad;
            p = PopSingleWheel(q);
            p.Data.Velocity = new Vector2(-40, 45);

            q = bob.PlayerObject.FindQuad("Wheel_Right") as Quad;
            p = PopSingleWheel(q);
            p.Data.Velocity = new Vector2(40, 55);

            ParticleEffects.DustCloudExplosion(MyLevel, bob.Pos);

            // Change bob into cart hero
            MyLevel.SwitchHeroType(bob, hero);

            // Push bob up
            bob.Core.Data.Velocity.Y += 14;// 20;
        }

        void QM()
        {
            if (MyLevel.Bobs.Count == 0) return;

            Bob b = MyLevel.Bobs[0];


            var QM = new GUI_Text("?", b.Pos + new Vector2(0, 250));
            QM.NoPosMod = false;
            QM.FixedToCamera = false;
            QM.Core.Data.Velocity = new Vector2(0, .3f);
            CampaignMenu.HappyBlueColor(QM.MyText);
            QM.MyText.Scale *= .9f;
            AddGameObject(QM);

            CinematicToDo(60, (Action)QM.Kill);
        }

        void KillMomentum()
        {
            int Delay = 0;
            foreach (Bob bob in MyLevel.Bobs)
            {
                Bob b = bob;
                CinematicToDo(Delay, () => b.SetHeroPhsx(BobPhsxBox.Instance));
                Delay += DelayIncr;
            }
        }

        Particles.Particle PopSingleWheel(Quad q)
        {
            Particles.Particle p = ParticleEffects.CoinExplosionTemplate;
            p.MyQuad = new SimpleQuad(q);
            p.MyQuad.Init();
            p.Base.Init();
            p.Size = new Vector2(100);
            q.Center.PosFromRelPos();
            p.Data.Position = q.Center.Pos;

            MyLevel.MainEmitter.EmitParticle(p);
            return p;
        }

        public override void PhsxStep()
        {
            base.PhsxStep();

        //    if (MyLevel.CurPhsxStep % 60 == 0)
        //        PopWheelsOff();
        }
    }
}