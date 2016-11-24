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
        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this is the first game update.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this is the first game update; otherwise, <c>false</c>.
        /// </value>
        public static bool FirstGameUpdate { get; set; } = true;

        /// <summary>
        ///     Gets or sets the last end scene time.
        /// </summary>
        /// <value>
        ///     The last end scene time.
        /// </value>
        public static float LastEndSceneTime { get; set; } = Environment.TickCount;

        /// <summary>
        ///     Gets or sets the last game draw time.
        /// </summary>
        /// <value>
        ///     The last game draw time.
        /// </value>
        public static float LastGameDrawTime { get; set; } = Environment.TickCount;

        /// <summary>
        ///     Gets or sets the last update time.
        /// </summary>
        /// <value>
        ///     The last update time.
        /// </value>
        public static float LastGameUpdateTime { get; set; } = Environment.TickCount;

        /// <summary>
        ///     Gets or sets the on draw delegates.
        /// </summary>
        /// <value>
        ///     The on draw delegates.
        /// </value>
        public static List<Delegate> OnDrawDelegates { get; set; }

        /// <summary>
        ///     Gets or sets the on end scene delegates.
        /// </summary>
        /// <value>
        ///     The on end scene delegates.
        /// </value>
        public static List<Delegate> OnEndSceneDelegates { get; set; }

        /// <summary>
        ///     Gets or sets the on update delegates.
        /// </summary>
        /// <value>
        ///     The on update delegates.
        /// </value>
        public static List<Delegate> OnGameUpdateDelegates { get; set; }

        /// <summary>
        ///     Gets the update time.
        /// </summary>
        /// <value>
        ///     The update time.
        /// </value>
        public static float UpdateTime => (1 / 30f) * 1000;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Updates the event.
        /// </summary>
        /// <param name="delegates">The delegates.</param>
        /// <param name="eventClass">The event class.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="localUpdateMethodName">Name of the local update method.</param>
        /// <param name="lastUpdateTime">The last update time.</param>
        /// <param name="updateTime">The update time.</param>
        /// <returns></returns>
        public static List<Delegate> UpdateEvent(
            List<Delegate> delegates,
            Type eventClass,
            string fieldName,
            string localUpdateMethodName,
            float lastUpdateTime,
            float updateTime)
        {
            if (delegates == null)
            {
                var memberInfo = eventClass.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
                if (memberInfo == null)
                {
                    Console.WriteLine("Error trying to obtain MemberInfo from the {0} class.", eventClass.FullName);
                    return null;
                }

                var updateHandlers = (List<Delegate>)memberInfo.GetValue(null);
                if (updateHandlers == null)
                {
                    Console.WriteLine("Error trying to obain UpdateHandlers from the {0} class.", eventClass.FullName);
                    return null;
                }

                delegates = new List<Delegate>();
                delegates.AddRange(
                    updateHandlers.Where(
                        x =>
                            x.Method.Name != localUpdateMethodName
                            && x.Method.DeclaringType?.FullName != "LeagueSharp.Sandbox.LeagueSharpBootstrapper"));

                delegates.RemoveAll(x => x.Method.Name != localUpdateMethodName);

                return delegates;
            }

            if (Environment.TickCount - lastUpdateTime < updateTime)
            {
                return delegates;
            }

            delegates.ForEach(x => x.Method.Invoke(x.Target, new object[] { EventArgs.Empty }));
            return delegates;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The OnDraw event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void FpsBoosterDraw(EventArgs args)
        {
            OnDrawDelegates = UpdateEvent(
                OnDrawDelegates,
                typeof(Drawing),
                "DrawHandlers",
                nameof(FpsBoosterDraw),
                LastGameDrawTime,
                UpdateTime);

            LastGameDrawTime = Environment.TickCount;
        }

        /// <summary>
        ///     The OnEndScene event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void FpsBoosterEndScene(EventArgs args)
        {
            OnEndSceneDelegates = UpdateEvent(
                OnEndSceneDelegates,
                typeof(Drawing),
                "EndSceneHandlers",
                nameof(FpsBoosterEndScene),
                LastEndSceneTime,
                UpdateTime);

            LastEndSceneTime = Environment.TickCount;
        }

        /// <summary>
        ///     The OnGameUpdate event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void FpsBoosterUpdate(EventArgs args)
        {
            // Skip the first update to make sure all events are loaded in
            if (FirstGameUpdate)
            {
                FirstGameUpdate = false;
                return;
            }

            OnGameUpdateDelegates = UpdateEvent(
                OnGameUpdateDelegates,
                typeof(Game),
                "UpdateHandlers",
                nameof(FpsBoosterUpdate),
                LastGameUpdateTime,
                UpdateTime);

            LastGameUpdateTime = Environment.TickCount;
        }

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        /// <summary>
        ///     Raises the <see cref="E:GameLoad" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnGameLoad(EventArgs args)
        {
            Game.OnUpdate += FpsBoosterUpdate;
            Drawing.OnDraw += FpsBoosterDraw;
            Drawing.OnEndScene += FpsBoosterEndScene;
        }

        #endregion
    }
}