using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    /// <summary>
    /// Interface for specifying the physical extent of an object
    /// <\summary>
    public interface IBound
    {
        /// <summary>
        /// Returns the TR bound
        /// </summary>
        /// <returns></returns>
        Vector2 TR_Bound();

        /// <summary>
        /// Returns the BL bound
        /// </summary>
        /// <returns></returns>
        Vector2 BL_Bound();

        /// <summary>
        /// Moves the object to a position presumed to be within bounds
        /// </summary>
        /// <param name="pos">The amount to shift the current position</param>
        void MoveToBounded(Vector2 shift);
    }
}