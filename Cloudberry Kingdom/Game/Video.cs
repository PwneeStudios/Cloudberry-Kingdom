using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Drawing;

namespace CloudberryKingdom.Game
{
    class Video
    {
        Video TestVideo;
        VideoPlayer VPlayer;
        Texture2D VTexture;
        EzTexture VEZTexture = new EzTexture();
        WrappedBool VBool;

        /// <summary>
        /// Video test. Will be canned once proper video class is implemented.
        /// </summary>
        private void VideoTest()
        {
                VBool = new WrappedBool(false);

                Thread VThread = null;
                VThread = new Thread(
                     new ThreadStart(
                         delegate
                         {
#if XBOX
                            Thread.CurrentThread.SetProcessorAffinity(new[] { 3 });
#endif
                             Tools.TheGame.Exiting += (o, e) =>
                             {
                                 if (VThread != null)
                                     VThread.Abort();
                             };

                             // Test load movie
                             TestVideo = Content.Load<Video>("Movies//TestCinematic");
                             VPlayer = new VideoPlayer();
                             VPlayer.IsLooped = false;
                             VPlayer.Play(TestVideo);

                             while (true)
                             {
                                 lock (VEZTexture)
                                 {
                                     VTexture = VPlayer.GetTexture();
                                     VEZTexture.Tex = VTexture;
                                 }
                             }
                         }))
                {
                    Name = "VideoThread",
#if WINDOWS
                    Priority = ThreadPriority.Lowest,
#endif
                };
                VThread.Start();
        }


        void Update()
        {
            //VPlayer.IsLooped = false;
            //if (VPlayer != null && (Tools.PhsxCount % 60 == 0 || VPlayer.State == MediaState.Stopped))
            //{
            //    //VPlayer.IsLooped = true;
            //    VPlayer.Play(TestVideo);
            //}

            //if (VPlayer1 != null)
            //    Console.WriteLine(string.Format("! {0} {1}", VPlayer1.PlayPosition.Ticks, VPlayer2.PlayPosition.Ticks));
            //if (VPlayer1 != null
            //    && VPlayer1.PlayPosition.Ticks >= 55130000)//155100000)
            //{
            //    //&& VPlayer.PlayPosition.TotalMilliseconds == 0)
            //    VPlayer1.Pause();
            //    Tools.Swap(ref VPlayer1, ref VPlayer2);
            //    VPlayer1.Resume();
            //    lock (VBool)
            //    {
            //        VBool.MyBool = true;
            //    }
            //}

            /*
            if (Tools.keybState.IsKeyDown(Keys.D6))
            {
                VPlayer1.Resume();
            }
            if (Tools.keybState.IsKeyDown(Keys.D7))
            {
                VPlayer1.Stop();
                VPlayer1.Play(TestVideo1);
            }*/

            //graphics.SynchronizeWithVerticalRetrace = false;
            //graphics.SynchronizeWithVerticalRetrace = true;
            //graphics.IsFullScreen = false;

            //this.TargetElapsedTime = _TargetElapsedTime;
            //this.TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 100);
            //this.TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 10);
            //this.TargetElapsedTime = new TimeSpan(TimeSpan.TicksPerSecond / 60);
        }
    }
}
