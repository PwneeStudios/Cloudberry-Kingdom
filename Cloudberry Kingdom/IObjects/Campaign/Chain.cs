﻿using System.IO;
using Microsoft.Xna.Framework;

using Drawing;
using System;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public partial class Chain : ObjectBase
    {
        public Vector2 p1, Desired_p2;
        Vector2 _p2;
        public Vector2 p2 { get { return _p2; } set { _p2 = Desired_p2 = value; } }

        protected QuadClass End1, End2;

        public bool DrawEnd = false;

        public override void MakeNew()
        {
            Core.ResetOnlyOnReset = true;

            if (!Core.BoxesOnly)
            {
            }
        }

        public Chain()
        {
            Init(false);
        }
        protected void Init(bool BoxesOnly)
        {
            Core.Init();
            Core.MyType = ObjectType.Undefined;
            Core.EditHoldable = false;
            Core.DrawLayer = 7;
            
            if (!BoxesOnly)
            {
                End1 = new QuadClass(null, true, true);
                End2 = new QuadClass(null, true, true);
            }

            MakeNew();

            Core.BoxesOnly = BoxesOnly;

            ChainTexture = Tools.TextureWad.FindByName("chain_tile_dark");
            //ChainTexture = Tools.TextureWad.FindByName("Chain_Tile");
            EndTexture = Tools.TextureWad.FindByName("Joint");
        }
        public EzTexture ChainTexture, EndTexture;

        public Vector2 GetEndPoint()
        {
            return p2 + Offset;
        }

        Vector2 Offset;
        float VibrateIntensity;

        int Step = 0;
        public override void PhsxStep()
        {
            if (VibrateIntensity > 0)
            {
                if (Core.MyLevel.GetPhsxStep() % 2 == 0)
                    Offset = VibrateIntensity * new Vector2(MyLevel.Rnd.Rnd.Next(-10, 10), MyLevel.Rnd.Rnd.Next(-10, 10));

                VibrateIntensity -= .0685f;
            }

            Step++;

            int Period = 75;// 60;
            if (Step % Period == 0)
            {
                Desired_p2.Y -= 63;
            }
            else if (Step % Period == 7)
            {
                VibrateIntensity = .9f;
            }

            _p2 += .2f * (Desired_p2 - p2);
        }

        public override void Draw()
        {
            if (Core.MyLevel == null) return;

            Camera cam = Core.MyLevel.MainCamera;
            if (!cam.OnScreen(p1, 100) && !cam.OnScreen(p2, 100))
                return;

            if (Tools.DrawGraphics && !Core.BoxesOnly)
            {
                //int ChainWidth = 44;
                //int ChainRepeat = 63;
                //int JointWidth = 90;

                int ChainWidth = (int)(44 * 1.23f);
                int ChainRepeat = (int)(63 * 1.23f);
                int JointWidth = (int)(90 * 1.23f);

                Tools.QDrawer.DrawLine(p2, p1,
                            new Color(255, 255, 255, 210),
                            ChainWidth,
                            ChainTexture, Tools.BasicEffect, ChainRepeat, 0, 0.2f);

                Tools.QDrawer.DrawSquareDot(p1, Color.White, JointWidth, EndTexture, Tools.BasicEffect);

                if (DrawEnd)
                    Tools.QDrawer.DrawSquareDot(p2, Color.White, JointWidth, EndTexture, Tools.BasicEffect);
            }
        }

        public override void Move(Vector2 shift)
        {
            Core.Data.Position += shift;
            Core.StartData.Position += shift;

            p1 += shift;
            p2 += shift;
        }

        public override void Reset(bool BoxesOnly)
        {
            Core.Active = true;

            Core.Data.Position = Core.StartData.Position;
            Core.Data.Velocity = Vector2.Zero;
        }

        public override void Interact(Bob bob)
        {
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            Chain ChainA = A as Chain;

            p1 = ChainA.p1;
            p2 = ChainA.p2;
        }
    }
}