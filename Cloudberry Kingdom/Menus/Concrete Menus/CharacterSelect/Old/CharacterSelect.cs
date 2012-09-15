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
    public enum SelectState { Initialize, PressAtoJoin, SignInChoice, SimpleSelect, CustomizeSelect, Waiting, Done, Leaving, Back };
    public partial class CharacterSelect
    {
        public bool Join = false;

        /// <summary>
        /// The length of time to delay phsx after a new menu is switched to
        /// </summary>
        int DelayPhsxLength = 16; // 55

        public SelectState MyState;
        bool GamerGuideUp = false;
        bool QuickJoin; // True if the character select has been brought up mid game

        FancyVector2 FancyCenter;

        bool ShowColorSelect = false;
        public int HoldIndex;

        public Bob Doll;

        BackdropPanel MyBackdrop;
        public SimpleMenu Simple;
        public CustomizeMenu Customize;
        public ArrowMenu Arrows;

        Menu SignInChoiceMenu;

        EzText GamerTag;
        EzText JoinText;

        QuadClass Backdrop;

        int PlayerIndex;
        public PlayerData Player { get { return PlayerManager.Get(PlayerIndex); } }

        public int[] ItemIndex = new int[5];
        public List<MenuListItem>[] ItemList = { ColorSchemeManager.ColorList, ColorSchemeManager.OutlineList, ColorSchemeManager.HatList, ColorSchemeManager.CapeColorList, ColorSchemeManager.CapeOutlineColorList };

        Vector2 PrevDir;
        int NoMoveCount;
        static int NoMoveDuration = 20;

        Vector2 Center, NormalZoomCenter;
        static float Left = -1115;
        static float Shift = -2 * Left / 3;
        static float Upper = 0;

        public static float Width;
        static Vector2[] Centers;
        public void InitCenters()
        {
            if (Centers == null)
            {
                Centers = new Vector2[4];
                float span = -160 + Tools.CurCamera.GetWidth() * CharacterSelectManager.ZoomMod;

                Width = span / 4f;

                for (int i = 0; i < 4; i++)
                    Centers[i] = new Vector2(-span / 2 + Width / 2 + i * Width, 0);
            }
        }

        Vector2 DollPos, DollVel;

        int Step;
        bool Accelerate;

        public void Release()
        {
            //Tools.TheGame.Resolution.Backbuffer.X = 1;
            FancyCenter.Release();

            MyBackdrop.Release();
            Simple.Release();
            Customize.Release();
            Arrows.Release();

            if (Doll != null)
            {
                Doll.Core.MyLevel.Bobs.Remove(Doll);
                Doll.Release(); Doll = null;
            }

            if (SignInChoiceMenu != null) SignInChoiceMenu.Release(); SignInChoiceMenu = null;

            if (GamerTag != null) GamerTag.Release(); GamerTag = null;
            if (JoinText != null) JoinText.Release(); JoinText = null;

            Backdrop.Release();
        }

        Vector2 DefaultDollPos = new Vector2(0, 0);
        Vector2 BobPos = new Vector2(0, 25);

        //Vector2 GamerTagRelPos = new Vector2(0, -455);
        Vector2 GamerTagRelPos = new Vector2(0, -602);
        public void SetState(SelectState NewState) { SetState(NewState, false); }
        public void SetState(SelectState NewState, bool Force) { SetState(NewState, Force, false); }
        public void SetState(SelectState NewState, bool Force, bool Invert)
        {
            Accelerate = false;

            bool Up = true;
            if (PlayerIndex % 2 == 0) Up = false;


            if (NewState != MyState || Force)
            {
                if ((MyState != SelectState.SimpleSelect && MyState != SelectState.CustomizeSelect) ||
                    (NewState != SelectState.SimpleSelect && NewState != SelectState.CustomizeSelect))
                    Step = 0;

                GUI_Panel.PresetPos dest;

                int Frames = 37;// 42;
                float BackdropY;
                switch (NewState)
                {
                    case SelectState.Done:
                    case SelectState.Initialize:
                        dest = GUI_Panel.PresetPos.Bottom;

                        MyBackdrop.SlideOut(dest, 0);
                        Simple.SlideOut(dest, 0);
                        Arrows.SlideOut(dest, 0);
                        Customize.SlideOut(dest, Frames);
                        Customize.SlideOut(dest, Frames);
      
                        BackdropY = -1550;
                        DollPos = DefaultDollPos;
                        DollVel = new Vector2(0, 0);
                        Backdrop.FancyPos.RelVal = new Vector2(0, BackdropY);
                        if (Up)
                            MyBackdrop.SlideOut(GUI_Panel.PresetPos.Bottom, 0);
                        else
                            MyBackdrop.SlideOut(GUI_Panel.PresetPos.Top, 0);


                        GamerTag.FancyPos.RelVal = new Vector2(0, BackdropY) + GamerTagRelPos;
                        if (QuickJoin)
                        {
                            if (Math.Abs(JoinText.FancyPos.GetDest().Y) < 800 || MyState == SelectState.Initialize)
                                JoinText.FancyPos.RelVal = new Vector2(0, 1700);
                        }
                        else
                        {
                            if (Math.Abs(JoinText.FancyPos.GetDest().Y) < 800 || MyState == SelectState.Initialize)
                                JoinText.FancyPos.RelVal = new Vector2(0, -1450);
                        }
                        //CustomizeMenu.FancyPos.RelVal = new Vector2(0, BackdropY - 675);

#if NOT_PC
                        SignInChoiceMenu.FancyPos.RelVal = new Vector2(0, -1550*1.1f);
                        SignInChoiceMenu.FancyPos.Update();
#else
                        SignInChoiceMenu.FancyPos.RelVal = new Vector2(-10000, -15500);
                        SignInChoiceMenu.FancyPos.Update();
#endif
                        break;


                    case SelectState.PressAtoJoin:
                        Player.Exists = false;

                        if (QuickJoin)
                        {
                            /*if (MyState == SelectState.SignInChoice)
                                SetState(SelectState.SimpleSelect);
                            else*/
                                SetState(SelectState.Done);
                            return;
                        }

                        bool AlreadySlidIn = false; // Whether the backdrop has already been slid in
                        if (MyState != SelectState.Initialize)
                        {
                            AlreadySlidIn = true;
                            Up = true;
                        }

                        dest = GUI_Panel.PresetPos.Bottom;

                        if (Simple.Active) Simple.SlideOut(dest, Frames);
                        if (Arrows.Active) Arrows.SlideOut(dest, Frames);
                        Customize.SlideOut(GUI_Panel.PresetPos.Right, Frames);

                        if (AlreadySlidIn)
                            MyBackdrop.SlideIn(Frames);
                        else
                        {
                            if (Up)
                            {
                                MyBackdrop.SlideOut(GUI_Panel.PresetPos.Bottom, 0);
                                MyBackdrop.SlideIn(Frames);
                            }
                            else
                            {
                                MyBackdrop.SlideOut(GUI_Panel.PresetPos.Top, 0);
                                MyBackdrop.SlideIn(Frames);
                            }
                        }

                        Simple.SlideOut(dest, Frames);
                        Arrows.SlideOut(dest, Frames);
                        Customize.SlideOut(GUI_Panel.PresetPos.Bottom, Frames);

                        if (AlreadySlidIn)
                        {
                            MyBackdrop.SlideIn(Frames);
                            JoinText.FancyPos.LerpTo(new Vector2(0, 100), Frames);
                        }
                        else
                        {
                            if (Up)
                            {
                                MyBackdrop.SlideOut(GUI_Panel.PresetPos.Bottom, 0);
                                MyBackdrop.SlideIn(Frames);
                                if (Math.Abs(JoinText.FancyPos.GetDest().Y) < 800 || MyState == SelectState.Initialize)
                                    JoinText.FancyPos.LerpTo(new Vector2(0, -1450), new Vector2(0, 100), Frames);
                            }
                            else
                            {
                                MyBackdrop.SlideOut(GUI_Panel.PresetPos.Top, 0);
                                MyBackdrop.SlideIn(Frames);
                                if (Math.Abs(JoinText.FancyPos.GetDest().Y) < 800 || MyState == SelectState.Initialize)
                                    JoinText.FancyPos.LerpTo(new Vector2(0, 1700), new Vector2(0, 100), Frames);
                            }
                        }


                        BackdropY = -1550;
                        DollPos = DefaultDollPos;
                        DollVel = new Vector2(0, 0);
                        Backdrop.FancyPos.LerpTo(new Vector2(0, BackdropY), Frames);

                        GamerTag.FancyPos.LerpTo(new Vector2(0, BackdropY) + GamerTagRelPos, Frames);
                        //JoinText.FancyPos.LerpTo(new Vector2(0, 100), Frames);

                        if (MyState == SelectState.SignInChoice)
                            SignInChoiceMenu.FancyPos.LerpTo(new Vector2(0, -1550*1.1f), Frames);
                        else
                        {
                            SignInChoiceMenu.FancyPos.RelVal = new Vector2(0, -1550*1.1f);
                            SignInChoiceMenu.FancyPos.Playing = false;
                        }

                        break;

                    case SelectState.Waiting:
                        dest = GUI_Panel.PresetPos.Bottom;

                        if (QuickJoin)
                        {
                            dest = GUI_Panel.PresetPos.Top;
                            BackdropY = 1800 * 1.3f;
                            MyBackdrop.SlideOut(dest, Frames);
                        }
                        else
                        {
                            BackdropY = 250;
                            MyBackdrop.SlideIn(Frames);
                        }
                        

                        if (Simple.Active) Simple.SlideOut(dest, Frames);
                        if (Arrows.Active) Arrows.SlideOut(dest, Frames);

                        if (Customize.Active) Customize.SlideOut(dest, Frames);

                        DollPos = DefaultDollPos;
                        DollVel = new Vector2(0, 0);
                        Backdrop.FancyPos.LerpTo(new Vector2(0, BackdropY), Frames);
                        GamerTag.FancyPos.LerpTo(new Vector2(0, BackdropY) + GamerTagRelPos, Frames);
                        if (Math.Abs(JoinText.FancyPos.GetDest().Y) < 800)
                            JoinText.FancyPos.LerpTo(new Vector2(0, 1700*1.1f), Frames);

                        SignInChoiceMenu.FancyPos.LerpTo(new Vector2(0, 1700*1.1f), Frames);

                        break;

                    case SelectState.Back:
                        Frames = 30;

                        if (Up)
                        {
                            BackdropY = 1800;
                            MyBackdrop.SlideOut(GUI_Panel.PresetPos.Top, Frames);
                        }
                        else
                        {
                            BackdropY = -1500;
                            MyBackdrop.SlideOut(GUI_Panel.PresetPos.Bottom, Frames);
                        }

                        if (Math.Abs(JoinText.FancyPos.GetDest().Y) < 800)
                            JoinText.FancyPos.LerpTo(new Vector2(0, (BackdropY - 300)*1.1f), Frames);

                        break;

                    case SelectState.Leaving:
                        dest = GUI_Panel.PresetPos.Bottom;

                        if (MyState != SelectState.Waiting)
                        {
                            BackdropY = -1500;
                            MyBackdrop.SlideOut(GUI_Panel.PresetPos.Bottom, Frames);
                        }
                        else
                        {
                            BackdropY = 1800;
                            MyBackdrop.SlideOut(GUI_Panel.PresetPos.Top, Frames);
                        }

                        Simple.SlideOut(dest, Frames);
                        if (Arrows.Active) Arrows.SlideOut(dest, Frames);

                        Customize.SlideOut(dest, Frames);
                        
                        DollPos = DefaultDollPos;
                        DollVel = new Vector2(0, 0);
                        
                        Backdrop.FancyPos.LerpTo(new Vector2(0, BackdropY), Frames);
                        GamerTag.FancyPos.LerpTo(new Vector2(0, BackdropY*1.1f) + GamerTagRelPos, Frames);

                        if (Math.Abs(JoinText.FancyPos.GetDest().Y) < 800)
                            JoinText.FancyPos.LerpTo(new Vector2(0, (BackdropY - 300)*1.1f), Frames);

                        SignInChoiceMenu.FancyPos.LerpTo(new Vector2(0, BackdropY - 100), Frames);

                        break;

                    case SelectState.CustomizeSelect:
                        Player.Exists = true;
                        CharacterSelectManager.UpdateAvailableHats();

                        GetIndices();

                        Simple.SlideOut(GUI_Panel.PresetPos.Bottom, Frames);
                        Arrows.SlideOut(GUI_Panel.PresetPos.Bottom, Frames);
                        Customize.SlideIn(Frames);

                        BackdropY = 425;

                        DollPos = DefaultDollPos;
                        Backdrop.FancyPos.LerpTo(new Vector2(0, BackdropY), Frames);                        

                        if (Math.Abs(JoinText.FancyPos.GetDest().Y) < 800)
                            JoinText.FancyPos.LerpTo(new Vector2(0, 1700), Frames);

                        GamerTag.FancyPos.LerpTo(new Vector2(0, -1500*1.1f) + GamerTagRelPos, Frames);
                        SignInChoiceMenu.FancyPos.LerpTo(new Vector2(0, 1700*1.1f), Frames);

                        break;

                    case SelectState.SimpleSelect:
                        if (MyState != SelectState.CustomizeSelect)
                            DollVel.X = 2;

                        Player.Exists = true;
                        CharacterSelectManager.UpdateAvailableHats();

                        SetGamerTag();

                        if (Invert)
                        {
                            Simple.SlideOut(GUI_Panel.PresetPos.Top, 0);
                            Arrows.SlideOut(GUI_Panel.PresetPos.Top, 0);
                            JoinText.FancyPos.RelVal = new Vector2(0, 1700);
                            Backdrop.FancyPos.LerpTo(new Vector2(0, 1900), 0);
                            GamerTag.FancyPos.LerpTo(new Vector2(0, 1900), 0);
                            JoinText.FancyPos.LerpTo(new Vector2(0, -1700), 0);
                        }
                        else
                        {
                            if (Math.Abs(JoinText.FancyPos.GetDest().Y) < 800)
                                JoinText.FancyPos.LerpTo(new Vector2(0, -1450), Frames);
                        }

                        MyBackdrop.SlideIn(Frames);
                        Simple.SlideIn(Frames);
                        Arrows.SlideIn(Frames);

                        Customize.SlideOut(GUI_Panel.PresetPos.Bottom, Frames);

                        //BackdropY = 250;
                        BackdropY = 500;
                        DollPos = DefaultDollPos;
                        Backdrop.FancyPos.LerpTo(new Vector2(0, BackdropY - 160), Frames);

                        GamerTag.FancyPos.LerpTo(new Vector2(0, BackdropY) + GamerTagRelPos, Frames);
                        //CustomizeMenu.FancyPos.LerpTo(new Vector2(0, -1300), Frames);

                        if (MyState == SelectState.SignInChoice)
                            SignInChoiceMenu.FancyPos.LerpTo(new Vector2(0, 1700*1.1f), Frames);
                        else
                            SignInChoiceMenu.FancyPos.RelVal = new Vector2(0, 1700);

                        break;

                    case SelectState.SignInChoice:
                        Player.Exists = false;

                        if (QuickJoin)
                        {
                            MyBackdrop.SlideOut(GUI_Panel.PresetPos.Top, 0);
                            MyBackdrop.SlideIn(Frames);

                            SignInChoiceMenu.FancyPos.RelVal = new Vector2(0, 1700);
                            SignInChoiceMenu.FancyPos.LerpTo(new Vector2(0, 120), Frames);
                        }
                        else
                            SignInChoiceMenu.FancyPos.LerpTo(new Vector2(0, 120), Frames);

                        BackdropY = -1550;
                        DollPos = BobPos;
                        DollVel = new Vector2(0, 0);
                        Backdrop.FancyPos.LerpTo(new Vector2(0, BackdropY), Frames);
                        GamerTag.FancyPos.LerpTo(new Vector2(0, BackdropY) + GamerTagRelPos, Frames);
                        if (Math.Abs(JoinText.FancyPos.GetDest().Y) < 800)
                            JoinText.FancyPos.LerpTo(new Vector2(0, 1700), Frames);
                        
                        //SignInChoiceMenu.FancyPos.LerpTo(new Vector2(0, 120), Frames);
                        break;
                }

                MyState = NewState;
            }
        }

        public CharacterSelect(int PlayerIndex, bool QuickJoin)
        {
            Tools.StartGUIDraw();

            this.PlayerIndex = PlayerIndex;
            this.QuickJoin = QuickJoin;

            InitCenters();
            Center = Centers[PlayerIndex] + new Vector2(0, -120);
            NormalZoomCenter = Center;
            FancyCenter = new FancyVector2();

            SetGamerTag();
            MakeText();

            MakeDoll();
            MakeBubble();
            
            GameData game = Tools.CurGameData;


            MyBackdrop = new BackdropPanel(PlayerIndex, this);
            MyBackdrop.Shift(NormalZoomCenter);
            game.AddGameObject(MyBackdrop);

            Simple = new SimpleMenu(PlayerIndex, this);
            Simple.Shift(NormalZoomCenter);
            game.AddGameObject(Simple);
            
            Customize = new CustomizeMenu(PlayerIndex, this);
            Customize.Shift(NormalZoomCenter);

            game.AddGameObject(Customize);

            Arrows = new ArrowMenu(PlayerIndex, this);
            Arrows.Shift(NormalZoomCenter);
            game.AddGameObject(Arrows);

            //MakeCustomizeMenu();

#if XBOX || XBOX_SIGNIN
            MakeSignInChoiceMenu();
#else
            // Dummy menu
            SignInChoiceMenu = new Menu();
            SignInChoiceMenu.FancyPos = new FancyVector2(FancyCenter);
#endif

            InitColorScheme(PlayerIndex);

            SetState(SelectState.Initialize, true);

            if (QuickJoin)
            {
#if XBOX || XBOX_SIGNIN
                if (Player.MyGamer == null)
                    SetState(SelectState.SignInChoice);
                else
                    SetState(SelectState.SimpleSelect);
#else
                SetState(SelectState.SimpleSelect);
#endif
            }
            else
            {
#if PC_VERSION
                if (PlayerIndex == 0)
                    SetState(SelectState.SimpleSelect, false, true);
                else
                    SetState(SelectState.PressAtoJoin);
#else
                SetState(SelectState.PressAtoJoin);
#endif
            }

            Tools.EndGUIDraw();
        }

        public void InitColorScheme(int PlayerIndex)
        {
            if (Player.ColorSchemeIndex == Unset.Int)
                SetIndex(PlayerIndex);
            else
                SetIndex(Player.ColorSchemeIndex);
        }

        public void StartCharacterSelect()
        {
            SetState(SelectState.Initialize);            
        }

        void GetIndices()
        {
            ItemIndex[0] = ItemList[0].FindIndex(item => (ClrTextFx)item.obj == Doll.MyColorScheme.SkinColor);
            //ItemIndex[1] = ItemList[1].FindIndex(item => (ClrTextFx)item.obj == Doll.MyColorScheme.OutlineColor);

            ItemIndex[1] = ColorSchemeManager.BeardInfo.FindIndex(hat => hat == Doll.MyColorScheme.BeardData);
            ItemIndex[2] = ColorSchemeManager.HatInfo.FindIndex(hat => hat == Doll.MyColorScheme.HatData);
            
            ItemIndex[3] = ItemList[3].FindIndex(item => (ClrTextFx)item.obj == Doll.MyColorScheme.CapeColor);
            ItemIndex[4] = ItemList[4].FindIndex(item => (ClrTextFx)item.obj == Doll.MyColorScheme.CapeOutlineColor);
            
            for (int i = 0; i < 5; i++)
                if (ItemIndex[i] < 0)
                    ItemIndex[i] = 0;
        }

        void MakeText()
        {
            // Press A to join
#if PC_VERSION
            JoinText = new EzText("Press\n" + ButtonString.Go_Controller(89) + "\nto join!", Tools.Font_Grobold42, 1000, true, true, .65f);
#else
            JoinText = new EzText("Press\n{pXbox_A,72,?}\nto join!", Tools.Font_Grobold42, true, true);
#endif
            JoinText.Scale = .7765f;

            //JoinText.Shadow = true;
            JoinText.ShadowOffset = new Vector2(7.5f, 7.5f);
            JoinText.ShadowColor = new Color(30, 30, 30);
            //JoinText.PicShadow = true;
            JoinText.ColorizePics = true;

            JoinText.FancyPos = new FancyVector2(FancyCenter);
            //JoinText.AddBackdrop();            
            ModBackdrop(JoinText);
        }

        void ModBackdrop(EzText Text)
        {
            if (Text.Backdrop == null) return;
            //Text.BackdropModAlpha = .6f;
            //Text.Backdrop.SetColor(new Color(200, 200, 200));

            Text.BackdropModAlpha = .835f;
            Text.Backdrop.SetColor(new Color(255, 255, 255));
        }

        void MakeDoll()
        {
            Doll = new Bob(BobPhsxNormal.Instance, false);
            Doll.MyPlayerIndex = Player.MyPlayerIndex;
            Doll.MyPiece = Tools.CurLevel.CurPiece;
            Doll.MyPieceIndex = PlayerIndex;
            Doll.CharacterSelect = true;
            Doll.CharacterSelect2 = true;
            Doll.AffectsCamera = false;
            Doll.Immortal = true;
            Doll.CompControl = true;
            Doll.DrawWithLevel = false;

            PhsxData data = new PhsxData();
            Doll.Init(false, data, Tools.CurGameData);

            Doll.SetColorScheme(Player.ColorScheme);

            Tools.CurLevel.AddBob(Doll);
        }

        void MakeBubble()
        {
            Backdrop = new QuadClass(FancyCenter);
            Backdrop.Base.e1.X = Backdrop.Base.e2.Y = 350;
            Backdrop.Quad.MyTexture = Tools.TextureWad.FindByName("CharMenu");//Bubble");
        }

        void SignInChoicePhsxStep()
        {
            SignInChoiceMenu.PhsxStep();
        }
        
        public void SelectingDollAnimations()
        {
            BobPhsxCharSelect DollPhsx = Doll.MyPhsx as BobPhsxCharSelect;

            bool New = true;
            if (New)
            {
                if (Step == 0)
                {
                    DollPhsx.OverrideAnimBehavior = false;
                    DollVel.X = 0;
                }
                if (Step == 1)
                {
                    Doll.PlayerObject.EnqueueAnimation("Stand");
                    Doll.PlayerObject.DequeueTransfers();

                    DollVel.X = 0;
                    DollPhsx.OverrideAnimBehavior = false;
                }
                else if (Step < 140)
                    Tools.Nothing();
                else
                {
                    DollVel.X = 0;
                    Doll.CapeWind = Cape.SineWind(new Vector2(-1.25f, .3f), .5f, .05f, Tools.t);
                    //Doll.Core.Data.Velocity = new Vector2(-10, 0);
                    //DollVel.X += .075f * (16 - DollVel.X);
                }
            }
            // Old, with wave.
            else
            {
                if (Step == 0)
                {
                    DollPhsx.OverrideAnimBehavior = false;
                    DollVel.X = 0;
                }
                if (Step == 1)
                {
                    //Doll.PlayerObject.EnqueueAnimation("Stand");
                    //Doll.PlayerObject.DequeueTransfers();

                    DollVel.X = 0;
                    DollPhsx.OverrideAnimBehavior = false;
                }
                else if (Step == 50)
                {
                    Doll.PlayerObject.EnqueueAnimation("Wave", 0, true, true);
                    Doll.PlayerObject.AnimQueue.Peek().AnimSpeed *= 5f;
                    Doll.PlayerObject.DestAnim().AnimSpeed *= 2.5f;

                    Doll.Core.Data.Velocity.X = 0;
                    DollPhsx.OverrideAnimBehavior = true;
                }
                else if (Step == 200)
                {
                    Doll.PlayerObject.EnqueueAnimation("Stand");
                    Doll.PlayerObject.AnimQueue.Peek().AnimSpeed *= 12.5f;

                    DollPhsx.OverrideAnimBehavior = false;
                }
                else if (Step < 240)
                    Tools.Nothing();
                else
                {
                    DollVel.X += .075f * (16 - DollVel.X);
                }
            }
        }

        public void SimpleToCustom()
        {
            SetState(SelectState.CustomizeSelect);
            Customize.MyMenu.SelectSound.Play();
        }

        public void SimpleToDone()
        {
            SetState(SelectState.Waiting);
            Customize.MyMenu.SelectSound.Play();
        }

        public void Randomize()
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == 1 || i == 2) continue;

                List<MenuListItem> list = new List<MenuListItem>(ItemList[i].Capacity);
                foreach (var item in ItemList[i])
                    if (PlayerManager.BoughtOrFree((Buyable)item.obj))
                        list.Add(item);

                ItemIndex[i] = ItemList[i].IndexOf(list.Choose(Tools.GlobalRnd));
            }

            Hat hat = CharacterSelectManager.AvailableHats.Choose(Tools.GlobalRnd);
            Hat beard = CharacterSelectManager.AvailableBeards.Choose(Tools.GlobalRnd);
            ItemIndex[1] = ColorSchemeManager.BeardInfo.IndexOf(beard);
            ItemIndex[2] = ColorSchemeManager.HatInfo.IndexOf(hat);

            Customize_UpdateColors();

            // Save new custom color scheme
            Player.CustomColorScheme = Player.ColorScheme;
            Player.ColorSchemeIndex = -1;
        }

        public void SimpleToBack()
        {
            if (QuickJoin)
            {
                SetState(SelectState.Done);
                Player.Exists = false;
            }
            else
                SetState(SelectState.PressAtoJoin);
            Customize.MyMenu.BackSound.Play();

//#if PC_VERSION
//            Player.Exists = true;
//#endif
        }

        void SimplePhsxStep()
        {
            if (!Tools.StepControl)
            {
                if (ButtonCheck.State(ControllerButtons.Y, PlayerIndex).Pressed)
                    SimpleToCustom();

                if (ButtonCheck.State(ControllerButtons.A, PlayerIndex).Pressed)
                    SimpleToDone();

                if (ButtonCheck.State(ControllerButtons.B, PlayerIndex).Pressed)
                    SimpleToBack();

                if (ButtonCheck.State(ControllerButtons.X, PlayerIndex).Pressed)
                    Randomize();
            }

            // Animation
            SelectingDollAnimations();
        }

        public void WaitingPhsxStep()
        {
            if (QuickJoin)
            {
                if (!FancyCenter.Playing)
                {
                    //CharacterSelectManager.Finish(PlayerIndex, true);
                    Join = true;
                    SetState(SelectState.Done);
                }

                return;
            }

            if (ButtonCheck.State(ControllerButtons.B, PlayerIndex).Pressed)
            {
                SetState(SelectState.SimpleSelect);
                Customize.MyMenu.BackSound.Play();
            }

            // Animation
            BobPhsxCharSelect DollPhsx = Doll.MyPhsx as BobPhsxCharSelect;
            if (Step == 1)
            {
                DollPhsx.OverrideAnimBehavior = false;
            }
            else
            {
                bool New = true;
                if (New)
                {
                    if (Step >= 32)
                    {
                        Doll.PlayerObject.EnqueueAnimation(0, 0, true, false);
                        Doll.PlayerObject.DequeueTransfers();
                    }
                }
                // Old, with wave and flip
                {
                    int Step_Flip = 32;                     // The time step to start the flip on
                    int Step_FlipSound = Step_Flip + 20;    // The time step to start the flip sound on
                    int Step_Wave = Step_FlipSound + 120;   // The time step to start the stickman hand waving
                    bool DoWave = false; // Whether to wave after the flip
                    bool DoFlip = false;
                    if (Step >= Step_Flip)
                    {
                        if (Step == Step_Flip)
                        {
                            if (DoFlip)
                            {
                                Doll.PlayerObject.EnqueueAnimation("Flip");
                                Doll.PlayerObject.AnimQueue.Peek().AnimSpeed *= 5f;
                                Doll.PlayerObject.DestAnim().AnimSpeed *= 2.5f;
                            }
                        }
                        else if (Step == Step_FlipSound)
                        {
                            if (DoFlip)
                                Tools.SoundWad.FindByName("Jump").Play();
                        }
                        else if (Step < Step_Wave)
                        {
                            if (Doll.PlayerObject.AnimQueue.Count == 0)
                            {
                                if (DoWave)
                                {
                                    Doll.PlayerObject.EnqueueAnimation("Wave", 0, true, false);
                                    Doll.PlayerObject.AnimQueue.Peek().AnimSpeed *= 5f;
                                    Doll.PlayerObject.DestAnim().AnimSpeed *= 2.5f;
                                }
                                else
                                {
                                    Doll.PlayerObject.EnqueueAnimation(0, 0, true, false);
                                    Doll.PlayerObject.AnimQueue.Peek().AnimSpeed *= 7.5f;
                                    Doll.PlayerObject.DestAnim().AnimSpeed *= 2.5f;
                                }
                            }
                        }
                    }
                }

                DollPhsx.OverrideAnimBehavior = true;
            }
        }

        void Customize_Dir(Vector2 Dir)
        {
            Customize_UpdateColors();
        }

        int HoldCapeIndex = 1, HoldCapeOutlineIndex = 1;
        public void Customize_UpdateColors()
        {
            bool ShowingCape =
                Player.ColorScheme.CapeColor.Clr.A > 0 ||
                Player.ColorScheme.CapeOutlineColor.Clr.A > 0;

            Player.ColorScheme.SkinColor = (ClrTextFx)ItemList[0][ItemIndex[0]].obj;
            //Player.ColorScheme.OutlineColor = (ClrTextFx)ItemList[1][ItemIndex[1]].obj;
            Player.ColorScheme.BeardData = ColorSchemeManager.BeardInfo[ItemIndex[1]];
            Player.ColorScheme.HatData = ColorSchemeManager.HatInfo[ItemIndex[2]];
            Player.ColorScheme.CapeColor = (ClrTextFx)ItemList[3][ItemIndex[3]].obj;
            Player.ColorScheme.CapeOutlineColor = (ClrTextFx)ItemList[4][ItemIndex[4]].obj;

            // If the cape has gone from not-shown to shown,
            // make sure both the cape color and cape outline color aren't invisible.
            if (!ShowingCape &&
                (Player.ColorScheme.CapeColor.Clr.A > 0 ||
                 Player.ColorScheme.CapeOutlineColor.Clr.A > 0))
            {
                if (Player.ColorScheme.CapeColor.Equals(ColorSchemeManager.None)
                    || Player.ColorScheme.CapeColor.Clr.A == 0)
                {
                    ItemIndex[3] = HoldCapeIndex;
                    Player.ColorScheme.CapeColor = (ClrTextFx)ItemList[3][ItemIndex[3]].obj;
                }
                if (Player.ColorScheme.CapeOutlineColor.Equals(ColorSchemeManager.None)
                    || Player.ColorScheme.CapeOutlineColor.Clr.A == 0)
                {
                    ItemIndex[4] = HoldCapeOutlineIndex;
                    Player.ColorScheme.CapeOutlineColor = (ClrTextFx)ItemList[4][ItemIndex[4]].obj;
                }
            }

            // If the outline color is null, set the cape color to null and vis a versa
            if (Player.ColorScheme.CapeOutlineColor.Equals(ColorSchemeManager.None)
                && Player.ColorScheme.CapeColor.Clr.A > 0)
            {
                HoldCapeIndex = ItemIndex[3];
                ItemIndex[3] = 0;
                Player.ColorScheme.CapeColor = (ClrTextFx)ItemList[3][ItemIndex[3]].obj;
            }
            if (Player.ColorScheme.CapeColor.Equals(ColorSchemeManager.None)
                && Player.ColorScheme.CapeOutlineColor.Clr.A > 0)
            {
                HoldCapeOutlineIndex = ItemIndex[4];
                ItemIndex[4] = 0;
                Player.ColorScheme.CapeOutlineColor = (ClrTextFx)ItemList[4][ItemIndex[4]].obj;
            }

            Doll.SetColorScheme(Player.ColorScheme);
        }

        bool HasCustom()
        {
            return Player.CustomColorScheme.SkinColor.Effect != null;
        }

        /// <summary>
        /// Select the next premade stickman to the right
        /// </summary>    
        public void SimpleSelect_Right()
        {
            int i = Player.ColorSchemeIndex;

            do
            {
                i++;
                if (i >= ColorSchemeManager.ColorSchemes.Count)
                {
                    if (HasCustom())
                        i = -1;
                    else
                        i = 0;
                    break;
                }
            }
            while (!AvailableColorScheme(ColorSchemeManager.ColorSchemes[i]));
            Player.ColorSchemeIndex = i;

            // Jiggle the arrow
            Arrows.MyMenu.Items[1].DoActivationAnimation();

            SetIndex(Player.ColorSchemeIndex);
        }

        bool AvailableColorScheme(ColorScheme scheme)
        {
            return CharacterSelectManager.AvailableHats.Contains(scheme.HatData) &&
                    CharacterSelectManager.AvailableBeards.Contains(scheme.BeardData) &&
                    PlayerManager.BoughtOrFree(scheme.SkinColor) &&
                    //PlayerManager.BoughtOrFree(scheme.OutlineColor) &&
                    PlayerManager.BoughtOrFree(scheme.CapeColor) &&
                    PlayerManager.BoughtOrFree(scheme.CapeOutlineColor);
        }

        /// <summary>
        /// Select the next premade stickman to the left
        /// </summary>
        public void SimpleSelect_Left()
        {
            int i = Player.ColorSchemeIndex;
            do
            {
                i--;
                if (i <= 0) break;
            }
            //while (!CharacterSelectManager.AvailableHats.Contains(ColorSchemeManager.ColorSchemes[i].HatData));
            while (!AvailableColorScheme(ColorSchemeManager.ColorSchemes[i]));
            Player.ColorSchemeIndex = i;

            //Player.ColorSchemeIndex--;

            // Jiggle the arrow
            Arrows.MyMenu.Items[0].DoActivationAnimation();

            int StartIndex = 0;
            if (HasCustom()) StartIndex = -1;
            if (Player.ColorSchemeIndex < StartIndex)
                Player.ColorSchemeIndex = ColorSchemeManager.ColorSchemes.Count - 1;

            SetIndex(Player.ColorSchemeIndex);
        }

        void Basic_Dir(Vector2 Dir)
        {
            if (Dir.X > .5f)
                SimpleSelect_Right();
            else
                SimpleSelect_Left();
        }

        void SetIndex(int i)
        {
            Player.ColorSchemeIndex = i;

            if (Player.ColorSchemeIndex == -1)
            {
                Player.ColorScheme = Player.CustomColorScheme;
                Doll.SetColorScheme(Player.ColorScheme);
            }
            else
            {
                Player.ColorSchemeIndex = Tools.Restrict(0, ColorSchemeManager.ColorSchemes.Count - 1, Player.ColorSchemeIndex);

                Player.ColorScheme = ColorSchemeManager.ColorSchemes[Player.ColorSchemeIndex];                
                Doll.SetColorScheme(Player.ColorScheme);
            }
        }

        void PressAtoJoinPhsx()
        {
            DollVel.X = 1;

            if (ButtonCheck.State(ControllerButtons.A, PlayerIndex).Pressed)
            {
#if XBOX || XBOX_SIGNIN
                if (Player.MyGamer != null)
                    SetState(SelectState.SimpleSelect);
                else
                    SetState(SelectState.SignInChoice);
#else
                SetState(SelectState.SimpleSelect);
#endif
                Customize.MyMenu.SelectSound.Play();
            }
        }

        void LeavingPhsx()
        {
            if (!Backdrop.FancyPos.Playing)
                SetState(SelectState.Done);
        }

