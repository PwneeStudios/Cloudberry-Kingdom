using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CloudberryEULA
{
	static class Program
	{
		/// <summary>
		/// Exit code to return when program ends.
		/// Defaults to a non-zero value. A successful call MUST call ExitApplication(0).
		/// </summary>
		static int ExitCode = 1;

		public static void ExitApplication(int ExitCode)
		{
			Program.ExitCode = ExitCode;
			Application.Exit();
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Installer());

			return ExitCode;
		}
	}
}
