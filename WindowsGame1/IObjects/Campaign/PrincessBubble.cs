using System.IO;
using Microsoft.Xna.Framework;

using Drawing;
using System;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class FrameAnimation
    {
        public int[] KeyFrames;
        public int[] KeyFrames_Length;
        public int[] KeyFramesTexture;
        public int Period;

        public void Init()
        {
            KeyFrames = new int[KeyFramesTexture.Length];
            KeyFrames[0] = 0;
            for (int i = 1; i < KeyFrames.Length; i++)
                KeyFrames[i] = KeyFrames[i - 1] + KeyFrames_Length[i - 1];
            Period = KeyFrames[KeyFrames.Length - 1];
        }

        public void SetTexture(int frame, ref int texture_frame)
        {
            for (int i = 0; i < KeyFrames.Length; i++)
                if (frame % Period == KeyFrames[i])
                    texture_frame = KeyFramesTexture[i];
        }
    }

    public partial class PrincessBubble : BerryBubble
    {
        public enum State { Float, Held, Fall, FloatUp, BlobHeld, None, Integrate };
        public State MyState = State.Float;

        static EzTexture[] Frames;

        public static FrameAnimation PoundingAnim = new FrameAnimation(), BossAnim = new FrameAnimation(), DeadAnim = new FrameAnimation();

        public FrameAnimation MyAnimation = PoundingAnim;

        static void InitFrames()
        {
            if (Frames == null)
            {
                PoundingAnim.KeyFrames_Length = new int[] { 23, 12, 10, 12 };
                PoundingAnim.KeyFramesTexture = new int[] { 0, 1, 0, 1, 0 };
                PoundingAnim.Init();

                BossAnim.KeyFrames_Length = new int[] { 25, 20, 25, 20 };
                BossAnim.KeyFramesTexture = new int[] { 2, 3, 2, 4, 2 };
                BossAnim.Init();

                DeadAnim.KeyFrames_Length = new int[] { 1 };
                DeadAnim.KeyFramesTexture = new int[] { 5, 5 };
                DeadAnim.Init();

                Frames = new EzTexture[] { Tools.Texture("princess_bubble_frame_1"), Tools.Texture("princess_bubble_frame_2"),
                    Tools.Texture("princess_boss_frame_1"), Tools.Texture("princess_boss_frame_2"), Tools.Texture("princess_boss_frame_3"), 
                    Tools.Texture("princess_boss_dead") };
            }
        }

        public PrincessBubble(Vector2 pos) : base(pos)
        {
            MyAnimation = PoundingAnim;
            AllowJumpOffOf = false;
        }

        public override void Release()
        {
            base.Release();
            MyBob = null;
        }

        public bool CanBePickedUp = true;
        public override void Interact(Bob bob)
        {
            if (MyState != State.Float) return;

            if (CanBePickedUp)
            {
                bool hold = Box.BoxOverlap(bob.Box2);
                if (hold)
                {
                    PickUp(bob);
                }
            }
        }

        public Action OnPickup;

        public Bob MyBob;
        public bool ShowWithMyBob = false;
        public void PickUp(Bob bob)
        {
            if (MyState == State.Held || MyState == State.BlobHeld) return;

            MyState = State.Held;
            MyBob = bob;

            bob.SetPlaceAnimData();
            bob.PlaceType = PlaceTypes.Princess;
            bob.HeldObject = this;
            this.Core.Held = true;
            this.Core.HeldOffset = new Vector2(0, 135 + 35);

            if (OnPickup != null) OnPickup(); OnPickup = null;
        }

        public void Drop()
        {
            if (MyBob == null) return;

            MyBob.UnsetPlaceAnimData();
            MyBob.HeldObject = null;
            MyBob = null;
            MyState = State.Float;
        }

        public void Fall()
        {
            MyState = State.Fall;

            if (MyBob != null)
                Core.Data.Velocity = .5f * (MyBob.Core.Data.Velocity + Core.Data.Velocity);
            else
                Core.Data.Velocity = Vector2.Zero;
        }

        public override void Reset(bool BoxesOnly)
        {
            base.Reset(BoxesOnly);

            if (MyState == State.Fall)
            {
                MyState = State.Held;
            }
        }

        int frame = 0;
        int texture_frame = 0;
        protected override void SetTexture()
        {
            base.SetTexture();
            if (Core.BoxesOnly) return;

            InitFrames();

            MyAnimation.SetTexture(frame, ref texture_frame);
            frame++;

            Tools.Write(texture_frame);
            Berry.Quad.MyTexture = Frames[texture_frame];
            //Berry.ScaleYToMatchRatio(263);
            Berry.ScaleYToMatchRatio(283);
            this.Core.HeldOffset = new Vector2(0, 135 + 53);

            //Camera.DisableOscillate = true;
            //Berry.ScaleYToMatchRatio(350);


            //Berry.TextureName = "princess";
            //Berry.Size = new Vector2(181.0184f, 187.6591f);
            //Berry.Angle = -152.4035f;

            Bubble.Pos = new Vector2(-23.14819f, -7.716309f);
            Bubble.Size = new Vector2(235.031f, 235.031f);
            Bubble.Angle = -2242.727f;
            Bubble.Show = false;
            Radius = 247;
        }


        /// <summary>
        /// If true the princess will oscillate as she floats.
        /// </summary>
        public bool Oscillate = true;

        public override void PhsxStep()
        {
            if (RotateSpeed != 0)
                Rotate(RotateSpeed);

            SetTexture();

            switch (MyState) {
                case State.None: break;

                case State.Integrate:
                    Core.Data.Integrate();
                    break;

                case State.Float:
                    if (Oscillate) base.PhsxStep();
                    break;
                
                case State.Held:
                    if (MyBob.Dead || MyBob.Dying)
                        Fall();
                    else
                        Core.Data.Velocity = MyBob.Core.Data.Velocity;
                    break;
                
                case State.Fall:
                    Core.Data.Velocity.Y -= Gravity;
                    Tools.Restrict(-45, 50, ref Core.Data.Velocity.Y);
                    Core.Data.Integrate();
                    break;

                case State.FloatUp:
                    Core.Data.Velocity.Y += Gravity * .0375f;
                    if (Core.Data.Velocity.Y > 2.8f) Core.Data.Velocity.Y = 2.8f;
                    Core.Data.Integrate();
                    break;
            }

            Box.Center = Core.Data.Position;
        }

        public override void Draw()
        {
            if (MyBob != null && ShowWithMyBob && !MyBob.Core.Show) return;

            base.Draw();
        }
    }
}