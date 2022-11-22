using System;
using System.Collections.Generic;
using Noyau.View;
using Util.View;

namespace AI.Model
{
    class RandomAI : AbstractAI
    {
        /*
        private const double ProbUseCard_Harvest = 0.1; // Chevalier : utile pour se débarrasser du brigant/empêcher la récolte d'un adversaire
        private const double ProbUseCard_Exange = 0.1; // Utile pour les négociations
        private const double ProbUseCard_DuringExange = 0.05; // Utile pour les négociations
        */

        /// <value>Probabilité d'utiliser une carte développement lors du début de la phase de construction</value>
        private const double ProbUseCard_Construction = 0.5; // Pour compléter le fruit des négociations

        /// <summary>
        /// Fonction de construction
        /// </summary>
        /// <param name="game">Partie dans laquelle construire</param>
        /// <param name="ai">Joueur IA dans la partie</param>
        private delegate void ConstructMethod(IGame game, IPlayer ai);

        /// <value>Dictionnaire associant type de construction avec méthode de construction</value>
        static private Dictionary<ConstructionType, ConstructMethod> ConstructionsMethods = new Dictionary<ConstructionType, ConstructMethod>()
        {
            { ConstructionType.CITY, ConstructCity },
            { ConstructionType.DEVELOPMENT_CARD, ConstructDevelopmentCard },
            { ConstructionType.NONE, ConstructNone },
            { ConstructionType.ROAD, ConstructRoad },
            { ConstructionType.SETTLEMENT, ConstructSettlement }
        };

        /// <summary>
        /// Utilisation d'une carte développement
        /// </summary>
        /// <param name="game">Partie dans laquelle utiliser la carte</param>
        /// <param name="ai">Joueur IA dans la partie</param>
        private delegate void UseCardMethod(IGame game, IPlayer ai);

        /// <value>Dictionnaire associant type de carte développement avec méthode d'utilisation de carte</value>
        static private Dictionary<CardType, UseCardMethod> UseCardMethods = new Dictionary<CardType, UseCardMethod>()
        {
            { CardType.KNIGHT, UseKnightCard },
            { CardType.RESSOURCE_MONOPOLY, UseMonopolyCard },
            { CardType.RESSOURCE_PAIR, UseResourcePairCard },
            { CardType.ROAD_BUILDING, UseRoadConstructionCard }
        };



        // Construct methods

        /// <summary>
        /// Constuit une ville sur un emplacement aléatoire possible
        /// </summary>
        /// <param name="game">Partie dans laquelle construire</param>
        /// <param name="ai">Joueur IA dans la partie</param>
        static private void ConstructCity(IGame game, IPlayer ai)
        {
            List<Coordinate> possibleCities = game.GameGrid.PossibleCities(ai);

            if (possibleCities.Count != 0)
            {
                Coordinate c = RandomChoice(possibleCities);
                GameView.Instance.OnConstruct(new ConstructEventArgs(game.Id, ConstructionType.CITY, c, IsOnline));
            }
        }

        /// <summary>
        /// Achète une carte de développement
        /// </summary>
        /// <param name="game">Partie dans laquelle acheter la carte</param>
        /// <param name="ai">Joueur IA dans la partie</param>
        static private void ConstructDevelopmentCard(IGame game, IPlayer ai)
        {
            GameView.Instance.OnConstruct(new ConstructEventArgs(game.Id, ConstructionType.DEVELOPMENT_CARD, null, IsOnline));
        }

        /// <summary>
        /// Ne constuit rien
        /// </summary>
        /// <param name="game">Partie dans laquelle ne rien construire</param>
        /// <param name="ai">Joueur IA dans la partie</param>
        static private void ConstructNone(IGame game, IPlayer ai) { }
        static private void ConstructRoad(IGame game, IPlayer ai)
        {
            List<Coordinate> possibleRoads = game.GameGrid.PossibleRoads(ai);

            if (possibleRoads.Count != 0)
            {
                Coordinate c = RandomChoice(possibleRoads);
                GameView.Instance.OnConstruct(new ConstructEventArgs(game.Id, ConstructionType.ROAD, c, IsOnline));
            }
        }

