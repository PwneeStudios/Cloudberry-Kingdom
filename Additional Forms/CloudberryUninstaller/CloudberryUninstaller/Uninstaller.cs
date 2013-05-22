using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CloudberryCleanup
{
	public partial class Uninstaller : Form
	{
		public Uninstaller()
		{
			InitializeComponent();
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (Yes.Checked)
			{
				string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				dir = Path.Combine(dir, "Cloudberry Kingdom");

				try
				{
					string file_options = Path.Combine(dir, "Options");
					File.Delete(file_options);
				}
				catch
				{
				}

				try
				{
					string file_data = Path.Combine(dir, "Player Data");
					File.Delete(file_data);
				}
				catch
				{
				}

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
				Program.ExitApplication(0);
			}
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{

		}

		private void Yes_CheckedChanged(object sender, EventArgs e)
		{
			if (Yes.Checked)
			{
				No.Checked = false;

				BerryHappy.Hide();
				BerrySad.Show();
			}
		}

		private void No_CheckedChanged(object sender, EventArgs e)
		{
			if (No.Checked)
			{
				Yes.Checked = false;

				BerrySad.Hide();
				BerryHappy.Show();
			}
		}
	}
}
