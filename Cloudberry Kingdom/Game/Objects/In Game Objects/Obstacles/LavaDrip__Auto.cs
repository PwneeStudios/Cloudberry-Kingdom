using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Obstacles;

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

            var u = PieceSeed.u;

            // General difficulty
            BobWidthLevel = new Param(PieceSeed, u[Upgrade.LavaDrip]);

            LavaDripStep = new Param(PieceSeed,
                u[Upgrade.LavaDrip] == 0 ? LavaDripStepCutoff + 1 : DifficultyHelper.Interp159(1450, 550, 250, u[Upgrade.LavaDrip]));

            Length = new VectorParam(PieceSeed,
                new Vector2(1400 - 100 * u[Upgrade.LavaDrip], 1400 + 80 * u[Upgrade.LavaDrip]));

            Speed = new Param(PieceSeed, Math.Min(3, 1 + .043f * u[Upgrade.Speed] + .017f * u[Upgrade.LavaDrip]));
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

            BL += new Vector2(-400, 0);
            TR += new Vector2(350, 0);

            // Get LavaDrip parameters
            LavaDrip_Parameters Params = (LavaDrip_Parameters)level.Style.FindParams(LavaDrip_AutoGen.Instance);

            float step = 5;

            Vector2 loc;
            if (level.PieceSeed.GeometryType == LevelGeometry.Right)
                loc = new Vector2(BL.X + 600, (TR.Y + BL.Y) / 2);
            else
                loc = new Vector2((TR.X + BL.X) / 2, BL.Y + 600);

            while (loc.X < TR.X && level.PieceSeed.GeometryType == LevelGeometry.Right ||
                   loc.Y < TR.Y && (level.PieceSeed.GeometryType == LevelGeometry.Up || level.PieceSeed.GeometryType == LevelGeometry.Down))
            {
                step = level.Rnd.RndFloat(Params.LavaDripStep.GetVal(loc),
                                      Params.LavaDripStep.GetVal(loc));

                Vector2 CamSize = level.MainCamera.GetSize();

                if (step < Params.LavaDripStepCutoff)
                {
                    LavaDrip LavaDrip = (LavaDrip)level.Recycle.GetObject(ObjectType.LavaDrip, true);
                    LavaDrip.BoxSize.Y = Params.Length.RndFloat(loc, level.Rnd);
                    LavaDrip.Init(loc, level);

                    int speed = (int)Params.Speed.GetVal(loc);
                    LavaDrip.SetPeriod(speed);

                    LavaDrip.Offset = level.Rnd.Rnd.Next(LavaDrip.Period);

                    LavaDrip.Core.GenData.LimitGeneralDensity = false;

                    level.AddObject(LavaDrip);
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