        /// <summary>
        /// Constuit une colonie sur un emplacement aléatoire possible
        /// </summary>
        /// <param name="game">Partie dans laquelle construire</param>
        /// <param name="ai">Joueur IA dans la partie</param>
        static private void ConstructSettlement(IGame game, IPlayer ai)
        {
            List<Coordinate> possibleColonies = game.GameGrid.PossibleColonies(ai);

            if (possibleColonies.Count != 0)
            {
                Coordinate c = RandomChoice(possibleColonies);
                GameView.Instance.OnConstruct(new ConstructEventArgs(game.Id, ConstructionType.SETTLEMENT, c, IsOnline));
            }
        }




        // DevelopmentCard methods

        /// <summary>
        /// Utilisation d'une carte chevalier et déplacement du bandit sur une case aléatoire adjacente à une colonie/ville adverse
        /// </summary>
        /// <param name="game">Partie dans laquelle utiliser la carte</param>
        /// <param name="ai">Joueur IA dans la partie</param>
        static private void UseKnightCard(IGame game, IPlayer ai)
        {
            if (ai.Cards.Count <= 7)
            {
                GameView.Instance.OnKnightCardUse(BanditMoveArgs(game, ai));
            }
        }

        /// <summary>
        /// Utilisation d'une carte monopole et confiscation d'une ressource aléatoire
        /// </summary>
        /// <param name="game">Partie dans laquelle utiliser la carte</param>
        /// <param name="ai">Joueur IA dans la partie</param>
        static private void UseMonopolyCard(IGame game, IPlayer ai)
        {
            GameView.Instance.OnMonopolyCardUse(new MonopolyCardUseEventArgs(game.Id, RandomRessource(), IsOnline));
        }

        /// <summary>
        /// Utilisation d'une carte invention et acquisition d'une carte aléatoire
        /// </summary>
        /// <param name="game">Partie dans laquelle utiliser la carte</param>
        /// <param name="ai">Joueur IA dans la partie</param>
        static private void UseResourcePairCard(IGame game, IPlayer ai)
        {
            GameView.Instance.OnResourcePairCardUse(new ResourcePairCardUseEventArgs(game.Id, RandomRessource(), RandomRessource(), IsOnline));
        }

        /// <summary>
        /// Utilisation d'une carte routes et placement des routes sur des emplaceme,nts aléatoires possibles
        /// </summary>
        /// <param name="game">Partie dans laquelle utiliser la carte</param>
        /// <param name="ai">Joueur IA dans la partie</param>
        static private void UseRoadConstructionCard(IGame game, IPlayer ai)
        {
            List<Coordinate> roads = RandomChoices(game.GameGrid.PossibleRoads(ai), 2);
            GameView.Instance.OnRoadConstructionCardUse(new RoadConstructionCardUseEventArgs(game.Id, roads[0], roads[1], IsOnline));
        }



        // Other private methods

        /// <summary>
        /// Utilisation d'une carte développement avec une probabilité passée en paramètre
        /// </summary>
        /// <param name="game">Partie dans laquelle utiliser une carte</param>
        /// <param name="ai">Joueur IA dans la partie</param>
        /// <param name="probability">Probabilité d'utilisation de la carte</param>
        private void UseDevelopmentCard(IGame game, IPlayer ai, double probability)
        {
            if (NumberOfUsableCards > 0 && !DevelopmentCardUsed && randGen.NextDouble() < probability)
            {
                UseCardMethods[RandomChoice(UsableCards)](game, ai);
            }
        }

        /// <summary>
        /// Choix aléatoire d'un joueur à détrousser et de l'emplacement du bandit
        /// </summary>
        /// <param name="game">Partie dans laquelle utiliser la carte</param>
        /// <param name="ai">Joueur IA dans la partie</param>
        static private BanditMoveEventArgs BanditMoveArgs(IGame game, IPlayer ai)
        {
            int targetId = RandomOtherPlayerId(game, ai);

            // List of possible tiles
            HashSet<Coordinate> tiles = new HashSet<Coordinate>();
            foreach ((Coordinate, ConstructionType) c in game.Players[targetId].Constructions)
            {
                if (c.Item2 == ConstructionType.SETTLEMENT || c.Item2 == ConstructionType.CITY)
                    tiles.UnionWith(CoordinatesTilesFromIntersection(c.Item1, game));
            }

            // Remove current bandit position
            if (tiles.Contains(game.GameGrid.CurrentThiefLocation))
            {
                tiles.Remove(game.GameGrid.CurrentThiefLocation);
            }

            // Remove tiles next to AI's settlements
            foreach (Coordinate c in new List<Coordinate>(tiles))
            {
                if (tiles.Count > 1)
                {
                    List<IIntersection> inters = game.GameGrid.GetIntersectionsFromTile(c);
                    int i;
                    for (i = 0; i < inters.Count && inters[i].PlayerId != ai.Id; i++) ;
                    if (i != inters.Count)
                        tiles.Remove(c);
                }
            }

            return new BanditMoveEventArgs(game.Id, RandomChoice(tiles), targetId, IsOnline);
        }



