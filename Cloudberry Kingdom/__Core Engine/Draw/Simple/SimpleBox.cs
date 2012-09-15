using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using CloudberryKingdom;

namespace Drawing
{
    public class SimpleBox
    {
        public bool Animated;
        public SimpleVector BL, TR;

        public void Release()
        {
            BL.Release();
            TR.Release();
        }

        public void SetHold()
        {
            BL.AnimData.Hold = BL.Pos;
            TR.AnimData.Hold = TR.Pos;
        }

        public void ReadAnim(int anim, int frame)
        {
            BL.Pos = BL.AnimData.Get(anim, frame);
            TR.Pos = TR.AnimData.Get(anim, frame);
        }

        public void Record(int anim, int frame)
        {
            BL.AnimData.Set(BL.Pos, anim, frame);
            TR.AnimData.Set(TR.Pos, anim, frame);
        }

        public void Transfer(int anim, float DestT, int AnimLength, bool Loop, bool Linear, float t)
        {
            BL.Pos = BL.AnimData.Transfer(anim, DestT, AnimLength, Loop, Linear, t);
            TR.Pos = TR.AnimData.Transfer(anim, DestT, AnimLength, Loop, Linear, t);
        }

        public void Calc(int anim, float t, int AnimLength, bool Loop, bool Linear)
        {
            BL.Pos = BL.AnimData.Calc(anim, t, AnimLength, Loop, Linear);
            TR.Pos = TR.AnimData.Calc(anim, t, AnimLength, Loop, Linear);
        }

        public SimpleBox(SimpleBox box)
        {
            Animated = box.Animated;

            BL = box.BL;
            TR = box.TR;
        }

        public SimpleBox(ObjectBox box)
        {
            Animated = true;

            BL.Pos = box.BL.Pos;
            TR.Pos = box.TR.Pos;

            BL.AnimData = box.BL.AnimData;
            TR.AnimData = box.TR.AnimData;
        }

        /// <summary>
        /// Copy and shift the source boxes's vertex locations.
        /// </summary>
        /// <param name="SourceBox">The source box</param>
        /// <param name="shift">The amount to shift</param>
        public void CopyUpdate(ref SimpleBox SourceBox, ref Vector2 shift)
        {
            BL.Vertex.xy = SourceBox.BL.Vertex.xy + shift;
            TR.Vertex.xy = SourceBox.TR.Vertex.xy + shift;
        }

        public void Update(ref BasePoint Base)
        {
            BL.Vertex.xy = Base.Origin + BL.Pos.X * Base.e1 + BL.Pos.Y * Base.e2;
            TR.Vertex.xy = Base.Origin + TR.Pos.X * Base.e1 + TR.Pos.Y * Base.e2;
        }

        public Vector2 Center()
        {
            return (TR.Vertex.xy + BL.Vertex.xy) / 2;
        }

        public float Width(ref BasePoint Base)
        {
            return (TR.Pos.X - BL.Pos.X) * Base.e1.Length();
        }

        public Vector2 Size()
        {
            return (TR.Vertex.xy - BL.Vertex.xy);
        }

        /*        public void DrawExtra(QuadDrawer Drawer, Color clr)
                {
                    Drawer.DrawLine(BL.Pos, new Vector2(TR.Pos.X, BL.Pos.Y), clr, .02f);
                    Drawer.DrawLine(BL.Pos, new Vector2(BL.Pos.X, TR.Pos.Y), clr, .02f);
                    Drawer.DrawLine(TR.Pos, new Vector2(TR.Pos.X, BL.Pos.Y), clr, .02f);
                    Drawer.DrawLine(TR.Pos, new Vector2(BL.Pos.X, TR.Pos.Y), clr, .02f);
                }
          */
    }
}