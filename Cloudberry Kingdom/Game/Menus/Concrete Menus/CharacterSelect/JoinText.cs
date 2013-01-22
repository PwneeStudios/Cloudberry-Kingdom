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
    public class JoinText : CkBaseMenu
    {
        CharacterSelect MyCharacterSelect;
        public JoinText(int Control, CharacterSelect MyCharacterSelect)
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

            // Press A to join
            Tools.Warning();
#if PC_VERSION
            string pressa = string.Format(Localization.WordString(Localization.Words.PressToJoin), ButtonString.Go_Controller(89));
#else
            string pressa = string.Format(Localization.WordString(Localization.Words.PressToJoin), ButtonString.Go(89));
#endif
            Text = new EzText(pressa, Resources.Font_Grobold42, true, true);
            Text.Scale = .7765f;

            Text.ShadowOffset = new Vector2(7.5f, 7.5f);
            Text.ShadowColor = new Color(30, 30, 30);
            Text.ColorizePics = true;

            MyPile.Add(Text);

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

        void SetGamerTag()
        {
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

        protected override void MyDraw()
        {
            if (CharacterSelectManager.FakeHide)
                return;

            base.MyDraw();
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;
            MyCharacterSelect.MyState = CharacterSelect.SelectState.Beginning;
            MyCharacterSelect.MyDoll.ShowBob = false;
            MyCharacterSelect.MyGamerTag.ShowGamerTag = false;
            MyCharacterSelect.Player.Exists = false;

            // Use this if statement if you want keyboard to control all characters (For debugging)
            //if (ButtonCheck.State(ControllerButtons.A, -2).Pressed)

            if (ButtonCheck.State(ControllerButtons.A, Control).Pressed)
            {
#if XBOX || XBOX_SIGNIN
                if (MyCharacterSelect.Player.MyGamer != null)
                    Call(new SimpleMenu(Control, MyCharacterSelect));
                else
                    Call(new SignInMenu(Control, MyCharacterSelect));
#else
                Call(new SimpleMenu(Control, MyCharacterSelect));
#endif
                Hide();
            }
        }
    }
}