using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.IO;

using CloudberryKingdom;

namespace Drawing
{
    public class Quad : BaseQuad
    {
        public ObjectVector Center, xAxis, yAxis;
        public ObjectVector[] Corner;

        public List<BaseQuad> Children;

        public override void Release()
        {
            base.Release();

            Center.Release(); Center = null;
            xAxis.Release(); xAxis = null;
            yAxis.Release(); yAxis = null;
            for (int i = 0; i < 4; i++)
                Corner[i].Release();
            Corner = null;
        }

        public void Resize()
        {
            float width = (Corner[1].RelPos - Corner[0].RelPos).Length();
            Vector2 NewSize = new Vector2(MyTexture.Tex.Width, MyTexture.Tex.Height);
            NewSize *= .5f * width / MyTexture.Tex.Width;
            ScaleCorners(NewSize);
        }

        public void MirrorUV_Horizontal()
        {
            Vector2 hold;

            hold = Vertices[0].uv;
            Vertices[0].uv = Vertices[1].uv;
            Vertices[1].uv = hold;

            hold = Vertices[2].uv;
            Vertices[2].uv = Vertices[3].uv;
            Vertices[3].uv = hold;
        }

        public override void CopyAnim(BaseQuad basequad, int Anim)
        {
            Quad quad = basequad as Quad;
            if (null != quad)
            {
                Center.CopyAnim(quad.Center, Anim);
                xAxis.CopyAnim(quad.xAxis, Anim);
                yAxis.CopyAnim(quad.yAxis, Anim);
                for (int i = 0; i < 4; i++)
                    Corner[i].CopyAnim(quad.Corner[i], Anim);
            }
        }

        public override void CopyAnimShallow(BaseQuad basequad, int Anim)
        {
            Quad quad = basequad as Quad;
            if (null != quad)
            {
                Center.AnimData = quad.Center.AnimData;
                xAxis.AnimData = quad.xAxis.AnimData;
                yAxis.AnimData = quad.yAxis.AnimData;
                for (int i = 0; i < 4; i++)
                    Corner[i].AnimData = quad.Corner[i].AnimData;
            }
        }

        public override void SetHold()
        {
            Center.AnimData.Hold = Center.RelPos;
            xAxis.AnimData.Hold = xAxis.RelPos;
            yAxis.AnimData.Hold = yAxis.RelPos;
            for (int i = 0; i < 4; i++)
                Corner[i].AnimData.Hold = Corner[i].RelPos;
        }

        public override void ReadAnim(int anim, int frame)
        {
            Center.RelPos = Center.AnimData.Get(anim, frame);
            xAxis.RelPos = xAxis.AnimData.Get(anim, frame);
            yAxis.RelPos = yAxis.AnimData.Get(anim, frame);
            for (int i = 0; i < 4; i++)
                Corner[i].RelPos = Corner[i].AnimData.Get(anim, frame);

            if (TextureIsAnimated && UpdateSpriteAnim && anim < TextureAnim.Anims.Length && TextureAnim.Anims[anim].Data != null)
            {
                var data = TextureAnim.Calc(anim, frame);
                MyTexture = data;

                Vertices[0].uv = new Vector2(0, 0);
                Vertices[1].uv = new Vector2(1, 0);
                Vertices[2].uv = new Vector2(0, 1);
                Vertices[3].uv = new Vector2(1, 1);
            }
        }

        public override void Record(int anim, int frame, bool UseRelativeCoords)
        {
            if (UseRelativeCoords)
            {
                Center.AnimData.Set(Center.RelPos, anim, frame);
                xAxis.AnimData.Set(xAxis.RelPos, anim, frame);
                yAxis.AnimData.Set(yAxis.RelPos, anim, frame);
                for (int i = 0; i < 4; i++)
                    Corner[i].AnimData.Set(Corner[i].RelPos, anim, frame);
            }
            else
            {
                Center.AnimData.Set(Center.Pos, anim, frame);
                xAxis.AnimData.Set(xAxis.Pos, anim, frame);
                yAxis.AnimData.Set(yAxis.Pos, anim, frame);
                for (int i = 0; i < 4; i++)
                    Corner[i].AnimData.Set(Corner[i].Pos, anim, frame);
            }
        }

