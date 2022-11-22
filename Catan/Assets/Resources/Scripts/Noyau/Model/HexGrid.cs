using System;
using System.Collections.Generic;
using Noyau.View;
using UnityEngine;
using Util.View;
using Random = System.Random;

namespace Noyau.Model
{
    /// <summary>
    /// <para>La classe HexGrid</para>
    /// <para>Représente le terrain de jeu.</para>
    /// <para>Contient les méthodes et le propriétés du terrain.</para>
    /// <para>Utilise les intersections, les arêtes, les cases et les coordonnées </para>
    /// </summary>
    /// <seealso cref="View.Coordinate"/>
    /// <seealso cref="View.IIntersection"/>
    /// <seealso cref="View.IEdge"/>
    /// <seealso cref="View.ITerrainTile"/>
    [Serializable]
    public class HexGrid : IHexGrid
    {
        /*
        Coordonnees stockees sous forme de string
        - Cases/Tiles : Systeme de coordonnees cubiques avec 3 axes x,y et z
        (0,0,0) est le centre de la grille
        exple: "1,0,1,NONE"
        Pour passer d'une case a une case adjacente, il suffit de faire +1,-1 dans 2 de ses coordonnees
        - Edges : "[Coordonnee case],[NE,E,SE]" (Nord-Est, Est, Sud-Est)
        exple: "2,1,0,SOUTH_EAST"
        - Intersections : "[Coordonnee case],[U/D]" (Up et Down)
        exple: "0,0,0,UP"
        */

        /// <inheritdoc/>
        public IDictionary<Coordinate, ITerrainTile> TerrainTiles { get; private set; }
        /// <value> Cast de TerrainTiles</value>
        /// <see cref="TerrainTiles"/>
        public TerrainTile GetTerrainTile(Coordinate coordinate)
        {
            if (TerrainTiles.ContainsKey(coordinate))
                return (TerrainTile)TerrainTiles[coordinate];
            else return null;
        }


        /// <inheritdoc/>
        public IDictionary<Coordinate, IEdge> Edges { get; set; }
        /// <value> Cast de Edges</value>
        /// <see cref="Edges"/>
        public Edge GetEdge(Coordinate coordinate)
        {
            if (Edges.ContainsKey(coordinate))
                return (Edge)Edges[coordinate];
            else return null;
        }


        /// <inheritdoc/>
        public IDictionary<Coordinate, IIntersection> Intersections { get; set; }
        /// <value> Cast de Intersection</value>
        /// <see cref="Intersections"/>
        public Intersection GetIntersection(Coordinate coordinate)
        {
            if (Intersections.ContainsKey(coordinate))
                return (Intersection)Intersections[coordinate];
            else return null;
        }

        /// <inheritdoc/>
        public Coordinate CurrentThiefLocation { get; private set; }

        /// <summary>
        /// Constructeur de HexGrid
        /// </summary>
        public HexGrid()
        {
            this.TerrainTiles = new Dictionary<Coordinate, ITerrainTile>();
            this.Edges = new Dictionary<Coordinate, IEdge>();
            this.Intersections = new Dictionary<Coordinate, IIntersection>();
        }

