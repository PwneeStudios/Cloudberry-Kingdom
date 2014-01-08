using System;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.InGameObjects;

namespace CloudberryKingdom.Levels
{
    public partial class Level
    {
        /// <summary>
        /// Makes a CameraZone for an up or down level.
        /// </summary>
        /// <param name="Up">Whether the level is an up level or a down level</param>
        /// <param name="Height">The height of the level, always positive.</param>
        /// <returns></returns>
        public CameraZone MakeVerticalCameraZone(LevelGeometry Geometry, float Height)
        {
            Vector2 Size = CoreMath.Abs(CurMakeData.PieceSeed.End - CurMakeData.PieceSeed.Start) / 2;
            Size.Y += 900;
            Size = Vector2Extension.Max(Size, MainCamera.GetSize());

            CameraZone CamZone = (CameraZone)MySourceGame.Recycle.GetObject(ObjectType.CameraZone, false);
            CamZone.Init((CurMakeData.PieceSeed.Start + CurMakeData.PieceSeed.End) / 2, Size);
            CamZone.Start = CurMakeData.PieceSeed.Start;
            //CamZone.End = CurMakeData.PieceSeed.End;
            if (Geometry == LevelGeometry.Up)
            {
                CamZone.End = CamZone.Start + new Vector2(0, Height);
                CamZone.CameraType = Camera.PhsxType.SideLevel_Up;
            }
            else if (Geometry == LevelGeometry.Down)
            {
                CamZone.End = CamZone.Start - new Vector2(0, Height);
                CamZone.CameraType = Camera.PhsxType.SideLevel_Down;

                CamZone.Start.Y += 125;
            }
            Vector2 Tangent = CamZone.End - CamZone.Start;

            //if (CurMakeData.InitialCamZone)
            //  CamZone.Start.Y += MainCamera.GetHeight() / 2 - 300;

            AddObject(CamZone);

            return CamZone;
        }

        /// <summary>
        /// Create the initial platforms the players start on.
        /// </summary>
        public float MakeVerticalInitialPlats(StyleData Style)
        {
            Vector2 size = new Vector2(350, 2250);
            Vector2 pos = CurPiece.StartData[0].Position + new Vector2(0, 200);
            NormalBlock block = null, startblock = null;

            // Find the closest block to pos on first row
            startblock = (NormalBlock)Tools.ArgMin(
                Blocks.FindAll(match => match.CoreData == "FirstRow"),
                element => (element.CoreData.Data.Position - pos).LengthSquared());

            switch (Style.MyInitialPlatsType)
            {
                case StyleData.InitialPlatsType.Up_TiledFloor:
                case StyleData.InitialPlatsType.Door:
                    block = (NormalBlock)Recycle.GetObject(ObjectType.NormalBlock, false);

                    // Tiled bottom
                    if (Style.MyInitialPlatsType == StyleData.InitialPlatsType.Up_TiledFloor)
                    {
                            Blocks.FindAll(match => match.CoreData == "FirstRow").ForEach(
                                element => element.CollectSelf());
                            FinalCamZone.End.Y += 400;
                            FinalCamZone.Start.Y -= 100;

                        block.CoreData.DrawLayer = 2;
                        block.Init(startblock.CoreData.Data.Position, Vector2.One, MyTileSetInfo);
                        block.Stretch(Side.Right, 2000);
                        block.Stretch(Side.Left, -2000);
                        block.Stretch(Side.Bottom, -800);
                        block.Extend(Side.Top, startblock.Box.Current.TR.Y + 1);
                        block.CoreData.EditorCode1 = "Tiled bottom";
                    }
                    else
                        block.Clone(startblock);

                    block.BlockCore.BlobsOnTop = false;
                    block.StampAsUsed(0);
                    block.CoreData.GenData.RemoveIfUnused = false;

                    AddBlock(block);

                    Door door = PlaceDoorOnBlock(pos, block, false);
                    door.CoreData.EditorCode1 = LevelConnector.StartOfLevelCode;

                    Level.SpreadStartPositions(CurPiece, CurMakeData, door.CoreData.Data.Position, new Vector2(50, 0));

                    // Make sure block is used
                    block.StampAsUsed(0);

                    return 0;

                default:
                    return 0;
            }
        }


