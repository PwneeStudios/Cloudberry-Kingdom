using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Particles;

namespace CloudberryKingdom
{
    public partial class ParticleEffects
    {
        public static Particle CoinExplosionTemplate;

        public static void Init_CoinExplosion()
        {
            CoinExplosionTemplate = new Particle();

            CoinExplosionTemplate.MyQuad.Init();
            CoinExplosionTemplate.MyQuad.MyEffect = Tools.BasicEffect;
            CoinExplosionTemplate.MyQuad.MyTexture = Tools.TextureWad.FindByName("checkpoint3");
            CoinExplosionTemplate.Size = new Vector2(45, 70) * 1.15f;
            CoinExplosionTemplate.SizeSpeed = new Vector2(1.5f, 1.5f);
            CoinExplosionTemplate.AngleSpeed = .02f;
            CoinExplosionTemplate.Life = 210;
            CoinExplosionTemplate.MyColor = new Vector4(1, 1, 1, 1);
            CoinExplosionTemplate.ColorVel = new Vector4(0, 0, 0, 0);
            CoinExplosionTemplate.Data.Acceleration = new Vector2(0, -1.2f);
            CoinExplosionTemplate.KillOffBottom = CoinExplosionTemplate.KillOffSides = true;
        }

        static void SetRandomCoin(Particle p)
        {
            switch (Tools.GlobalRnd.RndInt(0, 6))
            {
                case 0: p.MyQuad.TextureName = "CoinBlue"; break;
                case 1: p.MyQuad.TextureName = "CoinCyan"; break;
                case 2: p.MyQuad.TextureName = "CoinGreen"; break;
                case 3: p.MyQuad.TextureName = "CoinOrange"; break;
                case 4: p.MyQuad.TextureName = "CoinPurple"; break;
                case 5: p.MyQuad.TextureName = "CoinRed"; break;
                case 6: p.MyQuad.TextureName = "CoinYellow"; break;
            }
        }

        public static void CoinExplosion(Level level, Vector2 pos)
        {
            ParticleEmitter emit = level.MainEmitter;

            float intensity = 1f;

            int i;
            for (int k = 0; k < 14; k++)
            {
                var p = emit.GetNewParticle(CoinExplosionTemplate);

                Vector2 Dir = level.Rnd.RndDir();

                p.Data.Position = pos + 45 * Dir * intensity;
                p.Data.Velocity = 45 * intensity * (float)level.Rnd.Rnd.NextDouble() * Dir
                                                    + new Vector2(0, 13);
                p.Angle = level.Rnd.RndFloat(0, 100);
                p.Size *= intensity;

                //p.MyQuad.TextureName = "CoinBlue";
                p.MyQuad.TextureName = "CoinGreen";
                //SetRandomCoin(emit, i);
            }
        }
    }
}
