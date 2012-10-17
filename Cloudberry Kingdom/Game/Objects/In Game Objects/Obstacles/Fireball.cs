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

            Core.MyType = ObjectType.Fireball;
            AutoGenSingleton = Fireball_AutoGen.Instance;
            DeathType = Bobs.Bob.BobDeathType.Fireball;

            Core.ContinuousEnabled = true;

            Radius = 40;

            Alive = true;
            PrevStep = 0;
        }

        public override void Die()
        {
            base.Die();

            Alive = false;

            if (Core.MyLevel.PlayMode == 0)
            {
                ExplodeSound.Play(1);
                Explosion(Core.Data.Position, Core.MyLevel, .33f * Core.Data.Velocity, 1, 1);
            }
        }

        public Fireball(bool BoxesOnly)
        {
            base.Construct(BoxesOnly);

            if (!Core.BoxesOnly)
            {
                MyQuad = new HsvQuad();
            }
        }

        public void Init(PhsxData data, Level level)
        {
            base.Init(data.Position, level);

            Alive = true;

            Core.Init();

            Core.Data = data;

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

                    if (!Core.BoxesOnly)
                    {
                        MyQuad.Size = new Vector2(195);
                        MyQuad.Quad.MyTexture = FireballTexture;
                        MyQuad.Set(level.Info.Fireballs.Sprite);
                        MyQuad.Show = true;
                    }
                }
            }
        }

        float PrevStep;
        Vector2 GetPos()
        {
            float Step = Core.GetPhsxStep() % Period - Offset;

            if (PrevStep < 0 && Step > 0) Alive = true;

            PrevStep = Step;

            return Core.StartData.Position + Step * Core.StartData.Velocity;
        }

        protected override void ActivePhsxStep()
        {
            if (!Alive)
            {
                Core.Active = false;
                return;
            }
            else
                Core.Active = true;
            
            Pos = GetPos();
            
            base.ActivePhsxStep();
        }

        public override void Interact(Bob bob)
        {
            if (!Alive) return;

            base.Interact(bob);
        }

        protected override void DrawGraphics()
        {
            if (!Alive || !Core.MyLevel.MainCamera.OnScreen(Pos, 300)) return;

            // Point forward
            MyQuad.PointxAxisTo(-Core.Data.Velocity);

            MyQuad.Quad.MyEffect = Tools.HslEffect;

            // Shift forward
            Vector2 dir = Core.Data.Velocity;
            dir.Normalize();

            MyQuad.Pos = Core.Data.Position - 30 * dir;

            // Draw the fireball
            MyQuad.Draw();
        }

        protected override void DrawBoxes()
        {
            Circle.Draw(new Color(50, 50, 255, 220));
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            Fireball FireballA = A as Fireball;

            Radius = FireballA.Radius;
            Period = FireballA.Period;
            Offset = FireballA.Offset;

            Init(FireballA.Core.Data, FireballA.MyLevel);
            Core.StartData = FireballA.Core.StartData;
        }
    }
}
