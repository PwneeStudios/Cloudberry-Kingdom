using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class GUI_Text : GUI_Panel
    {
        public enum Style { Bubble, Fade, None };
        public Style MyStyle = Style.Bubble;

        public EzText MyText;

        public static GUI_Text SimpleTitle(string str)
        {
            return SimpleTitle(str, Style.Bubble);
        }
        public static GUI_Text SimpleTitle(string str, Style style)
        {
            Vector2 pos1 = new Vector2(-23, -23);
            Vector2 pos2 = new Vector2(23, 23);

            GUI_Text text = new GUI_Text(str, pos1, style);
            text.FixedToCamera = true;
            
            text.MyPile.FancyPos.LerpTo(pos1, pos2, 70, Drawing.LerpStyle.Linear);
            text.MyPile.FancyPos.Loop = true;
            return text;
        }

        public GUI_Text(string text, Vector2 pos)
        {
            Init(text, pos, true, Style.Bubble);
        }

        public GUI_Text(string text, Vector2 pos, Style style)
        {
            Init(text, pos, true, style);
        }

        public GUI_Text(string text, Vector2 pos, bool centered)
        {
            Init(text, pos, centered, Style.Bubble);
        }

        public void Init(string text, Vector2 pos, bool centered, Style style)
        {
            MyStyle = style;
            FixedToCamera = false;

            MyPile = new DrawPile();
            MyText = MakeText(text, centered);
            MyPile.Add(MyText);

            MyPile.Pos = pos + new Vector2(0, MyText.GetWorldHeight() / 2);

            if (MyStyle == Style.Bubble)
                MyPile.BubbleUp(true);
            if (MyStyle == Style.Fade)
                MyPile.FadeIn(.0175f);
            Active = true;
            Hid = false;
        }

        protected virtual EzText MakeText(string text, bool centered)
        {
            return new EzText(text, Tools.Font_DylanThin42, 1900, true, true, .575f);
        }

        public void Kill() { Kill(true); }
        public void Kill(bool sound)
        {
            if (MyStyle == Style.Bubble)
            {
                MyPile.BubbleDown(sound);
                MyPile.FadeOut(1f / 20f);
            }
            if (MyStyle == Style.Fade)
                MyPile.FadeOut(.0175f);

            ReleaseWhenDoneScaling = true;
        }

        /// <summary>
        /// When true the position is not modified by the phsx update.
        /// </summary>
        public bool NoPosMod = true;//false;

        public bool Oscillate = false;
        protected float OscillationHeight = 12; // 4.65f;
        protected float OscillationSpeed = .0262f;
        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            if (!NoPosMod)
            {
                if (Oscillate)
                {
                    Pos.RelVal = Core.StartData.Position +
                        new Vector2(0, OscillationHeight *
                            (float)Math.Sin(OscillationSpeed * (Core.MyLevel.CurPhsxStep) + Core.AddedTimeStamp));
                }
                else
                {
                    Core.Data.Integrate();
                    Pos.RelVal = Core.Data.Position;
                }
            }
        }

        protected override void MyDraw()
        {
            if (Hid) return;

            base.MyDraw();
        }
    }
}