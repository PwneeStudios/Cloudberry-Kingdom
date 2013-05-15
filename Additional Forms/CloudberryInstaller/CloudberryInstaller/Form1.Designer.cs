namespace CloudberryEULA
{
	partial class Installer
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Installer));
			this.EulaBox = new System.Windows.Forms.RichTextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.Checkbox = new System.Windows.Forms.CheckBox();
			this.CancelButton = new System.Windows.Forms.Button();
			this.NextButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// EulaBox
			// 
			this.EulaBox.Location = new System.Drawing.Point(16, 191);
			this.EulaBox.Name = "EulaBox";
			this.EulaBox.ReadOnly = true;
			this.EulaBox.Size = new System.Drawing.Size(497, 338);
			this.EulaBox.TabIndex = 0;
			this.EulaBox.Text = resources.GetString("EulaBox.Text");
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Georgia", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(12, 153);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(254, 23);
			this.label2.TabIndex = 2;
			this.label2.Text = "End User License Agreement";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::CloudberryEULA.Properties.Resources.XboxOfferBanner_500;
			this.pictureBox1.Location = new System.Drawing.Point(16, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(509, 125);
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			// 
			// Checkbox
			// 
			this.Checkbox.AutoSize = true;
			this.Checkbox.Location = new System.Drawing.Point(445, 546);
			this.Checkbox.Name = "Checkbox";
			this.Checkbox.Size = new System.Drawing.Size(68, 21);
			this.Checkbox.TabIndex = 4;
			this.Checkbox.Text = "Agree";
			this.Checkbox.UseVisualStyleBackColor = true;
			this.Checkbox.CheckedChanged += new System.EventHandler(this.Checkbox_CheckedChanged);
			// 
			// CancelButton
			// 
			this.CancelButton.Location = new System.Drawing.Point(317, 597);
			this.CancelButton.Name = "CancelButton";
			this.CancelButton.Size = new System.Drawing.Size(95, 27);
			this.CancelButton.TabIndex = 5;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.UseVisualStyleBackColor = true;
			this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
			// 
			// NextButton
			// 
			this.NextButton.Enabled = false;
			this.NextButton.Location = new System.Drawing.Point(418, 597);
			this.NextButton.Name = "NextButton";
			this.NextButton.Size = new System.Drawing.Size(95, 27);
			this.NextButton.TabIndex = 6;
			this.NextButton.Text = "Finish";
			this.NextButton.UseVisualStyleBackColor = true;
			this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
			// 
			// Installer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(529, 640);
			this.Controls.Add(this.NextButton);
			this.Controls.Add(this.CancelButton);
			this.Controls.Add(this.Checkbox);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.EulaBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "Installer";
			this.Text = "Cloudberry Kingdom Installer";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RichTextBox EulaBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.CheckBox Checkbox;
		private System.Windows.Forms.Button CancelButton;
		private System.Windows.Forms.Button NextButton;
	}
}

