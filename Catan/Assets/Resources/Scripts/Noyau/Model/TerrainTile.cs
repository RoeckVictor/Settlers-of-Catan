using System;
using System.Collections.Generic;
using System.Text;
using Noyau.View;
using Util.View;

namespace Noyau.Model
{
    /// <summary>
    /// <para> La classe TerrainTile.</para>
    /// <para> Représente une case du terrain.</para>
    /// <para> Contient les méthodes et les propriétés nécessaires aux cases du terrain.</para>
    /// </summary>
    public class TerrainTile : ITerrainTile
    {
        /// <inheritdoc/>
        public TerrainType Type { get; set; }

        /// <inheritdoc/>
        public int DiceProductionNumber { get; set; }

        /// <inheritdoc/>
        public bool ThiefIsPresent { get; set; }

        /// <value> Booléen vérouillant la case une fois que la partie a été initialisé correctement</value>
        private bool tileLocked;

        /// <inheritdoc/>
        public void ChangeTerrainType(TerrainType newType)
        {
            if (!tileLocked)
                this.Type = newType;
        }

        /// <inheritdoc/>
        public void ChangeTerrainNumber(int newNumber)
        {
            if (!tileLocked)
                this.DiceProductionNumber = newNumber;
        }

        /// <summary>
        /// Méthode vérouillant la case.
        /// </summary>
        public void LockTile()
        { tileLocked = true; }

        /// <summary>
        /// Constructeur de TerrainTiles
        /// </summary>
        /// <param name="type"> Le type de ressource de la case</param>
        /// <param name="diceProdNum"> Le numéro de la case</param>
        /// <param name="thiefPresent"> Le booléen indiquant si les brigands sont présents</param>
        public TerrainTile(TerrainType type, int diceProdNum, bool thiefPresent = false)
        {
            this.Type = type;
            this.DiceProductionNumber = diceProdNum;
            this.ThiefIsPresent = thiefPresent;
            this.tileLocked = false;
        }

        /// <summary>
        /// Constructeur par defaut de TerrainTile
        /// </summary>
        public TerrainTile() { }

        /// <summary>
        /// Fonction permettant de serialiser la classe TerrainTile.
        /// Cette fonction est necessaire pour le transfert d'informations par le reseau.
        /// </summary>
        /// <returns>Une chaine de caracteres correspondant a la classe serialisee</returns>
        public string Serialize()
        {
            return Type.ToString() + "+" + DiceProductionNumber.ToString() + "+" + ThiefIsPresent.ToString() + "+" + tileLocked.ToString();
        }

        /// <summary>
        /// Fonction permettant de deserialiser la classe TerrainTile.
        /// Cette fonction est necessaire pour le transfert d'informations par le reseau.
        /// </summary>
        /// <param name="serializedClass">Une chaine de caracteres correspondant a la classe serialisee</param>
        public void Deserialize(string str)
        {
            char[] seps = { '+' };
            string[] parts = str.Split(seps);
            Type = Callbacks.GetTerrainType((parts[0]));
            DiceProductionNumber = Int32.Parse(parts[1]);
            ThiefIsPresent = bool.Parse(parts[2]);
            tileLocked = bool.Parse(parts[3]);
        }
    }
}
