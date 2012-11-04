using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using CoreEngine;

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
        WrappedFloat ResourceCount;

        QuadClass BlackQuad, Splash;

        public InitialLoadingScreen(ContentManager Content, WrappedFloat ResourceCount)
        {
            this.ResourceCount = ResourceCount;

            Whinney = Content.Load<SoundEffect>("Whinney");

            Tools.TextureWad.FindOrLoad(Content, "Art\\LoadScreen_Initial\\LoadOutline");
            Tools.TextureWad.FindOrLoad(Content, "Art\\LoadScreen_Initial\\LoadFill");

            MyPile = new DrawPile();

            //Tools.TextureWad.FindOrLoad(Content, "Splash");
            //Splash = new QuadClass("Splash", 1400);
            //MyPile.Add(Splash);

            MyProgressBar = new ProgressBar();
            MyProgressBar.Pos = new Vector2(900, -400);

            BlackQuad = new QuadClass("White", 1400);
            BlackQuad.Quad.SetColor(new Color(0, 0, 0, 255));
            BlackQuad.Alpha = 0;
            BlackQuad.Layer = 1;
            MyPile.Add(BlackQuad);
        }

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
            if (LoadingPercent > 97.6f && Accelerate || !Resources.LoadingResources.MyBool)
            {
                BlackQuad.Alpha += .0223f;
                if (BlackQuad.Alpha >= 1)
                    DoneCount++;
            }

            if (NoShow)
            {
                if (!Resources.LoadingResources.MyBool)
                    IsDone = true;
            }
            else
            {
                if (!Resources.LoadingResources.MyBool && DoneCount > 1)
                    //if (!Resources.LoadingResources.MyBool)
                    IsDone = true;
            }
        }

        const bool NoShow = true;

        public void Draw()
        {
            if (NoShow) return;

            MyProgressBar.Pos = new Vector2(1100, -800);

            MyPile.Draw(0);

            MyProgressBar.Draw();
            
            MyPile.Draw(1);
        }
    }
}