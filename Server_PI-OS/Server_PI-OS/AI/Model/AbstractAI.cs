using System;
using System.Collections;
using System.Collections.Generic;
using Noyau.View;
using AI.View;
using System.Threading.Tasks;

namespace AI.Model
{
    abstract class AbstractAI : IAI
    {
        // Fonctions appelées par le noyau

        /// <summary>
        /// Initialise les attributs game et ai
        /// </summary>
        /// <param name="game">La partie en cours de l'IA</param>
        public virtual void InitAI(IGame game)
        {
            this.game = game;
            ai = game.Players[game.CurrentPlayer];
            DesertLocation = game.GameGrid.CurrentThiefLocation;
        }

        /// <inheritdoc/>
        public virtual void InitAI(IGame game, int playerId)
        {
            this.game = game;
            ai = game.Players[playerId];
            DesertLocation = game.GameGrid.CurrentThiefLocation;
        }

        /// <inheritdoc/>
        abstract public void PlaceFirstColony(IGame game);

        /// <inheritdoc/>
        abstract public void PlaceFirstRoad(IGame game);

        /// <inheritdoc/>
        abstract public void PlaceSecondColony(IGame game);

        /// <inheritdoc/>
        abstract public void PlaceSecondRoad(IGame game);

        /// <inheritdoc/>
        abstract public void HarvestBegin(IGame game);

        /// <inheritdoc/>
        abstract public void Construct(IGame game);

        /// <inheritdoc/>
        abstract public void Discard(IGame game);

        /// <inheritdoc/>
        abstract public void Exchange(IGame game);

        /// <inheritdoc/>
        abstract public void MoveBandit(IGame game);

        /// <summary>
        /// Réinitialise les attributs UsableCards, DevelopmentCardUsed et NumberOfUsableCards en début de tour
        /// </summary>
        public virtual void InitTurn()
        {
            UsableCards = new Dictionary<CardType, int>(ai.Cards);
            DevelopmentCardUsed = false;

            NumberOfUsableCards = 0;
            foreach (int number in UsableCards.Values)
            {
                NumberOfUsableCards += number;
            }

            NumberOfUsableCards -= UsableCards[CardType.VICTORY_POINT];
            UsableCards[CardType.VICTORY_POINT] = 0;
        }




        // Données diverses

        /// <value>La partie en cours de l'IA</value>
        protected IGame game;
        /// <value>Informations de l'IA dans le jeu</value>
        protected IPlayer ai;
        /// <value>True si une carte de développement à été utilisée durant le tour, false sinon</value>
        protected bool DevelopmentCardUsed;
        /// <value>Cartes utilisables durant ce tour</value>
        protected IDictionary<CardType, int> UsableCards;
        /// <value>Nombre de cartes potentiellement utilisables en début de tour</value>
        protected int NumberOfUsableCards;
        /// <value>Coordonnées du désert</value>
        protected Coordinate DesertLocation;
        /// <value>true si la partie est jouée sur le serveur, false sinon</value>
        /// <remarks>A changer entre la version serveur (true) et la version locale (false)</remarks>
        static protected Boolean IsOnline = false;
        /// <value>générateur de nombres aléatoires</value>
        static protected System.Random randGen = new System.Random();




        // Conversions

