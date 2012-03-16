namespace CloudberryKingdom.Levels
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
            //float ChanceToHaveUnused = .4f - .1f * (.4f - -.2f) * JumpLevel;
            //if (Rnd.RndFloat(0, 1) < ChanceToHaveUnused)
            {
                //float MaxChance = .105f;
                float MaxChance = .075f;
                float chance = MaxChance - .1f * (MaxChance - 0) * JumpLevel;
                ChanceToKeepUnused = Rnd.RndFloat(0, chance);
            }
        }

        public UpData(Rand Rnd)
            : base(Rnd)
        {
            VisualStyle = VisualStyles.Castle;
        }

        public override void Randomize()
        {
            base.Randomize();
        }
    }
}