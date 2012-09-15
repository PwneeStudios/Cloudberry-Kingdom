using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Drawing;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Goombas;

namespace CloudberryKingdom
{
    public class BobPhsxMap : BobPhsx
    {
        // Singleton
        protected override void InitSingleton()
        {
            base.InitSingleton();

            Specification = new HeroSpec(0, 0, 0, 0);
            Name = "Map";
            NameTemplate = "hero";
            Icon = new PictureIcon(Tools.TextureWad.FindByName("HeroIcon_Classic"), Color.White, DefaultIconWidth * 1.1f);
        }
        static readonly BobPhsxMap instance = new BobPhsxMap();
        public static BobPhsxMap Instance { get { return instance; } }

        public BackgroundNode CurrentNode, DestinationNode;
        float t;

        public void SetNode(BackgroundNode node)
        {
            CurrentNode = DestinationNode = node;
            t = 0;

            Tools.MoveTo(MyBob, node.Pos);
        }

        WorldGameData World
        {
            get
            {
                return Tools.CurGameData as WorldGameData;
            }
        }

        public void SetDestination(BackgroundNode node)
        {
            if (node == null) return;

            if (World != null && !World.PathIsOpen(CurrentNode, node)) return;

            DestinationNode = node;
            t = 0;
        }

        public BobPhsxMap()
        {
            DefaultValues();
        }

        public override void DefaultValues()
        {
        }

        public override void Init(Bob bob)
        {
            base.Init(bob);

            bob.MyCapeType = CapePrototype;
            if (CapePrototype == Cape.CapeType.None)
                bob.CanHaveCape = false;
        }

        public override bool CheckFor_xFlip()
        {
            if (CurrentNode.Pos.X == DestinationNode.Pos.X)
            {
                MyBob.PlayerObject.xFlip = false;
                return false;
            }

            return base.CheckFor_xFlip();

            //MyBob.PlayerObject.xFlip = false;
            //return false;
        }

        public override void PhsxStep()
        {
            if (World == null || World.NodeSelected) return;

            base.PhsxStep();

            if (CurrentNode == DestinationNode)
                MyBob.Core.Data.Velocity.X = 0;
            else
            {
                float speed = CurrentNode.Pos.Y == DestinationNode.Pos.Y ? 15 : 5;
                MyBob.Core.Data.Velocity.X = MyBob.PlayerObject.xFlip ? -speed : speed;
            }

            // Input
            if (CurrentNode == DestinationNode)
            {
                if (MyBob.CurInput.xVec.X < -.15f)
                    SetDestination(CurrentNode.Connect_W);
                if (MyBob.CurInput.xVec.X >  .15f)
                    SetDestination(CurrentNode.Connect_E);
                if (MyBob.CurInput.xVec.Y < -.15f)
                    SetDestination(CurrentNode.Connect_S);
                if (MyBob.CurInput.xVec.Y > .15f)
                    SetDestination(CurrentNode.Connect_N);
            }

            // Positioning
            {
                if ((Tools.CurCamera.Pos - Tools.CurCamera.Target).Length() < 1200)
                {
                    // Slide speed
                    //t += .05f;
                    t += .05f * .66666f * 1000f / (DestinationNode.Pos - CurrentNode.Pos).Length();
                    //t += .05f * .685f;
                }

                if (t > 1)
                {
                    t = 0;
                    CurrentNode = DestinationNode;
                }

                Vector2 p1 = CurrentNode.Pos;
                Vector2 p2 = DestinationNode.Pos;
                Vector2 pos = Vector2.Lerp(p1, p2, t);
                Tools.MoveTo(MyBob, pos);
            }
        }

        public override void AnimStep()
        {
            SpriteAnimStep();
        }

        protected virtual void SpriteAnimStep()
        {
            // Standing animation
            if (CurrentNode == DestinationNode && MyBob.PlayerObject.DestinationAnim() != 0)
            {
                MyBob.PlayerObject.AnimQueue.Clear();
                MyBob.PlayerObject.EnqueueAnimation(0, 1, true);
            }

            // Running animation
            if (CurrentNode != DestinationNode && MyBob.PlayerObject.DestinationAnim() != 1)
            {
                MyBob.PlayerObject.AnimQueue.Clear();

                MyBob.PlayerObject.EnqueueAnimation(1, 0, true);
                MyBob.PlayerObject.DequeueTransfers();
            }

            //// Jump animation
            //if (!Ducking)
            //    if (ShouldStartJumpAnim())
            //    {
            //        int anim = 2; float speed = .85f;
            //        if (CurJump > 1) { anim = 29; speed = 1.2f; }

            //        MyBob.PlayerObject.AnimQueue.Clear();
            //        MyBob.PlayerObject.EnqueueAnimation(anim, 0, false);
            //        MyBob.PlayerObject.DequeueTransfers();
            //        MyBob.PlayerObject.LastAnimEntry.AnimSpeed *= speed;

            //        StartJumpAnim = false;
            //    }

            float AnimSpeed = 1.5f;
            if (MyBob.PlayerObject.DestinationAnim() == 1 && MyBob.PlayerObject.Loop)
                AnimSpeed = 1.29f * RunAnimSpeed * Math.Max(.35f, .1f * Math.Abs(xVel));

            Obj.PlayUpdate(1);

            Obj.DoSpriteAnim = true;
        }
    }
}