namespace EatMyCass
{
    using LeagueSharp;

    /// <summary>
    ///     Unit Extensions
    /// </summary>
    public static class UnitExtensions
    {
        #region Public Methods and Operators


        /// <summary>
        /// Determines whether this target is poisoned.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if the target is poisoned; else <c>false</c></returns>
        public static bool IsPoisoned(this Obj_AI_Base target)
        {
            return target.HasBuffOfType(BuffType.Poison);
        }

        #endregion
    }
}