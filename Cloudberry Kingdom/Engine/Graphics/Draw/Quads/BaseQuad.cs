#define EDITOR

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.IO;

using CloudberryKingdom;

namespace CoreEngine
{
    public enum ObjectDrawOrder { WithOutline, BeforeOutline, AfterOutline, All, None };
    public enum ChangeMode { SingleFrame, SingleAnim, All };

    public class BaseQuad
    {
        public ObjectDrawOrder MyDrawOrder;

        public ObjectClass ParentObject;
        public Quad ParentQuad;

        public MyOwnVertexFormat[] Vertices;
        public int NumVertices;

        /// <summary>
        /// Color rotation matrix.
        /// </summary>
        public Matrix MyMatrix
        {
            get
            {
                return _MyMatrix;
            }

            set
            {
                _MyMatrix = value;
                MyMatrixSignature = ColorHelper.MatrixSignature(_MyMatrix);
            }
        }

        Matrix _MyMatrix = Matrix.Identity;
        public float MyMatrixSignature;

        public EzTexture MyTexture;
        public EzEffect MyEffect;

        public AnimationData_Texture TextureAnim;
        public bool UpdateSpriteAnim = true;
        public bool TextureIsAnimated { get { return TextureAnim != null && TextureAnim.Anims != null; } }

        public Color MyColor, PremultipliedColor;

        public string Name;

        public bool Is(string Name)
        {
#if XBOX
            return string.Compare(this.Name, Name, System.StringComparison.CurrentCultureIgnoreCase) == 0;
#else
            return
                string.Compare(this.Name, Name, true) == 0;
#endif
        }

        public bool Show = true;

        public bool Released;

#if EDITOR
        public bool Expanded = true; // Whether the tree node for this quad is expanded

        public ObjectVector ParentPoint, ChildPoint, ReleasePoint;
        public bool SetToBeParent, SetToBeChild;
#endif

        public void Clone(BaseQuad quad)
        {
            Show = quad.Show;

            MyMatrix = quad.MyMatrix;

            Name = quad.Name;
            MyDrawOrder = quad.MyDrawOrder;

            MyColor = quad.MyColor;
            PremultipliedColor = quad.PremultipliedColor;
            MyEffect = quad.MyEffect;
            MyTexture = quad.MyTexture;
        }

        virtual public void Release()
        {
            Released = true;

            ParentObject = null;
            ParentQuad = null;
            Vertices = null;
            MyTexture = null;
            MyEffect = null;
        }

        public void Update() { Update(1); }
        virtual public void Update(float Expand) { }

        virtual public void SetHold() { }
        virtual public void ReadAnim(int anim, int frame) { }
        virtual public void Record(int anim, int frame, bool UseRelativeCoords) { }
        virtual public void Calc(int anim, float t, int AnimLength, bool Loop, bool Linear) { }
        virtual public void Transfer(int anim, float DestT, int AnimLength, bool Loop, bool DestLinear, float t) { }

        public virtual void CopyAnim(BaseQuad quad, int Anim) { }
        public virtual void CopyAnimShallow(BaseQuad quad, int Anim) { }

        virtual public void Write(BinaryWriter writer)
        {
            // Version 51, 3/31/2010
            // Write draw order
            // Write quad name
            writer.Write(Name);
            writer.Write((int)MyDrawOrder);

            // Version 52, 4/1/2010
            // Write show bool
            writer.Write(Show);

            // Version 52, 7/25/2012
            // Write texture anim data
            if (TextureAnim == null)
                writer.Write(0);
            else
            {
                writer.Write(1);
                TextureAnim.Write(writer);
            }
        }

        virtual public void Read(BinaryReader reader, EzEffectWad EffectWad, EzTextureWad TextureWad, int VersionNumber)
        {
            // Version 51, 3/31/2010
            // Read in draw order
            // Read in quad name
            if (VersionNumber > 50)
            {
                Name = reader.ReadString();
                MyDrawOrder = (ObjectDrawOrder)reader.ReadInt32();
            }

            // Version 52, 4/1/2010
            // Read show bool
            if (VersionNumber > 51)
                Show = reader.ReadBoolean();

            // Version 54, 7/25/2012
            // Read texture anim data
            if (VersionNumber > 53)
            {
                int exists = reader.ReadInt32();
                if (exists == 1)
                {
                    if (TextureAnim == null) TextureAnim = new AnimationData_Texture();
                    TextureAnim.Read(reader);
                }
            }
        }

#if EDITOR
        virtual public void SaveState(int StateIndex)
        {
        }

        virtual public void RecoverState(int StateIndex)
        {
        }
#endif

        public Vector2 BL()
        {
            Vector2 BL = new Vector2(100000, 100000);
            for (int i = 0; i < NumVertices; i++)
                BL = Vector2.Min(BL, Vertices[i].xy);

            return BL;
        }

        public Vector2 TR()
        {
            Vector2 TR = new Vector2(-100000, -100000);
            for (int i = 0; i < NumVertices; i++)
                TR = Vector2.Max(TR, Vertices[i].xy);

            return TR;
        }

        public void SetTexture(string Name, EzTextureWad Wad)
        {
            MyTexture = Wad.FindByName(Name);
        }

        public void SetEffect(string Name, EzEffectWad Wad)
        {
            MyEffect = Wad.FindByName(Name);
        }

        public void OrphanSelf()
        {
            ParentQuad.RemoveQuadChild(this);
        }

        virtual public void FinishLoading(GraphicsDevice device, EzTextureWad TexWad, EzEffectWad EffectWad)
        {
            FinishLoading(device, TexWad, EffectWad, true);
        }
        virtual public void FinishLoading(GraphicsDevice device, EzTextureWad TexWad, EzEffectWad EffectWad, bool UseNames) { }

        virtual public void Draw() { }
        virtual public void Draw(QuadDrawer QDrawer) { }
        virtual public void DrawExtra(QuadDrawer QDrawer, bool Additional, float ScaleLines) { }
        virtual public bool HitTest(Vector2 x) { return false; }

#if EDITOR
        virtual public List<ObjectVector> GetObjectVectors() { return new List<ObjectVector>(); }

        public void ColoredDraw(QuadDrawer QDrawer, Color color)
        {
            MyOwnVertexFormat[] hold = new MyOwnVertexFormat[Vertices.Length];
            Vertices.CopyTo(hold, 0);

            SetColor(color);

            Draw(QDrawer);
            QDrawer.Flush();

            hold.CopyTo(Vertices, 0);
        }
#endif

        virtual public void SetColor(Color color)
        {
            MyColor = color;

            PremultipliedColor = ColorHelper.PremultiplyAlpha(color);

            for (int i = 0; i < NumVertices; i++)
                Vertices[i].Color = PremultipliedColor;
        }

        public virtual void Set_PosFromRelPos(ObjectVector v)
        {
        }

        public virtual void Set_RelPosFromPos(ObjectVector v)
        {
        }

#if EDITOR
        virtual public void ClickOnChildButton()
        {
            SetToBeChild = !SetToBeChild;
        }

        public void ClickOnReleaseButton()
        {
            if (ParentQuad != null && ParentQuad != ParentObject.ParentQuad)
                ParentQuad.RemoveQuadChild(this);
        }
#endif
    }
}