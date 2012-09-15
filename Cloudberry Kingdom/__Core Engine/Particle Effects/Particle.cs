using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Drawing;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom.Particles
{
    public class Particle
    {
        public static Bin<Particle> Pool = new Bin<Particle>(
            () => new Particle(),
            particle => { },
            1000);

        public void Recycle()
        {
            Pool.ReturnItem(this);
        }

        public void Copy(Particle template)
        {
            Frozen = template.Frozen;
            Code = template.Code;
            KillOffSides = template.KillOffSides;
            KillOffBottom = template.KillOffBottom;

            FadingIn = template.FadingIn;
            FadeInTargetAlpha = template.FadeInTargetAlpha;

            Data = template.Data;
            Size = template.Size;
            SizeSpeed = template.SizeSpeed;
            Angle = template.Angle;
            AngleSpeed = template.AngleSpeed;
            Life = template.Life;

            UseAttraction = template.UseAttraction;
            AttractionPoint = template.AttractionPoint;
            AttractionStrength = template.AttractionStrength;

            ColorVel = template.ColorVel;
            FadeInColorVel = template.FadeInColorVel;
            MyColor = template.MyColor;

            MyQuad = template.MyQuad;
            Base = template.Base;
        }

        public bool Frozen; public int Code;
        public bool KillOffSides, KillOffBottom;

        public bool FadingIn;
        public float FadeInTargetAlpha;

        public PhsxData Data;
        public Vector2 Size, SizeSpeed;
        public float Angle, AngleSpeed;
        public int Life;

        public bool UseAttraction;
        public Vector2 AttractionPoint;
        public float AttractionStrength;

        public Vector4 ColorVel, FadeInColorVel;
        public Vector4 MyColor;

        public SimpleQuad MyQuad;
        public BasePoint Base;

        public void Init()
        {
            MyQuad.Init();
            MyQuad.MyEffect = Tools.BasicEffect;;
        }

        public void UpdateQuad()
        {
            Base.Origin = Data.Position;
            float c = (float)Math.Cos(Angle);
            float s = (float)Math.Sin(Angle);
            Base.e1 = Size.X * new Vector2(c, s);
            Base.e2 = Size.Y * new Vector2(-s, c);

            MyQuad.SymmetricUpdate(ref Base);
        }

        public void SetSize(float size)
        {
            Size.X = Size.Y = size;
        }

        public void Phsx(Camera cam)
        {
            if (Frozen) return;

            if (FadingIn)
            {
                MyColor += FadeInColorVel;
                if (MyColor.W >= FadeInTargetAlpha)
                    FadingIn = false;
            }
            else
                MyColor += ColorVel;

            Angle += AngleSpeed;
            Size += SizeSpeed;

            if (UseAttraction)
            {
                Vector2 dif = AttractionPoint - Data.Position;
                dif.Normalize();
                Data.Velocity += dif * AttractionStrength;
            }
            else
                Data.Velocity += Data.Acceleration;

            Data.Position += Data.Velocity;
            Life -= 1;

            if (KillOffSides)
            {
                if (Data.Position.X + Size.X < cam.BL.X) Life = 0;
                if (Data.Position.X - Size.X > cam.TR.X) Life = 0;
            }
            if (KillOffBottom && Data.Position.Y + Size.Y < cam.BL.Y) Life = 0;
        }

        public void Draw()
        {
            if (KillOffSides)
            {
                if (Data.Position.X - Size.X > Tools.CurLevel.MainCamera.TR.X) return;
                if (Data.Position.X + Size.X < Tools.CurLevel.MainCamera.BL.X) return;
                if (Data.Position.Y + Size.Y < Tools.CurLevel.MainCamera.BL.Y) return;
                if (Data.Position.Y - Size.Y > Tools.CurLevel.MainCamera.TR.Y) return;
            }

            MyQuad.SetColor(new Color(MyColor));
            
            UpdateQuad();
            Tools.QDrawer.DrawQuad_Particle(ref MyQuad);
        }
    }
}
