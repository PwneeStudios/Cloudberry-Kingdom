#if WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Microsoft.Xna.Framework;

using CloudberryKingdom;

using System.Reflection;

namespace CloudberryKingdom.Viewer
{
    public partial class GameObjViewer : Form
    {
        public GameObjViewer()
        {
            InitializeComponent();

            FillTree();
        }

        void SetFloatBoxProperties(NumericUpDown box)
        {
            box.Maximum = 100000;
            box.Minimum = -100000;
        }

        public void Input()
        {
            FieldNode node = ObjTree.SelectedNode as FieldNode;

            // If selected node is a TreeNode
            if (null != node)
            {
                float sensitivity = 0;
                if (Tools.keybState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                    sensitivity = 1;
                if (Tools.keybState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Z))
                    sensitivity = .003f * .001f;
                else if (Tools.keybState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.X))
                    sensitivity = .001f;

                if (sensitivity > 0)
                {
                    if (node.field.FieldType == typeof(float))
                        node.Set(node.GetFloat() + sensitivity * Tools.DeltaMouse.X);
                    if (node.field.FieldType == typeof(Vector2))
                    {
                        Vector2 val = sensitivity * Tools.DeltaMouse;
                        if (Tools.keybState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                            val.Y = val.X;

                        node.Set(node.GetVector2() + val);
                    }
                }
            }
        }

        class FieldNode : TreeNode
        {
            public IViewable ParentViewable;
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
                    Clipboard.SetText(str);
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

        TreeNode ViewableToNode(IViewable viewable, int Depth)
        {
            TreeNode node = new TreeNode();
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
                if (!unviewables.Contains(info.Name) && info.FieldType.GetInterfaces().Contains(typeof(IViewable)))
                {
                    IViewable child = (IViewable)info.GetValue(viewable);
                    //IViewable child = (IViewable)viewable.GetType().InvokeMember(info.Name,
                    //    BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public,
                    //    null, viewable, null);

                    if (child != null) ViewableChildren.Add(new InstancePlusName(child, info.Name));
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
                IViewable viewable = obj as IViewable;
                if (null != viewable)
                    ObjTree.Nodes.Add(ViewableToNode(viewable, 0));
            }
            ObjTree.Nodes.Add(ViewableToNode(CharacterSelectManager.Instance, 0));
        }

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
        }

        private void Viewer_Load(object sender, EventArgs e)
        {

        }
    }
}
#endif