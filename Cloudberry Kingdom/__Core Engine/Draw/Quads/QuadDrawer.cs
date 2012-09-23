using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CloudberryKingdom;

namespace Drawing
{
    public class QuadDrawer
    {
        GraphicsDevice Device;

        public EzEffect DefaultEffect;
        public EzTexture DefaultTexture;

        private EzEffect CurrentEffect;
        private EzTexture CurrentTexture;

        /// <summary>
        /// Color rotation matrix.
        /// </summary>
        private Matrix CurrentMatrix
        {
            get
            {
                return _CurrentMatrix;
            }

            set
            {
                _CurrentMatrix = value;
                CurrentMatrixSignature = Tools.MatrixSignature(_CurrentMatrix);
            }
        }
        Matrix _CurrentMatrix;
        public float CurrentMatrixSignature;

        bool Current_U_Wrap, Current_V_Wrap;

        public MyOwnVertexFormat[] Vertices;
        public int N;
        public int TrianglesInBuffer;
        private int i;

        SimpleQuad LineQuad;

        SamplerState WrapWrap, ClampWrap, WrapClamp, ClampClamp;

        public float GlobalIllumination { get { return _GlobalIllumination; } set { _GlobalIllumination = value; } }
        float _GlobalIllumination = 1f;
        float Illumination = 1f;
        
        //float CurrentIllumination = 1f;
        //public void InvalidateIllumination() { CurrentIllumination = -1; }

        public QuadDrawer(GraphicsDevice device, int n)
        {
            Device = device;
            N = n;

            TrianglesInBuffer = 0;
            i = 0;
            Vertices = new MyOwnVertexFormat[N];

            LineQuad = new SimpleQuad();
            LineQuad.Init();

            WrapWrap = new SamplerState();
            WrapWrap.AddressU = TextureAddressMode.Wrap;
            WrapWrap.AddressV = TextureAddressMode.Wrap;

            ClampClamp = new SamplerState();
            ClampClamp.AddressU = TextureAddressMode.Clamp;
            ClampClamp.AddressV = TextureAddressMode.Clamp;

            ClampWrap = new SamplerState();
            ClampWrap.AddressU = TextureAddressMode.Clamp;
            ClampWrap.AddressV = TextureAddressMode.Wrap;

            WrapClamp = new SamplerState();
            WrapClamp.AddressU = TextureAddressMode.Wrap;
            WrapClamp.AddressV = TextureAddressMode.Clamp;
        }

        public void DrawQuad(Quad quad)
        {
            if (i + 6 > N ||
                i != 0 && (CurrentEffect.effect != quad.MyEffect.effect ||
                           CurrentTexture.Tex != quad.MyTexture.Tex ||
                           CurrentMatrixSignature != quad.MyMatrixSignature))
                Flush();

            if (i == 0)
            {
                CurrentEffect = quad.MyEffect;
                CurrentTexture = quad.MyTexture;
                CurrentMatrix = quad.MyMatrix;
            }
            CurrentEffect.CurrentIllumination = 1;
            CurrentEffect.Illumination.SetValue(1);
            

            Vertices[i] = quad.Vertices[0];
            Vertices[i + 1] = quad.Vertices[1];
            Vertices[i + 2] = quad.Vertices[2];

            Vertices[i + 3] = quad.Vertices[3];
            Vertices[i + 4] = quad.Vertices[2];
            Vertices[i + 5] = quad.Vertices[1];

            i += 6;
            TrianglesInBuffer += 2;
        }

        public void SetInitialState()
        {
            Device.SamplerStates[0] = ClampClamp;
            Device.SamplerStates[1] = ClampClamp;
            Device.SamplerStates[2] = ClampClamp;
        }

        void SetSamplerState()
        {
            //Device.SamplerStates[1] = ClampClamp;
            //return;
            
            if (Current_U_Wrap)
            {
                if (Current_V_Wrap) Device.SamplerStates[1] = WrapWrap;
                else Device.SamplerStates[1] = WrapClamp;
            }
            else
            {
                if (Current_V_Wrap) Device.SamplerStates[1] = ClampWrap;
                else Device.SamplerStates[1] = ClampClamp;
            }
        }

