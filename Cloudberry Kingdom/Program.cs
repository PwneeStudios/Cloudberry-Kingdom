using System;
using Drawing;

namespace CloudberryKingdom
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
#if WINDOWS
#if PC
#endif
        [STAThread]
#endif

        static void Main(string[] args)
        {
            CloudberryKingdom_XboxPC.ProcessArgs(args);

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            
#if DEBUG
            AppDomain.CurrentDomain.FirstChanceException += new EventHandler<System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs>(CurrentDomain_FirstChanceException);
#endif

#if GAME
            using (CloudberryKingdom_XboxPC game = new CloudberryKingdom_XboxPC())
#else
            using (Game_Editor game = new Game_Editor())            
#endif
            {
                game.Run();
            }
        }

        static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            Tools.Log(e.Exception.ToString());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Tools.Log(e.ExceptionObject.ToString());
        } 
    }
}

