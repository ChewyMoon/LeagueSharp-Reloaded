namespace MoonExperience
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     Tracks hero's experience
    /// </summary>
    internal class ExperienceTracker : IModule
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExperienceTracker" /> class.
        /// </summary>
        public ExperienceTracker()
        {
            this.Experience = new Dictionary<int, ExperienceData>(5);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the experience.
        /// </summary>
        /// <value>
        ///     The experience.
        /// </value>
        public Dictionary<int, ExperienceData> Experience { get; set; }

        #endregion

        #region Public Indexers

        /// <summary>
        ///     Gets the <see cref="ExperienceData" /> with the specified networkid.
        /// </summary>
        /// <value>
        ///     The <see cref="ExperienceData" />.
        /// </value>
        /// <param name="networkId">The networkid.</param>
        /// <returns></returns>
        public ExperienceData this[int networkId] => this.Experience[networkId];

        /// <summary>
        ///     Gets the <see cref="ExperienceData" /> with for specified unit.
        /// </summary>
        /// <value>
        ///     The <see cref="ExperienceData" />.
        /// </value>
        /// <param name="unit">The unit.</param>
        /// <returns></returns>
        public ExperienceData this[Obj_AI_Base unit] => this[unit.NetworkId];

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            Game.OnUpdate += this.OnUpdate;

            foreach (var hero in HeroManager.Enemies)
            {
                this.Experience[hero.NetworkId] = new ExperienceData();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            foreach (var hero in HeroManager.Enemies)
            {
                var data = this.Experience[hero.NetworkId];

                data.OldExperience = data.NewExperience;
                data.NewExperience = hero.Experience;
            }
        }

        #endregion
    }

    /// <summary>
    ///     Experience Data
    /// </summary>
    internal class ExperienceData
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the new experience.
        /// </summary>
        /// <value>
        ///     The new experience.
        /// </value>
        public float NewExperience { get; set; }

        /// <summary>
        ///     Gets or sets the old experience.
        /// </summary>
        /// <value>
        ///     The old experience.
        /// </value>
        public float OldExperience { get; set; }

        #endregion
    }
}