using System.IO;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class _____ : IObject
    {
        public void TextDraw() { }

        public void Release()
        {
            Core.Release();
        }

        ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        public void MakeNew()
        {
            Core.Init();
            Core.DrawLayer = 5;
        }

        public _____()
        {
            CoreData = new ObjectData();

            MakeNew();
        }

        public void Draw()
        {
        }

        public void PhsxStep()
        {
        }

        public void PhsxStep2() { }
        public void Reset(bool BoxesOnly) { }
        public void Clone(IObject A) { }
        public void Interact(Bob bob) { }
        public void Move(Vector2 shift) { }
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
//StubStubStubEnd4
    }
}