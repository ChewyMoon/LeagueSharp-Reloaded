namespace Cupcake.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    /// <summary>
    ///     Uses cupcakes as bait. Not really, it just calculates the "bait level" of an enemy through statistical waypoint
    ///     analaysis.
    /// </summary>
    /// <seealso cref="ICupcakeBait" />
    internal class CupcakeBait : ICupcakeBait
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CupcakeBait" /> class.
        /// </summary>
        public CupcakeBait()
        {
            Obj_AI_Base.OnNewPath += this.ObjAiBaseOnNewPath;
            Game.OnUpdate += this.GameOnUpdate;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the tick table.
        /// </summary>
        /// <value>
        ///     The tick table.
        /// </value>
        public Dictionary<int, List<PathData>> TickTable { get; } = new Dictionary<int, List<PathData>>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the bait level.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns>The bait level.</returns>
        public float GetBaitLevel(Obj_AI_Base unit)
        {
            var baitLevel = 0f;

            if (!this.TickTable.ContainsKey(unit.NetworkId) || this.TickTable[unit.NetworkId] == null)
            {
                return baitLevel;
            }

            var list = this.TickTable[unit.NetworkId];

            for (var i = list.Count - 1; i > 0; i--)
            {
                if (i - 1 >= list.Count || i - 1 <= 0)
                {
                    continue;
                }

                var dirPerc = GetDirectionDifferencePerc(
                    list[i].EndPosition - list[i].StartPosition, 
                    list[i - 1].EndPosition - list[i - 1].StartPosition);

                baitLevel += (1f / (list.Count - i)) * (dirPerc <= 100 ? dirPerc : 100);
            }

            baitLevel = baitLevel < 0 ? 0 : baitLevel / 2;

            return baitLevel;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the direction difference.
        /// </summary>
        /// <param name="dir1">The dir1.</param>
        /// <param name="dir2">The dir2.</param>
        /// <returns></returns>
        private static float GetDirectionDifferencePerc(Vector3 dir1, Vector3 dir2)
        {
            return (dir1.Normalized() * 100 - dir2.Normalized() * 100).To2D().Length() / 2;
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void GameOnUpdate(EventArgs args)
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(x => this.TickTable.ContainsKey(x.NetworkId)))
            {
                var list = this.TickTable[hero.NetworkId];

                var data = list?.FirstOrDefault();

                if (data == null)
                {
                    continue;
                }

                if (data.Time < Game.ClockTime - 1)
                {
                    list.Remove(data);
                }
            }
        }

        /// <summary>
        ///     Fired when an <see cref="Obj_AI_Base" /> has a new path.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectNewPathEventArgs" /> instance containing the event data.</param>
        private void ObjAiBaseOnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender.Type != GameObjectType.obj_AI_Hero
                || (!(args.Path.First().Distance(args.Path.Last()) > 0) && !sender.IsMoving))
            {
                return;
            }

            if (!this.TickTable.ContainsKey(sender.NetworkId))
            {
                this.TickTable.Add(sender.NetworkId, new List<PathData>());
            }

            this.TickTable[sender.NetworkId].Add(
                new PathData
                    {
                        Unit = (Obj_AI_Hero)sender, StartPosition = args.Path.First(), EndPosition = args.Path.Last(), 
                        Time = Game.ClockTime
                    });
        }

        #endregion
    }
}