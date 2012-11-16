//#define EDITOR
#if EDITOR
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using CloudberryKingdom;

namespace CoreEngine
{
    public class Game_Editor : Microsoft.Xna.Framework.Game
    {
        public static System.Windows.Forms.Control MainWindow;

        string TestString;

        bool SkipLoop = false;

        AnimationToolbox AnimToolBox;

        Toolbox ToolBox;
        TreeNode CurNodeDestination, GhostNode;
        TreeNode QuadNode, BoxNode;
        public static ObjectClass DraggingSource;
        int Count = 0;
        bool UpdateQuadList = false, UpdateQuadSelection = false;
        bool SkipExpandCollapseEvent = false;

        //ObjectSpace os;

        int anim, frame;
        bool SetToLoop;
        SetNameDialog Dlg;
        
        bool Additional = false; // When true additional widgets are displayed on selected quads
        bool Render = false;
        bool ShowTextureSelection = false;
        int TextureSelectHiddenCount = 0;
        Vector4 HoldCamera;
        int TextureSelectionPage = 0;
        int SelectedTextureIndex = 0;
        

        bool CameraMove = false;
        ChangeMode RecordMode = ChangeMode.SingleFrame;

        int[] SaveCustomClrs;
        IAsyncResult result;
        bool DialogUp = false;

        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        QuadDrawer QDrawer;
        SpriteBatch spriteBatch;
        float AspectRatio;
        int ScreenWidth, ScreenHeight;

        SpriteFont Font1;
        float fps;

        public WrappedFloat ResourceLoadedCountRef;

        Microsoft.Xna.Framework.Input.ButtonState LeftMouseButtonState, RightMouseButtonState;
        float[] LeftMouseClickTimeStamp = { 0, 0, 0, 0 };
        Vector2 DragStart, RightClickPos, RotationPoint;
        bool Dragging = false;

        List<ObjectClass> ObjectList;
        List<ObjectVector> SelectedPoints;
        public static List<BaseQuad> SelectedQuads;
        List<ObjectBox> SelectedBoxes;
        BaseQuad CopyQuad, CopiedParentQuad;
        Vector2 PrevMousePos, PrevMousePos_ScreenCoordinates;

        KeyboardState PrevKeyboardState;

        Effect effect, NoTextureEffect, CircleEffect;
        VertexDeclaration vertexDeclaration;
        Vector4 cameraPos;

        RenderTarget2D ObjectRenderTarget;
        Texture2D blob, WhiteTexture, backgroundTexture;

        EzEffectWad EffectWad;
        EzTextureWad TextureWad;

        //Spline TestSpline;
        //BendableQuad TestQuad;

        string CurrentFileName = "", CurrentFileLocation = "";

        public static ObjectClass MainObject;
        List<ObjectVector> SaveVecs;
        public static string s;

        public void UniqueAdd<T>(List<T> list, T element)
        {
            if (!list.Contains(element))
                list.Add(element);
        }

        public delegate Vector2 __ModPoint(Vector2 point, Vector2 center, float degree);
        Vector2 RotatePoint(Vector2 point, Vector2 RotationPoint, float angle)
        {
            Vector2 dif = point - RotationPoint;
            double ang = angle + Math.Atan2(dif.Y, dif.X);
            return RotationPoint + dif.Length() * new Vector2((float)Math.Cos(ang), (float)Math.Sin(ang));
        }
        Vector2 ScalePoint(Vector2 point, Vector2 RotationPoint, float amount)
        {
            Vector2 dif = point - RotationPoint;
            return RotationPoint + (1 + amount) * dif;
        }

        public Game_Editor()
        {
            Tools.GameClass = this;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //this.Components.Add(new GamerServicesComponent(this));
        }

