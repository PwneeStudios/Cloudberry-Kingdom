using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Rush : Challenge
    {
        protected StringWorldGameData StringWorld { get { return (StringWorldGameData)Tools.WorldMap; } }

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

                    // Add the Game Over panel, check for Awardments
                    game.WaitThenDo(105, ShowEndScreen);
                });

                return true;
            });
        }

        protected int i = 0;
        protected int StartIndex = 0;
        protected StringWorldTimed MyStringWorld;
        public override void Start(int StartLevel)
        {
            base.Start(StartLevel);
            i = StartIndex = StartLevel;

            // Create the timer
            Timer = new GUI_Timer();

            // Set the time expired function
            Timer.OnTimeExpired += OnTimeExpired;

            // Create the string world, and add the relevant game objects
            MyStringWorld = new StringWorldTimed(GetSeeds(), Timer);
            MyStringWorld.OnBeginLoad += () => MyStringWorld.LevelSeeds.AddRange(this.GetMoreSeeds());
            MyStringWorld.StartLevelMusic = game => { };

            // Start menu
            MyStringWorld.OnLevelBegin += level =>
                    level.MyGame.AddGameObject(InGameStartMenu.MakeListener());

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