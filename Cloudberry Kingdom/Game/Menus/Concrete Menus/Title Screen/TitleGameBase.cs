using Microsoft.Xna.Framework;
using System;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Awards;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public enum TitleBackgroundState { None, CharSelect,
                        Scene_Title, Scene, Scene_Blur, Scene_StoryMode, Scene_Blur_Dark,
                        Scene_Arcade, Scene_Arcade_Blur };

    public abstract class TitleGameData : GameData
    {
        public new static GameData Factory(LevelSeedData data, bool MakeInBackground)
        {
            return null;
        }

        public abstract void SetBackgroundState(TitleBackgroundState state);

        public override void Release()
        {
            base.Release();

            if (Released) return;

            if (Tools.WorldMap == this)
                Tools.WorldMap = null;
        }

        public TitleGameData()
        {
            LockLevelStart = false;
            SuppressQuickSpawn = true;

            Init();

            Tools.CurGameData.SuppressQuickSpawn = true;
            Tools.CurGameData.SuppressSongInfo = true;
        }

        public override void SetToReturnTo(int code)
        {
            base.SetToReturnTo(code);

            Tools.WorldMap = this;
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

            level.TimeLimit = -1;

            level.CurPiece = level.StartNewPiece(0, null, 4);

            Vector2 Center = new Vector2(1000, 0);
            
            level.MainCamera.MakeFancyPos();
            level.MainCamera.FancyPos.RelVal = Center;

            level.CurPiece.CamStartPos = Center;

            // Camera Zone
            CamZone = (CameraZone)Recycle.GetObject(ObjectType.CameraZone, false);
            CamZone.Init(Vector2.Zero, new Vector2(100000, 100000));
            CamZone.Start = Center;
            CamZone.End = Center;
            CamZone.FreeY = false;
            level.AddObject(CamZone);
            level.MainCamera.MyZone = CamZone;

            // Initialize the background
            level.MyTileSet = "forest";
            //level.MyBackground = Background.Get("KobblerPie");
            level.MyBackground = Background.Get("forest");
            //level.MyBackground = Background.Get("forest_snow");

            level.MyBackground.Init(level);

            return level;
        }

        public override void Init()
        {
            base.Init();

            Tools.CurLevel = MyLevel = MakeLevel();
            Tools.WorldMap = Tools.CurGameData = MyLevel.MyGame = this;
            Tools.CurGameType = TitleGameData.Factory;
            
            Tools.TitleGame = this;

            MyLevel.PlayMode = 0;
            MyLevel.ResetAll(false);
        }

        public override void AdditionalReset()
        {
            base.AdditionalReset();

            MyLevel.MainCamera.MyZone = CamZone;            
        }

        public bool PanCamera = true;
        protected float PanAcc = .02f;
        protected float PanMaxSpeed = 3.8f;
        protected float PanMaxDist = 29000;
        protected float PanMinDist = 3000;

        public override void PhsxStep()
        {
#if PC_VERSION
            Tools.TheGame.ShowMouse = true;
#endif
            Camera cam = MyLevel.MainCamera;
            cam.MyPhsxType = Camera.PhsxType.Fixed;

            cam.FancyPos.RelVal = Vector2.Zero;

            base.PhsxStep();
        }

        public override void PostDraw()
        {
            base.PostDraw();
        }
    }
}