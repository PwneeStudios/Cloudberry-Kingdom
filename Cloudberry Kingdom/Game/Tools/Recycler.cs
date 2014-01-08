using System;
using System.Collections.Generic;
using System.Linq;

using CloudberryKingdom.Obstacles;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.InGameObjects;

namespace CloudberryKingdom
{
    public enum ObjectType { 
                                Undefined,
                                Coin, Checkpoint, BerryBubble,
                                FlyingBlob, BlockEmitter, Spike, Fireball, FireSpinner, Boulder, Laser,
                                NormalBlock, FallingBlock, LavaBlock, MovingPlatform, MovingBlock, GhostBlock,
                                Cloud, BouncyBlock, SpikeyGuy, SpikeyLine,
                                Pendulum, Serpent, LavaDrip, Firesnake, ConveyorBlock,
                                Door, Wall,
                                ZoneTrigger, CameraZone,
								Length
                           };

    public class RecycleBin
    {
        ObjectType MyType;
        Stack<ObjectBase> FullObject, BoxObject;

        public void Release()
        {
            foreach (ObjectBase obj in FullObject)
                if (obj.CoreData.MyLevel == null)
                    obj.Release();

            foreach (ObjectBase obj in BoxObject)
                if (obj.CoreData.MyLevel == null)
                    obj.Release();
        }

        public RecycleBin(ObjectType type)
        {
            MyType = type;

            FullObject = new Stack<ObjectBase>();
            BoxObject = new Stack<ObjectBase>();
        }

        public ObjectBase GetObject(bool BoxesOnly)
        {
            if (BoxesOnly) return GetObject_BoxesOnly();
            else return GetObject_Graphical();
        }
        ObjectBase GetObject_BoxesOnly() { return __GetObject(true); }
        ObjectBase GetObject_Graphical() { return __GetObject(false); }
        ObjectBase __GetObject(bool BoxesOnly)
        {
            ObjectBase obj = null;

            //lock (this)
            {                
                if (BoxesOnly)
                {
                    if (BoxObject.Count > 0) obj = BoxObject.Pop();
                }
                else
                {
                    if (FullObject.Count > 0)
                    {
                        obj = FullObject.Pop();
                    }
                }

                if (obj != null)
                    obj.MakeNew();
                else
                    obj = NewObject(BoxesOnly);
            }

            return obj;
        }

        public void CollectObject(ObjectBase obj)
        {
            if (obj.CoreData.MarkedForDeletion)
                return;
            
            obj.CoreData.MarkedForDeletion = true;
            obj.CoreData.Active = false;
            obj.CoreData.Show = false;

            // If the object belongs to a level, add this object to the level's
            // pre-recycle bin, to be actually recycled when the level cleans its
            // object lists.
            if (obj.CoreData.MyLevel != null)
            {
                obj.CoreData.MyLevel.PreRecycleBin.Add(obj);
                return;
            }

            //lock (this)
            {
                if (obj.CoreData.BoxesOnly) BoxObject.Push(obj);
                else
                {
//                    if (FullObject.Contains(obj))
  //                      Tools.Write("@@@@@@ Double recyled!");
                    FullObject.Push(obj);
                }
            }
        }

        public ObjectBase NewObject(bool BoxesOnly)
        {
            switch (MyType)
            {
                case ObjectType.FlyingBlob:
                    return new FlyingBlob(BoxesOnly);
                case ObjectType.BlockEmitter:
                    return new BlockEmitter(BoxesOnly);
                case ObjectType.Coin:
                    return new Coin(BoxesOnly);
                case ObjectType.Spike:
                    return new Spike(BoxesOnly);
                case ObjectType.Fireball:
                    return new Fireball(BoxesOnly);
                case ObjectType.FireSpinner:
                    return new FireSpinner(BoxesOnly);
                case ObjectType.NormalBlock:
                    return new NormalBlock(BoxesOnly);
                case ObjectType.MovingPlatform:
                    return new MovingPlatform(BoxesOnly);
                case ObjectType.MovingBlock:
                    return new MovingBlock(BoxesOnly);
                case ObjectType.FallingBlock:
                    return new FallingBlock(BoxesOnly);
                case ObjectType.BouncyBlock:
                    return new BouncyBlock(BoxesOnly);
                case ObjectType.LavaBlock:
                    //return new LavaBlock(BoxesOnly);
                    return new LavaBlock_Castle(BoxesOnly);
                case ObjectType.Boulder:
                    return new Boulder(BoxesOnly);
                case ObjectType.SpikeyGuy:
                    return new SpikeyGuy(BoxesOnly);
                case ObjectType.CameraZone:
                    return new CameraZone();
                case ObjectType.Door:
                    return new Door(BoxesOnly);
                case ObjectType.Checkpoint:
                    return new Checkpoint();
                case ObjectType.Laser:
                    return new Laser(BoxesOnly);
                case ObjectType.Cloud:
                    return new Cloud(BoxesOnly);
                case ObjectType.GhostBlock:
                    return new GhostBlock(BoxesOnly);
                case ObjectType.ConveyorBlock:
                    return new ConveyorBlock(BoxesOnly);
                case ObjectType.SpikeyLine:
                    return new SpikeyLine(BoxesOnly);
                case ObjectType.Firesnake:
                    return new Firesnake(BoxesOnly);
                case ObjectType.BerryBubble:
                    return new BerryBubble(BoxesOnly);

                case ObjectType.Pendulum:
                    return new Pendulum(BoxesOnly);
                case ObjectType.Serpent:
                    return new Obstacles.Serpent(BoxesOnly);
                case ObjectType.LavaDrip:
                    return new Obstacles.LavaDrip(BoxesOnly);

                default:
                    return null;
                    throw (new System.Exception("No type found for desired object"));
            }
        }
    }

