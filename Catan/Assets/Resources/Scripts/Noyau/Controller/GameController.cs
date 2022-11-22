using System;
using System.Collections.Generic;
using System.Text;
using Noyau.View;
using Util.View;
using Noyau.Model;

using UnityEngine;

/// <summary>
/// <para>La classe GameController</para>
/// <para>Fait le lien entre la vue et le modèle</para>
/// </summary>
namespace Noyau.Controller
{
    public class GameController
    {
        /// <value>Un dictionnaire associant les parties à leurs ID. Utile pour le jeu en réseau.</value>
        internal IDictionary<Guid, Game> Games;

        /// <value>L'instance de jeu pour une partie en local</value>
        internal Game game => Games[Guid.Empty];

        /// <value>Booléen indiquant si l'eventHandler a déjà été enregistré. Utilisé pour éviter les doublons d'évènements</value>
        private bool eventHandlersRegistered = false;

        /// <value>Une grille vide utile pour les parties personnalisées</value>
        public static IHexGrid DefaultGrid => HexGrid.CreateDefaultEmptyGrid();
        private static int compteurDiscard = 0;
        public bool IsOnline;

        /// <summary>
        /// Constructeur de GameController
        /// </summary>
        public GameController()
        {
            Games = new Dictionary<Guid, Game>();
            IsOnline = false;
        }

        /// <summary>
        /// Constructeur de GameController prenant en compte une partie en ligne.
        /// </summary>
        /// <param name="isOnline">Determine si la partie est en ligne</param>
        public GameController(bool isOnline)
        {
            Games = new Dictionary<Guid, Game>();
            IsOnline = isOnline;
        }



        // Renvoie 2 jet de dés entre 1 et 6
        /// <summary>
        /// Méthode qui renvoie 3 jet de dés à 6 faces
        /// </summary>
        /// <returns>Une couple de résulat de lancer de dés</returns>
        public (int, int) ThrowDice()
        {
            System.Random random = new System.Random();
            return (random.Next(1, 6), random.Next(1, 6));
            //return (2, 5);
        }

        /// <summary>
        /// Méthode qui enregistre les méthodes du controller aux évènements de la vue.
        /// <remark>Appelée une fois seulement, les autres fois elle ne fera rien.</remark>
        /// </summary>
        internal void RegisterEventHandlers()
        {
            if (eventHandlersRegistered)
                return;

            GameView.Instance.GameCreate += GameCreateHandler;
            GameView.Instance.GameDelete += GameDeleteHandler;
            GameView.Instance.EndPhase += EndPhaseHandler;
            GameView.Instance.Construct += ConstructHandler;
            GameView.Instance.PlayerExchange += PlayerExchangeHandler;
            GameView.Instance.HarborExchange += HarborExchangeHandler;
            GameView.Instance.KnightCardUse += KnightCardUseHandler;
            GameView.Instance.BanditMove += BanditMoveHandler;
            GameView.Instance.MonopolyCardUse += MonopolyCardUseHandler;
            GameView.Instance.ResourcePairCardUse += ResourcePairCardUseHandler;
            GameView.Instance.RoadConstructionCardUse += RoadConstructionCardUseHandler;
            GameView.Instance.DiceThrow += DiceThrowHandler;
            GameView.Instance.DiscardExtraRessources += DiscardExtraRessourcesHandler;
            GameView.Instance.InitialRoad += InitialRoadHandler;
            GameView.Instance.InitialColony += InitialColonyHandler;
            GameView.Instance.FirstPlayerAI += FirstPlayerAIHandler;

            eventHandlersRegistered = true;
        }

