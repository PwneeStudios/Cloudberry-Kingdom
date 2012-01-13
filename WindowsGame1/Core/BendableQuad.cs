using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.IO;

using Drawing;

namespace Drawing
{
    public class BendableQuad : BaseQuad
    {
        GraphicsDevice Device;

        public int N, NumTriangles;
        public float Width, Resolution;

#if EDITOR
        public ObjectVector WidthControl, ResolutionControl;
        private static Vector2 WidthControlDir = new Vector2(-3f, -4f) / 5f;
        private static Vector2 ResolutionControlDir = new Vector2(-2f, -1f) / new Vector2(-2f, -1f).Length();
#endif

        public Spline MySpline;
        static ObjectVector temp1 = new ObjectVector();
        static ObjectVector temp2 = new ObjectVector();

        public void Release()
        {
            Device = null;
            MySpline.Release(); MySpline = null;
        }

        public override void CopyAnim(BaseQuad basequad, int Anim)
        {
            BendableQuad quad = basequad as BendableQuad;
            if (null != quad)
                MySpline.CopyAnim(quad.MySpline, Anim);
        }

        public override void CopyAnimShallow(BaseQuad basequad, int Anim)
        {
            BendableQuad quad = basequad as BendableQuad;
            if (null != quad)
                MySpline.CopyAnimShallow(quad.MySpline, Anim);
        }

        public override void SetHold()
        {
            MySpline.SetHold();
        }

#if EDITOR
        public override void SaveState(int StateIndex)
        {
            base.SaveState(StateIndex);

            MySpline.SaveState(StateIndex);
        }

        public override void RecoverState(int StateIndex)
        {
            base.RecoverState(StateIndex);

            MySpline.RecoverState(StateIndex);
        }
#endif

        public override void ReadAnim(int anim, int frame)
        {
            MySpline.ReadAnim(anim, frame);
        }

        public override void Record(int anim, int frame, bool UseRelativeCoords)
        {
            MySpline.Record(anim, frame, UseRelativeCoords);
        }

        public override void Transfer(int anim, float DestT, int AnimLength, bool Loop, bool Linear, float t)
        {
            MySpline.Transfer(anim, DestT, AnimLength, Loop, Linear, t);
        }

        public override void Calc(int anim, float t, int AnimLength, bool Loop, bool Linear)
        {
            MySpline.Calc(anim, t, AnimLength, Loop, Linear);
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            MySpline.Write(writer, ParentObject);

            writer.Write(NumVertices);
            writer.Write(N);
            writer.Write(NumTriangles);
            writer.Write(Width);
            writer.Write(Resolution);

            for (int i = 0; i < NumVertices; i++)
                WriteReadTools.WriteVertex(writer, Vertices[i]);

            WriteReadTools.WriteColor(writer, MyColor);

            writer.Write(MyTexture.Path);
            writer.Write(MyEffect.Name);

            if (ParentQuad == null)
                writer.Write(-1);
            else
                writer.Write(ParentObject.QuadList.IndexOf(ParentQuad));
        }

        public override void Read(BinaryReader reader, EzEffectWad EffectWad, EzTextureWad TextureWad, int VersionNumber)
        {
            base.Read(reader, EffectWad, TextureWad, VersionNumber);

            MySpline = new Spline();
            MySpline.Read(reader, ParentObject);

            Initialize(2);

            NumVertices = reader.ReadInt32();
            N = reader.ReadInt32();
            InitializeArray(N);


            NumTriangles = reader.ReadInt32();
            Width = reader.ReadSingle();
            Resolution = reader.ReadSingle();

            for (int i = 0; i < NumVertices; i++)
                WriteReadTools.ReadVertex(reader, ref Vertices[i]);

            WriteReadTools.ReadColor(reader, ref MyColor);

            MyTexture = TextureWad.FindByName(reader.ReadString());
            MyEffect = EffectWad.FindByName(reader.ReadString());

#if EDITOR
            Init();
#endif

            Update();

            int ParentQuadInt = reader.ReadInt32();
            if (ParentQuadInt == -1)
                ParentObject.ParentQuad.AddQuadChild(this);
            else
                ((Quad)ParentObject.QuadList[ParentQuadInt]).AddQuadChild(this, true);
        }

#if EDITOR
        private void Init()
        {
            ParentPoint = new ObjectVector();
            ChildPoint = new ObjectVector();
            ReleasePoint = new ObjectVector();
            ParentPoint.ClickEventCallback = ClickOnParentButton;
            ChildPoint.ClickEventCallback = ClickOnChildButton;
            ReleasePoint.ClickEventCallback = ClickOnReleaseButton;
            SetToBeChild = false;
        }
#endif

