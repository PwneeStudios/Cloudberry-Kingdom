using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Drawing;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom.Particles
{
    public class ParticleEmitter
    {
        public static Bin<ParticleEmitter> Pool = new Bin<ParticleEmitter>(
            () => new ParticleEmitter(300),
            emitter => { },
            20);

        public EzTexture MyTexture;

        public Vector2 Position;
        public int Delay;
        public int Amount;
        int Count;
        public int Index;

        public Particle ParticleTemplate;

        public LinkedList<Particle> Particles;

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
            Clean();
            Pool.ReturnItem(this);
        }

        public int TotalCapacity;
        public ParticleEmitter(int Capacity)
        {
            Init(Capacity);
        }

        void Init(int capacity)
        {
            TotalCapacity = capacity;

            Count = Index = 0;

            Particles = new LinkedList<Particle>();

            MyTexture = Tools.TextureWad.TextureList[0];

            On = true;

            DisplacementRange = 0;
            VelRange = 5f;
            VelBase = 2;
            VelDir = new Vector2(0, 0);

            ParticleTemplate = new Particle();
            ParticleTemplate.Init();
            ParticleTemplate.SetSize(150);
            ParticleTemplate.Life = 200;
        }

        /// <summary>
        /// Clear all particles.
        /// </summary>
        public void Clean()
        {
            foreach (Particle p in Particles)
                p.Recycle();
            Particles.Clear();
        }

        public void Absorb(ParticleEmitter emitter)
        {
            foreach (Particle p in emitter.Particles)
                Particles.AddLast(p);
            emitter.Particles.Clear();
        }

        public void KillParticle(LinkedListNode<Particle> node)
        {
            Particles.Remove(node);
            node.Value.Recycle();
        }

        public void EmitParticle(Particle p)
        {
            Particles.AddLast(p);
        }

        public Particle GetNewParticle(Particle template)
        {
            var p = Particle.Pool.Get();
            p.Copy(template);

            Particles.AddLast(p);

            if (Particles.Count > TotalCapacity)
                KillParticle(Particles.First);

            return p;
        }

        public void Draw()
        {
            if (Tools.UsingSpriteBatch) Tools.EndSpriteBatch();

            foreach (Particle p in Particles)
                p.Draw();
            Tools.QDrawer.Flush();
        }

        public void Unfreeze(int code)
        {
            foreach (Particle p in Particles)
                if (p.Code == code)
                    p.Frozen = false;
        }

        public void RestrictedUpdate(int code)
        {
            foreach (Particle p in Particles)
                if (p.Code == code)
                    p.Phsx(Tools.CurLevel.MainCamera);
        }

        public void Phsx()
        {
            UpdateStep();

            var node = Particles.First;
            while (node != null)
            {
                var p = node.Value;
                var next = node.Next;

                if (p.Life > 0)// && p.MyColor.W > 0)
                    p.Phsx(Tools.CurLevel.MainCamera);
                else
                    KillParticle(node);

                node = next;
            }
        }
    }
}
