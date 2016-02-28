namespace Snitched
{
    using LeagueSharp.Common;

    internal class Program
    {
        #region Methods

        /// <summary>
        ///     The entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += new Snitched().Load;
        }

        #endregion
    }
}