using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class ActionFactory : GameFactory
    {
        public override GameData Make(LevelSeedData data, bool MakeInBackground)
        {
            return new ActionGameData(data, MakeInBackground);
        }
    }

    public class ActionGameData : GameData
    {
        public static new GameFactory Factory = new ActionFactory();

        public ActionGameData() { }

        public ActionGameData(LevelSeedData LevelSeed, bool MakeInBackground)
        {
            Init(LevelSeed, MakeInBackground);
        }

        LevelSeedData Seed;
        public virtual void Init(LevelSeedData LevelSeed, bool MakeInBackground)
        {
            base.Init();

            Seed = LevelSeed;

            AllowQuickJoin = false;

            Loading = false;
            LevelSeed.Loaded.val = true;
            LevelSeed.MyGame = this;

            MyLevel = MakeEmptyLevel();
        }

        public bool Done = false;
        bool ActionTaken = false;

        public override void PhsxStep()
        {
            base.PhsxStep();

            if (!ActionTaken)
            {
                if (Seed.PostMake != null)
                    Seed.PostMake.Apply(MyLevel);

                ActionTaken = true;
            }

            if (Done)
            {
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();
        }

        public override void Draw()
        {
            Tools.TheGame.MyGraphicsDevice.Clear(Color.Black);

            base.Draw();
        }

        public override void Release()
        {
            base.Release();

            Seed = null;
        }

        Level MakeEmptyLevel()
        {
            Level level = new Level();
            level.MainCamera = new Camera();
            level.CurPiece = level.StartNewPiece(0, null, 4);
            level.CurPiece.StartData[0].Position = new Vector2(0, 0);
            level.MainCamera.BLCamBound = new Vector2(-100000, 0);
            level.MainCamera.TRCamBound = new Vector2(100000, 0);
            level.MainCamera.Update();
            level.TimeLimit = -1;

            level.MyBackground = new RegularBackground();
            level.MyBackground.Init(level);

            level.MyGame = this;

            return level;
        }

        public override void MakeBobs(Level level) { }

        public override void UpdateBobs() { }

        public override void Reset() { }
   }
}