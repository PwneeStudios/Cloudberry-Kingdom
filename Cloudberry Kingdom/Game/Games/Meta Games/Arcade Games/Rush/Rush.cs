using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;




namespace CloudberryKingdom
{
    public abstract class Rush : Challenge
    {
        class KillAllPlayersHelper : LambdaFunc<bool>
        {
            Rush rush;
            GameData game;

            public KillAllPlayersHelper(Rush rush, GameData game)
            {
                this.rush = rush;
                this.game = game;
            }

            public bool Apply()
            {
                Level level = game.MyLevel;

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

                // Add the Game Over panel, check for Awardments
                game.WaitThenDo(105, new ShowEndScreenProxy(rush));

                return true;
            }
        }

        public GUI_Timer Timer;
        protected void OnTimeExpired(GUI_Timer_Base Timer)
        {
        }

        class RushOnTimeExpiredLambda : Lambda_1<GUI_Timer_Base>
        {
            Rush rush;

            public RushOnTimeExpiredLambda(Rush rush)
            {
                this.rush = rush;
            }

            public void Apply(GUI_Timer_Base Timer)
            {
                GameData game = Timer.MyGame;
                Level level = game.MyLevel;

                // Remove the timer
                Timer.SlideOut(GUI_Panel.PresetPos.Top);
                Timer.Active = false;

                // End the level
                level.EndLevel();

                // Void the final door
                if (level.FinalDoor != null)
                    level.FinalDoor.OnOpen = null;

                game.AddToDo(new KillAllPlayersHelper(rush, game));
            }
        }

        protected int StartIndex = 0;
        protected StringWorldTimed MyStringWorld;
        public override void Start(int StartLevel)
        {
            base.Start(StartLevel);
            StartIndex = StartLevel;

            // Create the timer
            Timer = new GUI_Timer();

            // Set the time expired function
            Timer.OnTimeExpired.Add(new RushOnTimeExpiredLambda(this));

            // Create the string world, and add the relevant game objects
            MyStringWorld = new StringWorldTimed(GetSeed, Timer);
            MyStringWorld.StartLevelMusic = null;

            // Start menu
            MyStringWorld.OnLevelBegin = new OnLevelBeginLambda();


            // Invert level

            // Additional preprocessing
            SetGameParent(MyStringWorld);
            AdditionalPreStart();

            // Start
            MyStringWorld.Init(StartLevel);
        }

        class OnLevelBeginLambda : LambdaFunc_1<Level, bool>
        {
            public OnLevelBeginLambda()
            {
            }

            public bool Apply(Level level)
            {
                level.MyGame.AddGameObject(InGameStartMenu.MakeListener());
                return false;
            }
        }

        protected virtual void AdditionalPreStart()
        {
        }
    }
}