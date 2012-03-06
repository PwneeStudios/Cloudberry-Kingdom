using System.IO;

using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Sign : IObject
    {
        public void Release()
        {
            Core.Release();
        }

        public bool SkipPhsx;

        public QuadClass MyQuad;

        public ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        EzTexture OnTexture, OffTexture;

        public void MakeNew()
        {
            Core.Init();
            Core.DrawLayer = 1;
            Core.ResetOnlyOnReset = true;

            Core.EditHoldable = true;

            OnTexture = Tools.TextureWad.FindByName("Exit_On");
            OffTexture = Tools.TextureWad.FindByName("Exit_Off");

            MyQuad = new QuadClass("Exit_On", 275);
        }

        public Sign(bool BoxesOnly)
        {
            CoreData = new ObjectData();

            Core.BoxesOnly = BoxesOnly;

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        int Count = 1;
        bool OnState = true;
        void SetState(bool NewOnState)
        {
            OnState = NewOnState;

            if (NewOnState) MyQuad.Quad.MyTexture = OnTexture;
            else MyQuad.Quad.MyTexture = OffTexture;

            Count = 1;
        }

        public static int OffLength = 68, OnLength = 58;
        public void PhsxStep()
        {
            Count++;

            if (!OnState && Count == OffLength) { SetState(true); Count = 0; }
            if (OnState && Count == OnLength) { SetState(false); Count = 0; }
        }

        public void PhsxStep2() { }

        bool OnScreen()
        {
            if (Core.BoxesOnly) return false;

            if (Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + 150 + MyQuad.Base.e1.X || Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + 150 + MyQuad.Base.e2.Y)
                return false;
            if (Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - 150 - MyQuad.Base.e1.X || Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - 500 - MyQuad.Base.e2.Y)
                return false;

            return true;
        }

        public void TextDraw() { }
        
        public void Draw()
        {
            if (!OnScreen()) return;

            if (Tools.DrawGraphics)
            {
                float x = MyQuad.SizeX;
                if (Core.MyLevel != null && Core.MyLevel.ModZoom.X < 0)
                {
                    if (MyQuad.Base.e1.X > 0)
                        MyQuad.SizeX = -x;
                }
                else
                {
                    if (MyQuad.Base.e1.X < 0)
                        MyQuad.SizeX = -x;
                }

                MyQuad.Draw();
            }
        }


        public Vector2 GetBottom()
        {
            MyQuad.Update();
            return new Vector2(Core.Data.Position.X, MyQuad.BL.Y + 11.5f);
        }

        /// <summary>
        /// Moves the foot of the Sign to the specified position.
        /// </summary>
        /// <param name="pos"></param>
        public void PlaceAt(Vector2 pos)
        {
            Move(pos - GetBottom() - new Vector2(0, 16));
        }

        public void Update()
        {
            MyQuad.Base.Origin = Core.Data.Position;
        }

        public void Move(Vector2 shift)
        {
            Core.Data.Position += shift;
            Update();
        }

        public void Reset(bool BoxesOnly)
        {
            Core.Active = true;
        }

        public void Interact(Bob bob) { }
        public void Clone(IObject A)
        {
            Core.Clone(A.Core);

            Sign SignA = A as Sign;
        }

        public void Write(BinaryWriter writer)
        {
            Core.Write(writer);

            MyQuad.Write(writer);
        }
        public void Read(BinaryReader reader)
        {
            Core.Read(reader);

            MyQuad.Read(reader);
        }
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