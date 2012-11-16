using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class NormalBlockDraw
    {
        public bool Shadow = false;
        public Vector2 ShadowOffset = Vector2.Zero;
        public Color ShadowColor = Color.Black;

        public Color Tint;

        public PieceQuad MyPieces, MyTemplate;
        BlockBase MyBlock;

        public NormalBlockDraw() { MakeNew(); }

        public void MakeNew()
        {
            SetTint(Color.White);
            MyTemplate = null;
        }

        public void Clone(NormalBlockDraw DrawA)
        {
            SetTint(DrawA.Tint);
            MyTemplate = DrawA.MyTemplate;
        }

        public void SetTint(Vector4 v) { SetTint(new Color(v)); }

        public void SetTint(Color Tint)
        {
            this.Tint = Tint;

            if (MyPieces != null)
                MyPieces.SetColor(Tint);
        }

        public void Release()
        {
            MyPieces = null;
            MyBlock = null;
        }

        public static Vector2 ModCeilingSize = new Vector2(25f, 0f);
        public void Init(BlockBase block) { Init(block, null, false); }
        public void Init(BlockBase block, PieceQuad template, bool Invert)
        {
            if (MyTemplate != null)
                template = MyTemplate;

            //MyPieces.Init(Tools.TextureWad.FindByName("White"), Tools.BasicEffect);

            if (template != null)
            {
                if (MyPieces == null) MyPieces = new PieceQuad();
                MyPieces.Clone(template);
            }

            MyBlock = block;

            // Grow the block a bit if it is a ceiling piece
            Vector2 ModSize = Vector2.Zero;

            if (MyPieces != null)
            {
                MyPieces.Invert = Invert;

                MyPieces.MyOrientation = block.BlockCore.MyOrientation;

                MyPieces.CalcQuads(MyBlock.Box.Current.Size + ModSize);
            }

            // Tint
            SetTint(MyBlock.Core.MyTileSet.Tint);

            SetTint(Tint);
        }

        public void Update()
        {
            if (MyPieces != null)
                MyPieces.Base.Origin = MyBlock.Box.Current.Center;
        }

        public void Draw()
        {
            if (MyPieces != null)
            {
                MyPieces.Draw();
            }
        }
    }
}