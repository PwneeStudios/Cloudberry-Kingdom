using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using CloudberryKingdom;

namespace Drawing
{
    public struct SimpleVector : IReadWrite
    {
        public AnimationData AnimData;
        public MyOwnVertexFormat Vertex;
        public Vector2 Pos;

        public void RotateRight()
        {
            Tools.Swap(ref Pos.X, ref Pos.Y);
            Tools.Swap(ref Vertex.uv.X, ref Vertex.uv.X);
        }

        public void RotateLeft()
        {
            RotateRight();
            Pos.X *= -1;
            Vertex.uv.X *= -1;
        }

        public void Release()
        {
            AnimData.Release();
        }

        public void Write(StreamWriter writer)
        {
            Tools.WriteFields(this, writer, "Pos");
        }
        public void Read(StreamReader reader)
        {
            this = (SimpleVector)Tools.ReadFields(this, reader);
        }
    }

    public class SimpleBox
    {
        public bool Animated;
        public SimpleVector BL, TR;

        public void Release()
        {
            BL.Release();
            TR.Release();
        }

        public void SetHold()
        {
            BL.AnimData.Hold = BL.Pos;
            TR.AnimData.Hold = TR.Pos;
        }

        public void ReadAnim(int anim, int frame)
        {
            BL.Pos = BL.AnimData.Get(anim, frame);
            TR.Pos = TR.AnimData.Get(anim, frame);
        }

        public void Record(int anim, int frame)
        {
            BL.AnimData.Set(BL.Pos, anim, frame);
            TR.AnimData.Set(TR.Pos, anim, frame);
        }

        public void Transfer(int anim, float DestT, int AnimLength, bool Loop, bool Linear, float t)
        {
            BL.Pos = BL.AnimData.Transfer(anim, DestT, AnimLength, Loop, Linear, t);
            TR.Pos = TR.AnimData.Transfer(anim, DestT, AnimLength, Loop, Linear, t);
        }

        public void Calc(int anim, float t, int AnimLength, bool Loop, bool Linear)
        {
            BL.Pos = BL.AnimData.Calc(anim, t, AnimLength, Loop, Linear);
            TR.Pos = TR.AnimData.Calc(anim, t, AnimLength, Loop, Linear);
        }

        public SimpleBox(SimpleBox box)
        {
            Animated = box.Animated;

            BL = box.BL;
            TR = box.TR;
        }

        public SimpleBox(ObjectBox box)
        {
            Animated = true;

            BL.Pos = box.BL.Pos;
            TR.Pos = box.TR.Pos;

            BL.AnimData = box.BL.AnimData;
            TR.AnimData = box.TR.AnimData;
        }

        /// <summary>
        /// Copy and shift the source boxes's vertex locations.
        /// </summary>
        /// <param name="SourceBox">The source box</param>
        /// <param name="shift">The amount to shift</param>
        public void CopyUpdate(ref SimpleBox SourceBox, ref Vector2 shift)
        {
            BL.Vertex.xy = SourceBox.BL.Vertex.xy + shift;
            TR.Vertex.xy = SourceBox.TR.Vertex.xy + shift;
        }

        public void Update(ref BasePoint Base)
        {
            BL.Vertex.xy = Base.Origin + BL.Pos.X * Base.e1 + BL.Pos.Y * Base.e2;
            TR.Vertex.xy = Base.Origin + TR.Pos.X * Base.e1 + TR.Pos.Y * Base.e2;
        }

        public Vector2 Center()
        {
            return (TR.Vertex.xy + BL.Vertex.xy) / 2;
        }

        public float Width(ref BasePoint Base)
        {
            return (TR.Pos.X - BL.Pos.X) * Base.e1.Length();
        }

        public Vector2 Size()
        {
            return (TR.Vertex.xy - BL.Vertex.xy);
        }

        /*        public void DrawExtra(QuadDrawer Drawer, Color clr)
                {
                    Drawer.DrawLine(BL.Pos, new Vector2(TR.Pos.X, BL.Pos.Y), clr, .02f);
                    Drawer.DrawLine(BL.Pos, new Vector2(BL.Pos.X, TR.Pos.Y), clr, .02f);
                    Drawer.DrawLine(TR.Pos, new Vector2(TR.Pos.X, BL.Pos.Y), clr, .02f);
                    Drawer.DrawLine(TR.Pos, new Vector2(BL.Pos.X, TR.Pos.Y), clr, .02f);
                }
          */
    }

    public struct SimpleQuad : IReadWrite
    {
        public void Write(StreamWriter writer)
        {
            Tools.WriteFields(this, writer, "v0", "v1", "v2", "v3", "MySetColor", "PremultipliedColor", "BlendAddRatio", "_MyTexture");
        }
        public void Read(StreamReader reader)
        {
            this = (SimpleQuad)Tools.ReadFields(this, reader);
            SetColor(MySetColor, true);
        }

