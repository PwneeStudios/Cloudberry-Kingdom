using System.Text;

using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;

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
                return CoreData.BoxesOnly ? PhsxCutoff_BoxesOnly : PhsxCutoff_Playing;
            }
        }

        public abstract void Construct(bool BoxesOnly);

        public override void MakeNew()
        {
            base.MakeNew();

            CoreData.Init();
            CoreData.GenData.OverlapWidth = 60;

            CoreData.SkippedPhsx = true;
            CoreData.ContinuousEnabled = true;
        }

        public virtual void Die()
        {
        }

        public override void Reset(bool BoxesOnly)
        {
            CoreData.Active = true;

            CoreData.Data = CoreData.StartData;
        }

        public override void PhsxStep()
        {
            base.PhsxStep();

            CoreData.PosFromParentOffset();

            if (!CoreData.MyLevel.MainCamera.OnScreen(CoreData.Data.Position, PhsxCutoff))
            {
                CoreData.SkippedPhsx = true;
                CoreData.WakeUpRequirements = true;
                return;
            }
            CoreData.SkippedPhsx = false;

            ActivePhsxStep();
        }

        public override void Draw()
        {
            if (CoreData.SkippedPhsx) return;

            if (Tools.DrawGraphics && CoreData.MyLevel.PlayMode == 0)
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
