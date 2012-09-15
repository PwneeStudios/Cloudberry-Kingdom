using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Challenge_UpUp : Challenge_Escalation
    {
        static int[] NumLives = { 15, 12, 10, 5, 1 };

        static readonly Challenge_UpUp instance = new Challenge_UpUp();
        public static new Challenge_UpUp Instance { get { return instance; } }

        protected Challenge_UpUp()
        {
            ID = new Guid("1a8141b2-234b-4e1e-7298-0c36b6ebdcc3");
            Name = "Up up and away";
            MenuPic = "menupic_escalation";
            HighScore = SaveGroup.UpUpHighScore;
            HighLevel = SaveGroup.UpUpHighLevel;
        }

        protected override void PreStart_Tutorial()
        {
            MyStringWorld.OnSwapToFirstLevel += data => data.MyGame.AddGameObject(new Escalation_Tutorial(this, Campaign.PrincessPos.CenterToUp));
        }

        //int LevelLength_Short = 5050;
        //int LevelLength_Long = 8300;
        int LevelLength_Short = 8500;
        int LevelLength_Long = 8500;


        static TileSet[] tilesets = {
            TileSets.Castle, TileSets.Dungeon, TileSets.Terrace,
            TileSets.Castle, TileSets.Dungeon,
            TileSets.Castle, TileSets.Dungeon, TileSets._Night };

        //static List<BobPhsx> HeroTypes = new List<BobPhsx>(new BobPhsx[] {
        //    BobPhsxDouble.Instance, BobPhsxJetman.Instance, 
        //        BobPhsx.MakeCustom(Hero_BaseType.Wheel, Hero_Shape.Small, Hero_MoveMod.Classic),
        //    BobPhsxDouble.Instance, BobPhsxJetman.Instance, 
        //        BobPhsxWheel.Instance,
        //    BobPhsxDouble.Instance, BobPhsxJetman.Instance, 
        //        BobPhsxSmall.Instance,
        //    BobPhsxDouble.Instance, BobPhsxJetman.Instance, 
        //        BobPhsx.MakeCustom(Hero_BaseType.Wheel, Hero_Shape.Small, Hero_MoveMod.Double),
        //    BobPhsxDouble.Instance, BobPhsxJetman.Instance, 
        //        BobPhsx.MakeCustom(Hero_BaseType.Wheel, Hero_Shape.Small, Hero_MoveMod.Jetpack)
        //});

        //static List<BobPhsx> HeroTypes = new List<BobPhsx>(new BobPhsx[] {
        //    BobPhsxDouble.Instance, BobPhsxJetman.Instance, BobPhsxSmall.Instance
        //});

        //static List<BobPhsx> HeroTypes = new List<BobPhsx>(new BobPhsx[] {
        //    BobPhsxJetman.Instance
        //});

        static List<BobPhsx> HeroTypes = new List<BobPhsx>(new BobPhsx[] {
            BobPhsxNormal.Instance
        });

        protected override BobPhsx GetHero(int i)
        {
            return HeroTypes[i % HeroTypes.Count];
        }

        protected override TileSet GetTileSet(int i)
        {
            return tilesets[(i / LevelsPerTileset) % tilesets.Length];
        }

        protected override LevelSeedData Make(int Index, float Difficulty)
        {
            BobPhsx hero = GetHero(Index);

            // Adjust the length. Longer for higher levels.
            int Length;
            float t = ((Index - StartIndex) % LevelsPerTileset) / (float)(LevelsPerTileset - 1);
            Length = Tools.LerpRestrict(LevelLength_Short, LevelLength_Long, t);

            // Create the LevelSeedData
            LevelSeedData data = Campaign.HeroLevel(Difficulty, hero, Length, 1, LevelGeometry.Up);
            data.SetTileSet(GetTileSet(Index - StartIndex));

            // Adjust the piece seed data
            foreach (PieceSeedData piece in data.PieceSeeds)
            {
                piece.MyUpgrades1.Zero();
                piece.MyUpgrades1[Upgrade.FlyBlob] = 4.8f + Difficulty * 3;
                piece.MyUpgrades1[Upgrade.Speed] = 4.8f + Difficulty * 3;
                piece.MyUpgrades1[Upgrade.Jump] = 3.75f;
                piece.CalculateSimple();

                // Shorten the initial computer delay
                piece.Style.ComputerWaitLengthRange = new Vector2(8, 35);

                piece.Style.MyModParams = (level, p) =>
                {
                    Coin_Parameters Params = (Coin_Parameters)p.Style.FindParams(Coin_AutoGen.Instance);
                    Params.StartFrame = 90;
                    Params.FillType = Coin_Parameters.FillTypes.Regular;
                };
            }

            return data;
        }
    }
}