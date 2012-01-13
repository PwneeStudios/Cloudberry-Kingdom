using System;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class SpikeyLine : Floater_Core, IObject, IBound
    {
        public int Period, Offset;
        public Vector2 p1, p2;

        public override void MakeNew()
        {
            AutoGenSingleton = Floater_AutoGen.Instance;
            Core.MyType = ObjectType.SpikeyLine;
            Core.ContinuousEnabled = true;

            Period = 150;
            Offset = 0;
            p1 = new Vector2(0, 1300);
            p2 = new Vector2(0, -1300);

            base.MakeNew();

            //if (!Core.BoxesOnly)
            //{
            //    var color = new Vector4(.5f);
            //    MyObject.SetColor(new Color(color));
            //}

            Core.DrawLayer = 2;
            Core.DrawLayer2 = 6;
        }

        public SpikeyLine(bool BoxesOnly)
        {
            base.Construct(BoxesOnly);

            //if (!Core.BoxesOnly)
            //{
            //    MyObject.Quads[0].MyTexture = Tools.TextureWad.FindByName("SpikeBlob");
            //    MyObject.Quads[1].MyTexture = Tools.TextureWad.FindByName("SpikeyGuy4");
            //}
        }


        /// <summary>
        /// Get's the specified position of the floater at time t
        /// </summary>
        /// <param name="t">The parametric time variable, t = (Step + Offset) / Period</param>
        /// <returns></returns>
        public override Vector2 GetPos(float t)
        {
            Vector2 pos = Vector2.Lerp(p1, p2, t);
            return pos;
        }

        public override void PhsxStep()
        {
            float PhsxCutoff = 360;
            if (!Core.MyLevel.MainCamera.OnScreen(p1, PhsxCutoff) && !Core.MyLevel.MainCamera.OnScreen(p2, PhsxCutoff))
            {
                Core.SkippedPhsx = true;
                Core.WakeUpRequirements = true;
                return;
            }
            Core.SkippedPhsx = false;

            //int Step = (Core.MyLevel.GetPhsxStep() + Offset) % Period;
            int Step = Tools.Modulo(Core.MyLevel.GetPhsxStep() + Offset, Period);
            float t = (float)Step / (float)Period;

            Vector2 Pos = GetPos(t);

            Core.Data.Position = Pos;
            Tools.PointxAxisToAngle(ref MyObject.Base, Core.MyLevel.CurPhsxStep / 20f);

            base.PhsxStep();
        }


        bool OffScreen = false;
        public new void Draw()
        {
            if (Core.SkippedPhsx) return;

            if (Core.MyLevel.CurrentDrawLayer == Core.DrawLayer)
            {
                if (Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, 200))
                    OffScreen = false;
                else
                {
                    OffScreen = true;
                    return;
                }
            }
            else
                if (OffScreen) return;

            if (Tools.DrawGraphics)
            {
                MyObject.UpdateQuads();
                MyObject.Draw(Tools.QDrawer, Tools.EffectWad);
            }
            if (Tools.DrawBoxes)
            {
                //Box.Draw(Tools.QDrawer, Color.Blue, 10);
                Circle.Draw(new Color(50, 50, 255, 220));
            }
        }

        public override void Move(Vector2 shift)
        {
            p1 += shift;
            p2 += shift;

            base.Move(shift);            
        }

        public override void Clone(IObject A)
        {
            Core.Clone(A.Core);

            SpikeyLine FloaterA = A as SpikeyLine;

            Period = FloaterA.Period;
            Offset = FloaterA.Offset;
            p1 = FloaterA.p1;
            p2 = FloaterA.p2;

            Core.WakeUpRequirements = true;
            UpdateObject();
        }
    }
}
