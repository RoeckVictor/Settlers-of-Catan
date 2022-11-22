using System;
using System.Collections.Generic;
using Noyau.View;
using Util.View;
using UnityEngine;

namespace AI.Model
{
    class DifficultAI : AbstractAI
    {
        // Données initialisées au début de la partie

        /// <value>Dictionnaire associant coordonnée de terrain avec son type et sa probabilité de récolte</value>
        private Dictionary<Coordinate, (RessourceType, double)> ProbasHarvest = new Dictionary<Coordinate, (RessourceType, double)>();

        /// <summary>
        /// Initialise le dictionnaire ProbasHarvest
        /// </summary>
        private void EvaluateProbasHarvest()
        {
            foreach (KeyValuePair<Coordinate, ITerrainTile> c in game.GameGrid.TerrainTiles)
            {
                ProbasHarvest[c.Key] = (TerrainToRessource[c.Value.Type], ProbaHarvest(c.Value.DiceProductionNumber));
            }
        }

        /// <summary>
        /// Convertit le score aux dés à obtenir pour une récolte en une probabilité de récolte
        /// </summary>
        /// <param name="score">Score aux dés à obtenir pour récolter une ressource</param>
        /// <returns>La probabilité de récolte</returns>
        private double ProbaHarvest(int score)
        {
            return (6.0 - Math.Abs(7.0 - (double)score)) / 36.0;
        }


        // Données mises à jour à chaque début de phase

        /// <value>Copie locale des ressources du joueur</value>
        private Dictionary<RessourceType, int> Ressources = new Dictionary<RessourceType, int>();
        /// <value>Copie locale des intersections de la grille de jeu</value>
        private Dictionary<Coordinate, IIntersection> Intersections = new Dictionary<Coordinate, IIntersection>();
        /// <value>Copie locale des arêtes de la grille de jeu</value>
        private Dictionary<Coordinate, IEdge> Edges = new Dictionary<Coordinate, IEdge>();
        /// <value>Copie locale des constructions de l'IA</value>
        private List<(Coordinate, ConstructionType)> Constructions = new List<(Coordinate, ConstructionType)>();
        /// <value>Copie locale du nombre de points de victoire de l'IA</value>
        private int VictoryPoints = 0;
        /// <value>Copie locale du nombre de points de victoire de l'IA</value>
        private Dictionary<IPlayer, int> LongestRoads = new Dictionary<IPlayer, int>();

        /// <summary>
        /// Met à jour les copies locales des données de la partie
        /// </summary>
        private void UpdateDatas()
        {
            VictoryPoints = ai.VictoryPoints;
            Ressources = new Dictionary<RessourceType, int>(ai.Ressources);
            Constructions = new List<(Coordinate, ConstructionType)>(ai.Constructions);

            Intersections.Clear();
            foreach (KeyValuePair<Coordinate, IIntersection> inter in game.GameGrid.Intersections)
            {
                Intersections.Add(inter.Key, inter.Value.CreateCopy());
            }

            Edges.Clear();
            foreach (KeyValuePair<Coordinate, IEdge> edge in game.GameGrid.Edges)
            {
                Edges.Add(edge.Key, edge.Value.CreateCopy());
            }

            foreach (IPlayer player in game.Players)
            {
                LongestRoads[player] = player.LongestRoadLength();
            }

            /*
            foreach ((Coordinate, ConstructionType) c in game.Players[0].Constructions)
            {
                switch(c.Item2)
                {
                    case ConstructionType.CITY:
                        Intersections[c.Item1].PlayerId = 0;
                        break;
                    case ConstructionType.SETTLEMENT:
                        Intersections[c.Item1].PlayerId = 0;
                        break;
                    case ConstructionType.ROAD:
                        Edges[c.Item1].PlayerId = 0;
                        break;
                }
            }
            */
        }

        /// <summary>
        /// Renvoie le nombre de ressources présentes dans la copie locale de Ressources
        /// </summary>
        /// <returns>Le nombre total de ressources</returns>
        private int RessourcesCount()
        {
            int count = 0;
            foreach (int quantity in Ressources.Values)
            {
                count += quantity;
            }
            return count;
        }

        /// <summary>
        /// Renvoie la liste des intersections adjacentes à un terrain
        /// </summary>
        /// <param name="t">Coordonnée du terrain</param>
        /// <returns>La liste des intersections adjacentes au terrain</returns>
        public List<IIntersection> GetIntersectionsFromTile(Coordinate t)
        {
            return new List<IIntersection>()
            {
                Intersections[new Coordinate(t.X, t.Y, t.Z, Direction.UP)],
                Intersections[new Coordinate(t.X, t.Y, t.Z, Direction.DOWN)],
                Intersections[new Coordinate(t.X + 1, t.Y, t.Z - 1, Direction.DOWN)],
                Intersections[new Coordinate(t.X, t.Y + 1, t.Z - 1, Direction.DOWN)],
                Intersections[new Coordinate(t.X - 1, t.Y, t.Z + 1, Direction.UP)],
                Intersections[new Coordinate(t.X, t.Y - 1, t.Z + 1, Direction.UP)]
            };
        }



        // Données recalculées plusieurs fois par phase

        /// <value>Scores de récolte et d'accès des ressources pour l'IA</value>
        private Dictionary<RessourceType, double> RessourcesAccess = new Dictionary<RessourceType, double>();
        /// <value>Scores de besoin des ressources pour l'IA</value>
        private Dictionary<RessourceType, double> RessourcesNeeds = new Dictionary<RessourceType, double>();
        /// <value>Ressource la plus demandée par l'IA</value>
        private (RessourceType, double) maxRessourceNeeded;
        /// <value>Associe les coordonnées des intersections sur lequelles l'IA pourrait, à terme, placer une colonie, avec le nombre de routes à construire pour l'atteindre</value>
        private Dictionary<Coordinate, int> IntersectionsDistances = new Dictionary<Coordinate, int>();
        /// <value>Arbre des chemins les plus courts pour atteindre une intersection</value>
        private Dictionary<Coordinate, Coordinate> Paths = new Dictionary<Coordinate, Coordinate>();
        /// <value>Coordonnées et la distance de la meilleure intersection évaluée par l'IA</value>
        private Coordinate BestIntersection;
        /// <value>Score des intersections</value>
        private Dictionary<Coordinate, double> IntersectionsScores = new Dictionary<Coordinate, double>();
        /// <value>Liste des ports accessibles pour l'IA</value>
        private HashSet<HarborType> harbors = new HashSet<HarborType>();

        /// <summary>
        /// Met à jour les scores de récolte et d'accès des ressources pour l'IA
        /// </summary>
        private void EvaluateRessourcesAccess()
        {

            foreach (RessourceType type in Ressources.Keys)
            {
                RessourcesAccess[type] = 0.0; // 1.0 * (double)Ressources[type] / 5.0;
            }

            RessourcesAccess[RessourceType.NONE] = 0.1;

            foreach ((Coordinate, ConstructionType) c in Constructions)
            {
                double coeff;
                if (c.Item2 == ConstructionType.SETTLEMENT)
                {
                    coeff = 1.0;
                }
                else if (c.Item2 == ConstructionType.CITY)
                {
                    coeff = 2.0;
                }
                else
                {
                    coeff = 0.0;
                }

                if (coeff != 0.0)
                {
                    foreach (Coordinate coordTile in CoordinatesTilesFromIntersection(c.Item1, game))
                    {
                        RessourceType type = ProbasHarvest[coordTile].Item1;
                        double proba = ProbasHarvest[coordTile].Item2;
                        if (type != RessourceType.NONE)
                        {
                            RessourcesAccess[type] += coeff * proba;
                        }
                    }

                    HarborType harbor = Intersections[c.Item1].Harbor;
                    if (harbor != HarborType.NONE)
                    {
                        RessourcesAccess[HarborToRessource[harbor]] += 0.2;
                    }
                }
            }
        }

        /// <summary>
        /// Renvoie l'indice de rareté d'une ressource pour l'IA
        /// </summary>
        /// <param name="type">Le type de ressource à évaluer</param>
        /// <returns>L'indice de rareté de la ressource</returns>
        private double RessourceRarity(RessourceType type)
        {
            if (type == RessourceType.NONE)
            {
                return 0.0;
            }
            else if (RessourcesAccess[type] < 0.05)
            {
                return 20.0;
            }
            else
            {
                return 1.0 / RessourcesAccess[type];
            }
        }

