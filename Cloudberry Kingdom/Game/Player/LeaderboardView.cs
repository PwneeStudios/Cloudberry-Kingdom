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
        public enum LeaderboardType { FriendsScores, TopScores, MyScores, Length };
        public enum LeaderboardSortType { Score, Level, Length };
        public enum Message { None, Loading, NotRanked, NotRankedFriends, Length };

        LeaderboardType CurrentType;
        LeaderboardSortType CurrentSort;
        Message CurrentMessage;

        public static string LeaderboardType_ToString(LeaderboardType type)
        {
            switch (type)
            {
                case LeaderboardType.FriendsScores: return Localization.WordString(Localization.Words.FriendsScores);
                case LeaderboardType.MyScores: return Localization.WordString(Localization.Words.MyScores);
                case LeaderboardType.TopScores: return Localization.WordString(Localization.Words.TopScores);
                default: return "";
            }
        }

        public static string LeaderboardSortType_ToString(LeaderboardSortType type)
        {
            switch (type)
            {
                case LeaderboardSortType.Level: return Localization.WordString(Localization.Words.SortByLevel);
                case LeaderboardSortType.Score: return Localization.WordString(Localization.Words.SortByScore);
                default: return "";
            }
        }

        LeaderboardView CurrentView;



        int LeaderboardInex;
        Challenge CurrentChallenge;
        BobPhsx Hero;

        int DelayCount_LeftRight, MotionCount_LeftRight;
        const int SelectDelay = 18;

        public TitleGameData_MW Title;
        public LeaderboardGUI(TitleGameData_MW Title, int StartIndex)
        {
            EnableBounce();

            SetIndex(0);

            CurrentType = LeaderboardType.FriendsScores;
            CurrentSort = LeaderboardSortType.Level;
            CurrentMessage = Message.None;

            DelayCount_LeftRight = MotionCount_LeftRight = 0;

            this.Title = Title;
            if (Title != null)
                Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_Blur);

            UpdateView();
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

            var GameTitle = new EzText("Escalation, Classic", ItemFont);
            MyPile.Add(GameTitle, "GameTitle");

            Highlight = new HsvQuad();
            Highlight.TextureName = "WidePlaque";
            Highlight.TextureName = "Arcade_BoxLeft";
            Highlight.Show = false;
            MyPile.Add(Highlight, "Highlight");
            Highlight.Alpha = .3f;
            Highlight.Quad.MyEffect = Tools.HslEffect;
            Highlight.MyMatrix = ColorHelper.HsvTransform(1, 0, 1);
            //Highlight.Quad.SetColor(ColorHelper.GrayColor(.7f));

            TL = new QuadClass(); TL.Show = false; MyPile.Add(TL, "TL");
            Offset_GamerTag = new QuadClass(); Offset_GamerTag.Show = false; MyPile.Add(Offset_GamerTag, "Offset_GamerTag");
            Offset_Val = new QuadClass(); Offset_Val.Show = false; MyPile.Add(Offset_Val, "Offset_Val");
            ItemShift = new QuadClass(); ItemShift.Show = false; MyPile.Add(ItemShift, "Offset");

            // Messages
            CurrentMessage = Message.None;
            NotRankedFriends = new EzText(Localization.Words.NotRankedFriends, ItemFont, 2000, true, true);
            MyPile.Add(NotRankedFriends, "NotRankedFriends");

            NotRanked = new EzText(Localization.Words.NotRanked, ItemFont, 2000, true, true);
            MyPile.Add(NotRanked, "NotRanked");

            LoadingStr0 = Localization.WordString(Localization.Words.Loading);
            LoadingStr1 = Localization.WordString(Localization.Words.Loading) + ".";
            LoadingStr2 = Localization.WordString(Localization.Words.Loading) + "..";
            LoadingStr3 = Localization.WordString(Localization.Words.Loading) + "...";
            LoadingCount = 0;

            LoadingText = new EzText(LoadingStr1, ItemFont, 1000, true, true);
            MyPile.Add(LoadingText, "Loading");


            MyMenu = new Menu();
            MyMenu.OnB = MenuReturnToCaller;

            // Buttons
            MenuItem item;

            // View Gamer
            item = new MenuItem(new EzText(Localization.Words.ViewGamerCard, ItemFont));
            item.Name = "ViewGamer";
            item.JiggleOnGo = false;
            AddItem(item);
#if NOT_PC
            MyPile.Add(new QuadClass(ButtonTexture.Go, 90, "Button_ViewGamer"));
            item.Selectable = false;
