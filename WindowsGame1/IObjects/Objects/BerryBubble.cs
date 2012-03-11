using System.IO;
using Microsoft.Xna.Framework;

using Drawing;
using System;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public partial class BerryBubble : ObjectBase, IObject
    {
        public void TextDraw() { }
        public virtual void Release()
        {
            Core.Release();
        }

        public CircleBox Box;

        public DrawPile MyPile;
        public QuadClass Berry, Bubble;

        public ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        public float RotateSpeed = 0;

        public void Rotate(float degrees)
        {
            foreach (var quad in MyPile.MyQuadList)
                quad.Degrees += degrees;
        }

        public void MakeNew()
        {
            Core.ResetOnlyOnReset = true;

            if (!Core.BoxesOnly)
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

            if (Core.BoxesOnly) return;

            Bubble.Quad.MyTexture = BubbleTexture;
            Bubble.Alpha = 1;
            Berry.Quad.MyTexture = Popped ? PoppedTexture : UnpoppedTexture;

            Berry.ScaleXToMatchRatio(Radius);
            Bubble.ScaleXToMatchRatio(Popped ? Radius : PoppedRadius);

            _TexturesInitialized = true;
        }

        public void Die()
        {
            if (Core.MyLevel.PlayMode != 0) return;

            Core.MyLevel.AddPop(Pos);

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
            CoreData = new ObjectData();
            
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

            Core.BoxesOnly = BoxesOnly;
        }

        public float Radius = 180, PoppedRadius = 189;
        protected float Gravity = 2.9f;
        public void Initialize(Vector2 pos)
        {
            Core.Init();
            Core.MyType = ObjectType.BerryBubble;
            Core.DrawLayer = 9;

            Core.StartData.Position = Core.Data.Position = pos;

            Box.Initialize(Core.Data.Position, Radius);
            if (!Core.BoxesOnly)
            {
                SetTexture();
            }
        }

        public bool Popped = false;
        public virtual void PhsxStep()
        {
            if (RotateSpeed != 0)
                Rotate(RotateSpeed);

            if (Popped)
            {
                //Core.RemoveOnReset = true;

                Core.Data.Velocity.Y -= .7f*Gravity;
                Core.Data.Integrate();

                Bubble.Alpha -= .0375f;
                Bubble.Size += new Vector2(3.5f);

                if (Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - 500)
                {
                    //this.CollectSelf();
                    return;
                }
            }
            else
            {
                Core.Data.Position = Core.StartData.Position;
                Core.Data.Position += new Vector2(0, 9.65f * (float)Math.Sin(.075f * (Core.MyLevel.CurPhsxStep) + Core.AddedTimeStamp));
            }
        }

        public void PhsxStep2()
        {
        }

        public virtual void Draw()
        {
            if (Core.MyLevel == null) return;

            int DrawRange = 360;
            if (Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + DrawRange || Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + DrawRange)
                return;
            if (Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - DrawRange || Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - DrawRange)
                return;

            if (Tools.DrawGraphics && !Core.BoxesOnly)
            {
                MyPile.Pos = Core.Data.Position;
                MyPile.Draw();
                //MyQuad.Pos = Core.Data.Position;
                //MyQuad.Draw();
            }

            if (Tools.DrawBoxes)
                Box.Draw(new Color(50, 50, 255, 220));
        }

        public void Move(Vector2 shift)
        {
            Core.Data.Position += shift;
            Core.StartData.Position += shift;

            Box.Move(shift);
        }

        public virtual void Reset(bool BoxesOnly)
        {
            Core.Active = true;
            Popped = false;

            Core.Data.Position = Core.StartData.Position;
            Core.Data.Velocity = Vector2.Zero;

            SetTexture();
        }

        public bool AllowJumpOffOf = true;
        public virtual void Interact(Bob bob)
        {
            if (Popped) return;

            bool hold = Box.BoxOverlap(bob.Box2);
            if (hold)
            {
                if (Core.MyLevel.PlayMode == 0)
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
                    bob.MyPhsx.LandOnSomething(true);
                    bob.MyPhsx.MaxJumpAccelMultiple = 1 + .8f * bob.MyPhsx.BlobMod;
                    bob.Core.Data.Velocity.Y = 9.5f * bob.MyPhsx.BlobMod;
                    bob.PopModifier = HoldPopModifier;
                }
            }
        }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);

            BerryBubble BerryBubbleA = A as BerryBubble;

            Initialize(BerryBubbleA.Core.Data.Position);
        }

        public void Write(BinaryWriter writer)
        {
            Core.Write(writer);
        }
        public void Read(BinaryReader reader) { Core.Read(reader); }
//StubStubStubStart
public void OnUsed() { }
public void OnMarkedForDeletion() { }
public void OnAttachedToBlock() { }
public bool PermissionToUse() { return true; }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public void Smash(Bob bob) { }
public bool PreDecision(Bob bob) { return false; }
//StubStubStubEnd7
    }
}
