using System;
using System.Collections.Generic;



namespace CloudberryKingdom
{
    public class DifficultyGroups
    {
        class FixedPieceModHelper : Lambda_1<PieceSeedData>
        {
            float Difficulty;
            LevelSeedData LevelSeed;

            public FixedPieceModHelper(float Difficulty, LevelSeedData LevelSeed)
            {
                this.Difficulty = Difficulty;
                this.LevelSeed = LevelSeed;
            }

            public void Apply(PieceSeedData piece)
            {
                DifficultyGroups.FixedPieceSeed(piece, Difficulty, LevelSeed.DefaultHeroType);
            }
        }

        /// <summary>
        /// Returns a function that modifies a PieceSeed's difficulty
        /// </summary>
        public static Lambda_1<PieceSeedData> FixedPieceMod(float Difficulty, LevelSeedData LevelSeed)
        {
            return new FixedPieceModHelper(Difficulty, LevelSeed);
        }

        public static float HeroDifficultyMod(float Difficulty, BobPhsx hero)
        {
            if (hero is BobPhsxBox) return -.235f;
            if (hero is BobPhsxWheel) return -.1f;
            if (hero is BobPhsxRocketbox) return -.33f;
            if (hero is BobPhsxSmall) return -.1f;
            if (hero is BobPhsxSpaceship) return -.065f;
            if (hero is BobPhsxDouble) return 0;
            if (hero is BobPhsxBouncy) return -0.435f;

            return 0;
        }

        /// <summary>
        /// Modify the upgrades for a PieceSeed.
        /// Difficulty should range from 0 (Easy) to 4 (Masochistic)
        /// </summary>
        public static void FixedPieceSeed(PieceSeedData piece, float Difficulty, BobPhsx hero)
        {
            InitFixedUpgrades();

            // Up level
            if (piece.GeometryType == LevelGeometry.Up)
                piece.Rnd.Choose(UpUpgrades).Apply(piece, Difficulty);
            // Down level
            else if (piece.GeometryType == LevelGeometry.Down)
                piece.Rnd.Choose(DownUpgrades).Apply(piece, Difficulty);
            // Cart level
            else if (hero is BobPhsxRocketbox)
            {
                if (Difficulty < .5f)
                    Difficulty -= .8f;
                else
                    Difficulty -= 1.35f;

                piece.Rnd.Choose(CartUpgrades).Apply(piece, Difficulty);
            }
            // Generic hero level
            else
            {
                Difficulty += HeroDifficultyMod(Difficulty, hero);

                switch ((int)Difficulty)
                {
                    case 0: piece.Rnd.Choose(EasyUpgrades).Apply(piece, Difficulty); break;
                    case 1: piece.Rnd.Choose(NormalUpgrades).Apply(piece, Difficulty); break;
                    case 2: piece.Rnd.Choose(AbusiveUpgrades).Apply(piece, Difficulty); break;
                    default: piece.Rnd.Choose(HardcoreUpgrades).Apply(piece, Difficulty); break;
                }
            }

            // Mod upgrades to test things here
            //piece.MyUpgrades1[Upgrade.Elevator] = 5;
            //piece.MyUpgrades1.CalcGenData(piece.MyGenData.gen1, piece.Style);
            //piece.MyUpgrades2[Upgrade.Elevator] = 5;
            //piece.MyUpgrades2.CalcGenData(piece.MyGenData.gen1, piece.Style);

            piece.StandardClose();
        }

        static void InitFixedUpgrades()
        {
            if (EasyUpgrades != null)
                return;

            EasyUpgrades = new List<UpgradeSequence>();

            // Difficulties
            MakeEasyUpgrades();
            MakeNormalUpgrades();
            MakeAbusiveUpgrades();
            MakeHardcoreUpgrades();

            // Special hero overrides
            MakeCartUpgrades();

            // Up/down overrides
            MakeUpUpgrades();
            MakeDownUpgrades();
        }

