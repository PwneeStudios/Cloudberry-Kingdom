using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class Boss : ObjectBase
    {
        public virtual void DrawTitle()
        {
            Title.DrawBackdrop();
            Title.Draw(Tools.CurLevel.MainCamera);
        }

        protected EzText Title;
        protected Vector2 TitlePos = new Vector2(425, 150);

        protected WaitAndDoList MyWaitAndToDoList;

        protected ScenePart CurState;

        protected ScenePart Wanted_Idle;

        protected Camera cam;

        public Boss()
        {
            MyWaitAndToDoList = new WaitAndDoList();
        }

        public virtual Vector2 Pos
        {
            get
            {
                return Vector2.Zero;
            }
            set
            {
            }
        }

        public virtual void PhsxStep()
        {
            MyWaitAndToDoList.PhsxStep();

            if (CurState != null)
                CurState.PhsxStep();
        }
    }
}