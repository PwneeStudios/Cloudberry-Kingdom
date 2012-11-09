using System;
using System.Collections.Generic;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Obstacles;

namespace CloudberryKingdom
{
    public class ExplodeBobs : GUI_Panel
    {
        public ExplodeBobs(Speed speed)
        {
            Active = true;
            PauseOnPause = true;

            SetSpeed(speed);
        }

        public enum Speed { Regular, Fast };
        Speed MySpeed;

        public void SetSpeed(Speed speed)
        {
            MySpeed = speed;
            switch (MySpeed)
            {
                case Speed.Regular:
                    ScoreScreen.UseZoomIn = true;
                    InitialDelay_MultipleBobs = 48;
                    InitialDelay_OneBob = 27;

                    //ScoreScreen.UseZoomIn = false;
                    //InitialDelay_MultipleBobs = 56;
                    //InitialDelay_OneBob = 35;

                    break;

                case Speed.Fast:
                    InitialDelay_MultipleBobs = InitialDelay_MultipleBobs = 20; Delay = 20; break;
            }
        }

        public Lambda OnDone;
        void Finish()
        {
            if (OnDone != null)
                OnDone.Apply();

            Release();
        }

        int CompareBobs(Bob A, Bob B)
        {
            return A.Core.Data.Position.X.CompareTo(B.Core.Data.Position.X);
        }

        int Count = 0;
        public int InitialDelay_MultipleBobs = 56, Delay = 40, InitialDelay_OneBob = 28;
        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            Level level = Core.MyLevel;

            Count++;

            // Decide how long to wait before starting the action
            int InitialDelay;
            if (PlayerManager.GetNumPlayers() == 1)
                InitialDelay = InitialDelay_OneBob;
            else
                InitialDelay = InitialDelay_MultipleBobs;

            // Do nothing until that wait period is over
            if (Count < InitialDelay) return;

            // Afterward blow up a bob periodicially
            if ((Count - InitialDelay) % Delay == 0)
            {
                List<Bob> bobs = new List<Bob>();
                foreach (var bob in Core.MyLevel.Bobs)
                    if (bob.Core.Show && bob.GetPlayerData().IsAlive)
                        bobs.Add(bob);
                //List<Bob> bobs = Core.MyLevel.Bobs.FindAll(bob => bob.Core.Show && bob.GetPlayerData().IsAlive);
                bobs.Sort(CompareBobs);

                if (bobs.Count == 0)
                    Finish();
                else
                {
                    // Kill the first bob in the list
                    Bob targetbob = bobs[0];
                    
                    Fireball.Explosion(targetbob.Core.Data.Position, targetbob.Core.MyLevel);
                    Fireball.ExplodeSound.Play();

                    targetbob.Core.Show = false;
                    targetbob.Dead = true;
                }
            }
        }
    }
}