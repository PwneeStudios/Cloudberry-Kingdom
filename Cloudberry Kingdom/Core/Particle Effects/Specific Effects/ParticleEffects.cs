using System;

using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Particles;
using CloudberryKingdom.Obstacles;

namespace CloudberryKingdom
{
    public partial class ParticleEffects
    {
        public static Particle SingleAnimatedParticle(Vector2 Pos, Vector2 Size, PhsxData Data, EzTexture Texture)
        {
            var p = new Particle();
            p.MyQuad.Init();
            p.MyQuad.MyEffect = Tools.BasicEffect;
            p.MyQuad.MyTexture = Texture;
            p.Size = Size;
            p.SizeSpeed = new Vector2(0);
            p.AngleSpeed = 0;
            p.Life = 20;
            p.MyColor = new Vector4(1f, 1f, 1f, 1f);
            p.ColorVel = new Vector4(0, 0, 0, 0);

            return p;
        }

        static Particle DustCloudTemplate;
        static EzSound DustCloudSound;

        static Particle FlameTemplate;
        static Particle ThrustTemplate;
        static Particle PieceExplosionTemplate;
        static Particle CoalesceTemplate;

        public static void Init()
        {
            Init_CoinTemplate();
            Init_Coalesce();
            Init_PieceExplosion();
            Init_CoinExplosion();
            Init_DustCloud();
            Init_Flame();
            Init_Thrust();
            Init_Pop();
        }

        public static void Init_Coalesce()
        {
            CoalesceTemplate = new Particle();

            CoalesceTemplate.MyQuad.Init();
            CoalesceTemplate.MyQuad.MyEffect = Tools.BasicEffect;
            CoalesceTemplate.MyQuad.MyTexture = Tools.TextureWad.FindByName("Cloud1");
            CoalesceTemplate.SetSize(145);
            CoalesceTemplate.SizeSpeed = new Vector2(4, 4);
            CoalesceTemplate.AngleSpeed = -.03f;
            CoalesceTemplate.Life = 33;
            CoalesceTemplate.MyColor = new Vector4(1, .98f, .98f, 0);
            CoalesceTemplate.ColorVel = new Vector4(0, 0, 0, 0.014f);
            CoalesceTemplate.UseAttraction = true;
            CoalesceTemplate.AttractionStrength = 2.43f;
        }

        public static void Init_PieceExplosion()
        {
            PieceExplosionTemplate = new Particle();

            PieceExplosionTemplate.MyQuad.Init();
            PieceExplosionTemplate.MyQuad.MyEffect = Tools.BasicEffect;
            PieceExplosionTemplate.MyQuad.MyTexture = Tools.TextureWad.FindByName("Smoke2");
            PieceExplosionTemplate.SetSize(145);
            PieceExplosionTemplate.SizeSpeed = new Vector2(4, 4);
            PieceExplosionTemplate.AngleSpeed = .02f;
            PieceExplosionTemplate.Life = 210;
            PieceExplosionTemplate.MyColor = new Vector4(1, 1, 1, 1);
            PieceExplosionTemplate.ColorVel = new Vector4(0, 0, 0, 0);
            PieceExplosionTemplate.Data.Acceleration = new Vector2(0, -1.2f);
            PieceExplosionTemplate.KillOffBottom = PieceExplosionTemplate.KillOffSides = true;
        }

        public static void Init_DustCloud()
        {
            DustCloudSound = Tools.SoundWad.FindByName("DustCloud_Explode");

            DustCloudTemplate = new Particle();
          
            DustCloudTemplate.MyQuad.Init();
            DustCloudTemplate.MyQuad.MyEffect = Tools.BasicEffect;
            DustCloudTemplate.MyQuad.MyTexture = Tools.TextureWad.FindByName("Smoke2");
            DustCloudTemplate.SetSize(120);
            DustCloudTemplate.SizeSpeed = new Vector2(4, 4);
            DustCloudTemplate.AngleSpeed = .02f;
            DustCloudTemplate.Life = 48;
            DustCloudTemplate.MyColor = new Vector4(1.15f, 1.15f, 1.15f, 1.1f);
            DustCloudTemplate.ColorVel = new Vector4(0.02f, 0.02f, 0.02f, -.023f);
            DustCloudTemplate.Data.Acceleration = new Vector2(0, .60f);
        }