        public bool Animated;

        AnimationData_Texture TextureAnim;
        public void SetTextureAnim(AnimationData_Texture TextureAnim)
        {
            this.TextureAnim = TextureAnim;
            t = 0;
            anim = 0;
            Playing = true;
            speed = 1;
        }
        public void Set(TextureOrAnim t_or_a)
        {
            SetTextureOrAnim(t_or_a);
        }
        public void Set(string name)
        {
            SetTextureOrAnim(name);
        }


        public float t;

        public int anim;
        public bool Playing;
        public float speed;

        public void UpdateTextureAnim()
        {
            float dt = Tools.CurLevel.IndependentDeltaT;
            t += dt * TextureAnim.Speed * speed;
            CalcTexture(anim, t);
        }

        public bool UseGlobalIllumination;
        public float Illumination;

        Vector2 uv0, uv1, uv2, uv3;

        Vector2 PreCalc0, PreCalc1, PreCalc2, PreCalc3;

        public SimpleVector v0, v1, v2, v3;
        public EzEffect MyEffect;

        public EzTexture MyTexture
        {
            get { return _MyTexture; }
            set
            {
                _MyTexture = value;
                if (_MyTexture != null && _MyTexture.FromPacked)
                {
                    map(ref uv0, ref v0.Vertex.uv, ref _MyTexture.BL, ref _MyTexture.TR);
                    map(ref uv1, ref v1.Vertex.uv, ref _MyTexture.BL, ref _MyTexture.TR);
                    map(ref uv2, ref v2.Vertex.uv, ref _MyTexture.BL, ref _MyTexture.TR);
                    map(ref uv3, ref v3.Vertex.uv, ref _MyTexture.BL, ref _MyTexture.TR);
                    //UVFromBounds(_MyTexture.BL, _MyTexture.TR);
                    //MyTexture = MyTexture.Packed;
                }
            }
        }
        public EzTexture _MyTexture;

        public void CalcTexture(int anim, float frame)
        {
            this.anim = anim;
            this.t = frame;

            if (t > TextureAnim.Anims[anim].Data.Length)
            {
                if (TextureAnim.Reverse)
                {
                    speed *= -1;
                    t -= 1;
                }
                else
                    t = 0;
            }
            else if (t < 0)
            {
                if (TextureAnim.Reverse)
                {
                    speed *= -1;
                    t += 1;
                }
                else
                    t = TextureAnim.Anims[anim].Data.Length;
            }

            var data = TextureAnim.Calc(anim, frame);
            MyTexture = data.texture;

            if (TextureAnim.IsStrip)
            {
                v0.Vertex.uv = new Vector2(data.uv_bl.X, data.uv_bl.Y);
                v1.Vertex.uv = new Vector2(data.uv_tr.X, data.uv_bl.Y);
                v2.Vertex.uv = new Vector2(data.uv_bl.X, data.uv_tr.Y);
                v3.Vertex.uv = new Vector2(data.uv_tr.X, data.uv_tr.Y);
            }
        }

        void map(ref Vector2 base_uv, ref Vector2 new_uv, ref Vector2 bl, ref Vector2 tr)
        {
            if (base_uv.X == 0) new_uv.X = bl.X;
            else new_uv.X = tr.X;
            if (base_uv.Y == 1) new_uv.Y = bl.Y;
            else new_uv.Y = tr.Y;
        }

        public bool Hide;

        public bool U_Wrap, V_Wrap;

        public string Name;

        public void Release()
        {
            v0.Release();
            v1.Release();
            v2.Release();
            v3.Release();

            MyEffect = null;
            MyTexture = null;
        }

        public SimpleQuad(ref SimpleQuad quad)
        {
            Name = quad.Name;

            Animated = quad.Animated;
            TextureAnim = quad.TextureAnim;
            t = quad.t; anim = quad.anim; Playing = quad.Playing; speed = quad.speed;

            Hide = false;

            MySetColor = quad.MySetColor;
            PremultipliedColor = quad.PremultipliedColor;
            BlendAddRatio = quad.BlendAddRatio;

            uv0 = quad.uv0;
            uv1 = quad.uv1;
            uv2 = quad.uv2;
            uv3 = quad.uv3;

            v0 = quad.v0;
            v1 = quad.v1;
            v2 = quad.v2;
            v3 = quad.v3;

            PreCalc0 = quad.PreCalc0;
            PreCalc1 = quad.PreCalc1;
            PreCalc2 = quad.PreCalc2;
            PreCalc3 = quad.PreCalc3;

            MyEffect = quad.MyEffect;
            _MyTexture = quad._MyTexture;

            U_Wrap = quad.U_Wrap;
            V_Wrap = quad.V_Wrap;

            Illumination = quad.Illumination;
            UseGlobalIllumination = quad.UseGlobalIllumination;
        }

