using System;

namespace CloudberryKingdom
{
    public partial class Seed : ObjectBase, IObject, ILevelConnector
    {
        public DoorAction OnOpen { get; set; }

        int StartStep = 0;
        void Start_Init()
        {
            StartStep = 0;
        }

        void CoalescingRing(int Step, int Length)
        {
            float s = 9f / Length;

            int RingDensity = (int)Math.Max(1, 9f - s * Step);
            int PadLife = (int)Math.Max(1, 7f - s * Step);
            for (int i = 0; i < RingDensity; i++)
                ParticleEffects.Coalesce(Core.MyLevel, Core.Data.Position, PadLife);
        }

        void Start_PhsxStep()
        {
            StartStep++;

            CoalescingRing(StartStep, 30);
            if (StartStep > 27)
                ParticleEffects.Coalesce(Core.MyLevel, Core.Data.Position);

            float ExtraIntensity = 1.0f - (float)Math.Pow(Math.Abs(StartStep - 60) / 10f, 2); //Math.Max(0.1f, 1f - .03f * (StartStep - 45));
            if (StartStep >= 45 && StartStep <= 75)
            {
                //ParticleEffects.PieceOrb(Core.MyLevel, ParticleEffects.PieceOrbStyle.Cloud, Core.Data.Position, Core.MyLevel.CurPhsxStep, 3 * Intensity);
                for (int i = 0; i < 4; i++)
                {
                    ParticleEffects.PieceOrb(Core.MyLevel, ParticleEffects.PieceOrbStyle.Cloud, Core.Data.Position + (75 + ExtraIntensity * 80) * Tools.AngleToDir(i * 2 * Math.PI / 4), Core.MyLevel.CurPhsxStep, .7f * ExtraIntensity);
                }
            }

            if (StartStep > 55)
                ParticleEffects.PieceOrb(Core.MyLevel, ParticleEffects.PieceOrbStyle.Cloud, Core.Data.Position, Core.MyLevel.CurPhsxStep, Intensity);

            if (StartStep > 65)
                SetState(State.On);
        }
    }
}