        public static void Init_Flame()
        {
            FlameTemplate = new Particle();

            FlameTemplate.MyQuad.Init();
            FlameTemplate.MyQuad.UseGlobalIllumination = false;
            FlameTemplate.MyQuad.MyEffect = Tools.BasicEffect;
            FlameTemplate.MyQuad.MyTexture = Fireball.FlameTexture;// Tools.TextureWad.FindByName("Fire4");
            FlameTemplate.SetSize(196);//106);
            FlameTemplate.SizeSpeed = new Vector2(-.3f, -.3f);
            FlameTemplate.AngleSpeed = .015f;
            FlameTemplate.Life = 30;
            FlameTemplate.MyColor = new Vector4(.9f, 1f, 1f, 0f);
            FlameTemplate.ColorVel = new Vector4(0.0007f, -0.0007f, -0.0007f, -.06f);
            FlameTemplate.Data.Acceleration = new Vector2(0, .39f);//.35f);

            FlameTemplate.FadingIn = true;
            FlameTemplate.FadeInColorVel = new Vector4(0, 0, 0, .05f);//.07f);
            FlameTemplate.FadeInTargetAlpha = 1;// .5f;
        }

        public static void Init_Thrust()
        {
            ThrustTemplate = new Particle();

            ThrustTemplate.MyQuad.Init();
            ThrustTemplate.MyQuad.UseGlobalIllumination = false;
            ThrustTemplate.MyQuad.MyEffect = Tools.BasicEffect;
            ThrustTemplate.MyQuad.MyTexture = Fireball.FlameTexture;// Tools.TextureWad.FindByName("Fire");
            ThrustTemplate.SetSize(60);//33);
            ThrustTemplate.SizeSpeed = new Vector2(-.3f, -.3f);
            ThrustTemplate.AngleSpeed = .05f;
            ThrustTemplate.Life = 15;
            //ThrustTemplate.MyColor = new Vector4(1.15f, 1.15f, 1.15f, 1.1f);
            ThrustTemplate.MyColor = new Vector4(.93f, .9f, .9f, 1.1f);
            ThrustTemplate.ColorVel = new Vector4(0.02f, 0.02f, 0.02f, -.07f);
            ThrustTemplate.Data.Acceleration = new Vector2(0, 0);

            ThrustTemplate.MyQuad.BlendAddRatio = .5f;
        }

        public static void CartThrust(Level level, int Layer, Vector2 pos, Vector2 dir, Vector2 vel)
        {
            if (level.GetPhsxStep() % 2 != 0) return;

            ParticleEmitter emit = level.ParticleEmitters[Layer];

            for (int k = 0; k < 1; k++)
            {
                var p = emit.GetNewParticle(ThrustTemplate);
                p.Size *= 2;
                p.Life = 9;
                p.MyColor.X *= .8f;
                p.MyColor.Y *= .8f;
                p.MyColor.Z *= .8f;
                Vector2 Dir = Tools.GlobalRnd.RndDir();

                p.Data.Position = pos + 3 * Dir;
                p.Data.Velocity = vel + 13 * (Tools.GlobalRnd.RndFloat(.6f, .8f)) * dir + 1 * Dir
                    + new Vector2(0, (float)Math.Sin(Tools.t*23) * 5);
            }
        }