        /// <value>Conversion du type de terrain en type de ressource</value>
        static protected Dictionary<TerrainType, RessourceType> TerrainToRessource = new Dictionary<TerrainType, RessourceType>()
        {
            { TerrainType.NONE, RessourceType.NONE },
            { TerrainType.SEA, RessourceType.NONE },
            { TerrainType.DESERT, RessourceType.NONE },
            { TerrainType.FIELDS, RessourceType.WHEAT },
            { TerrainType.FOREST, RessourceType.LUMBER },
            { TerrainType.HILLS, RessourceType.BRICK },
            { TerrainType.MOUNTAINS, RessourceType.ORE },
            { TerrainType.PASTURE, RessourceType.WOOL }
        };
        /// <value>Conversion du type de port en type de ressource</value>
        static protected Dictionary<HarborType, RessourceType> HarborToRessource = new Dictionary<HarborType, RessourceType>()
        {
            { HarborType.NONE, RessourceType.NONE },
            { HarborType.GENERAL, RessourceType.NONE },
            { HarborType.WHEAT, RessourceType.WHEAT },
            { HarborType.LUMBER, RessourceType.LUMBER },
            { HarborType.BRICK, RessourceType.BRICK },
            { HarborType.ORE, RessourceType.ORE },
            { HarborType.WOOL, RessourceType.WOOL }
        };
        /// <value>Conversion du type de ressource en type de port</value>
        static protected Dictionary<RessourceType, HarborType> RessourceToHarbor = new Dictionary<RessourceType, HarborType>()
        {
            { RessourceType.NONE, HarborType.NONE },
            { RessourceType.WHEAT, HarborType.WHEAT },
            { RessourceType.LUMBER, HarborType.LUMBER },
            { RessourceType.BRICK, HarborType.BRICK },
            { RessourceType.ORE, HarborType.ORE },
            { RessourceType.WOOL, HarborType.WOOL }
        };



        // Fonctions utilitaires - Choix aléatoires

        /// <summary>
        /// Effectue un choix aléatoire dans un ensemble de catégories pondérées par leur probabilité relative d'être choisi
        /// </summary>
        /// <typeparam name="K">Le type de la catégorie</typeparam>
        /// <param name="quantities">Dictionaire contenant les catégories en clé, leur probabilité relative d'être choisi en valeur</param>
        /// <returns>Un élément choisi aléatoirement parmi les clés du dictionaire</returns>
        /// <exception cref="ArgumentException">La somme des probabilités relatives est nulle</exception>
        static protected K RandomChoice<K>(IDictionary<K, int> quantities)
        {
            int total = 0;
            foreach (int quantity in quantities.Values)
            {
                total += quantity;
            }

            if (total == 0)
            {
                throw new ArgumentException("Quantities have only zero values.");
            }

            IEnumerator<KeyValuePair<K, int>> e = quantities.GetEnumerator();
            for (int rand = randGen.Next(total); rand >= e.Current.Value;)
            {
                rand -= e.Current.Value;
                e.MoveNext();
            }

            return e.Current.Key;
        }


        /// <summary>
        /// Effectue un choix aléatoire d'un élément dans une liste non vide
        /// </summary>
        /// <typeparam name="T">Type des éléments de la liste</typeparam>
        /// <param name="list">Liste dans laquelle effectuer le choix aléatoire</param>
        /// <returns>Un élément aléatoire de la liste</returns>
        /// <exception cref="ArgumentException">La liste est vide</exception>
        static protected T RandomChoice<T>(IList<T> list)
        {
            if (list.Count == 0)
            {
                throw new ArgumentException("List is empty.");
            }

            return list[randGen.Next(list.Count)];
        }


        /// <summary>
        /// Effectue un choix aléatoire d'un élément dans une collection non vide
        /// </summary>
        /// <typeparam name="T">Type des éléments de la liste</typeparam>
        /// <param name="collection">Collection dans laquelle effectuer le choix aléatoire</param>
        /// <returns>Un élément aléatoire de la collection</returns>
        /// <exception cref="ArgumentException">La liste est vide</exception>
        static protected T RandomChoice<T>(ISet<T> collection)
        {
            if (collection.Count == 0)
            {
                throw new ArgumentException("Collection is empty.");
            }

            IEnumerator<T> e = collection.GetEnumerator();
            e.MoveNext();
            for (int i = randGen.Next(collection.Count); i > 0; i--)
            {
                e.MoveNext();
            }
            return e.Current;
        }


