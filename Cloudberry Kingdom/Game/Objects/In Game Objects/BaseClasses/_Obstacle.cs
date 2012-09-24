using System.Text;
using Microsoft.Xna.Framework;

using Drawing;

using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public abstract class _Obstacle : ObjectBase
    {
        public AutoGen AutoGenSingleton;

        /// <summary>
        /// This is the distance from the edge of the screen the obstacle must be before Physics updates are no longer active in-game.
        /// </summary>
        protected Vector2 PhsxCutoff_Playing = new Vector2(200, 200);

        /// <summary>
        /// This is the distance from the edge of the screen the obstacle must be before Physics updates are no longer active during level creation.
        /// </summary>
        protected Vector2 PhsxCutoff_BoxesOnly = new Vector2(-150, 200);

        /// <summary>
        /// This is the distance from the edge of the screen the obstacle must be before Physics updates are no longer active.
        /// </summary>
        protected Vector2 PhsxCutoff
        {
            get
            {
                return Core.BoxesOnly ? PhsxCutoff_BoxesOnly : PhsxCutoff_Playing;
            }
        }

        public abstract void Construct(bool BoxesOnly);

        public override void MakeNew()
        {
            base.MakeNew();

            Core.Init();
            Core.GenData.OverlapWidth = 60;

            Core.SkippedPhsx = true;
            Core.ContinuousEnabled = true;
        }

        public virtual void Die()
        {
        }

        public override void Reset(bool BoxesOnly)
        {
            Core.Active = true;

            Core.Data = Core.StartData;
        }

        public override void PhsxStep()
        {
            base.PhsxStep();

            Core.PosFromParentOffset();

            if (!Core.MyLevel.MainCamera.OnScreen(Pos, PhsxCutoff))
            {
                Core.SkippedPhsx = true;
                Core.WakeUpRequirements = true;
                return;
            }
            Core.SkippedPhsx = false;

            ActivePhsxStep();
        }

        public override void Draw()
        {
            if (Core.SkippedPhsx) return;

            if (Tools.DrawGraphics && Core.MyLevel.PlayMode == 0)
            {
                DrawGraphics();
            }

            if (Tools.DrawBoxes)
            {
                DrawBoxes();
            }
        }

        protected abstract void DrawGraphics();

        protected abstract void DrawBoxes();

        protected abstract void ActivePhsxStep();
    }
}
