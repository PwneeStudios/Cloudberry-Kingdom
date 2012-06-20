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
    public struct OneAnim { public Vector2[] Data; }
    public struct AnimationData
    {
        /// <summary>
        /// If false, only changing values are recorded
        /// </summary>
        public static bool RecordAll;

        public OneAnim[] Anims;

        public bool Linear;

        public Vector2 Hold;

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
                Anims = new OneAnim[length];
                for (int i = 0; i < length; i++)
                    WriteReadTools.ReadOneAnim(reader, ref Anims[i]);
            }
        }

        public AnimationData(AnimationData data)
        {
            Linear = false;

            Hold = Vector2.Zero;
            Anims = new OneAnim[data.Anims.Length];
            for (int i = 0; i < data.Anims.Length; i++)
                CopyAnim(data, i);
        }

        public void CopyAnim(AnimationData data, int Anim)
        {
            if (data.Anims[Anim].Data != null)
            {
                Anims[Anim].Data = new Vector2[data.Anims[Anim].Data.Length];
                data.Anims[Anim].Data.CopyTo(Anims[Anim].Data, 0);
            }
            else
                Anims[Anim].Data = null;
        }

        //public AnimationData()
        public void Init()
        {
            //Data = new Vector2[][]  { null };
            Anims = new OneAnim[] { new OneAnim() };
            Hold = Vector2.Zero;
        }

        public void InsertFrame(int anim, int frame)
        {
            if (anim >= Anims.Length) return;
            if (Anims[anim].Data == null) return;
            if (frame >= Anims[anim].Data.Length) return;

            OneAnim NewAnim = new OneAnim();
            NewAnim.Data = new Vector2[Anims[anim].Data.Length + 1];
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
                OneAnim NewAnim = new OneAnim();
                NewAnim.Data = new Vector2[Anims[anim].Data.Length - 1];
                for (int i = 0; i < frame; i++) NewAnim.Data[i] = Anims[anim].Data[i];
                for (int i = frame + 1; i < Anims[anim].Data.Length; i++) NewAnim.Data[i - 1] = Anims[anim].Data[i];
                Anims[anim] = NewAnim;
            }
        }

        public void AddFrame(Vector2 val, int anim)
        {
            int frame = 0;
            if (anim >= Anims.Length) frame = 0;
            else if (Anims[anim].Data == null) frame = 0;
            else frame = Anims[anim].Data.Length;

            Set(val, anim, frame);
        }
        public void Set(Vector2 val, int anim, int frame)
        {
            Vector2 Default = new Vector2(-123456, -123456);
            if (Anims[0].Data != null)
            {
                Default = Anims[0].Data[0];
            }

            if (anim >= Anims.Length)
            {
                OneAnim[] NewAnims = new OneAnim[anim + 1];
                Anims.CopyTo(NewAnims, 0);
                Anims = NewAnims;
            }

            if (Anims[anim].Data == null)
                Anims[anim].Data = new Vector2[] { Default };
            else
                if (frame > 0)
                    Default = Get(anim, frame - 1);

            if (frame >= Anims[anim].Data.Length && !(val == Default && Anims[anim].Data.Length <= 1))
            {
                Vector2[] NewData = new Vector2[frame + 1];
                for (int i = 0; i < frame + 1; i++) NewData[i] = Default;
                Anims[anim].Data.CopyTo(NewData, 0);
                Anims[anim].Data = NewData;
            }

            if (frame < Anims[anim].Data.Length)
                Anims[anim].Data[frame] = val;
        }

        public Vector2 Get(int anim, int frame)
        {
            Vector2 Default = Vector2.Zero;
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

        public Vector2 Transfer(int DestAnim, float DestT, int DestLength, bool DestLoop, bool DestLinear, float t)
        {
            Vector2 v1 = Hold;//Calc(OldAnim, OldT, OldLength, OldLoop);
            Vector2 v2 = Calc(DestAnim, DestT, DestLength, DestLoop, DestLinear);

            return Vector2.Lerp(v1, v2, t);
        }

        public Vector2 Calc(int anim, float t, int Length, bool Loop, bool Linear)
        {
            if (Linear)
            {
                Vector2 v2, v3;
                int i = (int)Math.Floor(t);
                if (!Loop)
                {
                    v2 = Get(anim, i);
                    v3 = Get(anim, (int)Math.Min(Length, i + 1));
                }
                else
                {
                    v2 = Get(anim, i);
                    if (i + 1 <= Length) v3 = Get(anim, i + 1);
                    else v3 = Get(anim, i + 1 - Length - 1);
                }

                return Vector2.Lerp(v2, v3, t - i);
            }
            else
            {
                Vector2 v1, v2, v3, v4;
                int i = (int)Math.Floor(t);
                if (!Loop)
                {
                    v1 = Get(anim, (int)Math.Max(0, i - 1));
                    v2 = Get(anim, i);
                    v3 = Get(anim, (int)Math.Min(Length, i + 1));
                    v4 = Get(anim, (int)Math.Min(Length, i + 2));
                }
                else
                {
                    if (i - 1 >= 0) v1 = Get(anim, i - 1);
                    else v1 = Get(anim, Length);
                    v2 = Get(anim, i);
                    if (i + 1 <= Length) v3 = Get(anim, i + 1);
                    else v3 = Get(anim, i + 1 - Length - 1);
                    if (i + 2 <= Length) v4 = Get(anim, i + 2);
                    else v4 = Get(anim, i + 2 - Length - 1);
                }

                return Vector2.CatmullRom(v1, v2, v3, v4, t - i);
            }
        }

        Vector3 VecAndLength(Vector2 v)
        {
            return new Vector3(v.X, v.Y, v.Length());
        }
        public Vector2 CalcAxis(int anim, float t, int Length, bool Loop, bool Linear)
        {
            Vector3 result;
            if (Linear)
            {
                Vector3 v2, v3;
                int i = (int)Math.Floor(t);
                if (!Loop)
                {
                    v2 = VecAndLength(Get(anim, i));
                    v3 = VecAndLength(Get(anim, (int)Math.Min(Length, i + 1)));
                }
                else
                {
                    v2 = VecAndLength(Get(anim, i));
                    if (i + 1 <= Length) v3 = VecAndLength(Get(anim, i + 1));
                    else v3 = VecAndLength(Get(anim, i + 1 - Length - 1));
                }

                result = Vector3.Lerp(v2, v3, t - i);
            }
            else
            {
                Vector3 v1, v2, v3, v4;
                int i = (int)Math.Floor(t);
                if (!Loop)
                {
                    v1 = VecAndLength(Get(anim, (int)Math.Max(0, i - 1)));
                    v2 = VecAndLength(Get(anim, i));
                    v3 = VecAndLength(Get(anim, (int)Math.Min(Length, i + 1)));
                    v4 = VecAndLength(Get(anim, (int)Math.Min(Length, i + 2)));
                }
                else
                {
                    if (i - 1 >= 0) v1 = VecAndLength(Get(anim, i - 1));
                    else v1 = VecAndLength(Get(anim, Length));
                    v2 = VecAndLength(Get(anim, i));
                    if (i + 1 <= Length) v3 = VecAndLength(Get(anim, i + 1));
                    else v3 = VecAndLength(Get(anim, i + 1 - Length - 1));
                    if (i + 2 <= Length) v4 = VecAndLength(Get(anim, i + 2));
                    else v4 = VecAndLength(Get(anim, i + 2 - Length - 1));
                }
                
                result = Vector3.CatmullRom(v1, v2, v3, v4, t - i);
            }

            Vector2 v = new Vector2(result.X, result.Y);
            v.Normalize();
            v *= result.Z;

            return v;
        }
    }
}