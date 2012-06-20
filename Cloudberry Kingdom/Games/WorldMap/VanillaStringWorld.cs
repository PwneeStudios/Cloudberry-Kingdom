using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class VanillaStringWorld : Challenge
    {
        protected int i = 0;
        protected int StartIndex = 0;
        protected StringWorldGameData MyStringWorld;
        public void Start()
        {
            // Create the string world, and add the relevant game objects
            MyStringWorld = new StringWorldGameData(GetSeeds());
            MyStringWorld.EOG_DoorAction = door =>
                {
                    if (door.Game.MakeScore == null)
                        door.Game.MakeScore = () => new ScoreScreen_Campaign(StatGroup.Game, door.Game);
                    door.Game.MyStatGroup = StatGroup.Game;
                    GameData.EOL_DoorAction(door);//, StatGroup.Game);
                };
            //MyStringWorld.StartLevelMusic = game => { };

            // Start the music on the first level
            MyStringWorld.OnSwapToFirstLevel += seed => LevelSeedData.BOL_StartMusic();

            // Record the players on the last level
            MyStringWorld.OnSwapToLastLevel += seed => seed.MyGame.MyLevel.StartRecording();

            // Additional preprocessing
            SetGameParent(MyStringWorld);
            AdditionalPreStart();

            // Start
            MyStringWorld.Init();
        }

        protected virtual void AdditionalPreStart()
        {
        }

        protected void ProcessList(List<LevelSeedData> seeds)
        {
            foreach (LevelSeedData seed in seeds)
                Campaign.ToStringSeed(seed);
        }
    }
}