        public string ContentDirectory;
        protected override void Initialize()
        {
            MainWindow = Control.FromHandle(this.Window.Handle);
            //MainWindow.Location = new System.Drawing.Point(10, 10);

            s = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()))), "Content\\Objects");
            //ScreenWidth = 1440;
            //ScreenHeight = 900;

            //ContentDirectory = Path.Combine(StorageContainer.TitleLocation, Content.RootDirectory);
            Globals.ContentDirectory = Content.RootDirectory;

            ScreenWidth = 650;
            ScreenHeight = 650;

            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            AspectRatio = (float)ScreenWidth / (float)ScreenHeight;
            graphics.IsFullScreen = false;
            graphics.PreferMultiSampling = true;

            graphics.ApplyChanges();
            Window.Title = "POS Editor <3";

            fps = 0;

            base.Initialize();
        }

        protected void LoadArtMusicSound(bool CreateNewWads)
        {
            Resources.LoadAssets(true);
            Resources.LoadResources_ImmediateForeground();

            Tools.Write("Art done...");
        }

        protected override void LoadContent()
        {
            EzTexture.game = this;

            //Globals.ContentDirectory = Path.Combine(StorageContainer.TitleLocation, Content.RootDirectory);
            Globals.ContentDirectory = Content.RootDirectory;

            Tools.Device = device = GraphicsDevice;

            //device.PresentationParameters.MultiSampleQuality = 1;
            //device.PresentationParameters.MultiSampleType = MultiSampleType.FourSamples;

            Tools.QDrawer = QDrawer = new QuadDrawer(device, 1000);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Font1 = Content.Load<SpriteFont>("Fonts/LilFont");

            RightMouseButtonState = LeftMouseButtonState = Microsoft.Xna.Framework.Input.ButtonState.Released;
            ObjectList = new List<ObjectClass>();
            SelectedPoints = new List<ObjectVector>();
            SelectedQuads = new List<BaseQuad>();
            SelectedBoxes = new List<ObjectBox>();
            PrevMousePos = new Vector2();

            WhiteTexture = Content.Load<Texture2D>("White");

            Tools.LoadEffects(Content, true);
            EffectWad = Tools.EffectWad;

            // Load the art!
            Tools.LoadBasicArt(Content);
            TextureWad = Tools.TextureWad;

            ResourceLoadedCountRef = new WrappedFloat();
            //Tools.LoadMoreArt(Content, ResourceLoadedCountRef);
            //Tools.TextureWad.LoadAll(1, Content, ResourceLoadedCountRef);
            LoadArtMusicSound(true);

            MainObject = new ObjectClass(QDrawer, graphics.GraphicsDevice, device.PresentationParameters, ScreenWidth / 8, ScreenHeight / 8, EffectWad.FindByName("BasicEffect"), TextureWad.FindByName("White"));
            ObjectList.Add(MainObject);

            QDrawer.DefaultEffect = EffectWad.FindByName("NoTexture");
            QDrawer.DefaultTexture = TextureWad.FindByName("WhiteTexture");

            SetUpCamera();
        }

        bool MouseOnScreen(MouseState State)
        {
            if (State.X < 0 || State.X > ScreenWidth) return false;
            if (State.Y < 0 || State.Y > ScreenHeight) return false;
            return true;
        }

        private Vector2 MouseCoordinate(MouseState State)
        {
            return new Vector2(
                cameraPos.X + 2 / cameraPos.Z * ((float)State.X / ScreenWidth - .5f) * AspectRatio,
                cameraPos.Y + 2 / cameraPos.W * (.5f - (float)State.Y / ScreenHeight));
        }

        private void SetUpCamera()
        {
            cameraPos = new Vector4(0, 0, 1, 1);
        }

        protected override void UnloadContent()
        {
        }

        void EndTetureDisplay()
        {
            ShowTextureSelection = false;
            cameraPos = HoldCamera;
        }

        int Delay = 0;
        protected override void Update(GameTime gameTime)
        {
            if (SkipLoop) return;

            Count++;

            if (Dlg != null) return;
            if (ToolBox == null)
            {
                ToolBox = new Toolbox();
                InitializeTree();
                ToolBox.Show();                
            }
            if (AnimToolBox == null)
            {
                AnimToolBox = new AnimationToolbox();
                //AnimToolBox.Show();
            }


            MouseState CurrentMouse = Mouse.GetState();
            Vector2 MousePos = MouseCoordinate(CurrentMouse);

            KeyboardState KeybState = Keyboard.GetState();
            if (PrevKeyboardState == null) PrevKeyboardState = KeybState;



            //MainObject.PlayUpdate((float)gameTime.ElapsedGameTime.TotalMilliseconds / 100);

            if (!Tools.StepControl || (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Enter) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Enter)))
            {
                foreach (ObjectClass obj in ObjectList)
                    if (Tools.StepControl)
                        obj.PlayUpdate(.1f);
                    else
                        obj.PlayUpdate((float)gameTime.ElapsedGameTime.TotalMilliseconds / 100);
            }

            if (!ShowTextureSelection)
            {
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Space) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Space))
                    Render = !Render;

                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Z) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Z))
                    CameraMove = !CameraMove;

                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.T) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.T))
                {
                    RecordMode++;
                    if ((int)RecordMode >= Tools.Length<ChangeMode>())
                        RecordMode = 0;
                }

                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Escape) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Escape))
                {
                    SelectedQuads.Clear();
                    SelectedBoxes.Clear();
                    UpdateQuadSelection = true;
                    SelectedPoints.Clear();
                }

                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Y) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Y))
                {
                    Additional = !Additional;
                }

                // Map texture coordinates from last box
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.X) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.X))
                {
                    ObjectBox box = MainObject.BoxList[MainObject.BoxList.Count - 1];
                    Vector2 size = box.TR.Pos - box.BL.Pos;

                    foreach (BaseQuad _quad in SelectedQuads)
                    {
                        if (_quad.MyDrawOrder == ObjectDrawOrder.WithOutline)
                            for (int i = 0; i < _quad.NumVertices; i++)
                            {
                                Vector2 UV = (_quad.Vertices[i].xy - box.BL.Pos) / size; ;
                                if (false)
                                    _quad.Vertices[i].uv = UV;
                                else
                                {
                                    Vector4 color = _quad.Vertices[i].Color.ToVector4();
                                    color.Y = UV.X;
                                    color.Z = UV.Y;
                                    _quad.Vertices[i].Color = new Color(color);
                                }
                            }
                    }
                }

                // Flip object about y-axis
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.F) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.F))
                {
                    MainObject.xFlip = !MainObject.xFlip;
                }

                // Copy vector states
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.LeftShift) && KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.C) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.C))
                {
                    SaveVecs = new List<ObjectVector>();
                    foreach (ObjectVector point in MainObject.GetObjectVectors())
                    {
                        ObjectVector NewPoint = new ObjectVector();
                        point.Clone(NewPoint, false);
                        SaveVecs.Add(NewPoint);
                    }
                }
                // Paste vector states
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.LeftShift) && KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.V) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.V))
                {
                    List<ObjectVector> L = MainObject.GetObjectVectors();
                    for (int i = 0; i < L.Count; i++)
                        SaveVecs[i].Clone(L[i], false);
                }

                // Copy quad
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.RightControl) && KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.C) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.C))
                {
                    if (SelectedQuads.Count > 0)
                    {
                        CopiedParentQuad = SelectedQuads[0].ParentQuad;
                        CopyQuad = new Quad((Quad)SelectedQuads[0], true);
                    }
                }
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.RightControl) && KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.V) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.V))
                {
                    if (CopyQuad != null)
                    {
                        Quad NewQuad = new Quad((Quad)CopyQuad, true);

                        NewQuad.Center.Move(MousePos);

                        NewQuad.Center.RelPosFromPos();
                            
                        MainObject.AddQuad(NewQuad);
                        UpdateQuadList = true;

                        // Add to same parent quad
                        // Make sure no one is set to be a child
                        foreach (BaseQuad quad in MainObject.QuadList)
                            quad.SetToBeChild = false;

                        NewQuad.ChildPoint.Click();
                        CopiedParentQuad.ParentPoint.Click();
                    }
                }

                // Box
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.B) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.B))
                {
                    ObjectBox Box = new ObjectBox();
                    Box.BL.Move(MainObject.CalcBLBound() - new Vector2(.05f, .05f));
                    Box.TR.Move(MainObject.CalcTRBound() + new Vector2(.05f, .05f));

                    MainObject.AddBox(Box);
                    UpdateQuadList = true;
                }

                // Animation
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.OemPeriod) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.OemPeriod))
                {
                    anim++;
                    frame = 0;
                    foreach (ObjectClass obj in ObjectList)
                        obj.Read(anim, frame);
                }
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.OemComma) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.OemComma))
                {
                    anim--;
                    if (anim < 0) anim = 0;
                    frame = 0;
                    foreach (ObjectClass obj in ObjectList)
                        obj.Read(anim, frame);
                }
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.OemSemicolon) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.OemSemicolon))
                {
                    frame++;
                    if (frame > MainObject.AnimLength[anim]) frame = MainObject.AnimLength[anim];

                    foreach (ObjectClass obj in ObjectList)
                        obj.Read(anim, frame);
                }
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.L) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.L))
                {
                    frame--;
                    if (frame < 0) frame = 0;

                    foreach (ObjectClass obj in ObjectList)
                        obj.Read(anim, frame);
                }

                // Insert frame
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.J) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.J))
                {
                    foreach (ObjectClass obj in ObjectList)
                        if (obj.AnimLength[anim] > 0)
                            obj.InsertFrame(anim, frame);
                }
                // Delete frame
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.K) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.K))
                {
                    foreach (ObjectClass obj in ObjectList)
                        if (obj.AnimLength[anim] > 0)
                            obj.DeleteFrame(anim, frame);
                }
                // Record
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.OemQuotes) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.OemQuotes))
                {
                    if (RecordMode != ChangeMode.SingleFrame)
                    {
                        foreach (BaseQuad quad in MainObject.QuadList)
                            if (quad is Quad)
                                ((Quad)quad).ModifyAllRecords(anim, frame, RecordMode);
                    }

                    foreach (ObjectClass obj in ObjectList)
                    {
                        if (obj.AnimLength[anim] < frame)
                            obj.AnimLength[anim] = frame;

                        obj.Record(anim, frame, true);
                        frame++;

                        // Update
                        if (frame <= obj.AnimLength[anim])
                        {
                            foreach (BaseQuad quad in obj.QuadList)
                                if (!SelectedQuads.Contains(quad))
                                    foreach (ObjectVector point in quad.GetObjectVectors())
                                        point.RelPos = point.AnimData.Get(anim, frame);
                            obj.ReadBoxData(anim, frame);
                        }
                    }
                }


                // Set anim speed
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.RightControl) && KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.OemTilde) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.OemTilde))
                {
                    Dlg = new SetNameDialog();

                    Dlg.textBox1.Text = MainObject.AnimSpeed[anim].ToString();
                    if (Dlg.ShowDialog() != DialogResult.Cancel)
                    {
                        float v;
                        if (float.TryParse(Dlg.textBox1.Text, out v))
                            foreach (ObjectClass obj in ObjectList)
                                obj.AnimSpeed[anim] = v;
                    }
                    Dlg = null;
                }

                // Set to loop
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.OemTilde) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.OemTilde))
                {
                    //MainObject.Loop = !MainObject.Loop;
                    SetToLoop = !SetToLoop;
                    foreach (ObjectClass obj in ObjectList)
                        obj.Loop = SetToLoop;
                }

                // Stop
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.E) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.E))
                    Tools.StepControl = !Tools.StepControl;


                // Play
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.P) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.P))
                {
                    foreach (ObjectClass obj in ObjectList)
                    {
                        obj.EnqueueAnimation(anim, 0, SetToLoop);
                        obj.Play = true;
                    }
                }

                // Name animation
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.OemQuestion) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.OemQuestion))
                {
                    Dlg = new SetNameDialog();

                    Dlg.textBox1.Text = MainObject.AnimName[anim];
                    if (Dlg.ShowDialog() != DialogResult.Cancel)
                        MainObject.AnimName[anim] = Dlg.textBox1.Text;

                    Dlg = null;
                }

                // Color dialog
                if (false)
                if (!KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.LeftShift) &&
                    !KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.RightControl) &&
                    KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.C) &&
                    !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.C))
                {
                    System.Windows.Forms.ColorDialog colorDlg = new System.Windows.Forms.ColorDialog();
                    colorDlg.AnyColor = true;
                    colorDlg.ShowHelp = true;
                    if (SaveCustomClrs != null) colorDlg.CustomColors = SaveCustomClrs;

                    if (colorDlg.ShowDialog() != DialogResult.Cancel)
                    {
                        SaveCustomClrs = colorDlg.CustomColors;
                        Color clr = new Color(colorDlg.Color.R, colorDlg.Color.G, colorDlg.Color.B);
                        foreach (BaseQuad quad in SelectedQuads)
                        {
                            quad.SetColor(clr);
                        }
                    }
                }


                // Draw order
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.OemMinus) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.OemMinus))
                {
                    foreach (BaseQuad quad in SelectedQuads)
                    {
                        int i = quad.ParentObject.QuadList.IndexOf(quad);
                        if (i > 0)
                        {
                            quad.ParentObject.QuadList.Remove(quad);
                            quad.ParentObject.QuadList.Insert(i - 1, quad);
                        }

                        i = quad.ParentQuad.Children.IndexOf(quad);
                        if (i > 0)
                        {
                            quad.ParentQuad.Children.Remove(quad);
                            quad.ParentQuad.Children.Insert(i - 1, quad);
                        }
                    }
                    UpdateQuadList = true;
                }
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.OemPlus) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.OemPlus))
                {
                    foreach (BaseQuad quad in SelectedQuads)
                    {
                        int i = quad.ParentObject.QuadList.IndexOf(quad);
                        if (i < quad.ParentObject.QuadList.Count - 1)
                        {
                            quad.ParentObject.QuadList.Remove(quad);
                            quad.ParentObject.QuadList.Insert(i + 1, quad);
                        }

                        i = quad.ParentQuad.Children.IndexOf(quad);
                        if (i < quad.ParentQuad.Children.Count - 1)
                        {
                            quad.ParentQuad.Children.Remove(quad);
                            quad.ParentQuad.Children.Insert(i + 1, quad);
                        }
                    }
                    UpdateQuadList = true;
                }

                // Save
                if (!DialogUp)
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.S) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.S))
                {
                    if (!KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.RightControl) &&
                        !KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.RightControl)
                        && CurrentFileLocation.Length > 0)
                    {
                        // Verify save
                        VerifySaveDialog verify = new VerifySaveDialog();
                        verify.FileName.Text = CurrentFileName;
                        DialogUp = true;
                        if (verify.ShowDialog() != DialogResult.Cancel)
                        {
                            FileStream stream = File.Open(CurrentFileLocation, FileMode.Create, FileAccess.Write, FileShare.None);
                            BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8);
                            MainObject.Write(writer);
                            writer.Close();
                            stream.Close();
                        }
                        verify.Close();
                        DialogUp = false;
                    }
                    else
                    {
                        DialogUp = true;

                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.Title = "Save as...";

                        ofd.InitialDirectory = s;
                        ofd.Filter = "Stickman Object file (*.smo)|*.smo";
                        ofd.CheckFileExists = false;
                        if (ofd.ShowDialog() != DialogResult.Cancel)
                        {
                            FileStream stream = File.Open(ofd.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
                            BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8);
                            MainObject.Write(writer);
                            writer.Close();
                            stream.Close();

                            CurrentFileLocation = ofd.FileName;
                            int i = CurrentFileLocation.LastIndexOf("\\");
                            CurrentFileName = CurrentFileLocation.Substring(i + 1);
                        }

                        DialogUp = false;
                    }
                }

                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.D) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.D))
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Title = "Load object...";

                    ofd.InitialDirectory = s;
                    ofd.Filter = "Stickman Object file (*.smo)|*.smo";
                    ofd.CheckFileExists = true;
                    if (ofd.ShowDialog() != DialogResult.Cancel)
                    {
                        ObjectList.Remove(MainObject);

                        FileStream stream = File.Open(ofd.FileName, FileMode.Open, FileAccess.Read, FileShare.None);

                        BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);
                        MainObject = new ObjectClass(QDrawer, graphics.GraphicsDevice, device.PresentationParameters, 400, 400, EffectWad.FindByName("BasicEffect"), TextureWad.FindByName("White"));
                        MainObject.ReadFile(reader, EffectWad, TextureWad);
                        reader.Close();
                        stream.Close();
                        MainObject.FinishLoading(QDrawer, device, TextureWad, EffectWad, device.PresentationParameters, 400, 400);

                        ObjectList.Add(MainObject);

                        CurrentFileLocation = ofd.FileName;
                        int i = CurrentFileLocation.LastIndexOf("\\");
                        CurrentFileName = CurrentFileLocation.Substring(i+1);

                        UpdateQuadList = true;
                    }
                }

                // Change quad effect
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Home) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Home))
                {
                    foreach (BaseQuad quad in SelectedQuads)
                    {
                        int i = EffectWad.EffectList.IndexOf(quad.MyEffect) + 1;
                        if (i >= EffectWad.EffectList.Count) i = 0;
                        quad.MyEffect = EffectWad.EffectList[i];
                    }
                }
            }

            // Change quad texture
            if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.PageUp) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.PageUp))
            {
                if (ShowTextureSelection)
                    TextureSelectionPage++;
                else
                {
                    ShowTextureSelection = true;
                    HoldCamera = cameraPos;
                    cameraPos = new Vector4(0, 0, 1, 1);

                    // If there is a quad selected, set the selected texture to that quads texture
                    if (SelectedQuads.Count != 0)
                        SelectedTextureIndex = TextureWad.TextureList.IndexOf(SelectedQuads[0].MyTexture);
                }
            }
            if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.PageDown) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.PageDown))
            {
                if (ShowTextureSelection)
                    TextureSelectionPage++;
            }

            if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Escape) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                if (ShowTextureSelection)
                    EndTetureDisplay();
            }

            if (!ShowTextureSelection)
            {
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.H) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.H))
                {
                    foreach (ObjectVector point in SelectedPoints)
                    {
                        point.ParentQuad = point.ParentQuad.ParentObject.ParentQuad;
                        point.RelPosFromPos();
                    }
                }


                // Rotate quad's UV coordinates
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.U) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.U))
                {
                    foreach (Quad quad in SelectedQuads)
                    {
                        Vector2[] uv = new Vector2[4];
                        for (int i = 0; i < 4; i++) uv[i] = quad.Vertices[i].uv;

                        quad.Vertices[0].uv = uv[2];
                        quad.Vertices[1].uv = uv[0];
                        quad.Vertices[2].uv = uv[3];
                        quad.Vertices[3].uv = uv[1];
                    }
                }

                // Mirror quad's UV coordinates
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.I) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.I))
                {
                    foreach (Quad quad in SelectedQuads)
                    {
                        Vector2[] uv = new Vector2[4];
                        for (int i = 0; i < 4; i++) uv[i] = quad.Vertices[i].uv;

                        quad.Vertices[0].uv = uv[2];
                        quad.Vertices[1].uv = uv[3];
                        quad.Vertices[2].uv = uv[0];
                        quad.Vertices[3].uv = uv[1];
                    }
                }



                // Create an object
                if (anim == 0 && frame == 0 && KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.A) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.A))
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Title = "Add an object to the scene...";

                    ofd.InitialDirectory = s;
                    ofd.Filter = "Stickman Object file (*.smo)|*.smo";
                    ofd.CheckFileExists = true;
                    if (ofd.ShowDialog() != DialogResult.Cancel)
                    {
                        FileStream stream = File.Open(ofd.FileName, FileMode.Open, FileAccess.Read, FileShare.None);

                        BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);
                        ObjectClass obj = new ObjectClass(QDrawer, graphics.GraphicsDevice, device.PresentationParameters, 400, 400, EffectWad.FindByName("BasicEffect"), TextureWad.FindByName("White"));
                        obj.ReadFile(reader, EffectWad, TextureWad);
                        reader.Close();
                        stream.Close();
                        obj.FinishLoading(QDrawer, device, TextureWad, EffectWad, device.PresentationParameters, 400, 400);

                        ObjectList.Add(obj);
                    }
                }

                // Create quad
                if (anim == 0 && frame == 0 && KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Q) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Q))
                {
                    Quad NewQuad = new Quad();
                    //NewQuad.Center.ParentQuad = MainObject.QuadList[0];

                    NewQuad.SetEffect("Basic", EffectWad);
                    NewQuad.MyTexture = TextureWad.TextureList[0];
                    NewQuad.Center.Move(MousePos);
                    NewQuad.Scale(new Vector2(1f / 8, 1f / 8));

                    NewQuad.Center.RelPosFromPos();

                    MainObject.AddQuad(NewQuad);

                    UpdateQuadList = true;
                }

                // Delete quad, box
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Delete) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Delete))
                {
                    List<ObjectBox> NewBoxList = new List<ObjectBox>();
                    NewBoxList.AddRange(MainObject.BoxList);
                    foreach (ObjectVector point in SelectedPoints)
                        foreach (ObjectBox box in MainObject.BoxList)
                            if (box.GetObjectVectors().Contains(point))
                            {
                                UpdateQuadList = true;
                                NewBoxList.Remove(box);
                            }
                    MainObject.BoxList = NewBoxList;

                    foreach (BaseQuad quad in SelectedQuads)
                    {
                        UpdateQuadList = true;
                        quad.ParentObject.RemoveQuad(quad);
                    }

                    SelectedQuads.Clear();
                    SelectedBoxes.Clear();
                    UpdateQuadSelection = true;
                    SelectedPoints.Clear();
                }



                if (SelectedPoints.Count == 0 && SelectedQuads.Count == 0)
                {
                }
                else
                {
                    // Save/Recover state
                    int NumPressed = -1;
                    for (int i = 0; i <= 9; i++)
                        if (KeybState.IsKeyDownCustom((Microsoft.Xna.Framework.Input.Keys)((int)Microsoft.Xna.Framework.Input.Keys.D0 + i)) && !PrevKeyboardState.IsKeyDownCustom((Microsoft.Xna.Framework.Input.Keys)((int)Microsoft.Xna.Framework.Input.Keys.D0 + i)))
                            NumPressed = i;

                    if (NumPressed >= 0)
                    {
                        foreach (BaseQuad quad in SelectedQuads)
                            if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                                quad.SaveState(NumPressed);
                            else
                                quad.RecoverState(NumPressed);
                        
                        foreach (ObjectBox box in SelectedBoxes)
                            if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                                box.SaveState(NumPressed);
                            else
                                box.RecoverState(NumPressed);
                    }
                }




                // Name the selected quad
                if (!DialogUp)
                if (SelectedQuads.Count > 0 || SelectedBoxes.Count > 0)
                    if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.N) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.N))
                    {
                        DialogUp = true;

                        Dlg = new SetNameDialog();
                        Dlg.Name = "Name the selected quads...";

                        // Set the current name
                        if (SelectedQuads.Count > 0)
                            Dlg.textBox1.Text = SelectedQuads[0].Name;
                        else
                            Dlg.textBox1.Text = SelectedBoxes[0].Name;

                        if (Dlg.ShowDialog() != DialogResult.Cancel)
                        {
                            foreach (BaseQuad quad in SelectedQuads)
                            {
                                quad.Name = Dlg.textBox1.Text;
                            }
                            foreach (ObjectBox box in SelectedBoxes)
                            {
                                box.Name = Dlg.textBox1.Text;
                            }
                        }
                        Dlg = null;
                        DialogUp = false;

                        UpdateQuadList = true;
                    }
                // Change the draw order of the quad
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.M) && !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.M))
                {
                    string[] names = Enum.GetNames(typeof(ObjectDrawOrder));
                    foreach (BaseQuad quad in SelectedQuads)
                    {
                        quad.MyDrawOrder++;
                        if ((int)quad.MyDrawOrder >= names.Length)
                            quad.MyDrawOrder = 0;
                    }
                }

                // Left mouse button just released
                if (CurrentMouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && LeftMouseButtonState == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    // Check to see if selection box is large enough to process
                    if (Dragging && (MousePos - DragStart).Length() > .1f)
                    {
                        if (!KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.RightControl))
                        {
                            SelectedQuads.Clear();
                            SelectedBoxes.Clear();
                            UpdateQuadSelection = true;
                            SelectedPoints.Clear();
                        }

                        int N = 50;
                        Vector2 shift = (MousePos - DragStart) / N;

                        foreach (ObjectClass obj in ObjectList)
                            foreach (BaseQuad quad in obj.QuadList)
                            {
                                if (!quad.Show || !QuadNode.Checked) continue;

                                bool flag = SelectedQuads.Contains(quad);
                                for (int j = 0; j <= N && !flag; j++)
                                    for (int k = 0; k <= N && !flag; k++)
                                    {
                                        if (quad.HitTest(DragStart + new Vector2(j * shift.X, k * shift.Y)))
                                        {
                                            SelectedQuads.Add(quad);
                                            UpdateQuadSelection = true;
                                            flag = true;
                                        }
                                    }
                            }
                    }
                }

                // End dragging
                if (CurrentMouse.LeftButton != Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    Dragging = false;

                // Right mouse button just pressed
                if (CurrentMouse.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && RightMouseButtonState == Microsoft.Xna.Framework.Input.ButtonState.Released)
                {
                    RightClickPos = MousePos;
                }

                // Save the rotation point if R is being pressed
                if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.R))
                    RotationPoint = MousePos;
                if (LeftMouseButtonState == Microsoft.Xna.Framework.Input.ButtonState.Pressed && SelectedQuads.Count > 0)
                    RotationPoint = MousePos;

                // Left mouse button just pressed
                float ClickDist = .03f;
                if (!ShowTextureSelection)
                    TextureSelectHiddenCount++;
                else
                    TextureSelectHiddenCount = 0;
                if (MouseOnScreen(CurrentMouse) && !CameraMove && !ShowTextureSelection && TextureSelectHiddenCount > 2)
                if (CurrentMouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && LeftMouseButtonState == Microsoft.Xna.Framework.Input.ButtonState.Released)
                {
                    // Save the current location
                    DragStart = MousePos;

                    // Save the current time
                    for (int i = LeftMouseClickTimeStamp.Length - 1; i > 0; i--)
                        LeftMouseClickTimeStamp[i] = LeftMouseClickTimeStamp[i - 1];
                    LeftMouseClickTimeStamp[0] = (float)gameTime.TotalGameTime.TotalSeconds;
                    bool TripleClick = (LeftMouseClickTimeStamp[0] - LeftMouseClickTimeStamp[2] < .5f);
                    bool DoubleClick = !TripleClick && (LeftMouseClickTimeStamp[0] - LeftMouseClickTimeStamp[1] < .25f);

                    bool flag = false;
                    {
                        if (!KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.RightControl))
                        {
                            SelectedBoxes.Clear();
                            SelectedPoints.Clear();
                        }

                        // Check for points clicked on
                        foreach (BaseQuad quad in SelectedQuads)
                            foreach (ObjectVector point in quad.GetObjectVectors())
                                if ((point.Pos - MousePos).Length() < ClickDist && !SelectedPoints.Contains(point))
                                {
                                    flag = true;
                                    if (TripleClick)
                                    {
                                    }
                                    else
                                        if (DoubleClick)
                                        {
                                            Quad _quad = quad as Quad;
                                            if (null != _quad)
                                                for (int i = 0; i < 4; i++)
                                                {
                                                    SelectedPoints.Add(_quad.Corner[i]);
                                                    _quad.Corner[i].Click();
                                                }
                                        }
                                        else
                                        {
                                            SelectedPoints.Add(point);
                                            point.Click();
                                        }
                                }
                        foreach (ObjectBox box in MainObject.BoxList)
                            if (box.Show && BoxNode.Checked)
                            foreach (ObjectVector point in box.GetObjectVectors())
                                if ((point.Pos - MousePos).Length() < ClickDist && !SelectedPoints.Contains(point))
                                {
                                    flag = true;
                                    // Add both points if double clicked, one if single clicked
                                    if (DoubleClick)
                                    {
                                        SelectedPoints.Add(box.TR);
                                        SelectedPoints.Add(box.BL);
                                    }
                                    else
                                        SelectedPoints.Add(point);
                                    point.Click();
                                    if (!SelectedBoxes.Contains(box))
                                    {
                                        UpdateQuadList = true;
                                        SelectedBoxes.Add(box);
                                    }
                                }

                    }

                    // Check for quads clicked on
                    float QuadSize = 100000f;
                    BaseQuad SmallestQuad = null;
                    if (MouseOnScreen(CurrentMouse) && !flag)
                    {
                        bool ClickedOnAQuad = false;
                        foreach (ObjectClass obj in ObjectList)
                            foreach (BaseQuad quad in obj.QuadList)
                            {
                                if (!quad.Show || !QuadNode.Checked) continue;

                                float Size = (quad.TR() - quad.BL()).Length();
                                if (quad.HitTest(MousePos) && (SelectedQuads.Contains(quad) || Size < QuadSize && !SelectedQuads.Contains(SmallestQuad)))
                                {
                                    ClickedOnAQuad = true;
                                    QuadSize = Size;
                                    SmallestQuad = quad;
                                }
                            }


                        if (ClickedOnAQuad)
                        {
                            if (!KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.RightControl))
                            {
                                SelectedQuads.Clear();
                                SelectedBoxes.Clear();
                                UpdateQuadSelection = true;
                            }
                            SelectedPoints.Clear();

                            if (!SelectedQuads.Contains(SmallestQuad))
                            {
                                SelectedQuads.Add(SmallestQuad);
                                UpdateQuadSelection = true;
                            }
                        }

                        /*
                        bool ClickedOnAQuad = false;
                        foreach (ObjectClass obj in ObjectList)
                            foreach (BaseQuad quad in obj.QuadList)
                            {
                                if (!quad.Show || !QuadNode.Checked) continue;

                                float Size = (quad.TR() - quad.BL()).Length();
                                if (quad.HitTest(MousePos) && Size < QuadSize)
                                {                                    
                                    ClickedOnAQuad = true;
                                    QuadSize = Size;

                                    if (!flag)
                                    {
                                        if (!KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.RightControl))
                                        {
                                            SelectedQuads.Clear();
                                            SelectedBoxes.Clear();
                                            UpdateQuadSelection = true;
                                        }
                                        SelectedPoints.Clear();

                                        if (!SelectedQuads.Contains(quad))
                                        {
                                            flag = true;
                                            SelectedQuads.Add(quad);
                                            UpdateQuadSelection = true;
                                        }
                                    }
                                }
                            }
                        */



                        if (!ClickedOnAQuad && !flag && !KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.RightControl))
                        {
                            SelectedQuads.Clear();
                            SelectedBoxes.Clear();
                            UpdateQuadSelection = true;
                        }
                        else
                            RotationPoint = MousePos;

                        // Start dragging
                        if (SelectedPoints.Count == 0 && SelectedQuads.Count == 0)
                            Dragging = true;
                    }
                }
            }




            PrevKeyboardState = KeybState;
            LeftMouseButtonState = CurrentMouse.LeftButton;
            RightMouseButtonState = CurrentMouse.RightButton;

            Vector2 MouseDelta = MousePos - PrevMousePos;
            Vector2 MouseDelta_ScreenCoordinates;
            MouseDelta_ScreenCoordinates.X = CurrentMouse.X - PrevMousePos_ScreenCoordinates.X;
            MouseDelta_ScreenCoordinates.Y = CurrentMouse.Y - PrevMousePos_ScreenCoordinates.Y;

            PrevMousePos = MousePos;
            PrevMousePos_ScreenCoordinates.X = CurrentMouse.X;
            PrevMousePos_ScreenCoordinates.Y = CurrentMouse.Y;

            if (!ShowTextureSelection && MouseOnScreen(CurrentMouse))
            {
                if (!CameraMove)
                {
                    // Rotate selected quads
                    if (CurrentMouse.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    {
                        __ModPoint mod = RotatePoint;
                        if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            mod = ScalePoint;

                        if (SelectedPoints.Count == 0)
                            foreach (BaseQuad _quad in SelectedQuads)
                            {
                                Quad quad = _quad as Quad;
                                if (quad != null)
                                {
                                    quad.Center.Pos = mod(quad.Center.Pos, RotationPoint, 4.5f * MouseDelta.Y);
                                    quad.Center.RelPosFromPos();
                                    quad.xAxis.Pos = mod(quad.xAxis.Pos, RotationPoint, 4.5f * MouseDelta.Y);
                                    quad.xAxis.RelPosFromPos();
                                    quad.yAxis.Pos = mod(quad.yAxis.Pos, RotationPoint, 4.5f * MouseDelta.Y);
                                    quad.yAxis.RelPosFromPos();
                                }
                            }


                        //if (SelectedQuads.Count == 0)
                        foreach (ObjectVector point in SelectedPoints)
                        {
                            if (point.ModifiedEventCallback == point.DefaultCallback)
                            {
                                point.Pos = mod(point.Pos, RotationPoint, 4.5f * MouseDelta.Y);
                                point.RelPosFromPos();
                            }
                        }
                    }

                    // Move selected quads
                    bool move = LeftMouseButtonState == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                    Vector2 delta = MouseDelta;
                    if (Tools.CntrlDown()) delta *= .2f;
                    if (Delay <= 0)
                    {
                        float shift = .0002f;
                        if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Left))
                        {
                            move = true;
                            delta = new Vector2(-shift, 0);
                        }
                        if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Right))
                        {
                            move = true;
                            delta = new Vector2(shift, 0);
                        }
                        if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Up))
                        {
                            move = true;
                            delta = new Vector2(0, shift);
                        }
                        if (KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Down))
                        {
                            move = true;
                            delta = new Vector2(0, -shift);
                        }
                    }
                    else
                        Delay--;

                    if (move && SelectedPoints.Count == 0)
                    {
                        Delay = 2000;
                        foreach (BaseQuad _quad in SelectedQuads)
                        {
                            Quad quad = _quad as Quad;
                            if (quad != null)
                                quad.Center.Move(quad.Center.Pos + delta);
                        }
                    }
                    else
                    {
                        if (!KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Up) &&
                            !KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Down) &&
                            !KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Left) &&
                            !KeybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Right))
                        Delay = 0;
                    }

                    if (LeftMouseButtonState == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                        foreach (ObjectVector point in SelectedPoints)
                        {
                            if (SelectedPoints.Count == 1 || point.ModifiedEventCallback == point.DefaultCallback)
                                point.Move(point.Pos + MouseDelta);
                        }
                }
                else
                {
                    // Camera move
                    if (RightMouseButtonState == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    {
                        float scale = MouseDelta_ScreenCoordinates.Y / ScreenWidth;
                        cameraPos.Z += scale;
                        cameraPos.W += scale;
                    }
                    if (LeftMouseButtonState == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    {
                        cameraPos.X -= MouseDelta.X;
                        cameraPos.Y -= MouseDelta.Y;
                        PrevMousePos -= MouseDelta;
                        //MousePos += MouseDelta;
                    }
                }
            }


            // Update toolbox
            UpdateToolBox();

            float t = (float)gameTime.TotalGameTime.Ticks / 50000000f;

            //SetUpVertices(2 * 3.14159f * (float)gameTime.TotalGameTime.Ticks / 50000000f);

            base.Update(gameTime);
        }

        void InitializeTree()
        {
            // Selection event
            ToolBox.ItemTree.AfterSelect += QuadTree_AfterSelect;
            ToolBox.ItemTree.AfterCheck += QuadTree_AfterCheck;
            ToolBox.ItemTree.DragOver += new DragEventHandler(ItemTree_DragOver);
            ToolBox.ItemTree.AfterExpand += new TreeViewEventHandler(ItemTree_AfterExpand);
            ToolBox.ItemTree.AfterCollapse += new TreeViewEventHandler(ItemTree_AfterCollapse);
            ToolBox.ItemTree.ItemDrag += new ItemDragEventHandler(ItemTree_ItemDrag);
            ToolBox.ItemTree.DragDrop += new DragEventHandler(ItemTree_DragDrop);

            // Create the root quad node
            QuadNode = new TreeNode();
            QuadNode.Name = QuadNode.Text = "Quads";
            QuadNode.Checked = true;
            ToolBox.ItemTree.Nodes.Add(QuadNode);

            // Create the root box node
            BoxNode = new TreeNode();
            BoxNode.Checked = true;
            BoxNode.Name = BoxNode.Text = "Boxes";
            ToolBox.ItemTree.Nodes.Add(BoxNode);
        }

        void ItemTree_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (SkipExpandCollapseEvent) return;

            if (e.Node.Name[0] == 'q')
                GetNodeQuad(e.Node, MainObject).Expanded = false;            
        }

        void ItemTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (SkipExpandCollapseEvent) return;

            if (e.Node.Name[0] == 'q')
                GetNodeQuad(e.Node, MainObject).Expanded = true;
        }

        void ItemTree_DragOver(object sender, DragEventArgs e)
        {
            TreeNode Node = null;
            bool AsChild = GetReceivingNode((TreeView)sender, new Vector2(e.X, e.Y), ref Node);
            if (Node == GhostNode && GhostNode != null) return;

            if (Node != CurNodeDestination)
            {
                if (GhostNode != null)
                    GhostNode.Remove();

                //if (CurNodeDestination != null)
                  //  CurNodeDestination.BackColor = System.Drawing.Color.White;
                
                if (ValidDragDestination(Node, true))
                    CurNodeDestination = Node;
                else
                    CurNodeDestination = null;

                if (CurNodeDestination != null)
                {
                    //CurNodeDestination.BackColor = System.Drawing.Color.Green;

                    GhostNode = new TreeNode();
                    GhostNode.Name = GhostNode.Text = "         ";
                    GhostNode.BackColor = System.Drawing.Color.LimeGreen;

                    //if (AsChild)
                        CurNodeDestination.Nodes.Insert(0, GhostNode);
                    /*else
                    {
                        CurNodeDestination.Parent.Nodes.Insert(CurNodeDestination.Index, GhostNode);
                    }*/

                    SkipExpandCollapseEvent = true;
                    CurNodeDestination.Expand();
                    SkipExpandCollapseEvent = false;
                }
            }
        }

        
        bool ValidDragDestination(TreeNode node, bool AsChild)
        {
            // Quad root is valid
            if (node == QuadNode) return true;

            if (node == null) return false;

            // Check node is a quad
            if (node.Name[0] == 'q')
            {
                BaseQuad DestinationQuad = GetNodeQuad(node, MainObject);

                // Make sure the node isn't one of the selected quads
                if (SelectedQuads.Contains(DestinationQuad))
                    return false;

                // Make sure node isn't a child
                foreach (BaseQuad quad in SelectedQuads)
                    if (quad is Quad && quad.ParentQuad != DestinationQuad &&
                        ((Quad)quad).GetAllChildren().Contains(DestinationQuad))
                        return false;
            }
            else
                return false;

            return true;
        }

        void ItemTree_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode DraggedNode;

            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                //TreeNode CurNodeDestination = null;
                //bool AsChild = GetReceivingNode((TreeView)sender, new Vector2(e.X, e.Y), ref CurNodeDestination);
                DraggedNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");

                // Check to make sure node can be added here
                if (CurNodeDestination != null && ValidDragDestination(CurNodeDestination, true))
                {
                    SkipLoop = true;

                    // If quad isn't from MainObject, add it
                    if (DraggingSource != MainObject)
                        MainObject.AddQuad(GetNodeQuad(DraggedNode, DraggingSource) as Quad, true);

                    // Make sure no one is set to be a child
                    foreach (BaseQuad quad in MainObject.QuadList)
                        quad.SetToBeChild = false;

                    // Update object
                    foreach (BaseQuad quad in SelectedQuads)
                    {
                        if (CurNodeDestination == QuadNode)
                            quad.ReleasePoint.Click();
                        else
                            quad.ChildPoint.Click();
                    }
                    if (CurNodeDestination != QuadNode)
                        GetNodeQuad(CurNodeDestination, MainObject).ParentPoint.Click();
                    UpdateQuadList = true;

                    SkipLoop = false;
                }

                if (GhostNode != null)
                    GhostNode.Remove();
            }
        }

        void ClearSelected(TreeNode RootNode)
        {
            SetNodeSelection(RootNode, false);
            foreach (TreeNode node in RootNode.Nodes)
                ClearSelected(node);
        }
        void ItemTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DraggingSource = MainObject;

            TreeNode Node = (TreeNode)e.Item;
            
            // Start dragging if the node is a quad
            if (Node.Name[0] == 'q')
            {
                BaseQuad quad = GetNodeQuad(Node, MainObject);

                // Unselect other nodes if not holding control
                if (!PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.RightControl))
                {
                    SelectedQuads.Clear();
                    ClearSelected(QuadNode);
                }

                // Select the quad
                if (!SelectedQuads.Contains(quad))
                {
                    SelectNode(Node);
                }

                ToolBox.ItemTree.DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        bool GetReceivingNode(TreeView Tree, Vector2 pos, ref TreeNode ReceivingNode)
        {
            System.Drawing.Point pt1 = Tree.PointToClient(new System.Drawing.Point((int)pos.X, (int)pos.Y));
            System.Drawing.Point pt2 = Tree.PointToClient(new System.Drawing.Point((int)pos.X, (int)pos.Y + 4));

            TreeNode node1 = Tree.GetNodeAt(pt1);
            TreeNode node2 = Tree.GetNodeAt(pt2);

            if (node1 == node2)
            {
                ReceivingNode = node1;
                return true;
            }
            else
            {
                ReceivingNode = node1;
                return false;
            }
        }

        public static TreeNode GetQuadNode(BaseQuad Quad, TreeNode RootNode, ObjectClass Obj)
        {
            foreach (TreeNode node in RootNode.Nodes)
            {
                if (GetNodeQuad(node, Obj) == Quad)
                    return node;

                TreeNode ReturnedNode = GetQuadNode(Quad, node, Obj);
                if (ReturnedNode != null)
                    return ReturnedNode;
            }

            return null;
        }
        public static BaseQuad GetNodeQuad(TreeNode Node, ObjectClass obj)
        {
            int Index = int.Parse(Node.Name.Substring(1));
            return obj.QuadList[Index];
        }
        ObjectBox GetNodeBox(TreeNode Node)
        {
            int Index = int.Parse(Node.Name.Substring(1));
            return MainObject.BoxList[Index];
        }

        void SetNodeSelection(TreeNode Node, bool Selected)
        {
            if (Selected)
                Node.BackColor = System.Drawing.Color.HotPink;
            else
                Node.BackColor = System.Drawing.Color.White;
        }

        void QuadTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Unselect other nodes if not holding control
            if (!PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.RightControl) &&
                !PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.LeftShift))
            {
                SelectedQuads.Clear();
                SelectedBoxes.Clear();
                SelectedPoints.Clear();
                ClearSelected(QuadNode);
                ClearSelected(BoxNode);
            }

            SelectNode(e.Node);

            // Shift select nodes
            if (e.Node.Parent != null && PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.LeftShift))
            {
                int Index = e.Node.Index;
                TreeNode ParentNode = e.Node.Parent;

                // Starting from this index, move backwards until another selected node is found
                int UpperIndex = Index - 1;
                bool FoundUpper = false;
                while (UpperIndex >= 0)
                {
                    if (e.Node.Parent.Nodes[UpperIndex].BackColor == System.Drawing.Color.HotPink)
                    {
                        FoundUpper = true;
                        break;
                    }
                                        
                    UpperIndex--;
                }

                // Starting from this index, move forward until another selected node is found
                int LowerIndex = Index + 1;
                bool FoundLower = false;
                while (LowerIndex < ParentNode.Nodes.Count)
                {
                    if (e.Node.Parent.Nodes[LowerIndex].BackColor == System.Drawing.Color.HotPink)
                    {
                        FoundLower = true;
                        break;
                    }
                    
                    LowerIndex++;
                }

                if (FoundLower && FoundUpper)
                {
                    // Select all in between
                    for (int i = UpperIndex + 1; i < LowerIndex; i++)
                        if (i != Index)
                            SelectNode(e.Node.Parent.Nodes[i]);
                }
                else
                {
                    if (FoundUpper)
                        for (int i = UpperIndex + 1; i < Index; i++)
                            SelectNode(e.Node.Parent.Nodes[i]);
                    else if (FoundLower)
                        for (int i = Index + 1; i < LowerIndex; i++)
                            SelectNode(e.Node.Parent.Nodes[i]);
                }
            }
        }
        void SelectNode(TreeNode node)
        {
            if (node.Name[0] == 'q')
            {
                BaseQuad quad = GetNodeQuad(node, MainObject);
                bool Selected = SelectedQuads.Contains(quad);
                if (Selected)
                    SelectedQuads.Remove(quad);
                else
                    SelectedQuads.Add(quad);
                SetNodeSelection(node, !Selected);

                //ToolBox.ItemTree.SelectedNode = null;
            }
            if (node.Name[0] == 'b')
            {
                ObjectBox Box = GetNodeBox(node);
                bool Selected = SelectedBoxes.Contains(Box);
                if (Selected)
                {
                    SelectedBoxes.Remove(Box);
                    SelectedPoints.Remove(Box.BL);
                    SelectedPoints.Remove(Box.TR);
                }
                else
                {
                    SelectedBoxes.Add(Box);
                    SelectedPoints.Add(Box.BL);
                    SelectedPoints.Add(Box.TR);
                }
                SetNodeSelection(node, !Selected);

                //ToolBox.ItemTree.SelectedNode = null;
            }
        }

        void QuadTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown) return;

            if (e.Node.Name[0] == 'q')
            {
                BaseQuad quad = GetNodeQuad(e.Node, MainObject);
                e.Node.Checked = quad.Show = !quad.Show;
            }
            if (e.Node.Name[0] == 'b')
            {
                ObjectBox Box = GetNodeBox(e.Node);
                e.Node.Checked = Box.Show = !Box.Show;
            }
        }

        public static void PopulateQuadNode(Quad RootQuad, TreeNode RootNode, ObjectClass Obj)
        {
            foreach (BaseQuad Child in RootQuad.Children)
            {
                TreeNode ChildNode = new TreeNode();
                ChildNode.Name = 'q' + Obj.QuadList.IndexOf(Child).ToString();
                ChildNode.Text = Child.Name;
                RootNode.Nodes.Add(ChildNode);

                if (Child is Quad)
                    PopulateQuadNode((Quad)Child, ChildNode, Obj);
            }
        }

        void EnsureExpanded(TreeNode node)
        {
            if (node.Parent != null)
            {
                if (!node.Parent.IsExpanded)
                    node.Parent.Expand();
                EnsureExpanded(node.Parent);
            }
        }

        void RefreshQuadSelection(TreeNode RootNode)
        {
            foreach (TreeNode node in RootNode.Nodes)
            {
                BaseQuad quad = GetNodeQuad(node, MainObject);
                SetNodeSelection(node, SelectedQuads.Contains(quad));
                node.Checked = quad.Show;

                // Make sure selected quads' parents are expanded
                if (SelectedQuads.Contains(quad))
                    EnsureExpanded(node);

                if (quad.Expanded)
                {
                    SkipExpandCollapseEvent = true;
                    node.Expand();
                    SkipExpandCollapseEvent = false;
                }
                RefreshQuadSelection(node);
            }            
        }

        void PopulateBoxNode(TreeNode RootNode)
        {
            foreach (ObjectBox Box in MainObject.BoxList)
            {
                TreeNode Node = new TreeNode();
                Node.Name = 'b' + MainObject.BoxList.IndexOf(Box).ToString();
                Node.Text = Box.Name;
                RootNode.Nodes.Add(Node);
            }
        }

        void RefreshBoxSelection(TreeNode RootNode)
        {
            foreach (TreeNode node in RootNode.Nodes)
            {
                ObjectBox Box = GetNodeBox(node);
                SetNodeSelection(node, SelectedBoxes.Contains(Box));
                node.Checked = Box.Show;
                RefreshBoxSelection(node);
            }
        }

        void UpdateToolBox()
        {
            if (UpdateQuadList)
            {
                QuadNode.Nodes.Clear();
                BoxNode.Nodes.Clear();
                
                // Populate the quad node
                PopulateQuadNode(MainObject.ParentQuad, QuadNode, MainObject);

                // Populate the box node
                PopulateBoxNode(BoxNode);

                UpdateQuadList = false;
                UpdateQuadSelection = true;
            }

            if (UpdateQuadSelection)
            {
                // Refresh the selected items
                RefreshQuadSelection(QuadNode);
                RefreshBoxSelection(BoxNode);

                UpdateQuadSelection = false;
            }
        }


        protected override void Draw(GameTime gameTime)
        {
            
            fps = .7f * fps + .3f * (1000f / Math.Max(1, gameTime.ElapsedGameTime.Milliseconds));
            /*device.SetRenderTarget(renderTarget);

            device.Clear(
                            ClearOptions.Target,
                            Color.CornflowerBlue,
                            0, 0);

            //device.Clear(Color.Black);
            DrawScene();

            device.SetRenderTarget(null);
            OverSampledRender = renderTarget;*/

            //device.Clear(Color.Black);
            DrawScene();
            //DrawFinal();

            base.Draw(gameTime);
        }




        private void DrawScene()
        {
            // set the viewport to the whole screen
            GraphicsDevice.Viewport = new Viewport
            {
                X = 0,
                Y = 0,
                Width = GraphicsDevice.PresentationParameters.BackBufferWidth,
                Height = GraphicsDevice.PresentationParameters.BackBufferHeight,
                MinDepth = 0,
                MaxDepth = 1
            };
            GraphicsDevice.Clear(Color.Black);

            Tools.QDrawer.SetAddressMode(true, true);
            Tools.Device.RasterizerState = RasterizerState.CullNone;
            Tools.Device.BlendState = BlendState.AlphaBlend;
            Tools.Device.DepthStencilState = DepthStencilState.DepthRead;

            Tools.QDrawer.SetInitialState();
            Tools.Device.RasterizerState = RasterizerState.CullNone;
            Tools.Device.BlendState = BlendState.AlphaBlend;
            Tools.Device.DepthStencilState = DepthStencilState.DepthRead;

            Tools.EffectWad.SetCameraPosition(cameraPos);
            Tools.SetDefaultEffectParams(AspectRatio);

            device.SetRenderTarget(Tools.DestinationRenderTarget);
                device.Clear(Color.Gray);
            
            if (ShowTextureSelection)
            {
                Quad quad = new Quad();
                quad.SetEffect("Basic", EffectWad);

                int Num = TextureWad.TextureList.Count;
                int N = 6;
                Vector2 shift = new Vector2(1.85f, 1.85f) / N;

                TextureSelectionPage = TextureSelectionPage % ((int)(Num / (N * N)) + 1);
                int IndexOffset = N * N * TextureSelectionPage;
                for (int i = 0; i < N * N; i++)
                {
                    int j = i % N;
                    int k = i / N;

                    if (i + IndexOffset >= Num) break;
                    if (j >= N || k >= N) break;

                    Vector2 loc = new Vector2(-1.85f / 2 + shift.X * (j + .5f), -1.85f / 2 + shift.Y * (k + .5f));

                    quad.xAxis.RelPos.X = shift.X / 2;
                    quad.yAxis.RelPos.Y = shift.Y / 2;
                    quad.Center.Move(loc);
                    quad.Update();

                    if (Math.Abs(loc.X - PrevMousePos.X) < shift.X / 2 &&
                        Math.Abs(loc.Y - PrevMousePos.Y) < shift.Y / 2 ||
                        SelectedTextureIndex == i + IndexOffset)
                    {
                        if (LeftMouseButtonState == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                        {
                            EndTetureDisplay();
                            if (SelectedQuads.Count == 0)
                                MainObject.MySkinTexture = TextureWad.TextureList[i + IndexOffset];
                            else
                                foreach (BaseQuad _quad in SelectedQuads)
                                    _quad.MyTexture = TextureWad.TextureList[i + IndexOffset];
                            SelectedTextureIndex = i + IndexOffset;
                        }

                        quad.SetEffect("NoTexture", EffectWad);
                        quad.MyTexture = TextureWad.TextureList[0];
                        if (SelectedTextureIndex == i + IndexOffset)
                            quad.SetColor(Color.Red);
                        else
                            quad.SetColor(Color.Blue);
                        quad.Draw(QDrawer);
                        QDrawer.Flush();
                        quad.SetEffect("Basic", EffectWad);
                        quad.SetColor(Color.White);
                    }

                    quad.MyTexture = TextureWad.TextureList[i + IndexOffset];
                    quad.Draw(QDrawer);
                    QDrawer.Flush();
                }
            }
            else
                if (!Render)
                {
                    {
                        QDrawer.DrawLine(new Vector2(-2, 0), new Vector2(2, 0), Color.Bisque, .01f / cameraPos.W);
                        QDrawer.DrawLine(new Vector2(0, -2), new Vector2(0, 2), Color.Bisque, .01f / cameraPos.W);
                    }

                    foreach (ObjectClass obj in ObjectList)
                        obj.Draw(true);

                    MainObject.Draw(true);

                        foreach (BaseQuad quad in SelectedQuads)
                        {
                            quad.ColoredDraw(QDrawer, new Color(100, 100, 255, 255));
                            if (quad is Quad)
                                ((Quad)quad).DrawChildren(QDrawer);
                        }
                }

            MainObject.Update(null);

            if (Render)
                MainObject.ContainedDraw();

            {
                if (QuadNode.Checked)
                    foreach (BaseQuad quad in SelectedQuads)
                        quad.DrawExtra(QDrawer, Additional, 1 / cameraPos.W);

                if (BoxNode.Checked)
                    foreach (ObjectBox box in MainObject.BoxList)
                        if (box == MainObject.BoxList[0])
                            box.DrawExtra(QDrawer, Color.DarkOliveGreen);
                        else
                            box.DrawExtra(QDrawer, Color.DarkMagenta);

                foreach (ObjectVector point in SelectedPoints)
                    QDrawer.DrawSquareDot(point.Pos, Color.AliceBlue, .02f / cameraPos.W);
                QDrawer.Flush();

            }

            MainObject.ParentQuad.Update();
            MainObject.Update(null);
            MainObject.Update(null);

            {
                // Draw the selection box
                if (Dragging)
                {
                    QDrawer.DrawBox(PrevMousePos, DragStart, Color.AliceBlue, .025f / cameraPos.W);
                    QDrawer.Flush();
                }
                if (SelectedQuads.Count > 0 || SelectedPoints.Count > 0)
                    QDrawer.DrawSquareDot(RotationPoint, Color.Red, .02f / cameraPos.W);

                // Draw the mouse cursor
                QDrawer.DrawLine(PrevMousePos, PrevMousePos + new Vector2(.035f, -.015f) / cameraPos.W, Color.Tomato, .0085f / cameraPos.W);
                QDrawer.DrawLine(PrevMousePos, PrevMousePos + new Vector2(.015f, -.035f) / cameraPos.W, Color.Tomato, .0085f / cameraPos.W);
                QDrawer.Flush();

                MouseState current_mouse = Mouse.GetState();
                Vector2 MouseX = MouseCoordinate(current_mouse);

                spriteBatch.Begin();
                spriteBatch.DrawString(Font1, fps.ToString(), new Vector2(ScreenWidth - 75, 5), Color.Azure);
                spriteBatch.DrawString(Font1, CurrentFileName.ToString(), new Vector2(130, 3), Color.Blue);
                spriteBatch.DrawString(Font1, frame.ToString() + " / " + MainObject.AnimLength[anim], new Vector2(5, 5), Color.Azure);
                spriteBatch.DrawString(Font1, anim.ToString() + ": " + MainObject.AnimName[anim], new Vector2(5, 25), Color.DarkOrange);
                if (RecordMode != ChangeMode.SingleFrame)
                    spriteBatch.DrawString(Font1, RecordMode.ToString(), new Vector2(85, 5), Color.Red);
                Vector2 pos = new Vector2(12, 50);
                foreach (BaseQuad quad in SelectedQuads)
                {
                    spriteBatch.DrawString(Font1, quad.Name + " " + quad.MyDrawOrder, pos, Color.Blue);
                    pos.Y += 20;
                }
                foreach (ObjectBox box in SelectedBoxes)
                {
                    spriteBatch.DrawString(Font1, box.Name, pos, Color.Red);
                    pos.Y += 20;
                }

                //spriteBatch.DrawString(Font1, MainObject.t.ToString() + ", " + MainObject.AnimQueue.Count, new Vector2(5, 125), Color.Azure);
                if (TestString != null)
                    spriteBatch.DrawString(Font1, TestString, new Vector2(5, 145), Color.Azure);
                //            spriteBatch.DrawString(Font1, MouseX.X.ToString(), new Vector2(300, 300), Color.Azure);
                spriteBatch.End();
            }
        }
    }
}

#endif