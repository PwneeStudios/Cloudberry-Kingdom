using System.IO;

using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class Sign : ObjectBase
    {
        public bool SkipPhsx;

        public QuadClass MyQuad;

        public override void MakeNew()
        {
            Core.Init();
            Core.DrawLayer = 1;
            Core.ResetOnlyOnReset = true;

            Core.EditHoldable = true;
        }

        public Sign(bool BoxesOnly, Level level)
        {
            Core.BoxesOnly = BoxesOnly;

            MakeNew();

            Core.BoxesOnly = BoxesOnly;

            var info = level.Info.Doors;
            MyQuad = new QuadClass();
            MyQuad.Set(info.Sign);
        }

        int Count = 1;
        bool OnState = true;
        void SetState(bool NewOnState)
        {
            OnState = NewOnState;

            if (NewOnState) MyQuad.Quad.CalcTexture(0, 0);
            else MyQuad.Quad.CalcTexture(0, 1);

            Count = 1;
        }

        public static int OffLength = 68, OnLength = 58;
        public override void PhsxStep()
        {
            Count++;

            if (!OnState && Count == OffLength) { SetState(true); Count = 0; }
            if (OnState && Count == OnLength) { SetState(false); Count = 0; }
        }

        bool OnScreen()
        {
            if (Core.BoxesOnly) return false;

            if (Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + 150 + MyQuad.Base.e1.X || Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + 150 + MyQuad.Base.e2.Y)
                return false;
            if (Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - 150 - MyQuad.Base.e1.X || Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - 500 - MyQuad.Base.e2.Y)
                return false;

            return true;
        }

        public override void Draw()
        {
            if (!OnScreen()) return;

            if (Tools.DrawGraphics)
            {
                if (!Info.Doors.Show || !Info.Doors.ShowSign) return;

                float x = MyQuad.SizeX;
                if (Core.MyLevel != null && Core.MyLevel.ModZoom.X < 0)
                {
                    if (MyQuad.Base.e1.X > 0)
                        MyQuad.SizeX = -x;
                }
                else
                {
                    if (MyQuad.Base.e1.X < 0)
                        MyQuad.SizeX = -x;
                }

                MyQuad.Draw();
            }
        }

        public Vector2 GetBottom()
        {
            MyQuad.Update();
            return new Vector2(Core.Data.Position.X, MyQuad.BL.Y + 11.5f);
        }

        /// <summary>
        /// Moves the foot of the Sign to the specified position.
        /// </summary>
        /// <param name="pos"></param>
        public void PlaceAt(Vector2 pos)
        {
            Move(pos - GetBottom() - new Vector2(0, 16));
        }

        public void Update()
        {
            MyQuad.Base.Origin = Core.Data.Position;
        }

        public override void Move(Vector2 shift)
        {
            Core.Data.Position += shift;
            Update();
        }

        public override void Reset(bool BoxesOnly)
        {
            Core.Active = true;
        }

        public override void Write(BinaryWriter writer)
        {
            Core.Write(writer);

            MyQuad.Write(writer);
        }
        public override void Read(BinaryReader reader)
        {
            Core.Read(reader);

            MyQuad.Read(reader);
        }
    }
}