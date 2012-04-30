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
            /*
            // Calculate illumination
            if (quad.UseGlobalIllumination)
                Illumination = GlobalIllumination * quad.Illumination;
            else
                Illumination = quad.Illumination;
            //Illumination = GlobalIllumination;

            if (i + 6 > N ||
                i != 0 &&
                    (CurrentEffect.effect != quad.MyEffect.effect ||
                     CurrentTexture.Tex != quad.MyTexture.Tex && !quad.MyTexture.FromPacked ||
                     CurrentEffect.CurrentIllumination != Illumination))
                Flush();

            if (i == 0)
            {
                CurrentEffect = quad.MyEffect;
                CurrentTexture = quad.MyTexture;

                if (CurrentTexture.FromPacked)
                    CurrentTexture = CurrentTexture.Packed;

                //if (CurrentIllumination != Illumination)
                //{
                //    CurrentIllumination = Illumination;
                //    CurrentEffect.effect.Parameters["Illumination"].SetValue(CurrentIllumination);
                //}
            }
            
            CurrentEffect.CurrentIllumination = 1;
            CurrentEffect.effect.Parameters["Illumination"].SetValue(1);
            */



            if (i + 6 > N ||
                i != 0 && (CurrentEffect.effect != quad.MyEffect.effect || CurrentTexture.Tex != quad.MyTexture.Tex))
                Flush();

            if (i == 0)
            {
                CurrentEffect = quad.MyEffect;
                CurrentTexture = quad.MyTexture;
            }
            CurrentEffect.CurrentIllumination = 1;
            CurrentEffect.effect.Parameters["Illumination"].SetValue(1);
            


            Vertices[i] = quad.Vertices[0];
            Vertices[i + 1] = quad.Vertices[1];
            Vertices[i + 2] = quad.Vertices[2];

            Vertices[i + 3] = quad.Vertices[3];
            Vertices[i + 4] = quad.Vertices[2];
            Vertices[i + 5] = quad.Vertices[1];

            i += 6;
            TrianglesInBuffer += 2;
        }

        void SetSamplerState()
        {
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
    
        /*
        public void SetAddressUMode(TextureAddressMode mode)
        {
            Current_U_Wrap = mode == TextureAddressMode.Wrap;
            SetSamplerState();
            //Device.SamplerStates[1].AddressU = mode;
        }
        public void SetAddressVMode(TextureAddressMode mode)
        {
            Current_V_Wrap = mode == TextureAddressMode.Wrap;
            SetSamplerState();
            //Device.SamplerStates[1].AddressV = mode;
        }*/

        public void SetAddressMode(bool U_Wrap, bool V_Wrap)
        {
            Current_U_Wrap = U_Wrap;
            Current_V_Wrap = V_Wrap;
            SetSamplerState();
        }

        public void DrawQuad_Simplified(SimpleQuad quad)
        {
            if (CurrentEffect.effect != quad.MyEffect.effect ||
                CurrentTexture.Tex != quad.MyTexture.Tex && !quad.MyTexture.FromPacked ||
                CurrentTexture != quad.MyTexture.Packed && quad.MyTexture.FromPacked)
                Flush();

            if (i == 0)
            {
                if (Current_U_Wrap != quad.U_Wrap || Current_V_Wrap != quad.V_Wrap)
                    SetAddressMode(quad.U_Wrap, quad.V_Wrap);

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

            //CurrentEffect.effect.Parameters["Illumination"].SetValue(.5f);
            Vertices[i] = quad.v0.Vertex;
            Vertices[i + 5] = Vertices[i + 1] = quad.v1.Vertex;
            Vertices[i + 4] = Vertices[i + 2] = quad.v2.Vertex;
            Vertices[i + 3] = quad.v3.Vertex;

            i += 6;
            TrianglesInBuffer += 2;
        }

        public void DrawQuad(SimpleQuad quad)
        {
            if (quad.Hide) return;

            if (quad.MyEffect == null || quad.MyTexture == null) return;

            if (Tools.UsingSpriteBatch) Tools.EndSpriteBatch();

            // Calculate illumination
            if (quad.UseGlobalIllumination)
                Illumination = GlobalIllumination * quad.Illumination;
            else
                Illumination = quad.Illumination;

            if (i + 6 > N ||
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

                if (CurrentTexture.FromPacked)
                    CurrentTexture = CurrentTexture.Packed;

                if (CurrentEffect.CurrentIllumination != Illumination)
                {
                    CurrentEffect.CurrentIllumination = Illumination;
                    CurrentEffect.Illumination.SetValue(Illumination);
                }
            }
            
            //CurrentEffect.effect.Parameters["Illumination"].SetValue(.5f);
            Vertices[i] = quad.v0.Vertex;
            Vertices[i + 1] = quad.v1.Vertex;
            Vertices[i + 2] = quad.v2.Vertex;

            Vertices[i + 3] = quad.v3.Vertex;
            Vertices[i + 4] = quad.v2.Vertex;
            Vertices[i + 5] = quad.v1.Vertex;

            i += 6;
            TrianglesInBuffer += 2;
        }

        public void DrawFilledBox(Vector2 BL, Vector2 TR, Color color)
        {
            LineQuad.MyEffect = DefaultEffect;
            LineQuad.MyEffect = Tools.EffectWad.EffectList[0];
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

            DrawQuad(LineQuad);
        }

        public void DrawLine(Vector2 x1, Vector2 x2, Color color, float width)
        {
            DrawLine(x1, x2, color, width, null, null, 1, 0, 0f);
        }
        public void DrawLine(Vector2 x1, Vector2 x2, Color color, float width, EzTexture Tex, EzEffect fx, float RepeatWidth, int Dir, bool Illumination)
        {
            bool Hold = LineQuad.UseGlobalIllumination;
            LineQuad.UseGlobalIllumination = Illumination;
            DrawLine(x1, x2, color, width, Tex, fx, RepeatWidth, Dir, 0f);
            LineQuad.UseGlobalIllumination = Hold;
        }
        public void DrawLine(Vector2 x1, Vector2 x2, Color color, float width, EzTexture Tex, EzEffect fx, float RepeatWidth, int Dir, float BlendAddRatio)
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
                    LineQuad.v0.Vertex.uv = new Vector2(0, 0);
                    LineQuad.v1.Vertex.uv = new Vector2(1, 0);
                    LineQuad.v2.Vertex.uv = new Vector2(0, r);
                    LineQuad.v3.Vertex.uv = new Vector2(1, r);
                }
                if (Dir == 1)
                {
                    LineQuad.v0.Vertex.uv = new Vector2(0, 0);
                    LineQuad.v1.Vertex.uv = new Vector2(0, 1);
                    LineQuad.v2.Vertex.uv = new Vector2(r, 0);
                    LineQuad.v3.Vertex.uv = new Vector2(r, 1);
                }
                LineQuad.U_Wrap = LineQuad.V_Wrap = true;
            }

            LineQuad.v0.Vertex.xy = x1 + Normal * width / 2; LineQuad.v0.Vertex.Color = color;
            LineQuad.v1.Vertex.xy = x1 - Normal * width / 2; LineQuad.v1.Vertex.Color = color;
            LineQuad.v2.Vertex.xy = x2 + Normal * width / 2; LineQuad.v2.Vertex.Color = color;
            LineQuad.v3.Vertex.xy = x2 - Normal * width / 2; LineQuad.v3.Vertex.Color = color;

            LineQuad.BlendAddRatio = BlendAddRatio;

            DrawQuad(LineQuad);

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


            DrawQuad(LineQuad);
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
            if (CurrentTexture != null)
            {
                CurrentEffect.xTexture.SetValue(CurrentTexture.Tex);

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