        /// <summary>
        /// Permet d'annuler d'annuler la souscription des fonctions de gestion d'evenements du noyau.
        /// Si elle est utilisee, le noyau ne reagira alors plus aux evenements des autres modules qu'il traite habituellement.
        /// </summary>
        internal void UnregisterEventHandlers()
        {
            if (!eventHandlersRegistered)
                return;
            GameView.Instance.GameCreate -= GameCreateHandler;
            GameView.Instance.GameDelete -= GameDeleteHandler;
            GameView.Instance.EndPhase -= EndPhaseHandler;
            GameView.Instance.Construct -= ConstructHandler;
            GameView.Instance.PlayerExchange -= PlayerExchangeHandler;
            GameView.Instance.HarborExchange -= HarborExchangeHandler;
            GameView.Instance.KnightCardUse -= KnightCardUseHandler;
            GameView.Instance.BanditMove -= BanditMoveHandler;
            GameView.Instance.MonopolyCardUse -= MonopolyCardUseHandler;
            GameView.Instance.ResourcePairCardUse -= ResourcePairCardUseHandler;
            GameView.Instance.RoadConstructionCardUse -= RoadConstructionCardUseHandler;
            GameView.Instance.DiceThrow -= DiceThrowHandler;
            GameView.Instance.DiscardExtraRessources -= DiscardExtraRessourcesHandler;
            GameView.Instance.InitialRoad -= InitialRoadHandler;
            GameView.Instance.InitialColony -= InitialColonyHandler;
            GameView.Instance.FirstPlayerAI -= FirstPlayerAIHandler;

            eventHandlersRegistered = false;
        }

        // Toutes les methodes de gestion des evenements Exterieur => Noyau

        /// <summary>
        /// Handler de création de partie
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement</param>
        public void GameCreateHandler(object sender, GameCreateEventArgs e)
        {
            Guid id;
            if (e.IsOnline)
            {
                id = e.GroupId;
            }
            // traitement de l'evenement
            id = Guid.Empty;
            Game newGame = new Game(id, e.TotalPlayerNumber, e.IANumber, e.Grid);
            Games[id] = newGame;
            Console.WriteLine(newGame.ToString());

            // Mise a jour de la vue
            newGame.CurrentPhase = GamePhase.INITIAL_BUILDING_1;
            GameView.Instance.Game = newGame;
            compteurDiscard = 0;

            // Notification des autres modules par evenement
            GameView.Instance.OnGameBegin(new GameStatusArgs(id, newGame, IsOnline, e.AIDifficulty));
        }

        // Appelée lors d'une demande de suppression de partie
        /// <summary>
        /// Handler de suppression de partie
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement</param>
        public void GameDeleteHandler(object sender, GameDeleteEventArgs e)
        {
            if (!Games.ContainsKey(e.GameId))
            {
                GameView.Instance.OnGameDeleted(new GameDeletedArgs(e.GameId, false, IsOnline));
                return;
            }
            Games.Remove(e.GameId);

            // MAJ vue
            GameView.Instance.Game = null;
            GameView.Instance.OnGameDeleted(new GameDeletedArgs(e.GameId, true, IsOnline));

        }

        // Appelée lors de la fin d'une phase, permet de passer à la phase suivante
        // ou au tour suivant en fonction de la phase

        /// <summary>
        /// Handler de fin de phase.
        /// </summary>
        /// <remarks>En fonction de la phase, fait passer à la phase ou au tour suivant.</remarks>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement</param>
        public void EndPhaseHandler(object sender, BaseEventArgs e)
        {
            Console.Write("In Endphase");
            Game currentGame = Games[e.GameId];
            if (Games[e.GameId].CurrentPhase.Equals(GamePhase.EXCHANGE))
            {
                Games[e.GameId].CurrentPhase = GamePhase.CONSTRUCTION;
                GameView.Instance.Game = Games[e.GameId];
                GameView.Instance.OnConstructionPhaseBegin(new GameStatusArgs(e.GameId, Games[e.GameId], IsOnline));
            }
            else if (Games[e.GameId].CurrentPhase.Equals(GamePhase.CONSTRUCTION))
            {
                UnityEngine.Debug.Log("CurrPlayer : " + Games[e.GameId].CurrentPlayer + " || ENDTURN");
                Games[e.GameId].CurrentPhase = GamePhase.RECOLT;
                currentGame.players[currentGame.CurrentPlayer].EndTurn();
                currentGame.CardUsedThisTurn = false;
                Games[e.GameId].CurrentPlayer = (Games[e.GameId].CurrentPlayer + 1) % Games[e.GameId].players.Count;
                GameView.Instance.Game = Games[e.GameId];

                GameView.Instance.OnHarvestPhaseBegin(new GameStatusArgs(e.GameId, Games[e.GameId], IsOnline));
            }

        }