        public BendableQuad(BendableQuad quad, bool DeepClone)
        {
            base.Clone(quad);

            Device = quad.Device; MySpline = new Spline(quad.MySpline, DeepClone); Initialize(quad.N);
#if EDITOR
            Init();
#endif
            Width = quad.Width;
            Resolution = quad.Resolution;

            for (int i = 0; i < NumVertices; i++)
                Vertices[i] = quad.Vertices[i];
        }
        public BendableQuad() { Name = "Bendable quad"; }
        public BendableQuad(GraphicsDevice device, int n, Spline spline)
        {
            Name = "Bendable quad";

            Device = device; MySpline = spline; Initialize(n);
#if EDITOR
            Init();
#endif
        }

        public override void FinishLoading(GraphicsDevice device, EzTextureWad TexWad, EzEffectWad EffectWad)
        {
            FinishLoading(device, TexWad, EffectWad, true);
        }
        public override void FinishLoading(GraphicsDevice device, EzTextureWad TexWad, EzEffectWad EffectWad, bool UseNames)
        {
            Device = device;

#if EDITOR
            WidthControl.ModifiedEventCallback = UpdateWidth;
            ResolutionControl.ModifiedEventCallback = UpdateResolution;

            ParentPoint.ClickEventCallback = ClickOnParentButton;
            ChildPoint.ClickEventCallback = ClickOnChildButton;
            ReleasePoint.ClickEventCallback = ClickOnReleaseButton;
#else
            if (Vertices == null || Vertices.Length != 2 * N)
                InitializeArray(N);
#endif

            if (UseNames)
            {
                MyTexture = TexWad.FindByName(MyTexture.Path);
                MyEffect = EffectWad.FindByName(MyEffect.Name);
            }
        }

#if EDITOR
        virtual public void ClickOnParentButton()
        {
            SetToBeParent = !SetToBeParent;
            Console.WriteLine("{0}", SetToBeParent);
            /*        if (SetToBeParent)
                    {
                        foreach (BaseQuad quad in ParentObject.QuadList)
                        {
                            if (quad.SetToBeChild && quad != this)
                                GlueQuadChild(quad);
                            quad.SetToBeChild = false;
                            if (quad is Quad)
                                ((Quad)quad).SetToBeParent = false;
                        }
                    } */
        }
#endif

        public override void Update()
        {
            if (!Show) return;

#if EDITOR
            ParentPoint.Pos = BL() + new Vector2(-.08f, -.03f);
            ChildPoint.Pos = BL() + new Vector2(-.08f, -.065f);
            ReleasePoint.Pos = BL() + new Vector2(-.08f, -.1f);
            if (SetToBeParent) ParentPoint.Pos += new Vector2(.025f, 0);
            if (SetToBeChild) ChildPoint.Pos += new Vector2(.025f, 0);
            if (ParentQuad != null && ParentQuad != ParentObject.ParentQuad) ReleasePoint.Pos += new Vector2(.025f, 0);

            //WidthControl.PosFromRelPos();
            //UpdateWidth(WidthControl.Pos);
            UpdateWidthControl();
            UpdateResolutionControl();
#endif

            MySpline.Update();

            DeformToSpline();
        }

#if EDITOR
        public void UpdateWidthControl()
        {
            Vector2 BL = CalcBLBound();
            float l = 1;
            if (ParentQuad != null) l = (ParentQuad.xAxis.Pos - ParentQuad.Center.Pos).Length();
            WidthControl.Pos = l * Width / .23f * WidthControlDir + BL;
            WidthControl.RelPosFromPos();
        }

        public void UpdateWidth(Vector2 NewPos)
        {
            Vector2 BL = CalcBLBound();
            float l = 1;
            if (ParentQuad != null) l = 1f / (ParentQuad.xAxis.Pos - ParentQuad.Center.Pos).Length();
            Width = Math.Max(.01f, l * .23f * Vector2.Dot(NewPos - BL, WidthControlDir));
            UpdateWidthControl();
        }

        public void UpdateResolutionControl()
        {
            Vector2 BL = CalcBLBound();
            ResolutionControl.Pos = Resolution / 500f * ResolutionControlDir + BL;
            ResolutionControl.RelPosFromPos();
        }

        public void UpdateResolution(Vector2 NewPos)
        {
            Vector2 BL = CalcBLBound();
            Resolution = Math.Max(10, 500f * Vector2.Dot(NewPos - BL, ResolutionControlDir));
            InitializeArray((int)Resolution);

            UpdateResolutionControl();
        }
#endif

#if EDITOR
        public override List<ObjectVector> GetObjectVectors()
        {
            List<ObjectVector> list = MySpline.GetObjectVectors();
            list.Add(WidthControl);
            list.Add(ResolutionControl);

            list.Add(ParentPoint);
            list.Add(ChildPoint);
            list.Add(ReleasePoint);

            return list;
        }
#endif

