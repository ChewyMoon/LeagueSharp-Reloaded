namespace FPSBooster
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using LeagueSharp;
    using LeagueSharp.Common;

    class Program
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether {CC2D43FA-BBC4-448A-9D0B-7B57ADF2655C}[first update].
        /// </summary>
        /// <value>
        /// {D255958A-8513-4226-94B9-080D98F904A1}  <c>true</c> if [first update]; otherwise, <c>false</c>.
        /// </value>
        private static bool FirstUpdate { get; set; } = true;

        /// <summary>
        /// Gets or sets the last update time.
        /// </summary>
        /// <value>
        /// The last update time.
        /// </value>
        private static float LastUpdateTime { get; set; } = Environment.TickCount;

        /// <summary>
        /// Gets or sets the on update delegates.
        /// </summary>
        /// <value>
        /// The on update delegates.
        /// </value>
        private static List<Delegate> OnUpdateDelegates { get; set; }

        /// <summary>
        /// Gets the update time.
        /// </summary>
        /// <value>
        /// The update time.
        /// </value>
        private static float UpdateTime => (1 / 30f) * 1000;

        #endregion

        #region Methods

        /// <summary>
        ///     The OnGameUpdate event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void FpsBoosterUpdate(EventArgs args)
        {
            // Skip the first update to make sure all events are loaded in
            if (FirstUpdate)
            {
                FirstUpdate = false;
                return;
            }

            if (OnUpdateDelegates == null)
            {
                var memberInfo = typeof(Game).GetField("UpdateHandlers", BindingFlags.Static | BindingFlags.NonPublic);
                if (memberInfo == null)
                {
                    Console.WriteLine("Error trying to obtain MemberInfo from the Game class.");
                    return;
                }

                var updateHandlers = (List<GameUpdate>)memberInfo.GetValue(null);
                if (updateHandlers == null)
                {
                    Console.WriteLine("Error trying to obain UpdateHandlers from the Game class.");
                    return;
                }

                OnUpdateDelegates = new List<Delegate>();
                OnUpdateDelegates.AddRange(
                    updateHandlers.Where(
                        x =>
                            x.Method.Name != "FpsBoosterUpdate"
                            && x.Method.DeclaringType?.FullName != "LeagueSharp.Sandbox.LeagueSharpBootstrapper"));

                updateHandlers.RemoveAll(x => x.Method.Name != "FpsBoosterUpdate");

                return;
            }

            if (Environment.TickCount - LastUpdateTime < UpdateTime)
            {
                return;
            }

            OnUpdateDelegates.ForEach(x => x.Method.Invoke(x.Target, new object[] { EventArgs.Empty }));
            LastUpdateTime = Environment.TickCount;
        }

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        /// <summary>
        /// Raises the <see cref="E:GameLoad" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void OnGameLoad(EventArgs args)
        {
            Game.OnUpdate += FpsBoosterUpdate;
        }

        #endregion
    }
}