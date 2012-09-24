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
        public EffectParameter xFlip, yFlip, FlipCenter, xTexture, ExtraTexture1_Param, ExtraTexture2_Param, Illumination, FlipVector, t, xCameraAspect, xCameraPos, Hsl;
        public EffectTechnique Simplest;

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
                xCameraPos.SetValue(CameraPosition);
                MySetCameraPosition = CameraPosition;
            }

            IsUpToDate = true;
        }

        public EzTexture ExtraTexture1;
        public void SetExtraTexture1(EzTexture texture)
        {
            if (ExtraTexture1 == texture) return;

            ExtraTexture1 = texture;

            if (texture != null) ExtraTexture1_Param.SetValue(texture.Tex);
        }

        public EzTexture ExtraTexture2;
        public void SetExtraTexture2(EzTexture texture)
        {
            if (ExtraTexture2 == texture) return;

            ExtraTexture2 = texture;

            if (texture != null) ExtraTexture2_Param.SetValue(texture.Tex);
        }
    }
}