        public void SetAddressMode(bool U_Wrap, bool V_Wrap)
        {
            Current_U_Wrap = U_Wrap;
            Current_V_Wrap = V_Wrap;
            SetSamplerState();
        }

        public void DrawQuad_Particle(ref SimpleQuad quad)
        {
            // Update anim
            if (quad.Playing) quad.UpdateTextureAnim();

            // Calculate illumination
            if (quad.UseGlobalIllumination)
                Illumination = GlobalIllumination * quad.Illumination;
            else
                Illumination = quad.Illumination;

            if (CurrentEffect.effect != quad.MyEffect.effect ||
                CurrentTexture.Tex != quad.MyTexture.Tex && !quad.MyTexture.FromPacked ||
                CurrentTexture != quad.MyTexture.Packed && quad.MyTexture.FromPacked ||
                CurrentEffect.CurrentIllumination != Illumination)
                Flush();

            if (i == 0)
            {
                if (Current_U_Wrap != quad.U_Wrap || Current_V_Wrap != quad.V_Wrap)
                    SetAddressMode(false, false);

                CurrentEffect = quad.MyEffect;
                CurrentTexture = quad.MyTexture;

                if (CurrentTexture.FromPacked)
                    CurrentTexture = CurrentTexture.Packed;

                if (CurrentEffect.CurrentIllumination != Illumination)
                {
                    CurrentEffect.CurrentIllumination = Illumination;
                    CurrentEffect.Illumination.SetValue(Illumination);
                }
            }

            Vertices[i] = quad.v0.Vertex;
            Vertices[i + 5] = Vertices[i + 1] = quad.v1.Vertex;
            Vertices[i + 4] = Vertices[i + 2] = quad.v2.Vertex;
            Vertices[i + 3] = quad.v3.Vertex;

            i += 6;
            TrianglesInBuffer += 2;
        }

        public void DrawQuad(ref SimpleQuad quad)
        {
            if (quad.Hide) return;

            if (quad.MyEffect == null || quad.MyTexture == null) { Tools.Break(); return; }

            if (Tools.UsingSpriteBatch) Tools.EndSpriteBatch();

            // Update anim
            if (quad.Playing) quad.UpdateTextureAnim();

            // Calculate illumination
            if (quad.UseGlobalIllumination)
                Illumination = GlobalIllumination * quad.Illumination;
            else
                Illumination = quad.Illumination;

            if (i + 6 >= N ||
                i != 0 &&
                    (CurrentEffect.effect != quad.MyEffect.effect ||
                     CurrentTexture.Tex != quad.MyTexture.Tex && !quad.MyTexture.FromPacked ||
                     CurrentTexture != quad.MyTexture.Packed && quad.MyTexture.FromPacked ||
                     Current_U_Wrap != quad.U_Wrap ||
                     Current_V_Wrap != quad.V_Wrap ||
                     CurrentEffect.CurrentIllumination != Illumination))
                Flush();

            if (i == 0)
            {
                if (Current_U_Wrap != quad.U_Wrap || Current_V_Wrap != quad.V_Wrap)
                    SetAddressMode(quad.U_Wrap, quad.V_Wrap);
        
                CurrentEffect = quad.MyEffect;
                CurrentTexture = quad.MyTexture;

                // Set extra textures
                if (quad.ExtraTexture1 != null) CurrentEffect.SetExtraTexture1(quad.ExtraTexture1);
                if (quad.ExtraTexture2 != null) CurrentEffect.SetExtraTexture2(quad.ExtraTexture2);

                if (CurrentTexture.FromPacked)
                    CurrentTexture = CurrentTexture.Packed;

                if (CurrentEffect.CurrentIllumination != Illumination)
                {
                    CurrentEffect.CurrentIllumination = Illumination;
                    CurrentEffect.Illumination.SetValue(Illumination);
                }
            }

            Vertices[i] = quad.v0.Vertex;
            Vertices[i + 5] = Vertices[i + 1] = quad.v1.Vertex;
            Vertices[i + 4] = Vertices[i + 2] = quad.v2.Vertex;
            Vertices[i + 3] = quad.v3.Vertex;

            i += 6;
            TrianglesInBuffer += 2;
        }

