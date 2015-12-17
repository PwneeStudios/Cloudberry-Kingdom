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
            if (Core.GetPhsxStep() % Period == 0 && Core.GetPhsxStep() > 60)
                Tools.CurCamera.StartShake(.5f, 36);
        }
    }
}