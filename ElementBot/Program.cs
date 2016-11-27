namespace ElementBot
{
    using ElementBot.Core;

    using LeagueSharp.Common;

    class Program
    {
        #region Methods

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += new ElementBot().Start;
        }

        #endregion
    }
}