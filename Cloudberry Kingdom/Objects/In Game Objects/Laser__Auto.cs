using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom.Levels
{
    public class Laser_Parameters : AutoGen_Parameters
    {
        public float LaserStepCutoff = 1499;
        public Param LaserStep, LaserPeriod;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            // General difficulty
            BobWidthLevel = new Param(PieceSeed);
            BobWidthLevel.SetVal(u =>
            {
                return u[Upgrade.Laser];
            });

            LaserStep = new Param(PieceSeed);
            LaserStep.SetVal(u =>
            {
                //return Math.Max(60, 550 - 38 * u[Upgrade.Laser]);

                float LaserLevel = u[Upgrade.Laser];
                if (LaserLevel == 0) return LaserStepCutoff + 1;
                else
                    //return Tools.DifficultyLerp159(1200, 400, 60, LaserLevel);
                    return Tools.DifficultyLerp159(1450, 400, 60, LaserLevel);
            });

            LaserPeriod = new Param(PieceSeed);
            LaserPeriod.SetVal(u =>
            {
                return Math.Max(70, 200 - 11 * u[Upgrade.Speed]);
            });
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
            level.AutoLasers(BL + new Vector2(-400, 0), TR + new Vector2(350, 0));
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
            level.CleanupLasers(BL, TR);
        }
    }

    public partial class Level
    {
        public void CleanupLasers(Vector2 BL, Vector2 TR)
        {
            // Get Laser parameters
            Laser_Parameters Params = (Laser_Parameters)Style.FindParams(Laser_AutoGen.Instance);

            /*
            Cleanup(ObjectType.Laser, delegate(Vector2 pos)
            {
                float dist = Params.LaserMinDist.GetVal(pos);
                return new Vector2(dist, dist);
            }, BL, TR); */
        }
        public void AutoLasers(Vector2 BL, Vector2 TR)
        {
            // Get Laser parameters
            Laser_Parameters Params = (Laser_Parameters)Style.FindParams(Laser_AutoGen.Instance);

            float step = 5;
            //Vector2 loc = (BL + TR) / 2;
            Vector2 loc;
            if (PieceSeed.GeometryType == LevelGeometry.Right)
                loc = new Vector2(BL.X + 600, (TR.Y + BL.Y) / 2);
            else
                loc = new Vector2((TR.X + BL.X) / 2, BL.Y + 600);

            while (loc.X < TR.X && PieceSeed.GeometryType == LevelGeometry.Right ||
                   loc.Y < TR.Y && (PieceSeed.GeometryType == LevelGeometry.Up || PieceSeed.GeometryType == LevelGeometry.Down))
            {
                step = Rnd.RndFloat(Params.LaserStep.GetVal(loc),
                                      Params.LaserStep.GetVal(loc));

                Vector2 CamSize = MainCamera.GetSize();

                if (step < Params.LaserStepCutoff)
                {
                    Laser laser = (Laser)Recycle.GetObject(ObjectType.Laser, true);
                    laser.Init(Vector2.Zero, this);

                    laser.Core.Data.Position = loc;
                    float shift = Rnd.RndFloat(-800, 800);

                    Vector2 p1, p2;
                    if (PieceSeed.GeometryType == LevelGeometry.Right)
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
                    laser.Offset = Rnd.Rnd.Next(laser.Period);
                    laser.WarnDuration = (int)(laser.Period * .35f);
                    laser.Duration = (int)(laser.Period * .2f);

                    laser.Core.GenData.LimitGeneralDensity = false;

                    // Make sure we stay in bounds
                    Tools.EnsureBounds_X(laser, TR, BL);

                    AddObject(laser);
                }

                if (PieceSeed.GeometryType == LevelGeometry.Right)
                    loc.X += step;
                else
                    loc.Y += step;
            }
        }
    }
}