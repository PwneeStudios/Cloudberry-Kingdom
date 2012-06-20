using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Particles;

namespace CloudberryKingdom
{
    public partial class Fireball : ObjectBase
    {
        static Particle ExplodeTemplate, EmitterTemplate;
        static public EzSound ExplodeSound;
        static float t;

        static Quad ShadeQuad;
        static public EzTexture FireballTexture, FlameTexture, EmitterTexture, BaseFireballTexture;
        static RenderTarget2D FireballRenderTarget, FlameRenderTarget, EmitterRenderTarget;
        static int DrawWidth, DrawHeight;
        public static ParticleEmitter Fireball_Emitter, Flame_Emitter, Emitter_Emitter;

        public static void PreInit()
        {
            FireballTexture = new EzTexture();
            FlameTexture = new EzTexture();
            EmitterTexture = new EzTexture();
        }

        public static void InitRenderTargets(GraphicsDevice device, PresentationParameters pp, int Width, int Height)
        {
            FireballTexture.Name = "FireballTexture";

            DrawWidth = Width;
            DrawHeight = Height;
            FireballRenderTarget = new RenderTarget2D(device,
                DrawWidth, DrawHeight, false,
                                   pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount,
                                   RenderTargetUsage.DiscardContents);


            EmitterTexture.Name = "EmitterTexture";

            FlameRenderTarget = new RenderTarget2D(device,
                300, 300, false,
                                   pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount,
                                   RenderTargetUsage.DiscardContents);

            EmitterRenderTarget = new RenderTarget2D(device,
                300, 300, false,
                                   pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount,
                                   RenderTargetUsage.DiscardContents);


            ShadeQuad = new Quad();
            ShadeQuad.MyEffect = Tools.EffectWad.FindByName("Fireball");
            ShadeQuad.MyTexture = Tools.TextureWad.FindByName("White");
            ShadeQuad.Scale(new Vector2(210 * 1.15f, 55 * 1.15f));

            ShadeQuad.Update();

            ExplodeSound = Tools.SoundWad.FindByName("Fireball_Explode");

            // Fireball particle emitter
            Fireball_Emitter = new ParticleEmitter(20);

            Fireball_Emitter.Position = new Vector2(55, 0);
            Fireball_Emitter.Amount = 2;
            Fireball_Emitter.Delay = 1;

            Fireball_Emitter.DisplacementRange = 10;
            Fireball_Emitter.VelRange = 1;
            Fireball_Emitter.VelBase = .1f;
            Fireball_Emitter.VelDir = new Vector2(-5, 0);

            Fireball_Emitter.ParticleTemplate.MyQuad.UseGlobalIllumination = false;
            Fireball_Emitter.ParticleTemplate.MyQuad.MyTexture = Tools.TextureWad.FindByName("Fireball");
            Fireball_Emitter.ParticleTemplate.MyQuad.MyEffect = Tools.BasicEffect;
            Fireball_Emitter.ParticleTemplate.SetSize(43);
            Fireball_Emitter.ParticleTemplate.Life = 140;
            Fireball_Emitter.ParticleTemplate.MyColor = new Vector4(1f, .7f, .7f, .75f);
            Fireball_Emitter.ParticleTemplate.ColorVel = new Vector4(0, .01f, 0, -.02f);
            Fireball_Emitter.ParticleTemplate.MyQuad.BlendAddRatio = 0f;

            // Flame particle emitter
            Flame_Emitter = new ParticleEmitter(40);
            Flame_Emitter.Position = new Vector2(0, 0);

            // Emitter particle emitter
            Emitter_Emitter = new ParticleEmitter(40);
            Emitter_Emitter.Position = new Vector2(0, 0);


            BaseFireballTexture = Tools.TextureWad.FindByName("Fireball");

            // Initialize explosion particle
            ExplodeTemplate = new Particle();
            ExplodeTemplate.MyQuad.Init();
            ExplodeTemplate.MyQuad.UseGlobalIllumination = false;
            ExplodeTemplate.MyQuad.MyEffect = Tools.BasicEffect;
            ExplodeTemplate.MyQuad.MyTexture = FlameTexture;
            ExplodeTemplate.SetSize(95);
            ExplodeTemplate.SizeSpeed = new Vector2(5, 5);// new Vector2(5, 5);
            ExplodeTemplate.AngleSpeed = .035f;
            ExplodeTemplate.Life = 35;
            //ExplodeTemplate.MyColor = new Vector4(.96f, .94f, .94f, 1f);
            //ExplodeTemplate.ColorVel = new Vector4(0.00f, -0.007f, -0.007f, -.03f);
            ExplodeTemplate.MyColor = new Vector4(1f, 1f, 1f, 1f);
            ExplodeTemplate.ColorVel = new Vector4(0.00f, -0.003f, -0.003f, -.03f);
            ExplodeTemplate.MyQuad.BlendAddRatio = 0f;

            // Initialize emitter particle
            EmitterTemplate = new Particle();
            EmitterTemplate.MyQuad.Init();
            EmitterTemplate.MyQuad.UseGlobalIllumination = false;
            EmitterTemplate.MyQuad.MyEffect = Tools.BasicEffect;//Circle");
            EmitterTemplate.MyQuad.MyTexture = Tools.TextureWad.FindByName("Fire");
            EmitterTemplate.SetSize(55);
            EmitterTemplate.SizeSpeed = new Vector2(2, 2);
            EmitterTemplate.AngleSpeed = .1f;
            EmitterTemplate.Life = 40;
            EmitterTemplate.MyColor = new Vector4(.6f, .62f, .62f, .75f);
            EmitterTemplate.ColorVel = new Vector4(0.006f, -0.0002f, -0.0002f, -.03f);
            EmitterTemplate.MyQuad.BlendAddRatio = 0f;
        }

