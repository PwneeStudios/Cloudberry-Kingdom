using Microsoft.Xna.Framework;
using CloudberryKingdom.Awards;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class ArcadeMenu : CategoryMenu
    {
        bool Long = false;

        public override void OnReturnTo()
        {
            base.OnReturnTo();
            SetLockColors();
        }

        void SetLockColors()
        {
            foreach (MenuItem item in MyMenu.Items)
            {
                Awardment award = item.MyObject as Awardment;
                if (null != award && !PlayerManager.Awarded(award))
                {
                    item.MyText.MyFloatColor = new Color(255, 100, 100).ToVector4();
                    item.MySelectedText.MyFloatColor = new Color(255, 160, 160).ToVector4();
                }
                else
                {
                    if (null != award)
                        SetItemProperties(item);
                }
            }
        }

        void MenuGo_LostLevels(MenuItem item)
        {
            Tools.BeginLoadingScreen(true);
            Active = false;

            MyGame.WaitThenDo(70, () =>
            {
                Tools.EndLoadingScreen();

                // Todo for when doom exits
                MyGame.AddToDo(() =>
                {
                    GameData game = MyGame;

                    SlideOut(PresetPos.Left, 0);
                    MyGame.WaitThenDo(12, () =>
                        SlideIn());
                        //Show());
                });
                MyGame.PhsxStepsToDo = 3;

                Campaign.InitCampaign(0);
                Tools.CurGameData = new Doom();
            });
        }

        public ArcadeMenu()
        {
            Campaign.InitCampaign(0);
            Campaign.IsPlaying = false;

            CallDelay = 20;
            
            SlideLength = 27;
            UseAdditionalSlideOutPos = true;
#if PC_VERSION
            DestinationScale *= 1.015f;
            ReturnToCallerDelay = 20;
            SlideInFrom = PresetPos.Left;
            SlideOutTo = PresetPos.Left;

            ReturnSlideOutTo = PresetPos.Right;
            OnAddSlideOutTo = PresetPos.Right;
#else
            ReturnToCallerDelay = 20;
            SlideInFrom = PresetPos.Left;
            SlideOutTo = PresetPos.Left;

            ReturnSlideOutTo = PresetPos.Right;
            OnAddSlideOutTo = PresetPos.Right;
#endif

            ItemPos = new Vector2(-1630, 572);
            PosAdd = new Vector2(0, -151) * 1.2f * 1.2f;

            RightPanel = new CategoryPics();

            MyPile = new DrawPile();

            // Menu
            if (Long)
            {
                MyMenu = new LongMenu();
                MyMenu.FixedToCamera = false;
                MyMenu.WrapSelect = false;
            }
            else
                MyMenu = new Menu(false);

            MyMenu.Control = -1;

            MyMenu.OnB = MenuReturnToCaller;


            // Header
            MenuItem Header = new MenuItem(new EzText("Arcade", Tools.Font_Dylan60));
            MyMenu.Add(Header);
            Header.Pos =
                new Vector2(-1834.998f, 999.1272f);
            SetHeaderProperties(Header.MyText);
            Header.MyText.Scale *= 1.15f;
            Header.Selectable = false;



            MenuItem item;
            ItemPos = new Vector2(-1689.523f, 520.4127f);

            //// Lost Levels
            //item = new MenuItem(new EzText("Lost Levels", ItemFont));
            //AddItem(item);
            //item.Go = MenuGo_LostLevels;
            //item.AdditionalOnSelect = () =>
            //        pics.Set("LostLevels", "", new Vector2(575.3966f, -444.4442f),
            //        true, new Vector2(924.6016f, -662.6984f));

            // Escalation
            item = AddChallenge(Challenge_Escalation.Instance, null, null);

            // Hero Rush
            item = AddChallenge(Challenge_HeroRush.Instance, null, Awardments.UnlockHeroRush2);

            // Hero Rush 2
            item = AddChallenge(Challenge_HeroRush2.Instance, Awardments.UnlockHeroRush2, null);

            //// Hero Rush
            //item = new MenuItem(new EzText("Hero Rush", ItemFont));
            //AddItem(item);
            //item.AdditionalOnSelect = () => pics.Set("menupic_herorush1", "", Vector2.Zero, false);
            //item.Go = menuitem =>
            //{
            //    DifficultyMenu levelmenu = new LevelMenu(false);
            //    levelmenu.ShowHighScore(SaveGroup.HeroRushHighScore.Top);
            //    levelmenu.MyMenu.SelectItem(Challenge_HeroRush.PreviousMenuIndex);

            //    levelmenu.StartFunc = (level, menuindex) =>
            //        {
            //            // Save the menu item index
            //            Challenge_HeroRush.PreviousMenuIndex = menuindex;

            //            // Start the game
            //            MyGame.PlayGame(() =>
            //                {
            //                    // Show Hero Rush title again if we're selecting from the menu
            //                    if (!MyGame.ExecutingPreviousLoadFunction)
            //                        HeroRush_Tutorial.ShowTitle = true;

            //                    Challenge_HeroRush.Instance.Start(level);
            //                });
            //        };

            //    levelmenu.ReturnFunc = () => { };

            //    Call(levelmenu);
            //};

            //// Hero Rush 2
            //item = new MenuItem(new EzText("Hero Rush 2", ItemFont));
            //AddItem(item);
            //EzText HeroRush2_Title = new EzText("Revenge of the\nDouble Jump", Tools.Font_DylanThin42, 800, true, true, .5f);
            //pics.MyPile.Add(HeroRush2_Title);
            //MyMenu.OnSelect += () => HeroRush2_Title.Show = false;
            //item.AdditionalOnSelect = () => {
            //    pics.Set("menupic_herorush2", "", Vector2.Zero,
            //        false);//, new Vector2(742.0625f, -738.0952f));
            //};

            //item.Go = menuitem =>
            //{
            //    DifficultyMenu levelmenu = new LevelMenu(false);
            //    levelmenu.ShowHighScore(SaveGroup.HeroRush2HighScore.Top);
            //    levelmenu.MyMenu.SelectItem(Challenge_HeroRush2.PreviousMenuIndex);

            //    levelmenu.StartFunc = (level, menuindex) =>
            //    {
            //        // Save the menu item index
            //        Challenge_HeroRush2.PreviousMenuIndex = menuindex;

            //        // Start the game
            //        MyGame.PlayGame(() =>
            //        {
            //            // Show Hero Rush title again if we're selecting from the menu
            //            if (!MyGame.ExecutingPreviousLoadFunction)
            //                HeroRush2_Tutorial.ShowTitle = true;

            //            Challenge_HeroRush2.Instance.Start(level);
            //        });
            //    };

            //    levelmenu.ReturnFunc = () => { };

            //    Call(levelmenu);
            //};

            //// Construct
            //item = AddEscalation(Challenge_Construct.Instance);

            //// Wheelie
            //item = AddChallenge(Challenge_Wheelie.Instance, Challenge_Escalation.Instance);

            //// Up up
            //item = AddChallenge(Challenge_UpUp.Instance, Challenge_Wheelie.Instance);


            // Back button
            item = MakeBackButton();
            item.AdditionalOnSelect = () => pics.Set(null, "", new Vector2(634.922f, -349.2057f), false, Vector2.Zero);

            // Backdrop
            QuadClass backdrop;
            
            backdrop = new QuadClass("WoodMenu_1", 1500);
            if (Long)
                backdrop.SizeY *= 1.02f;
            MyPile.Add(backdrop);
            backdrop.Pos = new Vector2(9.921265f, -111.1109f) + new Vector2(-297.6191f, 15.87299f);

            // Position
            EnsureFancy();
            MyMenu.Pos = new Vector2(332, -40f);
            MyPile.Pos = new Vector2(83.33417f, 130.9524f);

            MyMenu.SelectItem(1);

            SetLockColors();
        }

        EzText Goal;
        Vector2 GetGoalPos() { 
            //return new Vector2(75.23828f, -599.6826f);
            return new Vector2(-174.6031f, -603.1746f);
        }
        private MenuItem AddChallenge(Challenge challenge, Awardment prereq, Awardment goal)
        {
            MenuItem item;
            string name = challenge.MenuName != null ? challenge.MenuName : challenge.Name;
            item = new MenuItem(new EzText(name, ItemFont));
            item.MyObject = prereq;
            AddItem(item);
            item.AdditionalOnSelect = () =>
            {
                bool Locked = prereq != null && !PlayerManager.Awarded(prereq);

                string str = "";
                //if (goal != null && !PlayerManager.Awarded(goal))
                //    str = goal.Description;
                if (prereq != null && !PlayerManager.Awarded(prereq))
                    str = prereq.Description;
                pics.RemoveText();
                pics.Set(challenge.MenuPic, str, GetGoalPos(), Locked);

                //// Goal text
                //if (Goal != null) { MyPile.Remove(Goal); Goal = null; }
                //if (goal != null && !PlayerManager.Awarded(goal))
                //{
                //    Goal = new EzText(goal.Description, Tools.Font_DylanThin42, 10000, true, true);
                //    //MyPile.Add(Goal);
                //    Goal.Scale = 1.3f;
                //    Goal.Pos = new Vector2(500, -800);
                //}
            };

            item.Go = menuitem =>
            {
                bool Locked = prereq != null && !PlayerManager.Awarded(prereq);
                if (Locked) return;

                // If the goal has been met then allow difficulty selection
                //if (challenge.GetGoalMet())
                if (true)
                {
                    //DifficultyMenu levelmenu = new LevelMenu(true);
                    DifficultyMenu levelmenu = new LevelMenu(false, challenge.HighLevel.Top, challenge.StartLevels);
                    levelmenu.ShowHighScore(challenge.HighScore.Top);
                    levelmenu.MyMenu.SelectItem(Challenge_HeroRush.PreviousMenuIndex);

                    levelmenu.StartFunc = (level, menuindex) =>
                    {
                        // Save the menu item index
                        Challenge_HeroRush.PreviousMenuIndex = menuindex;

                        // Start the game
                        MyGame.PlayGame(() =>
                        {
                            // Show title again if we're selecting from the menu
                            if (!MyGame.ExecutingPreviousLoadFunction)
                                //Escalation_Tutorial.ShowTitle = true;
                                HeroRush_Tutorial.ShowTitle = true;

                            challenge.Start(level);
                        });
                    };

                    levelmenu.ReturnFunc = () => { };

                    Call(levelmenu);
                }
                // Otherwise select the easiest difficulty
                else
                {
                    // Start the game
                    MyGame.PlayGame(() =>
                    {
                        // Show title again if we're selecting from the menu
                        if (!MyGame.ExecutingPreviousLoadFunction)
                            //Escalation_Tutorial.ShowTitle = true;
                            HeroRush_Tutorial.ShowTitle = true;

                        challenge.Start(0);
                    });
                }
            };
            return item;
        }


        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();
        }
    }
}