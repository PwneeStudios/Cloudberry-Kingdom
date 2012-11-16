using System.Collections.Generic;
using Microsoft.Xna.Framework;



namespace CloudberryKingdom
{
    public class DrawPile : ViewReadWrite, IViewableList
    {
        public override string[] GetViewables()
        {
            return new string[] { "MyQuadList", "MyTextList" };
        }

        public override string CopyToClipboard(string suffix)
        {
            if (suffix == null || suffix == "") suffix = "MyPile.";

            string s = "";

            if (MyTextList != null)
            {
                if (MyTextList.Count > 0) s += "EzText _t;\n";
                foreach (EzText text in MyTextList)
                    s += string.Format("_t = {0}FindEzText(\"{1}\"); if (_t != null) {{ _t.Pos = {2}; _t.Scale = {3}f; }}\n", suffix, text.Name, Tools.ToCode(text.Pos), text.Scale);
            }

            if (MyQuadList != null && MyQuadList.Count > 0)
            {
                if (MyTextList.Count > 0) s += "\n";

                if (MyQuadList.Count > 0) s += "QuadClass _q;\n";
                foreach (QuadClass quad in MyQuadList)
                {
                    s += string.Format("_q = {0}FindQuad(\"{1}\"); if (_q != null) {{ _q.Pos = {2}; _q.Size = {3}; }}\n",
                        suffix, quad.Name, Tools.ToCode(quad.Pos), Tools.ToCode(quad.Size));
                }

                s += "\n";
            }

            s += string.Format("{0}Pos = {1};\n", suffix, Tools.ToCode(Pos));

            return s;
        }

        public override void ProcessMouseInput(Vector2 shift, bool ShiftDown)
        {
            if (ShiftDown)
                Scale((shift.X + shift.Y) * .00003f);
            else
                Pos += shift;
        }

        public void GetChildren(List<InstancePlusName> ViewableChildren)
        {
            if (MyQuadList != null)
                foreach (QuadClass quad in MyQuadList)
                {
                    string name = quad.Name;
                    if (name.Length == 0)
                        name = quad.TextureName;
                    ViewableChildren.Add(new InstancePlusName(quad, name));
                }

            if (MyTextList != null)
                foreach (EzText text in MyTextList)
                {
                    string name = text.MyString;
                    ViewableChildren.Add(new InstancePlusName(text, name));
                }
        }

        public FancyVector2 FancyScale;
        public Vector2 Size
        {
            get
            {
                return FancyScale.RelVal;
            }
            set
            {
                FancyScale.RelVal = value;
            }
        }

        public FancyVector2 FancyPos;
        public Vector2 Pos
        {
            get { return FancyPos.RelVal; }
            set { FancyPos.RelVal = value; }
        }

        public List<EzText> MyTextList = new List<EzText>();
        public List<QuadClass> MyQuadList = new List<QuadClass>();

        public DrawPile()
        {
            FancyPos = new FancyVector2();

            FancyScale = new FancyVector2();
            Size = new Vector2(1, 1);
        }

        public DrawPile(FancyVector2 Center)
        {
            FancyPos = new FancyVector2(Center);

            FancyScale = new FancyVector2();
            Size = new Vector2(1, 1);
        }

        public void Clear()
        {
            MyTextList.Clear();
            MyQuadList.Clear();
        }

        public void Add(QuadClass quad)
        {
            Add(quad, false, null);
        }
        public void Add(QuadClass quad, string name)
        {
            Add(quad, false, name);
        }
        public void Add(QuadClass quad, bool KeepFancyCenter)
        {
            Add(quad, KeepFancyCenter, null);
        }
        public void Add(QuadClass quad, bool KeepFancyCenter, string name)
        {
            if (name != null) quad.Name = name;

            if (KeepFancyCenter && quad.FancyPos != null && quad.FancyPos.Center != null && quad.FancyPos.Center is FancyVector2)
                ((FancyVector2)quad.FancyPos.Center).SetCenter(FancyPos, true);
            else
            {
                quad.MakeFancyPos();
                quad.FancyPos.SetCenter(FancyPos, true);
            }

            MyQuadList.Add(quad);
        }