        /// <summary>
        /// Met à jour le nombre de route à construire pour atteindre chaque intersection
        /// </summary>
        private void EvaluateIntersectionsDistances()
        {
            IntersectionsDistances.Clear();
            Paths.Clear();

            // Valeur pendant la phase d'initialisation
            if (Constructions.Count == 0 || Constructions.Count == 2)
            {
                foreach (Coordinate c in PossibleFirstColonies(game))
                {
                    IntersectionsDistances[c] = 0;
                }
                return;
            }

            // Initialisation avec toutes les coordonnées à distance 0
            foreach ((Coordinate, ConstructionType) construction in Constructions)
            {
                switch (construction.Item2)
                {
                    case ConstructionType.CITY:
                        IntersectionsDistances.Add(construction.Item1, 0);
                        Paths.Add(construction.Item1, construction.Item1);
                        break;
                    case ConstructionType.SETTLEMENT:
                        IntersectionsDistances.Add(construction.Item1, 0);
                        Paths.Add(construction.Item1, construction.Item1);
                        break;
                    case ConstructionType.ROAD:
                        Coordinate c1, c2;
                        (c1, c2) = IntersectionsFromEdge(construction.Item1);
                        if (Intersections[c1].PlayerId == -1 && !IntersectionsDistances.ContainsKey(c1))
                        {
                            IntersectionsDistances.Add(c1, 0);
                            Paths.Add(c1, c1);
                        }
                        if (Intersections[c2].PlayerId == -1 && !IntersectionsDistances.ContainsKey(c2))
                        {
                            IntersectionsDistances.Add(c2, 0);
                            Paths.Add(c2, c2);
                        }
                        break;
                }
            }

            // Propagation des coordonnées incluses dans IntersectionsDistances et Paths
            HashSet<Coordinate> newCoords = new HashSet<Coordinate>(IntersectionsDistances.Keys), lastCoords = new HashSet<Coordinate>(), tmp;
            for (int i = 1; newCoords.Count > 0; i++)
            {
                // Echange des collections
                tmp = newCoords;
                newCoords = lastCoords;
                lastCoords = tmp;

                // Ajout des coordonnées
                newCoords.Clear();
                foreach (Coordinate c in lastCoords)
                {
                    foreach (Coordinate neighbour in Neighbours(c, game))
                    {
                        if (!IntersectionsDistances.ContainsKey(neighbour) && Intersections[neighbour].PlayerId == -1 && Edges[EdgeFromIntersections(c, neighbour)].PlayerId == -1)
                        {
                            IntersectionsDistances.Add(neighbour, i);
                            Paths.Add(neighbour, c);
                            newCoords.Add(neighbour);
                        }
                    }
                }
            }

            // Suppression des coordonnées non constructibles
            foreach ((Coordinate, ConstructionType) construction in Constructions)
            {
                if (construction.Item2 == ConstructionType.SETTLEMENT || construction.Item2 == ConstructionType.CITY)
                {
                    if (construction.Item2 == ConstructionType.CITY)
                    {
                        IntersectionsDistances.Remove(construction.Item1);
                    }
                    foreach (Coordinate c in Neighbours(construction.Item1, game))
                    {
                        if (IntersectionsDistances.ContainsKey(c))
                        {
                            IntersectionsDistances.Remove(c);
                        }
                    }
                }
            }
            foreach (IPlayer player in game.Players)
            {
                if (player.Id != ai.Id)
                {
                    foreach ((Coordinate, ConstructionType) construction in player.Constructions)
                    {
                        if (construction.Item2 == ConstructionType.SETTLEMENT || construction.Item2 == ConstructionType.CITY)
                        {
                            foreach (Coordinate c in Neighbours(construction.Item1, game))
                            {
                                if (IntersectionsDistances.ContainsKey(c))
                                {
                                    IntersectionsDistances.Remove(c);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Evaluation des scores attribués à chaque intersection pour construire une colonie/ville
        /// </summary>
        private void EvaluateIntersectionsScores()
        {
            EvaluateIntersectionsDistances();
            double bestScore = 0.0;

            IntersectionsScores.Clear();
            BestIntersection = null;
            foreach (Coordinate c in IntersectionsDistances.Keys)
            {
                // Calcul de la valeur
                EvaluateIntersectionValue(c);

                // Calcul du coût
                double cost;
                //Debug.Log("colo : " + (RessourceRarity(RessourceType.BRICK) + RessourceRarity(RessourceType.LUMBER) + RessourceRarity(RessourceType.WHEAT) + RessourceRarity(RessourceType.WOOL)));
                //Debug.Log("ville : " + ((2.0 * RessourceRarity(RessourceType.WHEAT) * RessourceRarity(RessourceType.WHEAT) + 3.0 * RessourceRarity(RessourceType.ORE) * RessourceRarity(RessourceType.ORE)) / 10.0));
                if (Intersections[c].Building != ConstructionType.SETTLEMENT)
                {
                    // Colonie
                    cost = RessourceRarity(RessourceType.BRICK) + RessourceRarity(RessourceType.LUMBER) + RessourceRarity(RessourceType.WHEAT) + RessourceRarity(RessourceType.WOOL);
                    cost += (double)IntersectionsDistances[c] * (RessourceRarity(RessourceType.BRICK) + RessourceRarity(RessourceType.LUMBER));
                }
                else // if (Intersections[c].PlayerId == ai.Id)
                {
                    // Ville
                    cost = (2.0 * RessourceRarity(RessourceType.WHEAT) * RessourceRarity(RessourceType.WHEAT) + 3.0 * RessourceRarity(RessourceType.ORE) * RessourceRarity(RessourceType.ORE)) / 10.0;
                }

                IntersectionsScores[c] /= cost;

                if (IntersectionsScores[c] > bestScore)
                {
                    BestIntersection = c;
                    bestScore = IntersectionsScores[c];
                }
            }
        }

        /// <summary>
        /// Evaluation de la valeur d'une intersection
        /// </summary>
        /// <param name="c">La coordonnée de l'intersection</param>
        private void EvaluateIntersectionValue(Coordinate c)
        {
            IDictionary<Coordinate, IIntersection> inters = Intersections;
            IDictionary<Coordinate, ITerrainTile> tiles = game.GameGrid.TerrainTiles;

            IntersectionsScores[c] = 0.0;
            // Calcul de la rentabilité
            foreach (Coordinate coordTile in CoordinatesTilesFromIntersection(c, game))
            {
                if (TerrainToRessource[tiles[coordTile].Type] != RessourceType.NONE)
                {
                    IntersectionsScores[c] += ProbasHarvest[coordTile].Item2 * RessourcesNeeds[TerrainToRessource[tiles[coordTile].Type]];
                }
            }

            // Bonus pour la présence de port
            if (inters[c].PlayerId == -1 && !harbors.Contains(inters[c].Harbor))
            {
                if (inters[c].Harbor == HarborType.GENERAL)
                {
                    if (Constructions.Count < 4)
                        IntersectionsScores[c] += 0.4;
                    else
                        IntersectionsScores[c] += 0.1;
                }
                else if (inters[c].Harbor != HarborType.NONE)
                {
                    if (Constructions.Count == 0)
                        IntersectionsScores[c] += 0.03 * RessourcesNeeds[HarborToRessource[inters[c].Harbor]];
                    else if (Constructions.Count == 2)
                        IntersectionsScores[c] += 0.04 * RessourcesNeeds[HarborToRessource[inters[c].Harbor]];
                    else
                        IntersectionsScores[c] += 0.05 * RessourcesNeeds[HarborToRessource[inters[c].Harbor]];
                }
            }

            IntersectionsScores[c] += (double)VictoryPoints / 5.0;
        }

        /// <value>Vaut true s'il reste des emplacements de routes constuctibles pour l'IA. Réévalué à chaque appel de PossibleRoad.</value>
        private bool RoadConstructPossible = true;

        /// <summary>
        /// Renvoie l'ensemble des routes directement constructibles pour l'IA
        /// </summary>
        /// <returns>Un ensemble contenant les routes directement constructibles</returns>
        private HashSet<Coordinate> PossibleRoads()
        {
            if (!RoadConstructPossible)
            {
                return null;
            }

            HashSet<Coordinate> ownedIntersections = new HashSet<Coordinate>();
            // Initialisation avec toutes les coordonnées à distance 0
            foreach ((Coordinate, ConstructionType) construction in Constructions)
            {
                switch (construction.Item2)
                {
                    case ConstructionType.CITY:
                        ownedIntersections.Add(construction.Item1);
                        break;
                    case ConstructionType.SETTLEMENT:
                        ownedIntersections.Add(construction.Item1);
                        break;
                    case ConstructionType.ROAD:
                        Coordinate c1, c2;
                        (c1, c2) = IntersectionsFromEdge(construction.Item1);
                        if (Intersections[c1].PlayerId == -1 && !ownedIntersections.Contains(c1))
                        {
                            ownedIntersections.Add(c1);
                        }
                        if (Intersections[c2].PlayerId == -1 && !ownedIntersections.Contains(c2))
                        {
                            ownedIntersections.Add(c2);
                        }
                        break;
                }
            }

            HashSet<Coordinate> possibleRoads = new HashSet<Coordinate>();
            foreach (Coordinate c in ownedIntersections)
            {
                foreach (Coordinate neighbour in Neighbours(c, game))
                {
                    if (!ownedIntersections.Contains(neighbour))
                    {
                        Coordinate edge = EdgeFromIntersections(c, neighbour);
                        if (Edges[edge].PlayerId == -1 && !possibleRoads.Contains(edge))
                        {
                            possibleRoads.Add(edge);
                        }
                    }
                }
            }

            RoadConstructPossible = (possibleRoads.Count > 0);
            return (possibleRoads.Count == 0) ? null : possibleRoads;
        }

        // Non implémenté
        /*
        private enum Action { None, Construct, ExpandRoad, BuyCard };
        private List<ConstructionType> ConstructionsNeeds = new List<ConstructionType>();
        private Dictionary<CardType, double> CardsScores = new Dictionary<CardType, double>()
        {
            { CardType.KNIGHT, 0.0 },
            { CardType.RESSOURCE_MONOPOLY, 0.0 },
            { CardType.RESSOURCE_PAIR, 0.0 },
            { CardType.ROAD_BUILDING, 0.0 },
            { CardType.VICTORY_POINT, 0.0 }
        };
        private Dictionary<Action, double> ActionsScores = new Dictionary<Action, double>()
        {
            { Action.BuyCard, 0.0 },
            { Action.Construct, 0.0 },
            { Action.ExpandRoad, 0.0 },
        };

        private void EvaluateRessourcesNeeds()
        {
            maxRessourceNeeded = (RessourceType.NONE, 0.0);
            foreach (RessourceType type in Ressources.Keys)
            {
                RessourcesNeeds[type] = RessourceRarity(type);
                if (RessourcesNeeds[type] > maxRessourceNeeded.Item2)
                {
                    maxRessourceNeeded = (type, RessourcesNeeds[type]);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                UpdateActionsScore();
                UpdateConstructionsNeeds();
                UpdateRessourcesNeeds();
            }
        }

        private void UpdateActionsScore()
        {
            // Evaluation du score de l'action "Construct"
            EvaluateIntersectionsScores();
            ActionsScores[Action.Construct] += VictoryPoints;

            // Evaluation du score de l'action "Expand Road"

            // Evaluation du score de l'action "Buy Card"
            //Σtype_de_carte(Score(carte) x Probabilité_d’obtention(carte)) / Σressource_à_payer(Quantité_à_payer(ressource) x Rareté(ressource))
            EvaluateCardsScores();
        }

        static private int CompareActions((Action, double) action1, (Action, double) action2)
        {
            if (action1.Item2 > action2.Item2)
                return 1;
            else if (action1.Item2 < action2.Item2)
                return -1;
            else
                return 0;
        }
        private void UpdateConstructionsNeeds()
        {
            List<(Action, double)> bestActions = new List<(Action, double)>();
            foreach (KeyValuePair<Action, double> action in ActionsScores)
            {
                bestActions.Add((action.Key, action.Value));
            }

            bestActions.Sort(CompareActions);

            ConstructionsNeeds.Clear();
            for (int action = 0; ConstructionsNeeds.Count < 3;)
            {
                switch (bestActions[action].Item1)
                {
                    case Action.BuyCard:
                        ConstructionsNeeds.Add(ConstructionType.DEVELOPMENT_CARD);
                        action++;
                        break;
                    case Action.Construct:
                        if (Intersections[BestIntersection].PlayerId == ai.Id)
                        {
                            ConstructionsNeeds.Add(ConstructionType.CITY);
                        }
                        else
                        {
                            for (int i = 0; i < 3 && i < BestIntersection.Item2; i++)
                            {
                                ConstructionsNeeds.Add(ConstructionType.ROAD);
                            }
                            if (ConstructionsNeeds.Count < 3)
                            {
                                ConstructionsNeeds.Add(ConstructionType.SETTLEMENT);
                            }
                        }
                        action++;
                        break;
                    case Action.ExpandRoad:
                        // A compléter
                        action++;
                        break;
                    default:
                        while (ConstructionsNeeds.Count < 3)
                        {
                            ConstructionsNeeds.Add(ConstructionType.NONE);
                        }
                        break;
                }
            }
        }
        void EvaluateRessourceNeed(Dictionary<RessourceType, int> ressources, RessourceType type, int quantityNeeded, int priority)
        {
            RessourcesNeeds[type] = RessourceRarity(type) * (double)Math.Max(0, quantityNeeded - ressources[type]) / Math.Pow(2, priority);
            ressources[type] = Math.Max(0, ressources[type] - quantityNeeded);
        }
        private void UpdateRessourcesNeeds()
        {
            foreach (RessourceType type in RessourcesNeeds.Keys)
            {
                RessourcesNeeds[type] = 0.0;
            }

            Dictionary<RessourceType, int> ressources = new Dictionary<RessourceType, int>(Ressources);
            for (int i = 0; i < ConstructionsNeeds.Count; i++)
            {
                switch (ConstructionsNeeds[i])
                {
                    case ConstructionType.CITY:
                        EvaluateRessourceNeed(ressources, RessourceType.WHEAT, 2, i);
                        EvaluateRessourceNeed(ressources, RessourceType.ORE, 3, i);
                        break;
                    case ConstructionType.DEVELOPMENT_CARD:
                        EvaluateRessourceNeed(ressources, RessourceType.WHEAT, 1, i);
                        EvaluateRessourceNeed(ressources, RessourceType.WOOL, 1, i);
                        EvaluateRessourceNeed(ressources, RessourceType.ORE, 1, i);
                        break;
                    case ConstructionType.ROAD:
                        EvaluateRessourceNeed(ressources, RessourceType.LUMBER, 1, i);
                        EvaluateRessourceNeed(ressources, RessourceType.BRICK, 1, i);
                        break;
                    case ConstructionType.SETTLEMENT:
                        EvaluateRessourceNeed(ressources, RessourceType.LUMBER, 1, i);
                        EvaluateRessourceNeed(ressources, RessourceType.BRICK, 1, i);
                        EvaluateRessourceNeed(ressources, RessourceType.WHEAT, 1, i);
                        EvaluateRessourceNeed(ressources, RessourceType.WOOL, 1, i);
                        break;
                    default:
                        break;
                }
            }
        }
        private void EvaluateCardsScores()
        {
            IHexGrid grid = game.GameGrid;

            // Carte chevalier
            CardsScores[CardType.KNIGHT] = 0.0;
            ITerrainTile thiefTile = grid.TerrainTiles[grid.CurrentThiefLocation];
            foreach (IIntersection inter in grid.GetIntersectionsFromTile(grid.CurrentThiefLocation))
            {
                if (inter.PlayerId == ai.Id)
                {
                    CardsScores[CardType.KNIGHT] += ProbasHarvest[grid.CurrentThiefLocation].Item2 * RessourcesNeeds[TerrainToRessource[thiefTile.Type]];
                }
            }
            CardsScores[CardType.KNIGHT] += 1.0; // Valeur estimée du gain de la carte à prendre, à rééquilibrer
            foreach (IPlayer player in game.Players)
            {
                if (player.Cards.Count > 6 && player.Id != ai.Id)
                {
                    CardsScores[CardType.KNIGHT] += (double)(player.Cards.Count / 2) * 0.5; // Valeur estimée de la défausse des cartes des adversaires, à rééquilibrer
                }
            }

            // Carte Monopole
            CardsScores[CardType.RESSOURCE_MONOPOLY] = 0.0;
            foreach (IPlayer player in game.Players)
            {
                CardsScores[CardType.RESSOURCE_MONOPOLY] += (double)player.Cards.Count / 5.0 * (maxRessourceNeeded.Item2 + 0.5); // Valeur estimée de la récolte, à rééquilibrer
            }

            // Carte Invention
            CardsScores[CardType.RESSOURCE_PAIR] = RessourcesNeeds[maxRessourceNeeded.Item1] * 2.0;

            // Carte Route
            // A COMPLETER

            // Carte Point de victoire
            CardsScores[CardType.VICTORY_POINT] = VictoryPoints;
        }

        // Besoin en points de victoire
        private double EvaluateVictoryPointsNeeds()
        {
            return (double)ai.VictoryPoints;
        }

        private void ConstructRoad()
        {
            Coordinate edge;
            if (ActionsScores[Action.Construct] > ActionsScores[Action.ExpandRoad])
            {
                Coordinate intersection;
                intersection = BestIntersection;
                while (IntersectionsDistances[intersection] > 1)
                {
                    intersection = Paths[intersection];
                }
                edge = EdgeFromIntersections(Paths[intersection], intersection);
            }
            else
            {
                edge = new Coordinate();
            }
            GameView.Instance.OnConstruct(new ConstructEventArgs(game.Id, ConstructionType.ROAD, edge, IsOnline));
        }
        private void ConstructSettlement()
        {
            if (Intersections[BestIntersection].PlayerId == ai.Id)
            {
                GameView.Instance.OnConstruct(new ConstructEventArgs(game.Id, ConstructionType.CITY, BestIntersection, IsOnline));
            }
            else
            {
                GameView.Instance.OnConstruct(new ConstructEventArgs(game.Id, ConstructionType.SETTLEMENT, BestIntersection, IsOnline));
            }
        }

        private void ConstructDevelopmentCard()
        {
            GameView.Instance.OnConstruct(new ConstructEventArgs(game.Id, ConstructionType.DEVELOPMENT_CARD, null, IsOnline));
        }

        private void UseKnightCard()
        {
            Dictionary<Coordinate, int> scores = new Dictionary<Coordinate, int>();
            foreach (Coordinate c in game.GameGrid.TerrainTiles.Keys)
            {
                scores[c] = 0;
            }

            foreach (IPlayer player in game.Players)
            {
                if (player.Id != ai.Id)
                {
                    foreach ((Coordinate, ConstructionType) c in player.Constructions)
                    {
                        switch (c.Item2)
                        {
                            case ConstructionType.SETTLEMENT:
                                scores[c.Item1]++;
                                break;
                            case ConstructionType.CITY:
                                scores[c.Item1] += 2;
                                break;
                        }
                    }
                }
            }

            foreach ((Coordinate, ConstructionType) c in Constructions)
            {
                if (c.Item2 == ConstructionType.SETTLEMENT || c.Item2 == ConstructionType.CITY)
                {
                    scores[c.Item1] = 0;
                }
            }

            (Coordinate, double) bestTile = (null, 0.0);
            foreach (KeyValuePair<Coordinate, int> c in scores)
            {
                double score = (double)scores[c.Key] * ProbasHarvest[c.Key].Item2;
                if (score < bestTile.Item2)
                {
                    bestTile = (c.Key, c.Value);
                }
            }

            (int, int) bestPlayer = (0, 0);
            foreach (IIntersection inter in GetIntersectionsFromTile(bestTile.Item1))
            {
                int victoryPoints = game.Players[inter.PlayerId].VictoryPoints;
                if (inter.PlayerId != -1 && victoryPoints > bestPlayer.Item2)
                {
                    bestPlayer = (inter.PlayerId, victoryPoints);
                }
            }

            GameView.Instance.OnKnightCardUse(new BanditMoveEventArgs(game.Id, bestTile.Item1, bestPlayer.Item1, IsOnline));
        }

        private bool IsBestScore<K>(double score, IDictionary<K, double> scores)
        {
            bool isBest = true;
            foreach (double sc in scores.Values)
            {
                if (sc > score)
                {
                    isBest = false;
                }
            }
            return isBest;
        }

        string IntersectionToString(Coordinate c)
        {
            List<Coordinate> tiles = CoordinatesTilesFromIntersection(c, game);
            string str = "(" + c.X + " " + c.Y + " " + c.Z + " " + c.D + ") (Player n°" + Intersections[c].PlayerId + " - Type: " + Intersections[c].Building + " - Harbor: " + Intersections[c].Harbor + ")";
            foreach (Coordinate coord in tiles)
            {
                str += " (" + game.GameGrid.TerrainTiles[coord].Type + " " + game.GameGrid.TerrainTiles[coord].DiceProductionNumber + ")";
            }
            return str;
        }

        string EdgeToString(Coordinate c)
        {
            Coordinate c1, c2;
            (c1, c2) = IntersectionsFromEdge(c);
            HashSet<Coordinate> tiles = new HashSet<Coordinate>(CoordinatesTilesFromIntersection(c1, game));
            tiles.IntersectWith(CoordinatesTilesFromIntersection(c2, game));
            string str = "(" + c.X + " " + c.Y + " " + c.Z + " " + c.D + ") (Player n°" + Edges[c].PlayerId + " - Type: " + Edges[c].Building + ")";
            foreach (Coordinate coord in tiles)
            {
                str += " (" + game.GameGrid.TerrainTiles[coord].Type + " " + game.GameGrid.TerrainTiles[coord].DiceProductionNumber + ")";
            }
            return str;
        }

        private struct NodePath
        {
            public NodePath(Coordinate closeNeighbour, Coordinate farNeighbour = null, int distanceNeighbour = 0, List<Coordinate> longestPath = null, int pathLength = 0)
            {
                CloseNeighbour = closeNeighbour;
                FarNeighbour = farNeighbour;
                DistanceNeighbour = distanceNeighbour;
                LongestPath = longestPath;
                PathLength = pathLength;
            }
            public Coordinate CloseNeighbour;
            public Coordinate FarNeighbour;
            public List<Coordinate> LongestPath;
            public int DistanceNeighbour;
            public int PathLength;
        }
        
        private struct Node
        {
            public Node(List<NodePath> paths = null, List<Coordinate> longestPath = null, int pathLength = 0)
            {
                if(paths == null)
                {
                    Paths = new List<NodePath>();
                }
                else
                {
                    Paths = paths;
                }

                LongestPath = longestPath;
                PathLength = pathLength;
            }
            public List<NodePath> Paths;
            public List<Coordinate> LongestPath;
            public int PathLength;
        }

        
        void EvaluateExpandRoadScores()
        {
            Dictionary<Coordinate, Node> graph = new Dictionary<Coordinate, Node>();
            HashSet<Coordinate> simplifiedGraph = new HashSet<Coordinate>();

            // Ajout des coordonnées desservies par des routes
            foreach ((Coordinate, ConstructionType) c in Constructions)
            {
                if(c.Item2 == ConstructionType.ROAD)
                {
                    Coordinate c1, c2;
                    (c1, c2) = IntersectionsFromEdge(c.Item1);
                    if(!graph.ContainsKey(c1))
                    {
                        graph.Add(c1, new Node());
                    }
                    if (!graph.ContainsKey(c2))
                    {
                        graph.Add(c2, new Node());
                    }
                }
            }

            // Liaison des voisins et obtention des coordonnées des sommets du graphe simplifié
            List<Coordinate> graphKeys = new List<Coordinate>(graph.Keys);
            foreach (Coordinate c in graphKeys)
            {
                foreach(Coordinate neighbour in Neighbours(c, game))
                {
                    if(graph.ContainsKey(neighbour))
                    {
                        graph[c].Paths.Add(new NodePath(neighbour));
                    }
                }

                if(graph[c].Paths.Count % 2 == 1)
                {
                    simplifiedGraph.Add(c);
                }
            }

            // Liaison des sommets aux voisins distants
            foreach(Coordinate c in simplifiedGraph)
            {
                for(int i = 0; i < graph[c].Paths.Count; i++)
                {
                    if(graph[c].Paths[i].DistanceNeighbour != 0)
                    {
                        // Liaisons pour chaque sommet du chemin
                        Coordinate current = c, neighbour = graph[c].Paths[i].CloseNeighbour;
                        int distanceNeighbour;
                        for (distanceNeighbour = 0; !graph.ContainsKey(neighbour); distanceNeighbour++)
                        {
                            if(graph[neighbour].Paths[0].CloseNeighbour == current)
                            {
                                graph[neighbour].Paths[0] = new NodePath(current, c, distanceNeighbour);
                                current = neighbour;
                                neighbour = graph[neighbour].Paths[1].CloseNeighbour;
                            }
                            else
                            {
                                graph[neighbour].Paths[1] = new NodePath(current, c, distanceNeighbour);
                                current = neighbour;
                                neighbour = graph[neighbour].Paths[0].CloseNeighbour;
                            }
                        }

                        // Liaison pour le sommet final
                        for (int j = 0; j < graph[neighbour].Paths.Count; j++)
                        {
                            if(graph[neighbour].Paths[i].CloseNeighbour == current)
                            {
                                graph[neighbour].Paths[i] = new NodePath(current, c, distanceNeighbour);
                            }
                        }

                        // Liaison pour le sommet initial
                        graph[c].Paths[i] = new NodePath(graph[c].Paths[i].CloseNeighbour, neighbour, distanceNeighbour);
                    }
                }
            }

            // Evaluation des distances maximales
            foreach(Coordinate c in simplifiedGraph)
            {
                for(int i = 0; i < graph[c].Paths.Count; i++)
                {
                    (List<Coordinate>, int) longestPath = LongestPath(graph, c, graph[c].Paths[i].CloseNeighbour, new HashSet<Coordinate>() { c });
                    graph[c].Paths[i] = new NodePath(graph[c].Paths[i].CloseNeighbour, graph[c].Paths[i].FarNeighbour, graph[c].Paths[i].DistanceNeighbour, longestPath.Item1, longestPath.Item2);
                    
                    if(graph[c].LongestPath == null || longestPath.Item2 > graph[c].PathLength)
                    {
                        graph[c] = new Node(graph[c].Paths, graph[c].Paths[i].LongestPath, longestPath.Item2);
                    }
                }
            }

            // Propagation des résultats aux intersections du graphe non simplifié
            foreach(Coordinate c in graphKeys)
            {
                if(!simplifiedGraph.Contains(c))
                {
                    for(int i = 0; i < graph[c].Paths.Count; i++)
                    {
                        Coordinate neighbour = graph[c].Paths[i].FarNeighbour;
                        List<Coordinate> path = new List<Coordinate>(graph[neighbour].LongestPath);
                        path.Add(neighbour);
                        graph[c].Paths[i] = new NodePath(graph[c].Paths[i].CloseNeighbour, neighbour, graph[c].Paths[i].DistanceNeighbour, path, graph[neighbour].PathLength + graph[c].Paths[i].DistanceNeighbour);

                        if (graph[c].LongestPath == null || graph[c].Paths[i].PathLength > graph[c].PathLength)
                        {
                            graph[c] = new Node(graph[c].Paths, path, graph[c].Paths[i].PathLength);
                        }
                    }
                }
            }

            // Pour chaque intersection du graphe non simplifié, tester la construction de n routes.
            foreach(Coordinate c in graphKeys)
            {
                Coordinate bestTarget = BestTargetRoad()
            }
        }

        (List<Coordinate>, int) LongestPath(Dictionary<Coordinate, Node> graph, Coordinate root, Coordinate current, HashSet<Coordinate> visitedNodes)
        {
            if(graph[current].Paths.Count == 1 || visitedNodes.Contains(current))
            {
                // Fin de chaine ou présence d'un cycle. On compte la dernière intersection du cycle et on ne va pas plus loin.
                return (new List<Coordinate>() { current }, 0);
            }

            visitedNodes.Add(current);
            (List<Coordinate>, int) longestPath = (null, 0), childPath;
            foreach (NodePath path in graph[current].Paths)
            {
                // Pour chaque chemin, si celui-ci n'est pas root, (sinon ça reviendrait en arrière,) calculer le chemin le plus long
                if(path.FarNeighbour != root)
                {
                    childPath = LongestPath(graph, current, path.FarNeighbour, visitedNodes);
                    childPath.Item2 += path.DistanceNeighbour;
                    
                    if(childPath.Item2 > longestPath.Item2)
                    {
                        longestPath = childPath;
                    }
                }
            }
            longestPath.Item1.Add(current);
            visitedNodes.Remove(current);
            return longestPath;
        }

        (Coordinate, double) BestTargetRoad(Dictionary<Coordinate, Node> graph, Coordinate origin, Coordinate root, Coordinate current, HashSet<Coordinate> visitedNodes, int roadsNeededs, int deep)
        {
            if (deep == 0 || (Intersections[current].PlayerId != 0 && Intersections[current].PlayerId != ai.Id) || visitedNodes.Contains(current))
            {
                // Obstacle, boucle, ou limite atteinte. On ne peut pas aller plus loin.
                return Score(roadsNeeded, graph[current].PathLength)
            }

            visitedNodes.Add(current);
            (List<Coordinate>, int) bestTargetRoad = (null, 0), childPath;
            foreach (NodePath path in graph[current].Paths)
            {
                // Pour chaque chemin, si celui-ci n'est pas root, (sinon ça reviendrait en arrière,) calculer le chemin le plus long
                if (path.FarNeighbour != root)
                {
                    childPath = LongestPath(graph, current, path.FarNeighbour, visitedNodes);
                    childPath.Item2 += path.DistanceNeighbour;

                    if (childPath.Item2 > longestPath.Item2)
                    {
                        longestPath = childPath;
                    }
                }
            }
            longestPath.Item1.Add(current);
            visitedNodes.Remove(current);
            return longestPath;
        }

        double Score(int roadsNeeded, int newLength)
        {
            if (newLength > maxRoadsLength)
            {
                return VictoryPointsNeeds * (1.75 * (Math.Max(0.5, (double)(newLength - maxRoadsLength)) / 5.0)) / ((double)roadsNeeded * (RessourceRarity(RessourceType.BRICK) + RessourceRarity(RessourceType.LUMBER)));
            }
            else
            {
                return 0.0;
            }
        }
        */



        // Actions

        /// <summary>
        /// Renvoie la coordonnée de la prochaine route à construire
        /// </summary>
        /// <returns>La coordonnées de la prochaine route à construire</returns>
        Coordinate NextRoad()
        {
            if(BestIntersection == null)
            {
                HashSet<Coordinate> possibleRoads = PossibleRoads();
                if(possibleRoads == null)
                {
                    return null;
                }
                else
                {
                    return RandomChoice(possibleRoads);
                }
            }

            Coordinate source = BestIntersection, target = null;
            (Coordinate, double) bestDistantIntersection = (null, 0);
            if (source == Paths[source])
            {
                foreach (KeyValuePair<Coordinate, double> score in IntersectionsScores)
                {
                    if (score.Key != Paths[score.Key] && score.Value > bestDistantIntersection.Item2)
                    {
                        bestDistantIntersection = (score.Key, score.Value);
                    }
                }

                source = bestDistantIntersection.Item1;
            }

            if (source == null)
            {
                HashSet<Coordinate> possibleRoads = PossibleRoads();
                if (possibleRoads == null)
                {
                    return null;
                }
                else
                {
                    return RandomChoice(possibleRoads);
                }
            }

            while (source != Paths[source])
            {
                target = source;
                source = Paths[source];
            }

            return EdgeFromIntersections(source, target);
        }

        /// <summary>
        /// Renvoie les coordonnées des deux prochaines routes à construire
        /// </summary>
        /// <returns>Les coordonnées des deux prochaines routes à construire</returns>
        (Coordinate, Coordinate) NextRoads()
        {
            // Calcul première route
            Coordinate road1 = NextRoad(), road2;

            if(road1 == null)
            {
                return (null, null);
            }

            // Ajout temporaire route
            Edges[road1].AddBuilding(ConstructionType.ROAD, ai.Id);
            Constructions.Add((road1, ConstructionType.ROAD));

            // Réévaluation scores et calcul seconde route
            EvaluateIntersectionsScores();
            road2 = NextRoad();

            // Retrait route
            Edges[road1].AddBuilding(ConstructionType.NONE, -1);
            Constructions.RemoveAt(Constructions.Count - 1);

            return (road1, road2);
        }

        /// <summary>
        /// Calcule les meilleurs choix de joueur cible et de terrain à occuper pour le déplacement du bandit
        /// </summary>
        /// <returns>Les arguments contenant le joueur cible et le terrain à occuper</returns>
        private BanditMoveEventArgs BanditMoveArgs()
        {
            UpdateDatas();
            EvaluateRessourcesAccess();
            EvaluateIntersectionsScores();
            RessourcesNeeds[RessourceType.BRICK] = RessourceRarity(RessourceType.BRICK);
            RessourcesNeeds[RessourceType.LUMBER] = RessourceRarity(RessourceType.LUMBER);
            RessourcesNeeds[RessourceType.ORE] = RessourceRarity(RessourceType.ORE);
            RessourcesNeeds[RessourceType.WHEAT] = RessourceRarity(RessourceType.WHEAT);
            RessourcesNeeds[RessourceType.WOOL] = RessourceRarity(RessourceType.WOOL);

            if (BestIntersection == null)
            {
                RessourcesNeeds[RessourceType.ORE] = 1.0 + RessourceRarity(RessourceType.ORE) / 10.0 - Math.Min(1.0, (double)Ressources[RessourceType.ORE]);
                RessourcesNeeds[RessourceType.WHEAT] = 1.0 + RessourceRarity(RessourceType.WHEAT) / 10.0 - Math.Min(1.0, (double)Ressources[RessourceType.WHEAT]);
                RessourcesNeeds[RessourceType.WOOL] = 1.0 + RessourceRarity(RessourceType.WOOL) / 10.0 - Math.Min(1.0, (double)Ressources[RessourceType.WOOL]);
                RessourcesNeeds[RessourceType.BRICK] = 0;
                RessourcesNeeds[RessourceType.LUMBER] = 0;
            }
            else if (IntersectionsDistances[BestIntersection] == 0)
            {
                if (Intersections[BestIntersection].Building == ConstructionType.SETTLEMENT)
                {
                    RessourcesNeeds[RessourceType.ORE] = 3.0 + RessourceRarity(RessourceType.ORE) / 10.0 - Math.Min(3.0, (double)Ressources[RessourceType.ORE]);
                    RessourcesNeeds[RessourceType.WHEAT] = 2.0 + RessourceRarity(RessourceType.WHEAT) / 10.0 - Math.Min(2.0, (double)Ressources[RessourceType.WHEAT]);
                    RessourcesNeeds[RessourceType.BRICK] = RessourceRarity(RessourceType.BRICK) / 10.0 - Math.Min(0.0, (double)Ressources[RessourceType.BRICK]);
                    RessourcesNeeds[RessourceType.LUMBER] = RessourceRarity(RessourceType.LUMBER) / 10.0 - Math.Min(0.0, (double)Ressources[RessourceType.LUMBER]);
                    RessourcesNeeds[RessourceType.WOOL] = RessourceRarity(RessourceType.WOOL) / 10.0 - Math.Min(0.0, (double)Ressources[RessourceType.WOOL]);
                }
                else
                {
                    RessourcesNeeds[RessourceType.BRICK] = 1.0 + RessourceRarity(RessourceType.BRICK) / 10.0 - Math.Min(1.0, (double)Ressources[RessourceType.BRICK]);
                    RessourcesNeeds[RessourceType.LUMBER] = 1.0 + RessourceRarity(RessourceType.LUMBER) / 10.0 - Math.Min(1.0, (double)Ressources[RessourceType.LUMBER]);
                    RessourcesNeeds[RessourceType.WHEAT] = 1.0 + RessourceRarity(RessourceType.WHEAT) / 10.0 - Math.Min(1.0, (double)Ressources[RessourceType.WHEAT]);
                    RessourcesNeeds[RessourceType.WOOL] = 1.0 + RessourceRarity(RessourceType.WOOL) / 10.0 - Math.Min(1.0, (double)Ressources[RessourceType.WOOL]);
                    RessourcesNeeds[RessourceType.ORE] = RessourceRarity(RessourceType.ORE) / 10.0 - Math.Min(0.0, (double)Ressources[RessourceType.ORE]);
                }
            }
            else
            {
                RessourcesNeeds[RessourceType.BRICK] = 2.0 + RessourceRarity(RessourceType.BRICK) / 10.0 - Math.Min(2.0, (double)Ressources[RessourceType.BRICK]);
                RessourcesNeeds[RessourceType.LUMBER] = 2.0 + RessourceRarity(RessourceType.LUMBER) / 10.0 - Math.Min(2.0, (double)Ressources[RessourceType.LUMBER]);
                RessourcesNeeds[RessourceType.ORE] = RessourceRarity(RessourceType.ORE) / 10.0 - Math.Min(0.0, (double)Ressources[RessourceType.ORE]);
                RessourcesNeeds[RessourceType.WHEAT] = RessourceRarity(RessourceType.WHEAT) / 10.0 - Math.Min(0.0, (double)Ressources[RessourceType.WHEAT]);
                RessourcesNeeds[RessourceType.WOOL] = RessourceRarity(RessourceType.WOOL) / 10.0 - Math.Min(0.0, (double)Ressources[RessourceType.WOOL]);
            }

            Dictionary<IPlayer, double> playersScores = new Dictionary<IPlayer, double>();

            int targetId = -1;
            double scoreBestTarget = -1.0;
            foreach (IPlayer player in game.Players)
            {
                if (player.Id != ai.Id)
                {
                    if (player.TotalRessourceNumber == 0)
                    {
                        playersScores[player] = 0.0;
                    }
                    else
                    {
                        Constructions = new List<(Coordinate, ConstructionType)>(player.Constructions);
                        EvaluateRessourcesAccess();
                        playersScores[player] = 0.0;
                        double sommeRessourcesAccess = 0.0;
                        foreach (RessourceType type in RessourcesNeeds.Keys)
                        {
                            playersScores[player] += RessourcesNeeds[type] * RessourcesAccess[type];
                            sommeRessourcesAccess += RessourcesAccess[type];
                        }

                        playersScores[player] = playersScores[player] / sommeRessourcesAccess + (double)player.VictoryPoints / 4.0;
                    }

                    if (playersScores[player] > scoreBestTarget)
                    {
                        targetId = player.Id;
                        scoreBestTarget = playersScores[player];
                    }
                }
            }

            // List of possible tiles
            HashSet<Coordinate> tiles = new HashSet<Coordinate>();
            foreach ((Coordinate, ConstructionType) c in game.Players[targetId].Constructions)
            {
                if (c.Item2 == ConstructionType.SETTLEMENT || c.Item2 == ConstructionType.CITY)
                    tiles.UnionWith(CoordinatesTilesFromIntersection(c.Item1, game));
            }

            // Remove tiles next to AI's settlements
            foreach (Coordinate c in new List<Coordinate>(tiles))
            {
                if (tiles.Count > 1)
                {
                    List<IIntersection> inters = GetIntersectionsFromTile(c);
                    int i;
                    for (i = 0; i < inters.Count && inters[i].PlayerId != ai.Id; i++) ;
                    if (i != inters.Count)
                        tiles.Remove(c);
                }
            }

            // Remove current bandit position
            if (tiles.Contains(game.GameGrid.CurrentThiefLocation))
            {
                tiles.Remove(game.GameGrid.CurrentThiefLocation);
            }

            // Remove desert
            if (tiles.Contains(DesertLocation))
            {
                tiles.Remove(DesertLocation);
            }

            // Choose best tile
            (Coordinate, double) bestTile = (null, 0.0);
            foreach (Coordinate c in new List<Coordinate>(tiles))
            {
                double score = 0.0;
                double probaHarvest = ProbasHarvest[c].Item2;
                foreach (IIntersection inter in GetIntersectionsFromTile(c))
                {
                    if (inter.PlayerId != -1)
                    {
                        score += probaHarvest * game.Players[inter.PlayerId].VictoryPoints;
                    }
                }
                if (score > bestTile.Item2)
                {
                    bestTile = (c, score);
                }
            }

            return new BanditMoveEventArgs(game.Id, bestTile.Item1, targetId, IsOnline);
        }

        /// <summary>
        /// Place une colonie durant la phase d'initialisation
        /// </summary>
        /// <param name="game">Partie dans laquelle placer les colonies</param>
        private void FirstColonies(IGame game)
        {
            UpdateDatas();
            EvaluateRessourcesAccess();
            if (ai.VictoryPoints == 0)
            {
                RessourcesNeeds[RessourceType.BRICK] = 10.0;
                RessourcesNeeds[RessourceType.LUMBER] = 10.0;
                RessourcesNeeds[RessourceType.ORE] = 1.0;
                RessourcesNeeds[RessourceType.WHEAT] = 3.0;
                RessourcesNeeds[RessourceType.WOOL] = 3.0;
            }
            else
            {
                RessourcesNeeds[RessourceType.BRICK] = 5.0 * RessourceRarity(RessourceType.BRICK);
                RessourcesNeeds[RessourceType.LUMBER] = 5.0 * RessourceRarity(RessourceType.LUMBER);
                RessourcesNeeds[RessourceType.ORE] = 1.0 * RessourceRarity(RessourceType.ORE);
                RessourcesNeeds[RessourceType.WHEAT] = 3.0 * RessourceRarity(RessourceType.WHEAT);
                RessourcesNeeds[RessourceType.WOOL] = 3.0 * RessourceRarity(RessourceType.WOOL);
            }

            EvaluateIntersectionsScores();

            List<Coordinate> possibleColonies = PossibleFirstColonies(game);

            // Calcul de la valeur des intersections
            (Coordinate, double) bestCoord = (null, 0.0);
            foreach (Coordinate c in possibleColonies)
            {
                EvaluateIntersectionValue(c);
                if (IntersectionsScores[c] > bestCoord.Item2)
                {
                    bestCoord = (c, IntersectionsScores[c]);
                }
            }
            if (Intersections[bestCoord.Item1].Harbor != HarborType.NONE)
            {
                harbors.Add(Intersections[bestCoord.Item1].Harbor);
            }

            GameView.Instance.OnInitialColony(new InitialConstructEventArgs(game.Id, bestCoord.Item1, IsOnline));
        }

        /// <summary>
        /// Place une route durant la phase d'initialisation
        /// </summary>
        private void PlaceFirstRoads()
        {
            UpdateDatas();
            EvaluateRessourcesAccess();

            RessourcesNeeds[RessourceType.BRICK] = 5.0 * RessourceRarity(RessourceType.BRICK);
            RessourcesNeeds[RessourceType.LUMBER] = 5.0 * RessourceRarity(RessourceType.LUMBER);
            RessourcesNeeds[RessourceType.ORE] = 1.0 * RessourceRarity(RessourceType.ORE);
            RessourcesNeeds[RessourceType.WHEAT] = 3.0 * RessourceRarity(RessourceType.WHEAT);
            RessourcesNeeds[RessourceType.WOOL] = 3.0 * RessourceRarity(RessourceType.WOOL);

            EvaluateIntersectionsScores();

            Coordinate road = NextRoad();
            if(road == null)
            {
                road = RandomChoice(PossibleRoads());
            }
            GameView.Instance.OnInitialRoad(new InitialConstructEventArgs(game.Id, road, IsOnline));
        }


        /// <inheritdoc/>
        public override void InitAI(IGame game)
        {
            base.InitAI(game);
            EvaluateProbasHarvest();
        }

        /// <inheritdoc/>
        public override void PlaceFirstColony(IGame game)
        {
            InitAI(game);
            FirstColonies(game);
        }

        /// <inheritdoc/>
        public override void PlaceFirstRoad(IGame game)
        {
            PlaceFirstRoads();
        }

        /// <inheritdoc/>
        public override void PlaceSecondColony(IGame game)
        {
            FirstColonies(game);
        }

        /// <inheritdoc/>
        public override void PlaceSecondRoad(IGame game)
        {
            PlaceFirstRoads();
        }

        /// <inheritdoc/>
        public override void HarvestBegin(IGame game)
        {
            InitTurn();
            /*
            if (ai.Cards[CardType.KNIGHT] > 0 && ai.TotalRessourceNumber <= 6)
            {
                double bonusScore = 0.0;
                foreach (Coordinate c in GetIntersectionsFromTile(game.GameGrid.CurrentThiefLocation))
                {
                    if (Intersections[c].PlayerId == ai.Id)
                    {
                        bonusScore += ProbasHarvest[c].Item2 * RessourcesNeeds[ProbasHarvest[c].Item1];
                    }
                }

                if (IsBestScore(CardsScores[CardType.KNIGHT] + bonusScore, CardsScores))
                {
                    UseKnightCard();
                }
            }
            */
            GameView.Instance.OnDiceThrow(new BaseEventArgs(game.Id, false));
        }


        /// <inheritdoc/>
        public override void Construct(IGame game)
        {
            int arec = 0, brec, crec, drec, erec, frec, grec, hrec;
            string strrec = "";
            UpdateDatas();
            bool constructionDone = true;
            while (constructionDone)
            {
                arec++;
                if (arec == 20) throw new Exception("Here " + strrec);
                strrec = "";
                // Usage d'une carte chevalier si cela permet d'obtenir les 2 points de la victoire
                if (!DevelopmentCardUsed && VictoryPoints >= 8)
                {
                    strrec += "a";
                    // Si l'IA approche de la victoire, elle cherche à utiliser des cartes chevalier et/ou routes

                    // Evaluation chevalier
                    int maxKnights = -1;
                    if (!ai.HasGreatestArmy && UsableCards[CardType.KNIGHT] >= 1)
                    {
                        strrec += "b";
                        foreach (IPlayer player in game.Players)
                        {
                            if (player.HasGreatestArmy)
                            {
                                maxKnights = player.KnightCardsPlayed;
                            }
                        }

                        if ((maxKnights < 3 && ai.KnightCardsPlayed == 2) || ai.KnightCardsPlayed == maxKnights)
                        {
                            strrec += "c";
                            // Gain si utilisation chevalier
                            GameView.Instance.OnKnightCardUse(BanditMoveArgs());
                            DevelopmentCardUsed = true;
                        }
                        else
                        {
                            strrec += "d";
                            // Evaluation routes
                            int maxRoadLength = -1;
                            if (!ai.HasLongestRoad && UsableCards[CardType.ROAD_BUILDING] >= 1)
                            {
                                strrec += "e";
                                foreach (IPlayer player in game.Players)
                                {
                                    if (player.HasLongestRoad)
                                    {
                                        maxRoadLength = LongestRoads[player];
                                    }
                                }

                                if ((maxRoadLength < 5 && LongestRoads[ai] >= 4) || LongestRoads[ai] >= maxRoadLength - 1)
                                {
                                    strrec += "f";
                                    // Gain potentiel si utilisation routes
                                    Coordinate road1, road2;
                                    (road1, road2) = NextRoads();

                                    if (road1 != null)
                                    {
                                        strrec += "g";
                                        GameView.Instance.OnRoadConstructionCardUse(new RoadConstructionCardUseEventArgs(game.Id, road1, road2, IsOnline));
                                        Edges[road1].AddBuilding(ConstructionType.ROAD, ai.Id);
                                        Constructions.Add((road1, ConstructionType.ROAD));
                                        
                                        if(road2 != null)
                                        {
                                            strrec += "h";
                                            Edges[road2].AddBuilding(ConstructionType.ROAD, ai.Id);
                                            Constructions.Add((road2, ConstructionType.ROAD));
                                        }
                                        DevelopmentCardUsed = true;
                                    }
                                }
                            }
                        }
                    }
                }

                constructionDone = false;
                EvaluateRessourcesAccess();
                EvaluateIntersectionsScores();
                RessourcesNeeds[RessourceType.BRICK] = RessourceRarity(RessourceType.BRICK);
                RessourcesNeeds[RessourceType.LUMBER] = RessourceRarity(RessourceType.LUMBER);
                RessourcesNeeds[RessourceType.ORE] = RessourceRarity(RessourceType.ORE);
                RessourcesNeeds[RessourceType.WHEAT] = RessourceRarity(RessourceType.WHEAT);
                RessourcesNeeds[RessourceType.WOOL] = RessourceRarity(RessourceType.WOOL);

                if (BestIntersection == null)
                {
                    strrec += "i";
                    // L'IA est bloquée et n'a plus de meilleure intersection
                    RessourcesNeeds[RessourceType.ORE] = 1.0 + RessourceRarity(RessourceType.ORE) / 10.0 - Math.Min(1.0, (double)Ressources[RessourceType.ORE]);
                    RessourcesNeeds[RessourceType.WHEAT] = 1.0 + RessourceRarity(RessourceType.WHEAT) / 10.0 - Math.Min(1.0, (double)Ressources[RessourceType.WHEAT]);
                    RessourcesNeeds[RessourceType.WOOL] = 1.0 + RessourceRarity(RessourceType.WOOL) / 10.0 - Math.Min(1.0, (double)Ressources[RessourceType.WOOL]);
                    RessourcesNeeds[RessourceType.BRICK] = 0;
                    RessourcesNeeds[RessourceType.LUMBER] = 0;

                    if (Ressources[RessourceType.WOOL] >= 1 && Ressources[RessourceType.ORE] >= 1 && Ressources[RessourceType.WHEAT] >= 1)
                    {
                        strrec += "j";
                        // L'IA achète des cartes développement pour obtenir des points de victoire
                        GameView.Instance.OnConstruct(new ConstructEventArgs(game.Id, ConstructionType.DEVELOPMENT_CARD, null, IsOnline));
                        Ressources[RessourceType.ORE]--;
                        Ressources[RessourceType.WHEAT]--;
                        Ressources[RessourceType.WOOL]--;
                        constructionDone = true;
                    }
                    else if (!DevelopmentCardUsed)
                    {
                        strrec += "k";
                        if (UsableCards[CardType.KNIGHT] >= 1)
                        {
                            strrec += "l";
                            // Impossible. L'IA utilise ses cartes chevalier pour obtenir les points de victoire de la plus grande armée
                            if (RessourcesCount() > 7)
                            {
                                strrec += "m";
                                // Si l'IA possède trop de ressources pour éviter la défausse, elle construit le nombre nécessaire de routes pour diminuer ce nombre.
                                // Elle fait dans la mesure des emplacements possibles. Peut-être obtiendra-t-elle la plus longue route.
                                HashSet<Coordinate> possibleRoads = PossibleRoads();
                                brec = 0;
                                while (possibleRoads != null && Ressources[RessourceType.BRICK] >= 1 && Ressources[RessourceType.LUMBER] >= 1 && RessourcesCount() > 7)
                                {
                                    brec++;
                                    if (brec == 20) throw new Exception("Here");
                                    Coordinate road = RandomChoice(possibleRoads);
                                    GameView.Instance.OnConstruct(new ConstructEventArgs(game.Id, ConstructionType.ROAD, road, IsOnline));
                                    Constructions.Add((road, ConstructionType.ROAD));
                                    Edges[road].AddBuilding(ConstructionType.ROAD, ai.Id);
                                    Ressources[RessourceType.BRICK]--;
                                    Ressources[RessourceType.LUMBER]--;
                                    possibleRoads = PossibleRoads();
                                }
                            }

                            GameView.Instance.OnKnightCardUse(BanditMoveArgs());
                            DevelopmentCardUsed = true;
                            constructionDone = true;
                        }
                        else if (UsableCards[CardType.RESSOURCE_MONOPOLY] >= 1 || UsableCards[CardType.RESSOURCE_PAIR] >= 1)
                        {
                            strrec += "n";
                            bool useMonopolyCard = (UsableCards[CardType.RESSOURCE_MONOPOLY] >= 1), useRessourcePair = (UsableCards[CardType.RESSOURCE_PAIR] >= 1);
                            RessourceType[] ressourcesNeeded = new RessourceType[] { RessourceType.ORE, RessourceType.WHEAT, RessourceType.WOOL };

                            if (useMonopolyCard && useRessourcePair)
                            {
                                strrec += "o";
                                // Utilisation de la carte monopole si cela permet d'acheter au moins deux cartes développement
                                int nbDoubleRessource = 0;
                                foreach (RessourceType ressource in ressourcesNeeded)
                                {
                                    if (Ressources[ressource] >= 2)
                                    {
                                        nbDoubleRessource++;
                                    }
                                }

                                if (nbDoubleRessource == 2)
                                {
                                    useRessourcePair = false;
                                }
                                else
                                {
                                    useMonopolyCard = false;
                                }
                            }

                            if (useMonopolyCard)
                            {
                                strrec += "p";
                                (RessourceType, double) bestRessource = (RessourceType.NONE, 0.0);
                                foreach (RessourceType ressource in ressourcesNeeded)
                                {
                                    if (Ressources[ressource] == 0 && RessourceRarity(ressource) > bestRessource.Item2)
                                    {
                                        bestRessource = (ressource, RessourceRarity(ressource));
                                    }
                                }

                                foreach (IPlayer player in game.Players)
                                {
                                    if (player.Id != ai.Id)
                                    {
                                        Ressources[bestRessource.Item1] += player.Ressources[bestRessource.Item1];
                                    }
                                }

                                GameView.Instance.OnMonopolyCardUse(new MonopolyCardUseEventArgs(game.Id, bestRessource.Item1, IsOnline));
                                DevelopmentCardUsed = true;
                                constructionDone = true;
                            }
                            else if (useRessourcePair)
                            {
                                strrec += "q";
                                (RessourceType, double)[] bestRessources = new (RessourceType, double)[] { (RessourceType.NONE, 0.0), (RessourceType.NONE, 0.0) };
                                foreach (RessourceType ressource in ressourcesNeeded)
                                {
                                    if (Ressources[ressource] == 0)
                                    {
                                        if (RessourceRarity(ressource) > bestRessources[0].Item2)
                                        {
                                            bestRessources[1] = bestRessources[0];
                                            bestRessources[0] = (ressource, RessourceRarity(ressource));
                                        }
                                        else if(RessourceRarity(ressource) > bestRessources[1].Item2)
                                        {
                                            bestRessources[1] = (ressource, RessourceRarity(ressource));
                                        }
                                    }
                                }

                                if(bestRessources[1].Item1 == RessourceType.NONE)
                                {
                                    foreach (RessourceType ressource in ressourcesNeeded)
                                    {
                                        if (Ressources[ressource] <= 1 && RessourceRarity(ressource) > bestRessources[1].Item2)
                                        {
                                            bestRessources[1] = (ressource, RessourceRarity(ressource));
                                        }
                                    }
                                }

                                GameView.Instance.OnResourcePairCardUse(new ResourcePairCardUseEventArgs(game.Id, bestRessources[0].Item1, bestRessources[1].Item1, IsOnline));
                                Ressources[bestRessources[0].Item1]++;
                                Ressources[bestRessources[1].Item1]++;
                                DevelopmentCardUsed = true;
                                constructionDone = true;
                            }
                        }
                        else if (UsableCards[CardType.ROAD_BUILDING] >= 1)
                        {
                            strrec += "r";
                            // Construction de routes. Pourrait peut-être permettre d'obtenir la plus longue route
                            HashSet<Coordinate> possibleRoads = PossibleRoads();
                            List<Coordinate> roads;
                            if(possibleRoads != null)
                            {
                                strrec += "s";
                                if (possibleRoads.Count >= 2)
                                {
                                    roads = RandomChoices(new List<Coordinate>(possibleRoads), 2);
                                }
                                else
                                {
                                    roads = new List<Coordinate>(possibleRoads);
                                    roads.Add(null);
                                }

                                GameView.Instance.OnRoadConstructionCardUse(new RoadConstructionCardUseEventArgs(game.Id, roads[0], roads[1], IsOnline));
                                Constructions.Add((roads[0], ConstructionType.ROAD));
                                Constructions.Add((roads[1], ConstructionType.ROAD));
                                Edges[roads[0]].AddBuilding(ConstructionType.ROAD, ai.Id);
                                Edges[roads[1]].AddBuilding(ConstructionType.ROAD, ai.Id);
                                DevelopmentCardUsed = true;
                                constructionDone = true;
                            }
                        }
                    }
                }
                else if (IntersectionsDistances[BestIntersection] == 0)
                {
                    // L'IA a accès à l'intersection sur laquelle elle veut construire
                    if (Intersections[BestIntersection].Building == ConstructionType.SETTLEMENT)
                    {
                        // L'IA veut construire une ville
                        if (Ressources[RessourceType.WHEAT] >= 2 && Ressources[RessourceType.ORE] >= 3)
                        {
                            // C'est possible. L'IA construit.
                            GameView.Instance.OnConstruct(new ConstructEventArgs(game.Id, ConstructionType.CITY, BestIntersection, IsOnline));
                            Constructions[Constructions.LastIndexOf((BestIntersection, ConstructionType.SETTLEMENT))] = (BestIntersection, ConstructionType.CITY);
                            Intersections[BestIntersection].AddBuilding(ConstructionType.CITY, ai.Id);
                            VictoryPoints += 1;
                            Ressources[RessourceType.WHEAT] -= 2;
                            Ressources[RessourceType.ORE] -= 3;
                            constructionDone = true;
                        }
                        else
                        {
                            // Impossible. L'IA utilise si possible une carte Développement pour obtenir les ressources manquantes.
                            if (!DevelopmentCardUsed)
                            {
                                /*
                                if (UsableCards[CardType.KNIGHT] > 0 && RessourcesCount() <= 7 && 
                                   ((Ressources[RessourceType.WHEAT] >= 1) && (Ressources[RessourceType.ORE] >= 2) && (Ressources[RessourceType.WHEAT] + Ressources[RessourceType.ORE] >= 4)))
                                {
                                    // Pour récupérer une ressource, la carte chevalier.
                                    GameView.Instance.OnKnightCardUse(BanditMoveArgs());
                                    DevelopmentCardUsed = true;
                                    constructionDone = true;
                                }
                                else
                                */
                                if (UsableCards[CardType.RESSOURCE_PAIR] > 0 &&
                                   ((Ressources[RessourceType.ORE] >= 1) && (Ressources[RessourceType.WHEAT] + Ressources[RessourceType.ORE] >= 3)))
                                {
                                    // Pour récupérer deux ressources, la carte invention.
                                    (RessourceType, RessourceType) ressourcePair;
                                    switch(Ressources[RessourceType.ORE])
                                    {
                                        case 1:
                                            ressourcePair = (RessourceType.ORE, RessourceType.ORE);
                                            break;
                                        case 2:
                                            ressourcePair = (RessourceType.ORE, RessourceType.WHEAT);
                                            break;
                                        default:
                                            ressourcePair = (RessourceType.WHEAT, RessourceType.WHEAT);
                                            break;
                                    }

                                    GameView.Instance.OnResourcePairCardUse(new ResourcePairCardUseEventArgs(game.Id, ressourcePair.Item1, ressourcePair.Item2, IsOnline));
                                    Ressources[ressourcePair.Item1]++;
                                    Ressources[ressourcePair.Item2]++;
                                    DevelopmentCardUsed = true;
                                    constructionDone = true;
                                }
                                else if(UsableCards[CardType.RESSOURCE_MONOPOLY] > 0)
                                {
                                    // Pour récupérer au moins deux ressources du même type, la carte monopole.
                                    RessourceType ressourceToMonopolize = RessourceType.NONE;
                                    if (Ressources[RessourceType.ORE] == 0)
                                    {
                                        ressourceToMonopolize = RessourceType.ORE;
                                    }
                                    else if (Ressources[RessourceType.WHEAT] == 0)
                                    {
                                        ressourceToMonopolize = RessourceType.WHEAT;
                                    }
                                    else if (Ressources[RessourceType.ORE] == 1)
                                    {
                                        ressourceToMonopolize = RessourceType.ORE;
                                    }

                                    if(ressourceToMonopolize != RessourceType.NONE)
                                    {
                                        foreach(IPlayer player in game.Players)
                                        {
                                            if (player.Id != ai.Id)
                                            {
                                                Ressources[ressourceToMonopolize] += player.Ressources[ressourceToMonopolize];
                                            }
                                        }
                                        GameView.Instance.OnMonopolyCardUse(new MonopolyCardUseEventArgs(game.Id, ressourceToMonopolize, IsOnline));
                                        DevelopmentCardUsed = true;
                                        constructionDone = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // L'IA veut construire une colonie
                        if (Ressources[RessourceType.LUMBER] >= 1 && Ressources[RessourceType.BRICK] >= 1 && Ressources[RessourceType.WOOL] >= 1 && Ressources[RessourceType.WHEAT] >= 1)
                        {
                            // C'est possible. L'IA construit.
                            GameView.Instance.OnConstruct(new ConstructEventArgs(game.Id, ConstructionType.SETTLEMENT, BestIntersection, IsOnline));
                            Constructions.Add((BestIntersection, ConstructionType.SETTLEMENT));
                            Intersections[BestIntersection].AddBuilding(ConstructionType.SETTLEMENT, ai.Id);
                            VictoryPoints += 1;
                            Ressources[RessourceType.LUMBER] -= 1;
                            Ressources[RessourceType.BRICK] -= 1;
                            Ressources[RessourceType.WOOL] -= 1;
                            Ressources[RessourceType.WHEAT] -= 1;
                            if (Intersections[BestIntersection].Harbor != HarborType.NONE)
                            {
                                harbors.Add(Intersections[BestIntersection].Harbor);
                            }
                            constructionDone = true;
                        }
                        else
                        {
                            // Impossible. L'IA utilise si possible une carte Développement pour obtenir les ressources manquantes.
                            if (!DevelopmentCardUsed)
                            {
                                int nbMissingRessources = 0;
                                foreach (RessourceType ressource in new List<RessourceType>() { RessourceType.LUMBER, RessourceType.BRICK, RessourceType.WOOL, RessourceType.WHEAT })
                                {
                                    if (Ressources[ressource] == 0)
                                    {
                                        nbMissingRessources++;
                                    }
                                }

                                /*
                                if (nbMissingRessources == 1 && UsableCards[CardType.KNIGHT] > 0 && RessourcesCount() <= 7)
                                {
                                    // Pour récupérer une ressource, la carte chevalier.
                                    GameView.Instance.OnKnightCardUse(BanditMoveArgs());
                                    DevelopmentCardUsed = true;
                                    constructionDone = true;
                                }
                                else
                                */
                                if (nbMissingRessources <= 2 && UsableCards[CardType.RESSOURCE_PAIR] > 0)
                                {
                                    // Pour récupérer deux ressources, la carte invention.
                                    (RessourceType, RessourceType) ressourcePair = (RessourceType.NONE, RessourceType.NONE);
                                    int numRessource = 0;
                                    foreach (RessourceType ressource in new List<RessourceType>() { RessourceType.LUMBER, RessourceType.BRICK, RessourceType.WOOL, RessourceType.WHEAT })
                                    {
                                        if (Ressources[ressource] == 0)
                                        {
                                            if (numRessource == 0)
                                            {
                                                ressourcePair.Item1 = ressource;
                                            }
                                            else
                                            {
                                                ressourcePair.Item2 = ressource;
                                            }
                                            numRessource++;
                                        }
                                    }

                                    double scoreBestRessource = 0.0;
                                    if(ressourcePair.Item2 == RessourceType.NONE)
                                    {
                                        foreach(KeyValuePair<RessourceType, double> score in RessourcesNeeds)
                                        {
                                            if(score.Value > scoreBestRessource)
                                            {
                                                ressourcePair.Item2 = score.Key;
                                                scoreBestRessource = score.Value;
                                            }
                                        }
                                    }

                                    GameView.Instance.OnResourcePairCardUse(new ResourcePairCardUseEventArgs(game.Id, ressourcePair.Item1, ressourcePair.Item2, IsOnline));
                                    Ressources[ressourcePair.Item1]++;
                                    Ressources[ressourcePair.Item2]++;
                                    DevelopmentCardUsed = true;
                                    constructionDone = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    // L'IA n'a pas accès à l'intersection sur laquelle elle veut construire. Elle veut construire une route pour la rejoindre.
                    if (!DevelopmentCardUsed && UsableCards[CardType.ROAD_BUILDING] > 0)
                    {
                        // Utilisation d'une carte route pour économiser des ressources
                        Coordinate road1, road2;
                        (road1, road2) = NextRoads();
                        GameView.Instance.OnRoadConstructionCardUse(new RoadConstructionCardUseEventArgs(game.Id, road1, road2, IsOnline));
                        Edges[road1].AddBuilding(ConstructionType.ROAD, ai.Id);
                        Constructions.Add((road1, ConstructionType.ROAD));

                        if(road2 != null)
                        {
                            Edges[road2].AddBuilding(ConstructionType.ROAD, ai.Id);
                            Constructions.Add((road2, ConstructionType.ROAD));
                        }
                        DevelopmentCardUsed = true;
                        constructionDone = true;
                    }
                    else if (Ressources[RessourceType.LUMBER] >= 1 && Ressources[RessourceType.BRICK] >= 1)
                    {
                        // C'est possible. L'IA construit.
                        Coordinate road = NextRoad();
                        GameView.Instance.OnConstruct(new ConstructEventArgs(game.Id, ConstructionType.ROAD, road, IsOnline));
                        Edges[road].AddBuilding(ConstructionType.ROAD, ai.Id);
                        Constructions.Add((road, ConstructionType.ROAD));
                        Ressources[RessourceType.LUMBER] -= 1;
                        Ressources[RessourceType.BRICK] -= 1;
                        constructionDone = true;
                    }
                    else
                    {
                        // Impossible. L'IA utilise si possible une carte Développement pour obtenir les ressources manquantes.
                        if (!DevelopmentCardUsed)
                        {
                            /*
                            if (UsableCards[CardType.KNIGHT] > 0 && RessourcesCount() <= 7)
                            {
                                // Pour récupérer une ressource, la carte chevalier.
                                if (Ressources[RessourceType.LUMBER] + Ressources[RessourceType.BRICK] == 1)
                                {
                                    GameView.Instance.OnKnightCardUse(BanditMoveArgs());
                                    DevelopmentCardUsed = true;
                                    constructionDone = true;
                                }
                            }
                            else
                            */
                            if (UsableCards[CardType.RESSOURCE_PAIR] > 0)
                            {
                                // Pour récupérer deux ressources, la carte invention.
                                (RessourceType, RessourceType) ressourcesPair;
                                if (Ressources[RessourceType.LUMBER] >= 2)
                                {
                                    ressourcesPair = (RessourceType.BRICK, RessourceType.BRICK);
                                }
                                else if (Ressources[RessourceType.BRICK] >= 2)
                                {
                                    ressourcesPair = (RessourceType.LUMBER, RessourceType.LUMBER);
                                }
                                else
                                {
                                    ressourcesPair = (RessourceType.LUMBER, RessourceType.BRICK);
                                }

                                GameView.Instance.OnResourcePairCardUse(new ResourcePairCardUseEventArgs(game.Id, ressourcesPair.Item1, ressourcesPair.Item2, IsOnline));
                                Ressources[ressourcesPair.Item1]++;
                                Ressources[ressourcesPair.Item2]++;
                                DevelopmentCardUsed = true;
                                constructionDone = true;
                            }
                            else if (UsableCards[CardType.RESSOURCE_MONOPOLY] > 0)
                            {
                                // Pour récupérer au moins trois ressources du même type, la carte monopole.
                                RessourceType ressourceToMonopolize = RessourceType.NONE;
                                if (IntersectionsDistances[BestIntersection] >= 2)
                                {
                                    if(Ressources[RessourceType.BRICK] > 3)
                                    {
                                        ressourceToMonopolize = RessourceType.LUMBER;
                                    }
                                    else
                                    {
                                        ressourceToMonopolize = RessourceType.BRICK;
                                    }
                                }

                                if (ressourceToMonopolize != RessourceType.NONE)
                                {
                                    Ressources[ressourceToMonopolize] = 0;
                                    foreach (IPlayer player in game.Players)
                                    {
                                        if(player.Id != ai.Id)
                                        {
                                            Ressources[ressourceToMonopolize] += player.Ressources[ressourceToMonopolize];
                                        }
                                    }
                                    GameView.Instance.OnMonopolyCardUse(new MonopolyCardUseEventArgs(game.Id, ressourceToMonopolize, IsOnline));
                                    DevelopmentCardUsed = true;
                                    constructionDone = true;
                                }
                            }
                        }
                    }
                }

                if(BestIntersection != null && !constructionDone && !DevelopmentCardUsed)
                {
                    // L'IA augmente son nombre de chevaliers/routes si elle est proche de la victoire
                    if (VictoryPoints >= 6)
                    {
                        // Si l'IA approche de la victoire, elle cherche à utiliser des cartes chevalier et/ou routes
                        bool canUseKnight = (UsableCards[CardType.KNIGHT] >= 1 && RessourcesCount() <= 7), canUseRoads = (UsableCards[CardType.ROAD_BUILDING] >= 1);

                        // Evaluation chevalier
                        int maxKnights = -1;
                        if (canUseKnight)
                        {
                            foreach (IPlayer player in game.Players)
                            {
                                if (player.Id != ai.Id && player.KnightCardsPlayed > maxKnights)
                                {
                                    maxKnights = player.KnightCardsPlayed;
                                }
                            }
                        }

                        // Evaluation routes
                        int maxRoadLength = -1;
                        if (canUseRoads)
                        {
                            foreach (IPlayer player in game.Players)
                            {
                                if (player.Id != ai.Id && LongestRoads[player] > maxRoadLength)
                                {
                                    maxRoadLength = LongestRoads[player];
                                }
                            }
                        }

                        int scoreKnight = int.MinValue, scoreRoads = int.MinValue;

                        // Note : les scores sont négatifs ou nuls. Un score de 0 est maximal.
                        if (ai.HasGreatestArmy)
                        {
                            if(maxKnights - ai.KnightCardsPlayed >= 1)
                            {
                                // L'IA possède une avance confortable. On n'utilise pas la carte chevalier.
                                canUseKnight = false;
                            }
                            else
                            {
                                // L'IA possède une petite avance. On utilise la carte chevalier à tout prix.
                                scoreKnight = 0;
                            }
                        }
                        else
                        {
                            // L'IA a du retard. On peut utiliser la carte chevalier.
                            scoreKnight = ai.KnightCardsPlayed - maxKnights;
                        }

                        if (ai.HasLongestRoad)
                        {
                            // L'IA possède une avance. On peut utiliser la carte routes.
                            scoreRoads = maxRoadLength - LongestRoads[ai];
                        }
                        else
                        {
                            // L'IA a du retard. On peut utiliser la carte routes.
                            scoreRoads = LongestRoads[ai] - maxRoadLength;
                        }

                        if (canUseKnight && (!canUseRoads || scoreKnight > scoreRoads))
                        {
                            // Avancée vers la plus grande armée ou conservation de la plus grande armée si utilisation chevalier
                            GameView.Instance.OnKnightCardUse(BanditMoveArgs());
                            DevelopmentCardUsed = true;
                            constructionDone = true;
                        }
                        else if (canUseRoads)
                        {
                            // Avancée vers la plus grande route ou conservation de la plus grande route si utilisation routes
                            Coordinate road1, road2;
                            (road1, road2) = NextRoads();
                            if (road1 != null)
                            {
                                GameView.Instance.OnRoadConstructionCardUse(new RoadConstructionCardUseEventArgs(game.Id, road1, road2, IsOnline));
                                Edges[road1].AddBuilding(ConstructionType.ROAD, ai.Id);
                                Constructions.Add((road1, ConstructionType.ROAD));

                                if(road2 != null)
                                {
                                    Edges[road2].AddBuilding(ConstructionType.ROAD, ai.Id);
                                    Constructions.Add((road2, ConstructionType.ROAD));
                                }
                                DevelopmentCardUsed = true;
                                constructionDone = true;
                            }
                        }
                    }

                    if(!DevelopmentCardUsed)
                    {
                        // L'IA utilise une carte développement si elle en possède beaucoup et qu'elle se trouve dans une impasse.
                        int nbCards = (RessourcesCount() > 7) ? NumberOfUsableCards - UsableCards[CardType.KNIGHT] : NumberOfUsableCards;
                        if (nbCards >= 3)
                        {
                            CardType cardToUse;
                            if (RessourcesCount() > 7)
                            {
                                int nbKnightCards = UsableCards[CardType.KNIGHT];
                                UsableCards[CardType.KNIGHT] = 0;
                                cardToUse = RandomChoice(UsableCards);
                                UsableCards[CardType.KNIGHT] = nbKnightCards;
                            }
                            else
                            {
                                cardToUse = RandomChoice(UsableCards);
                            }

                            switch (cardToUse)
                            {
                                case CardType.KNIGHT:
                                    GameView.Instance.OnKnightCardUse(BanditMoveArgs());
                                    DevelopmentCardUsed = true;
                                    constructionDone = true;
                                    break;
                                case CardType.RESSOURCE_MONOPOLY:
                                    (RessourceType, double) bestRessource = (RessourceType.NONE, 0.0);
                                    foreach (KeyValuePair<RessourceType, int> type in Ressources)
                                    {
                                        if (RessourceRarity(type.Key) > bestRessource.Item2)
                                        {
                                            bestRessource = (type.Key, RessourceRarity(type.Key));
                                        }
                                    }
                                    foreach(IPlayer player in game.Players)
                                    {
                                        if (player.Id != ai.Id)
                                        {
                                            Ressources[bestRessource.Item1] += player.Ressources[bestRessource.Item1];
                                        }
                                    }
                                    
                                    GameView.Instance.OnMonopolyCardUse(new MonopolyCardUseEventArgs(game.Id, bestRessource.Item1, IsOnline));

                                    Ressources[bestRessource.Item1] = 0;
                                    foreach (IPlayer player in game.Players)
                                    {
                                        Ressources[bestRessource.Item1] += player.Ressources[bestRessource.Item1];
                                    }

                                    DevelopmentCardUsed = true;
                                    constructionDone = true;
                                    break;
                                case CardType.RESSOURCE_PAIR:
                                    bestRessource = (RessourceType.NONE, 0.0);
                                    foreach (KeyValuePair<RessourceType, int> type in Ressources)
                                    {
                                        if (RessourceRarity(type.Key) > bestRessource.Item2)
                                        {
                                            bestRessource = (type.Key, RessourceRarity(type.Key));
                                        }
                                    }
                                    GameView.Instance.OnResourcePairCardUse(new ResourcePairCardUseEventArgs(game.Id, bestRessource.Item1, bestRessource.Item1, IsOnline));
                                    Ressources[bestRessource.Item1] += 2;

                                    DevelopmentCardUsed = true;
                                    constructionDone = true;
                                    break;
                                case CardType.ROAD_BUILDING:
                                    Coordinate road1, road2;
                                    (road1, road2) = NextRoads();
                                    if (road1 != null)
                                    {
                                        GameView.Instance.OnRoadConstructionCardUse(new RoadConstructionCardUseEventArgs(game.Id, road1, road2, IsOnline));
                                        Edges[road1].AddBuilding(ConstructionType.ROAD, ai.Id);
                                        Constructions.Add((road1, ConstructionType.ROAD));

                                        if(road2 != null)
                                        {
                                            Edges[road2].AddBuilding(ConstructionType.ROAD, ai.Id);
                                            Constructions.Add((road2, ConstructionType.ROAD));
                                        }
                                        DevelopmentCardUsed = true;
                                        constructionDone = true;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            constructionDone = true;
            while (constructionDone)
            {
                // Il faut construire des routes si jamais il y a trop de ressources. Cela évite des problèmes avec le voleur.
                constructionDone = false;
                int nbBuyableCards = Math.Min(Math.Min(Ressources[RessourceType.WOOL], Ressources[RessourceType.ORE]), Ressources[RessourceType.WHEAT]);
                if(Ressources[RessourceType.BRICK] >= 1 && Ressources[RessourceType.LUMBER] >= 1 && RessourcesCount() - 3 * nbBuyableCards > 6)
                {
                    EvaluateRessourcesAccess();
                    EvaluateIntersectionsScores();
                    RessourcesNeeds[RessourceType.BRICK] = RessourceRarity(RessourceType.BRICK);
                    RessourcesNeeds[RessourceType.LUMBER] = RessourceRarity(RessourceType.LUMBER);
                    RessourcesNeeds[RessourceType.ORE] = RessourceRarity(RessourceType.ORE);
                    RessourcesNeeds[RessourceType.WHEAT] = RessourceRarity(RessourceType.WHEAT);
                    RessourcesNeeds[RessourceType.WOOL] = RessourceRarity(RessourceType.WOOL);

                    Coordinate road = NextRoad();

                    if (road != null)
                    {
                        GameView.Instance.OnConstruct(new ConstructEventArgs(game.Id, ConstructionType.ROAD, road, IsOnline));
                        Edges[road].AddBuilding(ConstructionType.ROAD, ai.Id);
                        Constructions.Add((road, ConstructionType.ROAD));
                        Ressources[RessourceType.LUMBER] -= 1;
                        Ressources[RessourceType.BRICK] -= 1;
                        constructionDone = true;
                    }
                }
            }

            constructionDone = true;
            while (BestIntersection != null && constructionDone)
            {
                constructionDone = false;
                bool buyCard;

                // L'IA achète une carte Développement si cela n'impacte pas (ou peu) les ressources dont elle a besoin
                if (IntersectionsDistances[BestIntersection] == 0)
                {
                    if (Intersections[BestIntersection].Building == ConstructionType.SETTLEMENT)
                    {
                        // L'IA a besoin de ressources pour une ville.
                        buyCard = ((Ressources[RessourceType.WOOL] >= 2) && (Ressources[RessourceType.ORE] >= 2) && (Ressources[RessourceType.WHEAT] >= 2)) ||
                                  ((Ressources[RessourceType.WOOL] >= 1) && (Ressources[RessourceType.ORE] >= 1) && (Ressources[RessourceType.WHEAT] >= 1) && RessourcesCount() > 6);
                    }
                    else
                    {
                        // L'IA a besoin de ressources pour une colonie.
                        buyCard = (Ressources[RessourceType.ORE] >= 1) && (
                                    ((Ressources[RessourceType.WOOL] >= 2) && (Ressources[RessourceType.WHEAT] >= 2)) ||
                                    ((Ressources[RessourceType.WOOL] >= 1) && (Ressources[RessourceType.WHEAT] >= 1) && (RessourcesCount() > 6))
                                  );
                    }
                }
                else
                {
                    // L'IA a besoin de ressources pour des routes.
                    buyCard = (Ressources[RessourceType.WOOL] >= 1 && Ressources[RessourceType.ORE] >= 1 && Ressources[RessourceType.WHEAT] >= 1);
                }

                if (buyCard)
                {
                    GameView.Instance.OnConstruct(new ConstructEventArgs(game.Id, ConstructionType.DEVELOPMENT_CARD, null, IsOnline));
                    Ressources[RessourceType.WOOL] -= 1;
                    Ressources[RessourceType.ORE] -= 1;
                    Ressources[RessourceType.WHEAT] -= 1;
                    constructionDone = true;
                }
            }

            if (BestIntersection != null && !DevelopmentCardUsed && UsableCards[CardType.KNIGHT] > 0 && RessourcesCount() <= 5)
            {
                bool useBanditCard = false;
                // Utilisation du bandit pour libérer un terrain
                foreach(IIntersection inter in GetIntersectionsFromTile(game.GameGrid.CurrentThiefLocation))
                {
                    if(inter.PlayerId == ai.Id)
                    {
                        useBanditCard = true;
                    }
                }

                // Utilisation du bandit pour gagner ou conserver les points de victoire de la plus grande armée
                if (!useBanditCard && VictoryPoints >= 6)
                {
                    int nbKnights = 0;
                    foreach (IPlayer player in game.Players)
                    {
                        if (player.KnightCardsPlayed > nbKnights)
                        {
                            nbKnights = player.KnightCardsPlayed;
                        }
                    }

                    useBanditCard = (!ai.HasGreatestArmy && ((nbKnights <= 2 && ai.KnightCardsPlayed >= 1) || ai.KnightCardsPlayed >= nbKnights - 1));
                    useBanditCard |= (ai.HasGreatestArmy && ai.KnightCardsPlayed == nbKnights);
                }

                if (!useBanditCard)
                {
                    // Utilisation du bandit si au moins deux adversaires ont plus de 7 ressources.
                    int nbTargets = 0;
                    foreach (IPlayer player in game.Players)
                    {
                        if (player.TotalRessourceNumber > 7)
                        {
                            nbTargets++;
                        }
                    }
                    useBanditCard = (nbTargets >= 2);
                }

                if(useBanditCard)
                {
                    GameView.Instance.OnKnightCardUse(BanditMoveArgs());
                    DevelopmentCardUsed = true;
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
                UpdateDatas();
                EvaluateRessourcesAccess();
                EvaluateIntersectionsScores();
                RessourcesNeeds[RessourceType.BRICK] = RessourceRarity(RessourceType.BRICK);
                RessourcesNeeds[RessourceType.LUMBER] = RessourceRarity(RessourceType.LUMBER);
                RessourcesNeeds[RessourceType.ORE] = RessourceRarity(RessourceType.ORE);
                RessourcesNeeds[RessourceType.WHEAT] = RessourceRarity(RessourceType.WHEAT);
                RessourcesNeeds[RessourceType.WOOL] = RessourceRarity(RessourceType.WOOL);

                if(BestIntersection == null)
                {
                    RessourcesNeeds[RessourceType.ORE] = 1.0 + RessourceRarity(RessourceType.ORE) / 10.0 - Math.Min(1.0, (double)Ressources[RessourceType.ORE]);
                    RessourcesNeeds[RessourceType.WHEAT] = 1.0 + RessourceRarity(RessourceType.WHEAT) / 10.0 - Math.Min(1.0, (double)Ressources[RessourceType.WHEAT]);
                    RessourcesNeeds[RessourceType.WOOL] = 1.0 + RessourceRarity(RessourceType.WOOL) / 10.0 - Math.Min(1.0, (double)Ressources[RessourceType.WOOL]);
                    RessourcesNeeds[RessourceType.BRICK] = 0;
                    RessourcesNeeds[RessourceType.LUMBER] = 0;
                }
                else if (IntersectionsDistances[BestIntersection] == 0)
                {
                    if (Intersections[BestIntersection].Building == ConstructionType.SETTLEMENT)
                    {
                        RessourcesNeeds[RessourceType.ORE] = 3.0 + RessourceRarity(RessourceType.ORE) / 10.0 - (double)Ressources[RessourceType.ORE];
                        RessourcesNeeds[RessourceType.WHEAT] = 2.0 + RessourceRarity(RessourceType.WHEAT) / 10.0 - (double)Ressources[RessourceType.WHEAT];
                        RessourcesNeeds[RessourceType.BRICK] = RessourceRarity(RessourceType.BRICK) / 10.0 - (double)Ressources[RessourceType.BRICK];
                        RessourcesNeeds[RessourceType.LUMBER] = RessourceRarity(RessourceType.LUMBER) / 10.0 - (double)Ressources[RessourceType.LUMBER];
                        RessourcesNeeds[RessourceType.WOOL] = RessourceRarity(RessourceType.WOOL) / 10.0 - (double)Ressources[RessourceType.WOOL];
                    }
                    else
                    {
                        RessourcesNeeds[RessourceType.BRICK] = 1.0 + RessourceRarity(RessourceType.BRICK) / 10.0 - (double)Ressources[RessourceType.BRICK];
                        RessourcesNeeds[RessourceType.LUMBER] = 1.0 + RessourceRarity(RessourceType.LUMBER) / 10.0 - (double)Ressources[RessourceType.LUMBER];
                        RessourcesNeeds[RessourceType.WHEAT] = 1.0 + RessourceRarity(RessourceType.WHEAT) / 10.0 - (double)Ressources[RessourceType.WHEAT];
                        RessourcesNeeds[RessourceType.WOOL] = 1.0 + RessourceRarity(RessourceType.WOOL) / 10.0 - (double)Ressources[RessourceType.WOOL];
                        RessourcesNeeds[RessourceType.ORE] = RessourceRarity(RessourceType.ORE) / 10.0 - (double)Ressources[RessourceType.ORE];
                    }
                }
                else
                {
                    RessourcesNeeds[RessourceType.BRICK] = 2.0 + RessourceRarity(RessourceType.BRICK) / 10.0 - (double)Ressources[RessourceType.BRICK];
                    RessourcesNeeds[RessourceType.LUMBER] = 2.0 + RessourceRarity(RessourceType.LUMBER) / 10.0 - (double)Ressources[RessourceType.LUMBER];
                    RessourcesNeeds[RessourceType.ORE] = RessourceRarity(RessourceType.ORE) / 10.0 - (double)Ressources[RessourceType.ORE];
                    RessourcesNeeds[RessourceType.WHEAT] = RessourceRarity(RessourceType.WHEAT) / 10.0 - (double)Ressources[RessourceType.WHEAT];
                    RessourcesNeeds[RessourceType.WOOL] = RessourceRarity(RessourceType.WOOL) / 10.0 - (double)Ressources[RessourceType.WOOL];
                }

                Dictionary<RessourceType, int> ressourcesToGiveUp = new Dictionary<RessourceType, int>();
                foreach(RessourceType type in ai.Ressources.Keys)
                {
                    ressourcesToGiveUp[type] = 0;
                }

                for(int i = ai.TotalRessourceNumber/2; i >= 0; i--)
                {
                    (RessourceType, double) worstType = (RessourceType.NONE, Double.MaxValue);
                    foreach(RessourceType type in ai.Ressources.Keys)
                    {
                        if (RessourcesNeeds[type] < worstType.Item2)
                        {
                            worstType = (type, RessourcesNeeds[type]);
                        }
                        ressourcesToGiveUp[worstType.Item1]++;
                        RessourcesNeeds[worstType.Item1] += 1.0;
                    }
                }

                arg = new List<(int, List<(RessourceType, int)>)>()
                {
                    (ai.Id, new List<(RessourceType, int)>(DictionaryToTupleList(ressourcesToGiveUp)))
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
            bool exchangeDone = true;
            while (exchangeDone)
            {
                exchangeDone = false;
                UpdateDatas();
                EvaluateRessourcesAccess();
                EvaluateIntersectionsScores();

                HashSet<RessourceType> ressourcesNeeded = new HashSet<RessourceType>();
                if(BestIntersection == null)
                {
                    // Objectif : Carte de développement
                    if(Ressources[RessourceType.ORE] < 1)
                    {
                        ressourcesNeeded.Add(RessourceType.ORE);
                    }
                    if (Ressources[RessourceType.WHEAT] < 1)
                    {
                        ressourcesNeeded.Add(RessourceType.WHEAT);
                    }
                    if (Ressources[RessourceType.WOOL] < 1)
                    {
                        ressourcesNeeded.Add(RessourceType.WOOL);
                    }
                    Ressources[RessourceType.ORE]--;
                    Ressources[RessourceType.WHEAT]--;
                    Ressources[RessourceType.WOOL]--;
                }
                else if (IntersectionsDistances[BestIntersection] == 0)
                {
                    if (Intersections[BestIntersection].Building == ConstructionType.SETTLEMENT)
                    {
                        // Objectif : Ville
                        if (Ressources[RessourceType.ORE] < 3)
                        {
                            ressourcesNeeded.Add(RessourceType.ORE);
                        }
                        if (Ressources[RessourceType.WHEAT] < 2)
                        {
                            ressourcesNeeded.Add(RessourceType.WHEAT);
                        }
                        Ressources[RessourceType.ORE] -= 3;
                        Ressources[RessourceType.WHEAT] -= 2;
                    }
                    else
                    {
                        // Objectif : Colonie
                        if (Ressources[RessourceType.BRICK] < 1)
                        {
                            ressourcesNeeded.Add(RessourceType.BRICK);
                        }
                        if (Ressources[RessourceType.LUMBER] < 1)
                        {
                            ressourcesNeeded.Add(RessourceType.LUMBER);
                        }
                        if (Ressources[RessourceType.WHEAT] < 1)
                        {
                            ressourcesNeeded.Add(RessourceType.WHEAT);
                        }
                        if (Ressources[RessourceType.WOOL] < 1)
                        {
                            ressourcesNeeded.Add(RessourceType.WOOL);
                        }
                        Ressources[RessourceType.LUMBER] -= 1;
                        Ressources[RessourceType.BRICK] -= 1;
                        Ressources[RessourceType.WOOL] -= 1;
                        Ressources[RessourceType.WHEAT] -= 1;
                    }
                }
                else
                {
                    // Objectif : Routes
                    if (Ressources[RessourceType.BRICK] < 2)
                    {
                        ressourcesNeeded.Add(RessourceType.BRICK);
                    }
                    if (Ressources[RessourceType.LUMBER] < 2)
                    {
                        ressourcesNeeded.Add(RessourceType.LUMBER);
                    }
                    Ressources[RessourceType.LUMBER] -= 2;
                    Ressources[RessourceType.BRICK] -= 2;
                }

                if (ressourcesNeeded.Count != 0)
                {
                    // Vérification des ports utilisables
                    int nbNeeded = (harbors.Contains(HarborType.GENERAL)) ? 3 : 4;
                    foreach (RessourceType type in ressourcesNeeded)
                    {
                        nbNeeded = (harbors.Contains(RessourceToHarbor[type])) ? 2 : nbNeeded;
                    }

                    // Choix de la ressource à donner
                    (RessourceType, double) ressourceToGive = (RessourceType.NONE, 0.0);
                    foreach (RessourceType type in ai.Ressources.Keys)
                    {
                        Ressources[type] -= nbNeeded;
                        double score = (double)Ressources[type] + RessourcesAccess[type] / 4.0;
                        if (Ressources[type] >= 0 && score > ressourceToGive.Item2)
                        {
                            ressourceToGive = (type, score);
                        }
                    }

                    if (ressourceToGive.Item1 != RessourceType.NONE)
                    {
                        // Suppressions des choix les moins intéressants
                        if (nbNeeded == 2)
                        {
                            foreach (RessourceType type in new List<RessourceType>(ressourcesNeeded))
                            {
                                if (!harbors.Contains(RessourceToHarbor[type]))
                                {
                                    ressourcesNeeded.Remove(type);
                                }
                            }
                        }

                        (RessourceType, double) ressourceToReceive = (RessourceType.NONE, Int32.MinValue);
                        foreach (RessourceType type in ressourcesNeeded)
                        {
                            double score = -(double)Ressources[type] - RessourcesAccess[type];
                            if (score > ressourceToReceive.Item2)
                            {
                                ressourceToReceive = (type, score);
                            }
                        }

                        HarborType harbor;
                        switch (nbNeeded)
                        {
                            case 2:
                                harbor = RessourceToHarbor[ressourceToReceive.Item1];
                                break;
                            case 3:
                                harbor = HarborType.GENERAL;
                                break;
                            default:
                                harbor = HarborType.NONE;
                                break;
                        }

                        GameView.Instance.OnHarborExchange(new HarborExchangeEventArgs(game.Id, harbor, ressourceToGive.Item1, ressourceToReceive.Item1, IsOnline));
                        exchangeDone = true;
                    }
                }
            }
            GameView.Instance.OnEndPhase(new BaseEventArgs(game.Id, IsOnline));
        }

        /// <inheritdoc/>
        public override void MoveBandit(IGame game)
        {
            GameView.Instance.OnBanditMove(BanditMoveArgs());
        }
    }
}
