using System;
using System.Collections.Generic;
using System.Text;

namespace Noyau.View
{
    /// <summary>
    /// <para> L'interface IPlayer.</para>
    /// <para> Représente un joueur de la partie.</para>
    /// <para> Contient les méthodes et les propriétés nécessaires à l'implémentation d'un joueur.</para>
    /// </summary>
    public interface IPlayer
    {
        /// <value> L'identifiant du joueur dans la partie </value>
        int Id { get; set; }

        /// <value> Le nombre de points du joueur</value>
        bool IsIA { get; set; }

        /// <value> Le nombre de points du joueur</value>
        int VictoryPoints { get; set; }

        /// <value> Booléen indiquant si le joueur possède la route la plus longue </value>
        bool HasLongestRoad { get; set; }

        /// <value> Booléen indiquant si le joueur possède l'armée la plus puissante </value>
        bool HasGreatestArmy { get; set; }

        /// <value> Booléen indiquant si le joueur possède l'armée la plus puissante </value>
        int KnightCardsPlayed { get; }

        /// <value> Dictionnaire indiquant le nombre de ressources que le joueur possède pour chaque type</value>
        /// <seealso cref="IDictionary{TKey, TValue}"/>
        /// <seealso cref="View.RessourceType"/>
        IDictionary<RessourceType, int> Ressources { get; set; }

        /// <value> Le nombre total de ressources</value>
        int TotalRessourceNumber { get; set; }

        /// <value> Dictionnaire indiquant le nombre de cartes que le joueur possède pour chaque type</value>
        /// <seealso cref="IDictionary{TKey, TValue}"/>
        /// <seealso cref="View.CardType"/>
        IDictionary<CardType, int> Cards { get; set; }

        /// <value> List contenant les cartes achetés par le joueur durant son tour. Elle ne peuvent pas être jouées le tour de l'achat.</value>
        /// <seealso cref="IList{T}"/>
        /// <seealso cref="View.CardType"/>
        IList<CardType> CardsBoughtThisTurn { get; set; }

        /// <value> List contenant les constructions du joueur et leurs coordonnées.</value>
        /// <seealso cref="IList{T}"/>
        /// <seealso cref="View.CardType"/>
        IList<(Coordinate, ConstructionType)> Constructions { get; set; }



        /// <summary>
        /// Vérifie si le joueur peut construire la construction <paramref name="ct"/> 
        /// </summary>
        /// <param name="ct">L'aménagement que le joueur veut construire</param>
        /// <returns> true si la construction est possible, false sinon. </returns>
        /// <seealso cref="View.ConstructionType"/>
        bool HasEnoughRessources(ConstructionType ct);
        /// <summary>
        /// Ajoute un ou plusieurs exemplaires d'une ressource au joueur.
        /// Une seule ressource est ajoutee par defaut.
        /// </summary>
        /// <param name="r">Le type de ressources a ajouter</param>
        /// <param name="num">Le nombre d'exemplaires a ajouter</param>
        void AddRessource(RessourceType r, int num = 1);
        /// <summary>
        /// Supprime une ressource possedee par le joueur.
        /// Une seule ressource est supprimee par defaut.
        /// </summary>
        /// <param name="r">Le type de ressources a supprimer</param>
        /// <param name="num">Le nombre d'exemplaires a supprimer</param>
        void RemoveRessource(RessourceType r, int num = 1);

        /// <summary>
        /// Renvoie la longueur de la plus longue route du joueur.
        /// </summary>
        /// <returns>Entier correspondant a la plus longue route</returns>
        int LongestRoadLength();

        /// <summary>
        /// Fonction permettant de serialiser l'interface IPlayer.
        /// Cette fonction est necessaire pour le transfert d'informations par le reseau.
        /// </summary>
        /// <returns>Une chaine de caracteres correspondant a l'interface serialisee</returns>
        string Serialize();
        /// <summary>
        /// Fonction permettant de deserialiser l'interface IPlayer.
        /// Cette fonction est necessaire pour le transfert d'informations par le reseau.
        /// </summary>
        /// <param name="serializedClass">Une chaine de caracteres correspondant a l'interface serialisee</param>
        void Deserialize(string serializedClass);

    }
}