        public void DrawFilledBox(Vector2 BL, Vector2 TR, Color color)
        {
            LineQuad.MyEffect = DefaultEffect;
            LineQuad.MyEffect = Tools.BasicEffect;;
            LineQuad.MyTexture = DefaultTexture;

            color = Tools.PremultiplyAlpha(color);

            LineQuad.v0.Vertex.xy = new Vector2(BL.X, TR.Y); LineQuad.v0.Vertex.Color = color;
            LineQuad.v1.Vertex.xy = TR; LineQuad.v1.Vertex.Color = color;
            LineQuad.v2.Vertex.xy = BL; LineQuad.v2.Vertex.Color = color;
            LineQuad.v3.Vertex.xy =  new Vector2(TR.X, BL.Y); LineQuad.v3.Vertex.Color = color;

            LineQuad.v0.Vertex.uv = new Vector2(0, 0);
            LineQuad.v1.Vertex.uv = new Vector2(1, 0);
            LineQuad.v2.Vertex.uv = new Vector2(0, 1);
            LineQuad.v3.Vertex.uv = new Vector2(1, 1);

            DrawQuad(ref LineQuad);
        }

        public void DrawLine(Vector2 x1, Vector2 x2, LineSpriteInfo info)
        {
            if (info.DrawEndPoints)
                DrawLineAndEndPoints(x1, x2, new Color(info.Tint), info.Width, info.End1.MyTexture, info.Sprite.MyTexture, info.End2.MyTexture, Tools.BasicEffect, info.RepeatWidth, info.Dir, 0, 0);
            else
                DrawLine(x1, x2, new Color(info.Tint), info.Width, info.Sprite.MyTexture, Tools.BasicEffect, info.RepeatWidth, info.Dir, 0, 0, info.Wrap);
        }
        public void DrawLine(Vector2 x1, Vector2 x2, LineSpriteInfo info, Vector4 Tint, float Width)
        {
            DrawLine(x1, x2, new Color(info.Tint * Tint), Width, info.Sprite.MyTexture, Tools.BasicEffect, info.RepeatWidth, info.Dir, 0, 0, info.Wrap);
        }
        public void DrawLine(Vector2 x1, Vector2 x2, LineSpriteInfo info, Vector4 Tint, float Width, float v_shift)
        {
            DrawLine(x1, x2, new Color(info.Tint * Tint), Width, info.Sprite.MyTexture, Tools.BasicEffect, info.RepeatWidth, info.Dir, 0, v_shift, info.Wrap);
        }
        public void DrawLine(Vector2 x1, Vector2 x2, Color color, float width)
        {
            DrawLine(x1, x2, color, width, null, null, 1, 0, 0f, 0, false);
        }
        public void DrawLine(Vector2 x1, Vector2 x2, Color color, float width, EzTexture Tex, EzEffect fx, float RepeatWidth, int Dir, bool Illumination)
        {
            bool Hold = LineQuad.UseGlobalIllumination;
            LineQuad.UseGlobalIllumination = Illumination;
            DrawLine(x1, x2, color, width, Tex, fx, RepeatWidth, Dir, 0f, 0, false);
            LineQuad.UseGlobalIllumination = Hold;
        }
        public void DrawLine(Vector2 x1, Vector2 x2, Color color, float width, EzTexture Tex, EzEffect fx, float RepeatWidth, int Dir, float BlendAddRatio)
        {
            DrawLine(x1, x2, color, width, Tex, fx, RepeatWidth, Dir, BlendAddRatio, 0, false);
        }
        public void DrawLine(Vector2 x1, Vector2 x2, Color color, float width, EzTexture Tex, EzEffect fx, float RepeatWidth, int Dir, float BlendAddRatio, float v_shift, bool Wrap)
        {
            color = Tools.PremultiplyAlpha(color);

            Vector2 Tangent = x2 - x1;
            Vector2 Normal = new Vector2(Tangent.Y, -Tangent.X);
            Normal.Normalize();

            LineQuad.MyEffect = DefaultEffect;
            if (fx == null)
                LineQuad.MyEffect = Tools.EffectWad.EffectList[1];
            else
                LineQuad.MyEffect = fx;

            if (Tex == null)
            {
                LineQuad.MyTexture = DefaultTexture;
            }
            else
            {
                LineQuad.MyTexture = Tex;

                float r = (x2 - x1).Length() / RepeatWidth;
                if (Dir == 0)
                {
                    LineQuad.v0.Vertex.uv = new Vector2(0, v_shift);
                    LineQuad.v1.Vertex.uv = new Vector2(1, v_shift);
                    LineQuad.v2.Vertex.uv = new Vector2(0, r + v_shift);
                    LineQuad.v3.Vertex.uv = new Vector2(1, r + v_shift);
                }
                if (Dir == 1)
                {
                    LineQuad.v0.Vertex.uv = new Vector2(v_shift, 0);
                    LineQuad.v1.Vertex.uv = new Vector2(v_shift, 1);
                    LineQuad.v2.Vertex.uv = new Vector2(r + v_shift, 0);
                    LineQuad.v3.Vertex.uv = new Vector2(r + v_shift, 1);
                }
                LineQuad.U_Wrap = LineQuad.V_Wrap = true;
            }

            LineQuad.U_Wrap = LineQuad.V_Wrap = Wrap;

            LineQuad.v0.Vertex.xy = x1 + Normal * width / 2; LineQuad.v0.Vertex.Color = color;
            LineQuad.v1.Vertex.xy = x1 - Normal * width / 2; LineQuad.v1.Vertex.Color = color;
            LineQuad.v2.Vertex.xy = x2 + Normal * width / 2; LineQuad.v2.Vertex.Color = color;
            LineQuad.v3.Vertex.xy = x2 - Normal * width / 2; LineQuad.v3.Vertex.Color = color;

            LineQuad.BlendAddRatio = BlendAddRatio;

            DrawQuad(ref LineQuad);

            LineQuad.BlendAddRatio = 0;
        }

