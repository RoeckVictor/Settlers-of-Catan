using System;
using System.Collections;
using System.Collections.Generic;
using Noyau.View;
using Util.View;
using AI.Model;
using AI.View;
using System.Diagnostics;
using UnityEngine;

namespace AI.Controller
{   
    /// <summary>
    /// Gere l'interface la creation et l'appel aux fonctionnalites des IAs.
    /// Classe instanciee une seule fois (singleton).
    /// </summary>
    public class AIManager : MonoBehaviour
    {
        /// <summary>
        /// Dictionnaire contenant l'ensemble des IAs groupees par partie.
        /// </summary>
        private Dictionary<Guid, Game> gamesWithIAs = new Dictionary<Guid, Game>();

        /// <summary>
        /// Propriete permettant de recuperer une instance unique thread-safe de la classe.
        /// </summary>
        public static AIManager Instance { get { return iaManager.Value; } }
        /// <summary>
        /// Constructeur prive utilise en interne pour generer l'instance unique de la classe.
        /// </summary>
        private static readonly Lazy<AIManager> iaManager = new Lazy<AIManager>(() => new AIManager());

        /// <value> Permet de savoir si l'AIManager a souscrit aux evenements du noyau.</value>
        private bool eventHandlersRegistered = false;

        /// <summary>
        /// Methode qui permet d'enregistrer les fonctions de l'IA afin de souscrire aux evenements des autres modules.
        /// </summary>
        public void RegisterEventHandlers()
        {
            if (eventHandlersRegistered)
                return;

            GameView.Instance.GameBegin += GameBeginHandler;
            GameView.Instance.GameDeleted += GameDeletedHandler;
            GameView.Instance.ConstructionDone += ConstructionDoneHandler;
            GameView.Instance.ConstructionPhaseBegin += ConstructionPhaseBeginHandler;
            GameView.Instance.HarvestPhaseBegin += HarvestPhaseBeginHandler;
            GameView.Instance.DiscardPhaseBegin += DiscardPhaseBeginHandler;
            GameView.Instance.ExchangePhaseBegin += ExchangePhaseBeginHandler;
            GameView.Instance.BanditMoveBegin += BanditMovePhaseBeginHandler;
            GameView.Instance.InitialConstructionFirstRound += InitialConstructHandler;
            eventHandlersRegistered = true;
        }

        /// <summary>
        /// Annule la souscription aux evenements des autres modules par l'IA.
        /// </summary>
        public void UnregisterEventHandlers()
        {
            if (!eventHandlersRegistered)
                return;

            UnityEngine.Debug.Log("UnregisterIA");
            GameView.Instance.GameBegin -= GameBeginHandler;
            GameView.Instance.GameDeleted -= GameDeletedHandler;
            GameView.Instance.ConstructionDone -= ConstructionDoneHandler;
            GameView.Instance.ConstructionPhaseBegin -= ConstructionPhaseBeginHandler;
            GameView.Instance.HarvestPhaseBegin -= HarvestPhaseBeginHandler;
            GameView.Instance.DiscardPhaseBegin -= DiscardPhaseBeginHandler;
            GameView.Instance.ExchangePhaseBegin -= ExchangePhaseBeginHandler;
            GameView.Instance.BanditMoveBegin -= BanditMovePhaseBeginHandler;
            GameView.Instance.InitialConstructionFirstRound -= InitialConstructHandler;
            eventHandlersRegistered = false;
        }

        
        /// <summary>
        /// Fonction gerant l'evenement de debut d'une nouvelle partie.
        /// Groupe les IAs dans la classe Game qui lui est propre.
        /// Le groupe d'IAs est stocke dans un dictionnaire dont la cle d'acces est l'ID de la partie dans le noyau.
        /// </summary>
        /// <param name="sender">L'objet ayant emis l'evenement</param>
        /// <param name="e">Donnees d'evenement decrivant l'etat de la partie</param>
        public void GameBeginHandler(object sender, GameStatusArgs e)
        {

            Dictionary<int, AbstractAI> iaPlayers = new Dictionary<int, AbstractAI>();

            for (var i = 0; i < e.Game.Players.Count; i++)
            {
                if (e.Game.Players[i].IsIA)
                    iaPlayers.Add(i, CreateAi(e.AIDifficulty));
            }

            Game newIAGame;
            if (iaPlayers.Count > 0)
            {
                newIAGame = new Game(iaPlayers);
                gamesWithIAs[e.GameId] = newIAGame;
            }
            else
                return;
        }

