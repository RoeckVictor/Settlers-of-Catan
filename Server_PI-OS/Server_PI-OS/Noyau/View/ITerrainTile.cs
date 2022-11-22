using System;
using System.Collections.Generic;
using System.Text;

namespace Noyau.View
{
    /// <summary>
    /// <para> L'interface ITerrainTile.</para>
    /// <para> Représente une case du terrain.</para>
    /// <para> Contient les méthodes et les propriétés nécessaires aux cases du terrain.</para>
    /// </summary>
    public interface ITerrainTile
    {
        /// <value> Le type de ressource sur la case.</value>
        /// <seealso cref="View.TerrainType"/>
        TerrainType Type { get; set; }

        /// <value> Le numéro de la case</value>
        int DiceProductionNumber { get; set; }

        /// <value> Booléen indiquant si les brigands sont sur la case</value>
        bool ThiefIsPresent { get; set; }

        /// <summary>
        /// Appelé uniquement lors de l'initialisation de la partie, permet d'assigner un TerrainType <paramref name="newType"/> à la case
        /// </summary>
        /// <param name="newType">Le nouveau TerrainType</param>
        /// <seealso cref="View.TerrainType"/>
        void ChangeTerrainType(TerrainType newType);

        /// <summary>
        /// Appelé uniquement lors de l'initialisation de la partie, permet d'assigner un numéro à la case
        /// </summary>
        /// <param name="newNumber">Le nouveau TerrainType</param>
        void ChangeTerrainNumber(int newNumber);

        string Serialize();

        void Deserialize(string str);

    }
}