        public void DrawLineAndEndPoints(Vector2 x1, Vector2 x2, Color color, float width, EzTexture Tex1, EzTexture Tex2, EzTexture Tex3, EzEffect fx, float RepeatWidth, int Dir, float BlendAddRatio, float v_shift)
        {
            color = Tools.PremultiplyAlpha(color);

            Vector2 Tangent = x2 - x1;
            Tangent.Normalize();

            Vector2 Mid1 = x1 + Tangent * width / Tex1.AspectRatio;
            Vector2 Mid2 = x2 - Tangent * width / Tex3.AspectRatio;

            Vector2 Normal = new Vector2(Tangent.Y, -Tangent.X);

            LineQuad.MyEffect = fx;
            LineQuad.v0.Vertex.Color = color;
            LineQuad.v1.Vertex.Color = color;
            LineQuad.v2.Vertex.Color = color;
            LineQuad.v3.Vertex.Color = color;
            LineQuad.BlendAddRatio = BlendAddRatio;

            LineQuad.v0.Vertex.uv = new Vector2(0, 1);
            LineQuad.v1.Vertex.uv = new Vector2(1, 1);
            LineQuad.v2.Vertex.uv = new Vector2(0, 0);
            LineQuad.v3.Vertex.uv = new Vector2(1, 0);
            
            // Draw end point
            LineQuad.U_Wrap = LineQuad.V_Wrap = false;
            LineQuad.MyTexture = Tex1;
            LineQuad.v0.Vertex.xy = x1 + Normal * width / 2;
            LineQuad.v1.Vertex.xy = x1 - Normal * width / 2;
            LineQuad.v2.Vertex.xy = Mid1 + Normal * width / 2;
            LineQuad.v3.Vertex.xy = Mid1 - Normal * width / 2;
            DrawQuad(ref LineQuad);


            // Draw Body
            float r = (Mid2 - Mid1).Length() / RepeatWidth;
            if (Dir == 0)
            {
                LineQuad.v0.Vertex.uv = new Vector2(0, v_shift);
                LineQuad.v1.Vertex.uv = new Vector2(1, v_shift);
                LineQuad.v2.Vertex.uv = new Vector2(0, r + v_shift);
                LineQuad.v3.Vertex.uv = new Vector2(1, r + v_shift);
            }
            if (Dir == 1)
            {
                LineQuad.v0.Vertex.uv = new Vector2(v_shift, 0);
                LineQuad.v1.Vertex.uv = new Vector2(v_shift, 1);
                LineQuad.v2.Vertex.uv = new Vector2(r + v_shift, 0);
                LineQuad.v3.Vertex.uv = new Vector2(r + v_shift, 1);
            }
            LineQuad.U_Wrap = LineQuad.V_Wrap = true;

            LineQuad.MyTexture = Tex2;
            LineQuad.v0.Vertex.xy = Mid1 + Normal * width / 2;
            LineQuad.v1.Vertex.xy = Mid1 - Normal * width / 2;
            LineQuad.v2.Vertex.xy = Mid2 + Normal * width / 2;
            LineQuad.v3.Vertex.xy = Mid2 - Normal * width / 2;
            DrawQuad(ref LineQuad);


            // Draw end point
            LineQuad.U_Wrap = LineQuad.V_Wrap = false;
            LineQuad.v0.Vertex.uv = new Vector2(0, 1);
            LineQuad.v1.Vertex.uv = new Vector2(1, 1);
            LineQuad.v2.Vertex.uv = new Vector2(0, 0);
            LineQuad.v3.Vertex.uv = new Vector2(1, 0);

            LineQuad.MyTexture = Tex3;
            LineQuad.v0.Vertex.xy = Mid2 + Normal * width / 2;
            LineQuad.v1.Vertex.xy = Mid2 - Normal * width / 2;
            LineQuad.v2.Vertex.xy = x2 + Normal * width / 2;
            LineQuad.v3.Vertex.xy = x2 - Normal * width / 2;
            DrawQuad(ref LineQuad);

            LineQuad.BlendAddRatio = 0;
        }

