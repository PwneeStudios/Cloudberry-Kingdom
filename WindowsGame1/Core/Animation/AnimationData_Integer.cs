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
    public struct FrameData
    {
        public EzTexture texture; public Vector2 uv_bl, uv_tr;
        public FrameData(EzTexture texture)
        {
            this.texture = texture;
            uv_bl = Vector2.Zero;
            uv_tr = Vector2.One;
        }
    }
    public struct OneAnim_Texture
    {
        public FrameData[] Data;
        public float Speed;
    }
    public class AnimationData_Texture
    {
        /// <summary>
        /// If false, only changing values are recorded
        /// </summary>
        public static bool RecordAll;

        public OneAnim_Texture[] Anims;

        public bool Linear;

        public int Hold;

        public float Speed;
        public bool Reverse;

        public AnimationData_Texture()
        {
            Linear = false;
            Hold = 0;
            Reverse = false;
            Speed = .1f;

            Anims = null;
        }

        public AnimationData_Texture(EzTexture texture)
        {
            Linear = false;
            Hold = 0;
            Reverse = false;
            Speed = .1f;

            if (texture == null)
            {
                Anims = null;
            }
            else
            {
                Anims = new OneAnim_Texture[1];
                AddFrame(texture, 0);
            }
        }

        public int Width, Height;
        public AnimationData_Texture(EzTexture strip, int width)
        {
            Width = width;
            Height = strip.Tex.Height;

            Linear = false;
            Hold = 0;
            Reverse = false;
            Speed = .1f;

            int frames = strip.Tex.Width / width;
            Anims = new OneAnim_Texture[1];

            Vector2 uv_size = new Vector2(1f / frames, 1);
            Vector2 uv_left = Vector2.Zero;
            for (int i = 0; i < frames; i++)
            {
                AddFrame(strip, 0, uv_left, uv_size);
                uv_left.X += uv_size.X;
            }
        }

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
            Linear = data.Linear;
            Reverse = data.Reverse;
            Speed = data.Speed;

            Hold = 0;
            Anims = new OneAnim_Texture[data.Anims.Length];
            for (int i = 0; i < data.Anims.Length; i++)
                CopyAnim(data, i);
        }

        public void CopyAnim(AnimationData_Texture data, int Anim)
        {
            if (data.Anims[Anim].Data != null)
            {
                Anims[Anim].Data = new FrameData[data.Anims[Anim].Data.Length];
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

        /*
        public int Width
        {
            get
            {
                var data = Anims[0].Data[0];
                return (int)(data.texture.Tex.Width * (data.uv_tr.X - data.uv_bl.X));
            }
        }
        public int Width
        {
            get
            {
                var data = Anims[0].Data[0];
                return (int)(data.texture.Tex.Width * (data.uv_tr.X - data.uv_bl.X));
            }
        }*/

        public void InsertFrame(int anim, int frame)
        {
            if (anim >= Anims.Length) return;
            if (Anims[anim].Data == null) return;
            if (frame >= Anims[anim].Data.Length) return;

            OneAnim_Texture NewAnim = new OneAnim_Texture();
            NewAnim.Data = new FrameData[Anims[anim].Data.Length + 1];
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
                NewAnim.Data = new FrameData[Anims[anim].Data.Length - 1];
                for (int i = 0; i < frame; i++) NewAnim.Data[i] = Anims[anim].Data[i];
                for (int i = frame + 1; i < Anims[anim].Data.Length; i++) NewAnim.Data[i - 1] = Anims[anim].Data[i];
                Anims[anim] = NewAnim;
            }
        }

        public void AddFrame(EzTexture val, int anim) { AddFrame(val, anim, Vector2.Zero, Vector2.One); }
        public void AddFrame(EzTexture val, int anim, Vector2 uv, Vector2 uv_size)
        {
            int frame = 0;
            if (anim >= Anims.Length) frame = 0;
            else if (Anims[anim].Data == null) frame = 0;
            else frame = Anims[anim].Data.Length;

            Set(val, anim, frame, uv, uv_size);
        }
        public void Set(EzTexture val, int anim, int frame, Vector2 uv, Vector2 uv_size)
        {
            FrameData Default = new FrameData(null);
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
            {
                Anims[anim].Data = new FrameData[] { new FrameData(null) };
            }
            else
                if (frame > 0)
                    Default = Get(anim, frame - 1);

            if (frame >= Anims[anim].Data.Length && !(val == Default.texture && uv == Default.uv_bl && Anims[anim].Data.Length <= 1))
            {
                FrameData[] NewData = new FrameData[frame + 1];
                for (int i = 0; i < frame + 1; i++)
                {
                    NewData[i] = new FrameData(null);
                    NewData[i].texture = Default.texture;
                }
                Anims[anim].Data.CopyTo(NewData, 0);
                Anims[anim].Data = NewData;
            }

            if (frame < Anims[anim].Data.Length)
            {
                Anims[anim].Data[frame].texture = val;
                Anims[anim].Data[frame].uv_bl = uv;
                Anims[anim].Data[frame].uv_tr = uv + uv_size;
            }
        }

        public FrameData Get(int anim, int frame)
        {
            if (Anims[0].Data == null)
                return new FrameData(null);

            FrameData Default = Anims[0].Data[0];

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

        public FrameData Calc(int anim, float t, int Length, bool Loop, bool Linear)
        {
            int i = (int)Math.Floor(t);
            return Get(anim, i);
        }

        public FrameData Calc(int anim, float t)
        {
            return Calc(anim, t, Anims[anim].Data.Length, true, Linear);
        }
    }
}