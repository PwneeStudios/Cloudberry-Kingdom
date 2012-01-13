namespace CloudberryKingdom.Levels
{
    public class DownData : VerticalData
    {
        protected override void CalculateKeepUnused(float JumpLevel)
        {
            {
                float MaxChance = .05f;
                float chance = MaxChance - .1f * (MaxChance - 0) * JumpLevel;
                ChanceToKeepUnused = Tools.RndFloat(0, chance);
            }
        }

        public DownData()
        {
            VisualStyle = VisualStyles.Castle;
        }

        public override void Randomize()
        {
            base.Randomize();
        }
    }
}