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
#if NOT_PC && (XBOX || XBOX_SIGNIN)
    public class SignInMenu : CkBaseMenu
    {
        CharacterSelect MyCharacterSelect;
        public SignInMenu(int Control, CharacterSelect MyCharacterSelect) : base(false)
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

        public override void Init()
        {
            base.Init();

            SlideInLength = 0;
            SlideOutLength = 0;
            CallDelay = 0;
            ReturnToCallerDelay = 0;

            MakeSignInChoiceMenu();

            CharacterSelect.Shift(this);
        }

        void MakeSignInChoiceMenu()
        {
            MyMenu = new Menu();
            MyMenu.Control = MyCharacterSelect.PlayerIndex;
            MyMenu.SelectIcon = new QuadClass();
            MyMenu.SelectIcon.ShadowOffset = new Vector2(7.5f, 7.5f);
            MyMenu.SelectIcon.ShadowColor = new Color(30, 30, 30);
            MyMenu.SelectIcon.Scale(90);
            MyMenu.SelectIcon.Quad.MyTexture = ButtonTexture.Go;

            MyMenu.FixedToCamera = false;

            EnsureFancy();

            MyMenu.OnB = new MenuReturnToCallerLambdaFunc(this);

            Vector2 pos = new Vector2(0, 0);
            float YSpacing = 200;

            MenuItem item;
            EzFont font = Resources.Font_Grobold42;
            float FontScale = .775f;

            item = new MenuItem(new EzText("Sign in?", font));
            item.Name = "Header";
            item.MyText.Scale = .89f;

            MyMenu.Add(item);
            item.Selectable = false;
            pos.Y -= 1.35f * YSpacing;

            string[] ItemString = { "Yes", "No" };
            for (int i = 0; i < 2; i++)
            {
                item = new MenuItem(new EzText(ItemString[i], font));
                item.MyText.Scale = item.MySelectedText.Scale = FontScale;
                item.SelectionOscillate = false;

                MyMenu.Add(item);
                item.SelectedPos = item.Pos = pos; pos.Y -= YSpacing;
                item.SelectedPos.X += 15;
                item.SelectIconOffset = new Vector2(275, 3);
            }
            MyMenu.SelectItem(1);

            MyMenu.Items[1].Name = "Yes";
            MyMenu.Items[1].Go = new SignInYesLambda(this);

            MyMenu.Items[2].Name = "No";
            MyMenu.Items[2].Go = new SignInNoLambda(this);

            MyMenu.MyPieceQuadTemplate = null;

            SetPos();
        }

        class SignInNoLambda : Lambda_1<MenuItem>
        {
            SignInMenu sim;
            public SignInNoLambda(SignInMenu sim)
            {
                this.sim = sim;
            }

            public void Apply(MenuItem item)
            {
                sim.MyCharacterSelect.Player.StoredName = "";
                sim.MyCharacterSelect.Player.Init();
                sim.Call(new SimpleMenu(sim.Control, sim.MyCharacterSelect));
                sim.Hide();
            }
        }

        class SignInYesLambda : Lambda_1<MenuItem>
        {
            SignInMenu sim;
            public SignInYesLambda(SignInMenu sim)
            {
                this.sim = sim;
            }

            public void Apply(MenuItem item)
            {
                if (!Guide.IsVisible)
                {
#if XBOX
                    if (!Guide.IsVisible)
                        Guide.ShowSignIn(4, false);
#else
                    Guide.ShowSignIn(1, false);
#endif
                }

                sim.GamerGuideUp = true;
            }
        }

        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-416.668f, 527.7777f); _item.MyText.Scale = 0.89f; _item.MySelectedText.Scale = 1f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(-108.333f, 149.4445f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(-88.88965f, -130.3333f); }
            _item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(-72.22559f, -108.8889f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(-88.88965f, -130.3333f); }
        }

#if XBOX || XBOX_SIGNIN
        bool GamerGuideUp = false;
        bool GuideUpPhsxStep()
        {
            if (!GamerGuideUp && !Guide.IsVisible) return false;

            if (Guide.IsVisible)
            {
                GamerGuideUp = true;
                return true;
            }
            else
                GamerGuideUp = false;

            // The guide just went down. Check to see if someone signed in and act accordingly.
            if (MyCharacterSelect.Player.MyGamer != null)
            {
                Call(new SimpleMenu(Control, MyCharacterSelect));
                Hide();
            }
            else
                ReturnToCaller();

            return false;
        }
#endif

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

#if XBOX || XBOX_SIGNIN
            if (GuideUpPhsxStep())
                return;
#endif

            MyCharacterSelect.MyState = CharacterSelect.SelectState.Selecting;
            MyCharacterSelect.MyDoll.ShowBob = false;
            MyCharacterSelect.MyGamerTag.ShowGamerTag = false;
        }
    }
#endif
}