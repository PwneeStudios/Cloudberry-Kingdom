using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom;

namespace Drawing
{
    public interface IPos
    {
        Vector2 Pos
        {
            get;
        }
    }

    public enum LerpStyle { Linear, SmallOvershoot, DecayPlusSmallOvershoot, DecayNoOvershoot, Sigmoid }
    public class FancyVector2 : ViewReadWrite, IPos
    {
        public override string[] GetViewables()
        {
            return new string[] { "RelVal" };
        }

        public AnimationData AnimData;

        public Vector2[] HoldVecs = new Vector2[5];

        public Vector2 RelVal, AbsVal;

        public float RelValX
        {
            get { return RelVal.X; }
            set { RelVal = new Vector2(value, RelVal.Y); }
        }
        public float RelValY
        {
            get { return RelVal.Y; }
            set { RelVal = new Vector2(RelVal.X, value); }
        }

        // Debug
        public int code = 0;

        // Debug AbsVal and RelVal
        //public Vector2 AbsVal;
        //public Vector2 _RelVal;
        //public Vector2 RelVal
        //{
        //    get { return _RelVal; }
        //    set
        //    {
        //        if (code == 23)
        //            Tools.Write("");
        //        _RelVal = value;
        //    }
        //}

        public IPos Center;
        public ObjectBase ObjCenter;
        // Debug Center
        //IPos _Center;
        //public IPos Center
        //{
        //    get { return _Center; }
        //    set
        //    {
        //        _Center = value;
        //    }
        //}

        public float Speed;
        public int TimeStamp, LastUpdate = int.MinValue;
        public float t;
        public bool Playing = false;
        public bool Loop = false;
        public Vector2 Pos
        {
            get { return Update(); }
        }

        public float Val
        {
            get { return Pos.X; }
            set { RelVal = new Vector2(value, RelVal.Y); }
        }


        public void Release()
        {
            AnimData.Release();
            Center = null;
            ObjCenter = null;
        }

        public FancyVector2()
        {
        }

        public FancyVector2(IPos Center)
        {
            this.Center = Center;
        }

        /// <summary>
        /// Sets the FancyPos's center FancyPos. Updates relative coordinates so that absolute coordinates are unaffected.
        /// </summary>
        public void SetCenter(IPos Center) { SetCenter(Center, false); }
        /// <summary>
        /// Sets the FancyPos's center FancyPos
        /// </summary>
        /// <param name="UsePosAsRelPos">Whether to use the current position as relative coordinates in the new system</param>
        public void SetCenter(IPos Center, bool UsePosAsRelPos)
        {
            if (this.Center == Center) return;

            if (!UsePosAsRelPos)
                RelVal = Update() - Center.Pos;
            this.Center = Center;
        }

        /// <summary>
        /// Sets the FancyPos's ObjCenter FancyPos. Updates relative coordinates so that absolute coordinates are unaffected.
        /// </summary>
        public void SetCenter(ObjectBase ObjCenter) { SetCenter(ObjCenter, false); }
        /// <summary>
        /// Sets the FancyPos's ObjCenter FancyPos
        /// </summary>
        /// <param name="UsePosAsRelPos">Whether to use the current position as relative coordinates in the new system</param>
        public void SetCenter(ObjectBase ObjCenter, bool UsePosAsRelPos)
        {
            if (this.ObjCenter == ObjCenter) return;

            if (!UsePosAsRelPos)
                RelVal = Update() - ObjCenter.Pos;
            this.ObjCenter = ObjCenter;
        }

        public Vector2 GetDest()
        {
            if (!Playing)
                return RelVal;
            else
                return AnimData.Get(0, AnimData.Anims[0].Data.Length - 1);
        }

        public void ToAndBack(Vector2 End, int Frames)
        {
            ToAndBack(RelVal, End, Frames);
        }
        public void ToAndBack(Vector2 Start, Vector2 End, int Frames)
        {
            RelVal = Start;

            AnimData = new AnimationData();
            AnimData.Init();
            AnimData.Set(Start, 0, 0);
            AnimData.Set(End, 0, 1);
            AnimData.Set(Start, 0, 2);

            Speed = 2f / Frames;
            TimeStamp = GetCurStep();
            t = 0;
            Playing = true;
        }

        public void MultiLerp(int Frames, params Vector2[] Positions)
        {
            MultiLerp(Frames, false, Positions);
        }
        public void MultiLerpReverse(int Frames, params Vector2[] Positions)
        {
            MultiLerp(Frames, true, Positions);
        }
        void MultiLerp(int Frames, bool Reverse, params Vector2[] Positions)
        {
            AnimData = new AnimationData();
            AnimData.Init();

            if (Reverse)
                for (int i = 0; i < Positions.Length; i++)
                    AnimData.Set(Positions[Positions.Length - 1 - i], 0, i);
            else
                for (int i = 0; i < Positions.Length; i++)
                    AnimData.Set(Positions[i], 0, i);

            Speed = 1f / Frames;
            TimeStamp = GetCurStep();
            t = 0;
            Playing = true;
        }

        public const LerpStyle DefaultLerpStyle = LerpStyle.DecayPlusSmallOvershoot;
        //public const LerpStyle DefaultLerpStyle = LerpStyle.DecayNoOvershoot;

