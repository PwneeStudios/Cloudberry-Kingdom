using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom.Levels
{
    public partial class Level
    {
        public bool MakeBig(int Length, int StartPhsxStep, int ReturnEarly, MakeData makeData)
        {
            CurMakeData = makeData;
            InitMakeData(CurMakeData);

            // Calculate the style parameters
            CurMakeData.PieceSeed.Style.CalcGenParams(CurMakeData.PieceSeed, this);

            // Move camera
            MainCamera.Data.Position = CurMakeData.CamStartPos;
            MainCamera.Update();

            // New bobs
            Bob[] Computers = CurMakeData.MakeBobs(this);

            // New level piece
            LevelPiece Piece = CurPiece = CurMakeData.MakeLevelPiece(this, Computers, Length, StartPhsxStep);

            // Camera Zone
            CameraZone CamZone = MakeCameraZone();
            Sleep();

            // Set the campera start position
            if (CurMakeData.InitialCamZone)
                CurPiece.CamStartPos = CurMakeData.CamStartPos = CamZone.Start;
            else
                CurPiece.CamStartPos = CurMakeData.CamStartPos;


            Vector2 BL_Bound, TR_Bound;

            BL_Bound = MainCamera.BL;
            TR_Bound = MainCamera.TR;
        
            FillBL = BL_Bound;


            // Initial platform

            // Final platform

            // Safety nets


            // Pre Fill #1
            foreach (AutoGen gen in Generators.PreFill_1_Gens)
            {
                gen.PreFill_1(this, BL_Bound, TR_Bound);
                Sleep();
            }

            // Change sparsity multiplier
            if (CurMakeData.SparsityMultiplier == 1)
                CurMakeData.SparsityMultiplier = CurMakeData.GenData.Get(DifficultyParam.FillSparsity) / 100f;

            Stage1RndFill(BL_Bound, TR_Bound, BL_Bound, 1);

            PlayMode = 2;
            RecordPosition = true;
            ResetAll(true);

            // Set special Bob parameters
            MySourceGame.SetAdditionalBobParameters(Computers);

            // Set bounds on movement
            CurMakeData.TRBobMoveZone = TR_Bound;
            CurMakeData.BLBobMoveZone = BL_Bound;
            if (ReturnEarly == 1) return false;

            // Stage 1 Run through
            int OneFinishedCount = 0;
            while (CurPhsxStep - Bobs[0].IndexOffset < CurPiece.PieceLength)
            {
/*                // End if all bobs have arrived
                if (!Bobs.Any(delegate(Bob bob) { return bob.Core.Data.Position.X < MaxRight + EndBuffer; }))
                    OneFinishedCount += 8;

                // End after first computer arrives at end
                if (Bobs.Any(delegate(Bob bob) { return bob.Core.Data.Position.X > MaxRight + EndBuffer - 100; }))
                    OneFinishedCount++;

                if (OneFinishedCount > 200)
                    break;
                */
                PhsxStep(true);
                foreach (AutoGen gen in Generators.ActiveFill_1_Gens)
                    gen.ActiveFill_1(this, BL_Bound, TR_Bound);
            }
            int LastStep = CurPhsxStep;

            Console.WriteLine("Stage 1 finished at " + LastStep.ToString());

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
                if (!obj.Core.GenData.Used && obj.Core.GenData.RemoveIfUnused)
                    Recycle.CollectObject(obj);
            CleanObjectList();
            Sleep();

            // Remove unused blocks
            foreach (BlockBase _block in Blocks)
                if (!_block.Core.GenData.Used && _block.Core.GenData.RemoveIfUnused)
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
            Console.WriteLine("Stage 2 finished at " + CurPhsxStep.ToString());

            OverlapCleanup();
            CleanAllObjectLists();
            Sleep();

            Cleanup(Objects.FindAll(delegate(ObjectBase obj) { return obj.Core.GenData.LimitGeneralDensity; }), delegate(Vector2 pos)
            {
                float dist = CurMakeData.GenData.Get(DifficultyParam.GeneralMinDist, pos);
                return new Vector2(dist, dist);
            }, true, BL_Bound, TR_Bound);
            Sleep();

            Cleanup(ObjectType.Coin, new Vector2(180, 180), BL_Bound, TR_Bound + new Vector2(500, 0));
            Sleep();


            foreach (AutoGen gen in Generators.Gens)
                gen.Cleanup_2(this, BL_Bound, TR_Bound);

            CleanAllObjectLists();

            //Tools.Recycle.Empty();

            return false;
        }     
    }
}