        // Appelée lorsqu'un joueur doit discard des cartes de sa main (après un 7)
        /// <summary>
        /// Handler de défausse de carte. Appelée lorsque le résultat d'un lancé de dés est 7.
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement</param>
        public void DiscardExtraRessourcesHandler(object sender, DiscardEventArgs e)
        {
            try
            {
                UnityEngine.Debug.Log("In DiscardHandler || currplayer : "+ Games[e.GameId].CurrentPlayer + " || isIa : "+Games[e.GameId].Players[Games[e.GameId].CurrentPlayer].IsIA);
                foreach ((int, List<(RessourceType, int)>) items in e.Discards)
                {

                    Games[e.GameId].players[items.Item1].DiscardRessources(items.Item2);
                    /*foreach(var h in items.Item2)
                    {
                        UnityEngine.Debug.Log(h.Item1 + " : " + h.Item2);
                    }*/
                }

                GameView.Instance.Game = Games[e.GameId];
                compteurDiscard++;
                int limCompteur = 1;
                for(int i = 0; i < Games[e.GameId].players.Count; i++)
                {
                    if (Games[e.GameId].players[i].IsIA)
                        limCompteur++;
                }
                if (compteurDiscard == limCompteur)
                {
                    compteurDiscard = 0;
                    Games[e.GameId].CurrentPhase = GamePhase.BANDIT_MOVE;
                    GameView.Instance.OnBanditMoveBegin(new GameStatusArgs(e.GameId, Games[e.GameId], IsOnline));
                }
                
               
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex.ToString());
            }

        }

