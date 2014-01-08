using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public abstract class Rush : Challenge
    {
        public GUI_Timer Timer;
        protected void OnTimeExpired(GUI_Timer_Base Timer)
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

            game.AddToDo(() =>
            {
                // Kill all the players
                foreach (Bob bob in level.Bobs)
                {
                    if (bob.IsVisible())
                    {
                        ParticleEffects.PiecePopFart(level, bob.CoreData.Data.Position);
                        bob.CoreData.Show = false;
                    }

                    if (!bob.Dead && !bob.Dying)
                        bob.Die(BobDeathType.None, true, false);
                }

                // Add the Game Over panel, check for Awardments
                game.WaitThenDo(105, ShowEndScreen);

                return true;
            });
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
            Timer.OnTimeExpired += OnTimeExpired;

            // Create the string world, and add the relevant game objects
            MyStringWorld = new StringWorldTimed(GetSeed, Timer);
            MyStringWorld.StartLevelMusic = game => { };

            // Start menu
            MyStringWorld.OnLevelBegin += level =>
                {
                    level.MyGame.AddGameObject(InGameStartMenu.MakeListener());
                    return false;
                };

            // Invert level

            // Additional preprocessing
            SetGameParent(MyStringWorld);
            AdditionalPreStart();

            // Start
            MyStringWorld.Init(StartLevel);
        }

        protected virtual void AdditionalPreStart()
        {
        }
    }
}