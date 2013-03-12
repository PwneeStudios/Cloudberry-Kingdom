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
        public enum Message { None, Loading, NoOne, NotRanked, NotRankedFriends, Length };

        LeaderboardType CurrentType = LeaderboardType.TopScores;
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
        public LeaderboardGUI(TitleGameData_MW Title, SignedInGamer LeaderboardGamer, int Control)
        {
            ToMake_Id = -1;
            DelayToMake = 0;

            Leaderboard.LeaderboardGamer = LeaderboardGamer;
            if (Leaderboard.LeaderboardGamer != null)
            {
                Leaderboard.LeaderboardFriends = new List<Gamer>();
                Leaderboard.LeaderboardFriends.Add(LeaderboardGamer);
                Leaderboard.LeaderboardFriends.AddRange(Leaderboard.LeaderboardGamer.GetFriends());
            }

            this.Control = Control;

            EnableBounce();

            CurrentType = LeaderboardType.TopScores;
            CurrentSort = LeaderboardSortType.Score;
            CurrentMessage = Message.None;

			SetIndex(Challenge.LeaderboardIndex);
            //SetIndex(0);

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

            var Header = new EzText("__Dummy__", ItemFont);
            MyPile.Add(Header, "Header");

            var GameTitle = new EzText("__Dummy__", ItemFont);
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
            NotRankedFriends = new EzText(Localization.Words.NotRankedFriends, ItemFont, 1400.0f/*2000*/, true, true, .785f);
            MyPile.Add(NotRankedFriends, "NotRankedFriends");

            NotRanked = new EzText(Localization.Words.NotRanked, ItemFont, 1400.0f/*2000*/, true, true, .785f);
            MyPile.Add(NotRanked, "NotRanked");

            LoadingStr0 = Localization.WordString(Localization.Words.Loading);
            LoadingStr1 = Localization.WordString(Localization.Words.Loading) + ".";
            LoadingStr2 = Localization.WordString(Localization.Words.Loading) + "..";
            LoadingStr3 = Localization.WordString(Localization.Words.Loading) + "...";
            LoadingCount = 0;

            LoadingText = new EzText(LoadingStr1, ItemFont, 1000, true, true);
            MyPile.Add(LoadingText, "Loading");


			MyMenu = new Menu(false);
			//MyMenu = new Menu();
            MyMenu.OnB = MenuReturnToCaller;

            // Buttons
            MenuItem item;

            // View Gamer
#if PS3
            item = new MenuItem(new EzText(Localization.Words.Profile, ItemFont));
#else
			item = new MenuItem(new EzText(Localization.Words.ViewGamerCard, ItemFont));
#endif
            item.Name = "ViewGamer";
            item.JiggleOnGo = false;
            AddItem(item);
if (ButtonCheck.ControllerInUse)
{
            MyPile.Add(new QuadClass(ButtonTexture.Go, 90, "Button_ViewGamer"));
            item.Selectable = false;
}
else
{
            item.Go = Cast.ToItem(ViewGamer);
}
            MyMenu.OnA = Cast.ToMenu(ViewGamer);

            // Switch View
            item = new MenuItem(new EzText("__Dummy__", ItemFont));
            item.Name = "SwitchView";
            item.JiggleOnGo = false;
            AddItem(item);
if (ButtonCheck.ControllerInUse)
{
            MyPile.Add(new QuadClass(ButtonTexture.X, 90, "Button_SwitchView"));
            item.Selectable = false;
}
            item.Go = Cast.ToItem(SwitchView);
			//MyMenu.OnY = SwitchView;
			MyMenu.OnX = Cast.ToMenu(SwitchView);

            // Switch Sort
			bool ShowSortOption = false;
            item = new MenuItem(new EzText("__Dummy__", ItemFont));
            item.Name = "SwitchSort";
            item.JiggleOnGo = false;
            AddItem(item);
if (ButtonCheck.ControllerInUse)
{
			if (ShowSortOption)
			{
				MyPile.Add(new QuadClass(ButtonTexture.X, 90, "Button_SwitchSort"));
			}
            item.Selectable = false;
}
            item.Go = Cast.ToItem(ViewGamer);
            //MyMenu.OnX = Cast.ToMenu(SwitchSort);
			if (!ShowSortOption)
			{
				item.Show = false; item.Selectable = false;
			}

			// Left/Right
			MyPile.Add(new QuadClass(ButtonTexture.LeftRight, 90, "Button_LeftRight"));
			
			string prevback = "";
			if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
			{
				prevback = Localization.WordString(Localization.Words.Previous) + " /\n" + Localization.WordString(Localization.Words.Next);
			}
			else
			{
				prevback = Localization.WordString(Localization.Words.Previous) + " / " + Localization.WordString(Localization.Words.Next);
			}
			EzText text = new EzText(prevback, ItemFont, 1000.0f, false, false, .75f);
			StartMenu.SetTextUnselected_Red(text);
			MyPile.Add(text, "LeftRight");


            // Back
if (ButtonCheck.ControllerInUse)
{
            MyPile.Add(new QuadClass(ButtonTexture.Back, 90, "Button_Back"));
            MyPile.Add(new QuadClass("BackArrow2", "BackArrow"));
            item.Selectable = false;
}
            item.Go = Cast.ToItem(SwitchSort);

            MyMenu.NoneSelected = true;

            EnsureFancy();
            SetPos();

            UpdateMessages();

if (ButtonCheck.ControllerInUse)
{
			MyMenu.NoneSelected = true;
}

            UpdateView();
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
                if      (LoadingCount % Total == 0)             LoadingText.SubstituteText(LoadingStr0);
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
                    else if (CurrentType == LeaderboardType.TopScores)
                        CurrentMessage = Message.NoOne;
                    else
                        CurrentMessage = Message.NotRanked;
                }
                else
                {
                    if (CurrentType == LeaderboardType.MyScores && CurrentView.MyLeaderboard.gamer_rank < 0)
                        CurrentMessage = Message.NotRanked;
                    else
                        CurrentMessage = Message.None;
                }
            }

            UpdateLoadingText();

            NotRanked.Show        = CurrentMessage == Message.NotRanked;
            NotRankedFriends.Show = CurrentMessage == Message.NotRankedFriends;
        }

        void ViewGamer()
        {
            CurrentView.ViewGamer();
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
            if (CurrentView != null && CurrentView.Loading) return;

            CurrentType = Incr(CurrentType);
            UpdateView();

            CurrentView.SetType(CurrentType);
        }

        void SwitchSort()
        {
            CurrentSort = Incr(CurrentSort);
            UpdateView();
        }

        void UpdateView()
        {
			MyMenu.FindItemByName("SwitchView").MyText.SubstituteText(LeaderboardType_ToString(Incr(CurrentType)));
			MyMenu.FindItemByName("SwitchView").MySelectedText.SubstituteText(LeaderboardType_ToString(Incr(CurrentType)));
			MyPile.FindEzText("Header").SubstituteText(LeaderboardType_ToString(CurrentType));
			MyMenu.FindItemByName("SwitchSort").MyText.SubstituteText(LeaderboardSortType_ToString(Incr(CurrentSort)));
			MyMenu.FindItemByName("SwitchSort").MySelectedText.SubstituteText(LeaderboardSortType_ToString(Incr(CurrentSort)));
        }

        public void SetIndex(int index)
        {
            LeaderboardInex = index;
            CurrentChallenge = ArcadeMenu.LeaderboardList[index].Item1;
            Hero = ArcadeMenu.LeaderboardList[index].Item2;

            string Name;
            int Id;
            if (CurrentChallenge == null)
            {
                Name = Localization.WordString(Localization.Words.PlayerLevel) + " (" +
                        Localization.WordString(Localization.Words.TheArcade) + " + " + Localization.WordString(Localization.Words.StoryMode) + ")";
                Id = 9999;
            }
            else
            {
                if (Hero == null)
                {
                    Name = Localization.WordString(CurrentChallenge.Name);
                    Id = CurrentChallenge.GameId_Level;
                }
                else
                {
                    Name = Localization.WordString(CurrentChallenge.Name) + ", " + Localization.WordString(Hero.Name);
                    Id = CurrentChallenge.CalcGameId_Level(Hero);
                }
            }

            MyPile.FindEzText("GameTitle").SubstituteText(Name);

            if (CurrentView == null)
                CurrentView = new LeaderboardView(Id, CurrentType);
            else
                ToMake_Id = Id;

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

        int ToMake_Id = -1;
        int DelayToMake = 0;

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active || BubblingOut) return;

            if (!CloudberryKingdomGame.OnlineFunctionalityAvailable())
            {
                ReturnToCaller();
                CloudberryKingdomGame.ShowError_MustBeSignedInToLive(Localization.Words.Err_MustBeSignedInToLive);
                return;
            }

            if (DelayToMake > 0)
            {
                DelayToMake--;
            }
            else if (ToMake_Id >= 0)
            {
                CurrentView = new LeaderboardView(ToMake_Id, CurrentType);
                ToMake_Id = -1;
                DelayToMake = 25;
            }

            // Get direction input
            Vector2 Dir = Vector2.Zero;
            if (Control < 0)
            {
                //Dir = ButtonCheck.GetMaxDir(Control == -1);
                Dir = ButtonCheck.GetMaxDir(false);
            }
            else
                Dir = ButtonCheck.GetDir(Control);

            if (DelayCount_LeftRight > 0)
                DelayCount_LeftRight--;

            if (Dir.Length() < .2f)
                DelayCount_LeftRight = 0;

            if (Math.Abs(Dir.X) < .875f)
                Dir.X = 0;

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
            {
                // Zoom camera as needed for bounce effect
                Vector2 v = zoom.Update();
                MasterAlpha = v.X * v.X;

                MyGame.Cam.Zoom = .001f * v;
                MyGame.Cam.SetVertexCamera();

                // Draw the scores
                CurrentView.Draw(TL.Pos + Pos.AbsVal, MasterAlpha);

                Tools.QDrawer.Flush();

                // Revert camera
                MyGame.Cam.Zoom = new Vector2(.001f);
                MyGame.Cam.SetVertexCamera();
                EzText.ZoomWithCamera_Override = false;
            }
        }

        void SetPos()
        {
if (Localization.CurrentLanguage.MyLanguage == Localization.Language.German)
{
	MenuItem _item;
	_item = MyMenu.FindItemByName("ViewGamer"); if (_item != null) { _item.SetPos = new Vector2(-885.7779f, 168.3334f); _item.MyText.Scale = 0.3767501f; _item.MySelectedText.Scale = 0.3767501f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("SwitchView"); if (_item != null) { _item.SetPos = new Vector2(-894.1109f, 17.78012f); _item.MyText.Scale = 0.3958333f; _item.MySelectedText.Scale = 0.3958333f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("SwitchSort"); if (_item != null) { _item.SetPos = new Vector2(-902.4446f, -132.7731f); _item.MyText.Scale = 0.3892502f; _item.MySelectedText.Scale = 0.3892502f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(1672.222f, 686.1112f);

	EzText _t;
	_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-1308.333f, 991.6661f); _t.Scale = 0.5240005f; }
    _t = MyPile.FindEzText("GameTitle"); if (_t != null) { _t.Pos = new Vector2(-1302.778f, 861.1112f); _t.Scale = 0.42f/*0.4570001f*/; }
	_t = MyPile.FindEzText("NotRankedFriends"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.4956669f; }
	_t = MyPile.FindEzText("NotRanked"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.4956669f; }
	_t = MyPile.FindEzText("Loading"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.3519495f; }
	_t = MyPile.FindEzText("LeftRight"); if (_t != null) { _t.Pos = new Vector2(777.7778f, 549.9998f); _t.Scale = 0.3982503f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-408.3335f, 2.777821f); _q.Size = new Vector2(1094.068f, 1006.303f); }
	_q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(1266.665f, 519.4443f); _q.Size = new Vector2(418.2869f, 684.4695f); }
	_q = MyPile.FindQuad("Highlight"); if (_q != null) { _q.Pos = new Vector2(-413.8886f, 643.8893f); _q.Size = new Vector2(1005.093f, 49.08278f); }
	_q = MyPile.FindQuad("TL"); if (_q != null) { _q.Pos = new Vector2(-1300.001f, 713.8893f); _q.Size = new Vector2(0.9999986f, 0.9999986f); }
	_q = MyPile.FindQuad("Offset_GamerTag"); if (_q != null) { _q.Pos = new Vector2(4820f, -363.889f); _q.Size = new Vector2(1f, 1f); }
	_q = MyPile.FindQuad("Offset_Val"); if (_q != null) { _q.Pos = new Vector2(13808.34f, -116.6667f); _q.Size = new Vector2(1f, 1f); }
	_q = MyPile.FindQuad("Offset"); if (_q != null) { _q.Pos = new Vector2(-869.4451f, -383.3332f); _q.Size = new Vector2(10.08327f, 10.08327f); }
	_q = MyPile.FindQuad("Button_ViewGamer"); if (_q != null) { _q.Pos = new Vector2(713.8889f, 777.7776f); _q.Size = new Vector2(67.25001f, 67.25001f); }
	_q = MyPile.FindQuad("Button_SwitchView"); if (_q != null) { _q.Pos = new Vector2(713.8892f, 622.2218f); _q.Size = new Vector2(65.16659f, 65.16659f); }
	_q = MyPile.FindQuad("Button_LeftRight"); if (_q != null) { _q.Pos = new Vector2(713.8888f, 480.5554f); _q.Size = new Vector2(73.74999f, 73.74999f); }
	_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(1536.111f, 227.7776f); _q.Size = new Vector2(67.99999f, 67.99999f); }
	_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(1369.444f, 216.6666f); _q.Size = new Vector2(77.71317f, 66.83332f); }

	MyPile.Pos = new Vector2(0f, 5.555542f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Italian)
{
	MenuItem _item;
	_item = MyMenu.FindItemByName("ViewGamer"); if (_item != null) { _item.SetPos = new Vector2(-888.5557f, 176.6667f); _item.MyText.Scale = 0.3826668f; _item.MySelectedText.Scale = 0.3826668f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("SwitchView"); if (_item != null) { _item.SetPos = new Vector2(-877.4443f, 23.33566f); _item.MyText.Scale = 0.4059168f; _item.MySelectedText.Scale = 0.4059168f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("SwitchSort"); if (_item != null) { _item.SetPos = new Vector2(-880.2222f, -116.1065f); _item.MyText.Scale = 0.4625835f; _item.MySelectedText.Scale = 0.4625835f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(1672.222f, 686.1112f);

	EzText _t;
	_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-1308.333f, 991.6661f); _t.Scale = 0.5240005f; }
	_t = MyPile.FindEzText("GameTitle"); if (_t != null) { _t.Pos = new Vector2(-1297.222f, 855.5557f); _t.Scale = 0.3989167f; }
	_t = MyPile.FindEzText("NotRankedFriends"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.4956669f; }
	_t = MyPile.FindEzText("NotRanked"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.4956669f; }
	_t = MyPile.FindEzText("Loading"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.3739281f; }
	_t = MyPile.FindEzText("LeftRight"); if (_t != null) { _t.Pos = new Vector2(797.2221f, 538.8887f); _t.Scale = 0.3720834f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-408.3335f, 2.777821f); _q.Size = new Vector2(1094.068f, 1006.303f); }
	_q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(1266.665f, 519.4443f); _q.Size = new Vector2(418.2869f, 684.4695f); }
	_q = MyPile.FindQuad("Highlight"); if (_q != null) { _q.Pos = new Vector2(-413.8886f, 643.8893f); _q.Size = new Vector2(1005.093f, 49.08278f); }
	_q = MyPile.FindQuad("TL"); if (_q != null) { _q.Pos = new Vector2(-1300.001f, 713.8893f); _q.Size = new Vector2(0.9999986f, 0.9999986f); }
	_q = MyPile.FindQuad("Offset_GamerTag"); if (_q != null) { _q.Pos = new Vector2(4820f, -363.889f); _q.Size = new Vector2(1f, 1f); }
	_q = MyPile.FindQuad("Offset_Val"); if (_q != null) { _q.Pos = new Vector2(13808.34f, -116.6667f); _q.Size = new Vector2(1f, 1f); }
	_q = MyPile.FindQuad("Offset"); if (_q != null) { _q.Pos = new Vector2(-869.4451f, -383.3332f); _q.Size = new Vector2(10.08327f, 10.08327f); }
	_q = MyPile.FindQuad("Button_ViewGamer"); if (_q != null) { _q.Pos = new Vector2(705.5554f, 783.3331f); _q.Size = new Vector2(67.25001f, 67.25001f); }
	_q = MyPile.FindQuad("Button_SwitchView"); if (_q != null) { _q.Pos = new Vector2(705.5557f, 624.9996f); _q.Size = new Vector2(65.16659f, 65.16659f); }
	_q = MyPile.FindQuad("Button_LeftRight"); if (_q != null) { _q.Pos = new Vector2(713.8888f, 480.5554f); _q.Size = new Vector2(73.74999f, 73.74999f); }
	_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(1536.111f, 227.7776f); _q.Size = new Vector2(67.99999f, 67.99999f); }
	_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(1369.444f, 216.6666f); _q.Size = new Vector2(77.71317f, 66.83332f); }

	MyPile.Pos = new Vector2(0f, 5.555542f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
{
	MenuItem _item;
    _item = MyMenu.FindItemByName("ViewGamer"); if (_item != null) { _item.SetPos = new Vector2(-908f, 182.2224f); _item.MyText.Scale = 0.414917f * .97f; _item.MySelectedText.Scale = 0.414917f * .97f; _item.SelectIconOffset = new Vector2(0f, 0f); }
    _item = MyMenu.FindItemByName("SwitchView"); if (_item != null) { _item.SetPos = new Vector2(-899.6663f, 31.66904f); _item.MyText.Scale = 0.4285002f * .97f; _item.MySelectedText.Scale = 0.4285002f * .97f; _item.SelectIconOffset = new Vector2(0f, 0f); }
    _item = MyMenu.FindItemByName("SwitchSort"); if (_item != null) { _item.SetPos = new Vector2(-874.6667f, -116.1065f); _item.MyText.Scale = 0.5009168f * .97f; _item.MySelectedText.Scale = 0.5009168f * .97f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(1672.222f, 686.1112f);

	EzText _t;
	_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-1308.333f, 991.6661f); _t.Scale = 0.5240005f; }
	_t = MyPile.FindEzText("GameTitle"); if (_t != null) { _t.Pos = new Vector2(-1302.778f, 861.1112f); _t.Scale = 0.4570001f; }
	_t = MyPile.FindEzText("NotRankedFriends"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.4956669f; }
	_t = MyPile.FindEzText("NotRanked"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.4956669f; }
	_t = MyPile.FindEzText("Loading"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.3795856f; }
    _t = MyPile.FindEzText("LeftRight"); if (_t != null) { _t.Pos = new Vector2(780.5557f, 555.5554f); _t.Scale = 0.4296669f * .97f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-408.3335f, 2.777821f); _q.Size = new Vector2(1094.068f, 1006.303f); }
	_q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(1266.665f, 519.4443f); _q.Size = new Vector2(418.2869f, 684.4695f); }
	_q = MyPile.FindQuad("Highlight"); if (_q != null) { _q.Pos = new Vector2(-413.8886f, 643.8893f); _q.Size = new Vector2(1005.093f, 49.08278f); }
	_q = MyPile.FindQuad("TL"); if (_q != null) { _q.Pos = new Vector2(-1300.001f, 713.8893f); _q.Size = new Vector2(0.9999986f, 0.9999986f); }
	_q = MyPile.FindQuad("Offset_GamerTag"); if (_q != null) { _q.Pos = new Vector2(4820f, -363.889f); _q.Size = new Vector2(1f, 1f); }
	_q = MyPile.FindQuad("Offset_Val"); if (_q != null) { _q.Pos = new Vector2(13808.34f, -116.6667f); _q.Size = new Vector2(1f, 1f); }
	_q = MyPile.FindQuad("Offset"); if (_q != null) { _q.Pos = new Vector2(-869.4451f, -383.3332f); _q.Size = new Vector2(10.08327f, 10.08327f); }
	_q = MyPile.FindQuad("Button_ViewGamer"); if (_q != null) { _q.Pos = new Vector2(705.5554f, 783.3331f); _q.Size = new Vector2(67.25001f, 67.25001f); }
	_q = MyPile.FindQuad("Button_SwitchView"); if (_q != null) { _q.Pos = new Vector2(705.5554f, 627.7772f); _q.Size = new Vector2(65.16659f, 65.16659f); }
	_q = MyPile.FindQuad("Button_LeftRight"); if (_q != null) { _q.Pos = new Vector2(705.5554f, 472.2213f); _q.Size = new Vector2(73.74999f, 73.74999f); }
	_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(1536.111f, 227.7776f); _q.Size = new Vector2(67.99999f, 67.99999f); }
	_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(1369.444f, 216.6666f); _q.Size = new Vector2(77.71317f, 66.83332f); }

	MyPile.Pos = new Vector2(0f, 5.555542f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
{
	MenuItem _item;
	_item = MyMenu.FindItemByName("ViewGamer"); if (_item != null) { _item.SetPos = new Vector2(-885.7779f, 168.3334f); _item.MyText.Scale = 0.3670001f; _item.MySelectedText.Scale = 0.3670001f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("SwitchView"); if (_item != null) { _item.SetPos = new Vector2(-882.9999f, 15.00238f); _item.MyText.Scale = 0.3969168f; _item.MySelectedText.Scale = 0.3969168f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("SwitchSort"); if (_item != null) { _item.SetPos = new Vector2(-880.2222f, -116.1065f); _item.MyText.Scale = 0.4625835f; _item.MySelectedText.Scale = 0.4625835f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(1672.222f, 686.1112f);

	EzText _t;
	_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-1313.888f, 991.6661f); _t.Scale = 0.5240005f; }
	_t = MyPile.FindEzText("GameTitle"); if (_t != null) { _t.Pos = new Vector2(-1302.778f, 861.1112f); _t.Scale = 0.4570001f; }
	_t = MyPile.FindEzText("NotRankedFriends"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.4956669f; }
	_t = MyPile.FindEzText("NotRanked"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.4956669f; }
	_t = MyPile.FindEzText("Loading"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.3519495f; }
	_t = MyPile.FindEzText("LeftRight"); if (_t != null) { _t.Pos = new Vector2(797.2221f, 538.8887f); _t.Scale = 0.3897502f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-408.3335f, 2.777821f); _q.Size = new Vector2(1094.068f, 1006.303f); }
	_q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(1266.665f, 519.4443f); _q.Size = new Vector2(418.2869f, 684.4695f); }
	_q = MyPile.FindQuad("Highlight"); if (_q != null) { _q.Pos = new Vector2(-413.8886f, 643.8893f); _q.Size = new Vector2(1005.093f, 49.08278f); }
	_q = MyPile.FindQuad("TL"); if (_q != null) { _q.Pos = new Vector2(-1300.001f, 713.8893f); _q.Size = new Vector2(0.9999986f, 0.9999986f); }
	_q = MyPile.FindQuad("Offset_GamerTag"); if (_q != null) { _q.Pos = new Vector2(4820f, -363.889f); _q.Size = new Vector2(1f, 1f); }
	_q = MyPile.FindQuad("Offset_Val"); if (_q != null) { _q.Pos = new Vector2(13808.34f, -116.6667f); _q.Size = new Vector2(1f, 1f); }
	_q = MyPile.FindQuad("Offset"); if (_q != null) { _q.Pos = new Vector2(-869.4451f, -383.3332f); _q.Size = new Vector2(10.08327f, 10.08327f); }
	_q = MyPile.FindQuad("Button_ViewGamer"); if (_q != null) { _q.Pos = new Vector2(702.7778f, 777.7776f); _q.Size = new Vector2(67.25001f, 67.25001f); }
	_q = MyPile.FindQuad("Button_SwitchView"); if (_q != null) { _q.Pos = new Vector2(705.5557f, 613.8885f); _q.Size = new Vector2(65.16659f, 65.16659f); }
	_q = MyPile.FindQuad("Button_LeftRight"); if (_q != null) { _q.Pos = new Vector2(708.3334f, 472.2221f); _q.Size = new Vector2(70.08331f, 70.08331f); }
	_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(1536.111f, 227.7776f); _q.Size = new Vector2(67.99999f, 67.99999f); }
	_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(1369.444f, 216.6666f); _q.Size = new Vector2(77.71317f, 66.83332f); }

	MyPile.Pos = new Vector2(0f, 5.555542f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Spanish)
{
	MenuItem _item;
	_item = MyMenu.FindItemByName("ViewGamer"); if (_item != null) { _item.SetPos = new Vector2(-896.1116f, 182.2222f); _item.MyText.Scale = 0.3883668f; _item.MySelectedText.Scale = 0.3883668f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("SwitchView"); if (_item != null) { _item.SetPos = new Vector2(-890.5558f, 15.00228f); _item.MyText.Scale = 0.3815225f; _item.MySelectedText.Scale = 0.3815225f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("SwitchSort"); if (_item != null) { _item.SetPos = new Vector2(-871.1111f, -127.2176f); _item.MyText.Scale = 0.4108887f; _item.MySelectedText.Scale = 0.4108887f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(1672.222f - 60, 686.1112f);

	EzText _t;
	_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-1308.333f, 991.6661f); _t.Scale = 0.5240005f; }
	_t = MyPile.FindEzText("GameTitle"); if (_t != null) { _t.Pos = new Vector2(-1291.667f, 861.1112f); _t.Scale = 0.3976667f; }
	_t = MyPile.FindEzText("NotRankedFriends"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.4956669f; }
	_t = MyPile.FindEzText("NotRanked"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.4956669f; }
	_t = MyPile.FindEzText("Loading"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.351667f; }
	_t = MyPile.FindEzText("LeftRight"); if (_t != null) { _t.Pos = new Vector2(786.888f, 547.222f); _t.Scale = 0.3899582f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-408.3335f, 2.777821f); _q.Size = new Vector2(1094.068f, 1006.303f); }
	_q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(1266.665f, 519.4443f); _q.Size = new Vector2(418.2869f, 684.4695f); }
	_q = MyPile.FindQuad("Highlight"); if (_q != null) { _q.Pos = new Vector2(-413.8886f, 643.8893f); _q.Size = new Vector2(1005.093f, 49.08278f); }
	_q = MyPile.FindQuad("TL"); if (_q != null) { _q.Pos = new Vector2(-1300.001f, 713.8893f); _q.Size = new Vector2(0.9999986f, 0.9999986f); }
	_q = MyPile.FindQuad("Offset_GamerTag"); if (_q != null) { _q.Pos = new Vector2(4820f, -363.889f); _q.Size = new Vector2(1f, 1f); }
	_q = MyPile.FindQuad("Offset_Val"); if (_q != null) { _q.Pos = new Vector2(13808.34f, -116.6667f); _q.Size = new Vector2(1f, 1f); }
	_q = MyPile.FindQuad("Offset"); if (_q != null) { _q.Pos = new Vector2(-869.4451f, -383.3332f); _q.Size = new Vector2(10.08327f, 10.08327f); }
	_q = MyPile.FindQuad("Button_ViewGamer"); if (_q != null) { _q.Pos = new Vector2(705.5554f, 783.3331f); _q.Size = new Vector2(67.25001f, 67.25001f); }
	_q = MyPile.FindQuad("Button_SwitchView"); if (_q != null) { _q.Pos = new Vector2(705.5554f, 616.6663f); _q.Size = new Vector2(65.16659f, 65.16659f); }
	_q = MyPile.FindQuad("Button_LeftRight"); if (_q != null) { _q.Pos = new Vector2(708.3333f, 469.4439f); _q.Size = new Vector2(73.74999f, 73.74999f); }
	_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(1536.111f, 227.7776f); _q.Size = new Vector2(67.99999f, 67.99999f); }
	_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(1369.444f, 216.6666f); _q.Size = new Vector2(77.71317f, 66.83332f); }

	MyPile.Pos = new Vector2(0f - 60, 5.555542f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Portuguese)
{
	MenuItem _item;
	_item = MyMenu.FindItemByName("ViewGamer"); if (_item != null) { _item.SetPos = new Vector2(-908.0002f, 168.3334f); _item.MyText.Scale = 0.3748334f; _item.MySelectedText.Scale = 0.3748334f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("SwitchView"); if (_item != null) { _item.SetPos = new Vector2(-905.2222f, 17.78011f); _item.MyText.Scale = 0.3670003f; _item.MySelectedText.Scale = 0.3670003f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("SwitchSort"); if (_item != null) { _item.SetPos = new Vector2(-874.6667f, -116.1065f); _item.MyText.Scale = 0.5009168f; _item.MySelectedText.Scale = 0.5009168f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(1672.222f, 686.1112f);

	EzText _t;
	_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-1308.333f, 991.6661f); _t.Scale = 0.5240005f; }
	_t = MyPile.FindEzText("GameTitle"); if (_t != null) { _t.Pos = new Vector2(-1302.778f, 861.1112f); _t.Scale = 0.4570001f; }
	_t = MyPile.FindEzText("NotRankedFriends"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.4956669f; }
	_t = MyPile.FindEzText("NotRanked"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.4956669f; }
	_t = MyPile.FindEzText("Loading"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.3595637f; }
	_t = MyPile.FindEzText("LeftRight"); if (_t != null) { _t.Pos = new Vector2(769.4445f, 552.7775f); _t.Scale = 0.3768338f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-408.3335f, 2.777821f); _q.Size = new Vector2(1094.068f, 1006.303f); }
	_q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(1266.665f, 519.4443f); _q.Size = new Vector2(418.2869f, 684.4695f); }
	_q = MyPile.FindQuad("Highlight"); if (_q != null) { _q.Pos = new Vector2(-413.8886f, 643.8893f); _q.Size = new Vector2(1005.093f, 49.08278f); }
	_q = MyPile.FindQuad("TL"); if (_q != null) { _q.Pos = new Vector2(-1300.001f, 713.8893f); _q.Size = new Vector2(0.9999986f, 0.9999986f); }
	_q = MyPile.FindQuad("Offset_GamerTag"); if (_q != null) { _q.Pos = new Vector2(4820f, -363.889f); _q.Size = new Vector2(1f, 1f); }
	_q = MyPile.FindQuad("Offset_Val"); if (_q != null) { _q.Pos = new Vector2(13808.34f, -116.6667f); _q.Size = new Vector2(1f, 1f); }
	_q = MyPile.FindQuad("Offset"); if (_q != null) { _q.Pos = new Vector2(-869.4451f, -383.3332f); _q.Size = new Vector2(10.08327f, 10.08327f); }
	_q = MyPile.FindQuad("Button_ViewGamer"); if (_q != null) { _q.Pos = new Vector2(688.8887f, 769.4443f); _q.Size = new Vector2(62.08334f, 62.08334f); }
	_q = MyPile.FindQuad("Button_SwitchView"); if (_q != null) { _q.Pos = new Vector2(691.6668f, 622.2218f); _q.Size = new Vector2(61.74989f, 61.74989f); }
	_q = MyPile.FindQuad("Button_LeftRight"); if (_q != null) { _q.Pos = new Vector2(699.9999f, 486.111f); _q.Size = new Vector2(66.74998f, 66.74998f); }
	_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(1536.111f, 227.7776f); _q.Size = new Vector2(67.99999f, 67.99999f); }
	_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(1369.444f, 216.6666f); _q.Size = new Vector2(77.71317f, 66.83332f); }

	MyPile.Pos = new Vector2(0f, 5.555542f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
{
	MenuItem _item;
	_item = MyMenu.FindItemByName("ViewGamer"); if (_item != null) { _item.SetPos = new Vector2(-885.778f, 185.0001f); _item.MyText.Scale = 0.4655002f; _item.MySelectedText.Scale = 0.4655002f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("SwitchView"); if (_item != null) { _item.SetPos = new Vector2(-877.4442f, 34.44678f); _item.MyText.Scale = 0.4790835f; _item.MySelectedText.Scale = 0.4790835f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("SwitchSort"); if (_item != null) { _item.SetPos = new Vector2(-874.6667f, -116.1065f); _item.MyText.Scale = 0.5009168f; _item.MySelectedText.Scale = 0.5009168f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(1672.222f, 686.1112f);

	EzText _t;
	_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-1350f, 988.8883f); _t.Scale = 0.5240005f; }
	_t = MyPile.FindEzText("GameTitle"); if (_t != null) { _t.Pos = new Vector2(-1344.445f, 858.3334f); _t.Scale = 0.4432501f; }
	_t = MyPile.FindEzText("NotRankedFriends"); if (_t != null) { _t.Pos = new Vector2(-402.7775f, -16.66664f); _t.Scale = 0.4863335f; }
	_t = MyPile.FindEzText("NotRanked"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.4925002f; }
	_t = MyPile.FindEzText("Loading"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.3746839f; }
	_t = MyPile.FindEzText("LeftRight"); if (_t != null) { _t.Pos = new Vector2(794.4443f, 563.8887f); _t.Scale = 0.4802502f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-408.3335f, 2.777821f); _q.Size = new Vector2(1094.068f, 1006.303f); }
	_q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(1266.665f, 519.4443f); _q.Size = new Vector2(418.2869f, 684.4695f); }
	_q = MyPile.FindQuad("Highlight"); if (_q != null) { _q.Pos = new Vector2(-413.8886f, 643.8893f); _q.Size = new Vector2(1005.093f, 49.08278f); }
	_q = MyPile.FindQuad("TL"); if (_q != null) { _q.Pos = new Vector2(-1300.001f, 713.8893f); _q.Size = new Vector2(0.9999986f, 0.9999986f); }
	_q = MyPile.FindQuad("Offset_GamerTag"); if (_q != null) { _q.Pos = new Vector2(4820f, -363.889f); _q.Size = new Vector2(1f, 1f); }
	_q = MyPile.FindQuad("Offset_Val"); if (_q != null) { _q.Pos = new Vector2(13808.34f, -116.6667f); _q.Size = new Vector2(1f, 1f); }
	_q = MyPile.FindQuad("Offset"); if (_q != null) { _q.Pos = new Vector2(-869.4451f, -383.3332f); _q.Size = new Vector2(10.08327f, 10.08327f); }
	_q = MyPile.FindQuad("Button_ViewGamer"); if (_q != null) { _q.Pos = new Vector2(705.5554f, 783.3331f); _q.Size = new Vector2(67.25001f, 67.25001f); }
	_q = MyPile.FindQuad("Button_SwitchView"); if (_q != null) { _q.Pos = new Vector2(705.5554f, 619.4439f); _q.Size = new Vector2(65.16659f, 65.16659f); }
	_q = MyPile.FindQuad("Button_LeftRight"); if (_q != null) { _q.Pos = new Vector2(705.5554f, 472.2213f); _q.Size = new Vector2(73.74999f, 73.74999f); }
	_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(1536.111f, 227.7776f); _q.Size = new Vector2(67.99999f, 67.99999f); }
	_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(1369.444f, 216.6666f); _q.Size = new Vector2(77.71317f, 66.83332f); }

	MyPile.Pos = new Vector2(0f, 5.555542f);
}
else
{
	MenuItem _item;
	_item = MyMenu.FindItemByName("ViewGamer"); if (_item != null) { _item.SetPos = new Vector2(-885.778f, 185.0001f); _item.MyText.Scale = 0.4655002f; _item.MySelectedText.Scale = 0.4655002f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("SwitchView"); if (_item != null) { _item.SetPos = new Vector2(-877.4442f, 34.44678f); _item.MyText.Scale = 0.4790835f; _item.MySelectedText.Scale = 0.4790835f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("SwitchSort"); if (_item != null) { _item.SetPos = new Vector2(-874.6667f, -116.1065f); _item.MyText.Scale = 0.5009168f; _item.MySelectedText.Scale = 0.5009168f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(1672.222f, 686.1112f);

	EzText _t;
	_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-1308.333f, 991.6661f); _t.Scale = 0.5240005f; }
	_t = MyPile.FindEzText("GameTitle"); if (_t != null) { _t.Pos = new Vector2(-1302.778f, 861.1112f); _t.Scale = 0.4570001f; }
	_t = MyPile.FindEzText("NotRankedFriends"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.4956669f; }
	_t = MyPile.FindEzText("NotRanked"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.4956669f; }
	_t = MyPile.FindEzText("Loading"); if (_t != null) { _t.Pos = new Vector2(-391.6667f, -16.66664f); _t.Scale = 0.3850924f; }
	_t = MyPile.FindEzText("LeftRight"); if (_t != null) { _t.Pos = new Vector2(802.7778f, 558.3332f); _t.Scale = 0.4802502f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-408.3335f, 2.777821f); _q.Size = new Vector2(1094.068f, 1006.303f); }
	_q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(1266.665f, 519.4443f); _q.Size = new Vector2(418.2869f, 684.4695f); }
	_q = MyPile.FindQuad("Highlight"); if (_q != null) { _q.Pos = new Vector2(-413.8886f, 643.8893f); _q.Size = new Vector2(1005.093f, 49.08278f); }
	_q = MyPile.FindQuad("TL"); if (_q != null) { _q.Pos = new Vector2(-1300.001f, 713.8893f); _q.Size = new Vector2(0.9999986f, 0.9999986f); }
	_q = MyPile.FindQuad("Offset_GamerTag"); if (_q != null) { _q.Pos = new Vector2(4820f, -363.889f); _q.Size = new Vector2(1f, 1f); }
	_q = MyPile.FindQuad("Offset_Val"); if (_q != null) { _q.Pos = new Vector2(13808.34f, -116.6667f); _q.Size = new Vector2(1f, 1f); }
	_q = MyPile.FindQuad("Offset"); if (_q != null) { _q.Pos = new Vector2(-869.4451f, -383.3332f); _q.Size = new Vector2(10.08327f, 10.08327f); }
	_q = MyPile.FindQuad("Button_ViewGamer"); if (_q != null) { _q.Pos = new Vector2(705.5554f, 783.3331f); _q.Size = new Vector2(67.25001f, 67.25001f); }
	_q = MyPile.FindQuad("Button_SwitchView"); if (_q != null) { _q.Pos = new Vector2(705.5554f, 619.4439f); _q.Size = new Vector2(65.16659f, 65.16659f); }
	_q = MyPile.FindQuad("Button_LeftRight"); if (_q != null) { _q.Pos = new Vector2(705.5554f, 472.2213f); _q.Size = new Vector2(73.74999f, 73.74999f); }
	_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(1536.111f, 227.7776f); _q.Size = new Vector2(67.99999f, 67.99999f); }
	_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(1369.444f, 216.6666f); _q.Size = new Vector2(77.71317f, 66.83332f); }

	MyPile.Pos = new Vector2(0f, 5.555542f);
}
		}
    }

    public class LeaderboardItem
    {
        public Gamer Player;
        public string GamerTag;
        public string Val;
        public string Rank;
		public float scale;
		public bool Special;

		static Dictionary<string, bool> SpecialNames = new Dictionary<string,bool> {
			{ "Tewth Brush", true },
			{ "TewthDecay", true },
			{ "PwneeStudios", true },
			{ "PwneeJordan", true },
			{ "PwneeTJ", true },
			{ "Pwnee", true },
																				   };
        public static LeaderboardItem DefaultItem = new LeaderboardItem(null, 0, 0);

        public LeaderboardItem(Gamer Player, int Val, int Rank)
        {
            this.Player = Player;
            this.Rank = Rank.ToString();

            if (Player == null)
            {
				this.GamerTag = Localization.WordString(Localization.Words.Loading) + "...";
                this.Val = "...";

                scale = 1;
            }
            else
            {
                this.GamerTag = Player.Gamertag;
                this.Val = Val.ToString();

                float width = Tools.QDrawer.MeasureString(Resources.Font_Grobold42.HFont, GamerTag).X;
                if (width > 850)
                    scale = 850 / width;
                else
                    scale = 1;
            }

			Special = false;
			if (SpecialNames.ContainsKey(GamerTag))
				Special = true;
        }

        public void Draw(Vector2 Pos, bool Selected, float alpha)
        {
            Vector4 color = ColorHelper.Gray(.9f);
            Vector4 ocolor = Color.Black.ToVector4();

            if (Selected)
			{
				color = Color.LimeGreen.ToVector4();
				ocolor = new Color(0, 0, 0).ToVector4();
			}
			else if (Special)
			{
				color = Color.Gold.ToVector4();
				ocolor = new Color(0, 0, 0).ToVector4();
			}
            
			
			color *= alpha;

            Vector2 GamerTag_Offset = .1f * new Vector2(LeaderboardGUI.Offset_GamerTag.Pos.X,
										-(1 - scale) * 1000);
            Vector2 Val_Offset = .1f * new Vector2(LeaderboardGUI.Offset_Val.Pos.X, 0);
            Vector2 Size = .1f * new Vector2(LeaderboardGUI.ItemShift.SizeX);

            if (Selected)
            {
                Tools.QDrawer.DrawString(Resources.Font_Grobold42.HOutlineFont, Rank, Pos, ocolor, Size);
				Tools.QDrawer.DrawString(Resources.Font_Grobold42.HOutlineFont, GamerTag, Pos + GamerTag_Offset, ocolor, scale * Size);
                Tools.QDrawer.DrawString(Resources.Font_Grobold42.HOutlineFont, Val, Pos + Val_Offset, ocolor, Size);
            }

            Tools.QDrawer.DrawString(Resources.Font_Grobold42.HFont, Rank, Pos, color, Size);
			Tools.QDrawer.DrawString(Resources.Font_Grobold42.HFont, GamerTag, Pos + GamerTag_Offset, color, scale * Size);
            Tools.QDrawer.DrawString(Resources.Font_Grobold42.HFont, Val, Pos + Val_Offset, color, Size);
        }
    }

    public class LeaderboardView
    {
        const int EntriesPerPage = 18;
        //const int EntriesPerPage = 3;
        public int TotalEntries;

        public bool Loading;

        int Index;
        int Start;
        int End() { return CoreMath.Restrict(0, TotalEntries, Start + EntriesPerPage); }

        public Dictionary<int, LeaderboardItem> Items
        {
            get
            {
                return MyLeaderboard.Items;
            }
        }

        public Leaderboard MyLeaderboard;

        int Control;
        public LeaderboardView(int Id, LeaderboardGUI.LeaderboardType CurrentType)
        {
            TotalEntries = 0;// 1000000;
            Index = 1;
            Start = 1;

            Loading = true;

            LeaderboardItem.DefaultItem = new LeaderboardItem(null, 0, 0);

            MyLeaderboard = new Leaderboard(Id);

            MyLeaderboard.SetType(CurrentType);
        }

        void IncrIndex(int change)
        {
            Index = CoreMath.Restrict(1, TotalEntries, Index + change);

            UpdateBounds();
        }

        void UpdateBounds()
        {
            if (Index >= End())
                //Start = Index - EntriesPerPage + 1;
                Start = Index - EntriesPerPage;
            if (Index < Start)
                Start = Index;
            Start = CoreMath.Restrict(1, TotalEntries, Start);
        }

        int DelayCount_UpDown, MotionCount_UpDown;
        const int SelectDelay = 11;
        public void PhsxStep(int Control)
        {
            // Try reloading board again if we haven't gotten results yet.
            if (Loading && MyLeaderboard != null && !MyLeaderboard.Updated)
            {
                MyLeaderboard.SetType(MyLeaderboard.MySortType);
            }

            // If the board has been updated then take information in
            if (MyLeaderboard != null && MyLeaderboard.Updated)
            {
                lock (Items)
                {
                    if (Loading)
                    {
                        Index = MyLeaderboard.StartIndex;
                        Start = Index - EntriesPerPage / 2 + 1;
                        TotalEntries = MyLeaderboard.TotalSize;
                        UpdateBounds();
                    }

                    MyLeaderboard.Updated = false;
                    Loading = false;
                }
            }

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

        public void ViewGamer()
        {
            lock (Items)
            {
                if (Items.Count > 0)
                {
                    if (Items.ContainsKey(Index))
                    {
                        Gamer gamer = Items[Index].Player;
                        if (gamer != null && MenuItem.ActivatingPlayer >= 0 && MenuItem.ActivatingPlayer <= 3) 
                        {
#if XBOX
                            CloudberryKingdomGame.ShowGamerCard((PlayerIndex)MenuItem.ActivatingPlayer, gamer);
#endif
                        }
                    }
                }
            }
        }

        public void SetType(LeaderboardGUI.LeaderboardType type)
        {
            if (type == LeaderboardGUI.LeaderboardType.FriendsScores && MyLeaderboard.FriendItems.Count > 0)
            {
                Loading = false;
                Start = Index = 1;
                TotalEntries = MyLeaderboard.FriendItems.Count;
                UpdateBounds();
            }
            else
            {
                Loading = true;
            }

            MyLeaderboard.SetType(type);
        }

        public void Draw(Vector2 Pos, float alpha)
        {
            lock (Items)
            {
                Vector2 CurPos = Pos;
                float Shift = .1f * LeaderboardGUI.ItemShift.X;

                if (MyLeaderboard == null) return;

                if (MyLeaderboard.MySortType == LeaderboardGUI.LeaderboardType.FriendsScores)
                {
                    DrawList(alpha, CurPos, Shift);
                }
                else
                {
                    DrawDict(alpha, CurPos, Shift);
                }
            }
        }

        void DrawList(float alpha, Vector2 CurPos, float Shift)
        {
            for (int i = Start; i <= End(); i++)
            {
                bool Selected = i == Index;

                if (Selected)
                {
                    LeaderboardGUI.Highlight.PosY = CurPos.Y - 70;
                    LeaderboardGUI.Highlight.Show = true;
                    LeaderboardGUI.Highlight.Draw();
                    LeaderboardGUI.Highlight.Show = false;
                }

                if (i-1 < MyLeaderboard.FriendItems.Count)
                {
                    MyLeaderboard.FriendItems[i-1].Draw(CurPos, Selected, alpha);
                }

                CurPos.Y += Shift;
            }
        }

        void DrawDict(float alpha, Vector2 CurPos, float Shift)
        {
            bool RequestMore = false;
            int MinExisting = Start, MaxExisting = Start, MinMissing = -1, MaxMissing = -1;

            for (int i = Start; i <= End(); i++)
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

                    MaxExisting = Math.Max(MaxExisting, i);
                    MinExisting = Math.Min(MinExisting, i);
                }
                else
                {
                    RequestMore = true;

                    MaxMissing = MaxMissing == -1 ? i : Math.Max(MaxMissing, i);
                    MinMissing = MinMissing == -1 ? i : Math.Min(MinMissing, i);

                    LeaderboardItem Default = LeaderboardItem.DefaultItem;
                    Default.Rank = i.ToString();

                    Default.Draw(CurPos, Selected, alpha);
                }

                CurPos.Y += Shift;
            }


            if (RequestMore)
            {
                int PageToRequest;

                if (MinMissing >= MaxExisting)
                {
                    PageToRequest = CoreMath.Restrict(0, TotalEntries, MinMissing - 1);
                }
                else
                {
                    PageToRequest = CoreMath.Restrict(0, TotalEntries, MaxMissing - 1);
                }

                MyLeaderboard.RequestMore(PageToRequest);
            }
        }
    }
}