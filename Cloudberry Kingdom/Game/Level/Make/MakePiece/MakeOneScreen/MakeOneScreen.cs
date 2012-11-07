using System;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.InGameObjects;

namespace CloudberryKingdom.Levels
{
    public partial class Level
    {
        class DoorLocker : Lambda_1<Door>
        {
            int Length;

            public DoorLocker(int Length)
            {
                this.Length = Length;
            }

            public void Apply(Door me)
            {
                if (me.Core.GetPhsxStep() > Length)
                    me.SetLock(false);
                else
                    me.SetLock(true);
            }
        }

        public bool MakeOneScreen(int Length, int ReturnEarly, MakeData makeData)
        {
            CurMakeData = makeData;
            InitMakeData(CurMakeData);

            OneScreenData Style = (OneScreenData)CurMakeData.PieceSeed.Style;

            // Calculate the style parameters
            CurMakeData.PieceSeed.Style.CalcGenParams(CurMakeData.PieceSeed, this);

            // Move camera
            MainCamera.Data.Position = CurMakeData.CamStartPos + Style.CamShift;
            MainCamera.Zoom *= Style.Zoom;
            MainCamera.Update();

            // New bobs
            Bob[] Computers = CurMakeData.MakeBobs(this);

            // New level piece
            LevelPiece Piece = CurPiece = CurMakeData.MakeLevelPiece(this, Computers, Length, StartPhsxStep);

            for (int i = 0; i < Piece.StartData.Length; i++)
                Piece.StartData[i].Position = MainCamera.Pos;


            // Camera Zone
            CameraZone CamZone = MakeCameraZone(new Vector2(2000, 2000));
            MainCamera.MyPhsxType = CamZone.CameraType = Camera.PhsxType.Fixed;
            Sleep();

            // Set the camera start position
            CamZone.Start = CurPiece.CamStartPos = CurMakeData.CamStartPos;
            CamZone.Zoom = Style.Zoom;

            Vector2 pos1 = MainCamera.Pos + new Vector2(-1000, -370);
            Vector2 pos2 = MainCamera.Pos + new Vector2(1000, -370);
            NormalBlock MainPlatform = null; Tools.Break();
            AddBlock(MainPlatform);

            // Start door
            Vector2 pos = new Vector2 (MainPlatform.Box.BL.X + 300, 0);
            Door door = PlaceDoorOnBlock(pos, MainPlatform, true, TileSets.None);

            door.MyBackblock.Core.MyTileSet = "castle";
            door.MyBackblock.Extend(Side.Right, pos.X + 350);
            door.MyBackblock.Extend(Side.Left, MainPlatform.Box.BL.X + 30);
            door.MyBackblock.Extend(Side.Bottom, MainPlatform.Box.BL.Y + 30);
            door.MyBackblock.Extend(Side.Top, MainPlatform.Box.TR.Y + 1000);

            Level.SpreadStartPositions(CurPiece, CurMakeData, door.Core.Data.Position, new Vector2(50, 0));

            // End door
            pos = new Vector2(MainPlatform.Box.TR.X - 300, 0);
            door = PlaceDoorOnBlock(pos, MainPlatform, true, TileSets.None);
            MakeFinalDoor.SetFinalDoor(door, this, pos);
            door.ExtraPhsx = new DoorLocker(Length);

            door.MyBackblock.Core.MyTileSet = "castle";
            door.MyBackblock.Extend(Side.Right, MainPlatform.Box.TR.X - 30);
            door.MyBackblock.Extend(Side.Left, pos.X - 350);
            door.MyBackblock.Extend(Side.Bottom, MainPlatform.Box.BL.Y + 30);
            door.MyBackblock.Extend(Side.Top, MainPlatform.Box.TR.Y + 1000);

            CurPiece.PieceLength = Length;

            // Pre Fill #2
            foreach (AutoGen gen in Generators.PreFill_2_Gens)
            {
                gen.PreFill_2(this, MainCamera.BL, MainCamera.TR);
                Sleep();
            }


            FinalizeBlocks();

            PlayMode = 1;
            NumModes = 1;
            RecordPosition = true;
            ResetAll(true);
            Sleep();

            // Set special Bob parameters
            MySourceGame.SetAdditionalBobParameters(Computers);

            if (ReturnEarly == 2) return false;

            // Stage 2 Run through
            while (CurPhsxStep < Length)
            {
                PhsxStep(true);
            }
            Console.WriteLine("Stage 2 finished at " + CurPhsxStep.ToString());
            RecordPosition = false;

            // Cleanup
            OverlapCleanup();
            CleanAllObjectLists();
            Sleep();

            foreach (AutoGen gen in Generators.Gens)
                gen.Cleanup_2(this, MainCamera.BL, MainCamera.TR);

            CleanAllObjectLists();

            //Tools.Recycle.Empty();

            return false;
        }
    }
}
