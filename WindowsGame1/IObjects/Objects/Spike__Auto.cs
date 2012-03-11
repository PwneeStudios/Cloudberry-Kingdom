using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Spikes;

namespace CloudberryKingdom.Levels
{
    public class Spike_Parameters : AutoGen_Parameters
    {
        public Param SpikeMinDist, MinSpikeDensity, MaxSpikeDensity, SpikePeriod;

        public enum OffsetStyles { Rnd, SawTooth, Sine };
        public OffsetStyles OffsetStyle;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            float lvl = PieceSeed.u[Upgrade.Spike];

            // General difficulty
            int MinNumOffsets = 1; if (lvl > 2.5f) MinNumOffsets = 2; if (lvl > 6.5f) MinNumOffsets = 3;
            int MaxNumOffsets = 2; if (lvl > 2.5f) MaxNumOffsets = 4; if (lvl > 6.5f) MaxNumOffsets = 8;
            NumOffsets = level.Rnd.RndInt(MinNumOffsets, MaxNumOffsets);

            OffsetStyle = (OffsetStyles)level.Rnd.RndEnum<OffsetStyles>();

            BobWidthLevel = new Param(PieceSeed, u => u[Upgrade.Spike]);

            SpikeMinDist = new Param(PieceSeed);
            SpikeMinDist.SetVal(u =>
            {
                float dist = 400 - u[Upgrade.Spike] * 40;// 23;
                if (dist < 35) dist = 35;
                return dist;
            });

            MinSpikeDensity = new Param(PieceSeed);
            MinSpikeDensity.SetVal(u =>
            {
                if (u[Upgrade.Spike] == 0)
                    return 0;

                return Tools.DifficultyLerp(6, 50, u[Upgrade.Spike]);
            });

            MaxSpikeDensity = new Param(PieceSeed);
            MaxSpikeDensity.SetVal(u =>
            {
                if (u[Upgrade.Spike] == 0)
                    return 0;

                return Tools.DifficultyLerp(9, 80, u[Upgrade.Spike]);
            });

            SpikePeriod = new Param(PieceSeed);
            SpikePeriod.SetVal(u =>
            {
                return Math.Max(60, 240 - 20 * u[Upgrade.Speed]);
            });            
        }

