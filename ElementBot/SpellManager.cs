namespace ElementBot
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.Data;
    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.Data.Enumerations;

    class SpellManager
    {
        #region Public Properties

        /// <summary>
        /// Gets the e.
        /// </summary>
        /// <value>
        /// The e.
        /// </value>
        public SpellDatabaseEntry E => this.GetSpell(SpellSlot.E);

        /// <summary>
        /// Gets the q.
        /// </summary>
        /// <value>
        /// The q.
        /// </value>
        public SpellDatabaseEntry Q => this.GetSpell(SpellSlot.Q);

        /// <summary>
        /// Gets the r.
        /// </summary>
        /// <value>
        /// The r.
        /// </value>
        public SpellDatabaseEntry R => this.GetSpell(SpellSlot.R);

        /// <summary>
        /// Gets the w.
        /// </summary>
        /// <value>
        /// The w.
        /// </value>
        public SpellDatabaseEntry W => this.GetSpell(SpellSlot.W);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the spell database.
        /// </summary>
        /// <value>
        /// The spell database.
        /// </value>
        private SpellDatabase SpellDatabase { get; } = Data.Get<SpellDatabase>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Uses the spell.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="mode">The mode.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public void UseSpell(SpellDatabaseEntry entry, Orbwalking.OrbwalkingMode mode)
        {
            var spell = entry.GetSpell();
            var castType = entry.CastType.First();

            // Disable activated abilites
            if (mode == Orbwalking.OrbwalkingMode.None)
            {
                if (castType == CastType.Activate || castType == CastType.Toggle)
                {
                    entry.GetSpell().Cast();
                }

                return;
            }

            switch (castType)
            {
                case CastType.EnemyChampions:
                    if (mode == Orbwalking.OrbwalkingMode.Combo)
                    {
                        spell.Cast(TargetSelector.GetTarget(spell.Range, TargetSelector.DamageType.True));
                    }

                    break;
                case CastType.EnemyMinions:
                    if (mode == Orbwalking.OrbwalkingMode.LaneClear)
                    {
                        var position =
                            MinionManager.GetBestLineFarmLocation(
                                MinionManager.GetMinions(spell.Range).Select(x => x.ServerPosition.To2D()).ToList(),
                                spell.Width,
                                spell.Range);

                        if (position.MinionsHit > 2)
                        {
                            spell.Cast(position.Position);
                        }
                    }
                    break;
                case CastType.EnemyTurrets:
                    spell.Cast(
                        ObjectManager.Get<Obj_AI_Turret>()
                            .Where(x => x.IsEnemy && !x.IsDead && x.IsValid)
                            .OrderBy(x => x.Distance(ObjectManager.Player))
                            .FirstOrDefault());
                    break;
                case CastType.AllyChampions:
                    spell.Cast(
                        HeroManager.Allies.Where(x => x.IsValidTarget(spell.Range, false))
                            .OrderBy(x => x.Distance(ObjectManager.Player))
                            .FirstOrDefault());
                    break;
                case CastType.AllyMinions:
                    spell.Cast(MinionManager.GetMinions(spell.Range, MinionTypes.All, MinionTeam.Ally).FirstOrDefault());
                    break;
                case CastType.AllyTurrets:
                    spell.Cast(
                        ObjectManager.Get<Obj_AI_Turret>()
                            .Where(x => x.IsAlly && !x.IsDead && x.IsValid)
                            .OrderBy(x => x.Distance(ObjectManager.Player))
                            .FirstOrDefault());
                    break;
                case CastType.HeroPets:
                    // todo REKSAI
                    break;
                case CastType.Position:
                    spell.Cast(TargetSelector.GetTarget(spell.Range, TargetSelector.DamageType.True));
                    break;
                case CastType.Direction:
                    spell.Cast(TargetSelector.GetTarget(spell.Range, TargetSelector.DamageType.True));
                    break;
                case CastType.Self:
                    spell.Cast();
                    break;
                case CastType.Charging:
                    spell.Cast();
                    break;
                case CastType.Toggle:
                    spell.Cast();
                    break;
                case CastType.Activate:
                    spell.Cast();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Uses the spells.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="combo">if set to <c>true</c> [combo].</param>
        public void UseSpells(Orbwalking.OrbwalkingMode mode, bool combo = false)
        {
            this.UseSpell(this.Q, mode);
            this.UseSpell(this.W, mode);
            this.UseSpell(this.E, mode);

            if (combo)
            {
                this.UseSpell(this.R, mode);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the spell.
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <returns></returns>
        private SpellDatabaseEntry GetSpell(SpellSlot slot)
        {
            return
                this.SpellDatabase.Get(
                    x =>
                        x.Slot == slot && x.ChampionName == ObjectManager.Player.ChampionName
                        && !x.CastType.Any(y => y == CastType.ImpossibleToCast)).FirstOrDefault();
        }

        #endregion
    }

    internal static class SpellManagerExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the spell.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public static Spell GetSpell(this SpellDatabaseEntry entry)
        {
            var spell = new Spell(entry.Slot);
            spell.SetSkillshot(spell.Delay / 1000, spell.Width, spell.Speed, spell.Collision, spell.Type);

            return spell;
        }

        #endregion
    }
}