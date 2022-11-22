using Noyau.View;

namespace AI.Model
{
	abstract class AbstractIntermediateAI : AbstractAI
    {
        /// <value>Instance de DifficultAI</value>
        private DifficultAI brain = new DifficultAI();
        /// <value>Instance de RandomAI</value>
        private RandomAI brainless = new RandomAI();
        /// <value>Probabilité de faire appel aux méthodes de DifficultAI</value>
        protected double level;

        /// <summary>
        /// Choisit une IA au hasard parmi RandomAI et DifficultAI. La probabilité d'obtenir l'IA difficult dépend de la valeur de level
        /// </summary>
        /// <returns>L'instance d'IA choisie aléatoirement</returns>
        protected AbstractAI ChooseRandomAI()
        {
            if(randGen.NextDouble() < level)
            {
                return brain;
            }
            else
            {
                return brainless;
            }
        }

        // Fonctions appelées par le noyau

        /// <inheritdoc/>
        public override void PlaceFirstColony(IGame game)
        {
            brain.InitAI(game);
            brainless.InitAI(game);
            ChooseRandomAI().PlaceFirstColony(game);
        }

        /// <inheritdoc/>
        public override void PlaceFirstRoad(IGame game)
        {
            ChooseRandomAI().PlaceFirstRoad(game);
        }

        /// <inheritdoc/>
        public override void PlaceSecondColony(IGame game)
        {
            ChooseRandomAI().PlaceSecondColony(game);
        }

        /// <inheritdoc/>
        public override void PlaceSecondRoad(IGame game)
        {
            ChooseRandomAI().PlaceSecondRoad(game);
        }

        /// <inheritdoc/>
        public override void HarvestBegin(IGame game)
        {
            brain.InitTurn();
            brainless.InitTurn();
            ChooseRandomAI().HarvestBegin(game);
        }

        /// <inheritdoc/>
        public override void Construct(IGame game)
        {
            ChooseRandomAI().Construct(game);
        }

        /// <inheritdoc/>
        public override void Discard(IGame game)
        {
            ChooseRandomAI().Discard(game);
        }

        /// <inheritdoc/>
        public override void Exchange(IGame game)
        {
            ChooseRandomAI().Exchange(game);
        }

        /// <inheritdoc/>
        public override void MoveBandit(IGame game)
        {
            ChooseRandomAI().MoveBandit(game);
        }
    }
}
