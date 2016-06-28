namespace MoonExperience
{
    using LeagueSharp.Common;

    /// <summary>
    ///     The program.
    /// </summary>
    internal class Program
    {
        #region Methods

        /// <summary>
        ///     Called when the application is started.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += new MoonExperience().OnLoad;
        }

        #endregion
    }
}