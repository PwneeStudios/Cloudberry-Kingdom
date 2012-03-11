using Microsoft.Xna.Framework;
using System;
using System.Text;
using System.Collections.Generic;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class ContinuousDiff : GUI_Panel
    {
        List<IObject> Objects = new List<IObject>();

        void GetObjects()
        {
            Objects = Rnd.Shuffle(MyGame.MyLevel.Objects.FindAll(obj => obj.Core.ContinuousEnabled));
        }

        public void EOL_DoorAction(Door door)
        {
            StatGroup group = door.Game.MyStatGroup;
            GameData game = door.Core.MyLevel.MyGame;

            // Close the door
            door.SetLock(true, false, true);

            // Ensure door isn't reused
            door.OnOpen = null;

            // Remove the timer
            Timer.SlideOut(GUI_Panel.PresetPos.Top);
            Timer.Active = false;

            // End this level
            door.Core.MyLevel.EndLevel();

            // Hide the player
            door.InteractingBob.Core.Show = false;

            /*
            // + Score xxxx
            game.WaitThenDo(8, () => {
                TextFloat text = new TextFloat("+ Score " + Score.ToString(), MyGame.Cam.Pos + new Vector2(0, -2000));
                text.MyText.MyFloatColor = MyGame.MyLevel.MyTileSetInfo.CoinScoreColor.ToVector4();
                text.MyText.Scale *= 6f;
                text.Core.Data.Velocity *= 5f;
                MyGame.AddGameObject(text);
            });
            */

            // Add the victory screen
            ExplodeBobs explode = new ExplodeBobs(ExplodeBobs.Speed.Fast);
            door.Core.MyLevel.MyGame.AddGameObject(explode);
            explode.OnDone = () =>
                game.WaitThenDo(16, () =>
                    game.AddGameObject(new BonusWin(Score)));
        }


        DiffSlider MySlider;
        GUI_Text PressA;
        bool Winding = true;
        public override void OnAdd()
        {
            base.OnAdd();

            Hide();

            // EOL door
            MyGame.MyLevel.FinalDoor.OnOpen = EOL_DoorAction;

            // Title
            MyGame.WaitThenDo_Pausable(43, () =>
                WorldMap.AddTitle(MyGame, "Bonus Challenge!", 110));

            // Start the music
            MyGame.WaitThenDo(20, () =>
            {
                Tools.SongWad.SuppressNextInfoDisplay = true;
                Tools.SongWad.SetPlayList(Tools.Song_140mph);
                Tools.SongWad.Restart(true);
            });

            // Slider
            MySlider = new DiffSlider();
            MyGame.AddGameObject(MySlider);

            // Press A text
            PressA = new GUI_Text("Press " + ButtonString.Go(88),
                                           new Vector2(0, -865), true);
            PressA.MyPile.Pos = new Vector2(1080.808f, -611.7802f);
            PressA.MyText.Scale *= .68f;
            PressA.PreventRelease = true;
            PressA.FixedToCamera = true;
            PressA.Oscillate = true;

            // Listen for (A)
            Listener PressA_Listener = null;
            PressA_Listener = new Listener(ControllerButtons.A, () =>
            {
                PressA_Listener.Release();

                Winding = false;
                MyGame.WaitThenDo(12, Start);
            });

            // Show the GUI
            SlideOut(PresetPos.Bottom);
            MySlider.SlideOut(PresetPos.Bottom);
            //MyGame.WaitThenDo(48, () => {
            MyGame.WaitThenDo(86, () =>
            {
                Show();
                SlideIn(38);
                MySlider.SlideIn(38);

                MyGame.WaitThenDo(35, () => {
                    MyGame.AddGameObject(PressA_Listener);
                });

                MyGame.WaitThenDo(55, () => {
                    MyGame.AddGameObject(PressA);
                    PressA.MyPile.BubbleUp(false);
                });
            });
        }

        void Start()
        {
            // Emphasize the selected score
            //MyPile.BubbleUp(true, 8, .6f);
            MyPile.Jiggle(true, 5, 1f);
            MyGame.WaitThenDo(47, () =>
            {
                // Hide the score
                SlideOut(PresetPos.Top, 40);
                PressA.SlideOut(PresetPos.Bottom, 40);
                MySlider.Hide(PresetPos.Bottom);

                MyGame.WaitThenDo(28, () =>
                {
                    // Add the timer
                    Timer = new GUI_Timer_Base();
                    Timer.Time = 15 * 62;
                    Timer.OnTimeExpired += timer => Fail();
                    MyGame.AddGameObject(Timer);
                    Timer.Active = false;
                    Timer.CountDownWhileDead = true;
                    Timer.MyPile.BubbleUp(false, 7, 1.5f);

                    MyGame.WaitThenDo(43, () => {
                        TutorialHelper.ReadyGo(MyGame, () => {
                            // Activate timer and add the players
                            Timer.Active = true;
                            ShowBobs();
                        });
                    });
                });
            });
        }

        void ShowBobs()
        {
            MyGame.EnterFrom(MyGame.MyLevel.StartDoor);
            //MyGame.MyLevel.AllowRecording = true;
            MyGame.MyLevel.HaveTimeLimit = true;
            MyGame.MyLevel.Bobs.ForEach(bob => bob.Immortal = false);
            MyGame.MyLevel.PreventReset = false;
            //MyGame.MyLevel.StartRecording();
        }

        GUI_Timer_Base Timer;
        void Fail()
        {
            GameData game = MyGame;
            Level level = game.MyLevel;

            // Remove the timer
            Timer.SlideOut(GUI_Panel.PresetPos.Top);
            Timer.Active = false;

            // End the level
            level.EndLevel();

            game.AddToDo(() =>
            {
                int Delay = 10;
                game.WaitThenDo(Delay, () =>
                {
                    // Kill all the players
                    foreach (Bob bob in level.Bobs)
                    {
                        if (bob.IsVisible())
                        {
                            ParticleEffects.PiecePopFart(level, bob.Core.Data.Position);
                            bob.Core.Show = false;
                        }

                        if (!bob.Dead && !bob.Dying)
                            bob.Die(Bob.BobDeathType.None, true, false);
                    }

                    // Fail screen
                    game.WaitThenDo(80, () =>
                    {
                        game.AddGameObject(new BonusFail());
                    });
                });

                return true;
            });
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();
            
            Objects = null;
        }

        int Score;
        void SetScore(int Score)
        {
            if (this.Score != Score)
            {
                this.Score = Score;
                UpdateScoreText();
            }
        }

        StringBuilder MyString = new StringBuilder(50, 50);
        EzText ScoreText;
        void UpdateScoreText()
        {
            MyString.Length = 0;
            MyString.Add(Score);

            ScoreText.SubstituteText(MyString);
        }
        
        public ContinuousDiff()
        {
            MyPile = new DrawPile();
            EnsureFancy();

            PauseOnPause = true;

            MyPile.FancyPos.UpdateWithGame = true;

            EzFont font = Tools.Font_DylanThin42;
            float scale = .88f;
            //EzFont font = Tools.Font_Dylan60;
            //float scale = 1f;
            
            ScoreText = new EzText("  ", font, 950, false, true);
            ScoreText.Scale = scale;
            ScoreText.Pos = new Vector2(1.297119f, 0f);// new Vector2(263.9231f, 0);
            ScoreText.MyFloatColor = new Color(255, 255, 255).ToVector4();
            MyPile.Add(ScoreText);

            EzText header = new EzText("Score:", font, 950, false, true);
            header.Scale = scale;
            header.Pos = new Vector2(-783.0857f, 0);
            header.MyFloatColor = new Color(255, 255, 255).ToVector4();
            MyPile.Add(header);

            MyPile.Pos = new Vector2(-207.0713f, -699.0602f);// new Vector2(0, 745.3846f);
        }

        int Count = 0;
        int Period = 145;
        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (Count == 0)
                GetObjects();

            //if (Hid || !Active) return;

            if (Winding)
            {
                float s = Tools.ZigZag(Period, Count);
                int score = Tools.Lerp(1000, 9999, s);
                int Incr = 200;
                score = Incr * (score / Incr);
                if (score > 9700) score = 9999;
                SetScore(score);

                int n = (int)Tools.Lerp(-1, Objects.Count, s);
                n = Tools.Restrict(0, Objects.Count, n);
                for (int i = 0; i < Objects.Count; i++)
                    Objects[i].Core.Active = Objects[i].Core.Show = i < n;

                MySlider.Val = s;
            }

            Count++;
        }
    }
}