    public class Bin<T> where T : class
    {
        Func<T> CreateNew;
        Action<T> MakeNew;
        Stack<T> MyStack;
        public Bin(Func<T> CreateNew, Action<T> MakeNew, int capacity)
        {
            this.CreateNew = CreateNew;
            this.MakeNew = MakeNew;
            MyStack = new Stack<T>(capacity);

            for (int i = 0; i < capacity; i++)
                MyStack.Push(CreateNew());
        }

        public T Get()
        {
            T item = null;

            lock (MyStack)
            {
                if (MyStack.Count == 0)
                    return CreateNew();

                item = MyStack.Pop();
                MakeNew(item);
            }

            return item;
        }
        public void ReturnItem(T T)
        {
            lock (MyStack)
            {
                MyStack.Push(T);
            }
        }
    }

    public class Recycler
    {
        static int MetaCount = 0;
        static Stack<Recycler> MetaBin = new Stack<Recycler>();
        public static Recycler GetRecycler()
        {
            Recycler bin = null;

            lock (MetaBin)
            {
                MetaCount++;
                if (MetaBin.Count == 0)
                    return new Recycler();

                bin = MetaBin.Pop();
            }

            return bin;
        }
        public static void ReturnRecycler(Recycler recycler)
        {
            recycler.Empty();
            lock (MetaBin)
            {
                MetaCount--;
                MetaBin.Push(recycler);
            }
        }
        public static void DumpMetaBin()
        {
            lock (MetaBin)
            {
                foreach (Recycler recycler in MetaBin)
                    recycler.Empty(false);
                GC.Collect();
            }
        }

        //Dictionary<ObjectType, RecycleBin> Bins;
        RecycleBin[] Bins;
        
        public Recycler()
        {
            Init();
        }

        public void Init()
        {
			int N = (int)ObjectType.Length;
            Bins = new RecycleBin[N];
        }

        public ObjectBase GetNewObject(ObjectType type, bool BoxesOnly)
        {
            if (type == ObjectType.Undefined)
                return null;

            if (Bins[(int)type] == null)
                Bins[(int)type] = new RecycleBin(type);

            ObjectBase obj = Bins[(int)type].NewObject(BoxesOnly);

            return obj;
        }

        //public ObjectBase this[ObjectType type, bool BoxesOnly]
        //{
        //    get { return GetObject(type, BoxesOnly); }
        //}

        public ObjectBase GetObject(ObjectType type, bool BoxesOnly)
        {
            //if (type == ObjectType.FlyingBlob)
            //    Tools.Write("!");

            if (type == ObjectType.Undefined)
                //throw (new System.Exception("No type found for desired object"));
                return null;

            //if (!Bins.CustomContainsKey(type))
              //  Bins.Add(type, new RecycleBin(type));

            if (Bins[(int)type] == null)
                Bins[(int)type] = new RecycleBin(type);

            //return Bins[type].GetObject(BoxesOnly);
            ObjectBase obj = Bins[(int)type].GetObject(BoxesOnly);

            return obj;
        }

        public void CollectObject(ObjectBase obj) { CollectObject(obj, true); }
        public void CollectObject(ObjectBase obj, bool CollectAssociates)
        {
            if (obj == null || obj.CoreData.MarkedForDeletion)
                return;

            //if (obj is Serpent) Tools.Write("!");

            // Actions to be taken when the object is deleted
            if (!obj.CoreData.OnDeletionCodeRan)
            {
                if (obj.CoreData.GenData.OnMarkedForDeletion != null)
                    obj.CoreData.GenData.OnMarkedForDeletion();
                obj.OnMarkedForDeletion();

                obj.CoreData.OnDeletionCodeRan = true;
            }

            // Get the object type
            ObjectType type = obj.CoreData.MyType;
            if (type == ObjectType.Undefined)
            {
                obj.CoreData.MarkedForDeletion = true;
                obj.CoreData.Active = false;
                obj.CoreData.Show = false;
                return;
            }

            if (Bins[(int)type] == null)
                Bins[(int)type] = new RecycleBin(type);

            Bins[(int)type].CollectObject(obj);

            // Collect associate objects
            if (CollectAssociates && obj.CoreData.Associations != null)
                for (int i = 0; i < obj.CoreData.Associations.Length; i++)
                    if (obj.CoreData.Associations[i].Guid > 0)
                    {
                        ObjectBase _obj = obj.CoreData.MyLevel.LookupGUID(obj.CoreData.Associations[i].Guid);
                        if (_obj == null) continue;

                        // Delete the associated object if DeleteWhenDeleted flag is set
                        if (obj.CoreData.Associations[i].DeleteWhenDeleted)
                            CollectObject(_obj);
                        // Otherwise remove the association
                        else
                        {
                            if (_obj.CoreData.Associations != null)
                            {
                                for (int j = 0; j < _obj.CoreData.Associations.Length; j++)
                                    if (_obj.CoreData.Associations[j].Guid == obj.CoreData.MyGuid)
                                        _obj.CoreData.Associations[j].Guid = 0;
                            }
                        }
                    }
        }

        public void Empty() { Empty(true); }
        public void Empty(bool DoGC)
        {
            for (int i = 0; i < Bins.Length; i++)
                if (Bins[i] != null)
                    Bins[i].Release();

            Init();

            if (DoGC)
                GC.Collect();
        }
    }
}
