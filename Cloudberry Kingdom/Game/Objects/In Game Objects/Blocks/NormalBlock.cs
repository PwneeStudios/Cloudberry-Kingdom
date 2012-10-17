using System.IO;
using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Blocks
{
    public delegate void BlockExtendCallback(NormalBlock block);
    public class NormalBlock : BlockBase
    {
        public bool Invert = false;

        public BlockBase HoldBlock;

        public bool Moved;

        public void BasicConstruction(bool BoxesOnly)
        {
            Core.BoxesOnly = BoxesOnly;

            MyBox = new AABox();
            MyDraw = new NormalBlockDraw();

            MakeNew();
        }

        public override void Release()
        {
            base.Release();
        
            if (MyDraw != null) MyDraw.Release();
            MyDraw = null;
        }

        public override void MakeNew()
        {
            Active = true;

            Invert = false;

            Box.MakeNew();

            if (MyDraw != null)
                MyDraw.MakeNew();

            BlockCore.Init();
            BlockCore.Layer = .3f;
            Core.DrawLayer = 1;
            Core.MyType = ObjectType.NormalBlock;
            Core.EditHoldable = Core.Holdable = true;

            Init(Vector2.Zero, Vector2.Zero, TileSets.DefaultTileSet);
        }

        public NormalBlock(bool BoxesOnly)
        {
            BasicConstruction(BoxesOnly);
        }

        PieceQuad GetPieceTemplate()
        {
            if (BlockCore.MyOrientation == PieceQuad.Orientation.RotateRight ||
                BlockCore.MyOrientation == PieceQuad.Orientation.RotateLeft)
                return GetPieceTemplate(Box.Current.Size.Y);
            else
                return GetPieceTemplate(Box.Current.Size.X);
        }

        PieceQuad GetPieceTemplate(float width)
        {
            var tileset = Core.MyTileSet;
            if (tileset.ProvidesTemplates)
            {
                if (MyLevel == null)
                    return tileset.GetPieceTemplate(this, null);
                else
                    return tileset.GetPieceTemplate(this, Rnd);
            }

            if (tileset.PassableSides)
            {
                BlockCore.UseTopOnlyTexture = false;
                Box.TopOnly = true;
            }

            Box.TopOnly = true;
            return null;
            //return PieceQuad.Catwalk;
        }

        public override void ResetPieces()
        {
            if (MyDraw == null) return;

            MyDraw.Init(this, GetPieceTemplate(), Invert);

            MyDraw.MyPieces.Center.Playing = false;
        }

        public void Init(Vector2 center, Vector2 size, TileSet tile)
        {
            Core.Data.Position = Core.StartData.Position = center;
            Core.MyTileSet = tile;
            Tools.Assert(Core.MyTileSet != null);

            if (tile.FixedWidths)
            {
                if (BlockCore.Ceiling)
                    tile.Ceilings.SnapWidthUp(ref size);
                else if (Box.TopOnly)
                    tile.Platforms.SnapWidthUp(ref size);
                else if (BlockCore.EndPiece && tile.EndBlock.Dict.Count > 0)
                    tile.EndBlock.SnapWidthUp(ref size);
                else if (BlockCore.StartPiece && tile.StartBlock.Dict.Count > 0)
                    tile.StartBlock.SnapWidthUp(ref size);
                else
                    tile.Pillars.SnapWidthUp(ref size);
                    
                //size.Y = 170;
                //Extend(Side.Bottom, Box.TR.Y - 170);
            }

            MyBox.Initialize(center, size);

            if (!Core.BoxesOnly)
                MyDraw.Init(this, GetPieceTemplate(), Invert);
                        
            Update();

            Box.Validate();
        }

        static float TopOnlyHeight = 60;

        public void CheckHeight()
        {
            Tools.Assert(Core.MyTileSet != null);

            if (BlockCore.DisableFlexibleHeight || !Core.MyTileSet.FlexibleHeight)
            {
                if (MyBox.Current.BL.Y > Core.MyLevel.MainCamera.BL.Y - 20)
                    MakeTopOnly();
            }
            else
            {
                if (Box.Current.Size.Y < TopOnlyHeight)
                    MakeTopOnly();
            }
        }

        public void MakeTopOnly()
        {
            Tools.Assert(Core.MyTileSet != null);

            Box.TopOnly = true;
            Extend(Side.Bottom, Box.Current.TR.Y - TopOnlyHeight);
            Update();
        }

        public override void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);

            Update();
        }

        public override void Reset(bool BoxesOnly)
        {
            if (Core.AlwaysBoxesOnly)
                BoxesOnly = true;
            else
                BlockCore.BoxesOnly = BoxesOnly;

            if (!Core.BoxesOnly && !Core.VisualResettedOnce)
                ResetPieces();

            Active = true;

            BlockCore.Data = BlockCore.StartData;

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();

            Update();

            if (!Core.BoxesOnly)
                Core.VisualResettedOnce = true;
        }

        public override void PhsxStep()
        {
            int Padding = 250;
            if (MyLevel.PlayMode != 0) Padding = -150;

            Active = Core.Active = true;
            Vector2 BL = MyBox.Current.BL;
            if (MyBox.Current.BL.X > BlockCore.MyLevel.MainCamera.TR.X + Padding || MyBox.Current.BL.Y > BlockCore.MyLevel.MainCamera.TR.Y + 500)//+ 1250)
                Active = Core.Active = false;
            Vector2 TR = MyBox.Current.TR;
            if (MyBox.Current.TR.X < BlockCore.MyLevel.MainCamera.BL.X - Padding || MyBox.Current.TR.Y < BlockCore.MyLevel.MainCamera.BL.Y - 250)//- 500)
                Active = Core.Active = false;
        }

        public override void PhsxStep2()
        {
            if (Moved)
                MyBox.SwapToCurrent();

            if (!Active) return;
        }


        public void Update()
        {
            MyDraw.Update();
        }

        public void MoveTo(Vector2 Pos)
        {
            Core.Data.Position = Pos;
            MyBox.Target.Center = Pos;

            Box.SetTarget(Box.Target.Center, Box.Target.Size);
            Moved = true;
        }

        public override void Extend(Side side, float pos)
        {
            Tools.Assert(Core.MyTileSet != null);

            MyBox.Invalidated = true;

            MyBox.Extend(side, pos);

            Update();
            MyBox.Validate();

            if (!Core.BoxesOnly)
                MyDraw.Init(this, GetPieceTemplate(), Invert);

            BlockCore.StartData.Position = MyBox.Current.Center;

            if (!Core.BoxesOnly)
                ResetPieces();
        }

        public override void Draw()
        {
            if (!Active) return;
            if (!Core.Active) return;

            Update();

            if (Tools.DrawBoxes)
                MyBox.Draw(Tools.QDrawer, Color.Olive, 15);

            if (BlockCore.BoxesOnly) return;

            if (Tools.DrawGraphics && Core.Show)
            {
                if (Core.MyTileSet != TileSets.None)
                {
                    //if (BlockCore.Ceiling)
                        MyDraw.Draw();
                    //Tools.QDrawer.Flush();
                }

                if (Core.Encased)
                {
                    MyBox.DrawFilled(Tools.QDrawer, new Color(100, 100, 200, 100));
                    MyBox.Draw(Tools.QDrawer, new Color(120, 120, 240, 150), 18, true);
                }

                BlockCore.Draw();
            }
        }

        public override void Clone(ObjectBase A)
        {
            NormalBlock BlockA = A as NormalBlock;
            BlockCore.Clone(A.Core);

            if (BlockA == null) return;

            Box.TopOnly = BlockA.Box.TopOnly;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size, A.Core.MyTileSet);

            if (MyDraw != null && BlockA.MyDraw != null)
                MyDraw.Clone(BlockA.MyDraw);
        }
        
        public override void Write(BinaryWriter writer)
        {
            BlockCore.Write(writer);

            Box.Write(writer);            
        }
        public override void Read(BinaryReader reader)
        {
            BlockCore.Read(reader);

            Box.Read(reader);
            ResetPieces();
        }

        public override bool PreDecision(Bob bob)
        {
            if (BlockCore.Ceiling)
            {
                if (bob.Core.Data.Position.X > Box.Current.BL.X - 100 &&
                    bob.Core.Data.Position.X < Box.Current.TR.X + 100)
                {
                    float NewBottom = Box.Current.BL.Y;

                    // If ceiling has a left neighbor make sure we aren't too close to it
                    if (BlockCore.TopLeftNeighbor != null)
                    {
                        if (NewBottom > BlockCore.TopLeftNeighbor.Box.Current.BL.Y - 100)
                            NewBottom = Math.Max(NewBottom, BlockCore.TopLeftNeighbor.Box.Current.BL.Y + 120);
                    }
                    Extend(Side.Bottom,
                        Math.Max(NewBottom,
                                 Math.Max(bob.Box.Target.TR.Y, bob.Box.Current.TR.Y)
                                    + bob.CeilingParams.BufferSize.GetVal(bob.Core.Data.Position)));

                    if (Box.Current.Size.Y < 170 ||
                        Box.Current.BL.Y > bob.Core.MyLevel.MainCamera.TR.Y - 75)
                    {
                        bob.DeleteObj(this);
                    }
                }

                return true;
            }

            return false;
        }

        public override bool PostCollidePreDecision(Bob bob)
        {
            if (BlockCore.Ceiling)
            {
                Extend(Side.Bottom, Math.Max(Box.Current.BL.Y, Math.Max(bob.Box.Target.TR.Y, bob.Box.Current.TR.Y) + bob.CeilingParams.BufferSize.GetVal(bob.Core.Data.Position)));
                return true;
            }

            return false;
        }

        public override void PostCollideDecision(Bob bob, ref ColType Col, ref bool Overlap, ref bool Delete)
        {
            Block_PostCollideDecision(this as BlockBase, bob, ref Col, ref Overlap, ref Delete);
            base.PostCollideDecision(bob, ref Col, ref Overlap, ref Delete);
        }

        public static void Block_PostCollideDecision(BlockBase block, Bob bob, ref ColType Col, ref bool Overlap, ref bool Delete)
        {
            bool MakeTopOnly = false;

            if (!block.Core.GenData.NoMakingTopOnly)
            {
                // If we interact with the block in any way besides landing on top of it, make it top only
                if ((Col == ColType.Bottom || Overlap) && Col != ColType.Top) MakeTopOnly = true;
                if (Col == ColType.Left || Col == ColType.Right) MakeTopOnly = true;

                //// Note if we use the left or right side of the block
                //if ((Col == ColType.Left || Col == ColType.Right) && Col != ColType.Top)
                //{
                //    if (bob.Box.Current.TR.Y < block.Box.Current.TR.Y)
                //        MakeTopOnly = true;
                //    else
                //        MakeTopOnly = true;
                //}

                // If we've used something besides the top of the block already,
                // or this tileset doesn't allow for top only blocks,
                // then make sure we don't make the block top only.
                if (block.BlockCore.NonTopUsed || !(block is NormalBlock) || !block.Info.AllowTopOnlyBlocks)
                {
                    if (MakeTopOnly)
                    {
                        MakeTopOnly = false;
                        Delete = true;
                    }
                }

                // If we are trying to make a block be top only that can't be, delete it
                if (MakeTopOnly && block.BlockCore.DeleteIfTopOnly)
                {
                    if (block.Core.GenData.Used)
                        MakeTopOnly = Delete = false;
                    else
                        Delete = true;
                }

                // If we have decided to make the block top only, actually do so
                if (MakeTopOnly)
                {
                    block.Extend(Side.Bottom, Math.Max(block.Box.Current.BL.Y, Math.Max(bob.Box.Target.TR.Y, bob.Box.Current.TR.Y) + bob.CeilingParams.BufferSize.GetVal(bob.Core.Data.Position)));
                    ((NormalBlock)block).CheckHeight();
                    if (Col != ColType.Top)
                        Col = ColType.NoCol;
                }
            }
        }

        public override void PostInteractWith(Bob bob, ref ColType Col, ref bool Overlap)
        {
            base.PostInteractWith(bob, ref Col, ref Overlap);

            BlockBase block = (BlockBase)this;

            // Draw block upside down if Bob used it upside down.
            if (Col == ColType.Bottom && bob.MyPhsx.Gravity < 0)
                BlockCore.CeilingDraw = true;

            // Normal blocks delete surrounding blocks when stamped as used
            if (block.Core.GenData.DeleteSurroundingOnUse && block is NormalBlock)
                foreach (BlockBase nblock in Core.MyLevel.Blocks)
                {
                    NormalBlock Normal = nblock as NormalBlock;
                    if (null != Normal && !Normal.Core.MarkedForDeletion && !Normal.Core.GenData.AlwaysUse)
                        if (!Normal.Core.GenData.Used &&
                            Math.Abs(Normal.Box.Current.TR.Y - block.Box.TR.Y) < 15 &&
                            !(Normal.Box.Current.TR.X < block.Box.Current.BL.X - 350 || Normal.Box.Current.BL.X > block.Box.Current.TR.X + 350))
                        {
                            bob.DeleteObj(Normal);
                            Normal.IsActive = false;
                        }
                }
        }

        public override void PostKeep(Bob bob, ref ColType Col, ref bool Overlap)
        {
            base.PostKeep(bob, ref Col, ref Overlap);

            BlockBase block = (NormalBlock)this;

            if (!block.Core.GenData.NoBottomShift)
            {
                // Shift bottom of block if necessary
                if (block is NormalBlock && !block.BlockCore.DeleteIfTopOnly)
                {
                    float NewBottom = Math.Max(block.Box.Current.BL.Y,
                                               Math.Max(Box.Target.TR.Y, Box.Current.TR.Y) + bob.CeilingParams.BufferSize.GetVal(Core.Data.Position));

                    if ((Col == ColType.Bottom || Overlap) && Col != ColType.Top &&
                        !block.BlockCore.NonTopUsed)
                    {
                        block.Extend(Side.Bottom, NewBottom);
                        ((NormalBlock)block).CheckHeight();
                    }
                }
            }
        }
    }
}
