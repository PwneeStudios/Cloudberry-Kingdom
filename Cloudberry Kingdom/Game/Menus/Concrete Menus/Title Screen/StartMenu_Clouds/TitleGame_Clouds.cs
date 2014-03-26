using Microsoft.Xna.Framework;
using System;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Awards;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class TitleGameData_Clouds : TitleGameData
    {
        public static GameData Factory()
        {
            return new TitleGameData_Clouds();
        }

        public override void SetBackgroundState(TitleBackgroundState state)
        {
            BackPanel.SetState(state);
        }

        public override void Release()
        {
            base.Release();
        }

        public TitleGameData_Clouds() : base()
        {
        }

        public override void SetToReturnTo(int code)
        {
            base.SetToReturnTo(code);
        }

        public override void ReturnTo(int code)
        {
            if (SetToRepaly)
            {
                Tools.SongWad.SetPlayList(Tools.Song_Heavens);
                Tools.SongWad.Restart(true, false);
            }

            base.ReturnTo(code);
        }
      
        protected override Level MakeLevel()
        {
            Level level = new Level();
            level.MainCamera = new Camera();

            level.TimeLimit = -1;

            level.CurPiece = level.StartNewPiece(0, null, 4);

            Vector2 Center = new Vector2(0, 0);
            
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

            return level;
        }

        public StartMenu_Clouds_Backpanel BackPanel;
        public StartMenu_Clouds_PressStart PressStart;
        public override void Init()
        {
            base.Init();

            Tools.CurGameType = TitleGameData_Clouds.Factory;
            
            Tools.TitleGame = this;

            // Anders background
            var background = Background.Get(BackgroundType._Terrace);
            background.Init(MyLevel);
            MyLevel.SetBackground(background);

            // Backdrop
            BackPanel = new StartMenu_Clouds_Backpanel();
            AddGameObject(BackPanel);
            BackPanel.SetState(TitleBackgroundState.Scene_Title);
            BackPanel.InitialZoomIn();

            // Music
            Tools.SongWad.SuppressNextInfoDisplay = true;
            Tools.SongWad.SetPlayList(Tools.Song_Heavens);
            Tools.SongWad.Start(true);

            // Fade in
            FadeIn(.0175f);
            WaitThenDo(18, _Init);
        }

        void _Init()
        {
            // Press Start
            PressStart = new StartMenu_Clouds_PressStart(this);
            AddGameObject(PressStart);
        }

        public override void AdditionalReset()
        {
            base.AdditionalReset();

            MyLevel.MainCamera.MyZone = CamZone;            
        }

        public override void PhsxStep()
        {
            base.PhsxStep();
        }

        public override void PostDraw()
        {
            base.PostDraw();
        }
    }
}