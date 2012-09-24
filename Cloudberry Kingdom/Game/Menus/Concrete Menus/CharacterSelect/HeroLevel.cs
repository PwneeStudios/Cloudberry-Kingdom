using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#if PC_VERSION
#elif XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif
using Drawing;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class HeroLevel : StartMenuBase
    {
        CharacterSelect MyCharacterSelect;
        public HeroLevel(int Control, CharacterSelect MyCharacterSelect)
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

            SetHeroLevel();

            CharacterSelect.Shift(this);
        }

        public bool ShowHeroLevel = false;
        void SetHeroLevel()
        {
            if (!ShowHeroLevel) return;

            Tools.StartGUIDraw();
            if (MyCharacterSelect.Player.Exists)
            {
                //string name = MyCharacterSelect.Player.GetName();
                string name = "Level 56";
                Text = new EzText(name, Tools.Font_Grobold42, true, true);
            }
            else
            {
                Text = new EzText("ERROR", Tools.LilFont, true, true);
            }

            Text.Shadow = false;
            Text.PicShadow = false;

            Tools.EndGUIDraw();
        }
    }
}