

namespace CloudberryKingdom
{
    public class Template_GameObject : GameObject
    {
        public Template_GameObject()
        {
        }

        public override void Init()
        {
            base.Init();
        }

        protected override void MyDraw()
        {
        }

        protected override void MyPhsxStep()
        {
            Level level = Core.MyLevel;
        }
    }
}