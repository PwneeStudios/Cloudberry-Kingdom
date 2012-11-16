using Microsoft.Xna.Framework;



namespace CloudberryKingdom
{
    public class Cloud_Parameters : AutoGen_Parameters
    {
        public Param Size, Shiftiness;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            var u = PieceSeed.u;

            FillWeight = new Param(PieceSeed);
            FillWeight.SetVal(.62f * u[Upgrade.Cloud]);

            Shiftiness = new Param(PieceSeed);
            Shiftiness.SetVal(1 + .33f * u[Upgrade.Cloud]);

            Size = new Param(PieceSeed);
            Size.SetVal(2f - .1f * u[Upgrade.Cloud]);
        }
    }

    public sealed class Cloud_AutoGen : AutoGen
    {
        static readonly Cloud_AutoGen instance = new Cloud_AutoGen();
        public static Cloud_AutoGen Instance { get { return instance; } }

        static Cloud_AutoGen() { }
        Cloud_AutoGen()
        {
            Do_WeightedPreFill_1 = true;
            //Generators.AddGenerator(this);
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            Cloud_Parameters Params = new Cloud_Parameters();
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

            // Get Cloud parameters
            Cloud_Parameters Params = (Cloud_Parameters)level.Style.FindParams(Cloud_AutoGen.Instance);

            // Make the new cloud
            pos += new Vector2(level.Rnd.Rnd.Next(0, 70), level.Rnd.Rnd.Next(0, 70));
            Cloud NewCloud = (Cloud)level.MySourceGame.Recycle.GetObject(ObjectType.Cloud, true);
            
            NewCloud.Shiftiness = Params.Shiftiness.GetVal(pos);
            NewCloud.Init(pos, level);

            NewCloud.Core.GenData.RemoveIfUnused = true;

            if (level.Style.RemoveBlockOnOverlap)
                NewCloud.Core.GenData.RemoveIfOverlap = true;

            level.AddObject(NewCloud);

            return NewCloud;
        }
    }
}
