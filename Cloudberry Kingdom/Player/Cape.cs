﻿using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Drawing;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public struct CapeNode
    {
        public PhsxData Data, StartData;
        public int AnchorIndex;
        public Vector2 AnchorOffset;

        public float Weight;

        public bool Show;
    }

    public struct CapeLink
    {
        public int j, k;
        public float L, a_in, a_out;
        public bool Show;

        public void Init()
        {
            Show = true;
        }
    }

    public class Cape
    {
        public static Vector2 SineWind(Vector2 Dir, float NormalIntensity, float Period, float t)
        {
            Vector2 Normal = new Vector2(-Dir.Y, Dir.X);
            return Dir + NormalIntensity * Normal * (float)Math.Sin(t / Period);
        }

        public Bob MyBob;
        public int LastPhsxUpdate;

        public MyOwnVertexFormat[] Vertices;
        public int NumVertices, NumTriangles;


        public bool DoScaling = false;
        public Vector2 ScaleCenter;
        public Vector2 Scale;

        public Vector2[] AnchorPoint = new Vector2[2];

        public CapeNode[] Nodes;
        public CapeLink[] Links;



        public Color _MyColor;
        public Color _MyOutlineColor;
        public Color MyColor { set { SetColor(value); } get { return _MyColor; } }
        public Color MyOutlineColor { set { SetOutlineColor(value); } get { return _MyOutlineColor; } }

        public QuadClass MyQuad;

        int Sections, HorizontalSections;
        float MaxForce;
        public int PhsxSteps = 5;
        public float strength_in = 2;
        float strength_out;
        public float Retard = .99f;
        public Vector2 Gravity = new Vector2(0, -1.76f);
        public bool GroundCollision = false;
        public float GroundHeight = 50;
        public float GravityScale = 1;

        public Vector2 Wind = Vector2.Zero;

        bool DrawLines, DrawNodes;

        public Vector2 p1, p2;

        public void Release()
        {
            MyBob = null;

            Vertices = null;
            Nodes = null;
            Links = null;

            if (MyQuad != null) MyQuad.Release(); MyQuad = null;
        }

        public void SetOutlineColor(Color clr)
        {
            if (clr == _MyOutlineColor) return;

            _MyOutlineColor = clr;
        }        

        public void SetColor(Color clr)
        {
            if (clr == MyColor) return;

            _MyColor = clr;
            Color PremultipliedColor = Tools.PremultiplyAlpha(clr);

            int count = 0;
            // Triangles
            float h2 = 1f / Sections;
            float h1 = 1f / HorizontalSections;
            for (int i = Sections - 1; i >= 0; i--)
            {
                for (int j = 0; j < HorizontalSections; j++)
                {
                    Vertices[count].Color = PremultipliedColor;
                    Vertices[count].uv = new Vector2(j * h1, i * h2);

                    Vertices[count + 1].Color = PremultipliedColor;
                    Vertices[count + 1].uv = new Vector2((j + 1) * h1, i * h2);

                    Vertices[count + 2].Color = PremultipliedColor;
                    Vertices[count + 2].uv = new Vector2(j * h1, (i + 1) * h2);

                    Vertices[count + 3].Color = PremultipliedColor;
                    Vertices[count + 3].uv = new Vector2((j + 1) * h1, (i + 1) * h2);

                    Vertices[count + 5].Color = PremultipliedColor;
                    Vertices[count + 5].uv = new Vector2((j + 1) * h1, i * h2);

                    Vertices[count + 4].Color = PremultipliedColor;
                    Vertices[count + 4].uv = new Vector2(j * h1, (i + 1) * h2);

                    count += 6;
                }
            }
        }

        public void Move(Vector2 Shift)
        {
            for (int i = 0; i < Nodes.Length; i++)
            {
                Nodes[i].Data.Position += Shift;
            }

            UpdateTriangles();
        }

        public void ModVel_Add(Vector2 Mod)
        {
            for (int i = 0; i < Nodes.Length; i++)
                Nodes[i].Data.Velocity += Mod;
        }

        public void ModVel_Mult(Vector2 Mod)
        {
            for (int i = 0; i < Nodes.Length; i++)
                Nodes[i].Data.Velocity *= Mod;
        }

        public void Reset()
        {
            for (int i = 0; i < Nodes.Length; i++)
            {
                Nodes[i].Data = Nodes[i].StartData;
                Nodes[i].Data.Position += AnchorPoint[0];
            }
        }

        public enum CapeType { Normal, Small, None };
        public Cape(Bob bob, CapeType Type)
        {
            MyBob = bob;

            MaxForce = .9f;
            Sections = 5;
            HorizontalSections = 1;
            PhsxSteps = 1;
            strength_in = .16f / 1.6f;
            strength_out = 0;
            float cross_strength_out = .005f;// 1.8f;
            float cross_strength_in = 0.11f;
            float horizontal_strength_out = .005f;// 1.8f;
            float horizontal_strength_in = 0.11f;
            Retard = .935f;
            Gravity = new Vector2(0, -1.45f) / 1.45f;
            float Weight = 1.6f / Sections;

            DrawLines = true;
            DrawNodes = true;

            p1 = new Vector2(-63, -45);
            p2 = new Vector2(-27, 0);


            switch (Type)
            {
                case CapeType.Small:
                    strength_in *= 1.5f;
                    p2 = new Vector2(-17, -2);
                    p1 = new Vector2(-34, -25);
                    break;
            }




            MyQuad = new QuadClass();
            MyQuad.SetToDefault();
            //MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName("FallingBlock13");//"White");
            //MyQuad.Quad.MyEffect = Tools.BasicEffect;


            //NumTriangles = 4 * Sections * HorizontalSections;
            NumTriangles = 2 * Sections * HorizontalSections;
            NumVertices = 3 * NumTriangles;
            Vertices = new MyOwnVertexFormat[NumVertices];


            Nodes = new CapeNode[(1 + 1 * Sections) * (HorizontalSections + 1)];
            Links = new CapeLink[Sections * (HorizontalSections + 1) +
                                 (Sections + 1) * HorizontalSections +
                                 Sections + 1 +
                                2 * Sections * HorizontalSections
            ];

            Vector2 TL = p1;
            Vector2 BL = p2;
            for (int i = 0; i < Sections + 1; i++)
            {
                float t = i / (float)Sections;
                Vector2 pos1 = t * TL + (1 - t) * BL;
                Vector2 pos2 = pos1;
                pos2.X *= -1;
                for (int j = 0; j <= HorizontalSections; j++)
                {
                    float s = j / (float)HorizontalSections;

                    Nodes[i * (HorizontalSections + 1) + j].Data.Position = s * pos2 + (1 - s) * pos1;
                    Nodes[i * (HorizontalSections + 1) + j].Data.Position.Y -= 30 * (1 - 2 * Math.Abs(s - .5f)) * t;
                    Nodes[i * (HorizontalSections + 1) + j].Show = j == 0 || j == HorizontalSections || i == Sections;
                    Nodes[i * (HorizontalSections + 1) + j].AnchorIndex = -1;

                    if (i == 0)
                    {
                        Nodes[i * (HorizontalSections + 1) + j].AnchorOffset = Nodes[i * HorizontalSections + j].Data.Position;
                        Nodes[i * (HorizontalSections + 1) + j].AnchorIndex = 0;
                    }
                }
            }

            for (int i = 0; i < Nodes.Length; i++)
            {
                Nodes[i].StartData = Nodes[i].Data;
                Nodes[i].Weight = Weight;
            }

            int count = 0;
            // Triangles
            float h2 = 1f / Sections;
            float h1 = 1f / HorizontalSections;
            for (int i = 0; i < Sections; i++)
            {
                for (int j = 0; j < HorizontalSections; j++)
                {
                    Vertices[count].uv = new Vector2(j * h1, i * h2);
                    Vertices[count].xy = Nodes[i * (HorizontalSections + 1) + j].Data.Position;

                    Vertices[count + 1].uv = new Vector2((j + 1) * h1, i * h2);
                    Vertices[count + 1].xy = Nodes[i * (HorizontalSections + 1) + j + 1].Data.Position;

                    Vertices[count + 2].uv = new Vector2(j * h1, (i + 1) * h2);
                    Vertices[count + 2].xy = Nodes[(i + 1) * (HorizontalSections + 1) + j].Data.Position;


                    Vertices[count + 3].uv = new Vector2((j + 1) * h1, (i + 1) * h2);
                    Vertices[count + 3].xy = Nodes[(i + 1) * (HorizontalSections + 1) + (j + 1)].Data.Position;

                    Vertices[count + 4].uv = new Vector2((j + 1) * h1, i * h2);
                    Vertices[count + 4].xy = Nodes[i * (HorizontalSections + 1) + j + 1].Data.Position;

                    Vertices[count + 5].xy = Nodes[(i + 1) * (HorizontalSections + 1) + j].Data.Position;

                    count += 6;
                }
            }

            count = 0;
            // Links
            for (int i = 0; i < Links.Length; i++) Links[i].Init();

            // Vertical
            for (int i = 0; i < Sections; i++)
            {
                for (int j = 0; j <= HorizontalSections; j++)
                {
                    Links[count].j = i * (HorizontalSections + 1) + j;
                    Links[count].k = (i + 1) * (HorizontalSections + 1) + j;
                    Links[count].a_in = strength_in;
                    Links[count].a_out = strength_out;
                    Links[count].Show = j == 0 || j == HorizontalSections;

                    count++;
                }
            }

            // Horizontal
            for (int i = 0; i <= Sections; i++)
            {
                for (int j = 0; j < HorizontalSections; j++)
                {
                    Links[count].j = i * (HorizontalSections + 1) + j;
                    Links[count].k = i * (HorizontalSections + 1) + j + 1;
                    Links[count].a_in = horizontal_strength_in;
                    Links[count].a_out = horizontal_strength_out;
                    Links[count].Show = i == Sections;

                    count++;
                }
            }

            
            // Horizontal props
            for (int i = 0; i <= Sections; i++)
            {
                int j = 0;
                //for (int j = 0; j < HorizontalSections; j++)
                {
                    Links[count].j = i * (HorizontalSections + 1) + j;
                    Links[count].k = i * (HorizontalSections + 1) + HorizontalSections;
                    Links[count].a_in = horizontal_strength_in;
                    Links[count].a_out = horizontal_strength_out;
                    Links[count].Show = false;

                    count++;
                }
            }
            
            // Cross 1
            for (int i = 0; i < Sections; i++)
            {
                for (int j = 0; j < HorizontalSections; j++)
                {
                    Links[count].j = i * (HorizontalSections + 1) + j;
                    Links[count].k = (i + 1) * (HorizontalSections + 1) + j + 1;
                    Links[count].a_in = cross_strength_in;
                    Links[count].a_out = cross_strength_out;
                    //Links[count].Show = i == j;
                    Links[count].Show = false;

                    count++;
                }
            }

            // Cross 2
            for (int i = 1; i <= Sections; i++)
            {
                for (int j = 0; j < HorizontalSections; j++)
                {
                    Links[count].j = i * (HorizontalSections + 1) + j;
                    Links[count].k = (i - 1) * (HorizontalSections + 1) + j + 1;
                    Links[count].a_in = cross_strength_in;
                    Links[count].a_out = cross_strength_out;
                    //Links[count].Show = i + j == HorizontalSections;
                    Links[count].Show = false;

                    count++;
                }
            }

            for (int i = 0; i < Links.Length; i++)
            {
                int j = Links[i].j;
                int k = Links[i].k;
                Links[i].L = Vector2.Distance(Nodes[j].Data.Position, Nodes[k].Data.Position);
            }
        }

        public void PhsxStep()
        {
            int CurStep;

            if (MyBob == null) return;
            if (MyBob.Core.MyLevel == null) return;
            if (MyBob.Core.MyLevel != Tools.CurLevel) return;
            CurStep = MyBob.Core.MyLevel.GetPhsxStep();
            if (!MyBob.CharacterSelect2 && LastPhsxUpdate == CurStep)
                return;

            LastPhsxUpdate = CurStep;

            MyColor = MyBob.MyColorScheme.CapeColor.Clr;

            for (int J = 0; J < PhsxSteps; J++)
            {
                for (int i = 0; i < Nodes.Length; i++)
                {
                    if (Nodes[i].AnchorIndex >= 0)
                    {
                        Nodes[i].Data.Position = AnchorPoint[Nodes[i].AnchorIndex] + Nodes[i].AnchorOffset;
                    }
                    else
                    {
                        // Integrate
                        Nodes[i].Data.Position += (Nodes[i].Data.Velocity + 0*Wind) / PhsxSteps;
                        Nodes[i].Data.Velocity += GravityScale * Gravity / PhsxSteps + Wind;
                        Nodes[i].Data.Velocity *= (float)Math.Pow(Retard / (1 + Nodes[i].Data.Velocity.LengthSquared() / 1500), 1f / PhsxSteps);

                        // Check if we are below the ground
                        if (GroundCollision)
                        {
                            if (Nodes[i].Data.Position.Y < AnchorPoint[0].Y - GroundHeight)
                                Nodes[i].Data.Position.Y = AnchorPoint[0].Y - GroundHeight;
                        }
                    }
                }

                for (int i = 0; i < Links.Length; i++)
                {
                    int j = Links[i].j;
                    int k = Links[i].k;
                    float L = Links[i].L;
                    float l = Vector2.Distance(Nodes[j].Data.Position, Nodes[k].Data.Position);

                    // Shorten link if too long
                    float ext = 1.8f;
                    if (l > ext * L)
                    {
                        Vector2 Center = (Nodes[j].Data.Position + Nodes[k].Data.Position) / 2;
                        Vector2 dif;
                        dif = (Nodes[j].Data.Position - Center); dif.Normalize();
                        Nodes[j].Data.Position = Center + ext * L / 2 * dif;
                        dif = (Nodes[k].Data.Position - Center); dif.Normalize();
                        Nodes[k].Data.Position = Center + ext * L / 2 * dif;
                    }

                    Vector2 F = Nodes[j].Data.Position - Nodes[k].Data.Position;
                    float Force = F.Length();
                    if (Force > MaxForce) F = MaxForce * F / Force;
                    if (l < L) F *= -Links[i].a_out * (l - L);
                    if (l > L) F *= -Links[i].a_in * (l - L);
                    F /= PhsxSteps;
                    Nodes[j].Data.Velocity += F / Nodes[j].Weight;
                    Nodes[k].Data.Velocity -= F / Nodes[j].Weight;
                }
            }

            UpdateTriangles();
        }

        void UpdateTriangles()
        {
            int count = 0;
            // Triangles
            float h2 = 1f / Sections;
            float h1 = 1f / HorizontalSections;
            for (int i = Sections - 1; i >= 0; i--)
            {
                for (int j = 0; j < HorizontalSections; j++)
                {
                    Vertices[count].xy = Nodes[i * (HorizontalSections + 1) + j].Data.Position;

                    Vertices[count + 1].xy = Nodes[i * (HorizontalSections + 1) + j + 1].Data.Position;

                    Vertices[count + 2].xy = Nodes[(i + 1) * (HorizontalSections + 1) + j].Data.Position;

                    Vertices[count + 3].xy = Nodes[(i + 1) * (HorizontalSections + 1) + (j + 1)].Data.Position;

                    Vertices[count + 5].xy = Nodes[i * (HorizontalSections + 1) + j + 1].Data.Position;

                    Vertices[count + 4].xy = Nodes[(i + 1) * (HorizontalSections + 1) + j].Data.Position;

                    count += 6;
                }
            }

            if (DoScaling)
                for (int i = 0; i < Vertices.Length; i++)
                    ApplyScaling(ref Vertices[i].xy);
        }

        void ApplyScaling(ref Vector2 v)
        {
            v = (v - ScaleCenter) * Scale + ScaleCenter;
        }

        public void Draw()
        {
            Tools.QDrawer.Flush();

            EzEffect Effect = MyQuad.Quad.MyEffect;

            if (!Effect.IsUpToDate)
                Effect.SetCameraParameters();

            
            Effect.xTexture.SetValue(MyQuad.Quad.MyTexture.Tex);
            Effect.effect.CurrentTechnique.Passes[0].Apply();

            Tools.QDrawer.SetAddressMode(true, true);

            Tools.Device.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices, 0, NumTriangles);

            Tools.QDrawer.Flush();

            float width = 16;
            if (DoScaling) width *= Scale.X;

            if (DrawLines)
                for (int i = 0; i < Links.Length; i++)
                {
                    if (Links[i].Show)
                    {
                        int j = Links[i].j;
                        int k = Links[i].k;

                        Vector2 p1 = Nodes[j].Data.Position;
                        Vector2 p2 = Nodes[k].Data.Position;
                        
                        if (DoScaling)
                        {
                            ApplyScaling(ref p1); ApplyScaling(ref p2);
                        }
                        Tools.QDrawer.DrawLine(p1, p2, MyOutlineColor, width, Tools.TextureWad.TextureList[2], Tools.EffectWad.EffectList[0], 100, 0, true);
                    }
                }

            if (DrawNodes)
                for (int i = 0; i < Nodes.Length; i++)
                {
                    if (Nodes[i].Show)
                    {
                        Vector2 p = Nodes[i].Data.Position;
                        if (DoScaling) ApplyScaling(ref p);
                        Tools.QDrawer.DrawSquareDot(p, MyOutlineColor, width, Tools.TextureWad.TextureList[1], Tools.EffectWad.EffectList[0]);
                    }
                }
        }
    }
}