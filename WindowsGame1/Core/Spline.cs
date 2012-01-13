using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.IO;

using Drawing;

namespace Drawing
{
    public class Spline
    {
        public int Nodes;
        public ObjectVector[] Node;
        public ObjectVector[] Control;

        public void Release()
        {
            for (int i = 0; i < Nodes; i++)
            {
                Node[i].Release();
                Control[i].Release();
            }
            Node = null;
            Control = null;
        }

        public void SetHold()
        {
            for (int i = 0; i < Nodes; i++)
            {
                Node[i].AnimData.Hold = Node[i].RelPos;
                Control[i].AnimData.Hold = Control[i].RelPos;
            }
        }

#if EDITOR
        public void SaveState(int StateIndex)
        {
            for (int i = 0; i < Nodes; i++)
            {
                Node[i].SaveState(StateIndex);
                Control[i].SaveState(StateIndex);
            }
        }

        public void RecoverState(int StateIndex)
        {
            for (int i = 0; i < Nodes; i++)
            {
                Node[i].RecoverState(StateIndex);
                Control[i].RecoverState(StateIndex);
            }
        }
#endif

        public void ReadAnim(int anim, int frame)
        {
            for (int i = 0; i < Nodes; i++)
            {
                Node[i].RelPos = Node[i].AnimData.Get(anim, frame);
                Control[i].RelPos = Control[i].AnimData.Get(anim, frame);
            }
        }

        public void Record(int anim, int frame, bool UseRelativeCoords)
        {
            if (UseRelativeCoords)
            {
                for (int i = 0; i < Nodes; i++)
                {
                    Node[i].AnimData.Set(Node[i].RelPos, anim, frame);
                    Control[i].AnimData.Set(Control[i].RelPos, anim, frame);
                }
            }
            else
            {
                for (int i = 0; i < Nodes; i++)
                {
                    Node[i].AnimData.Set(Node[i].Pos, anim, frame);
                    Control[i].AnimData.Set(Control[i].Pos, anim, frame);
                }
            }
        }

        public void Transfer(int anim, float DestT, int AnimLength, bool Loop, bool Linear, float t)
        {
            for (int i = 0; i < Nodes; i++)
            {
                Node[i].RelPos = Node[i].AnimData.Transfer(anim, DestT, AnimLength, Loop, Linear, t);
                Control[i].RelPos = Control[i].AnimData.Transfer(anim, DestT, AnimLength, Loop, Linear, t);
            }
        }

        public void Calc(int anim, float t, int AnimLength, bool Loop, bool Linear)
        {
            for (int i = 0; i < Nodes; i++)
            {
                Node[i].RelPos = Node[i].AnimData.Calc(anim, t, AnimLength, Loop, Linear);
                Control[i].RelPos = Control[i].AnimData.Calc(anim, t, AnimLength, Loop, Linear);
            }
        }

        public void Write(BinaryWriter writer, ObjectClass MainObject)
        {
            writer.Write(Nodes);
            for (int i = 0; i < Nodes; i++)
            {
                Node[i].Write(writer, MainObject);
                Control[i].Write(writer, MainObject);
            }
        }

        public void Read(BinaryReader reader, ObjectClass MainObject)
        {
            Nodes = reader.ReadInt32();
            Initialize(Nodes, Vector2.Zero);

            for (int i = 0; i < Nodes; i++)
            {
                Node[i].Read(reader, MainObject);
                Control[i].Read(reader, MainObject);
            }
        }

        public Spline(Spline spline, bool DeepClone)
        {
            Initialize(spline.Nodes, new Vector2(0, 0));
            /*
            List<ObjectVector> L1, L2;
            L1 = spline.GetObjectVectors();
            L2 = GetObjectVectors();

            for (int i = 0; i < L1.Count; i++)
            {
                L1[i].Clone(L2[i]);
            }*/
            for (int i = 0; i < Nodes; i++)
            {
                spline.Node[i].Clone(Node[i], DeepClone);
                spline.Control[i].Clone(Control[i], DeepClone);
            }
        }
        public Spline() { }
        public Spline(int nodes, Vector2 Top) { Initialize(nodes, Top); }

        public void CopyAnim(Spline spline, int Anim)
        {
            for (int i = 0; i < Math.Min(Nodes, spline.Nodes); i++)
            {
                Node[i].CopyAnim(spline.Node[i], Anim);
                Control[i].CopyAnim(spline.Control[i], Anim);
            }            
        }

        public void CopyAnimShallow(Spline spline, int Anim)
        {
            for (int i = 0; i < Math.Min(Nodes, spline.Nodes); i++)
            {
                Node[i].AnimData = spline.Node[i].AnimData;
                Control[i].AnimData = spline.Control[i].AnimData;
            }
        }
#if EDITOR
        public List<ObjectVector> GetObjectVectors()
        {
            List<ObjectVector> ObjectVectorList = new List<ObjectVector>();

            for (int i = 0; i < Nodes; i++)
            {
                ObjectVectorList.Add(Node[i]);
                ObjectVectorList.Add(Control[i]);
            }

            return ObjectVectorList;
        }
#endif

