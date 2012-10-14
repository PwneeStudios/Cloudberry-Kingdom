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

        MenuItem AddRow(MenuItem Item, Func<int, int> f)
        {
            return AddRow(Item, i => f(i).ToString());
        }
        MenuItem AddRow(MenuItem Item, Func<int, string> f)
        {
            AddItem(Item);

            int index = 0;
            EzText Text;
            for (int j = 0; j < 4; j++)
            {
                if (PlayerManager.Get(j).Exists)
                {
                    string val = f(j).ToString();

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

            MyMenu.OnB = MenuReturnToCaller;

            MakeBack();

            ItemPos = new Vector2(-1305, 950);
            PosAdd = new Vector2(0, -151) * 1.2f * 1.1f;
            Vector2 HeaderPosAdd = PosAdd + new Vector2(0, -120);

            BarPos = 
                new Vector2(340, 125);

            // Header
            MenuItem Header;
            if (group == StatGroup.Lifetime)
                Header = new MenuItem(new EzText("Statistics", Tools.Font_Grobold42_2));
            else if (group == StatGroup.Campaign)
            {
                Header = new MenuItem(new EzText("Campaign!", Tools.Font_Grobold42_2));
                Header.MyText.Scale *= .725f;
            }
            else
                Header = new MenuItem(new EzText("Stats!", Tools.Font_Grobold42_2));
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
            AddRow(new MenuItem(new EzText("Levels beat", ItemFont)), j => Stats[j].Levels);
            AddRow(new MenuItem(new EzText("Jumps", ItemFont)), j => Stats[j].Jumps);
            AddRow(new MenuItem(new EzText("Score", ItemFont)), j => Stats[j].Score);


            // Coins
            var coinitem = new MenuItem(new EzText("Coins", ItemFont));
            coinitem.Selectable = false;
            AddItem(coinitem);

            AddRow(new MenuItem(new EzText("    grabbed", ItemFont)), j => Stats[j].Coins);//.Selectable = false;
            AddRow(new MenuItem(new EzText("    out of", ItemFont)), j => Stats[j].TotalCoins);//.Selectable = false;
            AddRow(new MenuItem(new EzText("    (percent)", ItemFont)), j => Stats[j].CoinPercentGotten.ToString() + '%');
            //AddRow(new MenuItem(new EzText("Coins (Percent)", ItemFont)), j => Stats[j].CoinPercentGotten.ToString() + '%');
            //AddRow(new MenuItem(new EzText("Coins (Total)", ItemFont)), j => Stats[j].Coins).Selectable = false;

            AddRow(new MenuItem(new EzText("Blobs", ItemFont)), j => Stats[j].Blobs);
            AddRow(new MenuItem(new EzText("Checkpoints", ItemFont)), j => Stats[j].Checkpoints);
            AddRow(new MenuItem(new EzText("Average Life", ItemFont)), j => Stats[j].LifeExpectancy);
            
            //AddRow(new MenuItem(new EzText("Berries", ItemFont)), j => Stats[j].Berries);

            // Deaths
            Header = new MenuItem(new EzText("Deaths", Tools.Font_Grobold42_2));
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
                    string name = Bob.BobDeathNames[type];

                    AddRow(new MenuItem(new EzText(name, ItemFont)), j => Stats[j].DeathsBy[i]);
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

        Vector2 BarPos;
        public override void OnAdd()
        {
            base.OnAdd();

            // Scroll bar
#if PC_VERSION
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