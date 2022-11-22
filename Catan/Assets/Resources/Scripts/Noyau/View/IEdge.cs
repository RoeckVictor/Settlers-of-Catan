using System;
using System.Collections.Generic;
using System.Text;

namespace Noyau.View
{
    /// <summary>
    /// <para>L'interface IEdge</para>
    /// <para>Représente les arètes entre deux cases.</para>
    /// <para>Contient les méthodes et les propriétés nécessaires aux arêtes du terrain</para>
    /// </summary>
    public interface IEdge
    {
        /// <value> Le type de construction de l'arète NONE par défaut, ROAD si un joueur a construit une route </value>
        /// <seealso cref="View.ConstructionType"/>
        ConstructionType Building { get; set; }

        /// <value> PlayerId de l'arète, 0 par défaut, id du joueur ayant construit une route sinon</value>
        int PlayerId { get; set; }

        /// <summary>
        /// Rajoute une route <paramref name="b"/> crée par le joueur d'id <paramref name="playerId"/> à l'arète.
        /// </summary>
        /// <param name="b">Un ConstructionType, ici elle sera ROAD</param>
        /// <param name="playerId">L'identifiant joueur de l'arête</param>
        /// <seealso cref="View.ConstructionType"/>
        void AddBuilding(ConstructionType b, int playerId);

        /// <summary>
        /// Renvoie une copie de IEdge
        /// </summary>
        /// <returns>Une nouvelle instance de IEdge initialisée avec les champs de l'objet</returns>
        IEdge CreateCopy();

        /// <summary>
        /// Fonction permettant de serialiser l'interface IEdge.
        /// Cette fonction est necessaire pour le transfert d'informations par le reseau.
        /// </summary>
        /// <returns>Une chaine de caracteres correspondant a l'interface serialisee</returns>
        string Serialize();

        /// <summary>
        /// Fonction permettant de deserialiser l'interface IEdge.
        /// Cette fonction est necessaire pour le transfert d'informations par le reseau.
        /// </summary>
        /// <param name="serializedClass">Une chaine de caracteres correspondant a l'interface serialisee</param>
        void Deserialize(string str);
    }
}
