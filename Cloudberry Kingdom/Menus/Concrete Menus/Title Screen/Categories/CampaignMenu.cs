using Microsoft.Xna.Framework;
using CloudberryKingdom.Levels;
using System.Collections.Generic;
using System.Linq;

using Drawing;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public class CampaignMenu : StartMenuBase
    {
        public static string GetName(int difficulty)
        {
            //return DifficultyNames[difficulty].ToLower();

            return EzText.ColorToMarkup(DifficultyColor[difficulty]) +
                DifficultyNames[difficulty].ToLower() + 
                EzText.ColorToMarkup(Color.White);
        }
        public static Color[] DifficultyColor = { new Color(44, 44, 44), new Color(144, 200, 225), new Color(44, 203, 48), new Color(248, 136, 8), new Color(90, 90, 90), new Color(0, 255, 255) };
        public static string[] DifficultyNames = { "Custom", "Training", "Unpleasant", "Abusive", "Hardcore", "Masochistic" };

        protected override void ReleaseBody()
        {
                base.ReleaseBody();

            Castles.ForEach(castle => castle.Release()); Castles = null;
            Names = null;
            grass.Release(); grass = null;
        }

        public ObjectGroup CurCastle { get { return Castles[CastleIndex]; } }

        QuadClass Locked;
        EzText PressA, PressY;

        public ObjectGroup grass;
        int CastleIndex = 1;
        List<ObjectGroup> Castles = new List<ObjectGroup>();
        List<EzText> Names = new List<EzText>();

        void MakeCastle()
        {
            //FancyVector2 ObjCenter = Pos;
            FancyVector2 ObjCenter = Tools.CurGameData.Cam.FancyPos;

            Castles.Add(new ObjectGroup("CampaignCastles\\SmallCastle.lvl", ObjCenter,
                new Vector2(-59.52389f, -1088.095f), new Vector2(600)));
            Castles.Add(new ObjectGroup("CampaignCastles\\NormalCastle.lvl", ObjCenter, new Vector2(31.74609f, -1011.905f)));
            Castles.Add(new ObjectGroup("CampaignCastles\\BigCastle.lvl", ObjCenter, new Vector2(31.74609f, -1011.905f)));
            Castles.Add(new ObjectGroup("CampaignCastles\\BigBigCastle.lvl", ObjCenter, new Vector2(31.74609f, -1011.905f)));
            Castles.Add(new ObjectGroup("CampaignCastles\\MasoCastle.lvl", ObjCenter, new Vector2(75.39709f, 507.9367f)));

            foreach (ObjectGroup group in Castles)
            {
                group.lvl.HideAll("grass");
                group.Pos += new Vector2(702.3814f, -7.936523f)
                             + new Vector2(120, -36)
                             + new Vector2(-146f, -16f);
            }

            grass = new ObjectGroup("CampaignCastles\\Grass.lvl", ObjCenter);
            grass.Pos = new Vector2(1687.143f, -1340.635f);
            grass.Size = new Vector2(797.6187f, 797.6187f);
        }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.MyFloatColor = new Vector4(1, 1, 1, 1);
            text.Scale = FontScale * 1.46f;
            text.ShadowOffset = new Vector2(17);
            text.Shadow = false;
        }

#if PC_VERSION
        protected override void SetItemProperties(MenuItem item)
        {
            item.ScaleText(.9f);
            //base.SetItemProperties(item);
        }
