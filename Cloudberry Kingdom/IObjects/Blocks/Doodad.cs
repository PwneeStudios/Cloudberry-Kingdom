using System;
using System.IO;

using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Coins;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public delegate void HitDoodadCallback();
    public class Doodad : BlockBase
    {
        public override string ToString()
        {
            return string.Format("{0} : {1} : {2}, a {3}", Core.EditorCode1, Core.EditorCode2, Core.EditorCode3, base.ToString());
        }

        EzTexture HoldTexture1, HoldTexture2;
        bool Bool1;

        public int DelayToActive = 0;
        
        public enum Type { None, LayerZone, CameraStop, Plank, Block, DifficultyBox, Icon, SelectBox, CastlePiece, Coin, 
                           Envelope };
        public static int Type2Offset = 10;
        public static string[] TypeName = { "None", "LayerZone", "CameraStop", "Plank", "Block", "DifficultyBox", "Icon", "SelectBox", "CastlePiece", "Coin" };
        public static string[] Type2Name = { "Envelope" };
        public Type MyType;

        public SimpleObject MyObject;
        public float MyAnimSpeed;
        public string MyFileName;
        public int MyAnim;

        public HitDoodadCallback HitCallback;

        public QuadClass MyQuad;

        public override void MakeNew()
        {
            Active = true;

            BlockCore.Init();
            BlockCore.MyType = ObjectType.Doodad;
            Core.EditHoldable = true;
        }

        public override void Release()
        {
            base.Release();

            MyQuad = null;
        }

        public Doodad()
        {
            MyBox = new AABox();
            MyBox.Initialize(Vector2.Zero, Vector2.Zero);

            MyQuad = new QuadClass();

            MakeNew();
        }
            
        public void Init(Vector2 Pos, Vector2 Size, string filename, int Anim, float Speed)
        {
            BlockCore.Data.Position = Pos;

            MyBox.Initialize(Pos, Size);

            BlockCore.StartData.Position = Pos;

            MyFileName = filename;
            MyAnimSpeed = Speed;
            MyObject = Tools.LoadSimpleObject(Path.Combine(Globals.ContentDirectory, "Objects\\" + filename));

            MyAnim = Anim;
            MyObject.Read(Anim, 0);
            MyObject.Play = true;
            MyObject.Loop = true;
            MyObject.EnqueueAnimation(Anim, 0, true);
            MyObject.DequeueTransfers();
        }

        public void Init(Vector2 Pos, Vector2 Size)
        {
            BlockCore.Data.Position = Pos;

            MyBox.Initialize(Pos, Size);

            BlockCore.StartData.Position = Pos;            
        }

        public void InitType()
        {
            for (int i = 0; i < TypeName.Length; i++)
                if (string.Compare(Core.EditorCode1, TypeName[i], StringComparison.OrdinalIgnoreCase) == 0)
                    MyType = (Type)i;

            for (int i = 0; i < Type2Name.Length; i++)
                if (string.Compare(Core.EditorCode2, Type2Name[i], StringComparison.OrdinalIgnoreCase) == 0)
                    MyType = (Type)(i + Type2Offset);

            //Console.WriteLine("{0}, {1}", Core.EditorCode1, MyType);

            switch (MyType)
            {
                case Type.Coin:                    
                    IsActive = BlockCore.Active = false;
                    Core.Show = false;
                    Coin coin = (Coin)Core.Recycle.GetObject(ObjectType.Coin, false);
                    coin.Core.EditorCode1 = "DoodadCoin";
                    coin.Move(Core.Data.Position - coin.Core.Data.Position);
                    coin.Core.DrawLayer = Core.DrawLayer;
                    Core.MyLevel.AddObject(coin);
                    coin.Reset(false);

                    break;

                case Type.CastlePiece:
                    Core.Active = IsActive = false;

                    float Darken = 1;
                    Darken -= Core.DrawLayer * .01f;
                    MyQuad.Quad.SetColor(new Color(new Vector3(1, 1, 1) * Darken));

                    NormalBlock block;

                    Vector2 SideCenter = new Vector2(125, 600);
                    Vector2 ZoneCenter = new Vector2(0, 75);
                    Vector2 SideSize = new Vector2(90, 575);
                    Vector2 TopCenter = new Vector2(0, -115);

                    if (string.Compare(MyQuad.Quad.MyTexture.Path, "castle_big", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        SideCenter = new Vector2(145, 920);
                        ZoneCenter = new Vector2(0, 44);
                        SideSize = new Vector2(92, 875);
                        TopCenter = new Vector2(0, -138);
                    }
                    
                    block = (NormalBlock)Core.Recycle.GetObject(ObjectType.NormalBlock, false);
                    block.Init(new Vector2(Core.Data.Position.X, MyBox.Current.TR.Y) + TopCenter,
                               new Vector2(MyBox.Current.Size.X - 70, 10), Core.MyLevel.MyTileSetInfo);
                    block.Core.MyTileSet = TileSets.CastlePiece;
                    block.BlockCore.OnlyCollidesWithLowerLayers = true;
                    block.BlockCore.Show = false;
                    block.MakeTopOnly();
                    block.Core.DrawLayer = Core.DrawLayer;
                    block.Core.EditHoldable = false;
                    Core.MyLevel.AddedBlocks.Add(block);

                    block = (NormalBlock)Core.Recycle.GetObject(ObjectType.NormalBlock, false);
                    block.Core.MyTileSet = TileSets.CastlePiece;
                    block.BlockCore.OnlyCollidesWithLowerLayers = true;
                    block.Init(MyBox.Current.TR - SideCenter,
                               SideSize, Core.MyLevel.MyTileSetInfo);
                    block.BlockCore.Show = false;
                    //block.MakeTopOnly();
                    block.Core.DrawLayer = Core.DrawLayer;
                    block.Core.EditHoldable = false;
                    Core.MyLevel.AddedBlocks.Add(block);

                    block = (NormalBlock)Core.Recycle.GetObject(ObjectType.NormalBlock, false);
                    block.Core.MyTileSet = TileSets.CastlePiece;
                    block.BlockCore.OnlyCollidesWithLowerLayers = true;
                    SideCenter.X *= -1;
                    block.Init(MyBox.Current.TL() - SideCenter,
                               SideSize, Core.MyLevel.MyTileSetInfo);
                    block.BlockCore.Show = false;
                    //block.MakeTopOnly();
                    block.Core.DrawLayer = Core.DrawLayer;
                    block.Core.EditHoldable = false;
                    Core.MyLevel.AddedBlocks.Add(block);

                    Doodad zone = new Doodad();
                    zone.Init(new Vector2(Core.Data.Position.X, MyBox.Current.TR.Y) + ZoneCenter,
                               new Vector2(MyBox.Current.Size.X, 100));
                    zone.Core.EditorCode1 = "LayerZone";
                    zone.Core.EditorCode2 = (Core.DrawLayer - 1).ToString();
                    zone.InitType();
                    zone.Core.DrawLayer = 9;
                    zone.Core.EditHoldable = false;
                    Core.MyLevel.AddedBlocks.Add(zone);
                    break;

                case Type.LayerZone:
                    IsActive = false;
                    BlockCore.Active = false;
                    BlockCore.Show = false;

                    break;

                case Type.CameraStop:
                    IsActive = false;
                    BlockCore.Active = false;
                    BlockCore.Show = false;

                    break;

                case Type.Plank:
                    MyQuad.Shadow = true;
                    MyQuad.ShadowColor = new Color(50, 50, 50);
                    MyQuad.ShadowOffset = new Vector2(8, 8);
                    MyBox.TopOnly = true;
                    IsActive = true;
                    BlockCore.Active = true;

                    break;

                case Type.Icon:
                    MyQuad.Shadow = true;
                    MyQuad.ShadowColor = new Color(50, 50, 50);
                    MyQuad.ShadowOffset = new Vector2(8, 8);
                    IsActive = false;
                    BlockCore.Active = false;
                    BlockCore.Show = true;

                    Bool1 = (string.Compare(Core.EditorCode2, "bonus") != 0);
                    HoldTexture1 = Tools.TextureWad.FindByName("levelicon");
                    HoldTexture2 = Tools.TextureWad.FindByName("icon" + Core.EditorCode2);

                    break;

                case Type.Block:
                    IsActive = true;
                    BlockCore.Active = true;

                    if (string.Compare(Core.EditorCode2, "Shadow", 0) == 0)
                    {
                        MyQuad.Shadow = true;
                        MyQuad.ShadowColor = new Color(50, 50, 50);
                        MyQuad.ShadowOffset = new Vector2(14, 14);
                    }

                    break;

                case Type.Envelope:
                    IsActive = true;
                    BlockCore.Active = true;
                    Box.Target.Set(Box.Target.Center, new Vector2(75, 75));
                    Box.SwapToCurrent();
                    break;

                default:
                    IsActive = false;
                    BlockCore.Active = false;
                    //BlockCore.Show = false;

                    break;
            }

            if (Core.ContainsCode("Inactive"))
                IsActive = Core.Active = Core.Show = false;
        }

        public override void PhsxStep()
        {
            switch (MyType)
            {
                case Type.Envelope:
                    if (DelayToActive == 0)
                    {
                        IsActive = true;
                        BlockCore.Active = true;
                    }
                    else
                        DelayToActive--;
                    break;

                case Type.LayerZone:
                    foreach (Bob bob in Core.MyLevel.Bobs)
                    {
                        Vector2 BR = new Vector2(bob.Box.TR.X, bob.Box.BL.Y);
                        if (Phsx.PointAndAABoxCollisionTest(ref bob.Box.BL, MyBox) ||
                            Phsx.PointAndAABoxCollisionTest(ref BR, MyBox))
                        {
                            bob.Core.DrawLayer = int.Parse(Core.EditorCode2);
                        }
                    }

                    break;
            }
        }

        public override void PhsxStep2()
        {
            Box.SwapToCurrent();
        }

        public void Update()
        {
            if (MyObject != null)
            {
                MyObject.PlayUpdate(MyAnimSpeed);
                MyObject.Base.Origin = Core.Data.Position;
                MyObject.Base.e1.X = 3.5f * MyBox.Current.Size.X;
                MyObject.Base.e2.Y = 3.5f * MyBox.Current.Size.Y;

                if (MyType == Type.Envelope)
                {
                    MyObject.Base.e1.X = 650;
                    MyObject.Base.e2.Y = 650;
                }

                MyObject.Update();
            }
            else
            {
                MyQuad.Base.Origin = Core.Data.Position;
                MyQuad.Base.e1.X = Box.Current.Size.X;
                MyQuad.Base.e2.Y = Box.Current.Size.Y;
            }
        }

        public override void Draw()
        {
            MyBox.CalcBounds();
            Vector2 BL = MyBox.RealBL();
            if (BL.X > BlockCore.MyLevel.MainCamera.TR.X || BL.Y > BlockCore.MyLevel.MainCamera.TR.Y)
                return;
            Vector2 TR = MyBox.RealTR();
            if (TR.X < BlockCore.MyLevel.MainCamera.BL.X || TR.Y < BlockCore.MyLevel.MainCamera.BL.Y)
                return;

            if (Tools.DrawGraphics && BlockCore.Show)
            {
                Update();

                switch (MyType)
                {                        
                    case Type.Icon:
                        if (Bool1)
                        {
                            MyQuad.Shadow = true;
                            MyQuad.Quad.MyTexture = HoldTexture1;
                            MyQuad.Draw();

                            MyQuad.Shadow = false;
                            MyQuad.Quad.MyTexture = HoldTexture2;
                            MyQuad.Draw();
                        }
                        else
                        {
                            MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName("levelicon_boss");
                            MyQuad.Draw();
                        }
                        break;

                    default:
                        if (MyObject != null)
                            MyObject.Draw(Tools.QDrawer, Tools.EffectWad);
                        else
                            MyQuad.Draw();
                        break;
                }
            }          

            if (Tools.DrawBoxes)
            {
                switch (MyType)
                {
                    case Type.LayerZone:
                        Box.Draw(Tools.QDrawer, Color.BlueViolet, 15);
                        Box.DrawT(Tools.QDrawer, Color.BlueViolet, 15);
                        break;

                    default:
                        Box.Draw(Tools.QDrawer, Color.Azure, 15);
                        Box.DrawT(Tools.QDrawer, Color.Azure, 15);
                        break;
                }
            }
        }

        public override void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);

            BlockCore.Data.Position = BlockCore.StartData.Position = Box.Target.Center;
        }

        public override void Hit(Bob bob)
        {
            if (HitCallback != null)
                HitCallback();

            switch (MyType)
            {
                case Type.Envelope:                    
                    //Tools.Recycle.CollectObject(this);
                    DelayToActive = 45;
                    IsActive = Core.Active = false;
                    break;
            }
        }

        public override void Extend(Side side, float pos)
        {
            MyBox.Extend(side, pos);

            Update();

            BlockCore.StartData.Position = BlockCore.Data.Position = MyBox.Current.Center;
        }

        public void MatchBoxToQuad()
        {
            MyBox.Target.Center = MyQuad.Pos;
            MyBox.Target.Size = MyQuad.Size;
            MyBox.SwapToCurrent();
        }

        public void MakeBoxSquare()
        {
            MyBox.Target.Size.Y = MyBox.Target.Size.X;
            MyBox.SwapToCurrent();
        }

        public override void Reset(bool BoxesOnly)
        {
            BlockCore.BoxesOnly = BoxesOnly;

            BlockCore.Data = BlockCore.StartData;

            MyBox.Current.Center = BlockCore.StartData.Position;
            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            Doodad DoodadA = A as Doodad;
            BlockCore.Clone(DoodadA.BlockCore);

            MyQuad.Quad.MyTexture = DoodadA.MyQuad.Quad.MyTexture;            

            if (DoodadA.MyObject != null)
                Init(DoodadA.MyBox.Current.Center, DoodadA.Box.Current.Size, DoodadA.MyFileName, DoodadA.MyAnim, DoodadA.MyAnimSpeed);
            else
                Init(DoodadA.MyBox.Current.Center, DoodadA.Box.Current.Size);
        }

        public override void Write(BinaryWriter writer)
        {
            BlockCore.Write(writer);
            Box.Write(writer);

            if (MyObject == null)
            {
                writer.Write(0);
                MyQuad.Write(writer);
            }
            else
            {
                writer.Write(1);
                writer.Write(MyFileName);
                writer.Write(MyAnimSpeed);
                writer.Write(MyAnim);
            }
        }
        public override void Read(BinaryReader reader)
        {
            BlockCore.Read(reader);
            Box.Read(reader);

            int type = reader.ReadInt32();
            if (type == 0)
                MyQuad.Read(reader);
            else
            {
                MyFileName = reader.ReadString();
                MyAnimSpeed = reader.ReadSingle();
                MyAnim = reader.ReadInt32();

                Init(MyBox.Current.Center, MyBox.Current.Size, MyFileName, MyAnim, MyAnimSpeed);
            }
        }
    }
}
