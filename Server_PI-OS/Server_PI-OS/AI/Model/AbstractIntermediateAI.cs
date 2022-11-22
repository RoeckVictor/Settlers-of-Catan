using Noyau.View;

namespace AI.Model
{
	abstract class AbstractIntermediateAI : AbstractAI
    {
        private DifficultAI brain = new DifficultAI();
        private RandomAI brainless = new RandomAI();
        protected double level;

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
        /// <summary>
        /// Lors de la phase d'initialisation, place la première colonie de l'IA
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        public override void PlaceFirstColony(IGame game)
        {
            brain.InitAI(game);
            brainless.InitAI(game);
            ChooseRandomAI().PlaceFirstColony(game);
        }

        /// <summary>
        /// Lors de la phase d'initialisation, place la première route de l'IA
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        public override void PlaceFirstRoad(IGame game)
        {
            ChooseRandomAI().PlaceFirstRoad(game);
        }

        /// <summary>
        /// Lors de la phase d'initialisation, place la seconde colonie de l'IA
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        public override void PlaceSecondColony(IGame game)
        {
            ChooseRandomAI().PlaceSecondColony(game);
        }

        /// <summary>
        /// Lors de la phase d'initialisation, place la seconde route de l'IA
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        public override void PlaceSecondRoad(IGame game)
        {
            ChooseRandomAI().PlaceSecondRoad(game);
        }

        /// <summary>
        /// Exécute la phase de récolte de l'IA
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        public override void HarvestBegin(IGame game)
        {
            brain.InitTurn();
            brainless.InitTurn();
            ChooseRandomAI().HarvestBegin(game);
        }

        /// <summary>
        /// Exécute la phase de construction de l'IA
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        public override void Construct(IGame game)
        {
            ChooseRandomAI().Construct(game);
        }

        /// <summary>
        /// Défausse la moitié des cartes de l'IA, arondi à l'entier inférieur
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        public override void Discard(IGame game)
        {
            ChooseRandomAI().Discard(game);
        }

        /// <summary>
        /// Exécute la phase d'échange de l'IA
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        public override void Exchange(IGame game)
        {
            ChooseRandomAI().Exchange(game);
        }

        /// <summary>
        /// Déplace le jeton bandit de l'IA et vole une ressource
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        public override void MoveBandit(IGame game)
        {
            ChooseRandomAI().MoveBandit(game);
        }
    }
}
