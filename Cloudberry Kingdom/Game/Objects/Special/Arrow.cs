using Microsoft.Xna.Framework;

using CoreEngine;

namespace CloudberryKingdom
{
    public class Arrow : GameObject
    {
        public SimpleObject MyObject;

        public Arrow()
        {
            this.PreventRelease = false;
            MyObject = new SimpleObject(Prototypes.ArrowObj, false);
            SetScale(800);
            SetAnimation();
        }

        public enum Orientation { Left, Right };
        int MyOrientation = 1;

        /// <summary>
        /// Set the orientation of the arrow, to point left or to point right.
        /// </summary>
        public void SetOrientation(Orientation orientation)
        {
            if (orientation == Orientation.Left)
                MyOrientation = 1;
            else
                MyOrientation = -1;

            SetScale(Scale);
        }

        float Scale;
        public void SetScale(float Scale)
        {
            this.Scale = Scale;
            MyObject.Base.e1 = new Vector2(Scale, 0);
            MyObject.Base.e2 = new Vector2(0, Scale * MyOrientation);
        }

        void SetAnimation()
        {
            MyObject.Read(0, 0);
            MyObject.Play = true;
            MyObject.Loop = true;
            MyObject.EnqueueAnimation(0, (float)Tools.GlobalRnd.Rnd.NextDouble() * 1.5f, true);
            MyObject.DequeueTransfers();
            MyObject.Update();
        }

        public void AnimStep()
        {
            if (MyObject.DestinationAnim() == 0 && MyObject.Loop)
                MyObject.PlayUpdate(1f / 6.7f);
        }

        protected override void MyPhsxStep()
        {
            AnimStep();
        }

        Vector2 PointToPos;
        public void PointTo(Vector2 pos)
        {
            PointToPos = pos;
            CoreMath.PointxAxisTo(ref MyObject.Base, CoreData.Data.Position - PointToPos);
            MyObject.Base.e2 *= MyOrientation;
        }

        public void Update()
        {
            MyObject.Base.Origin = CoreData.Data.Position;

            MyObject.Update();
        }

        protected override void MyDraw()
        {
            if (!CoreData.MyLevel.MainCamera.OnScreen(CoreData.Data.Position, 600)) return;

            if (Tools.DrawGraphics)
            {
                Update();
                MyObject.Draw(Tools.QDrawer, Tools.EffectWad);
            }
        }

        public override void Move(Vector2 shift)
        {
            CoreData.Data.Position += shift;
        }
    }
}