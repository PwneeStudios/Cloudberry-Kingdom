using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class StatsMenu : CkBaseMenu
    {
        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MyText.Shadow = item.MySelectedText.Shadow = false;
        }

        protected override void SetHeaderProperties(EzText text)
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
        static float[] x2 = { 1722.699f, 2454.445f };
        static float[] x3 = { 1650f, 2075f, 2505f };
        static float[] x4 = { 1550f, 1905f, 2260f, 2615f};

        static float[][] x = { null, x1, x2, x3, x4 };
        static Vector2[][] name_pos = { null, x1_name, x2_name, x3_name, x4_name };

        class StringificationWrapper : LambdaFunc_1<int, string>
        {
            LambdaFunc_1<int, int> f;

            public StringificationWrapper(LambdaFunc_1<int, int> f)
            {
                this.f = f;
            }

            public string Apply(int i)
            {
                return f.Apply(i).ToString();
            }
        }

        MenuItem AddRow(MenuItem Item, LambdaFunc_1<int, int> f)
        {
            return AddRow(Item, new StringificationWrapper(f));
        }
        MenuItem AddRow(MenuItem Item, LambdaFunc_1<int, string> f)
        {
            AddItem(Item);

            int index = 0;
            EzText Text;
            for (int j = 0; j < 4; j++)
            {
                if (PlayerManager.Get(j).Exists)
                {
                    string val = f.Apply(j).ToString();

                    Text = new EzText(val, ItemFont, false, true);
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

#if PC_VERSION
        ScrollBar bar;
#endif

        class StatsLevels : LambdaFunc_1<int, int>
        {
            PlayerStats[] Stats;

            public StatsLevels(PlayerStats[] Stats)
            {
                this.Stats = Stats;
            }
        
            public int Apply(int j)
            {
 	            return Stats[j].Levels;
            }
        }

        class StatsJumps : LambdaFunc_1<int, int>
        {
            PlayerStats[] Stats;

            public StatsJumps(PlayerStats[] Stats)
            {
                this.Stats = Stats;
            }

            public int Apply(int j)
            {
                return Stats[j].Jumps;
            }
        }

        class StatsScore : LambdaFunc_1<int, int>
        {
            PlayerStats[] Stats;

            public StatsScore(PlayerStats[] Stats)
            {
                this.Stats = Stats;
            }

            public int Apply(int j)
            {
                return Stats[j].Score;
            }
        }

        class StatsCoins : LambdaFunc_1<int, int>
        {
            PlayerStats[] Stats;

            public StatsCoins(PlayerStats[] Stats)
            {
                this.Stats = Stats;
            }

            public int Apply(int j)
            {
                return Stats[j].Coins;
            }
        }

        class StatsTotalCoins : LambdaFunc_1<int, int>
        {
            PlayerStats[] Stats;

            public StatsTotalCoins(PlayerStats[] Stats)
            {
                this.Stats = Stats;
            }

            public int Apply(int j)
            {
                return Stats[j].TotalCoins;
            }
        }

        class StatsCoinPercentGotten : LambdaFunc_1<int, string>
        {
            PlayerStats[] Stats;

            public StatsCoinPercentGotten(PlayerStats[] Stats)
            {
                this.Stats = Stats;
            }

            public string Apply(int j)
            {
                return Stats[j].CoinPercentGotten.ToString() + '%';
            }
        }

        class StatsBlobs : LambdaFunc_1<int, int>
        {
            PlayerStats[] Stats;

            public StatsBlobs(PlayerStats[] Stats)
            {
                this.Stats = Stats;
            }

            public int Apply(int j)
            {
                return Stats[j].Blobs;
            }
        }

        class StatsCheckpoints : LambdaFunc_1<int, int>
        {
            PlayerStats[] Stats;

            public StatsCheckpoints(PlayerStats[] Stats)
            {
                this.Stats = Stats;
            }

            public int Apply(int j)
            {
                return Stats[j].Checkpoints;
            }
        }

        class StatsLifeExpectancy : LambdaFunc_1<int, string>
        {
            PlayerStats[] Stats;

            public StatsLifeExpectancy(PlayerStats[] Stats)
            {
                this.Stats = Stats;
            }

            public string Apply(int j)
            {
                return Stats[j].LifeExpectancy;
            }
        }

        class StatsDeathsBy : LambdaFunc_1<int, int>
        {
            PlayerStats[] Stats;
            int i;

            public StatsDeathsBy(PlayerStats[] Stats, int i)
            {
                this.Stats = Stats;
                this.i = i;
            }

            public int Apply(int j)
            {
                return Stats[j].DeathsBy[i];
            }
        }

        int n;
        float HeaderPos = -1595f;
        PlayerStats[] Stats = new PlayerStats[4];
        public StatsMenu(StatGroup group)
        {
            n = PlayerManager.GetNumPlayers();

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

            MyMenu.OnB = new MenuReturnToCallerLambdaFunc(this);

            MakeBack();

            ItemPos = new Vector2(-1305, 950);
            PosAdd = new Vector2(0, -151) * 1.2f * 1.1f;
            Vector2 HeaderPosAdd = PosAdd + new Vector2(0, -120);

            BarPos = 
                new Vector2(340, 125);

            // Header
            MenuItem Header;
            if (group == StatGroup.Lifetime)
                Header = new MenuItem(new EzText(Localization.Words.Statistics, Resources.Font_Grobold42_2));
            else if (group == StatGroup.Campaign)
            {
                Header = new MenuItem(new EzText(Localization.Words.StoryMode, Resources.Font_Grobold42_2));
                Header.MyText.Scale *= .725f;
            }
            else
                Header = new MenuItem(new EzText(Localization.Words.Statistics, Resources.Font_Grobold42_2));
            MyMenu.Add(Header);
            Header.Pos =
                new Vector2(HeaderPos, ItemPos.Y - 40);
            SetHeaderProperties(Header.MyText);
            Header.MyText.Scale *= 1.15f;
            Header.Selectable = false;
            ItemPos += HeaderPosAdd;

#if NOT_PC
            EzText Text;
            Header = new MenuItem(new EzText("", ItemFont));
            MyMenu.Add(Header);
            Header.Pos =
                new Vector2(-1138.889f, 988.0952f);
            Header.Selectable = false;
            int index = 0;
            for (int j = 0; j < 4; j++)
            {
                if (PlayerManager.Get(j).Exists)
                {
                    //string val = "Hobabby!";
                    string val = PlayerManager.Get(j).GetName();
                    Text = new EzText(val, ItemFont, true, true);
                    Text.Layer = 1;
                    MyPile.Add(Text);
                    Text.FancyPos.SetCenter(Header.FancyPos);
                    GamerTag.ScaleGamerTag(Text);

                    //Text.Pos = new Vector2(1090 + xSpacing * j, -147.1428f);
                    //if (j % 2 == 0) Text.FancyPos.RelVal.Y -= 150;
                    //Text.Scale *= .85f;

                    Text.Pos = name_pos[n][index];

                    if (n == 1) Text.Scale *= 1.15f;
                    else if (n == 2) Text.Scale *= 1.05f;
                    else if (n == 3) Text.Scale *= .95f;
                    else Text.Scale *= .85f;

                    index++;
                }
            }
#endif
            AddRow(new MenuItem(new EzText(Localization.Words.LevelsBeat, ItemFont)), new StatsLevels(Stats));
            AddRow(new MenuItem(new EzText(Localization.Words.Jumps, ItemFont)), new StatsJumps(Stats));
            AddRow(new MenuItem(new EzText(Localization.Words.Score, ItemFont)), new StatsScore(Stats));


            // Coins
            var coinitem = new MenuItem(new EzText(Localization.Words.Coins, ItemFont));
            coinitem.Selectable = false;
            AddItem(coinitem);

            AddRow(new MenuItem(new EzText(Localization.Words.Grabbed, ItemFont)), new StatsCoins(Stats));
            AddRow(new MenuItem(new EzText(Localization.Words.CoinsOutOf, ItemFont)), new StatsTotalCoins(Stats));
            AddRow(new MenuItem(new EzText(Localization.Words.Percent, ItemFont)), new StatsCoinPercentGotten(Stats));

            AddRow(new MenuItem(new EzText(Localization.Words.FlyingBlobs, ItemFont)), new StatsBlobs(Stats));
            AddRow(new MenuItem(new EzText(Localization.Words.Checkpoints, ItemFont)), new StatsCheckpoints(Stats));
            AddRow(new MenuItem(new EzText(Localization.Words.AverageLife, ItemFont)), new StatsLifeExpectancy(Stats));
            
            // Deaths
            Header = new MenuItem(new EzText(Localization.Words.Deaths, Resources.Font_Grobold42_2));
            MyMenu.Add(Header);
            Header.Pos =
                new Vector2(HeaderPos, ItemPos.Y - 40);
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

                    AddRow(new MenuItem(new EzText(word, ItemFont)), new StatsDeathsBy(Stats, i));
                }
            }


            // Back
            //MenuItem item = MakeBackButton();
            //item.ScaleText(1.4f);

            // Select first selectable item
            //MyMenu.SelectItem(0);
            MyMenu.SelectItem(1);



            // Darker Backdrop
            QuadClass Backdrop = new QuadClass("Backplate_1230x740", "Backdrop");
            MyPile.Add(Backdrop);
            MyPile.Add(Backdrop);

            EnsureFancy();
            MyMenu.Pos = new Vector2(67.45706f, 0f);
            MyPile.Pos = new Vector2(83.33417f, 130.9524f);

            SetPos();
        }


        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(1230.718f, 975.2383f); _item.MyText.Scale = 0.375f; _item.MySelectedText.Scale = 0.375f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            MyMenu.Pos = new Vector2(67.45706f, 0f);

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-91.66821f, -103.8888f); _q.Size = new Vector2(1907.893f, 1089.838f); }

            MyPile.Pos = new Vector2(83.33417f, 130.9524f);
        }

#if PC_VERSION
        class OnAddHelper : LambdaFunc<bool>
        {
            ScrollBar bar;

            public OnAddHelper(ScrollBar bar)
            {
                this.bar = bar;
            }

            public bool Apply()
            {
                return bar.MyMenu.HitTest();
            }
        }
#endif

        Vector2 BarPos;
        public override void OnAdd()
        {
            base.OnAdd();

            // Scroll bar
#if PC_VERSION
            bar = new ScrollBar((LongMenu)MyMenu, this);
            bar.BarPos = BarPos;
            MyGame.AddGameObject(bar);
            MyMenu.AdditionalCheckForOutsideClick = new OnAddHelper(bar);
#endif
        }

        void MakeBack()
        {
            MenuItem item;

            ItemPos = new Vector2(1230.718f, 975.2383f);
            item = MakeBackButton();
            item.UnaffectedByScroll = true;
            item.ScaleText(.5f);
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();
        }

        protected override void MyDraw()
        {
            base.MyDraw();

            MyPile.Draw(1);
        }
    }
}