        /// <summary>
        /// Plateau de jeu comportant uniquement les arrêtes, les cases océans, les ports et les intersections
        /// </summary>
        /// <returns>Un plateau de jeu comportant uniquement les arrêtes, les cases océans, les ports et les intersections</returns>
        internal static HexGrid EmptyLayout()
        {
            HexGrid g = new HexGrid();
            int i, j, k, x, y, z;

            // Définition du nombre de ligne et de la moitié
            int lines = 7;
            int half = (7 - 1) / 2;

            // Initialisation des booléens
            bool buildUp = true;
            bool buildDown = true;
            bool buildNE = true;
            bool buildE = true;
            bool buildSE = true;
            for (i = 0; i < lines; i++)
            {
                k = 7 - Math.Abs(half - i);
                for (j = 0; j < k; j++)
                {
                    // Définition des coordonnées des cases en fonction de i
                    if (i <= half)
                    {
                        x = j - i;
                        y = half - j;
                        z = i - half;
                    }
                    else
                    {
                        x = j - half;
                        y = (lines - 1) - i - j;
                        z = i - half;
                    }



                    // Vérification de l'emplacement de la coordonnée sur le terrain
                    // En fonction de la coordonnées, certaines intersections et arêtes ne seront pas rajoutés
                    // Vérification de la création de case de mer.
                    if (y == 3)
                    {
                        buildUp = false;
                        buildNE = false;
                    }
                    if (z == -3)
                    {
                        buildUp = false;
                        buildNE = false;
                        buildE = false;
                    }
                    if (x == 3)
                    {
                        buildUp = false;
                        buildNE = false;
                        buildE = false;
                        buildSE = false;
                    }

                    if (x == -3)
                    {
                        buildDown = false;
                        buildSE = false;
                    }
                    if (z == 3)
                    {
                        buildDown = false;
                        buildE = false;
                        buildSE = false;
                    }
                    if (y == -3)
                    {
                        buildDown = false;
                        buildNE = false;
                        buildE = false;
                        buildSE = false;
                    }

                    // Création des arêtes en fonction des tests
                    if (buildNE)
                        g.Edges.Add(new Coordinate(x, y, z, Direction.NORTH_EAST), new Edge(ConstructionType.NONE, -1));
                    if (buildE)
                        g.Edges.Add(new Coordinate(x, y, z, Direction.EAST), new Edge(ConstructionType.NONE, -1));
                    if (buildSE)
                        g.Edges.Add(new Coordinate(x, y, z, Direction.SOUTH_EAST), new Edge(ConstructionType.NONE, -1));

                    // Création des intersections hautes, et des ports si besoin
                    if (buildUp)
                    {
                        if (x == 0 && y == 2 && z == -2)
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.UP), new Intersection(ConstructionType.NONE, -1, HarborType.ORE));
                        else if (x == 1 && y == 1 && z == -2)
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.UP), new Intersection(ConstructionType.NONE, -1, HarborType.WOOL));
                        else if (x == 2 && y == -1 && z == -1)
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.UP), new Intersection(ConstructionType.NONE, -1, HarborType.GENERAL));
                        else if (x == -2 && y == 2 && z == 0)
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.UP), new Intersection(ConstructionType.NONE, -1, HarborType.LUMBER));
                        else if (x == 2 && y == -3 && z == 1)
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.UP), new Intersection(ConstructionType.NONE, -1, HarborType.GENERAL));
                        else if (x == -3 && y == 0 && z == 3)
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.UP), new Intersection(ConstructionType.NONE, -1, HarborType.BRICK));
                        else if (x == -1 && y == -2 && z == 3)
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.UP), new Intersection(ConstructionType.NONE, -1, HarborType.GENERAL));
                        else if (x == -3 && y == 2 && z == 1)
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.UP), new Intersection(ConstructionType.NONE, -1, HarborType.GENERAL));
                        else if (x == 0 && y == -3 && z == 3)
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.UP), new Intersection(ConstructionType.NONE, -1, HarborType.WHEAT));
                        else
                        {
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.UP), new Intersection(ConstructionType.NONE, -1, HarborType.NONE));
                        }
                    }

                    // Création des intersections basses, et des ports si besoin
                    if (buildDown)
                    {
                        if (x == 0 && y == 3 && z == -3)
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.DOWN), new Intersection(ConstructionType.NONE, -1, HarborType.ORE));
                        else if (x == 2 && y == 1 && z == -3)
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.DOWN), new Intersection(ConstructionType.NONE, -1, HarborType.WOOL));
                        else if (x == 3 && y == 0 && z == -3)
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.DOWN), new Intersection(ConstructionType.NONE, -1, HarborType.GENERAL));
                        else if (x == -1 && y == 3 && z == -2)
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.DOWN), new Intersection(ConstructionType.NONE, -1, HarborType.LUMBER));
                        else if (x == 3 && y == -2 && z == -1)
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.DOWN), new Intersection(ConstructionType.NONE, -1, HarborType.GENERAL));
                        else if (x == -2 && y == 0 && z == 2)
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.DOWN), new Intersection(ConstructionType.NONE, -1, HarborType.BRICK));
                        else if (x == 0 && y == -2 && z == 2)
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.DOWN), new Intersection(ConstructionType.NONE, -1, HarborType.GENERAL));
                        else if (x == -2 && y == 2 && z == 0)
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.DOWN), new Intersection(ConstructionType.NONE, -1, HarborType.GENERAL));
                        else if (x == 1 && y == -2 && z == 1)
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.DOWN), new Intersection(ConstructionType.NONE, -1, HarborType.WHEAT));
                        else
                        {
                            g.Intersections.Add(new Coordinate(x, y, z, Direction.DOWN), new Intersection(ConstructionType.NONE, -1, HarborType.NONE));
                        }
                    }

                    // Création des cases de mer


                    // Réinitalisation des booléens
                    buildNE = true;
                    buildE = true;
                    buildSE = true;
                    buildUp = true;
                    buildDown = true;
                }
            }


            return g;
        }

        /// <summary>
        /// Méthode créant un plateau de jeu comportant les Tuiles, Aretes et Intersections d'un layout de base
        /// </summary>
        /// <remarks>
        /// <para> La méthode est appelée uniquement lors de la création de terrain personnalisé</para>
        /// <para> La méthode appelle EmptyLayout</para>
        /// </remarks>
        /// <returns>Un plateau de jeu comportant les Tuiles, Aretes et Intersections d'un layout de base</returns>
        internal static HexGrid CreateDefaultEmptyGrid()
        {
            int i, j, k;

            int lines = 5;
            int half = (lines - 1) / 2;
            HexGrid g = EmptyLayout();

            Coordinate coor;

            for (i = 0; i < lines; i++)
            {
                k = 5 - Math.Abs(half - i);
                for (j = 0; j < k; j++)
                {
                    if (i <= half)
                        coor = new Coordinate(j - i, half - j, i - half, Direction.NONE);
                    else
                        coor = new Coordinate(j - half, (lines - 1) - i - j, i - half, Direction.NONE);
                    g.TerrainTiles.Add(coor, new TerrainTile(TerrainType.NONE, -1, false));
                }
            }
            g.CurrentThiefLocation = new Coordinate(0, 0, 0, Direction.NONE);
            return g;
        }

        //
        /// <summary>
        /// Méthode créant un plateau de jeu al�atoire conforme au r�gles de base
        /// </summary>
        /// <remarks>
        /// <para> La méthode utilise l'algorithme de Fisher-Yates : https://fr.wikipedia.org/wiki/M%C3%A9lange_de_Fisher-Yates</para>
        /// <para> La méthode appelle EmptyLayout</para>
        /// </remarks>
        /// <returns>un plateau de jeu al�atoire conforme au r�gles de base</returns>
        internal static HexGrid CreateRandomGrid()
        {
            List<int> tileNumbers = new List<int> { 2, 3, 3, 4, 4, 5, 5, 6, 6, 8, 8, 9, 9, 10, 10, 11, 11, 12 };
            List<TerrainType> tileTerrains = new List<TerrainType> { TerrainType.DESERT, TerrainType.PASTURE, TerrainType.PASTURE, TerrainType.PASTURE,
                        TerrainType.PASTURE,TerrainType.FOREST,TerrainType.FOREST,TerrainType.FOREST,TerrainType.FOREST,TerrainType.MOUNTAINS,
                        TerrainType.MOUNTAINS,TerrainType.MOUNTAINS,TerrainType.HILLS,TerrainType.HILLS,TerrainType.HILLS,TerrainType.FIELDS,
                        TerrainType.FIELDS,TerrainType.FIELDS,TerrainType.FIELDS};
            Random r = new Random();
            int i, j, tmp, k, lines, half;
            TerrainType tmp2;
            lines = 5;
            half = (lines - 1) / 2;
            HexGrid g = EmptyLayout();
            Coordinate coor;

            // Mélange de Fisher-Yates https://fr.wikipedia.org/wiki/M%C3%A9lange_de_Fisher-Yates
            // Randomize tileNumber array
            for (i = tileNumbers.Count - 1; i > 0; i--)
            {

                // Pick a random index
                // from 0 to i
                j = r.Next(0, i + 1);

                // Swap arr[i] with the
                // element at random index
                tmp = tileNumbers[i];
                tileNumbers[i] = tileNumbers[j];
                tileNumbers[j] = tmp;
            }
            for (i = tileTerrains.Count - 1; i > 0; i--)
            {

                // Pick a random index
                // from 0 to i
                j = r.Next(0, i + 1);

                // Swap arr[i] with the
                // element at random index
                tmp2 = tileTerrains[i];
                tileTerrains[i] = tileTerrains[j];
                tileTerrains[j] = tmp2;
            }

            for (i = 0; i < lines; i++)
            {
                k = 5 - Math.Abs(half - i);
                for (j = 0; j < k; j++)
                {
                    tmp2 = tileTerrains[0];
                    tileTerrains.RemoveAt(0);
                    tmp = 0;
                    if (i <= half)
                        coor = new Coordinate(j - i, half - j, i - half, Direction.NONE);
                    else
                        coor = new Coordinate(j - half, (lines - 1) - i - j, i - half, Direction.NONE);
                    if (!tmp2.Equals(TerrainType.DESERT))
                    {
                        tmp = tileNumbers[0];
                        tileNumbers.RemoveAt(0);
                        g.TerrainTiles.Add(coor, new TerrainTile(tmp2, tmp, false));
                    }
                    else
                    {
                        g.CurrentThiefLocation = coor;
                        g.TerrainTiles.Add(coor, new TerrainTile(tmp2, tmp, true));
                    }
                }
            }
            return g;


        }


        /// <summary>
        /// <para>Deplace les brigands sur le plateau de jeu</para>
        /// <para>Change la coordonnee dans Hexgrid + les booleens dans les TerrainTiles correspondants</para>
        /// </summary>
        /// <param name="tileLocation">La nouvelle position des brigands</param>
        /// <exception cref="Exception">Jetée lorsque la coordonnée n'est pas valide</exception>
        public void MoveThief(Coordinate tileLocation)
        {
            if (!this.TerrainTiles.ContainsKey(tileLocation))
                throw new Exception("Can't move thief to a non existent location");
            if (this.TerrainTiles[tileLocation].Type.Equals(TerrainType.SEA))
                throw new Exception("Oy mate, dem thieves ain't pirates ! They can't swim in there !");
            this.TerrainTiles[this.CurrentThiefLocation].ThiefIsPresent = false;
            this.TerrainTiles[tileLocation].ThiefIsPresent = true;

            this.CurrentThiefLocation = tileLocation;
        }

        /// <summary>
        /// <para>Méthode qui permet de construire une nouvelle route. </para>
        /// <para>Emplacement doit �tre valide pour le joueur, et il doit avoir les ressources n�cessaires.</para>
        /// </summary>
        /// <param name="p">Une copie du joueur construisant la route</param>
        /// <param name="roadLocation">L'emplacement de la route</param>
        /// <returns>Le nouvel état du joueur</returns>
        /// <exception cref="Exception"> Jetée lorsque le joueur ne peut pas construire de route à la coordonnée donnée </exception>
        public Player ConstructRoad(Player p, Coordinate roadLocation, bool isDevCard)
        {
            if (!PossibleRoads(p).Contains(roadLocation))
                throw new Exception("Can't build road here");
            if (!isDevCard)
            {
                if (!p.HasEnoughRessources(ConstructionType.ROAD))
                    throw new Exception("Can't build road, not enough ressources");
                p.RemoveRessource(RessourceType.BRICK, 1);
                p.RemoveRessource(RessourceType.LUMBER, 1);
            }

            p.Constructions.Add((roadLocation, ConstructionType.ROAD));
            Edges[roadLocation].AddBuilding(ConstructionType.ROAD, p.Id);
            return p;
        }
        /// <inheritdoc/>
        public List<Coordinate> PossibleRoads(IPlayer p)
        {
            List<Coordinate> res = new List<Coordinate>();
            Coordinate c, c1;
            for (int i = 0; i < p.Constructions.Count; i++)
            {
                c = p.Constructions[i].Item1;
                if (c.D.Equals(Direction.UP))
                {
                    c1 = new Coordinate(c.X, c.Y, c.Z, Direction.NORTH_EAST);
                    if (Edges.ContainsKey(c1))
                    {
                        if (Edges[c1].Building.Equals(ConstructionType.NONE) && Edges[c1].PlayerId.Equals(-1)
                            && !res.Contains(c1))
                            res.Add(c1);
                    }

                    c1 = new Coordinate(c.X, c.Y + 1, c.Z - 1, Direction.SOUTH_EAST);
                    if (Edges.ContainsKey(c1))
                    {
                        if (Edges[c1].Building.Equals(ConstructionType.NONE) && Edges[c1].PlayerId.Equals(-1)
                            && !res.Contains(c1))
                            res.Add(c1);
                    }

                    c1 = new Coordinate(c.X, c.Y + 1, c.Z - 1, Direction.EAST);
                    if (Edges.ContainsKey(c1))
                    {
                        if (Edges[c1].Building.Equals(ConstructionType.NONE) && Edges[c1].PlayerId.Equals(-1)
                            && !res.Contains(c1))
                            res.Add(c1);
                    }
                }
                else if (c.D.Equals(Direction.DOWN))
                {
                    c1 = new Coordinate(c.X, c.Y, c.Z, Direction.SOUTH_EAST);
                    if (Edges.ContainsKey(c1))
                    {
                        if (Edges[c1].Building.Equals(ConstructionType.NONE) && Edges[c1].PlayerId.Equals(-1)
                            && !res.Contains(c1))
                            res.Add(c1);
                    }

                    c1 = new Coordinate(c.X - 1, c.Y, c.Z + 1, Direction.NORTH_EAST);
                    if (Edges.ContainsKey(c1))
                    {
                        if (Edges[c1].Building.Equals(ConstructionType.NONE) && Edges[c1].PlayerId.Equals(-1)
                            && !res.Contains(c1))
                            res.Add(c1);
                    }

                    c1 = new Coordinate(c.X - 1, c.Y, c.Z + 1, Direction.EAST);
                    if (Edges.ContainsKey(c1))
                    {
                        if (Edges[c1].Building.Equals(ConstructionType.NONE) && Edges[c1].PlayerId.Equals(-1)
                            && !res.Contains(c1))
                            res.Add(c1);
                    }
                }
                else if (c.D.Equals(Direction.NORTH_EAST))
                {
                    c1 = new Coordinate(c.X, c.Y, c.Z, Direction.UP);
                    if (!(!(Intersections[c1].Building.Equals(ConstructionType.NONE))
                        && !(Intersections[c1].PlayerId.Equals(p.Id))))
                    {
                        c1 = new Coordinate(c.X, c.Y + 1, c.Z - 1, Direction.SOUTH_EAST);
                        if (Edges.ContainsKey(c1))
                        {
                            if (Edges[c1].Building.Equals(ConstructionType.NONE) && Edges[c1].PlayerId.Equals(-1)
                                && !res.Contains(c1))
                                res.Add(c1);
                        }

                        c1 = new Coordinate(c.X, c.Y + 1, c.Z - 1, Direction.EAST);
                        if (Edges.ContainsKey(c1))
                        {
                            if (Edges[c1].Building.Equals(ConstructionType.NONE) && Edges[c1].PlayerId.Equals(-1)
                                && !res.Contains(c1))
                                res.Add(c1);
                        }
                    }

                    c1 = new Coordinate(c.X + 1, c.Y, c.Z - 1, Direction.DOWN);
                    if (!(!(Intersections[c1].Building.Equals(ConstructionType.NONE))
                        && !(Intersections[c1].PlayerId.Equals(p.Id))))
                    {
                        c1 = new Coordinate(c.X, c.Y, c.Z, Direction.EAST);
                        if (Edges.ContainsKey(c1))
                        {
                            if (Edges[c1].Building.Equals(ConstructionType.NONE) && Edges[c1].PlayerId.Equals(-1)
                                && !res.Contains(c1))
                                res.Add(c1);
                        }

                        c1 = new Coordinate(c.X + 1, c.Y, c.Z - 1, Direction.SOUTH_EAST);
                        if (Edges.ContainsKey(c1))
                        {
                            if (Edges[c1].Building.Equals(ConstructionType.NONE) && Edges[c1].PlayerId.Equals(-1)
                                && !res.Contains(c1))
                                res.Add(c1);
                        }
                    }
                }
                else if (c.D.Equals(Direction.EAST))
                {
                    c1 = new Coordinate(c.X + 1, c.Y, c.Z - 1, Direction.DOWN);
                    if (!(!(Intersections[c1].Building.Equals(ConstructionType.NONE))
                        && !(Intersections[c1].PlayerId.Equals(p.Id))))
                    {
                        c1 = new Coordinate(c.X + 1, c.Y, c.Z - 1, Direction.SOUTH_EAST);
                        if (Edges.ContainsKey(c1))
                        {
                            if (Edges[c1].Building.Equals(ConstructionType.NONE) && Edges[c1].PlayerId.Equals(-1)
                                && !res.Contains(c1))
                                res.Add(c1);
                        }

                        c1 = new Coordinate(c.X, c.Y, c.Z, Direction.NORTH_EAST);
                        if (Edges.ContainsKey(c1))
                        {
                            if (Edges[c1].Building.Equals(ConstructionType.NONE) && Edges[c1].PlayerId.Equals(-1)
                                && !res.Contains(c1))
                                res.Add(c1);
                        }
                    }

                    c1 = new Coordinate(c.X, c.Y - 1, c.Z + 1, Direction.UP);
                    if (!(!(Intersections[c1].Building.Equals(ConstructionType.NONE))
                        && !(Intersections[c1].PlayerId.Equals(p.Id))))
                    {
                        c1 = new Coordinate(c.X, c.Y - 1, c.Z + 1, Direction.NORTH_EAST);
                        if (Edges.ContainsKey(c1))
                        {
                            if (Edges[c1].Building.Equals(ConstructionType.NONE) && Edges[c1].PlayerId.Equals(-1)
                                && !res.Contains(c1))
                                res.Add(c1);
                        }

                        c1 = new Coordinate(c.X, c.Y, c.Z, Direction.SOUTH_EAST);
                        if (Edges.ContainsKey(c1))
                        {
                            if (Edges[c1].Building.Equals(ConstructionType.NONE) && Edges[c1].PlayerId.Equals(-1)
                                && !res.Contains(c1))
                                res.Add(c1);
                        }
                    }
                }
                else
                {
                    c1 = new Coordinate(c.X, c.Y, c.Z, Direction.DOWN);
                    if (!(!(Intersections[c1].Building.Equals(ConstructionType.NONE))
                        && !(Intersections[c1].PlayerId.Equals(p.Id))))
                    {
                        c1 = new Coordinate(c.X - 1, c.Y, c.Z + 1, Direction.NORTH_EAST);
                        if (Edges.ContainsKey(c1))
                        {
                            if (Edges[c1].Building.Equals(ConstructionType.NONE) && Edges[c1].PlayerId.Equals(-1)
                                && !res.Contains(c1))
                                res.Add(c1);
                        }

                        c1 = new Coordinate(c.X - 1, c.Y, c.Z + 1, Direction.EAST);
                        if (Edges.ContainsKey(c1))
                        {
                            if (Edges[c1].Building.Equals(ConstructionType.NONE) && Edges[c1].PlayerId.Equals(-1)
                                && !res.Contains(c1))
                                res.Add(c1);
                        }
                    }

                    c1 = new Coordinate(c.X, c.Y - 1, c.Z + 1, Direction.UP);
                    if (!(!(Intersections[c1].Building.Equals(ConstructionType.NONE))
                        && !(Intersections[c1].PlayerId.Equals(p.Id))))
                    {
                        c1 = new Coordinate(c.X, c.Y - 1, c.Z + 1, Direction.NORTH_EAST);
                        if (Edges.ContainsKey(c1))
                        {
                            if (Edges[c1].Building.Equals(ConstructionType.NONE) && Edges[c1].PlayerId.Equals(-1)
                                && !res.Contains(c1))
                                res.Add(c1);
                        }

                        c1 = new Coordinate(c.X, c.Y, c.Z, Direction.EAST);
                        if (Edges.ContainsKey(c1))
                        {
                            if (Edges[c1].Building.Equals(ConstructionType.NONE) && Edges[c1].PlayerId.Equals(-1)
                                && !res.Contains(c1))
                                res.Add(c1);
                        }

                    }
                }
            }
            return res;
        }

        /// <summary>
        /// <para>Méthode qui permet de construire une nouvelle colonie. </para>
        /// <para>Emplacement doit �tre valide pour le joueur, et il doit avoir les ressources n�cessaires.</para>
        /// </summary>
        /// <param name="p">Une copie du joueur construisant la colonie</param>
        /// <param name="roadLocation">L'emplacement de la colonie</param>
        /// <returns>Le nouvel état du joueur</returns>
        /// <exception cref="Exception"> Jetée lorsque le joueur ne peut pas construire de colonie à la coordonnée donnée </exception>
        public Player ConstructColony(Player p, Coordinate colonyLocation, bool isInit)
        {
            

            if (!isInit)
            {
                if (!PossibleColonies(p).Contains(colonyLocation))
                    throw new Exception("Can't build colony here");
                if (!p.HasEnoughRessources(ConstructionType.SETTLEMENT))
                    throw new Exception("Can't build colony, not enough ressources");

                p.RemoveRessource(RessourceType.BRICK, 1);
                p.RemoveRessource(RessourceType.WHEAT, 1);
                p.RemoveRessource(RessourceType.WOOL, 1);
                p.RemoveRessource(RessourceType.LUMBER, 1);
            }
            
            p.Constructions.Add((colonyLocation, ConstructionType.SETTLEMENT));
            Intersections[colonyLocation].AddBuilding(ConstructionType.SETTLEMENT, p.Id);
            return p;
        }


        /// <summary>
        /// Vérifie si une colonie peut être construite à la coordonnée entrée en paramètre
        /// </summary>
        /// <param name="c1">La coordonnée dont on veut vérifier le voisinnage</param>
        /// <returns>True si la colonie peut être construite, false sinon.</returns>
        public bool CheckColony(Coordinate c1)
        {
            bool b1 = false;
            bool b2 = false;
            bool b3 = false;
            Coordinate c2, c3, c4;
            if (!Intersections[c1].Building.Equals(ConstructionType.NONE))
                return false;
            if (c1.D.Equals(Direction.UP))
            {

                c2 = new Coordinate(c1.X, c1.Y + 1, c1.Z - 1, Direction.DOWN);
                c3 = new Coordinate(c1.X + 1, c1.Y, c1.Z - 1, Direction.DOWN);
                c4 = new Coordinate(c1.X + 1, c1.Y + 1, c1.Z - 2, Direction.DOWN);
                if (Intersections.ContainsKey(c2))
                {
                    if (Intersections[c2].Building.Equals(ConstructionType.NONE))
                    {
                        b1 = true;
                    }
                }
                else
                    b1 = true;

                if (Intersections.ContainsKey(c3))
                {
                    if (Intersections[c3].Building.Equals(ConstructionType.NONE))
                    {
                        b2 = true;
                    }
                }
                else
                    b2 = true;

                if (Intersections.ContainsKey(c4))
                {
                    if (Intersections[c4].Building.Equals(ConstructionType.NONE))
                    {
                        b3 = true;
                    }
                }
                else
                    b3 = true;

                return b1 && b2 && b3;
            }
            else
            {
                c2 = new Coordinate(c1.X, c1.Y - 1, c1.Z + 1, Direction.UP);
                c3 = new Coordinate(c1.X - 1, c1.Y, c1.Z + 1, Direction.UP);
                c4 = new Coordinate(c1.X - 1, c1.Y - 1, c1.Z + 2, Direction.UP);
                if (Intersections.ContainsKey(c2))
                {
                    if (Intersections[c2].Building.Equals(ConstructionType.NONE))
                    {
                        b1 = true;
                    }
                }
                else
                    b1 = true;

                if (Intersections.ContainsKey(c3))
                {
                    if (Intersections[c3].Building.Equals(ConstructionType.NONE))
                    {
                        b2 = true;
                    }
                }
                else
                    b2 = true;

                if (Intersections.ContainsKey(c4))
                {
                    if (Intersections[c4].Building.Equals(ConstructionType.NONE))
                    {
                        b3 = true;
                    }
                }
                else
                    b3 = true;

                return b1 && b2 && b3;
            }
        }
        /// <summary>
        /// <para>Méthode qui permet de construire une nouvelle colonie. </para>
        /// <para>Emplacement doit �tre valide pour le joueur, et il doit avoir les ressources n�cessaires.</para>
        /// </summary>
        /// <param name="p">Une copie du joueur construisant la colonie</param>
        /// <param name="roadLocation">L'emplacement de la colonie</param>
        /// <returns>Le nouvel état du joueur</returns>
        /// <exception cref="Exception"> Jetée lorsque le joueur ne peut pas construire de colonie à la coordonnée donnée </exception>
        public List<Coordinate> PossibleColonies(IPlayer p)
        {
            List<Coordinate> res = new List<Coordinate>();
            Coordinate c, c1;

            for (int i = 0; i < p.Constructions.Count; i++)
            {
                c = p.Constructions[i].Item1;
                if (c.D.Equals(Direction.NORTH_EAST))
                {
                    c1 = new Coordinate(c.X, c.Y, c.Z, Direction.UP);
                    if (CheckColony(c1) && !res.Contains(c1))
                        res.Add(c1);

                    c1 = new Coordinate(c.X + 1, c.Y, c.Z - 1, Direction.DOWN);
                    if (CheckColony(c1) && !res.Contains(c1))
                        res.Add(c1);
                }
                else if (c.D.Equals(Direction.EAST))
                {
                    c1 = new Coordinate(c.X + 1, c.Y, c.Z - 1, Direction.DOWN);
                    if (CheckColony(c1) && !res.Contains(c1))
                        res.Add(c1);

                    c1 = new Coordinate(c.X, c.Y - 1, c.Z + 1, Direction.UP);
                    if (CheckColony(c1) && !res.Contains(c1))
                        res.Add(c1);
                }
                else if (c.D.Equals(Direction.SOUTH_EAST))
                {
                    c1 = new Coordinate(c.X, c.Y - 1, c.Z + 1, Direction.UP);
                    if (CheckColony(c1) && !res.Contains(c1))
                        res.Add(c1);

                    c1 = new Coordinate(c.X, c.Y, c.Z, Direction.DOWN);
                    if (CheckColony(c1) && !res.Contains(c1))
                        res.Add(c1);
                }
            }
            return res;
        }

        

        /// <summary>
        /// <para>Méthode qui permet de construire une nouvelle ville. </para>
        /// <para>Emplacement doit �tre valide pour le joueur, et il doit avoir les ressources n�cessaires.</para>
        /// </summary>
        /// <param name="p">Une copie du joueur construisant la ville</param>
        /// <param name="roadLocation">L'emplacement de la ville</param>
        /// <returns>Le nouvel état du joueur</returns>
        /// <exception cref="Exception"> Jetée lorsque le joueur ne peut pas construire de ville à la coordonnée donnée </exception>
        public Player ConstructCity(Player p, Coordinate cityLocation)
        {

            if (!PossibleCities(p).Contains(cityLocation))
                throw new Exception("Can't build city here");
            if (!p.HasEnoughRessources(ConstructionType.CITY))
                throw new Exception("Can't build city, not enough ressources");

            p.RemoveRessource(RessourceType.ORE, 3);
            p.RemoveRessource(RessourceType.WHEAT, 2);
            p.Constructions.Remove((cityLocation, ConstructionType.SETTLEMENT));
            p.Constructions.Add((cityLocation, ConstructionType.CITY));
            Intersections[cityLocation].AddBuilding(ConstructionType.CITY, p.Id);
            return p;
        }
        /// <inheritdoc/>
        public List<Coordinate> PossibleCities(IPlayer p)
        {
            List<Coordinate> res = new List<Coordinate>();
            Coordinate c;

            for (int i = 0; i < p.Constructions.Count; i++)
            {
                c = p.Constructions[i].Item1;
                if (p.Constructions[i].Item2.Equals(ConstructionType.SETTLEMENT))
                    res.Add(c);
            }

            return res;
        }

        /// <inheritdoc/>
        public List<IIntersection> GetIntersectionsFromTile(Coordinate t)
        {
            Console.WriteLine(t);

            if (!t.D.Equals(Direction.NONE))
                throw new Exception("Coordinate not from a tile");

            if (TerrainTiles[t].Type.Equals(TerrainType.SEA))
                throw new Exception("Coordinate is from a sea tile");
            Coordinate c;
            List<IIntersection> res = new List<IIntersection>();


            c = new Coordinate(t.X, t.Y, t.Z, Direction.UP);

            res.Add(Intersections[c]);
            c = new Coordinate(t.X, t.Y, t.Z, Direction.DOWN);
            res.Add(Intersections[c]);

            c = new Coordinate(t.X + 1, t.Y, t.Z - 1, Direction.DOWN);
            res.Add(Intersections[c]);
            c = new Coordinate(t.X, t.Y + 1, t.Z - 1, Direction.DOWN);
            res.Add(Intersections[c]);

            c = new Coordinate(t.X - 1, t.Y, t.Z + 1, Direction.UP);
            res.Add(Intersections[c]);
            c = new Coordinate(t.X, t.Y - 1, t.Z + 1, Direction.UP);
            res.Add(Intersections[c]);


            return res;

        }

        /// <summary>
        /// Renvoie les liste des cases autour d"une intersection
        /// </summary>
        /// <param name="t">La coordonnée de l'intersection</param>
        /// <returns>La liste d'au plus 3 cases autour de l'intersection</returns>
        public List<ITerrainTile> GetTileFromIntersection(Coordinate t)
        {
            Console.WriteLine(t);

            List<ITerrainTile> res = new List<ITerrainTile>();
            Coordinate c;
            if (!(t.X == 3 || t.X == -3 || t.Y == 3 || t.Y == -3 || t.Z == 3 || t.Z == -3))
            {
                c = new Coordinate(t.X, t.Y, t.Z, Direction.NONE);
                res.Add(TerrainTiles[c]);
            }

            if (t.D.Equals(Direction.DOWN))
            {
                if (t.Y != 3 && t.X != -2 && t.Z != 2)
                {
                    c = new Coordinate(t.X - 1, t.Y, t.Z + 1, Direction.NONE);
                    res.Add(TerrainTiles[c]);
                }
                if (t.X != 3 && t.Y != -2 && t.Z != 2)
                {
                    c = new Coordinate(t.X, t.Y - 1, t.Z + 1, Direction.NONE);
                    res.Add(TerrainTiles[c]);
                }


            }
            else if (t.D.Equals(Direction.UP))
            {
                if (t.Y != -3 && t.X != 2 && t.Z != -2)
                {
                    c = new Coordinate(t.X + 1, t.Y, t.Z - 1, Direction.NONE);
                    res.Add(TerrainTiles[c]);
                }
                if (t.X != -3 && t.Y != 2 && t.Z != -2)
                {
                    c = new Coordinate(t.X, t.Y + 1, t.Z - 1, Direction.NONE);
                    res.Add(TerrainTiles[c]);
                }

            }
            else
            {
                throw new ArgumentException("Coordinate is not from a tile.");
            }
            return res;
        }

        /// <summary>
        /// Fonction permettant de serialiser la classe HexGrid.
        /// Cette fonction est necessaire pour le transfert d'informations par le reseau.
        /// </summary>
        /// <returns>Une chaine de caracteres correspondant a la classe serialisee</returns>
        public string Serialize()
        {
            string chain = "";
            chain += "<CurrentThiefLocation>" + CurrentThiefLocation.Serialize() + "</CurrentThiefLocation>";
            chain += "<TerrainTiles>";
            foreach (KeyValuePair<Coordinate, ITerrainTile> entry in (Dictionary<Coordinate, ITerrainTile>)TerrainTiles)
            {
                chain += entry.Key.Serialize() + "#" + entry.Value.Serialize() + "|";
            }
            chain += "</TerrainTiles>";
            chain += "<Edges>";
            foreach (KeyValuePair<Coordinate, IEdge> entry in (Dictionary<Coordinate, IEdge>)Edges)
            {
                chain += entry.Key.Serialize() + "#" + entry.Value.Serialize() + "|";
            }
            chain += "</Edges>";
            chain += "<Intersections>";
            foreach (KeyValuePair<Coordinate, IIntersection> entry in (Dictionary<Coordinate, IIntersection>)Intersections)
            {
                chain += entry.Key.Serialize() + "#" + entry.Value.Serialize() + "|";
            }
            chain += "</Intersections>";
            return chain;
        }


        /// <summary>
        /// Fonction permettant de deserialiser la classe HexGrid.
        /// Cette fonction est necessaire pour le transfert d'informations par le reseau.
        /// </summary>
        /// <param name="serializedClass">Une chaine de caracteres correspondant a la classe serialisee</param>
        public void Deserialize(string serializedData)
        {
            string str = GetSubString(serializedData, "<CurrentThiefLocation>", "</CurrentThiefLocation>");
            Coordinate cord = new Coordinate();
            cord.Deserialize(str);
            CurrentThiefLocation = cord;



            str = GetSubString(serializedData, "<TerrainTiles>", "</TerrainTiles>");
            char[] seps = { '|' };
            char[] subSeps = { '#' };
            string[] parts = str.Split(seps);
            TerrainTiles.Clear();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Length > 0)
                {
                    string[] subParts = parts[i].Split(subSeps);
                    if (subParts.Length > 0)
                    {
                        Coordinate c = new Coordinate(); c.Deserialize(subParts[0]);
                        TerrainTile t = new TerrainTile(); t.Deserialize(subParts[1]);
                        if (!TerrainTiles.ContainsKey(c))
                            TerrainTiles.Add(c, t);
                    }
                }
            }
            str = GetSubString(serializedData, "<Edges>", "</Edges>");
            parts = str.Split(seps);
            Edges.Clear();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Length > 0)
                {
                    string[] subParts = parts[i].Split(subSeps);
                    if (subParts.Length > 0)
                    {
                        Coordinate c = new Coordinate(); c.Deserialize(subParts[0]);
                        Edge e = new Edge(); e.Deserialize(subParts[1]);
                        if (!Edges.ContainsKey(c))
                        {
                            Edges.Add(c, e);
                        }
                            
                    }
                }
            }
            str = GetSubString(serializedData, "<Intersections>", "</Intersections>");
            parts = str.Split(seps);
            Intersections.Clear();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Length > 0)
                {
                    string[] subParts = parts[i].Split(subSeps);
                    if (subParts.Length > 0)
                    {
                        Coordinate c = new Coordinate(); c.Deserialize(subParts[0]);
                        Intersection e = new Intersection(); e.Deserialize(subParts[1]);
                        if (!Intersections.ContainsKey(c))
                            Intersections.Add(c, e);
                    }
                }
            }
        }

        /// <summary>
        /// Fonction auxiliaire de deserialisation de HexGrid
        /// </summary>
        /// <param name="serializedClass">Une classe serialisee</param>
        /// <param name="begin">Le debut de la sous-chaine de caracteres</param>
        /// <param name="end">La fin de la sous-chaine de caracteres</param>
        /// <returns></returns>
        public string GetSubString(string serializedClass, string begin, string end)
        {
            int pFrom = serializedClass.IndexOf(begin) + begin.Length;
            int pTo = serializedClass.LastIndexOf(end);
            string x = serializedClass.Substring(pFrom, pTo - pFrom);
            return x;
        }

        /// <summary>
        /// Fonction permettant de remplacer les donnees de HexGrid par ceux passes en parametre.
        /// </summary>
        /// <param name="localTerrainTiles">Les nouvelles cases</param>
        /// <param name="localEdges">Les nouvelles aretes</param>
        /// <param name="localIntersection">Les nouvelles intersections</param>
        private void Update(Dictionary<Coordinate, TerrainTile> localTerrainTiles, Dictionary<Coordinate, Edge> localEdges, Dictionary<Coordinate, Intersection> localIntersection)
        {
            TerrainTiles.Clear();
            foreach (KeyValuePair<Coordinate, TerrainTile> entry in localTerrainTiles)
            {
                TerrainTiles.Add(entry.Key, entry.Value);
            }
            Edges.Clear();
            foreach (KeyValuePair<Coordinate, Edge> entry in localEdges)
            {
                Edges.Add(entry.Key, entry.Value);
            }
            Intersections.Clear();
            foreach (KeyValuePair<Coordinate, Intersection> entry in localIntersection)
            {
                Intersections.Add(entry.Key, entry.Value);
            }
        }
    }

    public class StringHexGrid
    {
        public string TerrainTiles { get; set; }
        public string Edges { get; set; }
        public string Intersections { get; set; }
        public string CurrentThiefLocation { get; set; }

        public StringHexGrid() { }
        public StringHexGrid(string terrainTiles, string edges, string intersections, string currentThiefLocation)
        {
            TerrainTiles = terrainTiles;
            Edges = edges;
            Intersections = intersections;
            CurrentThiefLocation = currentThiefLocation;

        }
    }
}
