namespace Cupcake
{
    using LeagueSharp;

    /// <summary>
    ///     A delicious treat most commonly served with icing on top.
    /// </summary>
    /// <seealso cref="ICupcakeIngredient" />
    public interface ICupcake : ICupcakeIngredient
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the prediction.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="range">The range.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="speed">The speed.</param>
        /// <param name="width">The width.</param>
        /// <param name="collision">if set to <c>true</c> the missile collides with minions, and should be treated as such.</param>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     <see cref="BakedCupcake" />
        /// </returns>
        BakedCupcake GetPrediction(
            Obj_AI_Base target, 
            float range, 
            float delay, 
            float speed, 
            float width, 
            bool collision, 
            Obj_AI_Hero source = null);

        #endregion
    }
}