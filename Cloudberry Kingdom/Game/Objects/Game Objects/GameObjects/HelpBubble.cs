using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class HelpBubble : GUI_Panel
    {
        public EzText MyText;

        /// <summary>
        /// The number of times to show the help bubble.
        /// If 0 then infinite
        /// </summary>
        public int TimesToShow = 0;

        public HelpBubble(Vector2 pos)
        {
            Init("Help!!!", pos, true);
        }

        public HelpBubble(string text, Vector2 pos)
        {
            Init(text, pos, true);
        }

        public HelpBubble(string text, Vector2 pos, bool centered)
        {
            Init(text, pos, centered);
        }

        enum Orientation { Left, Right }
        Orientation MyOrientation;

        public void ArrowToLeft()
        {
            if (MyOrientation == Orientation.Right)
            {
                Backdrop.Pos = new Vector2(-50.43185f, -91.26987f);
                Backdrop.Quad.MirrorUV_Horizontal();
                MyOrientation = Orientation.Left;
            }
        }

        public void ArrowToRight()
        {
            if (MyOrientation == Orientation.Left)
            {
                Backdrop.Quad.MirrorUV_Horizontal();
                MyOrientation = Orientation.Right;
            }
        }

        QuadClass Backdrop;
        public void Init(string text, Vector2 pos, bool centered)
        {
            FixedToCamera = false;
            Core.DrawLayer = 9;

            MyPile = new DrawPile();
            MyText = MakeText(text, centered);
            MyText.ZoomWithCam = true;
            MyText.MyFloatColor = Color.HotPink.ToVector4();
            MyPile.Add(MyText);

            Backdrop = new QuadClass("speechbubble_white", 400, true);
            Backdrop.Quad.UseGlobalIllumination = false;
            MyPile.Add(Backdrop);
            Backdrop.Size = new Vector2(499.2063f, 171.2172f);
            Backdrop.Pos = new Vector2(35.71419f, -79.36511f);// new Vector2(-39.68262f, -71.42859f);

            Backdrop.Quad.MirrorUV_Horizontal();
            Backdrop.Quad.MirrorUV_Vertical();
            MyOrientation = Orientation.Right;
            Backdrop.Quad.SetColor(.35f * Color.HotPink.ToVector4() + .65f * Color.White.ToVector4());

            MyPile.Pos = pos + new Vector2(0, MyText.GetWorldHeight() / 2);

            MyPile.Scale(1.15f);

            MyPile.BubbleUp(true);
            Active = true;
            Hid = false;
        }

        protected virtual EzText MakeText(string text, bool centered)
        {
            return new EzText(text, Resources.Font_Grobold42, 1000, true, true, .525f);
        }

        public void Kill() { Kill(true); }
        public void Kill(bool sound)
        {
            MyPile.BubbleDownAndFade(sound);
            ReleaseWhenDoneScaling = true;
        }

        public bool Oscillate = false;
        float OscillationHeight = 12; // 4.65f;
        int Count = 1;
        int TimesShown = 0;
        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            Count++;
            int Period = 90; int BubbleDown = 75;
            if (Count % Period == 0)
            {
                TimesShown++;
                MyPile.BubbleUp(false);
            }
            if (Count % Period == BubbleDown)
            {
                MyPile.BubbleDownAndFade(false);
                if (TimesShown == TimesToShow - 1)
                {
                    Active = false;
                    ReleaseWhenDoneScaling = true;
                }
            }

            /*
            if (Oscillate)
                Pos.RelVal = Core.StartData.Position +
                    new Vector2(0, OscillationHeight *
                        (float)Math.Sin(.0262f * (Core.MyLevel.CurPhsxStep) + Core.AddedTimeStamp));*/
        }

        protected override void MyDraw()
        {
            if (Hid) return;

            base.MyDraw();
        }
    }
}