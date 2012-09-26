using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_Forest_Title : StartMenuBase
    {
        public TitleGameData_Forest Title;
        public StartMenu_Forest_Title(TitleGameData_Forest Title)
            : base()
        {
            this.Title = Title;
            //Core.DrawLayer++;
        }

        public override void SlideIn(int Frames)
        {
            base.SlideIn(0);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, 0);
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }


        public QuadClass TitleQ;
        public override void Init()
        {
 	        base.Init();

            CallDelay = ReturnToCallerDelay = 0;

            MyPile = new DrawPile();

            EnsureFancy();

            TitleQ = new QuadClass("Title_Teal"); TitleQ.Quad.SetColor(Color.Black);
            TitleQ.Alpha = 1f;
            MyPile.Add(TitleQ, "Title");

            SetPos();
        }
        
        public void SetPos()
        {
            QuadClass _q;
            _q = MyPile.FindQuad("Title"); if (_q != null) { _q.Pos = new Vector2(-22.2207f, 52.7778f); _q.Size = new Vector2(1069.027f, 429.9995f); }

            MyPile.Pos = new Vector2(0f, 0f);
        }

        public void SetPos_Menu()
        {
            QuadClass _q;
            _q = MyPile.FindQuad("Title"); if (_q != null) { _q.Pos = new Vector2(-22.2207f, 55.90039f); _q.Size = new Vector2(1069.027f, 429.9995f); }

            MyPile.Pos = new Vector2(-83.33347f, 538.8888f);
        }

        public void SetPos_Campaign()
        {
            QuadClass _q;
            _q = MyPile.FindQuad("Title"); if (_q != null) { _q.Pos = new Vector2(-22.2207f, 55.90039f); _q.Size = new Vector2(1069.027f, 429.9995f); }

            MyPile.Pos = new Vector2(-83.33347f, 2538.8888f);
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep(); if (!Active) return;
        }

        float t = 0;
        protected override void MyDraw()
        {
            if (!Active) return;

            t += .01f;

            Vector4 c1 = new Vector4(0);
            Vector4 c2 = new Vector4(1);
            TitleQ.Quad.SetColor(Vector4.Lerp(c1, c2, t));

            // Oscillate brightness
            if (t > 1)
            {
                c1 = new Color(255, 255, 255).ToVector4();
                c2 = new Color(230, 230, 230).ToVector4();
                TitleQ.Quad.SetColor(Vector4.Lerp(c1, c2, CoreMath.Periodic(0, 1, 3, t - 1)));
            }


            TitleQ.Size = new Vector2(1069.027f, 429.9995f) * Tools.SmoothLerp(.85f, 1f, t);
                // .LerpRestrict(.8f, 1f, t);
            TitleQ.Pos = new Vector2(-22.2207f, 52.7778f + CoreMath.Periodic(0, 12, 9, Tools.t));

            base.MyDraw();
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();
        }
    }
}