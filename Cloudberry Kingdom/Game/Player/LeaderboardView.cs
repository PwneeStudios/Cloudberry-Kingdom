using System;
using System.IO;
using System.Collections.Generic;

using CoreEngine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

namespace CloudberryKingdom
{
    public class LeaderboardGUI : CkBaseMenu
    {
        public static float MasterAlpha;

        LeaderboardView TestView;

        public TitleGameData_MW Title;
        public LeaderboardGUI(TitleGameData_MW Title)
        {
            MasterAlpha = 1;

            this.Title = Title;
            Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_Blur);

            TestView = new LeaderboardView();
        }

        public override void Init()
        {
            base.Init();

            MyPile = new DrawPile();



            var BackBoxLeft = new QuadClass("Arcade_BoxLeft", 100, true);
            BackBoxLeft.Degrees = 90;
            BackBoxLeft.Alpha = 1f;
            MyPile.Add(BackBoxLeft, "BoxLeft");

            var Right = new QuadClass("Arcade_BoxLeft", 100, true);
            Right.Degrees = 90;
            Right.Alpha = 1f;
            MyPile.Add(Right, "BoxRight");

            var Header = new EzText("Top Scores", ItemFont);
            MyPile.Add(Header, "Header");

            Highlight = new HsvQuad();
            Highlight.TextureName = "WidePlaque"; Highlight.Show = false; MyPile.Add(Highlight, "Highlight");
            Highlight.Alpha = .3f;
            Highlight.Quad.MyEffect = Tools.HslEffect;
            Highlight.MyMatrix = ColorHelper.HsvTransform(1, 0, 1);
            //Highlight.Quad.SetColor(ColorHelper.GrayColor(.7f));

            TL = new QuadClass(); TL.Show = false; MyPile.Add(TL, "TL");
            Offset_GamerTag = new QuadClass(); Offset_GamerTag.Show = false; MyPile.Add(Offset_GamerTag, "Offset_GamerTag");
            Offset_Val = new QuadClass(); Offset_Val.Show = false; MyPile.Add(Offset_Val, "Offset_Val");
            ItemShift = new QuadClass(); ItemShift.Show = false; MyPile.Add(ItemShift, "Offset");

            MyMenu = new Menu();
            MyMenu.OnB = MenuReturnToCaller;

            // Buttons
            MenuItem item;

            // View Gamer
            item = new MenuItem(new EzText("View Profile", ItemFont));
            item.Name = "ViewGamer";
            item.JiggleOnGo = false;
            AddItem(item);
#if NOT_PC
            MyPile.Add(new QuadClass(ButtonTexture.Go, 90, "Button_ViewGamer"));
            item.Selectable = false;
#endif
            item.Go = Cast.ToItem(ViewGamer);

            // Switch View
            item = new MenuItem(new EzText("Friends Scores", ItemFont));
            item.Name = "SwitchView";
            item.JiggleOnGo = false;
            AddItem(item);
#if NOT_PC
            MyPile.Add(new QuadClass(ButtonTexture.Y, 90, "Button_SwitchView"));
            item.Selectable = false;
#endif
            item.Go = Cast.ToItem(SwitchView);

            // Switch Sort
            item = new MenuItem(new EzText("Sort Score", ItemFont));
            item.Name = "SwitchSort";
            item.JiggleOnGo = false;
            AddItem(item);
#if NOT_PC
            MyPile.Add(new QuadClass(ButtonTexture.X, 90, "Button_SwitchSort"));
            item.Selectable = false;
#endif
            item.Go = Cast.ToItem(SwitchSort);

            // Back
#if NOT_PC
            MyPile.Add(new QuadClass(ButtonTexture.Back, 90, "Button_Back"));
            MyPile.Add(new QuadClass("BackArrow2", "BackArrow"));
            item.Selectable = false;
#endif
            item.Go = Cast.ToItem(SwitchSort);

            MyMenu.NoneSelected = true;

            EnsureFancy();
            SetPos();
        }

        void ViewGamer()
        {
        }

        void SwitchView()
        {
        }

        void SwitchSort()
        {
        }

        protected override void SetItemProperties(MenuItem item)
        {
            StartMenu.SetItemProperties_Red(item);
            //base.SetItemProperties(item);
        }

        protected override void SetTextProperties(EzText text)
        {
            base.SetTextProperties(text);
        }

        protected override void SetSelectedTextProperties(EzText text)
        {
            base.SetSelectedTextProperties(text);
        }

        FancyVector2 zoom = new FancyVector2();
        public override void OnAdd()
        {
            base.OnAdd();

            SlideIn(0);
            zoom.MultiLerp(5, new Vector2[] { new Vector2(0.98f), new Vector2(1.02f), new Vector2(.99f), new Vector2(1.005f), new Vector2(1f) } );
        }

