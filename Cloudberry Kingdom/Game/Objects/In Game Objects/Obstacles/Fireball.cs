using System.IO;
using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Obstacles
{
    public partial class Fireball : _CircleDeath
    {
        public class FireballTileInfo : TileInfoBase
        {
            public SpriteInfo Sprite = new SpriteInfo(null, new Vector2(72, -1));
        }

        public HsvQuad MyQuad;

        public int Period, Offset;

        bool Alive;

        public override void MakeNew()
        {
            base.MakeNew();

            CoreData.MyType = ObjectType.Fireball;
            AutoGenSingleton = Fireball_AutoGen.Instance;
            DeathType = Bobs.BobDeathType.Fireball;

            PhsxCutoff_Playing = new Vector2(10000);
            PhsxCutoff_BoxesOnly = new Vector2(10000);

            CoreData.ContinuousEnabled = true;

            CoreData.DrawLayer = 8;

            Radius = 40;

            Alive = true;
            PrevStep = 0;
        }

        public override void Die()
        {
            base.Die();

            Alive = false;

            if (CoreData.MyLevel.PlayMode == 0)
            {
                ExplodeSound.Play(1);
                Explosion(CoreData.Data.Position, CoreData.MyLevel, .33f * CoreData.Data.Velocity, 1, 1);
            }
        }

        public Fireball(bool BoxesOnly)
        {
            base.Construct(BoxesOnly);

            if (!CoreData.BoxesOnly)
            {
                MyQuad = new HsvQuad();
            }
        }

        public void Init(PhsxData data, Level level)
        {
            base.Init(data.Position, level);

            Alive = true;

            CoreData.Data = data;

            if (!level.BoxesOnly)
            {
                if (level.Info.Fireballs.Sprite.Sprite != null)
                {
                    if (MyQuad == null) MyQuad = new HsvQuad();
                    MyQuad.Set(level.Info.Fireballs.Sprite);
                }
                else
                {
                    if (MyQuad == null) MyQuad = new HsvQuad();

                    if (!CoreData.BoxesOnly)
                    {
                        MyQuad.Size = new Vector2(195);
						if (CloudberryKingdomGame.RenderFireball)
							MyQuad.Quad.MyTexture = FireballTexture;
						else
							MyQuad.Quad.MyTexture = Tools.TextureWad.TextureList[1];
                        MyQuad.Set(level.Info.Fireballs.Sprite);
                        MyQuad.Show = true;
                    }
                }
            }
        }

        float PrevStep;
        Vector2 GetPos()
        {
            //if (!Alive) Tools.Write("!");

            float Step = (CoreData.MyLevel.IndependentPhsxStep - Offset + Period) % Period;

            if (PrevStep > Step) Alive = true;

            PrevStep = Step;

            //Tools.Write(Core.StartData.Velocity.Length());
            return CoreData.StartData.Position + Step * CoreData.StartData.Velocity;
        }

        protected override void ActivePhsxStep()
        {
            if (!Alive)
            {
                CoreData.Active = false;
                CoreData.Data.Position = GetPos();
                return;
            }
            else
                CoreData.Active = true;
            
            CoreData.Data.Position = GetPos();
            
            base.ActivePhsxStep();
        }

        public override void Interact(Bob bob)
        {
            if (!Alive) return;

            base.Interact(bob);
        }

        protected override void DrawGraphics()
        {
            if (!Alive || !CoreData.MyLevel.MainCamera.OnScreen(CoreData.Data.Position, 300)) return;

            // Point forward
            MyQuad.PointxAxisTo(-CoreData.Data.Velocity);

            MyQuad.Quad.MyEffect = Tools.HslEffect;

            // Shift forward
            Vector2 dir = CoreData.Data.Velocity;
            dir.Normalize();

            MyQuad.Pos = CoreData.Data.Position - 30 * dir;

            // Draw the fireball
            MyQuad.Draw();
        }

        protected override void DrawBoxes()
        {
            Circle.Draw(new Color(50, 50, 255, 220));
        }

        public override void Clone(ObjectBase A)
        {
            CoreData.Clone(A.CoreData);

            Fireball FireballA = A as Fireball;

            Radius = FireballA.Radius;
            Period = FireballA.Period;
            Offset = FireballA.Offset;

            Init(FireballA.CoreData.Data, FireballA.MyLevel);
            CoreData.StartData = FireballA.CoreData.StartData;
        }
    }
}