        /// <summary>
        /// Envoie une liste triée d'entiers aléatoires allant de 0 et strictement inférieurs à max
        /// </summary>
        /// <param name="max">Borne supérieure stricte des entiers aléatoires</param>
        /// <param name="numberOfIndices">Nombres d'entiers aléatoires à générer</param>
        /// <param name="uniques">Si true, les entiers renvoyés sont distincts</param>
        /// <returns>La liste triée d'entiers aléatoires</returns>
        /// <exception cref="ArgumentException">Les indices doivent être distincts, mais le nombre d'indices demandé est supérieur à la borne supérieure</exception>
        static protected List<int> RandomIndices(int max, int numberOfIndices, bool uniques)
        {
            List<int> indices = new List<int>(numberOfIndices);
            if (uniques)
            {
                if (numberOfIndices > max)
                {
                    throw new ArgumentException("unique indices, and maximum indice inferior than number of indices");
                }

                for (int i = 0; i < numberOfIndices; i++)
                {
                    int newInd = randGen.Next(max - i);
                    int j;
                    for (j = 0; j < i && indices[j] <= newInd; j++)
                    {
                        newInd++;
                    }
                    indices.Insert(j, newInd);
                }
            }
            else
            {
                for (int i = 0; i < numberOfIndices; i++)
                {
                    indices.Add(randGen.Next(max));
                }
            }

            return indices;
        }


        /// <summary>
        /// Effectue des choix aléatoires (tirages sans remise) dans un ensemble de catégories pondérées par leur probabilité relative d'être choisi
        /// </summary>
        /// <typeparam name="K">Le type de la catégorie</typeparam>
        /// <param name="quantities">Dictionaire contenant les catégories en clé, leur probabilité relative d'être choisi en valeur</param>
        /// <returns>Des éléments choisis aléatoirement parmi les clés du dictionaire</returns>
        /// <exception cref="ArgumentException">La somme des probabilités relatives est nulle</exception>
        static protected Dictionary<K, int> RandomChoices<K>(IDictionary<K, int> quantities, int numberOfChoices)
        {
            Dictionary<K, int> choices = new Dictionary<K, int>(quantities);

            int total = 0;
            foreach (K key in quantities.Keys)
            {
                total += quantities[key];
                choices[key] = 0;
            }

            List<int> indices = RandomIndices(total, numberOfChoices, true);

            IEnumerator<KeyValuePair<K, int>> e = quantities.GetEnumerator();
            e.MoveNext();
            total = e.Current.Value;
            for (int i = 0; i < numberOfChoices; i++)
            {
                while (total <= indices[i])
                {
                    e.MoveNext();
                    total += e.Current.Value;
                }
                choices[e.Current.Key]++;
            }

            return choices;
        }


        /// <summary>
        /// Effectue des choix aléatoires d'éléments distincts dans une liste non vide
        /// </summary>
        /// <typeparam name="T">Type des éléments de la liste</typeparam>
        /// <param name="list">Liste dans laquelle effectuer le choix aléatoire</param>
        /// <returns>Des éléments aléatoires distincts de la liste</returns>
        static protected List<T> RandomChoices<T>(IList<T> list, int numberOfChoices)
        {
            List<T> choices = new List<T>(numberOfChoices);
            List<int> indices = RandomIndices(list.Count, numberOfChoices, true);
            for (int i = 0; i < numberOfChoices; i++)
            {
                choices.Add(list[indices[i]]);
            }

            return choices;
        }


        /// <summary>
        /// Selectionne une ressource au hasard dans l'énumération RessourceType, RessourceType.NONE exclu
        /// </summary>
        /// <returns>La ressource choisie aléatoirement</returns>
        static protected RessourceType RandomRessource()
        {
            Array ressourcesTypes = Enum.GetValues(typeof(RessourceType));
            IEnumerator e = ressourcesTypes.GetEnumerator();
            e.MoveNext();

            if ((RessourceType)e.Current == RessourceType.NONE)
            {
                e.MoveNext();
            }

            for (int i = randGen.Next(ressourcesTypes.Length - 1); i > 0; i--)
            {
                e.MoveNext();
                if ((RessourceType)e.Current == RessourceType.NONE)
                {
                    e.MoveNext();
                }
            }

            return (RessourceType)e.Current;
        }


        /// <summary>
        /// Sélectionne aléatoirement l'indice d'un joueur dans une partie, AI exclu
        /// </summary>
        /// <param name="game">Partie dans laquelle la sélection est effectuée</param>
        /// <param name="ai">Joueur exclu de la sélection</param>
        /// <returns>Le joueur sélectionné aléatoirement</returns>
        static protected int RandomOtherPlayerId(IGame game, IPlayer ai)
        {
            int rand = randGen.Next(game.Players.Count - 1);
            if (rand >= ai.Id)
            {
                rand++;
            }

            return rand;
        }



