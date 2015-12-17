using System.Collections.Generic;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class LevelSequence
    {
        protected StringWorldGameData MyStringWorld;

        public List<string> Seeds = new List<string>(600);

        public LevelSequence()
        {
            MakeSeedList();
        }

        protected virtual void MakeSeedList()
        {
            for (int i = 0; i < 500; i++)
                Seeds.Add(i.ToString());
        }

        int StartIndex;
        public virtual void Start(int StartLevel)
        {
#if DEBUG
            // For debug purposes we reload the list of seeds each time, so that hot modifications can be made.
            Seeds.Clear();
            MakeSeedList();
#endif

            StartIndex = StartLevel;

            // Create the string world, and add the relevant game objects
            MyStringWorld = new StringWorldGameData(GetSeed);
            MyStringWorld.StartLevelMusic = null;

            // OnLevelBegin preprocessing for each level.
            MyStringWorld.OnLevelBegin += OnLevelBegin;

            // Additional preprocessing
            SetGameParent(MyStringWorld);
            AdditionalPreStart();

            // Start
            MyStringWorld.Init(StartLevel);
        }

        /// <summary>
        /// Returns true if the standard StringWorld processing should be skipped.
        /// Special levels (such as watching a movie) do not need the normal processing (and would crash the game if they happened).
        /// </summary>
        protected virtual bool OnLevelBegin(Level level)
        {
            if (level.MyGame is ActionGameData) return true;

            level.MyGame.AddGameObject(InGameStartMenu.MakeListener());
            return false;
        }

        protected virtual void SetGameParent(GameData game)
        {
            game.ParentGame = Tools.CurGameData;
            Tools.WorldMap = Tools.CurGameData = game;
            Tools.CurLevel = game.MyLevel;
        }

        protected virtual void AdditionalPreStart()
        {
        }

        public virtual LevelSeedData GetSeed(int Index)
        {
            var seed = new LevelSeedData();
            seed.ReadString(Seeds[Index]);

            //seed.PostMake = null;

            return seed;
        }
    }
}