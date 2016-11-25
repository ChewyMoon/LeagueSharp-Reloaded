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
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public static Menu Menu { get; set; }

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
        public static float UpdateTime => (1 / Menu.Item("UpdateSpeed").GetValue<Slider>().Value) * 1000;

        #endregion

        #region Public Methods and Operators

        // ReSharper disable once UnusedParameter.Local
        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        /// <summary>
        /// Updates the event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="delegates">The delegates.</param>
        /// <param name="eventClass">The event class.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="localUpdateMethodName">Name of the local update method.</param>
        /// <param name="lastUpdateTime">The last update time.</param>
        /// <param name="updateTime">The update time.</param>
        /// <returns></returns>
        public static Tuple<List<Delegate>, float> UpdateEvent<T>(
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

                var updateHandlers = (List<T>)memberInfo.GetValue(null);
                if (updateHandlers == null)
                {
                    Console.WriteLine("Error trying to obain UpdateHandlers from the {0} class.", eventClass.FullName);
                    return null;
                }

                delegates = new List<Delegate>();
                delegates.AddRange(
                    updateHandlers.Cast<Delegate>().Where(
                        x => x.Method.Name != localUpdateMethodName
                            && x.Method.DeclaringType?.FullName != "LeagueSharp.Sandbox.LeagueSharpBootstrapper"));

                delegates.RemoveAll(x => x.Method.Name != localUpdateMethodName);

                return new Tuple<List<Delegate>, float>(delegates, lastUpdateTime);
            }

            if (Environment.TickCount - lastUpdateTime < updateTime)
            {
                return new Tuple<List<Delegate>, float>(delegates, lastUpdateTime);
            }

            delegates.ForEach(x => x.Method.Invoke(x.Target, new object[] { EventArgs.Empty }));
            return new Tuple<List<Delegate>, float>(delegates, Environment.TickCount);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        private static void CreateMenu()
        {
            Menu = new Menu("FPS Booster", "FPSBooster", true);
            Menu.AddItem(new MenuItem("Enabled", "Enabled").SetValue(true));
            Menu.AddItem(
                new MenuItem("UpdateSpeed", "Time to update per second").SetValue(new Slider(60, 1, 300))
                    .SetTooltip("This changes how often L# scripts will update and draw every second."));
            Menu.AddToMainMenu();

            Menu.Item("Enabled").ValueChanged +=
                (sender, args) => Game.PrintChat("Please press F5 to enable/disable FPSBooster.");
        }

        /// <summary>
        ///     The OnDraw event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void FpsBoosterDraw(EventArgs args)
        {
            var result = UpdateEvent<DrawingDraw>(
                OnDrawDelegates,
                typeof(Drawing),
                "DrawHandlers",
                nameof(FpsBoosterDraw),
                LastGameDrawTime,
                UpdateTime);

            if (result == null)
            {
                return;
            }

            OnDrawDelegates = result.Item1;
            LastGameDrawTime = result.Item2;
        }

        /// <summary>
        ///     The OnEndScene event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void FpsBoosterEndScene(EventArgs args)
        {
            var result = UpdateEvent<DrawingEndScene>(
                OnEndSceneDelegates,
                typeof(Drawing),
                "EndSceneHandlers",
                nameof(FpsBoosterEndScene),
                LastEndSceneTime,
                UpdateTime);

            if (result == null)
            {
                return;
            }

            OnEndSceneDelegates = result.Item1;
            LastEndSceneTime = result.Item2;
        }

        /// <summary>
        ///     The OnGameUpdate event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void FpsBoosterUpdate(EventArgs args)
        {
            var result = UpdateEvent<GameUpdate>(
                OnGameUpdateDelegates,
                typeof(Game),
                "UpdateHandlers",
                nameof(FpsBoosterUpdate),
                LastGameUpdateTime,
                UpdateTime);

            if (result == null)
            {
                return;
            }

            OnGameUpdateDelegates = result.Item1;
            LastGameUpdateTime = result.Item2;
        }

        /// <summary>
        ///     Raises the <see cref="E:GameLoad" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnGameLoad(EventArgs args)
        {
            CreateMenu();

            if (!Menu.Item("Enabled").GetValue<bool>())
            {
                return;
            }

            Utility.DelayAction.Add(
                1,
                () =>
                    {
                        Game.OnUpdate += FpsBoosterUpdate;
                        Drawing.OnDraw += FpsBoosterDraw;
                        Drawing.OnEndScene += FpsBoosterEndScene;
                    });
        }

        #endregion
    }
}