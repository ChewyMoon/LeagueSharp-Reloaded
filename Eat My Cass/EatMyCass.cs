namespace EatMyCass
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    /// EAT MY ASS
    /// </summary>
    internal class EatMyCass
    {
        #region Properties

        /// <summary>
        ///     The stun buff types
        /// </summary>
        private static IEnumerable<BuffType> StunBuffTypes { get; } = new List<BuffType>
                                                                          {
                                                                              BuffType.Knockup, BuffType.Snare, 
                                                                              BuffType.Stun, BuffType.Suppression
                                                                          };

        /// <summary>
        ///     Gets or sets the e.
        /// </summary>
        /// <value>
        ///     The e.
        /// </value>
        private Spell E { get; set; }

        /// <summary>
        ///     Gets the minimum w range.
        /// </summary>
        /// <value>
        ///     The minimum w range.
        /// </value>
        private int MinimumWRange => 500;

        /// <summary>
        ///     Gets or sets the orbwalker.
        /// </summary>
        /// <value>
        ///     The orbwalker.
        /// </value>
        private Orbwalking.Orbwalker Orbwalker { get; set; }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private Obj_AI_Hero Player => ObjectManager.Player;

        /// <summary>
        /// </summary>
        /// <value>
        ///     The q.
        /// </value>
        private Spell Q { get; set; }

        /// <summary>
        ///     Gets or sets the E
        /// </summary>
        /// <value>
        ///     The r.
        /// </value>
        private Spell R { get; set; }

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        private List<Spell> Spells { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     The w.
        /// </value>
        private Spell W { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the target.
        /// </summary>
        /// <returns></returns>
        public Obj_AI_Hero GetTarget()
        {
            return Config.Instance["CustomTargeting"]
                       ? HeroManager.Enemies.Where(x => x.IsValidTarget(this.Q.Range) && x.IsPoisoned())
                             .OrderByDescending(
                                 x =>
                                 this.Player.CalcDamage(x, Damage.DamageType.Magical, 100) / (1 + x.Health)
                                 * TargetSelector.GetPriority(x))
                             .FirstOrDefault()
                         ?? TargetSelector.GetTarget(this.Q.Range, TargetSelector.DamageType.Magical)
                       : TargetSelector.GetTarget(this.Q.Range, TargetSelector.DamageType.Magical);
        }

        /// <summary>
        ///     Raises the <see cref="E:Load" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnLoad(EventArgs args)
        {
            this.Orbwalker = new Orbwalking.Orbwalker(Config.Instance.Load());

            this.Q = new Spell(SpellSlot.Q, 850);
            this.W = new Spell(SpellSlot.W, 900);
            this.E = new Spell(SpellSlot.E, 700);
            this.R = new Spell(SpellSlot.R, 825);

            this.Q.SetSkillshot(0.7f, 75, float.MaxValue, false, SkillshotType.SkillshotCircle);
            this.W.SetSkillshot(0.75f, 160, 1000, false, SkillshotType.SkillshotCircle);
            this.E.SetTargetted(0.125f, 1000);
            this.R.SetSkillshot(0.5f, (float)(80 * Math.PI / 180), 3200, false, SkillshotType.SkillshotCone);

            this.Spells = new List<Spell> { this.Q, this.W, this.E, this.R };

            Game.OnUpdate += this.OnUpdate;
            Obj_AI_Base.OnBuffAdd += this.OnBuffAdd;
            AntiGapcloser.OnEnemyGapcloser += this.OnEnemyGapcloser;
            Orbwalking.OnNonKillableMinion += this.OnNonKillableMinion;
            Orbwalking.BeforeAttack += this.OnBeforeAttack;
            Interrupter2.OnInterruptableTarget += this.OnInterruptableTarget;
            Drawing.OnDraw += this.OnDraw;

            Game.PrintChat("<font color='#00FFFF'>Eat My Cass:</font> <font color='#FFFFFF'>Loaded!</font>");
        }

        /// <summary>
        /// Raises the <see cref="E:BeforeAttack" /> event.
        /// </summary>
        /// <param name="args">The <see cref="Orbwalking.BeforeAttackEventArgs"/> instance containing the event data.</param>
        private void OnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var mode = this.Orbwalker.ActiveMode;
            var aaCombo = Config.Instance["AutoAttackCombo"];
            var aaHarass = Config.Instance["AutoAttackHarass"];

            if (mode == Orbwalking.OrbwalkingMode.Combo && args.Target.Type == GameObjectType.obj_AI_Hero)
            {
                args.Process = aaCombo;
            }

            if (mode == Orbwalking.OrbwalkingMode.Mixed && args.Target.Type == GameObjectType.obj_AI_Hero)
            {
                args.Process = aaHarass;
            }   
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Executes the combo.
        /// </summary>
        private void ExecuteCombo()
        {
            var useQ = Config.Instance["UseQCombo"];
            var useW = Config.Instance["UseWCombo"];
            var useE = Config.Instance["UseECombo"];
            var useR = Config.Instance["UseRCombo"];
            var useREnemyFacing = Config.Instance.Menu.Item("UseREnemyFacing").GetValue<Slider>().Value;
            var useRComboKillable = Config.Instance["UseRComboKillable"];
            var useRAboveEnemyHp = Config.Instance.Menu.Item("UseRAboveEnemyHp").GetValue<Slider>().Value;
            var useEPoisonCombo = Config.Instance["UseEPoisonCombo"];

            var target = this.GetTarget();

            if (target == null)
            {
                return;
            }

            if (useR && this.R.IsReady()
                && ((HeroManager.Enemies.Count(x => x.IsValidTarget(this.R.Range) && x.IsFacing(this.Player))
                     >= useREnemyFacing)
                    || (useRComboKillable
                        && this.Player.GetComboDamage(target, this.Spells.Select(x => x.Slot)) > target.Health
                        && this.Player.GetComboDamage(
                            target, 
                            this.Spells.Where(x => x.Slot != SpellSlot.R).Select(x => x.Slot)) < target.Health)
                    || (target.HealthPercent > useRAboveEnemyHp)) && target.IsFacing(this.Player)
                && !Config.Instance[$"Blacklist{target.ChampionName}"])
            {
                this.R.Cast(target, aoe: true);
            }

            if (useW && this.W.IsReady() && target.Distance(this.Player) >= this.MinimumWRange)
            {
                this.W.Cast(target);
            }

            if (useQ && this.Q.IsReady())
            {
                this.Q.Cast(target);
            }

            if (!useE || !this.E.IsReady() || !useEPoisonCombo || target.IsPoisoned())
            {
                this.E.CastOnUnit(target);
            }
        }

        /// <summary>
        ///     Executes the harass.
        /// </summary>
        private void ExecuteHarass()
        {
            var useQ = Config.Instance["UseQHarass"];
            var useW = Config.Instance["UseWHarass"];
            var useE = Config.Instance["UseEHarass"];
            var useEPoisonHarass = Config.Instance["UseEPoisonHarass"];
            var useEFarm = Config.Instance["UseEFarmHarass"];
            var harassMana = Config.Instance.Menu.Item("HarassMana").GetValue<Slider>().Value;

            var target = this.GetTarget();
            var minionE = MinionManager.GetMinions(this.E.Range)
                .FirstOrDefault(x => this.E.GetDamage(x) > x.Health + 10);

            if (target == null)
            {
                if (useEFarm && this.E.IsReady() && minionE != null
                    && this.Player.GetAutoAttackDamage(minionE) < this.E.GetDamage(minionE))
                {
                    this.E.CastOnUnit(minionE);
                }

                return;
            }

            if (this.Player.ManaPercent < harassMana)
            {
                return;
            }

            if (useQ && this.Q.IsReady())
            {
                this.Q.Cast(target);
            }

            if (useW && this.W.IsReady() && target.Distance(this.Player) >= this.MinimumWRange)
            {
                this.W.Cast(target);
            }

            if (!useE || !this.E.IsReady() || !useEPoisonHarass || target.IsPoisoned())
            {
                this.E.CastOnUnit(target);
            }
            else if (this.E.IsReady() && minionE != null)
            {
                this.E.CastOnUnit(minionE);
            }
        }

        /// <summary>
        ///     Executes the wave clear.
        /// </summary>
        private void ExecuteWaveClear()
        {
            var useQ = Config.Instance["UseQWaveClear"];
            var useE = Config.Instance["UseEWaveClear"];
            var farmOnlyNoChamps = Config.Instance["WaveClearChamps"];
            var waveClearMana = Config.Instance.Menu.Item("WaveClearMana").GetValue<Slider>().Value;

            if (farmOnlyNoChamps && HeroManager.Enemies.Any(x => x.IsValidTarget(this.Q.Range * 1.5f)))
            {
                return;
            }

            if (useQ && this.Q.IsReady() && (waveClearMana < this.Player.ManaPercent))
            {
                var minions = MinionManager.GetMinions(this.Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                var farmLoc = this.Q.GetCircularFarmLocation(minions);

                if (farmLoc.MinionsHit >= (minions.Any(x => x.Team == GameObjectTeam.Neutral) ? 1 : 2))
                {
                    this.Q.Cast(farmLoc.Position);
                }
            }

            if (useE && this.E.IsReady())
            {
                var minionE =
                    MinionManager.GetMinions(this.E.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .FirstOrDefault(x => this.E.GetDamage(x) > x.Health);

                if (minionE != null && this.E.GetDamage(minionE) > this.Player.GetAutoAttackDamage(minionE))
                {
                    this.E.CastOnUnit(minionE);
                }
            }

            if (Config.Instance["WaveClearHarass"])
            {
                this.ExecuteHarass();
            }
        }

        /// <summary>
        ///     Called when a buff is added.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Obj_AI_BaseBuffAddEventArgs" /> instance containing the event data.</param>
        private void OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffAddEventArgs args)
        {
            if (args.Buff.Caster.IsEnemy || !this.W.IsInRange(sender) || !Config.Instance["AutoWCC"]
                || StunBuffTypes.All(x => args.Buff.Type != x))
            {
                return;
            }

            this.W.Cast(sender);
        }

        /// <summary>
        ///     Raises the <see cref="E:Draw" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnDraw(EventArgs args)
        {
            var drawQ = Config.Instance["DrawQ"];
            var drawW = Config.Instance["DrawW"];
            var drawE = Config.Instance["DrawE"];
            var drawR = Config.Instance["DrawR"];

            if (drawQ && this.Q.Level > 0)
            {
                Render.Circle.DrawCircle(this.Player.Position, this.Q.Range, this.Q.IsReady() ? Color.Aqua : Color.Red);
            }

            if (drawW && this.W.Level > 0)
            {
                Render.Circle.DrawCircle(this.Player.Position, this.W.Range, this.W.IsReady() ? Color.Aqua : Color.Red);
            }

            if (drawE && this.E.Level > 0)
            {
                Render.Circle.DrawCircle(this.Player.Position, this.E.Range, this.E.IsReady() ? Color.Aqua : Color.Red);
            }

            if (drawR && this.R.Level > 0)
            {
                Render.Circle.DrawCircle(this.Player.Position, this.R.Range, this.R.IsReady() ? Color.Aqua : Color.Red);
            }
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Config.Instance["AntiGapcloser"])
            {
                return;
            }

            if (this.Player.HealthPercent <= 30 && this.R.IsReady() && gapcloser.Sender.IsFacing(this.Player))
            {
                this.R.Cast(gapcloser.Sender);
            }
            else if (gapcloser.End.Distance(this.Player.ServerPosition) >= this.MinimumWRange)
            {
                this.W.Cast(gapcloser.Sender);
            }
        }

        /// <summary>
        ///     Called when there is an interruptible target.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Interrupter2.InterruptableTargetEventArgs" /> instance containing the event data.</param>
        private void OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Config.Instance["Interrupter"] || !sender.IsFacing(this.Player) || !sender.IsValidTarget(this.R.Range))
            {
                return;
            }

            this.R.Cast(sender);
        }

        /// <summary>
        ///     Called when the orbwalker will miss a minion.
        /// </summary>
        /// <param name="minion">The minion.</param>
        private void OnNonKillableMinion(AttackableUnit minion)
        {
            var minionObj = minion as Obj_AI_Base;

            if (minionObj == null)
            {
                return;
            }

            if ((this.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear || !Config.Instance["UseEWaveClear"])
                && ((this.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed || !Config.Instance["UseEFarmHarass"])
                    || !(this.E.GetDamage(minionObj) > minion.Health)))
            {
                return;
            }

            this.E.CastOnUnit(minionObj);
        }

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            this.KillSteal();

            switch (this.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Mixed:
                    this.ExecuteHarass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    this.ExecuteWaveClear();
                    break;
                case Orbwalking.OrbwalkingMode.Combo:
                    this.ExecuteCombo();
                    break;
            }

           
        }

        /// <summary>
        /// Secures kills.
        /// </summary>
        private void KillSteal()
        {
            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range)))
            {
                var spell = this.Spells.Where(x => x.IsReady() && Config.Instance[$"Use{x.Slot}KS"])
                        .OrderByDescending(x => x.GetDamage(enemy))
                        .FirstOrDefault();

                if (spell?.GetDamage(enemy) > enemy.Health)
                {
                    spell.Cast(enemy);
                }
            }
        }

        #endregion
    }
}