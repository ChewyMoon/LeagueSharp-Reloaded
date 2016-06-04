namespace Cupcake
{
    /// <summary>
    ///     The chances of a skill shot hitting a target.
    /// </summary>
    public enum HitChance
    {
        /// <summary>
        ///     Impossible hit.
        /// </summary>
        Impossible = 0, 

        /// <summary>
        ///     The accuracy of the prediction is low.
        /// </summary>
        Low = 1, 

        /// <summary>
        ///     The accuracy of the prediction is medium.
        /// </summary>
        Medium = 2, 

        /// <summary>
        ///     The accuracy of the prediction is High. This is the highest accuracy.
        /// </summary>
        High = 3
    }
}