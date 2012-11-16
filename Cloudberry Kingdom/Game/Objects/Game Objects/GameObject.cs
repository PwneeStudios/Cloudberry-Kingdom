using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class GameObject : ObjectBase
    {
        public class AddGameObjectToCoreHelper : Lambda
        {
            GameObject obj;

            public AddGameObjectToCoreHelper(GameObject obj)
            {
                this.obj = obj;
            }

            public void Apply()
            {
                obj.Core.MyLevel.MyGame.AddGameObject(obj);
            }
        }

        /// <summary>
        /// Tries to add the game object to the game object's level's game.
        /// Returns true if failed.
        /// </summary>
        public bool AttemptToReAdd()
        {
            if (MyGame == null)
                if (Core.MyLevel.MyGame != null)
                    Core.MyLevel.MyGame.WaitThenDo(0, new AddGameObjectToCoreHelper(this));

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
            if (!PreventRelease && !Core.Released)
                ReleaseBody();
        }

        public Multicaster OnRelease = new Multicaster();
        protected virtual void ReleaseBody()
        {
            base.Release();

            if (OnRelease != null)
                OnRelease.Apply();
            OnRelease = null;

            MyGame = null;

            Core.Active = false;
            Core.MarkedForDeletion = true;
        }

        /// <summary>
        /// The GameData this GameObject belongs to.
        /// </summary>
        public GameData MyGame;

        public override void MakeNew()
        {
            Core.Init();
            Core.DrawLayer = 10;

            Core.IsGameObject = true;

            Core.RemoveOnReset = false;
            Core.ResetOnlyOnReset = true;
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

            if (HideOnReplay && (Core.MyLevel.Replay || Core.MyLevel.Watching)) return;

            if (Core.Released) return;

            MyDraw();
        }

        protected virtual void MyDraw()
        {
        }

        public int SkipPhsx = 0;
        public sealed override void PhsxStep()
        {
            if (SkipPhsx > 0) { SkipPhsx--; return; }

            Level level = Core.MyLevel;

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

            if (Core.MyLevel != null)
                Core.MyLevel.OnCameraChange.Add(new OnCameraChangeProxy(this));
        }


        class OnCameraChangeProxy : Lambda
        {
            GameObject go;

            public OnCameraChangeProxy(GameObject go)
            {
                this.go = go;
            }

            public void Apply()
            {
                go.OnCameraChange();
            }
        }

        /// <summary>
        /// Called if the main camera of the level is changed
        /// </summary>
        public virtual void OnCameraChange()
        {
        }
    }
}