        public void DrawCircle(Vector2 x, float r, Color color)
        {
            DrawSquareDot(x, color, 2 * r, DefaultTexture, Tools.CircleEffect);
        }

        static float CurLightSourceFade = 0;
        public void DrawLightSource(Vector2 x, float r, float Fade, Color color)
        {
            if (Fade != CurLightSourceFade)
            {
                Tools.LightSourceEffect.effect.Parameters["Fade"].SetValue(Fade);
                CurLightSourceFade = Fade;
            }
            DrawSquareDot(x, color, 2 * r, DefaultTexture, Tools.LightSourceEffect);
        }

        public void DrawSquareDot(Vector2 x, Color color, float width)
        {
            DrawSquareDot(x, color, width, null, null);
        }
        public void DrawSquareDot(Vector2 x, Color color, float width, EzTexture Tex, EzEffect fx)
        {
            color = Tools.PremultiplyAlpha(color);

            LineQuad.MyEffect = DefaultEffect;
            if (fx == null)
                LineQuad.MyEffect = Tools.EffectWad.EffectList[2];
            else
                LineQuad.MyEffect = fx;

            if (Tex == null)
            {
                LineQuad.MyTexture = DefaultTexture;
            }
            else
            {
                LineQuad.MyTexture = Tex;
            }

            LineQuad.v0.Vertex.xy = x + new Vector2(-1, 1) * width / 2; LineQuad.v0.Vertex.Color = color;
            LineQuad.v1.Vertex.xy = x + new Vector2(1, 1) * width / 2; LineQuad.v1.Vertex.Color = color;
            LineQuad.v2.Vertex.xy = x + new Vector2(-1, -1) * width / 2; LineQuad.v2.Vertex.Color = color;
            LineQuad.v3.Vertex.xy = x + new Vector2(1, -1) * width / 2; LineQuad.v3.Vertex.Color = color;

            LineQuad.v0.Vertex.uv = new Vector2(0, 0);
            LineQuad.v1.Vertex.uv = new Vector2(1, 0);
            LineQuad.v2.Vertex.uv = new Vector2(0, 1);
            LineQuad.v3.Vertex.uv = new Vector2(1, 1);

            LineQuad.U_Wrap = LineQuad.V_Wrap = false;

            DrawQuad(ref LineQuad);
        }


