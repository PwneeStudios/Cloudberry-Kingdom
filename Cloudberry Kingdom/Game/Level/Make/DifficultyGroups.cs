using System;
using System.Collections.Generic;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public delegate float DifficultyFunc(double Easy, double Normal, double Abusive, double Hardcore, double Masochistic);

    public class DifficultyGroups
    {
        /// <summary>
        /// Returns a function that modifies a PieceSeed's difficulty
        /// </summary>
        public static LevelSeedData.CustomDifficulty FixedPieceMod(float Difficulty, LevelSeedData LevelSeed, bool ScreenSaver)
        {
            return piece => FixedPieceSeed(piece, Difficulty, LevelSeed.DefaultHeroType, ScreenSaver);
        }

        public static DifficultyFunc GetFunc(float Difficulty)
        {
            //if (Difficulty < 0) Difficulty = 0;
            float d = Difficulty;

            return
                (Easy, Normal, Abusive, Hardcore, Masochistic) =>
                {
                    //if (d < 0) return 0;
                    if (d < 0) return CoreMath.LerpRestrict((float)0, (float)Easy, d - -1);
                    else if (d < 1) return CoreMath.SpecialLerp((float)Easy, (float)Normal, d - 0);
                    else if (d < 2) return CoreMath.SpecialLerp((float)Normal, (float)Abusive, d - 1);
                    else if (d < 3) return CoreMath.SpecialLerp((float)Abusive, (float)Hardcore, d - 2);
                    else return CoreMath.SpecialLerpRestrict((float)Hardcore, (float)Masochistic, d - 3);
                };
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
        public static void FixedPieceSeed(PieceSeedData piece, float Difficulty, BobPhsx hero, bool ScreenSaver)
        {
            InitFixedUpgrades();

            DifficultyFunc D = GetFunc(Difficulty);

            // Up level
            if (piece.GeometryType == LevelGeometry.Up)
                piece.Rnd.Choose(UpUpgrades)(piece.MyUpgrades1, D);
            // Down level
            else if (piece.GeometryType == LevelGeometry.Down)
                piece.Rnd.Choose(DownUpgrades)(piece.MyUpgrades1, D);
            // Cart level
            else if (hero is BobPhsxRocketbox)
            {
                if (Difficulty < .5f)
                    Difficulty -= .8f;
                else
                    Difficulty -= 1.35f;
                D = GetFunc(Difficulty);
                piece.Rnd.Choose(CartUpgrades)(piece.MyUpgrades1, D);
            }
            // Generic hero level
            else
            {
                Difficulty += HeroDifficultyMod(Difficulty, hero);

                D = GetFunc(Difficulty);

				if (ScreenSaver)
				{
					switch ((int)Difficulty)
					{
						case 0: Tools.GlobalRnd.Choose(EasyUpgrades_SS)(piece.MyUpgrades1, D); break;
						case 1: Tools.GlobalRnd.Choose(NormalUpgrades_SS)(piece.MyUpgrades1, D); break;
						case 2: Tools.GlobalRnd.Choose(AbusiveUpgrades_SS)(piece.MyUpgrades1, D); break;
						default: Tools.GlobalRnd.Choose(HardcoreUpgrades_SS)(piece.MyUpgrades1, D); break;
					}
				}
				else
				{
					switch ((int)Difficulty)
					{
						case 0: Tools.GlobalRnd.Choose(EasyUpgrades)(piece.MyUpgrades1, D); break;
						case 1: Tools.GlobalRnd.Choose(NormalUpgrades)(piece.MyUpgrades1, D); break;
						case 2: Tools.GlobalRnd.Choose(AbusiveUpgrades)(piece.MyUpgrades1, D); break;
						default: Tools.GlobalRnd.Choose(HardcoreUpgrades)(piece.MyUpgrades1, D); break;
					}
				}
            }

            // Mod upgrades to test things here
            //piece.MyUpgrades1[Upgrade.Elevator] = 5;
            //piece.MyUpgrades1.CalcGenData(piece.MyGenData.gen1, piece.Style);
            //piece.MyUpgrades2[Upgrade.Elevator] = 5;
            //piece.MyUpgrades2.CalcGenData(piece.MyGenData.gen1, piece.Style);

			// Extra masochistic
			if (Difficulty > 4.5f)
			{
				float s = CoreMath.Restrict(0, 1, (Difficulty - 4.5f) / 1.5f);

				float rnd = Tools.GlobalRnd.RndFloat(0, 100);
				if (rnd > 80)
					piece.MyUpgrades1[Upgrade.Serpent] = CoreMath.Lerp(piece.MyUpgrades1[Upgrade.Serpent], 10.0f, s);
				else if (rnd > 60)
					piece.MyUpgrades1[Upgrade.LavaDrip] = CoreMath.Lerp(piece.MyUpgrades1[Upgrade.LavaDrip], 10.0f, s);
				else if (rnd > 40)
					piece.MyUpgrades1[Upgrade.Fireball] = CoreMath.Lerp(piece.MyUpgrades1[Upgrade.Fireball], 10.0f, s);

				rnd = Tools.GlobalRnd.RndFloat(0, 100);
				if (rnd > 50)
					piece.MyUpgrades1[Upgrade.Ceiling] = CoreMath.Lerp(piece.MyUpgrades1[Upgrade.Ceiling], 10.0f, s);

				piece.MyUpgrades1[Upgrade.SpikeyGuy] = CoreMath.Lerp(piece.MyUpgrades1[Upgrade.SpikeyGuy], 10.0f, s);
				piece.MyUpgrades1[Upgrade.Pinky] = CoreMath.Lerp(piece.MyUpgrades1[Upgrade.Pinky], 10.0f, s);
				piece.MyUpgrades1[Upgrade.SpikeyLine] = CoreMath.Lerp(piece.MyUpgrades1[Upgrade.SpikeyLine], 10.0f, s);
				piece.MyUpgrades1[Upgrade.Spike] = CoreMath.Lerp(piece.MyUpgrades1[Upgrade.Spike], 10.0f, s);
				
				piece.MyUpgrades1[Upgrade.Speed] = CoreMath.Lerp(piece.MyUpgrades1[Upgrade.Speed], 10.0f, s);
			}

			if (hero is BobPhsxInvert)
			{
				piece.MyUpgrades1[Upgrade.BouncyBlock] /= 4.5f;
			}

            piece.StandardClose();
        }

        static void InitFixedUpgrades()
        {
            if (EasyUpgrades != null)
                return;

            EasyUpgrades = new List<Action<Upgrades, DifficultyFunc>>();
			EasyUpgrades_SS = new List<Action<Upgrades, DifficultyFunc>>();

            // Difficulties
            MakeEasyUpgrades();
            MakeNormalUpgrades();
            MakeAbusiveUpgrades();
            MakeHardcoreUpgrades();

			MakeEasyUpgrades_SS();
			MakeNormalUpgrades_SS();
			MakeAbusiveUpgrades_SS();
			MakeHardcoreUpgrades_SS();


            // Special hero overrides
            MakeCartUpgrades();

            // Up/down overrides
            MakeUpUpgrades();
            MakeDownUpgrades();
        }

		#region Upgrades
		static List<Action<Upgrades, DifficultyFunc>> UpUpgrades = new List<Action<Upgrades, DifficultyFunc>>();
        static void MakeUpUpgrades()
        {
            List<Action<Upgrades, DifficultyFunc>> f = UpUpgrades;

            f.Add((u, D) =>
            {
                u[Upgrade.FlyBlob] = D(0, 2, 5, 7.5, 10);
                u[Upgrade.FallingBlock] = D(1, 3.5, 5, 7.5, 10);
                u[Upgrade.MovingBlock] = D(1, 3.5, 5, 7.5, 10);
                u[Upgrade.GhostBlock] = D(1, 3.5, 5, 7.5, 10);

                u[Upgrade.Jump] = D(0, 3, 5, 7.5, 8);
                u[Upgrade.Speed] = D(0, 3, 5, 8.5, 15);
            });
        }

        static List<Action<Upgrades, DifficultyFunc>> DownUpgrades = new List<Action<Upgrades, DifficultyFunc>>();
        static void MakeDownUpgrades()
        {
            List<Action<Upgrades, DifficultyFunc>> f = DownUpgrades;

            f.Add((u, D) =>
            {
                u[Upgrade.FlyBlob] = D(0, 2, 5, 7.5, 10);
                u[Upgrade.FallingBlock] = D(1, 3.5, 5, 7.5, 10);
                u[Upgrade.MovingBlock] = D(1, 3.5, 5, 7.5, 10);
                u[Upgrade.GhostBlock] = D(1, 3.5, 5, 7.5, 10);

                u[Upgrade.Jump] = D(0, 3, 5, 7.5, 10);
                u[Upgrade.Speed] = D(0, 3, 4, 7, 10);

                u[Upgrade.Laser] = D(0, 1, 2, 5, 7.3);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.FlyBlob] = D(0, 2, 5, 7.5, 10);
                u[Upgrade.FallingBlock] = D(1, 3.5, 5, 7.5, 10);
                u[Upgrade.MovingBlock] = D(1, 3.5, 5, 7.5, 10);
                u[Upgrade.GhostBlock] = D(1, 3.5, 5, 7.5, 10);

                u[Upgrade.Jump] = D(0, 3, 5, 7.5, 10);
                u[Upgrade.Speed] = D(0, 3, 4, 7, 10);

                u[Upgrade.SpikeyLine] = D(0, 1, 2, 5, 7.3);
            });
        }

        static List<Action<Upgrades, DifficultyFunc>> CartUpgrades = new List<Action<Upgrades, DifficultyFunc>>();
        static void MakeCartUpgrades()
        {
            List<Action<Upgrades, DifficultyFunc>> f = CartUpgrades;

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(0, 1, 2, 3, 4);
                u[Upgrade.Speed] = D(1, 2, 3, 4, 5);
                //u[Upgrade.MovingBlock] = D(1, 2, 3, 6, 9);
                u[Upgrade.FallingBlock] = D(1, 2, 3, 6, 9);
                u[Upgrade.FireSpinner] = D(1, 3, 7, 9, 10);
                u[Upgrade.SpikeyLine] = D(0, 2, 3.6, 7, 9);
                u[Upgrade.Laser] = D(1, 3, 6, 7, 8.5);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(0, 1, 2, 3, 4);
                u[Upgrade.Speed] = D(1, 2, 3, 4, 5);
                u[Upgrade.GhostBlock] = D(1, 2, 3, 6, 9);
                u[Upgrade.FallingBlock] = D(1, 2, 3, 6, 9);
                u[Upgrade.SpikeyGuy] = D(1, 2, 3.6, 7, 9);
                u[Upgrade.Spike] = D(2, 3, 7, 9, 9);
                u[Upgrade.Laser] = D(0, 3, 5, 7, 8.5);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(0, 1, 2, 3, 4);
                u[Upgrade.Speed] = D(1, 2, 3, 4, 5);
                u[Upgrade.BouncyBlock] = D(1, 2, 3, 6, 9);
                u[Upgrade.Laser] = D(0, 3, 5, 7, 8.5);
                u[Upgrade.SpikeyLine] = D(1, 2, 3.6, 7, 9);
                u[Upgrade.Pinky] = D(1, 3, 7, 8, 8.5);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(0, 1, 2, 3, 4);
                u[Upgrade.Speed] = D(1, 2, 3, 4, 5);
                //u[Upgrade.FlyBlob] = D(1, 2, 2, 2, 8);
                u[Upgrade.GhostBlock] = D(1, 2, 3, 6, 9);
                //u[Upgrade.MovingBlock] = D(1, 2, 3, 6, 9);
                u[Upgrade.FireSpinner] = D(1, 3, 7, 9, 10);
                u[Upgrade.SpikeyLine] = D(0, 2, 3.6, 7, 9);
                u[Upgrade.Pinky] = D(1, 3, 6, 7, 8.5);
                u[Upgrade.Spike] = D(1, 3, 6, 7, 8.5);
            });
        }

        static List<Action<Upgrades, DifficultyFunc>> EasyUpgrades;
        static void MakeEasyUpgrades()
        {
            List<Action<Upgrades, DifficultyFunc>> f = EasyUpgrades;

			// Pendulum!
			f.Add((u, D) =>
			{
				if (Tools.GlobalRnd.RndFloat(0, 100) > 50)
				{
					u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
					u[Upgrade.Speed] = D(1, 2, 3, 5, 9);
					u[Upgrade.MovingBlock] = D(1, 1, 2.2, 3, 3);
					u[Upgrade.Pendulum] = D(2, 4, 6, 8, 10);
					u[Upgrade.FireSpinner] = D(1, 3, 5, 7, 9);
					u[Upgrade.SpikeyLine] = D(0, 3, 5, 7, 9);
				}
				else
				{
					u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
					u[Upgrade.Speed] = D(1, 2, 3, 5, 9);
					u[Upgrade.Pendulum] = D(2, 4, 6, 8, 10);
					u[Upgrade.FlyBlob] = D(1, 3, 5, 6, 7);
					u[Upgrade.Spike] = D(0, 3, 5, 7, 9.2);
					u[Upgrade.Ceiling] = D(0, 3, 5, 7, 10);
				}
			});


            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
                u[Upgrade.Speed] = D(1, 2, 3, 5, 9);
                u[Upgrade.MovingBlock] = D(1, 1, 2.2, 3, 3);
                u[Upgrade.FallingBlock] = D(1, 1, 2.2, 3, 3);
                u[Upgrade.FlyBlob] = D(1, 1, 2.2, 2.2, 2.2);
                u[Upgrade.FireSpinner] = D(1, 3, 5, 7, 9);
                u[Upgrade.SpikeyLine] = D(0, 3, 5, 7, 9);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
                u[Upgrade.Speed] = D(1, 2, 3, 5, 9);
                u[Upgrade.MovingBlock] = D(1, 1, 2.2, 3, 3);
                u[Upgrade.FallingBlock] = D(1, 1, 2.2, 3, 3);
                u[Upgrade.FireSpinner] = D(1, 3, 5, 7, 9);
                u[Upgrade.Spike] = D(1, 3, 5, 7, 10);
                u[Upgrade.Laser] = D(0, 0, 0, 3, 6);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
                u[Upgrade.Speed] = D(1, 2, 3, 5, 11);
                u[Upgrade.MovingBlock] = D(1, 1, 2.2, 3, 3);
                u[Upgrade.FallingBlock] = D(1, 1, 2.2, 3, 3);
                u[Upgrade.FlyBlob] = D(1, 1, 2.2, 2.2, 2.2);
                u[Upgrade.Pinky] = D(.75f, 3, 5, 7, 9);
                u[Upgrade.Spike] = D(1, 3, 5, 7, 9);
                u[Upgrade.FireSpinner] = D(0, 2, 3, 4, 7);
            });


            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
                u[Upgrade.Speed] = D(1, 2, 3, 5, 11);
                u[Upgrade.MovingBlock] = D(1, 1, 2.2, 3, 3);
                u[Upgrade.BouncyBlock] = D(1, 1, 2.2, 3, 3);
                u[Upgrade.GhostBlock] = D(1, 1, 2.2, 2.2, 2.2);
                u[Upgrade.Pinky] = D(.75f, 3, 5, 7, 9);
                u[Upgrade.Spike] = D(1, 3, 5, 7, 9);
                u[Upgrade.FireSpinner] = D(0, 2, 3, 4, 7);
            });




            // Older

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
                u[Upgrade.Speed] = D(1, 2, 3, 5, 11);
                u[Upgrade.MovingBlock] = D(1, 1, 2.2, 3, 3);
                u[Upgrade.BouncyBlock] = D(1, 1, 2.2, 3, 3);
                u[Upgrade.Elevator] = D(1, 1, 2.2, 2.2, 2.2);
                u[Upgrade.Pinky] = D(.8f, 3, 5, 7, 9);
                u[Upgrade.Spike] = D(1, 3, 5, 7, 9);
                u[Upgrade.Laser] = D(0, 2, 3, 4, 7);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(2, 4.8, 7.0, 8.4, 10);
                u[Upgrade.Speed] = D(1, 2, 8.2, 9.1, 11);
                u[Upgrade.MovingBlock] = D(1, 1, 2.2, 8, 10);
                u[Upgrade.FallingBlock] = D(1, 1, 2.2, 7, 10);
                u[Upgrade.FlyBlob] = D(1, 1, 2.2, 7, 10);
                //u[Upgrade.Fireball] =       D(0,   0,   0,   0,   4);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.SpikeyGuy] = D(2, 4, 5.2, 8, 9);
                u[Upgrade.Jump] = D(3, 2.5, 2, 4, 4.5);
                u[Upgrade.Spike] = D(0, 3, 7.5, 9, 10);
                u[Upgrade.Speed] = D(0, 2, 5.5, 8.8, 9.5);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(3.5f, 1, 0, 0, 0);
                u[Upgrade.SpikeyGuy] = D(0, 3.2, 5.5, 6, 8);
                u[Upgrade.Pinky] = D(1.2f, 3, 5.5, 7, 9);
                u[Upgrade.Spike] = D(0, 0, 0, 4, 10);
                u[Upgrade.Speed] = D(2, 3, 4, 8.8, 9);
                u[Upgrade.Ceiling] = D(1, 2, 4, 7, 10);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Laser] = D(2.5, 4, 5.5, 7.9, 10);
                u[Upgrade.Speed] = D(0, 0, 0, 1, 5);
                u[Upgrade.Ceiling] = D(1, 2, 4, 4, 4);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(3.6, 2, 0, 0, 0);
                u[Upgrade.Speed] = D(0, 0, 0, 1, 3);
                u[Upgrade.FireSpinner] = D(0, 1.5, 4, 6, 9);
                u[Upgrade.Pinky] = D(0, 1.5, 3.6, 5.7, 8);
                u[Upgrade.FallingBlock] = D(0, 1, 4, 6, 8);
                u[Upgrade.Cloud] = D(2, 2.5, 4, 6, 9);
                u[Upgrade.SpikeyGuy] = D(2, 3, 3.5, 5.6, 8.5);
                u[Upgrade.BouncyBlock] = D(0, 0, 4, 6, 8);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Laser] = D(2, 3.5, 4.2, 6, 9);
                u[Upgrade.Speed] = D(0, 1, 1.7, 3, 3);
                u[Upgrade.Elevator] = D(2.8f, 5, 7, 9, 9);
                u[Upgrade.MovingBlock] = D(1.8f, 3, 3, 3, 3);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.SpikeyLine] = D(.7f, 2, 4, 8.4, 9.5);
                u[Upgrade.Speed] = D(0, 2, 3, 4.5, 10);
                u[Upgrade.Elevator] = D(2, 3, 3, 4, 10);
                u[Upgrade.MovingBlock] = D(0, 2, 4, 4, 4);
                u[Upgrade.FlyBlob] = D(0, 2, 4, 4, 4);
                u[Upgrade.FireSpinner] = D(0, 2, 4, 4, 4);
                u[Upgrade.Jump] = D(1, 3, 4, 4, 4);
                u[Upgrade.Cloud] = D(0, 1, 2, 3, 4);
                u[Upgrade.Ceiling] = D(1, 2, 4, 7, 10);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.SpikeyGuy] = D(.7f, 3, 4.5, 7.2, 9);
                u[Upgrade.Speed] = D(.7f, 3, 3.5, 7.5, 9.5);
                u[Upgrade.Elevator] = D(3f, 6, 7, 8, 9);
                u[Upgrade.Laser] = D(0, 0, 0, 0, 4);
                u[Upgrade.Ceiling] = D(1, 2, 4, 7, 10);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.BouncyBlock] = D(3.6f, 8.2, 9, 9, 10);
                u[Upgrade.Spike] = D(2, 7.5, 8.5, 9, 10);
                u[Upgrade.FallingBlock] = D(4f, 2, 2, 3, 4);
                u[Upgrade.Speed] = D(0, 0, 2, 5, 9);
                u[Upgrade.FireSpinner] = D(0, 1, 3, 6, 9);
                u[Upgrade.Pinky] = D(0, 0, 0, 0, 6);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Laser] = D(1.8f, 3, 4, 6, 9.5);
                u[Upgrade.Speed] = D(0, 0, 0, 1, 3);
                u[Upgrade.FireSpinner] = D(1, 3, 6, 9, 9);
                u[Upgrade.Jump] = D(3, 4, 4, 0, 0);
				u[Upgrade.Ceiling] = D(0, 1, 2, 4, 8);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.BouncyBlock] = D(4, 8.2, 9, 9, 10);
                u[Upgrade.Spike] = D(0, 7.5, 8.5, 9, 10);
                u[Upgrade.MovingBlock] = D(0, 2, 2, 4, 9);
                u[Upgrade.Speed] = D(0, 0, 2, 6, 10);
                u[Upgrade.SpikeyLine] = D(0, 0, 0, 0, 4);
                //u[Upgrade.Fireball] =       D(0,   0,   0,  .5,   4);
            });


            f.Add((u, D) =>
            {
                u[Upgrade.FireSpinner] = D(1, 1.5, 2.5, 4, 8);
                u[Upgrade.Pinky] = D(1, 2, 3.5, 6, 10);
                u[Upgrade.MovingBlock] = D(1, 3, 4, 9, 10);
                u[Upgrade.Ceiling] = D(1, 2, 4, 7, 10);
            });
        }

        static List<Action<Upgrades, DifficultyFunc>> NormalUpgrades = new List<Action<Upgrades, DifficultyFunc>>();
        static void MakeNormalUpgrades()
        {
            List<Action<Upgrades, DifficultyFunc>> f = NormalUpgrades;
            f.AddRange(EasyUpgrades);

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(-1, 4.8, 7.5, 9, 10);
                u[Upgrade.Speed] = D(-1, 0, 6, 9, 10);
                u[Upgrade.MovingBlock] = D(-1, 2, 2, 6, 10);
                u[Upgrade.GhostBlock] = D(-1, 2, 2, 6, 10);
                u[Upgrade.BouncyBlock] = D(-1, 2, 2, 4, 10);
                u[Upgrade.FlyBlob] = D(-1, 2, 2, 4, 10);
                u[Upgrade.Spike] = D(-1, 2, 2, 9, 10);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(-1, 4, 4, 4, 4);
                u[Upgrade.Speed] = D(-1, 2, 2, 5, 9);
                u[Upgrade.MovingBlock] = D(-1, 2, 4, 7, 9);
                u[Upgrade.BouncyBlock] = D(-1, 2, 4, 7, 10);
                u[Upgrade.FireSpinner] = D(-1, 2, 4, 6, 9.5);
                u[Upgrade.Laser] = D(-1, 0, 0, 0, 5.5);
            });

            /*
            f.Add((u, D) =>
            {
                u[Upgrade.Speed] =       D(-1, 2, 4, 7, 10);
                u[Upgrade.Fireball] =    D(-1, 2, 4, 7, 10);
                u[Upgrade.Laser] =       D(-1, 2, 3, 4, 8);
                u[Upgrade.FireSpinner] = D(-1, 2, 4, 7, 10);
                u[Upgrade.Spike] =       D(-1, 2, 4, 7, 10);
            });*/

            f.Add((u, D) =>
            {
                u[Upgrade.MovingBlock] = D(-1, 6, 8.5, 9, 10);
                u[Upgrade.Jump] = D(-1, 4, 2, 0, 0);
                u[Upgrade.Speed] = D(-1, 3, 5.5, 7, 8.5);
                u[Upgrade.FireSpinner] = D(-1, 0, 5, 7.5, 9);
                u[Upgrade.Spike] = D(-1, 0, 0, 3, 8.5);
                u[Upgrade.Ceiling] = D(1, 2, 4, 7, 10);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(-1, 0, 0, 5, 8);
                u[Upgrade.FlyBlob] = D(-1, 2, 4, 7, 10);
                u[Upgrade.FallingBlock] = D(-1, 2, 3, 4, 10);
                u[Upgrade.Speed] = D(-1, 4, 6, 9, 10);
                u[Upgrade.GhostBlock] = D(-1, 5, 6, 9, 10);
                //u[Upgrade.Fireball] =       D(-1, 0, 0, 0, 5);
            });
            f.Add((u, D) =>
            {
                u[Upgrade.MovingBlock] = D(-1, 2, 4, 7, 10);
                u[Upgrade.FlyBlob] = D(-1, 2, 4, 7, 10);
                u[Upgrade.FallingBlock] = D(-1, 2, 4, 7, 10);
                u[Upgrade.SpikeyGuy] = D(-1, 1, 3, 6, 7);
                u[Upgrade.FireSpinner] = D(-1, 1, 4, 7, 10);
                u[Upgrade.Pinky] = D(-1, 1, 3, 6, 8);
                u[Upgrade.Speed] = D(-1, 0, 0, 2, 9);
            });

            /*
            f.Add((u, D) =>
            {
                u[Upgrade.Speed] =       D(-1, 0, 1, 3, 10);
                u[Upgrade.Jump] =        D(-1, 3, 5, 8, 10);
                u[Upgrade.Fireball] =    D(-1, 2, 4, 7, 10);
                u[Upgrade.Pinky] =       D(-1, 2, 4, 8, 10);
                u[Upgrade.FlyBlob] =     D(-1, 2, 4, 7, 10);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Speed] =       D(-1, 0, 1, 3, 10);
                u[Upgrade.Fireball] =    D(-1, 2, 4, 7, 10);
                u[Upgrade.Pinky] =       D(-1, 2, 4, 8, 10);
                u[Upgrade.BouncyBlock] = D(-1, 2, 4, 7, 10);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Speed] =       D(-1, 3, 4, 7, 10);
                u[Upgrade.Fireball] =    D(-1, 5, 7, 9, 10);
                u[Upgrade.Spike] =       D(-1, 0, 0, 0, 10);
            });*/

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(-1, 2, 4, 7, 10);
                u[Upgrade.Speed] = D(-1, 2, 3, 5, 10);
                u[Upgrade.MovingBlock] = D(-1, 4, 5, 7, 10);
                u[Upgrade.SpikeyGuy] = D(-1, 1.2, 2.7, 5, 7);
                u[Upgrade.FireSpinner] = D(-1, 1.5, 4, 8, 10);
            });
            /*
            f.Add((u, D) =>
            {
                u[Upgrade.Fireball] =    D(-1, 2,   4,   7,   10);
                u[Upgrade.FireSpinner] = D(-1, 2,   3,   7,   10);
                u[Upgrade.FlyBlob] =     D(-1, 2,   4,   7,   10);
                u[Upgrade.MovingBlock] = D(-1, 4,   6,   9,   10);
                u[Upgrade.Speed] =       D(-1, 0,   1,   3.5,  7);
            });*/

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(-1, 5.1, 7.5, 9, 10);
                u[Upgrade.GhostBlock] = D(-1, 2, 7, 9, 10);
                u[Upgrade.Speed] = D(-1, 0, 2, 3.5, 10);
                //u[Upgrade.Fireball] =   D(-1,   0, 0,     4,   7);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(-1, 5, 7.5, 9, 10);
                u[Upgrade.Speed] = D(-1, 2, 4, 6, 7);
                u[Upgrade.Laser] = D(-1, 1, 4, 6, 8.5);
                u[Upgrade.FallingBlock] = D(-1, 6.5, 9, 9, 10);
                u[Upgrade.FlyBlob] = D(-1, 2, 2, 9, 10);
                u[Upgrade.Ceiling] = D(1, 2, 3, 3, 4);
            });


            f.Add((u, D) =>
            {
                u[Upgrade.BouncyBlock] = D(-1, 2, 4, 7, 10);
                u[Upgrade.FlyBlob] = D(-1, 2, 4, 7, 10);
                u[Upgrade.Spike] = D(-1, 2, 4, 9, 10);
                u[Upgrade.Speed] = D(-1, 0, 0, 6, 10);
                u[Upgrade.FireSpinner] = D(-1, 0, 0, 4, 10);
                u[Upgrade.SpikeyLine] = D(-1, 0, 0, 3, 6);
            });
        }

        static List<Action<Upgrades, DifficultyFunc>> AbusiveUpgrades = new List<Action<Upgrades, DifficultyFunc>>();
        static void MakeAbusiveUpgrades()
        {
            List<Action<Upgrades, DifficultyFunc>> f = AbusiveUpgrades;
            f.AddRange(NormalUpgrades);

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(-1, -1, 4, 6, 9);
                u[Upgrade.Speed] = D(-1, -1, 4, 6, 9);
                u[Upgrade.MovingBlock] = D(-1, -1, 4, 4, 4);
                u[Upgrade.GhostBlock] = D(-1, -1, 3, 4, 4);
                u[Upgrade.FlyBlob] = D(-1, -1, 4, 4, 4);
                u[Upgrade.Pinky] = D(-1, -1, 2, 4, 7);
                u[Upgrade.Laser] = D(-1, -1, 2, 4, 5.5);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Cloud] = D(-1, -1, 2, 2, 4);
                u[Upgrade.FireSpinner] = D(-1, -1, 4, 8, 10);
                u[Upgrade.FlyBlob] = D(-1, -1, 5, 8, 10);
                u[Upgrade.Jump] = D(-1, -1, 4, 6, 9);
                u[Upgrade.Speed] = D(-1, -1, 2, 4, 6);
                u[Upgrade.MovingBlock] = D(-1, -1, 4, 4, 4);
                u[Upgrade.Ceiling] = D(-1, -1, 4, 7, 10);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(-1, -1, 7, 9, 9);
                u[Upgrade.Speed] = D(-1, -1, 4, 8, 9);
                u[Upgrade.SpikeyGuy] = D(-1, -1, 5.4, 7.2, 9);
                u[Upgrade.FallingBlock] = D(-1, -1, 9, 9, 9);
                u[Upgrade.FlyBlob] = D(-1, -1, 2, 6, 9);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(-1, -1, 7, 9, 10);
                u[Upgrade.Speed] = D(-1, -1, 4, 6, 8);
                u[Upgrade.FallingBlock] = D(-1, -1, 9, 9, 9);
                u[Upgrade.BouncyBlock] = D(-1, -1, 8, 9, 9);
                u[Upgrade.SpikeyGuy] = D(-1, -1, 3, 6, 9);
                u[Upgrade.Pinky] = D(-1, -1, 4, 6.8, 9);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.FireSpinner] = D(-1, -1, 2, 5, 9);
                u[Upgrade.FlyBlob] = D(-1, -1, 2, 2, 2);
                u[Upgrade.Laser] = D(-1, -1, 2, 4, 6);
                u[Upgrade.GhostBlock] = D(-1, -1, 2, 7, 9);
                u[Upgrade.Speed] = D(-1, -1, 6, 8, 9);
                u[Upgrade.Ceiling] = D(-1, -1, 4, 7, 10);
            });
        }

        static List<Action<Upgrades, DifficultyFunc>> HardcoreUpgrades = new List<Action<Upgrades, DifficultyFunc>>();
        static void MakeHardcoreUpgrades()
        {
            List<Action<Upgrades, DifficultyFunc>> f = HardcoreUpgrades;
            f.AddRange(AbusiveUpgrades);

            f.Add((u, D) =>
            {
                u[Upgrade.FireSpinner] = D(-1, -1, -1, 9, 10);
                u[Upgrade.Speed] = D(-1, -1, -1, 5, 8);
                u[Upgrade.MovingBlock] = D(-1, -1, -1, 2, 2);
            });

			f.Add((u, D) =>
			{
				u[Upgrade.Pendulum] = D(-1, -1, -1, 9, 10);
				u[Upgrade.Speed] = D(-1, -1, -1, 6, 8);
				u[Upgrade.MovingBlock] = D(-1, -1, -1, 4, 2);
				u[Upgrade.Spike] = D(-1, 0, 0, 0, 7);
				u[Upgrade.Pinky] = D(-1, 0, 0, 0, 9);
			});

			// Serpent!
			f.Add((u, D) =>
			{
				if (Tools.GlobalRnd.RndFloat(0, 100) > 50)
				{
					u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
					u[Upgrade.Speed] = D(1, 2, 3, 5, 11);
					u[Upgrade.MovingBlock] = D(1, 1, 2.2, 3, 3);
					u[Upgrade.FlyBlob] = D(2, 4, 6, 8, 10);
					u[Upgrade.FireSpinner] = D(1, 3, 5, 7, 9);
					u[Upgrade.Serpent] = D(0, 3, 5, 7, 9);
				}
				else
				{
					u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
					u[Upgrade.Speed] = D(1, 2, 3, 5, 11);
					u[Upgrade.Pendulum] = D(2, 4, 6, 8, 10);
					u[Upgrade.Serpent] = D(1, 3, 5, 6.5, 9);
					u[Upgrade.Spike] = D(0, 3, 5, 7, 10);
				}
			});

		}
		#endregion

		#region ScreenSaver upgrades (Old Escalation upgrades)
		static List<Action<Upgrades, DifficultyFunc>> EasyUpgrades_SS;
		static void MakeEasyUpgrades_SS()
		{
			List<Action<Upgrades, DifficultyFunc>> f = EasyUpgrades_SS;

			f.Add((u, D) =>
			{
				u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
				u[Upgrade.Speed] = D(1, 2, 3, 5, 11);
				u[Upgrade.MovingBlock] = D(1, 1, 2.2, 3, 3);
				u[Upgrade.FallingBlock] = D(1, 1, 2.2, 3, 3);
				u[Upgrade.FlyBlob] = D(1, 1, 2.2, 2.2, 2.2);
				u[Upgrade.FireSpinner] = D(1, 3, 5, 7, 9);
				u[Upgrade.SpikeyLine] = D(0, 3, 5, 7, 9);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
				u[Upgrade.Speed] = D(1, 2, 3, 5, 11);
				u[Upgrade.MovingBlock] = D(1, 1, 2.2, 3, 3);
				u[Upgrade.FallingBlock] = D(1, 1, 2.2, 3, 3);
				u[Upgrade.FireSpinner] = D(1, 3, 5, 7, 10);
				u[Upgrade.Spike] = D(1, 3, 5, 7, 10);
				u[Upgrade.Laser] = D(0, 0, 0, 3, 6);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
				u[Upgrade.Speed] = D(1, 2, 3, 5, 11);
				u[Upgrade.MovingBlock] = D(1, 1, 2.2, 3, 3);
				u[Upgrade.FallingBlock] = D(1, 1, 2.2, 3, 3);
				u[Upgrade.FlyBlob] = D(1, 1, 2.2, 2.2, 2.2);
				u[Upgrade.Pinky] = D(.75f, 3, 5, 7, 9);
				u[Upgrade.Spike] = D(1, 3, 5, 7, 9);
				u[Upgrade.FireSpinner] = D(0, 2, 3, 4, 7);
			});


			f.Add((u, D) =>
			{
				u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
				u[Upgrade.Speed] = D(1, 2, 3, 5, 11);
				u[Upgrade.MovingBlock] = D(1, 1, 2.2, 3, 3);
				u[Upgrade.BouncyBlock] = D(1, 1, 2.2, 3, 3);
				u[Upgrade.GhostBlock] = D(1, 1, 2.2, 2.2, 2.2);
				u[Upgrade.Pinky] = D(.75f, 3, 5, 7, 9);
				u[Upgrade.Spike] = D(1, 3, 5, 7, 9);
				u[Upgrade.FireSpinner] = D(0, 2, 3, 4, 7);
			});




			// Older

			f.Add((u, D) =>
			{
				u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
				u[Upgrade.Speed] = D(1, 2, 3, 5, 11);
				u[Upgrade.MovingBlock] = D(1, 1, 2.2, 3, 3);
				u[Upgrade.BouncyBlock] = D(1, 1, 2.2, 3, 3);
				u[Upgrade.Elevator] = D(1, 1, 2.2, 2.2, 2.2);
				u[Upgrade.Pinky] = D(.8f, 3, 5, 7, 9);
				u[Upgrade.Spike] = D(1, 3, 5, 7, 9);
				u[Upgrade.Laser] = D(0, 2, 3, 4, 7);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.Jump] = D(2, 4.8, 7.0, 8.4, 10);
				u[Upgrade.Speed] = D(1, 2, 8.2, 9.1, 11);
				u[Upgrade.MovingBlock] = D(1, 1, 2.2, 8, 10);
				u[Upgrade.FallingBlock] = D(1, 1, 2.2, 7, 10);
				u[Upgrade.FlyBlob] = D(1, 1, 2.2, 7, 10);
				//u[Upgrade.Fireball] =       D(0,   0,   0,   0,   4);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.SpikeyGuy] = D(2, 4, 5.2, 8, 10);
				u[Upgrade.Jump] = D(3, 2.5, 2, 4, 4.5);
				u[Upgrade.Spike] = D(0, 3, 7.5, 9, 10);
				u[Upgrade.Speed] = D(0, 2, 5.5, 8.8, 10);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.Jump] = D(3.5f, 1, 0, 0, 0);
				u[Upgrade.SpikeyGuy] = D(0, 3.2, 5.5, 8, 10);
				u[Upgrade.Pinky] = D(1.2f, 3, 5.5, 8, 10);
				u[Upgrade.Spike] = D(0, 0, 0, 4, 10);
				u[Upgrade.Speed] = D(2, 3, 4, 8.8, 10);
				u[Upgrade.Ceiling] = D(1, 2, 4, 7, 10);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.Laser] = D(2.5, 4, 5.5, 7.9, 10);
				u[Upgrade.Speed] = D(0, 0, 0, 1, 5);
				u[Upgrade.Ceiling] = D(1, 2, 4, 4, 4);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.Jump] = D(3.6, 2, 0, 0, 0);
				u[Upgrade.Speed] = D(0, 0, 0, 1, 3);
				u[Upgrade.FireSpinner] = D(0, 1.5, 4, 6, 9);
				u[Upgrade.Pinky] = D(0, 1.5, 3.6, 5.7, 8);
				u[Upgrade.FallingBlock] = D(0, 1, 4, 6, 8);
				u[Upgrade.Cloud] = D(2, 2.5, 4, 6, 9);
				u[Upgrade.SpikeyGuy] = D(2, 3, 3.5, 5.6, 10);
				u[Upgrade.BouncyBlock] = D(0, 0, 4, 6, 8);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.Laser] = D(2, 3.5, 4.2, 6, 9);
				u[Upgrade.Speed] = D(0, 1, 1.7, 3, 3);
				u[Upgrade.Elevator] = D(2.8f, 5, 7, 9, 9);
				u[Upgrade.MovingBlock] = D(1.8f, 3, 3, 3, 3);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.SpikeyLine] = D(.7f, 2, 4, 8.4, 9.5);
				u[Upgrade.Speed] = D(0, 2, 3, 4.5, 10);
				u[Upgrade.Elevator] = D(2, 3, 3, 4, 10);
				u[Upgrade.MovingBlock] = D(0, 2, 4, 4, 4);
				u[Upgrade.FlyBlob] = D(0, 2, 4, 4, 4);
				u[Upgrade.FireSpinner] = D(0, 2, 4, 4, 4);
				u[Upgrade.Jump] = D(1, 3, 4, 4, 4);
				u[Upgrade.Cloud] = D(0, 1, 2, 3, 4);
				u[Upgrade.Ceiling] = D(1, 2, 4, 7, 10);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.SpikeyGuy] = D(.7f, 3, 4.5, 7.6, 9.5);
				u[Upgrade.Speed] = D(.7f, 3, 3.5, 8, 10);
				u[Upgrade.Elevator] = D(3f, 6, 7, 9, 9);
				u[Upgrade.Laser] = D(0, 0, 0, 0, 4);
				u[Upgrade.Ceiling] = D(1, 2, 4, 7, 10);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.BouncyBlock] = D(3.6f, 8.2, 9, 9, 10);
				u[Upgrade.Spike] = D(2, 7.5, 8.5, 9, 10);
				u[Upgrade.FallingBlock] = D(4f, 2, 2, 3, 4);
				u[Upgrade.Speed] = D(0, 0, 2, 5, 10);
				u[Upgrade.FireSpinner] = D(0, 1, 3, 6, 9);
				u[Upgrade.Pinky] = D(0, 0, 0, 0, 6);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.Laser] = D(1.8f, 3, 4, 6, 9.5);
				u[Upgrade.Speed] = D(0, 0, 0, 1, 3);
				u[Upgrade.FireSpinner] = D(1, 3, 6, 9, 9);
				u[Upgrade.Jump] = D(3, 4, 4, 0, 0);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.BouncyBlock] = D(4, 8.2, 9, 9, 10);
				u[Upgrade.Spike] = D(0, 7.5, 8.5, 9, 10);
				u[Upgrade.MovingBlock] = D(0, 2, 2, 4, 9);
				u[Upgrade.Speed] = D(0, 0, 2, 6, 10);
				u[Upgrade.SpikeyLine] = D(0, 0, 0, 0, 4);
				//u[Upgrade.Fireball] =       D(0,   0,   0,  .5,   4);
			});


			f.Add((u, D) =>
			{
				u[Upgrade.FireSpinner] = D(1, 1.5, 2.5, 4, 8);
				u[Upgrade.Pinky] = D(1, 2, 3.5, 6, 10);
				u[Upgrade.MovingBlock] = D(1, 3, 4, 9, 10);
				u[Upgrade.Ceiling] = D(1, 2, 4, 7, 10);
			});
		}

		static List<Action<Upgrades, DifficultyFunc>> NormalUpgrades_SS = new List<Action<Upgrades, DifficultyFunc>>();
		static void MakeNormalUpgrades_SS()
		{
			List<Action<Upgrades, DifficultyFunc>> f = NormalUpgrades_SS;
			f.AddRange(EasyUpgrades_SS);

			f.Add((u, D) =>
			{
				u[Upgrade.Jump] = D(-1, 4.8, 7.5, 9, 10);
				u[Upgrade.Speed] = D(-1, 0, 6, 9, 10);
				u[Upgrade.MovingBlock] = D(-1, 2, 2, 6, 10);
				u[Upgrade.GhostBlock] = D(-1, 2, 2, 6, 10);
				u[Upgrade.BouncyBlock] = D(-1, 2, 2, 4, 10);
				u[Upgrade.FlyBlob] = D(-1, 2, 2, 4, 10);
				u[Upgrade.Spike] = D(-1, 2, 2, 9, 10);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.Jump] = D(-1, 4, 4, 4, 4);
				u[Upgrade.Speed] = D(-1, 2, 2, 5, 10);
				u[Upgrade.MovingBlock] = D(-1, 2, 4, 7, 10);
				u[Upgrade.BouncyBlock] = D(-1, 2, 4, 7, 10);
				u[Upgrade.FireSpinner] = D(-1, 2, 4, 8, 10);
				u[Upgrade.Laser] = D(-1, 0, 0, 0, 5.5);
			});

			/*
			f.Add((u, D) =>
			{
				u[Upgrade.Speed] =       D(-1, 2, 4, 7, 10);
				u[Upgrade.Fireball] =    D(-1, 2, 4, 7, 10);
				u[Upgrade.Laser] =       D(-1, 2, 3, 4, 8);
				u[Upgrade.FireSpinner] = D(-1, 2, 4, 7, 10);
				u[Upgrade.Spike] =       D(-1, 2, 4, 7, 10);
			});*/

			f.Add((u, D) =>
			{
				u[Upgrade.MovingBlock] = D(-1, 6, 8.5, 9, 10);
				u[Upgrade.Jump] = D(-1, 4, 2, 0, 0);
				u[Upgrade.Speed] = D(-1, 3, 5.5, 9, 9);
				u[Upgrade.FireSpinner] = D(-1, 0, 5, 9, 9);
				u[Upgrade.Spike] = D(-1, 0, 0, 3, 9);
				u[Upgrade.Ceiling] = D(1, 2, 4, 7, 10);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.Jump] = D(-1, 0, 0, 5, 8);
				u[Upgrade.FlyBlob] = D(-1, 2, 4, 7, 10);
				u[Upgrade.FallingBlock] = D(-1, 2, 3, 4, 10);
				u[Upgrade.Speed] = D(-1, 4, 6, 9, 10);
				u[Upgrade.GhostBlock] = D(-1, 5, 6, 9, 10);
				//u[Upgrade.Fireball] =       D(-1, 0, 0, 0, 5);
			});
			f.Add((u, D) =>
			{
				u[Upgrade.MovingBlock] = D(-1, 2, 4, 7, 10);
				u[Upgrade.FlyBlob] = D(-1, 2, 4, 7, 10);
				u[Upgrade.FallingBlock] = D(-1, 2, 4, 7, 10);
				u[Upgrade.SpikeyGuy] = D(-1, 1, 3, 6, 10);
				u[Upgrade.FireSpinner] = D(-1, 1, 4, 7, 10);
				u[Upgrade.Pinky] = D(-1, 1, 3, 6, 10);
				u[Upgrade.Speed] = D(-1, 0, 0, 2, 9);
			});

			/*
			f.Add((u, D) =>
			{
				u[Upgrade.Speed] =       D(-1, 0, 1, 3, 10);
				u[Upgrade.Jump] =        D(-1, 3, 5, 8, 10);
				u[Upgrade.Fireball] =    D(-1, 2, 4, 7, 10);
				u[Upgrade.Pinky] =       D(-1, 2, 4, 8, 10);
				u[Upgrade.FlyBlob] =     D(-1, 2, 4, 7, 10);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.Speed] =       D(-1, 0, 1, 3, 10);
				u[Upgrade.Fireball] =    D(-1, 2, 4, 7, 10);
				u[Upgrade.Pinky] =       D(-1, 2, 4, 8, 10);
				u[Upgrade.BouncyBlock] = D(-1, 2, 4, 7, 10);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.Speed] =       D(-1, 3, 4, 7, 10);
				u[Upgrade.Fireball] =    D(-1, 5, 7, 9, 10);
				u[Upgrade.Spike] =       D(-1, 0, 0, 0, 10);
			});*/

			f.Add((u, D) =>
			{
				u[Upgrade.Jump] = D(-1, 2, 4, 7, 10);
				u[Upgrade.Speed] = D(-1, 2, 3, 5, 10);
				u[Upgrade.MovingBlock] = D(-1, 4, 5, 7, 10);
				u[Upgrade.SpikeyGuy] = D(-1, 1.2, 2.7, 6, 10);
				u[Upgrade.FireSpinner] = D(-1, 1.5, 4, 8, 10);
			});
			/*
			f.Add((u, D) =>
			{
				u[Upgrade.Fireball] =    D(-1, 2,   4,   7,   10);
				u[Upgrade.FireSpinner] = D(-1, 2,   3,   7,   10);
				u[Upgrade.FlyBlob] =     D(-1, 2,   4,   7,   10);
				u[Upgrade.MovingBlock] = D(-1, 4,   6,   9,   10);
				u[Upgrade.Speed] =       D(-1, 0,   1,   3.5,  7);
			});*/

			f.Add((u, D) =>
			{
				u[Upgrade.Jump] = D(-1, 5.1, 7.5, 9, 10);
				u[Upgrade.GhostBlock] = D(-1, 2, 7, 9, 10);
				u[Upgrade.Speed] = D(-1, 0, 2, 3.5, 10);
				//u[Upgrade.Fireball] =   D(-1,   0, 0,     4,   7);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.Jump] = D(-1, 5, 7.5, 9, 10);
				u[Upgrade.Speed] = D(-1, 2, 4, 6, 7);
				u[Upgrade.Laser] = D(-1, 1, 4, 6, 8.5);
				u[Upgrade.FallingBlock] = D(-1, 6.5, 9, 9, 10);
				u[Upgrade.FlyBlob] = D(-1, 2, 2, 9, 10);
				u[Upgrade.Ceiling] = D(1, 2, 3, 3, 4);
			});


			f.Add((u, D) =>
			{
				u[Upgrade.BouncyBlock] = D(-1, 2, 4, 7, 10);
				u[Upgrade.FlyBlob] = D(-1, 2, 4, 7, 10);
				u[Upgrade.Spike] = D(-1, 2, 4, 9, 10);
				u[Upgrade.Speed] = D(-1, 0, 0, 6, 10);
				u[Upgrade.FireSpinner] = D(-1, 0, 0, 4, 10);
				u[Upgrade.SpikeyLine] = D(-1, 0, 0, 3, 6);
			});
		}

		static List<Action<Upgrades, DifficultyFunc>> AbusiveUpgrades_SS = new List<Action<Upgrades, DifficultyFunc>>();
		static void MakeAbusiveUpgrades_SS()
		{
			List<Action<Upgrades, DifficultyFunc>> f = AbusiveUpgrades_SS;
			f.AddRange(NormalUpgrades_SS);

			f.Add((u, D) =>
			{
				u[Upgrade.Jump] = D(-1, -1, 4, 6, 9);
				u[Upgrade.Speed] = D(-1, -1, 4, 6, 9);
				u[Upgrade.MovingBlock] = D(-1, -1, 4, 4, 4);
				u[Upgrade.GhostBlock] = D(-1, -1, 3, 4, 4);
				u[Upgrade.FlyBlob] = D(-1, -1, 4, 4, 4);
				u[Upgrade.Pinky] = D(-1, -1, 2, 4, 7);
				u[Upgrade.Laser] = D(-1, -1, 2, 4, 5.5);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.Cloud] = D(-1, -1, 2, 2, 4);
				u[Upgrade.FireSpinner] = D(-1, -1, 4, 8, 10);
				u[Upgrade.FlyBlob] = D(-1, -1, 5, 8, 10);
				u[Upgrade.Jump] = D(-1, -1, 4, 6, 9);
				u[Upgrade.Speed] = D(-1, -1, 2, 4, 6);
				u[Upgrade.MovingBlock] = D(-1, -1, 4, 4, 4);
				u[Upgrade.Ceiling] = D(-1, -1, 4, 7, 10);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.Jump] = D(-1, -1, 7, 9, 9);
				u[Upgrade.Speed] = D(-1, -1, 4, 8, 9);
				u[Upgrade.SpikeyGuy] = D(-1, -1, 5.4, 8.5, 10);
				u[Upgrade.FallingBlock] = D(-1, -1, 9, 9, 9);
				u[Upgrade.FlyBlob] = D(-1, -1, 2, 6, 9);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.Jump] = D(-1, -1, 7, 9, 10);
				u[Upgrade.Speed] = D(-1, -1, 4, 6, 9);
				u[Upgrade.FallingBlock] = D(-1, -1, 9, 9, 9);
				u[Upgrade.BouncyBlock] = D(-1, -1, 8, 9, 9);
				u[Upgrade.SpikeyGuy] = D(-1, -1, 3, 6, 10);
				u[Upgrade.Pinky] = D(-1, -1, 4, 7, 10);
			});

			f.Add((u, D) =>
			{
				u[Upgrade.FireSpinner] = D(-1, -1, 2, 5, 9);
				u[Upgrade.FlyBlob] = D(-1, -1, 2, 2, 2);
				u[Upgrade.Laser] = D(-1, -1, 2, 4, 6);
				u[Upgrade.GhostBlock] = D(-1, -1, 2, 7, 9);
				u[Upgrade.Speed] = D(-1, -1, 6, 8, 9);
				u[Upgrade.Ceiling] = D(-1, -1, 4, 7, 10);
			});
		}

		static List<Action<Upgrades, DifficultyFunc>> HardcoreUpgrades_SS = new List<Action<Upgrades, DifficultyFunc>>();
		static void MakeHardcoreUpgrades_SS()
		{
			List<Action<Upgrades, DifficultyFunc>> f = HardcoreUpgrades_SS;
			f.AddRange(AbusiveUpgrades_SS);

			f.Add((u, D) =>
			{
				u[Upgrade.FireSpinner] = D(-1, -1, -1, 9, 10);
				u[Upgrade.Speed] = D(-1, -1, -1, 5, 8);
				u[Upgrade.MovingBlock] = D(-1, -1, -1, 2, 2);
			});
		}
		#endregion
	}
}