        public SimpleQuad(Quad quad)
        {
            Name = quad.Name;

            Animated = true;
            TextureAnim = quad.TextureAnim;

            Hide = false;

            MySetColor = quad.MyColor;
            PremultipliedColor = quad.PremultipliedColor;
            BlendAddRatio = 0;

            uv0 = quad.Vertices[0].uv;
            uv1 = quad.Vertices[1].uv;
            uv2 = quad.Vertices[2].uv;
            uv3 = quad.Vertices[3].uv;

            v0.Vertex = quad.Vertices[0];
            v1.Vertex = quad.Vertices[1];
            v2.Vertex = quad.Vertices[2];
            v3.Vertex = quad.Vertices[3];

            v0.Pos = quad.Vertices[0].xy;
            v1.Pos = quad.Vertices[1].xy;
            v2.Pos = quad.Vertices[2].xy;
            v3.Pos = quad.Vertices[3].xy;

            v0.AnimData = quad.Corner[0].AnimData;
            v1.AnimData = quad.Corner[1].AnimData;
            v2.AnimData = quad.Corner[2].AnimData;
            v3.AnimData = quad.Corner[3].AnimData;

            PreCalc0 = PreCalc1 = PreCalc2 = PreCalc3 = Vector2.Zero;

            MyEffect = quad.MyEffect;
            _MyTexture = null;

            U_Wrap = V_Wrap = false;

            UseGlobalIllumination = true;
            Illumination = 1f;

            t = anim = 0; Playing = false; speed = 1;

            MyTexture = quad.MyTexture;
            //TextureName = quad.MyTexture.Name;
        }

        public void SetTextureOrAnim(TextureOrAnim t_or_a)
        {
            if (t_or_a.IsAnim)
            {
                SetTextureAnim(t_or_a.MyAnim);
                CalcTexture(0, 0);
            }
            else
                MyTexture = t_or_a.MyTexture;
        }
        public void SetTextureOrAnim(string name)
        {
            if (Tools.TextureWad.AnimationDict.ContainsKey(name))
                SetTextureAnim(Tools.TextureWad.AnimationDict[name]);
            else
                TextureName = name;
        }

        public int TexWidth
        {
            get
            {
                if (TextureAnim != null)
                    return TextureAnim.Width;
                else
                    return MyTexture.Tex.Width;
            }
        }
        public int TexHeight
        {
            get
            {
                if (TextureAnim != null)
                    return TextureAnim.Height;
                else
                    return MyTexture.Tex.Height;
            }
        }

        /// <summary>
        /// The name of the quad's texture. Setting will automatically search the TextureWad for a matching texture.
        /// </summary>
        public string TextureName
        {
            get { return MyTexture.Path; }
            set { MyTexture = Tools.TextureWad.FindByName(value); }
        }

        public void EnforceTextureRatio()
        {
            float Ratio = (float)MyTexture.Tex.Height / (float)MyTexture.Tex.Width;
            v0.Pos = new Vector2(-1, Ratio);
            v1.Pos = new Vector2(1, Ratio);
            v2.Pos = new Vector2(-1, -Ratio);
            v3.Pos = new Vector2(1, -Ratio);
        }

        public Vector2 TR { get { return v1.Vertex.xy; } }
        public Vector2 BL { get { return v2.Vertex.xy; } }

        public float Right  { get { return v1.Vertex.xy.X; } }
        public float Left   { get { return v2.Vertex.xy.X; } }
        public float Top    { get { return v1.Vertex.xy.Y; } }
        public float Bottom { get { return v2.Vertex.xy.Y; } }
        public float Width  { get { return Right - Left; } }
        public float Height { get { return Top - Bottom; } }

        public void Shift(Vector2 shift)
        {
            v0.Pos += shift;
            v1.Pos += shift;
            v2.Pos += shift;
            v3.Pos += shift;
        }

        public void FromBounds(Vector2 BL, Vector2 TR)
        {
            v0.Pos = new Vector2(BL.X, TR.Y);
            v1.Pos = TR;
            v2.Pos = BL;
            v3.Pos = new Vector2(TR.X, BL.Y);
        }

        public void RotateRight()
        {
            v0.RotateRight();
            v1.RotateRight();
            v2.RotateRight();
            v3.RotateRight();
        }

        public void RotateLeft()
        {
            v0.RotateLeft();
            v1.RotateLeft();
            v2.RotateLeft();
            v3.RotateLeft();
        }