        /// <summary>
        /// Set the period and period offset of the spike.
        /// The spike's position should already have been set.
        /// </summary>
        public void SetPeriod(Spike spike, Rand Rnd)
        {
            Vector2 pos = spike.Core.Data.Position;
            int period = (int)SpikePeriod.GetVal(pos);

            spike.SetPeriod(period);

            switch (OffsetStyle)
            {
                case OffsetStyles.Rnd: spike.Offset = ChooseOffset(period, Rnd); break;
                case OffsetStyles.SawTooth:
                    spike.Offset = (int)((pos.X / 700) * period) % period;
                    break;
                case OffsetStyles.Sine:
                    spike.Offset = (int)((.5 * Math.Sin(pos.X / 700 * Math.PI) + .5) * period);
                    break;
            }
            
            spike.Offset = EnforceOffset(spike.Offset, period);
        }
    }

    public sealed class Spike_AutoGen : AutoGen
    {
        static readonly Spike_AutoGen instance = new Spike_AutoGen();
        public static Spike_AutoGen Instance { get { return instance; } }

        static Spike_AutoGen() { }
        Spike_AutoGen()
        {
            Do_PreFill_2 = true;
            //Generators.AddGenerator(this);
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            Spike_Parameters Params = new Spike_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);
            level.AutoSpikes();
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
            level.CleanupSpikes(BL, TR);
        }
    }

    public partial class Level
    {
        public void CleanupSpikes(Vector2 BL, Vector2 TR)
        {
            // Get Spike parameters
            Spike_Parameters Params = (Spike_Parameters)Style.FindParams(Spike_AutoGen.Instance);
            
            Cleanup(ObjectType.Spike, pos =>
            {
                float dist = Params.SpikeMinDist.GetVal(pos);
                return new Vector2(dist, dist);
            }, BL, TR);
        }
        public void AutoSpikes()
        {
            // Get Spike parameters
            Spike_Parameters Params = (Spike_Parameters)Style.FindParams(Spike_AutoGen.Instance);

            if (Params.MinSpikeDensity.Val <= 0)
                return;

            float SpikeTopOffset = InfoWad.GetFloat("Spike_TopOffset");
            float SpikeBottomOffset = InfoWad.GetFloat("Spike_BottomOffset");
            float SpikeSideOffset = InfoWad.GetFloat("Spike_SideOffset");

            foreach (Block block in Blocks)
            {
                if (block.Core.Placed) continue;

                if (!(block.BlockCore.BlobsOnTop || block.BlockCore.Ceiling)) continue;

                //if (!(block is NormalBlock)) continue;
                if (block.BlockCore.Virgin) continue;
                if (block.BlockCore.Finalized) continue;
                if (block.BlockCore.MyType == ObjectType.LavaBlock) continue;


                // Add spikes
                float xdif = block.Box.Current.TR.X - block.Box.Current.BL.X - 110;
                float density = Rnd.RndFloat(Params.MinSpikeDensity.GetVal(block.Core.Data.Position),
                                               Params.MaxSpikeDensity.GetVal(block.Core.Data.Position));
                float average = (int)(xdif * (float)density / 2000f);
                int n = (int)average;
                //if (average < 1) if (Rnd.Rnd.NextDouble() < average) n = 1;
                if (average < 2) n = 2;

                for (int i = 0; i < n; i++)
                {
                    if (xdif > 15)
                    {
                        Spike spike = (Spike)Recycle.GetObject(ObjectType.Spike, true);//false);

                        float x = (float)Rnd.Rnd.NextDouble() * xdif + block.Box.Target.BL.X + 55;
                        float y;

                        if (block.BlockCore.BlobsOnTop)
                        {
                            y = block.Box.Target.TR.Y + SpikeTopOffset;
                            spike.SetDir(0);
                        }
                        else
                        {
                            y = block.Box.Target.BL.Y - SpikeBottomOffset;
                            spike.SetDir(2);
                        }

                        Vector2 pos = new Vector2(x, y);
                        Tools.MoveTo(spike, pos);
                        
                        //spike.Offset = Rnd.Rnd.Next(0, 200);
                        Params.SetPeriod(spike, Rnd);

                        spike.SetParentBlock(block);
                        AddObject(spike);
                    }
                }

                if (!block.Box.TopOnly)
                {
                    float ydif = block.Box.Current.TR.Y - block.Box.Current.BL.Y - 110;
                    average = (int)(ydif * (float)density / 2000f);
                    n = (int)average;
                    if (average < 1) if (Rnd.Rnd.NextDouble() < average) n = 1;
                    n = 4;
                    for (int i = 0; i < n; i++)
                    {
                        // Side spikes
                        if (ydif > 15 && xdif > 15)
                        {
                            Spike spike = (Spike)Recycle.GetObject(ObjectType.Spike, true);//false);

                            float y = (float)Rnd.Rnd.NextDouble() * ydif + block.Box.Target.BL.Y + 55;
                            float x;

                            if (Rnd.Rnd.Next(0, 2) == 0)
                            {
                                x = block.Box.Target.TR.X + SpikeSideOffset;
                                spike.SetDir(3);
                                y -= 25;
                            }
                            else
                            {
                                x = block.Box.Target.BL.X - SpikeSideOffset;
                                spike.SetDir(1);
                            }

                            spike.Core.Data.Position = new Vector2(x, y);
                            //spike.Offset = Rnd.Rnd.Next(0, 200);
                            Params.SetPeriod(spike, Rnd);

                            spike.SetParentBlock(block);
                            AddObject(spike);
                        }
                    }
                }
            }
        }
    }
}
