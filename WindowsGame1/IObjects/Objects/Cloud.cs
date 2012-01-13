using System.IO;
using Microsoft.Xna.Framework;

using Drawing;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Clouds
{
    public class Cloud : IObject
    {
        public void TextDraw() { }
        public void Release()
        {
            Core.Release();
        }

        static EzSound PuffSound;

        public Vector2 Displacement;
        public float Shiftiness;
        public Vector2 Size;// = new Vector2(240, 50);

        public QuadClass MyQuad;



        public AABox Box;

        public ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        public void MakeNew()
        {
            Core.Init();
            Core.DrawLayer = 9;
            Core.MyType = ObjectType.Cloud;
            Core.Holdable = true;

            Core.EditHoldable = true;

            Displacement = Vector2.Zero;

            if (!Core.BoxesOnly)
            {
                MyQuad.SetToDefault();
                MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName("Cloud1");
                MyQuad.Quad.SetColor(new Color(1f, 1f, 1f, .95f));
                //Box.Initialize(Core.Data.Position, Size);
            }

            Core.WakeUpRequirements = true;
           
            //Box.SetTarget(Core.Data.Position, Box.Current.Size);
            //Box.SwapToCurrent();
        }

        public void Init(Vector2 Size, TileSet tileset) { Init(Size, TileSets.Get(tileset)); }
        public void Init(Vector2 Size, TileSetInfo MyTileSet)
        {
            Core.MyTileSet = MyTileSet;
            this.Size = Size;
            Box.Initialize(Core.Data.Position, Size);

            Core.MyTileSet = MyTileSet;
            if (!Core.BoxesOnly)
                if (MyTileSet.Type == TileSet.Terrace)// || MyTileSet.Type == TileSet.Island)
                    MyQuad.Quad.SetColor(new Color(.6f, .6f, .6f, .95f));
        }

        public Cloud(bool BoxesOnly)
        {
            CoreData = new ObjectData();

            Box = new AABox();

            if (!BoxesOnly)
                MyQuad = new QuadClass();

            Core.BoxesOnly = BoxesOnly;

            MakeNew();            
        }

        public void PhsxStep()
        {
            if (!Core.Active) return;

            //float PhsxCutoff = 1800;
            float PhsxCutoff = 1000;
            if (Core.MyLevel.BoxesOnly) PhsxCutoff = -100;
            if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, PhsxCutoff))
            {
                Core.WakeUpRequirements = true;
                Core.SkippedPhsx = true;
                return;
            }
            Core.SkippedPhsx = false;

            int CurPhsxStep = Core.GetPhsxStep();

            Displacement *= .9f;

            float L = Displacement.Length();
            if (L > 1)
            {
                Displacement -= Displacement / L;
            }
            else
                Displacement *= .9f;

            Box.Current.Center = Core.Data.Position;
            if (!Core.BoxesOnly)
            {
                MyQuad.Base.Origin = Core.Data.Position;
                MyQuad.Base.Origin.Y -= Box.Current.Size.Y * .875f;
                MyQuad.Base.e1.X = Box.Current.Size.X * 1.5f;
                MyQuad.Base.e2.Y = Box.Current.Size.Y * 4.5f;
                MyQuad.Base.Origin += Displacement;
            }
            Box.SetTarget(Box.Current.Center + Displacement, Box.Current.Size);

            if (Core.WakeUpRequirements)
            {
                Box.SwapToCurrent();
                Core.WakeUpRequirements = false;
            }
        }

        public void PhsxStep2()
        {
            if (!Core.Active) return;
            if (Core.SkippedPhsx) return;

            Box.SwapToCurrent();
        }

        public void Draw()
        {
            if (!Core.Held)
            {
                if (!Core.Active || Core.SkippedPhsx) return;

                Vector2 BL = Box.Current.BL - new Vector2(225, 225);
                if (BL.X > Core.MyLevel.MainCamera.TR.X || BL.Y > Core.MyLevel.MainCamera.TR.Y)
                    return;
                Vector2 TR = Box.Current.TR + new Vector2(225, 225);
                if (TR.X < Core.MyLevel.MainCamera.BL.X || TR.Y < Core.MyLevel.MainCamera.BL.Y)
                    return;
            }

            if (!Core.BoxesOnly && Tools.DrawGraphics)
                MyQuad.Draw();
            if (Tools.DrawBoxes)
            {
                Box.DrawT(Tools.QDrawer, Color.Blue, 10);
                Box.Draw(Tools.QDrawer, Color.Azure, 10);
            }
        }

        public void Move(Vector2 shift)
        {
            Core.StartData.Position += shift;
            Core.Data.Position += shift;

            Box.Move(shift);

            MyQuad.Base.Origin += shift;
        }

        public void Interact(Bob bob)
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
                    //if (bob.Core.Data.Position.Y > bob.TargetPosition.Y) Delete = true;
                    if (bob.WantsToLand == false) Delete = true;
                    if (bob.Core.Data.Velocity.Y > 10) Delete = true;
                    if (Core.GenData.Used) Delete = false;
                    if (Delete)
                    {
                        this.CollectSelf();

                        Core.Active = false;
                        return;
                    }
                    else
                    {
                        this.StampAsUsed(Core.MyLevel.CurPhsxStep);

                        // Remove surrounding clouds
                        foreach (IObject cloud in Core.MyLevel.Objects)
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
                    if (bob.Box.BL.Y < Box.TR.Y - 75)
                    {
                        if (bob.Core.Data.Velocity.Y > 3.3f || bob.Core.Data.Velocity.Y < -3.5f)
                            bob.Core.Data.Velocity.Y *= .9f;
                    }

                    if (bob.Core.Data.Velocity.Y <= 0)
                    {
                        bob.MyPhsx.ObjectLandedOn = this;
                        bob.MyPhsx.LandOnSomething(false);

                        Displacement += Shiftiness * bob.Core.Data.Velocity / 2;
                    }
                }
            }
        }

        public void Reset(bool BoxesOnly)
        {
            Core.Active = true;
            Core.WakeUpRequirements = true;
        }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);

            Cloud CloudA = A as Cloud;

            //Box.Clone(CloudA.Box);
            
            Shiftiness = CloudA.Shiftiness;
            Size = CloudA.Size;
            Init(CloudA.Size, CloudA.Core.MyTileSet);

            Displacement = CloudA.Displacement;

            Core.WakeUpRequirements = true;
        }

        public void Write(BinaryWriter writer)
        {
            Core.Write(writer);

            MyQuad.Write(writer);

            Box.Write(writer);
        }
        public void Read(BinaryReader reader)
        {
            Core.Read(reader);

            MyQuad.Read(reader);

            Box.Read(reader);
            this.Size = Box.Current.Size;
        }
//StubStubStubStart
public void OnUsed() { }
public void OnMarkedForDeletion() { }
public void OnAttachedToBlock() { }
public bool PermissionToUse() { return true; }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public void Smash(Bob bob) { }
//StubStubStubEnd6
    }
}
