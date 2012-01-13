using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#if PC_VERSION
#else
using Microsoft.Xna.Framework.GamerServices;
#endif
using Drawing;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
#if NOT_PC
    public partial class CharacterSelect
    {
        void MakeSignInChoiceMenu()
        {
            SignInChoiceMenu = new Menu();
            SignInChoiceMenu.Control = PlayerIndex;
            SignInChoiceMenu.SelectIcon = new QuadClass();
            //SignInChoiceMenu.SelectIcon.Shadow = true;
            SignInChoiceMenu.SelectIcon.ShadowOffset = new Vector2(7.5f, 7.5f);
            SignInChoiceMenu.SelectIcon.ShadowColor = new Color(30, 30, 30);
            //SignInChoiceMenu.SelectIcon.Scale(75);
            SignInChoiceMenu.SelectIcon.Scale(90);
            SignInChoiceMenu.SelectIcon.Quad.MyTexture = ButtonTexture.Go;

            SignInChoiceMenu.FixedToCamera = false;
            SignInChoiceMenu.FancyPos = new FancyVector2(FancyCenter);

            SignInChoiceMenu.MyPieceQuadTemplate = PieceQuad.Get("DullMenu");

            SignInChoiceMenu.OnB = delegate(Menu menu)
            {
                SetState(SelectState.PressAtoJoin);
                return true;
            };

            Vector2 pos = new Vector2(0, 0);
            //float YSpacing = 142;
            float YSpacing = 200;

            MenuItem item;
            //EzFont font = Tools.Font_Dylan24;
            EzFont font = Tools.Font_DylanThin42;
            float FontScale = .775f;

            item = new MenuItem("Sign in?", font);
            //item.MyText.AddBackdrop(new Vector2(.72f, .55f), Vector2.Zero, new Vector2(350, 0), new Vector2(350, 1000));
            item.MyText.Scale = .89f;

            ModBackdrop(item.MyText);
            SignInChoiceMenu.Add(item);
            item.Selectable = false;
            pos.Y -= 1.35f * YSpacing;

            string[] ItemString = { "Yes", "No" };
            for (int i = 0; i < 2; i++)
            {
                item = new MenuItem(ItemString[i], font);
                item.MyText.Scale = item.MySelectedText.Scale = FontScale;
                item.SelectionOscillate = false;
                //item.MyText.AddBackdrop(new Vector2(.65f, .55f), Vector2.Zero, new Vector2(350, 0), new Vector2(350, 1000));
                //ModBackdrop(item.MyText);
                //item.MySelectedText.AddBackdrop(new Vector2(.65f, .65f), Vector2.Zero, new Vector2(370, 0), new Vector2(370, 1000));
                //ModBackdrop(item.MySelectedText);

                SignInChoiceMenu.Add(item);
                item.SelectedPos = item.Pos = pos; pos.Y -= YSpacing;
                item.SelectedPos.X += 15;
                item.SelectIconOffset = new Vector2(275, 3);
            }
            SignInChoiceMenu.SelectItem(1);

            SignInChoiceMenu.Items[1].Go = _item =>
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

                    GamerGuideUp = true;
                };
            SignInChoiceMenu.Items[2].Go = _item =>
                {
                    Player.StoredName = "";
                    Player.Init();
                    SetState(SelectState.SimpleSelect);
                };

            SignInChoiceMenu.MyPieceQuadTemplate = null;
            //SignInMenu.SetBoundary(new Vector2(15, 15));
        }
    }
#endif
}