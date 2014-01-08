using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Obstacles;

namespace CloudberryKingdom.Levels
{
    public class Firesnake_Parameters : AutoGen_Parameters
    {
        public Param Step, Period;
        public VectorParam RadiiX, RadiiY;

        public bool Make;

        public struct _Special
        {
        }
        public _Special Special;

        public List<List<ulong>> Snakes = new List<List<ulong>>();

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            if (PieceSeed.MyUpgrades1[Upgrade.Firesnake] > 0 ||
                PieceSeed.MyUpgrades2[Upgrade.Firesnake] > 0)
                Make = true;
            else
                Make = false;

            // General difficulty
            float FloaterLevel = PieceSeed.MyUpgrades1[Upgrade.Firesnake];
            if (FloaterLevel > 6) NumOffsets = 8;
            else NumOffsets = 4;
            
            BobWidthLevel = new Param(PieceSeed, u =>
                u[Upgrade.Firesnake]);

            Step = new Param(PieceSeed, u =>
            {
                float LineLevel = u[Upgrade.Firesnake];
                
                return DifficultyHelper.Interp159(1340, 830, 340, LineLevel);
            });

            Period = new Param(PieceSeed, u =>
                DifficultyHelper.Interp(290, 150, u[Upgrade.Firesnake]) *
                DifficultyHelper.Interp(1.7f, 1.0f, u[Upgrade.Speed]));
                //Math.Max(70, 200 - 11 * u[Upgrade.Speed]));

            RadiiX = new VectorParam(PieceSeed, u => new Vector2(400, 1000));
            RadiiY = new VectorParam(PieceSeed, u => new Vector2(1550, 1850));
        }
    }

    public sealed class Firesnake_AutoGen : AutoGen
    {
        static readonly Firesnake_AutoGen instance = new Firesnake_AutoGen();
        public static Firesnake_AutoGen Instance { get { return instance; } }

        static Firesnake_AutoGen() { }
        Firesnake_AutoGen()
        {
            Do_PreFill_2 = true;
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            Firesnake_Parameters Params = new Firesnake_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public Firesnake_Parameters GetParams(Level level)
        {
            return (Firesnake_Parameters)level.Style.FindParams(Firesnake_AutoGen.Instance);
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);

            Firesnake_Parameters Params = GetParams(level);

            foreach (List<ulong> GuidList in Params.Snakes)
            {
                // Convert the list to objects
                List<ObjectBase> Snake = level.GuidToObj(GuidList);

                // Find a deleted element to start with
                int start = 0;
                for (int i = 0; i < Snake.Count; i++)
                    if (Snake[i] == null)
                    {
                        start = i + 1;
                        break;
                    }

                // Find which elements can serve as heads
                List<int> PotentialHeads = new List<int>();

                int count = 0;
                for (int j = 0; j < Snake.Count; j++)
                {
                    int i = (j + start) % Snake.Count;

                    if (Snake[i] == null)
                    {
                        //if (count > 8)
                        if (count > 4)
                            PotentialHeads.Add((j - 1 + start) % Snake.Count);
                        count = 0;
                    }
                    else
                        count++;

                    if (count > Snake.Count / 2)
                        PotentialHeads.Add(i);
                }

                // If no potential heads were found delete everything
                if (PotentialHeads.Count == 0)
                {
                    foreach (Firesnake snake in Snake)
                        if (snake != null)
                            snake.CollectSelf();
                    continue;
                }

                // Choose a head
                int head = PotentialHeads.Choose(level.Rnd);
                
                // Find the end of the chain
                int l = 0;
                for (l = 0; l < Snake.Count / 2; l++)
                {
                    int i = (head - l + Snake.Count) % Snake.Count;

                    if (Snake[i] == null) break;
                }

                // Delete the rest
                while (l < Snake.Count)
                {
                    int i = (head - l + Snake.Count) % Snake.Count;

                    if (Snake[i] != null)
                        Snake[i].CollectSelf();

                    l++;
                }
            }

            Params.Snakes = null;
        }

        public void CreateLine(Level level, Vector2 pos, Action<Firesnake> process)
        {
            Firesnake_Parameters Params = GetParams(level);

            float Period = Params.Period.GetVal(pos);

            //int n = 36;
            int n = 18;
            
            List<ulong> Snake = new List<ulong>(n);

            //Vector2 Radii = new Vector2(Params.RadiiX.RndFloat(pos, level.Rnd), Params.RadiiY.RndFloat(pos, level.Rnd));
            //Vector2 Radii = new Vector2(0, 800);
            Vector2 Radii = new Vector2(500, 500);

            float periodstep = Period / n;
            for (int i = 0; i < n; i++)
            {
                Firesnake snake = (Firesnake)CreateAt(level, pos);
                snake.Offset = (int)(i * periodstep);
                snake.Radii = Radii;

                Snake.Add(snake.CoreData.MyGuid);
            }

            Params.Snakes.Add(Snake);
        }

        public override ObjectBase CreateAt(Level level, Vector2 pos)
        {
            // Get Floater parameters
            Firesnake_Parameters Params = (Firesnake_Parameters)level.Style.FindParams(Firesnake_AutoGen.Instance);

            // Get the new snake
            Firesnake NewSnake = (Firesnake)level.Recycle.GetObject(ObjectType.Firesnake, true);

            //if (level.Rnd.RndBool())
            //    pos.Y = level.FillBL.Y - 300;
            //else
                pos.Y = level.Fill_TR.Y + 550;

            NewSnake.Move(pos);

            NewSnake.Offset = level.Rnd.Rnd.Next(0, NewSnake.Period);

            // Discrete period offsets
            int NumOffsets = Params.NumOffsets;
            int Period = (int)(Params.Period.GetVal(pos) / NumOffsets) * NumOffsets;
            NewSnake.Period = Period;
            NewSnake.Offset = level.Rnd.Rnd.Next(0, NumOffsets) * Period / NumOffsets;

            NewSnake.CoreData.GenData.RemoveIfUnused = false;

            level.AddObject(NewSnake);

            return NewSnake;
        }

        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);

            // Get Floater parameters
            Firesnake_Parameters Params = (Firesnake_Parameters)level.Style.FindParams(Firesnake_AutoGen.Instance);

            if (!Params.Make) return;

            float step = 5;
            Vector2 loc = new Vector2(BL.X + 600, (TR.Y + BL.Y) / 2);

            while (loc.X < TR.X)
            {
                step = level.Rnd.RndFloat(Params.Step.GetVal(loc),
                                      Params.Step.GetVal(loc));

                float shift = level.Rnd.RndFloat(-800, 800);

                CreateLine(level, loc, spikey => { });

                loc.X += step;
            }
        }
    }
}