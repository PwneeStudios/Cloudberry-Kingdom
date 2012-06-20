using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Drawing;

namespace CloudberryKingdom
{
    public class QuadClass : IViewable, IReadWrite
    {
        public string[] GetViewables()
        {
            return new string[] { };
        }

        public static QuadClass FindQuad(List<QuadClass> list, string Name)
        {
            return list.Find(quad => string.Compare(quad.Name, Name, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public bool HitTest(Vector2 pos) { return HitTest(pos, Vector2.Zero); }
        public bool HitTest(Vector2 pos, Vector2 padding)
        {
            if (!Show) return false;

            Update();
            
            if (pos.X > TR.X + padding.X) return false;
            if (pos.X < BL.X - padding.X) return false;
            if (pos.Y > TR.Y + padding.Y) return false;
            if (pos.Y < BL.Y - padding.Y) return false;

            return true;
        }

        public bool HitTest_WithParallax(Vector2 pos, Vector2 padding, float Parallax)
        {
            if (!Show) return false;

            Update();

            var c = Tools.CurCamera.Pos;

            if (Base.e1.X > 0)
            {
                if (pos.X > Parallax * (TR.X - c.X) + c.X + padding.X) return false;
                if (pos.X < Parallax * (BL.X - c.X) + c.X - padding.X) return false;
                if (pos.Y > Parallax * (TR.Y - c.Y) + c.Y + padding.Y) return false;
                if (pos.Y < Parallax * (BL.Y - c.Y) + c.Y - padding.Y) return false;
            }
            else
            {
                if (pos.X > Parallax * (BL.X - c.X) + c.X + padding.X) return false;
                if (pos.X < Parallax * (TR.X - c.X) + c.X - padding.X) return false;
                if (pos.Y > Parallax * (TR.Y - c.Y) + c.Y + padding.Y) return false;
                if (pos.Y < Parallax * (BL.Y - c.Y) + c.Y - padding.Y) return false;
            }

            return true;
        }

        public Vector2 TR { get { return Quad.v1.Vertex.xy; } }
        public Vector2 BL { get { return Quad.v2.Vertex.xy; } }

        public float Left
        {
            get { return BL.X; }
            set { X = value + Size.X; }
        }

        public float X { get { return Pos.X; } set { Pos = new Vector2(value, Pos.Y); } }
        public float Y { get { return Pos.Y; } set { Pos = new Vector2(Pos.X, value); } }

        public bool Shadow = false;
        public Vector2 ShadowOffset = Vector2.Zero;
        public Color ShadowColor = Color.Black;
        public float ShadowScale = 1f;

        public SimpleQuad Quad = new SimpleQuad();
        
        public BasePoint Base;
        public FancyVector2 FancyPos, FancyScale, FancyAngle;

        public FancyVector2 FancyLightAlpha;
        public void MakeLightAlpha()
        {
            if (FancyLightAlpha != null) return;

            FancyLightAlpha = new FancyVector2();
        }

        public void SetDefaultShadow() { SetDefaultShadow(new Vector2(12, 12), new Color(.2f, .2f, .2f, 1f)); }
        public void SetDefaultShadow(float Offset) { SetDefaultShadow(new Vector2(Offset, Offset), new Color(.2f, .2f, .2f, 1f)); }
        public void SetDefaultShadow(Vector2 offset, Color color)
        {
            Shadow = true;
            ShadowColor = color;
            ShadowOffset = offset;
        }

        /// <summary>
        /// Name of the quad, used in DrawPiles
        /// </summary>
        public string Name = "";

        /// <summary>
        /// Layer of the quad, used in DrawPiles
        /// </summary>
        public int Layer = 0;

        public void MakeFancyPos()
        {
            if (FancyPos != null) FancyPos.Release();
            FancyPos = new FancyVector2();
            FancyPos.RelVal = Base.Origin;
        }

        public void Release()
        {
            if (FancyPos != null) FancyPos.Release(); FancyPos = null;
            if (FancyScale != null) FancyScale.Release(); FancyScale = null;
            if (FancyAngle != null) FancyAngle.Release(); FancyAngle = null;
        }

        public Vector2 LightAlpha
        {
            get {
                MakeLightAlpha();
                return FancyLightAlpha.RelVal; }
            set {
                MakeLightAlpha();
                FancyLightAlpha.RelVal = value; }
        }

        public Vector2 Pos
        {
            get
            {
                if (FancyPos == null)
                    return Base.Origin;
                else
                    return FancyPos.RelVal;
            }
            set
            {
                if (FancyPos == null)
                    Base.Origin = value;
                else
                    FancyPos.RelVal = value;
            }
        }
        public float PosY
        {
            get { return Pos.Y; }
            set { Pos = new Vector2(Pos.X, value); }
        }

        public float SizeX { get { return Size.X; } set { Size = new Vector2(value, Size.Y); } }
        public float SizeY { get { return Size.Y; } set { Size = new Vector2(Size.X, value); } }
        public Vector2 Size
        {
            get
            {
                if (FancyScale == null)
                    return new Vector2(Base.e1.Length(), Base.e2.Length());
                else
                    return FancyScale.RelVal;
            }
            set
            {
                if (FancyScale == null)
                {
                    Rescale(value);
                }
                else
                    FancyScale.RelVal = value;
            }
        }

        public float Degrees
        {
            get { return Tools.Degrees(Angle); }
            set { Angle = Tools.Radians(value); }
        }

        public float Angle
        {
            get
            {
                if (FancyAngle == null)
                    return 0;
                else
                    return FancyAngle.RelVal.X;
            }
            set
            {
                if (FancyAngle == null)
                    return;
                else
                    FancyAngle.RelValX = value;
            }
        }

        public QuadClass(Quad quad)
        {
            Initialize(null, false, false);
            Quad = new SimpleQuad(quad);
        }

        public QuadClass(QuadClass quad)
        {
            Initialize(null, false, false);
            quad.Clone(this);
        }

        public QuadClass()
        {
            Initialize(null, false, false);
        }
        public QuadClass(FancyVector2 Center)
        {
            Initialize(Center, false, false);
        }
        public QuadClass(FancyVector2 Center, bool UseFancySize)
        {
            Initialize(Center, UseFancySize, false);
        }
        public QuadClass(FancyVector2 Center, bool UseFancySize, bool UseFancyAngle)
        {
            Initialize(Center, UseFancySize, UseFancyAngle);
        }

        public void Initialize(FancyVector2 Center, bool UseFancySize, bool UseFancyAngle)
        {
            Quad.Init();
            Base.Init();

            SetToDefault();

            if (Center != null)
            {
                FancyPos = new FancyVector2();
                FancyPos.Center = Center;
            }

            if (UseFancySize)
            {
                FancyScale = new FancyVector2();
            }

            if (UseFancyAngle)
            {
                FancyAngle = new FancyVector2();
            }
        }

        /// <summary>
        /// The name of the quad's texture. Setting will automatically search the TextureWad for a matching texture.
        /// </summary>
        public string TextureName
        {
            get { return Quad.MyTexture.Path; }
            set { Quad.MyTexture = Tools.TextureWad.FindByName(value); }
        }

        /// <summary>
        /// The name of the quad's Effect. Setting will automatically search the EffectWad for a matching Effect.
        /// </summary>
        public string EffectName
        {
            get { return Quad.MyEffect.Name; }
            set { Quad.MyEffect = Tools.EffectWad.FindByName(value); }
        }

        public QuadClass Clone()
        {
            QuadClass cloned = new QuadClass();
            Clone(cloned);
            return cloned;
        }

        /// <summary>
        /// Copy this quads' properties TO another quad
        /// </summary>
        public void Clone(QuadClass quad)
        {
            quad.Base = Base;

            quad.Quad.MyTexture = Quad.MyTexture;
            quad.Quad.MyEffect = Quad.MyEffect;
            
            quad.Quad.SetColor(Quad.MySetColor);
        }

        public void SetTexture(string Name)
        {
            Quad.MyTexture = Tools.TextureWad.FindByName(Name);
        }

        public float WidthToScreenWidthRatio(Camera cam)
        {
            return .5f * Size.X / cam.GetWidth();
        }

        public void ScaleToTextureSize()
        {
            if (Quad.MyTexture != null)
            {
                Size = new Vector2(Quad.TexWidth, Quad.TexHeight);
                //Base.e1 = new Vector2(Quad.TexWidth, 0);
                //Base.e2 = new Vector2(0, Quad.TexHeight);
            }
        }

        public void Rescale(Vector2 size)
        {
            Base.e1.Normalize();
            Base.e1 *= size.X;
            Base.e2.Normalize();
            Base.e2 *= size.Y;
        }

        public void Scale(float scale)
        {
            Size *= scale;
            //Base.e1 *= scale;
            //Base.e2 *= scale;
        }

        /// <summary>
        /// Gets the scaling of the quad relative to its texture's size
        /// </summary>
        public Vector2 GetTextureScaling()
        {
            return new Vector2(Size.X / Quad.TexWidth,
                               Size.Y / Quad.TexHeight);
        }

        public void ScaleXToMatchRatio()
        {
            ScaleXToMatchRatio(Size.Y);
        }
        public void ScaleXToMatchRatio(float height)
        {
            if (Quad.MyTexture.Load())
                Size = new Vector2(height * Quad.TexWidth / Quad.TexHeight, height);
        }

        public void ScaleYToMatchRatio()
        {
            ScaleYToMatchRatio(Size.X);
        }
        public void ScaleYToMatchRatio(float width)
        {
            if (Quad.MyTexture.Load())
                Size = new Vector2(width, width * Quad.TexHeight / Quad.TexWidth);
        }

        public void RepeatY()
        {
            float V = (Size.Y / Quad.TexHeight) / (Size.X / Quad.TexWidth);
            Quad.UVFromBounds(Vector2.Zero, new Vector2(1, V));
            Quad.V_Wrap = true;
        }

        public void PointxAxisTo(float Radians)
        {
            Vector2 Dir = new Vector2((float)Math.Cos(Radians), (float)Math.Sin(Radians));
            PointxAxisTo(Dir);
        }

        public void PointxAxisTo(Vector2 Dir)
        {
            Tools.PointxAxisTo(ref Base.e1, ref Base.e2, Dir);
        }

        public void TextureParralax(float Parralax, Vector2 repeat, Vector2 shift, Camera Cam)
        {
            Vector2 offset = Parralax * repeat * (Cam.EffectivePos + shift)
                / (2 * new Vector2(Cam.AspectRatio / .001f, 1f / .001f));
            
            offset.Y *= -1;
            repeat *= .001f / Cam.EffectiveZoom.X;
            Quad.v0.Vertex.uv = new Vector2(-repeat.X / 2, -repeat.Y / 2) + offset;
            Quad.v1.Vertex.uv = new Vector2(repeat.X / 2, -repeat.Y / 2) + offset;
            Quad.v2.Vertex.uv = new Vector2(-repeat.X / 2, repeat.Y / 2) + offset;
            Quad.v3.Vertex.uv = new Vector2(repeat.X / 2, repeat.Y / 2) + offset;
        }

        public void FullScreen(Camera cam)
        {
            Size = new Vector2((cam.EffectiveTR.X - cam.EffectiveBL.X) / 2, (cam.EffectiveTR.Y - cam.EffectiveBL.Y) / 2);
            Pos = (cam.EffectiveTR + cam.EffectiveBL) / 2; //cam.EffectivePos + cam.ShakeOffset;
        }

        public void FromBounds(Vector2 BL, Vector2 TR)
        {
            Size = new Vector2((TR.X - BL.X) / 2, (TR.Y - BL.Y) / 2);
            Pos = (TR + BL) / 2;
        }

        public void SetToDefault()
        {
            Quad.MyEffect = Tools.BasicEffect;
            Quad.MyTexture = Tools.TextureWad.FindByName("White");
        }

        public QuadClass(string TextureName)
        {
            Initialize(null, false, false);
            Set(TextureName, 1);
            ScaleToTextureSize();
        }
        public QuadClass(string TextureName, float Width)
        {
            Initialize(null, false, false);
            Set(TextureName, Width);
        }
        public QuadClass(EzTexture Texture, float Width, bool Fancy)
        {
            Initialize(null, Fancy, Fancy);
            Set(Texture, Width);
        }
        public QuadClass(string TextureName, float Width, bool Fancy)
        {
            Initialize(null, Fancy, Fancy);
            Set(TextureName, Width);
        }
        public void Set(string TextureName, float Width)
        {
            Quad.MyEffect = Tools.BasicEffect;
            this.TextureName = TextureName;

            ScaleYToMatchRatio(Width);
        }
        public void Set(EzTexture Texture, float Width)
        {
            Quad.MyEffect = Tools.BasicEffect;
            Quad.MyTexture = Texture;

            ScaleYToMatchRatio(Width);
        }
        public void Set(TextureOrAnim t_or_a)
        {
            Quad.SetTextureOrAnim(t_or_a);
        }
        public void Set(string name)
        {
            Quad.SetTextureOrAnim(name);
        }


        public void Draw() { Draw(true, true, Shadow); }
        public void DrawShadow() { Draw(true, false, true); }

        /// <summary>
        /// If this quad is an element in a bigger structure (such as a DrawPile),
        /// this vector represents the scaling of the parent structure.
        /// </summary>
        public Vector2 ParentScaling = Vector2.One;
        public float ParentAlpha = 1f;

        public void MultiplyAlpha(float alpha)
        {
            Quad.MultiplyAlpha(alpha);
        }

        public float Alpha
        {
            get { return Quad.MySetColor.A / 255f; }
            set { Quad.SetAlpha(value); }
        }

        public bool Show = true;
        public void Draw(bool Update, bool DrawQuad, bool DrawShadow)
        {
            if (!Show) return;

            if (FancyLightAlpha != null)
            {
                FancyLightAlpha.RelVal = Tools.Restrict(0, 1000, FancyLightAlpha.RelVal);
                FancyLightAlpha.Update();

                var color = new Vector4(FancyLightAlpha.Pos.X / 1000f);
                color.W = FancyLightAlpha.Pos.Y / 1000f;

                Quad.SetColor(color);
            }

            if (FancyPos != null)
                Base.Origin = FancyPos.Update(ParentScaling);

            Vector2 HoldScale = Vector2.One;
            Color HoldColor = Quad.MySetColor;
            if (FancyScale != null)
            {
                Vector2 scale = FancyScale.Update() * ParentScaling;
                Base.e1 = new Vector2(scale.X, 0);
                Base.e2 = new Vector2(0, scale.Y);
            }
            else
            {
                if (ParentScaling != Vector2.One)
                {
                    HoldScale = Size;
                    Size *= ParentScaling;
                }
            }
            if (ParentAlpha != 1)
                MultiplyAlpha(ParentAlpha);

            if (FancyAngle != null)
            {
                float Angle = FancyAngle.Update().X;
                PointxAxisTo(Angle);
            }

            if (DrawShadow)
            {
                Quad.MyEffect = Tools.EffectWad.EffectList[1];
                Vector2 HoldSize = Size;
                Scale(ShadowScale);
                Base.Origin -= ShadowOffset;
                Color _HoldColor = Quad.MySetColor;
                Quad.SetColor(ShadowColor);
                Quad.Update(ref Base);
                Tools.QDrawer.DrawQuad(ref Quad);
                //Tools.QDrawer.Flush();
                Quad.MyEffect = Tools.BasicEffect;
                Base.Origin += ShadowOffset;
                Quad.SetColor(_HoldColor);
                Size = HoldSize;
            }

            if (Update)
                Quad.Update(ref Base);

            if (DrawQuad)
                Tools.QDrawer.DrawQuad(ref Quad);

            // Reset scaling if modified
            if (FancyScale == null && ParentScaling != Vector2.One)
                Size = HoldScale;
            // Reset alpha if modified
            if (ParentAlpha != 1)
                Quad.SetColor(HoldColor);
        }

        public void UpdateShift()
        {
            Quad.UpdateShift(ref Base);
        }

        public void UpdateShift_Precalc()
        {
            Quad.UpdateShift_Precalc(ref Base);
        }

        public void Update()
        {
            Quad.Update(ref Base);
        }

        public void Write(StreamWriter writer)
        {
            Tools.WriteFields(this, writer, "Quad", "Base");
        }
        public void Read(StreamReader reader)
        {
            Tools.ReadFields(this, reader);
        }

        public void Write(BinaryWriter writer)
        {            
            writer.Write(Quad.MyTexture.Path);
            writer.Write(Quad.MyEffect.Name);

            WriteReadTools.WriteVector2(writer, Base.e1);
            WriteReadTools.WriteVector2(writer, Base.e2);
            WriteReadTools.WriteVector2(writer, Base.Origin);
        }
        public void Read(BinaryReader reader)
        {         
            Quad.MyTexture = Tools.TextureWad.FindByName(reader.ReadString());
            Quad.MyEffect = Tools.EffectWad.FindByName(reader.ReadString());

            WriteReadTools.ReadVector2(reader, ref Base.e1);
            WriteReadTools.ReadVector2(reader, ref Base.e2);
            WriteReadTools.ReadVector2(reader, ref Base.Origin);
        }
    }
}