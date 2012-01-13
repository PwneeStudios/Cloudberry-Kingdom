using System;
using Microsoft.Xna.Framework;

using Drawing;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Particles;

namespace CloudberryKingdom
{
    public partial class ParticleEffects
    {
        static Particle DustCloudTemplate;
        static EzSound DustCloudSound;

        static Particle FlameTemplate;
        static Particle ThrustTemplate;
        static Particle PieceExplosionTemplate;
        static Particle CoalesceTemplate;

        public static void Init()
        {
            Init_Coalesce();
            Init_PieceExplosion();
            Init_CoinExplosion();
            Init_DustCloud();
            Init_Flame();
            Init_Thrust();
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

            int i;
            for (int k = 0; k < 1; k++)
            {
                i = emit.GetNextSlot();
                emit.Particles[i] = ThrustTemplate;
                emit.Particles[i].Size *= 2;
                emit.Particles[i].Life = 9;
                emit.Particles[i].MyColor.X *= .8f;
                emit.Particles[i].MyColor.Y *= .8f;
                emit.Particles[i].MyColor.Z *= .8f;
                Vector2 Dir = Tools.RndDir();

                emit.Particles[i].Data.Position = pos + 3 * Dir;
                emit.Particles[i].Data.Velocity = vel + 13 * (Tools.RndFloat(.6f, .8f)) * dir + 1 * Dir
                    + new Vector2(0, (float)Math.Sin(Tools.t*23) * 5);
            }
        }

        public static void Thrust(Level level, int Layer, Vector2 pos, Vector2 dir, Vector2 vel)
        {
            //if (level.GetPhsxStep() % 3 != 0) return;

            ParticleEmitter emit = level.ParticleEmitters[Layer];

            int i;
            for (int k = 0; k < 1; k++)
            {
                i = emit.GetNextSlot();
                emit.Particles[i] = ThrustTemplate;
                Vector2 Dir = Tools.RndDir();

                emit.Particles[i].Data.Position = pos + 3 * Dir;
                emit.Particles[i].Data.Velocity = vel + 23 * (.6f + .2f * (float)Tools.Rnd.NextDouble()) * dir + 1 * Dir;
            }
        }

        public static void HeroExplosion(Level level, Vector2 pos)
        {
            ParticleEmitter emit = level.MainEmitter;

            int i;
            for (int k = 0; k < 37; k++)
            {
                i = emit.GetNextSlot();
                emit.Particles[i] = DustCloudTemplate;
                Vector2 Dir = Tools.RndDir();

                float scale = 1.25f;
                emit.Particles[i].Data.Position = pos + 70 * Dir * scale;
                emit.Particles[i].Data.Velocity = 10 * (float)Tools.Rnd.NextDouble() * Dir * scale;
                emit.Particles[i].Size *= scale;
                if (k % 2 == 0)
                {
                    emit.Particles[i].Data.Velocity.Y = -.5f * Math.Abs(emit.Particles[i].Data.Velocity.Y);
                    emit.Particles[i].Data.Velocity.Y -= 10;
                }
                else
                {
                    emit.Particles[i].Data.Velocity.Y -= 10;
                    //emit.Particles[i].Data.Velocity.Y /= 3;
                }
            }

            DustCloudSound.Play();
        }

        public static void DustCloudExplosion(Level level, Vector2 pos)
        {
            ParticleEmitter emit = level.MainEmitter;

            int i;
            for (int k = 0; k < 27; k++)
            {
                i = emit.GetNextSlot();
                emit.Particles[i] = DustCloudTemplate;
                Vector2 Dir = Tools.RndDir();
                                
                emit.Particles[i].Data.Position = pos + 70 * Dir;
                emit.Particles[i].Data.Velocity = 10 * (float)Tools.Rnd.NextDouble() * Dir;
                if (k % 2 == 0)
                {
                    emit.Particles[i].Data.Velocity.Y = -.5f * Math.Abs(emit.Particles[i].Data.Velocity.Y);
                    emit.Particles[i].Data.Velocity.Y -= 10;
                }
                else
                {
                    emit.Particles[i].Data.Velocity.Y -= 10;
                    //emit.Particles[i].Data.Velocity.Y /= 3;
                }
            }

            DustCloudSound.Play();
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

            int i;
            for (int k = 0; k < 1; k++)
            {
                i = emit.GetNextSlot();
                emit.Particles[i] = FlameTemplate;
                Vector2 Dir = Tools.RndDir();

                if (ModFade)
                {
                    emit.Particles[i].FadeInColorVel.W *= 55 / life;
                    emit.Particles[i].ColorVel.W *= 55 / life;
                }

                emit.Particles[i].Data.Position = pos + 35 * Dir * intensity;
                emit.Particles[i].Data.Velocity = .35f * 2 * intensity * (float)Tools.Rnd.NextDouble() * Dir;
                //emit.Particles[i].Data.Acceleration = new Vector2(-.5f, 0);
                //emit.Particles[i].Data.Acceleration = -.25f * emit.Particles[i].Data.Velocity;
                    emit.Particles[i].Data.Acceleration = -.5f * emit.Particles[i].Data.Velocity;
                    emit.Particles[i].Life = life;
                    emit.Particles[i].AngleSpeed /= 2;
                emit.Particles[i].Angle = Tools.RndFloat(0, 100);
                emit.Particles[i].Size *= intensity;
            }
        }

