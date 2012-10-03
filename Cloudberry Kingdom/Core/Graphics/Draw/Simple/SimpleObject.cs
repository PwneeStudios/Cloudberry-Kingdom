using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using CloudberryKingdom;

namespace CoreEngine
{
    public class SimpleObject
    {
        public SimpleQuad[] Quads;
        public SimpleBox[] Boxes;
        public BasePoint Base;

        public bool xFlip, yFlip, CenterFlipOnBox;
        public Vector2 FlipCenter;

        public Queue<AnimQueueEntry> AnimQueue;
        public AnimQueueEntry LastAnimEntry;
        public int[] AnimLength;
        public string[] AnimName;
        public float[] AnimSpeed;
        public bool Play, Loop, Transfer, OldLoop, Linear;
        public int anim, OldAnim;
        public float t, OldT, StartT;

        public List<EzEffect> MyEffects;

        public bool Released;

        public void Release()
        {
            //return;
            if (Released) return;
            Released = true;

            if (Quads != null)
                for (int i = 0; i < Quads.Length; i++)
                    Quads[i].Release();
                //foreach (SimpleQuad quad in Quads)
                    //quad.Release();
            if (Boxes != null)
                foreach (SimpleBox box in Boxes)
                    box.Release();
            AnimQueue = null;
            LastAnimEntry = null;
            AnimLength = null;
            AnimName = null;
            AnimSpeed = null;

            MyEffects = null;
        }

        /// <summary>
        /// Update the list of effects associated with the object's quads
        /// </summary>
        public void UpdateEffectList()
        {
            if (Quads == null || Quads.Length == 0) return;

            if (MyEffects == null) MyEffects = new List<EzEffect>();
            else MyEffects.Clear();

            for (int i = 0; i < Quads.Length; i++)
                if (!MyEffects.Contains(Quads[i].MyEffect))
                    MyEffects.Add(Quads[i].MyEffect);
        }

