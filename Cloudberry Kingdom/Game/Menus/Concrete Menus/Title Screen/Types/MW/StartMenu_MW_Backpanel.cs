using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_MW_Backpanel : CkBaseMenu
    {
        public override void Hide(PresetPos pos, int frames)
        {
            base.Hide(pos, frames);
            ButtonCheck.PreLogIn = false;
        }

        public override void SlideIn(int Frames)
        {
            base.SlideIn(0);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            //base.SlideOut(Preset, 0);
            base.SlideOut(Preset, Frames);
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        public void InitialZoomIn()
        {
            var size = Scene.Size;
            Scene.FancyScale = new CoreEngine.FancyVector2();
            Scene.FancyScale.LerpTo(1.025f * size, size, 120, CoreEngine.LerpStyle.DecayNoOvershoot);
        }

        QuadClass Scene, Title, Scene_NoBob_Blur, Scene_Blur, Scene_Princess, Scene_NoBob_Brighten,
                    Scene_Kobbler, Scene_Kobbler_Blur;
        public override void Init()
        {
 	        base.Init();

            MyPile = new DrawPile();

            EnsureFancy();

            Scene = new QuadClass("Title_Screen", 1778);
            MyPile.Add(Scene, "Scene");

            Title = new QuadClass("Title", 1778);
            MyPile.Add(Title, "Title");

            Scene_NoBob_Blur = new QuadClass("Title_NoBob_Blur", 1778);
            MyPile.Add(Scene_NoBob_Blur, "Scene_NoBob_Blur");

            Scene_NoBob_Brighten = new QuadClass("Title_NoBob_Brighten", 1778);
            MyPile.Add(Scene_NoBob_Brighten, "Scene_NoBob_Brighten");

            Scene_Blur = new QuadClass("Title_Blur", 1778);
            MyPile.Add(Scene_Blur, "Scene_Blur");

            Scene_Princess = new QuadClass("Scene_Princess", 1778);
            MyPile.Add(Scene_Princess, "Scene_Princess");

            Scene_Kobbler = new QuadClass("Scene_Kobbler", 1778);
            MyPile.Add(Scene_Kobbler, "Scene_Kobbler");

            Scene_Kobbler_Blur = new QuadClass("Scene_Kobbler_Blur", 1778);
            MyPile.Add(Scene_Kobbler_Blur, "Scene_Kobbler_Blur");

            BlackBox();
        }

        public enum State { Scene_Title, Scene, Scene_Blur, Scene_Princess, Scene_NoBob_Brighten, Scene_Blur_Dark,
                            Scene_Kobbler, Scene_Kobbler_Blur };
        public void SetState(State state)
        {
            foreach (QuadClass quad in MyPile.MyQuadList)
            {
                quad.ResetFade();
                quad.Show = false;
            }

            switch (state)
            {
                case State.Scene_Title:
                    Title.Show = Scene.Show = true; break;

                case State.Scene:
                    Scene.Show = true; break;

                case State.Scene_Princess:
                    Scene_Princess.Show = true; break;

                case State.Scene_Kobbler:
                    Scene_Kobbler.Show = true; break;

                case State.Scene_Kobbler_Blur:
                    Scene_Kobbler_Blur.Show = true; break;

                case State.Scene_Blur:
                    if (MyState == State.Scene_Title) Scene.Show = true;
                    if (MyState == State.Scene_Princess) Scene_Princess.Show = true;
                    if (MyState == State.Scene_NoBob_Brighten) Scene_NoBob_Brighten.Show = true;
                    Scene_Blur.Alpha = .01f;
                    Scene_Blur.Fade(.25f);
                    Scene_Blur.Show = true; break;

                case State.Scene_Blur_Dark:
                    if (MyState == State.Scene_Title) Scene.Show = true;
                    if (MyState == State.Scene_Princess) Scene_Princess.Show = true;
                    if (MyState == State.Scene_NoBob_Brighten) Scene_NoBob_Brighten.Show = true;
                    if (MyState == State.Scene_Blur)
                    {
                        Scene_Blur.Fade(.25f);
                        Scene_Blur.Show = true;
                    }
                    else
                    {
                        Scene_Blur.Alpha = .01f;
                        Scene_Blur.Fade(.25f);
                        Scene_Blur.Show = true;
                    }
                    break;

                case State.Scene_NoBob_Brighten:
                    //if (MyState == State.Scene_Blur) { Scene_Blur.Show = true; Scene_Blur.Alpha = 1f; Scene_Blur.Fade(-.25f); }
                    if (MyState == State.Scene_Title)
                        //{ Scene.Show = true; Scene.Alpha = 1f; }
                        { Scene_NoBob_Blur.Show = true; Scene_NoBob_Blur.Alpha = 1f; }
                    Scene_NoBob_Brighten.Show = true;
                    Scene_NoBob_Brighten.Alpha = 0f;
                    Scene_NoBob_Brighten.Fade(.125f);
                    break;
            }
            MyState = state;
        }
        State MyState;

        void BlackBox()
        {
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();
        }
        
        float t = 0;
        protected override void MyDraw()
        {
            // Fade to black/from black for blurred background
            float black = Scene_Blur.Quad.MySetColor.R / 255f;
            if (MyState == State.Scene_Blur_Dark)
                black = CoreMath.Restrict(.7f, 1f, black - .33f);
            else
                black = CoreMath.Restrict(.7f, 1f, black + .33f);
            Scene_Blur.Quad.SetColor(ColorHelper.Gray(black));

            // Oscillate title color
            if (Title.Show)
            {
                t += .01f;

                //Scene.Size = new Vector2(1069.027f, 429.9995f) * Tools.SmoothLerp(.85f, 1f, t);

                Vector4 c1 = new Vector4(0);
                Vector4 c2 = new Vector4(1);
                Title.Quad.SetColor(Vector4.Lerp(c1, c2, t));
            }

            base.MyDraw();
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();
        }
    }
}