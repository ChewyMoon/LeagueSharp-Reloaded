namespace Cupcake
{
    using LeagueSharp;

    /// <summary>
    /// Uses cupcakes as bait. Not really, it just calculates the "bait level" of an enemy through statistical waypoint
    ///     analysis.
    /// </summary>
    /// <seealso cref="ICupcakeIngredient" />
    public interface ICupcakeBait : ICupcakeIngredient
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets the bait level.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns>The bait level.</returns>
        float GetBaitLevel(Obj_AI_Base unit);

        #endregion
    }
}