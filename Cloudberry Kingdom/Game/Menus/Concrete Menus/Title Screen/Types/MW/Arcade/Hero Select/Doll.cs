using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#if PC_VERSION
#elif XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif
using CoreEngine;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class HeroDoll : CkBaseMenu
    {
        public HeroDoll(int Control) : base(false)
        {
            this.Control = Control;
            
            Constructor();
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            if (MyDoll != null)
            {
                MyDoll.Core.MyLevel.Bobs.Remove(MyDoll);
                MyDoll.Release(); MyDoll = null;
            }
        }

        public Bob MyDoll;
        public override void Init()
        {
            base.Init();

            Core.DrawLayer++;

            SlideInLength = 0;
            SlideOutLength = 0;
            CallDelay = 0;
            ReturnToCallerDelay = 0;

            MyPile = new DrawPile();
            EnsureFancy();
        }

        public override void OnAdd()
        {
 	        base.OnAdd();

            MakeHeroDoll(BobPhsxNormal.Instance);
        }

        PlayerData player;
        public void MakeHeroDoll(BobPhsx hero)
        {
            var current_bob = MyGame.MyLevel.Bobs.Count == 0 ? null : MyGame.MyLevel.Bobs[0];
            var current_pos = current_bob == null ? Vector2.Zero : current_bob.Pos;

            // Grab cape
            Cape PrevCape = null;
            if (current_bob != null)
            {
                PrevCape = current_bob.MyCape;
                current_bob.MyCape = null;
            }

            // Get rid of old bobs.
            foreach (var bob in MyGame.MyLevel.Bobs)
                bob.Release();
            MyGame.MyLevel.Bobs.Clear();

            // Get an existing player
            player = PlayerManager.Players[PlayerManager.GetFirstPlayer()];

            // Make doll
            MyDoll = new Bob(hero, false);
            MyDoll.MyPlayerIndex = player.MyPlayerIndex;
            MyDoll.MyPiece = Tools.CurLevel.CurPiece;
            MyDoll.CharacterSelect = true;
            MyDoll.CharacterSelect2 = true;
            MyDoll.AffectsCamera = false;
            MyDoll.Immortal = true;
            MyDoll.CompControl = true;
            MyDoll.DrawWithLevel = false;

            PhsxData data = new PhsxData();
            MyDoll.Init(false, data, Tools.CurGameData);

            MyDoll.SetHeroPhsx(hero);
            MyDoll.MyPhsx.DollInitialize();

            MyDoll.Move(current_pos - MyDoll.Pos);
            MyGame.MyLevel.AddBob(MyDoll);

            for (int i = 0; i < 20; i++)
            {
                float Zoom = 2f * MyDoll.MyPhsx.DollCamZoomMod;
                Vector2 Pos = MyPile.Pos;
                Vector2 CurHeroDollPos = Pos / Zoom + Tools.CurGameData.CamPos;
                MyDoll.Move(CurHeroDollPos - MyDoll.Pos);

                MyDoll.AnimAndUpdate();
                MyDoll.PhsxStep();
                MyDoll.PhsxStep2();
                MyPhsxStep();
            }
            

            // Swap out old cape for the new one.
            //if (MyDoll.MyCape != null && PrevCape != null && MyDoll.MyCape.MyType == PrevCape.MyType)
            //{
            //    MyDoll.MyCape.Copy(PrevCape);
            //    PrevCape.Release();
            //}
            //else
            {
                if (PrevCape != null) PrevCape.Release();
            }

            UpdateColorScheme();

            SetPos();
        }

        void SetPos()
        {
            MyPile.Pos = new Vector2(-983.3336f, 433.3333f);
        }

        public void SetPhsx()
        {
            //MyDoll.PlayerObject.EnqueueAnimation("Stand", 0, true, true);
            //MyDoll.PlayerObject.DequeueTransfers();
        }

        public void UpdateColorScheme()
        {
            MyDoll.SetColorScheme(player.ColorScheme);
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            BobPhsxCharSelect HeroDollPhsx = MyDoll.MyPhsx as BobPhsxCharSelect;

            MyDoll.CapeWind = Cape.SineWind(new Vector2(-1.25f, -.1f), .5f, .05f, Tools.t) * .7f;
            MyDoll.MyPhsx.OnGround = true;
            MyDoll.MyPhsx.Vel = Vector2.Zero;
        }

        public void DrawBob()
        {
            Camera cam = Tools.CurLevel.MainCamera;
            Vector2 HoldZoom = cam.Zoom;

            cam.FancyPos.AbsVal = cam.FancyPos.RelVal = cam.Data.Position;

            // Draw the stickmen
            float Zoom = 2f * MyDoll.MyPhsx.DollCamZoomMod;
            cam.SetVertexZoom(new Vector2(Zoom));

            Vector2 Pos = MyPile.Pos;
            Vector2 CurHeroDollPos = Pos / Zoom + Tools.CurGameData.CamPos;

            MyDoll.Move(CurHeroDollPos - MyDoll.Pos);
            MyDoll.Draw();

            Tools.QDrawer.Flush();

            cam.Zoom = HoldZoom;
            cam.SetVertexCamera();
        }

        protected override void MyDraw()
        {
            base.MyDraw();

            if (!Core.Show || Core.Released || Hid) return;

            DrawBob();
        }
    }
}