#endif
            item.Go = Cast.ToItem(ViewGamer);
            MyMenu.OnA = Cast.ToMenu(ViewGamer);

            // Switch View
            item = new MenuItem(new EzText(Localization.Words.FriendsScores, ItemFont));
            item.Name = "SwitchView";
            item.JiggleOnGo = false;
            AddItem(item);
#if NOT_PC
            MyPile.Add(new QuadClass(ButtonTexture.Y, 90, "Button_SwitchView"));
            item.Selectable = false;
#endif
            item.Go = Cast.ToItem(SwitchView);
            MyMenu.OnY = SwitchView;

            // Switch Sort
            item = new MenuItem(new EzText(Localization.Words.SortByScore, ItemFont));
            item.Name = "SwitchSort";
            item.JiggleOnGo = false;
            AddItem(item);
#if NOT_PC
            MyPile.Add(new QuadClass(ButtonTexture.X, 90, "Button_SwitchSort"));
            item.Selectable = false;
#endif
            item.Go = Cast.ToItem(SwitchSort);
            MyMenu.OnX = Cast.ToMenu(SwitchSort);

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

            UpdateMessages();
        }

        EzText LoadingText, NotRanked, NotRankedFriends;
        int LoadingCount;
        string LoadingStr0, LoadingStr1, LoadingStr2, LoadingStr3;
        void UpdateLoadingText()
        {
            if (CurrentMessage == Message.Loading)
            {

                LoadingText.Show = true;

                LoadingCount++;

                int Delay = 12;
                int Total = 70;
                if      (LoadingCount % Total == 0)         LoadingText.SubstituteText(LoadingStr0);
                else if (LoadingCount % Total == 1 * Delay + 4) LoadingText.SubstituteText(LoadingStr1);
                else if (LoadingCount % Total == 2 * Delay + 4) LoadingText.SubstituteText(LoadingStr2);
                else if (LoadingCount % Total == 3 * Delay + 4) LoadingText.SubstituteText(LoadingStr3);

                LoadingText.Scale = 0.351667f * CoreMath.Periodic(1f, 1.1f, 2*Total, LoadingCount + 2);
            }
            else
            {
                LoadingText.Show = false;

                LoadingCount = 0;
            }
        }

        void UpdateMessages()
        {
            if (CurrentView == null)
            {
                CurrentMessage = Message.Loading;
            }
            else
            {
                if (CurrentView.Loading)
                    CurrentMessage = Message.Loading;
                else if (CurrentView.TotalEntries == 0)
                {
                    if (CurrentType == LeaderboardType.FriendsScores)
                        CurrentMessage = Message.NotRankedFriends;
                    else
                        CurrentMessage = Message.NotRanked;
                }
            }

            UpdateLoadingText();

            NotRanked.Show        = CurrentMessage == Message.NotRanked;
            NotRankedFriends.Show = CurrentMessage == Message.NotRankedFriends;
        }

        void ViewGamer()
        {
        }

        LeaderboardType Incr(LeaderboardType type)
        {
            return (LeaderboardType)((((int)type + 1) + (int)LeaderboardType.Length) % (int)LeaderboardType.Length);
        }

        LeaderboardSortType Incr(LeaderboardSortType type)
        {
            return (LeaderboardSortType)((((int)type + 1) + (int)LeaderboardSortType.Length) % (int)LeaderboardSortType.Length);
        }

        void SwitchView()
        {
            CurrentType = Incr(CurrentType);
            UpdateView();
        }

        void SwitchSort()
        {
            CurrentSort = Incr(CurrentSort);
            UpdateView();
        }

        void UpdateView()
        {
            MyMenu.FindItemByName("SwitchView").MyText.SubstituteText(LeaderboardType_ToString(Incr(CurrentType)));
            MyPile.FindEzText("Header").SubstituteText(LeaderboardType_ToString(CurrentType));
            MyMenu.FindItemByName("SwitchSort").MyText.SubstituteText(LeaderboardSortType_ToString(Incr(CurrentSort)));
        }

        public void SetIndex(int index)
        {
            LeaderboardInex = index;
            CurrentChallenge = ArcadeMenu.LeaderboardList[index].Item1;
            Hero = ArcadeMenu.LeaderboardList[index].Item2;

            CurrentView = new LeaderboardView();

            string Name;
            if (CurrentChallenge == null)
            {
                Name = Localization.WordString(Localization.Words.TotalArcade);
            }
            else
            {
                if (Hero == null)
                    Name = Localization.WordString(CurrentChallenge.Name);
                else
                    Name = Localization.WordString(CurrentChallenge.Name) + ", " + Localization.WordString(Hero.Name);
            }

            MyPile.FindEzText("GameTitle").SubstituteText(Name);
        }

        public void ChangeLeaderboard(int Direction)
        {
            int index = (LeaderboardInex + Direction + ArcadeMenu.LeaderboardList.Count) % ArcadeMenu.LeaderboardList.Count;
            SetIndex(index);
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

        public override void OnAdd()
        {
            base.OnAdd();
        }

        public override void Release()
        {
            LoadingText.Release();
            NotRanked.Release();
            NotRankedFriends.Release();

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

            // Get direction input
            Vector2 Dir = Vector2.Zero;
            if (Control < 0)
            {
                Dir = ButtonCheck.GetMaxDir(Control == -1);
            }
            else
                Dir = ButtonCheck.GetDir(Control);

            if (DelayCount_LeftRight > 0)
                DelayCount_LeftRight--;

            if (Dir.Length() < .2f)
                DelayCount_LeftRight = 0;

            // Left and right
            if (ButtonCheck.State(ControllerButtons.LS, Control).Pressed)
            {
                ChangeLeaderboard(-1);
                MotionCount_LeftRight = 0;
            }
            else if (ButtonCheck.State(ControllerButtons.RS, Control).Pressed)
            {
                ChangeLeaderboard(1);
                MotionCount_LeftRight = 0;
            }
            else if (Math.Abs(Dir.X) > .75f)//ButtonCheck.ThresholdSensitivity)
            {
                MotionCount_LeftRight++;
                if (DelayCount_LeftRight <= 0)
                {
                    DelayCount_LeftRight = SelectDelay - 5;
                    if (MotionCount_LeftRight > 1 * SelectDelay) DelayCount_LeftRight -= 4;
                    if (MotionCount_LeftRight > 2 * SelectDelay) DelayCount_LeftRight -= 3;

                    if (Dir.X > 0) ChangeLeaderboard(1);
                    else ChangeLeaderboard(-1);
                }
            }
            else
                MotionCount_LeftRight = 0;

            CurrentView.PhsxStep(Control);
        }

        protected override void MyDraw()
        {
            base.MyDraw();

            UpdateMessages();
            
            if (CurrentMessage == Message.None)
                CurrentView.Draw(TL.Pos + Pos.AbsVal, MasterAlpha);
        }

        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("ViewGamer"); if (_item != null) { _item.SetPos = new Vector2(-838.5555f, 143.3333f); _item.MyText.Scale = 0.6067498f; _item.MySelectedText.Scale = 0.6067498f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("SwitchView"); if (_item != null) { _item.SetPos = new Vector2(-808f, -68.33099f); _item.MyText.Scale = 0.5009168f; _item.MySelectedText.Scale = 0.5009168f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("SwitchSort"); if (_item != null) { _item.SetPos = new Vector2(-808f, -246.662f); _item.MyText.Scale = 0.5009168f; _item.MySelectedText.Scale = 0.5009168f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(1672.222f, 686.1112f);

            EzText _t;
            _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-1308.333f, 991.6661f); _t.Scale = 0.5240005f; }
            _t = MyPile.FindEzText("GameTitle"); if (_t != null) { _t.Pos = new Vector2(-1302.778f, 861.1112f); _t.Scale = 0.4570001f; }
            _t = MyPile.FindEzText("NotRankedFriends"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.4956669f; }
            _t = MyPile.FindEzText("NotRanked"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.4956669f; }
            _t = MyPile.FindEzText("Loading"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.351667f; }

            QuadClass _q;
            _q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-408.3335f, 2.777821f); _q.Size = new Vector2(1094.068f, 1006.303f); }
            _q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(1266.665f, 519.4443f); _q.Size = new Vector2(418.2869f, 684.4695f); }
            _q = MyPile.FindQuad("Highlight"); if (_q != null) { _q.Pos = new Vector2(-413.8886f, -921.1119f); _q.Size = new Vector2(1005.093f, 49.08278f); }
            _q = MyPile.FindQuad("TL"); if (_q != null) { _q.Pos = new Vector2(-1300.001f, 713.8893f); _q.Size = new Vector2(0.9999986f, 0.9999986f); }
            _q = MyPile.FindQuad("Offset_GamerTag"); if (_q != null) { _q.Pos = new Vector2(5697.219f, -580.5558f); _q.Size = new Vector2(1f, 1f); }
            _q = MyPile.FindQuad("Offset_Val"); if (_q != null) { _q.Pos = new Vector2(13808.34f, -116.6667f); _q.Size = new Vector2(1f, 1f); }
            _q = MyPile.FindQuad("Offset"); if (_q != null) { _q.Pos = new Vector2(-869.4451f, -383.3332f); _q.Size = new Vector2(10.08327f, 10.08327f); }
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
                this.GamerTag = Localization.WordString(Localization.Words.Loading) + "...";
                this.Val = "...";
            }
            else
            {
                this.GamerTag = Player.Gamertag;
                this.Val = Val.ToString();
            }
        }

        public void Draw(Vector2 Pos, bool Selected, float alpha)
        {
            Vector4 color = ColorHelper.Gray(.9f);
            Vector4 ocolor = Color.Black.ToVector4();

            if (Selected)
            {
                //ocolor = new Color(191, 191, 191).ToVector4();
                //color = new Color(175, 8, 64).ToVector4();

                color = Color.LimeGreen.ToVector4();
                ocolor = new Color(0, 0, 0).ToVector4();
            }
            
            color *= alpha;

            Vector2 GamerTag_Offset = .1f * new Vector2(LeaderboardGUI.Offset_GamerTag.Pos.X, 0);
            Vector2 Val_Offset = .1f * new Vector2(LeaderboardGUI.Offset_Val.Pos.X, 0);
            Vector2 Size = .1f * new Vector2(LeaderboardGUI.ItemShift.SizeX);

            if (Selected)
            {
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
        const int EntriesPerPage = 19;
        public int TotalEntries;

        public bool Loading;

        int Index;
        int Start;
        int End() { return CoreMath.Restrict(0, TotalEntries, Start + EntriesPerPage); }

        Dictionary<int, LeaderboardItem> Items;

        public LeaderboardView()
        {
            TotalEntries = 0;// 1000000;
            Index = 0;
            Start = 0;

            Loading = true;

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

        int DelayCount_UpDown, MotionCount_UpDown;
        const int SelectDelay = 11;
        public void PhsxStep(int Control)
        {
            // Get direction input
            Vector2 Dir = Vector2.Zero;
            if (Control < 0)
            {
                Dir = ButtonCheck.GetMaxDir(Control == -1);
            }
            else
                Dir = ButtonCheck.GetDir(Control);

            // Up and down
            if (DelayCount_UpDown > 0)
                DelayCount_UpDown--;

            if (Dir.Length() < .2f && !ButtonCheck.State(ControllerButtons.LT, Control).Down && !ButtonCheck.State(ControllerButtons.RT, Control).Down)
                DelayCount_UpDown = 0;

            int IncrMultiplier = 1;
            if (MotionCount_UpDown > SelectDelay * 5) IncrMultiplier = 2 + (MotionCount_UpDown - SelectDelay * 5) / SelectDelay;

            if (ButtonCheck.State(ControllerButtons.LT, Control).Down || ButtonCheck.State(ControllerButtons.RT, Control).Down)
            {
                int Incr = EntriesPerPage;

                MotionCount_UpDown++;
                if (DelayCount_UpDown <= 0)
                {
                    if (ButtonCheck.State(ControllerButtons.LT, Control).Down)
                        IncrIndex(-Incr * IncrMultiplier);
                    else
                        IncrIndex(Incr * IncrMultiplier);

                    DelayCount_UpDown = SelectDelay;

                    if (MotionCount_UpDown > SelectDelay * 1) DelayCount_UpDown -= 8;
                    if (MotionCount_UpDown > SelectDelay * 3) DelayCount_UpDown -= 4;
                }
            }
            else if (Math.Abs(Dir.Y) > ButtonCheck.ThresholdSensitivity)
            {
                MotionCount_UpDown++;
                if (DelayCount_UpDown <= 0)
                {
                    int Incr = IncrMultiplier;

                    if (Dir.Y > 0) IncrIndex(-Incr);
                    else IncrIndex(Incr);

                    DelayCount_UpDown = SelectDelay;
                    if (MotionCount_UpDown > SelectDelay * 1) DelayCount_UpDown -= 8;
                    if (MotionCount_UpDown > SelectDelay * 3) DelayCount_UpDown -= 4;
                    if (MotionCount_UpDown > SelectDelay * 4) DelayCount_UpDown -= 4;
                    if (MotionCount_UpDown > SelectDelay * 5) DelayCount_UpDown -= 4;
                    if (MotionCount_UpDown > SelectDelay * 6) DelayCount_UpDown -= 4;
                }
            }
            else
                MotionCount_UpDown = 0;
        }

        public void Draw(Vector2 Pos, float alpha)
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
                    Items[i].Draw(CurPos, Selected, alpha);
                }
                else
                {
                    LeaderboardItem Default = LeaderboardItem.DefaultItem;
                    Default.Rank = i.ToString();

                    Default.Draw(CurPos, Selected, alpha);
                }

                CurPos.Y += Shift;
            }
        }
    }
}