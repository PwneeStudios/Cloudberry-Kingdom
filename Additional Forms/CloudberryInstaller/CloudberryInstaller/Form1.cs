using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CloudberryEULA
{
	public partial class Installer : Form
	{
		public Installer()
		{
			InitializeComponent();
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void Checkbox_CheckedChanged(object sender, EventArgs e)
		{
			NextButton.Enabled = Checkbox.Checked;
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			Program.ExitApplication(1);
		}

		private void NextButton_Click(object sender, EventArgs e)
		{
			Program.ExitApplication(0);
		}
	}
}
