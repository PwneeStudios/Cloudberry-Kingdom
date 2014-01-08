using System.IO;
using Microsoft.Xna.Framework;

using CoreEngine;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Obstacles
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

            CoreData.Init();
            CoreData.DrawLayer = 9;
            CoreData.MyType = ObjectType.Cloud;
            CoreData.Holdable = true;

            CoreData.EditHoldable = true;

            Displacement = Vector2.Zero;

            CoreData.WakeUpRequirements = true;
        }

        public override void Init(Vector2 pos, Levels.Level level)
        {
            CloudTileInfo info = level.Info.Clouds;

            base.Init(pos, level);

            PeriodOffset = level.Rnd.RndInt(0, 1000);

            CoreData.MyTileSet = level.MyTileSet;

            Size = level.Info.Clouds.BoxSize;
            Box.Initialize(CoreData.Data.Position, Size);

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

            CoreData.BoxesOnly = BoxesOnly;

            MakeNew();            
        }

        protected override void ActivePhsxStep()
        {
            int CurPhsxStep = CoreData.GetPhsxStep();

            Displacement *= .9f;

            float L = Displacement.Length();
            if (L > 1)
            {
                Displacement -= Displacement / L;
            }
            else
                Displacement *= .9f;

            Box.Current.Center = CoreData.Data.Position;
            Box.SetTarget(Box.Current.Center + Displacement, Box.Current.Size);

            if (CoreData.WakeUpRequirements)
            {
                Box.SwapToCurrent();
                CoreData.WakeUpRequirements = false;
            }
        }

        public override void PhsxStep2()
        {
            if (!CoreData.Active) return;
            if (CoreData.SkippedPhsx) return;

            Box.SwapToCurrent();
        }

        protected override void DrawGraphics()
        {
            double t = 2 * System.Math.PI * (CoreData.GetPhsxStep() + PeriodOffset) / (float)220;
            Vector2 dis = new Vector2(0, (float)System.Math.Cos(t)) * 10;

            MyQuad.Pos = CoreData.Data.Position + Displacement + dis;
            MyQuad.Draw();
        }

        protected override void DrawBoxes()
        {
			Vector2 Offset, Drop;
			
			Offset = Vector2.Zero;
			Drop = new Vector2(0, -30);
			Tools.QDrawer.DrawFilledBox(Box.Current.BL + Offset + Drop, Box.Current.TR + Offset, new Color(255, 255, 255, 125));

			Offset = new Vector2(20, -10);
			Tools.QDrawer.DrawFilledBox(Box.Current.BL + Offset + Drop, Box.Current.TR + Offset, new Color(255, 255, 255, 125));

			Offset = new Vector2(-20, -20);
			Tools.QDrawer.DrawFilledBox(Box.Current.BL + Offset + Drop, Box.Current.TR + Offset, new Color(255, 255, 255, 125));


			//Box.Draw(Color.Azure, 10);
        }

        public override void Move(Vector2 shift)
        {
            base.Move(shift);

            Box.Move(shift);
        }

        public override void Interact(Bob bob)
        {
            if (!CoreData.Active) return;

            bool Overlap = false;
            if (!CoreData.SkippedPhsx)
            {
                Overlap = Phsx.BoxBoxOverlap(bob.Box, Box);

                if (Overlap && CoreData.MyLevel.PlayMode == 2)
                {
                    Overlap = Phsx.BoxBoxOverlap(bob.Box, Box);

                    bool Delete = false;

                    if (bob.WantsToLand == false) Delete = true;
                    if (bob.MyPhsx.DynamicGreaterThan(bob.CoreData.Data.Velocity.Y, 10))
                        Delete = true;
                    if (CoreData.GenData.Used) Delete = false;
                    if (Delete)
                    {
                        CollectSelf();

                        CoreData.Active = false;
                        return;
                    }
                    else
                    {
                        StampAsUsed(CoreData.MyLevel.CurPhsxStep);

                        // Remove surrounding clouds
                        foreach (ObjectBase cloud in CoreData.MyLevel.Objects)
                        {
                            Cloud Cloud = cloud as Cloud;
                            if (null != Cloud)
                                if (!Cloud.CoreData.GenData.Used &&
                                    (Cloud.CoreData.Data.Position - CoreData.Data.Position).Length() < 2.35f * Box.Current.Size.X)
                                {
                                    CoreData.Recycle.CollectObject(Cloud);
                                    cloud.CoreData.Active = false;
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
                        if (bob.MyPhsx.DynamicLessThan(bob.CoreData.Data.Velocity.Y, -3.5f))
                            bob.CoreData.Data.Velocity.Y *= .9f;
                    }

                    //if (bob.Core.Data.Velocity.Y <= 0)
                    //if (bob.Core.Data.Velocity.Y >= 0)
                    if (bob.MyPhsx.DynamicLessThan(bob.CoreData.Data.Velocity.Y, 0))
                    {
                        if (bob.MyPhsx.Gravity > 0)
                            bob.MyPhsx.LandOnSomething(false, this);
                        else
                            bob.MyPhsx.HitHeadOnSomething(this);

                        Displacement += Shiftiness * bob.CoreData.Data.Velocity / 2;
                    }
                }
            }
        }

        public override void Reset(bool BoxesOnly)
        {
            base.Reset(BoxesOnly);

            CoreData.WakeUpRequirements = true;
        }

        public override void Clone(ObjectBase A)
        {
            CoreData.Clone(A.CoreData);

            Cloud CloudA = A as Cloud;

            Shiftiness = CloudA.Shiftiness;
            Size = CloudA.Size;
            Init(CloudA.CoreData.Data.Position, CloudA.MyLevel);

            Displacement = CloudA.Displacement;

            CoreData.WakeUpRequirements = true;
        }

        public override void Write(BinaryWriter writer)
        {
            CoreData.Write(writer);

            MyQuad.Write(writer);

            Box.Write(writer);
        }
        public override void Read(BinaryReader reader)
        {
            CoreData.Read(reader);

            MyQuad.Read(reader);

            Box.Read(reader);
            this.Size = Box.Current.Size;
        }
    }
}
