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
#if GAME
            using (CloudberryKingdomGame game = new CloudberryKingdomGame())
#else
            using (Game_Editor game = new Game_Editor())            
#endif
            {
                game.Run();
            }
        }
    }
}

