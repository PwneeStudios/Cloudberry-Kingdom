using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Awards;
using CloudberryKingdom.Stats;
using CloudberryKingdom.Bobs;
using Drawing;

namespace CloudberryKingdom
{
    public class WorldGameData : GameData
    {
        public new static GameData Factory(LevelSeedData data, bool MakeInBackground)
        {
            return null;
        }

        public override void Release()
        {
            base.Release();

            if (Tools.WorldMap == this)
                Tools.WorldMap = null;

            if (Released) return;
        }

        public WorldGameData()
        {
            LockLevelStart = false;
            SuppressQuickSpawn = true;

            Init();

            Tools.CurGameData.SuppressQuickSpawn = true;
        }

        protected WorldMapGui MyGui;

        public override void SetToReturnTo(int code)
        {
            base.SetToReturnTo(code);

            WaitThenDo(10, FadeIn);
            NodeSelected = false;

            Tools.WorldMap = this;
        }

        void FadeIn()
        {
            MyLevel.UseLighting = false;
            FadeIn(.0675f);
        }

        public override void ReturnTo(int code)
        {
            CleanLastLevel();

            // Fire event
            ReturnToEvent();
        }

        protected CameraZone CamZone;
        protected virtual Level MakeLevel()
        {
            Level level = new Level();
            level.MainCamera = new Camera();

            level.DefaultHeroType = BobPhsxMap.Instance;
            
            return level;
        }

        void ProcessBackground()
        {
            foreach (BackgroundFloaterList list in MyLevel.MyBackground.MyCollection.Lists)
            {
                foreach (BackgroundFloater floater in list.Floaters)
                {
                    BackgroundNode node = floater as BackgroundNode;
                    if (null != node)
                        Nodes.Add(node);
                }
            }

            StartNode = Nodes[0];
        }

        protected BobPhsxMap MapBob
        {
            get
            {
                return MyLevel.Bobs[0].MyPhsx as BobPhsxMap;
            }
        }

        void Start()
        {
            MapBob.SetNode(StartNode);
        }

        protected List<BackgroundNode> Nodes = new List<BackgroundNode>();
        protected BackgroundNode StartNode;

        public override void Init()
        {
            base.Init();

            DefaultHeroType = BobPhsxMap.Instance;

            Tools.CurLevel = MyLevel = MakeLevel();
            Tools.WorldMap = Tools.CurGameData = MyLevel.MyGame = this;
            Tools.CurGameType = WorldGameData.Factory;

            Tools.WorldMap = this;

            ProcessBackground();

            MakeBobs(MyLevel);

            MyLevel.PlayMode = 0;
            MyLevel.ResetAll(false);

            // Gui
            MyGui = new WorldMapGui();
            AddGameObject(MyGui);
        }

        public override void AdditionalReset()
        {
            base.AdditionalReset();
            Start();

            MyLevel.MainCamera.MyZone = CamZone;
        }

        protected BackgroundNode CurrentNode;
        void CheckToSetNode(BackgroundNode node)
        {
            if (node != CurrentNode)
            {
                CurrentNode = node;
                SetNode(CurrentNode);
            }
        }

        protected virtual void SetNode(BackgroundNode node)
        {
        }

        public bool NodeSelected = false;
        public override void PhsxStep()
        {
            if (MapBob.CurrentNode != null)
                CheckToSetNode(MapBob.CurrentNode);

//#if PC_VERSION
//            Tools.TheGame.ShowMouse = true;
//#endif
            if (!NodeSelected && IsPlayable(MapBob.CurrentNode))
                if (ButtonCheck.State(ControllerButtons.A, -1).Pressed)
                {
                    NodeSelected = true;
                    WaitThenDo(0, ClosingCircle);
                    WaitThenDo(0 + 80, LaunchNode);
                }

            base.PhsxStep();
        }

        protected virtual void LaunchNode()
        {
        }

        protected void ClosingCircle()
        {
            MyLevel.LightLayer = Level.LightLayers.FrontOfEverything;
            MyLevel.MakeClosingCircle(125, MapBob.Pos);
        }

        public virtual bool IsBeaten(BackgroundNode node)
        {
            return true;
        }

        public virtual bool IsPlayable(BackgroundNode node)
        {
            return false;
        }

        public virtual bool PathIsOpen(BackgroundNode node1, BackgroundNode node2)
        {
            return IsBeaten(node1) || IsBeaten(node2);
        }

        protected virtual void DrawConnection(BackgroundNode n1, BackgroundNode n2, EzTexture texture, int Orientation)
        {
            bool Open = PathIsOpen(n1, n2);

            float RepeatWidth = 900;
            bool Repeat = true;
            if (Open)
                Tools.QDrawer.DrawLine(n1.Data.Position, n2.Data.Position, Color.White, 265, texture, Tools.BasicEffect, RepeatWidth, Orientation, 0f, 0, Repeat);
            else
                Tools.QDrawer.DrawLine(n1.Data.Position, n2.Data.Position, Tools.GrayColor(.3f), 265, texture, Tools.BasicEffect, RepeatWidth, Orientation, 0f, 0, Repeat);
        }

        public override void PreDraw()
        {
            base.PreDraw();

            foreach (BackgroundFloaterList list in MyLevel.MyBackground.MyCollection.Lists)
            {
                if (list.DoPreDraw)
                {
                    foreach (BackgroundFloater floater in list.Floaters)
                    {
                        BackgroundNode node = floater as BackgroundNode; if (null == node) continue;

                        if (node.Connect_S != null) DrawConnection(node, node.Connect_S, node.PathTexture_Vertical, 0);
                        if (node.Connect_W != null) DrawConnection(node, node.Connect_W, node.PathTexture_Horizontal, 1);
                    }

                    list.DoPreDraw = false;
                    list.Draw();
                    list.DoPreDraw = true;
                }
            }

            MyLevel.MyBackground.MyCollection.FinishDraw();
        }

        public override void PostDraw()
        {
            base.PostDraw();
        }

        protected void SetTitle(string Title)
        {
            MyGui.SetTitle(Title);
        }
    }
}