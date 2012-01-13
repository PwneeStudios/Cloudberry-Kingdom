using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Set<T> : IEnumerable
    {
        public Dictionary<T, bool> dict = new Dictionary<T,bool>();

        public int Count { get { return dict.Count; } }

        public bool this[T item]
        {
            get { return dict.ContainsKey(item); }
        }

        public static Set<T> operator+(Set<T> set, T item)
        {
            if (!set.dict.ContainsKey(item))
                set.dict.Add(item, true);

            return set;
        }

        IEnumerator IEnumerable.GetEnumerator() { return dict.Keys.GetEnumerator(); }

        public T Choose()
        {
            int i = Tools.RndInt(0, dict.Count - 1);
            return dict.ElementAt(i).Key;
        }

        public bool Contains(T item)
        {
            return dict.ContainsKey(item);
        }

    }

    public class GameObject : IObject
    {
        /// <summary>
        /// Tries to add the game object to the game object's level's game.
        /// Returns true if failed.
        /// </summary>
        public bool AttemptToReAdd()
        {
            if (MyGame == null)
                if (Core.MyLevel.MyGame != null)
                    Core.MyLevel.MyGame.WaitThenDo(0, () =>
                        Core.MyLevel.MyGame.AddGameObject(this));

            return MyGame == null;
        }

        public void TextDraw() { }

        public enum Tag { RemoveOnLevelFinish };
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

        public void Release()
        {
            if (!PreventRelease && !Core.Released)
                ReleaseBody();
        }

        public event Action OnRelease;
        protected virtual void ReleaseBody()
        {
            if (OnRelease != null)
                OnRelease();
            OnRelease = null;

            MyGame = null;

            Core.Active = false;

            Core.Release();

            Core.MarkedForDeletion = true;
        }

        ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        /// <summary>
        /// The GameData this GameObject belongs to.
        /// </summary>
        public GameData MyGame;

        public void MakeNew()
        {
            Core.Init();
            Core.DrawLayer = 10;

            Core.IsGameObject = true;

            Core.RemoveOnReset = false;
            Core.ResetOnlyOnReset = true;
        }

        public GameObject()
        {
            CoreData = new ObjectData();

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

        public void Draw()
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
        public void PhsxStep()
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
                Core.MyLevel.OnCameraChange += this.OnCameraChange;
        }

        /// <summary>
        /// Called if the main camera of the level is changed
        /// </summary>
        public virtual void OnCameraChange()
        {
        }

        public void PhsxStep2() { }
        public virtual void Reset(bool BoxesOnly) { }
        public void Clone(IObject A) { }
        public void Interact(Bob bob) { }
        public virtual void Move(Vector2 shift) { }
        public void Write(BinaryWriter writer)
        {
            Core.Write(writer);
        }
        public void Read(BinaryReader reader) { Core.Read(reader); }
//StubStubStubStart
public void OnUsed() { }
public void OnMarkedForDeletion() { }
public void OnAttachedToBlock() { }
public bool PermissionToUse() { return true; }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public void Smash(Bob bob) { }
//StubStubStubEnd6
    }
}