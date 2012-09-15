using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Awards;
using CloudberryKingdom.Stats;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class ArcadeGame : WorldGameData
    {
        protected override void LaunchNode()
        {
            var challenge = GuidToChallenge(MapBob.CurrentNode.Guid);
            if (challenge == null) return;

            // Start the game
            PlayGame(() =>
            {
                // Show title again if we're selecting from the menu
                if (!ExecutingPreviousLoadFunction)
                    HeroRush_Tutorial.ShowTitle = true;

                challenge.Start(0);
            });            
        }

        Challenge GuidToChallenge(int guid)
        {
            switch (guid)
            {
                case 2: return Challenge_Escalation.Instance;
                case 1: return Challenge_TimeCrisis.Instance;
                case 3: return Challenge_HeroRush.Instance;
                case 4: return Challenge_HeroRush2.Instance;
            }

            return null;
        }

        public override bool IsPlayable(BackgroundNode node)
        {
            var challenge = GuidToChallenge(node.Guid);
            if (challenge == null) return false;

            return true;
        }

        public override bool IsBeaten(BackgroundNode node)
        {
            var challenge = GuidToChallenge(node.Guid);
            if (challenge == null) return false;

            challenge.UpdateGoalMet();
            return challenge.IsGoalMet();
        }

        protected override void SetNode(BackgroundNode node)
        {
            var challenge = GuidToChallenge(node.Guid);
            if (challenge == null)
            {
                SetTitle("<Undefined>");
                MyGui.SetScore(-1);
            }
            else
            {
                // Title
                //SetTitle(challenge.MenuName);
                SetTitle(string.Format("{0}. {1}", node.Number, challenge.MenuName));

                // Score
                MyGui.SetScore(challenge.HighScore.Top);
            }
            
            //SetTitle(node.Guid.ToString());
        }

        public override void Release()
        {
            base.Release();
        }

        public ArcadeGame() : base()
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


            //Vector2 Start = new Vector2(-167.9758f, -121.624f);
            //float zoom = 0.0009726429f;
            Vector2 Start = new Vector2(-215.2059f, 88.64383f);
            float zoom = 0.0007808327f;

            Vector2 End = Start + new Vector2(0, 5000);

            
            

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

            // Initialize the background
            level.MyTileSet = "forest";
            level.MyBackground = Background.Get("Arcade");

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