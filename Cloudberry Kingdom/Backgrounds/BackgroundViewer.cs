#if WINDOWS && INCLUDE_EDITOR
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using XnaInput = Microsoft.Xna.Framework.Input;
using Forms = System.Windows.Forms;
using Xna = Microsoft.Xna.Framework;
using Drawing;

namespace CloudberryKingdom.Viewer
{
    public partial class BackgroundViewer : Form
    {
        public BackgroundViewer()
        {
            InitializeComponent();

            var path = "EditorToolPics";
            
            NewFloaterButton.Image = Image.FromFile(path + "\\rectangle_blue.png");
            SetButtonParams(NewFloaterButton);

            NewLayerButton.Image = Image.FromFile(path + "\\layers.png");
            SetButtonParams(NewLayerButton);

            TextureButton.Image = Image.FromFile(path + "\\Quad_24.png");
            SetButtonParams(TextureButton);

            LayerLockCheckbox.Image = Image.FromFile(path + "\\lock.png");
            LayerShowCheckbox.Image = Image.FromFile(path + "\\eye.png");

            PlayCheckbox.Image = Image.FromFile(path + "\\Play.png");
            SetButtonParams(PlayCheckbox);
            PlayCheckbox.Checked = false;

            CameraMoveCheckbox.Image = Image.FromFile(path + "\\MoveCam.png");
            SetButtonParams(CameraMoveCheckbox);
            CameraMoveCheckbox.Checked = true; CameraMoveCheckbox.Checked = false;

            MoveBoundsCheckbox.Image = Image.FromFile(path + "\\MoveBounds.png");
            SetButtonParams(MoveBoundsCheckbox);
            MoveBoundsCheckbox.Checked = true; MoveBoundsCheckbox.Checked = false;

            MoveQuadsCheckbox.Image = Image.FromFile(path + "\\MoveQuad.png");
            SetButtonParams(MoveQuadsCheckbox);
            MoveQuadsCheckbox.Checked = true;

            
            ColorButton.Text = "";

            SetVectorBox(pos_VectorBox);
            SetVectorBox(size_VectorBox);
            SetVectorBox(vel_VectorBox);

            SetVectorBox(offset_VectorBox);
            SetVectorBox(repeat_VectorBox);
            SetVectorBox(speed_VectorBox);

            LayerTree.KeyDown += new KeyEventHandler(LayerTree_KeyDown);
            LayerTree.AfterLabelEdit += new NodeLabelEditEventHandler(LayerTree_AfterLabelEdit);
            LayerTree.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(LayerTree_NodeMouseDoubleClick);
            LayerTree.BeforeLabelEdit += new NodeLabelEditEventHandler(LayerTree_BeforeLabelEdit);
            LayerTree.ItemDrag += new ItemDragEventHandler(LayerTree_ItemDrag);
            LayerTree.DragDrop += new DragEventHandler(LayerTree_DragDrop);
            LayerTree.DragOver += new DragEventHandler(LayerTree_DragOver);
            LayerTree.MouseDown += new MouseEventHandler(LayerTree_MouseDown);
            LayerTree.AllowDrop = true;

            PreviewNode = new TreeNode("              ");
            PreviewNode.BackColor = System.Drawing.Color.LimeGreen;

            FillTree();
        }
        void LayerTree_KeyDown(object sender, KeyEventArgs e)
        {
            var node = LayerTree.SelectedNode as TreeNode_;

            // Press F2 to rename a node.
            if (e.KeyData == Keys.F2 && node != null)
            {
                BeginLabelEdit(node);
            }
        }

        void LayerTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            BeginLabelEdit(e.Node);
        }

        void BeginLabelEdit(TreeNode node)
        {
            LayerTree.LabelEdit = true;
            node.BeginEdit();
        }

