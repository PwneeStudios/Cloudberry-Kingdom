using System;
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
				Program.ExitApplication(0);
			}
			else
			{
				Program.ExitApplication(1);
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
