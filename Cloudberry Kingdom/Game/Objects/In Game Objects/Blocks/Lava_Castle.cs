using System.IO;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class LavaBlock_Castle : LavaBlock
    {
        public LavaBlock_Castle(bool BoxesOnly) : base(BoxesOnly)
        {
        }

        public override void MakeNew()
        {
            base.MakeNew();

            MyQuad.EffectName = "Basic";
            MyQuad.TextureName = "Castle_Lava";
            TextureSize = new Vector2(2048, 128) * 1f;
        }

        protected override void SetQuad(Vector2 center, Vector2 size)
        {
            base.SetQuad(center, size);

            MyQuad.SizeY = 400;
            MyQuad.PosY = Box.Target.TR.Y - MyQuad.SizeY + 75;
        }

        public override void PhsxStep()
        {
            base.PhsxStep();

            u_offset = -.15f * Tools.t;
            SetUV();
        }

        public override void Draw()
        {
            Update();

            MyQuad.PosY = Box.Target.TR.Y - MyQuad.SizeY + 75
                + (float)System.Math.Cos(Tools.t) * 15;

            if (Tools.DrawGraphics)
            {
                if (!BlockCore.BoxesOnly)
                {
                    MyQuad.Quad.U_Wrap = true;

                    MyQuad.Draw();
                    Tools.QDrawer.Flush();
                }

                BlockCore.Draw();
            }

            if (Tools.DrawBoxes)
            {
                MyBox.Draw(Tools.QDrawer, Color.Olive, 15);
            }
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            LavaBlock BlockA = A as LavaBlock;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size);
        }
    }
}
