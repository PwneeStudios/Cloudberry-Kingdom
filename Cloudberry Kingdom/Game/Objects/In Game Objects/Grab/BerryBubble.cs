using System.IO;
using Microsoft.Xna.Framework;

using CoreEngine;
using System;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public partial class BerryBubble : ObjectBase
    {
        public CircleBox Box;

        public DrawPile MyPile;
        public QuadClass Berry, Bubble;

        public float RotateSpeed = 0;

        public void Rotate(float degrees)
        {
            foreach (var quad in MyPile.MyQuadList)
                quad.Degrees += degrees;
        }

        public override void MakeNew()
        {
            CoreData.ResetOnlyOnReset = true;

            if (!CoreData.BoxesOnly)
            {
            }

            Popped = false;
            SetTexture();
        }

        static bool _TexturesInitialized = false;
        static EzTexture PoppedTexture, UnpoppedTexture, BubbleTexture;
        protected virtual void SetTexture()
        {
            if (!_TexturesInitialized)
            {
                BubbleTexture = Tools.TextureWad.FindByName("bubble");
                //PoppedTexture = Tools.TextureWad.FindByName("cb_enthusiastic");
                PoppedTexture = Tools.TextureWad.FindByName("cb_falling");
                UnpoppedTexture = Tools.TextureWad.FindByName("berrybubble");
            }

            if (CoreData.BoxesOnly) return;

            Bubble.Quad.MyTexture = BubbleTexture;
            Bubble.Alpha = 1;
            Berry.Quad.MyTexture = Popped ? PoppedTexture : UnpoppedTexture;

            Berry.ScaleXToMatchRatio(Radius);
            Bubble.ScaleXToMatchRatio(Popped ? Radius : PoppedRadius);

            _TexturesInitialized = true;
        }

        public void Die()
        {
            if (CoreData.MyLevel.PlayMode != 0) return;

            ParticleEffects.AddPop(CoreData.MyLevel, CoreData.Data.Position);

            Popped = true;
            SetTexture();
        }

        public BerryBubble(Vector2 pos)
        {
            Init(false);
            Initialize(pos);
        }
        public BerryBubble(bool BoxesOnly)
        {
            Init(BoxesOnly);
        }
        protected void Init(bool BoxesOnly)
        {
            Box = new CircleBox();
            if (!BoxesOnly)
            {
                MyPile = new DrawPile();
                
                Berry = new QuadClass(null, true, true);
                MyPile.Add(Berry);
                Bubble = new QuadClass(null, true, true);
                MyPile.Add(Bubble);
            }

            MakeNew();

            CoreData.BoxesOnly = BoxesOnly;
        }

        public float Radius = 180, PoppedRadius = 189;
        protected float Gravity = 2.9f;
        public void Initialize(Vector2 pos)
        {
            CoreData.Init();
            CoreData.MyType = ObjectType.BerryBubble;
            CoreData.DrawLayer = 9;

            CoreData.StartData.Position = CoreData.Data.Position = pos;

            Box.Initialize(CoreData.Data.Position, Radius);
            if (!CoreData.BoxesOnly)
            {
                SetTexture();
            }
        }

        public bool Popped = false;
        public override void PhsxStep()
        {
            if (RotateSpeed != 0)
                Rotate(RotateSpeed);

            if (Popped)
            {
                //Core.RemoveOnReset = true;

                CoreData.Data.Velocity.Y -= .7f*Gravity;
                CoreData.Data.Integrate();

                Bubble.Alpha -= .0375f;
                Bubble.Size += new Vector2(3.5f);

                if (CoreData.Data.Position.Y < CoreData.MyLevel.MainCamera.BL.Y - 500)
                {
                    //CollectSelf();
                    return;
                }
            }
            else
            {
                CoreData.Data.Position = CoreData.StartData.Position;
                CoreData.Data.Position += new Vector2(0, 9.65f * (float)Math.Sin(.075f * (CoreData.MyLevel.CurPhsxStep) + CoreData.AddedTimeStamp));
            }
        }

        public override void Draw()
        {
            if (CoreData.MyLevel == null) return;

            int DrawRange = 360;
            if (CoreData.Data.Position.X > CoreData.MyLevel.MainCamera.TR.X + DrawRange || CoreData.Data.Position.Y > CoreData.MyLevel.MainCamera.TR.Y + DrawRange)
                return;
            if (CoreData.Data.Position.X < CoreData.MyLevel.MainCamera.BL.X - DrawRange || CoreData.Data.Position.Y < CoreData.MyLevel.MainCamera.BL.Y - DrawRange)
                return;

            if (Tools.DrawGraphics && !CoreData.BoxesOnly)
            {
                MyPile.Pos = CoreData.Data.Position;
                MyPile.Draw();
                //MyQuad.Pos = Core.Data.Position;
                //MyQuad.Draw();
            }

            if (Tools.DrawBoxes)
                Box.Draw(new Color(50, 50, 255, 220));
        }

        public override void Move(Vector2 shift)
        {
            CoreData.Data.Position += shift;
            CoreData.StartData.Position += shift;

            Box.Move(shift);
        }

        public override void Reset(bool BoxesOnly)
        {
            CoreData.Active = true;
            Popped = false;

            CoreData.Data.Position = CoreData.StartData.Position;
            CoreData.Data.Velocity = Vector2.Zero;

            SetTexture();
        }

        public bool AllowJumpOffOf = true;
        public override void Interact(Bob bob)
        {
            if (Popped) return;

            bool hold = Box.BoxOverlap(bob.Box2);
            if (hold)
            {
                if (CoreData.MyLevel.PlayMode == 0)
                {
                    bob.MyStats.Berries++;
                    Die();

                    bob.PopModifier++;
                    Tools.Pop(bob.PopModifier);
                }

                if (bob.Box.BL.Y > Box.Center.Y - 60)
                {
                    // The player landed on something
                    int HoldPopModifier = bob.PopModifier;
                    bob.MyPhsx.ObjectLandedOn = this;
                    bob.MyPhsx.LandOnSomething(true, this);
                    bob.MyPhsx.MaxJumpAccelMultiple = 1 + .8f * bob.MyPhsx.BlobMod;
                    bob.CoreData.Data.Velocity.Y = 9.5f * bob.MyPhsx.BlobMod;
                    bob.PopModifier = HoldPopModifier;
                }
            }
        }

        public override void Clone(ObjectBase A)
        {
            CoreData.Clone(A.CoreData);

            BerryBubble BerryBubbleA = A as BerryBubble;

            Initialize(BerryBubbleA.CoreData.Data.Position);
        }
    }
}