        public bool MakeVertical(int Length, float Height, int StartPhsxStep, int ReturnEarly, MakeData makeData)
        {
            CurMakeData = makeData;
            InitMakeData(CurMakeData);
            Style.ModNormalBlockWeight = .15f;

            VerticalData VStyle = (VerticalData)CurMakeData.PieceSeed.Style;
            LevelGeometry Geometry = CurMakeData.PieceSeed.GeometryType;

            // Shift start position up for down levels
            if (Geometry == LevelGeometry.Down)
            {
                CurMakeData.PieceSeed.Start.Y += Height;
                CurMakeData.PieceSeed.End.Y += Height;
                CurMakeData.CamStartPos = CurMakeData.PieceSeed.Start;
            }
            else
            {
                CurMakeData.PieceSeed.Start.Y += Height;
                CurMakeData.PieceSeed.End.Y += Height;
                CurMakeData.CamStartPos = CurMakeData.PieceSeed.Start;
            }

            // Calculate the style parameters
            CurMakeData.PieceSeed.Style.CalcGenParams(CurMakeData.PieceSeed, this);

            // Move camera
            MainCamera.Data.Position = CurMakeData.CamStartPos;
            MainCamera.Update();

            // New bobs
            Bob[] Computers = CurMakeData.MakeBobs(this);

            // New level piece
            LevelPiece Piece = CurPiece = CurMakeData.MakeLevelPiece(this, Computers, Length, StartPhsxStep);

            // Start data
            Vector2 StartPos = MainCamera.Pos + new Vector2(0, -400);
            if (Geometry == LevelGeometry.Down) StartPos.Y += 1200;
            for (int i = 0; i < Piece.StartData.Length; i++)
                Piece.StartData[i].Position = StartPos;

            // Camera Zone
            CameraZone CamZone = MakeVerticalCameraZone(Geometry, Height);
            FinalCamZone = CamZone;
            Sleep();

            // Set the camera start position
            if (CurMakeData.InitialCamZone)
                CurPiece.CamStartPos = CurMakeData.CamStartPos = CamZone.Start;
            else
                CurPiece.CamStartPos = CurMakeData.CamStartPos;


            Vector2 BL_Bound, TR_Bound;

            if (Geometry == LevelGeometry.Up)
            {
                BL_Bound = MainCamera.BL;
                TR_Bound = new Vector2(MainCamera.TR.X, MainCamera.TR.Y + Height);
            }
            else
            {
                BL_Bound = new Vector2(MainCamera.BL.X, MainCamera.BL.Y - Height);
                TR_Bound = MainCamera.TR;
                //BL_Bound.Y += 100;
            }

            FillBL = BL_Bound;

            // Safety nets
            Vector2 Size, Step;
            Size = new Vector2(100, 50);
            Step = new Vector2(300, 390);

            string BottomRowTag = "", TopRowTag = "";
            if (Geometry == LevelGeometry.Up)
            {
                BottomRowTag = "FirstRow";
                TopRowTag = "LastRow";
            }
            else if (Geometry == LevelGeometry.Down)
            {
                BottomRowTag = "LastRow";
                TopRowTag = "FirstRow";
            }

            // Vary the spacing depending on how high the hero can jump
            float StepMultiplier = SetStepMultiplier(ref Size, ref Step);

            BL_Bound.Y += 200;
            TR_Bound.Y -= 200;
            Vector2 _pos = BL_Bound;
            // Make sure the net is centered
            _pos.X = MainCamera.Data.Position.X - Step.X * (int)(.5f * (TR_Bound.X - BL_Bound.X) / Step.X);
            for (; _pos.X < TR_Bound.X + 400; _pos.X += Step.X)
            {
                bool ShouldBreak = false;
                for (_pos.Y = BL_Bound.Y; ; )
                {
                    NormalBlock block = (NormalBlock)NormalBlock_AutoGen.Instance.CreateAt(this, _pos);

                    block.Init(_pos, Size, MyTileSetInfo);
                    block.MakeTopOnly();

                    // Door catwalks need to be moved forward
                    if (_pos.Y == BL_Bound.Y || _pos.Y + Step.Y >= TR_Bound.Y)
                        block.BlockCore.DrawLayer = 4;

                    bool IsBottom = _pos.Y == BL_Bound.Y;
                    bool IsTop = ShouldBreak;

                    if (Geometry == LevelGeometry.Up ||
                        Geometry == LevelGeometry.Down && IsBottom)
                    {
                        block.CoreData.GenData.DeleteSurroundingOnUse = false;
                        block.CoreData.GenData.AlwaysLandOn = true;
                    }

                    if (IsBottom) block.CoreData.EditorCode1 = BottomRowTag;
                    if (IsTop) block.CoreData.EditorCode1 = TopRowTag;
                    if (IsTop && Geometry == LevelGeometry.Up)
                    {
                        block.BlockCore.Virgin = true;
                        block.BlockCore.BlobsOnTop = false;
                    }

                    // Increment y
                    _pos.Y += Step.Y * StepMultiplier;
                    if (ShouldBreak) break;
                    float Top = TR_Bound.Y - 300;
                    if (_pos.Y > Top)
                    {
                        _pos.Y = Top;
                        ShouldBreak = true;
                    }
                }
            }

            // Set flag when a block on the last row is used.
            bool EndReached = false;
            foreach (BlockBase block in Blocks.FindAll(match => match.CoreData == "LastRow"))
                block.CoreData.GenData.OnUsed = () => EndReached = true;

            // Initial platform
            if (CurMakeData.InitialPlats && VStyle.MakeInitialPlats)
            {
                MakeVerticalInitialPlats(VStyle);
                Sleep();
            }

            // Final platform
            MakeThing MakeFinalPlat = null;
            if (CurMakeData.FinalPlats)
            {
                if (VStyle.MyFinalPlatsType == StyleData.FinalPlatsType.Door) MakeFinalPlat = new MakeFinalDoorVertical(this);
                if (VStyle.MyFinalPlatsType == StyleData.FinalPlatsType.DarkBottom) MakeFinalPlat = new MakeDarkBottom(this);
            }

            if (MakeFinalPlat != null) MakeFinalPlat.Phase1();


            // Pre Fill #1
            foreach (AutoGen gen in Generators.PreFill_1_Gens)
            {
                gen.PreFill_1(this, BL_Bound, TR_Bound);
                Sleep();
            }

            // Change sparsity multiplier
            if (CurMakeData.SparsityMultiplier == 1)
                CurMakeData.SparsityMultiplier = CurMakeData.GenData.Get(DifficultyParam.FillSparsity) / 100f;


            // Stage 1 fill
            NormalBlock_Parameters BlockParams = (NormalBlock_Parameters)VStyle.FindParams(NormalBlock_AutoGen.Instance);
            if (BlockParams.DoStage1Fill)
            {
                //Fill_BL = BL_Bound + new Vector2(400, 0);
                Fill_BL = BL_Bound + new Vector2(400, 650);
                Fill_TR = TR_Bound + new Vector2(-400, -450);
                
                Stage1RndFill(Fill_BL, Fill_TR, BL_Bound, .35f * CurMakeData.SparsityMultiplier);
            }
            //foreach (BlockBase block in Blocks)
            //{
            //    NormalBlock nblock = block as NormalBlock;
            //    if (null != nblock)
            //        nblock.MakeTopOnly();
            //}

            DEBUG("Pre stage 1, about to reset");

            PlayMode = 2;
            RecordPosition = true;
            ResetAll(true);

            //int t1 = System.Environment.TickCount;

            // Set special Bob parameters
            MySourceGame.SetAdditionalBobParameters(Computers);

            CurMakeData.TRBobMoveZone = TR_Bound;
            CurMakeData.BLBobMoveZone = BL_Bound;
            if (ReturnEarly == 1) return false;

            // Stage 1 Run through
            int OneFinishedCount = 0;
            while (CurPhsxStep - Bobs[0].IndexOffset < CurPiece.PieceLength)
            {
                if (EndReached) OneFinishedCount += 15;
                /*
                // End if all bobs have arrived
                if (Geometry == LevelGeometry.Up   && !Bobs.Any(bob => bob.Core.Data.Position.Y < TR_Bound.Y) ||
                    Geometry == LevelGeometry.Down && !Bobs.Any(bob => bob.Core.Data.Position.Y > BL_Bound.Y + 300))
                    OneFinishedCount += 8;

                // End after first computer arrives at end
                if (Geometry == LevelGeometry.Up   && Bobs.Any(bob => bob.Core.Data.Position.Y > TR_Bound.Y - 100) ||
                    Geometry == LevelGeometry.Down && Bobs.Any(bob => bob.Core.Data.Position.Y < BL_Bound.Y + 280))
                    OneFinishedCount++;
                */
                if (OneFinishedCount > 200)
                    break;

                PhsxStep(true);
                foreach (AutoGen gen in Generators.ActiveFill_1_Gens)
                    gen.ActiveFill_1(this, BL_Bound, TR_Bound);
            }
            int LastStep = CurPhsxStep;

            // Continue making Final Platform
            if (MakeFinalPlat != null) MakeFinalPlat.Phase2();

            // Update the level's par time
            CurPiece.Par = LastStep;
            Par += CurPiece.Par;

            // Cleanup
            foreach (AutoGen gen in Generators.Gens)
                gen.Cleanup_1(this, BL_Bound, TR_Bound);

            // Overlapping blocks
            if (CurMakeData.PieceSeed.Style.RemovedUnusedOverlappingBlocks)
                BlockOverlapCleanup();
            Sleep();

            // Remove unused objects
            foreach (ObjectBase obj in Objects)
                if (!obj.CoreData.GenData.Used && obj.CoreData.GenData.RemoveIfUnused)
                    Recycle.CollectObject(obj);
            CleanObjectList();
            Sleep();

            // Remove unused blocks
            foreach (BlockBase _block in Blocks)
                if (!_block.CoreData.GenData.Used && _block.CoreData.GenData.RemoveIfUnused)
                    Recycle.CollectObject(_block);
            CleanBlockList();
            CleanDrawLayers();
            Sleep();

            //CurPiece.PieceLength = CurPhsxStep - StartPhsxStep;
            CurPiece.PieceLength = LastStep - StartPhsxStep;

            // Pre Fill #2
            foreach (AutoGen gen in Generators.PreFill_2_Gens)
            {
                gen.PreFill_2(this, BL_Bound, TR_Bound);
                Sleep();
            }


            FinalizeBlocks();

            //int t3 = System.Environment.TickCount;

            PlayMode = 1;
            RecordPosition = false;
            ResetAll(true);
            Sleep();

            // Set special Bob parameters
            MySourceGame.SetAdditionalBobParameters(Computers);

            if (ReturnEarly == 2) return false;

            // Stage 2 Run through
            while (CurPhsxStep < LastStep)
            {
                PhsxStep(true);
            }
            //int t4 = System.Environment.TickCount;
            //Tools.Write("Stage 2 finished at {0}, Time = {1}", CurPhsxStep, t4 - t3);

            OverlapCleanup();
            CleanAllObjectLists();
            Sleep();

            Cleanup(Objects.FindAll(delegate(ObjectBase obj) { return obj.CoreData.GenData.LimitGeneralDensity; }), delegate(Vector2 pos)
            {
                float dist = CurMakeData.GenData.Get(DifficultyParam.GeneralMinDist, pos);
                return new Vector2(dist, dist);
            }, true, BL_Bound, TR_Bound);
            Sleep();

            Cleanup(ObjectType.Coin, delegate(Vector2 pos)
            {
                return new Vector2(180, 180);
            }, BL_Bound, TR_Bound + new Vector2(500, 0));
            Sleep();


            foreach (AutoGen gen in Generators.Gens)
                gen.Cleanup_2(this, BL_Bound, TR_Bound);

            CleanAllObjectLists();

            // Finish making Final Platform
            if (MakeFinalPlat != null) { MakeFinalPlat.Phase3(); MakeFinalPlat.Cleanup(); }

            if (Geometry == LevelGeometry.Up)
            {
                var back = MakePillarBack(FinalDoor.CoreData.Data.Position + new Vector2(0, -400), FinalDoor.CoreData.Data.Position + new Vector2(0, 2000));
                back.BlockCore.CeilingDraw = true;
                MakePillarBack(StartDoor.CoreData.Data.Position + new Vector2(0, 400), StartDoor.CoreData.Data.Position - new Vector2(0, 2000));
            }
            else
            {
                var back = MakePillarBack(StartDoor.CoreData.Data.Position + new Vector2(0, -400), StartDoor.CoreData.Data.Position + new Vector2(0, 2000));
                back.BlockCore.CeilingDraw = true;
                MakePillarBack(FinalDoor.CoreData.Data.Position + new Vector2(0, 400), FinalDoor.CoreData.Data.Position - new Vector2(0, 2000));
            }

            return false;
        }

