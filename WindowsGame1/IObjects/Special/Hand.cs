using System;
using System.IO;

#if WINDOWS
using Forms = System.Windows.Forms;
#endif

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Clouds;
using CloudberryKingdom.Blocks;

#if WINDOWS
using CloudberryKingdom;
#endif

namespace CloudberryKingdom.Bobs
{
    public enum HandState { Null, Open, Closed };
    public enum HoldingState { Null, Nothing, New, Old };
    public class Hand : ObjectBase
    {
        public AABox Box;
        Vector2 HandSize;

        public int MyPlayerIndex;

        public bool SelectLayerZones = true;
        public bool OnlyMovingDoodads = false;
        public bool OnlySmallStaticDoodads = false;

        public QuadClass MyQuad;

        HandState MyHandState;
        HoldingState MyHoldingState;
        ObjectBase HeldObject;
        Vector2 HeldObjOffset;

        int HeldCount = 0;
        float SelectOffset;

        BlockBase CopyBlock;
        ObjectBase CopyObj;

        public void SetState(HandState NewState)
        {
            if (NewState != MyHandState)
            {
                switch (NewState)
                {
                    case HandState.Open:
                        MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName("Hand_Open");
                        break;

                    case HandState.Closed:
                        MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName("Hand_Closed");
                        break;
                }

                MyHandState = NewState;
            }
        }

        public void SetState(HoldingState NewState)
        {
            if (NewState != MyHoldingState)
            {
                MyHoldingState = NewState;
            }
        }

        public override void MakeNew()
        {
            SetState(HandState.Open);
            SetState(HoldingState.Nothing);
            HeldObject = null;

            HandSize = new Vector2(70, 70);

            MyQuad.Base.e1.X = HandSize.X;
            MyQuad.Base.e2.Y = HandSize.Y;

            Core.DrawLayer = Level.LastInLevelDrawLayer - 1;
        }

        public Hand()
        {
            MyQuad = new QuadClass();

            Box = new AABox(Core.Data.Position, HandSize);

            MakeNew();
        }

#if WINDOWS
        void DialogUpStart()
        {
            Tools.DialogUp = true;
            SaveMousePos = Tools.MousePos;
        }

        void DialogUpEnd()
        {
            Tools.DialogUp = false;
            Tools.MousePos = SaveMousePos;
        }
#endif

        Vector2 SaveMousePos = Vector2.Zero;
        public void GetPlayerInput()
        {
            Vector2 Dir = 26f * ButtonCheck.State(ControllerButtons.LJ, MyPlayerIndex).Dir;

#if WINDOWS
            if (Tools.keybState.IsKeyDownCustom(Keys.D2) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.D2))
            {
                OnlyMovingDoodads = !OnlyMovingDoodads;
            }
            if (Tools.keybState.IsKeyDownCustom(Keys.D3) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.D3))
            {
                OnlySmallStaticDoodads = !OnlySmallStaticDoodads;
            }

            Dir += Tools.DeltaMouse;

