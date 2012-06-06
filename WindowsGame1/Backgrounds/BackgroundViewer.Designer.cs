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
            this.AspectCheckbox = new System.Windows.Forms.CheckBox();
            this.size_yNum = new System.Windows.Forms.NumericUpDown();
            this.FixedPosCheckbox = new System.Windows.Forms.CheckBox();
            this.pos_xNum = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.speed_yNum = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.speed_xNum = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.size_xNum = new System.Windows.Forms.NumericUpDown();
            this.pos_yNum = new System.Windows.Forms.NumericUpDown();
            this.UV_Page = new System.Windows.Forms.TabPage();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.numericUpDown5 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown6 = new System.Windows.Forms.NumericUpDown();
            this.Texture_Page = new System.Windows.Forms.TabPage();
            this.TextureCombobox = new System.Windows.Forms.ComboBox();
            this.TextureButton = new System.Windows.Forms.Button();
            this.LayerBox = new System.Windows.Forms.GroupBox();
            this.NewFloaterButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.ParallaxNum = new System.Windows.Forms.NumericUpDown();
            this.PlayButton = new System.Windows.Forms.Button();
            this.ResetButton = new System.Windows.Forms.Button();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NewLayerButton = new System.Windows.Forms.Button();
            this.ToolTips = new System.Windows.Forms.ToolTip(this.components);
            this.QuadBox.SuspendLayout();
            this.QuadTab.SuspendLayout();
            this.XY_Page.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.size_yNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pos_xNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.speed_yNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.speed_xNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.size_xNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pos_yNum)).BeginInit();
            this.UV_Page.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).BeginInit();
            this.Texture_Page.SuspendLayout();
            this.LayerBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ParallaxNum)).BeginInit();
            this.MenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // LayerTree
            // 
            this.LayerTree.AllowDrop = true;
            this.LayerTree.Location = new System.Drawing.Point(12, 292);
            this.LayerTree.Name = "LayerTree";
            this.LayerTree.Size = new System.Drawing.Size(272, 430);
            this.LayerTree.TabIndex = 0;
            this.LayerTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.LayerTree_AfterSelect);
            // 
            // QuadBox
            // 
            this.QuadBox.Controls.Add(this.QuadTab);
            this.QuadBox.Location = new System.Drawing.Point(12, 129);
            this.QuadBox.Name = "QuadBox";
            this.QuadBox.Size = new System.Drawing.Size(272, 158);
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
            this.QuadTab.Size = new System.Drawing.Size(262, 133);
            this.QuadTab.TabIndex = 15;
            // 
            // XY_Page
            // 
            this.XY_Page.Controls.Add(this.AspectCheckbox);
            this.XY_Page.Controls.Add(this.size_yNum);
            this.XY_Page.Controls.Add(this.FixedPosCheckbox);
            this.XY_Page.Controls.Add(this.pos_xNum);
            this.XY_Page.Controls.Add(this.label5);
            this.XY_Page.Controls.Add(this.label1);
            this.XY_Page.Controls.Add(this.speed_yNum);
            this.XY_Page.Controls.Add(this.label2);
            this.XY_Page.Controls.Add(this.speed_xNum);
            this.XY_Page.Controls.Add(this.label3);
            this.XY_Page.Controls.Add(this.label4);
            this.XY_Page.Controls.Add(this.size_xNum);
            this.XY_Page.Controls.Add(this.pos_yNum);
            this.XY_Page.Location = new System.Drawing.Point(4, 22);
            this.XY_Page.Name = "XY_Page";
            this.XY_Page.Padding = new System.Windows.Forms.Padding(3);
            this.XY_Page.Size = new System.Drawing.Size(254, 107);
            this.XY_Page.TabIndex = 0;
            this.XY_Page.Text = "X, Y";
            this.XY_Page.UseVisualStyleBackColor = true;
            this.XY_Page.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // AspectCheckbox
            // 
            this.AspectCheckbox.AutoSize = true;
            this.AspectCheckbox.Location = new System.Drawing.Point(96, 70);
            this.AspectCheckbox.Name = "AspectCheckbox";
            this.AspectCheckbox.Size = new System.Drawing.Size(100, 17);
            this.AspectCheckbox.TabIndex = 12;
            this.AspectCheckbox.Text = "File aspect ratio";
            this.ToolTips.SetToolTip(this.AspectCheckbox, "Force the quad\'s aspect ratio to be the aspect ratio of it\'s texture.\\nThe height" +
                    " of the quad will be scaled; the width will be unaffected.");
            this.AspectCheckbox.UseVisualStyleBackColor = true;
            this.AspectCheckbox.CheckedChanged += new System.EventHandler(this.AspectCheckbox_CheckedChanged);
            // 
            // size_yNum
            // 
            this.size_yNum.DecimalPlaces = 2;
            this.size_yNum.Location = new System.Drawing.Point(96, 43);
            this.size_yNum.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.size_yNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.size_yNum.Name = "size_yNum";
            this.size_yNum.Size = new System.Drawing.Size(64, 20);
            this.size_yNum.TabIndex = 7;
            this.size_yNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // FixedPosCheckbox
            // 
            this.FixedPosCheckbox.AutoSize = true;
            this.FixedPosCheckbox.Location = new System.Drawing.Point(11, 70);
            this.FixedPosCheckbox.Name = "FixedPosCheckbox";
            this.FixedPosCheckbox.Size = new System.Drawing.Size(71, 17);
            this.FixedPosCheckbox.TabIndex = 11;
            this.FixedPosCheckbox.Text = "Fixed pos";
            this.ToolTips.SetToolTip(this.FixedPosCheckbox, "Fix the position of the quad relative to the camera.");
            this.FixedPosCheckbox.UseVisualStyleBackColor = true;
            this.FixedPosCheckbox.CheckedChanged += new System.EventHandler(this.FixedPosCheckbox_CheckedChanged);
            // 
            // pos_xNum
            // 
            this.pos_xNum.DecimalPlaces = 2;
            this.pos_xNum.Location = new System.Drawing.Point(26, 22);
            this.pos_xNum.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.pos_xNum.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.pos_xNum.Name = "pos_xNum";
            this.pos_xNum.Size = new System.Drawing.Size(64, 20);
            this.pos_xNum.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(163, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "speed";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(12, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "x";
            // 
            // speed_yNum
            // 
            this.speed_yNum.DecimalPlaces = 2;
            this.speed_yNum.Location = new System.Drawing.Point(166, 43);
            this.speed_yNum.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.speed_yNum.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.speed_yNum.Name = "speed_yNum";
            this.speed_yNum.Size = new System.Drawing.Size(64, 20);
            this.speed_yNum.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "y";
            // 
            // speed_xNum
            // 
            this.speed_xNum.DecimalPlaces = 2;
            this.speed_xNum.Location = new System.Drawing.Point(166, 22);
            this.speed_xNum.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.speed_xNum.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.speed_xNum.Name = "speed_xNum";
            this.speed_xNum.Size = new System.Drawing.Size(64, 20);
            this.speed_xNum.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "pos";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(93, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "size";
            // 
            // size_xNum
            // 
            this.size_xNum.DecimalPlaces = 2;
            this.size_xNum.Location = new System.Drawing.Point(96, 22);
            this.size_xNum.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.size_xNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.size_xNum.Name = "size_xNum";
            this.size_xNum.Size = new System.Drawing.Size(64, 20);
            this.size_xNum.TabIndex = 6;
            this.size_xNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // pos_yNum
            // 
            this.pos_yNum.DecimalPlaces = 2;
            this.pos_yNum.Location = new System.Drawing.Point(26, 43);
            this.pos_yNum.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.pos_yNum.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.pos_yNum.Name = "pos_yNum";
            this.pos_yNum.Size = new System.Drawing.Size(64, 20);
            this.pos_yNum.TabIndex = 5;
            // 
            // UV_Page
            // 
            this.UV_Page.Controls.Add(this.numericUpDown1);
            this.UV_Page.Controls.Add(this.numericUpDown2);
            this.UV_Page.Controls.Add(this.label7);
            this.UV_Page.Controls.Add(this.label8);
            this.UV_Page.Controls.Add(this.numericUpDown3);
            this.UV_Page.Controls.Add(this.label9);
            this.UV_Page.Controls.Add(this.numericUpDown4);
            this.UV_Page.Controls.Add(this.label10);
            this.UV_Page.Controls.Add(this.label11);
            this.UV_Page.Controls.Add(this.numericUpDown5);
            this.UV_Page.Controls.Add(this.numericUpDown6);
            this.UV_Page.Location = new System.Drawing.Point(4, 22);
            this.UV_Page.Name = "UV_Page";
            this.UV_Page.Padding = new System.Windows.Forms.Padding(3);
            this.UV_Page.Size = new System.Drawing.Size(254, 107);
            this.UV_Page.TabIndex = 1;
            this.UV_Page.Text = "U, V";
            this.UV_Page.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.DecimalPlaces = 2;
            this.numericUpDown1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericUpDown1.Location = new System.Drawing.Point(96, 43);
            this.numericUpDown1.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(64, 20);
            this.numericUpDown1.TabIndex = 18;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.DecimalPlaces = 2;
            this.numericUpDown2.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericUpDown2.Location = new System.Drawing.Point(26, 22);
            this.numericUpDown2.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(64, 20);
            this.numericUpDown2.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(163, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "speed";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(13, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "u";
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.DecimalPlaces = 2;
            this.numericUpDown3.Location = new System.Drawing.Point(166, 43);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown3.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(64, 20);
            this.numericUpDown3.TabIndex = 20;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 45);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(13, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "v";
            // 
            // numericUpDown4
            // 
            this.numericUpDown4.DecimalPlaces = 2;
            this.numericUpDown4.Location = new System.Drawing.Point(166, 22);
            this.numericUpDown4.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown4.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numericUpDown4.Name = "numericUpDown4";
            this.numericUpDown4.Size = new System.Drawing.Size(64, 20);
            this.numericUpDown4.TabIndex = 19;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(23, 6);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(56, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "bottom left";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(93, 6);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(45, 13);
            this.label11.TabIndex = 15;
            this.label11.Text = "top right";
            // 
            // numericUpDown5
            // 
            this.numericUpDown5.DecimalPlaces = 2;
            this.numericUpDown5.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericUpDown5.Location = new System.Drawing.Point(96, 22);
            this.numericUpDown5.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDown5.Name = "numericUpDown5";
            this.numericUpDown5.Size = new System.Drawing.Size(64, 20);
            this.numericUpDown5.TabIndex = 17;
            this.numericUpDown5.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numericUpDown6
            // 
            this.numericUpDown6.DecimalPlaces = 2;
            this.numericUpDown6.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericUpDown6.Location = new System.Drawing.Point(26, 43);
            this.numericUpDown6.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDown6.Name = "numericUpDown6";
            this.numericUpDown6.Size = new System.Drawing.Size(64, 20);
            this.numericUpDown6.TabIndex = 16;
            // 
            // Texture_Page
            // 
            this.Texture_Page.Controls.Add(this.TextureCombobox);
            this.Texture_Page.Controls.Add(this.TextureButton);
            this.Texture_Page.Location = new System.Drawing.Point(4, 22);
            this.Texture_Page.Name = "Texture_Page";
            this.Texture_Page.Size = new System.Drawing.Size(254, 107);
            this.Texture_Page.TabIndex = 2;
            this.Texture_Page.Text = "Texture";
            this.Texture_Page.UseVisualStyleBackColor = true;
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
            this.LayerBox.Controls.Add(this.NewFloaterButton);
            this.LayerBox.Controls.Add(this.label6);
            this.LayerBox.Controls.Add(this.ParallaxNum);
            this.LayerBox.Location = new System.Drawing.Point(12, 71);
            this.LayerBox.Name = "LayerBox";
            this.LayerBox.Size = new System.Drawing.Size(272, 52);
            this.LayerBox.TabIndex = 2;
            this.LayerBox.TabStop = false;
            this.LayerBox.Text = "Layer";
            // 
            // NewFloaterButton
            // 
            this.NewFloaterButton.Location = new System.Drawing.Point(236, 13);
            this.NewFloaterButton.Name = "NewFloaterButton";
            this.NewFloaterButton.Size = new System.Drawing.Size(28, 28);
            this.NewFloaterButton.TabIndex = 16;
            this.NewFloaterButton.Text = "N F";
            this.ToolTips.SetToolTip(this.NewFloaterButton, "New Quad (K)");
            this.NewFloaterButton.UseVisualStyleBackColor = true;
            this.NewFloaterButton.Click += new System.EventHandler(this.NewFloaterButton_Click);
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
            // PlayButton
            // 
            this.PlayButton.Location = new System.Drawing.Point(222, 27);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(28, 28);
            this.PlayButton.TabIndex = 3;
            this.PlayButton.Text = "Play";
            this.PlayButton.UseVisualStyleBackColor = true;
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(252, 27);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(28, 28);
            this.ResetButton.TabIndex = 4;
            this.ResetButton.Text = "Reset";
            this.ResetButton.UseVisualStyleBackColor = true;
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
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
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
            this.NewLayerButton.Text = "N L";
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
            // BackgroundViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 735);
            this.Controls.Add(this.NewLayerButton);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.PlayButton);
            this.Controls.Add(this.LayerBox);
            this.Controls.Add(this.QuadBox);
            this.Controls.Add(this.LayerTree);
            this.Controls.Add(this.MenuStrip);
            this.MainMenuStrip = this.MenuStrip;
            this.Name = "BackgroundViewer";
            this.Text = "BackgroundView";
            this.TopMost = true;
            this.QuadBox.ResumeLayout(false);
            this.QuadTab.ResumeLayout(false);
            this.XY_Page.ResumeLayout(false);
            this.XY_Page.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.size_yNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pos_xNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.speed_yNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.speed_xNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.size_xNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pos_yNum)).EndInit();
            this.UV_Page.ResumeLayout(false);
            this.UV_Page.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).EndInit();
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
        public System.Windows.Forms.NumericUpDown pos_xNum;
        public System.Windows.Forms.GroupBox LayerBox;
        public System.Windows.Forms.Button PlayButton;
        public System.Windows.Forms.Button ResetButton;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.NumericUpDown speed_yNum;
        public System.Windows.Forms.NumericUpDown speed_xNum;
        public System.Windows.Forms.NumericUpDown size_yNum;
        public System.Windows.Forms.NumericUpDown size_xNum;
        public System.Windows.Forms.NumericUpDown pos_yNum;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.NumericUpDown ParallaxNum;
        public System.Windows.Forms.Label StatusLabel;
        public System.Windows.Forms.CheckBox AspectCheckbox;
        public System.Windows.Forms.CheckBox FixedPosCheckbox;
        public System.Windows.Forms.MenuStrip MenuStrip;
        public System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        public System.Windows.Forms.TabControl QuadTab;
        public System.Windows.Forms.TabPage XY_Page;
        public System.Windows.Forms.TabPage UV_Page;
        public System.Windows.Forms.NumericUpDown numericUpDown1;
        public System.Windows.Forms.NumericUpDown numericUpDown2;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label label8;
        public System.Windows.Forms.NumericUpDown numericUpDown3;
        public System.Windows.Forms.Label label9;
        public System.Windows.Forms.NumericUpDown numericUpDown4;
        public System.Windows.Forms.Label label10;
        public System.Windows.Forms.Label label11;
        public System.Windows.Forms.NumericUpDown numericUpDown5;
        public System.Windows.Forms.NumericUpDown numericUpDown6;
        private System.Windows.Forms.Button TextureButton;
        private System.Windows.Forms.ComboBox TextureCombobox;
        private System.Windows.Forms.TabPage Texture_Page;
        public System.Windows.Forms.Button NewLayerButton;
        public System.Windows.Forms.Button NewFloaterButton;
        private System.Windows.Forms.ToolTip ToolTips;
    }
}
#endif