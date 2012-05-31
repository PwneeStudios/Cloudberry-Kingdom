#if WINDOWS
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Forms = System.Windows.Forms;

namespace CloudberryKingdom.Viewer
{
    public partial class BackgroundViewer : Form
    {
        public BackgroundViewer()
        {
            InitializeComponent();

            TextureButton.Image = Image.FromFile("C:\\Users\\Ez\\Desktop\\ToolbarIcons-full-20100505\\png\\24\\Modern3D\\objects\\painting.png");

            FillTree();
        }

        bool ContainsMouse
        {
            get
            {
                var pos = PointToClient(Cursor.Position);
                return
                    pos.X < Width && pos.X > 0 &&
                    pos.Y < Height && pos.Y > -17;
                    //ClientRectangle.Contains();
            }
        }

        bool MouseActiveInWorld_OnLastMouseClick;
        bool MouseActiveInWorld
        {
            get
            {
                if (Tools.PrevMouseDown() && !MouseActiveInWorld_OnLastMouseClick) return false;

                return !ContainsMouse && Tools.MouseInWindow && Tools.TheGame.IsActive;
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        class TreeNode_ : TreeNode
        {
            public BackgroundViewer Controller;

            public TreeNode_(BackgroundViewer Controller)
                : base()
            {
                this.Controller = Controller;
            }

            public virtual void SyncNumerics()
            {
            }
        }

        class TreeNode_List : TreeNode_
        {
            public BackgroundFloaterList MyList;
            public TreeNode_List(BackgroundFloaterList list, BackgroundViewer Controller)
                : base(Controller)
            {
                MyList = list;
            }

            public override void SyncNumerics()
            {
                base.SyncNumerics();

                // Sync Parallax
                Controller.ParallaxNum.Value = (decimal)MyList.Parallax;
            }
        }

        class TreeNode_Floater : TreeNode_
        {
            public BackgroundFloater MyFloater;

            public TreeNode_Floater(BackgroundFloater floater, BackgroundViewer Controller)
                : base(Controller)
            {
                MyFloater = floater;
            }

            public override void SyncNumerics()
            {
                base.SyncNumerics();

                // Sync Parallax
                Controller.ParallaxNum.Value = (decimal)MyFloater.Parent.Parallax;

                // Sync x,y values
                Controller.pos_xNum.Value = (decimal)MyFloater.Data.Position.X;
                Controller.pos_yNum.Value = (decimal)MyFloater.Data.Position.Y;
                Controller.size_xNum.Value = (decimal)MyFloater.MyQuad.Size.X;
                Controller.size_yNum.Value = (decimal)MyFloater.MyQuad.Size.Y;
                Controller.speed_xNum.Value = (decimal)MyFloater.Data.Velocity.X;
                Controller.speed_yNum.Value = (decimal)MyFloater.Data.Velocity.Y;

                // Sync aspect bool
                Controller.AspectCheckbox.Checked = MyFloater.FixedAspectPreference;
                Controller.FixedPosCheckbox.Checked = MyFloater.FixedPos;
            }
        }

        Background background { get { return Tools.CurLevel.MyBackground; } }

        /// <summary>
        /// Fill the tree with lists and floaters.
        /// </summary>
        void FillTree()
        {
            // Loop through each list.
            // Keep track of which layer we are on.
            int count = 0;
            foreach (var list in background.MyCollection.Lists)
            {
                count++;

                var list_node = new TreeNode_List(list, this);
                list_node.Text = string.Format("Layer {0}", count);
                LayerTree.Nodes.Add(list_node);

                // Loop through the floaters for this list.
                foreach (var floater in list.Floaters)
                {
                    var floater_node = new TreeNode_Floater(floater, this);
                    floater_node.Text = string.Format("{0}", floater.MyQuad.Quad.MyTexture.Name);
                    list_node.Nodes.Add(floater_node);
                }
            }
        }

        public void Input()
        {
            // Lock location
            //var loc = Form.FromHandle(Tools.TheGame.Window.Handle).Location;
            //loc.X -= this.Size.Width;
            ////loc.Y -= this.Size.Height;
            //this.Location = loc;


            if (Tools.MousePressed())
                MouseActiveInWorld_OnLastMouseClick = MouseActiveInWorld;

            var pos = Tools.MouseWorldPos();
            var delta = Tools.DeltaMouse;

            // Escape to deselect
            if (Tools.keybState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                DeselectAll();

            // No soft if inactive
            if (!MouseActiveInWorld)
            {
                foreach (var list in background.MyCollection.Lists)
                    foreach (var floater in list.Floaters)
                        floater.SoftSelected = false;
            }

            // If the mouse is down do movement/scaling of selected floaters,
            if ((Tools.MouseDown() || Tools.ShiftDown()) && !Tools.MousePressed())
            {
                foreach (var floater in SelectedFloaters)
                {
                    var parallax = floater.Parent.Parallax;

                    // If we are holding Shift then scale the quad.
                    if (Tools.ShiftDown())
                    {
                        floater.MyQuad.Size += delta / parallax;
                        if (AspectCheckbox.Checked)
                            floater.MyQuad.ScaleYToMatchRatio();

                        floater.InitialUpdate();
                    }
                    // Otherwise move it.
                    else if (MouseActiveInWorld)
                    {
                        floater.Data.Position += delta / parallax;
                        floater.InitialUpdate();
                    }
                }
            }
            // otherwise check for clicks and hovers.
            else if (MouseActiveInWorld)
            {
                // Update soft highlight list
                // First un-soft-highlight everything
                foreach (var list in background.MyCollection.Lists)
                    foreach (var floater in list.Floaters)
                        floater.SoftSelected = false;

                // Hit test. Find floater mouse is touching and would select if clicked.
                BackgroundFloater hit_floater = null;
                foreach (var list in background.MyCollection.Lists)
                    foreach (var floater in list.Floaters)
                        if (floater.MyQuad.HitTest_WithParallax(pos, Vector2.Zero, list.Parallax))
                        {
                            // Found a floater underneath the mouse.
                            // If multiple hit floaters show up, prefer hit floaters that are already selected.
                            
                            //if (hit_floater == null ||
                            //    floater.Selected || !hit_floater.Selected)
                            {
                                hit_floater = floater;
                            }
                        }

                // If we hit something soft select it.
                if (hit_floater != null)
                {
                    hit_floater.SoftSelected = true;

                    // If we clicked it,
                    if (Tools.MousePressed())
                    {
                        // Then select it.
                        if (!SelectedFloaters.Contains(hit_floater))
                        {
                            var node = GetNodeOf(hit_floater);
                            LayerTree.SelectedNode = node;
                        }
                    }
                }
                // Otherwise if Cntrl isn't pressed and we click, select nothing.
                else
                {
                    if (!Tools.CntrlDown() && Tools.MousePressed())
                        DeselectAll();
                }
            }
        }

        void DeselectAll()
        {
            foreach (var floater in SelectedFloaters)
                floater.Selected = false;
            SelectedFloaters.Clear();
            
            if (LayerTree.SelectedNode is TreeNode_Floater)
                LayerTree.SelectedNode = null;
        }

        /// <summary>
        /// Find the floater node that points to a given floater.
        /// </summary>
        TreeNode_ GetNodeOf(BackgroundFloater floater)
        {
            foreach (TreeNode node in LayerTree.Nodes)
                foreach (TreeNode _node in node.Nodes)
                {
                    TreeNode_Floater floater_node = _node as TreeNode_Floater;
                    if (null != floater_node && floater_node.MyFloater == floater)
                        return floater_node;
                }

            return null;
        }

        public List<BackgroundFloater> SelectedFloaters = new List<BackgroundFloater>();
        private void LayerTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode_ node = e.Node as TreeNode_;
            if (null != node)
                LayerTree_AfterSelect_Body(node);
        }
        void LayerTree_AfterSelect_Body(TreeNode_ node)
        {
            if (!Tools.CntrlDown())
                SelectedFloaters.Clear();

            // Enable/disable based on what is selected
            bool IsLayer = node is TreeNode_List;
            bool IsFloater = node is TreeNode_Floater;

            //ParallaxNum.Enabled = IsLayer;
            LayerBox.Enabled = IsLayer;
            QuadBox.Enabled = IsFloater;
            //QuadBox.Controls


            if (node != null)
            {
                node.SyncNumerics();

                // Highlight
                TreeNode_Floater floater_node = node as TreeNode_Floater;
                if (null != floater_node && !SelectedFloaters.Contains(floater_node.MyFloater))
                    SelectedFloaters.Add(floater_node.MyFloater);
            }

            // Update highlight list
            // First unhighlight everything
            foreach (var list in background.MyCollection.Lists)
                foreach (var floater in list.Floaters)
                    floater.Selected = false;
            // Then highlight what is selected
            foreach (var floater in SelectedFloaters)
                floater.Selected = true;
        }

        /// <summary>
        /// Click on the Aspect Checkbox event
        /// </summary>
        private void AspectCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var floater in SelectedFloaters)
                floater.FixedAspectPreference = AspectCheckbox.Checked;
        }

