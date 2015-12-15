using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#if PC
#elif XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif
using CoreEngine;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class GamerTag : CkBaseMenu
    {
        CharacterSelect MyCharacterSelect;
        public GamerTag(int Control, CharacterSelect MyCharacterSelect)
            : base(false)
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
        }

        EzText Text;
        public override void Init()
        {
            base.Init();

            SlideInLength = 0;
            SlideOutLength = 0;
            CallDelay = 0;
            ReturnToCallerDelay = 0;

            MyPile = new DrawPile();
            EnsureFancy();

            SetGamerTag();

            CharacterSelect.Shift(this);
        }

        public static void ScaleGamerTag(EzText GamerTag)
        {
            GamerTag.Scale *= 850f / GamerTag.GetWorldWidth();

            float Height = GamerTag.GetWorldHeight();
            float MaxHeight = 380;
            if (Height > MaxHeight)
                GamerTag.Scale *= MaxHeight / Height;
        }

        public bool ShowGamerTag = false;
        void SetGamerTag()
        {
            if (!ShowGamerTag) return;

            Tools.StartGUIDraw();
            if (MyCharacterSelect.Player.Exists)
            {
                string name = MyCharacterSelect.Player.GetName();
                Text = new EzText(name, Resources.Font_Grobold42, true, true);
                ScaleGamerTag(Text);
            }
            else
            {
                Text = new EzText("ERROR", Resources.LilFont, true, true);
            }

            Text.Shadow = false;
            Text.PicShadow = false;

            Tools.EndGUIDraw();
        }
    }
}