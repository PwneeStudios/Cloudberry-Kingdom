using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Obstacles;

namespace CloudberryKingdom.Levels
{
    public class SpikeyGuy_Parameters : AutoGen_Parameters
    {
        public Param FloaterMinDist, Density, FloaterPeriod, FloaterScale;
        public bool Make;

        public struct _Special
        {
            /// <summary>
            /// A special fill type. One giant circle. All floaters have the same pivot point.
            /// </summary>
            public bool Pinwheel;

            /// <summary>
            /// A special fill type. Multiple, concentric circles. All floaters have the same pivot point.
            /// </summary>
            public bool Rockwheel;
        }
        public _Special Special;


        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            if (PieceSeed.MyUpgrades1[Upgrade.Pinky] > 0 ||
                PieceSeed.MyUpgrades2[Upgrade.Pinky] > 0)
                Make = true;
            else
                Make = false;

            // General difficulty
            float FloaterLevel = PieceSeed.MyUpgrades1[Upgrade.Pinky];
            if (FloaterLevel > 6) NumOffsets = 8;
            else NumOffsets = 4;
            
            BobWidthLevel = new Param(PieceSeed);
            BobWidthLevel.SetVal(u =>
            {
                return u[Upgrade.Pinky];
            });

            FloaterMinDist = new Param(PieceSeed);
            FloaterMinDist.SetVal(u =>
                DifficultyHelper.Interp159(700, 320, 100, u[Upgrade.Pinky]));

            FloaterScale = new Param(PieceSeed);
            FloaterScale.SetVal(u =>
                Math.Min(160, 90 + 7 * u[Upgrade.Pinky]));

            FloaterPeriod = new Param(PieceSeed);
            FloaterPeriod.SetVal(u =>
            {
                return Math.Max(84, 274 - 10 * u[Upgrade.Speed]);
            });

            Density = new Param(PieceSeed);
            Density.SetVal(u =>
            {
                //return 29 * u[Upgrade.Floater_Spin];
                if (u[Upgrade.Pinky] == 0)
                    return 0;

                return DifficultyHelper.Interp(40, 73, u[Upgrade.Pinky]);
            });
        }
    }

    public class SpikeyGuy_AutoGen : AutoGen
    {
        static readonly SpikeyGuy_AutoGen instance = new SpikeyGuy_AutoGen();
        public static SpikeyGuy_AutoGen Instance { get { return instance; } }

        static SpikeyGuy_AutoGen() { }
        protected SpikeyGuy_AutoGen()
        {
            Do_PreFill_2 = true;
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            SpikeyGuy_Parameters Params = new SpikeyGuy_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);

            // Get Floater parameters
            SpikeyGuy_Parameters Params = (SpikeyGuy_Parameters)level.Style.FindParams(SpikeyGuy_AutoGen.Instance);

            level.Cleanup(ObjectType.SpikeyGuy,
            pos =>
            {
                float dist = Params.FloaterMinDist.GetVal(pos);
                return new Vector2(dist, dist);
            }, BL + new Vector2(400, 0), TR - new Vector2(500, 0),
            (A, B) =>
            {
                SpikeyGuy floater_A = A as SpikeyGuy;
                SpikeyGuy floater_B = B as SpikeyGuy;
                return CoreMath.Abs(floater_A.PivotPoint - floater_B.PivotPoint);
            }
            );
        }

        public override ObjectBase CreateAt(Level level, Vector2 pos)
        {
            // Get Floater parameters
            SpikeyGuy_Parameters Params = (SpikeyGuy_Parameters)level.Style.FindParams(SpikeyGuy_AutoGen.Instance);

            // Get the new floater
            SpikeyGuy NewFloater = (SpikeyGuy)level.Recycle.GetObject(ObjectType.SpikeyGuy, true);
            NewFloater.Length = 650;
            NewFloater.Init(pos, level);

            NewFloater.Offset = level.Rnd.Rnd.Next(0, NewFloater.Period);

            // Discrete period offsets
            int NumOffsets = Params.NumOffsets;
            int Period = (int)(Params.FloaterPeriod.GetVal(pos) / NumOffsets) * NumOffsets;
            NewFloater.Period = Period;
            NewFloater.Offset = Params.ChooseOffset(Period, level.Rnd);

            NewFloater.Core.GenData.RemoveIfUnused = false;

            level.AddObject(NewFloater);

            return NewFloater;
        }

        void Circle(Level level, Vector2 Center, float Radius, int Num, int Dir)
        {
            for (int j = 0; j < Num; j++)
            {
                SpikeyGuy floater = (SpikeyGuy)CreateAt(level, Center);

                floater.Period = 3 * floater.Period / 2;
                floater.Offset = (int)(j * ((float)floater.Period / Num));
                floater.Length = Radius;

                floater.Dir = Dir;

                floater.Core.GenData.KeepIfUnused = true;

                level.AddObject(floater);
            }
        }

        void Rockwheel(Level level, Vector2 BL, Vector2 TR)
        {
            Vector2 Center = (TR + BL) / 2;

            for (int i = 7; i > 1; i--)
            {
                int Num = 10;
                for (int k = 0; k < 2; k++)                
                    Circle(level, Center, 160 * i, Num, k % 2 == 0 ? 1 : -1);
            }
        }

        void Pinwheel(Level level, Vector2 BL, Vector2 TR)
        {
            Vector2 Center = (TR + BL) / 2;

            int Num = 30;

            if (level.Rnd.RndBool())
            {                
                for (int k = 0; k < 2; k++)
                    Circle(level, Center, 160 * 7, Num, k % 2 == 0 ? 1 : -1);
            }
            else
            {
                Circle(level, Center, 160 * 6, Num, 1);
                Circle(level, Center, 160 * 7, Num, -1);
            }
        }

        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);

            // Get Floater parameters
            SpikeyGuy_Parameters Params = (SpikeyGuy_Parameters)level.Style.FindParams(SpikeyGuy_AutoGen.Instance);

            if (Params.Special.Rockwheel)
                Rockwheel(level, BL, TR);
            if (Params.Special.Pinwheel)
                Pinwheel(level, BL, TR);

            if (!Params.Make) return;

            foreach (BlockBase block in level.Blocks)
            {
                if (block.Core.Placed) continue;

                if (block.BlockCore.Virgin) continue;
                if (block.BlockCore.Finalized) continue;
                if (block.BlockCore.MyType == ObjectType.LavaBlock) continue;

                // Add spinners
                float xdif = block.Box.Current.TR.X - block.Box.Current.BL.X - 30;
                float density = level.Rnd.RndFloat(Params.Density.GetVal(block.Core.Data.Position),
                                               Params.Density.GetVal(block.Core.Data.Position));
                float average = (int)(xdif * density / 2000f);
                int n = (int)average;
                if (average < 1) if (level.Rnd.Rnd.NextDouble() < average) n = 1;

                for (int i = 0; i < n; i++)
                {
                    if (xdif > 0)
                    {
                        float x = (float)level.Rnd.Rnd.NextDouble() * xdif + block.Box.Target.BL.X + 35;
                        float y;
                        
                        if (block.BlockCore.BlobsOnTop)
                        {
                            y = block.Box.Target.TR.Y - 80 + 50;
                        }
                        else
                        {
                            y = block.Box.Target.BL.Y + 80 - 50;
                        }

                        if (x > level.CurMakeData.PieceSeed.End.X - 400) continue;

                        SpikeyGuy floater = (SpikeyGuy)CreateAt(level, new Vector2(x, y));

                        floater.SetParentBlock(block);

                        level.AddObject(floater);
                    }
                }
            }
        }
    }
}