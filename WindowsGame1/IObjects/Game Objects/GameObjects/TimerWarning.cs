using System;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class TimerWarning : GameObject
    {
        public GUI_Timer MyTimer;
        QuadClass Fullscreen;

        public override void OnAdd()
        {
            base.OnAdd();

            Fullscreen.FullScreen(MyGame.MyLevel.MainCamera);
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();
        }

        public TimerWarning()
        {
            // Object is carried over through multiple levels, so prevent it from being released.
            PreventRelease = true;

            PauseOnPause = true;

            // Make a quad to fill the whole screen
            Fullscreen = new QuadClass();
        }

        float t;
        protected override void MyDraw()
        {
            base.MyDraw();

            if (SubThreshholdCount == 0)
                return;

            Fullscreen.Pos = Tools.CurLevel.MainCamera.Pos;
            Fullscreen.Draw();
        }

        //int Threshhold = 62 * 3;
        int Threshhold = 106;
        int SubThreshholdCount = 0;
        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (Core.MyLevel.Watching || Core.MyLevel.Finished) return;

            // Is time running out?
            if (MyTimer.Time < Threshhold)
            {
                float target_t = (Threshhold - MyTimer.Time) / (float)Threshhold;

                if (MyTimer.Time < 30)
                    target_t = 0;

                t = .8f * t + .2f * target_t;

                //int Period = 90;
                int Period = Threshhold + 15 + 1;

                // Do something dramatic every second
                if (SubThreshholdCount % Period == 15)
                {
#if XBOX
                    float LeftIntensity = Tools.Restrict(.4f, .4f, t);
                    float RightIntensity = Tools.Restrict(.1f, .3f, t);

                    foreach (PlayerData player in PlayerManager.AlivePlayers)
                        Tools.SetVibration(player.MyPlayerIndex, LeftIntensity, LeftIntensity, 30);
#endif
                }

                // Flash the screen
                float h = (float)Math.Cos(2 * Math.PI * (float)(SubThreshholdCount - 15) / Period);
                //h = Tools.Restrict(.5f, 1f, h) - .5f;
                //h = (Tools.Restrict(.35f, 1f, h) - .35f) * .4f;
                h = (Tools.Restrict(.65f, 1f, h) - .65f) * 1.5f;

                SetAlpha(h);


                SubThreshholdCount++;
            }
            else
            {
                SetAlpha(0);
                SubThreshholdCount = 0;
            }
        }

        void SetAlpha(float alpha)
        {
            Fullscreen.Quad.SetColor(new Vector4(.8f, .4f, .4f, alpha));
        }
    }
}