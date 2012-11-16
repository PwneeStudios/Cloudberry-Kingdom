#if WINDOWS && INCLUDE_EDITOR
namespace CloudberryKingdom
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
            this.offset_VectorBox = new System.Windows.Forms.TextBox();
            this.repeat_VectorBox = new System.Windows.Forms.TextBox();
            this.speed_VectorBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.Texture_Page = new System.Windows.Forms.TabPage();
            this.BlendAddRatioTrack = new System.Windows.Forms.TrackBar();
            this.EffectComboBox = new System.Windows.Forms.ComboBox();
            this.AlphaTrack = new System.Windows.Forms.TrackBar();
            this.ColorButton = new System.Windows.Forms.Button();
            this.TextureCombobox = new System.Windows.Forms.ComboBox();
            this.TextureButton = new System.Windows.Forms.Button();
            this.Extra_Page = new System.Windows.Forms.TabPage();
            this.ExtraTexture2Combobox = new System.Windows.Forms.ComboBox();
            this.ExtraTexture1Combobox = new System.Windows.Forms.ComboBox();
            this.Info = new System.Windows.Forms.TabPage();
            this.InfoNumLabel = new System.Windows.Forms.Label();
            this.InfoNumberNumbox = new System.Windows.Forms.NumericUpDown();
            this.NameLabel = new System.Windows.Forms.Label();
            this.InfoNameTextbox = new System.Windows.Forms.TextBox();
            this.LayerBox = new System.Windows.Forms.GroupBox();
            this.FixedCheckbox = new System.Windows.Forms.CheckBox();
            this.ForegroundCheckbox = new System.Windows.Forms.CheckBox();
            this.LayerShowCheckbox = new System.Windows.Forms.CheckBox();
            this.LayerLockCheckbox = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ParallaxNum = new System.Windows.Forms.NumericUpDown();
            this.NewFloaterButton = new System.Windows.Forms.Button();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dumpToCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boundingBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.axisLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NewLayerButton = new System.Windows.Forms.Button();
            this.ToolTips = new System.Windows.Forms.ToolTip(this.components);
            this.CameraMoveCheckbox = new System.Windows.Forms.CheckBox();
            this.PlayCheckbox = new System.Windows.Forms.CheckBox();
            this.MoveBoundsCheckbox = new System.Windows.Forms.CheckBox();
            this.MoveQuadsCheckbox = new System.Windows.Forms.CheckBox();
            this.QuadBox.SuspendLayout();
            this.QuadTab.SuspendLayout();
            this.XY_Page.SuspendLayout();
            this.UV_Page.SuspendLayout();
            this.Texture_Page.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BlendAddRatioTrack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AlphaTrack)).BeginInit();
            this.Extra_Page.SuspendLayout();
            this.Info.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InfoNumberNumbox)).BeginInit();
            this.LayerBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ParallaxNum)).BeginInit();
            this.MenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // LayerTree
            // 
            this.LayerTree.AllowDrop = true;
            this.LayerTree.Location = new System.Drawing.Point(16, 399);
            this.LayerTree.Margin = new System.Windows.Forms.Padding(4);
            this.LayerTree.Name = "LayerTree";
            this.LayerTree.Size = new System.Drawing.Size(317, 489);
            this.LayerTree.TabIndex = 20;
            this.LayerTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.LayerTree_AfterSelect);
            // 
            // QuadBox
            // 
            this.QuadBox.Controls.Add(this.QuadTab);
            this.QuadBox.Location = new System.Drawing.Point(16, 196);
            this.QuadBox.Margin = new System.Windows.Forms.Padding(4);
            this.QuadBox.Name = "QuadBox";
            this.QuadBox.Padding = new System.Windows.Forms.Padding(4);
            this.QuadBox.Size = new System.Drawing.Size(319, 196);
            this.QuadBox.TabIndex = 1;
            this.QuadBox.TabStop = false;
            this.QuadBox.Text = "Quad";
            // 
            // QuadTab
            // 
            this.QuadTab.Controls.Add(this.XY_Page);
            this.QuadTab.Controls.Add(this.UV_Page);
            this.QuadTab.Controls.Add(this.Texture_Page);
            this.QuadTab.Controls.Add(this.Extra_Page);
            this.QuadTab.Controls.Add(this.Info);
            this.QuadTab.Location = new System.Drawing.Point(8, 23);
            this.QuadTab.Margin = new System.Windows.Forms.Padding(4);
            this.QuadTab.Name = "QuadTab";
            this.QuadTab.SelectedIndex = 0;
            this.QuadTab.Size = new System.Drawing.Size(303, 160);
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
            this.XY_Page.Location = new System.Drawing.Point(4, 25);
            this.XY_Page.Margin = new System.Windows.Forms.Padding(4);
            this.XY_Page.Name = "XY_Page";
            this.XY_Page.Padding = new System.Windows.Forms.Padding(4);
            this.XY_Page.Size = new System.Drawing.Size(295, 131);
            this.XY_Page.TabIndex = 0;
            this.XY_Page.Text = "X, Y";
            this.XY_Page.UseVisualStyleBackColor = true;
            this.XY_Page.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // pos_VectorBox
            // 
            this.pos_VectorBox.BackColor = System.Drawing.SystemColors.Control;
            this.pos_VectorBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pos_VectorBox.Location = new System.Drawing.Point(83, 15);
            this.pos_VectorBox.Margin = new System.Windows.Forms.Padding(4);
            this.pos_VectorBox.Name = "pos_VectorBox";
            this.pos_VectorBox.Size = new System.Drawing.Size(171, 15);
            this.pos_VectorBox.TabIndex = 9;
            // 
            // size_VectorBox
            // 
            this.size_VectorBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.size_VectorBox.Location = new System.Drawing.Point(83, 38);
            this.size_VectorBox.Margin = new System.Windows.Forms.Padding(4);
            this.size_VectorBox.Name = "size_VectorBox";
            this.size_VectorBox.Size = new System.Drawing.Size(171, 15);
            this.size_VectorBox.TabIndex = 10;
            // 
            // vel_VectorBox
            // 
            this.vel_VectorBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.vel_VectorBox.Location = new System.Drawing.Point(83, 59);
            this.vel_VectorBox.Margin = new System.Windows.Forms.Padding(4);
            this.vel_VectorBox.Name = "vel_VectorBox";
            this.vel_VectorBox.Size = new System.Drawing.Size(171, 15);
            this.vel_VectorBox.TabIndex = 11;
            // 
            // AspectCheckbox
            // 
            this.AspectCheckbox.AutoSize = true;
            this.AspectCheckbox.Location = new System.Drawing.Point(25, 82);
            this.AspectCheckbox.Margin = new System.Windows.Forms.Padding(4);
            this.AspectCheckbox.Name = "AspectCheckbox";
            this.AspectCheckbox.Size = new System.Drawing.Size(130, 21);
            this.AspectCheckbox.TabIndex = 12;
            this.AspectCheckbox.Text = "File aspect ratio";
            this.ToolTips.SetToolTip(this.AspectCheckbox, "Force the quad\'s aspect ratio to be the aspect ratio of it\'s texture.");
            this.AspectCheckbox.UseVisualStyleBackColor = true;
            this.AspectCheckbox.CheckedChanged += new System.EventHandler(this.AspectCheckbox_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 59);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 17);
            this.label5.TabIndex = 10;
            this.label5.Text = "speed";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 15);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "pos";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 38);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 17);
            this.label4.TabIndex = 4;
            this.label4.Text = "size";
            // 
            // UV_Page
            // 
            this.UV_Page.Controls.Add(this.offset_VectorBox);
            this.UV_Page.Controls.Add(this.repeat_VectorBox);
            this.UV_Page.Controls.Add(this.speed_VectorBox);
            this.UV_Page.Controls.Add(this.label1);
            this.UV_Page.Controls.Add(this.label2);
            this.UV_Page.Controls.Add(this.label7);
            this.UV_Page.Location = new System.Drawing.Point(4, 25);
            this.UV_Page.Margin = new System.Windows.Forms.Padding(4);
            this.UV_Page.Name = "UV_Page";
            this.UV_Page.Padding = new System.Windows.Forms.Padding(4);
            this.UV_Page.Size = new System.Drawing.Size(295, 131);
            this.UV_Page.TabIndex = 1;
            this.UV_Page.Text = "U, V";
            this.UV_Page.UseVisualStyleBackColor = true;
            // 
            // offset_VectorBox
            // 
            this.offset_VectorBox.BackColor = System.Drawing.SystemColors.Control;
            this.offset_VectorBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.offset_VectorBox.Location = new System.Drawing.Point(83, 15);
            this.offset_VectorBox.Margin = new System.Windows.Forms.Padding(4);
            this.offset_VectorBox.Name = "offset_VectorBox";
            this.offset_VectorBox.Size = new System.Drawing.Size(171, 15);
            this.offset_VectorBox.TabIndex = 13;
            // 
            // repeat_VectorBox
            // 
            this.repeat_VectorBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.repeat_VectorBox.Location = new System.Drawing.Point(83, 38);
            this.repeat_VectorBox.Margin = new System.Windows.Forms.Padding(4);
            this.repeat_VectorBox.Name = "repeat_VectorBox";
            this.repeat_VectorBox.Size = new System.Drawing.Size(171, 15);
            this.repeat_VectorBox.TabIndex = 14;
            // 
            // speed_VectorBox
            // 
            this.speed_VectorBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.speed_VectorBox.Location = new System.Drawing.Point(83, 59);
            this.speed_VectorBox.Margin = new System.Windows.Forms.Padding(4);
            this.speed_VectorBox.Name = "speed_VectorBox";
            this.speed_VectorBox.Size = new System.Drawing.Size(171, 15);
            this.speed_VectorBox.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 59);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 17);
            this.label1.TabIndex = 29;
            this.label1.Text = "speed";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 15);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 17);
            this.label2.TabIndex = 27;
            this.label2.Text = "offset";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 38);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 17);
            this.label7.TabIndex = 28;
            this.label7.Text = "repeat";
            // 
            // Texture_Page
            // 
            this.Texture_Page.Controls.Add(this.BlendAddRatioTrack);
            this.Texture_Page.Controls.Add(this.EffectComboBox);
            this.Texture_Page.Controls.Add(this.AlphaTrack);
            this.Texture_Page.Controls.Add(this.ColorButton);
            this.Texture_Page.Controls.Add(this.TextureCombobox);
            this.Texture_Page.Controls.Add(this.TextureButton);
            this.Texture_Page.Location = new System.Drawing.Point(4, 25);
            this.Texture_Page.Margin = new System.Windows.Forms.Padding(4);
            this.Texture_Page.Name = "Texture_Page";
            this.Texture_Page.Size = new System.Drawing.Size(295, 131);
            this.Texture_Page.TabIndex = 2;
            this.Texture_Page.Text = "Texture";
            this.Texture_Page.UseVisualStyleBackColor = true;
            // 
            // BlendAddRatioTrack
            // 
            this.BlendAddRatioTrack.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BlendAddRatioTrack.Location = new System.Drawing.Point(111, 87);
            this.BlendAddRatioTrack.Maximum = 100;
            this.BlendAddRatioTrack.Name = "BlendAddRatioTrack";
            this.BlendAddRatioTrack.Size = new System.Drawing.Size(104, 56);
            this.BlendAddRatioTrack.TabIndex = 21;
            this.BlendAddRatioTrack.TickFrequency = 10;
            this.BlendAddRatioTrack.TickStyle = System.Windows.Forms.TickStyle.None;
            this.BlendAddRatioTrack.Scroll += new System.EventHandler(this.BlendAddRatioTrack_Scroll);
            // 
            // EffectComboBox
            // 
            this.EffectComboBox.FormattingEnabled = true;
            this.EffectComboBox.Location = new System.Drawing.Point(131, 46);
            this.EffectComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.EffectComboBox.Name = "EffectComboBox";
            this.EffectComboBox.Size = new System.Drawing.Size(160, 24);
            this.EffectComboBox.TabIndex = 20;
            this.EffectComboBox.SelectedIndexChanged += new System.EventHandler(this.EffectComboBox_SelectedIndexChanged);
            // 
            // AlphaTrack
            // 
            this.AlphaTrack.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.AlphaTrack.Location = new System.Drawing.Point(5, 87);
            this.AlphaTrack.Maximum = 100;
            this.AlphaTrack.Name = "AlphaTrack";
            this.AlphaTrack.Size = new System.Drawing.Size(104, 56);
            this.AlphaTrack.TabIndex = 19;
            this.AlphaTrack.TickFrequency = 10;
            this.AlphaTrack.TickStyle = System.Windows.Forms.TickStyle.None;
            this.AlphaTrack.Scroll += new System.EventHandler(this.AlphaTrack_Scroll);
            // 
            // ColorButton
            // 
            this.ColorButton.Location = new System.Drawing.Point(4, 46);
            this.ColorButton.Margin = new System.Windows.Forms.Padding(4);
            this.ColorButton.Name = "ColorButton";
            this.ColorButton.Size = new System.Drawing.Size(37, 34);
            this.ColorButton.TabIndex = 18;
            this.ColorButton.Text = "C";
            this.ToolTips.SetToolTip(this.ColorButton, "Color shading of the quad (O)");
            this.ColorButton.UseVisualStyleBackColor = true;
            this.ColorButton.Click += new System.EventHandler(this.ColorButton_Click);
            // 
            // TextureCombobox
            // 
            this.TextureCombobox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.TextureCombobox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.TextureCombobox.FormattingEnabled = true;
            this.TextureCombobox.Location = new System.Drawing.Point(131, 10);
            this.TextureCombobox.Margin = new System.Windows.Forms.Padding(4);
            this.TextureCombobox.Name = "TextureCombobox";
            this.TextureCombobox.Size = new System.Drawing.Size(160, 24);
            this.TextureCombobox.TabIndex = 17;
            this.TextureCombobox.SelectedIndexChanged += new System.EventHandler(this.TextureCombobox_SelectedIndexChanged);
            // 
            // TextureButton
            // 
            this.TextureButton.Location = new System.Drawing.Point(4, 4);
            this.TextureButton.Margin = new System.Windows.Forms.Padding(4);
            this.TextureButton.Name = "TextureButton";
            this.TextureButton.Size = new System.Drawing.Size(37, 34);
            this.TextureButton.TabIndex = 16;
            this.TextureButton.Text = "T";
            this.ToolTips.SetToolTip(this.TextureButton, "Texture file used for drawing the quad (T)");
            this.TextureButton.UseVisualStyleBackColor = true;
            this.TextureButton.Click += new System.EventHandler(this.TextureButton_Click);
            // 
            // Extra_Page
            // 
            this.Extra_Page.Controls.Add(this.ExtraTexture2Combobox);
            this.Extra_Page.Controls.Add(this.ExtraTexture1Combobox);
            this.Extra_Page.Location = new System.Drawing.Point(4, 25);
            this.Extra_Page.Name = "Extra_Page";
            this.Extra_Page.Size = new System.Drawing.Size(295, 131);
            this.Extra_Page.TabIndex = 3;
            this.Extra_Page.Text = "Extra";
            this.Extra_Page.UseVisualStyleBackColor = true;
            // 
            // ExtraTexture2Combobox
            // 
            this.ExtraTexture2Combobox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ExtraTexture2Combobox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ExtraTexture2Combobox.FormattingEnabled = true;
            this.ExtraTexture2Combobox.Location = new System.Drawing.Point(4, 36);
            this.ExtraTexture2Combobox.Margin = new System.Windows.Forms.Padding(4);
            this.ExtraTexture2Combobox.Name = "ExtraTexture2Combobox";
            this.ExtraTexture2Combobox.Size = new System.Drawing.Size(160, 24);
            this.ExtraTexture2Combobox.TabIndex = 19;
            this.ExtraTexture2Combobox.SelectedIndexChanged += new System.EventHandler(this.ExtraTexture2Combobox_SelectedIndexChanged);
            // 
            // ExtraTexture1Combobox
            // 
            this.ExtraTexture1Combobox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ExtraTexture1Combobox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ExtraTexture1Combobox.FormattingEnabled = true;
            this.ExtraTexture1Combobox.Location = new System.Drawing.Point(4, 4);
            this.ExtraTexture1Combobox.Margin = new System.Windows.Forms.Padding(4);
            this.ExtraTexture1Combobox.Name = "ExtraTexture1Combobox";
            this.ExtraTexture1Combobox.Size = new System.Drawing.Size(160, 24);
            this.ExtraTexture1Combobox.TabIndex = 18;
            this.ExtraTexture1Combobox.SelectedIndexChanged += new System.EventHandler(this.ExtraTexture1Combobox_SelectedIndexChanged);
            // 
            // Info
            // 
            this.Info.Controls.Add(this.InfoNumLabel);
            this.Info.Controls.Add(this.InfoNumberNumbox);
            this.Info.Controls.Add(this.NameLabel);
            this.Info.Controls.Add(this.InfoNameTextbox);
            this.Info.Location = new System.Drawing.Point(4, 25);
            this.Info.Name = "Info";
            this.Info.Padding = new System.Windows.Forms.Padding(3);
            this.Info.Size = new System.Drawing.Size(295, 131);
            this.Info.TabIndex = 4;
            this.Info.Text = "Info";
            this.Info.UseVisualStyleBackColor = true;
            this.Info.Click += new System.EventHandler(this.Info_Click);
            // 
            // InfoNumLabel
            // 
            this.InfoNumLabel.AutoSize = true;
            this.InfoNumLabel.Location = new System.Drawing.Point(116, 49);
            this.InfoNumLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.InfoNumLabel.Name = "InfoNumLabel";
            this.InfoNumLabel.Size = new System.Drawing.Size(56, 17);
            this.InfoNumLabel.TabIndex = 16;
            this.InfoNumLabel.Text = "number";
            // 
            // InfoNumberNumbox
            // 
            this.InfoNumberNumbox.Location = new System.Drawing.Point(23, 44);
            this.InfoNumberNumbox.Margin = new System.Windows.Forms.Padding(4);
            this.InfoNumberNumbox.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.InfoNumberNumbox.Name = "InfoNumberNumbox";
            this.InfoNumberNumbox.Size = new System.Drawing.Size(85, 22);
            this.InfoNumberNumbox.TabIndex = 15;
            this.ToolTips.SetToolTip(this.InfoNumberNumbox, "The number for this node.");
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(146, 20);
            this.NameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(43, 17);
            this.NameLabel.TabIndex = 13;
            this.NameLabel.Text = "name";
            // 
            // InfoNameTextbox
            // 
            this.InfoNameTextbox.Location = new System.Drawing.Point(23, 15);
            this.InfoNameTextbox.Name = "InfoNameTextbox";
            this.InfoNameTextbox.Size = new System.Drawing.Size(118, 22);
            this.InfoNameTextbox.TabIndex = 0;
            this.ToolTips.SetToolTip(this.InfoNameTextbox, "The name for this node.");
            // 
            // LayerBox
            // 
            this.LayerBox.Controls.Add(this.FixedCheckbox);
            this.LayerBox.Controls.Add(this.ForegroundCheckbox);
            this.LayerBox.Controls.Add(this.LayerShowCheckbox);
            this.LayerBox.Controls.Add(this.LayerLockCheckbox);
            this.LayerBox.Controls.Add(this.label8);
            this.LayerBox.Controls.Add(this.label6);
            this.LayerBox.Controls.Add(this.ParallaxNum);
            this.LayerBox.Location = new System.Drawing.Point(16, 114);
            this.LayerBox.Margin = new System.Windows.Forms.Padding(4);
            this.LayerBox.Name = "LayerBox";
            this.LayerBox.Padding = new System.Windows.Forms.Padding(4);
            this.LayerBox.Size = new System.Drawing.Size(319, 74);
            this.LayerBox.TabIndex = 2;
            this.LayerBox.TabStop = false;
            this.LayerBox.Text = "Layer";
            // 
            // FixedCheckbox
            // 
            this.FixedCheckbox.AutoSize = true;
            this.FixedCheckbox.Location = new System.Drawing.Point(161, 39);
            this.FixedCheckbox.Margin = new System.Windows.Forms.Padding(4);
            this.FixedCheckbox.Name = "FixedCheckbox";
            this.FixedCheckbox.Size = new System.Drawing.Size(63, 21);
            this.FixedCheckbox.TabIndex = 14;
            this.FixedCheckbox.Text = "Fixed";
            this.ToolTips.SetToolTip(this.FixedCheckbox, "Lock the layer, so it can\'t be moved (K)");
            this.FixedCheckbox.UseVisualStyleBackColor = true;
            // 
            // ForegroundCheckbox
            // 
            this.ForegroundCheckbox.AutoSize = true;
            this.ForegroundCheckbox.Location = new System.Drawing.Point(161, 16);
            this.ForegroundCheckbox.Margin = new System.Windows.Forms.Padding(4);
            this.ForegroundCheckbox.Name = "ForegroundCheckbox";
            this.ForegroundCheckbox.Size = new System.Drawing.Size(59, 21);
            this.ForegroundCheckbox.TabIndex = 13;
            this.ForegroundCheckbox.Text = "Fore";
            this.ToolTips.SetToolTip(this.ForegroundCheckbox, "Lock the layer, so it can\'t be moved (K)");
            this.ForegroundCheckbox.UseVisualStyleBackColor = true;
            this.ForegroundCheckbox.CheckedChanged += new System.EventHandler(this.ForegroundCheckbox_CheckedChanged);
            // 
            // LayerShowCheckbox
            // 
            this.LayerShowCheckbox.AutoSize = true;
            this.LayerShowCheckbox.Location = new System.Drawing.Point(228, 41);
            this.LayerShowCheckbox.Margin = new System.Windows.Forms.Padding(4);
            this.LayerShowCheckbox.Name = "LayerShowCheckbox";
            this.LayerShowCheckbox.Size = new System.Drawing.Size(18, 17);
            this.LayerShowCheckbox.TabIndex = 8;
            this.ToolTips.SetToolTip(this.LayerShowCheckbox, "Show the layer (S)");
            this.LayerShowCheckbox.UseVisualStyleBackColor = true;
            this.LayerShowCheckbox.CheckedChanged += new System.EventHandler(this.LayerShowCheckbox_CheckedChanged);
            // 
            // LayerLockCheckbox
            // 
            this.LayerLockCheckbox.AutoSize = true;
            this.LayerLockCheckbox.Location = new System.Drawing.Point(228, 16);
            this.LayerLockCheckbox.Margin = new System.Windows.Forms.Padding(4);
            this.LayerLockCheckbox.Name = "LayerLockCheckbox";
            this.LayerLockCheckbox.Size = new System.Drawing.Size(18, 17);
            this.LayerLockCheckbox.TabIndex = 7;
            this.ToolTips.SetToolTip(this.LayerLockCheckbox, "Lock the layer, so it can\'t be moved (K)");
            this.LayerLockCheckbox.UseVisualStyleBackColor = true;
            this.LayerLockCheckbox.CheckedChanged += new System.EventHandler(this.LayerLockCheckbox_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(96, 25);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 17);
            this.label8.TabIndex = 12;
            this.label8.Text = "parallax";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(101, 26);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 17);
            this.label6.TabIndex = 12;
            this.label6.Text = "parallax";
            this.ToolTips.SetToolTip(this.label6, "Parallax of the selected layer.");
            // 
            // ParallaxNum
            // 
            this.ParallaxNum.DecimalPlaces = 2;
            this.ParallaxNum.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.ParallaxNum.Location = new System.Drawing.Point(8, 23);
            this.ParallaxNum.Margin = new System.Windows.Forms.Padding(4);
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
            this.ParallaxNum.Size = new System.Drawing.Size(85, 22);
            this.ParallaxNum.TabIndex = 6;
            this.ToolTips.SetToolTip(this.ParallaxNum, "Parallax of the selected layer (X)");
            this.ParallaxNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.ParallaxNum.ValueChanged += new System.EventHandler(this.ParallaxNum_ValueChanged);
            // 
            // NewFloaterButton
            // 
            this.NewFloaterButton.Location = new System.Drawing.Point(55, 33);
            this.NewFloaterButton.Margin = new System.Windows.Forms.Padding(4);
            this.NewFloaterButton.Name = "NewFloaterButton";
            this.NewFloaterButton.Size = new System.Drawing.Size(37, 34);
            this.NewFloaterButton.TabIndex = 2;
            this.NewFloaterButton.Text = "Floater";
            this.ToolTips.SetToolTip(this.NewFloaterButton, "New Quad (Q)");
            this.NewFloaterButton.UseVisualStyleBackColor = true;
            this.NewFloaterButton.Click += new System.EventHandler(this.NewFloaterButton_Click);
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Location = new System.Drawing.Point(273, 68);
            this.StatusLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(60, 17);
            this.StatusLabel.TabIndex = 13;
            this.StatusLabel.Text = "Paused.";
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.showToolStripMenuItem});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.MenuStrip.Size = new System.Drawing.Size(349, 28);
            this.MenuStrip.TabIndex = 14;
            this.MenuStrip.Text = "menuStrip1";
            this.MenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MenuStrip_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.dumpToCodeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(174, 24);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(174, 24);
            this.saveAsToolStripMenuItem.Text = "Save as";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(174, 24);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // dumpToCodeToolStripMenuItem
            // 
            this.dumpToCodeToolStripMenuItem.Name = "dumpToCodeToolStripMenuItem";
            this.dumpToCodeToolStripMenuItem.Size = new System.Drawing.Size(174, 24);
            this.dumpToCodeToolStripMenuItem.Text = "Dump to code";
            this.dumpToCodeToolStripMenuItem.Click += new System.EventHandler(this.dumpToCodeToolStripMenuItem_Click);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cameraBoxToolStripMenuItem,
            this.boundingBoxToolStripMenuItem,
            this.axisLinesToolStripMenuItem});
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
            this.showToolStripMenuItem.Text = "Show";
            // 
            // cameraBoxToolStripMenuItem
            // 
            this.cameraBoxToolStripMenuItem.Checked = true;
            this.cameraBoxToolStripMenuItem.CheckOnClick = true;
            this.cameraBoxToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cameraBoxToolStripMenuItem.Name = "cameraBoxToolStripMenuItem";
            this.cameraBoxToolStripMenuItem.Size = new System.Drawing.Size(171, 24);
            this.cameraBoxToolStripMenuItem.Text = "Camera Box";
            // 
            // boundingBoxToolStripMenuItem
            // 
            this.boundingBoxToolStripMenuItem.Checked = true;
            this.boundingBoxToolStripMenuItem.CheckOnClick = true;
            this.boundingBoxToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.boundingBoxToolStripMenuItem.Name = "boundingBoxToolStripMenuItem";
            this.boundingBoxToolStripMenuItem.Size = new System.Drawing.Size(171, 24);
            this.boundingBoxToolStripMenuItem.Text = "Bounding Box";
            // 
            // axisLinesToolStripMenuItem
            // 
            this.axisLinesToolStripMenuItem.Checked = true;
            this.axisLinesToolStripMenuItem.CheckOnClick = true;
            this.axisLinesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.axisLinesToolStripMenuItem.Name = "axisLinesToolStripMenuItem";
            this.axisLinesToolStripMenuItem.Size = new System.Drawing.Size(171, 24);
            this.axisLinesToolStripMenuItem.Text = "Axis Lines";
            // 
            // NewLayerButton
            // 
            this.NewLayerButton.Location = new System.Drawing.Point(16, 33);
            this.NewLayerButton.Margin = new System.Windows.Forms.Padding(4);
            this.NewLayerButton.Name = "NewLayerButton";
            this.NewLayerButton.Size = new System.Drawing.Size(37, 34);
            this.NewLayerButton.TabIndex = 1;
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
            this.CameraMoveCheckbox.Location = new System.Drawing.Point(100, 33);
            this.CameraMoveCheckbox.Margin = new System.Windows.Forms.Padding(4);
            this.CameraMoveCheckbox.Name = "CameraMoveCheckbox";
            this.CameraMoveCheckbox.Size = new System.Drawing.Size(37, 34);
            this.CameraMoveCheckbox.TabIndex = 3;
            this.CameraMoveCheckbox.Text = "Cam";
            this.ToolTips.SetToolTip(this.CameraMoveCheckbox, "Move Camera (C)");
            this.CameraMoveCheckbox.UseVisualStyleBackColor = true;
            this.CameraMoveCheckbox.CheckedChanged += new System.EventHandler(this.CameraMoveCheckbox_CheckedChanged);
            // 
            // PlayCheckbox
            // 
            this.PlayCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.PlayCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.PlayCheckbox.Location = new System.Drawing.Point(297, 30);
            this.PlayCheckbox.Margin = new System.Windows.Forms.Padding(4);
            this.PlayCheckbox.Name = "PlayCheckbox";
            this.PlayCheckbox.Size = new System.Drawing.Size(37, 34);
            this.PlayCheckbox.TabIndex = 5;
            this.PlayCheckbox.Text = "Play";
            this.ToolTips.SetToolTip(this.PlayCheckbox, "Play or pause the level (P)");
            this.PlayCheckbox.UseVisualStyleBackColor = true;
            this.PlayCheckbox.CheckedChanged += new System.EventHandler(this.PlayCheckbox_CheckedChanged);
            // 
            // MoveBoundsCheckbox
            // 
            this.MoveBoundsCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.MoveBoundsCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MoveBoundsCheckbox.Location = new System.Drawing.Point(139, 33);
            this.MoveBoundsCheckbox.Margin = new System.Windows.Forms.Padding(4);
            this.MoveBoundsCheckbox.Name = "MoveBoundsCheckbox";
            this.MoveBoundsCheckbox.Size = new System.Drawing.Size(37, 34);
            this.MoveBoundsCheckbox.TabIndex = 4;
            this.MoveBoundsCheckbox.Text = "Bounds";
            this.ToolTips.SetToolTip(this.MoveBoundsCheckbox, "Move Bounds (B)");
            this.MoveBoundsCheckbox.UseVisualStyleBackColor = true;
            this.MoveBoundsCheckbox.CheckedChanged += new System.EventHandler(this.MoveBoundsCheckbox_CheckedChanged);
            // 
            // MoveQuadsCheckbox
            // 
            this.MoveQuadsCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.MoveQuadsCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MoveQuadsCheckbox.Location = new System.Drawing.Point(177, 33);
            this.MoveQuadsCheckbox.Margin = new System.Windows.Forms.Padding(4);
            this.MoveQuadsCheckbox.Name = "MoveQuadsCheckbox";
            this.MoveQuadsCheckbox.Size = new System.Drawing.Size(37, 34);
            this.MoveQuadsCheckbox.TabIndex = 21;
            this.MoveQuadsCheckbox.Text = "Quads";
            this.ToolTips.SetToolTip(this.MoveQuadsCheckbox, "Move Quads (M)");
            this.MoveQuadsCheckbox.UseVisualStyleBackColor = true;
            this.MoveQuadsCheckbox.CheckedChanged += new System.EventHandler(this.MoveQuadsCheckbox_CheckedChanged);
            // 
            // BackgroundViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 905);
            this.Controls.Add(this.MoveQuadsCheckbox);
            this.Controls.Add(this.MoveBoundsCheckbox);
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
            this.Margin = new System.Windows.Forms.Padding(4);
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
            this.Texture_Page.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BlendAddRatioTrack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AlphaTrack)).EndInit();
            this.Extra_Page.ResumeLayout(false);
            this.Info.ResumeLayout(false);
            this.Info.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InfoNumberNumbox)).EndInit();
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
        private System.Windows.Forms.TextBox offset_VectorBox;
        private System.Windows.Forms.TextBox repeat_VectorBox;
        private System.Windows.Forms.TextBox speed_VectorBox;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox MoveBoundsCheckbox;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boundingBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem axisLinesToolStripMenuItem;
        private System.Windows.Forms.CheckBox MoveQuadsCheckbox;
        private System.Windows.Forms.ToolStripMenuItem dumpToCodeToolStripMenuItem;
        private System.Windows.Forms.CheckBox FixedCheckbox;
        private System.Windows.Forms.CheckBox ForegroundCheckbox;
        public System.Windows.Forms.Label label8;
        private System.Windows.Forms.TrackBar AlphaTrack;
        private System.Windows.Forms.ComboBox EffectComboBox;
        private System.Windows.Forms.TabPage Extra_Page;
        private System.Windows.Forms.ComboBox ExtraTexture2Combobox;
        private System.Windows.Forms.ComboBox ExtraTexture1Combobox;
        private System.Windows.Forms.TabPage Info;
        public System.Windows.Forms.Label InfoNumLabel;
        public System.Windows.Forms.NumericUpDown InfoNumberNumbox;
        public System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.TextBox InfoNameTextbox;
        private System.Windows.Forms.TrackBar BlendAddRatioTrack;
    }
}
#endif