        // Appelée lorsqu'un joueur construit un aménagement
        /// <summary>
        /// Handler de construction d'aménagement
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement</param>
        public void ConstructHandler(object sender, ConstructEventArgs e)
        {

            try
            {

                // TODO rajouter le coût de construction

                /*
                if (e.ConstructionType.Equals(ConstructionType.CITY))
                {
                    Games[e.GameId].Players[Games[e.GameId].CurrentPlayer].Constructions.Add((e.ConstructionLocation, ConstructionType.CITY));
                    Games[e.GameId].gameGrid.Intersections.Remove(e.ConstructionLocation);
                    Games[e.GameId].gameGrid.Intersections.Add(e.ConstructionLocation, new Intersection(ConstructionType.CITY, Games[e.GameId].CurrentPlayer));
                }
                else if (e.ConstructionType.Equals(ConstructionType.SETTLEMENT))
                {
                    Games[e.GameId].Players[Games[e.GameId].CurrentPlayer].Constructions.Add((e.ConstructionLocation, ConstructionType.SETTLEMENT));
                    Games[e.GameId].gameGrid.Intersections.Remove(e.ConstructionLocation);
                    Games[e.GameId].gameGrid.Intersections.Add(e.ConstructionLocation, new Intersection(ConstructionType.SETTLEMENT, Games[e.GameId].CurrentPlayer));
                }
                else if (e.ConstructionType.Equals(ConstructionType.ROAD))
                {
                    Games[e.GameId].Players[Games[e.GameId].CurrentPlayer].Constructions.Add((e.ConstructionLocation, ConstructionType.ROAD));
                    Games[e.GameId].gameGrid.Edges.Remove(e.ConstructionLocation);
                    Games[e.GameId].gameGrid.Edges.Add(e.ConstructionLocation, new Edge(ConstructionType.ROAD, Games[e.GameId].CurrentPlayer));
                }
                else if (e.ConstructionType.Equals(ConstructionType.DEVELOPMENT_CARD))
                    Games[e.GameId].players[Games[e.GameId].CurrentPlayer].BuyCard();
                */

                ActionType action = ActionType.NONE;
                try
                {
                    Console.Write("In ConstructHandlerExchange");

                    if (e.ConstructionType.Equals(ConstructionType.CITY))
                    {
                        action = ActionType.CITY;
                        Games[e.GameId].players[Games[e.GameId].CurrentPlayer] = Games[e.GameId].gameGrid.ConstructCity(Games[e.GameId].players[Games[e.GameId].CurrentPlayer], e.ConstructionLocation);

                    }
                    else if (e.ConstructionType.Equals(ConstructionType.SETTLEMENT))
                    {
                        action = ActionType.SETTLEMENT;
                        Games[e.GameId].players[Games[e.GameId].CurrentPlayer] = Games[e.GameId].gameGrid.ConstructColony(Games[e.GameId].players[Games[e.GameId].CurrentPlayer], e.ConstructionLocation, false);

                    }
                    else if (e.ConstructionType.Equals(ConstructionType.ROAD))
                    {
                        action = ActionType.ROAD;
                        Games[e.GameId].players[Games[e.GameId].CurrentPlayer] = Games[e.GameId].gameGrid.ConstructRoad(Games[e.GameId].players[Games[e.GameId].CurrentPlayer], e.ConstructionLocation, false);
                        Game currGame = Games[e.GameId];
                        Player currPlayer = currGame.players[currGame.CurrentPlayer];
                        UpdateLongestRoad(e.GameId);
                    }
                    else if (e.ConstructionType.Equals(ConstructionType.DEVELOPMENT_CARD))
                    {
                        action = ActionType.DEVELOPMENT_CARD;
                        Games[e.GameId].players[Games[e.GameId].CurrentPlayer].BuyCard();
                    }
                    else
                    {
                        action = ActionType.ERROR;
                        GameView.Instance.OnConstructionDone(new ActionDoneInfoArgs(e.GameId, Games[e.GameId], action, false, Games[e.GameId].CurrentPlayer, RessourceType.NONE, RessourceType.NONE, IsOnline));

                        return;
                    }
                    // Verification du nombre de points
                    Game currentGame = Games[e.GameId];
                    Player currentPlayer = currentGame.players[currentGame.CurrentPlayer];
                    if (currentPlayer.VictoryPoints >= 10)
                    {
                        currentGame.CalculateRankingsAndFinish();
                    }

                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.Log(ex.ToString());
                    GameView.Instance.OnConstructionDone(new ActionDoneInfoArgs(e.GameId, Games[e.GameId], ActionType.ERROR, false, Games[e.GameId].CurrentPlayer, RessourceType.NONE, RessourceType.NONE, IsOnline));
                }


                // MAJ vue
                GameView.Instance.Game = Games[e.GameId];
                GameView.Instance.OnConstructionDone(new ActionDoneInfoArgs(e.GameId, Games[e.GameId], action, true, Games[e.GameId].CurrentPlayer, RessourceType.NONE, RessourceType.NONE, IsOnline));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                GameView.Instance.OnConstructionDone(new ActionDoneInfoArgs(e.GameId, Games[e.GameId], ActionType.ERROR, false, Games[e.GameId].CurrentPlayer, RessourceType.NONE, RessourceType.NONE, IsOnline));
                return;
            }
        }

        // Appelée lorsqu'un joueur fait un échange avec un autre

        /// <summary>
        /// Handler d'échange entre joueurs
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement</param>
        public void PlayerExchangeHandler(object sender, PlayerExchangeEventArgs e)
        {
            try
            {
                Console.Write("In PlayerExchange");
                Games[e.GameId].PlayerExchange((e.OtherPlayerId, (IList<(RessourceType t, int num)>)e.GiftedRessources), (Games[e.GameId].CurrentPlayer, (IList<(RessourceType t, int num)>)e.ReceivedRessources));
                GameView.Instance.Game = Games[e.GameId];
                GameView.Instance.OnExchangeDone(new ActionDoneInfoArgs(e.GameId, Games[e.GameId], ActionType.PLAYER_EXCHANGE, true, Games[e.GameId].CurrentPlayer, RessourceType.NONE, RessourceType.NONE, IsOnline));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                UnityEngine.Debug.Log(ex.ToString());
                GameView.Instance.OnExchangeDone(new ActionDoneInfoArgs(e.GameId, Games[e.GameId], ActionType.PLAYER_EXCHANGE, false, Games[e.GameId].CurrentPlayer, RessourceType.NONE, RessourceType.NONE, IsOnline));
                return;
            }
        }

