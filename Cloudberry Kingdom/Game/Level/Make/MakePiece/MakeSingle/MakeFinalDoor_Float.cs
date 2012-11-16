using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;




namespace CloudberryKingdom
{
    public class MakeFinalDoor_Float : MakeThing
    {
        Level MyLevel;

        /// <summary>
        /// The block on which the final door rests on.
        /// </summary>
        public BlockBase FinalBlock;

        /// <summary>
        /// The position of the final door.
        /// </summary>
        public Vector2 FinalPos;

        public MakeFinalDoor_Float(Level level)
        {
            MyLevel = level;
        }

        public override void Phase1()
        {
            base.Phase1();
        }

        public override void Phase2()
        {
            base.Phase2();

            Vector2 Pos = new Vector2(MyLevel.MaxRight + 90, MyLevel.MainCamera.BL.Y + Level.SafetyNetHeight + 65);

            // Cut computer run short once the computer reaches the door.
            int NewLastStep = MyLevel.LastStep;
            float MinDist = 100000;
            for (int j = 0; j < MyLevel.LastStep; j++)
            {
                NewLastStep = j;
                for (int i = 0; i < MyLevel.CurPiece.NumBobs; i++)                
                {
                    Vector2 BobPos = MyLevel.CurPiece.Recording[i].AutoLocs[j];
                    float Dist = Math.Abs(BobPos.X - Pos.X);

                    if (Dist < MinDist)
                    {
                        MinDist = Dist;
                        FinalPos = BobPos;
                    }
                }
            }

            MyLevel.LastStep = NewLastStep;

            // New style end blocks
            if (MyLevel.MyTileSet.FixedWidths)
            {
                // Create the block
                var block = FinalBlock = (NormalBlock)MyLevel.Recycle.GetObject(ObjectType.NormalBlock, true);

                block.BlockCore.EndPiece = true;
                ((NormalBlock)FinalBlock).Init(FinalPos + new Vector2(-130, -600), new Vector2(400, 400), MyLevel.MyTileSetInfo);
                
                block.Core.DrawLayer = 0;
                block.Core.Real = false;
            }
            // Old style end blocks
            else
            {
                // Create a dummy block
                int width = 400;
                FinalBlock = (NormalBlock)MyLevel.Recycle.GetObject(ObjectType.NormalBlock, true);
                ((NormalBlock)FinalBlock).Init(FinalPos + new Vector2(130, 0), new Vector2(width), MyLevel.MyTileSetInfo);
                FinalBlock.Core.MyTileSet = MyLevel.MyTileSet;
            }
        }

        public override void Phase3()
        {
            base.Phase3();

            Door door;

            // New style end blocks
            if (MyLevel.MyTileSet.FixedWidths)
            {
                door = MyLevel.PlaceDoorOnBlock(FinalPos, FinalBlock, false);
                MyLevel.AddBlock(FinalBlock);
                FinalBlock.Active = false;
                door.Mirror = true;
            }
            // Old style end blocks
            else
            {
                // Add door
                door = MyLevel.PlaceDoorOnBlock_Unlayered(FinalPos, FinalBlock, true);
                Tools.MoveTo(door, FinalPos);
            }

            SetFinalDoor(door, MyLevel, FinalPos);

            // New style end blocks
            if (MyLevel.MyTileSet.FixedWidths)
            {
            }

            // Push lava down
            MyLevel.PushLava(FinalBlock.Box.Target.TR.Y - 60);

            // Add exit sign
            if (MyLevel.Info.Doors.ShowSign)
            {
                Sign sign = new Sign(false, MyLevel);
                sign.PlaceAt(door.GetTop());
                MyLevel.AddObject(sign);
            }

            // Cleanup
            FinalBlock = null;
        }

        public static void AttachDoorAction(ILevelConnector door)
        {
            if (Tools.WorldMap != null)
            {
                StringWorldGameData stringworld = Tools.WorldMap as StringWorldGameData;
                if (stringworld != null)
                {
                    door.OnOpen = new EOL_StringWorldDoorActionProxy(stringworld);
                    door.OnEnter = new EOL_StringWorldDoorEndActionProxy(stringworld);
                }
            }
        }

        public static void SetFinalDoor(Door door, Level level, Vector2 FinalPos)
        {
            door.Core.EditorCode1 = LevelConnector.EndOfLevelCode;

            // Attach an action to the door
            AttachDoorAction(door);

            // Mod CameraZone
            CameraZone camzone = (CameraZone)Tools.Find(level.Objects, FindCamZoneLambda.FindCamZoneLambda_Static);

            camzone.End.X = FinalPos.X - level.MainCamera.GetWidth() / 2 + 420;

            if (level.PieceSeed.ZoomType == LevelZoom.Big)
                camzone.End.X -= 150;
        }
    }
}
