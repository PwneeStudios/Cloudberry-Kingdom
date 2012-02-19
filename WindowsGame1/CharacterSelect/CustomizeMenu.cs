using Microsoft.Xna.Framework;
using System;
using Drawing;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class CustomizeMenu : StartMenuBase
    {
        CharacterSelect Parent;

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            Parent = null;
        }

        public CustomizeMenu(int Control, CharacterSelect Parent) : base(false)
        {
            this.Control = Control;
            this.Parent = Parent;

//#if NOT_PC
            this.AutoDraw = false;
//#endif
            Constructor();
        }

        void ModBackdrop(EzText Text)
        {
            Text.BackdropModAlpha = .6f;
            Text.Backdrop.SetColor(new Color(200, 200, 200));
        }
        
        protected override void SetItemProperties(MenuItem item)
        {
            //base.SetItemProperties(item);

            item.MyText.Scale = item.MySelectedText.Scale = FontScale;

            item.MySelectedText.MyFloatColor = new Color(50, 220, 50).ToVector4();

            item.Go = null;
            
#if PC_VERSION
                item.Padding += new Vector2(0, 35);
#endif

            item.SelectIconOffset = new Vector2(0, -160);// new Vector2(435, 3);
        }

        protected override void AddItem(MenuItem item)
        {
            base.AddItem(item);

#if PC_VERSION
            item.Padding.Y -= 37;
#endif
        }

        QuadClass backdrop;
        Vector2 DefaultBackdropSize;
        public override void Init()
        {
            base.Init();

            MyPile = new DrawPile();
            MyPile.FancyPos.UpdateWithGame = true;

            // Make the menu
            MyMenu = new Menu(false);

            Control = this.Control;
            //MyMenu.ShowLastActivated = true;

            MyMenu.OnB = null;

#if NOT_PC
            MyMenu.SelectIcon = new QuadClass();
            //MyMenu.SelectIcon.Scale(75);
            MyMenu.SelectIcon.Scale(90);
            MyMenu.SelectIcon.Quad.MyTexture = ButtonTexture.Go;
#endif
            MyMenu.MyPieceQuadTemplate = PieceQuad.Get("DullMenu");

            MyMenu.OnB = menu => { Back(); return true; };


            ItemPos = new Vector2(-365, 55 + 110);
            PosAdd = new Vector2(0, -168 - 23);
            SelectedItemShift = new Vector2(75, 0);
            FontScale = .78f;
            
            AddItem(new MenuItem(new EzText("Skin", ItemFont)));
            AddItem(new MenuItem(new EzText("Border", ItemFont)));
            AddItem(new MenuItem(new EzText("Hat", ItemFont)));
            AddItem(new MenuItem(new EzText("Cape", ItemFont)));
            AddItem(new MenuItem(new EzText("Lining", ItemFont)));
            AddItem(new MenuItem(new EzText("Done", ItemFont)));


            MyMenu.Items[5].Go = _item =>
            {
#if PC_VERSION
                Parent.SimpleToBack();
#else
                if (CurClrSelect != null)
                    CurClrSelect.ReturnToCaller();

                Parent.SetState(SelectState.Waiting);
#endif
            };

            
            // Back
            //item = MakeBackButton();

            // Backdrop
            //QuadClass backdrop = new QuadClass("WoodMenu_1", 442);
            //backdrop.Size = backdrop.Size * new Vector2(1f, 1.86f);

            backdrop = new QuadClass("score screen", 482);// 442);
            backdrop.Size = backdrop.Size * new Vector2(1f, 2.03f);
            DefaultBackdropSize = backdrop.Size;
            backdrop.Quad.RotateUV();
            MyPile.Add(backdrop);
            backdrop.Pos =
                //new Vector2(-15.87302f, 39.68255f);
#if PC_VERSION
                new Vector2(55.55557f, 87.30157f);
#else
                new Vector2(-3.968235f, 95.23785f);
#endif

            backdrop.Show = false;

            EnsureFancy();

#if PC_VERSION
            MyPile.Pos = new Vector2(1000, 12);
            MyMenu.FancyPos.RelVal = new Vector2(1035f, 500f);
#else
            MyPile.Pos = new Vector2(0f, -655 + 150);
            MyMenu.FancyPos.RelVal = new Vector2(0, 0);
#endif
        }

        public override void OnAdd()
        {
            base.OnAdd();

#if PC_VERSION
            SlideOut(PresetPos.Right, 0);
#else
            SlideOut(PresetPos.Bottom, 0);
#endif
        }

        /// <summary>
        /// The current color selection GameObject, if there is one.
        /// Set to null once the selection object starts sliding out.
        /// </summary>
        public ColorSelectPanel CurClrSelect;

        /// <summary>
        /// The custom menu index associated with the current color selection box
        /// </summary>
        int ClrSelectIndex;

        public void CreateColorSelect()
        {
            ColorSelectPanel ClrSelect;

            Vector2 ShiftSelect = Vector2.Zero;
#if PC_VERSION
            //float scale = 1.18f;
            float scale = 1.31f;
            ShiftSelect = new Vector2(63.4917f, 0f);
#else
            //float scale = 1.248f;
            float scale = 1.31f;
#endif

            

            // Make the hat select
            if (MyMenu.CurIndex == 2)
            {
#if PC_VERSION
                int total = CharacterSelectManager.AvailableHats.Count;
                //int m = Tools.Restrict(4, 9, (int)Math.Ceiling(total / 4f));
                int m = Tools.Restrict(5, 9, (int)Math.Ceiling(total / 4f));
                float Height = m * 400 / 4;
                backdrop.SizeY = DefaultBackdropSize.Y * m / 4f;
#else
                int m = 4;
                float Height = 400;
#endif
                ClrSelect = new ColorSelectPanel(scale * new Vector2(300, Height), 4, m, Control);

                foreach (Hat hat in CharacterSelectManager.AvailableHats)
                {
                    QuadClass quad = new QuadClass();
                    quad.SetToDefault();

                    BaseQuad dollquad = Parent.Doll.PlayerObject.FindQuad(hat.QuadName);
                    if (hat.HatPicTexture != null)
                        quad.Quad.MyTexture = hat.HatPicTexture;
                    else
                    {
                        if (dollquad == null)
                            quad.Quad.MyTexture = Tools.TextureWad.FindByName("slashcircle");
                        else
                            quad.Quad.MyTexture = dollquad.MyTexture;
                    }
                    quad.Base.e1.X = 70 * scale;
                    quad.ScaleYToMatchRatio();

                    quad.Base.e1 *= hat.HatPicScale.X;
                    quad.Base.e2 *= hat.HatPicScale.Y;

                    if (Math.Abs(hat.HatPicShift.Y) > .4f)
                        quad.Quad.Shift(hat.HatPicShift * .9f);
                    else
                        quad.Quad.Shift(hat.HatPicShift * .365f);

                    ClrSelect.Grid.Add(quad, ColorSchemeManager.HatInfo.IndexOf(hat));
                }
            }
            // Make the color select
            else
            {
                var list = Parent.ItemList[MyMenu.CurIndex];

                // Count available colors
                int Count = 0;
                foreach (MenuListItem item in list)
                {
                    ClrTextFx data = (ClrTextFx)item.obj;

                    // Check if color is available
                    if (!CloudberryKingdomGame.UnlockAll)
                        if (data.Price > 0 && !PlayerManager.Bought(data)) continue;

                    Count++;
                }


                int WidthCount = 5;
                float ScaleSplotch = 1;
#if PC_VERSION
                int m = Tools.Restrict(5, 10, (int)Math.Ceiling(Count / 5f));
                //float Height = m * 400 / 5;
                //backdrop.SizeY = DefaultBackdropSize.Y * m / 5f;
                //WidthCount = 5;
                //ScaleSplotch = 1;
                float Height = m * 400 / 4;
                if (m == 6) ShiftSelect.Y -= 100;
                backdrop.SizeY = DefaultBackdropSize.Y * m / 4f;
                WidthCount = 5;
                ScaleSplotch = 1.08f;
#else
                int m = 5;
                float Height = 400;
#endif
                ClrSelect = new ColorSelectPanel(scale * new Vector2(300, Height), WidthCount, m, Control);

                foreach (MenuListItem item in list)
                {
                    ClrTextFx data = (ClrTextFx)item.obj;

                    // Check if color is available
                    if (!CloudberryKingdomGame.UnlockAll)
                        if (data.Price > 0 && !PlayerManager.Bought(data)) continue;

                    QuadClass quad = new QuadClass();
                    quad.SetToDefault();
                    // If there is a picture thumbnail associated with item,
                    // the quad should draw that thumbnail
                    if (data.PicTexture != null)
                    {
                        quad.Quad.MyTexture = data.PicTexture;

                        quad.Base.e1.X = 70 * scale;
                        quad.ScaleYToMatchRatio();

                        quad.Base.e1 *= data.PicScale.X;
                        quad.Base.e2 *= data.PicScale.Y;
                    }
                    else
                    {
                        if (data.Clr.A == 0)
                        {
                            quad.Quad.MyTexture = Tools.TextureWad.FindByName("slashcircle");
                            quad.Quad.MyEffect = Tools.BasicEffect;
                        }
                        else
                        {
                            if (data.PicTexture != null)
                                quad.Quad.MyTexture = data.PicTexture;
                            else
                                quad.Quad.MyTexture = data.Texture;

                            if (data.UsePaintTexture)
                                quad.Quad.MyEffect = Tools.EffectWad.FindByName("Paint");
                            else
                                quad.Quad.MyEffect = Tools.BasicEffect;

                            quad.Quad.SetColor(data.Clr);
                        }
                        quad.Base.e1.X = 60 * scale;
                        quad.Base.e2.Y = 70 * scale;
                    }

                    ClrSelect.Grid.Add(quad, list.IndexOf(item));
                }
            }
            
#if PC_VERSION
            ClrSelect.Grid.Pos = ShiftSelect + AmountShifted + new Vector2(1464.286f, -23.80954f) + new Vector2(1240, 0) + new Vector2(-1730.159f, 43.65082f);
            ClrSelect.Grid.SelectedScale = 1.5f;
#else
            //ClrSelect.Grid.Pos = AmountShifted + new Vector2(50f, -490f);
            ClrSelect.Grid.Pos = AmountShifted + new Vector2(-20f, -480f);
#endif

            //ClrSelect.Grid.SetIndex(Parent.ItemIndex[MyMenu.CurIndex]);
            ClrSelect.Grid.SetIndexViaAssociated(Parent.ItemIndex[MyMenu.CurIndex]);

            MyGame.AddGameObject(ClrSelect);
#if PC_VERSION
            ClrSelect.SlideOut(PresetPos.Right, 0);
#else
            ClrSelect.SlideOut(PresetPos.Bottom, 0);
#endif
            //ClrSelect.SlideIn(6);
            ClrSelect.SlideIn(0);

            // If there was a color select already up, remove it
            if (CurClrSelect != null)
                CurClrSelect.Release();

            CurClrSelect = ClrSelect;
            ClrSelectIndex = MyMenu.CurIndex;

            // Hide the menu
            MyMenu.Show = false;
            backdrop.Show = true;
        }

        protected override void MyPhsxStep()
        {
            // Skip menu phsx if color select is up and mouse is not in use or mouse is over color select
#if PC_VERSION
            //if (!(CurClrSelect != null && (!Tools.TheGame.MouseInUse || CurClrSelect.Grid.MouseInBox)))
            if (CurClrSelect == null)
                base.MyPhsxStep();
#else
            if (CurClrSelect == null)
                base.MyPhsxStep();
#endif
                

            // When set to true the menu phsx will be skipped
#if PC_VERSION
            bool SkipMenuPhsx = false;
            if (CurClrSelect != null && (!Tools.TheGame.MouseInUse || CurClrSelect.Grid.MouseInBox))
                SkipMenuPhsx = true;
#else
            bool SkipMenuPhsx = (CurClrSelect != null);
#endif


            // When the color select is up
            if (CurClrSelect != null)
            {
                // Check if the user is done selecting a color
                if (CurClrSelect.Grid.Done)
                {
                    // If the user canceled
                    if (CurClrSelect.Grid.ExitState == GUIExitState.Cancel)
                    {
                        Parent.ItemIndex[ClrSelectIndex] = Parent.HoldIndex;
                        MyMenu.BackSound.Play();
                    }
                    // If the user selected something
                    else
                    {
                        MyMenu.SelectSound.Play();

                        // Save new custom color scheme
                        Parent.Player.CustomColorScheme = Parent.Player.ColorScheme;
                        Parent.Player.ColorSchemeIndex = -1;
                    }

                    // Slide out the color select
                    CurClrSelect.ReturnToCaller();
                    CurClrSelect = null;

                    MyMenu.Show = true;
                    backdrop.Show = false;
                    SkipMenuPhsx = true;
                    SkipPhsx = 2;

                    // Burn one phsx step of the menu
                    bool Hold = MyMenu.CheckForOutsideClick;
                    MyMenu.CheckForOutsideClick = false;
                    MyMenu.PhsxStep();
                    MyMenu.PhsxStep();
                    MyMenu.PhsxStep();
                    MyMenu.CheckForOutsideClick = Hold;
                    MyMenu.SkipPhsx = true;
                }
                else
                {
#if PC_VERSION
                    // If the mouse is in the box update the color/texture index
                    if (CurClrSelect.Grid.MouseInBox)                                            
                        Parent.ItemIndex[ClrSelectIndex] = CurClrSelect.Grid.GetAssociatedIndex();
                    // Otherwise revert to last selected index
                    else
                        Parent.ItemIndex[ClrSelectIndex] = Parent.HoldIndex;
#else
                    // Update the color/texture index
                    //Parent.ItemIndex[ClrSelectIndex] = CurClrSelect.Grid.GetIndex();
                    Parent.ItemIndex[ClrSelectIndex] = CurClrSelect.Grid.GetAssociatedIndex();
#endif
                }

                Parent.Customize_UpdateColors();
            }

            if (!SkipMenuPhsx)
            {
                if (Parent.MyState != SelectState.CustomizeSelect)
                    return;

                // Check to see if we should bring a new color select up
                if (CurClrSelect == null)
                if (ButtonCheck.State(ControllerButtons.X, Control).Pressed ||
                    ButtonCheck.State(ControllerButtons.A, Control).Pressed)
                {
                    if (MyMenu.CurIndex < Parent.ItemIndex.Length && !MyMenu.NoneSelected
#if PC_VERSION
                        // Skip if the color select is up but the mouse isn't in use
                        && !(!Tools.TheGame.MouseInUse && CurClrSelect != null))
#else
                        )
#endif
                    {
                        Parent.HoldIndex = Parent.ItemIndex[MyMenu.CurIndex];

                        MyMenu.LastActivatedItem = MyMenu.CurIndex;

                        if (CurClrSelect != null)
                            CurClrSelect.ReturnToCaller();
                        CreateColorSelect();
                        MyMenu.SelectSound.Play();
                    }
                }

                // Check for the back button if color select isn't up
                if (CurClrSelect == null && ButtonCheck.State(ControllerButtons.B, Control).Pressed)
                    Back();
            }

            // Animation
            Parent.SelectingDollAnimations();
        }

        void Back()
        {
            if (CurClrSelect != null)
                CurClrSelect.ReturnToCaller();

            ButtonCheck.PreventInput();
            Parent.SetState(SelectState.SimpleSelect);
            MyMenu.BackSound.Play();
        }
    }
}