        /// <summary>
        /// Retourne une nouvelle instance d'une IA que l'on peut rajouter a une partie.
        /// Le type d'IA varie en fonction de la difficulte indiquee.
        /// </summary>
        /// <param name="difficulty">Niveau d'intelligence de l'IA creee</param>
        /// <returns>Une nouvelle instance d'IA</returns>
        internal AbstractAI CreateAi(int difficulty)
        {
            switch (difficulty)
            {
                case 0:
                    return new EasyAI();
                case 1:
                    return new MediumAI();
                case 2:
                    return new DifficultAI();
                default:
                    return new DifficultAI();
            }
        }

        // Appelée lors d'une demande de suppression de partie
        /// <summary>
        /// Handler de suppression de partie
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement</param>
        public void GameDeletedHandler(object sender, GameDeletedArgs e)
        {
            if (!gamesWithIAs.ContainsKey(e.GameId))
            {
                return;
            }

            gamesWithIAs.Remove(e.GameId);
        }

        /// <summary>
        /// Gere l'evenement de fin d'une construction.
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Donnees d'evenement decrivant le resultat d'une construction</param>
        public void ConstructionDoneHandler(object sender, ActionDoneInfoArgs e)
        {

            IGame game = e.Game;
            IPlayer currPlayer = game.Players[game.CurrentPlayer];
            //UnityEngine.Debug.Log("Hello Am AI");
            //UnityEngine.Debug.Log(" Joueur" + game.CurrentPlayer + " // Points : " + currPlayer.VictoryPoints);
            if (!currPlayer.IsIA || !e.ActionSuccessful)
            {
                return;
            }

            
            IAI currIA = gamesWithIAs[e.GameId].IAPlayers[e.Game.CurrentPlayer];
            if (game.CurrentPhase == GamePhase.INITIAL_BUILDING_1)
            {

                if (currPlayer.VictoryPoints == 1)
                    //currIA.PlaceFirstRoad(game);
                    StartCoroutine(DelayedFirstRoadConstruct(1, currIA, game));
                else
                    StartCoroutine(DelayedFirstColonyConstruct(1, currIA, game));

            }
            else if (game.CurrentPhase == GamePhase.INITIAL_BUILDING_2)
            {
                if (currPlayer.VictoryPoints == 2)
                    StartCoroutine(DelayedSecondRoadConstruct(1, currIA, game));
                else
                    StartCoroutine(DelayedSecondColonyConstruct(1, currIA, game));

            }

        }

        /// <summary>
        /// Retarde l'action de construction de l'IA.
        /// </summary>
        /// <param name="seconds">Le delai a appliquer(en secondes)</param>
        /// <param name="ai">L'IA devant effectuer l'action</param>
        /// <param name="game">La partie dans laquelle se trouve</param>
        /// <returns>Un IEnumerator pouvant etre appele comme coroutine Unity</returns>
        IEnumerator DelayedFirstRoadConstruct(int seconds, IAI ai, IGame game)
        {
            yield return new WaitForSeconds(seconds);
            ai.PlaceFirstRoad(game);
        }
        /// <summary>
        /// Retarde l'action de construction de l'IA.
        /// </summary>
        /// <param name="seconds">Le delai a appliquer(en secondes)</param>
        /// <param name="ai">L'IA devant effectuer l'action</param>
        /// <param name="game">La partie dans laquelle se trouve</param>
        /// <returns>Un IEnumerator pouvant etre appele comme coroutine Unity</returns>
        IEnumerator DelayedSecondRoadConstruct(int seconds, IAI ai, IGame game)
        {
            yield return new WaitForSeconds(seconds);
            ai.PlaceSecondRoad(game);
        }
        /// <summary>
        /// Retarde l'action de construction de l'IA.
        /// </summary>
        /// <param name="seconds">Le delai a appliquer(en secondes)</param>
        /// <param name="ai">L'IA devant effectuer l'action</param>
        /// <param name="game">La partie dans laquelle se trouve</param>
        /// <returns>Un IEnumerator pouvant etre appele comme coroutine Unity</returns>
        IEnumerator DelayedFirstColonyConstruct(int seconds, IAI ai, IGame game)
        {
            yield return new WaitForSeconds(seconds);
            ai.PlaceFirstColony(game);
        }
        /// <summary>
        /// Retarde l'action de construction de l'IA.
        /// </summary>
        /// <param name="seconds">Le delai a appliquer(en secondes)</param>
        /// <param name="ai">L'IA devant effectuer l'action</param>
        /// <param name="game">La partie dans laquelle se trouve</param>
        /// <returns>Un IEnumerator pouvant etre appele comme coroutine Unity</returns>
        IEnumerator DelayedSecondColonyConstruct(int seconds, IAI ai, IGame game)
        {
            yield return new WaitForSeconds(seconds);
            ai.PlaceSecondColony(game);
        }