        // Appelée lorsqu'un joueur fait un échange commercial
        /// <summary>
        /// Handler d'échanges commerciaux
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement</param>
        public void HarborExchangeHandler(object sender, HarborExchangeEventArgs e)
        {
            try
            {
                Console.Write("In HarborExchange");
                if (e.Harbor.Equals(HarborType.NONE))
                    Games[e.GameId].NoHarborExchange(Games[e.GameId].CurrentPlayer, e.RessourceToGive, e.RessourceToReceive);
                else if (e.Harbor.Equals(HarborType.GENERAL))
                    Games[e.GameId].GeneralHarborExchange(Games[e.GameId].CurrentPlayer, e.RessourceToGive, e.RessourceToReceive);
                else
                {
                    Games[e.GameId].SpecializedHarborExchange(Games[e.GameId].CurrentPlayer, e.RessourceToGive, e.RessourceToReceive);
                }


                GameView.Instance.Game = Games[e.GameId];
                GameView.Instance.OnExchangeDone(new ActionDoneInfoArgs(e.GameId, Games[e.GameId], ActionType.HARBOR_EXCHANGE, true, Games[e.GameId].CurrentPlayer, e.RessourceToGive, e.RessourceToReceive, IsOnline));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                GameView.Instance.OnExchangeDone(new ActionDoneInfoArgs(e.GameId, Games[e.GameId], ActionType.HARBOR_EXCHANGE, false, Games[e.GameId].CurrentPlayer, e.RessourceToGive, e.RessourceToReceive, IsOnline));
                return;
            }
        }

