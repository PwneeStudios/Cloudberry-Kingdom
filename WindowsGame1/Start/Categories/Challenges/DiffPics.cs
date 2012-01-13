using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class DiffPics : GUI_Panel
    {
        public override void OnAdd()
        {
            base.OnAdd();

            this.SlideOut(PresetPos.Right, 0);
        }

        QuadClass DiffPic;
        public DiffPics()
        {
            MyPile = new DrawPile();

            //MyPile.Backdrop = new PieceQuad();
            //MyPile.Backdrop.Clone(PieceQuad.SpeechBubble);
            //MyPile.Backdrop.CalcQuads(new Vector2(815, 860));
            //MyPile.BackdropShift = new Vector2(575, 120);
            //MyPile.Backdrop.SetColor(Color.Pink);
            //MyPile.Backdrop.SetAlpha(.8f);
            
            QuadClass Berry = new QuadClass();
            Berry.SetToDefault();
            Berry.TextureName = "cb_surprised";
            Berry.Scale(400);
            Berry.ScaleYToMatchRatio();

            Berry.Pos = new Vector2(1412, -418);
            //MyPile.Add(Berry);


            DiffPic = new QuadClass();
            DiffPic.SetToDefault();
            DiffPic.TextureName = "DiffPic0";
            DiffPic.Scale(1100);
            DiffPic.ScaleYToMatchRatio();

            DiffPic.Pos = new Vector2(950, 100);
            MyPile.Add(DiffPic);

            MyPile.Pos =
                new Vector2(-170f, 36f);
                //new Vector2(-90, 40);
        }

        public void SetDiffPic(int diff)
        {
            DiffPic.TextureName = string.Format("DiffPic{0}", diff);
        }
    }
}