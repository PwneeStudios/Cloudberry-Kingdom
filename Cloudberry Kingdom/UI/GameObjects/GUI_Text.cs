using Microsoft.Xna.Framework;
using System;

namespace CloudberryKingdom
{
    public class GUI_Text : GUI_Panel
    {
        public enum Style { Bubble, Fade, None };
        public Style MyStyle = Style.Bubble;

        public Text MyText;

        public static GUI_Text SimpleTitle(Localization.Words word)
        {
            return SimpleTitle(word, Style.Bubble);
        }
        public static GUI_Text SimpleTitle(Localization.Words word, Style style)
        {
            Vector2 pos1 = new Vector2(-23, -23);
            Vector2 pos2 = new Vector2(23, 23);

            GUI_Text text = new GUI_Text(word, pos1, style);
            text.FixedToCamera = true;
            
            text.MyPile.FancyPos.LerpTo(pos1, pos2, 70, CoreEngine.LerpStyle.Linear);
            text.MyPile.FancyPos.Loop = true;
            return text;
        }

        public GUI_Text(Localization.Words word, Vector2 pos)
        {
            Init(word, pos, true, Style.Bubble, Resources.Font_Grobold42);
        }

        public GUI_Text(Localization.Words word, Vector2 pos, bool centered)
        {
            Init(word, pos, centered, Style.Bubble, Resources.Font_Grobold42);
        }

        public GUI_Text(Localization.Words word, Vector2 pos, Style style)
        {
            Init(word, pos, true, style, Resources.Font_Grobold42);
        }

        public GUI_Text(string text, Vector2 pos)
        {
            Init(text, pos, true, Style.Bubble, Resources.Font_Grobold42);
        }

        public GUI_Text(string text, Vector2 pos, bool centered)
        {
            Init(text, pos, centered, Style.Bubble, Resources.Font_Grobold42);
        }

        public GUI_Text(string text, Vector2 pos, bool centered, CoreFont font)
        {
            Init(text, pos, centered, Style.Bubble, font);
        }

        public void Init(Localization.Words word, Vector2 pos, bool centered, Style style, CoreFont font)
        {
            MyStyle = style;
            FixedToCamera = false;

            MyPile = new DrawPile();
            MyText = MakeText(word, centered, font);
            MyPile.Add(MyText);

            MyPile.Pos = pos + new Vector2(0, MyText.GetWorldHeight() / 2);

            if (MyStyle == Style.Bubble)
                MyPile.BubbleUp(true);
            if (MyStyle == Style.Fade)
                MyPile.FadeIn(.0175f);
            Active = true;
            Hid = false;
        }

        public void Init(string text, Vector2 pos, bool centered, Style style, CoreFont font)
        {
			if (text.Length == 0) text = "_";

            MyStyle = style;
            FixedToCamera = false;

            MyPile = new DrawPile();
            MyText = MakeText(text, centered, font);
            MyPile.Add(MyText);

            MyPile.Pos = pos + new Vector2(0, MyText.GetWorldHeight() / 2);

            if (MyStyle == Style.Bubble)
                MyPile.BubbleUp(true);
            if (MyStyle == Style.Fade)
                MyPile.FadeIn(.0175f);
            Active = true;
            Hid = false;
        }

        protected virtual Text MakeText(Localization.Words word, bool centered, CoreFont font)
        {
            return new Text(word, font, 1900, true, true, .575f);
        }

        protected virtual Text MakeText(string text, bool centered, CoreFont font)
        {
            return new Text(text, font, 1900, true, true, .575f);
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

            //this.Pos.Update();
            //MyPile.FancyPos.Update();

            base.MyDraw();
        }
    }
}