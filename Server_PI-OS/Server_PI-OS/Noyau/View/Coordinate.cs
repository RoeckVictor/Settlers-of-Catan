using System;
using System.Collections.Generic;
using System.Text;
using Util.View;

namespace Noyau.View
{
    /// <summary>
    /// <para>La classe Coordinate.</para>
    /// <para>Elle représente les coordonnées d'une case hexagonale de la grille.</para>
    /// </summary>
    /// <remarks>La classe utilise l'interface IEquatable pour l'utilisation de equals.</remarks>
    public class Coordinate : IEquatable<Coordinate>
    {
        /// <value> La coordonnée X de la case.</value>
        public int X { get; set; }

        /// <value> La coordonnée Y de la case.</value>
        public int Y { get;  set; }

        /// <value> La coordonnée Z de la case.</value>
        public int Z { get; set; }

        /// <value> La Direction D de la case.</value>
        /// <seealso cref="View.Direction"/>
        public Direction D { get;  set; }

        /// <summary>
        /// Constructeur de coordinate.
        /// <param name="x">Un entier. La coordonnée x.</params>
        /// <param name="y">Un entier. La coordonnée y.</params>
        /// <param name="z">Un entier. La coordonnée z.</params>
        /// <param name="d">Un entier. La coordonnée d.</params>
        /// <seealso cref="View.Direction"/>
        /// </summary>
        public Coordinate(int x, int y, int z, Direction d)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.D = d;
        }

        public Coordinate() { }

        /// <summary>
        /// Méthode equals, compare les différents attributs de la coordonnée avec la coordonnée <paramref name="other"/>
        /// </summary>
        /// <param name="other">La coordonnée de comparaison</param>
        /// <returns>true si les attributs sont égaux, false sinon</returns>
        public bool Equals(Coordinate other)
        {
            return (other.X == this.X) && (other.Y == this.Y) && (other.Z == this.Z) && (other.D == this.D);
        }

        /// <summary>
        /// <para>Méthode equals entre une coordonnée et un objet <paramref name="obj"/>.</para>
        /// <para>La méthode vérifie si les deux objets sont du même type.</para>
        /// </summary>
        /// <param name="obj">L'objet de comparaison</param>
        /// <returns>true si les objets sont du même type et que les attributs sont égaux, false sinon</returns>
        public override bool Equals(object obj)
        {
            Coordinate other = obj as Coordinate;
            if (other != null)
            {
                return Equals(other);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Renvoie le HashCode de l'objet, utile pour les comparaisons.
        /// </summary>
        /// <returns>Le HashCode</returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ D.GetHashCode();
        }

        /// <summary>
        /// Méthode ToString.
        /// </summary>
        /// <returns>Une chaine du type [X:Y:Z:D]</returns>
        public override string ToString()
        {
            return X + ":" + Y + ":" + Z + ":" + D;
        }

        public string Serialize()
        {
            return X.ToString() + "+" + Y.ToString() + "+" + Z.ToString() + "+" + D.ToString();
        }

        public void Deserialize(string str)
        {
            char[] seps = { '+' };
            string[] parts = str.Split(seps);
            X = Int32.Parse(parts[0]);
            Y = Int32.Parse(parts[1]);
            Z = Int32.Parse(parts[2]);
            D = Callbacks.GetDirection((parts[3]));                
        }
    }

    /// <summary>
    /// <para>L'énumération Direction</para>
    /// <para>Elle sert à indiquer dans quel direction de la case se trouve un objet.</para>
    /// <list type="bullet">
    /// <item>
    /// <term>NONE</term>
    /// <description>Coordonnée NONE, utilisé pour les cases.</description>
    /// </item>
    /// <item>
    /// <term>UP</term>
    /// <description>Coordonnée UP, utilisé par les intersections. Montre que l'intersection est celle du haut de la case</description>
    /// </item>
    /// <item>
    /// <term>DOWN</term>
    /// <description>Coordonnée DOWN, utilisé par les intersections. Montre que l'intersection est celle du bas de la case</description>
    /// </item>
    /// <item>
    /// <term>NORTH_EAST</term>
    /// <description>Coordonnée NORTH_EAST, utilisé par les arêtes. Montre que l'arête est celle au nord-est de la case</description>
    /// </item>
    /// <item>
    /// <term>EAST</term>
    /// <description>Coordonnée EAST, utilisé par les arêtes. Montre que l'arête est celle à l'est de la case</description>
    /// </item>
    /// <item>
    /// <term>SOUTH_EAST</term>
    /// <description>Coordonnée SOUTH_EAST, utilisé par les arêtes. Montre que l'arête est celle au sud-est de la case</description>
    /// </item>
    /// </list>
    /// </summary>
    public enum Direction
    {
        NONE,
        UP,
        DOWN,
        NORTH_EAST,
        EAST,
        SOUTH_EAST
    }
}

