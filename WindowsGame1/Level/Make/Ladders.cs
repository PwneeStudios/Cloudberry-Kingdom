using Microsoft.Xna.Framework;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom.Levels
{
    public partial class Level
    {

        public enum LadderType { None, FinalPlat, FinalBouncy, Simple, Simple2, Double, SimpleMoving, DoubleMoving, MakePlat };
        public static Vector2 GetLadderSize(LadderType Type)
        {
            switch (Type)
            {
                case LadderType.Simple:
                    return new Vector2(1050, 0);

                case LadderType.Simple2:
                    return new Vector2(1050, 0);

                case LadderType.SimpleMoving:
                    return new Vector2(850, 0);

                case LadderType.DoubleMoving:
                    return new Vector2(850, 0);

                case LadderType.MakePlat:
                    return new Vector2(1000, 0);
            }

            return Vector2.Zero;
        }

        public void MakeLadder(PieceSeedData Piece)
        {
            Vector2 LeftCenter = Piece.Start;
            LadderType Ladder = Piece.Ladder;

            Vector2 Center, Size, pos;
            BlockEmitter bm;
            NormalBlock block;

            CameraZone CamZone;

            switch (Ladder)
            {
                case LadderType.FinalBouncy:
                    float y = LeftCenter.Y - MainCamera.GetHeight() / 2 - 250;
                    while (y < LeftCenter.Y + MainCamera.GetHeight() / 2 - 400)
                    {
                        BouncyBlock bouncy;
                        bouncy = (BouncyBlock)MySourceGame.Recycle.GetObject(ObjectType.BouncyBlock, false);
                        bouncy.Init(new Vector2(LeftCenter.X, y), new Vector2(220, 220), 70);
                        bouncy.Core.DrawLayer = 9;

                        bouncy.Core.GenData.RemoveIfUnused = true;
                        bouncy.BlockCore.BlobsOnTop = false;
                        bouncy.Core.GenData.AlwaysLandOn = true;

                        AddBlock(bouncy);

                        y += 200;
                    }

                    break;

                case LadderType.FinalPlat:
                    FinalPlat = block = (NormalBlock)Recycle.GetObject(ObjectType.NormalBlock, true);
                    block.Init(TR, Vector2.Zero, MyTileSetInfo);
                    block.Extend(Side.Left, LeftCenter.X);
                    block.Extend(Side.Right, LeftCenter.X + 5000);
                    block.Extend(Side.Top, LeftCenter.Y - MainCamera.GetHeight() / 2 + 150);
                    block.Extend(Side.Bottom, LeftCenter.Y - MainCamera.GetHeight() / 2 - 250);

                    AddBlock(block);


                    // Camera zone
                    FinalCamZone = CamZone = (CameraZone)Recycle.GetObject(ObjectType.CameraZone, false);
                    CamZone.Init(block.Box.Current.Center + new Vector2(700, 500),
                                 block.Box.Current.Size / 3 + new Vector2(0, MainCamera.GetHeight() / 2 + 1000));
                    CamZone.Start = LeftCenter - new Vector2(100000, 0);
                    CamZone.End = CamZone.Start;
                    CamZone.End.X = 400 + block.Box.Current.TR.X - MainCamera.GetWidth() / 2 - 1500;
                    CamZone.Start = CamZone.End;
                    AddObject(CamZone);


                    // Stop block, to prevent Bob from running off edge
                    block = (NormalBlock)Recycle.GetObject(ObjectType.NormalBlock, true);
                    block.Init(TR, Vector2.Zero, MyTileSetInfo);
                    block.Extend(Side.Left, LeftCenter.X + 4400);
                    block.Extend(Side.Right, LeftCenter.X + 4500);
                    block.Extend(Side.Bottom, LeftCenter.Y - 1500);
                    block.Extend(Side.Top, LeftCenter.Y + 1500);

                    AddBlock(block);

                    break;

                case LadderType.Simple:
                    Size = new Vector2(250, 40);
                    Center = LeftCenter + new Vector2(GetLadderSize(Ladder).X / 2, 0);

                    pos = Center - new Vector2(0, MainCamera.GetHeight() / 2);
                    while (pos.Y < Center.Y + MainCamera.GetHeight() / 2)
                    {
                        block = (NormalBlock)Recycle.GetObject(ObjectType.NormalBlock, true);
                        block.Box.TopOnly = true;
                        block.Init(pos, Size, MyTileSetInfo);

                        block.Core.GenData.RemoveIfUnused = false;
                        block.BlockCore.BlobsOnTop = false;
                        block.Core.GenData.AlwaysUse = true;
                        block.BlockCore.Virgin = true;

                        AddBlock(block);

                        pos.Y += 400;
                    }

                    break;

                case LadderType.Simple2:
                    Size = new Vector2(100, 40);
                    Center = LeftCenter + new Vector2(GetLadderSize(Ladder).X / 2, 0);

                    pos = Center - new Vector2(0, MainCamera.GetHeight() / 2);
                    Vector2 offset = new Vector2(90, 0);
                    int Count = 0;
                    while (pos.Y < Center.Y + MainCamera.GetHeight() / 2)
                    {
                        block = (NormalBlock)Recycle.GetObject(ObjectType.NormalBlock, true);
                        block.Box.TopOnly = true;
                        if (Count % 2 == 0)
                            block.Init(pos + offset, Size, MyTileSetInfo);
                        else
                            block.Init(pos - offset, Size, MyTileSetInfo);

                        block.Core.GenData.RemoveIfUnused = false;
                        block.BlockCore.BlobsOnTop = false;
                        block.Core.GenData.AlwaysUse = true;
                        block.BlockCore.Virgin = true;

                        AddBlock(block);

                        Count++;
                        pos.Y += 400;
                    }

                    break;

                case LadderType.SimpleMoving:
                    Center = LeftCenter + new Vector2(GetLadderSize(Ladder).X / 2, -MainCamera.GetHeight() / 2 - 250);

                    bm = (BlockEmitter)Recycle.GetObject(ObjectType.BlockEmitter, false);
                    bm.EmitData.Position = bm.Core.Data.Position = Center;
                    bm.EmitData.Velocity = new Vector2(0, 6);
                    bm.Delay = 100;
                    bm.Offset = 5;
                    bm.Size = new Vector2(250, 40);
                    bm.AlwaysOn = true;
                    bm.Range.Y = MainCamera.GetHeight() / 2;

                    bm.Core.GenData.RemoveIfUnused = false;
                    bm.Core.GenData.AlwaysUse = true;

                    AddObject(bm);

                    break;

                case LadderType.DoubleMoving:
                    // Left emitter                    
                    Center = LeftCenter + new Vector2(GetLadderSize(Ladder).X / 2, -MainCamera.GetHeight() / 2 - 250);

                    bm = (BlockEmitter)Recycle.GetObject(ObjectType.BlockEmitter, false);
                    bm.EmitData.Position = bm.Core.Data.Position = Center + new Vector2(-175, 0);
                    bm.EmitData.Velocity = new Vector2(0, 6);
                    bm.Delay = 100;
                    bm.Offset = 5;
                    bm.Size = new Vector2(120, 40);
                    bm.AlwaysOn = true;
                    bm.Range.Y = MainCamera.GetHeight() / 2;

                    bm.Core.GenData.RemoveIfUnused = false;
                    bm.Core.GenData.AlwaysUse = true;

                    AddObject(bm);

                    // Right emitter
                    Center = LeftCenter + new Vector2(GetLadderSize(Ladder).X / 2, MainCamera.GetHeight() / 2 + 250);

                    bm = (BlockEmitter)Recycle.GetObject(ObjectType.BlockEmitter, false);
                    bm.EmitData.Position = bm.Core.Data.Position = Center + new Vector2(175, 0);
                    bm.EmitData.Velocity = new Vector2(0, -6);
                    bm.Delay = 100;
                    bm.Offset = 5;
                    bm.Size = new Vector2(120, 40);
                    bm.AlwaysOn = true;
                    bm.Range.Y = MainCamera.GetHeight() / 2;

                    bm.Core.GenData.RemoveIfUnused = false;
                    bm.Core.GenData.AlwaysUse = true;

                    AddObject(bm);

                    break;
            }
        }
    }
}