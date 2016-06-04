namespace Cupcake
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using global::Cupcake.Modules;

    /// <summary>
    ///     IoC Container for <see cref="ICupcakeIngredient" />s.
    /// </summary>
    /// <seealso cref="ICupcakeIngredient" />
    public class CupcakeRecipe : IEnumerable<ICupcakeIngredient>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CupcakeRecipe" /> class.
        /// </summary>
        /// <param name="cupcakeImpl">The cupcake implementation.</param>
        /// <param name="baitImpl">The bait implementation.</param>
        /// <param name="batterImpl">The batter implementation.</param>
        /// <param name="criticImpl">The critic implementation.</param>
        public CupcakeRecipe(
            ICupcake cupcakeImpl, 
            ICupcakeBait baitImpl, 
            ICupcakeBatter batterImpl, 
            ICupcakeCritic criticImpl)
        {
            this.Cupcake = cupcakeImpl;
            this.CupcakeBait = baitImpl;
            this.CupcakeBatter = batterImpl;
            this.CupcakeCritic = criticImpl;
        }

        private CupcakeRecipe()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the default cupcake recipe.
        /// </summary>
        /// <value>
        ///     The default cupcake recipe.
        /// </value>
        public static CupcakeRecipe Default
            =>
                new CupcakeRecipe
                    {
                        Cupcake = new Cupcake(), CupcakeBait = new CupcakeBait(), CupcakeBatter = new CupcakeBatter(), 
                        CupcakeCritic = new CupcakeCritic()
                    };

        #endregion

        #region Properties

        internal ICupcake Cupcake { get; set; }

        internal ICupcakeBait CupcakeBait { get; set; }

        internal ICupcakeBatter CupcakeBatter { get; set; }

        internal ICupcakeCritic CupcakeCritic { get; set; }

        internal IEnumerable<ICupcakeIngredient> CupcakeIngredients { get; set; } = new List<ICupcakeIngredient>();

        #endregion

        #region Public Methods and Operators

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<ICupcakeIngredient> GetEnumerator()
        {
            return this.CupcakeIngredients.GetEnumerator();
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds the recipe.
        /// </summary>
        public void BuildRecipe()
        {
            this.CupcakeIngredients = new ICupcakeIngredient[]
                                          { this.Cupcake, this.CupcakeBait, this.CupcakeBatter, this.CupcakeCritic };
        }

        #endregion
    }
}