        // Fonctions utilitaires - Conversions de collections

        /// <summary>
        /// Convertit un dictionnaire en liste de tuples (Clé, Valeur)
        /// </summary>
        /// <typeparam name="K">Clé</typeparam>
        /// <typeparam name="V">Valeur</typeparam>
        /// <param name="dict">Dictionnaire à convertir</param>
        /// <returns>La liste de tuples résultant de la convertion</returns>
        static protected List<(K, V)> DictionaryToTupleList<K, V>(IDictionary<K, V> dict)
        {
            List<(K, V)> list = new List<(K, V)>();
            foreach (KeyValuePair<K, V> pair in dict)
            {
                list.Add((pair.Key, pair.Value));
            }
            return list;
        }



        // Fonctions utilitaires - Plateau de jeu

        /// <summary>
        /// Renvoie les coordonnées des terrains adjacents à une intersection
        /// </summary>
        /// <param name="c">Coordonnées de l'intersection</param>
        /// <param name="game">Partie dans laquelle se trouve l'intersection</param>
        /// <returns>La liste des coordonnées des terrains adjacents</returns>
        /// <exception cref="ArgumentException">Les coordonnées passées en paramètre ne correspondent pas à une intersection</exception>
        static protected List<Coordinate> CoordinatesTilesFromIntersection(Coordinate c, IGame game)
        {
            List<Coordinate> coordinates;
            if (c.D.Equals(Direction.DOWN))
            {
                coordinates = new List<Coordinate>()
                {
                    new Coordinate(c.X, c.Y, c.Z, Direction.NONE),
                    new Coordinate(c.X-1, c.Y, c.Z+1, Direction.NONE),
                    new Coordinate(c.X, c.Y-1, c.Z+1, Direction.NONE)
                };
            }
            else if (c.D.Equals(Direction.UP))
            {
                coordinates = new List<Coordinate>()
                {
                    new Coordinate(c.X, c.Y, c.Z, Direction.NONE),
                    new Coordinate(c.X+1, c.Y, c.Z-1, Direction.NONE),
                    new Coordinate(c.X, c.Y+1, c.Z-1, Direction.NONE)
                };
            }
            else
            {
                throw new ArgumentException("Coordinate is not from a tile.");
            }

            for (int i = 2; i >= 0; i--)
            {
                if (!game.GameGrid.TerrainTiles.ContainsKey(coordinates[i]))
                {
                    coordinates.RemoveAt(i);
                }
            }
            return coordinates;
        }


        /// <summary>
        /// Renvoie les deux intersections adjacentes à une arête
        /// </summary>
        /// <param name="e">Coordonnées de l'arête</param>
        /// <returns>Un tuple de coordonnées contenant les coordonnées des intersections</returns>
        /// <exception cref="ArgumentException">Les coordonnées passées en paramètre ne correspondent pas à une arête</exception>
        static protected (Coordinate, Coordinate) IntersectionsFromEdge(Coordinate e)
        {
            switch (e.D)
            {
                case Direction.EAST:
                    return (new Coordinate(e.X, e.Y - 1, e.Z + 1, Direction.UP), new Coordinate(e.X + 1, e.Y, e.Z - 1, Direction.DOWN));
                case Direction.NORTH_EAST:
                    return (new Coordinate(e.X, e.Y, e.Z, Direction.UP), new Coordinate(e.X + 1, e.Y, e.Z - 1, Direction.DOWN));
                case Direction.SOUTH_EAST:
                    return (new Coordinate(e.X, e.Y - 1, e.Z + 1, Direction.UP), new Coordinate(e.X, e.Y, e.Z, Direction.DOWN));
                default:
                    throw new ArgumentException("Coordinate is not from an edge.");
            }
        }

