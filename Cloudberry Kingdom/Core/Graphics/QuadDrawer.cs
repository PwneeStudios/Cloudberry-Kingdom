using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CloudberryKingdom;

namespace CoreEngine
{
    public class HackSpriteFont
    {
        public HackFont font;
        public int thickness;

        public HackSpriteFont(HackFont font_, int thickness_)
        {
            font = font_;
            thickness = thickness_;
        }
    }

    public class HackFont
    {
        public struct GlyphData
        {
            public Vector4 TextureCoordinates;
            public Vector2 Size;
            public Vector2 Offset;

            public GlyphData(Vector4 TextureCoordinates_, Vector2 Size_, Vector2 Offset_)
            {
                TextureCoordinates = TextureCoordinates_;
                Size = Size_;
                Offset = Offset_;
            }
        }

        Dictionary<int, GlyphData> Data = new Dictionary<int, GlyphData>(5000);

        /// Texture path.
        string texturePath_;

        /// Spacing offset between characters.
        int charSpacing_;

        public EzTexture MyTexture;

		public GlyphData GetData(char c, bool MakeMonospaced)
        {
			GlyphData data;

			if (Data.ContainsKey(c))
			{
				data = Data[c];
			}
			else
			{
				switch (c)
				{ 
					case 'Ő': data = Data['O']; break;
					case 'Ű': data = Data['U']; break;
					case 'ő': data = Data['o']; break;
					case 'ű': data = Data['u']; break;

					default: data = Data['#']; break;
				}
			}

			if (MakeMonospaced)
			{
				if (c == ',')
					data.Size.X = 40;
				else if (c == '.')
					data.Size.X = 40;
				else
					data.Size.X = 60;
			}

			return data;
        }

        public float CharSpacing;

        public HackFont(string name)
        {
            MyTexture = new EzTexture();
            MyTexture.Tex = Localization.FontTexture;

            var file = new StreamReader(File.OpenRead("Content/Fonts/" + name + ".fnt"));

            file.ReadLine(); // Burn one line
            file.ReadLine(); // Burn one line

            var line = file.ReadLine();
            int Char, X, Y, Width, Height, Xoffset, Yoffset, OrigW, OrigH;
            while (line != null)
            {
                int additional = 19;
                var data = line.Split('\t');
                Char = int.Parse(data[0]);
                X = int.Parse(data[1]);
                Y = int.Parse(data[2]);
                Width = int.Parse(data[3]) + additional;
                Height = int.Parse(data[4]) + additional;
                Xoffset = int.Parse(data[5]);
                Yoffset = int.Parse(data[6]);
                OrigW = int.Parse(data[7]) + additional;
                OrigH = int.Parse(data[8]) + additional;


				if (Data.ContainsKey(Char))
				{
					Console.WriteLine(Char);
				}
				else
				{
					Data.Add(Char, new GlyphData(
						new Vector4(X, Y, Width, Height),
						new Vector2(OrigW, OrigH),
						new Vector2(Xoffset, Yoffset)));
				}

                line = file.ReadLine();
            }
        }
    }

    public class QuadDrawer
    {
		SpriteBatch MySpriteBatch;
		public void StartSpriteBatch()
		{
			MySpriteBatch.Begin();
		}

		public void EndSpriteBatch()
		{
			MySpriteBatch.End();
		}

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
                CurrentMatrixSignature = ColorHelper.MatrixSignature(_CurrentMatrix);
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
			MySpriteBatch = new SpriteBatch(Device);

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

