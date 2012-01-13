using System;
using System.IO;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public enum GhostBlockState { PhasedIn, PhasedOut };
    public class GhostBlock : Block
    {
        public void TextDraw() { }

        public AABox MyBox;

        public SimpleObject MyObject;

        GhostBlockState State;
        
        /// <summary>
        /// When phased in, StateChange == 1 means fully phased in
        ///                 StateChange == 0 means fully phased out, ready to change state to phased out
        /// When phased out, StateChange == 1 means fully phased out
        ///                  StateChange == 0 means fully phased in, ready to change state to phased in
        /// </summary>
        float StateChange;

        public float MyAnimSpeed;

        public int InLength, OutLength, Offset;

        /// <summary>
        /// How close in time to the block fading out or in can the computer interact with the block.
        /// Units are time are in the time it takes for a phase change to happen.
        /// Smaller is harder.
        /// </summary>
        public float TimeSafety;

        public AABox Box { get { return MyBox; } }

        public bool Active;
        public bool IsActive { get { return Core.Active; } set { Core.Active = value; } }

        public BlockData CoreData;
        public BlockData BlockCore { get { return CoreData; } }
        public ObjectData Core { get { return CoreData as BlockData; } }
        public void Interact(Bob bob) { }

        public void MakeNew()
        {
            MyAnimSpeed = InfoWad.GetFloat("GhostBlock_AnimSpeed");

            MyBox.TopOnly = true;

            Core.Init();
            Core.DrawLayer = 3;
            CoreData.MyType = ObjectType.GhostBlock;

            SetState(GhostBlockState.PhasedIn);

            SetAnimation();

            MyObject.Boxes[0].Animated = false;
        }

        public void Release()
        {
            BlockCore.Release();
            Core.MyLevel = null;
        }

        public void SetState(GhostBlockState NewState) { SetState(NewState, false); }
        public void SetState(GhostBlockState NewState, bool ForceSet)
        {
            if (State != NewState || ForceSet)
            {
                switch (NewState)
                {
                    case GhostBlockState.PhasedIn:
                        break;
                    case GhostBlockState.PhasedOut:
                        break;
                }
            }

            State = NewState;
        }

        void SetAnimation()
        {
            MyObject.Read(0, 0);
            MyObject.Play = true;
            MyObject.Loop = true;
            MyObject.EnqueueAnimation(0, (float)Tools.Rnd.NextDouble() * 1.5f, true);
            MyObject.DequeueTransfers();
            MyObject.Update();
        }

        public GhostBlock(bool BoxesOnly)
        {
            CoreData = new BlockData();

            MyObject = new SimpleObject(Prototypes.GhostBlockObj, BoxesOnly);

            MyObject.Boxes[0].Animated = false;

            MyBox = new AABox();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public void Init(Vector2 center, Vector2 size)
        {
            Active = true;

            CoreData.Layer = .35f;
            Core.DrawLayer = 7;// 8;

            MyBox = new AABox(center, size);
            MyBox.Initialize(center, size);
            MyBox.TopOnly = true;

            Core.StartData.Position = Core.Data.Position = center;

            SetState(GhostBlockState.PhasedIn, true);

            Update();
        }

        public void Hit(Bob bob) { }
        public void LandedOn(Bob bob)
        {
        }
        public void HitHeadOn(Bob bob) { } public void SideHit(Bob bob) { } 

        public void Reset(bool BoxesOnly)
        {
            CoreData.BoxesOnly = BoxesOnly;

            Active = true;

            SetState(GhostBlockState.PhasedIn, true);

            CoreData.Data = CoreData.StartData;

            MyBox.Current.Center = CoreData.StartData.Position;

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();
            MyBox.TopOnly = true;

            Update();
        }
        
        public void AnimStep()
        {
            if (MyObject.DestinationAnim() == 0 && MyObject.Loop)
                MyObject.PlayUpdate(MyAnimSpeed);
        }

        public int Period { get { return InLength + OutLength; } }

        /// <summary>
        /// Gets the Ghosts current step in its periodic cycle,
        /// shifting to account for its Offset
        /// </summary>
        public int GetStep()
        {
            //return (Core.MyLevel.GetPhsxStep() + Offset) % (Period);
            return Tools.Modulo(Core.MyLevel.GetPhsxStep() + Offset, Period);
        }

        /// <summary>
        /// Calculate what the Offset should be such that at this moment in time
        /// the Ghost is at the given step in its periodic cycle.
        /// </summary>
        public void ModOffset(int DesiredStep)
        {
            int CurPhsxStep = Core.MyLevel.GetPhsxStep();

            // Make sure the desired step is positive
            DesiredStep = (DesiredStep + Period) % Period;

            // Calculate the new offset
            Offset = DesiredStep - CurPhsxStep % Period;

            // Make sure the offset is positive
            Offset = (Offset + Period) % Period;
        }

        public static int LengthOfPhaseChange = 35;

        public void PhsxStep()
        {
            Active = Core.Active = true;
            if (!Core.Held)
            {
                if (MyBox.Current.BL.X > CoreData.MyLevel.MainCamera.TR.X + 40 || MyBox.Current.BL.Y > CoreData.MyLevel.MainCamera.TR.Y + 200)
                    Active = Core.Active = false;
                if (MyBox.Current.TR.X < CoreData.MyLevel.MainCamera.BL.X  - 40|| MyBox.Current.TR.Y < CoreData.MyLevel.MainCamera.BL.Y - 200)
                    Active = Core.Active = false;
            }

            if (!Core.BoxesOnly && Active && Core.Active) AnimStep();

            if (Core.MyLevel.DefaultHeroType is BobPhsxSpaceship && Box.TopOnly)
            {
                Box.TopOnly = false;
            }

            Core.GenData.JumpNow = Core.GenData.TemporaryNoLandZone = false;

            int Step = GetStep();
            if (Step < InLength)
            {
                Core.Active = true;
                State = GhostBlockState.PhasedIn;

                // As Step approaches InLength the StateChange approaches 0 (faded out)
                StateChange = (InLength - Step) / (float)LengthOfPhaseChange;
                if (StateChange < .25f) Core.Active = false;

                // If we're about to fade out don't allow computer to land on this ghost
                // and jump if the computer is already on it
                //if (StateChange < .25f + TimeSafety) Core.GenData.TemporaryNoLandZone = true;
                if (StateChange < .25f + TimeSafety) Core.GenData.JumpNow = true;
                //if (StateChange < .25f + .65f)       Core.GenData.TemporaryNoLandZone = true;
                if (StateChange < .25f + .65f)       Core.GenData.JumpNow = true;
            }
            else
            {
                Core.Active = false;
                State = GhostBlockState.PhasedOut;

                // As Step approaches InLength + OutLength (the total period),
                // the StateChange approaches 0 (faded out)
                StateChange = (InLength + OutLength - Step) / (float)LengthOfPhaseChange;
                if (StateChange < .75f) Core.Active = true;

                // If we just faded in don't allow computer to land on this ghost,
                //if (StateChange < .75f + TimeSafety) Core.GenData.TemporaryNoLandZone = true;
                //if (StateChange < .75f + .15)        Core.GenData.TemporaryNoLandZone = true;
            }

            // Make sure StateChange lies between 0 and 1
            StateChange = Math.Min(1, StateChange);

            // If this is Stage 1 of the level gen and this ghost hasn't been uset yet,
            // then set it to be always active.
            // We can adjust its Offset once it is used.
            if (Core.MyLevel.PlayMode == 2 && Core.GenData.Used == false)
                Core.Active = true;

            Update();

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);

            CoreData.StoodOn = false;
        }

        public void PhsxStep2()
        {
            if (!Active) return;

            MyBox.SwapToCurrent();
        }


        public void Update()
        {
            if (CoreData.BoxesOnly) return;

            MyObject.Base.Origin -= MyObject.Boxes[0].Center() - MyBox.Current.Center;

            MyObject.Base.e1.X = 1;
            MyObject.Base.e2.Y = 1;
            MyObject.Update();           

            Vector2 CurSize = MyObject.Boxes[0].Size() / 2;
            Vector2 Scale = MyBox.Current.Size / CurSize;
            MyObject.Base.e1.X = Scale.X;
            MyObject.Base.e2.Y = Scale.Y;

            MyObject.Update();   
        }

        public void Extend(Side side, float pos)
        {
            switch (side)
            {
                case Side.Left:
                    MyBox.Target.BL.X = pos;
                    break;
                case Side.Right:
                    MyBox.Target.TR.X = pos;
                    break;
                case Side.Top:
                    MyBox.Target.TR.Y = pos;
                    break;
                case Side.Bottom:
                    MyBox.Target.BL.Y = pos;
                    break;
            }

            MyBox.Target.FromBounds();
            MyBox.SwapToCurrent();

            Update();

            CoreData.StartData.Position = MyBox.Current.Center;
        }

        public void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);

            Update();
        }
        public void Draw()
        {
            if (Active)
            {
                Update();

                if (Tools.DrawBoxes)
                    MyBox.Draw(Tools.QDrawer, Color.Olive, 15);
            }

            if (Tools.DrawGraphics)
            {
                if (Active && !CoreData.BoxesOnly)
                {
                    Vector4 Full, Half;
                    Full = new Vector4(1, 1f, 1f, 1f);
                    Half = new Vector4(1, 1f, 1f, 0.06f);

                    if (State == GhostBlockState.PhasedIn)
                    {
                        MyObject.SetColor(new Color((1 - StateChange) * Half + StateChange * Full));

                        MyObject.Draw(Tools.QDrawer, Tools.EffectWad);
                        Tools.QDrawer.Flush();
                    }
                    if (State == GhostBlockState.PhasedOut)
                    { 
                        MyObject.SetColor(new Color((1 - StateChange) * Full + StateChange * Half));

                        MyObject.Draw(Tools.QDrawer, Tools.EffectWad);
                        Tools.QDrawer.Flush();
                    }
                }

                BlockCore.Draw();
            }
        }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);

            GhostBlock BlockA = A as GhostBlock;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size);
            MyBox.TopOnly = BlockA.MyBox.TopOnly;
        }

        public void Write(BinaryWriter writer)
        {
            BlockCore.Write(writer);
        }
        public void Read(BinaryReader reader) { Core.Read(reader); }
//StubStubStubStart
public void OnUsed() { }
public void OnMarkedForDeletion() { }
public void OnAttachedToBlock() { }
public bool PermissionToUse() { return true; }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public void Smash(Bob bob) { }
//StubStubStubEnd6
    }
}
