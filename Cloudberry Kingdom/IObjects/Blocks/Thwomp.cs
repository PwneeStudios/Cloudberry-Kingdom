﻿using System.IO;
using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;

using Drawing;

namespace CloudberryKingdom
{
    public enum ThwompState { Waiting, Falling, Pause, Rising, Vibrate };
    public class Thwomp : BlockBase
    {
        public QuadClass MyQuad;
        ThwompState State;
        
        public override void MakeNew()
        {
            KillsOnSmash = true;

            Core.Init();
            Core.DrawLayer = 8;

            SetState(ThwompState.Waiting);
        }

        public override void Release()
        {
            base.Release();

            OnSmash = null;
            MyQuad = null;
        }

        public void SetState(ThwompState NewState) { SetState(NewState, false); }
        public void SetState(ThwompState NewState, bool ForceSet)
        {
            if (State != NewState || ForceSet)
            {
                Count = 0;

                switch (NewState)
                {
                    case ThwompState.Waiting:
                        MyQuad.Quad.Set("FallingBlock1");
                        break;
                    case ThwompState.Falling:
                        MyQuad.Quad.Set("FallingBlock2");
                        break;
                    case ThwompState.Pause:
                        MyQuad.Quad.Set("FallingBlock3");
                        break;
                    case ThwompState.Rising:
                        MyQuad.Quad.Set("FallingBlock4");
                        break;
                    case ThwompState.Vibrate:
                        Tools.Sound("Rumble_Short").Play();
                        MyQuad.Quad.Set("FallingBlock4");
                        VibrateIntensity = 3f;
                        break;
                }
            }

            // Always angry
            MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName(InfoWad.GetStr("FallingBlock_Angry_Texture"));

            State = NewState;
        }

