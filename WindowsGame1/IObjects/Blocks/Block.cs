using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Blocks
{
    public class BlockData : ObjectData
    {
        public void Decide_RemoveIfUnused(float ChanceToKeep)
        {
            GenData.Decide_RemoveIfUnused(ChanceToKeep);
            if (GenData.RemoveIfUnused || Tools.RndFloat() < .75f)
                BlobsOnTop = true;
            else
                Virgin = true;
        }

        public bool Safe;

        public bool Finalized, NoExtend;

        /// <summary>
        /// If false then objects attached to the block use Box.Target.Center as their reference point,
        /// otherwise they use CustomCenterAsParent
        /// </summary>
        public bool UseCustomCenterAsParent;
        public Vector2 CustomCenterAsParent;
        public Vector2 OffsetMultAsParent;

        /// <summary>
        /// If true the block is drawn upside down.
        /// </summary>
        public bool InvertDraw;

        /// <summary>
        /// If true, overlapping non-child objects are removed.
        /// </summary>
        public bool RemoveOverlappingObjects;

        /// <summary>
        /// True if any part of the block besides the top has been used.
        /// </summary>
        public bool NonTopUsed;

        /// <summary>
        /// If true the block can not have its height modified, even if the tile set allows it.
        /// </summary>
        public bool DisableFlexibleHeight;

        /// <summary>
        /// If true the block is deleted if ever attempted to be made TopOnly
        /// </summary>
        public bool DeleteIfTopOnly;

        public bool StoodOn, HitHead, NoComputerTouch;

        /// <summary>
        /// When true the block will not override the imposition of another block.
        /// </summary>
        public bool DoNotPushHard;

        /// <summary>
        /// If true then a player that lands on this block will take on its Y-velocity.
        /// </summary>
        public bool GivesVelocity;

        /// <summary>
        /// The speed of the top surface of this block
        /// </summary>
        public float GroundSpeed;

        public float Layer;

        public bool UseTopOnlyTexture;

        public bool Ceiling, CeilingDraw, BlobsOnTop, Virgin;

        public bool OnlyCollidesWithLowerLayers;

        public Block TopRightNeighbor, TopLeftNeighbor;

        public List<IObject> Objects = new List<IObject>();

        public override void Release()
        {
            base.Release();

            //Objects = null;
            Objects.Clear();

            TopRightNeighbor = null;
            TopLeftNeighbor = null;
        }

        public void Draw()
        {
            if (Objects == null) return;

            foreach (IObject obj in Objects)
                if (!obj.Core.MarkedForDeletion)
                    obj.Draw();
        }

        public override void Init()
        {
            base.Init();

            //Objects = new List<IObject>();

            DoNotPushHard = false;

            UseCustomCenterAsParent = false;
            CustomCenterAsParent = Vector2.Zero;
            OffsetMultAsParent = Vector2.One;

            InvertDraw = false;

            RemoveOverlappingObjects = true;

            Finalized = NoExtend = false;

            NonTopUsed = false;

            DisableFlexibleHeight = false;
            DeleteIfTopOnly = false;

            StoodOn = HitHead = NoComputerTouch = false;

            GivesVelocity = true;

            UseTopOnlyTexture = true;

            Layer = 0;

            CeilingDraw = Ceiling = BlobsOnTop = Virgin = false;

            TopLeftNeighbor = TopRightNeighbor = null;
        }

        public override void Clone(ObjectData A)
        {
            base.Clone(A);

            BlockData BlockDataA = A as BlockData;
            if (BlockDataA == null)
                throw(new Exception("Can't copy block data from object data"));

            Safe = BlockDataA.Safe;

            UseCustomCenterAsParent = BlockDataA.UseCustomCenterAsParent;
            CustomCenterAsParent = BlockDataA.CustomCenterAsParent;
            OffsetMultAsParent = BlockDataA.OffsetMultAsParent;

            InvertDraw = BlockDataA.InvertDraw;

            GivesVelocity = BlockDataA.GivesVelocity;

            BlobsOnTop = BlockDataA.BlobsOnTop;
            Ceiling = BlockDataA.Ceiling;
            CeilingDraw = BlockDataA.CeilingDraw;
            Virgin = BlockDataA.Virgin;

            UseTopOnlyTexture = BlockDataA.UseTopOnlyTexture;

            OnlyCollidesWithLowerLayers = BlockDataA.OnlyCollidesWithLowerLayers;

            Layer = BlockDataA.Layer;

            Objects.Clear();
            Objects.AddRange(BlockDataA.Objects);
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write(Safe);

            writer.Write(BlobsOnTop);
            writer.Write(Ceiling);
            writer.Write(Virgin);

            writer.Write(OnlyCollidesWithLowerLayers);

            writer.Write(Layer);
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);

            Safe = reader.ReadBoolean();

            BlobsOnTop = reader.ReadBoolean();
            Ceiling = reader.ReadBoolean();
            Virgin = reader.ReadBoolean();

            OnlyCollidesWithLowerLayers = reader.ReadBoolean();

            Layer = reader.ReadInt32();
        }
    }

    public class BlockBase
    {
        public virtual bool PostCollidePreDecision(Bob bob) { return false; }

        public virtual bool PostCollideDecision(Bob bob, ref ColType Col, ref bool Overlap)
        {
            return Block_PostCollideDecision(this as Block, bob, ref Col, ref Overlap);
        }

        public static bool Block_PostCollideDecision(Block block, Bob bob, ref ColType Col, ref bool Overlap)
        {
            // Decide if we should delete or keep the block
            bool Delete = false;

            // MAKE SAVENOBLOCK a countdown
            if (bob.SaveNoBlock) Delete = true;
            if (bob.BottomCol && Col == ColType.Top) Delete = true;
            if (bob.TopCol && Col == ColType.Bottom) Delete = true;
            if (Col == ColType.Top && bob.WantsToLand == false) Delete = true;
            // SHOULD BE able to override this
            if (Col == ColType.Bottom && bob.Core.Data.Position.Y < bob.TargetPosition.Y) Delete = true;
            if (Col == ColType.Bottom) Delete = true;
            // ???
            if (Overlap && Col == ColType.NoCol && !block.Box.TopOnly && !(block is NormalBlock && !block.BlockCore.NonTopUsed)) Delete = true;


            // Don't land on the very edge of the block
            if (!Delete && !bob.MyPhsx.OnGround)
            {
                float Safety = block.BlockCore.GenData.EdgeSafety;
                if (bob.Box.BL.X > block.Box.TR.X - Safety ||
                    bob.Box.TR.X < block.Box.BL.X + Safety)
                {
                    Delete = true;
                }
            }

            // Don't land on a block that says not to
            bool DesiresDeletion = false;
            if (block.Core.GenData.TemporaryNoLandZone ||
                !block.Core.GenData.Used && !block.PermissionToUse())
                DesiresDeletion = Delete = true;

            if (block.Core.GenData.Used) Delete = false;
            if (!DesiresDeletion && block.Core.GenData.AlwaysLandOn && !block.Core.MarkedForDeletion && Col == ColType.Top) Delete = false;
            if (!DesiresDeletion && block.Core.GenData.AlwaysLandOn_Reluctantly && bob.WantsToLand_Reluctant && !block.Core.MarkedForDeletion && Col == ColType.Top) Delete = false;
            // ??? IT SEEMS LIKE we are always overlapping if we are colliding?
            if (Overlap && block.Core.GenData.RemoveIfOverlap) Delete = true;
            if (!DesiresDeletion && block.Core.GenData.AlwaysUse && !block.Core.MarkedForDeletion) Delete = false;

            return Delete;
        }
        
        public virtual void PostKeep(Bob bob, ref ColType Col, ref bool Overlap) { }
        public virtual void PostInteractWith(Bob bob) { }
    }

    public interface Block : IObject
    {
        AABox Box { get; }
        bool IsActive { get; set; }
        BlockData BlockCore { get; }

        void Extend(Side side, float pos);

        void LandedOn(Bob bob);
        void HitHeadOn(Bob bob);
        void Smash(Bob bob);
        void SideHit(Bob bob);
        void Hit(Bob bob);

        bool PreDecision(Bob bob);

        bool PostCollidePreDecision(Bob bob);
        bool PostCollideDecision(Bob bob, ref ColType Col, ref bool Overlap);
        void PostKeep(Bob bob, ref ColType Col, ref bool Overlap);
        void PostInteractWith(Bob bob);
    }
}