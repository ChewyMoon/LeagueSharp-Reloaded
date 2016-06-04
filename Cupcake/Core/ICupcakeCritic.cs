namespace Cupcake
{
    using System.Collections.Generic;

    using LeagueSharp;

    using SharpDX;

    /// <summary>
    /// Critics the cupcakes and makes sure the other cupcakes aren't touching each other!
    /// </summary>
    /// <seealso cref="ICupcakeIngredient" />
    public interface ICupcakeCritic : ICupcakeIngredient
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets the collisions.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="speed">The speed.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="width">The width.</param>
        /// <param name="minions">The minions.</param>
        /// <param name="startPosition">The start position.</param>
        /// <param name="endPosition">The end position.</param>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        /// <returns>
        /// The number of collisions.
        /// </returns>
        int GetCollisions(
            float range,
            float speed, 
            float delay, 
            float width, 
            IEnumerable<Obj_AI_Base> minions, 
            Vector3 startPosition, 
            Vector3 endPosition,
            Obj_AI_Base target,
            Obj_AI_Hero source);

        #endregion
    }
}