        public Thwomp(bool BoxesOnly)
        {
            MySound = Tools.Sound("Bash");

            MyQuad = new QuadClass();

            MyBox = new AABox();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public Vector2 Size;
        public void Init(Vector2 center, Vector2 size)
        {
            Size = size;

            Active = true;

            BlockCore.Layer = .35f;
            MyBox = new AABox(center, Size);
            MyQuad.Base.Origin = BlockCore.Data.Position = BlockCore.StartData.Position = center;

            MyBox.Initialize(center, Size);

            SetState(ThwompState.Waiting, true);
            MyQuad.Base.e1.X = size.X * 1.105f;
            MyQuad.Base.e2.Y = size.Y * 1.105f;
            MyQuad.Quad.Shift(new Vector2(0, -11f) / size);

            Update();

            SetState(ThwompState.Waiting);
        }

        public bool KillsOnSmash;
        public Action<Bob> OnSmash;
        public override void Smash(Bob bob) { if (OnSmash != null) OnSmash(bob); }

        public override void HitHeadOn(Bob bob)
        {
            if (KillsOnSmash)
                bob.Die(Bob.BobDeathType.Other);

            //if (OnSmash != null) OnSmash(bob);
        }

        public override void Reset(bool BoxesOnly)
        {
            BlockCore.BoxesOnly = BoxesOnly;

            Active = true;

            SetState(ThwompState.Waiting, true);

            BlockCore.Data = BlockCore.StartData;

            BlockCore.StoodOn = false;

            MyBox.Current.Center = BlockCore.StartData.Position;

            MyBox.SetTarget(MyBox.Current.Center, Size);
            MyBox.SwapToCurrent();

            Update();
        }

        public int PauseAtBottomLength = 60;//93;
        public override void PhsxStep()
        {
            Active = Core.Active = true;
            if (!Core.Held)
            {
                if (MyBox.Current.BL.X > BlockCore.MyLevel.MainCamera.TR.X || MyBox.Current.BL.Y > BlockCore.MyLevel.MainCamera.TR.Y)
                    Active = Core.Active = false;
                if (MyBox.Current.TR.X < BlockCore.MyLevel.MainCamera.BL.X || MyBox.Current.TR.Y < BlockCore.MyLevel.MainCamera.BL.Y - 200)
                    Active = Core.Active = false;
            }


            if (Core.MyLevel.GetPhsxStep() % 2 == 0)
                Offset = Vector2.Zero;

            // Update the block's apparent center according to attached objects
            BlockCore.UseCustomCenterAsParent = true;
            BlockCore.CustomCenterAsParent = Box.Target.Center + Offset;

            //if (CoreData.StoodOn)
            //{
            //    if (Core.MyLevel.GetPhsxStep() % 2 == 0)
            //        Offset = new Vector2(MyLevel.Rnd.Rnd.Next(-10, 10), MyLevel.Rnd.Rnd.Next(-10, 10));
            //}

            Vibrate();
            if (State != ThwompState.Vibrate)
                VibrateIntensity = 0;

            if (State == ThwompState.Vibrate)
            {
                if (Count > 50)
                    SetState(ThwompState.Falling);
                if (MyQuad != null) MyQuad.Pos = Pos + Offset;
            }
            else if (State == ThwompState.Falling)
            {
                Core.Data.Velocity.Y = Tools.Restrict(-68, 0, Core.Data.Velocity.Y - 2.5f);
                Core.Data.Position.Y += BlockCore.Data.Velocity.Y;
                Box.SetTarget(Pos + new Vector2(0, -1), Size);
                if (MyQuad != null) MyQuad.Pos = Pos + Offset;

                // Interact with blocks
                foreach (BlockBase block in Core.MyLevel.Blocks)
                    ColWithBlock(block);
            }
            else if (State == ThwompState.Pause)
            {
                //Core.Data.Velocity.Y = Tools.Restrict(-40, 30, Core.Data.Velocity.Y - 1.5f);
                //Core.Data.Position.Y += CoreData.Data.Velocity.Y;
                //Box.SetTarget(Pos + new Vector2(0, -1), Size);
                //if (MyQuad != null) MyQuad.Pos = Pos + Offset;

                //// Interact with blocks
                //foreach (Block block in Core.MyLevel.Blocks)
                //    ColWithBlock(block);

                // Wait a bit and then start rising
                if (Count > PauseAtBottomLength)
                    SetState(ThwompState.Rising);
            }
            else if (State == ThwompState.Rising)
            {
                Core.Data.Velocity.Y = Tools.Restrict(-5, 20, Core.Data.Velocity.Y + 1.05f);
                Core.Data.Position.Y += BlockCore.Data.Velocity.Y;
                Box.SetTarget(Pos + new Vector2(0, -1), Size);
                if (MyQuad != null) MyQuad.Pos = Pos + Offset;

                // Stop when we reach the top
                if (Core.Data.Position.Y > Core.StartData.Position.Y)
                    SetState(ThwompState.Waiting);
            }
            else
            {
                Box.SetTarget(Pos + new Vector2(0, -1), Size);
                if (MyQuad != null) MyQuad.Pos = Pos + Offset;
            }

            Update();

            BlockCore.StoodOn = false;

            Count++;
        }

        public override void PhsxStep2()
        {
            if (!Active) return;

            MyBox.SwapToCurrent();
        }

        public void Update()
        {
            if (BlockCore.BoxesOnly) return;
        }

        protected EzSound MySound;
        int Count = 0;
        void ColWithBlock(BlockBase block)
        {
            if (block.Core.MarkedForDeletion || !block.IsActive || !block.Core.Real) return;

            ColType col = Phsx.CollisionTest(Box, block.Box);
            if (col == ColType.Top)
            {
                float VelFadeBegin = -15;
                if (MySound != null && Core.Data.Velocity.Y < -2)
                {
                    MySound.Play(Tools.Restrict(0, 1, Core.Data.Velocity.Y / VelFadeBegin));
                    Tools.CurCamera.StartShake(.5f, 36, false);
                }

                //Core.Data.Position += new Vector2(0, block.Box.TR.Y - Box.Target.BL.Y + 1f);
                Core.Data.Position += new Vector2(0, block.Box.TR.Y - Box.Target.BL.Y);
                Box.SetTarget(Pos, Size);
                Box.SetCurrent(Pos + new Vector2(0, 5), Size);
                //Box.Move(new Vector2(0, 5));
                //if (Core.Data.Velocity.Y < 0) Core.Data.Velocity.Y *= -1 * .5f;
                if (Core.Data.Velocity.Y < 0) Core.Data.Velocity.Y *= 0;

                if (MyQuad != null) MyQuad.Pos = Pos;

                // Hit bottom, wait a bit before going back up
                SetState(ThwompState.Pause);
            }
        }

        public override void Extend(Side side, float pos)
        {
            switch (side)
            {
                case Side.Left:
                    MyBox.Target.BL.X = pos;
                    break;
                case Side.Right:
                    MyBox.Target.TR.X = pos;
                    break;
                case Side.Top:
                    MyBox.Target.TR.Y = pos;
                    break;
                case Side.Bottom:
                    MyBox.Target.BL.Y = pos;
                    break;
            }

            MyBox.Target.FromBounds();
            MyBox.SwapToCurrent();

            Update();

            BlockCore.StartData.Position = MyBox.Current.Center;
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

                VibrateIntensity -= .0285f;
            }
        }

        public override void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);

            Update();
        }
        public override void Draw()
        {
            bool DrawSelf = true;
            if (!Core.Held)
            {
                if (!Active) DrawSelf = false;
                if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, 600))
                    DrawSelf = false;
            }

            if (DrawSelf)
            {
                Update();

                if (Tools.DrawBoxes)
                    MyBox.Draw(Tools.QDrawer, Color.Olive, 15);
            }

            if (Tools.DrawGraphics)
            {
                if (DrawSelf && !BlockCore.BoxesOnly)
                if (!BlockCore.BoxesOnly)
                    MyQuad.Draw();

                BlockCore.Draw();
            }
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            Thwomp BlockA = A as Thwomp;

            Init(BlockA.Box.Current.Center, BlockA.Size);
        }
    }
}