        public static void TexturePhsx()
        {
            Rand Rnd = Tools.GlobalRnd;

            // Fireball superparticle physics
            Fireball_Emitter.Phsx();

            // Flame superparticle physics
            for (int k = 0; k < 1; k++)
            {
                var p = Flame_Emitter.GetNewParticle(EmitterTemplate);
                Vector2 Dir = Rnd.RndDir();
                p.Data.Position = 20 * Dir; //+ Tools.CurLevel.MainCamera.Data.Position;
                //p.Data.Velocity = 4 * (float)MyLevel.Rnd.Rnd.NextDouble() * Dir;
                p.Data.Velocity = 8.5f * (float)Rnd.Rnd.NextDouble() * Dir;
                p.Data.Acceleration -= .07f * p.Data.Velocity;
                p.AngleSpeed *= 2 * (float)(Rnd.Rnd.NextDouble() - .5f);
                p.ColorVel.W *= (float)(.3f * Rnd.Rnd.NextDouble() + .7f);
            }
            Flame_Emitter.Phsx();

            // Emitter superparticle physics
            ParticleEffects.Flame(Emitter_Emitter, Vector2.Zero, Tools.TheGame.DrawCount, 1, 20, false);
            Emitter_Emitter.Phsx();
        }

        public static void DrawFireballTexture(GraphicsDevice device, EzEffectWad EffectWad)
        {
            t += 1f / 60;

            device.SetRenderTarget(FireballRenderTarget);
            device.Clear(Color.Transparent);
            float scalex = 350;
            float scaley = 150;

            ShadeQuad.MyEffect.effect.CurrentTechnique = ShadeQuad.MyEffect.Simplest;
            Tools.EffectWad.SetCameraPosition(new Vector4(0, 0, 1f / scalex, 1f / scaley));
            ShadeQuad.MyEffect.xCameraAspect.SetValue(1);


            float TimeScale = 4f;

            ShadeQuad.SetColor(Color.White);
            ShadeQuad.MyEffect.t.SetValue(t / TimeScale);
            if (Tools.DrawCount > 5)
            ShadeQuad.MyTexture = EmitterTexture;
            else
            ShadeQuad.MyTexture = BaseFireballTexture;
            Tools.QDrawer.DrawQuad(ShadeQuad);
            Tools.QDrawer.Flush();

            ShadeQuad.MyEffect.t.SetValue(t / TimeScale + 150);
            Tools.QDrawer.DrawQuad(ShadeQuad);
            Tools.QDrawer.Flush();

            ShadeQuad.MyEffect.t.SetValue(t / TimeScale + 300);
            Tools.QDrawer.DrawQuad(ShadeQuad);
            Tools.QDrawer.Flush();

            device.SetRenderTarget(Tools.DestinationRenderTarget);
            Tools.ResetViewport();

            FireballTexture.Tex = FireballRenderTarget;

            // Save to file
            //FireballTexture.Tex.Save(string.Format("Fireball_{0}.png", Tools.DrawCount), ImageFileFormat.Png);
        }

        public static void DrawEmitterTexture(GraphicsDevice device, EzEffectWad EffectWad)
        {
            t += 1f / 60;

            device.SetRenderTarget(FlameRenderTarget);
            device.Clear(Color.Transparent);
            float scalex = 175;
            float scaley = 175;

            EzEffect fx = Tools.EffectWad.EffectList[0];

            fx.effect.CurrentTechnique = fx.Simplest;
            Tools.EffectWad.SetCameraPosition(new Vector4(0, 0, 1f / scalex, 1f / scaley));
            fx.xCameraAspect.SetValue(1);

            Flame_Emitter.Draw();

            device.SetRenderTarget(Tools.DestinationRenderTarget);
            Tools.ResetViewport();
            FlameTexture.Tex = FlameRenderTarget;


            // Draw emitter
            device.SetRenderTarget(EmitterRenderTarget);
            device.Clear(Color.Transparent);

            // All these renderstates need to be ported to XNA 4.0
            /*
            device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            device.RenderState.SourceBlend = Blend.SourceAlpha;
            device.RenderState.SeparateAlphaBlendEnabled = true;
            */

            Emitter_Emitter.Draw();

            device.SetRenderTarget(Tools.DestinationRenderTarget);
            Tools.ResetViewport();
            EmitterTexture.Tex = EmitterRenderTarget;
        }

        static public void Explosion(Vector2 pos, Level level) { Explosion(pos, level, Vector2.Zero, 1, 1); }
        static public void Explosion(Vector2 pos, Level level, Vector2 vel, float Scale, float ScaleQuad)
        {
            Rand Rnd = Tools.GlobalRnd;

            int i;
            for (int k = 0; k < 20; k++)
            {
                var p = level.MainEmitter.GetNewParticle(ExplodeTemplate);
                Vector2 Dir = Rnd.RndDir();
                p.Data.Position = pos + 23 * Dir * Scale;
                //p.Data.Velocity = 4 * (float)Rnd.Rnd.NextDouble() * Dir;
                p.Data.Velocity = Scale * 8.5f * (float)Rnd.Rnd.NextDouble() * Dir;
                p.Data.Acceleration = -.045f * p.Data.Velocity;
                p.Data.Velocity += vel;

                p.Size *= ScaleQuad;

                p.AngleSpeed *= 2 * (float)(Rnd.Rnd.NextDouble() - .5f);
                //p.ColorVel.W *= (float)(.3f * Rnd.Rnd.NextDouble() + .7f);
            }
        }
    }
}