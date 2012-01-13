using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class FreeplayMenu : StartMenuBase
    {
        protected override void AddItem(MenuItem item)
        {
            base.AddItem(item);

            item.MyText.Scale = item.MySelectedText.Scale *= 0.875f;//1.228f;
        }

        public FreeplayMenu()
        {
            ItemPos = new Vector2(-1305, 620);
            PosAdd = new Vector2(0, -151) * 1.2f * 1.1f;

            //RightPanel = new BlurbBerry();


            MyMenu = new LongMenu();
            MyMenu.FixedToCamera = false;
            MyMenu.WrapSelect = false;

            MyMenu.Control = -1;

            MyMenu.OnB = MenuReturnToCaller;

            // Header
            EzText HeaderText = new EzText("Freeplay!", Tools.Font_Dylan42);

            MenuItem header = new MenuItem(HeaderText);
            header.Selectable = false;
            AddItem(header);

            header.MyText.Scale = header.MySelectedText.Scale *= 1.275f;

            CharacterSelectManager.ChooseHeroTextStyle(HeaderText);

            header.Pos += new Vector2(-134, 35);


            // Format the list of Freeplay games into a menu
            MenuItem item;
            List<string> games = new List<string>(new string[] { "Arcade", "Time Crisis", "Custom" });

            foreach (string name in games)
            {
                // Add individual game name
                item = new MenuItem(new EzText(name, ItemFont));

                item.Go = null;
                AddItem(item);
                //item.OnSelect = () => { if (RightPanel != null) ((BlurbBerry)RightPanel).SetText(name); };

                int Index = games.IndexOf(name);
                if (Index != 2)
                item.Go = menuitem =>
                {
                    Hide();

                    DifficultyMenu diffmenu = new DifficultyMenu(4);
                    switch (Index)
                    {
                        case 0:
                            diffmenu.StartFunc = (diff, menuindex) => Challenge_Escalation.Instance.Start(diff);
                            break;

                        case 1:
                            diffmenu.StartFunc = (diff, menuindex) => Challenge_TimeCrisis.Instance.Start(diff);
                            break;
                    }

                    diffmenu.ReturnFunc = () => { };

                    Call(diffmenu);
                };
            }

            MyMenu.SelectItem(1);
           
            // Backdrop
            MakeBackdrop(new Vector2(815, 750), new Vector2(-1580, this.ItemPos.Y - 200));
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            MyMenu.FancyPos.RelValX = 350;
        }
    }
}