        public void LerpTo(int EndIndex, int Frames) { LerpTo(EndIndex, Frames, DefaultLerpStyle); }
        public void LerpTo(int EndIndex, int Frames, LerpStyle Style)
        {
            LerpTo(HoldVecs[EndIndex], Frames, Style);
        }
        public void LerpTo(int StartIndex, int EndIndex, int Frames) { LerpTo(StartIndex, EndIndex, Frames, DefaultLerpStyle); }
        public void LerpTo(int StartIndex, int EndIndex, int Frames, LerpStyle Style)
        {
            LerpTo(HoldVecs[StartIndex], HoldVecs[EndIndex], Frames, Style);
        }
        public void LerpTo(float End, int Frames) { LerpTo(new Vector2(End), Frames, DefaultLerpStyle); }
        public void LerpTo(Vector2 End, int Frames) { LerpTo(End, Frames, DefaultLerpStyle); }
        public void LerpTo(float End, int Frames, LerpStyle Style) { LerpTo(new Vector2(End), Frames, Style); }
        public void LerpTo(Vector2 End, int Frames, LerpStyle Style)
        {
            LerpTo(RelVal, End, Frames, Style);
        }
        public void LerpTo(float Start, float End, int Frames) { LerpTo(new Vector2(Start), new Vector2(End), Frames); }
        public void LerpTo(float Start, float End, int Frames, LerpStyle Style) { LerpTo(new Vector2(Start), new Vector2(End), Frames, Style); }
        public void LerpTo(Vector2 Start, Vector2 End, int Frames) { LerpTo(Start, End, Frames, DefaultLerpStyle); }
        public void LerpTo(Vector2 Start, Vector2 End, int Frames, LerpStyle Style)
        {
            if (Frames == 0)
            {
                RelVal = End;
                Playing = false;
                return;
            }

            RelVal = Start;

            AnimData = new AnimationData();
            AnimData.Init();

            switch (Style)
            {
                case LerpStyle.Linear:
                    AnimData.Set(Start, 0, 0);
                    AnimData.Set(End, 0, 1);
                    Speed = 1f / Frames;
                    break;

                case LerpStyle.DecayPlusSmallOvershoot:
                    AnimData.Set(Start, 0, 0);
                    AnimData.Set((Start + End) / 2, 0, 1);
                    AnimData.Set(.8f * End + .2f * Start, 0, 2);
                    AnimData.Set(.95f * End + 0.05f * Start, 0, 3);
                    AnimData.Set(1f * End + 0f * Start, 0, 4);
                    AnimData.Set(1f * End + 0f * Start, 0, 5);
                    Speed = 3f / Frames;
                    AnimData.Linear = false;
                    break;

                case LerpStyle.Sigmoid:
                    AnimData.AddFrame(Start, 0);
                    AnimData.AddFrame(.07f * End + .93f * Start, 0);
                    AnimData.AddFrame(.22f * End + .78f * Start, 0);
                    AnimData.AddFrame(.5f * End + .5f * Start, 0);
                    AnimData.AddFrame(.8f * End + .2f * Start, 0);
                    AnimData.AddFrame(.95f * End + 0.05f * Start, 0);
                    AnimData.AddFrame(1f * End + 0f * Start, 0);
                    AnimData.AddFrame(1f * End + 0f * Start, 0);
                    Speed = 4f / Frames;
                    AnimData.Linear = false;
                    break;

                case LerpStyle.DecayNoOvershoot:
                    for (int i = 0; i < 12; i++)
                    {
                        //float s = Vector2.CatmullRom(new Vector2(1), new Vector2(.7f), new Vector2(.2f), new Vector2(0), 3 * i / 11f).X;
                        float s = (float)Math.Pow(.5, i);
                        AnimData.Set((1 - s) * End + s * Start, 0, i);
                    }
                    AnimData.Set(1f * End + 0f * Start, 0, 20);
                    Speed = 4f / Frames;
                    AnimData.Linear = false;
                    break;

                case LerpStyle.SmallOvershoot:
                    AnimData.Set(Start, 0, 0);
                    AnimData.Set((Start + End) / 2, 0, 1);
                    AnimData.Set(End, 0, 2);
                    AnimData.Set(End, 0, 3);
                    AnimData.Set(End, 0, 4);
                    Speed = 2f / Frames;
                    AnimData.Linear = false;
                    break;
            }

            TimeStamp = GetCurStep();
            t = 0;
            Playing = true;
        }

        public bool UpdateOnPause = true;
        public bool UpdateWithGame = false;
        int GetCurStep()
        {
            if (UpdateWithGame)
            {
                if (Tools.CurGameData != null)
                    return Tools.CurGameData.PhsxCount;
                else
                    return int.MinValue;
            }

            if (UpdateOnPause) return Tools.DrawCount;
            else return Tools.PhsxCount;
        }

        public Vector2 Update() { return Update(Vector2.One); }
        public Vector2 Update(Vector2 Scale)
        {
            int CurStep = GetCurStep();

            if (Playing && CurStep != LastUpdate)
            {
                LastUpdate = CurStep;

                int Length = AnimData.Anims[0].Data.Length;

                if (UpdateWithGame)
                    t += Speed;
                else
                    t = Speed * (CurStep - TimeStamp);

                if (Loop)
                {
                    t = t % (Length + 1);

                    RelVal = AnimData.Calc(0, t, Length, true, AnimData.Linear);
                }
                else
                {
                    if (t > Length - 1)
                    {
                        Playing = false;
                        RelVal = AnimData.Get(0, Length - 1);
                    }
                    else
                        RelVal = AnimData.Calc(0, t, Length, false, AnimData.Linear);
                }
            }

            AbsVal = Scale * RelVal;
            if (Center != null)
                AbsVal += Center.Pos;
            else if (ObjCenter != null)
                AbsVal += ObjCenter.Pos;
 
            return AbsVal;
        }
    }
}