        public Vector2 ClosestPoint(Vector2 point, Vector2 start)
        {
            float ClosestDist = 100;
            Vector2 Closest = new Vector2(0, 0);
            for (float t = start.X - .15f; t < start.X + .15f; t += .0005f)
            {
                for (float n = start.Y - .01f; n < start.Y + .01f; n += .0005f)
                {
                    float Dist = Vector2.Distance(GetVector(t, n), point);
                    if (Dist < ClosestDist)
                    {
                        ClosestDist = Dist;
                        Closest = new Vector2(t, n);
                    }
                }
            }

            return Closest;
        }

        public void Update()
        {
            for (int i = 0; i < Nodes; i++)
            {
                Node[i].PosFromRelPos();
                Control[i].PosFromRelPos();
            }
        }

        public void Initialize(int nodes, Vector2 Top)
        {
            Nodes = nodes;
            Node = new ObjectVector[Nodes];
            Control = new ObjectVector[Nodes];

            for (int i = 0; i < Nodes; i++)
            {
                Node[i] = new ObjectVector();
                Control[i] = new ObjectVector();
                Node[i].Pos = new Vector2(0, .15f * (2 * i + .75f)) + Top;
                Control[i].Pos = new Vector2(0, .15f * (2 * i + 0)) + Top;
                Node[i].RelPosFromPos();
                Control[i].RelPosFromPos();
            }
        }

        public static bool UsePrev;
        static Vector2 Prev;
        static public Vector2 Bezier(float t, Vector2 x0, Vector2 x1, Vector2 x2, Vector2 x3)
        {
            float t_2, t1_2;
            float t1 = 1 - t;
            t_2 = t * t;
            t1_2 = t1 * t1;
            return t1_2 * t1 * x0 + 3 * t1_2 * t * x1 + 3 * t1 * t_2 * x2 + t_2 * t * x3;
        }

        static public Vector2 BezierPrime(float t, Vector2 x0, Vector2 x1, Vector2 x2, Vector2 x3)
        {
            float t1 = 1 - t;
            return -3f * t1 * t1 * x0 + (3 * t1 * t1 - 6 * t1 * t) * x1 + (6 * t1 * t - 3 * t * t) * x2 + 3 * t * t * x3;
        }

        static public Vector2 BezierNormal(float t, Vector2 x0, Vector2 x1, Vector2 x2, Vector2 x3)
        {
            float t1 = 1 - t;
            Vector2 Tangent = -3f * t1 * t1 * x0 + (3 * t1 * t1 - 6 * t1 * t) * x1 + (6 * t1 * t - 3 * t * t) * x2 + 3 * t * t * x3;

            Vector2 Normal = new Vector2(Tangent.Y, -Tangent.X);
            Normal.Normalize();
            return Normal;
        }

        public Vector2 GetVector(float t, float n)
        {
            int i = (int)Math.Floor(t);
            i = Math.Max(0, Math.Min(Nodes - 2, i));
            t -= i;

            return Bezier(t, Node[i].RelPos, 2 * Node[i].RelPos - Control[i].RelPos, Control[i + 1].RelPos, Node[i + 1].RelPos)
                + n * BezierNormal(t, Node[i].RelPos, 2 * Node[i].RelPos - Control[i].RelPos, Control[i + 1].RelPos, Node[i + 1].RelPos);
        }

        public void GetBothVectors(float t, float n, ref Vector2 vec1, ref Vector2 vec2)
        {
            int i = (int)Math.Floor(t);
            i = Math.Max(0, Math.Min(Nodes - 2, i));
            t -= i;

            Vector2 BezPoint = Bezier(t, Node[i].RelPos, 2 * Node[i].RelPos - Control[i].RelPos, Control[i + 1].RelPos, Node[i + 1].RelPos);
            Vector2 normal = new Vector2();
            if (false)//UsePrev)
            {
                normal.X = BezPoint.Y - Prev.Y;
                normal.Y = Prev.X - BezPoint.X;
                normal.Normalize();
            }
            else
                normal = BezierNormal(t, Node[i].RelPos, 2 * Node[i].RelPos - Control[i].RelPos, Control[i + 1].RelPos, Node[i + 1].RelPos);
            vec1 = n * normal + BezPoint;
            vec2 = -n * normal + BezPoint;

            Prev = BezPoint;
            UsePrev = true;
        }

        public void Draw(QuadDrawer Drawer, float ScaleLines)
        {
            for (int i = 0; i < Nodes; i++)
            {
                Drawer.DrawLine(Node[i].Pos, Control[i].Pos, new Color(.2f, .2f, .8f), .016f * ScaleLines);
                Drawer.DrawSquareDot(Node[i].Pos, Color.PaleVioletRed, .033f * ScaleLines);
                Drawer.DrawSquareDot(Control[i].Pos, Color.PaleVioletRed, .033f * ScaleLines);
            }
        }
    }
}