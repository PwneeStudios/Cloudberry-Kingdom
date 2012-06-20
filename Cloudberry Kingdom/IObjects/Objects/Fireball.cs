using System.IO;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public partial class Fireball : ObjectBase
    {
        public class FireballTileInfo
        {
            public TextureOrAnim Sprite = null;
            public Vector2 Size = new Vector2(72), Shift = Vector2.Zero;
        }

        public override void Release()
        {
            base.Release();

            Parent = null;
        }

        public CircleBox Box;

        public QuadClass MyQuad;

        public int Life, StartLife;
        public ObjectBase Parent;
        public int CreationTimeStamp;

        public int FireballType;

        Vector2 Size;

        public override void MakeNew()
        {
            if (!Core.BoxesOnly)
            {
                //MyQuad.Init();
                //MyQuad.MyEffect = Tools.BasicEffect;
                //MyQuad.MyTexture = Tools.TextureWad.FindByName("White");
                //MyQuad.UseGlobalIllumination = false;
            }
        }


        public void Die()
        {
            if (Core.MyLevel.PlayMode != 0) return;

            ExplodeSound.Play(1);
            Explosion(Core.Data.Position, Core.MyLevel, .33f * Core.Data.Velocity, 1, 1);

            CollectSelf();
        }

        public Fireball(bool BoxesOnly)
        {
            //Box = new AABox();
            Box = new CircleBox();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public void Initialize(int type, PhsxData data, Level level)
        {
            Core.Init();
            Core.MyType = ObjectType.Fireball;
            Core.DrawLayer = 7;
            Core.RemoveOnReset = true;

            Core.Data = data;
            StartLife = 50;
            Life = StartLife;

            FireballType = type;

            if (level.Info.Fireballs.Sprite != null)
            {
                if (MyQuad == null)
                    MyQuad = new QuadClass();
                MyQuad.Quad.SetTextureOrAnim(level.Info.Fireballs.Sprite);
                MyQuad.ScaleYToMatchRatio(100);
            }
            else
            switch (FireballType)
            {
                case 0:
                    if (!Core.BoxesOnly)
                        MyQuad.Quad.MyTexture = FireballTexture;

                    //Size = 1.75f * new Vector2(15, 15);
                    Size = new Vector2(40);

                    MyQuad.Size = new Vector2(160);
                    break;
                case 1:
                    Size = new Vector2(120, 120);
                    /*
                    emitter.Amount = 3;
                    emitter.Delay = 1;
                    emitter.Particles = Particles;
                    emitter.DisplacementRange = 100;
                    emitter.ParticleSize = 150;
                    emitter.VelRange = 5;
                    emitter.Life = 200;
                    emitter.AlphaVel = -.01f;
                     */
                    break;
            }
                                    
            Box.Initialize(data.Position, Size.X);
        }

        public override void PhsxStep()
        {
            Life -= 1;
            if (Life == 0)
            {
                Vector2 Range = new Vector2(200, 200);
                if (Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, Range))
                    Life = 1;
                else
                    if (!Core.MarkedForDeletion)
                    {
                        Core.Recycle.CollectObject(this);
                        return;
                    }
            }

            Core.Data.Position += Core.Data.Velocity;
            Core.Data.Velocity += Core.Data.Acceleration;

            //Box.Target.Set(Core.Data.Position, Size);
            Box.Center = Core.Data.Position;
        }

        float MyAlpha;
        void SetAlpha(float Alpha)
        {
            if (Alpha != MyAlpha)
            {
                MyAlpha = Alpha;
                MyQuad.Quad.SetColor(new Color(1, 1, 1, MyAlpha));
            }
        }

        public override void Draw()
        {
            if (Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + 150 || Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + 150)
                return;
            if (Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - 150 || Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - 150)
                return;

            if (Tools.DrawGraphics && !Core.BoxesOnly)
            {
                // Fade in
                const float FadeInLength = 10;
                int Dif = StartLife - Life;
                if (Dif <= FadeInLength)
                    SetAlpha(Dif / FadeInLength);
                else
                    SetAlpha(1f);

                // Point forward
                MyQuad.PointxAxisTo(-Core.Data.Velocity);
                MyQuad.Base.e1 *= -1;

                // Shift forward
                Vector2 dir = Core.Data.Velocity;
                dir.Normalize();

                MyQuad.Pos = Core.Data.Position - 30 * dir;

                // Draw the fireball
                MyQuad.Draw();
            }

            if (Tools.DrawBoxes)
                //Box.Draw(Tools.QDrawer, Color.Blue, 10);
                Box.Draw(new Color(50, 50, 255, 220));
        }

        public override void Move(Vector2 shift)
        {
            Core.Data.Position += shift;

            Box.Move(shift);
        }

        public override void Interact(Bob bob)
        {
            bool hold = Box.BoxOverlap(bob.Box2);
            if (hold)
            {
                Life = 0;

                if (Core.MyLevel.PlayMode == 1)
                {
                    bool col = Box.BoxOverlap_Tiered(Core, bob, FireballEmitter_AutoGen.Instance);

                    if (col)
                    {
                        Core.Recycle.CollectObject(Parent);
                        Core.Recycle.CollectObject(this);
                    }
                }

                if (Core.MyLevel.PlayMode == 0)
                {
                    Die();
                    bob.Die(Bob.BobDeathType.Fireball, this);
                }
            }
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            Fireball FireballA = A as Fireball;

            Life = FireballA.Life;
            StartLife = FireballA.StartLife;
            Parent = FireballA.Parent;
            CreationTimeStamp = FireballA.CreationTimeStamp;

            FireballType = FireballA.FireballType;

            Size = FireballA.Size;

            Initialize(FireballA.FireballType, FireballA.Core.Data, FireballA.MyLevel);

            StartLife = FireballA.StartLife;
        }
    }
}
