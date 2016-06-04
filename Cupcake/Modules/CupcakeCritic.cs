namespace Cupcake.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    /// <summary>
    ///     Critics the cupcakes and makes sure the other cupcakes aren't touching each other!
    /// </summary>
    /// <seealso cref="ICupcakeCritic" />
    internal class CupcakeCritic : ICupcakeCritic
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets the collisions.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="speed">The speed.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="width">The width.</param>
        /// <param name="minions">The minions.</param>
        /// <param name="startPosition">The start position.</param>
        /// <param name="endPosition">The end position.</param>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        /// <returns>
        /// The number of collisions
        /// </returns>
        public int GetCollisions(
            float range,
            float speed, 
            float delay, 
            float width, 
            IEnumerable<Obj_AI_Base> minions, 
            Vector3 startPosition, 
            Vector3 endPosition, 
            Obj_AI_Base target,
            Obj_AI_Hero source)
        {
            var collisions =
                LeagueSharp.Common.Collision.GetCollision(
                    new List<Vector3>() { startPosition, endPosition },
                    new PredictionInput() { Speed = speed, Delay = delay, Radius = width, Range = range, Unit = target, From = source.ServerPosition});

            collisions.RemoveAll(x => x.NetworkId == source.NetworkId || x.NetworkId == target.NetworkId);
            return collisions.Count;

            /*return
                minions.Where(x => x.Team != source.Team)
                    .Select(
                        x =>
                        new
                            {
                                minion = x, 
                                pred = CupcakeFactory.Make<ICupcakeBatter>().PredictPosition(x, delay, speed, source)
                            })
                    .Select(
                        x =>
                        new { t = x, projection = x.pred.To2D().ProjectOn(startPosition.To2D(), endPosition.To2D()) })
                    .Count(
                        x =>
                        x.projection.IsOnSegment
                        && x.projection.SegmentPoint.Distance(x.t.pred, true)
                        < Math.Pow((x.t.minion.BoundingRadius * 2) + width, 2));*/
        }

        #endregion
    }
}