        public void RotateUV()
        {
            Vector2 hold = v0.Vertex.uv;
            
            v0.Vertex.uv = v1.Vertex.uv;
            v1.Vertex.uv = v3.Vertex.uv;
            v3.Vertex.uv = v2.Vertex.uv;
            v2.Vertex.uv = hold;
        }

        public void MirrorUV_Vertical()
        {
            Vector2 hold;
            
            hold = v0.Vertex.uv;
            v0.Vertex.uv = v2.Vertex.uv;
            v2.Vertex.uv = hold;

            hold = v1.Vertex.uv;
            v1.Vertex.uv = v3.Vertex.uv;
            v3.Vertex.uv = hold;
        }

        public void MirrorUV_Horizontal()
        {
            Vector2 hold;

            hold = v0.Vertex.uv;
            v0.Vertex.uv = v1.Vertex.uv;
            v1.Vertex.uv = hold;

            hold = v2.Vertex.uv;
            v2.Vertex.uv = v3.Vertex.uv;
            v3.Vertex.uv = hold;
        }

        public Vector2 UV_Offset
        {
            get
            {
                return v0.Vertex.uv;
            }
            set
            {
                UVFromBounds_2(value, value + UV_Repeat);
            }
        }
        public Vector2 UV_Repeat
        {
            get
            {
                return v3.Vertex.uv - v0.Vertex.uv;
            }
            set
            {
                UVFromBounds_2(UV_Offset, UV_Offset + value);
            }
        }

        public void UV_Phsx(Vector2 speed)
        {
            v0.Vertex.uv += speed;
            v1.Vertex.uv += speed;
            v2.Vertex.uv += speed;
            v3.Vertex.uv += speed;
        }

        // v0 v1
        // v2 v3
        public void UVFromBounds(Vector2 BL, Vector2 TR)
        {
            v0.Vertex.uv = new Vector2(BL.X, TR.Y);
            v1.Vertex.uv = TR;
            v2.Vertex.uv = BL;
            v3.Vertex.uv = new Vector2(TR.X, BL.Y);
        }

        public void UVFromBounds_2(Vector2 BL, Vector2 TR)
        {
            v0.Vertex.uv = BL;
            v1.Vertex.uv = new Vector2(TR.X, BL.Y);
            v2.Vertex.uv = new Vector2(BL.X, TR.Y);
            v3.Vertex.uv = TR;
        }

        public void Init()
        {
            UseGlobalIllumination = true;
            Illumination = 1f;

            BlendAddRatio = 0;
            SetColor(Color.White);

            v0.Pos = new Vector2(-1, 1);
            v1.Pos = new Vector2(1, 1);
            v2.Pos = new Vector2(-1, -1);
            v3.Pos = new Vector2(1, -1);

            v0.Vertex.xy = new Vector2(-1, 1);
            v1.Vertex.xy = new Vector2(1, 1);
            v2.Vertex.xy = new Vector2(-1, -1);
            v3.Vertex.xy = new Vector2(1, -1);

            uv0 = v0.Vertex.uv = new Vector2(0, 0);
            uv1 = v1.Vertex.uv = new Vector2(1, 0);
            uv2 = v2.Vertex.uv = new Vector2(0, 1);
            uv3 = v3.Vertex.uv = new Vector2(1, 1);
        }

        /// <summary>
        /// Shift the quad's absolute vertex coordinates. Does not effect normal coordinates.
        /// </summary>
        /// <param name="shift"></param>
        public void UpdatedShift(Vector2 shift)
        {
            v0.Vertex.xy += shift;
            v1.Vertex.xy += shift;
            v2.Vertex.xy += shift;
            v3.Vertex.xy += shift;
        }

        /// <summary>
        /// Copy and shift the source quad's vertex locations.
        /// </summary>
        /// <param name="SourceQuad">The source quad</param>
        /// <param name="shift">The amount to shift</param>
        public void CopyUpdate(ref SimpleQuad SourceQuad, ref Vector2 shift)
        {
            v0.Vertex.xy = SourceQuad.v0.Vertex.xy + shift;
            v1.Vertex.xy = SourceQuad.v1.Vertex.xy + shift;
            v2.Vertex.xy = SourceQuad.v2.Vertex.xy + shift;
            v3.Vertex.xy = SourceQuad.v3.Vertex.xy + shift;
        }

        public void UpdateShift(ref BasePoint Base)
        {
            v0.Vertex.xy = Base.Origin + PreCalc0;
            v1.Vertex.xy = Base.Origin + PreCalc1;
            v2.Vertex.xy = Base.Origin + PreCalc2;
            v3.Vertex.xy = Base.Origin + PreCalc3;
        }