        void ModifyAxis(ObjectVector axis, int anim, int frame, ChangeMode RecordMode)
        {
            Vector2 Change_axis = CoreMath.CartesianToPolar(axis.RelPos) - CoreMath.CartesianToPolar(axis.AnimData.Get(anim, frame));
            Change_axis.Y = 1 + Change_axis.Y / axis.AnimData.Get(anim, frame).Length();

            for (int _anim = 0; _anim < axis.AnimData.Anims.Length; _anim++)
            {
                if (RecordMode == ChangeMode.SingleAnim && _anim != anim) continue;

                if (axis.AnimData.Anims[_anim].Data != null)
                    for (int _frame = 0; _frame < axis.AnimData.Anims[_anim].Data.Length; _frame++)
                        if (anim != _anim || frame != _frame)
                        {
                            Vector2 polar = CoreMath.CartesianToPolar(axis.AnimData.Anims[_anim].Data[_frame]);
                            polar.X += Change_axis.X;
                            polar.Y *= Change_axis.Y;
                            axis.AnimData.Anims[_anim].Data[_frame] = CoreMath.PolarToCartesian(polar);
                        }
            }
        }

        Vector2 ToAxisCoordinates(Vector2 v, Vector2 xAxisPos, Vector2 yAxisPos)
        {
            return new Vector2(Vector2.Dot(v, xAxisPos) / xAxisPos.Length(), Vector2.Dot(v, yAxisPos) / yAxisPos.Length());
        }
        Vector2 FromAxisCoordinates(Vector2 v, Vector2 xAxisPos, Vector2 yAxisPos)
        {
            return v.X * xAxisPos / xAxisPos.Length() + v.Y * yAxisPos / yAxisPos.Length();
        }

        void ModifyPoint(ObjectVector point, int anim, int frame, ChangeMode RecordMode)
        {
            Vector2 Change_point = ToAxisCoordinates(point.RelPos - point.AnimData.Get(anim, frame), xAxis.RelPos, yAxis.RelPos);

            for (int _anim = 0; _anim < point.AnimData.Anims.Length; _anim++)
            {
                if (RecordMode == ChangeMode.SingleAnim && _anim != anim) continue;

                if (point.AnimData.Anims[_anim].Data != null)
                    for (int _frame = 0; _frame < point.AnimData.Anims[_anim].Data.Length; _frame++)
                        if (anim != _anim || frame != _frame)
                        {
                            Vector2 v = ToAxisCoordinates(point.AnimData.Get(_anim, _frame),
                                                          xAxis.AnimData.Get(_anim, _frame),
                                                          yAxis.AnimData.Get(_anim, _frame));
                            v += Change_point;
                            point.AnimData.Anims[_anim].Data[_frame] = FromAxisCoordinates(v,
                                                                                           xAxis.AnimData.Get(_anim, _frame),
                                                                                           yAxis.AnimData.Get(_anim, _frame));
                        }
            }
        }

        public void ModifyAllRecords(int anim, int frame, ChangeMode RecordMode)
        {
            for (int i = 0; i < 4; i++)
                ModifyPoint(Corner[i], anim, frame, RecordMode);

            ModifyAxis(xAxis, anim, frame, RecordMode);
            ModifyAxis(yAxis, anim, frame, RecordMode);

            ModifyPoint(Center, anim, frame, RecordMode);
        }

        public void ShowChildren()
        {
            foreach (BaseQuad child in Children)
                child.Show = true;
        }

        public void HideChildren()
        {
            foreach (BaseQuad child in Children)
                child.Show = false;
        }

        public override void Transfer(int anim, float DestT, int AnimLength, bool Loop, bool Linear, float t)
        {
            Center.RelPos = Center.AnimData.Transfer(anim, DestT, AnimLength, Loop, Linear, t);
            xAxis.RelPos = xAxis.AnimData.Transfer(anim, DestT, AnimLength, Loop, Linear, t);
            yAxis.RelPos = yAxis.AnimData.Transfer(anim, DestT, AnimLength, Loop, Linear, t);
            for (int i = 0; i < 4; i++)
                Corner[i].RelPos = Corner[i].AnimData.Transfer(anim, DestT, AnimLength, Loop, Linear, t);
        }

