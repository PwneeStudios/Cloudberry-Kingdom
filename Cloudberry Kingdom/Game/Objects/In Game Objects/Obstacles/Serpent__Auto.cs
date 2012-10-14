using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Obstacles;

namespace CloudberryKingdom.Levels
{
    public class Serpent_Parameters : AutoGen_Parameters
    {
        public float SerpentStepCutoff = 1499;
        public Param SerpentStep, SerpentPeriod;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            // General difficulty
            BobWidthLevel = new Param(PieceSeed);
            BobWidthLevel.SetVal(u =>
            {
                return u[Upgrade.Serpent];
            });

            SerpentStep = new Param(PieceSeed);
            SerpentStep.SetVal(u =>
            {
                float SerpentLevel = u[Upgrade.Serpent];
                if (SerpentLevel == 0)
                    return SerpentStepCutoff + 1;
                else
                    return DifficultyHelper.Interp159(1450, 550, 250, SerpentLevel);
            });

            SerpentPeriod = new Param(PieceSeed);
            SerpentPeriod.SetVal(u =>
            {
                return Math.Max(70, 200 - 9 * u[Upgrade.Speed]);
            });
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
            level.AutoSerpents(BL + new Vector2(-400, 0), TR + new Vector2(350, 0));
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
            level.CleanupSerpents(BL, TR);
        }
    }

    public partial class Level
    {
        public void CleanupSerpents(Vector2 BL, Vector2 TR)
        {
            // Get Serpent parameters
            Serpent_Parameters Params = (Serpent_Parameters)Style.FindParams(Serpent_AutoGen.Instance);

            /*
            Cleanup(ObjectType.Serpent, delegate(Vector2 pos)
            {
                float dist = Params.SerpentMinDist.GetVal(pos);
                return new Vector2(dist, dist);
            }, BL, TR); */
        }
        public void AutoSerpents(Vector2 BL, Vector2 TR)
        {
            // Get Serpent parameters
            Serpent_Parameters Params = (Serpent_Parameters)Style.FindParams(Serpent_AutoGen.Instance);

            float step = 5;

            Vector2 loc;
            if (PieceSeed.GeometryType == LevelGeometry.Right)
                loc = new Vector2(BL.X + 600, (TR.Y + BL.Y) / 2);
            else
                loc = new Vector2((TR.X + BL.X) / 2, BL.Y + 600);

            while (loc.X < TR.X && PieceSeed.GeometryType == LevelGeometry.Right ||
                   loc.Y < TR.Y && (PieceSeed.GeometryType == LevelGeometry.Up || PieceSeed.GeometryType == LevelGeometry.Down))
            {
                step = Rnd.RndFloat(Params.SerpentStep.GetVal(loc),
                                      Params.SerpentStep.GetVal(loc));

                Vector2 CamSize = MainCamera.GetSize();

                if (step < Params.SerpentStepCutoff)
                {
                    Serpent serpent = (Serpent)Recycle.GetObject(ObjectType.Serpent, true);
                    serpent.Init(loc, this);

                    int period = (int)Params.SerpentPeriod.GetVal(loc);
                    serpent.SetPeriod(period);

                    serpent.Offset = Rnd.Rnd.Next(period);

                    serpent.Core.GenData.LimitGeneralDensity = false;

                    // Make sure we stay in bounds
                    //Tools.EnsureBounds_X(serpent, TR, BL);

                    AddObject(serpent);
                }

                if (PieceSeed.GeometryType == LevelGeometry.Right)
                    loc.X += step;
                else
                    loc.Y += step;
            }
        }
    }
}