        bool EditingNodeText = false;
        void LayerTree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            EditingNodeText = true;
        }

        void LayerTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            EditingNodeText = false;
            LayerTree.LabelEdit = false;

            var node = e.Node as TreeNode_;
            if (null == node) return;

            if (e.Label == null)
                node.SetName(node.GetName());
            else
                node.SetName(e.Label);
        }

        void SetVectorBox(TextBox box)
        {
            box.Validating += ReverseSync;
            box.KeyPress += VectorBox_KeyPress;

            box.BackColor = System.Drawing.SystemColors.Control;
            box.GotFocus += new EventHandler(box_GotFocus);
            box.LostFocus += new EventHandler(box_LostFocus);
        }
        void box_LostFocus(object sender, EventArgs e)
        {
            var box = sender as TextBox;
            if (null == box) return;

            box.BackColor = System.Drawing.SystemColors.Control;
        }
        void box_GotFocus(object sender, EventArgs e)
        {
            var box = sender as TextBox;
            if (null == box) return;

            box.BackColor = System.Drawing.SystemColors.Window;
        }
        void VectorBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            var box = sender as TextBox;

            if (null != box && e.KeyChar == 13)
            {
                ReverseSync(sender, e);
                ProcessTabKey(true);
                //box.Parent.Focus();
            }
        }

        TreeNode GetReceivingNode(TreeView Tree, Vector2 pos)
        {
            var pt1 = Tree.PointToClient(new System.Drawing.Point((int)pos.X, (int)pos.Y));

            TreeNode node1 = Tree.GetNodeAt(pt1);

            return node1;
        }

        void LayerTree_MouseDown(object sender, MouseEventArgs e)
        {
            var node = ((TreeView)sender).GetNodeAt(new System.Drawing.Point((int)e.X, (int)e.Y));

            LayerTree.SelectedNode = node;
        }

        void LayerTree_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;

            var ReceivingNode = GetReceivingNode((TreeView)sender, new Vector2(e.X, e.Y));

            if (ReceivingNode == PreviewNode) { return; }
            if (DraggedNode == null || ReceivingNode == null) { KillPreview(); return; }
            if (DraggedNode == ReceivingNode) { KillPreview(); return; }
            var fnode = DraggedNode as TreeNode_Floater;
            var lnode = DraggedNode as TreeNode_List;

            TreeNode_List ReceivingLayerNode = GetReceivingLayerNode(ReceivingNode);

            // Insert the preview node in the right spot, depending on whether it's a floater or a list.
            if (null != lnode)
            {
                // Drop a layer node
                if (lnode != ReceivingLayerNode)
                {
                    KillPreview();
                    
                    int InsertIndex = LayerTree.Nodes.IndexOf(ReceivingLayerNode) + 1;

                    // If we are at the very top of the tree, preview as the first node.
                    var pt = LayerTree.PointToClient(new System.Drawing.Point((int)e.X, (int)e.Y));
                    if (pt.Y < 9)
                        InsertIndex = 0;

                    InsertIndex = Tools.Restrict(0, LayerTree.Nodes.Count, InsertIndex);

                    // Insert the preivew node.
                    LayerTree.Nodes.Insert(InsertIndex, PreviewNode);
                    PreviewIsValid = true;
                }
                else
                    KillPreview();
            }
            else
            {
                // Drop a floater node
                if (ReceivingLayerNode != null)
                {
                    KillPreview();
                    
                    int InsertIndex = ReceivingLayerNode.Nodes.IndexOf(ReceivingNode) + 1;
                    InsertIndex = Tools.Restrict(0, ReceivingLayerNode.Nodes.Count, InsertIndex);

                    //if (ReceivingLayerNode != LayerTree.SelectedNode.Parent ||
                    //    Math.Abs(InsertIndex - LayerTree.SelectedNode.Index) > 0)
                    {
                        ReceivingLayerNode.Nodes.Insert(InsertIndex, PreviewNode);
                        PreviewIsValid = true;
                    }
                }
                else
                    KillPreview();
            }
        }

        private void KillPreview()
        {
            if (PreviewNode.Parent != null) PreviewNode.Parent.Nodes.Remove(PreviewNode);
            else if (LayerTree.Nodes.Contains(PreviewNode)) LayerTree.Nodes.Remove(PreviewNode);

            PreviewIsValid = false;
        }

        bool PreviewIsValid = false;
        TreeNode PreviewNode;
        TreeNode_ DraggedNode;
        void LayerTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DraggedNode = e.Item as TreeNode_;
            LayerTree.DoDragDrop(DraggedNode, DragDropEffects.Move);
        }

        void LayerTree_DragDrop(object sender, DragEventArgs e)
        {
            if (DraggedNode == null || !PreviewIsValid) { KillPreview(); return; }

            var fnode = DraggedNode as TreeNode_Floater;
            var lnode = DraggedNode as TreeNode_List;

            // Insert the node in the right spot, depending on whether it's a floater or a list.
            if (null != lnode)
            {
                // Drop a layer node
                lnode.Free();

                background.MyCollection.Lists.Insert(PreviewNode.Index, lnode.MyList);
                LayerTree.Nodes.Insert(PreviewNode.Index, lnode);

                LayerTree.SelectedNode = lnode;
            }
            else
            {
                // Drop a floater node
                var NewLayerNode = PreviewNode.Parent as TreeNode_List;
                if (null != NewLayerNode)
                {
                    // Update the scaling of the floater to deal with the new parallax
                    fnode.MyFloater.ChangeParallax(((TreeNode_List)fnode.Parent).MyList.Parallax, NewLayerNode.MyList.Parallax);

                    // Move the floater and its node to the proper lists.
                    fnode.Free();
                    NewLayerNode.Insert(PreviewNode.Index, fnode);
                    
                    LayerTree.SelectedNode = fnode;
                }
            }

            KillPreview();
        }

        private static TreeNode_List GetReceivingLayerNode(TreeNode ReceivingNode)
        {
            TreeNode_List ReceivingLayerNode = null;

            if (ReceivingNode is TreeNode_Floater)
                ReceivingLayerNode = ReceivingNode.Parent as TreeNode_List;
            else if (ReceivingNode is TreeNode_List)
                ReceivingLayerNode = ReceivingNode as TreeNode_List;
            else
                ReceivingLayerNode = null;
            return ReceivingLayerNode;
        }


        private void SetButtonParams(ButtonBase button)
        {
            button.Text = "";
            button.Size = new System.Drawing.Size(28, 28);
            //button.FlatStyle = FlatStyle.Flat;
            //button.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(255, 240, 240, 240);
            //button.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(255, 255, 255, 255);
            //button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(255, 150, 150, 150);
            //button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(255, 200, 200, 200);
            
            button.FlatStyle = FlatStyle.Standard;
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

        public class TreeNode_ : TreeNode
        {
            public BackgroundViewer Controller;

            public TreeNode_(BackgroundViewer Controller)
                : base()
            {
                this.Controller = Controller;
            }

            public virtual string GetName()
            {
                return null;
            }
            public virtual void SetName(string name)
            {
                this.Text = name;
            }

            public virtual void SyncNumerics()
            {
            }

            public virtual void Insert(int Index, TreeNode_ node)
            {
                //node.Free();
            }

            public virtual void Free()
            {
                if (Parent == null) return;

                Parent.Nodes.Remove(this);
            }

            public bool UserSetName = false;
        }

        public class TreeNode_List : TreeNode_
        {
            public BackgroundFloaterList MyList;
            public TreeNode_List(BackgroundFloaterList list, BackgroundViewer Controller)
                : base(Controller)
            {
                MyList = list;

                Controller.LayerNameCount++;

                // Name the list if it is unnamed.
                Controller.NameList(list);

                SetName(MyList.Name);
                UserSetName = true;
            }

            public override string GetName()
            {
                return MyList.Name;
            }
            public override void SetName(string name)
            {
                base.SetName(name);

                MyList.Name = name;
            }

            public TreeNode_Floater Add(BackgroundFloater floater)
            {
                MyList.Floaters.Add(floater);
                
                var node = new TreeNode_Floater(floater, Controller);
                Nodes.Add(node);

                return node;
            }

            public override void SyncNumerics()
            {
                base.SyncNumerics();

                // Sync bools
                Controller.LayerShowCheckbox.Checked = MyList.Show;
                Controller.LayerLockCheckbox.Checked = MyList.Lock;

                // Sync Parallax
                Controller.ParallaxNum.Value = (decimal)MyList.Parallax;
            }

            public override void Insert(int Index, TreeNode_ node)
            {
                base.Insert(Index, node);

                var fnode = node as TreeNode_Floater;
                if (null == fnode)
                    throw new Exception("Layer can only have floater children.");

                //MyList.Floaters.Add(fnode.MyFloater);
                //Nodes.Add(fnode);
                MyList.Floaters.Insert(Index, fnode.MyFloater);
                Nodes.Insert(Index, fnode);
            }

            public override void Free()
            {
                //if (Parent == null) return;

                Controller.LayerTree.Nodes.Remove(this);
                Controller.background.MyCollection.Lists.Remove(MyList);

                base.Free();
            }
        }

        public class TreeNode_Floater : TreeNode_
        {
            public BackgroundFloater MyFloater;

            public TreeNode_Floater(BackgroundFloater floater, BackgroundViewer Controller)
                : base(Controller)
            {
                MyFloater = floater;
                MyFloater.Node = this;

                if (floater.Name == null)
                    floater.Name = string.Format("{0}", floater.MyQuad.Quad.MyTexture.Name);

                SetName(MyFloater.Name);
            }

            public override string GetName()
            {
                return MyFloater.Name;
            }
            public override void SetName(string name)
            {
                base.SetName(name);

                MyFloater.Name = name;

                if (MyFloater.Name == MyFloater.MyQuad.Quad.MyTexture.Name)
                    UserSetName = false;
                else
                    UserSetName = true;
            }

            public override void SyncNumerics()
            {
                base.SyncNumerics();

                Tools.Assert(MyFloater.Parent != null);

                // Sync color
                var c = MyFloater.MyQuad.Quad.MySetColor;
                var clr = System.Drawing.Color.FromArgb(255, c.R, c.G, c.B);
                //Controller.ColorButton.ForeColor = clr;
                Controller.ColorButton.BackColor = clr;

                // Sync x,y values
                Controller.pos_VectorBox.Text = MyFloater.StartData.Position.ToSimpleString();
                Controller.size_VectorBox.Text = MyFloater.MyQuad.Size.ToSimpleString();
                Controller.vel_VectorBox.Text = MyFloater.StartData.Velocity.ToSimpleString();

                // Sync u,v values
                Controller.offset_VectorBox.Text = MyFloater.MyQuad.Quad.UV_Offset.ToSimpleString();
                Controller.repeat_VectorBox.Text = MyFloater.MyQuad.Quad.UV_Repeat.ToSimpleString();
                Controller.speed_VectorBox.Text = MyFloater.uv_speed.ToSimpleString();

                // Sync aspect bool
                Controller.AspectCheckbox.Checked = MyFloater.FixedAspectPreference;
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

        int LayerNameCount = 0;
        /// <summary>
        /// Fill the tree with lists and floaters.
        /// </summary>
        void FillTree()
        {
            // Loop through each list.
            foreach (var list in background.MyCollection.Lists)
            {
                // Add the list node
                var list_node = new TreeNode_List(list, this);
                LayerTree.Nodes.Add(list_node);

                // Loop through the floaters for this list.
                foreach (var floater in list.Floaters)
                {
                    list_node.Nodes.Add(new TreeNode_Floater(floater, this));
                    AddTextureToCombobox(floater.MyQuad.Quad.MyTexture);
                }
            }
        }

        private void NameList(BackgroundFloaterList list)
        {
            if (list.Name == null)
                list.Name = string.Format("Layer {0}", LayerNameCount);
        }

        BackgroundFloater FloaterCopy;
        void CopyFloater()
        {
            var node = LayerTree.SelectedNode as TreeNode_Floater;
            if (null != node)
                CopyFloater(node.MyFloater);
        }
        void CopyFloater(BackgroundFloater source)
        {
            FloaterCopy = new BackgroundFloater(source);
        }
        void PasteFloater()
        {
            if (CurrentLayerNode == null) return;
            if (FloaterCopy == null) return;

            var floater = new BackgroundFloater(FloaterCopy);
            floater.StartData.Position = Tools.CurCamera.Pos;
            floater.InitialUpdate();
            
            var node = CurrentLayerNode.Add(floater);

            DeselectAll();
            LayerTree.SelectedNode = node;
        }

        public void PreDraw()
        {
            if (axisLinesToolStripMenuItem.Checked)
            {
                var extent = 10000;
                var width = 12.5f;
                Tools.QDrawer.DrawLine(new Vector2(0, extent), new Vector2(0, -extent), new Xna.Color(1f, 1f, 1f, .5f), width);
                Tools.QDrawer.DrawLine(new Vector2(extent, 0), new Vector2(-extent, 0), new Xna.Color(1f, 1f, 1f, .5f), width);
            }
        }

        public void Draw()
        {
            var cam = new Camera(); cam.Update();

            // Bounding box
            if (boundingBoxToolStripMenuItem.Checked)
                Tools.QDrawer.DrawBox(background.BL, background.TR, Xna.Color.Purple, 50);

            // Camera box
            if (cameraBoxToolStripMenuItem.Checked)
            {
                float Thickness = 20;
                var DefaultSize = new Vector2(1777, 1000) + new Vector2(Thickness);
                var Pos = background.MyLevel.MainCamera.Pos;
                Tools.QDrawer.DrawBox(Pos - DefaultSize, Pos + DefaultSize, Xna.Color.MediumPurple, Thickness);
            }

            Tools.QDrawer.Flush();
        }

        public void Input()
        {
            if (EditingNodeText) return;

            // Lock location
            //var loc = Form.FromHandle(Tools.TheGame.Window.Handle).Location;
            //loc.X -= this.Size.Width;
            ////loc.Y -= this.Size.Height;
            //this.Location = loc;

            // Can not edit while level is active.
            if (!Tools.EditorPause)
            {
                if (ButtonCheck.State(XnaInput.Keys.P).Released)
                    PlayCheckbox.Checked = !PlayCheckbox.Checked;

                return;
            }

            if (Tools.MousePressed())
                MouseActiveInWorld_OnLastMouseClick = MouseActiveInWorld;

            var pos = Tools.MouseWorldPos();
            var delta = Tools.DeltaMouse;

            // Hot Keys
            if (!Tools.CntrlDown() && !Tools.ShiftDown() && !Tools.DialogUp)
            {
                if (ButtonCheck.State(XnaInput.Keys.T).Released)
                    TextureButton_Click(null, null);
                if (ButtonCheck.State(XnaInput.Keys.O).Released)
                    ColorButton_Click(null, null);
                if (ButtonCheck.State(XnaInput.Keys.K).Released)
                    LayerLockCheckbox.Checked = !LayerLockCheckbox.Checked;
                if (ButtonCheck.State(XnaInput.Keys.S).Released)
                    LayerShowCheckbox.Checked = !LayerShowCheckbox.Checked;
                if (ButtonCheck.State(XnaInput.Keys.L).Released)
                    NewLayerButton_Click(null, null);
                if (ButtonCheck.State(XnaInput.Keys.Q).Released)
                    NewFloaterButton_Click(null, null);
                if (ButtonCheck.State(XnaInput.Keys.P).Released)
                    PlayCheckbox.Checked = !PlayCheckbox.Checked;
                if (ButtonCheck.State(XnaInput.Keys.B).Released)
                    MoveBoundsCheckbox.Checked = !MoveBoundsCheckbox.Checked;
                if (ButtonCheck.State(XnaInput.Keys.C).Released)
                    CameraMoveCheckbox.Checked = !CameraMoveCheckbox.Checked;
                if (ButtonCheck.State(XnaInput.Keys.M).Released)
                    MoveQuadsCheckbox.Checked = !MoveQuadsCheckbox.Checked;
                if (ButtonCheck.State(XnaInput.Keys.X).Released)
                {
                    ParallaxNum.Focus();
                    ParallaxNum.Select(0, 100);
                }
            }

            // Copy
            if (Tools.CntrlDown() && ButtonCheck.State(XnaInput.Keys.C).Released)
                CopyFloater();

            // Paste
            if (Tools.CntrlDown() && ButtonCheck.State(XnaInput.Keys.V).Released)
                PasteFloater();

            // Escape to deselect
            if (Tools.keybState.IsKeyDown(XnaInput.Keys.Escape))
                DeselectAll();

            // Delete
            if (ButtonCheck.State(XnaInput.Keys.Delete).Released)
                DeleteSelected();

            // No soft if inactive
            if (!MouseActiveInWorld)
            {
                foreach (var list in background.MyCollection.Lists)
                    foreach (var floater in list.Floaters)
                        floater.SoftSelected = false;
            }

            if (CameraMoveCheckbox.Checked || Tools.CurRightMouseDown())
                MouseInput_Camera(ref pos, ref delta);
            else if (MoveBoundsCheckbox.Checked)
                MouseInput_MoveBounds(ref pos, ref delta);
            else
                MouseInput_Floaters(ref pos, ref delta);
        }

        private void MouseInput_Camera(ref Vector2 pos, ref Vector2 delta)
        {
            // If the mouse is down do movement/scaling of camera.
            if ((Tools.MouseDown() || Tools.CurRightMouseDown() || Tools.ShiftDown()) && !Tools.MousePressed())
            {
                var cam = Tools.CurCamera;
                // If we are holding Shift then zoom the camera.
                if (Tools.ShiftDown())
                {
                    cam.Zoom -= new Vector2(Tools.RawDeltaMouse.X - Tools.RawDeltaMouse.Y) * .000006f * (cam.Zoom.X / .001f);
                    if (cam.Zoom.X > .01) cam.Zoom = new Vector2(.01f);
                    if (cam.Zoom.X < .0001) cam.Zoom = new Vector2(.0001f);

                    cam.EffectiveZoom = cam.Zoom;
                    //cam.Update();
                }
                // Otherwise move it.
                else if (MouseActiveInWorld)
                {
                    //cam.Pos -= delta * .5f / (cam.Zoom.X / .001f);
                    cam.Pos -= delta * 1f;
                    cam.EffectivePos = cam.Pos;

                    //cam.Update();
                    //cam.SetVertexCamera();
                }
            }
        }

        private void MouseInput_MoveBounds(ref Vector2 pos, ref Vector2 delta)
        {
            // If shift is down do scaling of the bounds.
            if (Tools.ShiftDown())
            {
                background.BL -= delta;
                background.TR += delta;
            }
            // If the left mouse is down do moving of the bounds.
            else if (Tools.MouseDown() && !Tools.MousePressed())
            {
                background.BL += delta;
                background.TR += delta;
            }
        }

        private void MouseInput_Floaters(ref Vector2 pos, ref Vector2 delta)
        {
            // If the mouse is down do movement/scaling of selected floaters,
            if ((Tools.MouseDown() || Tools.ShiftDown()) && !Tools.MousePressed())
            {
                foreach (var floater in SelectedFloaters)
                {
                    if (!floater.Editable) continue;

                    var parallax = floater.Parent.Parallax;

                    // If we are holding Shift then scale the quad.
                    if (Tools.ShiftDown())
                    {
                        if (AspectCheckbox.Checked)
                        {
                            floater.MyQuad.Size += new Vector2(delta.X + delta.Y) / parallax;
                            floater.MyQuad.ScaleYToMatchRatio();
                        }
                        else
                            floater.MyQuad.Size += delta / parallax;

                        floater.InitialUpdate();

                        SyncSelectedNumerics();
                    }
                    // Otherwise move it.
                    else if (MouseActiveInWorld)
                    {
                        floater.StartData.Position += delta / parallax;
                        floater.InitialUpdate();
                        SyncSelectedNumerics();
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
                {
                    if (!list.Editable) continue;

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

        private void SyncSelectedNumerics()
        {
            var node = LayerTree.SelectedNode as TreeNode_;
            if (null != node)
                node.SyncNumerics();
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

            LayerTree.SelectedNode = null;
            EnableDisable();
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
                var CopyOfList = new List<BackgroundFloater>(SelectedFloaters);
                foreach (var floater in CopyOfList)
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

            EnableDisable();

            if (node != null)
            {
                node.SyncNumerics();
                if (CurrentLayerNode != null && CurrentLayerNode != node)
                    CurrentLayerNode.SyncNumerics();

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

        private void EnableDisable()
        {
            // Enable/disable based on what is selected
            bool IsLayer = CurrentLayerNode != null; //node is TreeNode_List;
            bool IsFloater = LayerTree.SelectedNode is TreeNode_Floater;

            //ParallaxNum.Enabled = IsLayer;
            LayerBox.Enabled = IsLayer;
            QuadBox.Enabled = IsFloater;
            NewFloaterButton.Enabled = IsLayer;
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
        /// Set parallax event. Update the layer and all floaters.
        /// </summary>
        private void ParallaxNum_ValueChanged(object sender, EventArgs e)
        {
            if (null == CurrentLayerNode) return;

            CurrentLayer.SetParallaxAndPropagate((float)ParallaxNum.Value);
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
                var texture = Tools.Texture(Tools.GetFileName(Dlg.FileName));
                foreach (var floater in SelectedFloaters)
                    SetFloaterTexture(floater, texture);

                SyncSelectedNumerics();

                AddTextureToCombobox(texture);
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

        BackgroundFloater CurrentFloater
        {
            get
            {
                if (CurrentFloaterNode == null)
                    return null;
                else
                    return CurrentFloaterNode.MyFloater;
            }
        }

        TreeNode_Floater CurrentFloaterNode
        {
            get
            {
                return LayerTree.SelectedNode as TreeNode_Floater;
            }
        }

        private void NewFloaterButton_Click(object sender, EventArgs e)
        {
            if (CurrentLayer != null)
            {
                var floater = new BackgroundFloater(Tools.CurLevel);
                floater.StartData.Position = Tools.CurCamera.Pos;
                floater.MyQuad.Size = new Vector2(400, 400);
                floater.MyQuad.Quad.SetColor(new Vector4(1));
                floater.FixedAspectPreference = true;
                floater.InitialUpdate();

                // Add the floater
                var node = CurrentLayerNode.Add(floater);

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

        private void MenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentFileName != null)
            {
                Save(CurrentFileName);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var Dlg = new Forms.OpenFileDialog();
            Tools.DialogUp = true;
            Dlg.Title = "Choose file...";

            if (CurrentFileName == null)
                Dlg.InitialDirectory = Tools.DefaultDynamicDirectory();
            else
                Dlg.InitialDirectory = CurrentFileName;
            Dlg.Filter = "BKG Texture (*.bkg)|*.bkg";
            Dlg.CheckFileExists = true;

            // Check the user didn't cancel
            if (Dlg.ShowDialog() != Forms.DialogResult.Cancel)
            {
                CurrentFileName = Dlg.FileName;
                background.Load(CurrentFileName);
                
                LayerTree.Nodes.Clear();
                FillTree();
            }

            Tools.DialogUp = false;
        }

        string CurrentFileName = null;
        void SaveAs()
        {
            var Dlg = new Forms.SaveFileDialog();
            Tools.DialogUp = true;
            Dlg.Title = "Choose location...";

            Dlg.InitialDirectory = Tools.DefaultDynamicDirectory();
            Dlg.Filter = "BKG Texture (*.bkg)|*.bkg";
            //Dlg.CheckFileExists = true;

            // Check the user didn't cancel
            if (Dlg.ShowDialog() != Forms.DialogResult.Cancel)
            {
                CurrentFileName = Dlg.FileName;
                Save(Dlg.FileName);
            }

            Tools.DialogUp = false;
        }

        void Save(string filename)
        {
            background.Save(filename);
        }

        private void ShowLayer_Click(object sender, EventArgs e)
        {

        }

        private void ColorButton_Click(object sender, EventArgs e)
        {
            var clr = new Forms.ColorDialog();
            Tools.DialogUp = true;

            // Check the user didn't cancel
            if (clr.ShowDialog() != Forms.DialogResult.Cancel)
            {
                var c = clr.Color;
                CurrentFloater.MyQuad.Quad.SetColor(new Xna.Color(c.R, c.G, c.B, c.A));
                CurrentFloaterNode.SyncNumerics();
            }

            Tools.DialogUp = false;
        }

        /// <summary>
        /// Toggle button for Move Camera tool.
        /// </summary>
        private void CameraMoveCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (CameraMoveCheckbox.Checked)
            {
                AllButtonsOff(CameraMoveCheckbox);

                Tools.WinForm.Cursor = Cursors.SizeAll;
            }
            else
                DefaultSwitchOff();
        }

        bool Playing = false;
        private void PlayCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (PlayCheckbox.Checked)
            {
                AllButtonsOff(PlayCheckbox);
                
                Playing = true;
                StatusLabel.Text = "Playing";
                Tools.WinForm.Cursor = Cursors.No;
            }
            else
            {
                if (Playing)
                {
                    background.Reset();
                    StatusLabel.Text = "Stopped";
                    Playing = false;
                }

                DefaultSwitchOff();
            }
        }

        private void LayerShowCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentLayer != null) CurrentLayer.Show = LayerShowCheckbox.Checked;
        }

        private void LayerLockCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentLayer != null) CurrentLayer.Lock = LayerLockCheckbox.Checked;
        }


        /// <summary>
        /// Sync floater values to form values.
        /// </summary>
        void ReverseSync(object sender, EventArgs e)
        {
            ReverseSync();
        }
        void ReverseSync()
        {
            foreach (var floater in SelectedFloaters)
                ReverseSync(floater);
        }
        void ReverseSync(BackgroundFloater floater)
        {
            floater.Data.Position = floater.StartData.Position = FromVectorBox(pos_VectorBox, floater.StartData.Position);
            floater.MyQuad.Size = FromVectorBox(size_VectorBox, floater.MyQuad.Size);
            floater.StartData.Velocity = floater.Data.Velocity = FromVectorBox(vel_VectorBox, floater.Data.Velocity);

            floater.MyQuad.Quad.UV_Offset = FromVectorBox(offset_VectorBox, floater.MyQuad.Quad.UV_Offset);
            floater.MyQuad.Quad.UV_Repeat = FromVectorBox(repeat_VectorBox, floater.MyQuad.Quad.UV_Repeat);
            floater.uv_offset = floater.MyQuad.Quad.UV_Offset;
            floater.uv_speed = FromVectorBox(speed_VectorBox, floater.uv_speed);

            // If we are repeating more than once, or have UV speed, use texture wrapping.
            if (floater.uv_speed != Vector2.Zero ||
                floater.MyQuad.Quad.UV_Repeat.X > 1 ||
                floater.MyQuad.Quad.UV_Repeat.Y > 1)
                floater.MyQuad.Quad.U_Wrap = floater.MyQuad.Quad.V_Wrap = true;
            else
                floater.MyQuad.Quad.U_Wrap = floater.MyQuad.Quad.V_Wrap = false;

            floater.InitialUpdate();
            SyncSelectedNumerics();
        }
        Vector2 FromVectorBox(TextBox box, Vector2 v)
        {
            try
            {
                return Tools.ParseToVector2(box.Text);
            }
            catch
            {
                return v;
            }
        }

        private void BackgroundViewer_Load(object sender, EventArgs e)
        {

        }

        private void MoveBoundsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (MoveBoundsCheckbox.Checked)
            {
                AllButtonsOff(MoveBoundsCheckbox);

                Tools.WinForm.Cursor = Cursors.Cross;
            }
            else
                DefaultSwitchOff();
        }

        /// <summary>
        /// For the group of toggle buttons that should only have one down at a time, this will turn all of them off except one.
        /// </summary>
        void AllButtonsOff(CheckBox except)
        {
            ProgramaticCheck = true;
            if (CameraMoveCheckbox != except) CameraMoveCheckbox.Checked = false;
            if (MoveBoundsCheckbox != except) MoveBoundsCheckbox.Checked = false;
            if (MoveQuadsCheckbox != except) MoveQuadsCheckbox.Checked = false;
            if (PlayCheckbox != except) PlayCheckbox.Checked = false;
            ProgramaticCheck = false;
        }
        bool ProgramaticCheck = false;

        private void MoveQuadsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (MoveQuadsCheckbox.Checked)
            {
                AllButtonsOff(MoveQuadsCheckbox);

                Tools.WinForm.Cursor = Cursors.Default;
            }
            else
                DefaultSwitchOff();
        }

        void DefaultSwitchOff()
        {
            if (ProgramaticCheck) return;
            
            Tools.WinForm.Cursor = Cursors.Default;
            MoveQuadsCheckbox.Checked = true;
        }

        private void TextureCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var floater in SelectedFloaters)
                SetFloaterTexture(floater, (EzTexture)TextureCombobox.SelectedItem);

            SyncSelectedNumerics();
        }

        void SetFloaterTexture(BackgroundFloater floater, EzTexture texture)
        {
            floater.MyQuad.Quad.MyTexture = texture;

            if (AspectCheckbox.Checked)
            {
                floater.MyQuad.ScaleYToMatchRatio();
                floater.InitialUpdate();

                if (!floater.Node.UserSetName)
                    floater.Node.SetName(floater.MyQuad.Quad.MyTexture.Name);
            }
        }

        void AddTextureToCombobox(EzTexture texture)
        {
            if (TextureCombobox.Items.Contains(texture)) return;

            //TextureCombobox.Items.Add(Tools.GetFileName(texture.Name));
            TextureCombobox.Items.Add(texture);
        }

        private void dumpToCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.WriteCode(background);
        }
    }
}
#endif