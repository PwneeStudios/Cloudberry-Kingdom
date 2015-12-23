using Microsoft.Xna.Framework;

namespace CloudberryKingdom.Levels
{
    public class ConveyorBlock_Parameters : AutoGen_Parameters
    {
        public Param Width, KeepUnused, Speed;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            KeepUnused = new Param(PieceSeed);
            if (level.DefaultHeroType is BobPhsxSpaceship)
            {
                KeepUnused.SetVal(u => BobPhsxSpaceship.KeepUnused(u[Upgrade.Conveyor]));
            }

            FillWeight = new Param(PieceSeed, u =>
                u[Upgrade.Conveyor] * (.5f + .5f * u[Upgrade.Conveyor] / 10f));

            Speed = new Param(PieceSeed, u =>
            {
                return DifficultyHelper.Interp(.04175f, .16f, u[Upgrade.Conveyor]) *
                    DifficultyHelper.Interp(1f, 1.55f, u[Upgrade.Speed]);
            });

            Width = new Param(PieceSeed, u =>
                DifficultyHelper.InterpRestrict19(240, 60, u[Upgrade.Conveyor]));
        }
    }

    public sealed class ConveyorBlock_AutoGen : AutoGen
    {
        static readonly ConveyorBlock_AutoGen instance = new ConveyorBlock_AutoGen();
        public static ConveyorBlock_AutoGen Instance { get { return instance; } }

        static ConveyorBlock_AutoGen() { }
        ConveyorBlock_AutoGen()
        {
            Do_WeightedPreFill_1 = true;
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            ConveyorBlock_Parameters Params = new ConveyorBlock_Parameters();
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

        public ConveyorBlock_Parameters GetParams(Level level)
        {
            return (ConveyorBlock_Parameters)level.Style.FindParams(ConveyorBlock_AutoGen.Instance);
        }

        public override ObjectBase CreateAt(Level level, Vector2 pos, Vector2 BL, Vector2 TR)
        {
            base.CreateAt(level, pos, BL, TR);

            StyleData Style = level.Style;
            RichLevelGenData GenData = level.CurMakeData.GenData;
            PieceSeedData piece = level.CurMakeData.PieceSeed;

            // Get ConveyorBlock parameters
            ConveyorBlock_Parameters Params = GetParams(level);

            int Width = (int)Params.Width.GetVal(pos);
            float Height = 60;
            Vector2 size = new Vector2(Width, Height);
            //float speed = Params.Speed.GetVal(pos);
            float speed = .35f * Params.Speed.GetVal(pos);

            speed *= -1;//level.Rnd.RndBit();

            ConveyorBlock conveyblock = (ConveyorBlock)level.Recycle.GetObject(ObjectType.ConveyorBlock, false);
            conveyblock.Init(pos, size);
            conveyblock.Speed = speed;

            conveyblock.BlockCore.BlobsOnTop = true;

            conveyblock.BlockCore.Decide_RemoveIfUnused(Params.KeepUnused.GetVal(pos), level.Rnd);
            conveyblock.BlockCore.GenData.EdgeSafety = GenData.Get(DifficultyParam.EdgeSafety, pos);

            if (level.Style.RemoveBlockOnOverlap)
                conveyblock.BlockCore.GenData.RemoveIfOverlap = true;

            level.AddBlock(conveyblock);

            return conveyblock;
        }
    }
}
