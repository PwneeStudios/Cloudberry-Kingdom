#define XBOX

using System;
using System.Collections.Generic;
using System.Linq;





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
                                ZoneTrigger, CameraZone
                           };

    public class RecycleBin
    {
        ObjectType MyType;
        Stack<ObjectBase> FullObject, BoxObject;

        public void Release()
        {
            foreach (ObjectBase obj in FullObject)
                if (obj.Core.MyLevel == null)
                    obj.Release();

            foreach (ObjectBase obj in BoxObject)
                if (obj.Core.MyLevel == null)
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
            if (obj.Core.MarkedForDeletion)
                return;
            
            obj.Core.MarkedForDeletion = true;
            obj.Core.Active = false;
            obj.Core.Show = false;

            // If the object belongs to a level, add this object to the level's
            // pre-recycle bin, to be actually recycled when the level cleans its
            // object lists.
            if (obj.Core.MyLevel != null)
            {
                obj.Core.MyLevel.PreRecycleBin.Add(obj);
                return;
            }

            //lock (this)
            {
                if (obj.Core.BoxesOnly) BoxObject.Push(obj);
                else
                {
//                    if (FullObject.Contains(obj))
  //                      Console.WriteLine("@@@@@@ Double recyled!");
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
                    return new Serpent(BoxesOnly);
                case ObjectType.LavaDrip:
                    return new LavaDrip(BoxesOnly);

                default:
                    return null;
                    throw (new System.Exception("No type found for desired object"));
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
            //Bins = new Dictionary<ObjectType, RecycleBin>();
            int N = Tools.GetValues<ObjectType>().Count();//Enum.GetValues(typeof(ObjectType)).Length;
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

        public ObjectBase this[ObjectType type, bool BoxesOnly]
        {
            get { return GetObject(type, BoxesOnly); }
        }

        public ObjectBase GetObject(ObjectType type, bool BoxesOnly)
        {
            //if (type == ObjectType.FlyingBlob)
            //    Tools.Write("!");

            if (type == ObjectType.Undefined)
                //throw (new System.Exception("No type found for desired object"));
                return null;

            //if (!Bins.ContainsKey(type))
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
            if (obj == null || obj.Core.MarkedForDeletion)
                return;

            // Actions to be taken when the object is deleted
            if (!obj.Core.OnDeletionCodeRan)
            {
                if (obj.Core.GenData.OnMarkedForDeletion != null)
                    obj.Core.GenData.OnMarkedForDeletion.Apply();
                obj.OnMarkedForDeletion();

                obj.Core.OnDeletionCodeRan = true;
            }

            // Get the object type
            ObjectType type = obj.Core.MyType;
            if (type == ObjectType.Undefined)
            {
                obj.Core.MarkedForDeletion = true;
                obj.Core.Active = false;
                obj.Core.Show = false;
                return;
            }

            if (Bins[(int)type] == null)
                Bins[(int)type] = new RecycleBin(type);

            Bins[(int)type].CollectObject(obj);

            // Collect associate objects
            if (CollectAssociates && obj.Core.Associations != null)
                for (int i = 0; i < obj.Core.Associations.Length; i++)
                    if (obj.Core.Associations[i].Guid > 0)
                    {
                        ObjectBase _obj = obj.Core.MyLevel.LookupGUID(obj.Core.Associations[i].Guid);
                        if (_obj == null) continue;

                        // Delete the associated object if DeleteWhenDeleted flag is set
                        if (obj.Core.Associations[i].DeleteWhenDeleted)
                            CollectObject(_obj);
                        // Otherwise remove the association
                        else
                        {
                            if (_obj.Core.Associations != null)
                            {
                                for (int j = 0; j < _obj.Core.Associations.Length; j++)
                                    if (_obj.Core.Associations[j].Guid == obj.Core.MyGuid)
                                        _obj.Core.Associations[j].Guid = 0;
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
