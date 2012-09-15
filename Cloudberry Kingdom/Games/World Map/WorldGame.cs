using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Awards;
using CloudberryKingdom.Stats;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class WorldGame : WorldGameData
    {
        protected override void LaunchNode()
        {
            
        }

        //Challenge GuidToChallenge(int guid)
        //{
        //    switch (guid)
        //    {
        //        case 2: return Challenge_Escalation.Instance;
        //        case 1: return Challenge_TimeCrisis.Instance;
        //        case 3: return Challenge_HeroRush.Instance;
        //        case 4: return Challenge_HeroRush2.Instance;
        //    }

        //    return null;
        //}

        public override bool IsBeaten(BackgroundNode node)
        {
            //var challenge = GuidToChallenge(node.Guid);
            //if (challenge == null) return false;

            //challenge.UpdateGoalMet();
            //return challenge.IsGoalMet();
            return true;
        }

        protected override void SetNode(BackgroundNode node)
        {
            //var challenge = GuidToChallenge(node.Guid);
            //if (challenge == null)
            //{
            //    SetTitle("<Undefined>");
            //    MyGui.SetScore(-1);
            //}
            //else
            //{
            //    // Title
            //    SetTitle(string.Format("{0}. {1}", node.Number, challenge.MenuName));

            //    // Score
            //    MyGui.SetScore(challenge.HighScore.Top);
            //}

            SetTitle("");
            return;

            switch (Tools.GlobalRnd.RndInt(0, 5))
            {
                case 0: SetTitle("Beefy Outpost of Saucey Vomit"); break;
                case 1: SetTitle("Tiny Keep of Sadness"); break;
                case 2: SetTitle("Massive Spire of Infinity"); break;
                case 3: SetTitle("Altruistic Shack of Despair"); break;
                case 4: SetTitle("Happy Cespool of the Tiger"); break;
                case 5: SetTitle("Raunchy Ranch of Wretchedness"); break;
                default: SetTitle("Beefie Outpost of Saucey Vomit"); break;
            }
            //SetTitle(node.Guid.ToString());
        }

        public override void Release()
        {
            base.Release();
        }

        public WorldGame() : base()
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

            level.DefaultHeroType = BobPhsxMap.Instance;

            level.TimeLimit = -1;

            level.CurPiece = level.StartNewPiece(0, null, 4);


            float zoom = 0.00072f;
            Vector2 Start = new Vector2(-1000000000);
            Vector2 End = new Vector2(1000000000);


            level.MainCamera.MakeFancyPos();
            level.MainCamera.FancyPos.RelVal = Start;

            level.CurPiece.CamStartPos = Start;

            // Camera Zone
            CamZone = (CameraZone)Recycle.GetObject(ObjectType.CameraZone, false);
            CamZone.Init(Vector2.Zero, new Vector2(100000, 100000));
            CamZone.Start = Start;
            CamZone.End = End;
            CamZone.Zoom = zoom / .001f;
            CamZone.CameraType = Camera.PhsxType.WorldMap;
            level.AddObject(CamZone);
            level.MainCamera.MyZone = CamZone;
            level.CurPiece.CamStartPos = new Vector2(0, 0);

            // Initialize the background
            level.MyTileSet = "forest";
            level.MyBackground = Background.Get("World");

            level.MyBackground.Init(level);
            
            return level;
        }

        public override void Init()
        {
            base.Init();
        }

        public override void AdditionalReset()
        {
            base.AdditionalReset();
        }

        public override void PostDraw()
        {
            base.PostDraw();
        }
    }
}