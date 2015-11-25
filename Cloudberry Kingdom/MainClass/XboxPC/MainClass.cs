using System;
using CoreEngine;

namespace CloudberryKingdom
{
    static class MainClass
    {
		/// <summary>
		/// Put this in to make sure Tools static constructor is called immediately.
		/// Probably not needed, but there is not time to test removing it.
		/// </summary>
		static float dummy_t;
		
#if WINDOWS
#if PC
#endif
        [STAThread]
#endif
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
        {
            dummy_t = Tools.t;
            Tools.Write("Main Constructor");

            CloudberryKingdomGame.ProcessArgs(args);

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

#if DEBUG && WINDOWS && !SDL2
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
			Tools.Log("Exception on MainClass.CurrentDomain_FirstChanceException\n" + Tools.ExceptionStr(e.Exception));
            //Tools.Log(e.Exception.ToString());
        }
#endif

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
			Tools.Log(string.Format("{0}\nExceptionObject:\n{1}\nsender:\n{2}", "Exception on MainClass.CurrentDomain_UnhandledException",
				e.ExceptionObject == null ? "" : e.ExceptionObject.ToString(),
				sender == null ? "" : sender.ToString()));
            //Tools.Log(e.ExceptionObject.ToString());
        }
    }
}
