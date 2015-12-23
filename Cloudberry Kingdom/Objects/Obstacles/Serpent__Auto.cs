using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Obstacles;

namespace CloudberryKingdom.Levels
{
    public class Serpent_Parameters : AutoGen_Parameters
    {
        public float SerpentStepCutoff = 1651;
        public Param SerpentStep, SerpentPeriod, NumToMake;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            // General difficulty
            BobWidthLevel = new Param(PieceSeed, u => u[Upgrade.Serpent]);

            NumToMake = new Param(PieceSeed, u => u[Upgrade.Serpent] < 4 ? 3 : 2);

            SerpentStep = new Param(PieceSeed, u =>
            {
                float SerpentLevel = u[Upgrade.Serpent];
                if (SerpentLevel == 0)
                    return SerpentStepCutoff + 1;
                else
                    return DifficultyHelper.Interp159(1650, 860, 350, SerpentLevel);
            });

            SerpentPeriod = new Param(PieceSeed, u => Math.Max(70, 200 - 9 * u[Upgrade.Speed]));
        }
    }

    public sealed class Serpent_AutoGen : AutoGen
    {
        static readonly Serpent_AutoGen instance = new Serpent_AutoGen();
        public static Serpent_AutoGen Instance { get { return instance; } }

        static Serpent_AutoGen() { }
        Serpent_AutoGen()
        {
            Do_PreFill_2 = true;
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            Serpent_Parameters Params = new Serpent_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);

            BL += new Vector2(-400, 0);
            TR += new Vector2(350, 0);

            // Get Serpent parameters
            Serpent_Parameters Params = (Serpent_Parameters)level.Style.FindParams(Serpent_AutoGen.Instance);

            float step = 5;

            Vector2 loc;
            if (level.PieceSeed.GeometryType == LevelGeometry.Right)
                loc = new Vector2(BL.X + 600, (TR.Y + BL.Y) / 2);
            else
                loc = new Vector2((TR.X + BL.X) / 2, BL.Y + 600);

            while (loc.X < TR.X && level.PieceSeed.GeometryType == LevelGeometry.Right ||
                   loc.Y < TR.Y && (level.PieceSeed.GeometryType == LevelGeometry.Up || level.PieceSeed.GeometryType == LevelGeometry.Down))
            {
                step = level.Rnd.RndFloat(Params.SerpentStep.GetVal(loc),
                                      Params.SerpentStep.GetVal(loc));

                Vector2 CamSize = level.MainCamera.GetSize();

                if (step < Params.SerpentStepCutoff)
                {
                    int period = (int)Params.SerpentPeriod.GetVal(loc);
                    int offset = level.Rnd.Rnd.Next(period);
                    int num = (int)Params.NumToMake.GetVal(loc);

                    // Create 2 serpents in this location, with offset perios.
                    for (int i = 0; i < num; i++)
                    {
                        Serpent serpent = (Serpent)level.Recycle.GetObject(ObjectType.Serpent, true);
                        serpent.Init(loc, level);

                        serpent.SetPeriod(period);

                        serpent.Offset = (int)(offset + i * period / (float)num);

                        serpent.Core.GenData.LimitGeneralDensity = false;

                        // Make sure we stay in bounds
                        //Tools.EnsureBounds_X(serpent, TR, BL);

                        level.AddObject(serpent);
                    }
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

            level.Cleanup_xCoord(ObjectType.Serpent, 10);
        }
    }
}