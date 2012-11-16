


namespace CloudberryKingdom
{
    public class ShowGuide : GUI_Panel
    {
        public ShowGuide()
        {
            Bob.GuideActivated = true;

            Active = true;
            PauseOnPause = true;

            Tags += GameObject.Tag.RemoveOnLevelFinish;
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            Bob.GuideActivated = false;
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            Level level = MyGame.MyLevel;

            // Don't affect speed if a replay is being watched
            if (level.Replay || level.Watching)
                return;

            // Otherwise show the ShowGuide
            Bob.GuideActivated = true;
       }
    }
}