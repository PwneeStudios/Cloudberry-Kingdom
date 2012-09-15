using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Challenge_Wheelie : Challenge_Escalation
    {
        static int[] NumLives = { 15, 12, 10, 5, 1 };

        static readonly Challenge_Wheelie instance = new Challenge_Wheelie();
        public static new Challenge_Wheelie Instance { get { return instance; } }

        protected Challenge_Wheelie()
        {
            ID = new Guid("1a8141b2-111b-ce1e-7298-0c36b6ebdcc3");
            Name = "Wheelie";
            MenuPic = "menupic_escalation";
            HighScore = SaveGroup.WheelieHighScore;
            HighLevel = SaveGroup.WheelieHighLevel;
        }

        protected override void PreStart_Tutorial()
        {
            MyStringWorld.OnSwapToFirstLevel += data => data.MyGame.AddGameObject(new Escalation_Tutorial(this, Campaign.PrincessPos.CenterToUp));
        }

        int LevelLength_Short = 9000;
        int LevelLength_Long = 9000;
        static TileSet[] tilesets = {
            TileSets.Dungeon, TileSets._Night, TileSets.Castle, TileSets.Dungeon,
            TileSets._NightSky };

        static List<BobPhsx> HeroTypes = new List<BobPhsx>(new BobPhsx[] {
            BobPhsx.MakeCustom(Hero_BaseType.Wheel, Hero_Shape.Classic, Hero_MoveMod.Classic),
            BobPhsx.MakeCustom(Hero_BaseType.Wheel, Hero_Shape.Small, Hero_MoveMod.Classic),
            BobPhsx.MakeCustom(Hero_BaseType.Wheel, Hero_Shape.Classic, Hero_MoveMod.Double),
            BobPhsx.MakeCustom(Hero_BaseType.Wheel, Hero_Shape.Small, Hero_MoveMod.Jetpack),
            BobPhsx.MakeCustom(Hero_BaseType.Wheel, Hero_Shape.Small, Hero_MoveMod.Double) });
 
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
            LevelSeedData data = Campaign.HeroLevel(Difficulty, hero, Length, 1, LevelGeometry.Right);
            data.SetTileSet(GetTileSet(Index - StartIndex));

            // Adjust the piece seed data
            foreach (PieceSeedData piece in data.PieceSeeds)
            {
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