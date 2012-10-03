using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CloudberryKingdom;

namespace CoreEngine
{
    public class EzEffectWad
    {
        public Vector2 ModZoom = Vector2.One;

        public List<EzEffect> EffectList;

        Vector4 _CameraPosition;
        public Vector4 CameraPosition { get { return _CameraPosition; } set { _CameraPosition = value; } }
        public void SetCameraPosition(Vector4 CameraPosition)
        {
            //CameraPosition.Z *= -1;
            CameraPosition.Z *= ModZoom.X;
            CameraPosition.W *= ModZoom.Y;

            this.CameraPosition = CameraPosition;

            foreach (EzEffect effect in EffectList)
                effect.IsUpToDate = false;
        }

        Vector4 HoldCamPos;
        public void SetDefaultZoom()
        {
            HoldCamPos = CameraPosition;
            SetCameraPosition(new Vector4(CameraPosition.X, CameraPosition.Y, .001f, .001f));
        }

        public void ResetCameraPos()
        {
            SetCameraPosition(HoldCamPos);
        }

        public EzEffectWad()
        {
            EffectList = new List<EzEffect>();
        }

        public EzEffect FindByName(string name)
        {
            foreach (EzEffect effect in EffectList)
                if (effect.Name.CompareTo(name) == 0)
                    return effect;
            return Tools.BasicEffect;
        }

        public void AddEffect(Effect effect, string Name)
        {
            if (EffectList.Exists(match => string.Compare(match.Name, Name, StringComparison.OrdinalIgnoreCase) == 0))
            {
                FindByName(Name).effect = effect;
            }
            else
            {
                EzEffect Neweffect = new EzEffect();
                Neweffect.Name = Name;
                Neweffect.effect = effect;

                Neweffect.FlipVector = effect.Parameters["FlipVector"];
                Neweffect.FlipCenter = effect.Parameters["FlipCenter"];
                Neweffect.xTexture = effect.Parameters["xTexture"];
                Neweffect.Illumination = effect.Parameters["Illumination"];
                Neweffect.t = effect.Parameters["t"];
                Neweffect.xCameraAspect = effect.Parameters["xCameraAspect"];
                Neweffect.xCameraPos = effect.Parameters["xCameraPos"];

                Neweffect.ExtraTexture1_Param = effect.Parameters["ExtraTexture1"];
                Neweffect.ExtraTexture2_Param = effect.Parameters["ExtraTexture2"];

                Neweffect.Hsl = effect.Parameters["ColorMatrix"];

                Neweffect.Simplest = effect.Techniques["Simplest"];

                Neweffect.MyWad = this;

                EffectList.Add(Neweffect);
            }
        }
    }
}