        public void Insert(int index, QuadClass quad)
        {
            quad.MakeFancyPos();
            quad.FancyPos.SetCenter(FancyPos, true);
            MyQuadList.Insert(index, quad);
        }

        public void Add(EzText text)
        {
            text.MakeFancyPos();
            text.FancyPos.SetCenter(FancyPos, true);
            MyTextList.Add(text);
        }

        public void Add(EzText text, string name)
        {
            text.Name = name;
            text.MakeFancyPos();
            text.FancyPos.SetCenter(FancyPos, true);
            MyTextList.Add(text);
        }

        public void Remove(EzText text)
        {
            MyTextList.Remove(text);
        }

        public QuadClass FindQuad(string Name)
        {
            return QuadClass.FindQuad(MyQuadList, Name);
        }

        public EzText FindEzText(string Name)
        {
            foreach (var text in MyTextList)
                if (string.Compare(text.Name, Name, System.StringComparison.InvariantCultureIgnoreCase) == 0)
                    return text;
            
            return null;
        }

        public float AlphaVel = 0;

        public FancyColor MyFancyColor = new FancyColor();
        public float Alpha
        {
            get { return MyFancyColor.A; }
            set { MyFancyColor.Color = new Color(255, 255, 255, value); }
        }

        public void Scale(float scale)
        {
            foreach (QuadClass quad in MyQuadList)
            {
                quad.Pos *= scale;
                //quad.ShadowOffset *= scale;
                quad.ShadowScale = 1f / scale;
                quad.Scale(scale);

                quad.ShadowOffset *= 18f * (scale - 1) + 1;

                Vector2 PosShift = .5f * (scale - 1) * quad.Size;
                Vector2 Hold = quad.ShadowOffset;
                quad.ShadowOffset += 1f * PosShift * new Vector2(1, 1.5f);
                quad.ShadowOffset.X = .5f * (quad.ShadowOffset.X + Hold.X - PosShift.X);
            }

            foreach (EzText text in MyTextList)
            {
                text.Pos *= scale;
                text.Scale *= scale;
            }
        }

        List<Vector2> SavedScales, SavedPositions, SavedShadowOffsets;
        public void SaveScale()
        {
            SavedScales = new List<Vector2>();

            foreach (QuadClass quad in MyQuadList)
                SavedScales.Add(quad.Size);

            SavedPositions = new List<Vector2>();

            foreach (QuadClass quad in MyQuadList)
                SavedPositions.Add(quad.Pos);

            SavedShadowOffsets = new List<Vector2>();

            foreach (QuadClass quad in MyQuadList)
                SavedShadowOffsets.Add(quad.ShadowOffset);
        }

        public void RevertScale()
        {
            foreach (QuadClass quad in MyQuadList)
            {
                quad.Size = SavedScales[MyQuadList.IndexOf(quad)];

                quad.Pos = SavedPositions[MyQuadList.IndexOf(quad)];

                quad.ShadowOffset = SavedShadowOffsets[MyQuadList.IndexOf(quad)];
                quad.ShadowScale = 1;
            }
        }

        public void Update()
        {
            Alpha += AlphaVel;
        }

        public void Draw() { Draw(0); }
        public void Draw(int Layer)
        {
            if (Fading) Fade();

            if (FancyScale != null) FancyScale.Update();
            if (MyFancyColor != null) MyFancyColor.Update();

            if (Alpha <= 0) return;

            DrawNonText(Layer);

            DrawText(Layer);
            Tools.Render.EndSpriteBatch();
        }

        public void DrawNonText(int Layer)
        {
            FancyPos.Update();

            foreach (QuadClass quad in MyQuadList)
            {
                if (quad.Layer == Layer)
                {
                    quad.ParentScaling = Size;
                    quad.ParentAlpha = Alpha;
                    quad.Draw();
                }
            }

            Tools.QDrawer.Flush();
        }