        public void DrawToScaleQuad(Vector2 x, Color color, float width)
        {
            DrawToScaleQuad(x, color, width, null, null);
        }
        public void DrawToScaleQuad(Vector2 x, Color color, float width, EzTexture Tex, EzEffect fx)
        {
            color = Tools.PremultiplyAlpha(color);

            LineQuad.MyEffect = DefaultEffect;
            if (fx == null)
                LineQuad.MyEffect = Tools.EffectWad.EffectList[2];
            else
                LineQuad.MyEffect = fx;

            if (Tex == null)
            {
                LineQuad.MyTexture = DefaultTexture;
            }
            else
            {
                LineQuad.MyTexture = Tex;
            }

            float a = width / LineQuad.MyTexture.AspectRatio;

            LineQuad.v0.Vertex.xy = x + new Vector2(-width, a) / 2; LineQuad.v0.Vertex.Color = color;
            LineQuad.v1.Vertex.xy = x + new Vector2(width, a) / 2; LineQuad.v1.Vertex.Color = color;
            LineQuad.v2.Vertex.xy = x + new Vector2(-width, -a) / 2; LineQuad.v2.Vertex.Color = color;
            LineQuad.v3.Vertex.xy = x + new Vector2(width, -a) / 2; LineQuad.v3.Vertex.Color = color;

            LineQuad.v0.Vertex.uv = new Vector2(0, 0);
            LineQuad.v1.Vertex.uv = new Vector2(1, 0);
            LineQuad.v2.Vertex.uv = new Vector2(0, 1);
            LineQuad.v3.Vertex.uv = new Vector2(1, 1);


            DrawQuad(ref LineQuad);
        }

        public void DrawBox(Vector2 BL, Vector2 TR, Color color, float width)
        {
            Vector2 BR = new Vector2(TR.X, BL.Y);
            Vector2 TL = new Vector2(BL.X, TR.Y);
            Vector2 Offset = new Vector2(Math.Sign(TR.X - BL.X) * width / 2, 0);

            DrawLine(BL - Offset, BR + Offset, color, width);
            DrawLine(BR, TR, color, width);
            DrawLine(TR + Offset, TL - Offset, color, width);
            DrawLine(TL, BL, color, width);
        }

        public void SetToDefaultTexture()
        {
            Flush();
            CurrentTexture = DefaultTexture;
            CurrentEffect.xTexture.SetValue(CurrentTexture.Tex);
        }

        /// <summary>
        /// Makes sure the current texture isn't set to any RenderTargets
        /// </summary>
        public void WashTexture()
        {
            CurrentTexture = Tools.TextureWad.DefaultTexture;
        }

        public void Flush()
        {
//#if DEBUG
//            if (Device.SamplerStates[1] == null) Tools.Write("!");
//            if (Device.SamplerStates[1] != ClampClamp) Tools.Write("!");
//#endif

            if (CurrentTexture != null)
            {
                CurrentEffect.xTexture.SetValue(CurrentTexture.Tex);
                CurrentEffect.Hsl.SetValue(CurrentMatrix);

                if (!CurrentEffect.IsUpToDate)
                    CurrentEffect.SetCameraParameters();

                if (i > 0)
                {
                    CurrentEffect.effect.CurrentTechnique.Passes[0].Apply();

                    Device.DrawUserPrimitives<MyOwnVertexFormat>(PrimitiveType.TriangleList, Vertices, 0, TrianglesInBuffer);
                }
            }

            TrianglesInBuffer = 0;
            i = 0;
        }
    }
}