        void BubbleDown()
        {
            zoom.MultiLerp(5, new Vector2[] { new Vector2(1.0f), new Vector2(1.01f), new Vector2(.9f), new Vector2(.4f), new Vector2(0f) });
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            ReturnToCallerDelay = 15;

            if (Frames == 0)
            {
                base.SlideOut(Preset, Frames);
                return;
            }

            BubbleDown();
            MyGame.WaitThenDo(15, Return);

            Active = true;

            ReleaseWhenDone = false;
            ReleaseWhenDoneScaling = false;
        }

        void Return()
        {
            Release();
            //ReleaseWhenDone = false;
            //ReleaseWhenDoneScaling = false;

            //base.SlideOut(PresetPos.Left, 0);
            //Active = false;
        }

        public override void Release()
        {
            TL.Release();
            Offset_GamerTag.Release();
            Offset_Val.Release();
            ItemShift.Release();
            Highlight.Release();

            base.Release();
        }

        public static QuadClass TL, Offset_GamerTag, Offset_Val, ItemShift;
        public static HsvQuad Highlight;

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            TestView.PhsxStep(Control);
        }

        protected override void MyDraw()
        {
            if (zoom != null)
            {
                Vector2 v = zoom.Update();
                MasterAlpha = v.X * v.X;
                
                MyGame.Cam.Zoom = .001f * v;
                MyGame.Cam.SetVertexCamera();
                EzText.ZoomWithCamera_Override = true;

                Console.WriteLine("Alpha " + v.X.ToString());
            }
            else
            {
                MasterAlpha = 1f;
            }

            MyPile.Alpha = MasterAlpha;

            base.MyDraw();

            TestView.Draw(TL.Pos + Pos.AbsVal);
        }

        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("ViewGamer"); if (_item != null) { _item.SetPos = new Vector2(-838.5555f, 143.3333f); _item.MyText.Scale = 0.6067498f; _item.MySelectedText.Scale = 0.6067498f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("SwitchView"); if (_item != null) { _item.SetPos = new Vector2(-808f, -68.33099f); _item.MyText.Scale = 0.5009168f; _item.MySelectedText.Scale = 0.5009168f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("SwitchSort"); if (_item != null) { _item.SetPos = new Vector2(-808f, -246.662f); _item.MyText.Scale = 0.5009168f; _item.MySelectedText.Scale = 0.5009168f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(1672.222f, 686.1112f);

            EzText _t;
            _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-999.9999f, 991.6661f); _t.Scale = 0.7420002f; }

            QuadClass _q;
            _q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-408.3335f, 2.777821f); _q.Size = new Vector2(1094.068f, 1006.303f); }
            _q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(1266.665f, 519.4443f); _q.Size = new Vector2(418.2869f, 684.4695f); }
            _q = MyPile.FindQuad("Highlight"); if (_q != null) { _q.Pos = new Vector2(-413.8886f, 196.111f); _q.Size = new Vector2(1005.176f, 53.3328f); }
            _q = MyPile.FindQuad("TL"); if (_q != null) { _q.Pos = new Vector2(-1300.001f, 777.778f); _q.Size = new Vector2(0.9999986f, 0.9999986f); }
            _q = MyPile.FindQuad("Offset_GamerTag"); if (_q != null) { _q.Pos = new Vector2(5697.219f, -580.5558f); _q.Size = new Vector2(1f, 1f); }
            _q = MyPile.FindQuad("Offset_Val"); if (_q != null) { _q.Pos = new Vector2(13808.34f, -116.6667f); _q.Size = new Vector2(1f, 1f); }
            _q = MyPile.FindQuad("Offset"); if (_q != null) { _q.Pos = new Vector2(-852.7783f, -380.5554f); _q.Size = new Vector2(10.08327f, 10.08327f); }
            _q = MyPile.FindQuad("Button_ViewGamer"); if (_q != null) { _q.Pos = new Vector2(763.8883f, 705.5554f); _q.Size = new Vector2(90.83309f, 90.83309f); }
            _q = MyPile.FindQuad("Button_SwitchView"); if (_q != null) { _q.Pos = new Vector2(777.7781f, 513.889f); _q.Size = new Vector2(70.41661f, 70.41661f); }
            _q = MyPile.FindQuad("Button_SwitchSort"); if (_q != null) { _q.Pos = new Vector2(777.7776f, 338.8888f); _q.Size = new Vector2(72.99996f, 72.99996f); }
            _q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(1491.666f, -852.7777f); _q.Size = new Vector2(90f, 90f); }
            _q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(1277.778f, -877.7778f); _q.Size = new Vector2(100f, 86f); }

            MyPile.Pos = new Vector2(0f, 5.555542f);
        }
    }

    public class LeaderboardItem
    {
        public Gamer Player;
        public string GamerTag;
        public string Val;
        public string Rank;

        public static LeaderboardItem DefaultItem = new LeaderboardItem(null, 0, 0);

        public LeaderboardItem(Gamer Player, int Val, int Rank)
        {
            this.Player = Player;
            this.Rank = Rank.ToString();

            if (Player == null)
            {
                this.GamerTag = "Loading...";
                this.Val = "...";
            }
            else
            {
                this.GamerTag = Player.Gamertag;
                this.Val = Val.ToString();
            }
        }

        public void Draw(Vector2 Pos, bool Selected)
        {
            Vector4 color = (Selected ? Color.LimeGreen : ColorHelper.GrayColor(.9f)).ToVector4();
            color *= LeaderboardGUI.MasterAlpha;

            Vector2 GamerTag_Offset = .1f * new Vector2(LeaderboardGUI.Offset_GamerTag.Pos.X, 0);
            Vector2 Val_Offset = .1f * new Vector2(LeaderboardGUI.Offset_Val.Pos.X, 0);
            Vector2 Size = .1f * new Vector2(LeaderboardGUI.ItemShift.SizeX);

            if (Selected)
            {
                Vector4 ocolor = Color.Black.ToVector4();
                Tools.QDrawer.DrawString(Resources.Font_Grobold42.HOutlineFont, Rank, Pos, ocolor, Size);
                Tools.QDrawer.DrawString(Resources.Font_Grobold42.HOutlineFont, GamerTag, Pos + GamerTag_Offset, ocolor, Size);
                Tools.QDrawer.DrawString(Resources.Font_Grobold42.HOutlineFont, Val, Pos + Val_Offset, ocolor, Size);
            }

            Tools.QDrawer.DrawString(Resources.Font_Grobold42.HFont, Rank, Pos, color, Size);
            Tools.QDrawer.DrawString(Resources.Font_Grobold42.HFont, GamerTag, Pos + GamerTag_Offset, color, Size);
            Tools.QDrawer.DrawString(Resources.Font_Grobold42.HFont, Val, Pos + Val_Offset, color, Size);
        }
    }

    public class LeaderboardView
    {
        public int TotalEntries = 10000;
        int EntriesPerPage = 20;

        int Index = 0;
        int Start = 0;
        int End() { return CoreMath.Restrict(0, TotalEntries, Start + EntriesPerPage); }

        Dictionary<int, LeaderboardItem> Items;

        public LeaderboardView()
        {
            LeaderboardItem.DefaultItem = new LeaderboardItem(null, 0, 0);

            Items = new Dictionary<int, LeaderboardItem>();
        }

        void IncrIndex(int change)
        {
            Index = CoreMath.Restrict(0, TotalEntries - 1, Index + change);

            UpdateBounds();
        }

        void UpdateBounds()
        {
            if (Index >= End())
                Start = Index - EntriesPerPage + 1;
            if (Index < Start)
                Start = Index;
        }

        int DelayCount, MotionCount;
        const int SelectDelay = 11;
        public void PhsxStep(int Control)
        {
            if (DelayCount > 0)
                DelayCount--;

            Vector2 Dir = Vector2.Zero;
            if (Control < 0)
            {
                Dir = ButtonCheck.GetMaxDir(Control == -1);
            }
            else
                Dir = ButtonCheck.GetDir(Control);

            if (Dir.Length() < .2f)
                DelayCount = 0;

            if (Math.Abs(Dir.Y) > ButtonCheck.ThresholdSensitivity)
            {
                MotionCount++;
                if (DelayCount <= 0)
                {
                    int Incr = 1;
                    if (MotionCount > SelectDelay * 5) Incr = 2 + (MotionCount - SelectDelay * 5) / SelectDelay;

                    if (Dir.Y > 0) IncrIndex(-Incr);
                    else IncrIndex(Incr);

                    DelayCount = SelectDelay;
                    if (MotionCount > SelectDelay * 1) DelayCount -= 8;
                    if (MotionCount > SelectDelay * 3) DelayCount -= 4;
                    if (MotionCount > SelectDelay * 4) DelayCount -= 4;
                    if (MotionCount > SelectDelay * 5) DelayCount -= 4;
                    if (MotionCount > SelectDelay * 6) DelayCount -= 4;
                }
            }
            else
                MotionCount = 0;
        }

        public void Draw(Vector2 Pos)
        {
            //int Start = Index;
            //int End = Math.Min(TotalEntries - 1, Start + EntriesPerPage);

            Vector2 CurPos = Pos;
            float Shift = .1f * LeaderboardGUI.ItemShift.X;

            for (int i = Start; i < End(); i++)
            {
                bool Selected = i == Index;

                if (Selected)
                {
                    LeaderboardGUI.Highlight.PosY = CurPos.Y - 70;
                    LeaderboardGUI.Highlight.Show = true;
                    LeaderboardGUI.Highlight.Draw();
                    LeaderboardGUI.Highlight.Show = false;
                }

                if (Items.ContainsKey(i))
                {
                    Items[i].Draw(CurPos, Selected);
                }
                else
                {
                    LeaderboardItem Default = LeaderboardItem.DefaultItem;
                    Default.Rank = i.ToString();

                    Default.Draw(CurPos, Selected);
                }

                CurPos.Y += Shift;
            }
        }
    }
}