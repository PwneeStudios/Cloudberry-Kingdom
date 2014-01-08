using Microsoft.Xna.Framework;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class TextFloat : GameObject
    {
        public EzText MyText;

        public TextFloat(Localization.Words Text, Vector2 pos)
        {
            CoreData.DrawLayer = Level.LastInLevelDrawLayer - 1;

            CoreData.Data.Position = pos;
            CoreData.Data.Velocity = new Vector2(0, 8);

            MyText = new EzText(Text, Resources.Font_Grobold42, 1000, true, true);
            MyText.Scale = .5f;
            MyText.MyFloatColor = new Color(228, 0, 69).ToVector4();
            MyText.OutlineColor = Color.White.ToVector4();

            MyText.ZoomWithCam = true;
        }

        public TextFloat(string Text, Vector2 pos)
        {
            CoreData.DrawLayer = Level.LastInLevelDrawLayer - 1;

            CoreData.Data.Position = pos;
            CoreData.Data.Velocity = new Vector2(0, 8);

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
            CoreData.Data.Position += CoreData.Data.Velocity;
            Alpha += AlphaSpeed;

            if (Alpha <= 0)
                this.Release();
        }

        void Update()
        {
            MyText.Pos = CoreData.Data.Position;
            MyText.Alpha = Alpha;            
        }

        protected override void MyDraw()
        {
            base.MyDraw();

            if (CoreData.MarkedForDeletion || !CoreData.Active) return;

            if (!CoreData.MyLevel.MainCamera.OnScreen(CoreData.Data.Position, 600)) return;

            if (Tools.DrawGraphics)
            {
                Update();
                MyText.Draw(CoreData.MyLevel.MainCamera);
            }
        }

        public override void Move(Vector2 shift)
        {
            CoreData.Data.Position += shift;
        }
    }
}