#if XBOX || XBOX_SIGNIN
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

            // Check to see if the person signed in
            if (Player.MyGamer != null)
            {
                if (MyState == SelectState.SignInChoice)
                    SetState(SelectState.SimpleSelect);
            }
            else
                SetState(SelectState.PressAtoJoin);

            return false;
        }
#endif

        public void PhsxStep()
        {
#if PC_VERSION
            if (!Tools.TheGame.MouseInUse)
            {
                if (Math.Abs(ButtonCheck.GetMaxDir().Y) > 0.25f)
                    Simple.MyMenu.Active = true;
                else if (Math.Abs(ButtonCheck.GetMaxDir().X) > 0.25f)
                {
                    Simple.MyMenu.NoneSelected = true;
                    Simple.MyMenu.Active = false;
                }
            }
            else if (!Arrows.MyMenu.NoneSelected)
            {
                Simple.MyMenu.NoneSelected = true;
                Simple.MyMenu.Active = false;
            }
            else
                Simple.MyMenu.Active = true;
#elif XBOX || XBOX_SIGNIN
            if (GuideUpPhsxStep())
                return;
#endif

#if NOT_PC && (XBOX || XBOX_SIGNIN)
            // Check for signout
            if (MyState != SelectState.SignInChoice && MyState != SelectState.PressAtoJoin)
            {
                if (Player.StoredName.Length > 0 && Player.MyGamer == null)
                {
                    if (QuickJoin)
                        SetState(SelectState.Done);
                    else
                        SetState(SelectState.PressAtoJoin);
                }
            }
#endif
            FancyCenter.Update();

            Step++;

            if (Step > 1 && Step < 60)
            {
                if (Accelerate)
                {
                    Tools.DrawCount++;
                    Step++;
                }
                else
                {
                    if (ButtonCheck.State(ControllerButtons.A, PlayerIndex).Pressed ||
                        ButtonCheck.State(ControllerButtons.B, PlayerIndex).Pressed)
                        Accelerate = true;
                }
            }

            if (Step > DelayPhsxLength)
            {
                //Simple.PhsxStep();

                switch (MyState)
                {
                    case SelectState.PressAtoJoin: PressAtoJoinPhsx(); break;
#if PC_VERSION
#else
                    case SelectState.SignInChoice: SignInChoicePhsxStep(); break;
#endif
                    case SelectState.SimpleSelect: SimplePhsxStep(); break;
                    //case SelectState.CustomizeSelect: CustomizePhsxStep(); break;
                    case SelectState.Waiting: WaitingPhsxStep(); break;
                    case SelectState.Leaving: LeavingPhsx(); break;
                    default: break;
                }
            }

            // Left/Right
            Vector2 Dir = ButtonCheck.GetDir(PlayerIndex);
            if (NoMoveCount > 0)
                NoMoveCount--;

            float Sensitivty = ButtonCheck.ThresholdSensitivity;
            if (Math.Abs(Dir.X) > Sensitivty &&
                (Math.Abs(Dir.X - PrevDir.X) > Sensitivty || NoMoveCount == 0))
            {
                switch (MyState)
                {
                    case SelectState.SimpleSelect: Basic_Dir(Dir); break;
                    case SelectState.CustomizeSelect: if (!ShowColorSelect) Customize_Dir(Dir); break;
                }

                NoMoveCount = NoMoveDuration;
            }
            PrevDir = Dir;
            
            // Update positions
            Vector2 CurCenter = Center + Tools.CurLevel.MainCamera.Data.Position;
            FancyCenter.RelVal = CurCenter;

            Backdrop.FancyPos.Update();
            Vector2 CurDollPos = (DollPos + Backdrop.Pos + Center) / 2 + Tools.CurLevel.MainCamera.Data.Position;
            //Doll.Move(CurDollPos - Doll.Core.Data.Position);
            if (DollVel.X != 0)
                Doll.Core.Data.Velocity += .5f * (DollVel - Doll.Core.Data.Velocity);
            Doll.MyPhsx.OnGround = true;
        }

        public void Draw2()
        {
//#if NOT_PC
            if (MyBackdrop.OnScreen())
                MyBackdrop.DrawNonText2();

            if (Customize.OnScreen())
                Customize.DrawNonText2();

            if (Simple.OnScreen())
                Simple.DrawNonText2();

            if (Arrows.OnScreen())
                Arrows.DrawNonText2();
#if NOT_PC
            if (OnScreen(SignInChoiceMenu.FancyPos, 0, 100))
                SignInChoiceMenu.DrawNonText2();
            else
                SignInChoiceMenu.FancyPos.Update();
#endif
            if (Customize.CurClrSelect != null)
                Customize.CurClrSelect.ManualDraw();
//#endif
        }

        public void DrawText()
        {
//#if NOT_PC
            if (MyBackdrop.OnScreen())
                MyBackdrop.DrawText();

            BaseColorSelect clr = Customize.CurClrSelect;
            if (Customize.OnScreen() &&
                !(clr != null && clr.Active && !clr.Pos.Playing))
                Customize.DrawText();

            if (Simple.OnScreen())
                Simple.DrawText();

            if (Arrows.OnScreen())
                Arrows.DrawText();
#if NOT_PC
            if (OnScreen(SignInChoiceMenu.FancyPos, 0, 100))
                SignInChoiceMenu.DrawText(0);
#endif
            if (GamerTag != null)
            {
                Player.SetNameText(GamerTag);
                GamerTag.Draw(Tools.CurLevel.MainCamera, false);
            }

            if (OnScreen(JoinText.FancyPos))
                JoinText.Draw(Tools.CurLevel.MainCamera, false);
            else
                JoinText.FancyPos.Update();
        }

        bool OnScreen(FancyVector2 pos) { return OnScreen(pos, 0, 0); }
        bool OnScreen(FancyVector2 pos, float LowerShift, float UpperShift)
        {
            if (pos.RelVal.Y < -1250 + LowerShift || pos.RelVal.Y > 1550 + UpperShift)
                return false;
            else
                return true;
        }

        public void DrawBob()
        {
            if (OnScreen(Backdrop.FancyPos, -250, 0))
            {
                Vector2 CurDollPos = (DollPos + Backdrop.Pos + Center) / CharacterSelectManager.BobZoom + Tools.CurLevel.MainCamera.Data.Position;

                Doll.Move(CurDollPos - Doll.Core.Data.Position);
                Doll.Draw();
            }
            else
                Doll.SkipPrepare = true;
        }
        public void Draw()
        {
//#if NOT_PC
            if (MyBackdrop.OnScreen())
                MyBackdrop.DrawNonText();

            if (Customize.OnScreen())
                Customize.DrawNonText();

            if (Simple.OnScreen())
                Simple.DrawNonText();

            if (Arrows.OnScreen())
                Arrows.DrawNonText();
#if NOT_PC
            if (OnScreen(SignInChoiceMenu.FancyPos, 0, 100))
                SignInChoiceMenu.DrawNonText(0);
#endif

            if (OnScreen(Backdrop.FancyPos, -250, 0))
            {
                //Backdrop.Draw();
                Tools.QDrawer.Flush();

                Backdrop.FancyPos.Update();
                //Vector2 CurDollPos = (DollPos + Backdrop.Pos + Center) / 2 + Tools.CurLevel.MainCamera.Data.Position;
                //Doll.Move(CurDollPos - Doll.Core.Data.Position);

                //Doll.Draw();
            }
            else
                Backdrop.FancyPos.Update();
            
            GamerTag.DrawBackdrop();

            if (OnScreen(JoinText.FancyPos))
                JoinText.DrawBackdrop();
            else
                JoinText.FancyPos.Update();

            Tools.QDrawer.Flush();
        }
    }
}