        public override void Calc(int anim, float t, int AnimLength, bool Loop, bool Linear)
        {
            if (!Show && Children.Count == 0) return;

            if (TextureIsAnimated && UpdateSpriteAnim)
            {
                t = t + .5f;
                var data = TextureAnim.Calc(anim, t, AnimLength, Loop);
                MyTexture = data;

                Vertices[0].uv = new Vector2(0, 0);
                Vertices[1].uv = new Vector2(1, 0);
                Vertices[2].uv = new Vector2(0, 1);
                Vertices[3].uv = new Vector2(1, 1);

                int frame = (int)Math.Floor(t);
                Center.RelPos = Center.AnimData.Get(anim, frame);
                xAxis.RelPos = xAxis.AnimData.Get(anim, frame);
                yAxis.RelPos = yAxis.AnimData.Get(anim, frame);
                for (int i = 0; i < 4; i++)
                    Corner[i].RelPos = Corner[i].AnimData.Get(anim, frame);
            }
            else
            {
                Center.RelPos = Center.AnimData.Calc(anim, t, AnimLength, Loop, Linear);
                xAxis.RelPos = xAxis.AnimData.CalcAxis(anim, t, AnimLength, Loop, Linear);
                yAxis.RelPos = yAxis.AnimData.CalcAxis(anim, t, AnimLength, Loop, Linear);
                for (int i = 0; i < 4; i++)
                    Corner[i].RelPos = Corner[i].AnimData.Calc(anim, t, AnimLength, Loop, Linear);
            }
        }


        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            if (ParentQuad == null || ParentQuad == ParentObject.ParentQuad)
                writer.Write(-1);
            else
                writer.Write(ParentObject.QuadList.IndexOf(ParentQuad));

            Center.Write(writer, ParentObject);
            xAxis.Write(writer, ParentObject);
            yAxis.Write(writer, ParentObject);

            for (int i = 0; i < NumVertices; i++)
                Corner[i].Write(writer, ParentObject);

            for (int i = 0; i < NumVertices; i++)
                WriteReadTools.WriteVertex(writer, Vertices[i]);

            WriteReadTools.WriteColor(writer, MyColor);

            writer.Write(MyTexture.Path);
            writer.Write(MyEffect.Name);


        }

        public override void Read(BinaryReader reader, EzEffectWad EffectWad, EzTextureWad TextureWad, int VersionNumber)
        {
            base.Read(reader, EffectWad, TextureWad, VersionNumber);

            int ParentQuadInt = reader.ReadInt32();
            if (ParentQuadInt == -1)
                ParentQuad = ParentObject.ParentQuad;
            else
                ((Quad)ParentObject.QuadList[ParentQuadInt]).AddQuadChild(this);

            Center.Read(reader, ParentObject);
            xAxis.Read(reader, ParentObject);
            yAxis.Read(reader, ParentObject);

            for (int i = 0; i < 4; i++)
                Corner[i].Read(reader, ParentObject);

            for (int i = 0; i < NumVertices; i++)
                WriteReadTools.ReadVertex(reader, ref Vertices[i]);

            WriteReadTools.ReadColor(reader, ref MyColor);

            string name = reader.ReadString();
            MyTexture = TextureWad.FindByPathOrName(name);//reader.ReadString());
            MyEffect = EffectWad.FindByName(reader.ReadString());
        }

#if EDITOR
        public override void SaveState(int StateIndex)
        {
            base.SaveState(StateIndex);

            for (int i = 0; i < 4; i++)
                Corner[i].SaveState(StateIndex);

            Center.SaveState(StateIndex);
            xAxis.SaveState(StateIndex);
            yAxis.SaveState(StateIndex);
        }

        public override void RecoverState(int StateIndex)
        {
            base.RecoverState(StateIndex);

            for (int i = 0; i < 4; i++)
                Corner[i].RecoverState(StateIndex);

            Center.RecoverState(StateIndex);
            xAxis.RecoverState(StateIndex);
            yAxis.RecoverState(StateIndex);
        }
#endif
        public List<BaseQuad> GetAllChildren()
        {
            List<BaseQuad> list = new List<BaseQuad>();

            foreach (BaseQuad quad in Children)
            {
                list.Add(quad);
                if (quad is Quad)
                    list.AddRange(((Quad)quad).GetAllChildren());
            }

            return list;
        }

