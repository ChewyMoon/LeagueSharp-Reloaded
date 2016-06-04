namespace Cupcake
{
    using LeagueSharp;

    using SharpDX;

    /// <summary>
    ///     The path data.
    /// </summary>
    public class PathData
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the end position.
        /// </summary>
        /// <value>
        ///     The end position.
        /// </value>
        public Vector3 EndPosition { get; set; }

        /// <summary>
        ///     Gets or sets the start position.
        /// </summary>
        /// <value>
        ///     The start position.
        /// </value>
        public Vector3 StartPosition { get; set; }

        /// <summary>
        ///     Gets or sets the time.
        /// </summary>
        /// <value>
        ///     The time.
        /// </value>
        public float Time { get; set; }

        /// <summary>
        ///     Gets or sets the unit.
        /// </summary>
        /// <value>
        ///     The unit.
        /// </value>
        public Obj_AI_Hero Unit { get; set; }

        #endregion
    }
}