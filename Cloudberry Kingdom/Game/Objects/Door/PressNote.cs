using System;
using System.IO;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.InGameObjects;

namespace CloudberryKingdom
{
    public class PressNote : GUI_Text
    {
        public const int ButtonScale = 87;
        public const float TextScale = .555f / .7f * .8f;

        Door Parent;

        public PressNote(Door Parent) :
#if PC_VERSION
            base("Press " + ButtonString.Up(ButtonScale), Parent.Pos, true)
#else
            base("Press " + ButtonString.X(ButtonScale), Parent.Pos, true)
#endif
        {
#if CAFE
#endif
			MyText.Shadow = true;
			MyText.ShadowOffset = new Vector2(15);
			MyText.ShadowColor = ColorHelper.GrayColor(.5f);
			MyText.ShadowColor.A = 30;

            this.Parent = Parent;
            Oscillate = true;
            OscillationSpeed = .0125f;

            MyText.Scale = TextScale;
            MyText.ZoomWithCam = true;

            Core.DrawLayer = Level.LastInLevelDrawLayer;
            Core.RemoveOnReset = true;

            Core.ParentObject = Parent;

            QuadClass backdrop;
			backdrop = new QuadClass("WidePlaque", 300, true);
            MyPile.Add(backdrop);

            Pos.RelVal = Parent.Pos;

            Active = true;



			EzText _t;
			_t = MyPile.FindEzText(""); if (_t != null) { _t.Pos = new Vector2(0f, 0f); _t.Scale = 0.6342857f; }

			QuadClass _q;
			_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(-25f, -47.22223f); _q.Size = new Vector2(388.2494f, 146.3333f); }

			MyPile.Pos = new Vector2(33f, 340f);
        }

        int Life = 255;

        public int LifeSpeed = 11;//9;
        public void FadeIn()
        {
            Count = 0;
            Life = Math.Min(255, Life + LifeSpeed);
        }

        public int DelayToFadeOut = 60;//80;
        int Count = 0;
        public void FadeOut()
        {
            Life = Math.Max(-50, Life - LifeSpeed);

            if (Life < 0) { Kill(); Active = false; Parent.ClearNote();  }
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

			if (Parent == null || Parent.OnOpen == null)
			{
				Life = Math.Min(Life, 90);
				FadeOut();
				//Kill();
				//Active = false;
			}
			else
			{
				// Fade out if we haven't been activated in a while.
				if (Count > DelayToFadeOut ||
					Count > 1 && Life < 255)
					FadeOut();
				else
					Count++;
			}
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}