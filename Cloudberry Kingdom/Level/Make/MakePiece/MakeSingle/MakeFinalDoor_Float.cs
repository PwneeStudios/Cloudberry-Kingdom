using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;

namespace CloudberryKingdom.Levels
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

            // Create a dummy block
            int width = 400;
            FinalBlock = (NormalBlock)MyLevel.Recycle.GetObject(ObjectType.NormalBlock, true);
            ((NormalBlock)FinalBlock).Init(FinalPos + new Vector2(130, 0), new Vector2(width), MyLevel.MyTileSetInfo);
            FinalBlock.Core.MyTileSet = MyLevel.MyTileSet;
        }

        public override void Phase3()
        {
            base.Phase3();

            // Sky
            if (FinalBlock.Core.MyTileSet == TileSets.Island)
            {
                FinalPos.X += 130;
            }

            // Add door
            Door door = MyLevel.PlaceDoorOnBlock_Unlayered(FinalPos, FinalBlock, true);
            Tools.MoveTo(door, FinalPos);

            // Terrace-To-Castle
            if (MyLevel.Style.MyFinalDoorStyle == StyleData.FinalDoorStyle.TerraceToCastle)
            {
                MyLevel.MadeBackBlock.Core.MyTileSet = TileSets.CastlePiece2;
                MyLevel.MadeBackBlock.Stretch(Side.Right, 1500);
                MyLevel.MadeBackBlock.Stretch(Side.Left, -50);
                MyLevel.MadeBackBlock.Extend(Side.Bottom, -2200);
            }

            SetFinalDoor(door, MyLevel, FinalPos);

            if (FinalBlock.Core.MyTileSet == TileSets.Island)
            {
                MyLevel.MadeBackBlock.Extend(Side.Bottom, -2200);
            }

            // Push lava down
            MyLevel.PushLava(FinalBlock.Box.Target.TR.Y - 60);

            // Add exit sign
            Sign sign = new Sign(false);
            sign.PlaceAt(door.GetTop());
            MyLevel.AddObject(sign);

            // Cleanup
            FinalBlock = null;
        }

        public static void AttachDoorAction(ILevelConnector door)
        {
            if (Tools.WorldMap != null)
            {
                StringWorldGameData stringworld = Tools.WorldMap as StringWorldGameData;
                if (stringworld != null)
                    door.OnOpen = stringworld.EOL_StringWorldDoorAction;
            }
        }

        public static void SetFinalDoor(Door door, Level level, Vector2 FinalPos)
        {
            door.Core.EditorCode1 = LevelConnector.EndOfLevelCode;

            // Attach an action to the door
            AttachDoorAction(door);

            // Mod CameraZone
            CameraZone camzone = (CameraZone)level.Objects.Find(obj =>
                obj.Core.MyType == ObjectType.CameraZone);

            camzone.End.X = FinalPos.X - level.MainCamera.GetWidth() / 2 + 420;

            if (level.PieceSeed.ZoomType == LevelZoom.Big)
                camzone.End.X -= 150;
        }
    }
}
