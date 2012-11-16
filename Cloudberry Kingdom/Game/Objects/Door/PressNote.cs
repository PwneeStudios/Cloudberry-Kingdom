using System;
using System.IO;

using Microsoft.Xna.Framework;

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
            this.Parent = Parent;
            Oscillate = true;
            OscillationSpeed = .0125f;

            MyText.Scale = TextScale;
            MyText.ZoomWithCam = true;

            Core.DrawLayer = Level.LastInLevelDrawLayer;
            Core.RemoveOnReset = true;

            Core.ParentObject = Parent;

            QuadClass backdrop;
            backdrop = new QuadClass("Cloud1", 300, true);
            MyPile.Add(backdrop);
            backdrop.Pos = new Vector2(-189.0432f, -27.00623f);
            backdrop.Size = new Vector2(303.8582f, 157.471f);
            backdrop = new QuadClass("Cloud1", 300, true);
            MyPile.Add(backdrop);
            backdrop.Pos = new Vector2(106.59253f, -69.44446f);
            backdrop.Size = new Vector2(288.4258f, 149.755f);

            Pos.RelVal = Parent.Pos;

            MyPile.FancyPos.RelVal = new Vector2(33, 340);

            Active = true;
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

            // Fade out if we haven't been activated in a while.
            if (Count > DelayToFadeOut ||
                Count > 1 && Life < 255)
                FadeOut();
            else
                Count++;
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}