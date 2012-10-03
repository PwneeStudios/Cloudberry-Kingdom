#if WINDOWS
using System;
using System.Windows.Forms;

#if EDITOR
using Microsoft.Xna.Framework;
#endif

namespace CoreEngine
{
    public partial class Toolbox : Form
    {
#if EDITOR
        public Toolbox()
        {
            InitializeComponent();
            
            ItemTree.DragEnter += new DragEventHandler(ItemTree_DragEnter);

            ResizeButton.Click += new EventHandler(ResizeButton_Click);
            ImportButton.Click += new EventHandler(ImportButton_Click);   
        }

        void ItemTree_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        void ImportButton_Click(object sender, EventArgs e)
        {
            (new ImportQuads()).Show(); // Dialog();
        }

        void ResizeButton_Click(object sender, EventArgs e)
        {
            foreach (BaseQuad _quad in Game_Editor.SelectedQuads)
            {
                Quad quad = _quad as Quad;
                if (null != quad)
                {
                    float width = (quad.Corner[1].RelPos - quad.Corner[0].RelPos).Length();
                    Vector2 NewSize = new Vector2(quad.MyTexture.Tex.Width, quad.MyTexture.Tex.Height);
                    NewSize *= .5f * width / quad.MyTexture.Tex.Width;
                    quad.ScaleCorners(NewSize);
                }
            }
        }
#endif
        private void Toolbox_Load(object sender, EventArgs e)
        {
#if EDITOR
            this.Location = Game_Editor.MainWindow.Location;
            this.Left -= this.Width;
#endif
        }

        void ItemTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
        }

        private void WidthBar_Scroll(object sender, EventArgs e)
        {
#if EDITOR
            TrackBar trackbar = (System.Windows.Forms.TrackBar)sender;
            Game_Editor.MainObject.OutlineWidth = new Vector2(trackbar.Value) / 100f;
            this.WidthText.Text = string.Format("{0:0.00}", Game_Editor.MainObject.OutlineWidth.X);
#endif
        }
    }
}
#endif