        public struct UpgradeSequenceSingle
        {
            public void Apply(PieceSeedData Piece, float Difficulty)
            {
                float d = Difficulty;

                float val = 0;
                if (d < 0) val = CoreMath.LerpRestrict((float)0, (float)Values[0], d - -1);
                else if (d < 1) val = CoreMath.SpecialLerp((float)Values[0], (float)Values[1], d - 0);
                else if (d < 2) val = CoreMath.SpecialLerp((float)Values[1], (float)Values[2], d - 1);
                else if (d < 3) val = CoreMath.SpecialLerp((float)Values[2], (float)Values[3], d - 2);
                else val = CoreMath.SpecialLerpRestrict((float)Values[3], (float)Values[4], d - 3);

                Piece.u[MyUpgrade] = val;
            }

            Upgrade MyUpgrade;
            public double[] Values;

            public UpgradeSequenceSingle(Upgrade MyUpgrade, params double[] values)
            {
                this.MyUpgrade = MyUpgrade;

                Values = new double[5];
                for (int i = 0; i < 5; i++)
                    Values[i] = values[i];
            }
        }

        public struct UpgradeSequence
        {
            public void Apply(PieceSeedData Piece, float Difficulty)
            {
                foreach (UpgradeSequenceSingle upgrade in UpgradeList)
                    upgrade.Apply(Piece, Difficulty);
            }

            List<UpgradeSequenceSingle> UpgradeList;
            public UpgradeSequence(params UpgradeSequenceSingle[] Upgrades)
            {
                UpgradeList = new List<UpgradeSequenceSingle>();

                for (int i = 0; i < Upgrades.Length; i++)
                    UpgradeList.Add(Upgrades[i]);
            }
        }

