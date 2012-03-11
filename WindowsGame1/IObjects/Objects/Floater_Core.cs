using System.Text;
using Microsoft.Xna.Framework;

using Drawing;

using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Floater_Core : ObjectBase, IObject, IBound
    {
        public void TextDraw() { }
        public void Release()
        {
            Core.Release();
        }

        public static int DrawLayer = 8;
        public static EzTexture ChainTexture;

        public SimpleObject MyObject;

        public CircleBox Circle;

        public ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        public virtual void MakeNew()
        {
            //Box.Initialize(Core.Data.Position, MyObject.Boxes[0].Size() / 2);

            Circle.Initialize(Core.Data.Position, MyObject.Boxes[0].Width(ref MyObject.Base) / 2);

            Init();

            Core.SkippedPhsx = true;
            Core.ContinuousEnabled = true;
        }

        public Floater_Core() { }
        public Floater_Core(bool BoxesOnly) { Construct(BoxesOnly); }

        public void Construct(bool BoxesOnly)
        {
            if (ChainTexture == null)
            {
                //ChainTexture = Tools.TextureWad.FindByName("Chain_regular_big");
                ChainTexture = Tools.TextureWad.FindByName("Chain_Tile");
            }

            CoreData = new ObjectData();

            MyObject = new SimpleObject(Prototypes.floater.MyObject, BoxesOnly);

            MyObject.Boxes[0].Animated = false;
            if (!BoxesOnly)
                MyObject.Quads[1].Animated = false;

            //Box = new AABox();
            Circle = new CircleBox();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public Floater_Core(string file)
        {
            CoreData = new ObjectData();

            ObjectClass SourceObject;
            FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);

            SourceObject = new ObjectClass(Tools.QDrawer, Tools.Device, Tools.EffectWad.FindByName("BasicEffect"), Tools.TextureWad.FindByName("White"));
            SourceObject.ReadFile(reader, Tools.EffectWad, Tools.TextureWad);
            reader.Close();
            stream.Close();

            SourceObject.ConvertForSimple();
            MyObject = new SimpleObject(SourceObject);
            MyObject.Base.e1 *= 525;// *.125f;
            MyObject.Base.e2 *= 525;// *.125f;

            MyObject.Boxes[0].Animated = false;
            MyObject.Quads[1].Animated = false;

            MyObject.Read(0, 1);
            MyObject.Play = true;
            MyObject.EnqueueAnimation(0, 0, true);
            MyObject.DequeueTransfers();
            MyObject.Update();


            Core.Data.Position = new Vector2(0, 0);
            Core.Data.Velocity = new Vector2(0, 0);

            //Box = new AABox(Core.Data.Position, MyObject.Boxes[0].Size() / 2);
            Circle = new CircleBox(Core.Data.Position, MyObject.Boxes[0].Width(ref MyObject.Base) / 2);

            Init();
        }

        public virtual void Init()
        {
            Core.Init();
            Core.DrawLayer = DrawLayer;
            Core.ContinuousEnabled = true;

            Core.GenData.OverlapWidth = 60;

            Circle.Center = Core.Data.Position;

            UpdateObject();
        }

        public Vector2 TR_Bound()
        {
            Vector2 TR = GetPos(0);
            float step = .2f;
            float t = step;
            while (t <= 1)
            {
                TR = Vector2.Max(TR, GetPos(t));
                t += step;
            }

            return TR;
        }

        public Vector2 BL_Bound()
        {
            Vector2 BL = GetPos(0);
            float step = .2f;
            float t = step;
            while (t <= 1)
            {
                BL = Vector2.Min(BL, GetPos(t));
                t += step;
            }

            return BL;
        }

        /// <summary>
        /// Get's the specified position of the floater at time t
        /// </summary>
        /// <param name="t">The parametric time variable, t = (Step + Offset) / Period</param>
        /// <returns></returns>
        public virtual Vector2 GetPos(float t)
        {
            return Vector2.Zero;
        }

        public virtual void Scale(float scale)
        {
            MyObject.Scale(scale);
            Circle.Scale(scale);
        }

        public virtual void PhsxStep()
        {
            AnimStep();

            /*
            Box.Current.Center = MyObject.Boxes[0].Center();
            Box.Current.Center.Y += 10;
            Box.Current.Size = MyObject.Boxes[0].Size() / 2f;
            Box.Current.Size.Y *= .7f;
            Box.SetTarget(Box.Current.Center, Box.Current.Size + new Vector2(.0f, .02f));

            Box.SetCurrent(Box.Current.Center, Box.Current.Size);
             */

            UpdateObject();

            Circle.Center = MyObject.Boxes[0].Center();
            Circle.Radius = .89f * MyObject.Boxes[0].Width(ref MyObject.Base) / 2f;

            if (Core.WakeUpRequirements)
            {
                //Box.SwapToCurrent();
                Circle.Invalidate();

                Core.WakeUpRequirements = false;
            }
        }

        public void PhsxStep2()
        {
            if (Core.SkippedPhsx) return;

            /*
            Box.SetCurrent(Box.Current.Center, Box.Current.Size);
            Circle.Invalidate();

            UpdateObject();
             * */
        }


        public void AnimStep() { AnimStep(Core.SkippedPhsx); }
        public void AnimStep(bool Skip)
        {
            if (Skip) return;

            MyObject.PlayUpdate(1000f / 60f / 150f * Core.IndependentDeltaT);
        }

        public void UpdateObject()
        {
            if (MyObject != null)
            {
                MyObject.Base.Origin = Core.Data.Position;
                MyObject.UpdateBoxes();
            }
        }

        public void Draw()
        {
            if (Core.SkippedPhsx) return;

            if (Tools.DrawGraphics)
            {
                MyObject.Draw(Tools.QDrawer, Tools.EffectWad);
            }
            if (Tools.DrawBoxes)
            {
                //Box.Draw(Tools.QDrawer, Color.Blue, 10);
                Circle.Draw(new Color(50, 50, 255, 120));
            }
        }

        public void MoveToBounded(Vector2 shift)
        {
            Move(shift);
        }

        public virtual void Move(Vector2 shift)
        {
            Core.StartData.Position += shift;
            Core.Data.Position += shift;

            //Box.Move(shift);
            Circle.Move(shift);

            MyObject.Base.Origin += shift;
            MyObject.Update();
        }

        public void Reset(bool BoxesOnly)
        {
            Core.Active = true;

            Core.Data.Position = Core.StartData.Position;
            Core.Data.Velocity = Vector2.Zero;
        }

        protected Bob.BobDeathType DeathType = Bob.BobDeathType.Floater;

        public AutoGen AutoGenSingleton;
        public void Interact(Bob bob)
        {
            if (!Core.SkippedPhsx)
            {
                bool Col = Circle.BoxOverlap(bob.Box2);
                
                //bool Col = false;
                //AABox box;
                //if (Core.MyLevel.PlayMode != 0)
                //{
                //    box = bob.GetBox(10);
                //    Col = Circle.BoxOverlap(box);
                //}
                //if (Col && !Circle.BoxOverlap(bob.Box2))
                //{
                //    Tools.Write("hmm");
                //    bool b = Circle.BoxOverlap(bob.Box2);
                //    Tools.Write(b);
                //}

                if (Col)
                {
                    if (Core.MyLevel.PlayMode == 0)
                        bob.Die(DeathType, this);

                    if (Core.MyLevel.PlayMode != 0)
                    {
                        bool col = Circle.BoxOverlap_Tiered(Core, bob, AutoGenSingleton);

                        if (col)
                            Core.Recycle.CollectObject(this);
                    }
                }
            }
        }

        public void CloneBoxObject(SimpleObject SimpleObjA, SimpleObject SimpleObjB)
        {
            SimpleObjA.Base = SimpleObjB.Base;
            for (int i = 0; i < SimpleObjA.Boxes.Length; i++)
            {
                SimpleObjA.Boxes[i].TR = SimpleObjB.Boxes[i].TR;
                SimpleObjA.Boxes[i].BL = SimpleObjB.Boxes[i].BL;
            }
        }

        public virtual void Clone(IObject A)
        {
            Core.Clone(A.Core);

            Core.WakeUpRequirements = true;
            UpdateObject();
        }

        public void Write(BinaryWriter writer)
        {
            Core.Write(writer);
        }
        public void Read(BinaryReader reader) { Core.Read(reader); }
//StubStubStubStart
public void OnUsed() { }
public void OnMarkedForDeletion() { }
public virtual void OnAttachedToBlock() { }
public bool PermissionToUse() { return true; }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public void Smash(Bob bob) { }
public bool PreDecision(Bob bob) { return false; }
//StubStubStubEnd7
    }
}
