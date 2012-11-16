using System;
using System.IO;
using Microsoft.Xna.Framework;






namespace CloudberryKingdom
{
    public partial class Firesnake : _CircleDeath
    {
        public class FiresnakeTileInfo : TileInfoBase
        {
            public SpriteInfo Sprite = new SpriteInfo("EmitterTexture", new Vector2(320), Vector2.Zero, Color.White);
        }

        public QuadClass MyQuad;

        Vector2 Size;

        public Vector2 Orbit;
        public Vector2 Radii;
        public int Period, Offset;

        public override void MakeNew()
        {
            base.MakeNew();

            AutoGenSingleton = Firesnake_AutoGen.Instance;
            Core.MyType = ObjectType.Firesnake;
            DeathType = Bobs.Bob.BobDeathType.Firesnake;

            PhsxCutoff_Playing = new Vector2(400);
            PhsxCutoff_BoxesOnly = new Vector2(-100, 400);

            Size = new Vector2(140);

            Core.Init();
            Core.MyType = ObjectType.Firesnake;
            Core.DrawLayer = 7;

            Orbit = Vector2.Zero;
        }

        public override void Init(Vector2 pos, Level level)
        {
            FiresnakeTileInfo info = level.Info.Firesnakes;

            Size = info.Sprite.Size * level.Info.ScaleAll * level.Info.ScaleAllObjects;

            if (!level.BoxesOnly)
            {
                MyQuad.Set(info.Sprite);
            }

            base.Init(pos, level);
        }

        public Firesnake(bool BoxesOnly)
        {
            if (!BoxesOnly)
            {
                MyQuad = new QuadClass();
            }

            Construct(BoxesOnly);
        }

        public override void PhsxStep()
        {
            double t = 2 * Math.PI * (Core.GetIndependentPhsxStep() + Offset) / (float)Period;
            Pos = CoreMath.AngleToDir(t) * Radii + Orbit;

            base.PhsxStep();
        }

        protected override void DrawGraphics()
        {
            //// Chains
            //Tools.QDrawer.DrawLine(Orbit, Core.Data.Position,
            //            new Color(255, 255, 255, 215),
            //            Info.Orbs.ChainWidth,
            //            Info.Orbs.ChainSprite.MyTexture, Tools.BasicEffect, Info.Orbs.ChainRepeatWidth, 0, 0f);

            // Draw the Firesnake
            MyQuad.Pos = Pos;
            MyQuad.Draw();
        }

        public override void Move(Vector2 shift)
        {
            base.Move(shift);

            Orbit += shift;
        }

        public override void Reset(bool BoxesOnly)
        {
            Core.Active = true;
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            Firesnake FiresnakeA = A as Firesnake;
            Init(A.Pos, A.MyLevel);

            Size = FiresnakeA.Size;

            Period = FiresnakeA.Period;
            Offset = FiresnakeA.Offset;
            Radii = FiresnakeA.Radii;
            Orbit = FiresnakeA.Orbit;
        }
    }
}
