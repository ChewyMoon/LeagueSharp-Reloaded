namespace Cupcake
{
    using SharpDX;

    /// <summary>
    ///     A cupcake fresh from the oven!
    /// </summary>
    public class BakedCupcake
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the cast position.
        /// </summary>
        /// <value>
        ///     The cast position.
        /// </value>
        public Vector3 CastPosition { get; set; }

        /// <summary>
        ///     Gets or sets the hit chance.
        /// </summary>
        /// <value>
        ///     The hit chance.
        /// </value>
        public HitChance HitChance { get; set; }

        /// <summary>
        ///     Gets or sets the unit position.
        /// </summary>
        /// <value>
        ///     The unit position.
        /// </value>
        public Vector3 UnitPosition { get; set; }

        #endregion
    }
}