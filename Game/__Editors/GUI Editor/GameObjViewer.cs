#if WINDOWS
using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Microsoft.Xna.Framework;
using input = Microsoft.Xna.Framework.Input;

using System.Reflection;

namespace CloudberryKingdom.Viewer
{
    public partial class GameObjViewer : Form
    {
        public GameObjViewer()
        {
            InitializeComponent();

			this.ObjTree.MouseClick += new MouseEventHandler(ObjTree_MouseClick);

            FillTree();
        }

		System.Drawing.Point LastRightClick;
		void ObjTree_MouseClick(object sender, MouseEventArgs e)
		{
			var tree_node = this.ObjTree.GetNodeAt(e.Location);
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				FieldNode node = tree_node as FieldNode;
				if (null != node)
				{
					//SelectedFieldNode = node;
					//node.SetClipboard();
				}
				else
				{
					ViewableNode vnode = tree_node as ViewableNode;
					if (null != vnode)
					{
						//vnode.SetClipboard();

						if (MultiSelected.Contains(vnode))
						{
							vnode.BackColor = System.Drawing.Color.White;
							MultiSelected.Remove(vnode);
						}
						else
						{
							vnode.BackColor = System.Drawing.Color.Honeydew;
							MultiSelected.Add(vnode);

							// Loop through all intermediate points between last right click and this right click.
							if (Tools.ShiftDown())
							{
								System.Drawing.Point point = LastRightClick;
								while (point.Y != e.Location.Y)
								{
									var _tree_node = this.ObjTree.GetNodeAt(point);
									ViewableNode _vnode = _tree_node as ViewableNode;

									if (null != _vnode)
									{
										if (!MultiSelected.Contains(_vnode))
										{
											_vnode.BackColor = System.Drawing.Color.Honeydew;
											MultiSelected.Add(_vnode);
										}
									}

									if (point.Y > e.Location.Y)
										point.Y--;
									else
										point.Y++;
								}
							}

							LastRightClick = e.Location;
						}
					}
				}
			}
		}

        void SetFloatBoxProperties(NumericUpDown box)
        {
            box.Maximum = 100000;
            box.Minimum = -100000;
        }

		public static List<MenuItem> SelectedMenuItems = new List<MenuItem>();
		public static List<QuadClass> SelectedQuads = new List<QuadClass>();
		public static List<Text> SelectedTexts = new List<Text>();

        bool PositionSet = false;
        public void Input()
        {
			Vector2 delta = Tools.DeltaMouse;
			//delta.X = 0;
			//delta.Y = 0;

			SelectedMenuItems.Clear();
			foreach (var item in MultiSelected)
			{
				MenuItem mitem = item.MyItem as MenuItem;
				if (null != mitem)
					SelectedMenuItems.Add(mitem);
			}

			SelectedQuads.Clear();
			foreach (var item in MultiSelected)
			{
				QuadClass qitem = item.MyItem as QuadClass;
				if (null != qitem)
					SelectedQuads.Add(qitem);
			}

			SelectedTexts.Clear();
			foreach (var item in MultiSelected)
			{
				Text titem = item.MyItem as Text;
				if (null != titem)
					SelectedTexts.Add(titem);
			}

            if (!PositionSet)
            {
                this.Location = new System.Drawing.Point(1460, 194);
                Form.FromHandle(Tools.GameClass.Window.Handle).Location = new System.Drawing.Point(170, 200);

                PositionSet = true;
            }

            FieldNode node = ObjTree.SelectedNode as FieldNode;

            // If selected node is a TreeNode
            if (null != node)
            {
                float sensitivity = 0;
                if (Tools.Keyboard.IsKeyDown(input.Keys.LeftShift))
                    sensitivity = 1;
                if (Tools.Keyboard.IsKeyDown(input.Keys.Z))
                    sensitivity = .003f * .001f;
                else if (Tools.Keyboard.IsKeyDown(input.Keys.X))
                    sensitivity = .001f;

                if (sensitivity > 0)
                {
                    if (node.field.FieldType == typeof(float))
                        node.Set(node.GetFloat() + sensitivity * Tools.DeltaMouse.X);
                    if (node.field.FieldType == typeof(Vector2))
                    {
                        Vector2 val = sensitivity * Tools.DeltaMouse;
                        if (Tools.Keyboard.IsKeyDown(input.Keys.LeftControl))
                            val.Y = val.X;

                        node.Set(node.GetVector2() + val);
                    }
                }
            }
            else
            {
                ViewableNode vnode = ObjTree.SelectedNode as ViewableNode;
                if (vnode != null &&
                    (Tools.CntrlDown() || Tools.ShiftDown()))
                {
                    //vnode.MyItem.ProcessMouseInput(Tools.DeltaMouse, Tools.ShiftDown());
					foreach (ViewableNode vn in MultiSelected)
						vn.MyItem.ProcessMouseInput(delta, Tools.ShiftDown());
                }
            }
        }

        class ViewableNode : TreeNode
        {
            public ViewReadWrite MyItem;

