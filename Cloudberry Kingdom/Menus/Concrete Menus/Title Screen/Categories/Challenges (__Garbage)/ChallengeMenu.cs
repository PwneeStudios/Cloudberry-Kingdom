using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    /*
    public class ChallengeMenu : StartMenuBase
    {
        protected override void AddItem(MenuItem item)
        {
            base.AddItem(item);

            item.MyText.Scale = item.MySelectedText.Scale *= 0.875f;//1.228f;
        }

        public ChallengeMenu()
        {
            ItemPos = new Vector2(-1305, 620);
            PosAdd = new Vector2(0, -151) * 1.2f * 1.1f;

            //RightPanel = new BlurbBerry();


            MyMenu = new LongMenu();
            MyMenu.FixedToCamera = false;
            MyMenu.WrapSelect = false;

            MyMenu.Control = -1;

            MyMenu.OnB = MenuReturnToCaller;


            // Format the list of challenges into a menu
            ChallengeMenuItem item;
            foreach (ChallengeGroup group in ChallengeList.Groups)
            {
                // Header
                MenuItem header;
                if (ChallengeList.Groups.IndexOf(group) == 0)
                {
                    // CHALLENGES title
                    EzText ChallengeText = new EzText("Challenges!", Tools.Font_Grobold42);

                    header = new MenuItem(ChallengeText);
                    AddItem(header);

                    header.MyText.Scale = header.MySelectedText.Scale *= 1.275f;

                    CharacterSelectManager.ChooseHeroTextStyle(ChallengeText);

                    header.Pos += new Vector2(-134, 35);
                }
                else
                {
                    header = new MenuItem(new EzText(group.Name, ItemFont));
                    AddItem(header);
                    header.MyText.Scale *= 1.175f;
                    header.Pos.X -= 70;
                    ItemPos.Y -= 30;
                }
                header.Selectable = false;

                foreach (Challenge challenge in group.Challenges)
                {
                    // Add individual challenges
                    item = new ChallengeMenuItem(new EzText(challenge.Name, ItemFont));
                    item.MyChallenge = challenge;

                    item.RightEdge = 610;
                    
                    AddItem(item);
                    item.AdditionalOnSelect = () => { if (RightPanel != null) ((BlurbBerry)RightPanel).SetText(challenge.Name); };

                    if (challenge.ID != Guid.Empty)
                        item.Go = menuitem =>
                            {
                                ChallengeMenuItem challengemenuitem = menuitem as ChallengeMenuItem;

                                Hide();
                                
                                DifficultyMenu diffmenu = new DifficultyMenu(4);
                                diffmenu.StartFunc = (diff, menuindex) => challengemenuitem.MyChallenge.Start(diff);
                                diffmenu.ReturnFunc = () =>
                                    {
                                        challengemenuitem.MyChallenge.Aftermath();
                                        challengemenuitem.CalcStars();
                                    };
                                                
                                Call(diffmenu);
                            };

                    // Set stars to the maximum from all current players
                    item.CalcStars();
                };

                // Space before header
                ItemPos.Y -= 60;
            }

            MyMenu.SelectItem(1);
           
            // Backdrop
            MakeBackdrop(new Vector2(815, 750), new Vector2(-1580, this.ItemPos.Y - 200));
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            MyMenu.FancyPos.RelVal.X = 350;
        }
    }*/
}