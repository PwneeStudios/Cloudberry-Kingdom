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
            this.ItemTree.Location = new System.Drawing.Point(16, 84);
            this.ItemTree.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ItemTree.Name = "ItemTree";
            this.ItemTree.Size = new System.Drawing.Size(327, 767);
            this.ItemTree.TabIndex = 0;
            this.ItemTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ItemTree_AfterSelect);
            // 
            // ResizeButton
            // 
            this.ResizeButton.BackColor = System.Drawing.Color.Red;
            this.ResizeButton.Font = new System.Drawing.Font("akaDylan Plain", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResizeButton.Location = new System.Drawing.Point(125, 15);
            this.ResizeButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ResizeButton.Name = "ResizeButton";
            this.ResizeButton.Size = new System.Drawing.Size(67, 62);
            this.ResizeButton.TabIndex = 2;
            this.ResizeButton.Text = "Re- size";
            this.ResizeButton.UseVisualStyleBackColor = false;
            // 
            // ImportButton
            // 
            this.ImportButton.BackColor = System.Drawing.Color.Lime;
            this.ImportButton.Font = new System.Drawing.Font("akaDylan Plain", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ImportButton.Location = new System.Drawing.Point(200, 15);
            this.ImportButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.Size = new System.Drawing.Size(67, 62);
            this.ImportButton.TabIndex = 4;
            this.ImportButton.Text = "Im- port";
            this.ImportButton.UseVisualStyleBackColor = false;
            // 
            // FreeButton
            // 
            this.FreeButton.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.FreeButton.Font = new System.Drawing.Font("akaDylan Plain", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FreeButton.Location = new System.Drawing.Point(275, 15);
            this.FreeButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.FreeButton.Name = "FreeButton";
            this.FreeButton.Size = new System.Drawing.Size(67, 62);
            this.FreeButton.TabIndex = 5;
            this.FreeButton.Text = "Free";
            this.FreeButton.UseVisualStyleBackColor = false;
            // 
            // WidthBar
            // 
            this.WidthBar.Location = new System.Drawing.Point(16, 15);
            this.WidthBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.WidthBar.Maximum = 100;
            this.WidthBar.Name = "WidthBar";
            this.WidthBar.Size = new System.Drawing.Size(101, 56);
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
            this.WidthText.Location = new System.Drawing.Point(37, 55);
            this.WidthText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.WidthText.Name = "WidthText";
            this.WidthText.Size = new System.Drawing.Size(50, 25);
            this.WidthText.TabIndex = 7;
            this.WidthText.Text = "1.00";
            // 
            // Toolbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 866);
            this.Controls.Add(this.WidthText);
            this.Controls.Add(this.WidthBar);
            this.Controls.Add(this.FreeButton);
            this.Controls.Add(this.ImportButton);
            this.Controls.Add(this.ResizeButton);
            this.Controls.Add(this.ItemTree);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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