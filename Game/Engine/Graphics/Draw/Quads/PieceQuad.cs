using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CoreEngine;

namespace CloudberryKingdom
{
    public struct PieceQuadData
    {
        public Vector2 Center_BL_Shift;
        public Vector2 Center_TR_Shift;
        
        public float LeftWidth, RightWidth, TopWidth, BottomWidth;
        public float RepeatWidth, RepeatHeight;

        public Vector2 UV_Multiples;

        /// <summary>
        /// Make quad flush against bottom, instead of top.
        /// </summary>
        public bool BottomFlush;

        /// <summary>
        /// Make center quad's texture upside down.
        /// </summary>
        public bool UpsideDown;

        /// <summary>
        /// Make center quad's texture mirrored horizontally.
        /// </summary>
        public bool Mirror;
    }

    public class PieceQuad
    {
        // Some pillar info
        public float BoxHeight = -1;

        /// <summary>
        /// Used when Style.UseLowerBlockBounds == true. Block will have a lower lip than usual.
        /// This is used for InvertHero, so he is standing on the bottom of blocks properly.
        /// (Otherwise you want the lips to be higher, so your head doesn't hit them as easily).
        /// </summary>
        public float ModLowerBlockBound = 0;

        /// <summary>
        /// When ModLowerBlockBound is used, this says whether ModLowerBlockBound is an offset from the normal bound, or an absolute bound.
        /// </summary>
        public bool RelativeLowerLip = false;

        public void CalcTexture(int anim, float t)
        {
            Center.CalcTexture(anim, t);
        }

        public float Group_CutoffWidth = 0;

        /// <summary>
        /// Layer of the quad, used in DrawPiles
        /// </summary>
        public int Layer = 0;

        public static Dictionary<string, PieceQuad> Dict;

        public static PieceQuad Get(string name)
        {
            return Dict[name];
        }

        public static PieceQuad MovingBlock, FallingBlock, BouncyBlock, Elevator;
        public static BlockGroup FallGroup, BouncyGroup, MovingGroup, ElevatorGroup;

        public SimpleQuad Center;

        public float t;
        public bool Playing = false;

        public enum Orientation { Normal, UpsideDown, RotateRight, RotateLeft };
        public Orientation MyOrientation = Orientation.Normal;

        public PieceQuadData Data;

        public Color MyColor;

        public BasePoint Base;

        /// <summary>
        /// Clone the properties of another PieceQuad to this PieceQuad.
        /// </summary>
        public void Clone(PieceQuad PieceQuadA)
        {
            FixedHeight = PieceQuadA.FixedHeight;

            Data = PieceQuadA.Data;

            MyOrientation = PieceQuadA.MyOrientation;

            Center = PieceQuadA.Center;

            Base = PieceQuadA.Base;

            SetColor(PieceQuadA.MyColor);
        }
        
        /// <summary>
        /// The width of the pillar this piecequad is associated with, if any.
        /// </summary>
        public int Pillar_Width = 0;

        public PieceQuad(int width, string texture, float left, float right, float top)
        {
            _Constructor(width, texture, left, right, top, false, 0, false);
        }
        public PieceQuad(int width, string texture, float left, float right, float top, bool upside_down)
        {
            _Constructor(width, texture, left, right, top, upside_down, 0, false);
        }
        public PieceQuad(int width, string texture, float left, float right, float top, bool upside_down, float lowerlip, bool relativelowerlip)
        {
            _Constructor(width, texture, left, right, top, upside_down, lowerlip, relativelowerlip);
        }
        void _Constructor(int width, string texture, float left, float right, float top, bool upside_down, float lowerlip, bool relativelowerlip)
        {
            ModLowerBlockBound = lowerlip;
            RelativeLowerLip = relativelowerlip;

            InitAll();
            Init(null, Tools.BasicEffect);
            Pillar_Width = width;

            Data.RepeatWidth = 2000;
            Data.RepeatHeight = 2000;
            Data.UV_Multiples = new Vector2(1, 0);
            Center.U_Wrap = Center.V_Wrap = false;

            FixedHeight = 0; // Flag to tell ParseExtra to set the height properly

            Center.SetTextureOrAnim(texture);

            int tex_width = Center.TexWidth;
            int tex_height = Center.TexHeight;

            if (upside_down) { Data.UpsideDown = true; Data.BottomFlush = true; }
            Data.Center_BL_Shift.X = left;
            Data.Center_TR_Shift.X = right;
            Data.Center_TR_Shift.Y = 
            Data.Center_BL_Shift.Y = top;

            // Extend the quad down to properly scale quad
            if (FixedHeight == 0)
            {
                float sprite_width = 2 * width + Data.Center_TR_Shift.X - Data.Center_BL_Shift.X;
                FixedHeight = sprite_width * (float)tex_height / (float)tex_width;
            }
        }