        /// <summary>
        /// Click on the Fixed Pos Checkbox event
        /// </summary>
        private void FixedPosCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var floater in SelectedFloaters)
                floater.FixedPos = FixedPosCheckbox.Checked;
        }

        /// <summary>
        /// Set parallax event. Update the layer and all floaters.
        /// </summary>
        private void ParallaxNum_ValueChanged(object sender, EventArgs e)
        {
            var layer_node = LayerTree.SelectedNode as TreeNode_List;
            if (null == layer_node) return;

            layer_node.MyList.SetParallaxAndPropagate((float)ParallaxNum.Value);
        }

        private void TextureButton_Click(object sender, EventArgs e)
        {
            var Dlg = new Forms.OpenFileDialog();
            Tools.DialogUp = true;
            Dlg.Title = "Choose texture...";

            Dlg.InitialDirectory = Tools.DefaultDynamicDirectory();
            Dlg.Filter = "PNG Texture (*.png)|*.png|JPG Texture (*.jpg)|*.jpg";
            Dlg.CheckFileExists = true;
            Dlg.Multiselect = false;

            // Check the user didn't cancel
            if (Dlg.ShowDialog() != Forms.DialogResult.Cancel)
            {
                // Set the texture
                foreach (var floater in SelectedFloaters)
                    floater.MyQuad.TextureName = Tools.GetFileName(Dlg.FileName);
            }

            Tools.DialogUp = false;
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }
    }
}
#endif