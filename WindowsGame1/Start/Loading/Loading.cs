using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Drawing;

namespace CloudberryKingdom
{
    public class ProgressBar : IViewable
    {
        public enum BarType { Scale, Reveal };
        public BarType MyType = BarType.Scale;

        public string[] GetViewables()
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
            if (InitialLoadingScreen.style == 1)
                MyType = BarType.Reveal;

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
            if (InitialLoadingScreen.style == 0)
                Outline.TextureName = "LoadOutline";
            else
                Outline.TextureName = "LoadOutline_2";
            Outline.ScaleToTextureSize();

            if (InitialLoadingScreen.style == 0)
                Fill.TextureName = "LoadFill";
            else
                Fill.TextureName = "LoadFill_2";
            Fill.ScaleToTextureSize();
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

        SoundEffect Whinney, Smack, Pop, Angels;

        DrawPile MyPile;
        ProgressBar MyProgressBar;

        int LogoCount = 0;
        WrappedFloat ResourceCount;

        QuadClass BlackQuad;

        public static int style = 1;
        public InitialLoadingScreen(ContentManager Content, WrappedFloat ResourceCount)
        {
            this.ResourceCount = ResourceCount;

            Whinney = Content.Load<SoundEffect>("Whinney");
            //Smack = Content.Load<SoundEffect>("Smack");
            Pop = Content.Load<SoundEffect>("Pop 1");
            //Angels = Content.Load<SoundEffect>("Angels");

            if (style == 0)
            {
                Tools.TextureWad.FindOrLoad(Content, "Art\\LoadScreen\\LoadOutline");
                Tools.TextureWad.FindOrLoad(Content, "Art\\LoadScreen\\LoadFill");
            }
            else if (style == 1)
            {
                Tools.TextureWad.FindOrLoad(Content, "Art\\LoadScreen\\LoadOutline_2");
                Tools.TextureWad.FindOrLoad(Content, "Art\\LoadScreen\\LoadFill_2");
            }

            MyPile = new DrawPile();

            if (style == 0)
            {
                QuadClass AboveBar = new QuadClass();
                AboveBar.SetToDefault();
                AboveBar.Quad.MyTexture = Tools.TextureWad.FindOrLoad(Content, "Art\\LoadScreen\\PwneeStudios");
                AboveBar.ScaleToTextureSize();
                MyPile.Add(AboveBar);
            }
            else if (style == 1)
            {
                EzText pwnee = new EzText("pwnee studios", Tools.Font_DylanThin42, true, true);
                CampaignMenu.HappyBlueColor(pwnee);
                pwnee.Scale *= .9f;
                MyPile.Add(pwnee);
            }

            BlackQuad = new QuadClass("White", 1000);
            BlackQuad.Quad.SetColor(new Color(0, 0, 0, 255));
            BlackQuad.Alpha = 0;
            BlackQuad.Layer = 1;
            MyPile.Add(BlackQuad);

            MyProgressBar = new ProgressBar();
            MyProgressBar.Pos = new Vector2(0, -80);
        }

        public static int TotalResources = (int)(EzTextureWad.PercentToLoad * 553);
        //public static int TotalResources = 553;

        static int FakeResources = 360;//70;
        public bool Accelerate = false;
        int DoneCount = 0;
        public void PhsxStep()
        {
            LogoCount++;

            float LoadingPercent;

            //lock (ResourceCount)
            {
                LoadingPercent = 100 * (ResourceCount.MyFloat - 0) / (TotalResources + FakeResources - 0);
                MyProgressBar.SetPercent(LoadingPercent);

                // 'Load' the fake resources
                if (ResourceCount.MyFloat >= TotalResources - 5)
                {
                    ResourceCount.MyFloat += .5f;
                    if (Accelerate)
                        ResourceCount.MyFloat += .033f * (TotalResources + FakeResources);
                }
            }

            // Fade
            if (LoadingPercent > 97.6f && Accelerate)
            {
                BlackQuad.Alpha += .0223f;
                if (BlackQuad.Alpha >= 1)
                    DoneCount++;
            }

            if (!Tools.TheGame.LoadingResources.MyBool && DoneCount > 1)
            //if (!Tools.TheGame.LoadingResources.MyBool)
                IsDone = true;
        }

        public void Draw()
        {
            MyProgressBar.Draw();

            MyPile.Draw(0);
            MyPile.Draw(1);
        }
    }
}