        public static void Thrust(Level level, int Layer, Vector2 pos, Vector2 dir, Vector2 vel, float scale)
        {
            //if (level.GetPhsxStep() % 3 != 0) return;

            ParticleEmitter emit = level.ParticleEmitters[Layer];

            for (int k = 0; k < 1; k++)
            {
                var p = emit.GetNewParticle(ThrustTemplate);
                Vector2 Dir = Tools.GlobalRnd.RndDir();

                p.Data.Position = pos + 3 * Dir;
                p.Data.Velocity = vel + 23 * (.6f + .2f * (float)Tools.GlobalRnd.Rnd.NextDouble()) * dir + 1 * Dir;

                p.Size *= scale;
            }
        }

        public static void HeroExplosion(Level level, Vector2 pos)
        {
            ParticleEmitter emit = level.MainEmitter;

            for (int k = 0; k < 37; k++)
            {
                var p = emit.GetNewParticle(DustCloudTemplate);
                Vector2 Dir = Tools.GlobalRnd.RndDir();

                float scale = 1.25f;
                p.Data.Position = pos + 70 * Dir * scale;
                p.Data.Velocity = 10 * (float)Tools.GlobalRnd.Rnd.NextDouble() * Dir * scale;
                p.Size *= scale;
                if (k % 2 == 0)
                {
                    p.Data.Velocity.Y = -.5f * Math.Abs(p.Data.Velocity.Y);
                    p.Data.Velocity.Y -= 10;
                }
                else
                {
                    p.Data.Velocity.Y -= 10;
                    //p.Data.Velocity.Y /= 3;
                }
            }

            DustCloudSound.Play();
        }

        public static void DustCloudExplosion(Level level, Vector2 pos)
        {
            ParticleEmitter emit = level.MainEmitter;

            for (int k = 0; k < 27; k++)
            {
                var p = emit.GetNewParticle(DustCloudTemplate);
                Vector2 Dir = Tools.GlobalRnd.RndDir();
                                
                p.Data.Position = pos + 70 * Dir;
                p.Data.Velocity = 10 * (float)Tools.GlobalRnd.Rnd.NextDouble() * Dir;
                if (k % 2 == 0)
                {
                    p.Data.Velocity.Y = -.5f * Math.Abs(p.Data.Velocity.Y);
                    p.Data.Velocity.Y -= 10;
                }
                else
                {
                    p.Data.Velocity.Y -= 10;
                    //p.Data.Velocity.Y /= 3;
                }
            }

            DustCloudSound.Play(.2f);
        }

        public static void Flame(Level level, Vector2 pos, int frame, float intensity, int life, bool ModFade)
        {
            Flame(level.MainEmitter, pos, frame, intensity, life, ModFade);
        }
        public static void Flame(ParticleEmitter emitter, Vector2 pos, int frame, float intensity, int life, bool ModFade)
        {
            //if (frame % 3 != 0) return;
            //if (Tools.TheGame.DrawCount % 3 == 0) return;

            ParticleEmitter emit = emitter;

            for (int k = 0; k < 1; k++)
            {
                var p = emit.GetNewParticle(FlameTemplate);
                Vector2 Dir = Tools.GlobalRnd.RndDir();

                if (ModFade)
                {
                    p.FadeInColorVel.W *= 55 / life;
                    p.ColorVel.W *= 55 / life;
                }

                p.Data.Position = pos + 35 * Dir * intensity;
                p.Data.Velocity = .35f * 2 * intensity * (float)Tools.GlobalRnd.Rnd.NextDouble() * Dir;
                //p.Data.Acceleration = new Vector2(-.5f, 0);
                //p.Data.Acceleration = -.25f * p.Data.Velocity;
                    p.Data.Acceleration = -.5f * p.Data.Velocity;
                    p.Life = life;
                    p.AngleSpeed /= 2;
                p.Angle = Tools.GlobalRnd.RndFloat(0, 100);
                p.Size *= intensity;
            }
        }