        private float SetStepMultiplier(ref Vector2 Size, ref Vector2 Step)
        {
            float StepMultiplier = 1;
            if (DefaultHeroType is BobPhsxJetman)
                StepMultiplier = 3;
            else if (DefaultHeroType is BobPhsxDouble)
                StepMultiplier = 1.75f;
            else if (DefaultHeroType is BobPhsxSmall)
            {
                Size = new Vector2(100, 50);
                Step = new Vector2(240, 390);
                StepMultiplier = 1.5f;
            }
            else if (DefaultHeroType is BobPhsxBox)
                StepMultiplier = .7f;
            else if (DefaultHeroType is BobPhsxBig)
                StepMultiplier = .7f;
            else if (DefaultHeroType is BobPhsxMeat)
            {
                Size = new Vector2(100, 50);
                Step = new Vector2(240, 390);
                StepMultiplier = 1000;
            }
            return StepMultiplier;
        }

        private NormalBlock MakePillarBack(Vector2 p1, Vector2 p2)
        {
            NormalBlock doo = (NormalBlock)Recycle.GetObject(ObjectType.NormalBlock, true);
            doo.Init((p1 + p2) / 2, new Vector2(350, Math.Abs(p2.Y - p1.Y) / 2), MyTileSetInfo);

            AddBlock(doo);

            doo.BlockCore.Virgin = true;
            doo.BlockCore.RemoveOverlappingObjects = false;
            SetBackblockProperties(doo);

            return doo;
        }
    }
}
