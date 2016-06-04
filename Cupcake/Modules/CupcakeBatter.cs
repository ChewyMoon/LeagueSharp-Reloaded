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
        ///     Predicts the position of the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="speed">The speed.</param>
        /// <param name="source">The source.</param>
        /// <returns>The prediction <see cref="Vector3" /></returns>
        public Vector3 PredictPosition(Obj_AI_Base target, float delay, float speed, Obj_AI_Hero source = null)
        {
            delay = Math.Abs(delay) < float.Epsilon ? 0.125f : delay;
            source = source ?? ObjectManager.Player;

            var direction = GetTargetDirection(target);
            var position = target.ServerPosition;

            if (target.IsMoving)
            {
                // speed != 0
                if (Math.Abs(speed) > float.Epsilon)
                {
                    var vmc = Geometry.VectorMovementCollision(
                        target.ServerPosition.To2D(), 
                        target.GetWaypoints().Last(), 
                        target.MoveSpeed, 
                        source.ServerPosition.To2D(), 
                        speed, 
                        delay);

                    var t = (float)vmc[0];
                    var v = (Vector2)vmc[1];

                    if (Math.Abs(t) > float.Epsilon && !float.IsNaN(t))
                    {
                        position = new Vector3(v, target.ServerPosition.Z);
                    }
                    else
                    {
                        position = target.ServerPosition
                                   + ((direction.Normalized() * ((source.Distance(target) / speed) + delay))
                                      * target.MoveSpeed);
                    }
                }
                else
                {
                    position = target.ServerPosition + (direction.Normalized() * delay * target.MoveSpeed);
                }
            }

            var baitLevel = CupcakeFactory.Make<ICupcakeBait>().GetBaitLevel(target) / 100;

            if (target.IsMoving && baitLevel > 0 && baitLevel < 200)
            {
                position = position + ((target.ServerPosition - position) * baitLevel);
            }

            return position;
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