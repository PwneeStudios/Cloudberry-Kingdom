#if WINDOWS && DEBUG
namespace CloudberryKingdom.Viewer
{
    public partial class BackgroundViewer
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
            this.components = new System.ComponentModel.Container();
            this.LayerTree = new System.Windows.Forms.TreeView();
            this.QuadBox = new System.Windows.Forms.GroupBox();
            this.QuadTab = new System.Windows.Forms.TabControl();
            this.XY_Page = new System.Windows.Forms.TabPage();
            this.pos_VectorBox = new System.Windows.Forms.TextBox();
            this.size_VectorBox = new System.Windows.Forms.TextBox();
            this.vel_VectorBox = new System.Windows.Forms.TextBox();
            this.AspectCheckbox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.UV_Page = new System.Windows.Forms.TabPage();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.Texture_Page = new System.Windows.Forms.TabPage();
            this.ColorButton = new System.Windows.Forms.Button();
            this.TextureCombobox = new System.Windows.Forms.ComboBox();
            this.TextureButton = new System.Windows.Forms.Button();
            this.LayerBox = new System.Windows.Forms.GroupBox();
            this.LayerShowCheckbox = new System.Windows.Forms.CheckBox();
            this.LayerLockCheckbox = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.ParallaxNum = new System.Windows.Forms.NumericUpDown();
            this.NewFloaterButton = new System.Windows.Forms.Button();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NewLayerButton = new System.Windows.Forms.Button();
            this.ToolTips = new System.Windows.Forms.ToolTip(this.components);
            this.CameraMoveCheckbox = new System.Windows.Forms.CheckBox();
            this.PlayCheckbox = new System.Windows.Forms.CheckBox();
            this.QuadBox.SuspendLayout();
            this.QuadTab.SuspendLayout();
            this.XY_Page.SuspendLayout();
            this.UV_Page.SuspendLayout();
            this.Texture_Page.SuspendLayout();
            this.LayerBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ParallaxNum)).BeginInit();
            this.MenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // LayerTree
            // 
            this.LayerTree.AllowDrop = true;
            this.LayerTree.Location = new System.Drawing.Point(12, 324);
            this.LayerTree.Name = "LayerTree";
            this.LayerTree.Size = new System.Drawing.Size(272, 398);
            this.LayerTree.TabIndex = 0;
            this.LayerTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.LayerTree_AfterSelect);
            // 
            // QuadBox
            // 
            this.QuadBox.Controls.Add(this.QuadTab);
            this.QuadBox.Location = new System.Drawing.Point(12, 159);
            this.QuadBox.Name = "QuadBox";
            this.QuadBox.Size = new System.Drawing.Size(272, 159);
            this.QuadBox.TabIndex = 1;
            this.QuadBox.TabStop = false;
            this.QuadBox.Text = "Quad";
            // 
            // QuadTab
            // 
            this.QuadTab.Controls.Add(this.XY_Page);
            this.QuadTab.Controls.Add(this.UV_Page);
            this.QuadTab.Controls.Add(this.Texture_Page);
            this.QuadTab.Location = new System.Drawing.Point(6, 19);
            this.QuadTab.Name = "QuadTab";
            this.QuadTab.SelectedIndex = 0;
            this.QuadTab.Size = new System.Drawing.Size(262, 130);
            this.QuadTab.TabIndex = 15;
            // 
            // XY_Page
            // 
            this.XY_Page.Controls.Add(this.pos_VectorBox);
            this.XY_Page.Controls.Add(this.size_VectorBox);
            this.XY_Page.Controls.Add(this.vel_VectorBox);
            this.XY_Page.Controls.Add(this.AspectCheckbox);
            this.XY_Page.Controls.Add(this.label5);
            this.XY_Page.Controls.Add(this.label3);
            this.XY_Page.Controls.Add(this.label4);
            this.XY_Page.Location = new System.Drawing.Point(4, 22);
            this.XY_Page.Name = "XY_Page";
            this.XY_Page.Padding = new System.Windows.Forms.Padding(3);
            this.XY_Page.Size = new System.Drawing.Size(254, 104);
            this.XY_Page.TabIndex = 0;
            this.XY_Page.Text = "X, Y";
            this.XY_Page.UseVisualStyleBackColor = true;
            this.XY_Page.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // pos_VectorBox
            // 
            this.pos_VectorBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pos_VectorBox.Location = new System.Drawing.Point(62, 14);
            this.pos_VectorBox.Name = "pos_VectorBox";
            this.pos_VectorBox.Size = new System.Drawing.Size(100, 13);
            this.pos_VectorBox.TabIndex = 26;
            // 
            // size_VectorBox
            // 
            this.size_VectorBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.size_VectorBox.Location = new System.Drawing.Point(62, 33);
            this.size_VectorBox.Name = "size_VectorBox";
            this.size_VectorBox.Size = new System.Drawing.Size(100, 13);
            this.size_VectorBox.TabIndex = 25;
            // 
            // vel_VectorBox
            // 
            this.vel_VectorBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.vel_VectorBox.Location = new System.Drawing.Point(62, 50);
            this.vel_VectorBox.Name = "vel_VectorBox";
            this.vel_VectorBox.Size = new System.Drawing.Size(100, 13);
            this.vel_VectorBox.TabIndex = 24;
            // 
            // AspectCheckbox
            // 
            this.AspectCheckbox.AutoSize = true;
            this.AspectCheckbox.Location = new System.Drawing.Point(19, 69);
            this.AspectCheckbox.Name = "AspectCheckbox";
            this.AspectCheckbox.Size = new System.Drawing.Size(100, 17);
            this.AspectCheckbox.TabIndex = 12;
            this.AspectCheckbox.Text = "File aspect ratio";
            this.ToolTips.SetToolTip(this.AspectCheckbox, "Force the quad\'s aspect ratio to be the aspect ratio of it\'s texture.\\nThe height" +
                    " of the quad will be scaled; the width will be unaffected.");
            this.AspectCheckbox.UseVisualStyleBackColor = true;
            this.AspectCheckbox.CheckedChanged += new System.EventHandler(this.AspectCheckbox_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "speed";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "pos";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "size";
            // 
            // UV_Page
            // 
            this.UV_Page.Controls.Add(this.textBox3);
            this.UV_Page.Controls.Add(this.textBox2);
            this.UV_Page.Controls.Add(this.textBox1);
            this.UV_Page.Controls.Add(this.label7);
            this.UV_Page.Controls.Add(this.label10);
            this.UV_Page.Controls.Add(this.label11);
            this.UV_Page.Location = new System.Drawing.Point(4, 22);
            this.UV_Page.Name = "UV_Page";
            this.UV_Page.Padding = new System.Windows.Forms.Padding(3);
            this.UV_Page.Size = new System.Drawing.Size(254, 130);
            this.UV_Page.TabIndex = 1;
            this.UV_Page.Text = "U, V";
            this.UV_Page.UseVisualStyleBackColor = true;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(62, 69);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 20);
            this.textBox3.TabIndex = 24;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(62, 40);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 23;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(62, 14);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 22;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 72);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "speed";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 17);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(33, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "offset";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(14, 43);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(42, 13);
            this.label11.TabIndex = 15;
            this.label11.Text = "repeats";
            // 
            // Texture_Page
            // 
            this.Texture_Page.Controls.Add(this.ColorButton);
            this.Texture_Page.Controls.Add(this.TextureCombobox);
            this.Texture_Page.Controls.Add(this.TextureButton);
            this.Texture_Page.Location = new System.Drawing.Point(4, 22);
            this.Texture_Page.Name = "Texture_Page";
            this.Texture_Page.Size = new System.Drawing.Size(254, 130);
            this.Texture_Page.TabIndex = 2;
            this.Texture_Page.Text = "Texture";
            this.Texture_Page.UseVisualStyleBackColor = true;
            // 
            // ColorButton
            // 
            this.ColorButton.Location = new System.Drawing.Point(3, 37);
            this.ColorButton.Name = "ColorButton";
            this.ColorButton.Size = new System.Drawing.Size(28, 28);
            this.ColorButton.TabIndex = 24;
            this.ColorButton.Text = "C";
            this.ColorButton.UseVisualStyleBackColor = true;
            this.ColorButton.Click += new System.EventHandler(this.ColorButton_Click);
            // 
            // TextureCombobox
            // 
            this.TextureCombobox.FormattingEnabled = true;
            this.TextureCombobox.Location = new System.Drawing.Point(39, 8);
            this.TextureCombobox.Name = "TextureCombobox";
            this.TextureCombobox.Size = new System.Drawing.Size(121, 21);
            this.TextureCombobox.TabIndex = 23;
            // 
            // TextureButton
            // 
            this.TextureButton.Location = new System.Drawing.Point(3, 3);
            this.TextureButton.Name = "TextureButton";
            this.TextureButton.Size = new System.Drawing.Size(28, 28);
            this.TextureButton.TabIndex = 22;
            this.TextureButton.Text = "T";
            this.TextureButton.UseVisualStyleBackColor = true;
            this.TextureButton.Click += new System.EventHandler(this.TextureButton_Click);
            // 
            // LayerBox
            // 
            this.LayerBox.Controls.Add(this.LayerShowCheckbox);
            this.LayerBox.Controls.Add(this.LayerLockCheckbox);
            this.LayerBox.Controls.Add(this.label6);
            this.LayerBox.Controls.Add(this.ParallaxNum);
            this.LayerBox.Location = new System.Drawing.Point(12, 93);
            this.LayerBox.Name = "LayerBox";
            this.LayerBox.Size = new System.Drawing.Size(272, 60);
            this.LayerBox.TabIndex = 2;
            this.LayerBox.TabStop = false;
            this.LayerBox.Text = "Layer";
            // 
            // LayerShowCheckbox
            // 
            this.LayerShowCheckbox.AutoSize = true;
            this.LayerShowCheckbox.Location = new System.Drawing.Point(193, 33);
            this.LayerShowCheckbox.Name = "LayerShowCheckbox";
            this.LayerShowCheckbox.Size = new System.Drawing.Size(15, 14);
            this.LayerShowCheckbox.TabIndex = 25;
            this.LayerShowCheckbox.UseVisualStyleBackColor = true;
            this.LayerShowCheckbox.CheckedChanged += new System.EventHandler(this.LayerShowCheckbox_CheckedChanged);
            // 
            // LayerLockCheckbox
            // 
            this.LayerLockCheckbox.AutoSize = true;
            this.LayerLockCheckbox.Location = new System.Drawing.Point(193, 13);
            this.LayerLockCheckbox.Name = "LayerLockCheckbox";
            this.LayerLockCheckbox.Size = new System.Drawing.Size(15, 14);
            this.LayerLockCheckbox.TabIndex = 24;
            this.LayerLockCheckbox.UseVisualStyleBackColor = true;
            this.LayerLockCheckbox.CheckedChanged += new System.EventHandler(this.LayerLockCheckbox_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(76, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "parallax";
            // 
            // ParallaxNum
            // 
            this.ParallaxNum.DecimalPlaces = 2;
            this.ParallaxNum.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.ParallaxNum.Location = new System.Drawing.Point(6, 19);
            this.ParallaxNum.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.ParallaxNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.ParallaxNum.Name = "ParallaxNum";
            this.ParallaxNum.Size = new System.Drawing.Size(64, 20);
            this.ParallaxNum.TabIndex = 11;
            this.ParallaxNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.ParallaxNum.ValueChanged += new System.EventHandler(this.ParallaxNum_ValueChanged);
            // 
            // NewFloaterButton
            // 
            this.NewFloaterButton.Location = new System.Drawing.Point(41, 27);
            this.NewFloaterButton.Name = "NewFloaterButton";
            this.NewFloaterButton.Size = new System.Drawing.Size(28, 28);
            this.NewFloaterButton.TabIndex = 16;
            this.NewFloaterButton.Text = "Floater";
            this.ToolTips.SetToolTip(this.NewFloaterButton, "New Quad (K)");
            this.NewFloaterButton.UseVisualStyleBackColor = true;
            this.NewFloaterButton.Click += new System.EventHandler(this.NewFloaterButton_Click);
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Location = new System.Drawing.Point(234, 55);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(46, 13);
            this.StatusLabel.TabIndex = 13;
            this.StatusLabel.Text = "Paused.";
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(296, 24);
            this.MenuStrip.TabIndex = 14;
            this.MenuStrip.Text = "menuStrip1";
            this.MenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MenuStrip_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.loadToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.saveAsToolStripMenuItem.Text = "Save as";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.loadToolStripMenuItem.Text = "Load";
            // 
            // NewLayerButton
            // 
            this.NewLayerButton.Location = new System.Drawing.Point(12, 27);
            this.NewLayerButton.Name = "NewLayerButton";
            this.NewLayerButton.Size = new System.Drawing.Size(28, 28);
            this.NewLayerButton.TabIndex = 15;
            this.NewLayerButton.Text = "Layer";
            this.ToolTips.SetToolTip(this.NewLayerButton, "New Layer (L)");
            this.NewLayerButton.UseVisualStyleBackColor = true;
            this.NewLayerButton.Click += new System.EventHandler(this.NewLayerButton_Click);
            // 
            // ToolTips
            // 
            this.ToolTips.AutoPopDelay = 50000;
            this.ToolTips.InitialDelay = 400;
            this.ToolTips.ReshowDelay = 100;
            this.ToolTips.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip1_Popup);
            // 
            // CameraMoveCheckbox
            // 
            this.CameraMoveCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.CameraMoveCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.CameraMoveCheckbox.Location = new System.Drawing.Point(75, 27);
            this.CameraMoveCheckbox.Name = "CameraMoveCheckbox";
            this.CameraMoveCheckbox.Size = new System.Drawing.Size(28, 28);
            this.CameraMoveCheckbox.TabIndex = 26;
            this.CameraMoveCheckbox.Text = "Cam";
            this.CameraMoveCheckbox.UseVisualStyleBackColor = true;
            this.CameraMoveCheckbox.CheckedChanged += new System.EventHandler(this.CameraMoveCheckbox_CheckedChanged);
            // 
            // PlayCheckbox
            // 
            this.PlayCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.PlayCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.PlayCheckbox.Location = new System.Drawing.Point(248, 24);
            this.PlayCheckbox.Name = "PlayCheckbox";
            this.PlayCheckbox.Size = new System.Drawing.Size(28, 28);
            this.PlayCheckbox.TabIndex = 27;
            this.PlayCheckbox.Text = "Play";
            this.PlayCheckbox.UseVisualStyleBackColor = true;
            this.PlayCheckbox.CheckedChanged += new System.EventHandler(this.PlayCheckbox_CheckedChanged);
            // 
            // BackgroundViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 735);
            this.Controls.Add(this.PlayCheckbox);
            this.Controls.Add(this.CameraMoveCheckbox);
            this.Controls.Add(this.NewFloaterButton);
            this.Controls.Add(this.NewLayerButton);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.LayerBox);
            this.Controls.Add(this.QuadBox);
            this.Controls.Add(this.LayerTree);
            this.Controls.Add(this.MenuStrip);
            this.MainMenuStrip = this.MenuStrip;
            this.Name = "BackgroundViewer";
            this.Text = "BackgroundView";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.BackgroundViewer_Load);
            this.QuadBox.ResumeLayout(false);
            this.QuadTab.ResumeLayout(false);
            this.XY_Page.ResumeLayout(false);
            this.XY_Page.PerformLayout();
            this.UV_Page.ResumeLayout(false);
            this.UV_Page.PerformLayout();
            this.Texture_Page.ResumeLayout(false);
            this.LayerBox.ResumeLayout(false);
            this.LayerBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ParallaxNum)).EndInit();
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TreeView LayerTree;
        public System.Windows.Forms.GroupBox QuadBox;
        public System.Windows.Forms.GroupBox LayerBox;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.NumericUpDown ParallaxNum;
        public System.Windows.Forms.Label StatusLabel;
        public System.Windows.Forms.CheckBox AspectCheckbox;
        public System.Windows.Forms.MenuStrip MenuStrip;
        public System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        public System.Windows.Forms.TabControl QuadTab;
        public System.Windows.Forms.TabPage XY_Page;
        public System.Windows.Forms.TabPage UV_Page;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label label10;
        public System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button TextureButton;
        private System.Windows.Forms.ComboBox TextureCombobox;
        private System.Windows.Forms.TabPage Texture_Page;
        public System.Windows.Forms.Button NewLayerButton;
        public System.Windows.Forms.Button NewFloaterButton;
        private System.Windows.Forms.ToolTip ToolTips;
        private System.Windows.Forms.CheckBox LayerLockCheckbox;
        private System.Windows.Forms.CheckBox LayerShowCheckbox;
        private System.Windows.Forms.Button ColorButton;
        private System.Windows.Forms.CheckBox CameraMoveCheckbox;
        public System.Windows.Forms.CheckBox PlayCheckbox;
        private System.Windows.Forms.TextBox pos_VectorBox;
        private System.Windows.Forms.TextBox size_VectorBox;
        private System.Windows.Forms.TextBox vel_VectorBox;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
    }
}
#endif