        public override bool HitTest(Vector2 x)
        {
            bool SameSign = true;
            float sign = 0;
            for (int i = 0; i < 3; i++)
            {
                Vector2 d = (Vertices[(i + 1) % 3].xy - Vertices[i].xy);
                Vector2 n = new Vector2(-d.Y, d.X);

                float hold = Vector2.Dot(n, x - Vertices[i].xy);
                if (sign != 0 && Math.Sign(sign) != Math.Sign(hold))
                    SameSign = false;
                sign = hold;
            }

            if (SameSign) return true;
            else SameSign = true;

            sign = 0;
            for (int i = 0; i < 3; i++)
            {
                Vector2 d = (Vertices[(i + 1) % 3 + 1].xy - Vertices[i + 1].xy);
                Vector2 n = new Vector2(-d.Y, d.X);

                float hold = Vector2.Dot(n, x - Vertices[i + 1].xy);
                if (sign != 0 && Math.Sign(sign) != Math.Sign(hold))
                    SameSign = false;
                sign = hold;
            }

            return SameSign;
        }

#if EDITOR
        public override List<ObjectVector> GetObjectVectors()
        {
            List<ObjectVector> ObjectVectorList = new List<ObjectVector>();

            for (int i = 0; i < 4; i++)
                ObjectVectorList.Add(Corner[i]);

            ObjectVectorList.Add(Center);
            ObjectVectorList.Add(xAxis);
            ObjectVectorList.Add(yAxis);

            ObjectVectorList.Add(ParentPoint);
            ObjectVectorList.Add(ChildPoint);
            ObjectVectorList.Add(ReleasePoint);

            return ObjectVectorList;
        }
#endif

        public void RemoveQuadChild(BaseQuad child) { RemoveQuadChild(child, true); }
        public void RemoveQuadChild(BaseQuad child, bool AddToRoot)
        {
            if (Children.Contains(child))
            {
                if (child is Quad)
                {
                    Quad child_quad = (Quad)child;
                    child_quad.Center.ParentQuad = null; child_quad.Center.RelPosFromPos();
                    child_quad.xAxis.ParentQuad = null; child_quad.xAxis.RelPosFromPos();
                    child_quad.yAxis.ParentQuad = null; child_quad.yAxis.RelPosFromPos();
                }

                if (child.ParentQuad == ParentObject.ParentQuad)
                    child.ParentQuad = null;
                else
                {
                    child.ParentQuad = null;
                    if (AddToRoot)
                        ParentObject.ParentQuad.AddQuadChild(child);
                }

                Children.Remove(child);
            }
        }


        public void AddQuadChild(BaseQuad child, bool KeepNumericData)
        {
            if (this == child)
                return;

            if (child.ParentQuad != null)
                child.ParentQuad.RemoveQuadChild(child, false);

            if (child is Quad)
            {
                Quad child_quad = (Quad)child;

                if (child_quad.GetAllChildren().Contains(this))
                    return;

                child_quad.Center.ParentQuad = this; if (!KeepNumericData) child_quad.Center.RelPosFromPos();
                child_quad.xAxis.ParentQuad = this; if (!KeepNumericData) child_quad.xAxis.RelPosFromPos();
                child_quad.yAxis.ParentQuad = this; if (!KeepNumericData) child_quad.yAxis.RelPosFromPos();
            }

            child.ParentQuad = this;

            Children.Add(child);
        }

        public void AddQuadChild(BaseQuad child)
        {
            AddQuadChild(child, false);
        }

#if EDITOR
        virtual public void ClickOnParentButton()
        {
            SetToBeParent = !SetToBeParent;

            if (SetToBeParent)
            {
                foreach (BaseQuad quad in ParentObject.QuadList)
                {
                    if (quad.SetToBeChild && quad != this)
                        AddQuadChild(quad);
                    quad.SetToBeChild = false;
                    if (quad is Quad)
                        ((Quad)quad).SetToBeParent = false;
                }
            }
        }
#endif

        public override void FinishLoading(GraphicsDevice device, EzTextureWad TexWad, EzEffectWad EffectWad)
        {
            FinishLoading(device, TexWad, EffectWad, true);
        }
        public override void FinishLoading(GraphicsDevice device, EzTextureWad TexWad, EzEffectWad EffectWad, bool UseNames)
        {
            Center.ModifiedEventCallback = UpdateCenter;
            xAxis.ModifiedEventCallback = UpdatexAxis;
            yAxis.ModifiedEventCallback = UpdateyAxis;

#if EDITOR
            ParentPoint.ClickEventCallback = ClickOnParentButton;
            ChildPoint.ClickEventCallback = ClickOnChildButton;
            ReleasePoint.ClickEventCallback = ClickOnReleaseButton;
#endif

            if (UseNames)
            {
                if (MyTexture.Name != null)
                    MyTexture = TexWad.FindByPathOrName(MyTexture.Path);
                MyEffect = EffectWad.FindByName(MyEffect.Name);
            }
        }