        /// <summary>
        /// Renvoie l'arête adjacente commune à deux intersections voisines
        /// </summary>
        /// <param name="c1">Première intersection</param>
        /// <param name="c2">Seconde intersection</param>
        /// <returns>Les coordonnées de l'arête commune</returns>
        /// <exception cref="ArgumentException">L'une des coordonnées passées en paramètres ne correspondent pas à une intersection, ou les intersections ne sont pas voisines</exception>
        static protected Coordinate EdgeFromIntersections(Coordinate c1, Coordinate c2)
        {
            if (c1.D == Direction.DOWN && c2.D == Direction.UP)
            {
                Coordinate tmp = c2;
                c2 = c1;
                c1 = tmp;
            }

            if (c1.D != Direction.UP && c2.D != Direction.DOWN)
            {
                throw new ArgumentException("Coordinates are not from intersection from the same edge.");
            }

            if (c1.X == c2.X - 1 && c1.Y == c2.Y - 1 && c1.Z == c2.Z + 2)
            {
                return new Coordinate(c1.X, c1.Y + 1, c1.Z - 1, Direction.EAST);
            }
            else if (c1.X == c2.X - 1 && c1.Y == c2.Y && c1.Z == c2.Z + 1)
            {
                return new Coordinate(c1.X, c1.Y, c1.Z, Direction.NORTH_EAST);
            }
            else if (c1.X == c2.X && c1.Y == c2.Y - 1 && c1.Z == c2.Z + 1)
            {
                return new Coordinate(c1.X, c1.Y + 1, c1.Z - 1, Direction.SOUTH_EAST);
            }
            else
            {
                throw new ArgumentException("Coordinates are not from intersection from the same edge.");
            }
        }

        /// <summary>
        /// Renvoie la liste des intersections voisines à une intersection passée en paramètre
        /// </summary>
        /// <param name="c">Coordonnées de l'intersection</param>
        /// <param name="game">Partie dans laquelle se trouve l'intersection</param>
        /// <returns>La liste des coordonnées des intersections voisines</returns>
        /// <exception cref="ArgumentException">Les coordonnées passées en paramètre ne correspondent pas à une intersection</exception>
        static protected List<Coordinate> Neighbours(Coordinate c, IGame game)
        {
            List<Coordinate> neighbours;
            if (c.D.Equals(Direction.DOWN))
            {
                neighbours = new List<Coordinate>()
                {
                    new Coordinate(c.X-1, c.Y-1, c.Z+2, Direction.UP),
                    new Coordinate(c.X-1, c.Y, c.Z+1, Direction.UP),
                    new Coordinate(c.X, c.Y-1, c.Z+1, Direction.UP)
                };
            }
            else if (c.D.Equals(Direction.UP))
            {
                neighbours = new List<Coordinate>()
                {
                    new Coordinate(c.X+1, c.Y+1, c.Z-2, Direction.DOWN),
                    new Coordinate(c.X+1, c.Y, c.Z-1, Direction.DOWN),
                    new Coordinate(c.X, c.Y+1, c.Z-1, Direction.DOWN)
                };
            }
            else
            {
                throw new ArgumentException("Coordinate is not from an intersection.");
            }

            for (int i = 2; i >= 0; i--)
            {
                if (!game.GameGrid.Intersections.ContainsKey(neighbours[i]))
                {
                    neighbours.RemoveAt(i);
                }
            }
            return neighbours;
        }

        /// <summary>
        /// Renvoie la liste des colonies possibles durant la phase d'initialisation
        /// </summary>
        /// <param name="game">Partie dans laquelle se trouvent les emplacements de colonie possibles</param>
        /// <returns>La liste des coordonnées possibles</returns>
        static protected List<Coordinate> PossibleFirstColonies(IGame game)
        {
            HashSet<Coordinate> coordinates = new HashSet<Coordinate>(game.GameGrid.Intersections.Keys);
            foreach (IPlayer player in game.Players)
            {
                foreach ((Coordinate, ConstructionType) construction in player.Constructions)
                {
                    if (construction.Item2 == ConstructionType.SETTLEMENT)
                    {
                        coordinates.Remove(construction.Item1);
                        foreach (Coordinate c in Neighbours(construction.Item1, game))
                        {
                            coordinates.Remove(c);
                        }
                    }
                }
            }

            return new List<Coordinate>(coordinates);
        }
    }
}
