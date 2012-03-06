using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class TextList : IObject
    {
        public void TextDraw() { }

        public void Release()
        {
            Core.Release();
            Text = null;
        }

        public List<EzText> Text;
        public int Index;
        public float ContinuousIndex;

        public ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        public Camera MyCam;

        public bool FadeOut;
        public float Alpha;

        public void MakeNew()
        {
        }

        public TextList()
        {
            CoreData = new ObjectData();

            Text = new List<EzText>();
            SetIndex(0);

            Alpha = 1;
        }

        public void SetIndex(int index)
        {
            ContinuousIndex = Index = index;
        }

        public void AddLine(String s)
        {
            Text.Add(new EzText(s, Tools.Font_Dylan20, true));
        }

        public void PhsxStep()
        {
            ContinuousIndex += .2f * (Index - ContinuousIndex);

            if (FadeOut) Alpha = Math.Max(0, Alpha - .03f);
        }

        public void PhsxStep2() { }

        public void Draw()
        {
            //PhsxStep();

            for (int i = Index - 2; i <= Index + 2; i++)
            {
                if (i >= 0 && i < Text.Count)
                {
                    Text[i]._Pos = Core.Data.Position - (i - ContinuousIndex) * new Vector2(0, 100);
                    Text[i].MyFloatColor.W = Alpha * .5f * (2 - Math.Abs(i - ContinuousIndex));
                    Text[i].Draw(MyCam, false);
                }
            }
            Tools.EndSpriteBatch();
        }

        public void ShiftUp()
        {
            Index++; if (Index >= Text.Count) Index = Text.Count - 1;
        }
        public void ShiftDown()
        {
            Index--; if (Index < 0) Index = 0;
        }

        public void Move(Vector2 shift)
        {
            Core.Data.Position += shift;
        }

        public void Reset(bool BoxesOnly) { }
        public void Interact(Bob bob) { }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);
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
