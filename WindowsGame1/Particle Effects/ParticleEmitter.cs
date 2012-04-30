using System;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom.Particles
{
    public class ParticleEmitter
    {
        public static Bin<ParticleEmitter> Pool = new Bin<ParticleEmitter>(
            () => new ParticleEmitter(300),
            emiiter => { },
            20);

        public EzTexture MyTexture;

        public Vector2 Position;
        public int Delay;
        public int Amount;
        int Count;
        public int Index;

        public Particle ParticleTemplate;

        public int Capacity;
        public Particle[] Particles;

        public int DisplacementRange;
        public float VelRange;
        public float VelBase;
        public Vector2 VelDir;
        
        public bool On;

        public Level MyLevel;

        public int LastActiveStep; // Tracks the last RealTime step the emitter was active
        public bool IsActive()
        {
            return Tools.TheGame.DrawCount == LastActiveStep;
        }
        void UpdateStep()
        {
            LastActiveStep = Tools.TheGame.DrawCount;
        }

        public void Release()
        {
            MyLevel = null;

            Pool.ReturnItem(this);
            /*
            if (Particles != null)
                foreach (Particle particle in Particles)
                    particle.Release();
            
            Particles = null;
            
            ParticleTemplate.Release();*/
        }

        public ParticleEmitter(int Capacity)
        {
            Init(Capacity);
        }

        void Init(int capacity)
        {
            Count = Index = 0;

            Capacity = capacity;
            Particles = new Particle[Capacity];

            MyTexture = Tools.TextureWad.FindByName("White");

            On = true;

            DisplacementRange = 0;
            VelRange = 5f;
            VelBase = 2;
            VelDir = new Vector2(0, 0);
        
            ParticleTemplate.SetSize(150);
            ParticleTemplate.Life = 200;
        }

        /// <summary>
        /// Clear all particles.
        /// </summary>
        public void Clean()
        {
            for (int i = 0; i < Capacity; i++)
                Particles[i].Life = 0;
        }

        public void Absorb(ParticleEmitter emitter)
        {
            for (int i = 0; i < emitter.Capacity; i++)
            {
                if (emitter.Particles[i].Life > 0)
                {
                    FindNextSlot();
                    Particles[Index] = emitter.Particles[i];
                    //Particles[Index].MyCam = MyLevel.MainCamera;
                }
            }
        }

        public int EmitParticle(Particle p)
        {
            int i = GetNextSlot();
            Particles[i] = p;

            return i;
        }

        public void Draw()
        {
            //Tools.Device.RenderState.DestinationBlend = Blend.One;
            for (int i = Index + 1; i < Capacity; i++) if (Particles[i].Life > 0) Particles[i].Draw();
            for (int i = 0; i <= Index; i++) if (Particles[i].Life > 0) Particles[i].Draw();

            //for (int i = Index; i > 0; i--) if (Particles[i].Life > 0) Particles[i].Draw();
            //for (int i = Capacity - 1; i > Index; i--) if (Particles[i].Life > 0) Particles[i].Draw();
            
            Tools.QDrawer.Flush();
            //Tools.Device.RenderState.DestinationBlend = Blend.InverseSourceAlpha; 
        }

        public void Unfreeze(int code)
        {
            for (int i = 0; i < Capacity; i++)
                if (Particles[i].Code == code)
                    Particles[i].Frozen = false;
        }

        public void RestrictedUpdate(int code)
        {
            for (int i = 0; i < Capacity; i++)
                if (Particles[i].Code == code)
                    Particles[i].Phsx(Tools.CurLevel.MainCamera);
        }

        public void FindNextSlot()
        {
            int StartIndex = Index;
            while (Particles[Index].Life > 0)
            {
                Index++;
                if (Index >= Capacity) Index = 0;
                if (Index == StartIndex) break;
            }
        }

        public void FindPrevSlot()
        {
            int StartIndex = Index;
            while (Particles[Index].Life > 0)
            {
                Index--;
                if (Index < 0) Index = Capacity - 1;
                if (Index == StartIndex) break;
            }
        }

        public int GetNextSlot()
        {
            FindNextSlot();
            return Index;
        }

        public int GetPrevSlot()
        {
            FindPrevSlot();
            return Index;
        }

        public void Phsx()
        {
            UpdateStep();

            for (int i = 0; i < Capacity; i++)
                if (Particles[i].Life > 0)
                    Particles[i].Phsx(Tools.CurLevel.MainCamera);
            return;
            if (!On)
                return;

            if (MyLevel != null)
            {
                if (Position.X > MyLevel.MainCamera.TR.X + 200 ||
                    Position.X < MyLevel.MainCamera.BL.X - 200 ||
                    Position.Y > MyLevel.MainCamera.TR.Y + 200 ||
                    Position.Y < MyLevel.MainCamera.BL.Y - 200)
                    return;
            }

            Count++;
            if (Count > Delay)
            {
                Count = 0;
                for (int i = 0; i < Amount; i++)
                {
                    // Find availabe particle slot
                    FindNextSlot();

                    Particles[Index] = ParticleTemplate;
//                    Particle particle = new Particle();
  //                  particle.Data.Position = Position;
                    Particles[Index].Data.Position = Position;

                    double a, r;
                    a = Tools.GlobalRnd.Rnd.Next(360) / 180f * Math.PI;
                    r = VelRange * Tools.GlobalRnd.Rnd.NextDouble() + VelBase;
                    Particles[Index].Data.Velocity = new Vector2((float)(r * Math.Cos(a)), (float)(r * Math.Sin(a)));
                    Particles[Index].Data.Velocity += VelDir;

                    //particle.Data.Position += new Vector2((float)(Math.Cos(a)), (float)(Math.Sin(a))) * Tools.GlobalRnd.Rnd.Next(DisplacementRange);
                    Particles[Index].Data.Position += new Vector2((float)(Math.Cos(a)), (float)(Math.Sin(a))) * Tools.GlobalRnd.Rnd.Next(DisplacementRange);
                }
            }
        }
    }
}
