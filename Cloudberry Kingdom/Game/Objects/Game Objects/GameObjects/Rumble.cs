using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class Rumble : GUI_Panel
    {
        public Rumble()
        {
            Active = true;
            PauseOnPause = true;
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            int Period = 105;// 96;
            if (CoreData.GetPhsxStep() % Period == 0 && CoreData.GetPhsxStep() > 60)
                Tools.CurCamera.StartShake(.5f, 36);
        }
    }
}