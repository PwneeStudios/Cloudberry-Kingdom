#if WINDOWS
using System;
using System.Windows.Forms;

#if EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using CloudberryKingdom;
#endif

namespace CoreEngine
{
    public partial class ImportQuads : Form
    {
#if EDITOR
        ObjectClass SourceObject;

        public ImportQuads()
        {
            InitializeComponent();

            Tree.ItemDrag += new ItemDragEventHandler(Tree_ItemDrag);
            //Tree.DragEnter += new DragEventHandler(Tree_DragEnter);
            Tree.DragDrop += new DragEventHandler(Tree_DragDrop);
        }

        void Tree_DragDrop(object sender, DragEventArgs e)
        {
        }

        void Tree_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        void Tree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            Game_Editor.DraggingSource = SourceObject;

            Tree.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Source object...";

            ofd.InitialDirectory = Game_Editor.s;
            ofd.Filter = "Stickman Object file (*.smo)|*.smo";
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() != DialogResult.Cancel)
            {
                FileStream stream = File.Open(ofd.FileName, FileMode.Open, FileAccess.Read, FileShare.None);

                BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);
                SourceObject = new ObjectClass(Tools.QDrawer, Tools.Device, Tools.EffectWad.FindByName("BasicEffect"), Tools.TextureWad.FindByName("White"));
                SourceObject.ReadFile(reader, Tools.EffectWad, Tools.TextureWad);
                reader.Close();
                stream.Close();
                SourceObject.FinishLoading(Tools.QDrawer, Tools.Device, Tools.TextureWad, Tools.EffectWad, Tools.Device.PresentationParameters, 400, 400);

                string CurrentFileLocation = ofd.FileName;
                int i = CurrentFileLocation.LastIndexOf("\\");
                this.FileName.Text = CurrentFileLocation.Substring(i + 1);

                Tree.Nodes.Clear();

                // Create the root quad node
                TreeNode QuadNode = new TreeNode();
                QuadNode.Name = QuadNode.Text = "Quads";
                QuadNode.Checked = true;
                Tree.Nodes.Add(QuadNode);

                // Populate the tree
                Game_Editor.PopulateQuadNode(SourceObject.ParentQuad, QuadNode, SourceObject);

                // Populate the animation list box
                for (int j = 0; j < SourceObject.AnimName.Length; j++)
                    AnimListBox.Items.Add(SourceObject.AnimName[j]);
            }
        }



#else
        private void LoadButton_Click(object sender, EventArgs e) { }
#endif
        private void ImportQuads_Load(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }

#if EDITOR
        List<BaseQuad> GetCheckedQuads(TreeNode node)
        {
            List<BaseQuad> list = new List<BaseQuad>();
            foreach (TreeNode child in node.Nodes)
            {
                if (child.Checked)
                {
                    BaseQuad quad = Game_Editor.GetNodeQuad(child, SourceObject);
                    list.Add(quad);
                    list.AddRange(GetCheckedQuads(child));
                }
            }
            return list;
        }

        List<string> GetCheckedAnims()
        {
            List<string> list = new List<string>();
            foreach (string name in AnimListBox.CheckedItems)
                list.Add(name);
            return list;
        }
#endif
        private void Button_OK_Click(object sender, EventArgs e)
        {
#if EDITOR
            if (Tree.Nodes.Count < 0)
                return;

            List<BaseQuad> quads = GetCheckedQuads(Tree.Nodes[0]);
            List<string> anims = GetCheckedAnims();

            Game_Editor.MainObject.ImportAnimData(SourceObject, quads, anims);

            this.Close();
#endif
        }

        private void Button_No_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AnimListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
#endif