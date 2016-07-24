namespace Cupcake.Modules
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    /// <summary>
    ///     The cupcake batter. Helps predict the targets position.
    /// </summary>
    /// <seealso cref="ICupcakeBatter" />
    internal class CupcakeBatter : ICupcakeBatter
    {
        #region Public Methods and Operators

        /// <summary>
        /// Predicts the position of the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="speed">The speed.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="source">The source.</param>
        /// <returns>
        /// The prediction <see cref="Vector3" />
        /// </returns>
        public BakedCupcake PredictPosition(Obj_AI_Base target, float delay, float speed, float radius, Obj_AI_Hero source = null)
        {
            source = source ?? ObjectManager.Player;

            var path = target.GetWaypoints();
            path.CutPath(delay * target.MoveSpeed);

            if (path.Count >= 2 && path.PathLength() >= (delay * speed) - (radius + target.BoundingRadius))
            {
                Console.WriteLine("target moving");
                var tT = 0f;
                for (var i = 0; i < path.Count - 1; i++)
                {
                    var a = path[i];
                    var b = path[i + 1];

                    var tB = a.Distance(b) / speed;
                    var direction = (b - a).Normalized();

                    a -= speed * tT * direction;

                    var vmc = Geometry.VectorMovementCollision(a, b, target.MoveSpeed, source.ServerPosition.To2D(), speed, tT);
                    var t = (float)vmc[0];
                    var pos = (Vector2)vmc[1];

                    if (pos.IsValid() && t >= tT && t <= tT + tB)
                    {
                        if (pos.Distance(b, true) < 20)
                        {
                            break;
                        }

                        var p = pos + radius * direction;

                        return new BakedCupcake() { CastPosition = pos.To3D(), UnitPosition = p.To3D() };
                    }

                    tT += tB;
                }
            }
            Console.WriteLine("target not moving");

            return new BakedCupcake() {CastPosition = target.ServerPosition, UnitPosition = target.ServerPosition};
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the target direction.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>The target's direction.</returns>
        private static Vector3 GetTargetDirection(Obj_AI_Base target)
        {
            return target.IsMoving && target.Path.Length >= 2
                       ? target.Path[target.Path.Length - 1] - target.Path[0]
                       : target.ServerPosition;
        }

        #endregion
    }
}