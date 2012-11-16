

namespace CloudberryKingdom
{
    public class VerticalData : StyleData
    {
        public VerticalData(Rand Rnd) : base(Rnd) { }

        public enum VisualStyles { Pillar, Castle };
        public VisualStyles VisualStyle;
    }

    public class UpData : VerticalData
    {
        protected override void CalculateKeepUnused(float JumpLevel)
        {
            // Extra fill: keep unused
            //if (Rnd.RndFloat(0, 1) < ChanceToHaveUnused)
            {
                float MaxChance = .075f;
                float chance = MaxChance - .1f * (MaxChance - 0) * JumpLevel;
                ChanceToKeepUnused = Rnd.RndFloat(0, chance);
            }
        }

        public UpData(Rand Rnd)
            : base(Rnd)
        {
            VisualStyle = VisualStyles.Castle;
            BlockFillType = _BlockFillType.TopOnly;
        }

        public override void Randomize()
        {
            base.Randomize();
        }
    }
}