        // Public methods

        /// <inheritdoc/>
        public override void PlaceFirstColony(IGame game)
        {
            InitAI(game);
            Coordinate c = RandomChoice(PossibleFirstColonies(game));
            GameView.Instance.OnInitialColony(new InitialConstructEventArgs(game.Id, c, IsOnline));
        }

        /// <inheritdoc/>
        public override void PlaceFirstRoad(IGame game)
        {
            Coordinate c = RandomChoice(game.GameGrid.PossibleRoads(ai));
            GameView.Instance.OnInitialRoad(new InitialConstructEventArgs(game.Id, c, IsOnline));
        }

        /// <inheritdoc/>
        public override void PlaceSecondColony(IGame game)
        {
            Coordinate c = RandomChoice(PossibleFirstColonies(game));
            GameView.Instance.OnInitialColony(new InitialConstructEventArgs(game.Id, c, IsOnline));
        }

        /// <inheritdoc/>
        public override void PlaceSecondRoad(IGame game)
        {
            Coordinate c = RandomChoice(game.GameGrid.PossibleRoads(ai));
            GameView.Instance.OnInitialRoad(new InitialConstructEventArgs(game.Id, c, IsOnline));
        }

        /// <inheritdoc/>
        public override void HarvestBegin(IGame game)
        {
            InitTurn();
            //UseDevelopmentCard(game, ai, ProbUseCard_Harvest);
            GameView.Instance.OnDiceThrow(new BaseEventArgs(game.Id, false));
        }

        /// <inheritdoc/>
        public override void Construct(IGame game)
        {
            UseDevelopmentCard(game, ai, ProbUseCard_Construction);
            List<ConstructionType> possibleConstructions = new List<ConstructionType>(ConstructionsMethods.Keys);
            ConstructionType choice;
            bool noConstruction = false;
            while (!noConstruction)
            {
                foreach (ConstructionType ct in new List<ConstructionType>(possibleConstructions))
                {
                    if (ct != ConstructionType.NONE && !ai.HasEnoughRessources(ct))
                    {
                        possibleConstructions.Remove(ct);
                    }
                }

                noConstruction = false;
                if (possibleConstructions.Count != 0)
                {
                    int countPossibleColonies = game.GameGrid.PossibleColonies(ai).Count;
                    int countPossibleCities = game.GameGrid.PossibleCities(ai).Count;
                    if (countPossibleColonies > 0 && (possibleConstructions.Contains(ConstructionType.SETTLEMENT) || randGen.NextDouble() < 0.8))
                    {
                        if (possibleConstructions.Contains(ConstructionType.SETTLEMENT))
                        {
                            choice = ConstructionType.SETTLEMENT;
                        }
                        else
                        {
                            choice = ConstructionType.NONE;
                        }
                    }
                    else if (possibleConstructions.Contains(ConstructionType.CITY) && countPossibleCities > 0)
                    {
                        choice = ConstructionType.CITY;
                    }
                    else if (possibleConstructions.Contains(ConstructionType.DEVELOPMENT_CARD) && randGen.NextDouble() < 0.5)
                    {
                        choice = ConstructionType.DEVELOPMENT_CARD;
                    }
                    else if (possibleConstructions.Contains(ConstructionType.ROAD) && (countPossibleColonies == 0 || randGen.NextDouble() < 0.2))
                    {
                        choice = ConstructionType.ROAD;
                    }
                    else
                    {
                        choice = ConstructionType.NONE;
                        noConstruction = true;
                    }
                    ConstructionsMethods[choice](game, ai);
                }
            }

            GameView.Instance.OnEndPhase(new BaseEventArgs(game.Id, IsOnline));
        }

        /// <inheritdoc/>
        public override void Discard(IGame game)
        {
            List<(int, List<(RessourceType, int)>)> arg;
            if (ai.TotalRessourceNumber > 7)
            {
                arg = new List<(int, List<(RessourceType, int)>)>()
                {
                    (ai.Id, DictionaryToTupleList(RandomChoices(ai.Ressources, ai.TotalRessourceNumber/2)))
                };
            }
            else
            {
                arg = new List<(int, List<(RessourceType, int)>)>()
                {
                    (ai.Id, new List<(RessourceType, int)>())
                };
            }

            GameView.Instance.OnDiscardExtraRessources(new DiscardEventArgs(game.Id, arg, IsOnline));
        }

