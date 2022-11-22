using System;
using System.Collections.Generic;
using System.Text;

namespace Noyau.View
{
    /// <summary>
    /// <para>L'interface IHexGrid</para>
    /// <para>Représente le terrain de jeu.</para>
    /// <para>Contient les méthodes et le propriétés du terrain.</para>
    /// <para>Utilise les intersections, les arêtes, les cases et les coordonnées </para>
    /// </summary>
    /// <seealso cref="View.Coordinate"/>
    /// <seealso cref="View.IIntersection"/>
    /// <seealso cref="View.IEdge"/>
    /// <seealso cref="View.ITerrainTile"/>
    public interface IHexGrid
    {

        /// <value> Dictionnaire contenant l'ensemble des cases</value>
        /// <seealso cref="IDictionary{TKey, TValue}"/>
        /// <seealso cref="View.Coordinate"/>
        /// <seealso cref="View.ITerrainTile"/>
        IDictionary<Coordinate, ITerrainTile> TerrainTiles { get; }

        /// <value> Dictionnaire contenant l'ensemble des arêtes</value>
        /// /// <seealso cref="IDictionary{TKey, TValue}"/>
        /// <seealso cref="View.Coordinate"/>
        /// <seealso cref="View.IEdge"/>
        IDictionary<Coordinate, IEdge> Edges { get; }

        /// <value> Dictionnaire contenant l'ensemble des intersections</value>
        /// <seealso cref="IDictionary{TKey, TValue}"/>
        /// <seealso cref="View.Coordinate"/>
        /// <seealso cref="View.IIntersection"/>
        IDictionary<Coordinate, IIntersection> Intersections { get; }

        /// <value>Les coordonnées de la case où se trouvent les brigands</value>
        Coordinate CurrentThiefLocation { get; }

        /// <summary>
        /// Méthode renvoyant une liste des coordonnées où la construction de route est possible pour le joueur <paramref name="p"/>
        /// </summary>
        /// <param name="p">Le joueur p</param>
        /// <returns>La liste des coordonnées de construction</returns>
        /// <seealso cref="List{T}"/>
        /// <seealso cref="View.Coordinate"/>
        List<Coordinate> PossibleRoads(IPlayer p);

        /// <summary>
        /// Méthode renvoyant une liste des coordonnées où la construction de colonie est possible pour le joueur <paramref name="p"/>
        /// </summary>
        /// <param name="p">Le joueur p</param>
        /// <returns>La liste des coordonnées de construction</returns>
        /// <seealso cref="List{T}"/>
        /// <seealso cref="View.Coordinate"/>
        List<Coordinate> PossibleColonies(IPlayer p);

        /// <summary>
        /// Méthode renvoyant une liste des coordonnées où la construction de ville est possible pour le joueur <paramref name="p"/>
        /// </summary>
        /// <param name="p">Le joueur p</param>
        /// <returns>La liste des coordonnées de construction</returns>
        /// <seealso cref="List{T}"/>
        /// <seealso cref="View.Coordinate"/>
        List<Coordinate> PossibleCities(IPlayer p);


        /// <summary>
        /// Renvoie la liste des intersections autour d'une case
        /// </summary>
        /// <param name="t">La coordonnée de la case</param>
        /// <returns>la liste des intersections autour de la case <paramref name="t"/></returns>
        /// <exception cref="Exception">Jetée lorsque la coordonnée n'est pas valide</exception>
        List<IIntersection> GetIntersectionsFromTile(Coordinate t);

        /// <summary>
        /// <para>Deplace les brigands sur le plateau de jeu</para>
        /// <para>Change la coordonnee dans Hexgrid + les booleens dans les TerrainTiles correspondants</para>
        /// </summary>
        /// <param name="tileLocation">La nouvelle position des brigands</param>
        /// <exception cref="Exception">Jetée lorsque la coordonnée n'est pas valide</exception>
        void MoveThief(Coordinate tileLocation);
        /// <summary>
        /// Fonction permettant de serialiser l'interface IHexGrid.
        /// Cette fonction est necessaire pour le transfert d'informations par le reseau.
        /// </summary>
        /// <returns>Une chaine de caracteres correspondant a l'interface serialisee</returns>
        string Serialize();
        /// <summary>
        /// Fonction permettant de deserialiser l'interface IHexGrid.
        /// Cette fonction est necessaire pour le transfert d'informations par le reseau.
        /// </summary>
        /// <param name="serializedClass">Une chaine de caracteres correspondant a l'interface serialisee</param>
        void Deserialize(string serializedData);
    }
}
