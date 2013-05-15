namespace CloudberryCleanup
{
	partial class Uninstaller
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Uninstaller));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.Yes = new System.Windows.Forms.RadioButton();
			this.No = new System.Windows.Forms.RadioButton();
			this.NextButton = new System.Windows.Forms.Button();
			this.BerryHappy = new System.Windows.Forms.PictureBox();
			this.BerrySad = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.BerryHappy)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.BerrySad)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("GROBOLD", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(23, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(449, 25);
			this.label1.TabIndex = 0;
			this.label1.Text = "Cloudberry Kingdom is being uninstalled.";
			this.label1.Click += new System.EventHandler(this.label1_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Georgia", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(24, 65);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(375, 20);
			this.label2.TabIndex = 1;
			this.label2.Text = "Would you like to delete your saved data as well?";
			// 
			// Yes
			// 
			this.Yes.AutoSize = true;
			this.Yes.Location = new System.Drawing.Point(72, 116);
			this.Yes.Name = "Yes";
			this.Yes.Size = new System.Drawing.Size(197, 21);
			this.Yes.TabIndex = 2;
			this.Yes.TabStop = true;
			this.Yes.Text = "Yes, delete my saved files.";
			this.Yes.UseVisualStyleBackColor = true;
			this.Yes.CheckedChanged += new System.EventHandler(this.Yes_CheckedChanged);
			// 
			// No
			// 
			this.No.AutoSize = true;
			this.No.Location = new System.Drawing.Point(72, 143);
			this.No.Name = "No";
			this.No.Size = new System.Drawing.Size(183, 21);
			this.No.TabIndex = 3;
			this.No.TabStop = true;
			this.No.Text = "No, keep my saved files.";
			this.No.UseVisualStyleBackColor = true;
			this.No.CheckedChanged += new System.EventHandler(this.No_CheckedChanged);
			// 
			// NextButton
			// 
			this.NextButton.Location = new System.Drawing.Point(72, 191);
			this.NextButton.Name = "NextButton";
			this.NextButton.Size = new System.Drawing.Size(100, 30);
			this.NextButton.TabIndex = 4;
			this.NextButton.Text = "Finish";
			this.NextButton.UseVisualStyleBackColor = true;
			this.NextButton.Click += new System.EventHandler(this.button1_Click);
			// 
			// BerryHappy
			// 
			this.BerryHappy.Image = ((System.Drawing.Image)(resources.GetObject("BerryHappy.Image")));
			this.BerryHappy.InitialImage = null;
			this.BerryHappy.Location = new System.Drawing.Point(241, 9);
			this.BerryHappy.Name = "BerryHappy";
			this.BerryHappy.Size = new System.Drawing.Size(389, 330);
			this.BerryHappy.TabIndex = 5;
			this.BerryHappy.TabStop = false;
			this.BerryHappy.Click += new System.EventHandler(this.pictureBox1_Click);
			// 
			// BerrySad
			// 
			this.BerrySad.Image = ((System.Drawing.Image)(resources.GetObject("BerrySad.Image")));
			this.BerrySad.InitialImage = null;
			this.BerrySad.Location = new System.Drawing.Point(250, -33);
			this.BerrySad.Name = "BerrySad";
			this.BerrySad.Size = new System.Drawing.Size(389, 399);
			this.BerrySad.TabIndex = 6;
			this.BerrySad.TabStop = false;
			// 
			// Uninstaller
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(538, 299);
			this.Controls.Add(this.NextButton);
			this.Controls.Add(this.No);
			this.Controls.Add(this.Yes);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.BerryHappy);
			this.Controls.Add(this.BerrySad);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "Uninstaller";
			this.Text = "Cloudberry Kingdom Uninstaller";
			((System.ComponentModel.ISupportInitialize)(this.BerryHappy)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.BerrySad)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton Yes;
		private System.Windows.Forms.RadioButton No;
		private System.Windows.Forms.Button NextButton;
		private System.Windows.Forms.PictureBox BerryHappy;
		private System.Windows.Forms.PictureBox BerrySad;
	}
}

