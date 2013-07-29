using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Windows.Forms;

namespace CloudberryCleanup
{
	public partial class Uninstaller : Form
	{
		public Uninstaller()
		{
			InitializeComponent();

            No.Checked = true;
            Check_No();
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		void TryToDeleteFile(string path)
		{
			try
			{
				File.Delete(path);
			}
			catch
			{
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (Yes.Checked)
			{
				string dir = SaveDir();

				var files = Directory.EnumerateFiles(dir);
				foreach (var file in files)
				{
					if (file.Contains("Player Data"))
					{
						TryToDeleteFile(file);
					}
				}
				//string file_data = Path.Combine(dir, "Player Data");
				//TryToDeleteFile(file_data);

				string file_bam = Path.Combine(dir, "SaveData.bam");
				TryToDeleteFile(file_bam);

				string file_options = Path.Combine(dir, "Options");
				TryToDeleteFile(file_options);

				string file_stamp = Path.Combine(dir, "Stamp");
				TryToDeleteFile(file_stamp);

				try
				{
					Directory.Delete(dir);
				}
				catch (Exception ex)
				{
					if (ex is DirectoryNotFoundException)
					{
					}
					else
					{
						MessageBox.Show("Cloudberry Kingdom save directory could not be deleted. This may be because custom files were placed in the folder by the user, or because some files are currently in use.");
					}
				}

				Program.ExitApplication(0);
			}
			else
			{
				MakeStamp();
				Program.ExitApplication(0);
			}
		}

		private static string SaveDir()
		{
			string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			dir = Path.Combine(dir, "Cloudberry Kingdom");
			return dir;
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{

		}

		private void Yes_CheckedChanged(object sender, EventArgs e)
		{
			if (Yes.Checked)
			{
                Check_Yes();
			}
		}

        private void Check_Yes()
        {
            No.Checked = false;

            //BerryHappy.Hide();
            //BerrySad.Show();
        }

		private void No_CheckedChanged(object sender, EventArgs e)
		{
			if (No.Checked)
			{
                Check_No();
			}
		}

        private void Check_No()
        {
            Yes.Checked = false;

            //BerrySad.Hide();
            //BerryHappy.Show();
        }


		/// <summary>
		/// Creates a time stamp file marking this moment in time.
		/// </summary>
		void MakeStamp()
		{
            string s = "";

			//var s = DateTime.Now.ToString();
            DoWithInvariantCulture(() => s = DateTime.Now.ToString());

			string dir = SaveDir();
			string file_stamp = Path.Combine(dir, "Stamp");

			File.WriteAllText(file_stamp, s);
		}

		/// <summary>
		/// Total seconds passed since the last time stamp file was created.
		/// </summary>
		/// <returns></returns>
		double SecondsSinceStamp()
		{
			string dir = SaveDir();

			string file_stamp = Path.Combine(dir, "Stamp");
			if (!File.Exists(file_stamp))
			{
				return int.MaxValue;
			}

			var stamp = File.ReadAllText(file_stamp);

            //var date = DateTime.Parse(stamp);
            DateTime date = new DateTime();
            DoWithInvariantCulture(() => date = DateTime.Parse(stamp));
			var diff = DateTime.Now - date;

			return diff.TotalSeconds;
		}

        static void DoWithInvariantCulture(Action f)
        {
            var HoldCurrentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            f();

            Thread.CurrentThread.CurrentCulture = HoldCurrentCulture;
        }

		private void Uninstaller_Load(object sender, EventArgs e)
		{
			string dir = SaveDir();
			
			if (!Directory.Exists(dir))
			{
				Program.ExitApplication(0);
				return;
			}

			if (SecondsSinceStamp() < 3.0)
			{
				Program.ExitApplication(0);
				return;
			}
		}
	}
}
