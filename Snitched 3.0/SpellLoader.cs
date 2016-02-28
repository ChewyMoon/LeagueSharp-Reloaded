namespace Snitched
{
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     Gets spells that can be used for Snitched.
    /// </summary>
    internal class SpellLoader
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the usable spells.
        /// </summary>
        /// <returns></returns>
        public static List<Spell> GetUsableSpells()
        {
            var slots = new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
            var dummyTarget = new Obj_AI_Base();
            var usableSpells = new List<Spell>();

            foreach (var slot in slots)
            {
                var spellInst = ObjectManager.Player.GetSpell(slot);
                var targettingType = spellInst.SData.TargettingType;
                var sdata = spellInst.SData;

                if (targettingType != SpellDataTargetType.Unit
                    && (!targettingType.ToString().Contains("Location")
                        || !(ObjectManager.Player.GetSpellDamage(dummyTarget, slot) > 0)))
                {
                    Game.PrintChat("Discared Spell {0}. Type: {1}", spellInst.Slot, spellInst.SData.TargettingType);
                    continue;
                }

                var spell = new Spell(slot, sdata.CastRange);

                if (targettingType != SpellDataTargetType.Unit)
                {
                    spell.SetSkillshot(
                        sdata.CastFrame / 30f,
                        sdata.LineWidth.Equals(0) ? sdata.CastRadius : sdata.LineWidth,
                        sdata.MissileSpeed,
                        sdata.HaveHitBone,
                        targettingType == SpellDataTargetType.Cone
                            ? SkillshotType.SkillshotCone
                            : sdata.CastRadius > 0 ? SkillshotType.SkillshotCircle : SkillshotType.SkillshotLine);
                }
                else if (targettingType == SpellDataTargetType.Unit)
                {
                    spell.SetTargetted(sdata.CastFrame / 30f, sdata.MissileSpeed);
                }

                usableSpells.Add(spell);
                Game.PrintChat("Added Spell {0}. Type: {1}.", spellInst.Slot, spellInst.SData.TargettingType);
            }

            return usableSpells;
        }

        #endregion
    }
}