using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#if PC_VERSION
#elif XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif
using Drawing;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
//#if NOT_PC
    public partial class CharacterSelect
    {
        public static void ScaleGamerTag(EzText GamerTag)
        {
            GamerTag.Scale *= 850f / GamerTag.GetWorldWidth();

            float Height = GamerTag.GetWorldHeight();
            float MaxHeight = 380;
            if (Height > MaxHeight)
                GamerTag.Scale *= MaxHeight / Height;
        }

        void SetGamerTag()
        {
            FancyVector2 Hold = null;
            if (GamerTag != null) Hold = GamerTag.FancyPos;

            Tools.StartGUIDraw();
            if (Player.Exists)
            {
                string name = Player.GetName();
                GamerTag = new EzText(name, Tools.Font_DylanThin42, true, true);
                ScaleGamerTag(GamerTag);
            }
            else
            {
                GamerTag = new EzText("ERROR", Tools.LilFont, true, true);
            }

            GamerTag.Shadow = false;
            GamerTag.PicShadow = false;

            if (Hold != null)
                GamerTag.FancyPos = Hold;
            else
                GamerTag.FancyPos = new FancyVector2(FancyCenter);
            //GamerTag.AddBackdrop(new Vector2(.535f, .625f), new Vector2(33, 4.5f));
            ModBackdrop(GamerTag);

            Tools.EndGUIDraw();
        }
    }
//#endif
}