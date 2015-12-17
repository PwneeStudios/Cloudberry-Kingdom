using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace CloudberryKingdom
{
    public class ProgressBar : ViewReadWrite
    {
        public enum BarType { Scale, Reveal };
        public BarType MyType = BarType.Scale;

        public override string[] GetViewables()
        {
            return new string[] { };
        }

        public Vector2 Full_BL { get { return Outline.BL; } }
        public Vector2 Full_TR { get { return Outline.TR; } }
        public Vector2 Current_TR
        {
            get
            {
                return new Vector2(Full_BL.X + (Full_TR.X - Full_BL.X) * PercentDone / 100f, Full_TR.Y);
            }
        }

        public DrawPile MyPile;

        public QuadClass Outline, Fill;

        public float PercentDone;
        public float Width;

        public Vector2 Pos
        {
            get { return MyPile.Pos; }
            set { MyPile.Pos = value; }
        }

        public ProgressBar()
        {
            Initialize();

            InitStandardBar();
        }

        void Initialize()
        {
            MyPile = new DrawPile();

            Outline = new QuadClass();
            Outline.SetToDefault();
            MyPile.Add(Outline);

            Fill = new QuadClass();
            Fill.SetToDefault();
            MyPile.Add(Fill);
        }

        public void InitStandardBar()
        {
            Outline.TextureName = "LoadOutline";
            Outline.ScaleYToMatchRatio(192);

            Fill.TextureName = "LoadFill";
            Fill.ScaleYToMatchRatio(192);
            Width = Fill.Size.X;
        }

        public ProgressBar(string FillName, string OutlineName, float Width)
        {
            Initialize();

            this.Width = Width;
            InitCustomBar(FillName, OutlineName, Width);
			Fill.Quad.SetColor(ColorHelper.GrayColor(.92435f));
			Outline.Quad.SetColor(ColorHelper.GrayColor(.9435f));
        }

        public void InitCustomBar(string FillName, string OutlineName, float Width)
        {
            Fill.TextureName = FillName;
            Fill.ScaleYToMatchRatio(Width);

            Outline.TextureName = OutlineName;
            Outline.ScaleYToMatchRatio(Width);

            this.Width = Width;

            Outline.Shadow = true;
            Outline.ShadowColor = new Color(.2f, .2f, .2f, 1f);
            Outline.ShadowOffset = new Vector2(12, 12);
        }

        public void SetPercent(float Percent)
        {
            Pos = new Vector2(0, -150);

            PercentDone = Percent;

            float FillWidth = Width * PercentDone / 100f;
            if (FillWidth < .01f) FillWidth = .01f;
            Fill.Size = new Vector2(FillWidth, Fill.Size.Y);
            Fill.Pos = new Vector2(FillWidth - Width, Fill.Pos.Y);

            switch (MyType)
            {
                case BarType.Scale:
                    break;

                case BarType.Reveal:
                    Fill.Quad.v1.Vertex.uv.X = PercentDone / 100f;
                    Fill.Quad.v3.Vertex.uv.X = PercentDone / 100f;

                    break;
            }
        }

        public void Draw()
        {
            MyPile.Draw();
        }

        public void Draw(bool Selected)
        {
            MyPile.Draw(Selected);
        }
    }

    public class InitialLoadingScreen
    {
        public bool IsDone = false;

        SoundEffect Whinney;

        DrawPile MyPile;
        ProgressBar MyProgressBar;

        int LogoCount = 0;
#if MONO
		int LogoCount_Max = 60 * 4 - 50 - 93; // 4 seconds, minus 50 frames to fade out, minus 1.5 seconds extra (Ubisoft compliance request)
		//int LogoCount_Max = 60 * 7 - 50 - 93; // 4 seconds, minus 50 frames to fade out, minus 1.5 seconds extra (Ubisoft compliance request)
#elif PC
		int LogoCount_Max = 60 * 4 - 50 - 93; // 4 seconds, minus 50 frames to fade out, minus 1.5 seconds extra (Ubisoft compliance request)
#else
		int LogoCount_Max = 60 * 5 - 50 - 93; // 5 seconds, minus 50 frames to fade out, minus 1.5 seconds extra (Ubisoft compliance request)
#endif
        WrappedFloat ResourceCount;

        QuadClass BlackQuad, Splash;

        public InitialLoadingScreen(ContentManager Content, WrappedFloat ResourceCount)
        {
            this.ResourceCount = ResourceCount;

            MyPile = new DrawPile();

            MyProgressBar = new ProgressBar();
            MyProgressBar.Pos = new Vector2(900, -400);

            BlackQuad = new QuadClass("White", 2000);
            BlackQuad.Quad.SetColor(new Color(0, 0, 0, 255));
            BlackQuad.Alpha = 0;
            BlackQuad.Layer = 1;
            MyPile.Add(BlackQuad);

			string text =
@"{pCopyRightSymbol,78,?,4,4} 2013 by Pwnee Studios, Corp. All Rights Reserved.
Distributed by Ubisoft Entertainment under license from Pwnee Studios, Corp.
Cloudberry Kingdom, Pwnee, and Pwnee Studios are trademarks of Pwnee Studios, Corp. and is used under license.
Ubisoft and the Ubisoft logo are trademarks of Ubisoft Entertainment in the US and/or other countries.";
			text = text.Replace("\r", "");
            Legal = new EzText(text, Resources.Font_Grobold42, 10000, false, false, .66f);

            Legal.MyFloatColor = ColorHelper.Gray(.9f);

			Loading = new EzText (Localization.Words.Loading, Resources.Font_Grobold42);
			Loading.Scale = .7f;
			Loading.Pos = new Vector2 (300, -600);
			Loading.MyFloatColor = ColorHelper.Gray(.9f);
			Loading.Alpha = 0;

            BlackQuad.Alpha = 1;

			if (!CloudberryKingdomGame.HideLogos) {
				MyPile.Add (Legal);
				MyPile.Add (Loading);
			}
        }
		EzText Legal, Loading;

        public static int TotalResources = 805;

        public bool Accelerate = false;
        int DoneCount = 0;
        public void PhsxStep()
        {
            LogoCount++;

            float LoadingPercent;

            LoadingPercent = 100f * ResourceCount.MyFloat / TotalResources;
            MyProgressBar.SetPercent(LoadingPercent);

            // 'Load' the fake resources
            if (ResourceCount.MyFloat >= TotalResources - 5)
            {
                ResourceCount.MyFloat += .5f;
                if (Accelerate)
                    ResourceCount.MyFloat = CoreMath.Restrict(0, TotalResources,
                        ResourceCount.MyFloat + .033f * (TotalResources));
            }

            // Fade
			//if (LoadingPercent > 97.6f && Accelerate || !Resources.LoadingResources.MyBool || LogoCount > LogoCount_Max)
			if (Resources.FinalLoadDone || LogoCount > LogoCount_Max)
            {
                if (ReadyToFade)
                {
					#if LONG_LOAD
					Legal.Alpha -= .0233f;
					if (LogoCount > LogoCount_Max + 60)
					{
						Loading.Alpha += .03f;
						if (LogoCount > LogoCount_Max + 40 + 5 * 60)
						{
							BlackQuad.Alpha += .0223f;
							if (BlackQuad.Alpha >= 1)
								DoneCount++;
						}
					}
					#else
					BlackQuad.Alpha += .0223f;
                    if (BlackQuad.Alpha >= 1)
                        DoneCount++;
					#endif
                }
            }

            if (!Resources.LoadingResources.MyBool && ReadyToFade && BlackQuad.Alpha >= 1 && DoneCount > 25)
                IsDone = true;

			if (CloudberryKingdomGame.QuickStart)
				IsDone = true;
        }

        int DrawCount = 0;
        bool ReadyToFade = false;
        public void Draw()
        {
			if (CloudberryKingdomGame.QuickStart)
				ReadyToFade = true;

            Legal.Scale = .25f;
            Legal.Pos = new Vector2(-1500, -500);

            DrawCount++;
            if (!ReadyToFade && DrawCount > 2)
                BlackQuad.Alpha -= .0633f;
            //if (DrawCount > 68)
            if (DrawCount > 90)
                ReadyToFade = true;

            MyProgressBar.Pos = new Vector2(1100, -800);

            MyPile.Draw(0);

            //MyProgressBar.Draw();
            
            MyPile.Draw(1);
        }
    }
}