using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Challenge_HeroRush2 : Challenge_HeroRush
    {
        public static List<HeroSpec> HeroList = new List<HeroSpec>(100);

        static readonly Challenge_HeroRush2 instance = new Challenge_HeroRush2();
        public static new Challenge_HeroRush2 Instance { get { return instance; } }

        static bool GoalMet = false;
        public override bool GetGoalMet() { return GoalMet; }
        public override void SetGoalMet(bool value) { GoalMet = value; }

        static Vector2[][] IconPos = new Vector2[][] { 
            new Vector2[] { new Vector2(240, 400) },
            new Vector2[] { new Vector2(255, 310), new Vector2(255, 650) },
            new Vector2[] { new Vector2(255, 310), new Vector2(255, 650), new Vector2(10, 512) } };

        static float[] IconScale = new float[] { .8f, .8f, .8f };

        protected override void MakeExitDoorIcon(int levelindex)
        {
            IconPos = new Vector2[][] { 
            new Vector2[] { new Vector2(0, 470) },
            new Vector2[] { new Vector2(0, 450), new Vector2(255, 630) },
            new Vector2[] { new Vector2(-10, 350), new Vector2(-10, 690), new Vector2(235, 552) } };


            HeroSpec spec = HeroList[(levelindex + 1 - StartIndex) % HeroList.Count];
            GameData game = Tools.CurGameData;
            Vector2 pos = Tools.CurLevel.FinalDoor.Pos;

            // Delete the exit sign
            foreach (IObject obj in Tools.CurLevel.Objects)
                if (obj is Sign)
                    obj.Core.MarkedForDeletion = true;

            // Count number of icons needed
            int Total = 0;
            if (spec.basetype != Hero_BaseType.Classic) Total++;
            if (spec.move != Hero_MoveMod.Classic) Total++;
            if (spec.shape != Hero_Shape.Classic) Total++;

            // Make the icons
            if (Total == 0)
            {
                game.AddGameObject(new DoorIcon(BobPhsxNormal.Instance, pos + IconPos[0][0], IconScale[0]));
                return;
            }

            int Count = 0;
            if (spec.basetype != Hero_BaseType.Classic)
            {
                game.AddGameObject(new DoorIcon(BobPhsx.GetPhsx(spec.basetype), pos + IconPos[Total - 1][Count], IconScale[Total - 1]));
                Count++;
            }

            if (spec.move != Hero_MoveMod.Classic)
            {
                game.AddGameObject(new DoorIcon(BobPhsx.GetPhsx(spec.move), pos + IconPos[Total - 1][Count], IconScale[Total - 1]));
                Count++;
            }

            if (spec.shape != Hero_Shape.Classic)
            {
                game.AddGameObject(new DoorIcon(BobPhsx.GetPhsx(spec.shape), pos + IconPos[Total - 1][Count], IconScale[Total - 1]));
                Count++;
            }
        }

        protected Challenge_HeroRush2()
        {
            ID = new Guid("2a5144f2-585e-4716-7398-0c96b6ebdec7");
            MenuName = "Hero Rush 2";
            Name = "Hero Rush 2, Revenge of the Double Jump";
            MenuPic = "menupic_herorush2";
            HighScore = SaveGroup.HeroRush2HighScore;
            HighLevel = SaveGroup.HeroRush2HighLevel;
            Goal = 35;

            // Create the list of custom heros
            foreach (Hero_BaseType basetype in Tools.GetValues<Hero_BaseType>())
                foreach (Hero_Shape shape in Tools.GetValues<Hero_Shape>())
                    foreach (Hero_MoveMod move in Tools.GetValues<Hero_MoveMod>())
                    {
                        // Spaceships can only have their shape modified
                        if (basetype == Hero_BaseType.Spaceship && move != Hero_MoveMod.Classic)
                            continue;

                        // Normal bob is added later (to make sure it is first)
                        if (basetype == Hero_BaseType.Classic && shape == Hero_Shape.Classic && move == Hero_MoveMod.Classic)
                            continue;

                        // Bouncey can not be double jump
                        if (basetype == Hero_BaseType.Bouncy && move == Hero_MoveMod.Double)
                            continue;

                        //HeroList[NumHeros++] = new HeroSpec(basetype, shape, move);
                        HeroList.Add(new HeroSpec(basetype, shape, move));
                    }
        }

        void ShuffleHeros()
        {
            // Shuffle the non-classic heros
            HeroList = Tools.Shuffle(HeroList);

            // The first hero is always classic
            HeroList.Insert(0, new HeroSpec(Hero_BaseType.Classic, Hero_Shape.Classic, Hero_MoveMod.Classic));
        }

        protected override void ShowEndScreen()
        {
            Tools.CurGameData.AddGameObject(new GameOverPanel(HighScore, HighLevel, StringWorld,
                score => Awardments.CheckForAward_HeroRush2_Score(score),
                level => { if (level >= Goal) SetGoalMet(true); }));
        }

        public override void Start(int StartLevel)
        {
            ShuffleHeros();

            base.Start(StartLevel);
        }

        protected override BobPhsx GetHero(int i)
        {
            //return BobPhsx.MakeCustom(HeroList[i % NumHeros]);
            return BobPhsx.MakeCustom(HeroList[i % HeroList.Count]);
        }

        protected override void PreStart_Tutorial(bool TemporarySkip)
        {
            HeroRush_Tutorial.TemporarySkip = TemporarySkip;
            MyStringWorld.OnSwapToFirstLevel += data => data.MyGame.AddGameObject(new HeroRush2_Tutorial(this));
        }
    }
}