        public void DrawText(int Layer)
        {
            foreach (EzText text in MyTextList)
            {
                if (text.Layer == Layer)
                {
                    text.ParentScaling = Size;
                    text.ParentAlpha = Alpha;
                    text.Draw(Tools.CurCamera, false);
                }
            }
        }

        public OscillateParams MyOscillateParams;
        public void Draw(bool Selected)
        {
            if (Selected)
            {
                SaveScale();
                Scale(MyOscillateParams.GetScale());
                Draw();
                RevertScale();
            }
            else
            {
                MyOscillateParams.Reset();
                Draw();
            }
        }

        public bool Fading = false;
        public float FadeSpeed = 0;
        public void FadeIn(float speed)
        {
            Alpha = 0;
            Fading = true;
            FadeSpeed = speed;
        }
        public void FadeOut(float speed)
        {
            Alpha = 1;
            Fading = true;
            FadeSpeed = -speed;
        }
        void Fade()
        {
            Alpha += FadeSpeed;
        }

        public void BubbleDownAndFade(bool sound)
        {
            BubbleDown(sound, 5);
            FadeOut(1f / 20f);
        }

        public static Vector2[] BubbleScale = { new Vector2(0.001f), new Vector2(1.15f), new Vector2(.94f), new Vector2(1.05f), new Vector2(1f) };
        public void BubbleUp(bool sound) { BubbleUp(sound, 5, 1); }
        public void BubbleUp(bool sound, int Length, float Intensity)
        {
            Vector2[] scales;
            if (Intensity == 1)
                scales = BubbleScale;
            else
            {
                scales = new Vector2[BubbleScale.Length];
                for (int i = 0; i < BubbleScale.Length; i++)
                    scales[i] = (BubbleScale[i] - Vector2.One) * Intensity + Vector2.One;
            }

            FancyScale.MultiLerp(Length, scales);
            MyFancyColor.LerpTo(new Vector4(1f, 1f, 1f, 1f), Length);
            if (sound)
                Tools.CurGameData.WaitThenDo(2, new WaitThenPop(this));
        }

        class WaitThenPop : Lambda
        {
            DrawPile MyDrawPile_;
            public WaitThenPop(DrawPile MyDrawPile)
            {
                MyDrawPile_ = MyDrawPile;
            }

            public void Apply()
            {
                Tools.Pop(MyDrawPile_.MyPopPitch);
            }
        }

        /// <summary>
        /// The pitch of the pop noise when the draw pile is popped. Must be 1, 2, 3.
        /// </summary>
        public int MyPopPitch = 2;
        public void BubbleDown(bool sound) { BubbleDown(sound, 5); }
        public void BubbleDown(bool sound, int Length)
        {
            FancyScale.LerpTo(new Vector2(.1f), Length + 1);
            MyFancyColor.LerpTo(new Vector4(1f, 1f, 1f, 0f), Length);
            if (sound) Tools.SoundWad.FindByName("Pop_2").Play();
        }
        static Vector2[] JiggleScale = { new Vector2(1.15f), new Vector2(.94f), new Vector2(1.05f), new Vector2(1f) };
        public void Jiggle(bool sound) { Jiggle(sound, 5, 1f); }
        public void Jiggle(bool sound, int Length, float Intensity)
        {
            var mapped = new Vector2[JiggleScale.Length];
            for (int i = 0; i < JiggleScale.Length; i++)
                mapped[i] = (JiggleScale[i] - Vector2.One) * Intensity + Vector2.One;

            FancyScale.MultiLerp(Length, mapped);

            MyFancyColor.LerpTo(new Vector4(1f, 1f, 1f, 1f), Length);
            if (sound)
                Tools.CurGameData.WaitThenDo(2, new WaitThenPop2());
        }

        class WaitThenPop2 : Lambda
        {
            public void Apply()
            {
                Tools.SoundWad.FindByName("Pop_2").Play();
            }
        }
    }
}