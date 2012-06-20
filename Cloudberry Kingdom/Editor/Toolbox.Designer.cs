#if WINDOWS
namespace Drawing
{
    partial class Toolbox
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
            this.ItemTree = new System.Windows.Forms.TreeView();
            this.ResizeButton = new System.Windows.Forms.Button();
            this.ImportButton = new System.Windows.Forms.Button();
            this.FreeButton = new System.Windows.Forms.Button();
            this.WidthBar = new System.Windows.Forms.TrackBar();
            this.WidthText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.WidthBar)).BeginInit();
            this.SuspendLayout();
            // 
            // ItemTree
            // 
            this.ItemTree.AllowDrop = true;
            this.ItemTree.CheckBoxes = true;
            this.ItemTree.Location = new System.Drawing.Point(12, 68);
            this.ItemTree.Name = "ItemTree";
            this.ItemTree.Size = new System.Drawing.Size(246, 624);
            this.ItemTree.TabIndex = 0;
            this.ItemTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ItemTree_AfterSelect);
            // 
            // ResizeButton
            // 
            this.ResizeButton.BackColor = System.Drawing.Color.Red;
            this.ResizeButton.Font = new System.Drawing.Font("akaDylan Plain", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResizeButton.Location = new System.Drawing.Point(94, 12);
            this.ResizeButton.Name = "ResizeButton";
            this.ResizeButton.Size = new System.Drawing.Size(50, 50);
            this.ResizeButton.TabIndex = 2;
            this.ResizeButton.Text = "Re- size";
            this.ResizeButton.UseVisualStyleBackColor = false;
            // 
            // ImportButton
            // 
            this.ImportButton.BackColor = System.Drawing.Color.Lime;
            this.ImportButton.Font = new System.Drawing.Font("akaDylan Plain", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ImportButton.Location = new System.Drawing.Point(150, 12);
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.Size = new System.Drawing.Size(50, 50);
            this.ImportButton.TabIndex = 4;
            this.ImportButton.Text = "Im- port";
            this.ImportButton.UseVisualStyleBackColor = false;
            // 
            // FreeButton
            // 
            this.FreeButton.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.FreeButton.Font = new System.Drawing.Font("akaDylan Plain", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FreeButton.Location = new System.Drawing.Point(206, 12);
            this.FreeButton.Name = "FreeButton";
            this.FreeButton.Size = new System.Drawing.Size(50, 50);
            this.FreeButton.TabIndex = 5;
            this.FreeButton.Text = "Free";
            this.FreeButton.UseVisualStyleBackColor = false;
            // 
            // WidthBar
            // 
            this.WidthBar.Location = new System.Drawing.Point(12, 12);
            this.WidthBar.Maximum = 100;
            this.WidthBar.Name = "WidthBar";
            this.WidthBar.Size = new System.Drawing.Size(76, 45);
            this.WidthBar.TabIndex = 6;
            this.WidthBar.TickFrequency = 20;
            this.WidthBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.WidthBar.Value = 100;
            this.WidthBar.Scroll += new System.EventHandler(this.WidthBar_Scroll);
            // 
            // WidthText
            // 
            this.WidthText.AutoSize = true;
            this.WidthText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WidthText.Location = new System.Drawing.Point(28, 45);
            this.WidthText.Name = "WidthText";
            this.WidthText.Size = new System.Drawing.Size(40, 20);
            this.WidthText.TabIndex = 7;
            this.WidthText.Text = "1.00";
            // 
            // Toolbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 704);
            this.Controls.Add(this.WidthText);
            this.Controls.Add(this.WidthBar);
            this.Controls.Add(this.FreeButton);
            this.Controls.Add(this.ImportButton);
            this.Controls.Add(this.ResizeButton);
            this.Controls.Add(this.ItemTree);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Toolbox";
            this.Text = "Toolbox";
            this.Load += new System.EventHandler(this.Toolbox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.WidthBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TreeView ItemTree;
        public System.Windows.Forms.Button ResizeButton;
        public System.Windows.Forms.Button ImportButton;
        public System.Windows.Forms.Button FreeButton;
        private System.Windows.Forms.TrackBar WidthBar;
        private System.Windows.Forms.Label WidthText;
    }
}
#endif