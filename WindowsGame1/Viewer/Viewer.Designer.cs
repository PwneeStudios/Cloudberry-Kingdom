#if WINDOWS
namespace CloudberryKingdom.Viewer
{
    public partial class Viewer
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
            this.ViewerTree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // ViewerTree
            // 
            this.ViewerTree.Location = new System.Drawing.Point(12, 12);
            this.ViewerTree.Name = "ViewerTree";
            this.ViewerTree.Size = new System.Drawing.Size(299, 450);
            this.ViewerTree.TabIndex = 0;
            this.ViewerTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ViewerTree_AfterSelect);
            // 
            // Viewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 474);
            this.Controls.Add(this.ViewerTree);
            this.Name = "Viewer";
            this.Text = "Viewer";
            this.Load += new System.EventHandler(this.Viewer_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView ViewerTree;
    }
}
#endif