            // Switch grab layerzone 
            if (Tools.keybState.IsKeyDownCustom(Keys.L) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.L))
                SelectLayerZones = !SelectLayerZones;

            // copy
            if (Tools.keybState.IsKeyDownCustom(Keys.C) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.C))
            {
                if (HeldObject != null)
                {
                    BlockBase block = HeldObject as BlockBase;
                    if (null != block)
                    {
                        CopyBlock = (BlockBase)HeldObject;
                        CopyObj = null;
                    }
                    else
                    {
                        ObjectBase Obj = HeldObject as ObjectBase;
                        if (null != Obj)
                        {
                            CopyObj = (ObjectBase)HeldObject;
                            CopyBlock = null;
                        }
                    }
                }
            }

            // paste
            if (Tools.keybState.IsKeyDownCustom(Keys.V) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.V))
            {
                if (CopyBlock != null)
                {
                    BlockBase block = (BlockBase)Core.Recycle.GetObject(CopyBlock.Core.MyType, false);
                    block.Clone(CopyBlock);
                    Core.MyLevel.AddBlock(block);
                    block.Move(Core.Data.Position - block.Core.Data.Position);
                    HeldObject = block;
                }
                else
                {
                    if (CopyObj != null)
                    {
                        ObjectBase Obj = (ObjectBase)Core.Recycle.GetObject(CopyObj.Core.MyType, false);
                        Obj.Clone(CopyObj);
                        Core.MyLevel.AddObject(Obj);
                        Obj.Move(Core.Data.Position - Obj.Core.Data.Position);
                        HeldObject = Obj;
                    }
                }
            }

            // Save level
            if (ButtonCheck.State(Keys.S).Pressed)
            {
                String FileName = Tools.CurLevel.SourceFile;

                if (!ButtonCheck.State(Keys.LeftControl).Down &&
                    FileName.Length > 0)
                {
                    // Verify save
                    VerifySaveDialog verify = new VerifySaveDialog();
                    DialogUpStart();
                    verify.FileName.Text = FileName;

                    // Check the user didn't cancel
                    if (verify.ShowDialog() != Forms.DialogResult.Cancel)
                    {
                        // Save the level
                        Tools.CurLevel.Save(FileName, false);
                    }
                    verify.Close();

                    DialogUpEnd();
                }
                else
                {
                    Forms.OpenFileDialog ofd = new Forms.OpenFileDialog();
                    DialogUpStart();
                    ofd.Title = "Save as...";

                    ofd.InitialDirectory = Level.DefaultLevelDirectory();
                    ofd.Filter = "Level file (*.lvl)|*.lvl";
                    ofd.CheckFileExists = false;

                    // Check the user didn't cancel
                    if (ofd.ShowDialog() != Forms.DialogResult.Cancel)
                    {
                        // Save the level
                        Tools.CurLevel.Save(ofd.FileName, false);
                    }

                    DialogUpEnd();
                }
            }

            // load level
            if (Tools.keybState.IsKeyDownCustom(Keys.G) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.G))
            {
                Tools.Dlg = new BlockDialog();
                if (Tools.Dlg.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
                {
                    Level level = new Level();
                    level.Load(Tools.Dlg.textBox1.Text);

                    Tools.CurLevel.PurgeEditables();
                    Tools.CurLevel.AbsorbLevel(level);
                }
                Tools.Dlg = null;
            }

            Doodad doodad = HeldObject as Doodad;

            // Set draw layer
            if (MyHandState == HandState.Open)
            {
                for (int i = 0; i < Level.NumDrawLayers; i++)
                    if (Tools.keybState.IsKeyDownCustom(Keys.NumPad0 + i) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.NumPad0 + i))                    
                        Tools.CurLevel.ShowDrawLayer[i] = !Tools.CurLevel.ShowDrawLayer[i];
            }
            else            
            {
                // Center
                if (Tools.keybState.IsKeyDownCustom(Keys.O) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.O))
                    if (HeldObject != null)
                    {
                        HeldObject.Pos = Tools.CurCamera.Pos;
                        SetState(HandState.Open);
                        HeldObject = null;
                    }

                // Set selected object's draw layer
                if (HeldObject != null)
                {
                    for (int i = 0; i < Level.NumDrawLayers; i++)
                        if (Tools.keybState.IsKeyDownCustom(Keys.NumPad0 + i))
                        {
                            Core.MyLevel.RelayerObject(HeldObject, i, true);
                        }

                    if (Tools.keybState.IsKeyDownCustom(Keys.OemPlus) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.OemPlus))
                        //Core.MyLevel.RelayerObject(HeldObject, HeldObject.Core.DrawLayer, true);
                        Core.MyLevel.MoveUpOneSublayer(HeldObject);
                    if (Tools.keybState.IsKeyDownCustom(Keys.OemMinus) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.OemMinus))
                        //Core.MyLevel.RelayerObject(HeldObject, HeldObject.Core.DrawLayer, false);
                        Core.MyLevel.MoveDownOneSublayer(HeldObject);
                }

                // Cast to Door
                Door door = HeldObject as Door;

                // set code
                if (HeldObject != null)
                {
                    // Set Object
                    if (null != doodad &&
                        ButtonCheck.State(Keys.X).Pressed &&
                        ButtonCheck.State(Keys.LeftControl).Down)
                    {
                        Tools.DialogUp = true;

                        Forms.OpenFileDialog ofd = new Forms.OpenFileDialog();
                        ofd.Title = "Load object...";

                        ofd.InitialDirectory = Tools.DefaultObjectDirectory();
                        ofd.Filter = "Stickman Object file (*.smo)|*.smo";
                        ofd.CheckFileExists = true;
                        if (ofd.ShowDialog() != Forms.DialogResult.Cancel)
                        {
                            String FileName = Tools.GetFileNamePlusExtension(ofd.FileName);

                            doodad.Init(doodad.Core.Data.Position, doodad.MyBox.Current.Size, FileName, 0, 1);
                        }

                        Tools.DialogUp = false;
                    }

                    // Set texture
                    if (ButtonCheck.State(Keys.X).Pressed &&
                        !ButtonCheck.State(Keys.LeftControl).Down)
                    {
                        Tools.DialogUp = true;

                        Forms.OpenFileDialog ofd = new Forms.OpenFileDialog();
                        ofd.Title = "Load texture...";

                        ofd.InitialDirectory = Tools.SourceTextureDirectory();
                        ofd.Filter = "Texture file (*.png)|*.png";
                        ofd.CheckFileExists = true;
                        if (ofd.ShowDialog() != Forms.DialogResult.Cancel)
                        {
                            String FileName = Tools.GetFileName(ofd.FileName);

                            if (null != doodad)
                                doodad.MyQuad.TextureName = FileName;
                            if (door != null)
                                door.MyQuad.TextureName = FileName;
                        }

                        Tools.DialogUp = false;
                    }

                    if (Tools.keybState.IsKeyDownCustom(Keys.D1) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.D1))
                    {
                        Tools.Dlg = new BlockDialog();
                        DialogUpStart();
                        Tools.Dlg.textBox1.Text = "";
                        if (null != doodad)
                            if (doodad.MyObject == null)
                                Tools.Dlg.textBox1.Text = doodad.MyQuad.Quad.MyTexture.Path;
                            else
                            {
                                Tools.Dlg.textBox1.Text = doodad.MyFileName;
                                doodad.Core.EditorCode1 = doodad.MyAnim.ToString() + ", " + doodad.MyAnimSpeed.ToString();
                            }
                        if (null != door)
                            Tools.Dlg.textBox1.Text = door.MyQuad.Quad.MyTexture.Path;
                        Tools.Dlg.textBox2.Text = HeldObject.Core.EditorCode1;
                        Tools.Dlg.textBox3.Text = HeldObject.Core.EditorCode2;
                        Tools.Dlg.textBox4.Text = HeldObject.Core.EditorCode3;
                        if (Tools.Dlg.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
                        {
                            HeldObject.Core.EditorCode1 = Tools.Dlg.textBox2.Text;
                            HeldObject.Core.EditorCode2 = Tools.Dlg.textBox3.Text;
                            HeldObject.Core.EditorCode3 = Tools.Dlg.textBox4.Text;
                            if (null != doodad)
                            {
                                // check to see if we're loading an obj
                                string filename = Tools.Dlg.textBox1.Text;
                                if (filename.Substring(filename.Length - 4, 4) == ".smo")
                                {
                                    Vector2 parse = new Vector2(0, 1);
                                    
                                    string ParseText = Tools.Dlg.textBox2.Text;
                                    if (ParseText.Length > 0)
                                        parse = Tools.ParseToVector2(ParseText);

                                    doodad.Init(doodad.Core.Data.Position, doodad.MyBox.Current.Size, filename, (int)parse.X, parse.Y);
                                }
                                else
                                {
                                    doodad.MyObject = null;
                                    doodad.MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName(Tools.Dlg.textBox1.Text);

                                    Vector2 size = new Vector2(doodad.MyQuad.Base.e1.X, doodad.MyQuad.Base.e2.Y);
                                    //Vector2 size = 2 * new Vector2(doodad.MyQuad.Quad.MyTexture.Tex.Width, doodad.MyQuad.Quad.MyTexture.Tex.Height);                                    
                                    
                                    //doodad.MyQuad.Base.e2.Y = 1000;
                                    //doodad.MyQuad.ScaleXToMatchRatio();
                                    //doodad.Init(doodad.Core.Data.Position, new Vector2(doodad.MyQuad.Base.e1.X, doodad.MyQuad.Base.e2.Y));
                                    doodad.Init(doodad.Core.Data.Position, size);
                                }
                            }
                            if (null != door)
                                door.MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName(Tools.Dlg.textBox1.Text);
                        }
                        Tools.Dlg = null;
                        DialogUpEnd();
                    }
                }                       
            }

            // Set correct ratio for doodad
            if (Tools.keybState.IsKeyDownCustom(Keys.Z))
            {
                if (null != doodad)
                {
                    if (doodad.MyObject != null)
                        doodad.MakeBoxSquare();
                    else
                    {
                        doodad.MyQuad.ScaleXToMatchRatio();

                        doodad.MatchBoxToQuad();
                    }
                }
            }

            bool xOnly = false;
            if (Tools.keybState.IsKeyDown(Keys.D))
            {
                xOnly = true;
                Dir.Y = 0;
            }

            // Scale selected block
            if (Tools.keybState.IsKeyDownCustom(Keys.LeftShift) ||
                Tools.keybState.IsKeyDownCustom(Keys.RightShift))
            {
                if (ButtonCheck.State(Keys.LeftShift).Pressed)
                    SaveMousePos = Tools.MousePos;

                if (MyHandState == HandState.Closed && MyHoldingState != HoldingState.Nothing)
                {
                    BlockBase block = HeldObject as BlockBase;
                    if (null != block)
                    {
                        if (Tools.keybState.IsKeyDown(Keys.Down))
                            block.Extend(Side.Bottom, block.Box.Current.BL.Y - Dir.Y);
                        else
                        {
                            block.Extend(Side.Right, block.Box.Current.TR.X + Dir.X);
                            block.Extend(Side.Left, block.Box.Current.BL.X - Dir.X);
                            block.Extend(Side.Top, block.Box.Current.TR.Y + Dir.Y);
                            block.Extend(Side.Bottom, block.Box.Current.BL.Y - Dir.Y);
                        }
                    }

                    Door door = HeldObject as Door;
                    if (null != door)
                    {
                        door.MyQuad.Base.e1.X += Dir.X;
                        door.MyQuad.Base.e2.Y += Dir.Y;
                    }

                    Cloud cloud = HeldObject as Cloud;
                    if (null != cloud)
                    {
                        cloud.Box.Current.Size += Dir;
                        cloud.Box.Target.Size += Dir;
                    }
                }
            }
