using System;
using System.Collections.Generic;
using System.Text;

namespace Noyau.View
{
    /// <summary>
    /// <para>L'interface IGame.</para>
    /// <para>Elle contient les propriétés et les méthodes nécessaires au bon fonctionnement d'une partie.</para>
    /// </summary>
    public interface IGame
    {
        /// <value>L'identifiant de la partie, utile pour le jeu en réseau.</value>
        Guid Id { get; set; }

        /// <value>Liste des joueurs de la partie</value>
        /// <seealso cref="IList{T}"/>
        /// <seealso cref="View.IPlayer"/>
        IList<IPlayer> Players { get; set; }

        /// <value>Le terrain de jeu</value>
        /// <seealso cref="View.IHexGrid"/>
        IHexGrid GameGrid { get; set; }

        /// <value>L'identifiant du joueur jouant son tour</value>
        int CurrentPlayer { get; set; }

        /// <value>La phase du tour actuelle</value>
        /// <seealso cref="View.GamePhase"/>
        GamePhase CurrentPhase { get; set; }

        /// <value>Booléen indiquant si une carte a été joué ce tour.</value>
        bool CardUsedThisTurn { get; set; }

        /// <summary>
        /// Renvoie la liste des ports du joueur
        /// </summary>
        /// <returns>La liste des ports du joueur</returns>
        IList<(Coordinate, HarborType)> GetCurrentPlayerHarbors();

        /// <value>Couple d'entier représentant les valeurs des jets de dés.</value>
        (int, int) lastDice { get; set; }

        void Deserialize(string serializedClass);

        string Serialize();
    }
}
