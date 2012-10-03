using System.IO;
using Microsoft.Xna.Framework;

using CoreEngine;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Clouds
{
    public class Cloud : _Obstacle
    {
        public class CloudTileInfo : TileInfoBase
        {
            public SpriteInfo Sprite = new SpriteInfo(Tools.Texture("Cloud1"),
                new Vector2(250, 180f), new Vector2(0, -50),
                new Color(1f, 1f, 1f, .95f));
            public Vector2 BoxSize = new Vector2(180, 50);
        }

        public Vector2 Displacement;
        public float Shiftiness;
        public Vector2 Size;

        public QuadClass MyQuad;

        public AABox Box;

        public Cloud() { }
        public Cloud(bool BoxesOnly) { Construct(BoxesOnly); }

        int PeriodOffset = 0;

        public override void MakeNew()
        {
            base.MakeNew();

            PhsxCutoff_Playing = new Vector2(1000);
            PhsxCutoff_BoxesOnly = new Vector2(-100);

            Core.Init();
            Core.DrawLayer = 9;
            Core.MyType = ObjectType.Cloud;
            Core.Holdable = true;

            Core.EditHoldable = true;

            Displacement = Vector2.Zero;

            Core.WakeUpRequirements = true;
        }

        public override void Init(Vector2 pos, Levels.Level level)
        {
            CloudTileInfo info = level.Info.Clouds;

            base.Init(pos, level);

            PeriodOffset = level.Rnd.RndInt(0, 1000);

            Core.MyTileSet = level.MyTileSet;

            Size = level.Info.Clouds.BoxSize;
            Box.Initialize(Pos, Size);

            if (!level.BoxesOnly)
            {
                MyQuad.Set(info.Sprite);
            }
        }

        public override void Construct(bool BoxesOnly)
        {
            Box = new AABox();

            if (!BoxesOnly)
                MyQuad = new QuadClass();

            Core.BoxesOnly = BoxesOnly;

            MakeNew();            
        }

        protected override void ActivePhsxStep()
        {
            int CurPhsxStep = Core.GetPhsxStep();

            Displacement *= .9f;

            float L = Displacement.Length();
            if (L > 1)
            {
                Displacement -= Displacement / L;
            }
            else
                Displacement *= .9f;

            Box.Current.Center = Pos;
            Box.SetTarget(Box.Current.Center + Displacement, Box.Current.Size);

            if (Core.WakeUpRequirements)
            {
                Box.SwapToCurrent();
                Core.WakeUpRequirements = false;
            }
        }

        public override void PhsxStep2()
        {
            if (!Core.Active) return;
            if (Core.SkippedPhsx) return;

            Box.SwapToCurrent();
        }

        protected override void DrawGraphics()
        {
            double t = 2 * System.Math.PI * (Core.GetPhsxStep() + PeriodOffset) / (float)220;
            Vector2 dis = new Vector2(0, (float)System.Math.Cos(t)) * 10;

            MyQuad.Pos = Pos + Displacement + dis;
            MyQuad.Draw();
        }

        protected override void DrawBoxes()
        {
            Box.Draw(Color.Azure, 10);
            //Box.DrawT(Color.Blue, 10);
        }

        public override void Move(Vector2 shift)
        {
            base.Move(shift);

            Box.Move(shift);
        }

        public override void Interact(Bob bob)
        {
            if (!Core.Active) return;

            bool Overlap = false;
            if (!Core.SkippedPhsx)
            {
                Overlap = Phsx.BoxBoxOverlap(bob.Box, Box);

                if (Overlap && Core.MyLevel.PlayMode == 2)
                {
                    Overlap = Phsx.BoxBoxOverlap(bob.Box, Box);

                    bool Delete = false;

                    if (bob.WantsToLand == false) Delete = true;
                    if (bob.MyPhsx.DynamicGreaterThan(bob.Core.Data.Velocity.Y, 10))
                        Delete = true;
                    if (Core.GenData.Used) Delete = false;
                    if (Delete)
                    {
                        CollectSelf();

                        Core.Active = false;
                        return;
                    }
                    else
                    {
                        StampAsUsed(Core.MyLevel.CurPhsxStep);

                        // Remove surrounding clouds
                        foreach (ObjectBase cloud in Core.MyLevel.Objects)
                        {
                            Cloud Cloud = cloud as Cloud;
                            if (null != Cloud)
                                if (!Cloud.Core.GenData.Used &&
                                    (Cloud.Core.Data.Position - Core.Data.Position).Length() < 2.35f * Box.Current.Size.X)
                                {
                                    Core.Recycle.CollectObject(Cloud);
                                    cloud.Core.Active = false;
                                }
                        }
                    }
                }

                if (Overlap)
                {
                    if (bob.MyPhsx.Gravity > 0 && bob.Box.BL.Y < Box.TR.Y - 75 ||
                        bob.MyPhsx.Gravity < 0 && bob.Box.TR.Y > Box.BL.Y + 75)
                    {
                        //if (bob.Core.Data.Velocity.Y < -3.5f)
                        if (bob.MyPhsx.DynamicLessThan(bob.Core.Data.Velocity.Y, -3.5f))
                            bob.Core.Data.Velocity.Y *= .9f;
                    }

                    //if (bob.Core.Data.Velocity.Y <= 0)
                    //if (bob.Core.Data.Velocity.Y >= 0)
                    if (bob.MyPhsx.DynamicLessThan(bob.Core.Data.Velocity.Y, 0))
                    {
                        if (bob.MyPhsx.Gravity > 0)
                            bob.MyPhsx.LandOnSomething(false, this);
                        else
                            bob.MyPhsx.HitHeadOnSomething(this);

                        Displacement += Shiftiness * bob.Core.Data.Velocity / 2;
                    }
                }
            }
        }

        public override void Reset(bool BoxesOnly)
        {
            base.Reset(BoxesOnly);

            Core.WakeUpRequirements = true;
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            Cloud CloudA = A as Cloud;

            Shiftiness = CloudA.Shiftiness;
            Size = CloudA.Size;
            Init(CloudA.Pos, CloudA.MyLevel);

            Displacement = CloudA.Displacement;

            Core.WakeUpRequirements = true;
        }

        public override void Write(BinaryWriter writer)
        {
            Core.Write(writer);

            MyQuad.Write(writer);

            Box.Write(writer);
        }
        public override void Read(BinaryReader reader)
        {
            Core.Read(reader);

            MyQuad.Read(reader);

            Box.Read(reader);
            this.Size = Box.Current.Size;
        }
    }
}
