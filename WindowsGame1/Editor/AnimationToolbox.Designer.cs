#if WINDOWS
namespace Drawing
{
    partial class AnimationToolbox
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
            this.AnimList = new System.Windows.Forms.ListBox();
            this.TimeScroll = new System.Windows.Forms.HScrollBar();
            this.Time = new System.Windows.Forms.NumericUpDown();
            this.PlayButton = new System.Windows.Forms.Button();
            this.StopButton = new System.Windows.Forms.Button();
            this.LoopCheckBox = new System.Windows.Forms.CheckBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Time)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // AnimList
            // 
            this.AnimList.FormattingEnabled = true;
            this.AnimList.Location = new System.Drawing.Point(12, 11);
            this.AnimList.Name = "AnimList";
            this.AnimList.Size = new System.Drawing.Size(132, 225);
            this.AnimList.TabIndex = 0;
            // 
            // TimeScroll
            // 
            this.TimeScroll.Location = new System.Drawing.Point(147, 11);
            this.TimeScroll.Name = "TimeScroll";
            this.TimeScroll.Size = new System.Drawing.Size(180, 57);
            this.TimeScroll.TabIndex = 1;
            // 
            // Time
            // 
            this.Time.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Time.Location = new System.Drawing.Point(330, 12);
            this.Time.Name = "Time";
            this.Time.Size = new System.Drawing.Size(89, 24);
            this.Time.TabIndex = 2;
            // 
            // PlayButton
            // 
            this.PlayButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayButton.ForeColor = System.Drawing.Color.Green;
            this.PlayButton.Location = new System.Drawing.Point(330, 42);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(44, 26);
            this.PlayButton.TabIndex = 3;
            this.PlayButton.Text = "P";
            this.PlayButton.UseVisualStyleBackColor = true;
            // 
            // StopButton
            // 
            this.StopButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StopButton.ForeColor = System.Drawing.Color.Red;
            this.StopButton.Location = new System.Drawing.Point(375, 42);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(44, 26);
            this.StopButton.TabIndex = 4;
            this.StopButton.Text = "S";
            this.StopButton.UseVisualStyleBackColor = true;
            // 
            // LoopCheckBox
            // 
            this.LoopCheckBox.AutoSize = true;
            this.LoopCheckBox.Location = new System.Drawing.Point(330, 74);
            this.LoopCheckBox.Name = "LoopCheckBox";
            this.LoopCheckBox.Size = new System.Drawing.Size(50, 17);
            this.LoopCheckBox.TabIndex = 5;
            this.LoopCheckBox.Text = "Loop";
            this.LoopCheckBox.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown1.Location = new System.Drawing.Point(330, 97);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(89, 24);
            this.numericUpDown1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(260, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Anim Speed";
            // 
            // AnimationToolbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 252);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.LoopCheckBox);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.PlayButton);
            this.Controls.Add(this.Time);
            this.Controls.Add(this.TimeScroll);
            this.Controls.Add(this.AnimList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AnimationToolbox";
            this.Text = "Animation Toolbox";
            this.Load += new System.EventHandler(this.AnimationToolbox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Time)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox AnimList;
        private System.Windows.Forms.HScrollBar TimeScroll;
        private System.Windows.Forms.NumericUpDown Time;
        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.CheckBox LoopCheckBox;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label1;
    }
}
#endif