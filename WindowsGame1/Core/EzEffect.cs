using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CloudberryKingdom;

namespace Drawing
{
    public class EzEffect
    {
        public EzEffectWad MyWad;

        public Effect effect;
        public string Name;
        public EffectParameter xFlip, yFlip, FlipCenter, xTexture, Illumination, FlipVector;

        public float CurrentIllumination = 0;

        /// <summary>
        /// Whether the effect has the up-to-date parameters set
        /// </summary>
        public bool IsUpToDate;
        public Vector4 MySetCameraPosition;

        public void SetCameraParameters()
        {
            Vector4 CameraPosition = MyWad.CameraPosition;

            if (CameraPosition != MySetCameraPosition)
            {
                //Vector4 cam = CameraPosition;
                //cam.Z *= -1;
                //effect.Parameters["xCameraPos"].SetValue(cam);

                effect.Parameters["xCameraPos"].SetValue(CameraPosition);
                MySetCameraPosition = CameraPosition;
            }

            IsUpToDate = true;
        }
    }

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
            return EffectList[0];
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
                Neweffect.xFlip = effect.Parameters["xFlip"];
                Neweffect.yFlip = effect.Parameters["yFlip"];
                Neweffect.FlipCenter = effect.Parameters["FlipCenter"];
                Neweffect.xTexture = effect.Parameters["xTexture"];
                Neweffect.Illumination = effect.Parameters["Illumination"];

                Neweffect.MyWad = this;

                EffectList.Add(Neweffect);
            }
        }
    }
}