        public override bool HitTest(Vector2 x)
        {
            for (int j = 0; j < NumTriangles; j++)
            {
                bool sign = true;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 d = (Vertices[(i + 1) % 3 + j].xy - Vertices[i + j].xy);
                    Vector2 n = new Vector2(-d.Y, d.X);

                    float h = Vector2.Dot(n, x - Vertices[i + j].xy);
                    if (h > 0 && j % 2 == 0 || h < 0 && j % 2 == 1)
                        sign = false;
                }

                if (sign) return true;
            }

            return false;
        }

        public void InitializeArray(int n)
        {
            N = n;
            NumTriangles = 2 * (N - 1);
            NumVertices = 2 * N;
            Vertices = new MyOwnVertexFormat[NumVertices];

            float h = 1f / (N - 1);
            for (int i = 0; i < N; i++)
            {
                Vertices[2 * i].uv = new Vector2(i * h, 1);
                Vertices[2 * i + 1].uv = new Vector2(i * h, 0);
            }

            SetColor(MyColor);
        }

        public void RotateUV()
        {
            int j = 0;
            if (Vertices[1].uv == Vector2.Zero)
            {
                if (Vertices[0].uv.X == 0) j = 0;
                else j = 4;
            }
            else if (Vertices[0].uv == Vector2.Zero)
            {
                if (Vertices[1].uv.X == 0) j = 1;
                else j = 5;
            }
            else if (Vertices[2 * N - 1].uv == Vector2.Zero)
            {
                if (Vertices[2 * N - 2].uv.X == 0) j = 3;
                else j = 7;
            }
            else if (Vertices[2 * N - 2].uv == Vector2.Zero)
            {
                if (Vertices[2 * N - 1].uv.X == 0) j = 2;
                else j = 6;
            }
            j++;
            if (j > 7) j = 0;

            float h = 1f / (N - 1);
            for (int i = 0; i < N; i++)
            {
                switch (j)
                {
                    case 0:
                    case 4:
                        Vertices[2 * i].uv = new Vector2(i * h, 1);
                        Vertices[2 * i + 1].uv = new Vector2(i * h, 0);
                        break;
                    case 1:
                    case 5:
                        Vertices[2 * i + 1].uv = new Vector2(i * h, 1);
                        Vertices[2 * i].uv = new Vector2(i * h, 0);
                        break;
                    case 2:
                    case 6:
                        Vertices[2 * N - 1 - 2 * i].uv = new Vector2(i * h, 1);
                        Vertices[2 * N - 1 - (2 * i + 1)].uv = new Vector2(i * h, 0);
                        break;
                    case 3:
                    case 7:
                        Vertices[2 * N - 1 - (2 * i + 1)].uv = new Vector2(i * h, 1);
                        Vertices[2 * N - 1 - 2 * i].uv = new Vector2(i * h, 0);
                        break;
                }
            }
            if (j > 3)
            {
                Vector2 Hold;
                for (int i = 0; i < 2 * N; i++)
                {
                    Hold = Vertices[i].uv;
                    Vertices[i].uv.X = Hold.Y;
                    Vertices[i].uv.Y = Hold.X;
                }
            }
        }

        public void Initialize(int n)
        {
            MyColor = Color.White;

            InitializeArray(n);
#if EDITOR
            WidthControl = new ObjectVector();
            WidthControl.ModifiedEventCallback = UpdateWidth;
            WidthControl.Move(CalcBLBound() - new Vector2(.17f, .17f));

            ResolutionControl = new ObjectVector();
            ResolutionControl.ModifiedEventCallback = UpdateResolution;
            ResolutionControl.Move(CalcBLBound() - new Vector2(.07f, .07f));
#endif
        }

        public Vector2 GeoCenter()
        {
            Vector2 v = Vector2.Zero;
            int Count = 0;
            for (float t = 0; t < MySpline.Nodes; t += .5f)
            {
                v += GetVector(t, 0);
                Count++;
            }

            return v / Count;
        }

        /// <summary>
        /// Returns an absolute coordinate along the quad's spline.
        /// </summary>
        /// <param name="t">The distance along the spline</param>
        /// <param name="n">The distance along the normal to the spline</param>
        /// <returns></returns>
        public Vector2 GetVector(float t, float n)
        {
            temp1.ParentQuad = ParentQuad;
            temp1.RelPos = MySpline.GetVector(t, n);
            temp1.FastPosFromRelPos(ParentQuad);

            return temp1.Pos;
        }

