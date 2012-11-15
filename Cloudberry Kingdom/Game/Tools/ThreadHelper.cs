using System;
using System.Threading;

namespace CloudberryKingdom
{
    public static class ThreadHelper
    {
        public static Thread EasyThread(int affinity, string name, Action action)
        {
            Thread NewThread = new Thread(
                new ThreadStart(
                    delegate
                    {
#if XBOX && !WINDOWS
                    Thread.CurrentThread.SetProcessorAffinity(new[] { affinity });
#endif
                        var ThisThread = Thread.CurrentThread;
                        EventHandler<EventArgs> abort = (s, e) =>
                        {
                            if (ThisThread != null)
                            {
                                ThisThread.Abort();
                            }
                        };
                        Tools.TheGame.Exiting += abort;

                        action();

                        Tools.TheGame.Exiting -= abort;
                    }))
            {
                Name = name,
#if WINDOWS
                //Priority = ThreadPriority.Highest,
                Priority = ThreadPriority.Lowest,
#endif
            };

            NewThread.Start();

            return NewThread;
        }
    }
}