        public void UpdateShift_Precalc(ref BasePoint Base)
        {
            PreCalc0 = v0.Pos.X * Base.e1 + v0.Pos.Y * Base.e2;
            PreCalc1 = v1.Pos.X * Base.e1 + v1.Pos.Y * Base.e2;
            PreCalc2 = v2.Pos.X * Base.e1 + v2.Pos.Y * Base.e2;
            PreCalc3 = v3.Pos.X * Base.e1 + v3.Pos.Y * Base.e2;
        }

        public void Update(ref BasePoint Base)
        {
            v0.Vertex.xy = Base.Origin + v0.Pos.X * Base.e1 + v0.Pos.Y * Base.e2;
            v1.Vertex.xy = Base.Origin + v1.Pos.X * Base.e1 + v1.Pos.Y * Base.e2;
            v2.Vertex.xy = Base.Origin + v2.Pos.X * Base.e1 + v2.Pos.Y * Base.e2;
            v3.Vertex.xy = Base.Origin + v3.Pos.X * Base.e1 + v3.Pos.Y * Base.e2;
        }

        public void SymmetricUpdate(ref BasePoint Base)
        {
            Vector2 x = v0.Pos.X * Base.e1;
            Vector2 y = v0.Pos.Y * Base.e2;
            v0.Vertex.xy = Base.Origin + x + y;
            v1.Vertex.xy = Base.Origin - x + y;
            v2.Vertex.xy = Base.Origin + x - y;
            v3.Vertex.xy = Base.Origin - x - y;
        }

        public void SetHold()
        {
            v0.AnimData.Hold = v0.Pos;
            v1.AnimData.Hold = v1.Pos;
            v2.AnimData.Hold = v2.Pos;
            v3.AnimData.Hold = v3.Pos;
        }

        public void ReadAnim(int anim, int frame)
        {
            v0.Pos = v0.AnimData.Get(anim, frame);
            v1.Pos = v1.AnimData.Get(anim, frame);
            v2.Pos = v2.AnimData.Get(anim, frame);
            v3.Pos = v3.AnimData.Get(anim, frame);
        }

        public void Transfer(int anim, float DestT, int AnimLength, bool Loop, bool Linear, float t)
        {
            v0.Pos = v0.AnimData.Transfer(anim, DestT, AnimLength, Loop, Linear, t);
            v1.Pos = v1.AnimData.Transfer(anim, DestT, AnimLength, Loop, Linear, t);
            v2.Pos = v2.AnimData.Transfer(anim, DestT, AnimLength, Loop, Linear, t);
            v3.Pos = v3.AnimData.Transfer(anim, DestT, AnimLength, Loop, Linear, t);
        }

        public void Calc(int anim, float t, int AnimLength, bool Loop, bool Linear)
        {
            v0.Pos = v0.AnimData.Calc(anim, t, AnimLength, Loop, Linear);
            v1.Pos = v1.AnimData.Calc(anim, t, AnimLength, Loop, Linear);
            v2.Pos = v2.AnimData.Calc(anim, t, AnimLength, Loop, Linear);
            v3.Pos = v3.AnimData.Calc(anim, t, AnimLength, Loop, Linear);
        }

        public Color MySetColor, PremultipliedColor;
        public float BlendAddRatio;
        public void SetColor(Color color) { SetColor(color, false); }
        public void SetColor(Color color, bool ForceUpdate)
        {
            if (!ForceUpdate && color == MySetColor) return;

            MySetColor = color;

            PremultipliedColor = Tools.PremultiplyAlpha(color, BlendAddRatio);

            v0.Vertex.Color = PremultipliedColor;
            v1.Vertex.Color = PremultipliedColor;
            v2.Vertex.Color = PremultipliedColor;
            v3.Vertex.Color = PremultipliedColor;
        }
        public void SetColor(Vector4 color) { SetColor(new Color(color)); }

        public void SetAlpha(float alpha)
        {
            if (alpha < 0) alpha = 0;
            if (alpha > 1) alpha = 1;

            Color color = MySetColor;
            color.A = (byte)(alpha * 255);
            SetColor(color);
        }

        public void MultiplyAlpha(float alpha)
        {
            Color color = MySetColor;
            color.A = (byte)(alpha * color.A);
            SetColor(color);
        }
    }

    public struct BasePoint
    {
        public Vector2 Origin, e1, e2;

        public void Init()
        {
            Origin = Vector2.Zero;
            e1 = new Vector2(1, 0);
            e2 = new Vector2(0, 1);
        }

        public Vector2 GetScale()
        {
            return new Vector2(e1.X, e2.Y);
        }

        public void SetScale(Vector2 scale)
        {
            e1.X = scale.X;
            e2.Y = scale.Y;
        }
    }

