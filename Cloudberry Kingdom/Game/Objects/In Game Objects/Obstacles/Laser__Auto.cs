using System;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class Laser_Parameters : AutoGen_Parameters
    {
        public float LaserStepCutoff = 1499;
        public Param LaserStep, LaserPeriod;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            var u = PieceSeed.u;

            // General difficulty
            BobWidthLevel = new Param(PieceSeed, u[Upgrade.Laser]);

            LaserStep = new Param(PieceSeed,
                u[Upgrade.Laser] == 0 ? LaserStepCutoff + 1 : DifficultyHelper.Interp159(1450, 400, 60, u[Upgrade.Laser]));

            LaserPeriod = new Param(PieceSeed, Math.Max(70, 200 - 11 * u[Upgrade.Speed]));
        }
    }

    public sealed class Laser_AutoGen : AutoGen
    {
        static readonly Laser_AutoGen instance = new Laser_AutoGen();
        public static Laser_AutoGen Instance { get { return instance; } }

        static Laser_AutoGen() { }
        Laser_AutoGen()
        {
            Do_PreFill_2 = true;
            //Generators.AddGenerator(this);
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            Laser_Parameters Params = new Laser_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);

            BL += new Vector2(-400, 0);
            TR += new Vector2(350, 0);

            // Get Laser parameters
            Laser_Parameters Params = (Laser_Parameters)level.Style.FindParams(Laser_AutoGen.Instance);

            float step = 5;

            Vector2 loc;
            if (level.PieceSeed.GeometryType == LevelGeometry.Right)
                loc = new Vector2(BL.X + 600, (TR.Y + BL.Y) / 2);
            else
                loc = new Vector2((TR.X + BL.X) / 2, BL.Y + 600);

            while (loc.X < TR.X && level.PieceSeed.GeometryType == LevelGeometry.Right ||
                   loc.Y < TR.Y && (level.PieceSeed.GeometryType == LevelGeometry.Up || level.PieceSeed.GeometryType == LevelGeometry.Down))
            {
                step = level.Rnd.RndFloat(Params.LaserStep.GetVal(loc),
                                      Params.LaserStep.GetVal(loc));

                Vector2 CamSize = level.MainCamera.GetSize();

                if (step < Params.LaserStepCutoff)
                {
                    Laser laser = (Laser)level.Recycle.GetObject(ObjectType.Laser, true);
                    laser.Init(Vector2.Zero, level);

                    laser.Core.Data.Position = loc;
                    float shift = level.Rnd.RndFloat(-800, 800);

                    Vector2 p1, p2;
                    if (level.PieceSeed.GeometryType == LevelGeometry.Right)
                    {
                        p1 = loc + new Vector2(shift, CamSize.Y / 2 + 300);
                        p2 = loc - new Vector2(shift, CamSize.Y / 2 + 300);
                    }
                    else
                    {
                        p1 = loc + new Vector2(CamSize.X / 2 + 300, shift);
                        p2 = loc - new Vector2(CamSize.X / 2 + 300, shift);
                    }

                    laser.SetLine(p1, p2);
                    laser.Period = (int)Params.LaserPeriod.GetVal(loc);
                    laser.Offset = level.Rnd.Rnd.Next(laser.Period);
                    laser.WarnDuration = (int)(laser.Period * .35f);
                    laser.Duration = (int)(laser.Period * .2f);

                    laser.Core.GenData.LimitGeneralDensity = false;

                    // Make sure we stay in bounds
                    Tools.EnsureBounds_X(laser, TR, BL);

                    level.AddObject(laser);
                }

                if (level.PieceSeed.GeometryType == LevelGeometry.Right)
                    loc.X += step;
                else
                    loc.Y += step;
            }
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
        }
    }
}