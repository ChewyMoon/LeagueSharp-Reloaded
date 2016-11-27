using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementBot.Core
{
    using LeagueSharp;
    using LeagueSharp.Common;
    /*
        Things bot needs to do:

        buy items, move, cast spell, attack champions/turrets/minions etc
        neural network decides orbwalking mode and maybe movement pos?
         
         */
    class ElementBot
    {
        /// <summary>
        /// Starts the bot.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        public void Start(EventArgs args)
        {
            if (Game.MapId != GameMapId.HowlingAbyss || Game.Type != GameType.ARAM)
            {
                Game.PrintChat("ElementBot only supports ARAM!");
                return;
            }

            

            Game.OnUpdate += this.OnUpdate;
        }

        /// <summary>
        /// Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OnUpdate(EventArgs args)
        {
            
        }
    }
}
