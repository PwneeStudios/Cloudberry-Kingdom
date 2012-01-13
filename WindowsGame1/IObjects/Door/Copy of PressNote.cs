using System;
using System.IO;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class PressNote : IObject
    {
        QuadClass MyQuad;
        EzText MyText;

        public ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        public void Release()
        {
            Core.Release();
        }

        //public const int ButtonScale = 80;
        //public const float TextScale = .55f;
        //public const Vector2 PosOffset = new Vector2(20, 513);

        public const int ButtonScale = 76;
        public const float TextScale = .4f;
        public static Vector2 PosOffset = new Vector2(20, 400);

        float Alpha = 0;
        public void MakeNew()
        {
            Core.Init();
            Core.MyType = ObjectType.Undefined;
            Core.DrawLayer = Level.LastInLevelDrawLayer - 1;

            Core.RemoveOnReset = true;

            MyQuad.Quad.Init();
            MyQuad.EffectName = "Basic";
            MyQuad.TextureName = "controller_x_big";
            MyQuad.Quad.SetColor(new Color(255, 255, 255, 0));

            MyQuad.Quad.EnforceTextureRatio();
            MyQuad.Scale(ButtonScale);
        }

        public PressNote(IObject Parent)
        {
            CoreData = new ObjectData();

            MyQuad = new QuadClass();

#if PC_VERSION
            MyText = new EzText("Press " + ButtonString.Up(ButtonScale), Tools.Font_Dylan60, 850, true, true);
#else
            MyText = new EzText("Press " + ButtonString.X(ButtonScale), Tools.Font_Dylan60, 850, true, true);
#endif
            MyText.Scale = TextScale;
            MyText.MyFloatColor = new Color(255, 255, 255).ToVector4();
            MyText.OutlineColor = new Color(0, 0, 0).ToVector4();

            MakeNew();

            Core.ParentObject = Parent;
            Parent.Core.MyLevel.AddObject(this);

            SetAlpha(0);
        }

        void SetAlpha(float Alpha)
        {
            this.Alpha = Alpha;
            MyQuad.Quad.SetColor(new Color(1f, 1f, 1f, Alpha / 255f));
            MyText.Alpha = Alpha / 255f;
        }

        public int FadeSpeed = 11;//9;
        public void FadeIn()
        {
            Count = 0;

            Alpha = Math.Min(255, Alpha + FadeSpeed);
            SetAlpha(Alpha);
        }

        //public int DelayToFadeOut = 40;
        public int DelayToFadeOut = 60;//80;
        int Count = 0;
        public void FadeOut()
        {
            Alpha = Math.Max(-50, Alpha - FadeSpeed);
            SetAlpha(Alpha);
        }

        public void Update()
        {
            if (Core.BoxesOnly) return;

            MyQuad.Pos = Core.Data.Position;
            MyText.Pos = Core.Data.Position;

            if (Core.ParentObject != null)
            {
                MyQuad.Pos += Core.ParentObject.Core.Data.Position;
                MyText.Pos += Core.ParentObject.Core.Data.Position;
            }
        }

        public void PhsxStep()
        {
            // Fade out if we haven't been activated in a while.
            if (Count > DelayToFadeOut ||
                Count > 1 && Alpha < 255)
                FadeOut();
            else
                Count++;
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

        public void TextDraw()
        {
            if (!OnScreen()) return;

            if (Tools.DrawGraphics)
            {
                //Update();
                //MyQuad.Draw();
                //MyText.Draw(Core.MyLevel.MainCamera);
            }
        }
        
        public void Draw()
        {
            //if (!OnScreen()) return;

            if (Tools.DrawGraphics)
            {
                Update();
                //MyQuad.Draw();
                MyText.Draw(Core.MyLevel.MainCamera, true);
            }
        }

        public void Move(Vector2 shift)
        {
            Core.Data.Position += shift;
            MyText.Pos += shift;
            Update();
        }

        public void Reset(bool BoxesOnly)
        {
            Core.Active = true;
        }

        public void Interact(Bob bob)
        {
        }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);

            PressNote PressXA = A as PressNote;

            PressXA.MyQuad.Clone(MyQuad);
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
//StubStubStubEnd5
    }
}