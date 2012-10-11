using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_MW_SubMenu : StartMenu_MW_Campaign
    {
        public StartMenu_MW_SubMenu(TitleGameData_MW Title)
            : base(Title)
        {
            this.Title = Title;
        }

        protected override void CreateMenu()
        {
            MenuItem item;

            // Levels
            int Count = 0;
            for (int lvl = 100; lvl < 200; lvl += 10)
            {
                //string text = lvl.ToString();
                string text = string.Format("{0}-{1}", lvl, lvl + 9);

                item = new CampaignLevelItem(new EzText(text, ItemFont), lvl);
                item.Name = "item" + Count.ToString();
                item.Go = Cast.ToItem(Go);
                AddItem(item);

                Count++;
            }

            MyMenu.SelectItem(1);
            SetPos();
        }

        void Go()
        {
        }

        void Go(int StartLevel)
        {
        }

        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("item0"); if (_item != null) { _item.SetPos = new Vector2(875.3335f, 267.7778f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("item1"); if (_item != null) { _item.SetPos = new Vector2(886.4446f, 78.55569f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("item2"); if (_item != null) { _item.SetPos = new Vector2(875.3334f, -102.3332f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("item3"); if (_item != null) { _item.SetPos = new Vector2(872.5552f, -294.3334f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("item4"); if (_item != null) { _item.SetPos = new Vector2(878.111f, -491.8895f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("item5"); if (_item != null) { _item.SetPos = new Vector2(1536.445f, 260.5554f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("item6"); if (_item != null) { _item.SetPos = new Vector2(1536.444f, 74.11118f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("item7"); if (_item != null) { _item.SetPos = new Vector2(1539.223f, -106.7776f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("item8"); if (_item != null) { _item.SetPos = new Vector2(1533.666f, -295.9998f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("item9"); if (_item != null) { _item.SetPos = new Vector2(1530.889f, -490.778f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(-816.6669f, 122.2223f);

            EzText _t;
            _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-800.0029f, 863.8889f); _t.Scale = 1.3f; }
            MyPile.Pos = new Vector2(0f, 0f);
        }
    }
}