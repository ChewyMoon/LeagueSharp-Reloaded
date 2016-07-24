namespace MoonExperience
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    /// <summary>
    ///     A simple experience tracker.
    /// </summary>
    internal class MoonExperience
    {
        #region Public Properties

        /// <summary>
        ///     Gets the caster minion experiece.
        /// </summary>
        /// <value>
        ///     The caster minion experiece.
        /// </value>
        public int CasterMinionExperiece => this.Config["CasterMinionExp", false].GetValue<Slider>().Value;

        /// <summary>
        ///     Gets or sets the configuration.
        /// </summary>
        /// <value>
        ///     The configuration.
        /// </value>
        public Config Config { get; set; }

        /// <summary>
        ///     Gets or sets the enemies nearby.
        /// </summary>
        /// <value>
        ///     The enemies nearby.
        /// </value>
        public Dictionary<int, int> EnemiesNearby { get; set; }

        /// <summary>
        ///     Gets or sets the experience per champ.
        /// </summary>
        /// <value>
        ///     The experience per champ.
        /// </value>
        public Dictionary<int, float> ExperiencePerChamp { get; set; }

        /// <summary>
        ///     Gets or sets the experience tracker.
        /// </summary>
        /// <value>
        ///     The experience tracker.
        /// </value>
        public ExperienceTracker ExperienceTracker { get; set; }

        /// <summary>
        ///     Gets or sets the last time notified.
        /// </summary>
        /// <value>
        ///     The last time notified.
        /// </value>
        public Dictionary<int, float> LastTimeNotified { get; set; }

        /// <summary>
        ///     Gets the melee minion experiece.
        /// </summary>
        /// <value>
        ///     The melee minion experiece.
        /// </value>
        public int MeleeMinionExperiece => this.Config["MeleeMinionExp", false].GetValue<Slider>().Value;

        /// <summary>
        ///     Gets the siege minion experiece.
        /// </summary>
        /// <value>
        ///     The siege minion experiece.
        /// </value>
        public int SiegeMinionExperiece => this.Config["SiegeMinionExp", false].GetValue<Slider>().Value;

        /// <summary>
        ///     Gets the supe minion experiece.
        /// </summary>
        /// <value>
        ///     The supe minion experiece.
        /// </value>
        public int SuperMinionExperiece => this.Config["SuperMinionExp", false].GetValue<Slider>().Value;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Raises the <see cref="E:Load" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnLoad(EventArgs args)
        {
            this.Config = new Config();
            this.Config.Load();

            this.ExperienceTracker = new ExperienceTracker();
            this.ExperienceTracker.Load();

            this.ExperiencePerChamp = new Dictionary<int, float>();
            this.LastTimeNotified = new Dictionary<int, float>();
            this.EnemiesNearby = new Dictionary<int, int>();

            for (var i = 2; i <= HeroManager.Enemies.Count; i++)
            {
                this.ExperiencePerChamp[i - 1] = 130.4f / i / 100;
            }

            HeroManager.Enemies.ForEach(
                x =>
                    {
                        this.LastTimeNotified[x.NetworkId] = 0;
                        this.EnemiesNearby[x.NetworkId] = 0;
                    });

            Game.OnUpdate += this.OnUpdate;
            Drawing.OnDraw += this.OnDraw;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the number of enemies nearby.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Number of enemies nearby.</returns>
        private int GetEnemiesNearby(ExperienceData data)
        {
            var xpDiff = data.NewExperience - data.OldExperience;

            return
                this.ExperiencePerChamp.Where(
                    x =>
                    Math.Abs(this.MeleeMinionExperiece * x.Value - xpDiff) < float.Epsilon
                    || Math.Abs(this.CasterMinionExperiece * x.Value - xpDiff) < float.Epsilon
                    || Math.Abs(this.SiegeMinionExperiece * x.Value - xpDiff) < float.Epsilon
                    || Math.Abs(this.SuperMinionExperiece * x.Value - xpDiff) < float.Epsilon)
                    .Select(xpPerChamp => xpPerChamp.Key)
                    .FirstOrDefault();
        }

        /// <summary>
        ///     Raises the <see cref="E:Draw" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnDraw(EventArgs args)
        {
            if (!this.Config["DrawText"])
            {
                return;
            }

            foreach (var hero in
                HeroManager.Enemies.Where(
                    x => x.IsValidTarget(this.Config["RangeCheck", false].GetValue<Slider>().Value)))
            {
                var enemiesNearby = this.EnemiesNearby[hero.NetworkId];

                if (enemiesNearby == 0)
                {
                    continue;
                }

                var text = $"{enemiesNearby} enemies nearby!";
                var size = Drawing.GetTextExtent(text);

                Vector2 pos;
                if (Drawing.WorldToScreen(hero.Position, out pos))
                {
                    Drawing.DrawText(pos.X - size.Width / 2f, pos.Y, Color.Red, text);
                }
            }

            foreach (var hero in HeroManager.Enemies)
            {
                var enemiesNearby = this.EnemiesNearby[hero.NetworkId];

                if (enemiesNearby == 0)
                {
                   // continue;
                }

                var text = $"{enemiesNearby} enemies nearby!";
                var size = Drawing.GetTextExtent(text);

                Vector2 pos;
                if (Drawing.WorldToScreen(hero.Position, out pos))
                {
                    Drawing.DrawText(pos.X - size.Width / 2f, pos.Y, Color.Red, text);
                }
            }
        }

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            foreach (
                var hero in
                    HeroManager.Enemies.Where(
                        x => x.IsValidTarget(this.Config["RangeCheck", false].GetValue<Slider>().Value)))
            {
                var enemiesNearby = this.GetEnemiesNearby(this.ExperienceTracker[hero.NetworkId]);

                if ((enemiesNearby == 0)
                    || (Game.Time - this.LastTimeNotified[hero.NetworkId]
                        < this.Config["Cooldown", false].GetValue<Slider>().Value
                        && this.EnemiesNearby[hero.NetworkId] == enemiesNearby))
                {
                    if (enemiesNearby == 0)
                    {
                        this.EnemiesNearby[hero.NetworkId] = 0;
                    }

                    continue;
                }

                if (hero.CountEnemiesInRange(this.Config["RangeCheck", false].GetValue<Slider>().Value) == enemiesNearby)
                {
                    this.EnemiesNearby[hero.NetworkId] = enemiesNearby;
                    continue;
                }

                if (this.Config["PingChampion"])
                {
                    for (var i = 0; i < enemiesNearby; i++)
                    {
                        Game.ShowPing(PingCategory.Fallback, hero, true);
                    }
                }

                if (this.Config["DisplayNotification"])
                {
                    Notifications.AddNotification($"{hero.ChampionName} has {enemiesNearby} heroes around!", 5);
                }

                if (this.Config["PrintText"])
                {
                    Game.PrintChat($"{hero.ChampionName} has {enemiesNearby} heroes around!");
                }

                this.EnemiesNearby[hero.NetworkId] = enemiesNearby;
            }
        }

        #endregion
    }
}