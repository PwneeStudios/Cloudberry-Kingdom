using System.IO;
using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public partial class Fireball : _CircleDeath
    {
        public class FireballTileInfo : TileInfoBase
        {
            public SpriteInfo Sprite = new SpriteInfo(null, new Vector2(72, -1));
        }

        public override void Release()
        {
            base.Release();

            Parent = null;
        }

        public HsvQuad MyQuad;

        public int Life, StartLife;
        public ObjectBase Parent;
        public int CreationTimeStamp;

        public int FireballType;

        Vector2 Size;

        public override void MakeNew()
        {
            base.MakeNew();
        }

        public override void Die()
        {
            base.Die();

            if (Core.MyLevel.PlayMode != 0) return;

            ExplodeSound.Play(1);
            Explosion(Core.Data.Position, Core.MyLevel, .33f * Core.Data.Velocity, 1, 1);

            CollectSelf();
        }

        public Fireball(bool BoxesOnly)
        {
            base.Construct(BoxesOnly);

            if (!Core.BoxesOnly)
            {
                MyQuad = new HsvQuad();
            }
        }

        public void Init(int type, PhsxData data, Level level)
        {
            base.Init(data.Position, level);

            Core.Init();
            Core.MyType = ObjectType.Fireball;
            AutoGenSingleton = FireballEmitter_AutoGen.Instance;
            DeathType = Bobs.Bob.BobDeathType.Fireball;

            Core.DrawLayer = 7;
            Core.RemoveOnReset = true;

            Core.Data = data;
            StartLife = 50;
            Life = StartLife;

            FireballType = type;

            Radius = 50;

            if (level.Info.Fireballs.Sprite.Sprite != null)
            {
                if (MyQuad == null) MyQuad = new HsvQuad();
                MyQuad.Set(level.Info.Fireballs.Sprite);
                //MyQuad.Quad.MirrorUV_Vertical();
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

                Size = new Vector2(40);
            }
                                    
            Circle.Initialize(data.Position, Size.X);
        }

        public override void PhsxStep()
        {
            //base.PhsxStep();
            Core.SkippedPhsx = false;

            Life -= 1;
            if (Life == 0)
            {
                Vector2 Range = new Vector2(200, 200);
                if (Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, Range))
                    Life = 1;
                else
                    if (!Core.MarkedForDeletion)
                    {
                        Core.Recycle.CollectObject(this);
                        return;
                    }
            }

            Core.Data.Position += Core.Data.Velocity;
            Core.Data.Velocity += Core.Data.Acceleration;

            //Box.Target.Set(Core.Data.Position, Size);
            Circle.Center = Core.Data.Position;
        }

        float MyAlpha;
        void SetAlpha(float Alpha)
        {
            if (Alpha != MyAlpha)
            {
                MyAlpha = Alpha;
                //MyQuad.Quad.SetColor(new Color(1, 1, 1, MyAlpha));
                MyQuad.Quad.SetColor(new Color(1, .70f, .70f, MyAlpha));
            }
        }

        protected override void DrawGraphics()
        {
            if (Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + 150 || Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + 150)
                return;
            if (Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - 150 || Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - 150)
                return;

            // Fade in
            const float FadeInLength = 10;
            int Dif = StartLife - Life;
            if (Dif <= FadeInLength)
                SetAlpha(Dif / FadeInLength);
            else
                SetAlpha(1f);

            // Point forward
            MyQuad.PointxAxisTo(-Core.Data.Velocity);

            MyQuad.Quad.MyEffect = Tools.HslEffect;
            //Tools.HslEffect.Hsl.SetValue(ColorHelper.HsvTransform(0, 0, 200));

            // Shift forward
            Vector2 dir = Core.Data.Velocity;
            dir.Normalize();

            MyQuad.Pos = Core.Data.Position - 30 * dir;

            // Draw the fireball
            MyQuad.Draw();

            //Tools.QDrawer.Flush();
        }

        protected override void DrawBoxes()
        {
            Circle.Draw(new Color(50, 50, 255, 220));
        }

        public override void Move(Vector2 shift)
        {
            Core.Data.Position += shift;

            Circle.Move(shift);
        }

        public override void CollectSelf()
        {
            base.CollectSelf();

            if (MyLevel.PlayMode != 0)
                Parent.CollectSelf();
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            Fireball FireballA = A as Fireball;

            Life = FireballA.Life;
            StartLife = FireballA.StartLife;
            Parent = FireballA.Parent;
            CreationTimeStamp = FireballA.CreationTimeStamp;

            FireballType = FireballA.FireballType;

            Size = FireballA.Size;

            Init(FireballA.FireballType, FireballA.Core.Data, FireballA.MyLevel);

            StartLife = FireballA.StartLife;
        }
    }
}
