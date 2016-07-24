namespace Cupcake
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.Data;
    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.Data.Enumerations;

    using SharpDX;

    using CollisionableObjects = LeagueSharp.Data.Enumerations.CollisionableObjects;

    /// <summary>
    ///     A delicious treat most commonly served with icing on top.
    /// </summary>
    /// <seealso cref="ICupcake" />
    public class Cupcake : ICupcake
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the prediction.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="range">The range.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="speed">The speed.</param>
        /// <param name="width">The width.</param>
        /// <param name="collision">if set to <c>true</c> the missile collides with minions.</param>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     <see cref="BakedCupcake" />
        /// </returns>
        public BakedCupcake GetPrediction(
            Obj_AI_Base target, 
            float range, 
            float delay, 
            float speed, 
            float width, 
            bool collision, 
            Obj_AI_Hero source = null)
        {
            if (target.IsDead)
            {
                // Throw exception?
                return new BakedCupcake();
            }

            source = source ?? ObjectManager.Player;

            // server tick broscience
            delay += 0.07f + (Game.Ping / 2000f);

            if (target.Type != GameObjectType.obj_AI_Hero)
            {
                Console.WriteLine("WTF");
                return new BakedCupcake
                           {
                               CastPosition = CupcakeFactory.Make<ICupcakeBatter>()
                                             .PredictPosition(target, delay, speed, speed, source).CastPosition
                };
            }

            var hitChance = 0;

            var position = CupcakeFactory.Make<ICupcakeBatter>().PredictPosition(target, delay, speed, width, source);

            if (source.Distance(position.CastPosition) > range || source.Distance(position.UnitPosition) > range)
            {
                Console.WriteLine("oor");
                return new BakedCupcake { CastPosition = Vector3.Zero, HitChance = HitChance.Impossible };
            }

            var baitLevel = CupcakeFactory.Make<ICupcakeBait>().GetBaitLevel(target);
            var rangeOffset = range + (width / 2f) - (target.IsFacing(source) ? target.BoundingRadius : 0);
            bool col = false, col2 = false, col3 = false;

            if (collision)
            {
                var minions = MinionManager.GetMinions(
                    source.ServerPosition, 
                    range, 
                    MinionTypes.All, 
                    MinionTeam.NotAlly);

                if (minions.Count > 0)
                {
                    col = CupcakeFactory.Make<ICupcakeCritic>()
                              .GetCollisions(
                                  range, 
                                  speed, 
                                  delay, 
                                  width < 100 ? 100 : width + 25, 
                                  minions, 
                                  source.ServerPosition, 
                                  position.CastPosition, 
                                  target, 
                                  source) > 0;

                    col2 = CupcakeFactory.Make<ICupcakeCritic>()
                               .GetCollisions(
                                   range, 
                                   speed, 
                                   delay, 
                                   width < 100 ? 100 : width + 25, 
                                   minions, 
                                   source.ServerPosition, 
                                   target.ServerPosition, 
                                   target, 
                                   source) > 0;

                    col3 = CupcakeFactory.Make<ICupcakeCritic>()
                               .GetCollisions(
                                   range, 
                                   speed, 
                                   delay, 
                                   width, 
                                   minions, 
                                   source.ServerPosition, 
                                   position.CastPosition, 
                                   target, 
                                   source) > 0;
                }
            }

            if (position.CastPosition.Distance(source.ServerPosition) < rangeOffset * rangeOffset)
            {
                if (!collision || !col)
                {
                    if ((!target.IsMoving || baitLevel < 30) && (!collision || !col2))
                    {
                        hitChance += 3;
                    }
                    else
                    {
                        hitChance += 2;
                    }
                }
                else if (!col3)
                {
                    if (!target.IsMoving || baitLevel < 30)
                    {
                        hitChance += 1;
                    }

                    hitChance += 1;
                }
            }

            if (position.CastPosition.IsWall())
            {
                hitChance -= 1;
            }

            return new BakedCupcake {CastPosition = position.CastPosition, UnitPosition = position.UnitPosition, HitChance = (HitChance)hitChance };
        }

        /// <summary>
        ///     Gets the prediction.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="spell">The spell.</param>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     <see cref="BakedCupcake" />
        /// </returns>
        public BakedCupcake GetPrediction(Obj_AI_Base target, SpellDatabaseEntry spell, Obj_AI_Hero source = null)
        {
            return this.GetPrediction(
                target, 
                spell.Range, 
                spell.Delay / 1000f, 
                spell.MissileSpeed, 
                spell.SpellType == SpellType.SkillshotCircle ? spell.Radius : spell.Width, 
                spell.CollisionObjects.Any(
                    x => x != CollisionableObjects.YasuoWall || x != CollisionableObjects.BraumShield), 
                source);
        }

        /// <summary>
        ///     Gets the prediction.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     <see cref="BakedCupcake" />
        /// </returns>
        public BakedCupcake GetPrediction(Obj_AI_Base target, SpellSlot slot, Obj_AI_Hero source = null)
        {
            return this.GetPrediction(
                target, 
                Data.Get<SpellDatabase>().GetByName((source ?? ObjectManager.Player).Spellbook.GetSpell(slot).Name), 
                source);
        }

        #endregion
    }
}