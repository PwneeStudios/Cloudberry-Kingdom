using System.Text;
using Microsoft.Xna.Framework;

using Drawing;

using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Floater_Core : ObjectBase, IBound
    {
        public static int DrawLayer = 8;
        public static EzTexture ChainTexture;

        public SimpleObject MyObject;

        public CircleBox Circle;

        public override void MakeNew()
        {
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

            MyObject = new SimpleObject(Prototypes.SpikeyGuyObj.MyObject, BoxesOnly);

            MyObject.Boxes[0].Animated = false;
            if (!BoxesOnly)
                MyObject.Quads[1].Animated = false;

            Circle = new CircleBox();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public Floater_Core(string file)
        {
            ObjectClass SourceObject;
            Tools.UseInvariantCulture();
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

        public float Radius = 120;
        public override void PhsxStep()
        {
            AnimStep();
            UpdateObject();

            Circle.Center = MyObject.Boxes[0].Center();
            Circle.Radius = Radius;
            //Circle.Radius = .89f * MyObject.Boxes[0].Width(ref MyObject.Base) / 2f;

            if (Core.WakeUpRequirements)
            {
                Circle.Invalidate();

                Core.WakeUpRequirements = false;
            }
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

        public override void Draw()
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

        public override void Move(Vector2 shift)
        {
            Core.StartData.Position += shift;
            Core.Data.Position += shift;

            //Box.Move(shift);
            Circle.Move(shift);

            MyObject.Base.Origin += shift;
            MyObject.Update();
        }

        public override void Reset(bool BoxesOnly)
        {
            Core.Active = true;

            Core.Data.Position = Core.StartData.Position;
            Core.Data.Velocity = Vector2.Zero;
        }

        protected Bob.BobDeathType DeathType = Bob.BobDeathType.Floater;

        public AutoGen AutoGenSingleton;
        public override void Interact(Bob bob)
        {
            if (!Core.SkippedPhsx)
            {
                bool Col = Circle.BoxOverlap(bob.Box2);
                
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

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            Core.WakeUpRequirements = true;
            UpdateObject();
        }
    }
}