        public void DeformToSpline()
        {
            temp1.ParentQuad = ParentQuad;
            temp2.ParentQuad = ParentQuad;

            Spline.UsePrev = false;
            float h = (float)(MySpline.Nodes - 1.001f) / (N - 1);
            if (ParentQuad != null)
                for (int i = 0; i < N; i++)
                {
                    MySpline.GetBothVectors(i * h, Width, ref temp1.RelPos, ref temp2.RelPos);
                    //temp1.PosFromRelPos();
                    temp1.FastPosFromRelPos(ParentQuad);
                    Vertices[2 * i].xy = temp1.Pos;
                    //temp2.PosFromRelPos();
                    temp2.FastPosFromRelPos(ParentQuad);
                    Vertices[2 * i + 1].xy = temp2.Pos;
                    /*
                                    temp1.RelPos = MySpline.GetVector(i * h, Width);
                                    temp1.PosFromRelPos();
                                    Vertices[2 * i].xy = temp1.Pos;

                                    temp1.RelPos = MySpline.GetVector(i * h, -Width);
                                    temp1.PosFromRelPos();
                                    Vertices[2 * i + 1].xy = temp1.Pos;
                    */
                }
        }

        public override void Draw(QuadDrawer QDrawer)
        {
            Draw();
        }

        public override void Draw()
        {
            if (!Show) return;

            if (!MyEffect.IsUpToDate)
                MyEffect.SetCameraParameters();

            MyEffect.xTexture.SetValue(MyTexture.Tex);

            MyEffect.effect.CurrentTechnique.Passes[0].Apply();
            
            Device.DrawUserPrimitives<MyOwnVertexFormat>(PrimitiveType.TriangleStrip, Vertices, 0, NumTriangles);
        }

        public Vector2 CalcBLBound()
        {
            Vector2 BL = Vector2.Min(MySpline.Control[0].Pos, MySpline.Node[0].Pos);
            for (int i = 1; i < MySpline.Nodes; i++)
                BL = Vector2.Min(BL, Vector2.Min(MySpline.Control[i].Pos, MySpline.Node[i].Pos));

            for (int i = 0; i < NumVertices; i++)
                BL = Vector2.Min(BL, Vertices[i].xy);

            return BL;
        }

        public Vector2 CalcTRBound()
        {
            Vector2 TR = Vector2.Max(MySpline.Control[0].Pos, MySpline.Node[0].Pos);
            for (int i = 1; i < MySpline.Nodes; i++)
                TR = Vector2.Max(TR, Vector2.Max(MySpline.Control[i].Pos, MySpline.Node[i].Pos));

            for (int i = 0; i < NumVertices; i++)
                TR = Vector2.Max(TR, Vertices[i].xy);

            return TR;
        }

#if EDITOR
        public override void DrawExtra(QuadDrawer Drawer, bool Additional, float ScaleLines)
        {
            if (!Show) return;

            Vector2 BL = CalcBLBound();
            Vector2 TR = CalcTRBound();

            if (Additional)
                if (ParentQuad != null)
                {
                    Drawer.DrawLine(BL, ParentQuad.CalcBLBound(), new Color(192, 192, 192, 50), .05f * ScaleLines);
                    Drawer.DrawLine(TR, ParentQuad.CalcTRBound(), new Color(192, 192, 192, 50), .05f * ScaleLines);
                }

            MySpline.Draw(Drawer, ScaleLines);

            if (Additional)
            {
                Drawer.DrawLine(BL, WidthControl.Pos, new Color(.6f, .6f, .2f), .015f);
                Drawer.DrawLine(BL, ResolutionControl.Pos, new Color(.2f, .6f, .6f), .015f);

                float H = TR.Y - BL.Y;
                float W = TR.X - BL.X;

                Drawer.DrawLine(BL, BL + new Vector2(W / 2, 0), Color.PapayaWhip, .012f);
                Drawer.DrawLine(BL, BL + new Vector2(0, H / 2), Color.PapayaWhip, .012f);
                Drawer.DrawLine(TR, TR + new Vector2(-W / 2, 0), Color.PapayaWhip, .012f);
                Drawer.DrawLine(TR, TR + new Vector2(0, -H / 2), Color.PapayaWhip, .012f);

                Drawer.DrawLine(BL + new Vector2(-.08f, -.03f) + new Vector2(-.01f, 0), BL + new Vector2(-.08f, -.03f) + new Vector2(.035f, 0), Color.SeaGreen, .015f);
                Drawer.DrawSquareDot(ParentPoint.Pos, Color.Sienna, .03f);
                Drawer.DrawLine(BL + new Vector2(-.08f, -.065f) + new Vector2(-.01f, 0), BL + new Vector2(-.08f, -.065f) + new Vector2(.035f, 0), Color.SeaGreen, .015f);
                Drawer.DrawSquareDot(ChildPoint.Pos, Color.SeaShell, .03f);
                Drawer.DrawLine(BL + new Vector2(-.08f, -.1f) + new Vector2(-.01f, 0), BL + new Vector2(-.08f, -.1f) + new Vector2(.035f, 0), Color.SeaGreen, .015f);
                Drawer.DrawSquareDot(ReleasePoint.Pos, Color.SeaShell, .03f);
            }
        }
#endif
    }
}