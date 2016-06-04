namespace Cupcake
{
    using System;
    using System.Linq;

    /// <summary>
    ///     Creates cupcakes!
    /// </summary>
    public class CupcakeFactory
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the recipe.
        /// </summary>
        /// <value>
        ///     The recipe.
        /// </value>
        public static CupcakeRecipe Recipe { get; set; } = CupcakeRecipe.Default;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Makes a <see cref="ICupcakeIngredient" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Make<T>() where T : ICupcakeIngredient
        {
            if (!Recipe.CupcakeIngredients.Any())
            {
                Recipe.BuildRecipe();
            }

            return (T) Recipe.FirstOrDefault(x => x is T);
        }

        #endregion
    }
}