        /// <summary>
        /// Gere l'evenement de construction initiale.
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Donnees d'evenement decrivant le statut de la partie</param>
        public void InitialConstructHandler(object sender, GameStatusArgs e)
        {
            UnityEngine.Debug.Log(" Joueur" + e.Game.CurrentPlayer + " // Points : " + e.Game.Players[e.Game.CurrentPlayer].VictoryPoints);
            gamesWithIAs[e.GameId].IAPlayers[e.Game.CurrentPlayer].PlaceFirstColony(GameView.Instance.GetGame(e.GameId));
        }

        /// <summary>
        /// Gere l'evenement de debut de phase de construction.
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Donnees d'evenement decrivant le statut de la partie</param>
        public void ConstructionPhaseBeginHandler(object sender, GameStatusArgs e)
        {
            IGame game = e.Game;
            IPlayer currPlayer = game.Players[game.CurrentPlayer];
            if (!currPlayer.IsIA)
            {
                return;
            }

            //gamesWithIAs[e.GameId].IAPlayers[e.Game.CurrentPlayer].Construct(e.Game);
            StartCoroutine(DelayedConstruct(1, gamesWithIAs[e.GameId].IAPlayers[e.Game.CurrentPlayer], e.Game));
        }
        /// <summary>
        /// Retarde l'action de construction de l'IA.
        /// </summary>
        /// <param name="seconds">Le delai a appliquer(en secondes)</param>
        /// <param name="ai">L'IA devant effectuer l'action</param>
        /// <param name="game">La partie dans laquelle se trouve</param>
        /// <returns>Un IEnumerator pouvant etre appele comme coroutine Unity</returns>
        IEnumerator DelayedConstruct(int seconds, IAI ai, IGame game)
        {
            yield return new WaitForSeconds(seconds);
            ai.Construct(game);
        }

        /// <summary>
        /// Gere l'evenement de debut de phase de recolte.
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Donnees d'evenement decrivant le statut de la partie</param>
        public void HarvestPhaseBeginHandler(object sender, GameStatusArgs e)
        {
            IGame game = e.Game;
            IPlayer currPlayer = game.Players[game.CurrentPlayer];
            UnityEngine.Debug.Log("IN AI HARVEST BEGIN || currentPlayer "+currPlayer.Id);
            if (!currPlayer.IsIA)
            {
                return;
            }
            StartCoroutine(DelayedDiceThrow(1, gamesWithIAs[e.GameId].IAPlayers[e.Game.CurrentPlayer], e.Game));
        }
        /// <summary>
        /// Retarde l'action de lancer de des l'IA.
        /// </summary>
        /// <param name="seconds">Le delai a appliquer(en secondes)</param>
        /// <param name="ai">L'IA devant effectuer l'action</param>
        /// <param name="game">La partie dans laquelle se trouve</param>
        /// <returns>Un IEnumerator pouvant etre appele comme coroutine Unity</returns>
        IEnumerator DelayedDiceThrow(int seconds, IAI ai, IGame game)
        {
            yield return new WaitForSeconds(seconds);
            ai.HarvestBegin(game);
        }

        /// <summary>
        /// Gere l'evenement de debut de phase de defausse.
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Donnees d'evenement decrivant le statut de la partie</param>
        public void DiscardPhaseBeginHandler(object sender, GameStatusArgs e)
        {
            IGame game = e.Game;
            IPlayer currPlayer = game.Players[game.CurrentPlayer];
            /*if (!currPlayer.IsIA)
            {
                return;
            }*/

            foreach(IPlayer p in game.Players)
            {
                if (p.IsIA)
                {
                    gamesWithIAs[e.GameId].IAPlayers[p.Id].Discard(e.Game);
                }
                    
            }
        }

        /// <summary>
        /// Gere l'evenement de debut de phase d'echange.
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Donnees d'evenement decrivant le statut de la partie</param>
        public void ExchangePhaseBeginHandler(object sender, GameStatusArgs e)
        {
            IGame game = e.Game;
            IPlayer currPlayer = game.Players[game.CurrentPlayer];
            if (!currPlayer.IsIA)
            {
                return;
            }

            gamesWithIAs[e.GameId].IAPlayers[e.Game.CurrentPlayer].Exchange(e.Game);
        }

        /// <summary>
        /// Gere l'evenement de debut de phase de deplacement du bandit.
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Donnees d'evenement decrivant le statut de la partie</param>
        public void BanditMovePhaseBeginHandler(object sender, GameStatusArgs e)
        {
            IGame game = e.Game;
            IPlayer currPlayer = game.Players[game.CurrentPlayer];
            if (!currPlayer.IsIA)
            {
                return;
            }

            gamesWithIAs[e.GameId].IAPlayers[e.Game.CurrentPlayer].MoveBandit(e.Game);
        }

    }

}