#if FALSE
using System;
using System.Collections.Generic;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public delegate float DifficultyFunc(double Easy, double Normal, double Abusive, double Hardcore, double Masochistic);

    public class DifficultyGroups
    {
        /// <summary>
        /// Returns a function that modifies a PieceSeed's difficulty
        /// </summary>
        public static LevelSeedData.CustomDifficulty FixedPieceMod(float Difficulty, LevelSeedData LevelSeed)
        {
            return piece => FixedPieceSeed(piece, Difficulty, LevelSeed.DefaultHeroType);
        }

        public static DifficultyFunc GetFunc(float Difficulty)
        {
            //if (Difficulty < 0) Difficulty = 0;
            float d = Difficulty;

            return
                (Easy, Normal, Abusive, Hardcore, Masochistic) =>
                {
                    //if (d < 0) return 0;
                    if (d < 0) return CoreMath.LerpRestrict((float)0, (float)Easy, d - -1);
                    else if (d < 1) return CoreMath.SpecialLerp((float)Easy, (float)Normal, d - 0);
                    else if (d < 2) return CoreMath.SpecialLerp((float)Normal, (float)Abusive, d - 1);
                    else if (d < 3) return CoreMath.SpecialLerp((float)Abusive, (float)Hardcore, d - 2);
                    else return CoreMath.SpecialLerpRestrict((float)Hardcore, (float)Masochistic, d - 3);
                };
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

            DifficultyFunc D = GetFunc(Difficulty);

            // Up level
            if (piece.GeometryType == LevelGeometry.Up)
                piece.Rnd.Choose(UpUpgrades)(piece.MyUpgrades1, D);
            // Down level
            else if (piece.GeometryType == LevelGeometry.Down)
                piece.Rnd.Choose(DownUpgrades)(piece.MyUpgrades1, D);
            // Cart level
            else if (hero is BobPhsxRocketbox)
            {
                if (Difficulty < .5f)
                    Difficulty -= .8f;
                else
                    Difficulty -= 1.35f;
                D = GetFunc(Difficulty);
                piece.Rnd.Choose(CartUpgrades)(piece.MyUpgrades1, D);
            }
            // Generic hero level
            else
            {
                Difficulty += HeroDifficultyMod(Difficulty, hero);

                D = GetFunc(Difficulty);

                switch ((int)Difficulty)
                {
                    case 0: piece.Rnd.Choose(EasyUpgrades)(piece.MyUpgrades1, D); break;
                    case 1: piece.Rnd.Choose(NormalUpgrades)(piece.MyUpgrades1, D); break;
                    case 2: piece.Rnd.Choose(AbusiveUpgrades)(piece.MyUpgrades1, D); break;
                    default: piece.Rnd.Choose(HardcoreUpgrades)(piece.MyUpgrades1, D); break;
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

            EasyUpgrades = new List<Action<Upgrades, DifficultyFunc>>();

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

        static List<Action<Upgrades, DifficultyFunc>> UpUpgrades = new List<Action<Upgrades, DifficultyFunc>>();
        static void MakeUpUpgrades()
        {
            List<Action<Upgrades, DifficultyFunc>> f = UpUpgrades;

            f.Add((u, D) =>
            {
                u[Upgrade.FlyBlob] = D(0, 2, 5, 7.5, 10);
                u[Upgrade.FallingBlock] = D(1,3.5, 5, 7.5, 10);
                u[Upgrade.MovingBlock] = D(1,3.5, 5, 7.5, 10);
                u[Upgrade.GhostBlock] = D(1,3.5, 5, 7.5, 10);
                
                u[Upgrade.Jump] = D(0, 3, 5, 7.5, 8);
                u[Upgrade.Speed] = D(0, 3, 5, 8.5, 15);
            });
        }

        static List<Action<Upgrades, DifficultyFunc>> DownUpgrades = new List<Action<Upgrades, DifficultyFunc>>();
        static void MakeDownUpgrades()
        {
            List<Action<Upgrades, DifficultyFunc>> f = DownUpgrades;

            f.Add((u, D) =>
            {
                u[Upgrade.FlyBlob] = D(0, 2, 5, 7.5, 10);
                u[Upgrade.FallingBlock] = D(1, 3.5, 5, 7.5, 10);
                u[Upgrade.MovingBlock] = D(1, 3.5, 5, 7.5, 10);
                u[Upgrade.GhostBlock] = D(1, 3.5, 5, 7.5, 10);

                u[Upgrade.Jump] = D(0, 3, 5, 7.5, 10);
                u[Upgrade.Speed] = D(0, 3, 4, 7, 10);

                u[Upgrade.Laser] = D(0,1,2, 5, 7.3);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.FlyBlob] = D(0, 2, 5, 7.5, 10);
                u[Upgrade.FallingBlock] = D(1, 3.5, 5, 7.5, 10);
                u[Upgrade.MovingBlock] = D(1, 3.5, 5, 7.5, 10);
                u[Upgrade.GhostBlock] = D(1, 3.5, 5, 7.5, 10);

                u[Upgrade.Jump] = D(0, 3, 5, 7.5, 10);
                u[Upgrade.Speed] = D(0, 3, 4, 7, 10);

                u[Upgrade.SpikeyLine] = D(0, 1, 2, 5, 7.3);
            });
        }

        static List<Action<Upgrades, DifficultyFunc>> CartUpgrades = new List<Action<Upgrades, DifficultyFunc>>();
        static void MakeCartUpgrades()
        {
            List<Action<Upgrades, DifficultyFunc>> f = CartUpgrades;

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(0, 1, 2, 3, 4);
                u[Upgrade.Speed] = D(1, 2, 3, 4, 5);
                //u[Upgrade.MovingBlock] = D(1, 2, 3, 6, 9);
                u[Upgrade.FallingBlock] = D(1, 2, 3, 6, 9);
                u[Upgrade.FireSpinner] = D(1, 3, 7, 9, 10);
                u[Upgrade.SpikeyLine] = D(0, 2,3.6, 7, 9);
                u[Upgrade.Laser] = D(1, 3, 6, 7, 8.5);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(0, 1, 2, 3, 4);
                u[Upgrade.Speed] = D(1, 2, 3, 5.5, 8);
                u[Upgrade.GhostBlock] = D(1, 2, 3, 6, 9);
                u[Upgrade.FallingBlock] = D(1, 2, 3, 6, 9);
                u[Upgrade.SpikeyGuy] = D(1, 2, 3.6, 6.5, 9);
                u[Upgrade.Spike] = D(2, 3, 7, 8, 9);
                u[Upgrade.Laser] = D(0, 3, 5, 7, 8.5);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(0, 1, 2, 3, 4);
                u[Upgrade.Speed] = D(1, 2, 3, 4, 5);
                u[Upgrade.BouncyBlock] = D(1, 2, 3, 6, 9);
                u[Upgrade.Laser] = D(0, 3, 5, 7, 8.5);
                u[Upgrade.SpikeyLine] = D(1, 2, 3.6, 7, 9);
                u[Upgrade.Pinky] = D(1, 3, 7, 8, 8.5);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(0, 1, 2, 3, 4);
                u[Upgrade.Speed] = D(1, 2, 3, 4, 5);
                //u[Upgrade.FlyBlob] = D(1, 2, 2, 2, 8);
                u[Upgrade.GhostBlock] = D(1, 2, 3, 6, 9);
                //u[Upgrade.MovingBlock] = D(1, 2, 3, 6, 9);
                u[Upgrade.FireSpinner] = D(1, 3, 7, 9, 10);
                u[Upgrade.SpikeyLine] = D(0, 2, 3.6, 7, 9);
                u[Upgrade.Pinky] = D(1, 3, 6, 7, 8.5);
                u[Upgrade.Spike] = D(1, 3, 6, 7, 8.5);
            });
        }

        static List<Action<Upgrades, DifficultyFunc>> EasyUpgrades;
        static void MakeEasyUpgrades()
        {
            List<Action<Upgrades, DifficultyFunc>> f = EasyUpgrades;

            // New obstacles
            f.Add((u, D) =>
            {
                u[Upgrade.Jump] =     D(2, 5, 3, 2, 0);
                u[Upgrade.Speed] =    D(0, 1, 2, 7, 11);
                u[Upgrade.Serpent] =  D(1, 1, 4, 7, 9);
                u[Upgrade.LavaDrip] = D(1, 1, 4, 7, 9);
                u[Upgrade.Pendulum] = D(1, 1, 4, 7, 9);
                u[Upgrade.Fireball] = D(0, 0, 0, 0, 9);
            });


            // Standard obstacles


            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
                u[Upgrade.Speed] = D(1, 2, 3, 6, 11);
                u[Upgrade.MovingBlock] = D(1, 1, 2.2, 3, 3);
                u[Upgrade.FallingBlock] = D(1, 1, 2.2, 3, 3);
                u[Upgrade.FlyBlob] = D(1, 1, 2.2, 2.2, 2.2);
                u[Upgrade.FireSpinner] = D(1, 3, 5, 7, 9);
                u[Upgrade.SpikeyLine] = D(0, 3, 5, 7, 9);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
                u[Upgrade.Speed] = D(1, 2, 3, 6, 11);
                u[Upgrade.MovingBlock] = D(1, 1, 2.2, 3, 6);
                u[Upgrade.FallingBlock] = D(1, 1, 2.2, 3, 3);
                u[Upgrade.FireSpinner] = D(1, 3, 5, 7, 10);
                u[Upgrade.Spike] = D(1, 3, 5, 7, 10);
                u[Upgrade.Laser] = D(0, 0, 0, 3, 8);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
                u[Upgrade.Speed] = D(1, 2, 3, 6, 11);
                u[Upgrade.MovingBlock] = D(1, 1, 2.2, 3, 6);
                u[Upgrade.FallingBlock] = D(1, 1, 2.2, 3, 3);
                u[Upgrade.FlyBlob] = D(1, 1, 2.2, 2.2, 2.2);
                u[Upgrade.Pinky] = D(.75f, 3, 5, 7, 9);
                u[Upgrade.Spike] = D(1, 3, 5, 7, 9);
                u[Upgrade.FireSpinner] = D(0, 2, 3, 5, 9);
            });


            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
                u[Upgrade.Speed] = D(1, 2, 3, 6, 11);
                u[Upgrade.MovingBlock] = D(1, 1, 2.2, 3, 3);
                u[Upgrade.BouncyBlock] = D(1, 1, 2.2, 3, 3);
                u[Upgrade.GhostBlock] = D(1, 1, 2.2, 2.2, 2.2);
                u[Upgrade.Pinky] = D(.75f, 3, 5, 7, 9);
                u[Upgrade.Spike] = D(1, 3, 5, 7, 9);
                u[Upgrade.FireSpinner] = D(0, 2, 3, 6, 9);
            });




            // Older

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] = D(2, 5, 5, 5, 5);
                u[Upgrade.Speed] = D(1, 2, 3, 6, 11);
                u[Upgrade.MovingBlock] = D(1, 1, 2.2, 3, 3);
                u[Upgrade.BouncyBlock] = D(1, 1, 2.2, 3, 3);
                u[Upgrade.Elevator] = D(1, 1, 2.2, 2.2, 2.2);
                u[Upgrade.Pinky] = D(.8f, 3, 5, 7, 9);
                u[Upgrade.Spike] = D(1, 3, 5, 7, 9);
                //u[Upgrade.Laser] = D(0, 2, 3, 4, 8);
                u[Upgrade.Serpent] = D(0, 0, 0, 2, 9);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] =           D(2, 4.8,   7.0, 8.4, 10);
                u[Upgrade.Speed] =          D(1,   2,   8.2, 9.1, 11);
                u[Upgrade.MovingBlock] =    D(1,   1,   2.2, 8,   10);
                u[Upgrade.FallingBlock] =   D(1,   1,   2.2, 7,   10);
                u[Upgrade.FlyBlob] =        D(1,   1,   2.2, 7,   10);
                u[Upgrade.LavaDrip] =       D(0,   0,   0,   3,   8);
                u[Upgrade.Fireball] =       D(0,   0,   0,   0,   8);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Serpent] = D(0, 0, 0, 2, 9);
                u[Upgrade.SpikeyGuy] =      D(2,   4, 5,   7,   10);
                u[Upgrade.Jump] =           D(3, 2.5,   2,   4,   4.5);
                u[Upgrade.Spike] =          D(0,   3, 7,   8,   10);
                u[Upgrade.Speed] =          D(0,   2, 5,   8,   10);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] =      D(3.5f, 1, 0,  0,   0);
                u[Upgrade.SpikeyGuy] = D(0,3.2,5,   6.5,   10);
                u[Upgrade.Pinky] =     D(1.2f, 3,5.5, 6.5,   10);
                u[Upgrade.Spike] =     D(0, 0, 0, 6,       10);
                u[Upgrade.Speed] =     D(2, 3, 4, 8,     10);
                u[Upgrade.Ceiling] =   D(1, 2, 4, 7,       10);
                u[Upgrade.LavaDrip] =  D(0, 0, 0, 2, 7);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Laser] =          D(2.5, 4, 5.5, 7.9,   10);
                u[Upgrade.Speed] =          D(0,   0,   0,   5,    8);
                u[Upgrade.Ceiling] =        D(1,   2,   4,   4,   10);
                u[Upgrade.Fireball] =       D(0,   0,   0,   0,   4);
                u[Upgrade.SpikeyLine] =     D(0,   0,   0,   2,   7);
                u[Upgrade.Cloud] =          D(0,   0,   0,   0,   8);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] =           D(3.6, 2,   0,   0,    0);
                u[Upgrade.Speed] =          D(0,   0,   0,   6,    8);
                u[Upgrade.FireSpinner] =    D(0, 1.5,   4,   6,    9);
                u[Upgrade.Pinky] =          D(0, 1.5, 3.6, 5.7,    9);
                u[Upgrade.FallingBlock] =   D(0,   1,   4,   6,    8);
                u[Upgrade.Cloud] =          D(2, 2.5,   4,   6,    9);
                u[Upgrade.SpikeyGuy] =      D(2,   3, 3.5, 5.6,   10);
                u[Upgrade.BouncyBlock] =    D(0,   0,   4,   6,    8);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Laser] =          D(2, 3.5, 4.2,   6,    9);
                u[Upgrade.Speed] =          D(0,   1, 1.7,   5,    8);
                u[Upgrade.Elevator] =       D(2.8f,   5,   7,   9,    9);
                u[Upgrade.MovingBlock] =    D(1.8f,   3,   3,   3,    3);
                u[Upgrade.Pendulum] =       D(0,   0,   0,   0,   8);
                u[Upgrade.SpikeyLine] =     D(0,   0,   0,   2,   7);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.SpikeyLine] =     D(.7f,   2,   4, 8.4,10);
                u[Upgrade.Speed] =          D(0,   2,   3,   7,  10);
                u[Upgrade.Elevator] =       D(2,   3,   3,   4,  10);
                u[Upgrade.MovingBlock] =    D(0,   2,   4,   4,  10);
                u[Upgrade.FlyBlob] =        D(0,   2,   4,   4,   4);
                u[Upgrade.FireSpinner] =    D(0,   2,   4,   6,  10);
                u[Upgrade.Jump] =           D(1,   3,   4,   4,   4);
                u[Upgrade.Cloud] =          D(0,   1,   2,   3,   4);
                u[Upgrade.Ceiling] =        D(1,   2,   4,   7,   10);
                u[Upgrade.SpikeyGuy] =      D(0,   0,   0,   2,   6);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.SpikeyGuy] =      D(.7f,   3, 4.5,   7,  9.5);
                u[Upgrade.Speed] =          D(.7f,   3, 3.5,   8,   10);
                u[Upgrade.Elevator] =       D(3f, 6,   7,   9,    9);
                u[Upgrade.Laser] =          D(0,   0,   0,   0,    6);
                u[Upgrade.Ceiling] =        D(1,   2,   4,   7,   10);
                u[Upgrade.BouncyBlock] =     D(0,   0,   0,   0,   8);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.BouncyBlock] =    D(3.6f, 8.2,   9,   9,   10);
                u[Upgrade.Spike] =          D(2, 7.5, 8.5,   8,   10);
                u[Upgrade.FallingBlock] =   D(4f,   2,   2,   3,    4);
                u[Upgrade.Speed] =          D(0,   0,   2,   6,   10);
                u[Upgrade.FireSpinner] =    D(0,   1,   3,   6,   10);
                u[Upgrade.Pinky] =          D(0,   0,   0,   2,   10);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Laser] =          D(1.8f,   3,   4,   6,  9.5);
                u[Upgrade.Speed] =          D(0,   0,   0,   5,    8);
                u[Upgrade.FireSpinner] =    D(1,   3,   6,   9,    9);
                u[Upgrade.Jump] =           D(3,   4,   4,   0,    0);
                u[Upgrade.Spike] =          D(0,   0,   0,   3,   7);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.BouncyBlock] =    D(4, 8.2,   9,   9,   10);
                u[Upgrade.Spike] =          D(0, 7.5, 8.5,   8,   10);
                u[Upgrade.MovingBlock] =    D(0,   2,   2,   4,    9);
                u[Upgrade.Speed] =          D(0,   0,   2,   6,   10);
                u[Upgrade.SpikeyLine] =     D(0,   0,   0,   2,   5);
                u[Upgrade.Fireball] =       D(0,   0,   0,   0,   8);
            });


            f.Add((u, D) =>
            {
                u[Upgrade.FireSpinner] = D(1, 1.5, 2.5, 4, 9);
                u[Upgrade.Pinky] =       D(1, 2,   3.5, 6, 10);
                u[Upgrade.MovingBlock] = D(1, 3,   4,   9, 10);
                u[Upgrade.Ceiling] =     D(1, 2,   4,   7, 10);
                u[Upgrade.Spike] =          D(0,   0,   0,   2,   6);
                u[Upgrade.Speed] =          D(0,   0,   0,   4,   8);
            });
        }

        static List<Action<Upgrades, DifficultyFunc>> NormalUpgrades = new List<Action<Upgrades, DifficultyFunc>>();
        static void MakeNormalUpgrades()
        {
            List<Action<Upgrades, DifficultyFunc>> f = NormalUpgrades;
            f.AddRange(EasyUpgrades);

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] =        D(-1,4.8,7.5, 9,  10);
                u[Upgrade.Speed] =       D(-1, 0, 6,   9,  10);
                u[Upgrade.MovingBlock] = D(-1, 2, 2,   6,  10);
                u[Upgrade.GhostBlock] =  D(-1, 2, 2,   6,  10);
                u[Upgrade.BouncyBlock] = D(-1, 2, 2,   4,  10);
                u[Upgrade.FlyBlob] =     D(-1, 2, 2,   4,  10);
                u[Upgrade.Spike] =       D(-1, 2, 2, 8,  10);
                u[Upgrade.Fireball] =      D(0,   0,   0,   0,   7.5);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] =           D(-1, 4, 4, 4, 4);
                u[Upgrade.Speed] =          D(-1, 2, 2, 6, 10);
                u[Upgrade.MovingBlock] =    D(-1, 2, 4, 7, 10);
                u[Upgrade.BouncyBlock] =    D(-1, 2, 4, 7, 7);
                u[Upgrade.FireSpinner] =    D(-1, 2, 4, 8, 10);
                u[Upgrade.Laser] =          D(-1, 0, 0, 0, 6.5);
                u[Upgrade.Serpent] =        D(0,   0,   0,   3,   6);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.MovingBlock] = D(-1, 6, 8.5, 9,  10);  
                u[Upgrade.Jump] =        D(-1, 4,   2,   0,   0);
                u[Upgrade.Speed] =       D(-1, 3, 5.5,   9,   9);
                u[Upgrade.FireSpinner] = D(-1, 0,   5,   9,   9);
                u[Upgrade.Spike] =       D(-1, 0,   0,   3,   9);
                u[Upgrade.Ceiling] =     D(1,  2,   4,   7,   10);
                u[Upgrade.Fireball] =    D(0,   0,   0,   0,   7);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] =           D(-1, 0, 0, 5, 8);
                u[Upgrade.FlyBlob] =        D(-1, 2, 4, 7, 10);
                u[Upgrade.FallingBlock] =   D(-1, 2, 3, 4, 7);
                u[Upgrade.Speed] =          D(-1, 4, 6, 9, 10);
                u[Upgrade.GhostBlock] =     D(-1, 5, 6, 9, 7);
                u[Upgrade.Pendulum] =       D(-1, 0, 0, 0, 10);
                u[Upgrade.LavaDrip] =       D(0, 0, 0, 0, 5);
            });
            f.Add((u, D) =>
            {
                u[Upgrade.MovingBlock] =  D(-1, 2, 4, 7, 10);
                u[Upgrade.FlyBlob] =      D(-1, 2, 4, 7, 10);
                u[Upgrade.FallingBlock] = D(-1, 2, 4, 7, 10);
                u[Upgrade.SpikeyGuy] =    D(-1, 1, 3, 6, 10);
                u[Upgrade.FireSpinner] =  D(-1, 1, 4, 6, 10);
                u[Upgrade.Pinky] =        D(-1, 1, 3, 6, 10);
                u[Upgrade.Speed] =        D(-1, 0, 0, 5,  9);
            });

            /*
            f.Add((u, D) =>
            {
                u[Upgrade.Speed] =       D(-1, 0, 1, 3, 10);
                u[Upgrade.Jump] =        D(-1, 3, 5, 8, 10);
                u[Upgrade.Fireball] =    D(-1, 2, 4, 7, 10);
                u[Upgrade.Pinky] =       D(-1, 2, 4, 8, 10);
                u[Upgrade.FlyBlob] =     D(-1, 2, 4, 7, 10);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Speed] =       D(-1, 0, 1, 3, 10);
                u[Upgrade.Fireball] =    D(-1, 2, 4, 7, 10);
                u[Upgrade.Pinky] =       D(-1, 2, 4, 8, 10);
                u[Upgrade.BouncyBlock] = D(-1, 2, 4, 7, 10);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Speed] =       D(-1, 3, 4, 7, 10);
                u[Upgrade.Fireball] =    D(-1, 5, 7, 9, 10);
                u[Upgrade.Spike] =       D(-1, 0, 0, 0, 10);
            });*/

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] =           D(-1, 2,   4, 7, 10);
                u[Upgrade.Speed] =          D(-1, 2,   3, 7, 10);
                u[Upgrade.MovingBlock] =    D(-1, 4,   5, 7, 10);
                u[Upgrade.SpikeyGuy] =      D(-1, 1.2,2.7,6, 10);
                u[Upgrade.FireSpinner] =    D(-1, 1.5, 4, 8, 10);
                u[Upgrade.SpikeyLine] =       D(-1, 0, 0, 2, 10);
                u[Upgrade.Laser] =       D(-1, 0, 0, 0, 5);
            });

            /*
            f.Add((u, D) =>
            {
                u[Upgrade.Fireball] =    D(-1, 2,   4,   7,   10);
                u[Upgrade.FireSpinner] = D(-1, 2,   3,   7,   10);
                u[Upgrade.FlyBlob] =     D(-1, 2,   4,   7,   10);
                u[Upgrade.MovingBlock] = D(-1, 4,   6,   9,   10);
                u[Upgrade.Speed] =       D(-1, 0,   1,   3.5,  7);
            });*/

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] =       D(-1, 5.1, 7.5,   9,   10);
                u[Upgrade.GhostBlock] = D(-1,   2, 7,     9,   10);
                u[Upgrade.Speed] =      D(-1,   0, 2,   3.5,   10);
                u[Upgrade.Spike] =       D(-1, 0, 0, 0, 10);
                u[Upgrade.LavaDrip] =       D(-1, 0, 0, 0, 10);
                //u[Upgrade.Fireball] =   D(-1,   0, 0,     4,   7);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] =         D(-1, 5, 7.5, 9, 10);
                u[Upgrade.Speed] =        D(-1, 2,   4,   6, 8.5);
                u[Upgrade.Laser] =        D(-1, 1,   4,   6, 8.5);
                u[Upgrade.FallingBlock] = D(-1, 6.5, 9,   9, 10);
                u[Upgrade.FlyBlob] =      D(-1, 2,   2,   9, 10);
                u[Upgrade.Ceiling] =      D(1,  2,   3,   3,   7);
                u[Upgrade.Serpent] =       D(-1, 0, 0, 0, 5);
                u[Upgrade.Spike] =       D(-1, 0, 0, 2, 6);
            });


            f.Add((u, D) =>
            {
                u[Upgrade.BouncyBlock] = D(-1, 2, 4, 7, 10);
                u[Upgrade.FlyBlob] =     D(-1, 2, 4, 7, 10);
                u[Upgrade.Spike] =       D(-1, 2, 4, 8, 10);
                u[Upgrade.Speed] =       D(-1, 0, 0, 7, 10);
                u[Upgrade.FireSpinner] = D(-1, 0, 0, 4, 10);
                u[Upgrade.SpikeyLine] =  D(-1, 0, 0, 3, 6);
                u[Upgrade.MovingBlock] =       D(-1, 0, 0, 2, 10);
                u[Upgrade.Laser] =       D(-1, 0, 0, 0, 7);
            });
        }

        static List<Action<Upgrades, DifficultyFunc>> AbusiveUpgrades = new List<Action<Upgrades, DifficultyFunc>>();
        static void MakeAbusiveUpgrades()
        {
            List<Action<Upgrades, DifficultyFunc>> f = AbusiveUpgrades;
            f.AddRange(NormalUpgrades);

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] =           D(-1, -1, 4, 6, 9);
                u[Upgrade.Speed] =          D(-1, -1, 4, 6, 9);
                u[Upgrade.MovingBlock] =    D(-1, -1, 4, 4, 7);
                u[Upgrade.GhostBlock] =     D(-1, -1, 3, 4, 4);
                u[Upgrade.FlyBlob] =        D(-1, -1, 4, 4, 4);
                u[Upgrade.Pinky] =          D(-1, -1, 2, 4, 7.5);
                u[Upgrade.Laser] =          D(-1, -1, 2, 4, 6.5);
                u[Upgrade.Spike] =       D(-1, 0, 0, 0, 7.5);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Cloud] =          D(-1, -1, 2, 2, 4);
                u[Upgrade.FireSpinner] =    D(-1, -1, 4, 8, 10);
                u[Upgrade.FlyBlob] =        D(-1, -1, 5, 8, 10);
                u[Upgrade.Jump] =           D(-1, -1, 4, 6, 9);
                u[Upgrade.Speed] =          D(-1, -1, 2, 5.5, 9);
                u[Upgrade.MovingBlock] =    D(-1, -1, 4, 4, 4);
                u[Upgrade.Ceiling] =        D(-1, -1, 4, 7, 10);
                u[Upgrade.Fireball] =       D(-1, 0, 0, 0, 8);
                u[Upgrade.SpikeyLine] =       D(-1, 0, 0, 2, 6);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] =           D(-1, -1, 7,   9, 9);
                u[Upgrade.Speed] =          D(-1, -1, 4,   8, 9);
                u[Upgrade.SpikeyGuy] =      D(-1, -1,5,  7, 10);
                u[Upgrade.FallingBlock] =   D(-1, -1, 9,   9, 9);
                u[Upgrade.FlyBlob] =        D(-1, -1, 2,   6, 9);
                u[Upgrade.LavaDrip] =       D(-1, 0, 0, 3, 9);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.Jump] =           D(-1, -1, 7, 9, 10);
                u[Upgrade.Speed] =          D(-1, -1, 4, 7, 9);
                u[Upgrade.FallingBlock] =   D(-1, -1, 9, 9, 9);
                u[Upgrade.BouncyBlock] =    D(-1, -1, 8, 9, 9);
                u[Upgrade.SpikeyGuy] =      D(-1, -1, 3, 6, 6);
                u[Upgrade.Pinky] =          D(-1, -1, 4, 6, 10);
                u[Upgrade.Spike] =       D(-1, 0, 0, 2, 8);
                u[Upgrade.Laser] =       D(-1, 0, 0, 2, 6);
            });

            f.Add((u, D) =>
            {
                u[Upgrade.FireSpinner] = D(-1, -1, 2, 5, 9);
                u[Upgrade.FlyBlob] =     D(-1, -1, 2, 2, 2);
                u[Upgrade.Laser] =       D(-1, -1, 2, 4, 6);
                u[Upgrade.GhostBlock] =  D(-1, -1, 2, 7, 9);
                u[Upgrade.Speed] =       D(-1, -1, 6, 8, 9);
                u[Upgrade.Ceiling] =     D(-1, -1, 4, 7, 10);
                u[Upgrade.Serpent] =       D(-1, 0, 0, 0, 8);
            });
        }

        static List<Action<Upgrades, DifficultyFunc>> HardcoreUpgrades = new List<Action<Upgrades, DifficultyFunc>>();
        static void MakeHardcoreUpgrades()
        {
            List<Action<Upgrades, DifficultyFunc>> f = HardcoreUpgrades;
            f.AddRange(AbusiveUpgrades);

            f.Add((u, D) =>
            {
                u[Upgrade.FireSpinner] = D(-1, -1, -1, 9, 10);
                u[Upgrade.Speed] =       D(-1, -1, -1, 6, 9);
                u[Upgrade.MovingBlock] = D(-1, -1, -1, 4, 2);
                u[Upgrade.Fireball] =     D(-1, 0, 0, 0, 9);
            });
        }
    }
}
#endif