using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System;

using Microsoft.Xna.Framework;

using CoreEngine;
using CoreEngine.Random;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class GameObject : ObjectBase
    {
        /// <summary>
        /// Tries to add the game object to the game object's level's game.
        /// Returns true if failed.
        /// </summary>
        public bool AttemptToReAdd()
        {
            if (MyGame == null)
                if (CoreData.MyLevel.MyGame != null)
                    CoreData.MyLevel.MyGame.WaitThenDo(0, () =>
                        CoreData.MyLevel.MyGame.AddGameObject(this));

            return MyGame == null;
        }

        public enum Tag { RemoveOnLevelFinish, CharSelect };
        public Set<Tag> Tags = new Set<Tag>();

        /// <summary>
        /// If true the object is prevented from being released.
        /// </summary>
        public bool PreventRelease = false;

        public bool PauseOnPause = false;

        /// <summary>
        /// Whether to SoftPause the game
        /// </summary>
        public bool SoftPause
        {
            set
            {
                _SoftPause = value;

                AffectGameSoftPause();
            }

            get { return _SoftPause; }
        }
        bool _SoftPause;

        /// <summary>
        /// Whether to pause the game
        /// </summary>
        public bool PauseGame
        {
            set
            {
                _PauseGame = value;

                AffectGamePause();
            }

            get { return _PauseGame; }
        }
        bool _PauseGame;

        /// <summary>
        /// Whether to pause the level.
        /// </summary>
        public bool PauseLevel
        {
            set
            {
                _PauseLevel = value;

                AffectLevelPause();
            }

            get { return _PauseLevel; }
        }
        bool _PauseLevel;

        /// <summary>
        /// Communicates the GameObject's desire to SoftPause to the parent game.
        /// </summary>
        void AffectGameSoftPause()
        {
            if (MyGame == null) return;

            if (SoftPause)
                MyGame.SoftPause = true;
            else
                MyGame.UpdateSoftPause();
        }

        /// <summary>
        /// Communicates the GameObject's desire to pause to the parent game.
        /// </summary>
        void AffectGamePause()
        {
            if (MyGame == null) return;

            if (PauseGame)
                MyGame.PauseGame = true;
            else
                MyGame.UpdateGamePause();
        }

        /// <summary>
        /// Communicates the GameObject's desire to pause to the parent game.
        /// </summary>
        void AffectLevelPause()
        {
            if (MyGame == null) return;

            if (PauseLevel)
                MyGame.PauseLevel = true;
            else
                MyGame.UpdateLevelPause();
        }

        public void ForceRelease()
        {
            ReleaseBody();
        }

        public override void Release()
        {
            if (!PreventRelease && !CoreData.Released)
                ReleaseBody();
        }

        public event Action OnRelease;
        protected virtual void ReleaseBody()
        {
            base.Release();

            if (OnRelease != null)
                OnRelease();
            OnRelease = null;

            MyGame = null;

            CoreData.Active = false;
            CoreData.MarkedForDeletion = true;
        }

        /// <summary>
        /// The GameData this GameObject belongs to.
        /// </summary>
        public GameData MyGame;

        public override void MakeNew()
        {
            CoreData.Init();
            CoreData.DrawLayer = 10;

            CoreData.IsGameObject = true;

            CoreData.RemoveOnReset = false;
            CoreData.ResetOnlyOnReset = true;
        }

        public GameObject()
        {
            MakeNew();
        }

        public virtual void Init()
        {
        }

        public bool HideOnReplay = false;
        public bool PauseOnReplay = false;

        /// <summary>
        /// When true the object is automatically drawn with the level.
        /// Set to false if you are manually calling the ManualDraw function somewhere
        /// </summary>
        public bool AutoDraw = true;

        /// <summary>
        /// Explicitly calls the object's draw code.
        /// Should be used if manually drawing the object.
        /// </summary>
        public void ManualDraw()
        {
            MyDraw();
        }

        public override void Draw()
        {
            if (!AutoDraw) return;

            if (HideOnReplay && (CoreData.MyLevel.Replay || CoreData.MyLevel.Watching)) return;

            if (CoreData.Released) return;

            MyDraw();
        }

        protected virtual void MyDraw()
        {
        }

        public int SkipPhsx = 0;
        public sealed override void PhsxStep()
        {
            if (SkipPhsx > 0) { SkipPhsx--; return; }

            Level level = CoreData.MyLevel;

            if (PauseOnReplay && (level.Replay || level.Watching))
                return;

            MyPhsxStep();
        }

        protected virtual void MyPhsxStep()
        {
        }

        /// <summary>
        /// Called immediately after this GameObject is added to a GameData.
        /// </summary>
        public virtual void OnAdd()
        {
            AffectGamePause();

            if (CoreData.MyLevel != null)
                CoreData.MyLevel.OnCameraChange += this.OnCameraChange;
        }

        /// <summary>
        /// Called if the main camera of the level is changed
        /// </summary>
        public virtual void OnCameraChange()
        {
        }
    }
}