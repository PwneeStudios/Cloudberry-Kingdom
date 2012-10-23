using CoreEngine;
using CoreEngine.Random;

namespace CloudberryKingdom.Levels
{
    public class DownData : VerticalData
    {
        protected override void CalculateKeepUnused(float JumpLevel)
        {
            {
                float MaxChance = .05f;
                float chance = MaxChance - .1f * (MaxChance - 0) * JumpLevel;
                ChanceToKeepUnused = Rnd.RndFloat(0, chance);
            }
        }

        public DownData(Rand Rnd)
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