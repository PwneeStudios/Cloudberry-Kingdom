using Microsoft.Xna.Framework;

namespace CloudberryKingdom.Awards
{
    public class AwardmentMessage : CkBaseMenu
    {
        public static int Style = 0;

        protected QuadClass Backdrop;

        public AwardmentMessage(Awardment award)
        {
            //Core.DrawLayer++;
            Core.DrawLayer += 2;

            PauseOnPause = false;
            PauseLevel = false;
            FixedToCamera = true;
            Core.RemoveOnReset = false;

            MyPile = new DrawPile();

            MakeBackdrop();

            SetText(award);

            EzText Title;
            if (Style == 0)
            {
                if (award.Unlockable == null)
                    //Title = new EzText("Game unlocked:", Tools.Font_Grobold42_2, 1800, false, false, .575f);
                    Title = new EzText("", Tools.Font_Grobold42_2, 1800, false, false, .575f);
                else
                    Title = new EzText("Awardment unlocked:", Tools.Font_Grobold42_2, 1800, false, false, .575f);
                Title.Pos =
                    new Vector2(-1726.192f, 369.0475f);
                    //new Vector2(-1686.508f, 341.2697f);
                Title.Scale *= .79f;
                MyPile.Add(Title);
            }
            else
            {
                Title = new EzText("Awardment", Tools.Font_Grobold42_2, 1800, false, false, .575f);
                Title.Pos = new Vector2(-1690.476f, 468.254f);
                Title.Scale *= .82f;
                MyPile.Add(Title);

                Title = new EzText("unlocked:", Tools.Font_Grobold42_2, 1800, false, false, .575f);
                Title.Pos = new Vector2(-1448.412f, 214.2857f);
                Title.Scale *= .82f;
                MyPile.Add(Title);
            }

            QuadClass cloud = new QuadClass("cloud1", 250, true);
            //cloud.Pos = new Vector2(1285.715f, 63.49208f);
            //cloud.Size = new Vector2(555.5547f, 447.5287f);
            cloud.Pos = new Vector2(1293.431f, 86.64027f);
            cloud.Size = new Vector2(586.4189f, 551.6953f);
            MyPile.Add(cloud);

            if (award.Unlockable == null)
            {
                QuadClass HatQuad = new QuadClass("Stickman", 263);
                HatQuad.Pos += new Vector2(1383.016f, 157.0794f);
                MyPile.Add(HatQuad);
            }
            else
            {
                QuadClass HatQuad = AwardmentMenu.MakeHatQuad(award.Unlockable);
                HatQuad.Pos += new Vector2(1373.016f, 115.0794f);
                MyPile.Add(HatQuad);
            }
        }

        protected virtual void MakeBackdrop()
        {
            Backdrop = new QuadClass(null, true, false);
            Backdrop.TextureName = "WidePlaque";
            Backdrop.Size = new Vector2(1750f, 284.8255f);
            Backdrop.Pos = new Vector2(-11.9043f, 59.52365f);

            MyPile.Add(Backdrop);
            MyPile.Pos = new Vector2(0, -800);
        }

        public override void SlideIn(int Frames)
        {
            Pos.RelVal = new Vector2(0, 0);
            Active = true;
            MyPile.BubbleUp(true);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            if (Frames == 0) return;

            Kill(true);
            Active = false;
        }

        protected EzText Text;
        public void SetText(Awardment award)
        {
            string text = award.Name;

            // Erase previous text
            MyPile.MyTextList.Clear();

            // Add the new text
            Text = new EzText(text, ItemFont, 1800, false, false, .575f);

            if (award.Unlockable == null)
            {
                Text.Scale *= 1.15f;
                MyPile.Add(Text);

                Vector2 size = Text.GetWorldSize();
                Text.Pos = new Vector2(-size.X / 2 - 350, size.Y * .85f) + new Vector2(38, 32);
            }
            else
            {
                Text.Scale *= .74f;
                MyPile.Add(Text);
                SizeAndPosition();
            }
        }

        protected virtual void SizeAndPosition()
        {
            Vector2 size = Text.GetWorldSize();
            float MaxSize = 1200;
            if (size.X > MaxSize)
                Text.Scale *= MaxSize / size.X;

            size = Text.GetWorldSize();
            Text.Pos = new Vector2(-size.X / 2 - 350, size.Y * .85f);

            if (Style == 0)
                Text.Pos += new Vector2(0, -42);
            else
                Text.Pos += new Vector2(790, -7);
        }

        protected int Step = 0;
        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            Step++;
            if (Step < 40) return;

            if (ShouldDie())
            {
                ReturnToCaller(false);
                PauseLevel = false;
            }
        }

        const int Duration = (int)(2.3 * 62);
        protected bool ShouldDie()
        {
            return Step > Duration;
        }

        public void Kill() { Kill(true); }
        public void Kill(bool sound)
        {
            MyPile.BubbleDownAndFade(sound);
            ReleaseWhenDone = false;
            ReleaseWhenDoneScaling = true;
        }
    }
}