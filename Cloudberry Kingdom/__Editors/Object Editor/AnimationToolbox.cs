#if WINDOWS
using System;
using System.Windows.Forms;

namespace Drawing
{
    public partial class AnimationToolbox : Form
    {
#if EDITOR
        public AnimationToolbox()
        {
            InitializeComponent();
        }
#endif
        private void AnimationToolbox_Load(object sender, EventArgs e)
        {

        }
    }
}
#endif