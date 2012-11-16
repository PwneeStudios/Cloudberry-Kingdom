using System;

using Microsoft.Xna.Framework;



namespace CloudberryKingdom
{
    public class GhostBlock_Parameters : AutoGen_Parameters
    {
        public Param InLength, OutLength, Width, KeepUnused, TimeSafety;

        public enum BoxTypes { TopOnly, Full, Long };
        public BoxTypes BoxType; 

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            var u = PieceSeed.u;

            BoxType = BoxTypes.TopOnly;

            KeepUnused = new Param(PieceSeed);
            if (level.DefaultHeroType is BobPhsxSpaceship)
            {
                KeepUnused.SetVal(BobPhsxSpaceship.KeepUnused(u[Upgrade.GhostBlock]));
            }

            FillWeight = new Param(PieceSeed, u[Upgrade.GhostBlock]);

            InLength = new Param(PieceSeed, DifficultyHelper.Interp(147, 75, u[Upgrade.Speed]) *
                    DifficultyHelper.Interp(1.275f, .275f, u[Upgrade.GhostBlock]));

            OutLength = new Param(PieceSeed, Math.Max(60, 110 - 4 * u[Upgrade.Speed]));

            Width = new Param(PieceSeed, Math.Max(40, 93 - 2 * u[Upgrade.GhostBlock]));

            TimeSafety = new Param(PieceSeed);
            TimeSafety.SetVal(Math.Max(0f, DifficultyHelper.Interp(1f, 0f, u[Upgrade.GhostBlock])));

            // Masochistic
            if (PieceSeed.u[Upgrade.GhostBlock] == 10)
                Masochistic = true;
        }
    }

    public sealed class GhostBlock_AutoGen : AutoGen
    {
        static readonly GhostBlock_AutoGen instance = new GhostBlock_AutoGen();
        public static GhostBlock_AutoGen Instance { get { return instance; } }

        static GhostBlock_AutoGen() { }
        GhostBlock_AutoGen()
        {
            Do_WeightedPreFill_1 = true;
            //Generators.AddGenerator(this);
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            GhostBlock_Parameters Params = new GhostBlock_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
        }

        public override ObjectBase CreateAt(Level level, Vector2 pos, Vector2 BL, Vector2 TR)
        {
            base.CreateAt(level, pos, BL, TR);

            StyleData Style = level.Style;
            RichLevelGenData GenData = level.CurMakeData.GenData;
            PieceSeedData piece = level.CurMakeData.PieceSeed;

            // Get GhostBlock parameters
            GhostBlock_Parameters Params = (GhostBlock_Parameters)level.Style.FindParams(GhostBlock_AutoGen.Instance);

            int InLength = (int)Params.InLength.GetVal(pos);
            int OutLength = (int)Params.OutLength.GetVal(pos);
            int Period = InLength + OutLength;
            int Offset = level.Rnd.Rnd.Next(Period);
            
            GhostBlock gblock = null;
            int NumGhosts = 1; // Number of ghosts to create
            for (int i = 0; i < NumGhosts; i++)
            {
                // Stagger the offsets.
                // NOTE: THIS IS DEPRECATED. We are setting the Offset when the ghost is used.
                Offset = (Offset + i * Period / NumGhosts) % Period;

                int Width = (int)Params.Width.GetVal(pos);
                Vector2 size = new Vector2(Width, Width);
                //Vector2 offset = new Vector2(MyLevel.Rnd.Rnd.Next(-60, 0), MyLevel.Rnd.Rnd.Next(-60, 0) - size.Y);
                Vector2 offset = Vector2.Zero;
                if (i == 1) offset = new Vector2(50, 0);

                gblock = (GhostBlock)level.Recycle.GetObject(ObjectType.GhostBlock, false);
                
                // Box type
                if (Params.BoxType == GhostBlock_Parameters.BoxTypes.Long)
                    gblock.TallBox = true;
                else
                    gblock.TallBox = false;

                gblock.Init(pos + offset, size, level);

                gblock.BlockCore.BlobsOnTop = false;

                gblock.BlockCore.Decide_RemoveIfUnused(Params.KeepUnused.GetVal(pos), level.Rnd);
                gblock.BlockCore.GenData.EdgeSafety = GenData.Get(DifficultyParam.EdgeSafety, pos);
                gblock.TimeSafety = Params.TimeSafety.GetVal(pos);

                if (level.Style.RemoveBlockOnOverlap)
                    gblock.BlockCore.GenData.RemoveIfOverlap = true;


                gblock.InLength = InLength;
                gblock.OutLength = OutLength;
                gblock.Offset = Offset;
                //if (i == 1) gblock.Offset = Offset + (int)((InLength + OutLength) * .5f);
                //if (i == 2) gblock.Offset = Offset + (int)((InLength + OutLength) * .666f);

                GhostBlock block = gblock;
                block.Core.GenData.OnUsed = new OnGhostUsedLambda(block, level);

                // Box type
                if (Params.BoxType == GhostBlock_Parameters.BoxTypes.TopOnly)
                {
                    gblock.Box.TopOnly = true;
                    gblock.TallBox = false;
                }
                else if (Params.BoxType == GhostBlock_Parameters.BoxTypes.Full)
                {
                    gblock.Box.TopOnly = false;
                    gblock.TallBox = false;
                }
                else if (Params.BoxType == GhostBlock_Parameters.BoxTypes.Long)
                {
                    gblock.Box.TopOnly = false;
                    gblock.TallBox = true;
                }

                level.AddBlock(gblock);
            }

            return gblock;
        }

        class OnGhostUsedLambda : Lambda
        {
            GhostBlock block;
            Level level;

            public OnGhostUsedLambda(GhostBlock block, Level level)
            {
                this.block = block;
                this.level = level;
            }

            public void Apply()
            {
                if (block.Core.GenData.Used) return;

                int max = block.InLength - GhostBlock.LengthOfPhaseChange;
                block.ModOffset(level.Rnd.RndInt((int)(.25f * max), (int)(.75f * max)));
            }
        }
    }
}