        /// <summary>
        /// Find the index of a quad of the given name.
        /// </summary>
        public int GetQuadIndex(string name)
        {
            for (int i = 0; i < Quads.Length; i++)
            {
                if (string.Compare(Quads[i].Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return i;
            }

            return -1;
        }

        public void Scale(float scale)
        {
            Base.e1 *= scale;
            Base.e2 *= scale;
        }

        public void SetColor(Color color)
        {
            if (Quads != null)
                for (int i = 0; i < Quads.Length; i++)
                    Quads[i].SetColor(color);
        }

        public SimpleObject(SimpleObject obj, bool BoxesOnly)
        {
            Constructor(obj, BoxesOnly, false);
        }
        void Constructor(SimpleObject obj, bool BoxesOnly, bool DeepCopy)
        {
            Base = obj.Base;

            CenterFlipOnBox = obj.CenterFlipOnBox;

            if (!BoxesOnly)
            {
                Quads = new SimpleQuad[obj.Quads.Length];
                for (int i = 0; i < obj.Quads.Length; i++)
                    Quads[i] = new SimpleQuad(ref obj.Quads[i]);
            }

            Boxes = new SimpleBox[obj.Boxes.Length];
            for (int i = 0; i < obj.Boxes.Length; i++)
                Boxes[i] = new SimpleBox(obj.Boxes[i]);

            AnimQueue = new Queue<AnimQueueEntry>();
            AnimQueueEntry[] array = obj.AnimQueue.ToArray();
            if (array.Length > 0)
            {
                LastAnimEntry = new AnimQueueEntry(array[array.Length - 1]);
                for (int i = 0; i < array.Length - 1; i++)
                    AnimQueue.Enqueue(new AnimQueueEntry(array[i]));
                AnimQueue.Enqueue(LastAnimEntry);
            }

            Play = obj.Play;
            Loop = obj.Loop;
            anim = obj.anim;
            t = obj.t;

            if (DeepCopy)
            {
                AnimLength = new int[50];
                obj.AnimLength.CopyTo(AnimLength, 0);
                AnimSpeed = new float[50];
                obj.AnimSpeed.CopyTo(AnimSpeed, 0);
                AnimName = new string[50];
                obj.AnimName.CopyTo(AnimName, 0);

                UpdateEffectList();
            }
            else
            {
                AnimLength = obj.AnimLength;
                AnimSpeed = obj.AnimSpeed;
                AnimName = obj.AnimName;
                
                MyEffects = obj.MyEffects;
            }
        }

        public SimpleObject(ObjectClass obj)
        {
            Base.Init();

            CenterFlipOnBox = obj.CenterFlipOnBox;

            Quads = new SimpleQuad[obj.QuadList.Count];
            for (int i = 0; i < obj.QuadList.Count; i++)
                Quads[i] = new SimpleQuad((Quad)obj.QuadList[i]);

            Boxes = new SimpleBox[obj.BoxList.Count];
            for (int i = 0; i < obj.BoxList.Count; i++)
                Boxes[i] = new SimpleBox(obj.BoxList[i]);

            AnimQueue = new Queue<AnimQueueEntry>();
            AnimQueueEntry[] array = obj.AnimQueue.ToArray();
            if (array.Length > 0)
            {
                LastAnimEntry = new AnimQueueEntry(array[array.Length - 1]);
                for (int i = 0; i < array.Length - 1; i++)
                    AnimQueue.Enqueue(new AnimQueueEntry(array[i]));
                AnimQueue.Enqueue(LastAnimEntry);
            }

            Play = obj.Play;
            Loop = obj.Loop;
            anim = obj.anim;
            t = obj.t;

            AnimLength = new int[50];
            obj.AnimLength.CopyTo(AnimLength, 0);
            AnimSpeed = new float[50];
            obj.AnimSpeed.CopyTo(AnimSpeed, 0);
            AnimName = new string[50];
            obj.AnimName.CopyTo(AnimName, 0);

            UpdateEffectList();
        }

        public Vector2 GetBoxCenter(int i)
        {
            Vector2 c = Boxes[i].Center();
            if (xFlip)
                c.X = FlipCenter.X - (c.X - FlipCenter.X);
            if (yFlip)
                c.Y = FlipCenter.Y - (c.Y - FlipCenter.Y);
            return c;
        }

        public void CopyUpdate(SimpleObject source)
        {
            Vector2 shift = Base.Origin - source.Base.Origin;

            if (Quads != null)
                for (int i = 0; i < Quads.Length; i++)
                    Quads[i].CopyUpdate(ref source.Quads[i], ref shift);

            for (int i = 0; i < Boxes.Length; i++)
                Boxes[i].CopyUpdate(ref source.Boxes[i], ref shift);

            if (Boxes.Length > 0)
                FlipCenter = Boxes[0].Center();
            else
                FlipCenter = Base.Origin;
        }

        public void UpdateQuads()
        {
            if (Quads != null)
                for (int i = 0; i < Quads.Length; i++)
                    Quads[i].Update(ref Base);
        }

        public void UpdateBoxes()
        {
            for (int i = 0; i < Boxes.Length; i++)
                Boxes[i].Update(ref Base);

            if (Boxes.Length > 0)
                FlipCenter = Boxes[0].Center();
            else
                FlipCenter = Base.Origin;
        }

        public void Update()
        {
            UpdateQuads();

            UpdateBoxes();
        }

        /// <summary>
        /// Shift each quad's absolute vertex coordinates. Does not effect normal coordinates.
        /// </summary>
        /// <param name="shift"></param>
        public void UpdatedShift(Vector2 shift)
        {
            for (int i = 0; i < Quads.Length; i++)
                Quads[i].UpdatedShift(shift);
        }

        public void Draw() { Draw(Tools.QDrawer, Tools.EffectWad); }

        public void Draw(QuadDrawer QDrawer, EzEffectWad EffectWad)
        {
            int n = 0;
            if (Quads != null) n = Quads.Length;

            Draw(QDrawer, EffectWad, 0, n - 1);
        }
        public void Draw(QuadDrawer QDrawer, EzEffectWad EffectWad, int StartIndex, int EndIndex)
        {
            if (xFlip || yFlip)
                foreach (EzEffect fx in MyEffects)
                {
                    fx.FlipCenter.SetValue(FlipCenter);
                    fx.FlipVector.SetValue(new Vector2(xFlip ? 1 : -1, yFlip ? 1 : -1));
                }

            
            for (int i = StartIndex; i <= EndIndex; i++)
                QDrawer.DrawQuad(ref Quads[i]);

            if (xFlip || yFlip)
            {
                QDrawer.Flush();
                foreach (EzEffect fx in MyEffects)
                    fx.FlipVector.SetValue(new Vector2(-1, -1));
            }
        }

        public void DrawQuad(ref SimpleQuad Quad)
        {
            if (xFlip || yFlip)
                foreach (EzEffect fx in MyEffects)
                {
                    fx.FlipCenter.SetValue(FlipCenter);
                    fx.FlipVector.SetValue(new Vector2(xFlip ? 1 : -1, yFlip ? 1 : -1));
                }

            Tools.QDrawer.DrawQuad(ref Quad);

            if (xFlip || yFlip)
            {
                Tools.QDrawer.Flush();
                foreach (EzEffect fx in MyEffects)
                    fx.FlipVector.SetValue(new Vector2(-1, -1));
            }
        }

        public void EnqueueTransfer(int _anim, float _destT, float speed, bool DestLoop)
        {
            DequeueTransfers();

            AnimQueueEntry NewEntry = new AnimQueueEntry();
            NewEntry.anim = _anim;
            NewEntry.AnimSpeed = speed;
            NewEntry.StartT = 0;
            NewEntry.EndT = 1;
            NewEntry.DestT = _destT;
            NewEntry.Loop = DestLoop;
            NewEntry.Type = AnimQueueEntryType.Transfer;
            NewEntry.Initialized = false;

            AnimQueue.Enqueue(NewEntry);
            LastAnimEntry = NewEntry;
        }

        public void EnqueueAnimation(int _anim, float startT, bool loop)
        {
            EnqueueTransfer(_anim, startT, .5f, loop);

            AnimQueueEntry NewEntry = new AnimQueueEntry();
            NewEntry.anim = _anim;
            NewEntry.AnimSpeed = 1;
            NewEntry.StartT = startT;
            NewEntry.EndT = -1;
            NewEntry.Loop = loop;
            NewEntry.Type = AnimQueueEntryType.Play;
            NewEntry.Initialized = false;

            AnimQueue.Enqueue(NewEntry);
            LastAnimEntry = NewEntry;
        }

        public int DestinationAnim()
        {
            if (AnimQueue.Count > 0)
                return LastAnimEntry.anim;
            else
                return anim;
        }

        public void DequeueTransfers()
        {
            while (AnimQueue.Count > 0 && AnimQueue.Peek().Type == AnimQueueEntryType.Transfer)
                AnimQueue.Dequeue();
        }

        public void PlayUpdate(float DeltaT)
        {
            if (!Play) return;
            if (AnimQueue.Count == 0) return;

            AnimQueueEntry CurAnimQueueEntry = AnimQueue.Peek();

            if (!CurAnimQueueEntry.Initialized)
            {
                t = CurAnimQueueEntry.StartT;
                Loop = CurAnimQueueEntry.Loop;
                anim = CurAnimQueueEntry.anim;

                if (CurAnimQueueEntry.Type == AnimQueueEntryType.Transfer)
                    SetHold();

                CurAnimQueueEntry.Initialized = true;
            }

            t += DeltaT * AnimSpeed[anim] * CurAnimQueueEntry.AnimSpeed;
            if (CurAnimQueueEntry.Type == AnimQueueEntryType.Transfer)
            {
                if (t > 1)
                {
                    AnimQueue.Dequeue();
                    if (AnimQueue.Count > 0)
                    {
                        AnimQueueEntry Next = AnimQueue.Peek();
                        if (Next.anim == anim)
                            Next.StartT = CurAnimQueueEntry.DestT;
                    }

                    t = 1;
                }
            }
            else
            {
                if (t > CurAnimQueueEntry.EndT && t - DeltaT <= CurAnimQueueEntry.EndT)
                {
                    t = CurAnimQueueEntry.EndT;
                    AnimQueue.Dequeue();
                }
                else
                {
                    if (Loop)
                    {
                        if (t <= 0)
                            t += AnimLength[anim] + 1;
                        if (t > AnimLength[anim] + 1)
                            t -= AnimLength[anim] + 1;
                    }
                    else
                    {
                        if (t < 0)
                        {
                            t = 0;
                            AnimQueue.Dequeue();
                        }
                        if (t > AnimLength[anim])
                        {
                            t = AnimLength[anim];
                            AnimQueue.Dequeue();
                        }
                    }
                }
            }

            if (CurAnimQueueEntry.Type == AnimQueueEntryType.Play)
            {
                if (Quads != null)
                    for (int i = 0; i < Quads.Length; i++)
                        if (Quads[i].Animated)
                            Quads[i].Calc(anim, t, AnimLength[anim], Loop, Linear);
                for (int i = 0; i < Boxes.Length; i++)
                    if (Boxes[i].Animated)
                        Boxes[i].Calc(anim, t, AnimLength[anim], Loop, Linear);
            }
            else
            {
                if (Quads != null)
                    for (int i = 0; i < Quads.Length; i++)
                        Quads[i].Transfer(anim, CurAnimQueueEntry.DestT, AnimLength[anim], CurAnimQueueEntry.Loop, Linear, t);
                for (int i = 0; i < Boxes.Length; i++)
                    Boxes[i].Transfer(anim, CurAnimQueueEntry.DestT, AnimLength[anim], CurAnimQueueEntry.Loop, Linear, t);
            }
        }

        public void SetHold()
        {
            if (Quads != null)
                for (int i = 0; i < Quads.Length; i++)
                    Quads[i].SetHold();
            for (int i = 0; i < Boxes.Length; i++)
                Boxes[i].SetHold();
        }

        public void Read(int anim, int frame)
        {
            if (Quads != null)
                for (int i = 0; i < Quads.Length; i++)
                    Quads[i].ReadAnim(anim, frame);
            for (int i = 0; i < Boxes.Length; i++)
                Boxes[i].ReadAnim(anim, frame);
        }
    }
}