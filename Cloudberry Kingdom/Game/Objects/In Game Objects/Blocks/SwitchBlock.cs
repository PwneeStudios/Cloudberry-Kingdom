using Microsoft.Xna.Framework;
using CoreEngine;
using System.IO;
using System;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Blocks
{
    public class SwitchBlock : BlockBase
    {
        QuadClass MyQuad;

        public override void MakeNew()
        {
            BlockCore.Init();
            BlockCore.MyType = ObjectType.OnOffBlock;

            Core.EditHoldable = false;

            VibrateIntensity = 0;
            Offset = Vector2.Zero;
        }

        public Action OnActivate;

        public SwitchBlock(Vector2 Pos)
        {
            Active = true;

            Vector2 Size = new Vector2(100);
            MyBox = new AABox(Pos, Size);
            MyQuad = new QuadClass("difficultybox1", Size.X * 1.05f);

            MakeNew();

            Core.StartData.Position = Core.Data.Position = Pos;

            BlockCore.Layer = .9f;

            Update();
        }

        public void Update()
        {
            MyQuad.Pos = Box.Current.Center;
        }

        public override void PhsxStep()
        {
            CountSinceActivation++;

            bool HoldActive = Active;
            Core.Show = true;

            Vibrate();

            Update();

            Box.SetTarget(BlockCore.Data.Position + Offset, Box.Current.Size);
        }

        Vector2 Offset;
        float VibrateIntensity;
        void Vibrate()
        {
            int step = Core.MyLevel.GetPhsxStep();
            if (step % 2 == 0)
                Offset = Vector2.Zero;

            // Update the block's apparent center according to attached objects
            BlockCore.UseCustomCenterAsParent = true;
            BlockCore.CustomCenterAsParent = Box.Target.Center + Offset;

            if (VibrateIntensity > 0)
            {
                if (Core.MyLevel.GetPhsxStep() % 2 == 0)
                    Offset = VibrateIntensity * new Vector2(MyLevel.Rnd.Rnd.Next(-10, 10), MyLevel.Rnd.Rnd.Next(-10, 10));

                VibrateIntensity -= .0585f;
            }
        }

        public override void PhsxStep2()
        {
            Box.SwapToCurrent();
        }

        public override void Draw()
        {
            MyQuad.Draw();

            if (Tools.DrawBoxes)
                Box.Draw(Tools.QDrawer, Color.Olive, 15);
        }

        public override void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);

            Update();
        }

        public int DelayToActive = 40;
        int CountSinceActivation;
        public override void HitHeadOn(Bob bob)
        {
            VibrateIntensity = 1.2f;

            if (OnActivate != null && CountSinceActivation > DelayToActive) OnActivate();

            CountSinceActivation = 0;
        }

        public override void Reset(bool BoxesOnly)
        {
            CountSinceActivation = 0;

            BlockCore.BoxesOnly = BoxesOnly;

            Active = true;

            BlockCore.Data = BlockCore.StartData;

            MyBox.Current.Center = BlockCore.StartData.Position;
            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();

            Update();
        }
    }
}
