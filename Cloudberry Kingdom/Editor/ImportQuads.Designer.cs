#if WINDOWS
namespace Drawing
{
    partial class ImportQuads
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
            this.FileName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.LoadButton = new System.Windows.Forms.Button();
            this.Tree = new System.Windows.Forms.TreeView();
            this.label3 = new System.Windows.Forms.Label();
            this.Button_OK = new System.Windows.Forms.Button();
            this.Button_No = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.AnimListBox = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // FileName
            // 
            this.FileName.AutoSize = true;
            this.FileName.Font = new System.Drawing.Font("akaDylan Plain", 17.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FileName.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.FileName.Location = new System.Drawing.Point(12, 27);
            this.FileName.Name = "FileName";
            this.FileName.Size = new System.Drawing.Size(170, 32);
            this.FileName.TabIndex = 4;
            this.FileName.Text = "File name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "File name";
            // 
            // LoadButton
            // 
            this.LoadButton.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.LoadButton.Font = new System.Drawing.Font("akaDylan Plain", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadButton.Location = new System.Drawing.Point(205, 12);
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.Size = new System.Drawing.Size(50, 50);
            this.LoadButton.TabIndex = 6;
            this.LoadButton.Text = "Load";
            this.LoadButton.UseVisualStyleBackColor = false;
            this.LoadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // Tree
            // 
            this.Tree.CheckBoxes = true;
            this.Tree.Location = new System.Drawing.Point(18, 92);
            this.Tree.Name = "Tree";
            this.Tree.Size = new System.Drawing.Size(260, 244);
            this.Tree.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Quads";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // Button_OK
            // 
            this.Button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Button_OK.Font = new System.Drawing.Font("akaDylan Plain", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button_OK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.Button_OK.Location = new System.Drawing.Point(297, 342);
            this.Button_OK.Name = "Button_OK";
            this.Button_OK.Size = new System.Drawing.Size(73, 42);
            this.Button_OK.TabIndex = 9;
            this.Button_OK.Text = "OK";
            this.Button_OK.UseVisualStyleBackColor = true;
            this.Button_OK.Click += new System.EventHandler(this.Button_OK_Click);
            // 
            // Button_No
            // 
            this.Button_No.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Button_No.Font = new System.Drawing.Font("akaDylan Plain", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button_No.ForeColor = System.Drawing.Color.Red;
            this.Button_No.Location = new System.Drawing.Point(376, 342);
            this.Button_No.Name = "Button_No";
            this.Button_No.Size = new System.Drawing.Size(73, 42);
            this.Button_No.TabIndex = 10;
            this.Button_No.Text = "NO";
            this.Button_No.UseVisualStyleBackColor = true;
            this.Button_No.Click += new System.EventHandler(this.Button_No_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(284, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Animations";
            // 
            // AnimListBox
            // 
            this.AnimListBox.CheckOnClick = true;
            this.AnimListBox.FormattingEnabled = true;
            this.AnimListBox.Location = new System.Drawing.Point(287, 92);
            this.AnimListBox.Name = "AnimListBox";
            this.AnimListBox.Size = new System.Drawing.Size(162, 244);
            this.AnimListBox.TabIndex = 13;
            this.AnimListBox.SelectedIndexChanged += new System.EventHandler(this.AnimListBox_SelectedIndexChanged);
            // 
            // ImportQuads
            // 
            this.AcceptButton = this.Button_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Button_No;
            this.ClientSize = new System.Drawing.Size(466, 411);
            this.Controls.Add(this.AnimListBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Button_No);
            this.Controls.Add(this.Button_OK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Tree);
            this.Controls.Add(this.LoadButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FileName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ImportQuads";
            this.Text = "Import Quads";
            this.Load += new System.EventHandler(this.ImportQuads_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label FileName;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button LoadButton;
        private System.Windows.Forms.TreeView Tree;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Button_OK;
        private System.Windows.Forms.Button Button_No;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckedListBox AnimListBox;
    }
}
#endif