        /// <inheritdoc/>
        public override void Exchange(IGame game)
        {
            //UseDevelopmentCard(game, ai, ProbUseCard_Exange);
            /*
            double probExchange = 0.1 * System.Math.Min(10.0, (double)ai.TotalRessourceNumber);
            int nbGiftedRessources, nbReceivedRessources;
            Dictionary<RessourceType, int> giftedRessources = new Dictionary<RessourceType, int>(ai.Ressources);
            Dictionary<RessourceType, int> receivedRessources = new Dictionary<RessourceType, int>(ai.Ressources);

            for (int i = 0; randGen.NextDouble() < probExchange && i < 3; i++)
            {
                /*
                if (i != 0)
                {
                    UseDevelopmentCard(game, ai, ProbUseCard_DuringExange);
                }
                *\/

                // init
                nbGiftedRessources = randGen.Next(System.Math.Min(2, ai.TotalRessourceNumber) / 2) + 1;
                nbReceivedRessources = nbGiftedRessources + randGen.Next(3);
                foreach (RessourceType type in ai.Ressources.Keys)
                {
                    giftedRessources[type] = 0;
                    receivedRessources[type] = 0;
                }

                // generate GiftedRessources list
                giftedRessources = RandomChoices(ai.Ressources, nbGiftedRessources);

                // generate ReceivedRessources list
                for (int j = 0; j < nbReceivedRessources; j++)
                {
                    receivedRessources[RandomRessource()]++;
                }

                // Choose a player
                int otherPlayerId = RandomOtherPlayerId(game, ai);
                List<(RessourceType, int)> listGiftedRessources = DictionaryToTupleList(giftedRessources);
                List<(RessourceType, int)> receivedGiftedRessources = DictionaryToTupleList(receivedRessources);

                GameView.Instance.OnPlayerExchange(new PlayerExchangeEventArgs(game.Id, listGiftedRessources, otherPlayerId, receivedGiftedRessources, IsOnline));

                probExchange /= 2.0;
            }
            */

            List<(Coordinate, HarborType)> harbors = new List<(Coordinate, HarborType)>(game.GetCurrentPlayerHarbors());
            HashSet<HarborType> harborsTypes = new HashSet<HarborType>();
            foreach ((Coordinate, HarborType) type in harbors)
            {
                harborsTypes.Add(type.Item2);
            }

            Dictionary<RessourceType, int> probaToGive = new Dictionary<RessourceType, int>();
            Dictionary<RessourceType, int> probaToReceive = new Dictionary<RessourceType, int>();
            int cost = (harborsTypes.Contains(HarborType.GENERAL)) ? 3 : 4;
            bool somethingToGive = false, somethingToReceive = false;
            foreach (RessourceType ressource in ai.Ressources.Keys)
            {
                probaToGive[ressource] = Math.Max(0, ai.Ressources[ressource] - cost);
                somethingToGive |= (probaToGive[ressource] != 0);

                probaToReceive[ressource] = Math.Max(0, 2 - ai.Ressources[ressource]);
                somethingToReceive |= (probaToReceive[ressource] != 0);
                if (harborsTypes.Contains(RessourceToHarbor[ressource]))
                {
                    probaToReceive[ressource] *= 2;
                }
            }

            if (somethingToGive && somethingToReceive)
            {
                RessourceType ressourceToGive = RandomChoice(probaToGive);
                RessourceType ressourceToReceive = RandomChoice(probaToReceive);
                HarborType harbor;
                if (harborsTypes.Contains(RessourceToHarbor[ressourceToReceive]))
                {
                    harbor = RessourceToHarbor[ressourceToReceive];
                }
                else if (harborsTypes.Contains(HarborType.GENERAL))
                {
                    harbor = HarborType.GENERAL;
                }
                else
                {
                    harbor = HarborType.NONE;
                }
                GameView.Instance.OnHarborExchange(new HarborExchangeEventArgs(game.Id, harbor, ressourceToGive, ressourceToReceive, IsOnline));
            }
            GameView.Instance.OnEndPhase(new BaseEventArgs(game.Id, IsOnline));
        }

        /// <inheritdoc/>
        public override void MoveBandit(IGame game)
        {
            GameView.Instance.OnBanditMove(BanditMoveArgs(game, ai));
        }
    }
}