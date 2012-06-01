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

            NewFloaterButton.Image = Image.FromFile("C:\\Users\\Ez\\Desktop\\EditorToolPics\\rectangle_blue.png");
            SetButtonParams(NewFloaterButton);

            NewLayerButton.Image = Image.FromFile("C:\\Users\\Ez\\Desktop\\EditorToolPics\\layers.png");
            SetButtonParams(NewLayerButton);

            TextureButton.Image = Image.FromFile("C:\\Users\\Ez\\Desktop\\EditorToolPics\\Quad_24.png");
            SetButtonParams(TextureButton);

            ////LayerTree.DragEnter += new DragEventHandler(LayerTree_DragEnter);
            LayerTree.ItemDrag += new ItemDragEventHandler(LayerTree_ItemDrag);
            LayerTree.DragDrop += new DragEventHandler(LayerTree_DragDrop);
            LayerTree.DragOver += new DragEventHandler(LayerTree_DragOver);
            LayerTree.AllowDrop = true;

            //LayerTree.DragDrop += new DragEventHandler(LayerTree_DragDrop);
            LayerTree.MouseDown += new MouseEventHandler(LayerTree_MouseDown);

            FillTree();
        }

        TreeNode GetReceivingNode(TreeView Tree, Vector2 pos)
        {
            System.Drawing.Point pt1 = Tree.PointToClient(new System.Drawing.Point((int)pos.X, (int)pos.Y));

            TreeNode node1 = Tree.GetNodeAt(pt1);

            return node1;
        }

        ////void LayerTree_DragDrop(object sender, DragEventArgs e)
        ////{
            
        ////}

        void LayerTree_MouseDown(object sender, MouseEventArgs e)
        {
            var node = ((TreeView)sender).GetNodeAt(new System.Drawing.Point((int)e.X, (int)e.Y));

            LayerTree.SelectedNode = node;
            //if (node != null)
            //{
            //    LayerTree.DoDragDrop(node, DragDropEffects.Move);
            //}
        }



        void LayerTree_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
            //var node = GetReceivingNode((TreeView)sender, new Vector2(e.X, e.Y));
        }

        TreeNode_ DraggedNode;
        void LayerTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DraggedNode = e.Item as TreeNode_;
            LayerTree.DoDragDrop(DraggedNode, DragDropEffects.Move);
        }

        void LayerTree_DragDrop(object sender, DragEventArgs e)
        {
            var ReceivingNode = GetReceivingNode((TreeView)sender, new Vector2(e.X, e.Y));

            if (DraggedNode == null || ReceivingNode == null) return;
            if (DraggedNode == ReceivingNode) return;
            var fnode = DraggedNode as TreeNode_Floater;
            var lnode = DraggedNode as TreeNode_List;

            TreeNode_List ReceivingLayerNode = null;

            if (ReceivingNode is TreeNode_Floater)
                ReceivingLayerNode = ReceivingNode.Parent as TreeNode_List;
            else if (ReceivingNode is TreeNode_List)
                ReceivingLayerNode = ReceivingNode as TreeNode_List;
            else
                ReceivingLayerNode = null;

            if (null != lnode)
            {
                if (lnode != ReceivingLayerNode)
                {
                    //lnode.Free();
                    LayerTree.Nodes.Remove(lnode);
                    LayerTree.Nodes.Insert(LayerTree.Nodes.IndexOf(ReceivingLayerNode) + 1, lnode);

                    LayerTree.SelectedNode = lnode;
                }
            }
            else
            {
                if (ReceivingLayerNode != null)
                {
                    ReceivingLayerNode.Insert(ReceivingLayerNode.Nodes.IndexOf(ReceivingNode) + 1, fnode);
                    LayerTree.SelectedNode = fnode;
                }
            }
        }



        private void SetButtonParams(Button button)
        {
            button.Text = "";
            button.Size = new System.Drawing.Size(28, 28);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(255, 240, 240, 240);
            button.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(255, 255, 255, 255);
            button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(255, 150, 150, 150);
            button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(255, 200, 200, 200);
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

            public virtual void Insert(int Index, TreeNode_ node)
            {
                node.Free();
            }

            public virtual void Free()
            {
                if (Parent == null) return;

                Parent.Nodes.Remove(this);
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

                //Controller.background.MyCollection.Sort();
                //Controller.LayerTree.Sort();
            }

            public override void Insert(int Index, TreeNode_ node)
            {
                base.Insert(Index, node);

                var fnode = node as TreeNode_Floater;
                if (null == fnode)
                    throw new Exception("Layer can only have floater children.");

                MyList.Floaters.Add(fnode.MyFloater);
                Nodes.Add(fnode);
            }
        }

        class TreeNode_Floater : TreeNode_
        {
            public BackgroundFloater MyFloater;

            public TreeNode_Floater(BackgroundFloater floater, BackgroundViewer Controller)
                : base(Controller)
            {
                MyFloater = floater;

                Text = string.Format("{0}", floater.MyQuad.Quad.MyTexture.Name);
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

            public override void Free()
            {
                if (Parent == null) return;

                var list_parent = Parent as TreeNode_List;
                if (null != list_parent)
                    list_parent.MyList.Floaters.Remove(this.MyFloater);

                base.Free();
            }

            public override void Insert(int Index, TreeNode_ node)
            {
                base.Insert(Index, node);
                throw new Exception("Floater node can't have children.");
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
                    list_node.Nodes.Add(new TreeNode_Floater(floater, this));
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

            // Delete
            if (Tools.keybState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Delete))
                DeleteSelected();

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
            // Unselect floaters
            foreach (var floater in SelectedFloaters)
                floater.Selected = false;
            SelectedFloaters.Clear();
            
            // If selected item is a floater, select nothing.
            if (LayerTree.SelectedNode is TreeNode_Floater)
                LayerTree.SelectedNode = null;

            // Update fake highlighting
            FakeHighlightTree();
        }

        void DeleteSelected()
        {
            // Delete a layer
            var layer_node = LayerTree.SelectedNode as TreeNode_List;
            if (null != layer_node)
            {
                LayerTree.Nodes.Remove(layer_node);
                background.MyCollection.Lists.Remove(layer_node.MyList);
            }
            else
            {
                foreach (var floater in SelectedFloaters)
                {
                    var floater_node = GetNodeOf(floater) as TreeNode_Floater;
                    if (null != floater_node)
                    {
                        floater_node.Parent.Nodes.Remove(floater_node);
                        floater_node.MyFloater.Parent.Floaters.Remove(floater_node.MyFloater);
                    }
                }
            }
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
            // Fake highlight the tree
            FakeHighlightTree();
        }

        /// <summary>
        /// Provides the illusion of a multi-select tree.
        /// </summary>
        private void FakeHighlightTree()
        {
            foreach (TreeNode lnode in LayerTree.Nodes)
                foreach (TreeNode fnode in lnode.Nodes)
                {
                    TreeNode_Floater floater_node = fnode as TreeNode_Floater;
                    if (null != floater_node)
                    {
                        if (floater_node.MyFloater.Selected)
                        {
                            floater_node.BackColor = System.Drawing.Color.FromArgb(255, 51, 153, 255);
                            floater_node.ForeColor = System.Drawing.Color.White;
                        }
                        else
                        {
                            floater_node.BackColor = System.Drawing.Color.White;
                            floater_node.ForeColor = System.Drawing.Color.Black;
                        }
                    }
                }
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

        BackgroundFloaterList CurrentLayer
        {
            get
            {
                if (CurrentLayerNode == null)
                    return null;
                else
                    return CurrentLayerNode.MyList;
            }
        }

        TreeNode_List CurrentLayerNode
        {
            get
            {
                var lnode = LayerTree.SelectedNode as TreeNode_List;
                if (null != lnode)
                    return lnode;

                var fnode = LayerTree.SelectedNode as TreeNode_Floater;
                if (null != fnode)
                    return fnode.Parent as TreeNode_List;

                return null;
            }
        }

        private void NewFloaterButton_Click(object sender, EventArgs e)
        {
            if (CurrentLayer != null)
            {
                var floater = new BackgroundFloater(Tools.CurLevel, -100000, 100000);
                floater.Data.Position = Tools.CurCamera.Pos;
                floater.MyQuad.Size = new Vector2(400, 400);
                floater.FixedAspectPreference = true;

                // Add the floater
                CurrentLayer.Floaters.Add(floater);
                var node = new TreeNode_Floater(floater, this);
                CurrentLayerNode.Nodes.Add(node);

                // Select the floater
                LayerTree.SelectedNode = node;
            }
        }

        private void NewLayerButton_Click(object sender, EventArgs e)
        {
            var list = new BackgroundFloaterList();
            list.MyLevel = background.MyLevel;
            list.Parallax = .9f;

            // Add list
            background.MyCollection.Lists.Add(list);
            var node = new TreeNode_List(list, this);
            LayerTree.Nodes.Add(node);

            // Select the list
            LayerTree.SelectedNode = node;
        }

        //static int NodeSorter(TreeNode n1, TreeNode n2)
        //{
        //    var l1 = n1 as TreeNode_List;
        //    var l2 = n2 as TreeNode_List;
        //    if (null != l1 && null != l2) return l1.MyList.Parallax.CompareTo(l2.MyList.Parallax);

        //    return 0;
        //}
    }
}
#endif