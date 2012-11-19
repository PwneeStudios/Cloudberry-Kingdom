using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Particles;

namespace CloudberryKingdom
{
    public partial class ParticleEffects
    {
        public static Particle DieTemplate;
        
        public static void Init_CoinTemplate()
        {
            DieTemplate = new Particle();
            DieTemplate.MyQuad.Init();
            DieTemplate.MyQuad.MyEffect = Tools.BasicEffect;
            DieTemplate.MyQuad.MyTexture = Tools.TextureWad.FindByName("Coin");

            DieTemplate.SetSize(45);
            DieTemplate.SizeSpeed = new Vector2(10, 10);
            DieTemplate.AngleSpeed = .013f;
            DieTemplate.Life = 20;
            DieTemplate.MyColor = new Vector4(1f, 1f, 1f, .75f);
            DieTemplate.ColorVel = new Vector4(0, 0, 0, -.065f);
        }

        public static void CoinDie_Old(Level level, Vector2 pos)
        {
            // Normal
            for (int j = 0; j < 3; j++)
            {
                //var p = level.ParticleEmitters[5].GetNewParticle(ParticleEffects.DieTemplate);
                var p = level.MainEmitter.GetNewParticle(ParticleEffects.DieTemplate);

                p.Data.Position = pos + level.Rnd.RndDir(35);
                p.MyQuad.MyTexture = Tools.TextureWad.FindByName("Pop");

                p.MyColor.W *= .6f;
            }
        }

        public static void CoinDie_Perfect(Level level, Vector2 pos)
        {
            // Perfect
            for (int j = 0; j < 3; j++)
            {
                var p = level.ParticleEmitters[5].GetNewParticle(ParticleEffects.DieTemplate);
                //var p = level.MainEmitter.GetNewParticle(ParticleEffects.DieTemplate);

                p.Data.Position = pos + level.Rnd.RndDir(35);
                p.MyQuad.MyTexture = Tools.TextureWad.FindByName("Sparkle");
                p.Data.Velocity = Tools.GlobalRnd.RndDir() * (Tools.GlobalRnd.RndFloat(10, 20));
                    p.Data.Velocity *= .8f;
                p.Size *= 2;
                p.SizeSpeed = new Vector2(40);
            }
        }

        public static void CoinDie_Spritely(Level level, Vector2 pos)
        {
            // Spritely
            for (int j = 0; j < 10; j++)
            {
                var p = level.ParticleEmitters[5].GetNewParticle(ParticleEffects.DieTemplate);
                //var p = level.MainEmitter.GetNewParticle(ParticleEffects.DieTemplate);

                p.Data.Position = pos;
                p.MyQuad.MyTexture = Tools.TextureWad.FindByName("Sparkle");
                p.Data.Velocity = Tools.GlobalRnd.RndDir() * (Tools.GlobalRnd.RndFloat(7, 9));
                p.Size *= 4.75f;
                p.SizeSpeed = new Vector2(0);
                p.Life = (int)(p.Life * 1.25f);
                p.ColorVel.W /= 1.25f;
            }
        }

        public static void CoinDie_ExtraLife(Level level, Vector2 pos)
        {
            // Spritely
            for (int j = 0; j < 10; j++)
            {
                var p = level.ParticleEmitters[5].GetNewParticle(ParticleEffects.DieTemplate);
                //var p = level.MainEmitter.GetNewParticle(ParticleEffects.DieTemplate);

                p.Data.Position = pos;
                p.MyQuad.MyTexture = Tools.TextureWad.FindByName("CoinCollect");
                p.Data.Velocity = Tools.GlobalRnd.RndDir() * (Tools.GlobalRnd.RndFloat(7, 10));
                p.Size *= 5f;
                p.SizeSpeed = new Vector2(0);
                p.Life = (int)(p.Life * 1.25f);
                p.ColorVel.W /= 1.25f;
            }
        }

        public static void CoinDie_New(Level level, Vector2 pos)
        {
            pos += new Vector2(22, 0);

            // Coin collect
            for (int j = 0; j < 1; j++)
            {
                var p = level.ParticleEmitters[5].GetNewParticle(ParticleEffects.DieTemplate);
                //var p = level.MainEmitter.GetNewParticle(ParticleEffects.DieTemplate);

                p.Data.Position = pos;
                p.MyQuad.MyTexture = Tools.TextureWad.FindByName("CoinCollect");
                p.Data.Velocity = Vector2.Zero;
                p.AngleSpeed = -.0525f;
                p.Size *= 2.7f;
                p.SizeSpeed = new Vector2(-10);
                p.Life = (int)(p.Life * 1.35f);
                p.ColorVel.W *= 1f;
                p.MyColor.W *= .5f;
            }

            for (int j = 0; j < 3; j++)
            {
                var p = level.ParticleEmitters[5].GetNewParticle(ParticleEffects.DieTemplate);
                //var p = level.MainEmitter.GetNewParticle(ParticleEffects.DieTemplate);

                p.Data.Position = pos;
                //p.MyQuad.MyTexture = Tools.TextureWad.FindByName("Sparkle");
                p.MyQuad.MyTexture = Tools.TextureWad.FindByName("CoinCollect");
                p.Data.Velocity = CoreMath.DegreesToDir(120 * j) * (Tools.GlobalRnd.RndFloat(5, 8));
                p.Size *= 1.5f;
                p.SizeSpeed = new Vector2(0);
                p.Life = (int)(p.Life * 1.25f);
                p.ColorVel.W /= 1.25f;
                p.MyColor.W *= .7f;
            }
        }
    }
}
