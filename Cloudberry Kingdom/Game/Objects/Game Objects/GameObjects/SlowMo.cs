

namespace CloudberryKingdom
{
    public class SlowMo : GUI_Panel
    {
        public SlowMo()
        {
            Active = true;
            PauseOnPause = true;

            Tags += GameObject.Tag.RemoveOnLevelFinish;
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            Tools.PhsxSpeed = 1;
        }

        int Speed = 0;

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            Level level = MyGame.MyLevel;

            // Don't affect speed if a replay is being watched
            if (level.Replay || level.Watching)
                return;

            Tools.PhsxSpeed = Speed;

            // On (B)
            if (ButtonCheck.State(ControllerButtons.X, Control).Pressed)
            {
                // Change the speed
                if (Speed == 1) Speed = 0;
                else Speed = 1;
            }
        }
    }
}