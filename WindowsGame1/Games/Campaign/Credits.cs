using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Coins;
using Drawing;

namespace CloudberryKingdom
{
    public class EndText : GUI_Panel
    {
        public EndText() { }

        public override void OnAdd()
        {
            base.OnAdd();
            MyGame.FadeIn(.03f);

            MyPile = new DrawPile();
            EnsureFancy();

            MyPile.FancyPos.UpdateWithGame = true;

            // Black backdrop
            QuadClass Backdrop = new QuadClass();
            Backdrop.FullScreen(MyGame.Cam); Backdrop.Pos = Vector2.Zero;
            Backdrop.Quad.SetColor(Color.Black);
            MyPile.Add(Backdrop);

            // Centered text
            EzText MyText = new EzText("For the real ending,\nbeat the campaign on", Tools.Font_DylanThin42, 1500, true, true, .65f);
            CampaignMenu.HappyBlueColor(MyText);
            MyText.Pos += new Vector2(0, 150);
            MyPile.Add(MyText);

            EzText HText = new EzText("HARDCORE", Tools.Font_DylanThin42, 1500, true, true);
            HText.Pos += new Vector2(0, -350);
            CampaignMenu.HardcoreColor(HText);
            MyPile.Add(HText);

            //MyText.Alpha = 0;
            //MyGame.AddToDo(() =>
            //{
            //    if (MyText.Alpha < 1) { MyText.Alpha += .03125f; return false; }
            //    else return true;
            //});

            // Show for a period and remove
            //MyGame.WaitThenDo(143, () => MyGame.FadeToBlack());
            MyGame.WaitThenDo(165, () => MyGame.FadeToBlack(.009f));
            MyGame.WaitThenDo(300, () => { MyGame.EndGame(false); Release(); });
        }

        protected override void MyDraw()
        {
            if (MyGame == null) return;

            //MyPile.FancyPos.SetCenter(MyGame.Cam);
            base.MyDraw();
        }
    }

    public class BerryScore : StartMenuBase
    {
        public BerryScore(bool CallBaseConstructor) : base(CallBaseConstructor) { }

        public BerryScore(GameData game)
            : base(false)
        {
            MyGame = game;
            FontScale = .6f;

            Constructor();
        }

        public void Kill()
        {
            MyPile.BubbleDownAndFade(true);
            ReleaseWhenDone = false;
            ReleaseWhenDoneScaling = true;
        }

        public override void Show()
        {
            SlideLength = 0;
            base.Show();
            MyPile.BubbleUp(true);
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            GreenItem(item);
        }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.Shadow = false;
        }

        public override void Init()
        {
            base.Init();

            MyPile = new DrawPile();

            QuadClass Backdrop = new QuadClass("menupic_bg_cloud", 800, true);
            MyPile.Add(Backdrop);
            Backdrop.Size = new Vector2(1390.278f, 1056.363f);
            Backdrop.Pos = new Vector2(0, 0);

            QuadClass Berry;
            Berry = new QuadClass("cb_naked"); MyPile.Add(Berry); Berry.Pos = new Vector2(-15.2771f, -83.33368f);
            Berry = new QuadClass("cb_naked"); MyPile.Add(Berry); Berry.Pos = new Vector2(-515.2776f, -215.278f);
            Berry = new QuadClass("cb_naked"); MyPile.Add(Berry); Berry.Pos = new Vector2(588.8889f, -263.8891f);

            EzText Text = new EzText("Berries freed:", Tools.Font_DylanThin42, true, true);
            CampaignMenu.EasyColor(Text);
            MyPile.Add(Text);
            Text.Pos = new Vector2(-209.7227f, 351.3889f);

            // Berry score
            int Berries = PlayerManager.PlayerSum(p => p.GetStats(StatGroup.Level).Berries);
            Text = new EzText(Berries.ToString(), Tools.Font_DylanThin42, true, true);
            MyPile.Add(Text);
            Text.Pos = new Vector2(868.0557f, 333.3334f);

            // Absorb stats
            PlayerManager.AbsorbTempStats();
            PlayerManager.AbsorbLevelStats();
            PlayerManager.AbsorbGameStats();
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        protected override void MyDraw()
        {
            if (Core.MyLevel.Replay || Core.MyLevel.Watching) return;

            Pos.SetCenter(Core.MyLevel.MainCamera, true);
            Pos.Update();

            base.MyDraw();
        }

        protected override void MyPhsxStep()
        {
            Level level = Core.MyLevel;
            base.MyPhsxStep();
        }
    }

