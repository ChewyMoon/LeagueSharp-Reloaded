namespace EatMyCass
{
    using System.Drawing;
    using System.Reflection;

    using LeagueSharp.Common;

    using Color = SharpDX.Color;

    /// <summary>
    ///     Assembly Configuration
    /// </summary>
    internal class Config
    {
        #region Static Fields

        /// <summary>
        ///     The instance
        /// </summary>
        private static Config instance;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static Config Instance => instance ?? (instance = new Config());

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        #endregion

        #region Public Indexers

        public bool this[string menuName] => this.Menu.Item(menuName).GetValue<bool>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        /// <returns>A <see cref="Menu" /> for the <see cref="Orbwalking.Orbwalker" />.</returns>
        public Menu Load()
        {
            this.Menu = new Menu("Eat My Cass", "EatMyCass-CM", true).SetFontStyle(
                FontStyle.Bold,
                new Color(0, 255, 255));

            var orbwalkerSettings = new Menu("Orbwalker Settings", "OrbwalkerSettings");
            this.Menu.AddSubMenu(orbwalkerSettings);

            var comboMenu = new Menu("Combo Settings", "ComboSettings");
            var blackListMenu = new Menu("Blacklisted Ult Champions", "BlackPeopleLOL");
            HeroManager.Enemies.ForEach(
                x => blackListMenu.AddItem(new MenuItem($"Blacklist{x.ChampionName}", x.ChampionName)).SetValue(false));
            comboMenu.AddSubMenu(blackListMenu);
            comboMenu.AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseWCombo", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseEPoisonCombo", "Use E Only if Poisoned").SetValue(false));
            comboMenu.AddItem(new MenuItem("UseRCombo", "Use R").SetValue(true));
            comboMenu.AddItem(
                new MenuItem("UseREnemyFacing", "Use R on X Enemies Facing").SetValue(
                    new Slider(3, 1, HeroManager.Enemies.Count)));
            comboMenu.AddItem(new MenuItem("UseRComboKillable", "Use R If Killable With Combo").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseRAboveEnemyHp", "Use R if Target Health Above").SetValue(new Slider(75)));
            this.Menu.AddSubMenu(comboMenu);

            var lastHitMenu = new Menu("Last Hit Settings", "LasthitSettings");
            lastHitMenu.AddItem(new MenuItem("LastHitE", "Last Hit with E").SetValue(true));
            lastHitMenu.AddItem(new MenuItem("LastHitMana", "Last Hit Mana").SetValue(new Slider(50)));
            this.Menu.AddSubMenu(lastHitMenu);

            var harassMenu = new Menu("Harass Settings", "HarassSettings");
            harassMenu.AddItem(new MenuItem("UseQHarass", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("UseWHarass", "Use W").SetValue(true));
            harassMenu.AddItem(new MenuItem("UseEHarass", "Use E").SetValue(true));
            harassMenu.AddItem(new MenuItem("UseEPoisonHarass", "Use E Only if Poisoned").SetValue(true));
            harassMenu.AddItem(new MenuItem("UseEFarmHarass", "Last hit with E while Harassing").SetValue(false));
            harassMenu.AddItem(new MenuItem("HarassMana", "Harass Mana").SetValue(new Slider(50)));
            this.Menu.AddSubMenu(harassMenu);

            var waveClearMenu = new Menu("WaveClear Settings", "WaveClearSettings");
            waveClearMenu.AddItem(new MenuItem("UseQWaveClear", "Use Q").SetValue(true));

            // waveClearMenu.AddItem(new MenuItem("UseWWaveClear", "Use W").SetValue(true));
            waveClearMenu.AddItem(new MenuItem("UseEWaveClear", "Use E").SetValue(true));

            // waveClearMenu.AddItem(new MenuItem("UseEPoisonWaveClear", "E Only Poisoned Minions").SetValue(true));
            waveClearMenu.AddItem(new MenuItem("WaveClearChamps", "Wave Clear only if no Champs").SetValue(true));
            waveClearMenu.AddItem(new MenuItem("WaveClearHarass", "Harass in Wave Clear").SetValue(true));
            waveClearMenu.AddItem(new MenuItem("WaveClearMana", "WaveClear Mana").SetValue(new Slider(65)));
            this.Menu.AddSubMenu(waveClearMenu);

            var ksMenu = new Menu("KillSteal Settings", "KillStealSettings");
            ksMenu.AddItem(new MenuItem("UseQKS", "Use Q").SetValue(true));
            ksMenu.AddItem(new MenuItem("UseWKS", "Use W").SetValue(true));
            ksMenu.AddItem(new MenuItem("UseEKS", "Use E").SetValue(true));
            ksMenu.AddItem(new MenuItem("UseRKS", "Use R").SetValue(false));
            this.Menu.AddSubMenu(ksMenu);

            var drawingSettings = new Menu("Drawing Settings", "DrawingSettings");
            drawingSettings.AddItem(new MenuItem("DrawQ", "Draw Q").SetValue(true));
            drawingSettings.AddItem(new MenuItem("DrawW", "Draw W").SetValue(false));
            drawingSettings.AddItem(new MenuItem("DrawE", "Draw E").SetValue(false));
            drawingSettings.AddItem(new MenuItem("DrawR", "Draw R").SetValue(false));
            this.Menu.AddSubMenu(drawingSettings);

            var miscMenu = new Menu("Miscellaneous Settings", "MiscSettings");
            miscMenu.AddItem(new MenuItem("AutoAttackCombo", "Auto Attack in Combo").SetValue(false));
            miscMenu.AddItem(new MenuItem("AutoAttackHarass", "Auto Attack in Harass").SetValue(false));

            // miscMenu.AddItem(new MenuItem("AutoAttackInERange", "Auto Attack in E Range").SetValue(true));
            miscMenu.AddItem(new MenuItem("CustomTargeting", "Advanced Targeting").SetValue(true));
            miscMenu.AddItem(new MenuItem("AutoWCC", "Auto W on CC'd Targets").SetValue(true));
            miscMenu.AddItem(new MenuItem("AntiGapcloser", "Enable Anti-Gapcloser").SetValue(true));
            miscMenu.AddItem(new MenuItem("Interrupter", "Interrupt with R").SetValue(true));
            miscMenu.AddItem(
                new MenuItem("DontQWIfTargetPoisoned", "Wait for Poison to Expire before Q/W").SetValue(true));
            miscMenu.AddItem(new MenuItem("StackTear", "Stack Tear").SetValue(true));
            this.Menu.AddSubMenu(miscMenu);

            this.Menu.AddItem(new MenuItem("Seperator", string.Empty));
            this.Menu.AddItem(
                    new MenuItem("Assembly", $"Eat My Cass version {Assembly.GetExecutingAssembly().GetName().Version}"))
                .SetFontStyle(FontStyle.Bold, new Color(0, 255, 255));
            this.Menu.AddItem(new MenuItem("Author", "By ChewyMoon <3"));
            this.Menu.AddToMainMenu();

            return orbwalkerSettings;
        }

        #endregion
    }
}