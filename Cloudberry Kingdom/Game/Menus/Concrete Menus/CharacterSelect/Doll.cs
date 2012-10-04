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
    public class Doll : CkBaseMenu
    {
        CharacterSelect MyCharacterSelect;
        public Doll(int Control, CharacterSelect MyCharacterSelect) : base(false)
        {
            this.Tags += Tag.CharSelect;
            this.Control = Control;
            this.MyCharacterSelect = MyCharacterSelect;
            
            Constructor();
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            MyCharacterSelect = null;

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

            SlideInLength = 0;
            SlideOutLength = 0;
            CallDelay = 0;
            ReturnToCallerDelay = 0;

            MakeDoll();
        }

        void MakeDoll()
        {
            MyPile = new DrawPile();
            EnsureFancy();

            MyDoll = new Bob(BobPhsxNormal.Instance, false);
            MyDoll.MyPlayerIndex = MyCharacterSelect.Player.MyPlayerIndex;
            MyDoll.MyPiece = Tools.CurLevel.CurPiece;
            MyDoll.MyPieceIndex = MyCharacterSelect.PlayerIndex;
            MyDoll.CharacterSelect = true;
            MyDoll.CharacterSelect2 = true;
            MyDoll.AffectsCamera = false;
            MyDoll.Immortal = true;
            MyDoll.CompControl = true;
            MyDoll.DrawWithLevel = false;

            PhsxData data = new PhsxData();
            MyDoll.Init(false, data, Tools.CurGameData);

            MyDoll.SetColorScheme(MyCharacterSelect.Player.ColorScheme);

            MyDoll.PlayerObject.EnqueueAnimation("Stand", 0, true, true);
            MyDoll.PlayerObject.DequeueTransfers();

            Tools.CurLevel.AddBob(MyDoll);
        }

        public void UpdateColorScheme()
        {
            MyDoll.SetColorScheme(MyCharacterSelect.Player.ColorScheme);
        }

        public void GetIndices(int[] ItemIndex, List<MenuListItem>[] ItemList)
        {
            ItemIndex[0] = ItemList[0].FindIndex(item => (ClrTextFx)item.obj == MyDoll.MyColorScheme.SkinColor);

            ItemIndex[1] = ColorSchemeManager.BeardInfo.FindIndex(hat => hat == MyDoll.MyColorScheme.BeardData);
            ItemIndex[2] = ColorSchemeManager.HatInfo.FindIndex(hat => hat == MyDoll.MyColorScheme.HatData);

            ItemIndex[3] = ItemList[3].FindIndex(item => (ClrTextFx)item.obj == MyDoll.MyColorScheme.CapeColor);
            ItemIndex[4] = ItemList[4].FindIndex(item => (ClrTextFx)item.obj == MyDoll.MyColorScheme.CapeOutlineColor);

            for (int i = 0; i < 5; i++)
                if (ItemIndex[i] < 0)
                    ItemIndex[i] = 0;
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            BobPhsxCharSelect DollPhsx = MyDoll.MyPhsx as BobPhsxCharSelect;

            MyDoll.CapeWind = Cape.SineWind(new Vector2(-1.25f, -.1f), .5f, .05f, Tools.t) * .7f;
            MyDoll.MyPhsx.OnGround = true;
        }

        public bool ShowBob = false;
        public void DrawBob()
        {
            if (ShowBob)
            {
                //MyPile.Pos = new Vector2(-152.7773f, 269.4444f);
                MyPile.Pos = new Vector2(-10, 260);

                Vector2 Pos = CharacterSelect.Centers[Control] + MyPile.Pos;
                Vector2 CurDollPos = Pos / (CharacterSelectManager.BobZoom / CharacterSelectManager.ZoomMod) + Tools.CurGameData.CamPos;
                //Vector2 CurDollPos = Pos / 2.35f + Tools.CurGameData.CamPos;

                MyDoll.Move(CurDollPos - MyDoll.Core.Data.Position);
                MyDoll.Draw();
            }
            else
                MyDoll.SkipPrepare = true;
        }
    }
}