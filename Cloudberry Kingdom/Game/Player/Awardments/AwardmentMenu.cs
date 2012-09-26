using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom.Awards
{
    public class AwardmentMenu : StartMenuBase
    {
        EzText Description;
        QuadClass Hat;

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MyText.Shadow = item.MySelectedText.Shadow = false;
        }

        protected void SetHeaderProperties2(EzText text)
        {
            base.SetHeaderProperties(text);
            text.Shadow = false;
            text.MyFloatColor = CoreMath.Gray(.923f);
        }

        public override void Init()
        {
            base.Init();

            CategoryDelays();
        }

#if PC_VERSION
        int ButtonSize = 89;
#else
        int ButtonSize = 110;
#endif

        void MakeBack()
        {
            MenuItem item;

            // Stats
            ItemPos.Y += 70;
            item = new MenuItem(new EzText(ButtonString.Go(ButtonSize) + " Stats", ItemFont));
            item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedNextColor;
            item.Go = _item => Stats();
            MyMenu.OnA = m => Stats();
            ItemPos = new Vector2(1357.7f, 991.4287f);
            //AddItem(item);
            ItemPos.Y -= 83;
            //item.ScaleText(1.65f);
            item.ScaleText(.5f);
            item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedNextColor;
            item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedNextColor;

            item.UnaffectedByScroll = true;

            // Back
            //ItemPos = new Vector2(-1257.38f, -3781.123f);
            //item = MakeBackButton();
            //item.ScaleText(1.4f);

            ItemPos = new Vector2(1357.7f, 951.4287f);
            item = MakeBackButton();
            item.UnaffectedByScroll = true;
            item.ScaleText(.5f);
        }

        public AwardmentMenu()
        {
            MyPile = new DrawPile();

            // Header
            //EzText Header = new EzText("Awardments!", Tools.Font_Grobold42_2);
            //MyPile.Add(Header);
            //Header.Pos =
            //    new Vector2(-1295.316f, 725.3176f) + new Vector2(-297.6191f, 15.87299f);
            //SetHeaderProperties(Header);
            //Header.Scale *= 1.15f;

            // Make the menu
            ItemPos = new Vector2(-1305, 620);
            PosAdd = new Vector2(0, -151) * 1.2f * 1.1f;

            MyMenu = new LongMenu();
            MyMenu.FixedToCamera = false;
            MyMenu.WrapSelect = false;

            MyMenu.Control = -1;

            MyMenu.OnB = MenuReturnToCaller;

            // Header
            MenuItem Header = new MenuItem(new EzText("Awardments!", Tools.Font_Grobold42_2));
            MyMenu.Add(Header);
            Header.Pos =
                new Vector2(-1608.809f, 951.508f);
            SetHeaderProperties(Header.MyText);
            Header.MyText.Scale *= 1.15f;
            Header.Selectable = false;


            MakeBack();
            ItemPos = new Vector2(-1305, 620);
            MenuItem item;

            // Stats
            item = MakeStats();

            // Format the list of Awardments into a menu
            foreach (Awardment award in Awardments.Awards)
            {
                if (award.Unlockable == null) continue;

                // Add individual Awardments
                item = new MenuItem(new EzText(award.Name, ItemFont));

                if (PlayerManager.Awarded(award))
                    item.SetIcon(ObjectIcon.CheckIcon.Clone());
                else
                    item.SetIcon(ObjectIcon.UncheckedIcon.Clone());
                item.Icon.Pos = new Vector2(-67.46094f, -95.23816f);
                AddItem(item);

                Awardment ThisAward = award;
                item.AdditionalOnSelect += () =>
                    {
                        SetDescription(ThisAward.Description);
                        SetHat(ThisAward.Unlockable);

                        // Freeplay header
                        if (FreeplayText != null)
                        {
                            FreeplayText.Show = false;
                            if (ThisAward == Awardments.HoldForwardFreeplay ||
                                ThisAward == Awardments.NoCoinFreeplay)
                                FreeplayText.Show = true;
                        }

                        // Progress
                        SetProgressText(ThisAward);
                    };

                // Space before header
                ItemPos.Y -= 60;
            }

            //MakeBack();
            


            // Select first selectable item
            MyMenu.SelectItem(2);

            // Backdrop
            QuadClass backdrop;
            
            //backdrop = new QuadClass("Backplate_1500x900", 1500, true);
            //MyPile.Add(backdrop);
            //backdrop.Pos = new Vector2(3009.921265f, -111.1109f) + new Vector2(-297.6191f, 15.87299f);

            backdrop = new QuadClass("score screen", 1500, true);
            MyPile.Add(backdrop);
            MyPile.Add(backdrop);
            backdrop.Size = new Vector2(853.1744f, 1973.215f);
            backdrop.Pos = new Vector2(869.0458f, -35.71438f);

            backdrop = new QuadClass("score screen", 1500, true);
            MyPile.Add(backdrop);
            MyPile.Add(backdrop);
            backdrop.Size = new Vector2(853.1744f, 1973.215f);
            backdrop.Pos = new Vector2(-825.3976f, -71.42863f);

            EnsureFancy();
            MyMenu.Pos = new Vector2(67.45706f, 0f);
            MyPile.Pos = new Vector2(83.33417f, 130.9524f);


            FreeplayText = new EzText("Freeplay:", Tools.Font_Grobold42);
            FreeplayText.Scale *= .5f;
            MyPile.Add(FreeplayText);
            CampaignMenu.AbusiveColor(FreeplayText);
            FreeplayText.Pos = new Vector2(142.8594f, 817.4604f);

            ProgressText = null;
        }

        void Clear()
        {
            SetDescription(null);
            SetHat(null);

            // Freeplay header
            if (FreeplayText != null)
                FreeplayText.Show = false;

            // Progress
            SetProgressText(null);
        }

        private MenuItem MakeStats()
        {
            MenuItem item;
            item = new MenuItem(new EzText(ButtonString.Go(ButtonSize) + " Stats", ItemFont));
            item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedNextColor;
            item.Go = _item => Stats();
            MyMenu.OnA = m => Stats();
            AddItem(item);
            ItemPos.Y -= 83;
            //item.ScaleText(1.65f);
            //item.ScaleText(.5f);
            item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedNextColor;
            item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedNextColor;
            item.AdditionalOnSelect = Clear;
            return item;
        }

        EzText FreeplayText = null, ProgressText = null;

        void ProgressTextProperties(EzText text)
        {
            text.Scale = .5f;
            MyPile.Add(text);
        }
        
        void SetProgressText(Awardment ThisAward)
        {
            if (ProgressText != null)
            {
                MyPile.MyTextList.Remove(ProgressText);
                ProgressText = null;
            }

            if (ThisAward == null) return;

            Vector2 pos = new Vector2(158.7344f, -811.1111f);

            // Jumps
            if (ThisAward == Awardments.JumpAlot)
            {
                if (MaxJumps <= Awardments.LotsOfJumps)
                    ProgressText = new EzText("Progress:{c26,188,241,255}" + MaxJumps + " / " + Awardments.LotsOfJumps, Tools.Font_Grobold42);
                else
                    ProgressText = new EzText("Jumps:{c26,188,241,255}" + MaxJumps, Tools.Font_Grobold42);
            }

            // Hero Rush
            if (ThisAward == Awardments.HeroRush_Score)
                ProgressText = new EzText("High Score:{c26,188,241,255}" + SaveGroup.HeroRushHighScore.Top, Tools.Font_Grobold42);

            // Hero Rush 2
            if (ThisAward == Awardments.HeroRush2_Score)
                ProgressText = new EzText("High Score:{c26,188,241,255}" + SaveGroup.HeroRush2HighScore.Top, Tools.Font_Grobold42);

            // Escalation
            if (ThisAward == Awardments.Escalation_Levels)
                ProgressText = new EzText("Highest Level:{c26,188,241,255}" + SaveGroup.EscalationHighLevel.Top, Tools.Font_Grobold42);

            // Fast abusive castle
            if (ThisAward == Awardments.FastCampaign2)
            {
                var hsc = SaveGroup.CampaignScores[2].Time;
                if (hsc.Scores.Count > 0 && hsc.Top > 0 && hsc.Top < 62 * 60 * 10000)
                    ProgressText = new EzText("Best Time:{c26,188,241,255}" + SaveGroup.CampaignScores[2].Time.Scores[0]._ToString(ScoreEntry.Format.Time), Tools.Font_Grobold42);
            }

            if (ProgressText != null)
            {
                ProgressTextProperties(ProgressText);
                ProgressText.Pos = pos;
            }
        }

        int MaxJumps { get { return PlayerManager.PlayerMax(p => p.LifetimeStats.Jumps); } }

        public override void OnAdd()
        {
            SlideInFrom = PresetPos.Right;
            SlideOutTo = PresetPos.Right;

            base.OnAdd();

            // Scroll bar
#if PC_VERSION
            {
                ScrollBar bar = new ScrollBar((LongMenu)MyMenu, this);
                bar.BarPos = new Vector2(-2384.921f, 135);
                MyGame.AddGameObject(bar);
                MyMenu.AdditionalCheckForOutsideClick += () => bar.MyMenu.HitTest();
            }
#endif
        }

        public static QuadClass MakeHatQuad(Hat hat)
        {
            QuadClass HatQuad = new QuadClass(hat.GetTexture(), 330, true);
            HatQuad.Pos = hat.HatPicShift * HatQuad.Size;
            HatQuad.Size *= hat.HatPicScale;

            return HatQuad;
        }

        void SetHat(Hat hat)
        {
            // Destroy previous hat quad
            if (Hat != null)
            {
                MyPile.MyQuadList.Remove(Hat);
                Hat.Release();
            }

            if (hat == null) return;

            // Make a new quad
            Hat = MakeHatQuad(hat);
            Hat.Pos += new Vector2(849, -690);
            MyPile.Add(Hat);

            // Position the hat
            Description.CalcBounds();
            Hat.PosY = .35f * Hat.Pos.Y + .65f * (Description.BL.Y + 240);
            if (Hat.Pos.Y < -750)
                Hat.PosY = -750;
        }

        void SetDescription(string text)
        {
            if (Description != null)
            {
                MyPile.MyTextList.Remove(Description);
                Description.Release();
            }

            if (text == null) return;

            Description = new EzText(text, Tools.Font_Grobold42, 885, false, false, .63f);
            MyPile.Add(Description);
            Description.Pos =
                new Vector2(160, 703.0954f);
                //new Vector2(191.2696f, 703.0954f);
            SetHeaderProperties2(Description);
            Description.Scale *= .96f;// 1.12f;
        }

        bool Stats()
        {
            Hide();
            Call(new StatsMenu(StatGroup.Lifetime));
            return true;
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();
        }
    }
}