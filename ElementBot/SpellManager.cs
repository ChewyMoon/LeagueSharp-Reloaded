using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementBot
{
    using System.Security.Cryptography.X509Certificates;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.Data;
    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.Data.Enumerations;

    using SpellDatabase = LeagueSharp.Data.DataTypes.SpellDatabase;

    class SpellManager
    {
        private SpellDatabase SpellDatabase { get; set; } = Data.Get<SpellDatabase>();

        public SpellDatabaseEntry Q => this.GetSpell(SpellSlot.Q);
        public SpellDatabaseEntry W => this.GetSpell(SpellSlot.W);
        public SpellDatabaseEntry E => this.GetSpell(SpellSlot.E);
        public SpellDatabaseEntry R => this.GetSpell(SpellSlot.R);

        private SpellDatabaseEntry GetSpell(SpellSlot slot)
        {
            return
                this.SpellDatabase.Get(
                    x =>
                    x.Slot == slot && x.ChampionName == ObjectManager.Player.ChampionName
                    && !x.CastType.Any(y => y == CastType.ImpossibleToCast)).FirstOrDefault();
        }

        public void UseSpells(bool combo = false)
        {
          
        }
    }
}