        private void InitVertices()
        {
            Children = new List<BaseQuad>();

            NumVertices = 4;

            Vertices = new MyOwnVertexFormat[4];

            Vertices[0].uv = new Vector2(0, 0);
            Vertices[1].uv = new Vector2(1, 0);
            Vertices[2].uv = new Vector2(0, 1);
            Vertices[3].uv = new Vector2(1, 1);

            Center = new ObjectVector();
            Center.ModifiedEventCallback = UpdateCenter;
            xAxis = new ObjectVector();
            yAxis = new ObjectVector();
            xAxis.CenterPoint = Center;
            yAxis.CenterPoint = Center;
            xAxis.Move(new Vector2(1f, 0));
            yAxis.Move(new Vector2(0, 1f));
            xAxis.ModifiedEventCallback = UpdatexAxis;
            yAxis.ModifiedEventCallback = UpdateyAxis;

#if EDITOR
            ParentPoint = new ObjectVector();
            ChildPoint = new ObjectVector();
            ReleasePoint = new ObjectVector();
            ParentPoint.ParentQuad = this;
            ChildPoint.ParentQuad = this;
            ParentPoint.ClickEventCallback = ClickOnParentButton;
            ChildPoint.ClickEventCallback = ClickOnChildButton;
            ReleasePoint.ClickEventCallback = ClickOnReleaseButton;
            SetToBeParent = SetToBeChild = false;
#endif

            Corner = new ObjectVector[4];
            for (int i = 0; i < 4; i++)
            {
                Corner[i] = new ObjectVector();
                Corner[i].ParentQuad = this;
            }

            Corner[0].Move(new Vector2(-1, 1));
            Corner[1].Move(new Vector2(1, 1));
            Corner[2].Move(new Vector2(-1, -1));
            Corner[3].Move(new Vector2(1, -1));
        }

        public void ScaleCorners(Vector2 size)
        {
            Corner[0].RelPos = new Vector2(-size.X, size.Y);
            Corner[1].RelPos = new Vector2(size.X, size.Y);
            Corner[2].RelPos = new Vector2(-size.X, -size.Y);
            Corner[3].RelPos = new Vector2(size.X, -size.Y);
        }

        public Quad(Quad quad, bool DeepClone)
        {
            base.Clone(quad);

            if (DeepClone && quad.TextureAnim != null)
                TextureAnim = new AnimationData_Texture(quad.TextureAnim);
            else
                TextureAnim = quad.TextureAnim;

            InitVertices();

            quad.Center.Clone(Center, DeepClone);
            quad.xAxis.Clone(xAxis, DeepClone);
            quad.yAxis.Clone(yAxis, DeepClone);
            for (int i = 0; i < 4; i++)
                quad.Corner[i].Clone(Corner[i], DeepClone);

            for (int i = 0; i < NumVertices; i++)
                Vertices[i] = quad.Vertices[i];
        }

        public Quad()
        {
            Name = "Quad";

            InitVertices();

            SetColor(new Color(1f, 1f, 1f));
        }


        public void UpdateCenter(Vector2 NewPos)
        {
            Center.Pos = NewPos;
            Center.RelPosFromPos();
        }

        public Vector2 Size
        {
            get { return new Vector2(xAxis.RelPos.Length(), yAxis.RelPos.Length()); }
            set
            {
                xAxis.RelPos.Normalize();
                yAxis.RelPos.Normalize();
                Scale(value);
            }
        }

        public void Scale(Vector2 Stretch)
        {
            xAxis.RelPos *= Stretch.X;
            yAxis.RelPos *= Stretch.Y;
        }

        public void PointxAxisTo(Vector2 dir)
        {
            dir.Normalize();

            float l = xAxis.RelPos.Length();
            xAxis.RelPos = dir * l;
            xAxis.PosFromRelPos();

            l = yAxis.RelPos.Length();
            yAxis.RelPos.X = -xAxis.RelPos.Y;
            yAxis.RelPos.Y = xAxis.RelPos.X;
            yAxis.RelPos.Normalize();
            yAxis.RelPos *= l;
            yAxis.PosFromRelPos();
        }

