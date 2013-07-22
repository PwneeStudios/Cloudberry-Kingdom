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
			this.SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			this.label1.Click += new System.EventHandler(this.label1_Click);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// Yes
			// 
			resources.ApplyResources(this.Yes, "Yes");
			this.Yes.Name = "Yes";
			this.Yes.TabStop = true;
			this.Yes.UseVisualStyleBackColor = true;
			this.Yes.CheckedChanged += new System.EventHandler(this.Yes_CheckedChanged);
			// 
			// No
			// 
			resources.ApplyResources(this.No, "No");
			this.No.Name = "No";
			this.No.TabStop = true;
			this.No.UseVisualStyleBackColor = true;
			this.No.CheckedChanged += new System.EventHandler(this.No_CheckedChanged);
			// 
			// NextButton
			// 
			resources.ApplyResources(this.NextButton, "NextButton");
			this.NextButton.Name = "NextButton";
			this.NextButton.UseVisualStyleBackColor = true;
			this.NextButton.Click += new System.EventHandler(this.button1_Click);
			// 
			// Uninstaller
			// 
			this.AcceptButton = this.NextButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.NextButton);
			this.Controls.Add(this.No);
			this.Controls.Add(this.Yes);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "Uninstaller";
			this.ShowIcon = false;
			this.Load += new System.EventHandler(this.Uninstaller_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton Yes;
		private System.Windows.Forms.RadioButton No;
        private System.Windows.Forms.Button NextButton;
	}
}

