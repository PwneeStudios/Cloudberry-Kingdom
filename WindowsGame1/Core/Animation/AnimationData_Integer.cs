using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.IO;

namespace Drawing
{
    public struct OneAnim_Texture { public EzTexture[] Data; }
    public struct AnimationData_Texture
    {
        /// <summary>
        /// If false, only changing values are recorded
        /// </summary>
        public static bool RecordAll;

        public OneAnim_Texture[] Anims;

        public bool Linear;

        public int Hold;

        public void Release()
        {
            if (Anims != null)
                for (int i = 0; i < Anims.Length; i++)
                    Anims[i].Data = null;
            Anims = null;
        }

        public void Write(BinaryWriter writer)
        {
            if (Anims == null) writer.Write(-1);
            else
            {
                writer.Write(Anims.Length);
                for (int i = 0; i < Anims.Length; i++)
                    WriteReadTools.WriteOneAnim(writer, Anims[i]);
            }
        }

        public void Read(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            if (length == -1) Anims = null;
            else
            {
                Anims = new OneAnim_Texture[length];
                for (int i = 0; i < length; i++)
                    WriteReadTools.ReadOneAnim(reader, ref Anims[i]);
            }
        }

        public AnimationData_Texture(AnimationData_Texture data)
        {
            Linear = false;

            Hold = 0;
            Anims = new OneAnim_Texture[data.Anims.Length];
            for (int i = 0; i < data.Anims.Length; i++)
                CopyAnim(data, i);
        }

        public void CopyAnim(AnimationData_Texture data, int Anim)
        {
            if (data.Anims[Anim].Data != null)
            {
                Anims[Anim].Data = new EzTexture[data.Anims[Anim].Data.Length];
                data.Anims[Anim].Data.CopyTo(Anims[Anim].Data, 0);
            }
            else
                Anims[Anim].Data = null;
        }

        //public AnimationData_Integer()
        public void Init()
        {
            Anims = new OneAnim_Texture[] { new OneAnim_Texture() };
            Hold = 0;
        }

        public void InsertFrame(int anim, int frame)
        {
            if (anim >= Anims.Length) return;
            if (Anims[anim].Data == null) return;
            if (frame >= Anims[anim].Data.Length) return;

            OneAnim_Texture NewAnim = new OneAnim_Texture();
            NewAnim.Data = new EzTexture[Anims[anim].Data.Length + 1];
            for (int i = 0; i < frame; i++) NewAnim.Data[i] = Anims[anim].Data[i];
            NewAnim.Data[frame] = Anims[anim].Data[frame];
            for (int i = frame + 1; i < Anims[anim].Data.Length + 1; i++) NewAnim.Data[i] = Anims[anim].Data[i-1];
            Anims[anim] = NewAnim;
        }

        public void DeleteFrame(int anim, int frame)
        {
            if (anim >= Anims.Length) return;
            if (Anims[anim].Data == null) return;
            if (frame >= Anims[anim].Data.Length) return;

            if (Anims[anim].Data.Length > 1)
            {
                OneAnim_Texture NewAnim = new OneAnim_Texture();
                NewAnim.Data = new EzTexture[Anims[anim].Data.Length - 1];
                for (int i = 0; i < frame; i++) NewAnim.Data[i] = Anims[anim].Data[i];
                for (int i = frame + 1; i < Anims[anim].Data.Length; i++) NewAnim.Data[i - 1] = Anims[anim].Data[i];
                Anims[anim] = NewAnim;
            }
        }

        public void AddFrame(EzTexture val, int anim)
        {
            int frame = 0;
            if (anim >= Anims.Length) frame = 0;
            else if (Anims[anim].Data == null) frame = 0;
            else frame = Anims[anim].Data.Length;

            Set(val, anim, frame);
        }
        public void Set(EzTexture val, int anim, int frame)
        {
            EzTexture Default = null;
            if (Anims[0].Data != null)
            {
                Default = Anims[0].Data[0];
            }

            if (anim >= Anims.Length)
            {
                OneAnim_Texture[] NewAnims = new OneAnim_Texture[anim + 1];
                Anims.CopyTo(NewAnims, 0);
                Anims = NewAnims;
            }

            if (Anims[anim].Data == null)
                Anims[anim].Data = new EzTexture[] { Default };
            else
                if (frame > 0)
                    Default = Get(anim, frame - 1);

            if (frame >= Anims[anim].Data.Length && !(val == Default && Anims[anim].Data.Length <= 1))
            {
                EzTexture[] NewData = new EzTexture[frame + 1];
                for (int i = 0; i < frame + 1; i++) NewData[i] = Default;
                Anims[anim].Data.CopyTo(NewData, 0);
                Anims[anim].Data = NewData;
            }

            if (frame < Anims[anim].Data.Length)
                Anims[anim].Data[frame] = val;
        }

        public EzTexture Get(int anim, int frame)
        {
            EzTexture Default = null;
            if (Anims[0].Data != null)
                Default = Anims[0].Data[0];


            if (anim >= Anims.Length)
                return Default;

            if (Anims[anim].Data == null)
                return Default;
            else
            {
                int Length = Anims[anim].Data.Length;
                if (Length > 0)
                {
                    if (frame >= Length)
                        Default = Anims[anim].Data[Length - 1];
                    else if (frame > 0)
                        Default = Get(anim, frame - 1);
                    else
                        Default = Anims[anim].Data[0];
                }
                else
                    return Default;
            }
            if (frame >= Anims[anim].Data.Length || frame < 0)
                return Default;

            return Anims[anim].Data[frame];
        }

        public int Transfer(int DestAnim, float DestT, int DestLength, bool DestLoop, bool DestLinear, float t)
        {
            return Hold;
            //int v1 = Hold;
            //int v2 = Calc(DestAnim, DestT, DestLength, DestLoop, DestLinear);

            //return int.Lerp(v1, v2, t);
        }

        public EzTexture Calc(int anim, float t, int Length, bool Loop, bool Linear)
        {
            if (Linear)
            {
                EzTexture v2, v3;
                int i = (int)Math.Floor(t);
                if (!Loop)
                {
                    v2 = Get(anim, i);
                    //v3 = Get(anim, (int)Math.Min(Length, i + 1));
                }
                else
                {
                    v2 = Get(anim, i);
                    //if (i + 1 <= Length) v3 = Get(anim, i + 1);
                    //else v3 = Get(anim, i + 1 - Length - 1);
                }

                //return int.Lerp(v2, v3, t - i);
                return v2;
            }
            else
            {
                EzTexture v1, v2, v3, v4;
                int i = (int)Math.Floor(t);
                if (!Loop)
                {
                    //v1 = Get(anim, (int)Math.Max(0, i - 1));
                    v2 = Get(anim, i);
                    //v3 = Get(anim, (int)Math.Min(Length, i + 1));
                    //v4 = Get(anim, (int)Math.Min(Length, i + 2));
                }
                else
                {
                    //if (i - 1 >= 0) v1 = Get(anim, i - 1);
                    //else v1 = Get(anim, Length);
                    v2 = Get(anim, i);
                    //if (i + 1 <= Length) v3 = Get(anim, i + 1);
                    //else v3 = Get(anim, i + 1 - Length - 1);
                    //if (i + 2 <= Length) v4 = Get(anim, i + 2);
                    //else v4 = Get(anim, i + 2 - Length - 1);
                }

                //return int.CatmullRom(v1, v2, v3, v4, t - i);
                return v2;
            }
        }
    }
}