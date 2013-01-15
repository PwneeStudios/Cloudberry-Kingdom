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

        static Vector2[][] IconPos = new Vector2[][] { 
            new Vector2[] { new Vector2(0, 470) },
            new Vector2[] { new Vector2(0, 450), new Vector2(255, 630) },
            new Vector2[] { new Vector2(-10, 350), new Vector2(-10, 690), new Vector2(235, 552) } };

        static float[] IconScale = new float[] { .8f, .8f, .8f };

        protected override void AdditionalSwap(int levelindex)
        {
            Awardments.CheckForAward_HeroRush2_Level(levelindex - StartIndex);
        }
        
        protected override void MakeExitDoorIcon(int levelindex)
        {
            //IconPos = new Vector2[][] { 
            //new Vector2[] { new Vector2(0, 470) },
            //new Vector2[] { new Vector2(0, 450), new Vector2(255, 630) },
            //new Vector2[] { new Vector2(-10, 350), new Vector2(-10, 690), new Vector2(235, 552) } };


            HeroSpec spec = HeroList[(levelindex + 1 - StartIndex) % HeroList.Count];
            GameData game = Tools.CurGameData;
            Vector2 pos = Tools.CurLevel.FinalDoor.Pos;

            // Delete the exit sign
            foreach (ObjectBase obj in Tools.CurLevel.Objects)
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
            GameTypeId = 3;
            MenuName = Name = Localization.Words.HybridRush;            
        }

        void MakeHeroList()
        {
            HeroList.Clear();

            // Create the list of custom heros
            Hero_BaseType[] BaseTypes = new Hero_BaseType[]
                { Hero_BaseType.Bouncy, Hero_BaseType.Box, Hero_BaseType.Classic, Hero_BaseType.Spaceship,
                  Hero_BaseType.Wheel };

            Hero_MoveMod[] MoveTypes = new Hero_MoveMod[] { Hero_MoveMod.Classic, Hero_MoveMod.Double, Hero_MoveMod.Jetpack };

            for (int i = 0; i < 2; i++)
            {
                foreach (Hero_BaseType basetype in BaseTypes)
                    foreach (Hero_Shape shape in Tools.GetValues<Hero_Shape>())
                        foreach (Hero_MoveMod move in MoveTypes)
                        {
                            if (i == 0 && basetype == Hero_BaseType.Box) continue;

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
        }

        void ShuffleHeros()
        {
            MakeHeroList();

            HeroList.Insert(0, new HeroSpec(Hero_BaseType.RocketBox, Hero_Shape.Classic, Hero_MoveMod.Classic));
            HeroList.Insert(0, new HeroSpec(Hero_BaseType.RocketBox, Hero_Shape.Classic, Hero_MoveMod.Classic));

            // Shuffle the non-classic heros
            HeroList = Tools.GlobalRnd.Shuffle(HeroList);

            // The first hero is always classic
            HeroList.Insert(0, new HeroSpec(Hero_BaseType.Classic, Hero_Shape.Classic, Hero_MoveMod.Classic));
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