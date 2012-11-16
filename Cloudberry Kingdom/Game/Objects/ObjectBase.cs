﻿using System;
using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;








namespace CloudberryKingdom
{
    public class ObjectBase : ViewReadWrite
    {
        public GameData Game { get { return Core.MyLevel.MyGame; } }
        public Level MyLevel { get { return Core.MyLevel; } }
        public Camera Cam { get { return Core.MyLevel.MainCamera; } }
        public Rand Rnd { get { return Core.MyLevel.Rnd; } }

        public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }

        protected ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }
        public TileSet.TileSetInfo Info
        {
            get
            {
                if (CoreData.MyLevel == null)
                    return null;

                return CoreData.MyLevel.MyTileSet.MyTileSetInfo;
            }
        }

        public ObjectBase()
        {
            CoreData = new ObjectData();
        }

        public virtual void Init(Vector2 pos, Level level)
        {
            Core.StartData.Position = Core.Data.Position = pos;

            var tile = Core.MyTileSet = level.MyTileSet;
            Tools.Assert(Core.MyTileSet != null);
        }

        public virtual void Release()
        {
            Core.Release();
        }

        public void SetParentBlock(BlockBase block)
        {
            Core.SetParentBlock(block);
            OnAttachedToBlock();
        }

        public virtual void CollectSelf()
        {
            if (Core.MarkedForDeletion) return;

            Core.Recycle.CollectObject(this);
        }

        public void StampAsUsed(int CurPhsxStep)
        {
            Core.GenData.__StampAsUsed(CurPhsxStep);
            OnUsed();
        }

        public virtual void MakeNew() { }
        public virtual void PhsxStep() { }
        public virtual void PhsxStep2() { }
        public virtual void Draw() { }
        public virtual void TextDraw() { }
        public virtual void Reset(bool BoxesOnly) { }
        public virtual void Clone(ObjectBase A) { Core.Clone(A.Core); }
        public virtual void Read(BinaryReader reader) { Core.Read(reader); }
        public virtual void Write(BinaryWriter writer) { Core.Write(writer); }
        public virtual void Interact(Bob bob) { }
        public virtual void Move(Vector2 shift)
        {
            Core.StartData.Position += shift;
            Core.Data.Position += shift;
        }

        public virtual void OnUsed() { }
        public virtual void OnMarkedForDeletion() { }
        public virtual void OnAttachedToBlock() { }
        public virtual bool PermissionToUse() { return true; }
        
        public virtual void Smash(Bob bob) { }
        public virtual bool PreDecision(Bob bob) { return false; }
    }

    public struct GenerationData
    {
        /// <summary>
        /// Prevents an obstacle from being deleted if it is unused, including in object cleanup phase due to crowding.
        /// </summary>
        public bool KeepIfUnused;

        /// <summary>
        /// If true, object will be removed if it's outside the level generator's bounds.
        /// </summary>
        public bool EnforceBounds;

        public bool AlwaysUse, AlwaysLandOn, AlwaysLandOn_Reluctantly;
        public bool Used, RemoveIfUnused, RemoveIfUsed, RemoveIfOverlap;
        public int UsedTimeStamp;

        public enum OverlapPreference { RemoveLowerThanMe, RemoveHigherThanMe, RemoveRandom }
        public OverlapPreference MyOverlapPreference;

        public void Decide_RemoveIfUnused(float ChanceToKeep, Rand Rnd)
        {
            if (Rnd.Rnd.NextDouble() < ChanceToKeep)
            {
                RemoveIfUnused = false;
            }
            else
            {
                RemoveIfUnused = true;
            }
        }

        /// <summary>
        /// Removes the object if it overlaps with a block. Defaults to false.
        /// </summary>
        public bool NoBlockOverlap;
        public float OverlapWidth;

        /// <summary>
        /// For all objects with this set to true, a cleanup is performed with a uniform minimum distance enforced
        /// </summary>
        public bool LimitGeneralDensity;

        /// <summary>
        /// For all objects of the same type with this set to true, a cleanup is performed with some minimum distance enforced
        /// </summary>
        public bool LimitDensity;

        /// <summary>
        /// If true when this object is used it deletes similar surrounding objects. Implemented for NormalBlocks only.
        /// </summary>
        public bool DeleteSurroundingOnUse;

        public bool EdgeJumpOnly;

        /// <summary>
        /// When true the computer will not choose to land on this block.
        /// </summary>
        public bool TemporaryNoLandZone;

        /// <summary>
        /// When true the computer will ALWAYS jump off this block immediately.
        /// </summary>
        public bool JumpNow;

        /// <summary>
        /// How close to the edge the computer is allowed to land on this block and still use it.
        /// </summary>
        public float EdgeSafety;

        public bool NoBottomShift;
        public bool NoMakingTopOnly;

        public Lambda OnUsed, OnMarkedForDeletion;
        public void __StampAsUsed(int Step)
        {
            if (Used)
                return;

            if (OnUsed != null)
                OnUsed.Apply();

            Used = true;
            UsedTimeStamp = Step;
        }

        public void Release()
        {
            OnUsed = OnMarkedForDeletion = null;
        }

        public void Init()
        {
            OnUsed = OnMarkedForDeletion = null;

            AlwaysUse = AlwaysLandOn = false;

            EnforceBounds = true;

            KeepIfUnused = false;

            RemoveIfUsed = false;
            RemoveIfUnused = true;
            Used = false;

            NoBlockOverlap = false;
            OverlapWidth = 35;
            LimitGeneralDensity = false;
            LimitDensity = true;

            DeleteSurroundingOnUse = true;

            EdgeJumpOnly = false;

            TemporaryNoLandZone = false;
            JumpNow = false;

            EdgeSafety = 0;

            NoBottomShift = false;
            NoMakingTopOnly = false;
        }
    }

    public static class BlockExtension
    {
        public static void StampAsFullyUsed(this BlockBase block, int CurPhsxStep)
        {
            block.StampAsUsed(CurPhsxStep);
            block.BlockCore.NonTopUsed = true;
        }

        public static void Stretch(this BlockBase block, Side side, float amount)
        {
            block.Box.CalcBounds();
            switch (side) {
                case Side.Right: block.Extend(side, block.Box.GetTR().X + amount); break;
                case Side.Left: block.Extend(side, block.Box.GetBL().X + amount); break;
                case Side.Top: block.Extend(side, block.Box.GetTR().Y + amount); break;
                case Side.Bottom: block.Extend(side, block.Box.GetBL().Y + amount); break;
            }
        }
    }

    public class ObjectData
    {
        public Recycler Recycle
        {
            get
            {
                if (MyLevel != null)
                    return MyLevel.Recycle;
                else
                    Tools.Break();

                return null;
            }
        }

        /// <summary>
        /// Whether this object belongs to a level in the middle of level generation
        /// </summary>
        public bool LevelGenRunning()
        {
            if (MyLevel == null || MyLevel.PlayMode == 0 || MyLevel.Watching || MyLevel.Replay) return false;
            else return true;
        }

        /// <summary>
        /// True when the object skipped its phsx update, usually due to being off screen.
        /// </summary>
        public bool SkippedPhsx;

        /// <summary>
        /// True when the object has been inactive and needs to perform additional actions before normal activity resumes.
        /// </summary>
        public bool WakeUpRequirements;

        TileSet _TileSet = null;
        public TileSet MyTileSet
        {
            get { return _TileSet; }
            set
            {
                _TileSet = value;
                if (_TileSet == null)
                    Tools.Break();
            }
        }
        
        /// <summary>
        /// Whether the object should be drawn encased in glass.
        /// </summary>
        public bool Encased;

        public AutoGen_Parameters GetParams(AutoGen singleton)
        {
            return MyLevel.CurPiece.MyData.Style.FindParams(singleton);
        }

        public string EditorCode1, EditorCode2, EditorCode3;

        public bool Held, Placed;
        public Vector2 HeldOffset;

        public bool Holdable, EditHoldable;

        public int Tag; // Used for selectively scrolling out different blocks
        public static UInt64 NextId = 0;
        public UInt64 MyGuid;
        public static UInt64 GetId() { return NextId++; }

        public int DebugCode;

        public ObjectType MyType;
        public bool BoxesOnly, Show, AlwaysBoxesOnly;

        public bool VisualResettedOnce = false;

        public bool RemoveOnReset, MarkedForDeletion;
        public bool Active;
        //public bool _Active;
        //public bool Active
        //{
        //    get { return _Active; }
        //    set
        //    {
        //        _Active = value;
        //        if (this == "BossCenter" && value) Tools.Break();
        //    }
        //}


        /// <summary>
        /// If true the object has been set to be collected during the level generation process,
        /// the deletion was caused by the level gen algorithm deciding this object should be removed.
        /// </summary>
        public bool DeletedByBob;

        /// <summary>
        /// Whether the objects OnMarkedForDeletion code has been ran or not.
        /// Used to prevent re-running the code.
        /// </summary>
        public bool OnDeletionCodeRan;

        /// <summary>
        /// If the object is attached to a parent object and this flag is set to true,
        /// then the object is drawn independetly of the parent object.
        /// </summary>
        public bool DoNotDrawWithParent;

        /// <summary>
        /// Whether the object should interact with the characters.
        /// </summary>
        public bool Real = true;


        /// <summary>
        /// Whether the object is owned by a game.
        /// If False usually it is owned by a level.
        /// GameObjects are drawn by the level, but their physics is handled by the Game.
        /// </summary>
        public bool IsGameObject;

        /// <summary>
        /// Does not make a clone of the object on reset, instead just calling Reset()
        /// </summary>
        public bool ResetOnlyOnReset;

        public int DrawLayer2 = -1, DrawLayer3 = -1;

        public Level MyLevel;
        public int DrawLayer, DrawSubLayer;
        
        /// <summary>
        /// Prevents the DrawSubLayer from being overridden by it's actual position in the DrawLayer array.
        /// </summary>
        public bool FixSubLayer = false;

        /// <summary>
        /// If true then this obstacle can be continuously scaled up and down in difficulty.
        /// </summary>
        public bool ContinuousEnabled;

        public PhsxData Data, StartData;

        /// <summary>
        /// The frame that the object was added on.
        /// </summary>
        public int AddedTimeStamp;
        
        public GenerationData GenData;

        public AssociatedObjData[] Associations;
        public struct AssociatedObjData
        {
            public UInt64 Guid;
            public bool DeleteWhenDeleted, UseWhenUsed;

            public void Zero()
            {
                Guid = 0;
                DeleteWhenDeleted = false;
                UseWhenUsed = false;
            }
        }

        public UInt64 ParentObjId;
        public ObjectBase ParentObject;
        public BlockBase ParentBlock;
        public Vector2 ParentOffset;

        /// <summary>
        /// If the object just interacted with a player, this should point to the player's Bob
        /// </summary>
        public Bob InteractingBob
        {
            get { return _InteractingBob; }
            set
            {
                _InteractingBob = value;

                if (InteractingBob != null)
                    InteractingPlayer = InteractingBob.GetPlayerData();
            }
        }
        public Bob _InteractingBob;

        /// <summary>
        /// If the object just interacted with a player, this should point to the player
        /// </summary>
        public PlayerData InteractingPlayer;

        public bool DoNotScrollOut;

        public int StepOffset; // When an object from one level is added to another, an offset is calculated to keep it synchronized

        public static void AddAssociation(bool DeleteWhenDeleted, bool UseWhenUsed, params ObjectBase[] objs)
        {
            foreach (ObjectBase obj in objs)
                foreach (ObjectBase _obj in objs)
                    if (obj != _obj)
                        obj.Core.AddAssociate(_obj, DeleteWhenDeleted, UseWhenUsed);
        }

        public void AddAssociate(ObjectBase obj, bool DeleteWhenDeleted, bool UseWhenUsed)
        {
            int FreeIndex = 0;
            if (Associations == null)
            {
                Associations = new AssociatedObjData[5];
                for (int i = 0; i < 5; i++)
                    Associations[i].Zero();
            }
            else
                while (Associations[FreeIndex].Guid != 0)
                    FreeIndex++;

            Associations[FreeIndex].Guid = obj.Core.MyGuid;
            Associations[FreeIndex].DeleteWhenDeleted = DeleteWhenDeleted;
            Associations[FreeIndex].UseWhenUsed = UseWhenUsed;
        }

        public bool IsAssociatedWith(ObjectBase obj)
        {
            if (Associations == null || obj.Core.Associations == null)
                return false;
            else
            {
                foreach (AssociatedObjData data in Associations)
                {
                    if (data.Guid == obj.Core.MyGuid)
                        return true;
                }
                return false;
            }
        }

        public int GetAssociatedIndex(ObjectBase obj)
        {
            for (int i = 0; i < Associations.Length; i++)
                if (Associations[i].Guid == obj.Core.MyGuid)
                    return i;

            return -1;
        }

        public AssociatedObjData GetAssociationData(ObjectBase obj)
        {
            foreach (var objdata in Associations)
                if (objdata.Guid == obj.Core.MyGuid)
                    return objdata;
            return Associations[Associations.Length - 1];
        }

        public bool Released = false;
        public virtual void Release()
        {
#if DEBUG_OBJDATA
            if (!Released)
            {
                lock (MetaCount) { MetaCount.MyInt--; }
            }
#endif
            Released = true;

            Associations = null;
            MyLevel = null;
            ParentObject = null;
            GenData.Release();

            InteractingPlayer = null;
            InteractingBob = null;
        }

        public void SetParentObj(ObjectBase obj)
        {
            ParentObject = obj;
            ParentObjId = obj.Core.MyGuid;
        }

        public void SetParentBlock(BlockBase block)
        {
            ParentBlock = block;

            ParentOffset = Data.Position - ParentBlock.Box.Current.Center;
        }

        public Vector2 GetPosFromParentOffset()
        {
            BlockData pdata = ParentBlock.BlockCore;

            //return ParentBlock.Box.Target.Center + ParentOffset;
            if (pdata.UseCustomCenterAsParent)
                return pdata.CustomCenterAsParent +
                    pdata.OffsetMultAsParent * ParentOffset;
            else
                return ParentBlock.Box.Target.Center + ParentOffset;
        }

        public void PosFromParentOffset()
        {
            if (ParentBlock != null)
                Data.Position = GetPosFromParentOffset();
        }

        public int GetPhsxStep()
        {
            return MyLevel.GetPhsxStep() + StepOffset;
        }

        public float GetIndependentPhsxStep()
        {
            return MyLevel.GetIndependentPhsxStep() + StepOffset;
        }

        public float IndependentDeltaT { get { return MyLevel.IndependentDeltaT; } }

        public bool ContainsCode(string code)
        {
            return (string.Compare(EditorCode1, code) == 0 ||
                    string.Compare(EditorCode2, code) == 0 ||
                    string.Compare(EditorCode3, code) == 0);
        }

        public static bool operator ==(ObjectData data, string str)
        {
            return string.Compare(data.EditorCode1, str) == 0;
        }

        public static bool operator !=(ObjectData data, string str)
        {
            return !(data == str);
        }


        public virtual void Init()
        {
            Released = false;

            MyTileSet = TileSets.None;
            Encased = false;

            EditorCode1 = EditorCode2 = EditorCode3 = "";

            MyGuid = GetId();

            Associations = null;

            IsGameObject = false;

            VisualResettedOnce = false;

            HeldOffset = Vector2.Zero;
            Held = Placed = false;

            Holdable = EditHoldable = false;

            Tag = -1;

            DebugCode = 0;

            ContinuousEnabled = false;

            Show = true;

            Data = new PhsxData();
            StartData = new PhsxData();

            ParentBlock = null;
            ParentOffset = Vector2.Zero;

            DoNotScrollOut = false;

            RemoveOnReset = false;
            MarkedForDeletion = false;
            Active = true;

            DeletedByBob = false;
            OnDeletionCodeRan = false;

            MyLevel = null;
            DrawLayer = 0;
            DrawLayer2 = DrawLayer3 = -1;

            DoNotDrawWithParent = false;

            StepOffset = 0;
            
            GenData.Init();

            Associations = null;
        }

        public virtual void Clone(ObjectData A)
        {
            MyTileSet = A.MyTileSet;
            Encased = A.Encased;

            if (A.EditorCode1 == null) EditorCode1 = null; else EditorCode1 = string.Copy(A.EditorCode1);
            if (A.EditorCode2 == null) EditorCode2 = null; else EditorCode2 = string.Copy(A.EditorCode2);
            if (A.EditorCode3 == null) EditorCode3 = null; else EditorCode3 = string.Copy(A.EditorCode3);

            MyGuid = A.MyGuid;
            ParentObjId = A.ParentObjId;
            ParentObject = A.ParentObject;

            if (A.Associations != null)
            {
                Associations = new AssociatedObjData[A.Associations.Length];
                A.Associations.CopyTo(Associations, 0);
            }
            else
                Associations = null;

            IsGameObject = A.IsGameObject;

            AddedTimeStamp = A.AddedTimeStamp;

            ContinuousEnabled = A.ContinuousEnabled;

            VisualResettedOnce = A.VisualResettedOnce;

            HeldOffset = A.HeldOffset;
            Held = A.Held;
            Placed = A.Placed;

            DrawSubLayer = A.DrawSubLayer;

            Holdable = A.Holdable;
            EditHoldable = A.EditHoldable;

            Tag = A.Tag;

            Show = A.Show;

            DoNotScrollOut = A.DoNotScrollOut;

            MyType = A.MyType;
            BoxesOnly = A.BoxesOnly;
            AlwaysBoxesOnly = A.AlwaysBoxesOnly;

            MarkedForDeletion = A.MarkedForDeletion;
            Active = A.Active;

            DeletedByBob = A.DeletedByBob;
            OnDeletionCodeRan = A.OnDeletionCodeRan;

            MyLevel = A.MyLevel;
            DrawLayer = A.DrawLayer;
            DrawLayer2 = A.DrawLayer2;
            DrawLayer3 = A.DrawLayer3;

            DoNotDrawWithParent = A.DoNotDrawWithParent;

            Data = A.Data;
            StartData = A.StartData;

            StepOffset = A.StepOffset;

            GenData = A.GenData;

            ParentBlock = A.ParentBlock;
            ParentOffset = A.ParentOffset;
        }

        public virtual void Write(BinaryWriter writer)
        {
            writer.Write((uint)MyTileSet.Guid);
            writer.Write(EditorCode1);
            writer.Write(EditorCode2);
            writer.Write(EditorCode3);

            writer.Write(MyGuid);
            writer.Write(ParentObjId);

            writer.Write(DrawSubLayer);

            writer.Write(Tag);

            writer.Write(Show);

            writer.Write(Held);
            writer.Write(Placed);
            writer.Write(Holdable);
            writer.Write(EditHoldable);

            writer.Write(DoNotScrollOut);

            writer.Write((int)MyType);
            writer.Write(BoxesOnly);
            writer.Write(AlwaysBoxesOnly);

            writer.Write(MarkedForDeletion);
            writer.Write(Active);

            writer.Write(DrawLayer);
            //writer.Write(DrawLayer2);

            WriteReadTools.WritePhsxData(writer, Data);
            WriteReadTools.WritePhsxData(writer, StartData);

            writer.Write(StepOffset);
            //WriteReadTools.WritePhsxData(writer, GenData);

            WriteReadTools.WriteVector2(writer, ParentOffset);
        }

        public virtual void Read(BinaryReader reader)
        {
            //MyTileSet = (TileSet)reader.ReadUInt32();
            int TileGuid = (int)reader.ReadUInt32();
            MyTileSet = TileSets.GuidLookup[TileGuid];

            EditorCode1 = reader.ReadString();
            EditorCode2 = reader.ReadString();
            EditorCode3 = reader.ReadString();

            MyGuid = reader.ReadUInt64();
            ParentObjId = reader.ReadUInt64();

            DrawSubLayer = reader.ReadInt32();

            Tag = reader.ReadInt32();

            Show = reader.ReadBoolean();            

            Held = reader.ReadBoolean();
            Placed = reader.ReadBoolean();
            Holdable = reader.ReadBoolean();
            EditHoldable = reader.ReadBoolean();

            DoNotScrollOut = reader.ReadBoolean();

            MyType = (ObjectType)reader.ReadInt32();
            BoxesOnly = reader.ReadBoolean();
            AlwaysBoxesOnly = reader.ReadBoolean();

            MarkedForDeletion = reader.ReadBoolean();
            Active = reader.ReadBoolean();

            DrawLayer = reader.ReadInt32();
            //DrawLayer2 = reader.ReadInt32();

            WriteReadTools.ReadPhsxData(reader, ref Data);
            WriteReadTools.ReadPhsxData(reader, ref StartData);

            StepOffset = reader.ReadInt32();
            //WriteReadTools.WritePhsxData(Readr, GenData = reader

            WriteReadTools.ReadVector2(reader, ref ParentOffset);
        }
    }
}