        public void UpdatexAxis(Vector2 NewPos)
        {
            float l = (NewPos - Center.Pos).Length();
            Vector2 axis = NewPos - Center.Pos;
            axis.Normalize();
            xAxis.Pos = Math.Max(l, .0125f) * axis + Center.Pos;
            //float l = Vector2.Dot(NewPos - Center.Pos, axis);
            //if (l > .0125f)
            {
                //xAxis.Pos = l * axis + Center.Pos;
                //xAxis.Pos = NewPos;
                //yAxis.Pos = yAxis.Pos - (xAxis.Pos - Center.Pos) * Vector2.Dot(yAxis.Pos - Center.Pos, xAxis.Pos - Center.Pos) / (xAxis.Pos - Center.Pos).LengthSquared();
                yAxis.Pos.X = -(NewPos.Y - Center.Pos.Y); yAxis.Pos.Y = NewPos.X - Center.Pos.X;
                yAxis.Pos += Center.Pos;

                xAxis.RelPosFromPos();
                yAxis.RelPosFromPos();
            }
        }

        public void UpdateyAxis(Vector2 NewPos)
        {
            float L = Vector2.Distance(yAxis.Pos, Center.Pos);//yAxis.RelPos.LengthSquared();
            if (true)//(NewPos - Center.Pos).LengthSquared() > .001f)
            {
                yAxis.Pos = NewPos - Center.Pos;
                yAxis.Pos.Normalize();
                yAxis.Pos *= L;
                yAxis.Pos += Center.Pos;

                xAxis.Pos = xAxis.Pos - (yAxis.Pos - Center.Pos) * Vector2.Dot(xAxis.Pos - Center.Pos, yAxis.Pos - Center.Pos) / (yAxis.Pos - Center.Pos).LengthSquared();
                xAxis.Pos = xAxis.Pos - Center.Pos;
                xAxis.Pos.Normalize();
                xAxis.Pos *= L;
                xAxis.Pos += Center.Pos;

                xAxis.RelPosFromPos();
                yAxis.RelPosFromPos();
            }
        }

        public Vector2 CalcBLBound()
        {
            Vector2 BL = Center.Pos;
            for (int i = 0; i < NumVertices; i++)
                BL = Vector2.Min(BL, Vertices[i].xy);

            return BL;
        }

        public Vector2 CalcTRBound()
        {
            Vector2 TR = Center.Pos;
            for (int i = 0; i < NumVertices; i++)
                TR = Vector2.Max(TR, Vertices[i].xy);

            return TR;
        }

        public override void Update(float Expand)
        {
            if (!Show && Children.Count == 0) return;

            Center.PosFromRelPos();
            xAxis.PosFromRelPos();
            yAxis.PosFromRelPos();

#if EDITOR
            Vector2 BL = CalcBLBound();

            ParentPoint.Pos = BL + new Vector2(-.08f, -.03f);
            ChildPoint.Pos = BL + new Vector2(-.08f, -.065f);
            ReleasePoint.Pos = BL + new Vector2(-.08f, -.1f);
            if (SetToBeParent) ParentPoint.Pos += new Vector2(.025f, 0);
            if (SetToBeChild) ChildPoint.Pos += new Vector2(.025f, 0);
            if (ParentQuad != null && ParentQuad != ParentObject.ParentQuad) ReleasePoint.Pos += new Vector2(.025f, 0);
#endif

            for (int i = 0; i < 4; i++)
            {
                Vector2 HoldRelPos = Corner[i].RelPos;
                Corner[i].RelPos *= Expand;
                Corner[i].PosFromRelPos();
                Corner[i].RelPos = HoldRelPos;

                Vertices[i].xy = Corner[i].Pos;
            }
        }

        public override void Set_PosFromRelPos(ObjectVector v)
        {
            Vector2 C1 = Vector2.Zero;
            Vector2 C2 = Vector2.Zero;
            if (v.CenterPoint != null) C1 = v.CenterPoint.Pos;

            C2 = Center.Pos;
            if (v.CenterPoint == null) C1 = Center.Pos;

            v.Pos = C1 + v.RelPos.X * (xAxis.Pos - C2) + v.RelPos.Y * (yAxis.Pos - C2);
        }

        public override void Set_RelPosFromPos(ObjectVector v)
        {
            Vector2 C = new Vector2(0, 0);
            if (v.CenterPoint != null) C = v.CenterPoint.Pos;

            if (v.CenterPoint == null) C = Center.Pos;

            Vector2 Dif = v.Pos - C;
            Vector2 axis = xAxis.Pos - Center.Pos;
            v.RelPos.X = Vector2.Dot(Dif, axis) / axis.LengthSquared();
            axis = yAxis.Pos - Center.Pos;
            v.RelPos.Y = Vector2.Dot(Dif, axis) / axis.LengthSquared();
        }

