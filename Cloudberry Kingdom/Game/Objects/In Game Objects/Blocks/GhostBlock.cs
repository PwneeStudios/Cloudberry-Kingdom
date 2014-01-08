using System;
using System.IO;
using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom.Blocks
{
    public enum GhostBlockState { PhasedIn, PhasedOut };
    public class GhostBlock : BlockBase
    {
        public class GhostBlockTileInfo : TileInfoBase
        {
            public BlockGroup Group = null;
            public TextureOrAnim Sprite = Tools.TextureWad.FindTextureOrAnim("fading block");
            public Vector2 Shift = Vector2.Zero;
        }

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

        public override void MakeNew()
        {
            TallBox = false;
            TallInvertBox = false;

            MyAnimSpeed = .1666f;

            MyBox.TopOnly = true;

            CoreData.Init();
            CoreData.DrawLayer = 3;
            BlockCore.MyType = ObjectType.GhostBlock;

            SetState(GhostBlockState.PhasedIn);

            SetAnimation();

            MyObject.Boxes[0].Animated = false;
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
            //MyObject.EnqueueAnimation(0, (float)MyLevel.Rnd.Rnd.NextDouble() * 1.5f, true);
            MyObject.EnqueueAnimation(0, (float)0, true);
            MyObject.DequeueTransfers();
            MyObject.Update();
        }

        public GhostBlock(bool BoxesOnly)
        {
            MyObject = new SimpleObject(Prototypes.GhostBlockObj, BoxesOnly);

            MyObject.Boxes[0].Animated = false;

            MyBox = new AABox();

            MakeNew();

            CoreData.BoxesOnly = BoxesOnly;
        }

        public static float TallScale = 1.45f;
        public static float TallInvertScale = 1.635f;
        public bool TallBox, TallInvertBox;
        public void Init(Vector2 center, Vector2 size, Level level)
        {
            Active = true;

            BlockCore.Layer = .35f;
            CoreData.DrawLayer = 7;

            if (TallBox)
                size.Y *= TallScale;
            else if (TallInvertBox)
                size.Y *= TallInvertScale;

            // Use PieceQuad group if it exists.
            if (level.Info.GhostBlocks.Group != null)
                base.Init(ref center, ref size, level, level.Info.GhostBlocks.Group);
            // Otherwise use old SimpleObject
            else
            {
                CoreData.StartData.Position = CoreData.Data.Position = center;
             
                MyBox.Initialize(center, size);
            }

            MyBox.TopOnly = true;

            SetState(GhostBlockState.PhasedIn, true);

            Update();
        }

        public override void Reset(bool BoxesOnly)
        {
            BlockCore.BoxesOnly = BoxesOnly;

            Active = true;

            // Sub in image
            if (!BoxesOnly)
                MyObject.Quads[0].Set(Info.GhostBlocks.Sprite);

            SetState(GhostBlockState.PhasedIn, true);

            BlockCore.Data = BlockCore.StartData;

            MyBox.Current.Center = BlockCore.StartData.Position;

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();

            Update();
        }
        
        public void AnimStep()
        {
            if (MyObject.DestinationAnim() == 0 && MyObject.Loop)
                MyObject.PlayUpdate(MyAnimSpeed * CoreData.IndependentDeltaT);
        }

        public int Period { get { return InLength + OutLength; } }

        /// <summary>
        /// Gets the Ghosts current step in its periodic cycle,
        /// shifting to account for its Offset
        /// </summary>
        public float GetStep()
        {
            //return CoreMath.Modulo(Core.MyLevel.GetPhsxStep() + Offset, Period);
            return CoreMath.Modulo(CoreData.MyLevel.GetIndependentPhsxStep() + Offset, (float)Period);
        }

        /// <summary>
        /// Calculate what the Offset should be such that at this moment in time
        /// the Ghost is at the given step in its periodic cycle.
        /// </summary>
        public void ModOffset(int DesiredStep)
        {
            //int CurPhsxStep = Core.MyLevel.GetPhsxStep();
            int CurPhsxStep = (int)(CoreData.MyLevel.GetIndependentPhsxStep() + .49f);

            // Make sure the desired step is positive
            DesiredStep = (DesiredStep + Period) % Period;

            // Calculate the new offset
            Offset = DesiredStep - CurPhsxStep % Period;

            // Make sure the offset is positive
            Offset = (Offset + Period) % Period;
        }

        public static int LengthOfPhaseChange = 35;

        public override void PhsxStep()
        {
            Active = CoreData.Active = true;
            if (!CoreData.Held)
            {
                if (MyBox.Current.BL.X > BlockCore.MyLevel.MainCamera.TR.X + 40 || MyBox.Current.BL.Y > BlockCore.MyLevel.MainCamera.TR.Y + 200)
                    Active = CoreData.Active = false;
                if (MyBox.Current.TR.X < BlockCore.MyLevel.MainCamera.BL.X  - 40|| MyBox.Current.TR.Y < BlockCore.MyLevel.MainCamera.BL.Y - 200)
                    Active = CoreData.Active = false;
            }

            if (!CoreData.BoxesOnly && Active && CoreData.Active) AnimStep();

            CoreData.GenData.JumpNow = CoreData.GenData.TemporaryNoLandZone = false;

            float Step = GetStep();
            if (Step < InLength)
            {
                CoreData.Active = true;
                State = GhostBlockState.PhasedIn;

                // As Step approaches InLength the StateChange approaches 0 (faded out)
                StateChange = (InLength - Step) / (float)LengthOfPhaseChange;
                if (StateChange < .25f) CoreData.Active = false;

                // If we're about to fade out don't allow computer to land on this ghost
                // and jump if the computer is already on it
                if (StateChange < .25f + TimeSafety) CoreData.GenData.JumpNow = true;
                if (StateChange < .25f + .65f)       CoreData.GenData.JumpNow = true;
            }
            else
            {
                CoreData.Active = false;
                State = GhostBlockState.PhasedOut;

                // As Step approaches InLength + OutLength (the total period),
                // the StateChange approaches 0 (faded out)
                StateChange = (InLength + OutLength - Step) / (float)LengthOfPhaseChange;
                if (StateChange < .75f) CoreData.Active = true;
            }

            // Make sure StateChange lies between 0 and 1
            StateChange = Math.Min(1, StateChange);

            // If this is Stage 1 of the level gen and this ghost hasn't been uset yet,
            // then set it to be always active.
            // We can adjust its Offset once it is used.
            if (CoreData.MyLevel.PlayMode == 2 && CoreData.GenData.Used == false)
                CoreData.Active = true;

            Update();

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);

            BlockCore.StoodOn = false;
        }

        public override void PhsxStep2()
        {
            if (!Active) return;

            MyBox.SwapToCurrent();
        }

        public void Update()
        {
            if (BlockCore.BoxesOnly) return;

            if (TallBox)
                MyObject.Base.Origin -= MyObject.Boxes[0].Center() - MyBox.Current.Center - new Vector2(0, MyBox.Current.Size.Y * (TallScale - 1) / 2);
            else if (TallInvertBox)
                MyObject.Base.Origin -= MyObject.Boxes[0].Center() - MyBox.Current.Center - new Vector2(0, MyBox.Current.Size.Y * (TallInvertScale - 1) / 2);
            else
                MyObject.Base.Origin -= MyObject.Boxes[0].Center() - MyBox.Current.Center;
            if (Info != null) MyObject.Base.Origin += Info.GhostBlocks.Shift;

            MyObject.Base.e1.X = 1;
            MyObject.Base.e2.Y = 1;
            MyObject.Update();           

            Vector2 CurSize = MyObject.Boxes[0].Size() / 2;
            Vector2 Scale = MyBox.Current.Size / CurSize;

            if (TallBox)
                Scale.Y /= TallScale;
            else if (TallInvertBox)
                Scale.Y /= TallInvertScale;

            MyObject.Base.e1.X = Scale.X;
            MyObject.Base.e2.Y = Scale.Y;

            MyObject.Update();   
        }

        public override void Extend(Side side, float pos)
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

            BlockCore.StartData.Position = MyBox.Current.Center;
        }

        public override void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);

            Update();
        }
        public override void Draw()
        {
            if (Active)
            {
                Update();

				if (Tools.DrawBoxes)
				{
                    Vector4 Full, Half;
                    Full = new Vector4(.8f, .2f, .8f, 1.0f);
                    Half = new Vector4(.8f, .2f, .8f, 0.2f);
                    Color color;

                    if (State == GhostBlockState.PhasedIn)
                        color = new Color((1 - StateChange) * Half + StateChange * Full);
                    else
                        color = new Color((1 - StateChange) * Full + StateChange * Half);

					MyBox.DrawFilled(Tools.QDrawer, color);

					//MyBox.DrawFilled(Tools.QDrawer, Color.Green);
				}
            }

            if (Tools.DrawGraphics)
            {
                if (Active && !BlockCore.BoxesOnly)
                {
                    Vector4 Full, Half;
                    Full = new Vector4(1, 1f, 1f, 1f);
                    Half = new Vector4(1, 1f, 1f, 0.06f);
                    Color color;

                    if (State == GhostBlockState.PhasedIn)
                        color = new Color((1 - StateChange) * Half + StateChange * Full);
                    else
                        color = new Color((1 - StateChange) * Full + StateChange * Half);

                    if (Info.GhostBlocks.Group == null)
                    {
                        MyObject.SetColor(color);
                        MyObject.Draw(Tools.QDrawer, Tools.EffectWad);
                    }
                    else
                    {
                        MyDraw.Update();
                        MyDraw.Draw();
                    }
                }
            }

            BlockCore.Draw();
        }

        public override void PostInteractWith(Bob bob, ref ColType Col, ref bool Overlap)
        {
            base.PostInteractWith(bob, ref Col, ref Overlap);

            GhostBlock block = this as GhostBlock;

            // Ghost blocks delete surrounding blocks when stamped as used
            foreach (BlockBase gblock in CoreData.MyLevel.Blocks)
            {
                GhostBlock ghost = gblock as GhostBlock;
                if (null != ghost && !ghost.CoreData.MarkedForDeletion)
                    if (!ghost.CoreData.GenData.Used &&
                        (ghost.CoreData.Data.Position - block.CoreData.Data.Position).Length() < 200)
                    {
                        bob.DeleteObj(ghost);
                        ghost.IsActive = false;
                    }
            }
        }

        public override void Clone(ObjectBase A)
        {
            CoreData.Clone(A.CoreData);

            GhostBlock BlockA = A as GhostBlock;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size, A.MyLevel);
            MyBox.TopOnly = BlockA.MyBox.TopOnly;
        }
    }
}
