using Microsoft.Xna.Framework;
using System;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Awards;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class TitleGameData_Intense : TitleGameData
    {
        public static GameData Factory()
        {
            return new TitleGameData_Intense();
        }

        public override void Release()
        {
            base.Release();
        }

        public TitleGameData_Intense() : base()
        {
        }

        public override void SetToReturnTo(int code)
        {
            base.SetToReturnTo(code);
        }

        public override void ReturnTo(int code)
        {
            base.ReturnTo(code);
        }
      
        protected override Level MakeLevel()
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

            return level;
        }

        public override void Init()
        {
            base.Init();

            Tools.CurGameType = TitleGameData_Intense.Factory;
            
            Tools.TitleGame = this;

            // Backdrop
            AddGameObject(new StartMenu_Intense_Backpanel()); 

            // Menu
            GUI_Panel panel;

            if (CloudberryKingdomGame.StartAsFreeplay)
                panel = new CustomLevel_GUI();
            else
            {
                panel = new StartMenu_Intense();

                //panel = new StartMenu();

                //panel = new AdvancedCustomGUI();
                //panel = new SimpleCustomGUI();

                //panel = new StatsMenu();
                //panel = new AwardmentMenu();
                //panel = new CategoryMenu();
                //panel = new CampaignMenu();
                //panel = new ArcadeMenu();
            }

                AddGameObject(panel);
        }

        public override void AdditionalReset()
        {
            base.AdditionalReset();

            MyLevel.MainCamera.MyZone = CamZone;            
        }

        public override void PhsxStep()
        {
#if PC_VERSION
            Tools.TheGame.ShowMouse = true;
#endif
            Camera cam = MyLevel.MainCamera;
            cam.MyPhsxType = Camera.PhsxType.Fixed;

            // Pan the camera
            cam.FancyPos.RelVal += cam.Data.Velocity;
            if (PanCamera)
            {

                if (cam.Pos.X > PanMaxDist)
                    cam.Data.Acceleration.X = -PanAcc;
                if (cam.Pos.X < PanMinDist)
                    cam.Data.Acceleration.X = PanAcc;
                if (cam.Data.Acceleration == Vector2.Zero)
                    cam.Data.Acceleration = new Vector2(1, 0);

                if (cam.Data.Velocity.X < PanMaxSpeed && cam.Data.Acceleration.X > 0 ||
                    cam.Data.Velocity.X > -PanMaxSpeed && cam.Data.Acceleration.X < 0)
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