        public override void Draw()
        {
            Draw(Tools.QDrawer);
        }


        override public void Draw(QuadDrawer Drawer)
        {
            if (!Show) return;

            Drawer.DrawQuad(this);
        }

#if EDITOR
        public void DrawChildren(QuadDrawer Drawer)
        {
            foreach (BaseQuad child_quad in GetAllChildren())
            {
                child_quad.ColoredDraw(Drawer, Color.PowderBlue);
            }
        }

        public override void DrawExtra(QuadDrawer Drawer, bool Additional, float ScaleLines)
        {
            if (!Show) return;

            foreach (ObjectVector point in GetObjectVectors())
            {
                if (point.ParentQuad is BendableQuad)
                {
                    Vector2 SplineLoc = ((BendableQuad)point.ParentQuad).MySpline.GetVector(point.RelPos.X, 0);
                    Drawer.DrawSquareDot(SplineLoc, new Color(.2f, .8f, .9f), .03f * ScaleLines);
                }
            }

            if (Additional)
                if (ParentQuad != null)
                {
                    Drawer.DrawLine(Corner[0].Pos, ParentQuad.Corner[0].Pos, new Color(192, 192, 192, 50), .05f * ScaleLines);
                    Drawer.DrawLine(Corner[3].Pos, ParentQuad.Corner[3].Pos, new Color(192, 192, 192, 50), .05f * ScaleLines);
                }

            Drawer.DrawLine(Center.Pos, xAxis.Pos, new Color(.2f, .6f, .2f), .015f * ScaleLines);
            Drawer.DrawLine(Center.Pos, yAxis.Pos, new Color(.2f, .6f, .2f), .015f * ScaleLines);
            Drawer.DrawSquareDot(Center.Pos, new Color(.4f, .8f, .4f), .033f * ScaleLines);
            Drawer.DrawSquareDot(xAxis.Pos, new Color(.8f, .4f, .4f), .033f * ScaleLines);
            Drawer.DrawSquareDot(yAxis.Pos, new Color(.4f, .4f, .8f), .033f * ScaleLines);

            Vector2 BL = CalcBLBound();
            Vector2 TR = CalcTRBound();

            if (Additional)
            {
                Drawer.DrawLine(BL + new Vector2(-.08f, -.03f) + new Vector2(-.01f, 0), BL + new Vector2(-.08f, -.03f) + new Vector2(.035f, 0), Color.SeaGreen, .015f * ScaleLines);
                Drawer.DrawSquareDot(ParentPoint.Pos, Color.Sienna, .03f * ScaleLines);
                Drawer.DrawLine(BL + new Vector2(-.08f, -.065f) + new Vector2(-.01f, 0), BL + new Vector2(-.08f, -.065f) + new Vector2(.035f, 0), Color.SeaGreen, .015f * ScaleLines);
                Drawer.DrawSquareDot(ChildPoint.Pos, Color.SeaShell, .03f * ScaleLines);
                Drawer.DrawLine(BL + new Vector2(-.08f, -.1f) + new Vector2(-.01f, 0), BL + new Vector2(-.08f, -.1f) + new Vector2(.035f, 0), Color.SeaGreen, .015f * ScaleLines);
                Drawer.DrawSquareDot(ReleasePoint.Pos, Color.SeaShell, .03f * ScaleLines);
            }

            for (int i = 0; i < 4; i++)
                Drawer.DrawSquareDot(Corner[i].Pos, new Color(.8f, .4f, .4f), .035f * ScaleLines);

            if (Additional)
            {
                float H = TR.Y - BL.Y;
                float W = TR.X - BL.X;

                Drawer.DrawLine(BL, BL + new Vector2(W / 2, 0), Color.PapayaWhip, .006f * ScaleLines);
                Drawer.DrawLine(BL, BL + new Vector2(0, H / 2), Color.PapayaWhip, .006f * ScaleLines);
                Drawer.DrawLine(TR, TR + new Vector2(-W / 2, 0), Color.PapayaWhip, .006f * ScaleLines);
                Drawer.DrawLine(TR, TR + new Vector2(0, -H / 2), Color.PapayaWhip, .006f * ScaleLines);
            }
        }
#endif
    }
}