            public void SetClipboard()
            {
                string str = MyItem.CopyToClipboard("");

                try
                {
                    Clipboard.SetText(str);
                }
                catch
                {
                }
            }
        }

        class FieldNode : TreeNode
        {
            public ViewReadWrite ParentViewable;
            public FieldInfo field;

            public string RootText = "";

            public void SetText()
            {
                Text = RootText + field.GetValue(ParentViewable).ToString();
            }

            public void Set(object value)
            {
                field.SetValue(ParentViewable, value);
                
                SetText();
                SetClipboard();
            }

            public void SetClipboard()
            {
                string str = "";
                if (field.FieldType == typeof(float))
                    str = string.Format("{0}f", GetFloat());
                if (field.FieldType == typeof(Vector2))
                {
                    Vector2 v = GetVector2();
                    str = string.Format("new Vector2({0}f, {1}f)", v.X, v.Y);
                }

                try
                {
					System.Windows.Forms.Clipboard.SetText(str);
                }
                catch
                {
                }
            }

            public Vector2 GetVector2()
            {
                return (Vector2)field.GetValue(ParentViewable);
            }

            public float GetFloat()
            {
                return (float)field.GetValue(ParentViewable);
            }
        }

        TreeNode ViewableToNode(ViewReadWrite viewable, int Depth)
        {
            //TreeNode node = new TreeNode();
            ViewableNode node = new ViewableNode();
            node.MyItem = viewable;
            node.Name = node.Text = viewable.GetType().Name;

            if (Depth > 10) return node;

            string[] _viewables = viewable.GetViewables();
            List<string> viewables = new List<string>(), unviewables = new List<string>();

            foreach (string fieldname in _viewables)
            {
                if (fieldname[0] == '!')
                    unviewables.Add(fieldname.Substring(1));
                else
                    viewables.Add(fieldname);
            }

            TreeNode childnode;
            

            List<InstancePlusName> ViewableChildren = new List<InstancePlusName>();

            foreach (FieldInfo info in viewable.GetType().GetFields())
            {
                // Check if field is itself viewable and not banned
                ViewReadWrite child = info.GetValue(viewable) as ViewReadWrite;
                //if (info.Name == "MyMenu") Tools.Write("!");

                if (!info.FieldType.IsValueType && null == child) continue;

                //if (!unviewables.Contains(info.Name) && info.FieldType.GetInterfaces().Contains(typeof(ViewReadWrite)))
                if (!unviewables.Contains(info.Name) && child is ViewReadWrite)
                {
                    ViewableChildren.Add(new InstancePlusName(child, info.Name));
                }
                else
                {
                    // Check if field is listed as viewable
                    if (viewables.Contains(info.Name))
                    {
                        FieldNode fieldnode = new FieldNode();
                        fieldnode.Text = fieldnode.Name = info.Name;
                        node.Nodes.Add(fieldnode);

                        fieldnode.field = info;
                        fieldnode.ParentViewable = viewable;

                        fieldnode.Text += ". . . ";

                        fieldnode.RootText = fieldnode.Text;

                        fieldnode.SetText();
                    }
                }
            }

            // If viewable has a list of children add all of them as child nodes
            IViewableList viewablelist = viewable as IViewableList;
            if (null != viewablelist)
                viewablelist.GetChildren(ViewableChildren);


            // Add viewable children
            foreach (InstancePlusName child in ViewableChildren)
            {
                childnode = ViewableToNode(child.instance, Depth + 1);
                node.Nodes.Add(childnode);
                childnode.Text += " : " + child.name;
            }

            return node;
        }

        void FillTree()
        {
            foreach (GameObject obj in Tools.CurGameData.MyGameObjects)
            {
                ViewReadWrite viewable = obj as ViewReadWrite;
                if (null != viewable)
                    ObjTree.Nodes.Add(ViewableToNode(viewable, 0));
            }
            ObjTree.Nodes.Add(ViewableToNode(CharacterSelectManager.Instance, 0));
        }

		List<ViewableNode> MultiSelected = new List<ViewableNode>();

        FieldNode SelectedFieldNode = null;
        [STAThread]
        private void ViewerTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            FieldNode node = e.Node as FieldNode;
            if (null != node)
            {
                SelectedFieldNode = node;

                node.SetClipboard();
            }
            else
            {
                ViewableNode vnode = e.Node as ViewableNode;
				if (null != vnode)
				{
					vnode.SetClipboard();

					// Clear multi-select
					foreach (ViewableNode vn in MultiSelected)
						vn.BackColor = System.Drawing.Color.White;
					MultiSelected.Clear();

					vnode.BackColor = System.Drawing.Color.Honeydew;
					MultiSelected.Add(vnode);
				}
            }
        }

        private void Viewer_Load(object sender, EventArgs e)
        {

        }

        private void dumpToCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var to_save = this.ObjTree.SelectedNode as ViewableNode;
            if (null == to_save) return;

            Tools.WriteCode("o", to_save.MyItem);
        }
    }
}
#endif