        public void SetColorMatrix(Matrix m, float signature)
        {
            if (CurrentMatrixSignature != signature)
            {
                Flush();

                CurrentMatrixSignature = signature;
                CurrentMatrix = m;

                Tools.HslEffect.Hsl.SetValue(CurrentMatrix);
            }
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

            if (i + 6 >= N ||
                CurrentEffect.effect != quad.MyEffect.effect ||
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
/*
            if (quad.Hide) return;

			//Vector2 pos = (quad.TR + quad.BL) / 2;
			Vector2 pos = (quad.v0.Vertex.xy + quad.v3.Vertex.xy) / 2;
			Vector2 campos = Tools.CurCamera.Pos;
			Vector2 screensize = new Vector2(Tools.TheGame.MyGraphicsDeviceManager.PreferredBackBufferWidth, Tools.TheGame.MyGraphicsDeviceManager.PreferredBackBufferHeight);
			//Vector2 camscale = Tools.CurCamera.Zoom;
			Vector2 camscale = Tools.CurCamera.TR - Tools.CurCamera.BL;
			
			pos = (pos - campos) / camscale * screensize;
			
			pos.X =  pos.X + .5f * screensize.X;
			pos.Y = -pos.Y + .5f * screensize.Y;

			Vector2 tex_size = new Vector2(Tools.TextureWad.DefaultTexture.Width, Tools.TextureWad.DefaultTexture.Height);
			//Vector2 scale  = .5f * (quad.TR - quad.BL) / tex_size;
			Vector2 scale = .5f * (quad.v0.Vertex.xy - quad.v3.Vertex.xy) / tex_size;
			scale.X = Math.Abs(scale.X);

			MySpriteBatch.Draw(Tools.TextureWad.DefaultTexture.Tex, pos, null, quad.v0.Vertex.Color, 0, .5f * tex_size, scale, SpriteEffects.None, 0);
*/









			if (quad.Hide) return;

			if (quad.MyEffect == null || quad.MyTexture == null) { Tools.Break(); return; }

			if (Tools.Render.UsingSpriteBatch) Tools.Render.EndSpriteBatch();

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

            color = ColorHelper.PremultiplyAlpha(color);

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


		public void DrawLine(Vector2 x1, Vector2 x2, LineSpriteInfo info, int RepeatWidth)
		{
			if (info.DrawEndPoints)
				DrawLineAndEndPoints(x1, x2, new Color(info.Tint), info.Width, info.End1.MyTexture, info.Sprite.MyTexture, info.End2.MyTexture, Tools.BasicEffect, RepeatWidth, info.Dir, 0, 0);
			else
				DrawLine(x1, x2, new Color(info.Tint), info.Width, info.Sprite.MyTexture, Tools.BasicEffect, RepeatWidth, info.Dir, 0, 0, info.Wrap);
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
            color = ColorHelper.PremultiplyAlpha(color);

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
            color = ColorHelper.PremultiplyAlpha(color);

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
            color = ColorHelper.PremultiplyAlpha(color);

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
            color = ColorHelper.PremultiplyAlpha(color);

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

            //if (CurrentTexture != null)
            if (CurrentTexture != null && i > 0)
            {
                CurrentEffect.xTexture.SetValue(CurrentTexture.Tex);
                CurrentEffect.Hsl.SetValue(CurrentMatrix);

                // Test HSV transform
                //CurrentEffect.Hsl.SetValue(ColorHelper.HsvTransform(Tools.Num_0_to_2, 1f, Tools.Num_0_to_360));

                if (!CurrentEffect.IsUpToDate)
                    CurrentEffect.SetCameraParameters();

                //if (i > 0)
                {
                    CurrentEffect.effect.CurrentTechnique.Passes[0].Apply();

                    Device.DrawUserPrimitives<MyOwnVertexFormat>(PrimitiveType.TriangleList, Vertices, 0, TrianglesInBuffer);
                }
            }

            TrianglesInBuffer = 0;
            i = 0;
        }

        public void DrawPic(Vector2 pos, Vector2 pos2, EzTexture texture, Color color)
        {
            if (CurrentTexture != texture || CurrentEffect != Tools.BasicEffect)
                Flush();

            CurrentTexture = texture;
            CurrentEffect = Tools.BasicEffect;

            Vertices[i].Color =
            Vertices[i + 5].Color = Vertices[i + 1].Color =
            Vertices[i + 4].Color = Vertices[i + 2].Color =
            Vertices[i + 3].Color = color;

            Vertices[i].xy = new Vector2(pos.X, pos.Y);
            Vertices[i + 5].xy = Vertices[i + 1].xy = new Vector2(pos.X, pos2.Y);
            Vertices[i + 4].xy = Vertices[i + 2].xy = new Vector2(pos2.X, pos.Y);
            Vertices[i + 3].xy = new Vector2(pos2.X, pos2.Y);

            Vertices[i].uv = new Vector2(0, 0);
            Vertices[i + 5].uv = Vertices[i + 1].uv = new Vector2(0, 1);
            Vertices[i + 4].uv = Vertices[i + 2].uv = new Vector2(1, 0);
            Vertices[i + 3].uv = new Vector2(1, 1);

            i += 6;
            TrianglesInBuffer += 2;
        }

		public void DrawString(HackSpriteFont spritefont, string s, Vector2 position, Vector4 color, Vector2 scale)
		{
			DrawString(spritefont, s, position, color, scale, false);
		}
        public void DrawString(HackSpriteFont spritefont, string s, Vector2 position, Vector4 color, Vector2 scale, bool MakeMonospaced)
        {
			if (CloudberryKingdomGame.ForceSuperPause) return;

            HackFont font = spritefont.font;

            scale *= 1.12f;

            EzEffect fx = null;
            switch (spritefont.thickness)
            {
                case 0: fx = Tools.Text_NoOutline; break;
                case 1: fx = Tools.Text_ThinOutline; break;
                case 2: fx = Tools.Text_ThickOutline; break;
                default: fx = null; return;
            }

            if (CurrentTexture != font.MyTexture || i > 1000 || fx != CurrentEffect)
                Flush();

            CurrentTexture = font.MyTexture;
            CurrentEffect = fx;

            Vector2 p = position + new Vector2(35, -25) * scale / 2.0533333f;
	        for (int j = 0; j < s.Length; ++j)
	        {
                HackFont.GlyphData data = font.GetData(s[j], MakeMonospaced);

				// Correct for initial offset
				if (j == 0)
				{
					switch (s[0])
					{
						case 'C': p.X -= 3.25f * scale.X; break;
						case 'L': p.X += 1.00f * scale.X; break;
						case 'P': p.X -= 1.00f * scale.X; break;
						default: break;
					}
				}

                Vector4 tq = data.TextureCoordinates;
                Vector2 d = data.Size;
                Vector2 l = p + new Vector2(data.Offset.X, -data.Offset.Y) * scale;


				if (s[j] != '・')
				if (s[j] != '　')
                if (s[j] != ' ')
                {
                    Vector2 inv_size = Vector2.One / new Vector2(font.MyTexture.Tex.Width, font.MyTexture.Tex.Height);

                    Vertices[i].Color =
                    Vertices[i + 5].Color = Vertices[i + 1].Color =
                    Vertices[i + 4].Color = Vertices[i + 2].Color =
                    Vertices[i + 3].Color = new Color(color);

                    Vertices[i].xy = new Vector2(l.X, l.Y);
                    Vertices[i + 5].xy = Vertices[i + 1].xy = new Vector2(l.X, l.Y - tq.W * scale.Y);
                    Vertices[i + 4].xy = Vertices[i + 2].xy = new Vector2(l.X + tq.Z * scale.X, l.Y);
                    Vertices[i + 3].xy = new Vector2(l.X + tq.Z * scale.X, l.Y - tq.W * scale.Y);

                    Vertices[i].uv = new Vector2(tq.X, tq.Y) * inv_size;
                    Vertices[i + 5].uv = Vertices[i + 1].uv = new Vector2(tq.X, tq.Y + tq.W) * inv_size;
                    Vertices[i + 4].uv = Vertices[i + 2].uv = new Vector2(tq.X + tq.Z, tq.Y) * inv_size;
                    Vertices[i + 3].uv = new Vector2(tq.X + tq.Z, tq.Y + tq.W) * inv_size;

                    i += 6;
                    TrianglesInBuffer += 2;
                }

				if (s[j] != '・')
				{
					p += new Vector2(d.X + font.CharSpacing - 18, 0) * scale;					
				}

                //Flush();
	        }
        }

		public void DrawString(HackSpriteFont spritefont, StringBuilder s, Vector2 position, Vector4 color, Vector2 scale)
		{
			DrawString(spritefont, s, position, color, scale, false);
		}
        public void DrawString(HackSpriteFont spritefont, StringBuilder s, Vector2 position, Vector4 color, Vector2 scale, bool MakeMonospaced)
        {
            HackFont font = spritefont.font;

            scale *= 1.12f;

            EzEffect fx = null;
            switch (spritefont.thickness)
            {
                case 0: fx = Tools.Text_NoOutline; break;
                case 1: fx = Tools.Text_ThinOutline; break;
                case 2: fx = Tools.Text_ThickOutline; break;
                default: fx = null; return;
            }

            if (CurrentTexture != font.MyTexture || i > 1000 || fx != CurrentEffect)
                Flush();

            CurrentTexture = font.MyTexture;
            CurrentEffect = fx;

            Vector2 p = position + new Vector2(35, -25) * scale / 2.0533333f;
            for (int j = 0; j < s.Length; ++j)
            {
				if (i + 10 > N) Flush();

                HackFont.GlyphData data = font.GetData(s[j], MakeMonospaced);

                Vector4 tq = data.TextureCoordinates;
                Vector2 d = data.Size;
                Vector2 l = p + new Vector2(data.Offset.X, -data.Offset.Y) * scale;

                if (s[j] != ' ')
                {
                    Vector2 inv_size = Vector2.One / new Vector2(font.MyTexture.Tex.Width, font.MyTexture.Tex.Height);

                    Vertices[i].Color =
                    Vertices[i + 5].Color = Vertices[i + 1].Color =
                    Vertices[i + 4].Color = Vertices[i + 2].Color =
                    Vertices[i + 3].Color = new Color(color);

                    Vertices[i].xy = new Vector2(l.X, l.Y);
                    Vertices[i + 5].xy = Vertices[i + 1].xy = new Vector2(l.X, l.Y - tq.W * scale.Y);
                    Vertices[i + 4].xy = Vertices[i + 2].xy = new Vector2(l.X + tq.Z * scale.X, l.Y);
                    Vertices[i + 3].xy = new Vector2(l.X + tq.Z * scale.X, l.Y - tq.W * scale.Y);

                    Vertices[i].uv = new Vector2(tq.X, tq.Y) * inv_size;
                    Vertices[i + 5].uv = Vertices[i + 1].uv = new Vector2(tq.X, tq.Y + tq.W) * inv_size;
                    Vertices[i + 4].uv = Vertices[i + 2].uv = new Vector2(tq.X + tq.Z, tq.Y) * inv_size;
                    Vertices[i + 3].uv = new Vector2(tq.X + tq.Z, tq.Y + tq.W) * inv_size;

                    i += 6;
                    TrianglesInBuffer += 2;
                }

                p += new Vector2(d.X + font.CharSpacing - 18, 0) * scale;

                //Flush();
            }
        }

		public Vector2 MeasureString(HackSpriteFont spritefont, string s)
		{
			return MeasureString(spritefont, s, false);
		}
        public Vector2 MeasureString(HackSpriteFont spritefont, string s, bool MakeMonospaced)
        {
            HackFont font = spritefont.font;

	        Vector2 size = Vector2.Zero;

	        if ( s.Length == 0 ) return Vector2.Zero;

	        for( int j = 0; j < s.Length; ++j )
	        {
				HackFont.GlyphData data = font.GetData(s[j], MakeMonospaced);
                Vector2 dim = data.Size;

		        size.X += dim.X + (float)( font.CharSpacing - 18 );
		        size.Y = Math.Max( size.Y, dim.Y );
	        }

            size.Y = Math.Max( size.Y, 133 );

	        size = size - new Vector2( (float)( font.CharSpacing ), 0 ) + new Vector2(50, 0);
            size *= 1.12f;

            if (size.X < 0) Tools.Nothing();

            return size;
        }

		public Vector2 MeasureString(HackSpriteFont spritefont, StringBuilder s)
		{
			return MeasureString(spritefont, s, false);
		}
        public Vector2 MeasureString(HackSpriteFont spritefont, StringBuilder s, bool MakeMonospaced)
        {
            HackFont font = spritefont.font;

	        Vector2 size = Vector2.Zero;

	        if ( s.Length == 0 ) return Vector2.Zero;

	        for( int j = 0; j < s.Length; ++j )
	        {
                HackFont.GlyphData data = font.GetData(s[j], MakeMonospaced);
                Vector2 dim = data.Size;

		        size.X += dim.X + (float)( font.CharSpacing - 18 );
		        size.Y = Math.Max( size.Y, dim.Y );
	        }

            size.Y = Math.Max( size.Y, 133 );

	        size = size - new Vector2( (float)( font.CharSpacing ), 0 ) + new Vector2(50, 0);
            size *= 1.12f;

            if (size.X < 0) Tools.Nothing();

            return size;
        }
    }
}