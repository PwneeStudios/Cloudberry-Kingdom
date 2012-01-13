namespace CloudberryKingdom.Levels
{
    public class VerticalData : StyleData
    {
        public enum VisualStyles { Pillar, Castle };
        public VisualStyles VisualStyle;
    }

    public class UpData : VerticalData
    {
        protected override void CalculateKeepUnused(float JumpLevel)
        {
            // Extra fill: keep unused
            //float ChanceToHaveUnused = .4f - .1f * (.4f - -.2f) * JumpLevel;
            //if (Tools.RndFloat(0, 1) < ChanceToHaveUnused)
            {
                float MaxChance = .105f;// .135f;
                float chance = MaxChance - .1f * (MaxChance - 0) * JumpLevel;
                ChanceToKeepUnused = Tools.RndFloat(0, chance);
            }
        }

        public UpData()
        {
            VisualStyle = VisualStyles.Castle;
        }

        public override void Randomize()
        {
            base.Randomize();
        }
    }
}