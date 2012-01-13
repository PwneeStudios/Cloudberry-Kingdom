using Microsoft.Xna.Framework;
using System;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Awards;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class TitleGameData : GameData
    {
        public static bool PromoTitle =
            //true;
            false;

        public Vector2 Center;

        public new static GameData Factory(LevelSeedData data, bool MakeInBackground)
        {
            return null;
        }

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
      
        CameraZone CamZone;
        public Level MakeLevel()
        {
            Level level = new Level();
            level.MainCamera = new Camera();

            level.TimeLimit = -1;

            level.CurPiece = level.StartNewPiece(0, null, 4);

            if (PromoTitle)
            {
                Center = new Vector2(0, 0);
                Camera.DisableOscillate = true;
            }
            else
                Center = new Vector2(0, 1130 + 230);
            level.MainCamera.Zoom *= .7f;
            level.MainCamera.MakeFancyPos();
            level.MainCamera.FancyPos.RelVal = Center;

            level.CurPiece.CamStartPos = Center;


            // Camera Zone
            CamZone = (CameraZone)Recycle.GetObject(ObjectType.CameraZone, false);
            CamZone.Init(Vector2.Zero, new Vector2(100000, 100000));
            CamZone.Start = Center;
            CamZone.End = Center;
            CamZone.FreeY = false;
            CamZone.Zoom *= .7f;
            level.AddObject(CamZone);
            level.MainCamera.MyZone = CamZone;

            // Initialize the background
            if (PromoTitle)
                level.MyBackground = Background.Get(BackgroundType.NightSky);
            else
                level.MyBackground = Background.Get(BackgroundType.Outside);
            //level.MyBackground = Background.Get(BackgroundType.Chaos);
            
            level.MyBackground.Init(level);
            level.MyBackground.AddSpan(level.BL - new Vector2(58000, 0), level.TR + new Vector2(58000, 0));

            // No clouds
            //if (PromoTitle)
            //    level.MyBackground.MyCollection.Clear();

            return level;
        }

        public static int InitialDelay = 40;//5;
     
        public override void Init()
        {
            base.Init();

            Tools.CurLevel = MyLevel = MakeLevel();
            Tools.WorldMap = Tools.CurGameData = MyLevel.MyGame = this;
            Tools.CurGameType = TitleGameData.Factory;
            
            Tools.TitleGame = this;

            if (!PromoTitle)
            WaitThenDo(InitialDelay, () =>
            {
                GUI_Panel panel;

                panel = new StartMenu();

                //panel = new AdvancedCustomGUI();
                //panel = new SimpleCustomGUI();

                //panel = new StatsMenu();
                //panel = new AwardmentMenu();
                //panel = new CategoryMenu();
                //panel = new CampaignMenu();
                //panel = new ArcadeMenu();

                this.AddGameObject(panel);
            });

            MyLevel.PlayMode = 0;
            MyLevel.ResetAll(false);
        }

        public override void AdditionalReset()
        {
            base.AdditionalReset();

            MyLevel.MainCamera.MyZone = CamZone;            
        }

        public bool PanCamera = false;//true;
        public override void PhsxStep()
        {
#if PC_VERSION
            Tools.TheGame.ShowMouse = true;
#endif
            Camera cam = MyLevel.MainCamera;
            cam.MyPhsxType = Camera.PhsxType.Fixed;

            // Pan the camera
            //cam.Pos += cam.Data.Velocity;
            cam.FancyPos.RelVal += cam.Data.Velocity;
            if (PanCamera)
            {
                float acc = .02f;
                float MaxSpeed = 3.8f; //30f;
                float MaxDist = 34000;

                if (cam.Pos.X > MaxDist)
                    cam.Data.Acceleration.X = -acc;
                if (cam.Pos.X < -MaxDist)
                    cam.Data.Acceleration.X = acc;
                if (cam.Data.Acceleration == Vector2.Zero)
                    cam.Data.Acceleration = new Vector2(1, 0);

                if (cam.Data.Velocity.X < MaxSpeed && cam.Data.Acceleration.X > 0 ||
                    cam.Data.Velocity.X > -MaxSpeed && cam.Data.Acceleration.X < 0)
                    cam.Data.Velocity += cam.Data.Acceleration;

                cam.Data.UpdatePosition();
            }
            else
                Cam.Data.Velocity *= .98f;

            base.PhsxStep();
        }

        public override void PostDraw()
        {
            base.PostDraw();
        }
    }
}