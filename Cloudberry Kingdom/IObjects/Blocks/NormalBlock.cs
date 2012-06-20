using System.IO;
using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Blocks
{
    public delegate void BlockExtendCallback(NormalBlock block);
    public class NormalBlock : BlockBase
    {
        public NormalBlockDraw MyDraw;

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

            if (tileset == TileSets.Castle)
                return GetPieceTemplate_Inside1(width);
            else if (tileset == TileSets.Dungeon)
                return GetPieceTemplate_Inside2(width);
            else if (tileset == TileSets.Island)
                return GetPieceTemplate_Island(width);
            else if (tileset == TileSets.Dark)
                return GetPieceTemplate_Dark(width);
            else if (tileset == TileSets.Rain || tileset == TileSets.Terrace)
                return GetPieceTemplate_Outside(width);
            else if (tileset == TileSets.DarkTerrace)
                return GetPieceTemplate_Outside(width);
            else if (tileset == TileSets.OutsideGrass)
                return GetPieceTemplate_OutsideGrass(width);
            else if (tileset == TileSets.TileBlock)
                return GetPieceTemplate_TileBlock(width);
            else if (tileset == TileSets.CastlePiece)
                return GetPieceTemplate_Castle(false, width);
            else if (tileset == TileSets.CastlePiece2)
                return GetPieceTemplate_Castle(true, width);
            else if (tileset == TileSets.Catwalk)
            {
                Box.TopOnly = true;
                return PieceQuad.Catwalk;
            }
            else if (tileset == TileSets.Cement)
                return GetPieceTemplate_Cement(width);
            else
                return GetPieceTemplate_Inside2(width);
        }

        PieceQuad GetPieceTemplate_Castle(bool Shift, float width)
        {
            PieceQuad Template = null;
            if (Box.TopOnly && BlockCore.UseTopOnlyTexture)
            {
                if (Shift) Template = PieceQuad.Castle2;
                else Template = PieceQuad.Castle;
            }
            else
            {
                if (Shift) Template = PieceQuad.Castle2;
                else Template = PieceQuad.Castle;
            }

            return Template;
        }

        PieceQuad GetPieceTemplate_Outside(float width)
        {
            // Check to see if we should make this a pillar, a floater, or a wall
            PieceQuad Template = null;
            if (Box.TopOnly && BlockCore.UseTopOnlyTexture)
            {
                Template = PieceQuad.Inside2_Thin;
            }
            else
            {
                if (width < 60)
                    Template = PieceQuad.Outside_Smallest;
                else if (width < 90)
                    Template = PieceQuad.Outside_Smaller;
                else if (width < 125)
                    Template = PieceQuad.Outside_Small;
                else if (width < 370)
                    Template = PieceQuad.Outside_Medium;
                else if (width < 600)
                    Template = PieceQuad.Outside_Large;
                else
                    Template = PieceQuad.Outside_XLarge;
                //OutsideBlock;                
            }

            return Template;
        }

        PieceQuad GetPieceTemplate_OutsideGrass(float width)
        {
            // Check to see if we should make this a pillar, a floater, or a wall
            PieceQuad Template = null;
            Template = PieceQuad.OutsideBlock;
            
            return Template;
        }

        PieceQuad GetPieceTemplate_Cement(float width)
        {
            // Check to see if we should make this a pillar, a floater, or a wall
            PieceQuad Template = null;
            Template = PieceQuad.Cement;

            return Template;
        }

        PieceQuad GetPieceTemplate_TileBlock(float width)
        {
            // Check to see if we should make this a pillar, a floater, or a wall
            PieceQuad Template = null;
            Template = PieceQuad.TileBlock;

            return Template;
        }

        PieceQuad GetPieceTemplate_Inside1(float width)
        {
            // Check to see if we should make this a pillar, a floater, or a wall
            PieceQuad Template = null;
            if (Box.TopOnly && BlockCore.UseTopOnlyTexture)
            {
                if (width < 60)
                    Template = PieceQuad.Floating_Small;
                else if (width < 100)
                    Template = PieceQuad.Floating_Medium;
                else if (width < 180)
                    Template = PieceQuad.Floating_Large;
                else
                    Template = PieceQuad.Floating_Xlarge;
            }
            else
            {
                if (width < 270)
                {
                    if (width < 60)
                        Template = PieceQuad.BrickPillar_Small;
                    else if (width < 120)
                        Template = PieceQuad.BrickPillar_Medium;
                    else if (width < 270)
                        Template = PieceQuad.BrickPillar_Large;
                    else if (width < 450)
                        Template = PieceQuad.BrickPillar_LargePlus;
                    else
                        Template = PieceQuad.BrickPillar_Xlarge;
                }
                else
                {
                    Template = PieceQuad.BrickWall;
                }
            }

            return Template;
        }

        PieceQuad GetPieceTemplate_Inside2(float width)
        {
            // Check to see if we should make this a pillar, a floater, or a wall
            PieceQuad Template = null;
            if (Box.TopOnly && BlockCore.UseTopOnlyTexture)
            {
                Template = PieceQuad.Inside2_Thin;
            }
            else
            {
                if (width < 60)
                    Template = PieceQuad.Inside2_Smallest;
                else if (width < 150)
                    Template = PieceQuad.Inside2_Smaller;
                else if (width < 250)
                    Template = PieceQuad.Inside2_Small;
                else if (width < 350)
                    Template = PieceQuad.Inside2_Medium;
                else if (width < 500)
                    Template = PieceQuad.Inside2_Large;
                else
                    Template = PieceQuad.Inside2_XLarge;

                if (width < 86)
                    Template = PieceQuad.Inside2_Pillar_Smallest;
                else if (width < 128)
                    Template = PieceQuad.Inside2_Pillar_Smaller;
                else if (width < 230)
                    Template = PieceQuad.Inside2_Pillar_Small;
                else if (width < 444)
                    Template = PieceQuad.Inside2_Pillar_Medium;
                else if (width < 800)
                    Template = PieceQuad.Inside2_Pillar_Large;
                else
                    Template = PieceQuad.Inside2_Pillar_XLarge;                
            }

            return Template;
        }

        PieceQuad GetPieceTemplate_Dark(float width)
        {
            // Check to see if we should make this a pillar, a floater, or a wall
            PieceQuad Template = null;
            if (Box.TopOnly && BlockCore.UseTopOnlyTexture)
                Template = PieceQuad.Inside2_Thin;
            else
                Template = PieceQuad.DarkPillars.Get(width);

            return Template;
        }

        PieceQuad GetPieceTemplate_Island(float width)
        {
            Box.TopOnly = true;
            PieceQuad Template = PieceQuad.Islands.Get(width);

            return Template;
        }

        public void ResetPieces()
        {
            MyDraw.Init(this, GetPieceTemplate());
        }

        public void Init(Vector2 center, Vector2 size, TileSet tile)
        {
            Core.Data.Position = Core.StartData.Position = center;
            Core.MyTileSet = tile;
            Tools.Assert(Core.MyTileSet != null);

            if (tile.FixedWidths)
            {
                if (Box.TopOnly)
                    tile.Platforms.SnapWidthUp(ref size);
                else
                    tile.Pillars.SnapWidthUp(ref size);
                    
                //size.Y = 170;
                //Extend(Side.Bottom, Box.TR.Y - 170);
            }

            MyBox.Initialize(center, size);

            if (!Core.BoxesOnly)
                MyDraw.Init(this, GetPieceTemplate());
                        
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

            if (Core.MyTileSet == TileSets.Cement)
                Core.MyTileSet = TileSets.Catwalk;

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
            Active = Core.Active = true;
            Vector2 BL = MyBox.Current.BL;
            if (MyBox.Current.BL.X > BlockCore.MyLevel.MainCamera.TR.X || MyBox.Current.BL.Y > BlockCore.MyLevel.MainCamera.TR.Y + 500)//+ 1250)
                Active = Core.Active = false;
            Vector2 TR = MyBox.Current.TR;
            if (MyBox.Current.TR.X < BlockCore.MyLevel.MainCamera.BL.X || MyBox.Current.TR.Y < BlockCore.MyLevel.MainCamera.BL.Y - 250)//- 500)
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
            if (Core.MyTileSet == TileSets.Island)
                Box.TopOnly = true;

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

            // This is a hack to make sure dungeon blocks don't repeat vertically
            if (Core.MyTileSet.DungeonLike && !BlockCore.Ceiling && MyBox.Current.Size.Y > 2000)
                Extend(Side.Bottom, MyBox.Current.TR.Y - 1950);

            if (!Core.BoxesOnly)
                MyDraw.Init(this, GetPieceTemplate());

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
                    MyDraw.Draw();
                    Tools.QDrawer.Flush();
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
                // make sure we don't make the block top only
                if (block.BlockCore.NonTopUsed || !(block is NormalBlock))
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
