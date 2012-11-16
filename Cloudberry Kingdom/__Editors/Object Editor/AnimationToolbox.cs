#if WINDOWS
using System;
using System.Windows.Forms;

namespace CloudberryKingdom
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