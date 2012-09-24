﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Drawing;

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

        QuadClass BlackQuad, Splash;

        public static int style = 0;
        public InitialLoadingScreen(ContentManager Content, WrappedFloat ResourceCount)
        {
            this.ResourceCount = ResourceCount;

            Whinney = Content.Load<SoundEffect>("Whinney");
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
                //QuadClass AboveBar = new QuadClass();
                //AboveBar.SetToDefault();
                //AboveBar.Quad.MyTexture = Tools.TextureWad.FindOrLoad(Content, "Art\\Tigar\\Splash");
                //MyPile.Add(AboveBar);

                Tools.TextureWad.FindOrLoad(Content, "Art\\Tigar\\Splash");
                Splash = new QuadClass("Art\\Tigar\\Splash", 1400);
                MyPile.Add(Splash);

                MyProgressBar = new ProgressBar();
                MyProgressBar.Pos = new Vector2(900, -400);
            }
            else if (style == 1)
            {
                EzText pwnee = new EzText("pwnee studios", Tools.Font_Grobold42, true, true);
                CampaignMenu._x_x_HappyBlueColor(pwnee);
                pwnee.Scale *= .9f;
                MyPile.Add(pwnee);

                MyProgressBar = new ProgressBar();
                MyProgressBar.Pos = new Vector2(0, -80);
            }

            BlackQuad = new QuadClass("White", 1400);
            BlackQuad.Quad.SetColor(new Color(0, 0, 0, 255));
            BlackQuad.Alpha = 0;
            BlackQuad.Layer = 1;
            MyPile.Add(BlackQuad);
        }

        public static int TotalResources = (int)(EzTextureWad.PercentToLoad * 699);
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
                        ResourceCount.MyFloat  = Tools.Restrict(0, TotalResources + FakeResources,
                            ResourceCount.MyFloat + .033f * (TotalResources + FakeResources));
                }
            }

            // Fade
            if (LoadingPercent > 97.6f && Accelerate || !Tools.TheGame.LoadingResources.MyBool)
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
            MyProgressBar.Pos = new Vector2(1100, -800);

            MyPile.Draw(0);

            MyProgressBar.Draw();
            
            MyPile.Draw(1);
        }
    }
}