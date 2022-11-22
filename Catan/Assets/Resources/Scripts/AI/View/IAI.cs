using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Noyau.View;

namespace AI.View
{
    public interface IAI
    {
        /// <summary>
        /// Initialise l'IA
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        /// <remarks>A appeler impérativement si PlaceFirstColony n'est pas appelé</remarks>
        void InitAI(IGame game, int playerId);

        /// <summary>
        /// Lors de la phase d'initialisation, place la première colonie de l'IA
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        /// <remarks>Initialise automatiquement l'IA</remarks>
        void PlaceFirstColony(IGame game);

        /// <summary>
        /// Lors de la phase d'initialisation, place la première route de l'IA
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        void PlaceFirstRoad(IGame game);

        /// <summary>
        /// Lors de la phase d'initialisation, place la seconde colonie de l'IA
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        void PlaceSecondColony(IGame game);

        /// <summary>
        /// Lors de la phase d'initialisation, place la seconde route de l'IA
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        void PlaceSecondRoad(IGame game);

        /// <summary>
        /// Exécute la phase de récolte de l'IA
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        void HarvestBegin(IGame game);

        /// <summary>
        /// Exécute la phase de construction de l'IA
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        void Construct(IGame game);

        /// <summary>
        /// Défausse la moitié des cartes de l'IA, arondi à l'entier inférieur
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        void Discard(IGame game);

        /// <summary>
        /// Exécute la phase d'échange de l'IA
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        void Exchange(IGame game);

        /// <summary>
        /// Déplace le jeton bandit de l'IA et vole une ressource
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        void MoveBandit(IGame game);
    }
}
