using Microsoft.Xna.Framework;
using System;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Awards;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class TitleGameData_Forest : TitleGameData
    {
        public static GameData Factory()
        {
            return new TitleGameData_Forest();
        }

        public override void Release()
        {
            base.Release();
        }

        public TitleGameData_Forest() : base()
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

        string background = null;
        public void SetBackground(string background)
        {
            if (this.background == background) return;

            this.background = background;
            MyLevel.MyBackground = Background.Get(background);
            MyLevel.MyBackground.Init(MyLevel);
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

            // Initialize the background
            level.MyTileSet = "forest";
            //level.MyBackground = Background.Get("forest");
            level.MyBackground = Background.Get("forest_snow");

            level.MyBackground.Init(level);
            return level;
        }

        public StartMenu_Forest_PressStart PressStart;
        public StartMenu_Forest_Title Title;
        public StartMenu_MW_Black Black;
        public override void Init()
        {
            base.Init();

            Tools.CurGameType = TitleGameData_Forest.Factory;
            
            Tools.TitleGame = this;

            //FadeIn(.03f);
            //WaitThenDo(30, _Init);

            FadeIn(.015f);
            WaitThenDo(32, _Init);

            // Black
            Black = new StartMenu_MW_Black();
            Black.MyPile.Alpha = 0;
            AddGameObject(Black);

            // Music
            Tools.Warning();
            Tools.MusicVolume.Val = 0f;
            Tools.SongWad.SuppressNextInfoDisplay = true;
            Tools.SongWad.SetPlayList(Tools.Song_WritersBlock);
            Tools.SongWad.Start(true);
        }

        void _Init()
        {
            //Cam.FancyZoom.LerpTo(1f * .001f, 1.05f * .001f, 100, Drawing.LerpStyle.Linear);

            // Title
            Title = new StartMenu_Forest_Title(this);
            AddGameObject(Title); 

            // Press Start
            PressStart = new StartMenu_Forest_PressStart(this);
            AddGameObject(PressStart);
        }

            /*
            // Menu
            GUI_Panel panel;

            if (CloudberryKingdomGame.StartAsFreeplay)
                panel = new CustomLevel_GUI();
            else
            {
                panel = new StartMenu_Forest();

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
        }*/

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
            //CamZone.Zoom = Tools.LerpRestrict(.9f, 1f, Tools.t / 3);
            //CamZone.Zoom = .5f;

            Camera cam = MyLevel.MainCamera;
            cam.MyPhsxType = Camera.PhsxType.Fixed;

            // Pan the camera
            //PanCamera = false;
            PanCamera = true;
            //PanMaxSpeed = 3.8f;
            PanMaxSpeed = 1.35f;
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

            //cam.FancyPos.RelVal = Vector2.Zero;
            //cam.Pos = cam.Data.Velocity = Vector2.Zero;
        }

        public override void PostDraw()
        {
            base.PostDraw();
        }
    }
}