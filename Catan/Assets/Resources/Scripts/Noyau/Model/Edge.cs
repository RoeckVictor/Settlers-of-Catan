using System;
using System.Collections.Generic;
using System.Text;
using Noyau.View;
using Util.View;

namespace Noyau.Model
{

    /// <summary>
    /// <para>La classe Edge</para>
    /// <para>Représente les arètes entre deux cases.</para>
    /// <para>Contient les méthodes et les propriétés nécessaires aux arêtes du terrain</para>
    /// </summary>
    [Serializable]
    public class Edge : IEdge
    {
        /// <inheritdoc/>
        public ConstructionType Building { get; set; }

        /// <inheritdoc/>
        public int PlayerId { get; set; }

        /// <summary>
        /// Constructeur d'Edge.
        /// </summary>
        /// <param name="b">Le type de construction de la route</param>
        /// <param name="id">L'id joueur de la route</param>
        public Edge(ConstructionType b = ConstructionType.NONE, int id = 0)
        {
            this.Building = b;
            this.PlayerId = id;
        }

        public Edge() { }
        /// <inheritdoc/>
        public void AddBuilding(ConstructionType b, int playerId)
        {
            this.Building = b;
            this.PlayerId = playerId;
        }

        /// <inheritdoc/>
        public IEdge CreateCopy()
        {
            return new Edge(Building, PlayerId);
        }

        /// <summary>
        /// Fonction permettant de serialiser la classe Edge.
        /// Cette fonction est necessaire pour le transfert d'informations par le reseau.
        /// </summary>
        /// <returns>Une chaine de caracteres correspondant a la classe serialisee</returns>
        public string Serialize()
        {
            return Building.ToString() + "+" + PlayerId.ToString();
        }

        /// <summary>
        /// Fonction permettant de deserialiser la classe Edge.
        /// Cette fonction est necessaire pour le transfert d'informations par le reseau.
        /// </summary>
        /// <param name="serializedClass">Une chaine de caracteres correspondant a la classe serialisee</param>
        public void Deserialize(string str)
        {
            char[] seps = { '+' };
            string[] parts = str.Split(seps);
            Building = Callbacks.GetConstructionype((parts[0]));
            PlayerId = Int32.Parse(parts[1]);
        }
    }
}
