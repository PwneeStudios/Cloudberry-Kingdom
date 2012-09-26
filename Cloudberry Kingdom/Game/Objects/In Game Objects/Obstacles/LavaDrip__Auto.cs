using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.LavaDrips;

namespace CloudberryKingdom.Levels
{
    public class LavaDrip_Parameters : AutoGen_Parameters
    {
        public float LavaDripStepCutoff = 1499;
        public Param LavaDripStep, Speed;
        public VectorParam Length;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            // General difficulty
            BobWidthLevel = new Param(PieceSeed);
            BobWidthLevel.SetVal(u =>
            {
                return u[Upgrade.LavaDrip];
            });

            LavaDripStep = new Param(PieceSeed);
            LavaDripStep.SetVal(u =>
            {
                float LavaDripLevel = u[Upgrade.LavaDrip];
                if (LavaDripLevel == 0)
                    return LavaDripStepCutoff + 1;
                else
                    return DifficultyHelper.Interp159(1450, 550, 250, LavaDripLevel);
            });

            Length = new VectorParam(PieceSeed,
                u => new Vector2(1400 - 100 * u[Upgrade.LavaDrip], 1400 + 80 * u[Upgrade.LavaDrip]));

            Speed = new Param(PieceSeed);
            Speed.SetVal(u =>
            {
                return Math.Min(3, 1 + .043f * u[Upgrade.Speed] + .017f * u[Upgrade.LavaDrip]);
            });
        }
    }

    public sealed class LavaDrip_AutoGen : AutoGen
    {
        static readonly LavaDrip_AutoGen instance = new LavaDrip_AutoGen();
        public static LavaDrip_AutoGen Instance { get { return instance; } }

        static LavaDrip_AutoGen() { }
        LavaDrip_AutoGen()
        {
            Do_PreFill_2 = true;
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            LavaDrip_Parameters Params = new LavaDrip_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);
            level.AutoLavaDrips(BL + new Vector2(-400, 0), TR + new Vector2(350, 0));
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
            level.CleanupLavaDrips(BL, TR);
        }
    }

    public partial class Level
    {
        public void CleanupLavaDrips(Vector2 BL, Vector2 TR)
        {
            // Get LavaDrip parameters
            LavaDrip_Parameters Params = (LavaDrip_Parameters)Style.FindParams(LavaDrip_AutoGen.Instance);

            /*
            Cleanup(ObjectType.LavaDrip, delegate(Vector2 pos)
            {
                float dist = Params.LavaDripMinDist.GetVal(pos);
                return new Vector2(dist, dist);
            }, BL, TR); */
        }
        public void AutoLavaDrips(Vector2 BL, Vector2 TR)
        {
            // Get LavaDrip parameters
            LavaDrip_Parameters Params = (LavaDrip_Parameters)Style.FindParams(LavaDrip_AutoGen.Instance);

            float step = 5;

            Vector2 loc;
            if (PieceSeed.GeometryType == LevelGeometry.Right)
                loc = new Vector2(BL.X + 600, (TR.Y + BL.Y) / 2);
            else
                loc = new Vector2((TR.X + BL.X) / 2, BL.Y + 600);

            while (loc.X < TR.X && PieceSeed.GeometryType == LevelGeometry.Right ||
                   loc.Y < TR.Y && (PieceSeed.GeometryType == LevelGeometry.Up || PieceSeed.GeometryType == LevelGeometry.Down))
            {
                step = Rnd.RndFloat(Params.LavaDripStep.GetVal(loc),
                                      Params.LavaDripStep.GetVal(loc));

                Vector2 CamSize = MainCamera.GetSize();

                if (step < Params.LavaDripStepCutoff)
                {
                    LavaDrip LavaDrip = (LavaDrip)Recycle.GetObject(ObjectType.LavaDrip, true);
                    LavaDrip.BoxSize.Y = Params.Length.RndFloat(loc, this.Rnd);
                    LavaDrip.Init(loc, this);

                    int speed = (int)Params.Speed.GetVal(loc);
                    LavaDrip.SetPeriod(speed);

                    LavaDrip.Offset = Rnd.Rnd.Next(LavaDrip.Period);

                    LavaDrip.Core.GenData.LimitGeneralDensity = false;

                    // Make sure we stay in bounds
                    //Tools.EnsureBounds_X(LavaDrip, TR, BL);

                    AddObject(LavaDrip);
                }

                if (PieceSeed.GeometryType == LevelGeometry.Right)
                    loc.X += step;
                else
                    loc.Y += step;
            }
        }
    }
}