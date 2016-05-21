namespace Snitched
{
    using System.Reflection;

    using LeagueSharp.Common;

    /// <summary>
    ///     Handles configuration settings.
    /// </summary>
    internal class Config
    {
        #region Static Fields

        /// <summary>
        ///     The instance
        /// </summary>
        private static Config instance;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="Config" /> class from being created.
        /// </summary>
        private Config()
        {
            this.Menu = new Menu("Snitched 3.0", "Snitched3", true);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static Config Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = new Config();
                return instance;
            }
        }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        #endregion

        #region Public Indexers

        /// <summary>
        ///     Gets the <see cref="MenuItem" /> with the specified name.
        /// </summary>
        /// <value>
        ///     The <see cref="MenuItem" />.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public MenuItem this[string name] => this.Menu.Item(name);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public void CreateMenu()
        {
            var buffMenu = new Menu("Buff Stealing Settings", "BuffStealSettings");
            AddSpellsToMenu(buffMenu, "BuffSteal");
            buffMenu.AddItem(new MenuItem("StealBlueBuff", "Steal Blue Buff").SetValue(true));
            buffMenu.AddItem(new MenuItem("StealRedBuff", "Steal Red Buff").SetValue(true));
            buffMenu.AddItem(new MenuItem("StealAllyBuffs", "Steal Ally Buffs").SetValue(false));
            this.Menu.AddSubMenu(buffMenu);

            var objectiveMenu = new Menu("Objective Stealing Settings", "ObjectiveStealSettings");
            AddSpellsToMenu(objectiveMenu, "ObjectiveSteal");
            objectiveMenu.AddItem(new MenuItem("StealBaron", "Steal Baron").SetValue(true));
            objectiveMenu.AddItem(new MenuItem("StealDragon", "Steal Dragon").SetValue(true));
            objectiveMenu.AddItem(
                new MenuItem("SmartObjectiveSteal", "Smart Objective Steal").SetValue(true)
                    .SetTooltip(
                        "Checks for allies the dragon/baron pit, and if there is none it will attempt to steal. Overrides keybind."));
            objectiveMenu.AddItem(
                new MenuItem("StealObjectiveKeyBind", "Steal Objectives").SetValue(new KeyBind(90, KeyBindType.Press)));
            objectiveMenu.Item("StealObjectiveKeyBind").Permashow();
            this.Menu.AddSubMenu(objectiveMenu);

            var ksMenu = new Menu("Kill Stealing Settings", "KillStealingSettings");
            var champMenu = new Menu("Champions", "Champions");
            HeroManager.Enemies.ForEach(
                x => champMenu.AddItem(new MenuItem("KS" + x.ChampionName, x.ChampionName).SetValue(true)));
            ksMenu.AddSubMenu(champMenu);
            AddSpellsToMenu(ksMenu, "KS");
            ksMenu.AddItem(new MenuItem("DontStealOnCombo", "Dont Steal if Combo'ing").SetValue(true));
            // TODO: More options?
            this.Menu.AddSubMenu(ksMenu);

            var miscMenu = new Menu("Miscellaneous Settings", "MiscSettings");
            miscMenu.AddItem(
                new MenuItem("ETALimit", "Missile Arrival Time Limit (MS)").SetValue(new Slider(3000, 0, 15000)));
            miscMenu.AddItem(new MenuItem("DistanceLimit", "Distance Limit").SetValue(new Slider(5000, 0, 15000)));
            miscMenu.AddItem(new MenuItem("StealFOW", "Steal in FOW").SetValue(false));
            miscMenu.AddItem(new MenuItem("DrawADPS", "Draw Average DPS").SetValue(true));
            this.Menu.AddSubMenu(miscMenu);

            this.Menu.AddItem(new MenuItem("Seperator1", " "));
            this.Menu.AddItem(new MenuItem("Info", "Snitched by ChewyMoon"));
            this.Menu.AddItem(new MenuItem("Info2", " Version: " + Assembly.GetExecutingAssembly().GetName().Version));

            this.Menu.AddToMainMenu();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds the spells to menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <param name="name">The name.</param>
        private static void AddSpellsToMenu(Menu rootMenu, string name)
        {
            var menu = rootMenu.AddSubMenu(new Menu("Spells", name + "Spells"));
            SpellLoader.GetUsableSpells()
                .ForEach(x => menu.AddItem(new MenuItem(name + x.Slot, "Use " + x.Slot)).SetValue(true).DontSave());
        }

        #endregion
    }
}