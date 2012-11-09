using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.InGameObjects;

namespace CloudberryKingdom.Levels
{
    public class MakeFinalDoorVertical : MakeThing
    {
        protected Level MyLevel;

        /// <summary>
        /// The block on which the final door rests on.
        /// </summary>
        BlockBase FinalBlock;

        /// <summary>
        /// The position of the final door.
        /// </summary>
        Vector2 FinalPos;

        public MakeFinalDoorVertical(Level level)
        {
            MyLevel = level;
        }

        public override void Phase1()
        {
            base.Phase1();
        }

        class ElementPositionProjectY : LambdaFunc_1<BlockBase, float>
        {
            public float Apply(BlockBase element)
            {
                return element.Core.Data.Position.Y;
            }
        }

        public override void Phase2()
        {
            base.Phase2();

            // Find a final block that was used by the computer.
            //FinalBlock = MyLevel.Blocks.Find(match => match.Core == "LastRow" && match.Core.GenData.Used);
            if (MyLevel.CurMakeData.PieceSeed.GeometryType == LevelGeometry.Down)
                FinalBlock = Tools.ArgMin(MyLevel.Blocks.FindAll(match => match.Core.GenData.Used), new ElementPositionProjectY());
            else
                FinalBlock = Tools.ArgMax(MyLevel.Blocks.FindAll(match => match.Core.GenData.Used), new ElementPositionProjectY());

            FinalPos = FinalBlock.Core.Data.Position;

            // Cut computer run short once the computer reaches the door.
            int Earliest = 100000;
            Vector2 pos = FinalPos;
            float Closest = -1;
            int NewLastStep = MyLevel.LastStep;
            for (int i = 0; i < MyLevel.CurPiece.NumBobs; i++)
                for (int j = MyLevel.LastStep - 1; j > 0; j--)
                {
                    Vector2 BobPos = MyLevel.CurPiece.Recording[i].AutoLocs[j];
                    float Dist = (BobPos - FinalPos).Length();

                    if (Closest == -1 || Dist < Closest)
                    {
                        Earliest = Math.Min(Earliest, j);
                        Closest = Dist;
                        pos = BobPos;
                        NewLastStep = j;
                    }
                }

            MyLevel.LastStep = Math.Min(Earliest, MyLevel.LastStep);
        }

        protected Door MadeDoor;
        public override void Phase3()
        {
            base.Phase3();

            // Add door
            MadeDoor = MyLevel.PlaceDoorOnBlock(FinalPos, FinalBlock, false);
            MadeDoor.Core.EditorCode1 = LevelConnector.EndOfLevelCode;

            // Attach an action to the door
            MakeFinalDoor.AttachDoorAction(MadeDoor);
            //door.OnOpen = ((StringWorldGameData)Tools.WorldMap).EOL_StringWorldDoorAction;
        }

        public override void Cleanup()
        {
            MadeDoor = null;
            FinalBlock = null;
        }
    }
}
