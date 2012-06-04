using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public enum PlaceTypes { FallingBlock, BouncyBlock, SuperBouncyBlock, GhostBlock, MovingBlock, FlyingBlob, NormalBlock, Princess };
   
    public class PlaceGameData : NormalGameData
    {
        public static BobPhsx[] AllowedHeros = { BobPhsxNormal.Instance, BobPhsxDouble.Instance, BobPhsxJetman.Instance, BobPhsxSmall.Instance, BobPhsxBox.Instance, BobPhsxBouncy.Instance };
        public static PlaceTypes[] EditorAllowedPlaceTypes = { PlaceTypes.FallingBlock, PlaceTypes.FlyingBlob, PlaceTypes.BouncyBlock, PlaceTypes.GhostBlock, PlaceTypes.MovingBlock };
        public static ObjectType[] PlaceObjectType = { ObjectType.FallingBlock, ObjectType.BouncyBlock, ObjectType.BouncyBlock, ObjectType.GhostBlock, ObjectType.MovingBlock, ObjectType.FlyingBlob, ObjectType.NormalBlock };

        public enum PlaceState { Uninitialized, Set, Prove };
        public PlaceState State = PlaceState.Uninitialized;

        public static bool ShowedPlaceExplanation = false, ShowedProveExplanation;

        public static ObjectType[] ValidPlaceObjectTypes = { ObjectType.NormalBlock, ObjectType.FallingBlock, ObjectType.MovingBlock, ObjectType.FlyingBlob, ObjectType.GhostBlock };
        public static string[] PlaceTypeNames = { "Falling blocks", "Bouncy", "Super bouncy", "Ghost", "Moving Blocks", "Blobs", "Blocks" };
        public PlaceTypes PlaceType;

        public static new GameData Factory(LevelSeedData data, bool MakeInBackground)
        {
            return new PlaceGameData(data, MakeInBackground);
        }

        public override void SetAdditionalBobParameters(List<Bob> Bobs)
        {
            base.SetAdditionalBobParameters(Bobs);

            foreach (Bob bob in Bobs)
            {
                bob.MoveData.PlacePlatforms = (State != PlaceState.Prove);
                bob.MoveData.PlaceObject = PlaceType;
            }
        }

        public PlaceGameData(LevelSeedData LevelSeed, bool MakeInBackground)
        {
            Init(LevelSeed, MakeInBackground);
        }

        public override void Init(LevelSeedData LevelSeed, bool MakeInBackground)
        {
            PlaceType = LevelSeed.PlaceObjectType;

            State = PlaceState.Uninitialized;

            base.Init(LevelSeed, MakeInBackground);
        }

        public override void SetAdditionalLevelParameters()
        {
            base.SetAdditionalLevelParameters();

            MyLevel.AllowRecording = false;
        }

        void KeepPlacedObjects()
        {
            // Keep placed objects
            Action<ObjectBase> process = _obj => _obj.Core.RemoveOnReset = false;

            MyLevel.Objects.FindAll(match => match.Core.Placed).ForEach(process);
            MyLevel.Blocks.FindAll(match => match.Core.Placed).ForEach(block => process(block));
        }

        public override void GotCheckpoint(Bob CheckpointBob)
        {
            base.GotCheckpoint(CheckpointBob);

            KeepPlacedObjects();
        }

        void DoorAction_Set(Door door)
        {
            MyLevel.UndoSoftEndLevel();

            // Give bonus to completing player
            door.Core.MyLevel.EndOfLevelBonus(door.InteractingBob.MyPlayerData, false);

            // Turn off computer guide
            RemoveAllGameObjects(match => match is ShowGuide);

            // Set to reset (should not cost a life)
            MyLevel.SetToReset = true;
            MyLevel.FreeReset = true;

            // Can't watch computer, but can watch replay after level
            MyLevel.CanWatchComputer = false;
            MyLevel.AllowRecording = true;

            State = PlaceState.Prove;

            // Keep placed objects
            KeepPlacedObjects();

            // Revert level
            RevertLevel();

            // Disable player place-object ability
            foreach (Bob bob in MyLevel.Bobs)
            {
                bob.MoveData.PlacePlatforms = false;

                bob.UnsetPlaceAnimData();
            }

            // Start recording
            door.Core.MyLevel.MyGame.ToDoOnReset.Add(() => door.Core.MyLevel.StartRecording());

            door.OnOpen = HoldAction;

            // Open door
            door.SetLock(false, false, false);

            // Pause before opening initial door
            PhsxStepsToDo += 3;
            ToDoOnReset.Add(() => WaitThenDo(1, () => EnterFrom(MyLevel.StartDoor)));
        }

        DoorAction HoldAction;
        public override void AdditionalReset()
        {
// Uncomment to debug jump hint
//FullJump = false;
//ReferenceFrame = 0;

            base.AdditionalReset();

            if (State == PlaceState.Uninitialized)
            {
                State = PlaceState.Set;

                // Replace door's action
                ObjectBase obj = MyLevel.FindIObject(LevelConnector.EndOfLevelCode);
                
                Door door = obj as Door;
                if (null != door)
                {
                    HoldAction = door.OnOpen;
                    door.OnOpen = me =>
                        {
                            MyLevel.SoftEndLevel();
                            door.SetLock(true, false, true);

                            WaitThenDo(45, () => DoorAction_Set(door));
                        };
                }
            }

            if (State == PlaceState.Set)
            {
                // Encase blocks
                /*
                foreach (Block block in MyLevel.Blocks)
                    block.Core.Encased = true;
                */

                ////MyLevel.SuppressCheckpoints = true;
                //MyLevel.GhostCheckpoints = true;
                
                //DBox.Init("Press {pBig_Button_A,50,50} while in the air to place an object.", Tools.MedFont);



                // Show explanation
                //string ObjName = "object";
                if (!ShowedPlaceExplanation)
                {
                    ShowedPlaceExplanation = true;

                    WaitThenDo(40, () =>
                        {
                            HintBlurb2 hint = new HintBlurb2();
                            hint.SetText("Hold " + ButtonString.Jump(85) + " while falling to lay an object.");
                            AddGameObject(hint);
                        });
                }


                foreach (Bob bob in MyLevel.Bobs)
                {
                    bob.PlaceType = PlaceType;

                    bob.HeldObject = Recycle.GetObject(PlaceObjectType[(int)PlaceType], false);
                    if (PlaceType == PlaceTypes.SuperBouncyBlock)
                    {
                        BouncyBlock fblock = bob.HeldObject as BouncyBlock;
                        fblock.Init(Vector2.Zero, new Vector2(125, 125), 100);
                    }
                    if (PlaceType == PlaceTypes.BouncyBlock)
                    {
                        BouncyBlock fblock = bob.HeldObject as BouncyBlock;
                        fblock.Init(Vector2.Zero, new Vector2(75, 75), 100);
                    }
                    if (PlaceType == PlaceTypes.FallingBlock)
                    {
                        FallingBlock fblock = bob.HeldObject as FallingBlock;
                        fblock.Init(Vector2.Zero, new Vector2(75, 75), 100, MyLevel);
                    }
                    if (PlaceType == PlaceTypes.MovingBlock)
                    {
                        MovingBlock mblock = bob.HeldObject as MovingBlock;
                        mblock.Init(Vector2.Zero, new Vector2(75, 75), MyLevel);
                    }
                    if (PlaceType == PlaceTypes.GhostBlock)
                    {
                        GhostBlock gblock = bob.HeldObject as GhostBlock;
                        gblock.Init(Vector2.Zero, new Vector2(55, 55));
                        gblock.InLength = 60;
                        gblock.OutLength = 60;
                    }
                    bob.HeldObject.Core.MyLevel = MyLevel;
                    bob.HeldObject.Core.Held = true;
                }
            }

            if (State == PlaceState.Prove)
            {
                // Start recording
                if (!MyLevel.Watching && MyLevel.Recording && !(MyLevel.PlayMode != 0 || MyLevel.Watching))
                    MyLevel.StartRecording();

                // Encase blocks
                /*
                foreach (Block block in MyLevel.Blocks)
                    block.Core.Encased = false;
                */

                ////MyLevel.SuppressCheckpoints = false;
                //MyLevel.GhostCheckpoints = false;

                // Show explanation
                if (!ShowedProveExplanation)
                {
                    ShowedProveExplanation = true;
                }

                foreach (Bob bob in MyLevel.Bobs)
                {
                    bob.HeldObject = null;
                }
            }
        }

        static bool FullJump = false;
        int ReferenceFrame = 0;
        public override void PhsxStep()
        {
            base.PhsxStep();

            if (MyLevel == null || MyLevel.LevelReleased || Released) return;

            // If the player hasn't done a full jump off a placed piece,
            // check to see if we should display a hint about holding {JUMP} to jump higher
            if (State == PlaceState.Set && !FullJump && MyLevel.PlayMode == 0)
            {
                foreach (Bob bob in MyLevel.Bobs)
                {
                    BobPhsxNormal phsx = bob.MyPhsx as BobPhsxNormal;
                    if (null == phsx) continue;

                    if (phsx.ApexReached && phsx.PlacedJump && phsx.JumpCount < 2)
                        FullJump = true;

                    if (ReferenceFrame == 0 && phsx.PlacedJump && MyLevel.CurPhsxStep > 30)
                        ReferenceFrame = MyLevel.CurPhsxStep;
                }

                if (!FullJump && ReferenceFrame > 0 && MyLevel.CurPhsxStep - ReferenceFrame > 62 * 4)
                {
                    HintBlurb2 hint = new HintBlurb2();
                    hint.SetText("Hold " + ButtonString.Jump(85) + " to jump higher");
                    AddGameObject(hint);

                    FullJump = true;
                }
            }
        }
    }
}