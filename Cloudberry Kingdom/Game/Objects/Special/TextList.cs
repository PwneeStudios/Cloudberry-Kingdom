using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using System.IO;

namespace CloudberryKingdom
{
    public class TextList : ObjectBase
    {
        public override void Release()
        {
            base.Release();

            Text = null;
        }

        public List<EzText> Text;
        public int Index;
        public float ContinuousIndex;

        public Camera MyCam;

        public bool FadeOut;
        public float Alpha;

        public override void MakeNew()
        {
        }

        public TextList()
        {
            Text = new List<EzText>();
            SetIndex(0);

            Alpha = 1;
        }

        public void SetIndex(int index)
        {
            ContinuousIndex = Index = index;
        }

        //public void AddLine(String s)
        //{
        //    Text.Add(new EzText(s, Resources.Font_Grobold42, true));
        //}

        public override void PhsxStep()
        {
            ContinuousIndex += .2f * (Index - ContinuousIndex);

            if (FadeOut) Alpha = Math.Max(0, Alpha - .03f);
        }

        public override void Draw()
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
            Tools.Render.EndSpriteBatch();
        }

        public void ShiftUp()
        {
            Index++; if (Index >= Text.Count) Index = Text.Count - 1;
        }
        public void ShiftDown()
        {
            Index--; if (Index < 0) Index = 0;
        }

        public override void Move(Vector2 shift)
        {
            Core.Data.Position += shift;
        }
    }
}
