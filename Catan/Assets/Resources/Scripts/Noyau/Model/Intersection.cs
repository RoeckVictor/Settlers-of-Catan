using System;
using System.Collections.Generic;
using System.Text;
using Noyau.View;
using Util.View;

namespace Noyau.Model
{
    /// <summary>
    /// <para>La classe Intersection</para>
    /// <para>Représente l'intersection entre </para>
    /// <para>Contient les méthodes et les propriétés nécessaires aux arêtes du terrain</para>
    /// </summary>
    public class Intersection : IIntersection
    {
        /// <inheritdoc/>
        public ConstructionType Building { get; set; }

        /// <inheritdoc/>
        public int PlayerId { get; set; }

        /// <inheritdoc/>
        public HarborType Harbor { get; set; }

        /// <summary>
        /// Constructeur d'Intersection.
        /// </summary>
        /// <param name="b">Le type de construction de l'intersection</param>
        /// <param name="id">L'identifiant joueur de l'intersection</param>
        /// <param name="h">Le type de port de l'intersection</param>
        /// <seealso cref="View.ConstructionType"/>
        /// <seealso cref="View.HarborType"/>
        public Intersection(ConstructionType b = ConstructionType.NONE, int id = 0, HarborType h = HarborType.NONE)
        {
            Building = b;
            PlayerId = id;
            Harbor = h;
        }

        /// <summary>
        /// Constructeur par defaut de Intersection.
        /// </summary>
        public Intersection() { }

        /// <inheritdoc/>
        public void AddBuilding(ConstructionType b, int playerId)
        {
            this.Building = b;
            this.PlayerId = playerId;
        }

        /// <inheritdoc/>
        public IIntersection CreateCopy()
        {
            return new Intersection(Building, PlayerId, Harbor);
        }

        /// <summary>
        /// Fonction permettant de serialiser la classe Intersection.
        /// Cette fonction est necessaire pour le transfert d'informations par le reseau.
        /// </summary>
        /// <returns>Une chaine de caracteres correspondant a la classe serialisee</returns>
        public string Serialize()
        {
            return Building.ToString() + "+" + PlayerId.ToString() + "+" + Harbor.ToString();
        }

        /// <summary>
        /// Fonction permettant de deserialiser la classe Intersection.
        /// Cette fonction est necessaire pour le transfert d'informations par le reseau.
        /// </summary>
        /// <param name="serializedClass">Une chaine de caracteres correspondant a la classe serialisee</param>
        public void Deserialize(string str)
        {
            char[] seps = { '+' };
            string[] parts = str.Split(seps);
            Building = Callbacks.GetConstructionype((parts[0]));
            PlayerId = Int32.Parse(parts[1]);
            Harbor = Callbacks.GetHarborType(parts[2]);
        }
    }
}
