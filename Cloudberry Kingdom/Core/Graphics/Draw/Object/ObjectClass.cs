using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.IO;

using CloudberryKingdom;

namespace Drawing
{
    public class ObjectClass
    {
        public float LoadingRunSpeed = .135f;
        
        public float CapeThickness = 16;
        public Vector2 p1_Left = new Vector2(-63, -45);
        public Vector2 p2_Left = new Vector2(-27, 0);
        public Vector2 p1_Right = new Vector2(63, -45);
        public Vector2 p2_Right = new Vector2(27, 0);



        public static int ObjectClassVersionNumber = 54;
        public int VersionNumber;

        public float ContainedQuadAngle = 0;
        public Quad ContainedQuad = new Quad();
        EzTexture ContainedTexture;
        public EzTexture MySkinTexture;
        public EzEffect MySkinEffect;

        /// <summary>
        /// If true the outline pixel shader uses a more computationally expensive but refined method.
        /// </summary>
        public bool RefinedOutline = false;

        public Quad ParentQuad;
        public List<BaseQuad> QuadList;

        private QuadDrawer QDrawer;
        public bool xFlip, yFlip, CenterFlipOnBox;
        public Vector2 FlipCenter;
        public Vector2 OutlineWidth = new Vector2(1);
        public Color OutlineColor, InsideColor;

        //RenderTarget2D DepthVelocityRenderTarget
        RenderTarget2D ObjectRenderTarget, ToTextureRenderTarget;
        int DrawWidth, DrawHeight;
        public Texture2D ObjTex, ObjDepthTex;

        public List<EzEffect> MyEffects;

        public bool DonePlaying { get { return AnimQueue.Count == 0; } }

        public Queue<AnimQueueEntry> AnimQueue;
        public AnimQueueEntry LastAnimEntry;
        public int[] AnimLength;
        public string[] AnimName;
        public float[] AnimSpeed;
        public bool Play, Loop, Transfer, OldLoop, Linear;
        public int anim, OldAnim;
        public float t, OldT, StartT;

        public List<ObjectBox> BoxList;

        public bool BoxesOnly;

        public void Release()
        {
            //DepthVelocityRenderTarget.Dispose();
            if (ObjectRenderTarget != null && OriginalRenderTarget)
                ObjectRenderTarget.Dispose();

            ContainedQuad.Release();
            ParentQuad.Release();

            if (QuadList != null)
            foreach (BaseQuad quad in QuadList)
            {
                quad.Release();
            }
            QuadList = null;

            foreach (ObjectBox box in BoxList)
                box.Release();
            BoxList = null;

            QuadList = null;
            BoxList = null;

            AnimQueue = null;
            AnimLength = null;
            AnimName = null;
            AnimSpeed = null;
        }

        public void ConvertForSimple()
        {
            AnimationData.RecordAll = true;
            //for (int i = 0; i < AnimName.Length; i++)
            for (int i = AnimName.Length - 1; i >= 0; i--)
            {
                //for (int j = 0; j <= AnimLength[i]; j++)
                for (int j = AnimLength[i]; j >= 0; j--)
                {
                    Read(i, j);
                    Update(null);
                    //RecordByDrawOrder(i, j, true, ObjectDrawOrder.AfterOutline);
                    Record(i, j, false);
                }
            }
            AnimationData.RecordAll = false;
        }

        public void Write(BinaryWriter writer)
        {
            // Object version number
            writer.Write(VersionNumber);

            // Anim data
            writer.Write(AnimLength.Length);
            for (int i = 0; i < AnimLength.Length; i++)
                writer.Write(AnimLength[i]);

            writer.Write(AnimName.Length);
            for (int i = 0; i < AnimName.Length; i++)
                writer.Write(AnimName[i]);

            writer.Write(AnimSpeed.Length);
            for (int i = 0; i < AnimSpeed.Length; i++)
                writer.Write(AnimSpeed[i]);

            // Write number of quads and their type
            writer.Write(QuadList.Count);
            foreach (BaseQuad quad in QuadList)
            {
                if (quad is Quad) writer.Write(0);
                else writer.Write(1);
            }

            // Write each quad's data
            foreach (BaseQuad quad in QuadList)
            {
                quad.Write(writer);
            }

            // Boxes
            writer.Write(BoxList.Count);
            foreach (ObjectBox box in BoxList)
                box.Write(writer, this);
        }

        public void ReadFile(EzReader reader)
        {
            ReadFile(reader.reader, Tools.EffectWad, Tools.TextureWad);
        }
        public void ReadFile(BinaryReader reader, EzEffectWad EffectWad, EzTextureWad TextureWad)
        {
            // Get object version number
            VersionNumber = reader.PeekChar();
            if (VersionNumber > 50)
                VersionNumber = reader.ReadInt32();

            // Get anim data
            int length;

            length = reader.ReadInt32();
            AnimLength = new int[length];
            for (int i = 0; i < length; i++)
                AnimLength[i] = reader.ReadInt32();

            length = reader.ReadInt32();
            AnimName = new string[length];
            for (int i = 0; i < length; i++)
                AnimName[i] = reader.ReadString();

            length = reader.ReadInt32();
            AnimSpeed = new float[length];
            for (int i = 0; i < length; i++)
                AnimSpeed[i] = reader.ReadSingle();

            // Get number of quads
            int n = reader.ReadInt32();

            // Create a list of quads to hold the data to be loaded
            for (int i = 0; i < n; i++)
            {
                int QuadType = reader.ReadInt32();

                if (QuadType == 0)
                {
                    Quad NewQuad = new Quad();
                    AddQuad(NewQuad);
                }
                else
                    Tools.Break();
            }

            // Load the quad data
            foreach (BaseQuad quad in QuadList)
            {
                quad.Read(reader, EffectWad, TextureWad, VersionNumber);
            }

            // Load the box data
            n = reader.ReadInt32();
            for (int i = 0; i < n; i++)
            {
                ObjectBox NewBox = new ObjectBox();
                NewBox.Read(reader, this, VersionNumber);
                AddBox(NewBox);
            }

            // Sort the quad list
            Sort();

            // Update object version number
            VersionNumber = ObjectClassVersionNumber;

            UpdateEffectList();
        }

#if EDITOR
        public List<ObjectVector> GetObjectVectors()
        {
            List<ObjectVector> L = new List<ObjectVector>();
            foreach (BaseQuad quad in QuadList)
                L.AddRange(quad.GetObjectVectors());
            return L;
        }
#endif