#else
            if (false) { }
#endif
            else
            {
#if XBOX
                Core.Data.Position += Dir;
#else
                if (ButtonCheck.State(Keys.LeftShift).Released)
                    Tools.MousePos = SaveMousePos;
                else
                {
                    if (xOnly)
                        Core.Data.Position.X = Tools.MouseWorldPos().X;
                    else
                        Core.Data.Position = Tools.MouseWorldPos();
                }
#endif
                if (MyHandState == HandState.Closed && MyHoldingState != HoldingState.Nothing)
                {
                    if (HeldCount >= 25)
                        SelectOffset = 0;
                    else
                        SelectOffset *= -.85f;

                    Tools.MoveTo(HeldObject, Core.Data.Position + HeldObjOffset + new Vector2(SelectOffset, 0));
                    Tools.Write(HeldObject.Pos);
                }
            }

#if WINDOWS
            // Make block
            if (Tools.keybState.IsKeyDownCustom(Keys.M) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.M))
            {
                NormalBlock block = (NormalBlock)Core.Recycle.GetObject(ObjectType.NormalBlock, false);
                block.Init(Core.Data.Position, new Vector2(100, 100), Core.MyLevel.MyTileSetInfo);
                if (Tools.CurLevel.CurEditorDrawLayer >= 0)
                    block.Core.DrawLayer = Tools.CurLevel.CurEditorDrawLayer;

                Core.MyLevel.AddBlock(block);
            }

            // Make top only
            if (Tools.keybState.IsKeyDownCustom(Keys.LeftShift) && !Tools.keybState.IsKeyDownCustom(Keys.LeftControl) && Tools.keybState.IsKeyDownCustom(Keys.Tab) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.Tab))
            {
                NormalBlock block = HeldObject as NormalBlock;
                if (null != block)
                {
                    if (block.Box.TopOnly)
                        block.Box.TopOnly = false;
                    else
                        block.MakeTopOnly();
                }
            }

            // Make invisible
            if (!Tools.keybState.IsKeyDownCustom(Keys.LeftShift) && !Tools.keybState.IsKeyDownCustom(Keys.LeftControl) && Tools.keybState.IsKeyDownCustom(Keys.Tab) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.Tab))
            {
                if (HeldObject != null)
                {
                    HeldObject.Core.Show = !HeldObject.Core.Show;
                }
            }

            // Change tile set
            if (!Tools.keybState.IsKeyDownCustom(Keys.LeftShift) && Tools.keybState.IsKeyDownCustom(Keys.LeftControl) && Tools.keybState.IsKeyDownCustom(Keys.Tab) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.Tab))
            {
                if (HeldObject != null)
                {
                    NormalBlock block = HeldObject as NormalBlock;
                    int tile_index = TileSets.TileList.IndexOf(HeldObject.Core.MyTileSet);
                    tile_index++;
                    if (tile_index >= TileSets.TileList.Count) tile_index = 0;
                    HeldObject.Core.MyTileSet = TileSets.TileList[tile_index];

                    if (null != block)
                        block.ResetPieces();
                }
            }

            // Delete
            if (Tools.keybState.IsKeyDownCustom(Keys.Delete) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.Delete))
            {
                if (HeldObject != null)
                    Core.Recycle.CollectObject(HeldObject);                                

                SetState(HandState.Open);
                SetState(HoldingState.Nothing);
            }



            // Make door
            if (Tools.keybState.IsKeyDownCustom(Keys.N) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.N))
            {
                Door door = (Door)Core.Recycle.GetObject(ObjectType.Door, false);
                door.Core.Data.Position = door.Core.StartData.Position = Core.Data.Position;
                if (Tools.CurLevel.CurEditorDrawLayer >= 0)
                    door.Core.DrawLayer = Tools.CurLevel.CurEditorDrawLayer;

                Core.MyLevel.AddObject(door);
            }

            // Make cloud
            if (Tools.keybState.IsKeyDownCustom(Keys.L) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.L))
            {
                Cloud cloud = (Cloud)Core.Recycle.GetObject(ObjectType.Cloud, false);
                cloud.Init(new Vector2(100, 100), TileSets.Terrace);
                cloud.Move(Core.Data.Position);
                if (Tools.CurLevel.CurEditorDrawLayer >= 0)
                    cloud.Core.DrawLayer = Tools.CurLevel.CurEditorDrawLayer;

                Core.MyLevel.AddObject(cloud);
            }

            // Clear all
            if (Tools.keybState.IsKeyDownCustom(Keys.K) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.K))
            {
                foreach (BlockBase block in Core.MyLevel.Blocks)
                    Core.Recycle.CollectObject(block);

                foreach (ObjectBase obj in Core.MyLevel.Objects)
                    if (obj != this)
                        Core.Recycle.CollectObject(obj);
            }

            // Make doodad
            if (Tools.keybState.IsKeyDownCustom(Keys.J) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.J))
            {
                Doodad doo = (Doodad)Core.Recycle.GetObject(ObjectType.Doodad, false);
                doo.Init(Core.Data.Position, new Vector2(100, 100));
                if (Tools.CurLevel.CurEditorDrawLayer >= 0)
                    doo.Core.DrawLayer = Tools.CurLevel.CurEditorDrawLayer;                
                
                Core.MyLevel.AddBlock(doo);
            }
