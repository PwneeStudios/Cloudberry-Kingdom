using System.Linq;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public partial class Seed : IObject, ILevelConnector
    {
        public void Start_DownLaunch()
        {
            Launched = Panning = false;
            PreLaunchCount = 0;
        }

        public void DownLaunch()
        {
            Launched = true;

            // Prepare the stickmen
            foreach (Bob bob in Core.MyLevel.Bobs)
            {
                // Revive if needed
                if (!PlayerManager.IsAlive(bob.MyPlayerIndex))
                {
                    PlayerManager.ReviveBob(bob);
                    //Generic.RevivePlayer(bob.MyPlayerIndex);                    
                }
                bob.Move(this.Core.Data.Position - bob.Core.Data.Position);

                int i = (int)bob.MyPlayerIndex;

                bob.ScreenWrap = false;

                bob.EndSuckedIn();

                bob.SetToCinematic();
                bob.AffectsCamera = false;

                bob.Core.Data.Velocity = new Vector2(0, -150 + 1.15f * i);
                //Tools.RndFloat(-1.95f, 1.5f), 0);

                bob.PlayerObject.xFlip = false;

                // Set animation
                bob.PlayerObject.AnimQueue.Clear();
                bob.PlayerObject.EnqueueAnimation("Launched", Tools.RndFloat(0, 1), true, true, 1, 3.3f, false);
                bob.PlayerObject.DequeueTransfers();
            }
        }

        public void DownLaunch_PhsxStep()
        {
            if (Core.MarkedForDeletion) return;

            if (!SkipPhsx)
            {
                ParticleEffects.Coalesce(Core.MyLevel, Core.Data.Position);
                ParticleEffects.PieceOrb(Core.MyLevel, ParticleEffects.PieceOrbStyle.Cloud, Core.Data.Position, Core.MyLevel.CurPhsxStep, Intensity);
            }

            if (!Launched)
            {
                PreLaunchPhsx();
                return;
            }

            if (LandingBlock == null)
                return;

            Camera cam = Core.MyLevel.MainCamera;
            
            LaunchStep++;

            if (LaunchStep == 45)
                Tools.SoundWad.FindByName("Launched_Flying").Play();

            // Move stickmen
            bool AllOffScreen = Core.MyLevel.Bobs.All(delegate(Bob bob) { return bob.Core.Data.Position.Y < cam.BL.Y - 600; });
            foreach (Bob bob in Core.MyLevel.Bobs)
            {
                if (!bob.Cinematic) continue;

                        bob.Core.Data.Velocity.Y -= 7.5f;

                // Integrate
                bob.Core.Data.Velocity.Y += bob.Core.Data.Acceleration.Y;
                bob.Core.Data.Position += bob.Core.Data.Velocity;

            }

            // Check for end of Launch cinematic
            if (AllOffScreen && LaunchStep > 150)
                EndDownLaunchCinematic();
        }

        // Finish the launch cinematic, killing the seed in the end
        void EndDownLaunchCinematic()
        {
            Core.MyLevel.MyGame.AddToDo(delegate()
            {
/*                if (Tools.CurLevel != LandingSeedData.MyGame.MyLevel)
                {
                    LandingSeedData.MyGame.ToDo.Add(delegate()
                    {
                        // Wait until game is the current game
                        //  if (Tools.CurGameData != LandingSeedData.MyGame)
                        //    return true;

                        Tools.Recycle.Empty();

                        // Start music
                        Tools.StartPlaylist();

                        // Start recording
                        Tools.CurLevel.StartRecording();

                        return true;
                    });
                }
*/
                ((StringWorldGameData)Tools.WorldMap).SetLevel(NextLevelSeedData);

                // Shift all bobs up so they fall from the top of the screen
                foreach (Bob bob in Tools.CurLevel.Bobs)
                    Tools.MoveTo(bob, new Vector2(bob.Core.Data.Position.X, Tools.CurLevel.MainCamera.TR.Y));

                ((StringWorldGameData)Tools.WorldMap).LevelBegin(Tools.CurLevel);

                // Get rid of this seed
                Core.MarkedForDeletion = true;

                return true;
            });
        }
    }
}