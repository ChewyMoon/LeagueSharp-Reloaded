namespace Snitched
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.SDK;

    using Damage = LeagueSharp.SDK.Core.Wrappers.Damages.Damage;
    using Geometry = LeagueSharp.Common.Geometry;
    using Spell = LeagueSharp.Common.Spell;

    internal class Snitched
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        private List<Spell> Spells { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Loads Snitched.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void Load(EventArgs args)
        {
            Variables.Orbwalker.Enabled = false;

            Config.Instance.CreateMenu();

            HealthPrediction.Load();

            ObjectTracker.OnObjectiveCreated += (sender, type) => HealthPrediction.TrackObject((Obj_AI_Base)sender);
            ObjectTracker.OnObjectiveDead += (sender, type) => HealthPrediction.UntrackObject((Obj_AI_Base)sender);
            ObjectTracker.Load();

            this.Spells = SpellLoader.GetUsableSpells();

            Game.OnUpdate += this.Game_OnUpdate;

            Game.PrintChat(
                "<b><font color=\"#00AAFF\">S</font><font color=\"#f06DBD\">nitched</font> <font color=\"#00AAFF\">3.0</font></b> "
                + "by <font color=\"#9400D3\"><b>ChewyMoon</b></font> Loaded!");
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Game_OnUpdate(EventArgs args)
        {
            if (ObjectTracker.Baron != null)
            {
                this.HandleObjective(ObjectTracker.Baron, ObjectiveType.Baron);
            }

            if (ObjectTracker.Dragon != null)
            {
                this.HandleObjective(ObjectTracker.Dragon, ObjectiveType.Dragon);
            }

            if (ObjectTracker.BlueBuffs.Any())
            {
                ObjectTracker.BlueBuffs.ForEach(x => this.HandleBuff(x, ObjectiveType.Blue));
            }

            if (ObjectTracker.RedBuffs.Any())
            {
                ObjectTracker.RedBuffs.ForEach(x => this.HandleBuff(x, ObjectiveType.Red));
            }

            this.StealKills();
        }

        /// <summary>
        ///     Handles the buff.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="type">The type.</param>
        private void HandleBuff(Obj_AI_Base unit, ObjectiveType type)
        {
            if (!Config.Instance["Steal" + type + "Buff"].IsActive())
            {
                return;
            }

            var alliesinRange = unit.CountAlliesInRange(1000);

            if (!Config.Instance["StealAllyBuffs"].IsActive() && alliesinRange > 0)
            {
                return;
            }

            this.StealObject(unit, StealType.BuffSteal);
        }

        /// <summary>
        ///     Handles the objective.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="type">The type.</param>
        private void HandleObjective(Obj_AI_Base unit, ObjectiveType type)
        {
            if ((Config.Instance["SmartObjectiveSteal"].IsActive()
                 && !Config.Instance["StealObjectiveKeyBind"].IsActive() && unit.CountAlliesInRange(500) != 0)
                || !Config.Instance["StealObjectiveKeyBind"].IsActive())
            {
                return;

            }

            if (!Config.Instance["Steal" + type].IsActive())
            {
                return;
            }

            this.StealObject(unit, StealType.ObjectiveSteal);
        }

        /// <summary>
        ///     Steals the kills.
        /// </summary>
        private void StealKills()
        {
            if (Config.Instance["DontStealOnCombo"].IsActive()
                && Orbwalking.Orbwalker.Instances.Any(x => x.ActiveMode == Orbwalking.OrbwalkingMode.Combo))
            {
                return;
            }

            foreach (var enemy in
                HeroManager.Enemies.Where(
                    x =>
                    Utility.IsValidTarget(x, Config.Instance["DistanceLimit"].GetValue<Slider>().Value)
                    && Config.Instance["KS" + x.ChampionName].IsActive()))
            {
                var spell =
                    EnumerableExtensions.MinOrDefault(
                        this.Spells.Where(
                            x =>
                            Damage.GetSpellDamage(ObjectManager.Player, enemy, x.Slot) > enemy.Health
                            && x.IsInRange(enemy) && Config.Instance["KillSteal" + x.Slot].IsActive()
                            && x.GetMissileArrivalTime(enemy)
                            < Config.Instance["ETALimit"].GetValue<Slider>().Value / 1000f),
                        x => x.GetDamage(enemy));

                if (spell != null)
                {
                    spell.Cast(enemy);
                }
            }
        }

        /// <summary>
        ///     Steals the object.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="type">The type.</param>
        private void StealObject(Obj_AI_Base unit, StealType type)
        {
            if (Geometry.Distance(ObjectManager.Player, unit)
                > Config.Instance["DistanceLimit"].GetValue<Slider>().Value)
            {
                return;
            }

            if (!unit.IsVisible && !Config.Instance["StealFOW"].IsActive())
            {
                return;
            }

            var spell =
                EnumerableExtensions.MaxOrDefault(
                    this.Spells.Where(
                        x =>
                        x.IsReady() && x.IsInRange(unit) && Config.Instance[type.ToString() + x.Slot].IsActive()
                        && x.GetMissileArrivalTime(unit) < Config.Instance["ETALimit"].GetValue<Slider>().Value / 1000f),
                    x => Damage.GetSpellDamage(ObjectManager.Player, unit, x.Slot));

            if (spell == null)
            {
                return;
            }

            var healthPred = HealthPrediction.GetPredictedHealth(unit, spell.GetMissileArrivalTime(unit));

            if (spell.GetDamage(unit) >= healthPred)
            {
                spell.Cast(unit);
            }
        }

        #endregion
    }

    /// <summary>
    ///     The type of stealing. Used to check menu values.
    /// </summary>
    internal enum StealType
    {
        /// <summary>
        ///     The objective steal
        /// </summary>
        ObjectiveSteal,

        /// <summary>
        ///     The kill steal
        /// </summary>
        KillSteal,

        /// <summary>
        ///     The buff steal
        /// </summary>
        BuffSteal
    }
}