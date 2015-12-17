using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class StartMenu_Clouds_Backpanel : CkBaseMenu
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

        protected QuadClass Scene, TM, Title, Left, Right;

        public override void Init()
        {
 	        base.Init();

            MyPile = new DrawPile();

            EnsureFancy();

            Setup();
            SetPos();
        }

        protected virtual void Setup()
        {
            Scene = new QuadClass("TitleScreen_Background");
            MyPile.Add(Scene, "Scene");

            TM = new QuadClass("TradeMarkSymbol");
            MyPile.Add(TM, "TM");

            Title = new QuadClass("CloudberryKingdomTitle");
            MyPile.Add(Title, "Title");

            Right = new QuadClass("TitleScreen_Right");
            MyPile.Add(Right, "Right");

            Left = new QuadClass("TitleScreen_Left");
            MyPile.Add(Left, "Left");
        }

        public void SetState(TitleBackgroundState state)
        {
            foreach (QuadClass quad in MyPile.MyQuadList)
            {
                quad.ResetFade();
                quad.Show = false;
            }

			foreach (EzText text in MyPile.MyTextList)
			{
				text.Show = false;
			}

            switch (state)
            {
                case TitleBackgroundState.None:
                    Title.Show = Scene.Show = false;
                    break;

                case TitleBackgroundState.Scene_Title:
                    Title.Show = Scene.Show = true;
                    Right.Show = Left.Show = true;
                    break;

                case TitleBackgroundState.CharSelect:
                    Scene.Show = true;
                    Title.Show = false;
                    break;






                case TitleBackgroundState.Scene:
                    Scene.Show = true;
                    Title.Show = false;
                    break;

                case TitleBackgroundState.Scene_StoryMode:
                    Scene.Show = true;
                    Title.Show = false;
                    Left.Show = true;
                    break;

                case TitleBackgroundState.Scene_Arcade:
                    Scene.Show = true;
                    Title.Show = false;
                    Right.Show = true;
                    break;

                case TitleBackgroundState.Scene_Arcade_Blur:
                    Scene.Show = true;
                    Title.Show = false;
                    Right.Show = true;
                    break;

                case TitleBackgroundState.Scene_Blur:
                    Scene.Show = true;
                    Title.Show = false;
                    Right.Show = Left.Show = true;
                    break;

                case TitleBackgroundState.Scene_Blur_Dark:
                    Scene.Show = true;
                    Title.Show = false;
                    Right.Show = Left.Show = true;
                    break;
            }
            MyState = state;

            TM.Show = Title.Show;
        }
        TitleBackgroundState MyState;

        protected virtual void SetPos()
        {
            QuadClass _q;
            _q = MyPile.FindQuad("Scene"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(1778f, 1000.125f); }
            _q = MyPile.FindQuad("TM"); if (_q != null) { _q.Pos = new Vector2(1177.778f, 811.1111f); _q.Size = new Vector2(42.55546f, 31.9166f); }
            _q = MyPile.FindQuad("Title"); if (_q != null) { _q.Pos = new Vector2(63.88864f, 486.1108f); _q.Size = new Vector2(1170.618f, 358.9894f); }

            MyPile.Pos = new Vector2(0f, 0f);
        }

		protected override void ReleaseBody()
		{
			base.ReleaseBody();
		}

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

#if PC
			CloudberryKingdomGame.SuppressSavingTextDuration = 60;
#endif
        }
        
        float t = 0;
        protected override void MyDraw()
        {
            // Oscillate title color
            if (Title.Show)
            {
                t += .01f;

                Vector4 c1 = new Vector4(0);
                Vector4 c2 = new Vector4(1);
                Title.Quad.SetColor(Vector4.Lerp(c1, c2, t));
            }

            if (CloudberryKingdomGame.AndersSwitch)
            {
                Title.Draw();
            }
            else
            {
                base.MyDraw();
            }
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();
        }
    }
}