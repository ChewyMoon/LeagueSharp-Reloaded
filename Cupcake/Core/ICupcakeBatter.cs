namespace Cupcake
{
    using LeagueSharp;

    using SharpDX;

    /// <summary>
    /// The cupcake batter. Helps predict the targets position.
    /// </summary>
    /// <seealso cref="ICupcakeIngredient" />
    public interface ICupcakeBatter : ICupcakeIngredient
    {
        #region Public Methods and Operators

        /// <summary>
        /// Predicts the position.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="speed">The speed.</param>
        /// <param name="source">The source.</param>
        /// <returns>The predicted <see cref="Vector3"/></returns>
        Vector3 PredictPosition(Obj_AI_Base target, float delay, float speed, Obj_AI_Hero source = null);

        #endregion
    }
}