    public class SimpleObject
    {
        public SimpleQuad[] Quads;
        public SimpleBox[] Boxes;
        public BasePoint Base;

        public bool xFlip, yFlip, CenterFlipOnBox;
        public Vector2 FlipCenter;

        public Queue<AnimQueueEntry> AnimQueue;
        public AnimQueueEntry LastAnimEntry;
        public int[] AnimLength;
        public string[] AnimName;
        public float[] AnimSpeed;
        public bool Play, Loop, Transfer, OldLoop, Linear;
        public int anim, OldAnim;
        public float t, OldT, StartT;

        public List<EzEffect> MyEffects;

        public bool Released;

        public void Release()
        {
            //return;
            if (Released) return;
            Released = true;

            if (Quads != null)
                for (int i = 0; i < Quads.Length; i++)
                    Quads[i].Release();
                //foreach (SimpleQuad quad in Quads)
                    //quad.Release();
            if (Boxes != null)
                foreach (SimpleBox box in Boxes)
                    box.Release();
            AnimQueue = null;
            LastAnimEntry = null;
            AnimLength = null;
            AnimName = null;
            AnimSpeed = null;

            MyEffects = null;
        }

        /// <summary>
        /// Update the list of effects associated with the object's quads
        /// </summary>
        public void UpdateEffectList()
        {
            if (Quads == null || Quads.Length == 0) return;

            if (MyEffects == null) MyEffects = new List<EzEffect>();
            else MyEffects.Clear();

            for (int i = 0; i < Quads.Length; i++)
                if (!MyEffects.Contains(Quads[i].MyEffect))
                    MyEffects.Add(Quads[i].MyEffect);
        }

