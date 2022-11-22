namespace Noyau.View
{
    /// <summary>
    /// <para>L'interface IIntersection</para>
    /// <para>Représente l'intersection entre des cases</para>
    /// <para>Contient les méthodes et les propriétés nécessaires aux arêtes du terrain</para>
    /// </summary>
    public interface IIntersection
    {
        /// <value> Le type de construction de l'intersection NONE par défaut, SETTLEMENT ou CITY sinon </value>
        /// <seealso cref="View.ConstructionType"/>
        ConstructionType Building { get; set; }

        /// <value> PlayerId de l'intersection, 0 par défaut, id du joueur ayant construit un aménagement sinon.</value>
        int PlayerId { get; set; }

        /// <value> Le type de port de l'intersection, NONE pour toutes les intersections, sauf celle étant dans des emplacements de ports.</value>
        /// <seealso cref="View.HarborType"/>
        HarborType Harbor { get; set; }

        /// <summary>
        /// Rajoute une construction <paramref name="b"/> crée par le joueur d'id <paramref name="playerId"/> à l'intersection.
        /// </summary>
        /// <param name="b">Un ConstructionType, ici elle sera SETTLEMENT ou CITY</param>
        /// <param name="playerId">L'identifiant joueur de l'arête</param>
        /// <seealso cref="View.ConstructionType"/>
        /// <seealso cref="View.HarborType"/>
        void AddBuilding(ConstructionType b, int playerId);

        /// <summary>
        /// Renvoie une copie de IIntersection
        /// </summary>
        /// <returns>Une nouvelle instance de IIntersection initialisée avec les champs de l'objet</returns>
        IIntersection CreateCopy();
        /// <summary>
        /// Fonction permettant de serialiser l'interface IIntersection.
        /// Cette fonction est necessaire pour le transfert d'informations par le reseau.
        /// </summary>
        /// <returns>Une chaine de caracteres correspondant a l'interface serialisee</returns>
        string Serialize();
        /// <summary>
        /// Fonction permettant de deserialiser l'interface IIntersection.
        /// Cette fonction est necessaire pour le transfert d'informations par le reseau.
        /// </summary>
        /// <param name="serializedClass">Une chaine de caracteres correspondant a l'interface serialisee</param>
        void Deserialize(string str);
    }
}