        static void SetRandomPiece(Particle p)
        {
            switch (Tools.GlobalRnd.Rnd.Next(0, 8))
            {
                case 0:
                    p.MyQuad.Set("FallingBlock2");
                    break;

                case 1:
                    p.MyQuad.Set("FallingBlock1");
                    break;

                case 2:
                    p.MyQuad.Set("blue_small");
                    break;

                case 3:
                    p.MyQuad.Set("fading_block");
                    p.Size *= new Vector2(1f, 1.11f);
                    break;

                case 4:
                    p.MyQuad.Set("SpikeyGuy");
                    p.Size *= new Vector2(1.2f, 1.6f);
                    break;

                case 5:
                    p.MyQuad.Set("blob2_body");
                    break;

                case 6:
                    p.MyQuad.Set("checkpoint3");
                    p.Size *= new Vector2(1, 1);
                    break;

                case 7:
                    p.MyQuad.Set("CoinBlue");
                    p.Size *= new Vector2(.3f, .7f);
                    break;
            }
        }

        public static void Coalesce(Level level, Vector2 pos)
        {
            Coalesce(level, pos, 0);
        }
        public static void Coalesce(Level level, Vector2 pos, int PadLife)
        {
            ParticleEmitter emit = level.MainEmitter;

            int num = 1;
            if (Tools.TheGame.DrawCount % 2 == 0) num = 0;

            for (int k = 0; k < num; k++)
            {
                var p = emit.GetNewParticle(CoalesceTemplate);
                Vector2 Dir = Tools.GlobalRnd.RndDir();

                p.Data.Position = pos + 1500 * Dir;
                
                p.Angle = Tools.GlobalRnd.RndFloat(0, 100);
                //p.Size *= intensity;

                p.AttractionPoint = pos;

                Vector2 dif = pos - p.Data.Position;
                dif.Normalize();
                p.Data.Velocity = 9 * new Vector2(-dif.Y, dif.X);

                //p.SizeSpeed = Vector2.Zero;
                p.Size *= 1.25f;
                //p.Life += 16;
                //SetRandomPiece(emit, i);

                p.Life += PadLife;
            }
        }

        public static void PieceExplosion(Level level, Vector2 pos, int frame, float intensity)
        {
            ParticleEmitter emit = level.MainEmitter;

            int num = 2;
            if (Tools.TheGame.DrawCount % 2 == 0) num = 3;
            num *= 2;

            for (int k = 0; k < num; k++)
            {
                var p = emit.GetNewParticle(PieceExplosionTemplate);
                Vector2 Dir = Tools.GlobalRnd.RndDir();

                p.Data.Position = pos + 75 * Dir * intensity;
                p.Data.Velocity = 125 * intensity * (float)Tools.GlobalRnd.Rnd.NextDouble() * Dir;
                p.Angle = Tools.GlobalRnd.RndFloat(0, 100);
                p.Size *= intensity;

                SetRandomPiece(p);
            }
        }