        /// <summary>
        /// Find the index of a quad of the given name.
        /// </summary>
        public int GetQuadIndex(string name)
        {
            for (int i = 0; i < Quads.Length; i++)
            {
                if (string.Compare(Quads[i].Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return i;
            }

            return -1;
        }

        public void Scale(float scale)
        {
            Base.e1 *= scale;
            Base.e2 *= scale;
        }

        public void SetColor(Color color)
        {
            if (Quads != null)
                for (int i = 0; i < Quads.Length; i++)
                    Quads[i].SetColor(color);
        }

        public SimpleObject(SimpleObject obj, bool BoxesOnly)
        {
            Constructor(obj, BoxesOnly, false);
        }
        void Constructor(SimpleObject obj, bool BoxesOnly, bool DeepCopy)
        {
            Base = obj.Base;

            CenterFlipOnBox = obj.CenterFlipOnBox;

            if (!BoxesOnly)
            {
                Quads = new SimpleQuad[obj.Quads.Length];
                for (int i = 0; i < obj.Quads.Length; i++)
                    Quads[i] = new SimpleQuad(ref obj.Quads[i]);
            }

            Boxes = new SimpleBox[obj.Boxes.Length];
            for (int i = 0; i < obj.Boxes.Length; i++)
                Boxes[i] = new SimpleBox(obj.Boxes[i]);

            AnimQueue = new Queue<AnimQueueEntry>();
            AnimQueueEntry[] array = obj.AnimQueue.ToArray();
            if (array.Length > 0)
            {
                LastAnimEntry = new AnimQueueEntry(array[array.Length - 1]);
                for (int i = 0; i < array.Length - 1; i++)
                    AnimQueue.Enqueue(new AnimQueueEntry(array[i]));
                AnimQueue.Enqueue(LastAnimEntry);
            }

            Play = obj.Play;
            Loop = obj.Loop;
            anim = obj.anim;
            t = obj.t;

            if (DeepCopy)
            {
                AnimLength = new int[50];
                obj.AnimLength.CopyTo(AnimLength, 0);
                AnimSpeed = new float[50];
                obj.AnimSpeed.CopyTo(AnimSpeed, 0);
                AnimName = new string[50];
                obj.AnimName.CopyTo(AnimName, 0);

                UpdateEffectList();
            }
            else
            {
                AnimLength = obj.AnimLength;
                AnimSpeed = obj.AnimSpeed;
                AnimName = obj.AnimName;
                
                MyEffects = obj.MyEffects;
            }
        }

        public SimpleObject(ObjectClass obj)
        {
            Base.Init();

            CenterFlipOnBox = obj.CenterFlipOnBox;

            Quads = new SimpleQuad[obj.QuadList.Count];
            for (int i = 0; i < obj.QuadList.Count; i++)
                Quads[i] = new SimpleQuad((Quad)obj.QuadList[i]);

            Boxes = new SimpleBox[obj.BoxList.Count];
            for (int i = 0; i < obj.BoxList.Count; i++)
                Boxes[i] = new SimpleBox(obj.BoxList[i]);

            AnimQueue = new Queue<AnimQueueEntry>();
            AnimQueueEntry[] array = obj.AnimQueue.ToArray();
            if (array.Length > 0)
            {
                LastAnimEntry = new AnimQueueEntry(array[array.Length - 1]);
                for (int i = 0; i < array.Length - 1; i++)
                    AnimQueue.Enqueue(new AnimQueueEntry(array[i]));
                AnimQueue.Enqueue(LastAnimEntry);
            }

            Play = obj.Play;
            Loop = obj.Loop;
            anim = obj.anim;
            t = obj.t;

            AnimLength = new int[50];
            obj.AnimLength.CopyTo(AnimLength, 0);
            AnimSpeed = new float[50];
            obj.AnimSpeed.CopyTo(AnimSpeed, 0);
            AnimName = new string[50];
            obj.AnimName.CopyTo(AnimName, 0);

            UpdateEffectList();
        }

        public Vector2 GetBoxCenter(int i)
        {
            Vector2 c = Boxes[i].Center();
            if (xFlip)
                c.X = FlipCenter.X - (c.X - FlipCenter.X);
            if (yFlip)
                c.Y = FlipCenter.Y - (c.Y - FlipCenter.Y);
            return c;
        }

        public void CopyUpdate(SimpleObject source)
        {
            Vector2 shift = Base.Origin - source.Base.Origin;

            if (Quads != null)
                for (int i = 0; i < Quads.Length; i++)
                    Quads[i].CopyUpdate(ref source.Quads[i], ref shift);

            for (int i = 0; i < Boxes.Length; i++)
                Boxes[i].CopyUpdate(ref source.Boxes[i], ref shift);

            if (Boxes.Length > 0)
                FlipCenter = Boxes[0].Center();
            else
                FlipCenter = Base.Origin;
        }

        public void UpdateQuads()
        {
            if (Quads != null)
                for (int i = 0; i < Quads.Length; i++)
                    Quads[i].Update(ref Base);
        }

        public void UpdateBoxes()
        {
            for (int i = 0; i < Boxes.Length; i++)
                Boxes[i].Update(ref Base);

            if (Boxes.Length > 0)
                FlipCenter = Boxes[0].Center();
            else
                FlipCenter = Base.Origin;
        }

        public void Update()
        {
            UpdateQuads();

            UpdateBoxes();
        }

        /// <summary>
        /// Shift each quad's absolute vertex coordinates. Does not effect normal coordinates.
        /// </summary>
        /// <param name="shift"></param>
        public void UpdatedShift(Vector2 shift)
        {
            for (int i = 0; i < Quads.Length; i++)
                Quads[i].UpdatedShift(shift);
        }

        public void Draw() { Draw(Tools.QDrawer, Tools.EffectWad); }

        public void Draw(QuadDrawer QDrawer, EzEffectWad EffectWad)
        {
            int n = 0;
            if (Quads != null) n = Quads.Length;

            Draw(QDrawer, EffectWad, 0, n - 1);
        }
        public void Draw(QuadDrawer QDrawer, EzEffectWad EffectWad, int StartIndex, int EndIndex)
        {
            if (xFlip || yFlip)
                foreach (EzEffect fx in MyEffects)
                {
                    fx.FlipCenter.SetValue(FlipCenter);
                    fx.FlipVector.SetValue(new Vector2(xFlip ? 1 : -1, yFlip ? 1 : -1));
                }

            
            for (int i = StartIndex; i <= EndIndex; i++)
                QDrawer.DrawQuad(ref Quads[i]);

            if (xFlip || yFlip)
            {
                QDrawer.Flush();
                foreach (EzEffect fx in MyEffects)
                    fx.FlipVector.SetValue(new Vector2(-1, -1));
            }
        }

        public void DrawQuad(ref SimpleQuad Quad)
        {
            if (xFlip || yFlip)
                foreach (EzEffect fx in MyEffects)
                {
                    fx.FlipCenter.SetValue(FlipCenter);
                    fx.FlipVector.SetValue(new Vector2(xFlip ? 1 : -1, yFlip ? 1 : -1));
                }

            Tools.QDrawer.DrawQuad(ref Quad);

            if (xFlip || yFlip)
            {
                Tools.QDrawer.Flush();
                foreach (EzEffect fx in MyEffects)
                    fx.FlipVector.SetValue(new Vector2(-1, -1));
            }
        }

        public void EnqueueTransfer(int _anim, float _destT, float speed, bool DestLoop)
        {
            DequeueTransfers();

            AnimQueueEntry NewEntry = new AnimQueueEntry();
            NewEntry.anim = _anim;
            NewEntry.AnimSpeed = speed;
            NewEntry.StartT = 0;
            NewEntry.EndT = 1;
            NewEntry.DestT = _destT;
            NewEntry.Loop = DestLoop;
            NewEntry.Type = AnimQueueEntryType.Transfer;
            NewEntry.Initialized = false;

            AnimQueue.Enqueue(NewEntry);
            LastAnimEntry = NewEntry;
        }

        public void EnqueueAnimation(int _anim, float startT, bool loop)
        {
            EnqueueTransfer(_anim, startT, .5f, loop);

            AnimQueueEntry NewEntry = new AnimQueueEntry();
            NewEntry.anim = _anim;
            NewEntry.AnimSpeed = 1;
            NewEntry.StartT = startT;
            NewEntry.EndT = -1;
            NewEntry.Loop = loop;
            NewEntry.Type = AnimQueueEntryType.Play;
            NewEntry.Initialized = false;

            AnimQueue.Enqueue(NewEntry);
            LastAnimEntry = NewEntry;
        }

        public int DestinationAnim()
        {
            if (AnimQueue.Count > 0)
                return LastAnimEntry.anim;
            else
                return anim;
        }

        public void DequeueTransfers()
        {
            while (AnimQueue.Count > 0 && AnimQueue.Peek().Type == AnimQueueEntryType.Transfer)
                AnimQueue.Dequeue();
        }

        public void PlayUpdate(float DeltaT)
        {
            if (!Play) return;
            if (AnimQueue.Count == 0) return;

            AnimQueueEntry CurAnimQueueEntry = AnimQueue.Peek();

            if (!CurAnimQueueEntry.Initialized)
            {
                t = CurAnimQueueEntry.StartT;
                Loop = CurAnimQueueEntry.Loop;
                anim = CurAnimQueueEntry.anim;

                if (CurAnimQueueEntry.Type == AnimQueueEntryType.Transfer)
                    SetHold();

                CurAnimQueueEntry.Initialized = true;
            }

            t += DeltaT * AnimSpeed[anim] * CurAnimQueueEntry.AnimSpeed;
            if (CurAnimQueueEntry.Type == AnimQueueEntryType.Transfer)
            {
                if (t > 1)
                {
                    AnimQueue.Dequeue();
                    if (AnimQueue.Count > 0)
                    {
                        AnimQueueEntry Next = AnimQueue.Peek();
                        if (Next.anim == anim)
                            Next.StartT = CurAnimQueueEntry.DestT;
                    }

                    t = 1;
                }
            }
            else
            {
                if (t > CurAnimQueueEntry.EndT && t - DeltaT <= CurAnimQueueEntry.EndT)
                {
                    t = CurAnimQueueEntry.EndT;
                    AnimQueue.Dequeue();
                }
                else
                {
                    if (Loop)
                    {
                        if (t <= 0)
                            t += AnimLength[anim] + 1;
                        if (t > AnimLength[anim] + 1)
                            t -= AnimLength[anim] + 1;
                    }
                    else
                    {
                        if (t < 0)
                        {
                            t = 0;
                            AnimQueue.Dequeue();
                        }
                        if (t > AnimLength[anim])
                        {
                            t = AnimLength[anim];
                            AnimQueue.Dequeue();
                        }
                    }
                }
            }

            if (CurAnimQueueEntry.Type == AnimQueueEntryType.Play)
            {
                if (Quads != null)
                    for (int i = 0; i < Quads.Length; i++)
                        if (Quads[i].Animated)
                            Quads[i].Calc(anim, t, AnimLength[anim], Loop, Linear);
                for (int i = 0; i < Boxes.Length; i++)
                    if (Boxes[i].Animated)
                        Boxes[i].Calc(anim, t, AnimLength[anim], Loop, Linear);
            }
            else
            {
                if (Quads != null)
                    for (int i = 0; i < Quads.Length; i++)
                        Quads[i].Transfer(anim, CurAnimQueueEntry.DestT, AnimLength[anim], CurAnimQueueEntry.Loop, Linear, t);
                for (int i = 0; i < Boxes.Length; i++)
                    Boxes[i].Transfer(anim, CurAnimQueueEntry.DestT, AnimLength[anim], CurAnimQueueEntry.Loop, Linear, t);
            }
        }

        public void SetHold()
        {
            if (Quads != null)
                for (int i = 0; i < Quads.Length; i++)
                    Quads[i].SetHold();
            for (int i = 0; i < Boxes.Length; i++)
                Boxes[i].SetHold();
        }

        public void Read(int anim, int frame)
        {
            if (Quads != null)
                for (int i = 0; i < Quads.Length; i++)
                    Quads[i].ReadAnim(anim, frame);
            for (int i = 0; i < Boxes.Length; i++)
                Boxes[i].ReadAnim(anim, frame);
        }
    }
}