        /// <summary>
        /// Update the list of effects associated with the object's quads
        /// </summary>
        public void UpdateEffectList()
        {
            if (QuadList == null || QuadList.Count == 0) return;

            if (MyEffects == null) MyEffects = new List<EzEffect>();
            else MyEffects.Clear();

            foreach (BaseQuad quad in QuadList)
                if (!MyEffects.Contains(quad.MyEffect))
                    MyEffects.Add(quad.MyEffect);
        }

        public Vector2 CalcBLBound()
        {
            Vector2 BL = new Vector2(10000000, 10000000);

            foreach (BaseQuad quad in QuadList)
            {
                BL = Vector2.Min(BL, quad.BL());
            }

            return BL;
        }

        public Vector2 CalcTRBound()
        {
            Vector2 TR = new Vector2(-10000000, -10000000);

            foreach (BaseQuad quad in QuadList)
            {
                TR = Vector2.Max(TR, quad.TR());
            }

            return TR;
        }

        public void EnqueueTransfer(int _anim, float _destT, float speed, bool DestLoop)
        {
            EnqueueTransfer(_anim, _destT, speed, false, false);
        }
        public void EnqueueTransfer(int _anim, float _destT, float speed, bool DestLoop, bool KeepTransfers)
        {
            if (!KeepTransfers)
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

        public void ImportAnimData(ObjectClass SourceObj, List<BaseQuad> SourceQuads, List<string> SourceAnims)
        {
            foreach (BaseQuad SourceQuad in SourceQuads)
            {
                // Find the corresponding quad
                BaseQuad quad = FindQuad(SourceQuad.Name);
                if (quad != null)
                {
                    // Loop through all the animations and copy the values
                    foreach (string AnimName in SourceAnims)
                    {
                        if (HasAnim(AnimName))
                        {
                            int AnimIndex = FindAnim(AnimName);
                            quad.CopyAnim(SourceQuad, AnimIndex);
                        }
                    }
                }
            }
        }

        public void ImportAnimDataShallow(ObjectClass SourceObj, List<BaseQuad> SourceQuads, List<string> SourceAnims)
        {
            foreach (BaseQuad SourceQuad in SourceQuads)
            {
                // Find the corresponding quad
                BaseQuad quad = FindQuad(SourceQuad.Name);
                if (quad != null)
                {
                    // Loop through all the animations and copy the values
                    foreach (string AnimName in SourceAnims)
                    {
                        if (HasAnim(AnimName))
                        {
                            int AnimIndex = FindAnim(AnimName);
                            quad.CopyAnimShallow(SourceQuad, AnimIndex);
                        }
                    }
                }
            }
        }

        public bool HasAnim(string name)
        {
            for (int i = 0; i < AnimName.Length; i++)
                if (string.Compare(name, AnimName[i], StringComparison.OrdinalIgnoreCase) == 0)
                    return true;
            return false;
        }
        public int FindAnim(string name)
        {
            for (int i = 0; i < AnimName.Length; i++)
                if (string.Compare(name, AnimName[i], StringComparison.OrdinalIgnoreCase) == 0)
                    return i;
            return 0;
        }
        public void EnqueueAnimation(string name)
        {
            EnqueueAnimation(name, 0, false, true);
        }
        public void EnqueueAnimation(string name, float startT, bool loop, bool clear)
        {
            EnqueueAnimation(FindAnim(name), startT, loop, clear);
        }
        public void EnqueueAnimation(string name, float startT, bool loop, bool clear, float TransferSpeed, float PlaySpeed)
        {
            EnqueueAnimation(name, startT, loop, clear, TransferSpeed, PlaySpeed, false);
        }
        public void EnqueueAnimation(string name, float startT, bool loop, bool clear, float TransferSpeed, float PlaySpeed, bool KeepTransfers)
        {
            EnqueueAnimation(FindAnim(name), startT, loop, clear, KeepTransfers, TransferSpeed);
            //AnimQueue.Peek().AnimSpeed *= 5f;
            DestAnim().AnimSpeed *= PlaySpeed;
        }
        public void EnqueueAnimation(int _anim, float startT, bool loop)
        {
            EnqueueAnimation(_anim, startT, loop, true, false, 1);
        }
        public void EnqueueAnimation(int _anim, float startT, bool loop, bool clear)
        {
            EnqueueAnimation(_anim, startT, loop, true, false, 1);
        }
        public void EnqueueAnimation(int _anim, float startT, bool loop, bool clear, bool KeepTransfers, float TransferSpeed)
        {
            if (clear)
                AnimQueue.Clear();

            EnqueueTransfer(_anim, startT, .5f * TransferSpeed, loop, KeepTransfers);

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

        public AnimQueueEntry DestAnim()
        {
            return LastAnimEntry;
        }

        public void DequeueTransfers()
        {
            while (AnimQueue.Count > 0 && AnimQueue.Peek().Type == AnimQueueEntryType.Transfer)
                AnimQueue.Dequeue();
        }

        public void SetAnimT(float t, bool Loop)
        {
            this.t = t;

            foreach (BaseQuad quad in QuadList)
                quad.Calc(anim, t, AnimLength[anim], Loop, false);
        }

        /// <summary>
        /// Returns true when the animation is at the specified time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool AtTime(float time)
        {
            return t > time && OldT <= time;
        }

        public bool DoSpriteAnim = true;
        public void PlayUpdate(float DeltaT)
        {
            OldT = t;

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
                if (!Loop && t > CurAnimQueueEntry.EndT && t - DeltaT <= CurAnimQueueEntry.EndT)
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
                if (!BoxesOnly && QuadList != null)
                    foreach (BaseQuad quad in QuadList)
                    {
                        quad.UpdateSpriteAnim = DoSpriteAnim;
                        quad.Calc(anim, t, AnimLength[anim], Loop, Linear);
                    }
                foreach (ObjectBox box in BoxList)
                    box.Calc(anim, t, AnimLength[anim], Loop, Linear);
            }
            else
            {
                if (!BoxesOnly && QuadList != null)
                    foreach (BaseQuad quad in QuadList)
                        quad.Transfer(anim, CurAnimQueueEntry.DestT, AnimLength[anim], CurAnimQueueEntry.Loop, Linear, t);
                foreach (ObjectBox box in BoxList)
                    box.Transfer(anim, CurAnimQueueEntry.DestT, AnimLength[anim], CurAnimQueueEntry.Loop, Linear, t);
            }
        }

        public void SetHold()
        {
            if (!BoxesOnly && QuadList != null)
                foreach (BaseQuad quad in QuadList)
                    quad.SetHold();
            foreach (ObjectBox box in BoxList)
                box.SetHold();
        }

#if EDITOR
        public void DeleteFrame(int anim, int frame)
        {
            if (frame <= AnimLength[anim])
            {
                AnimLength[anim]--;

                foreach (BaseQuad quad in QuadList)
                    foreach (ObjectVector point in quad.GetObjectVectors())
                        point.AnimData.DeleteFrame(anim, frame);
                foreach (ObjectBox box in BoxList)
                    foreach (ObjectVector point in box.GetObjectVectors())
                        point.AnimData.DeleteFrame(anim, frame);
            }
        }

        public void InsertFrame(int anim, int frame)
        {
            if (frame <= AnimLength[anim])
            {
                AnimLength[anim]++;

                foreach (BaseQuad quad in QuadList)
                    foreach (ObjectVector point in quad.GetObjectVectors())
                        point.AnimData.InsertFrame(anim, frame);
                foreach (ObjectBox box in BoxList)
                    foreach (ObjectVector point in box.GetObjectVectors())
                        point.AnimData.InsertFrame(anim, frame);
            }
        }
#endif

        public void RecordByDrawOrder(int anim, int frame, bool UseRelativeCoords, ObjectDrawOrder DrawOrder)
        {
            foreach (BaseQuad quad in QuadList)
                if (quad.MyDrawOrder == DrawOrder)
                {
                    Quad hold = quad.ParentQuad;
                    ParentQuad.AddQuadChild(quad);
                    //quad.OrphanSelf();
                    //quad.Update();
                    quad.Record(anim, frame, UseRelativeCoords);
                    hold.AddQuadChild(quad);
                }
        }

        public void Record(int anim, int frame, bool UseRelativeCoords)
        {
            foreach (BaseQuad quad in QuadList)
                quad.Record(anim, frame, UseRelativeCoords);
            //                foreach (ObjectVector point in quad.GetObjectVectors())
            //                  point.AnimData.Set(point.RelPos, anim, frame);
            foreach (ObjectBox box in BoxList)
                box.Record(anim, frame, UseRelativeCoords);
            //                foreach (ObjectVector point in box.GetObjectVectors())
            //                  point.AnimData.Set(point.RelPos, anim, frame);
        }

        ///// <summary>
        ///// Clears a given animation back to 0 frames.
        ///// </summary>
        //public void ClearAnim(int anim)
        //{
        //    foreach (BaseQuad quad in QuadList)
        //        quad.ClearAnim(anim);
        //    foreach (ObjectBox box in BoxList)
        //        box.ClearAnim(anim);
        //}

        public void Read(int anim, int frame)
        {
            ReadQuadData(anim, frame);
            ReadBoxData(anim, frame);
        }
        public void Read_NoTexture(int anim, int frame)
        {
            if (!BoxesOnly && QuadList != null)
                foreach (BaseQuad quad in QuadList)
                    if (quad is Quad)
                        quad.UpdateSpriteAnim = false;

            ReadQuadData(anim, frame);
            ReadBoxData(anim, frame);

            if (!BoxesOnly && QuadList != null)
                foreach (BaseQuad quad in QuadList)
                    if (quad is Quad)
                        quad.UpdateSpriteAnim = DoSpriteAnim;
        }

        public void ReadQuadData(int anim, int frame)
        {
            if (!BoxesOnly && QuadList != null)
                foreach (BaseQuad quad in QuadList)
                    quad.ReadAnim(anim, frame);
        }
        public void ReadBoxData(int anim, int frame)
        {
            foreach (ObjectBox box in BoxList)
                box.ReadAnim(anim, frame);
        }



        public void FinishLoading()
        {
            FinishLoading(Tools.QDrawer, Tools.Device, Tools.TextureWad, Tools.EffectWad, Tools.Device.PresentationParameters, 0, 0, true);
        }
        public void FinishLoading(QuadDrawer Drawer, GraphicsDevice device, EzTextureWad TexWad, EzEffectWad EffectWad, PresentationParameters pp, int Width, int Height)
        {
            FinishLoading(Drawer, device, TexWad, EffectWad, pp, Width, Height, true);
        }
        public void FinishLoading(QuadDrawer Drawer, GraphicsDevice device, EzTextureWad TexWad, EzEffectWad EffectWad, PresentationParameters pp, int Width, int Height, bool UseNames)
        {
            QDrawer = Drawer;
            ParentQuad.FinishLoading(device, TexWad, EffectWad);
            if (!BoxesOnly && QuadList != null)
                foreach (BaseQuad quad in QuadList)
                    quad.FinishLoading(device, TexWad, EffectWad, UseNames);

            if (Width > 0 && Height > 0)
                InitRenderTargets(device, pp, Width, Height);
        }

        public ObjectClass()
        {
            ObjectClassInit(Tools.QDrawer, Tools.Device, Tools.Device.PresentationParameters, 0, 0, Tools.EffectWad.FindByName("BasicEffect"), Tools.TextureWad.FindByName("White"));
        }

        public ObjectClass(ObjectClass obj, bool _BoxesOnly, bool DeepClone)
        {
            LoadingRunSpeed = obj.LoadingRunSpeed;
        
            CapeThickness = obj.CapeThickness;
            p1_Left = obj.p1_Left;
            p2_Left = obj.p2_Left;
            p1_Right = obj.p1_Right;
            p2_Right = obj.p2_Right;



            Linear = obj.Linear;

            VersionNumber = obj.VersionNumber;

            BoxesOnly = _BoxesOnly;

            LoadingRunSpeed = obj.LoadingRunSpeed;

            AnimQueue = new Queue<AnimQueueEntry>();
            AnimQueueEntry[] array = obj.AnimQueue.ToArray();
            if (array.Length > 0)
            {
                LastAnimEntry = new AnimQueueEntry(array[array.Length - 1]);
                for (int i = 0; i < array.Length - 1; i++)
                    AnimQueue.Enqueue(new AnimQueueEntry(array[i]));
                AnimQueue.Enqueue(LastAnimEntry);
            }



            CenterFlipOnBox = obj.CenterFlipOnBox;

            OutlineWidth = obj.OutlineWidth;
            OutlineColor = obj.OutlineColor;
            InsideColor = obj.InsideColor;

            ParentQuad = new Quad(obj.ParentQuad, DeepClone);
            ParentQuad.ParentObject = this;
            ParentQuad.MyEffect = obj.ParentQuad.MyEffect;
            ParentQuad.MyTexture = obj.ParentQuad.MyTexture;

            MySkinTexture = obj.MySkinTexture;
            MySkinEffect = obj.MySkinEffect;

            // Add quads and boxes            
            if (!BoxesOnly)
            {
                QuadList = new List<BaseQuad>();

                foreach (BaseQuad quad in obj.QuadList)
                {
                    if (quad is Quad)
                    {
                        Quad nquad = new Quad((Quad)quad, DeepClone);
                        QuadList.Add(nquad);
                        nquad.ParentObject = this;
                        if (quad.ParentQuad == quad.ParentObject.ParentQuad)
                            ParentQuad.AddQuadChild(nquad);
                    }
                }
            }

            // Clone boxes
            BoxList = new List<ObjectBox>();
            foreach (ObjectBox box in obj.BoxList)
                BoxList.Add(new ObjectBox(box, DeepClone));


            // Make sure pointers match up
            if (!BoxesOnly && QuadList != null)
            {
                for (int i = 0; i < obj.QuadList.Count; i++)
                {
                    // Preserve Parent-Point relationship (for quads attached to splines)
                    if (obj.QuadList[i] is Quad)
                    {
                        BaseQuad parent = ((Quad)obj.QuadList[i]).Center.ParentQuad;
                        if (parent != null)
                        {
                            if (parent == obj.ParentQuad)
                                ((Quad)QuadList[i]).Center.ParentQuad = ParentQuad;
                            else
                            {
                                int j = obj.QuadList.IndexOf((BaseQuad)(parent));
                                ((Quad)QuadList[i]).Center.ParentQuad = QuadList[j];
                            }
                        }
                    }

                    // Preserve Parent-Child quad relationship
                    if (obj.QuadList[i].ParentQuad != obj.ParentQuad)
                        ((Quad)QuadList[obj.QuadList.IndexOf(obj.QuadList[i].ParentQuad)]).AddQuadChild(QuadList[i]);
                }
            }
            for (int i = 0; i < obj.BoxList.Count; i++)
            {
                if (!BoxesOnly && obj.BoxList[i].BL.ParentQuad != obj.ParentQuad)
                    BoxList[i].TR.ParentQuad = BoxList[i].BL.ParentQuad = (Quad)QuadList[obj.QuadList.IndexOf(obj.BoxList[i].BL.ParentQuad)];
                else
                    BoxList[i].TR.ParentQuad = BoxList[i].BL.ParentQuad = ParentQuad;
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

            QDrawer = obj.QDrawer;

            InitRenderTargets(obj);

            UpdateEffectList();
        }

        public ObjectClass(QuadDrawer Drawer, GraphicsDevice device, EzEffect BaseEffect, EzTexture BaseTexture)
        {
            ObjectClassInit(Drawer, device, device.PresentationParameters, 0, 0, BaseEffect, BaseTexture);
        }

        public ObjectClass(QuadDrawer Drawer, GraphicsDevice device, PresentationParameters pp, int Width, int Height, EzEffect BaseEffect, EzTexture BaseTexture)
        {
            ObjectClassInit(Drawer, device, device.PresentationParameters, Width, Height, BaseEffect, BaseTexture);
        }

        public void ObjectClassInit(QuadDrawer Drawer, GraphicsDevice device, PresentationParameters pp, int Width, int Height, EzEffect BaseEffect, EzTexture BaseTexture)
        {
            VersionNumber = ObjectClassVersionNumber;

            AnimQueue = new Queue<AnimQueueEntry>();

            CenterFlipOnBox = true;

            OutlineWidth = new Vector2(1);
            OutlineColor = Color.Black;

            ParentQuad = new Quad();
            ParentQuad.ParentObject = this;
            ParentQuad.MyEffect = BaseEffect;
            ParentQuad.MyTexture = BaseTexture;

            QuadList = new List<BaseQuad>();
            BoxList = new List<ObjectBox>();

            AnimLength = new int[50];
            AnimSpeed = new float[50];
            AnimName = new string[50];
            for (int i = 0; i < 50; i++)
            {
                AnimName[i] = "Anim_" + i.ToString();
                AnimSpeed[i] = 1f;
            }

            QDrawer = Drawer;

            if (Height > 0 && Width > 0)
                InitRenderTargets(device, pp, Width, Height);

            UpdateEffectList();
        }

        public void MakeRenderTargetUnique(int width, int height)
        {
            if (!OriginalRenderTarget)
                InitRenderTargets(Tools.Device, Tools.Device.PresentationParameters, width, height);
        }

        public bool OriginalRenderTarget = true;
        private void InitRenderTargets(ObjectClass obj)
        {
            OriginalRenderTarget = false;

            DrawWidth = obj.DrawWidth;
            DrawHeight = obj.DrawHeight;

            ObjectRenderTarget = obj.ObjectRenderTarget;
        }

        private void InitRenderTargets(GraphicsDevice device, PresentationParameters pp, int Width, int Height)
        {
            OriginalRenderTarget = true;

            DrawWidth = Width;
            DrawHeight = Height;

            ObjectRenderTarget = new RenderTarget2D(device,
                DrawWidth, DrawHeight, false,
                pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            ToTextureRenderTarget = new RenderTarget2D(device,
                DrawWidth, DrawHeight, false,
                pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);
        }

        public List<BaseQuad> FindQuads(params string[] names)
        {
            List<BaseQuad> list = new List<BaseQuad>();
            foreach (string name in names)
                list.Add(FindQuad(name));
            return list;
        }
        public BaseQuad FindQuad(string name)
        {
            foreach (BaseQuad quad in QuadList)
            {
                if (string.Compare(quad.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return quad;
            }

            return null;
        }

        void AddToNewList(List<BaseQuad> NewList, BaseQuad quad)
        {
            if (quad == ParentQuad || NewList.Contains(quad))
                return;
            if (quad.ParentQuad != null)
                AddToNewList(NewList, quad.ParentQuad);
            NewList.Add(quad);
        }
        public void Sort()
        {
            List<BaseQuad> NewList = new List<BaseQuad>();

            foreach (BaseQuad quad in QuadList)
                AddToNewList(NewList, quad);
            QuadList = NewList;
        }

        /// <summary>
        /// Scale the object's ParentQuad
        /// </summary>
        /// <param name="scale">The amount to scale by, as a ratio</param>
        public void Scale(float scale)
        {
            ParentQuad.Scale(new Vector2(scale, scale));
        }

        /// <summary>
        /// Scale the object's ParentQuad
        /// </summary>
        /// <param name="scale">The amount to scale by, as a ratio</param>
        public void Scale(Vector2 scale)
        {
            ParentQuad.Scale(scale);
        }

        /// <summary>
        /// Move the object to a new position, updating the entire structure
        /// </summary>
        /// <param name="NewPosition">The new position</param>
        public void MoveTo(Vector2 NewPosition)
        {
            MoveTo(NewPosition, true);
        }

        /// <summary>
        /// Move the object to a new position, updating the entire structure
        /// </summary>
        /// <param name="NewPosition">The new position</param>
        /// <param name="Update">Whether to update the object structure</param>
        public void MoveTo(Vector2 NewPosition, bool UpdateObject)
        {
            ParentQuad.Center.Move(NewPosition);
            ParentQuad.Update();
            if (UpdateObject)
                Update(null);
        }

        public void SetColor(Color color)
        {
            foreach (BaseQuad quad in QuadList)
                quad.SetColor(color);
        }

        public void UpdateBoxes()
        {
            foreach (ObjectBox box in BoxList)
                box.Update();
        }

        public void Update(BaseQuad quad) { Update(quad, ObjectDrawOrder.None, 1); }
        public void Update(BaseQuad quad, ObjectDrawOrder Exclude) { Update(quad, Exclude, 1); }
        public void Update(BaseQuad quad, ObjectDrawOrder Exclude, float Expand)
        {
            ParentQuad.Update();

            UpdateBoxes();

            if (!BoxesOnly && QuadList != null)
            {
                if (Exclude != ObjectDrawOrder.None)
                {
                    foreach (BaseQuad _quad in QuadList)
                    {
                        Quad __quad = _quad as Quad;
                        if (_quad.MyDrawOrder != Exclude || (__quad != null && __quad.Children.Count > 0 && __quad.Children[0].MyDrawOrder != Exclude))
                            _quad.Update(Expand);
                    }
                }
                else
                    foreach (BaseQuad _quad in QuadList)
                        _quad.Update(Expand);
            }

            if (BoxList.Count > 1)
                FlipCenter = BoxList[1].Center();
            else if (BoxList.Count > 0)
                FlipCenter = BoxList[0].Center();
            else
                FlipCenter = ParentQuad.Center.Pos;
        }
        /*
                public void Update(BaseQuad quad)
                {
                    if (quad == null)
                        ParentQuad.Update();

                    if (quad == null)
                    {
                        foreach (ObjectBox box in BoxList)
                            box.Update();
                        if (!BoxesOnly)
                            foreach (BaseQuad _quad in QuadList)
                                Update(_quad);
                    }
                    else
                    {
                        if (quad.ParentQuad != ParentQuad && quad.ParentQuad != null)
                            Update(quad.ParentQuad);

                        quad.Update();
                    }

                    if (BoxList.Count > 0)
                        FlipCenter = BoxList[0].Center();
                    else
                        FlipCenter = ParentQuad.Center.Pos;
                }*/


        public void AddBox(ObjectBox box)
        {
            BoxList.Add(box);
            box.TR.ParentQuad = ParentQuad;
            box.TR.RelPosFromPos();
            box.BL.ParentQuad = ParentQuad;
            box.BL.RelPosFromPos();
        }

        public void AddQuad(Quad quad) { AddQuad(quad, true); }
        public void AddQuad(Quad quad, bool ChangeParent)
        {
            quad.Update();
            QuadList.Add(quad);
            quad.ParentObject = this;
            if (ChangeParent)
                ParentQuad.AddQuadChild(quad);
        }

        public void RemoveQuad(BaseQuad quad)
        {
            if (quad.ParentQuad != null)
                quad.ParentQuad.RemoveQuadChild(quad, false);

            if (quad is Quad)
            {
                List<BaseQuad> ChildQuads = new List<BaseQuad>(((Quad)quad).Children);
                foreach (BaseQuad child_quad in ChildQuads)
                    ((Quad)quad).RemoveQuadChild(child_quad);
            }

            QuadList.Remove(quad);
        }

        public void PreDraw(GraphicsDevice device, EzEffectWad EffectWad)
        {
            if (BoxList.Count > 0)
            {
                //Update(null);

                ObjTex = DrawTexture(device, EffectWad);
                //ObjDepthTex = DrawDepthTexture(device, EffectWad);
            }
        }

        public void ContainedDraw() { ContainedDraw(null); }
        public void ContainedDraw(SpriteAnimGroup AnimGroup)
        {
            if (BoxList.Count > 0)
            {
                ContainedQuad.MyEffect = MySkinEffect;// ParentQuad.MyEffect;
                //quad.Scale(new Vector2(1f / 8, 1f / 8));

                float scalex = (BoxList[0].TR.Pos.X - BoxList[0].BL.Pos.X) / 2;
                float scaley = (BoxList[0].TR.Pos.Y - BoxList[0].BL.Pos.Y) / 2;
                float locx = (BoxList[0].TR.Pos.X + BoxList[0].BL.Pos.X) / 2;
                float locy = (BoxList[0].TR.Pos.Y + BoxList[0].BL.Pos.Y) / 2;
                if (xFlip) locx = FlipCenter.X - (locx - FlipCenter.X);
                if (yFlip) locy = FlipCenter.Y - (locy - FlipCenter.Y);

                ContainedQuad.Center.Move(new Vector2(locx, locy));

                ContainedQuad.xAxis.RelPos = new Vector2(1, 0);
                ContainedQuad.yAxis.RelPos = new Vector2(0, 1);
                if (ContainedQuadAngle != 0)
                {
                    ContainedQuad.PointxAxisTo(Tools.AngleToDir(ContainedQuadAngle));
                }
                //if (AnimGroup == null || !xFlip)
                //    ContainedQuad.Scale(new Vector2(scalex, scaley));
                //else
                //    ContainedQuad.Scale(new Vector2(-scalex, scaley));
                if (AnimGroup == null)
                    ContainedQuad.Scale(new Vector2(scalex, scaley));
                else
                    ContainedQuad.Scale(new Vector2(xFlip ? -scalex : scalex, yFlip ? -scaley : scaley));
                ContainedQuad.Update();

                if (AnimGroup == null)
                {
                    Tools.QDrawer.Flush();

                    // Rotate parent quad if needed
                    if (ContainedQuadAngle != 0)
                    {
                        EzEffect effect = Tools.BasicEffect;
                        effect.effect.Parameters["xCameraAngle"].SetValue(ContainedQuadAngle);
                        effect.effect.Parameters["Pivot"].SetValue(ContainedQuad.Center.Pos);
                        EffectTechnique HoldTechnique = effect.effect.CurrentTechnique;
                        effect.effect.CurrentTechnique = effect.effect.Techniques["PivotTechnique"];
                        Draw(Tools.EffectWad, false, ObjectDrawOrder.BeforeOutline);
                        effect.effect.CurrentTechnique = HoldTechnique;
                    }
                    else
                        Draw(Tools.EffectWad, false, ObjectDrawOrder.BeforeOutline);
                    Tools.QDrawer.Flush();

                    if (MySkinEffect != null)
                        ContainedQuad.MyEffect = MySkinEffect;
                    else
                        ContainedQuad.MyEffect = Tools.BasicEffect;

                    if (RefinedOutline)
                        ContainedQuad.MyEffect.effect.CurrentTechnique = ContainedQuad.MyEffect.effect.Techniques["RefinedOutline"];
                    else
                        ContainedQuad.MyEffect.effect.CurrentTechnique = ContainedQuad.MyEffect.effect.Techniques["Outline"];

                    ContainedQuad.MyEffect.effect.Parameters["OutlineScale"].SetValue(
                        //new Vector2(1 / scalex, 1 / scaley) * OutlineWidth);
                        OutlineWidth);
                    ContainedQuad.MyEffect.effect.Parameters["OutlineColor"].SetValue(OutlineColor.ToVector4());
                    ContainedQuad.MyEffect.effect.Parameters["InsideColor"].SetValue(InsideColor.ToVector4());
                    ContainedQuad.MyEffect.effect.Parameters["OutlineColor"].SetValue(OutlineColor.ToVector4());
                    ContainedQuad.MyEffect.effect.Parameters["InsideColor"].SetValue(InsideColor.ToVector4());
                    ContainedQuad.MyEffect.effect.Parameters["SceneTexture"].SetValue(ObjTex);
                    //ContainedQuad.MyEffect.effect.Parameters["xTexture"].SetValue(ObjTex);

                    //if (ContainedTexture == null)
                    //    ContainedTexture = new EzTexture();
                    //ContainedQuad.MyTexture = ContainedTexture;
                    //ContainedQuad.MyTexture.Tex = ObjTex;// ObjDepthTex;

                    if (MySkinTexture == null)
                        ContainedQuad.MyTexture = Tools.TextureWad.TextureList[0];
                    else
                        ContainedQuad.MyTexture = MySkinTexture;
                    // This line should not be commented for old stickman bob, with complex outline pixel shader.
                    //QDrawer.DrawQuad(ContainedQuad);
                    QDrawer.Flush();

                    ContainedQuad.MyEffect.effect.CurrentTechnique = ContainedQuad.MyEffect.Simplest;
                    ContainedQuad.MyEffect.effect.Parameters["OutlineScale"].SetValue(new Vector2(1, 1));

                    Tools.QDrawer.Flush();

                    // Rotate parent quad if needed
                    if (ContainedQuadAngle != 0)
                    {
                        EzEffect effect = Tools.BasicEffect;
                        effect.effect.Parameters["xCameraAngle"].SetValue(ContainedQuadAngle);
                        effect.effect.Parameters["Pivot"].SetValue(ContainedQuad.Center.Pos);
                        //effect.effect.Parameters["Illumination"].SetValue(Tools.QDrawer.GlobalIllumination);
                        EffectTechnique HoldEffect = effect.effect.CurrentTechnique;
                        effect.effect.CurrentTechnique = effect.effect.Techniques["PivotTechnique"];
                        Draw(Tools.EffectWad, false, ObjectDrawOrder.AfterOutline);
                        effect.effect.CurrentTechnique = HoldEffect;// effect.effect.Techniques["Simplest"];
                    }
                    else                    
                        Draw(Tools.EffectWad, false, ObjectDrawOrder.AfterOutline);
                    Tools.QDrawer.Flush();
                }
                else
                {
                    QDrawer.Flush();
                    Texture2D hold = ContainedQuad.MyTexture.Tex;
                    Vector2 padding = Vector2.Zero;
                    ContainedQuad.MyTexture.Tex = AnimGroup.Get(anim, t, ref padding); //AnimGroup.SpriteAnims[0].Frames[3];                                        
                    ContainedQuad.MyEffect = Tools.BasicEffect;
                    ContainedQuad.MyEffect.effect.CurrentTechnique = ContainedQuad.MyEffect.Simplest;
                    QDrawer.DrawQuad(ContainedQuad);
                    QDrawer.Flush();
                    ContainedQuad.MyTexture.Tex = hold;
                }
            }
        }

        public Quad ExtraQuadToDraw = null;
        public EzTexture ExtraQuadToDrawTexture = null;
        public bool DrawExtraQuad = false;

        public void Draw(bool UpdateFirst) { Draw(Tools.EffectWad, UpdateFirst); }
        public void Draw(EzEffectWad EffectWad, bool UpdateFirst) { Draw(EffectWad, UpdateFirst, ObjectDrawOrder.All); }
        public void Draw(EzEffectWad EffectWad, bool UpdateFirst, ObjectDrawOrder DrawOrder)
        {
            if (UpdateFirst)
                Update(null);

            //if (xFlip) foreach (EzEffect fx in EffectWad.EffectList) fx.xFlip.SetValue(true);
            //if (xFlip) foreach (EzEffect fx in EffectWad.EffectList) fx.FlipCenter.SetValue(FlipCenter);

            if ((xFlip || yFlip) && !BoxesOnly && QuadList != null)
                foreach (EzEffect fx in MyEffects) fx.FlipVector.SetValue(new Vector2(xFlip ? 1 : -1, yFlip ? 1 : -1));
            if (xFlip || yFlip)
                foreach (EzEffect fx in MyEffects) fx.FlipCenter.SetValue(FlipCenter);

            if (!BoxesOnly && QuadList != null)
                foreach (BaseQuad quad in QuadList)
                {
                    if (DrawOrder != ObjectDrawOrder.All)
                        if (quad.MyDrawOrder != DrawOrder) continue;

                    quad.Draw();
                }

            // Extra quad to draw. Pretty fucking leaky hack.
            if (DrawExtraQuad && ExtraQuadToDraw != null)
            {
                QDrawer.Flush();

                var Hold = ExtraQuadToDraw.MyTexture;
                ExtraQuadToDraw.MyTexture = ExtraQuadToDrawTexture;
                ExtraQuadToDraw.Draw(QDrawer);
                ExtraQuadToDraw.MyTexture = Hold;
            }

            QDrawer.Flush();

            if ((xFlip || yFlip) && !BoxesOnly && QuadList != null)
                foreach (EzEffect fx in MyEffects) fx.FlipVector.SetValue(new Vector2(-1, -1));
        }

        public SpriteAnim AnimToSpriteFrames(int anim, int NumFrames, bool Loop, Vector2 Padding)
        {
            return AnimToSpriteFrames(anim, NumFrames, Loop, 0, AnimLength[anim], Padding);
        }
        public SpriteAnim AnimToSpriteFrames(int anim, int NumFrames, bool Loop, float StartT, float EndT, Vector2 Padding)
        {
            SpriteAnim Sprites = new SpriteAnim();
            Sprites.Frames = new Texture2D[NumFrames];

            if (NumFrames <= 1)
                Sprites.dt = 1;
            else
            {
                if (Loop)
                    Sprites.dt = (EndT + 1) / (float)NumFrames;
                else
                    Sprites.dt = EndT / (float)NumFrames;
            }

            this.anim = anim;
            Update(null);
            for (int i = 0; i < NumFrames; i++)
            {
                SetAnimT(StartT + i * Sprites.dt, Loop);
                Update(null);
                Tools.Device.BlendState = BlendState.NonPremultiplied;
                PreDraw(Tools.Device, Tools.EffectWad);
                Tools.Device.BlendState = BlendState.AlphaBlend;
                Sprites.Frames[i] = DrawToTexture(Tools.Device, Tools.EffectWad, Padding);
            }

            return Sprites;
        }

        public Texture2D DrawToTexture(GraphicsDevice device, EzEffectWad EffectWad, Vector2 Padding)
        {
            Vector4 HoldCameraPos = EffectWad.CameraPosition;
            float HoldCameraAspect = EffectWad.EffectList[0].xCameraAspect.GetValueSingle();

            device.SetRenderTarget(ToTextureRenderTarget);
            device.Clear(Color.Transparent);
            foreach (EzEffect fx in MyEffects) fx.effect.CurrentTechnique = fx.Simplest;
            float scalex = Padding.X + (BoxList[0].TR.Pos.X - BoxList[0].BL.Pos.X) / 2;
            float scaley = Padding.Y + (BoxList[0].TR.Pos.Y - BoxList[0].BL.Pos.Y) / 2;
            float posx = (BoxList[0].TR.Pos.X + BoxList[0].BL.Pos.X) / 2;
            float posy = (BoxList[0].TR.Pos.Y + BoxList[0].BL.Pos.Y) / 2;
            if (xFlip) posx = FlipCenter.X - (posx - FlipCenter.X);
            if (yFlip) posy = FlipCenter.Y - (posy - FlipCenter.Y);

            EffectWad.SetCameraPosition(new Vector4(posx, posy, 1f / scalex, 1f / scaley));
            foreach (EzEffect fx in MyEffects) fx.xCameraAspect.SetValue(1);
            ContainedDraw(null);
            device.SetRenderTarget(Tools.DestinationRenderTarget);
            Tools.ResetViewport();

            EffectWad.SetCameraPosition(HoldCameraPos);
            foreach (EzEffect fx in MyEffects) fx.xCameraAspect.SetValue(HoldCameraAspect);

            Texture2D tex = ToTextureRenderTarget;
            Color[] Array = new Color[tex.Width * tex.Height];
            tex.GetData<Color>(Array);
            Texture2D tex2 = new Texture2D(Tools.Device, tex.Width, tex.Height);
            tex2.SetData<Color>(Array);

            Array = null;
            //tex.Dispose();

            return tex2;
        }
        public Texture2D DrawTexture(GraphicsDevice device, EzEffectWad EffectWad)
        {
            if (MyEffects == null) return ObjectRenderTarget;

            Vector4 HoldCameraPos = EffectWad.CameraPosition;
            float HoldCameraAspect = EffectWad.EffectList[0].xCameraAspect.GetValueSingle();
            
            device.SetRenderTarget(ObjectRenderTarget);
            device.Clear(Color.Blue);
            foreach (EzEffect fx in MyEffects) fx.effect.CurrentTechnique = fx.effect.Techniques["DepthVelocityInfo"];
            float scalex = (BoxList[0].TR.Pos.X - BoxList[0].BL.Pos.X) / 2;
            float scaley = (BoxList[0].TR.Pos.Y - BoxList[0].BL.Pos.Y) / 2;
            float posx = (BoxList[0].TR.Pos.X + BoxList[0].BL.Pos.X) / 2;
            float posy = (BoxList[0].TR.Pos.Y + BoxList[0].BL.Pos.Y) / 2;
            if (xFlip) posx = FlipCenter.X - (posx - FlipCenter.X);
            if (yFlip) posy = FlipCenter.Y - (posy - FlipCenter.Y);

            EffectWad.SetCameraPosition(new Vector4(posx, posy, 1f / scalex, 1f / scaley));
            foreach (EzEffect fx in MyEffects) fx.xCameraAspect.SetValue(1);
            Draw(EffectWad, false, ObjectDrawOrder.WithOutline);
            device.SetRenderTarget(Tools.DestinationRenderTarget);
            Tools.ResetViewport();

            EffectWad.SetCameraPosition(HoldCameraPos);
            foreach (EzEffect fx in MyEffects) fx.xCameraAspect.SetValue(HoldCameraAspect);
            foreach (EzEffect fx in MyEffects) fx.effect.CurrentTechnique = fx.Simplest;
            return ObjectRenderTarget;
        }
    }
}