        public enum PieceOrbStyle { BigRnd, Cloud, Fire };
        public static void PieceOrb(Level level, PieceOrbStyle style, Vector2 pos, int frame, float intensity)
        {
            PieceOrb(level, style, pos, frame, intensity, null, Vector2.One, Vector4.One);
        }
        public static void PieceOrb(Level level, PieceOrbStyle style, Vector2 pos, int frame, float intensity, EzTexture texture, Vector2 size, Vector4 color)
        {
            // Number of particles to emit
            int num = 1;
            if (Tools.TheGame.DrawCount % 2 == 0) num = 2;

            float vel = 1, acc = 1, angle_vel = 1;
            int life = 20;
            switch (style)
            {
                case PieceOrbStyle.BigRnd:
                    num *= 2;

                    float t = 2 - intensity;
                    float s = intensity - 1;
                    vel = s * 130 + t * 60;
                    acc = s * -.182f + t * -.128f;
                    life = (int)(s * 12.5f + t * 18);
                    size *= s * 1.3f + t * 1.15f;
                    
                    break;

                case PieceOrbStyle.Cloud:
                    num = 1;

                    vel = 50;
                    acc = -.128f;
                    color *= new Vector4(.90f, .815f, .815f, .7f);
                    size *= 1.2f * 1.5f * 1.5f;
                    angle_vel = -10f;
                    if (texture == null)
                        texture = Tools.TextureWad.FindByName("Cloud1");
                    life = 20;
                    break;

                case PieceOrbStyle.Fire:
                    num = 1;
                    
                    vel = 50;
                    acc = -.128f;
                    color *= new Vector4(.90f, .815f, .815f, .7f);
                    size *= 1.2f * 1.5f * 1.5f;
                    angle_vel = -10f;
                    if (texture == null)
                        texture = Fireball.EmitterTexture;
                    life = 20;
                    break;
            }

            ParticleEmitter emit = level.MainEmitter;

            for (int k = 0; k < num; k++)
            {
                var p = emit.GetNewParticle(PieceExplosionTemplate);
                Vector2 Dir = Tools.GlobalRnd.RndDir();

                p.MyColor *= color;

                p.Data.Position = pos + 75 * Dir;
                //p.Data.Velocity = 25 * intensity * (float)Tools.GlobalRnd.Rnd.NextDouble() * Dir;
                //p.Data.Velocity = 18 * intensity * (float)Tools.GlobalRnd.Rnd.NextDouble() * Dir;
                //p.Data.Velocity = 50 * intensity * (float)Tools.GlobalRnd.Rnd.NextDouble() * Dir;
                p.Data.Velocity = vel * (float)Tools.GlobalRnd.Rnd.NextDouble() * Dir;
                    p.Data.Position += Tools.GlobalRnd.RndFloat(0, 2) * p.Data.Velocity;
                p.Angle = Tools.GlobalRnd.RndFloat(0, 100);
                p.AngleSpeed *= angle_vel;
                p.Size *= size;
                //p.Data.Acceleration = -.15f * p.Data.Velocity;
                //p.Data.Acceleration = -.28f * p.Data.Velocity;                
                //p.Data.Acceleration = -.128f * p.Data.Velocity;           
                p.Data.Acceleration = acc * p.Data.Velocity;

                //p.Life = 15;
                //p.Life = 10;
                //p.Life = 20;
                p.Life = life;

                if (texture == null)
                    SetRandomPiece(p);
                else
                    p.MyQuad.MyTexture = texture;
            }
        }

        /// <summary>
        /// Create an explosion of pieces at the specified location, plus a fart sound.
        /// </summary>
        public static void PiecePopFart(Level level, Vector2 pos)
        {
            Tools.SoundWad.FindByName("Piece_Explosion_Small").Play(1f);
            for (int i = 0; i < 4; i++)
                PieceExplosion(level, pos, 0, 1);
        }

        static Particle PopTemplate;
        public static void AddPop(Level level, Vector2 pos)
        {
            AddPop(level, pos, 85, PopTemplate.MyQuad.MyTexture);
        }
        public static void AddPop(Level level, Vector2 pos, float size)
        {
            AddPop(level, pos, size, PopTemplate.MyQuad.MyTexture);
        }
        public static void AddPop(Level level, Vector2 pos, float size, EzTexture tex)
        {
            if (level.NoParticles) return;

            var p = level.MainEmitter.GetNewParticle(PopTemplate);
            p.Data.Position = pos;
            p.SetSize(size);
            p.MyQuad.MyTexture = tex;
        }

        static void Init_Pop()
        {
            PopTemplate = new Particle();
            PopTemplate.MyQuad.Init();
            PopTemplate.MyQuad.MyEffect = Tools.EffectWad.FindByName("Shell");
            PopTemplate.MyQuad.MyTexture = Tools.TextureWad.FindByName("White");
            PopTemplate.SetSize(85);
            PopTemplate.SizeSpeed = new Vector2(10, 10);
            PopTemplate.AngleSpeed = 0;
            PopTemplate.Life = 20;
            PopTemplate.MyColor = new Vector4(1f, 1f, 1f, .75f);
            PopTemplate.ColorVel = new Vector4(0, 0, 0, -.065f);
        }
    }
}