        public PieceQuad()
        {
            InitAll();
        }

        void InitAll()
        {
            Center = new SimpleQuad();
            Center.Init();

            Base.Init();            
        }

        public void SetAlpha(float Alpha)
        {
            Vector4 clr = MyColor.ToVector4();
            clr.W = Alpha;
            SetColor(new Color(clr));
        }

        public void SetColor(Color color)
        {
            if (MyColor == color)
                return;

            MyColor = color;

            Center.SetColor(color);
        }

        public void SetEffect(CoreEffect effect)
        {
            Center.MyEffect = effect;
        }

        public void Init(CoreTexture tex, CoreEffect fx)
        {
            Data.Center_BL_Shift = Vector2.Zero;
            Data.Center_TR_Shift = Vector2.Zero;

            Data.LeftWidth = Data.RightWidth = Data.TopWidth = Data.BottomWidth = 0;
            Data.RepeatWidth = Data.RepeatHeight = 0;

            Center.U_Wrap = false;
            Center.V_Wrap = false;

            Center.MyTexture = tex;

            Center.MyEffect = fx;

            Center.U_Wrap = Center.V_Wrap = true;

            Data.UV_Multiples = new Vector2(1, 1);

            SetColor(Color.White);
        }

        public Vector2 FromBounds(Vector2 TR, Vector2 BL)
        {
            CalcQuads((TR - BL) / 2);
            return (TR + BL) / 2;
        }

        /// <summary>
        /// Whether to turn upside down this PieceQuad.
        /// </summary>
        public bool Invert;

        public void CalcQuads(Vector2 Size)
        {
            // Prevent the center from being a sliver
            if (Data.LeftWidth + Data.RightWidth + 3 > 2 * Size.X)
                Size.X = .5f * (Data.LeftWidth + Data.RightWidth);
            
            float x1, x2, x3, x4, y1, y2, y3, y4;
            x1 = -Size.X;
            x2 = x1 + Data.LeftWidth;
            x4 = Size.X;
            x3 = x4 - Data.RightWidth;
            y1 = -Size.Y;
            y2 = y1 + Data.BottomWidth;
            y4 = Size.Y;
            y3 = y4 - Data.TopWidth;

            if (FixedHeight >= 0)
                y2 = y3 - FixedHeight;

            Vector2 SmallShift = new Vector2(1f, 1f);

            if (Data.BottomFlush || Invert)
                Center.FromBounds(new Vector2(x2, y1) + Data.Center_BL_Shift, new Vector2(x3 - 1, y1 + y3 - y2 - 1) + Data.Center_TR_Shift + SmallShift);
            else
                Center.FromBounds(new Vector2(x2, y2) + Data.Center_BL_Shift, new Vector2(x3 - 1, y3 - 1) + Data.Center_TR_Shift + SmallShift);            
            
            float U, V;

            if (Data.UV_Multiples.X == 0)
                U = (x3 - x2) / Data.RepeatWidth;
            else
            {
                U = (x3 - x2) / Data.RepeatWidth;
                U = Math.Max(1, (int)(U / Data.UV_Multiples.X)) * Data.UV_Multiples.X;
            }

            if (Data.UV_Multiples.Y == 0)
                V = (y3 - y2) / Data.RepeatHeight;
            else
            {
                V = (y3 - y2) / Data.RepeatHeight;
                V = Math.Max(1, (int)(V / Data.UV_Multiples.Y)) * Data.UV_Multiples.Y;
            }


            if (FixedHeight >= 0)
                V = .995f;

            Orientation HoldOrientation;
            switch (MyOrientation)
            {
                case Orientation.Normal:
                    Center.UVFromBounds(new Vector2(0, V), new Vector2(U, 0));
                    break;

                case Orientation.UpsideDown:
                    Center.UVFromBounds(new Vector2(0, 0), new Vector2(U, V));
                    break;

                case Orientation.RotateRight:
                    HoldOrientation = MyOrientation;
                    MyOrientation = Orientation.Normal;
                    Tools.Swap(ref Size.X, ref Size.Y);
                    CalcQuads(Size);
                    MyOrientation = HoldOrientation;

                    Center.RotateRight();

                    break;

                case Orientation.RotateLeft:
                    HoldOrientation = MyOrientation;
                    MyOrientation = Orientation.Normal;
                    Tools.Swap(ref Size.X, ref Size.Y);
                    CalcQuads(Size);
                    MyOrientation = HoldOrientation;

                    Center.RotateLeft();

                    break;
            }

            if (Data.UpsideDown || Invert)
                Center.MirrorUV_Vertical();

            if (Data.Mirror)
                Center.MirrorUV_Horizontal();
        }

        public float FixedHeight = -1;
        public void Update()
        {
            Center.Update(ref Base);
        }

        public void Draw()
        {
            Update();

            Tools.QDrawer.DrawQuad(ref Center);
        }
    }
}