#endif
        public override void Call(GUI_Panel child, int Delay)
        {
            base.Call(child, Delay);

            Hide();
        }

        bool SlidInOnce = false;
        Vector2 castlepos, grasspos;
        public override void SlideIn(int Frames)
        {
            TitleGameData title = Tools.WorldMap as TitleGameData;
            title.PanCamera = false;
            
            MyGame.WaitThenDo(30, () =>
                base.SlideIn(Frames));

            if (!SlidInOnce)
            {
                castlepos = CurCastle.FancyPos.RelVal;
                grasspos = grass.FancyPos.RelVal;
                SlidInOnce = true;
            }

            // Slam in the castle
            Vector2 pos = castlepos;
            CurCastle.FancyPos.RelVal = pos + new Vector2(0, 3600);
            int FallLength = 25;
            int WaitLength = 16;
            MyGame.WaitThenDo(WaitLength, () =>
                CurCastle.FancyPos.LerpTo(pos, FallLength, LerpStyle.Linear));
            MyGame.WaitThenDo(WaitLength + FallLength - 1, () =>
            {
                MyGame.Cam.StartShake(2.1f, 35, false);//28));
                Tools.Sound("Bash").Play();
            });

            // Raise the grass
            Vector2 gpos = grasspos;
            grass.FancyPos.RelVal = gpos + new Vector2(0, -800);
            MyGame.WaitThenDo(WaitLength - 10, () =>
                grass.FancyPos.LerpTo(gpos, 25, LerpStyle.Linear));

            // Activate the menu
            Active = false;
            MyGame.WaitThenDo(FallLength + WaitLength, () => Active = true);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, Frames);

            if (Frames > 0)
            {
                TitleGameData title = Tools.WorldMap as TitleGameData;
                title.PanCamera = true;
                //MyGame.Cam.FancyPos.LerpTo(new Vector2(MyGame.Cam.FancyPos.RelVal.X - 1500, title.Center.Y), SlideInLength); 

                Vector2 gpos = grass.FancyPos.RelVal;
                grass.FancyPos.LerpTo(gpos + new Vector2(0, -800), 25, LerpStyle.Linear);
                Vector2 pos = CurCastle.FancyPos.RelVal;
                CurCastle.FancyPos.LerpTo(pos + new Vector2(0, 3600), 30, LerpStyle.Linear);
            }
        }

        public CampaignMenu()
        {
            CallDelay = 20;
            ReturnToCallerDelay = 24;
            
            //DestinationScale *= .8f;
            SlideLength = 40;
            SlideInFrom = SlideOutTo = PresetPos.Left;


            MakeCastle();

            MyPile = new DrawPile();

#if PC_VERSION
            QuadClass backdrop = new QuadClass("score screen", 1500, true);
            //MyPile.Add(backdrop);
            backdrop.Size = new Vector2(539.6826f, 2655.755f);
            backdrop.Pos = new Vector2(-1281.745f, -1317.46f);
#endif
            
#if PC_VERSION
            PressA = new EzText("Select difficulty", Tools.Font_Grobold42);
#else
            PressA = new EzText("Press " + ButtonString.Go(100) + " to select", Tools.Font_Grobold42);
#endif
            PressA.Scale *= .9f;
            PressA.Pos = new Vector2(-1686.508f, 805.5554f);
            MyPile.Add(PressA);

            PressY = new EzText("Press " + ButtonString.Y(100) + " for high scores", Tools.Font_Grobold42);
            PressY.Scale *= .5f;
#if PC_VERSION
            PressY.Pos = new Vector2(-1837.301f, 817.4599f);
#else
            PressY.Pos = new Vector2(-1623.015f, 551.5872f);
#endif
            MyPile.Add(PressY);

            Locked = new QuadClass("locked", 300, true);
#if PC_VERSION
            Locked.Pos = 
                new Vector2(-281.746f, 588.8888f);
                //new Vector2(0, 200);
            Locked.Size =
                new Vector2(596.8036f, 257.9439f);
                //new Vector2(614.1063f, 240.837f) * 1.45f;
#else
            Locked.Pos = new Vector2(-972.2217f, 595.2379f);
            Locked.Size = new Vector2(614.1063f, 240.837f);
#endif
            Locked.FancyAngle.RelVal = new Vector2(0.07936529f, 0);
            MyPile.Add(Locked);


            EzText name;
            Vector2 pos =
                new Vector2(-1680.238f, -16.74597f); // Left
                //new Vector2(-1652.46f, -830.2379f); // BL
                //new Vector2(-1751.666f, 903.8889f); // TL

#if PC_VERSION
            bool Centered = false;
#else
            bool Centered = false;
#endif

            name = new EzText(DifficultyNames[1].ToLower(), Tools.Font_Grobold42, Centered, true);
            SetHeaderProperties(name);
            _x_x_EasyColor(name);
            name.Pos = pos;
            Names.Add(name);

            name = new EzText(DifficultyNames[2].ToLower(), Tools.Font_Grobold42, Centered, true);
            SetHeaderProperties(name);
            UnpleasantColor(name);
            name.Pos = pos;
            Names.Add(name);

            name = new EzText(DifficultyNames[3].ToLower(), Tools.Font_Grobold42, Centered, true);
            SetHeaderProperties(name);
            AbusiveColor(name);
            name.Pos = pos;
            Names.Add(name);

            name = new EzText(DifficultyNames[4].ToLower(), Tools.Font_Grobold42, Centered, true);
            SetHeaderProperties(name);
            _x_x_HardcoreColor(name);
            name.Pos = pos;
            Names.Add(name);

            name = new EzText(DifficultyNames[5].ToLower(), Tools.Font_Grobold42, Centered, true);
            SetHeaderProperties(name);
            _x_x_MasochisticColor(name);
            name.Pos = pos;
            Names.Add(name);

            //foreach (EzText _name in Names) MyPile.Add(_name);

            MakeMenu();

            EnsureFancy();
#if PC_VERSION
            if (MyPile != null) MyMenu.FancyPos.RelVal = new Vector2(-472.2224f, 15.87305f);
#else
            if (MyPile != null) MyMenu.FancyPos.RelVal = new Vector2(-234.1274f, -63.49194f);
#endif
            MyPile.Pos = new Vector2(83.33417f, 130.9524f);
        }

        public override void OnAdd()
        {
            base.OnAdd();

            ClosingCenter = new FancyVector2(MyGame.Cam);
            ClosingCenter.RelVal = new Vector2(583.333f, -910.6347f);
        }

        public FancyVector2 ClosingCenter;
        bool Start(Menu menu)
        {
            if (IsLocked) return true;

            Active = false;

            // Slide out
            MyGame.WaitThenDo(8, () =>
                base.SlideOut(PresetPos.Left, 36));

            // Camera swipe (Closing circle)
            MyGame.WaitThenDo(33, () =>
            {
                MyGame.MyLevel.LightLayer = Level.LightLayers.FrontOfEverything;
                MyGame.MyLevel.MakeClosingCircle(120, ClosingCenter);

                // Kill the music
                MyGame.WaitThenDo(30, () => //100, () =>
                    Tools.SongWad.FadeOut());
            });

            MyGame.WaitThenDo(138, () => 
                {
                    // Todo for when the campaign exits
                    MyGame.AddToDo(() =>
                        {
                            GameData game = MyGame;

                            MyGame.Black();
                            MyGame.WaitThenDo(10, () =>
                                game.FadeIn(.033f));
                            MyGame.WaitThenDo(75, () =>
                                Tools.PlayHappyMusic());
                        
                            MyGame.MyLevel.UseLighting = false;
                            ReturnToCaller();
                            SlideOut(PresetPos.Left, 0);
                        });
                    MyGame.PhsxStepsToDo = 3;

                    //Tools.CurGameData = new Campaign_String();
                });

            return true;
        }

        bool IsLocked { get { return CastleIndex > LockManager.CampaignLock; } }

        void SetCastleIndex(int Index)
        {
            CastleIndex = Index;

            // Mark the castle as locked or unlocked
#if PC_VERSION
            PressA.Show = false;
#else
            PressA.Show = !IsLocked;
#endif
            Locked.Show = IsLocked;
            PressY.Show = !IsLocked;

            ShowHighScore();
        }

        EzText ScoreText;
        CampaignList CampaignScores { get { return SaveGroup.CampaignScores[CastleIndex]; } }
        public void ShowHighScore()
        {
            if (ScoreText != null) MyPile.Remove(ScoreText); ScoreText = null;

            if (IsLocked) return;

            int score = CampaignScores.Score.Top;

            if (score == 0)
            {
                PressY.Show = false;
                return;
            }

            string str = ScoreEntry.DottedScore("High Score", score, 23, 4);
            ScoreText = new EzText(str, Tools.Font_Grobold42, 1450, false, true, .6f);
            MyPile.Add(ScoreText);

            ScoreText.Scale = .86f;
            ScoreText.MyFloatColor = new Color(255, 255, 255).ToVector4();
            ScoreText.OutlineColor = new Color(0, 0, 0).ToVector4();

            ScoreText.Pos =
                new Vector2(-319.0487f, -974.1271f);
            ScoreText.Layer = 1;
        }

        void MakeMenu()
        {
            ItemPos = new Vector2(-1317, 600);
            PosAdd = new Vector2(0, -151) * 1.37f;

            MyMenu = new Menu(false);

            MyMenu.Control = -1;

            MyMenu.OnB = MenuReturnToCaller;
            MyMenu.OnA = Start;

#if PC_VERSION
            MyMenu.AlwaysSelected = true;
            MakePcMenu();
#else
            MakeXboxMenu();
#endif
        }

        void MakePcMenu()
        {
            // List
            foreach (EzText name in Names)
            {
                MenuItem item = new MenuItem(name, name);
                item.Go = _item => Start(MyMenu);
                AddItem(item);
            }
            MyMenu.OnSelect += () => SetCastleIndex(MyMenu.CurIndex);

            MyMenu.SelectItem(1);
        }

        void MakeXboxMenu()
        {
            // List
            MenuList list = new MenuList();
            foreach (EzText name in Names)
            {
                MenuItem item = new MenuItem(name, name);
                list.AddItem(item, name);
            }
            AddItem(list);
            list.OnIndexSelect = () =>
                SetCastleIndex(list.ListIndex);
            list.SetIndex(1);

            list.DoIndexWrapping = false;
            list.Pos = new Vector2(-650f, -1.365112f);
            list.CustomArrow = true;
            list.RightArrow.TextureName = "charmenu_rarrow_0";
            list.LeftArrow.TextureName = "charmenu_larrow_0";
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (ScoreText != null && Active)
                if (ButtonCheck.State(ControllerButtons.Y, MyMenu.Control).Pressed)
                    BringHighScoreMenu();
        }

        void BringHighScoreMenu()
        {
            //Call(new HighScorePanel(
            base.Call(new HighScorePanel(CampaignScores), 13);
        }

        protected override void MyDraw()
        {
            CurCastle.Draw(0, 5, true);
            grass.Draw();
            CurCastle.Draw(6, 100, false);

            base.MyDraw();
        }
    }
}