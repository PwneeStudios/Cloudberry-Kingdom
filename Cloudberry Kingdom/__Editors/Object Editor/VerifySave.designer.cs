#if WINDOWS
namespace CloudberryKingdom
{
    partial class VerifySaveDialog
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
            this.Button_OK = new System.Windows.Forms.Button();
            this.Button_No = new System.Windows.Forms.Button();
            this.FileName = new System.Windows.Forms.Label();
            this.TopText = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Button_OK
            // 
            this.Button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Button_OK.Font = new System.Drawing.Font("akaDylan Plain", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button_OK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.Button_OK.Location = new System.Drawing.Point(282, 93);
            this.Button_OK.Name = "Button_OK";
            this.Button_OK.Size = new System.Drawing.Size(73, 42);
            this.Button_OK.TabIndex = 1;
            this.Button_OK.Text = "OK";
            this.Button_OK.UseVisualStyleBackColor = true;
            this.Button_OK.Click += new System.EventHandler(this.button1_Click);
            // 
            // Button_No
            // 
            this.Button_No.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Button_No.Font = new System.Drawing.Font("akaDylan Plain", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button_No.ForeColor = System.Drawing.Color.Red;
            this.Button_No.Location = new System.Drawing.Point(361, 93);
            this.Button_No.Name = "Button_No";
            this.Button_No.Size = new System.Drawing.Size(73, 42);
            this.Button_No.TabIndex = 2;
            this.Button_No.Text = "NO";
            this.Button_No.UseVisualStyleBackColor = true;
            // 
            // FileName
            // 
            this.FileName.AutoSize = true;
            this.FileName.Font = new System.Drawing.Font("akaDylan Plain", 17.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FileName.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.FileName.Location = new System.Drawing.Point(58, 54);
            this.FileName.Name = "FileName";
            this.FileName.Size = new System.Drawing.Size(170, 32);
            this.FileName.TabIndex = 3;
            this.FileName.Text = "File name";
            // 
            // TopText
            // 
            this.TopText.AutoSize = true;
            this.TopText.Font = new System.Drawing.Font("akaDylan Plain", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TopText.Location = new System.Drawing.Point(14, 9);
            this.TopText.Name = "TopText";
            this.TopText.Size = new System.Drawing.Size(426, 27);
            this.TopText.TabIndex = 4;
            this.TopText.Text = "Are you sure you want to save";
            this.TopText.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("akaDylan Plain", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(389, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 49);
            this.label3.TabIndex = 5;
            this.label3.Text = "?";
            // 
            // VerifySaveDialog
            // 
            this.AcceptButton = this.Button_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Button_No;
            this.ClientSize = new System.Drawing.Size(457, 152);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TopText);
            this.Controls.Add(this.FileName);
            this.Controls.Add(this.Button_No);
            this.Controls.Add(this.Button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "VerifySaveDialog";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.NameDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Button_OK;
        private System.Windows.Forms.Button Button_No;
        public System.Windows.Forms.Label FileName;
        private System.Windows.Forms.Label TopText;
        private System.Windows.Forms.Label label3;
    }
}
#endif