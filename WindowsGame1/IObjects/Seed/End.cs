using System;

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public partial class Seed : ObjectBase, IObject, ILevelConnector
    {
        int EndStep = 0;
        void End_Init()
        {
            EndStep = 0;
        }

        void End_PhsxStep()
        {
            EndStep++;

            // Suck in stickmen to center
            foreach (Bob bob in Core.MyLevel.Bobs)
                if (bob.SuckedIn && bob.SuckedInSeed == this)
                {
                    bob.Core.Data.Velocity *= .8f;
                    bob.Core.Data.Position += .8f * (Core.Data.Position - bob.Core.Data.Position);
                }

            int RingDensity = (int)Math.Max(1, 9f - .3f * EndStep);
            int PadLife = (int)Math.Max(1, 7f - .3f * EndStep);
            if (RingDensity > 1)
                for (int i = 0; i < RingDensity; i++)
                    ParticleEffects.Coalesce(Core.MyLevel, Core.Data.Position, PadLife);

            float ModIntensity = 1f - EndStep / 45f;
            ParticleEffects.PieceOrb(Core.MyLevel, ParticleEffects.PieceOrbStyle.Cloud, Core.Data.Position, Core.MyLevel.CurPhsxStep, Intensity * ModIntensity);

            if (EndStep > 45)
                SetState(State.Off);
        }
    }
}