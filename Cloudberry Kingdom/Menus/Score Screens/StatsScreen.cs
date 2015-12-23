using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Stats
{
    public class StatsMenu : CkBaseMenu
    {
        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MyText.Shadow = item.MySelectedText.Shadow = false;
        }

        protected override void SetHeaderProperties(Text text)
        {
            text.MyFloatColor = new Vector4(.6f, .6f, .6f, 1f);
            text.OutlineColor = new Vector4(0f, 0f, 0f, 1f);

            text.Shadow = false;
            text.ShadowColor = new Color(.2f, .2f, .2f, 1f);
            text.ShadowOffset = new Vector2(12, 12);

            text.Scale = FontScale * .9f;
        }

        static Vector2[] x1_name = { new Vector2(1431.285f, -158.9048f) };
        static Vector2[] x2_name = { new Vector2(1217, -147), new Vector2(2056, -147) };
        static Vector2[] x3_name = { new Vector2(1225f, -295), new Vector2(1624f, -127.3015f), new Vector2(2116f, -295) };
        static Vector2[] x4_name = { new Vector2(1090f, -295), new Vector2(1445f, -127.3015f), new Vector2(1800f, -295), new Vector2(2155f, -127.3015f) };

        static float[] x1 = { 1920 };
        static float[] x2 = { 1722.699f - 200, 2454.445f - 30 };
        static float[] x3 = { 1650f - 200, 2075f - 140, 2505f - 80 };
        static float[] x4 = { 1525f - 200, 1905f - 180, 2260f - 160, 2615f - 140 };

        static float[][] x = { null, x1, x2, x3, x4 };
        static Vector2[][] name_pos = { null, x1_name, x2_name, x3_name, x4_name };

        string FormatFunc(int n)
        {
            return string.Format("{0:n0}", n);
        }

        MenuItem AddRow(MenuItem Item, Func<int, int> f)
        {
            //return AddRow(Item, i => f(i).ToString());
            return AddRow(Item, i => FormatFunc(f(i)));
        }
        MenuItem AddRow(MenuItem Item, Func<int, string> f)
        {
            AddItem(Item);

            Item.MyText.Scale = .5f;
            Item.MySelectedText.Scale = .5f;

            int index = 0;
            Text Text;
            for (int j = 0; j < 4; j++)
            {
                if (PlayerManager.Get(j).Exists)
                {
                    string val = f(j).ToString();

                    Text = new Text(val, ItemFont, false, true);
                    Text.Layer = 1;
                    MyPile.Add(Text);
                    Text.FancyPos.SetCenter(Item.FancyPos);
                    //Text.Pos = new Vector2(1550 + xSpacing * j, -147.1428f);
                    Text.Pos = new Vector2(x[n][index], -147.1428f);
                    
                    Text.Scale *= .9f;
                    if (val.Length <= 5)
                        Text.Scale *= .7f;
                    else if (val.Length == 6)
                        Text.Scale *= .6f;
                    else
                        Text.Scale *= .5f;

                    Text.Scale *= .77f;

                    Text.RightJustify = true;

                    index++;
                }
            }

            return Item;
        }

        public override void Init()
        {
            base.Init();

            CategoryDelays();
        }

#if PC
        ScrollBar bar;
#endif

        Vector2 HeaderPosAdd;

        void SetParams()
        {
        //float[] x1 = { 1920 };
        //float[] x2 = { 1722.699f - 200, 2454.445f - 30 };
        //float[] x3 = { 1650f - 200, 2075f - 140, 2505f - 80 };
        //float[] x4 = { 1525f - 200, 1905f - 180, 2260f - 160, 2615f - 140};
        //x = new float[][]{ null, x1, x2, x3, x4 };

			//PlayerManager.Players[0].Exists = true;
			//PlayerManager.Players[1].Exists = true;
			//PlayerManager.Players[2].Exists = false;
			//PlayerManager.Players[3].Exists = false;
            

            ItemPos = new Vector2(-1225, 950);
            PosAdd = new Vector2(0, -141) * 1.2f * 1.1f;
            HeaderPosAdd = PosAdd + new Vector2(0, -120);

            BarPos = new Vector2(340, 125);
        }

        int n;
        float HeaderPos = -1595f;
        PlayerStats[] Stats = new PlayerStats[4];
        public StatsMenu(StatGroup group)
        {
            EnableBounce();

            // Grab the stats for each player
            for (int i = 0; i < 4; i++)
            {
                var player = PlayerManager.Get(i);
                
                Stats[i] = player.GetSummedStats(group);
                if (!Tools.CurLevel.Finished && !Tools.CurLevel.CoinsCountInStats)
                    Stats[i].TotalCoins += Tools.CurLevel.NumCoins;
            }

            MyPile = new DrawPile();

            // Make the menu
            MyMenu = new LongMenu();
            MyMenu.FixedToCamera = false;
            MyMenu.WrapSelect = false;

            MyMenu.Control = -1;

            MyMenu.OnB = MenuReturnToCaller;

            SetParams();
            n = PlayerManager.GetNumPlayers();

            MenuItem Header;

            // Header
            if (group == StatGroup.Lifetime)
                Header = new MenuItem(new Text(Localization.Words.Statistics, Resources.Font_Grobold42_2));
            else if (group == StatGroup.Campaign)
            {
                Header = new MenuItem(new Text(Localization.Words.StoryMode, Resources.Font_Grobold42_2));
                Header.MyText.Scale *= .725f;
            }
            else
                Header = new MenuItem(new Text(Localization.Words.Statistics, Resources.Font_Grobold42_2));
            MyMenu.Add(Header);
            Header.Pos = new Vector2(HeaderPos, ItemPos.Y - 40);
            SetHeaderProperties(Header.MyText);
            Header.MyText.Scale *= 1.15f;
            Header.Selectable = false;
            ItemPos += HeaderPosAdd;

#if NOT_PC
            Text Text;
            Header = new MenuItem(new Text("", ItemFont));
            MyMenu.Add(Header);
            Header.Pos = new Vector2(-1138.889f, 988.0952f);
            Header.Selectable = false;
            int index = 0;
            if (PlayerManager.NumPlayers > 1)
            for (int j = 0; j < 4; j++)
            {
                if (PlayerManager.Get(j).Exists)
                {
                    string val = PlayerManager.Get(j).GetName();
                    Text = MakeGamerTag(Header, index, val);

                    index++;
                }
            }
#endif
            AddRow(new MenuItem(new Text(Localization.Words.LevelsBeat, ItemFont)), j => Stats[j].Levels);
            AddRow(new MenuItem(new Text(Localization.Words.Jumps, ItemFont)), j => Stats[j].Jumps);
            AddRow(new MenuItem(new Text(Localization.Words.Score, ItemFont)), j => Stats[j].Score);

            AddRow(new MenuItem(new Text(Localization.Words.FlyingBlobs, ItemFont)), j => Stats[j].Blobs);
            AddRow(new MenuItem(new Text(Localization.Words.Checkpoints, ItemFont)), j => Stats[j].Checkpoints);
            AddRow(new MenuItem(new Text(Localization.Words.AverageLife, ItemFont)), j => Stats[j].LifeExpectancy);

            // Coins
            Header = new MenuItem(new Text(Localization.Words.Coins, Resources.Font_Grobold42_2));
            MyMenu.Add(Header);
            Header.Pos = new Vector2(HeaderPos, ItemPos.Y - 40);
            SetHeaderProperties(Header.MyText);
            Header.MyText.Scale *= 1.15f;
            Header.Selectable = false;
            ItemPos += HeaderPosAdd;

            //var coinitem = new MenuItem(new Text(Localization.Words.Coins, ItemFont));
            //coinitem.Selectable = false;
            //AddItem(coinitem);

            AddRow(new MenuItem(new Text(Localization.Words.Grabbed, ItemFont)), j => Stats[j].Coins);//.Selectable = false;
            AddRow(new MenuItem(new Text(Localization.Words.CoinsOutOf, ItemFont)), j => Stats[j].TotalCoins);//.Selectable = false;
            AddRow(new MenuItem(new Text(Localization.Words.Percent, ItemFont)), j => Stats[j].CoinPercentGotten.ToString() + '%');



            // Deaths
            Header = new MenuItem(new Text(Localization.Words.Deaths, Resources.Font_Grobold42_2));
            MyMenu.Add(Header);
            Header.Pos = new Vector2(HeaderPos, ItemPos.Y - 40);
            SetHeaderProperties(Header.MyText);
            Header.MyText.Scale *= 1.15f;
            Header.Selectable = false;
            ItemPos += HeaderPosAdd;

            int NumDeathTypes = Tools.Length<Bob.BobDeathType>();
            for (int i = 1; i < NumDeathTypes; i++)
            {
                Bob.BobDeathType type = (Bob.BobDeathType)i;

                if (Bob.BobDeathNames.ContainsKey(type))
                {
                    Localization.Words word = Bob.BobDeathNames[type];

                    AddRow(new MenuItem(new Text(word, ItemFont)), j => Stats[j].DeathsBy[i]);
                }
            }


            // Back
            //MenuItem item = MakeBackButton();
            //item.ScaleText(1.4f);

            // Select first selectable item
            //MyMenu.SelectItem(0);
            MyMenu.SelectItem(1);



            // Darker Backdrop
            QuadClass Backdrop;
			//if (UseSimpleBackdrop)
			if (true)
            {
                Backdrop = new QuadClass("Arcade_BoxLeft", 1500, true);
                Backdrop.Alpha *= .8f;
            }
            else
                Backdrop = new QuadClass("Backplate_1230x740", "Backdrop");

            MyPile.Add(Backdrop);
            MyPile.Add(Backdrop);

            EnsureFancy();
            MyMenu.Pos = new Vector2(67.45706f, 0f);
            MyPile.Pos = new Vector2(83.33417f, 130.9524f);

            MakeBack();

            SetPos();
        }

        private Text MakeGamerTag(MenuItem Header, int index, string val)
        {
            Text Text;
            Text = new Text(val, ItemFont, true, true);
            Text.Layer = 1;
            MyPile.Add(Text);
            Text.FancyPos.SetCenter(Header.FancyPos);
            //GamerTag.ScaleGamerTag(Text);

            Text.Pos = name_pos[n][index];

			//if (n == 1) Text.Scale *= .65f;
			//else if (n == 2) Text.Scale *= .5f;
			//else if (n == 3) Text.Scale *= .4f;
			//else Text.Scale *= .4f;

			float MaxWidth = 100;
			if (n == 1) MaxWidth = 900;
			else if (n == 2) MaxWidth = 700;
			else if (n == 3) MaxWidth = 700;
			else MaxWidth = 600;

			float w = Text.GetWorldWidth();
			if (w > MaxWidth)
				Text.Scale *= MaxWidth / w;

            return Text;
        }


        void SetPos()
        {
			//MenuItem _item;
			//_item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(1230.718f, 975.2383f); _item.MyText.Scale = 0.375f; _item.MySelectedText.Scale = 0.375f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			//MyMenu.Pos = new Vector2(67.45706f, 0f);

			//QuadClass _q;
			//_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-91.66821f, -103.8888f); _q.Size = new Vector2(1907.893f, 1089.838f); }
			//_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(1522.222f, -983.3331f); _q.Size = new Vector2(90f, 90f); }
			//_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(1322.222f, -1008.333f); _q.Size = new Vector2(100f, 86f); }

			//MyPile.Pos = new Vector2(83.33417f, 130.9524f);

			MyMenu.Pos = new Vector2(67.45706f, 0f);

			QuadClass _q;
			_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(1338.889f, -986.1112f); _q.Size = new Vector2(65.49991f, 65.49991f); }
			_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(1188.889f, -994.4443f); _q.Size = new Vector2(78.29454f, 67.33331f); }

			MyPile.Pos = new Vector2(83.33417f, 130.9524f);

        }

        Vector2 BarPos;
        public override void OnAdd()
        {
            base.OnAdd();

            // Scroll bar
#if PC
            bar = new ScrollBar((LongMenu)MyMenu, this);
            bar.BarPos = BarPos;
            MyGame.AddGameObject(bar);
            MyMenu.AdditionalCheckForOutsideClick += () => bar.MyMenu.HitTest();
#endif
        }

        void MakeBack()
        {
            MenuItem item;

            ItemPos = new Vector2(1230.718f, 975.2383f);
            
#if PC
            item = MakeBackButton();
            item.UnaffectedByScroll = true;
            item.ScaleText(.5f);
#else
            MakeStaticBackButton();
#endif
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();
        }

        protected override void MyDraw()
        {
            base.MyDraw();
        }
    }
}