        static List<UpgradeSequence> UpUpgrades = new List<UpgradeSequence>();
        static void MakeUpUpgrades()
        {
            List<UpgradeSequence> f = UpUpgrades;

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.FlyBlob, 0, 2, 5, 7.5, 10)));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.FlyBlob, 0, 2, 5, 7.5, 10),
                new UpgradeSequenceSingle(Upgrade.FallingBlock, 1, 3.5, 5, 7.5, 10),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, 1, 3.5, 5, 7.5, 10),
                new UpgradeSequenceSingle(Upgrade.GhostBlock, 1, 3.5, 5, 7.5, 10),

                new UpgradeSequenceSingle(Upgrade.Jump, 0, 3, 5, 7.5, 8),
                new UpgradeSequenceSingle(Upgrade.Speed, 0, 3, 5, 8.5, 15)
            ));
        }

        static List<UpgradeSequence> DownUpgrades = new List<UpgradeSequence>();
        static void MakeDownUpgrades()
        {
            List<UpgradeSequence> f = DownUpgrades;

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.FlyBlob, 0, 2, 5, 7.5, 10),
                new UpgradeSequenceSingle(Upgrade.FallingBlock, 1, 3.5, 5, 7.5, 10),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, 1, 3.5, 5, 7.5, 10),
                new UpgradeSequenceSingle(Upgrade.GhostBlock, 1, 3.5, 5, 7.5, 10),

                new UpgradeSequenceSingle(Upgrade.Jump, 0, 3, 5, 7.5, 10),
                new UpgradeSequenceSingle(Upgrade.Speed, 0, 3, 4, 7, 10),

                new UpgradeSequenceSingle(Upgrade.Laser, 0, 1, 2, 5, 7.3)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.FlyBlob, 0, 2, 5, 7.5, 10),
                new UpgradeSequenceSingle(Upgrade.FallingBlock, 1, 3.5, 5, 7.5, 10),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, 1, 3.5, 5, 7.5, 10),
                new UpgradeSequenceSingle(Upgrade.GhostBlock, 1, 3.5, 5, 7.5, 10),

                new UpgradeSequenceSingle(Upgrade.Jump, 0, 3, 5, 7.5, 10),
                new UpgradeSequenceSingle(Upgrade.Speed, 0, 3, 4, 7, 10),

                new UpgradeSequenceSingle(Upgrade.SpikeyLine, 0, 1, 2, 5, 7.3)
            ));
        }

        static List<UpgradeSequence> CartUpgrades = new List<UpgradeSequence>();
        static void MakeCartUpgrades()
        {
            List<UpgradeSequence> f = CartUpgrades;

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, 0, 1, 2, 3, 4),
                new UpgradeSequenceSingle(Upgrade.Speed, 1, 2, 3, 4, 5),
                //new UpgradeSequenceSingle(Upgrade.MovingBlock, 1, 2, 3, 6, 9),
                new UpgradeSequenceSingle(Upgrade.FallingBlock, 1, 2, 3, 6, 9),
                new UpgradeSequenceSingle(Upgrade.FireSpinner, 1, 3, 7, 9, 10),
                new UpgradeSequenceSingle(Upgrade.SpikeyLine, 0, 2, 3.6, 7, 9),
                new UpgradeSequenceSingle(Upgrade.Laser, 1, 3, 6, 7, 8.5)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, 0, 1, 2, 3, 4),
                new UpgradeSequenceSingle(Upgrade.Speed, 1, 2, 3, 4, 5),
                new UpgradeSequenceSingle(Upgrade.GhostBlock, 1, 2, 3, 6, 9),
                new UpgradeSequenceSingle(Upgrade.FallingBlock, 1, 2, 3, 6, 9),
                new UpgradeSequenceSingle(Upgrade.SpikeyGuy, 1, 2, 3.6, 7, 9),
                new UpgradeSequenceSingle(Upgrade.Spike, 2, 3, 7, 9, 9),
                new UpgradeSequenceSingle(Upgrade.Laser, 0, 3, 5, 7, 8.5)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, 0, 1, 2, 3, 4),
                new UpgradeSequenceSingle(Upgrade.Speed, 1, 2, 3, 4, 5),
                new UpgradeSequenceSingle(Upgrade.BouncyBlock, 1, 2, 3, 6, 9),
                new UpgradeSequenceSingle(Upgrade.Laser, 0, 3, 5, 7, 8.5),
                new UpgradeSequenceSingle(Upgrade.SpikeyLine, 1, 2, 3.6, 7, 9),
                new UpgradeSequenceSingle(Upgrade.Pinky, 1, 3, 7, 8, 8.5)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, 0, 1, 2, 3, 4),
                new UpgradeSequenceSingle(Upgrade.Speed, 1, 2, 3, 4, 5),
                //new UpgradeSequenceSingle(Upgrade.FlyBlob, 1, 2, 2, 2, 8),
                new UpgradeSequenceSingle(Upgrade.GhostBlock, 1, 2, 3, 6, 9),
                //new UpgradeSequenceSingle(Upgrade.MovingBlock, 1, 2, 3, 6, 9),
                new UpgradeSequenceSingle(Upgrade.FireSpinner, 1, 3, 7, 9, 10),
                new UpgradeSequenceSingle(Upgrade.SpikeyLine, 0, 2, 3.6, 7, 9),
                new UpgradeSequenceSingle(Upgrade.Pinky, 1, 3, 6, 7, 8.5),
                new UpgradeSequenceSingle(Upgrade.Spike, 1, 3, 6, 7, 8.5)
            ));
        }

        static List<UpgradeSequence> EasyUpgrades;
        static void MakeEasyUpgrades()
        {
            List<UpgradeSequence> f = EasyUpgrades;

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, 2, 5, 5, 5, 5),
                new UpgradeSequenceSingle(Upgrade.Speed, 1, 2, 3, 5, 11),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, 1, 1, 2.2, 3, 3),
                new UpgradeSequenceSingle(Upgrade.FallingBlock, 1, 1, 2.2, 3, 3),
                new UpgradeSequenceSingle(Upgrade.FlyBlob, 1, 1, 2.2, 2.2, 2.2),
                new UpgradeSequenceSingle(Upgrade.FireSpinner, 1, 3, 5, 7, 9),
                new UpgradeSequenceSingle(Upgrade.SpikeyLine, 0, 3, 5, 7, 9)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, 2, 5, 5, 5, 5),
                new UpgradeSequenceSingle(Upgrade.Speed, 1, 2, 3, 5, 11),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, 1, 1, 2.2, 3, 3),
                new UpgradeSequenceSingle(Upgrade.FallingBlock, 1, 1, 2.2, 3, 3),
                new UpgradeSequenceSingle(Upgrade.FireSpinner, 1, 3, 5, 7, 10),
                new UpgradeSequenceSingle(Upgrade.Spike, 1, 3, 5, 7, 10),
                new UpgradeSequenceSingle(Upgrade.Laser, 0, 0, 0, 3, 6)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, 2, 5, 5, 5, 5),
                new UpgradeSequenceSingle(Upgrade.Speed, 1, 2, 3, 5, 11),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, 1, 1, 2.2, 3, 3),
                new UpgradeSequenceSingle(Upgrade.FallingBlock, 1, 1, 2.2, 3, 3),
                new UpgradeSequenceSingle(Upgrade.FlyBlob, 1, 1, 2.2, 2.2, 2.2),
                new UpgradeSequenceSingle(Upgrade.Pinky, .75f, 3, 5, 7, 9),
                new UpgradeSequenceSingle(Upgrade.Spike, 1, 3, 5, 7, 9),
                new UpgradeSequenceSingle(Upgrade.FireSpinner, 0, 2, 3, 4, 7)
            ));


            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, 2, 5, 5, 5, 5),
                new UpgradeSequenceSingle(Upgrade.Speed, 1, 2, 3, 5, 11),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, 1, 1, 2.2, 3, 3),
                new UpgradeSequenceSingle(Upgrade.BouncyBlock, 1, 1, 2.2, 3, 3),
                new UpgradeSequenceSingle(Upgrade.GhostBlock, 1, 1, 2.2, 2.2, 2.2),
                new UpgradeSequenceSingle(Upgrade.Pinky, .75f, 3, 5, 7, 9),
                new UpgradeSequenceSingle(Upgrade.Spike, 1, 3, 5, 7, 9),
                new UpgradeSequenceSingle(Upgrade.FireSpinner, 0, 2, 3, 4, 7)
            ));




            // Older

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, 2, 5, 5, 5, 5),
                new UpgradeSequenceSingle(Upgrade.Speed, 1, 2, 3, 5, 11),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, 1, 1, 2.2, 3, 3),
                new UpgradeSequenceSingle(Upgrade.BouncyBlock, 1, 1, 2.2, 3, 3),
                new UpgradeSequenceSingle(Upgrade.Elevator, 1, 1, 2.2, 2.2, 2.2),
                new UpgradeSequenceSingle(Upgrade.Pinky, .8f, 3, 5, 7, 9),
                new UpgradeSequenceSingle(Upgrade.Spike, 1, 3, 5, 7, 9),
                new UpgradeSequenceSingle(Upgrade.Laser, 0, 2, 3, 4, 7)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, 2, 4.8, 7.0, 8.4, 10),
                new UpgradeSequenceSingle(Upgrade.Speed, 1, 2, 8.2, 9.1, 11),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, 1, 1, 2.2, 8, 10),
                new UpgradeSequenceSingle(Upgrade.FallingBlock, 1, 1, 2.2, 7, 10),
                new UpgradeSequenceSingle(Upgrade.FlyBlob, 1, 1, 2.2, 7, 10)
                //u[Upgrade.Fireball] =       D(0,   0,   0,   0,   4);
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.SpikeyGuy, 2, 4, 5.2, 8, 10),
                new UpgradeSequenceSingle(Upgrade.Jump, 3, 2.5, 2, 4, 4.5),
                new UpgradeSequenceSingle(Upgrade.Spike, 0, 3, 7.5, 9, 10),
                new UpgradeSequenceSingle(Upgrade.Speed, 0, 2, 5.5, 8.8, 10)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, 3.5f, 1, 0, 0, 0),
                new UpgradeSequenceSingle(Upgrade.SpikeyGuy, 0, 3.2, 5.5, 8, 10),
                new UpgradeSequenceSingle(Upgrade.Pinky, 1.2f, 3, 5.5, 8, 10),
                new UpgradeSequenceSingle(Upgrade.Spike, 0, 0, 0, 4, 10),
                new UpgradeSequenceSingle(Upgrade.Speed, 2, 3, 4, 8.8, 10),
                new UpgradeSequenceSingle(Upgrade.Ceiling, 1, 2, 4, 7, 10)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Laser, 2.5, 4, 5.5, 7.9, 10),
                new UpgradeSequenceSingle(Upgrade.Speed, 0, 0, 0, 1, 5),
                new UpgradeSequenceSingle(Upgrade.Ceiling, 1, 2, 4, 4, 4)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, 3.6, 2, 0, 0, 0),
                new UpgradeSequenceSingle(Upgrade.Speed, 0, 0, 0, 1, 3),
                new UpgradeSequenceSingle(Upgrade.FireSpinner, 0, 1.5, 4, 6, 9),
                new UpgradeSequenceSingle(Upgrade.Pinky, 0, 1.5, 3.6, 5.7, 8),
                new UpgradeSequenceSingle(Upgrade.FallingBlock, 0, 1, 4, 6, 8),
                new UpgradeSequenceSingle(Upgrade.Cloud, 2, 2.5, 4, 6, 9),
                new UpgradeSequenceSingle(Upgrade.SpikeyGuy, 2, 3, 3.5, 5.6, 10),
                new UpgradeSequenceSingle(Upgrade.BouncyBlock, 0, 0, 4, 6, 8)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Laser, 2, 3.5, 4.2, 6, 9),
                new UpgradeSequenceSingle(Upgrade.Speed, 0, 1, 1.7, 3, 3),
                new UpgradeSequenceSingle(Upgrade.Elevator, 2.8f, 5, 7, 9, 9),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, 1.8f, 3, 3, 3, 3)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.SpikeyLine, .7f, 2, 4, 8.4, 9.5),
                new UpgradeSequenceSingle(Upgrade.Speed, 0, 2, 3, 4.5, 10),
                new UpgradeSequenceSingle(Upgrade.Elevator, 2, 3, 3, 4, 10),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, 0, 2, 4, 4, 4),
                new UpgradeSequenceSingle(Upgrade.FlyBlob, 0, 2, 4, 4, 4),
                new UpgradeSequenceSingle(Upgrade.FireSpinner, 0, 2, 4, 4, 4),
                new UpgradeSequenceSingle(Upgrade.Jump, 1, 3, 4, 4, 4),
                new UpgradeSequenceSingle(Upgrade.Cloud, 0, 1, 2, 3, 4),
                new UpgradeSequenceSingle(Upgrade.Ceiling, 1, 2, 4, 7, 10)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.SpikeyGuy, .7f, 3, 4.5, 7.6, 9.5),
                new UpgradeSequenceSingle(Upgrade.Speed, .7f, 3, 3.5, 8, 10),
                new UpgradeSequenceSingle(Upgrade.Elevator, 3f, 6, 7, 9, 9),
                new UpgradeSequenceSingle(Upgrade.Laser, 0, 0, 0, 0, 4),
                new UpgradeSequenceSingle(Upgrade.Ceiling, 1, 2, 4, 7, 10)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.BouncyBlock, 3.6f, 8.2, 9, 9, 10),
                new UpgradeSequenceSingle(Upgrade.Spike, 2, 7.5, 8.5, 9, 10),
                new UpgradeSequenceSingle(Upgrade.FallingBlock, 4f, 2, 2, 3, 4),
                new UpgradeSequenceSingle(Upgrade.Speed, 0, 0, 2, 5, 10),
                new UpgradeSequenceSingle(Upgrade.FireSpinner, 0, 1, 3, 6, 9),
                new UpgradeSequenceSingle(Upgrade.Pinky, 0, 0, 0, 0, 6)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Laser, 1.8f, 3, 4, 6, 9.5),
                new UpgradeSequenceSingle(Upgrade.Speed, 0, 0, 0, 1, 3),
                new UpgradeSequenceSingle(Upgrade.FireSpinner, 1, 3, 6, 9, 9),
                new UpgradeSequenceSingle(Upgrade.Jump, 3, 4, 4, 0, 0)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.BouncyBlock, 4, 8.2, 9, 9, 10),
                new UpgradeSequenceSingle(Upgrade.Spike, 0, 7.5, 8.5, 9, 10),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, 0, 2, 2, 4, 9),
                new UpgradeSequenceSingle(Upgrade.Speed, 0, 0, 2, 6, 10),
                new UpgradeSequenceSingle(Upgrade.SpikeyLine, 0, 0, 0, 0, 4)
                //u[Upgrade.Fireball] =       D(0,   0,   0,  .5,   4);
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.FireSpinner, 1, 1.5, 2.5, 4, 8),
                new UpgradeSequenceSingle(Upgrade.Pinky, 1, 2, 3.5, 6, 10),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, 1, 3, 4, 9, 10),
                new UpgradeSequenceSingle(Upgrade.Ceiling, 1, 2, 4, 7, 10)
            ));
        }

        static List<UpgradeSequence> NormalUpgrades = new List<UpgradeSequence>();
        static void MakeNormalUpgrades()
        {
            List<UpgradeSequence> f = NormalUpgrades;
            f.AddRange(EasyUpgrades);

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, -1, 4.8, 7.5, 9, 10),
                new UpgradeSequenceSingle(Upgrade.Speed, -1, 0, 6, 9, 10),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, -1, 2, 2, 6, 10),
                new UpgradeSequenceSingle(Upgrade.GhostBlock, -1, 2, 2, 6, 10),
                new UpgradeSequenceSingle(Upgrade.BouncyBlock, -1, 2, 2, 4, 10),
                new UpgradeSequenceSingle(Upgrade.FlyBlob, -1, 2, 2, 4, 10),
                new UpgradeSequenceSingle(Upgrade.Spike, -1, 2, 2, 9, 10)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, -1, 4, 4, 4, 4),
                new UpgradeSequenceSingle(Upgrade.Speed, -1, 2, 2, 5, 10),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, -1, 2, 4, 7, 10),
                new UpgradeSequenceSingle(Upgrade.BouncyBlock, -1, 2, 4, 7, 10),
                new UpgradeSequenceSingle(Upgrade.FireSpinner, -1, 2, 4, 8, 10),
                new UpgradeSequenceSingle(Upgrade.Laser, -1, 0, 0, 0, 5.5)
            ));

            /*
            f.Add(new UpgradeSequence(
                u[Upgrade.Speed] =       D(-1, 2, 4, 7, 10);
                u[Upgrade.Fireball] =    D(-1, 2, 4, 7, 10);
                u[Upgrade.Laser] =       D(-1, 2, 3, 4, 8);
                new UpgradeSequenceSingle(Upgrade.FireSpinner, -1, 2, 4, 7, 10),
                u[Upgrade.Spike] =       D(-1, 2, 4, 7, 10);
            });*/

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.MovingBlock, -1, 6, 8.5, 9, 10),
                new UpgradeSequenceSingle(Upgrade.Jump, -1, 4, 2, 0, 0),
                new UpgradeSequenceSingle(Upgrade.Speed, -1, 3, 5.5, 9, 9),
                new UpgradeSequenceSingle(Upgrade.FireSpinner, -1, 0, 5, 9, 9),
                new UpgradeSequenceSingle(Upgrade.Spike, -1, 0, 0, 3, 9),
                new UpgradeSequenceSingle(Upgrade.Ceiling, 1, 2, 4, 7, 10)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, -1, 0, 0, 5, 8),
                new UpgradeSequenceSingle(Upgrade.FlyBlob, -1, 2, 4, 7, 10),
                new UpgradeSequenceSingle(Upgrade.FallingBlock, -1, 2, 3, 4, 10),
                new UpgradeSequenceSingle(Upgrade.Speed, -1, 4, 6, 9, 10),
                new UpgradeSequenceSingle(Upgrade.GhostBlock, -1, 5, 6, 9, 10)
                //u[Upgrade.Fireball] =       D(-1, 0, 0, 0, 5);
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.MovingBlock, -1, 2, 4, 7, 10),
                new UpgradeSequenceSingle(Upgrade.FlyBlob, -1, 2, 4, 7, 10),
                new UpgradeSequenceSingle(Upgrade.FallingBlock, -1, 2, 4, 7, 10),
                new UpgradeSequenceSingle(Upgrade.SpikeyGuy, -1, 1, 3, 6, 10),
                new UpgradeSequenceSingle(Upgrade.FireSpinner, -1, 1, 4, 7, 10),
                new UpgradeSequenceSingle(Upgrade.Pinky, -1, 1, 3, 6, 10),
                new UpgradeSequenceSingle(Upgrade.Speed, -1, 0, 0, 2, 9)
            ));

            /*
            f.Add(new UpgradeSequence(
                u[Upgrade.Speed] =       D(-1, 0, 1, 3, 10);
                u[Upgrade.Jump] =        D(-1, 3, 5, 8, 10);
                u[Upgrade.Fireball] =    D(-1, 2, 4, 7, 10);
                u[Upgrade.Pinky] =       D(-1, 2, 4, 8, 10);
                u[Upgrade.FlyBlob] =     D(-1, 2, 4, 7, 10);
            });

            f.Add(new UpgradeSequence(
                u[Upgrade.Speed] =       D(-1, 0, 1, 3, 10);
                u[Upgrade.Fireball] =    D(-1, 2, 4, 7, 10);
                u[Upgrade.Pinky] =       D(-1, 2, 4, 8, 10);
                new UpgradeSequenceSingle(Upgrade.BouncyBlock, -1, 2, 4, 7, 10)
            ));

            f.Add(new UpgradeSequence(
                u[Upgrade.Speed] =       D(-1, 3, 4, 7, 10);
                u[Upgrade.Fireball] =    D(-1, 5, 7, 9, 10);
                u[Upgrade.Spike] =       D(-1, 0, 0, 0, 10);
            });*/

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, -1, 2, 4, 7, 10),
                new UpgradeSequenceSingle(Upgrade.Speed, -1, 2, 3, 5, 10),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, -1, 4, 5, 7, 10),
                new UpgradeSequenceSingle(Upgrade.SpikeyGuy, -1, 1.2, 2.7, 6, 10),
                new UpgradeSequenceSingle(Upgrade.FireSpinner, -1, 1.5, 4, 8, 10)
            ));
            /*
            f.Add(new UpgradeSequence(
                u[Upgrade.Fireball] =    D(-1, 2,   4,   7,   10);
                new UpgradeSequenceSingle(Upgrade.FireSpinner, -1, 2,   3,   7,   10),
                u[Upgrade.FlyBlob] =     D(-1, 2,   4,   7,   10);
                new UpgradeSequenceSingle(Upgrade.MovingBlock, -1, 4,   6,   9,   10),
                u[Upgrade.Speed] =       D(-1, 0,   1,   3.5,  7);
            });*/

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, -1, 5.1, 7.5, 9, 10),
                new UpgradeSequenceSingle(Upgrade.GhostBlock, -1, 2, 7, 9, 10),
                new UpgradeSequenceSingle(Upgrade.Speed, -1, 0, 2, 3.5, 10)
                //u[Upgrade.Fireball] =   D(-1,   0, 0,     4,   7);
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, -1, 5, 7.5, 9, 10),
                new UpgradeSequenceSingle(Upgrade.Speed, -1, 2, 4, 6, 7),
                new UpgradeSequenceSingle(Upgrade.Laser, -1, 1, 4, 6, 8.5),
                new UpgradeSequenceSingle(Upgrade.FallingBlock, -1, 6.5, 9, 9, 10),
                new UpgradeSequenceSingle(Upgrade.FlyBlob, -1, 2, 2, 9, 10),
                new UpgradeSequenceSingle(Upgrade.Ceiling, 1, 2, 3, 3, 4)
            ));


            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.BouncyBlock, -1, 2, 4, 7, 10),
                new UpgradeSequenceSingle(Upgrade.FlyBlob, -1, 2, 4, 7, 10),
                new UpgradeSequenceSingle(Upgrade.Spike, -1, 2, 4, 9, 10),
                new UpgradeSequenceSingle(Upgrade.Speed, -1, 0, 0, 6, 10),
                new UpgradeSequenceSingle(Upgrade.FireSpinner, -1, 0, 0, 4, 10),
                new UpgradeSequenceSingle(Upgrade.SpikeyLine, -1, 0, 0, 3, 6)
            ));
        }

        static List<UpgradeSequence> AbusiveUpgrades = new List<UpgradeSequence>();
        static void MakeAbusiveUpgrades()
        {
            List<UpgradeSequence> f = AbusiveUpgrades;
            f.AddRange(NormalUpgrades);

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, -1, -1, 4, 6, 9),
                new UpgradeSequenceSingle(Upgrade.Speed, -1, -1, 4, 6, 9),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, -1, -1, 4, 4, 4),
                new UpgradeSequenceSingle(Upgrade.GhostBlock, -1, -1, 3, 4, 4),
                new UpgradeSequenceSingle(Upgrade.FlyBlob, -1, -1, 4, 4, 4),
                new UpgradeSequenceSingle(Upgrade.Pinky, -1, -1, 2, 4, 7),
                new UpgradeSequenceSingle(Upgrade.Laser, -1, -1, 2, 4, 5.5)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Cloud, -1, -1, 2, 2, 4),
                new UpgradeSequenceSingle(Upgrade.FireSpinner, -1, -1, 4, 8, 10),
                new UpgradeSequenceSingle(Upgrade.FlyBlob, -1, -1, 5, 8, 10),
                new UpgradeSequenceSingle(Upgrade.Jump, -1, -1, 4, 6, 9),
                new UpgradeSequenceSingle(Upgrade.Speed, -1, -1, 2, 4, 6),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, -1, -1, 4, 4, 4),
                new UpgradeSequenceSingle(Upgrade.Ceiling, -1, -1, 4, 7, 10)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, -1, -1, 7, 9, 9),
                new UpgradeSequenceSingle(Upgrade.Speed, -1, -1, 4, 8, 9),
                new UpgradeSequenceSingle(Upgrade.SpikeyGuy, -1, -1, 5.4, 8.5, 10),
                new UpgradeSequenceSingle(Upgrade.FallingBlock, -1, -1, 9, 9, 9),
                new UpgradeSequenceSingle(Upgrade.FlyBlob, -1, -1, 2, 6, 9)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.Jump, -1, -1, 7, 9, 10),
                new UpgradeSequenceSingle(Upgrade.Speed, -1, -1, 4, 6, 9),
                new UpgradeSequenceSingle(Upgrade.FallingBlock, -1, -1, 9, 9, 9),
                new UpgradeSequenceSingle(Upgrade.BouncyBlock, -1, -1, 8, 9, 9),
                new UpgradeSequenceSingle(Upgrade.SpikeyGuy, -1, -1, 3, 6, 10),
                new UpgradeSequenceSingle(Upgrade.Pinky, -1, -1, 4, 7, 10)
            ));

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.FireSpinner, -1, -1, 2, 5, 9),
                new UpgradeSequenceSingle(Upgrade.FlyBlob, -1, -1, 2, 2, 2),
                new UpgradeSequenceSingle(Upgrade.Laser, -1, -1, 2, 4, 6),
                new UpgradeSequenceSingle(Upgrade.GhostBlock, -1, -1, 2, 7, 9),
                new UpgradeSequenceSingle(Upgrade.Speed, -1, -1, 6, 8, 9),
                new UpgradeSequenceSingle(Upgrade.Ceiling, -1, -1, 4, 7, 10)
            ));
        }

        static List<UpgradeSequence> HardcoreUpgrades = new List<UpgradeSequence>();
        static void MakeHardcoreUpgrades()
        {
            List<UpgradeSequence> f = HardcoreUpgrades;
            f.AddRange(AbusiveUpgrades);

            f.Add(new UpgradeSequence(
                new UpgradeSequenceSingle(Upgrade.FireSpinner, -1, -1, -1, 9, 10),
                new UpgradeSequenceSingle(Upgrade.Speed, -1, -1, -1, 5, 8),
                new UpgradeSequenceSingle(Upgrade.MovingBlock, -1, -1, -1, 2, 2)
            ));
        }
    }
}