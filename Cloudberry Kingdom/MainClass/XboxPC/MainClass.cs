using System;
using CoreEngine;

namespace CloudberryKingdom
{
    static class MainClass
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
            CloudberryKingdomGame.ProcessArgs(args);

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

#if DEBUG && WINDOWS
            AppDomain.CurrentDomain.FirstChanceException += new EventHandler<System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs>(CurrentDomain_FirstChanceException);
#endif

#if GAME
            using (XnaGameClass game = new XnaGameClass())
#else
            using (Game_Editor game = new Game_Editor())            
#endif
            {
                game.Run();
            }
        }

#if DEBUG && WINDOWS
        static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            Tools.Log(e.Exception.ToString());
        }
#endif

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Tools.Log(e.ExceptionObject.ToString());
        } 
    }
}