    public class Campaign_CreditsLevel : WorldMap
    {
        public static bool WatchedIntro;

        PrincessBubble princess;
        public Campaign_CreditsLevel()
            : base(false)
        {
            SaveGroup.SaveAll();

            Data = Campaign.Data;
            WorldName = "CreditsLevel";

            Init("Doom\\ExitLevel.lvl");
            MyLevel.PreventReset = true;

            MakeCenteredCamZone(.8f);
            Cam.MyZone.End.X += 100000;
            Cam.MyPhsxType = Camera.PhsxType.SideLevel_Right;
            MakeBackground(BackgroundType.Outside);

            // Players
            SetHeroType(BobPhsxDouble.Instance);
            MakePlayers();

            EnterFromAndClose(FindDoor("Enter"), 68);

            // Start the music
            WaitThenDo(100, () =>
            {
                Tools.PlayHappyMusic();
            });

            // Princess
            Bob mvp = MvpBob;

            princess = new PrincessBubble(Vector2.Zero);
            if (Campaign.Index >= 3) princess.MyAnimation = PrincessBubble.DeadAnim;
            MyLevel.AddObject(princess);
            princess.ShowWithMyBob = true;
            princess.PickUp(mvp);

            // Blocks
            foreach (BlockBase block in MyLevel.Blocks)
            {
                // Make top only
                NormalBlock nblock = block as NormalBlock;
                if (null != nblock)
                {
                    nblock.BlockCore.UseTopOnlyTexture = false;
                    nblock.Box.TopOnly = true;
                    nblock.ResetPieces();
                }

                // Extend grass
                if (block.Core.MyTileSet == TileSets.OutsideGrass)
                {
                    block.Extend(Side.Left, -10000);
                    block.Extend(Side.Right, 90000);
                }
            }

            AddBubbles();
            //AddCoins();

            // Menu
            AddGameObject(InGameStartMenu_Campaign.MakeListener());
        }

        void AddBubbles()
        {
            Vector2 p1 = Doors["Enter"].Pos;
            Vector2 p2 = p1 + new Vector2(38000, 0);

            Vector2 pos = p1;
            while (pos.X < p2.X)
            {
                var bubble = new BerryBubble(pos + new Vector2(0, MyLevel.Rnd.RndFloat(-200, 1500)));
                bubble.Core.DrawLayer = 9;
                MyLevel.AddObject(bubble);

                pos.X += MyLevel.Rnd.RndFloat(300, 600);
            }
        }

        void AddCoins()
        {
            Vector2 p1 = Doors["Enter"].Pos;
            Vector2 p2 = p1 + new Vector2(30000, 0);

            Vector2 pos = p1;
            while (pos.X < p2.X)
            {
                Coin NewCoin = (Coin)Recycle.GetObject(ObjectType.Coin, false);

                Tools.MoveTo(NewCoin, pos + new Vector2(0, MyLevel.Rnd.RndFloat(300, 1500)));
                NewCoin.Core.DrawLayer = 9;
                MyLevel.AddObject(NewCoin);
                //NewCoin.SetType(Coin.CoinType.Red);

                pos.X += MyLevel.Rnd.RndFloat(150, 300) / 2;
            }
        }

        int Num = 0;
        void Credit(string NameStr, string JobStr) { Credit(NameStr, JobStr, false, false, Vector2.Zero); }
        void Credit(string NameStr, string JobStr, bool Swapped, bool Long, Vector2 Shift)
        {
            Num++;

            GUI_Text Name, Job;

            float vel = 1.8f;

            Name = new GUI_Text(NameStr, new Vector2(-780, 600));
            Name.NoPosMod = false;
            Name.FixedToCamera = true;
            Name.Core.Data.Velocity = new Vector2(vel, 0);
            if (Swapped)
                CampaignMenu.RegularColor(Name.MyText);
            else
                CampaignMenu.HappyBlueColor(Name.MyText);
            Name.MyText.Scale *= .9f;
            AddGameObject(Name);

            //Job = new GUI_Text(JobStr, new Vector2(400, -600));
            Job = new GUI_Text(JobStr, new Vector2(-280, 300));
            Job.NoPosMod = false;
            Job.FixedToCamera = true;
            Job.Core.Data.Velocity = new Vector2(vel, 0);
            if (Swapped)
                CampaignMenu.HappyBlueColor(Job.MyText);
            else
                CampaignMenu.RegularColor(Job.MyText);
            Job.MyText.Scale *= .75f;
            AddGameObject(Job);

            if (Long)
                Job.Shift(new Vector2(0, -Job.MyText.GetWorldHeight() / 2));

            Job.Shift(Shift);
            Name.Shift(Shift);

            WaitThenDo(CreditLength, () => {
                Job.Kill(true);
                WaitThenDo(15, () => {
                    Name.Kill(true);
                });
            });
        }

