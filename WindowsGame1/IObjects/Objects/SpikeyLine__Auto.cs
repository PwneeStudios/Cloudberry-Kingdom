using System;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom.Levels
{
    public class SpikeyLine_Parameters : AutoGen_Parameters
    {
        public Param LineStep, LinePeriod;
        public bool Make;

        public struct _Special
        {
        }
        public _Special Special;

        //public List<List<SpikeyLine>> Lines = new List<List<SpikeyLine>>();

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            if (PieceSeed.MyUpgrades1[Upgrade.SpikeyLine] > 0 ||
                PieceSeed.MyUpgrades2[Upgrade.SpikeyLine] > 0)
                Make = true;
            else
                Make = false;

            // General difficulty
            float FloaterLevel = PieceSeed.MyUpgrades1[Upgrade.SpikeyLine];
            if (FloaterLevel > 6) NumOffsets = 8;
            else NumOffsets = 4;
            
            BobWidthLevel = new Param(PieceSeed, u =>
                u[Upgrade.SpikeyLine]);

            LineStep = new Param(PieceSeed, u =>
            {
                float LineLevel = u[Upgrade.SpikeyLine];
                
                //return Tools.DifficultyLerp159(1200, 800, 340, LineLevel);
                return Tools.DifficultyLerp159(1550, 800, 340, LineLevel);
            });

            LinePeriod = new Param(PieceSeed, u =>
                Math.Max(70, 200 - 11 * u[Upgrade.Speed]));
        }
    }

    public sealed class SpikeyLine_AutoGen : AutoGen
    {
        static readonly SpikeyLine_AutoGen instance = new SpikeyLine_AutoGen();
        public static SpikeyLine_AutoGen Instance { get { return instance; } }

        static SpikeyLine_AutoGen() { }
        SpikeyLine_AutoGen()
        {
            Do_PreFill_2 = true;
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            SpikeyLine_Parameters Params = new SpikeyLine_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
            level.CleanupSpinFloaters(BL, TR);
        }

        public void CreateLine(Level level, Vector2 pos, Action<SpikeyLine> process)
        {
            // Get Floater parameters
            SpikeyLine_Parameters Params = (SpikeyLine_Parameters)level.Style.FindParams(SpikeyLine_AutoGen.Instance);
            float Period = Params.LinePeriod.GetVal(pos);

            int n = 12;
            //List<SpikeyLine> Line = new List<SpikeyLine>(n);

            float periodstep = Period / n;
            for (int i = 0; i < n; i++)
            {
                SpikeyLine spikey = (SpikeyLine)CreateAt(level, pos);
                spikey.Offset = (int)(i * periodstep);

                //Line.Add(spikey);
            }

            //Params.Lines.Add(Line);
        }

        public override IObject CreateAt(Level level, Vector2 pos)
        {
            // Get Floater parameters
            SpikeyLine_Parameters Params = (SpikeyLine_Parameters)level.Style.FindParams(SpikeyLine_AutoGen.Instance);

            // Get the new floater
            SpikeyLine NewFloater = (SpikeyLine)level.Recycle.GetObject(ObjectType.SpikeyLine, true);

            if (level.PieceSeed.GeometryType == LevelGeometry.Right)
            {
                NewFloater.p1.Y = level.MainCamera.TR.Y + 300;
                NewFloater.p2.Y = level.MainCamera.BL.Y - 300;
            }
            else
            {
                NewFloater.p1.Y = NewFloater.p2.Y = 0;
                NewFloater.p1.X = level.MainCamera.TR.X + 200;
                NewFloater.p2.X = level.MainCamera.BL.X - 200;
            }

            NewFloater.Move(pos);

            NewFloater.Offset = level.Rnd.Rnd.Next(0, NewFloater.Period);

            // Discrete period offsets
            int NumOffsets = Params.NumOffsets;
            int Period = (int)(Params.LinePeriod.GetVal(pos) / NumOffsets) * NumOffsets;
            NewFloater.Period = Period;
            NewFloater.Offset = level.Rnd.Rnd.Next(0, NumOffsets) * Period / NumOffsets;

            NewFloater.Core.GenData.RemoveIfUnused = false;

            // Bigger range for bigger levels
            //if (level.PieceSeed.ZoomType == LevelZoom.Big)
            //{
            //    NewFloater.p1.Y += 1300;
            //    NewFloater.p2.Y -= 1300;
            //}

            level.AddObject(NewFloater);

            return NewFloater;
        }

        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);

            // Get Floater parameters
            SpikeyLine_Parameters Params = (SpikeyLine_Parameters)level.Style.FindParams(SpikeyLine_AutoGen.Instance);

            if (!Params.Make) return;

            float step = 5;

            if (level.PieceSeed.GeometryType == LevelGeometry.Right)
            {
                Vector2 loc = new Vector2(BL.X + 600, (TR.Y + BL.Y) / 2);

                while (loc.X < TR.X)
                {
                    step = level.Rnd.RndFloat(Params.LineStep.GetVal(loc),
                                          Params.LineStep.GetVal(loc));

                    CreateLine(level, loc, spikey => { });

                    loc.X += step;
                }
            }
            else
            {
                Vector2 loc = new Vector2((TR.X + BL.X) / 2, BL.Y + 600);

                while (loc.Y < TR.Y)
                {
                    step = level.Rnd.RndFloat(Params.LineStep.GetVal(loc),
                                          Params.LineStep.GetVal(loc));

                    CreateLine(level, loc, spikey => { });

                    loc.Y += step;
                }
            }
        }
    }

    public partial class Level
    {
        public void CleanupSpikeyLines(Vector2 BL, Vector2 TR)
        {
            // Get Floater parameters
            SpikeyLine_Parameters Params = (SpikeyLine_Parameters)Style.FindParams(SpikeyLine_AutoGen.Instance);
/*
            Cleanup(ObjectType.SpikeyLine,
            delegate(Vector2 pos)
            {
                float dist = Params.FloaterMinDist.GetVal(pos);
                return new Vector2(dist, dist);
            }, BL + new Vector2(400, 0), TR - new Vector2(500, 0),
            delegate(IObject A, IObject B)
            {
                SpikeyLine floater_A = A as SpikeyLine;
                SpikeyLine floater_B = B as SpikeyLine;
                return Tools.Abs(floater_A.PivotPoint - floater_B.PivotPoint);
            }
            );*/
        }
    }
}