        static void SetRandomPiece(ParticleEmitter emit, int i)
        {
            switch (Tools.Rnd.Next(0, 8))
            {
                case 0:
                    emit.Particles[i].MyQuad.MyTexture = Tools.TextureWad.FindByName(InfoWad.GetStr("FallingBlock_Touched_Texture"));
                    break;

                case 1:
                    emit.Particles[i].MyQuad.MyTexture = Tools.TextureWad.FindByName(InfoWad.GetStr("FallingBlock_Falling_Texture"));
                    break;

                case 2:
                    emit.Particles[i].MyQuad.MyTexture = Tools.TextureWad.FindByName("blue_small");
                    break;

                case 3:
                    emit.Particles[i].MyQuad.MyTexture = Tools.TextureWad.FindByName("fading block");
                    emit.Particles[i].Size *= new Vector2(1f, 1.11f);
                    break;

                case 4:
                    emit.Particles[i].MyQuad.MyTexture = Tools.TextureWad.FindByName("SpikeyGuy");
                    //emit.Particles[i].MyQuad.MyTexture = Tools.TextureWad.FindByName("tree_small");
                    emit.Particles[i].Size *= new Vector2(1.2f, 1.6f);
                    break;

                case 5:
                    emit.Particles[i].MyQuad.MyTexture = Tools.TextureWad.FindByName("blob2_body");
                    break;

                case 6:
                    emit.Particles[i].MyQuad.MyTexture = Tools.TextureWad.FindByName("checkpoint3");
                    emit.Particles[i].Size *= new Vector2(1, 1);
                    break;

                case 7:
                    emit.Particles[i].MyQuad.MyTexture = Tools.TextureWad.FindByName("CoinBlue");
                    emit.Particles[i].Size *= new Vector2(.3f, .7f);
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

            int i;
            for (int k = 0; k < num; k++)
            {
                i = emit.GetNextSlot();

                emit.Particles[i] = CoalesceTemplate;
                Vector2 Dir = Tools.RndDir();

                emit.Particles[i].Data.Position = pos + 1500 * Dir;
                
                emit.Particles[i].Angle = Tools.RndFloat(0, 100);
                //emit.Particles[i].Size *= intensity;

                emit.Particles[i].AttractionPoint = pos;

                Vector2 dif = pos - emit.Particles[i].Data.Position;
                dif.Normalize();
                emit.Particles[i].Data.Velocity = 9 * new Vector2(-dif.Y, dif.X);

                //emit.Particles[i].SizeSpeed = Vector2.Zero;
                emit.Particles[i].Size *= 1.25f;
                //emit.Particles[i].Life += 16;
                //SetRandomPiece(emit, i);

                emit.Particles[i].Life += PadLife;
            }
        }

        public static void PieceExplosion(Level level, Vector2 pos, int frame, float intensity)
        {
            ParticleEmitter emit = level.MainEmitter;

            int num = 2;
            if (Tools.TheGame.DrawCount % 2 == 0) num = 3;
            num *= 2;
            int i;
            for (int k = 0; k < num; k++)
            {
                i = emit.GetNextSlot();               
                //i = emit.GetPrevSlot();
                
                emit.Particles[i] = PieceExplosionTemplate;
                Vector2 Dir = Tools.RndDir();

                emit.Particles[i].Data.Position = pos + 75 * Dir * intensity;
                emit.Particles[i].Data.Velocity = 125 * intensity * (float)Tools.Rnd.NextDouble() * Dir;
                emit.Particles[i].Angle = Tools.RndFloat(0, 100);
                emit.Particles[i].Size *= intensity;


                SetRandomPiece(emit, i);                
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

            int i;
            for (int k = 0; k < num; k++)
            {
                i = emit.GetNextSlot();
                emit.Particles[i] = PieceExplosionTemplate;
                Vector2 Dir = Tools.RndDir();

                emit.Particles[i].MyColor *= color;

                emit.Particles[i].Data.Position = pos + 75 * Dir;
                //emit.Particles[i].Data.Velocity = 25 * intensity * (float)Tools.Rnd.NextDouble() * Dir;
                //emit.Particles[i].Data.Velocity = 18 * intensity * (float)Tools.Rnd.NextDouble() * Dir;
                //emit.Particles[i].Data.Velocity = 50 * intensity * (float)Tools.Rnd.NextDouble() * Dir;
                emit.Particles[i].Data.Velocity = vel * (float)Tools.Rnd.NextDouble() * Dir;
                    emit.Particles[i].Data.Position += Tools.RndFloat(0, 2) * emit.Particles[i].Data.Velocity;
                emit.Particles[i].Angle = Tools.RndFloat(0, 100);
                emit.Particles[i].AngleSpeed *= angle_vel;
                emit.Particles[i].Size *= size;
                //emit.Particles[i].Data.Acceleration = -.15f * emit.Particles[i].Data.Velocity;
                //emit.Particles[i].Data.Acceleration = -.28f * emit.Particles[i].Data.Velocity;                
                //emit.Particles[i].Data.Acceleration = -.128f * emit.Particles[i].Data.Velocity;           
                emit.Particles[i].Data.Acceleration = acc * emit.Particles[i].Data.Velocity;

                //emit.Particles[i].Life = 15;
                //emit.Particles[i].Life = 10;
                //emit.Particles[i].Life = 20;
                emit.Particles[i].Life = life;

                if (texture == null)
                    SetRandomPiece(emit, i);
                else
                    emit.Particles[i].MyQuad.MyTexture = texture;
            }
        }


        /// <summary>
        /// Create an explosion of pieces at the specified location, plus a fart sound.
        /// </summary>
        public static void PiecePopFart(Level level, Vector2 pos)
        {
            Tools.SoundWad.FindByName("Piece Explosion Small").Play(1f);
            for (int i = 0; i < 4; i++)
                PieceExplosion(level, pos, 0, 1);
        }
    }
}
