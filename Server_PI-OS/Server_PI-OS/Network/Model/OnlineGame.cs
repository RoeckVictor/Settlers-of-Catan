using System.Collections.Generic;
using System;
using Noyau.View;
using Util.View;
using Network.Controller;

namespace Network.Model
{
    class OnlineGame
    {
        /// <summary>
        /// Liste de tous les joueurs de la partie
        /// </summary>
        public List<Player> Players { get; private set; }
        /// <summary>
        /// Nombre joueur total autorisé dans le jeu
        /// </summary>
        public int NbPlayers { get; private set; }
        /// <summary>
        /// Nombre d'IA à allouer pour compléter les joueurs
        /// </summary>
        public int NbIA { get; set; }
        /// <summary>
        /// Idnetifiant du jeu dans le Noyau
        /// </summary>
        public Guid InGame { get; private set; }
        /// <summary>
        /// Nom du groupe de jeu
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Accès public ou privé de la partie de jeu
        /// Vrai si la partie est publique et faux si elle est privée
        /// </summary>
        public bool Access { get; private set; }
        /// <summary>
        /// Une copie de la grid du jeu
        /// </summary>
        public IHexGrid Grid { get; private set; }
        /// <summary>
        /// Si la partie a commencé ou pas
        /// </summary>
        public bool hasStarted { get; set; }
        /// <summary>
        /// nombre de joueurs connectés à la partie
        /// </summary>
        public int nbConnected { get; set; }
        /// <summary>
        /// nombre de joueurs prêts à commencer la partie
        /// </summary>
        public int nbReady { get; set; }

        private int Count;


        /// <summary>
        /// Constructeur de la partie de jeu
        /// </summary>
        /// <param name="nbPlayers">Le nombre total de joueurs dans la partie</param>
        /// <param name="nbIA">Le nombre d'IA de la partie</param>
        /// <param name="name">Le nom de la partie</param>
        /// <param name="access">L'accessibilité de la partie: public ou privé</param>
        /// <param name="grid">La grille du jeu</param>
        /// <param name="start">Indique si la partie est lancée</param>
        /// <param name="nbconnect">Le nombre de joueurs connectés à la partie</param>
        /// <param name="nbready">Le nombre de joueurs TCP prêts dans la partie</param>
        public OnlineGame(int nbPlayers, int nbIA, string name, bool access, IHexGrid grid, bool start, int nbconnect, int nbready)
        {
            Count = 0;
            Players = new List<Player>();
            NbPlayers = nbPlayers;
            NbIA = nbIA;
            Name = name;
            Access = access;
            Grid = grid;
            hasStarted = start;
            nbConnected = nbconnect;
            nbReady = nbready;

        }
        public OnlineGame() { nbConnected = 0; nbReady = 0; NbIA = 0; NbPlayers = 0; }

        public void setId(Guid guid) { InGame = guid; }
        public void setNbIA(int ia) { NbIA = ia; }

        public void setPlayers(List<Player> players) { Players = players; }

        /// <summary>
        /// Renvoie la liste des logins(noms) et idetifiants des joueurs du jeu
        /// </summary>
        public List<(int, string)> listPlayers()
        {
            List<(int, string)> list = new List<(int, string)>();
            foreach (Player p in Players)
            {
                list.Add((p.IdInGame, p.Name));
            }
            return list;
        }

        /// <summary>
        /// Renvoie la liste des avatars et idetifiants des joueurs du jeu
        /// </summary>
        public List<(int, int)> listAvatar()
        {
            List<(int, int)> list = new List<(int, int)>();
            foreach (Player p in Players)
            {
                list.Add((p.IdInGame, p.Avatar));
            }
            return list;
        }

        /// <summary>
        /// Ajoute un joueur en partie
        /// </summary>
        /// <param name="c">Un joueur à ajouter dans la liste des joeuurs de cette partie</param>
        public bool AddPlayer(Player c)
        {
            if (Count < NbPlayers)
            {
                foreach (Player player in Players)
                {
                    if (player.Equals(c))
                        return false; // already exists
                }
                c.SetIdInGame(Players.Count);
                Players.Add(c);
                nbConnected += 1;
                Count++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Vérifie si la partie est pleine
        /// </summary>
        public bool IsFull()
        {
            return (nbConnected == NbPlayers - ClientManager.GetIAsInGame(InGame));
        }
    }
}