        // Appelée lorsqu'un joueur utilise une carte chevalier
        /// <summary>
        /// Handler d'utilisation de carte chevalier
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement</param>
        public void KnightCardUseHandler(object sender, BanditMoveEventArgs e)
        {
            try
            {
                Console.Write("In KnightCard");
                Games[e.GameId].UseKnightCard(e.NewThiefLocation, e.PlayerToStealFrom);

                GameView.Instance.Game = Games[e.GameId];

                GameView.Instance.OnCardUsageDone(new ActionDoneInfoArgs(e.GameId, Games[e.GameId], ActionType.KNIGHT, true, Games[e.GameId].CurrentPlayer, RessourceType.NONE, RessourceType.NONE, IsOnline));
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex.ToString());
                GameView.Instance.OnCardUsageDone(new ActionDoneInfoArgs(e.GameId, Games[e.GameId], ActionType.KNIGHT, false, Games[e.GameId].CurrentPlayer, RessourceType.NONE, RessourceType.NONE, IsOnline));
                return;
            }

        }


        // Appelée lorsque le dé tombe sur 7

        /// <summary>
        /// Handler de déplacement de bandit, après la phase de défausse.
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement</param>
        public void BanditMoveHandler(object sender, BanditMoveEventArgs e)
        {
            try
            {
                Console.Write("In BanditMove");
                Games[e.GameId].gameGrid.MoveThief(e.NewThiefLocation);
                if (e.PlayerToStealFrom != -1)
                    Games[e.GameId].Steal(e.NewThiefLocation, e.PlayerToStealFrom);

                Games[e.GameId].CurrentPhase = GamePhase.EXCHANGE;
                GameView.Instance.Game = Games[e.GameId];
                GameView.Instance.OnExchangePhaseBegin(new DiceResultsInfoArgs(e.GameId, Games[e.GameId], (0, 0), IsOnline));
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex.ToString());
                Games[e.GameId].CurrentPhase = GamePhase.EXCHANGE;
                GameView.Instance.Game = Games[e.GameId];
                GameView.Instance.OnExchangePhaseBegin(new DiceResultsInfoArgs(e.GameId, Games[e.GameId], (0, 0), IsOnline));
                return;
            }
        }

        // Appelée lorsqu'un joueur utilise une carte de monopole
        /// <summary>
        /// Handler d'utilisation de carte monopole
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement</param>
        public void MonopolyCardUseHandler(object sender, MonopolyCardUseEventArgs e)
        {
            Console.Write("In Monopoly");
            Games[e.GameId].UseMonopolyCard(e.RessourceToMonopolize);
            GameView.Instance.Game = Games[e.GameId];

            GameView.Instance.OnCardUsageDone(new ActionDoneInfoArgs(e.GameId, Games[e.GameId], ActionType.RESSOURCE_MONOPOLY, true, Games[e.GameId].CurrentPlayer, e.RessourceToMonopolize, RessourceType.NONE, IsOnline));
        }

        // Appelée lorsqu'un joueur utilise une carte de récolte de 2 ressources
        /// <summary>
        /// Handler d'utilisation de carte de récupération de ressources
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement</param>
        public void ResourcePairCardUseHandler(object sender, ResourcePairCardUseEventArgs e)
        {
            Console.Write("In ResourcePair");
            Games[e.GameId].UseRessourcePairCard(e.RessourceToGet1, e.RessourceToGet2);
            GameView.Instance.Game = Games[e.GameId];

            GameView.Instance.OnCardUsageDone(new ActionDoneInfoArgs(e.GameId, Games[e.GameId], ActionType.RESSOURCE_PAIR, true, Games[e.GameId].CurrentPlayer, e.RessourceToGet1, e.RessourceToGet2, IsOnline));
        }

        // Appelée lorsqu'un joueur utilise une carte de construction de 2 routes
        /// <summary>
        /// Handler d'utlisation de carte construction de routes
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement</param>
        public void RoadConstructionCardUseHandler(object sender, RoadConstructionCardUseEventArgs e)
        {
            try
            {
                Console.Write("In RoadConstruct");
                Games[e.GameId].UseRoadConstructionCard(e.FirstRoadLocation, e.SecondRoadLocation);
                UpdateLongestRoad(e.GameId);
                Game currGame = Games[e.GameId];
                Player currPlayer = currGame.players[currGame.CurrentPlayer];
                if (currPlayer.VictoryPoints >= 10)
                {
                    currGame.CalculateRankingsAndFinish();
                }
                GameView.Instance.Game = Games[e.GameId];
                GameView.Instance.OnCardUsageDone(new ActionDoneInfoArgs(e.GameId, Games[e.GameId], ActionType.ROAD_BUILDING, true, Games[e.GameId].CurrentPlayer, RessourceType.NONE, RessourceType.NONE, IsOnline));
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex.ToString());
                GameView.Instance.OnCardUsageDone(new ActionDoneInfoArgs(e.GameId, Games[e.GameId], ActionType.ROAD_BUILDING, false, Games[e.GameId].CurrentPlayer, RessourceType.NONE, RessourceType.NONE, IsOnline));
                return;
            }
        }

        // Appelée lorsque les dés doivent être jeté au début d'un tour
        /// <summary>
        /// Handler de lancement de dés
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement</param>
        public void DiceThrowHandler(object sender, BaseEventArgs e)
        {
            try
            {
                Console.Write("In DiceThrow");
                (int, int) resultThrow = ThrowDice();
                UnityEngine.Debug.Log("currPlayer : " + Games[e.GameId].CurrentPlayer + " || dices : " + resultThrow);
                Games[e.GameId].lastDice = resultThrow;
                int sum = resultThrow.Item1 + resultThrow.Item2;
                if (sum == 7)
                {
                    Games[e.GameId].CurrentPhase = GamePhase.DISCARD;
                    GameView.Instance.Game = Games[e.GameId];
                    GameView.Instance.OnDiscardPhaseBegin(new DiceResultsInfoArgs(e.GameId, Games[e.GameId], resultThrow, IsOnline));
                }
                else
                {
                    Games[e.GameId].RecoltAndDistribute(sum);
                    Games[e.GameId].CurrentPhase = GamePhase.EXCHANGE;
                    GameView.Instance.Game = Games[e.GameId];
                    GameView.Instance.OnExchangePhaseBegin(new DiceResultsInfoArgs(e.GameId, Games[e.GameId], resultThrow, IsOnline));
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex);
            }

        }

        /// <summary>
        /// Gestion de l'evenement FirstPlayerAI.
        /// </summary>
        /// <param name="sender">L'objet ayant emis l'evenement</param>
        /// <param name="e">Les donnees de l'evenement (informations de la partie)</param>
        public void FirstPlayerAIHandler(object sender, BaseEventArgs e)
        {
            GameView.Instance.Game = Games[e.GameId];
            GameView.Instance.OnInitialConstructionFirstRound(new GameStatusArgs(e.GameId, Games[e.GameId], e.IsOnline));
        }

        /// <summary>
        /// Handler d'initialisation des premières construction de colonies en début de partie
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement</param>
        public void InitialColonyHandler(object sender, InitialConstructEventArgs e)
        {
            try
            {

                Game currentGame = Games[e.GameId];
                // Construire la colonie
                if (currentGame.gameGrid.CheckColony(e.BuildingCoordinate))
                {
                    /*
                    currentGame.Players[currentGame.CurrentPlayer].Constructions.Add((e.BuildingCoordinate, ConstructionType.SETTLEMENT));
                    HarborType h = currentGame.gameGrid.Intersections[e.BuildingCoordinate].Harbor;
                    currentGame.gameGrid.Intersections.Remove(e.BuildingCoordinate);
                    currentGame.gameGrid.Intersections.Add(e.BuildingCoordinate, new Intersection(ConstructionType.SETTLEMENT, currentGame.CurrentPlayer, h));
                    */
                    currentGame.gameGrid.ConstructColony(currentGame.players[currentGame.CurrentPlayer], e.BuildingCoordinate, true);
                    if (currentGame.CurrentPhase.Equals(GamePhase.INITIAL_BUILDING_2))
                    {
                        List<ITerrainTile> li = currentGame.gameGrid.GetTileFromIntersection(e.BuildingCoordinate);
                        currentGame.DistributeInitialRessources(li);
                    }
                    UnityEngine.Debug.Log("Before OnConstructionDone ");
                    // MAJ vue
                    GameView.Instance.Game = currentGame;
                    GameView.Instance.OnConstructionDone(new ActionDoneInfoArgs(e.GameId, currentGame, ActionType.SETTLEMENT, true, Games[e.GameId].CurrentPlayer, RessourceType.NONE, RessourceType.NONE, e.IsOnline));
                }
                else
                {
                    UnityEngine.Debug.Log("[INITIAL COLONY HANDLER] Colony coordinate (" + e.BuildingCoordinate + ") not valid");
                    GameView.Instance.OnConstructionDone(new ActionDoneInfoArgs(e.GameId, currentGame, ActionType.SETTLEMENT, false, Games[e.GameId].CurrentPlayer, RessourceType.NONE, RessourceType.NONE, e.IsOnline));
                }
                
                
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex.ToString());
                UnityEngine.Debug.Log(e.BuildingCoordinate);
                GameView.Instance.OnConstructionDone(new ActionDoneInfoArgs(e.GameId, Games[e.GameId], ActionType.SETTLEMENT, false, Games[e.GameId].CurrentPlayer, RessourceType.NONE, RessourceType.NONE, e.IsOnline));
                return;
            }
        }




        /// <summary>
        /// Handler d'initialisation des premières construction de routes en début de partie
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement</param>
        public void InitialRoadHandler(object sender, InitialConstructEventArgs e)
        {
            int player = Games[e.GameId].CurrentPlayer;
            try
            {
                Game currentGame = Games[e.GameId];

                // Construire la route
                /*currentGame.Players[currentGame.CurrentPlayer].Constructions.Add((e.BuildingCoordinate, ConstructionType.ROAD));
                currentGame.gameGrid.Edges.Remove(e.BuildingCoordinate);
                currentGame.gameGrid.Edges.Add(e.BuildingCoordinate, new Edge(ConstructionType.ROAD, currentGame.CurrentPlayer));*/
                currentGame.gameGrid.ConstructRoad(currentGame.players[currentGame.CurrentPlayer], e.BuildingCoordinate, true);
                //currentGame.gameGrid.ConstructRoad(currentGame.players[currentGame.CurrentPlayer], e.BuildingCoordinate);


                if (currentGame.CurrentPhase == GamePhase.INITIAL_BUILDING_1)
                {
                    int nextPlayer = (currentGame.CurrentPlayer + 1) % currentGame.Players.Count;
                    if (currentGame.Players[nextPlayer].VictoryPoints == 1)
                        currentGame.CurrentPhase = GamePhase.INITIAL_BUILDING_2;
                    else
                        currentGame.CurrentPlayer = nextPlayer;


                }
                else if (currentGame.CurrentPhase == GamePhase.INITIAL_BUILDING_2)
                {
                    int nextPlayer = (currentGame.CurrentPlayer + currentGame.Players.Count - 1) % currentGame.Players.Count;
                    if (currentGame.Players[nextPlayer].VictoryPoints == 2)
                        currentGame.CurrentPhase = GamePhase.RECOLT;
                    else
                        currentGame.CurrentPlayer = nextPlayer;
                }


                // MAJ vue
                GameView.Instance.Game = currentGame;
                if (currentGame.CurrentPhase == GamePhase.RECOLT)
                    GameView.Instance.OnHarvestPhaseBegin(new GameStatusArgs(e.GameId, currentGame, IsOnline));

                GameView.Instance.OnConstructionDone(new ActionDoneInfoArgs(e.GameId, currentGame, ActionType.ROAD, true, player, RessourceType.NONE, RessourceType.NONE, IsOnline));


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                UnityEngine.Debug.Log(ex.ToString());
                GameView.Instance.OnConstructionDone(new ActionDoneInfoArgs(e.GameId, Games[e.GameId], ActionType.ROAD, false, player, RessourceType.NONE, RessourceType.NONE, IsOnline));
                return;
            }
            /*
            try
            {
                Game currentGame = Games[e.GameId];

                // Construire la route
               
                currentGame.gameGrid.ConstructRoad(currentGame.players[currentGame.CurrentPlayer], e.BuildingCoordinate);

                // MAJ vue
                GameView.Instance.Game = Games[e.GameId];
                GameView.Instance.OnConstructionDone(new ActionDoneInfoArgs(e.GameId, Games[e.GameId], true));

                if (currentGame.CurrentPhase == GamePhase.INITIAL_BUILDING_1)
                {
                   int nextPlayer = (currentGame.CurrentPlayer + 1) % currentGame.players.Count;
                   if (currentGame.players[nextPlayer].VictoryPoints == 1)
                    currentGame.CurrentPhase = GamePhase.INITIAL_BUILDING_2;
                   else
                    currentGame.CurrentPlayer = nextPlayer;
                }

                if (currentGame.CurrentPhase == GamePhase.INITIAL_BUILDING_2)
                {
                    int nextPlayer = (currentGame.CurrentPlayer - 1) % currentGame.players.Count;
                    if (currentGame.players[nextPlayer].VictoryPoints == 2)
                      currentGame.CurrentPhase = GamePhase.RECOLT;
                    currentGame.CurrentPlayer = nextPlayer;
                }

                // MAJ vue
                GameView.Instance.Game = currentGame;
                GameView.Instance.OnConstructionDone(new ActionDoneInfoArgs(e.GameId, currentGame, true));
                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                GameView.Instance.OnConstructionDone(new ActionDoneInfoArgs(e.GameId, Games[e.GameId], false));
            }
            */
        }
        //...

        /// <summary>
        /// Met a jour le joueur ayant le bonus de points de la route la plus longue.
        /// La route doit etre au minimum d'une longueur de 5.
        /// En cas d'egalite, le joueur possedant deja le bonus le conserve.
        /// </summary>
        /// <param name="gameID">L'identifiant de la partie a mettre a jour</param>
        public void UpdateLongestRoad(Guid gameID)
        {
            Game currGame = Games[gameID];
            Player currPlayer = currGame.players[currGame.CurrentPlayer];
            int playerLongestRoad = currPlayer.LongestRoadLength();
            bool longestRoadFound = false;

            if (playerLongestRoad < 5)
                currPlayer.HasLongestRoad = false;

            if (playerLongestRoad >= 5 && !currPlayer.HasLongestRoad)
            {
                foreach (Player p in currGame.players)
                {
                    if (p.HasLongestRoad)
                    {
                        longestRoadFound = true;
                        if (playerLongestRoad > p.LongestRoadLength())
                        {
                            p.HasLongestRoad = false;
                            currPlayer.HasLongestRoad = true;
                        }
                    }
                }

                if (!longestRoadFound)
                {
                    currPlayer.HasLongestRoad = true;
                }
            }

            Debug.Log("Le joueur " + currGame.CurrentPlayer + " a sa plus longue route de:" + playerLongestRoad);
        }

    }
}