        int Step = 0;
        int FirstCredit = 190;
        int CreditLength = 165;
        int Space = 40;
        public override void PhsxStep()
        {
            base.PhsxStep();

            Step++;

            Num = 0;
            if (Step == FirstCredit)
                Credit("  Jordan Ezra Fisher", "Architect,\n        Sweet stickman graphics", false, true, new Vector2(145, 0));
			Num++;

            if (Step == Num * (CreditLength + Space) + FirstCredit)
                Credit("TJ Lutz", "Team lead");
            Num++;

            if (Step == Num * (CreditLength + Space) + FirstCredit)
                Credit("Anders Larsson", "Sexy Swedish Artist");
            Num++;

            if (Step == Num * (CreditLength + Space) + FirstCredit)
                Credit("Giacomo Tappainer", "Additional Art", false, false, new Vector2(40, 0));
            Num++;

            if (Step == Num * (CreditLength + Space) + FirstCredit)
                Credit("Blind Digital,\n                Peacemaker (Kari Sigurdsson)", "Musical score", false, false, new Vector2(-20, -40));
            Num++;

            if (Step == Num * (CreditLength + Space) + FirstCredit)
                Credit("James Stant", "Cloudberry Theme");
            Num++;

            if (Step == Num * (CreditLength + Space) + FirstCredit)
                Credit("Scott Thompson,\n     Catherine Arthur", "Sound effects");
            Num++;

            if (Step == Num * (CreditLength + Space) + FirstCredit)
                Credit("Tommy Engdahl\n", "Quality Assurance");
            Num++;

            if (Step == Num * (CreditLength + Space) + FirstCredit)
                Credit("Charles Martin\nRobert Sulway\nJene Fisher (momma bear)\n", "Special thanks", true, true, new Vector2(290, -650));
            //Credit("Special thanks", "Charles Martin\nRobert Sulway\nJene Fisher (momma bear)\n", true, true, new Vector2(250, -650));
            Num++;

            if (Step == Num * (CreditLength + Space) + FirstCredit)
                Credit("Dave Valdman", "Super special thanks", true, true, new Vector2(0, -350));
            Num++;

            // Fade
            if (Step == Num * (CreditLength + Space) + FirstCredit + 73)
            {
                //FadeToBlack(.009f);
                //WaitThenDo(140, () => EndGame(false));

                // Kill players
                if (princess != null)
                {
                    Fireball.Explosion(princess.Pos, MyLevel, Vector2.Zero, 3.95f, 2.3f);
                    princess.CollectSelf();
                    princess.Core.Show = false;
                    Fireball.ExplodeSound.Play();
                }
                WaitThenDo(50, () =>
                    AddGameObject(new ExplodeBobs(ExplodeBobs.Speed.Regular)));

                // Kill remaining berries
                WaitThenDo(170, () =>
                {
                    Tools.Pop(2);
                    WaitThenDo(10, () => Tools.Pop(3));
                    
                    foreach (ObjectBase obj in MyLevel.Objects)
                    {
                        BerryBubble b = obj as BerryBubble;
                        if (null != b)
                            b.Die();
                    }
                });


                // Show berry score, fade to black
                WaitThenDo(245, () =>
                {
                    var berryscore = new BerryScore(this);
                    AddGameObject(berryscore);
                    WaitThenDo(140, () =>
                    {
                        berryscore.Kill();
                        WaitThenDo(45, () => FadeToBlack(.009f));

                        // Show message if not hardcore or harder
                        if (Campaign.Index <= 2)
                            WaitThenDo(140, () => AddGameObject(new EndText()));
                        else
                            WaitThenDo(230, () => EndGame(false));
                    });
                });
            }
        }

        public override void AdditionalReset()
        {
            base.AdditionalReset();

            Step = 0;
        }
    }
}