#endif

            if (ButtonCheck.State(ControllerButtons.A, MyPlayerIndex).Pressed
#if WINDOWS
                ||
                Tools.CurMouseState.LeftButton == ButtonState.Pressed &&
                Tools.PrevMouseState.LeftButton != ButtonState.Pressed
#endif     
                )
            {
                if (MyHandState == HandState.Open)
                {
                    // Check to see if we grab anything
                    ObjectBase BestFitObj = null;
                    float BestFit = 0;
                    foreach (BlockBase block in Core.MyLevel.Blocks)
                    {
                        if (!block.Core.EditHoldable) continue;
                        if (!Tools.CurLevel.ShowDrawLayer[block.Core.DrawLayer]) continue;
                        if (block is Doodad && ((Doodad)block).MyType == Doodad.Type.LayerZone && !SelectLayerZones) continue;
                        if (OnlyMovingDoodads)
                        {
                            if (block is Doodad)
                            {
                                if (((Doodad)block).MyObject == null) continue;
                            }
                            else
                                continue;
                        }
                        if (OnlySmallStaticDoodads)
                        {
                            if (block is Doodad)
                            {
                                if (((Doodad)block).MyObject != null) continue;
                                if (((Doodad)block).Box.Current.Size.Length() > 1200) continue;
                            }
                            else
                                continue;
                        }

                        if ((Tools.CurLevel.CurEditorDrawLayer == -1 || block.Core.DrawLayer == Tools.CurLevel.CurEditorDrawLayer)
                            && Phsx.OverlapRatio(block.Box, Box) > 0)
                            //&& Phsx.BoxBoxOverlap(block.Box, Box))
                        {
                            float fit = Phsx.OverlapRatio(block.Box, Box);
                            Tools.Write(fit);
                            if (fit > BestFit)
                            {
                                BestFitObj = block;
                                BestFit = fit;
                            }                            
                        }
                    }

                    foreach (ObjectBase obj in Core.MyLevel.Objects)
                    {
                        if (obj.Core.DrawLayer < 0 || !Tools.CurLevel.ShowDrawLayer[obj.Core.DrawLayer]) continue;
#if WINDOWS
                        if (obj.Core.Holdable || obj.Core.EditHoldable)
#else
                        if (obj.Core.Holdable)
#endif
                        {
                            if ((Tools.CurLevel.CurEditorDrawLayer == -1 || obj.Core.DrawLayer == Tools.CurLevel.CurEditorDrawLayer) &&
                                Phsx.PointAndAABoxCollisionTest(ref obj.Core.Data.Position, Box))
                                BestFitObj = obj;
                        }
                    }

                    if (BestFitObj != null)
                    {
                        HeldObject = BestFitObj;
                        SetState(HandState.Closed);
                        SetState(HoldingState.Old);

                        HeldObjOffset = HeldObject.Core.Data.Position - Core.Data.Position;

                        HeldCount = 0;
                        SelectOffset = 40;
                    }
                }
                else
                {
                    SetState(HandState.Open);
                    HeldObject = null;
                }
            }
        }

        public void Update()
        {
            MyQuad.Base.Origin = Core.Data.Position;
        }

        public override void Draw()
        {
            if (Tools.ScreenshotMode || Tools.CapturingVideo) return;

            if (Tools.DrawGraphics)
            {
                //if (HeldObject != null)
                //{
                //    HeldObject.Draw();
                //}

                Update();

                MyQuad.Draw();
            }
        }

        public override void Move(Vector2 shift)
        {
            Core.Data.Position += shift;

            Box.Move(shift);
        }

        public override void PhsxStep()
        {
            // Enforce camera boundary
            Core.Data.Position.X = Math.Max(Core.MyLevel.MainCamera.BL.X, Core.Data.Position.X);
            Core.Data.Position.Y = Math.Max(Core.MyLevel.MainCamera.BL.Y, Core.Data.Position.Y);
            Core.Data.Position.X = Math.Min(Core.MyLevel.MainCamera.TR.X, Core.Data.Position.X);
            Core.Data.Position.Y = Math.Min(Core.MyLevel.MainCamera.TR.Y, Core.Data.Position.Y);

            Box.SetTarget(Core.Data.Position, HandSize);
            Box.SwapToCurrent();

            GetPlayerInput();

#if WINDOWS
            Core.MyLevel.MainCamera.Target = Vector2.Max(Core.MyLevel.MainCamera.Target, Core.MyLevel.MainCamera.Data.Position + 2 * (Core.Data.Position - Core.MyLevel.MainCamera.TR));
            Core.MyLevel.MainCamera.Target = Vector2.Min(Core.MyLevel.MainCamera.Target, Core.MyLevel.MainCamera.Data.Position + 2 * (Core.Data.Position - Core.MyLevel.MainCamera.BL));
#else
            Core.MyLevel.MainCamera.Target.X += Vector2.Max(Vector2.Zero, Core.Data.Position - Core.MyLevel.MainCamera.TR).X;
            Core.MyLevel.MainCamera.Target.X += Vector2.Min(Vector2.Zero, Core.Data.Position - Core.MyLevel.MainCamera.BL).X;
#endif
        }
    }
}
