using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;




namespace CloudberryKingdom
{
    public class ParticleEmitterBin
    {
        Stack<ParticleEmitter> MyStack;

        public ParticleEmitterBin()
        {
            const int capacity = 20;
            MyStack = new Stack<ParticleEmitter>(capacity);
            for (int i = 0; i < capacity; ++i)
                MyStack.Push(new ParticleEmitter(300));
        }

        public ParticleEmitter Get()
        {
            ParticleEmitter item = null;

            lock (MyStack)
            {
                if (MyStack.Count == 0)
                    return new ParticleEmitter(300);

                item = MyStack.Pop();
            }

            return item;
        }

        public void ReturnItem(ParticleEmitter item)
        {
            lock (MyStack)
            {
                MyStack.Push(item);
            }
        }
    }

    public class ParticleEmitter
    {
        public static ParticleEmitterBin Pool = new ParticleEmitterBin();

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
            if (Tools.Render.UsingSpriteBatch) Tools.Render.EndSpriteBatch();

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
