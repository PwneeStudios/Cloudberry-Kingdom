using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;

namespace CloudberryKingdom
{
    public struct SimpleQuad
    {
        public bool Animated;

        public void NextKeyFrame()
        {
            var data = TextureAnim.NextKeyFrame();
            MyTexture = data;
        }

        public AnimationData_Texture TextureAnim;
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
            t += dt * TextureAnim.Anims[anim].Speed * speed;

            CalcTexture(anim, t);
        }

        public bool UseGlobalIllumination;
        public float Illumination;

        Vector2 uv0, uv1, uv2, uv3;

        Vector2 PreCalc0, PreCalc1, PreCalc2, PreCalc3;

        public SimpleVector v0, v1, v2, v3;
        public EzEffect MyEffect;

        /// <summary>
        /// Extra textures used as input more advanced pixel shaders.
        /// </summary>
        public EzTexture ExtraTexture1, ExtraTexture2;

        /// <summary>
        /// Texture to be drawn on this quad.
        /// </summary>
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

            if (t >= TextureAnim.Anims[anim].Data.Length)
                t = 0;
            else if (t < 0)
            {
                t += TextureAnim.Anims[anim].Data.Length;
                if (t < 0)
                    t = TextureAnim.Anims[anim].Data.Length - 1;
            }

            var data = TextureAnim.Calc(anim, t);
            MyTexture = data;
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

            ExtraTexture1 = quad.ExtraTexture1;
            ExtraTexture2 = quad.ExtraTexture2;

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
            ExtraTexture1 = ExtraTexture2 = null;

            U_Wrap = V_Wrap = false;

            UseGlobalIllumination = true;
            Illumination = 1f;

            t = anim = 0; Playing = false; speed = 1;
            
            MyTexture = quad.MyTexture;
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
                    return MyTexture.Width;
            }
        }
        public int TexHeight
        {
            get
            {
                if (TextureAnim != null)
                    return TextureAnim.Height;
                else
                    return MyTexture.Height;
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
            float Ratio = (float)MyTexture.Height / (float)MyTexture.Width;
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
                if (_MyTexture.FromPacked)
                    return v0.Vertex.uv / new Vector2(_MyTexture.TR.X - _MyTexture.BL.X, _MyTexture.BL.Y - _MyTexture.TR.Y);
                else
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
                if (_MyTexture.FromPacked)
                    return (v3.Vertex.uv - v0.Vertex.uv) / new Vector2(_MyTexture.TR.X - _MyTexture.BL.X, _MyTexture.BL.Y - _MyTexture.TR.Y);
                else
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

            DefaultCorners();

            uv0 = v0.Vertex.uv = new Vector2(0, 0);
            uv1 = v1.Vertex.uv = new Vector2(1, 0);
            uv2 = v2.Vertex.uv = new Vector2(0, 1);
            uv3 = v3.Vertex.uv = new Vector2(1, 1);
        }

        public void DefaultCorners()
        {
            v0.Pos = new Vector2(-1, 1);
            v1.Pos = new Vector2(1, 1);
            v2.Pos = new Vector2(-1, -1);
            v3.Pos = new Vector2(1, -1);

            v0.Vertex.xy = new Vector2(-1, 1);
            v1.Vertex.xy = new Vector2(1, 1);
            v2.Vertex.xy = new Vector2(-1, -1);
            v3.Vertex.xy = new Vector2(1, -1);
        }

        bool IsDefault()
        {
            if (v0.Pos != new Vector2(-1, 1)) return false;
            if (v1.Pos != new Vector2(1, 1)) return false;
            if (v2.Pos != new Vector2(-1, -1)) return false;
            if (v3.Pos != new Vector2(1, -1)) return false;

            if (uv0 != new Vector2(0, 0)) return false;
            if (uv1 != new Vector2(1, 0)) return false;
            if (uv2 != new Vector2(0, 1)) return false;
            if (uv3 != new Vector2(1, 1)) return false;

            if (v0.Vertex.uv != new Vector2(0, 0)) return false;
            if (v1.Vertex.uv != new Vector2(1, 0)) return false;
            if (v2.Vertex.uv != new Vector2(0, 1)) return false;
            if (v3.Vertex.uv != new Vector2(1, 1)) return false;

            return true;
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

            PremultipliedColor = ColorHelper.PremultiplyAlpha(color, BlendAddRatio);

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
}