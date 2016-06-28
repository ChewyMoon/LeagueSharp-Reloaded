namespace MoonExperience
{
    using System.Reflection;
    using System.Threading.Tasks;

    using LeagueSharp.Common;

    /// <summary>
    ///     The configuration for this assembly.
    /// </summary>
    internal class Config : IModule
    {
        #region Public Properties

        /// <summary>
        ///     Gets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; private set; }

        #endregion

        #region Public Indexers

        /// <summary>
        ///     Gets the <see cref="System.Boolean" /> with the specified name.
        /// </summary>
        /// <value>
        ///     The <see cref="System.Boolean" />.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public bool this[string name] => this[name, false].IsActive();

        /// <summary>
        ///     Gets the <see cref="MenuItem" /> with the specified name.
        /// </summary>
        /// <value>
        ///     The <see cref="MenuItem" />.
        /// </value>
        /// <param name="name">The name.</param>
        /// <param name="champUnique">if set to <c>true</c> [champ unique].</param>
        /// <returns></returns>
        public MenuItem this[string name, bool champUnique = false] => this.Menu.Item(name, champUnique);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            this.Menu = new Menu("MoonExperience", "MoonExperience-ChewyMoon", true);

            var alerterMenu = new Menu("Alert Settings", "AlertSettings");
            alerterMenu.AddItem(new MenuItem("PingChampion", "Send a Ping (local)").SetValue(true));
            alerterMenu.AddItem(new MenuItem("DrawText", "Draw Text on Champion").SetValue(true));
            alerterMenu.AddItem(new MenuItem("DisplayNotification", "Show Notification").SetValue(true));
            alerterMenu.AddItem(new MenuItem("PrintText", "Print Text").SetValue(true));
            alerterMenu.AddItem(new MenuItem("Cooldown", "Notification Cooldown (s)").SetValue(new Slider(10, 0, 30)));
            alerterMenu.AddItem(new MenuItem("RangeCheck", "Alerter Range").SetValue(new Slider(1500, 0, 5000)));
            this.Menu.AddSubMenu(alerterMenu);

            var advancedMenu = new Menu("Advanced Settings", "AdvancedSettings");
            advancedMenu.AddItem(new MenuItem("MeleeMinionExp", "Melee Minion Experience").SetValue(new Slider(59)));
            advancedMenu.AddItem(new MenuItem("CasterMinionExp", "Caster Minion Experience").SetValue(new Slider(29)));
            advancedMenu.AddItem(new MenuItem("SiegeMinionExp", "Siege Minion Experience").SetValue(new Slider(92)));
            advancedMenu.AddItem(new MenuItem("SuperMinionExp", "Super Minion Experience").SetValue(new Slider(97)));
            this.Menu.AddSubMenu(advancedMenu);

            this.Menu.AddItem(new MenuItem("Seperator", string.Empty));
            this.Menu.AddItem(
                new MenuItem("ADVERTISEMENT-alF1ZXJ5", "Install GFUEL Assemblies").SetValue(false).DontSave());
            this.Menu.AddItem(new MenuItem("Seperator2", string.Empty));

            this.Menu.AddItem(
                new MenuItem("Info", "MoonExperience " + Assembly.GetExecutingAssembly().GetName().Version));
            this.Menu.AddItem(new MenuItem("Creator", "Made by ChewyMoon"));
            this.Menu.AddToMainMenu();

            this.Menu.Item("ADVERTISEMENT-alF1ZXJ5").ValueChanged += (sender, args) =>
                {
                    if (!args.GetNewValue<bool>())
                    {
                        return;
                    }

                    Task.Factory.StartNew(
                        () =>
                        AssemblyInstaller.InstallAssembly(
                            new[]
                                {
                                    "ps://profile/G-FUEL+Utility/554", 
                                    "ls://project/AlterEgojQuery/ElBundle/ElUtilitySuite/"
                                }));

                    args.Process = false;
                };
        }

        #endregion
    }
}