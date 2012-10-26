using Microsoft.Xna.Framework;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class TextFloat : GameObject
    {
        public EzText MyText;

        public TextFloat(string Text, Vector2 pos)
        {
            Core.DrawLayer = Level.LastInLevelDrawLayer - 1;

            Core.Data.Position = pos;
            Core.Data.Velocity = new Vector2(0, 8);

            //MyText = new EzText(Text, Resources.Font_Grobold42, 1000, true, true);
            //MyText.Scale = 1.1f;
            //MyText.MyFloatColor = new Vector4(.9f, 1f, 1f, 1f);
            
            MyText = new EzText(Text, Resources.Font_Grobold42, 1000, true, true);
            MyText.Scale = .5f;
            MyText.MyFloatColor = new Color(228, 0, 69).ToVector4();
            MyText.OutlineColor = Color.White.ToVector4();

            MyText.ZoomWithCam = true;
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            MyText.Release();
        }

        float Alpha = 4.5f;
        float AlphaSpeed = -.08f;

        protected override void MyPhsxStep()
        {
            Core.Data.Position += Core.Data.Velocity;
            Alpha += AlphaSpeed;

            if (Alpha <= 0)
                this.Release();
        }

        void Update()
        {
            MyText.Pos = Core.Data.Position;
            MyText.Alpha = Alpha;            
        }

        protected override void MyDraw()
        {
            base.MyDraw();

            if (Core.MarkedForDeletion || !Core.Active) return;

            if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, 600)) return;

            if (Tools.DrawGraphics)
            {
                Update();
                MyText.Draw(Core.MyLevel.MainCamera);
            }
        }

        public override void Move(Vector2 shift)
        {
            Core.Data.Position += shift;
        }
    }
}