using System;
using System.Collections.Generic;
using Network.Model;
using Util.View;

namespace Network.Controller
{
    class GameManager
    {
        /// <summary>
        /// <para>La classe GameManager</para>
        /// <para>Contient l'ensemble des méthodes servant à manipuler toutes les instances de jeu crées</para>
        /// </summary>
        public Dictionary<Guid, OnlineGame> Games { get; set; }

        public GameManager()
        {
            Games = new Dictionary<Guid, OnlineGame>();
        }

        /// <summary>
        /// Fonction appelée pour supprimer une partie dans le dictionnaire
        /// </summary>
        /// <param name="id">Identifiant du jeu à supprimer</param>
        public bool removeGame(Guid id)
        {
            try
            {
                if (Games.ContainsKey(id))
                    Games.Remove(id);
                return true;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        /// <summary>
        /// Fonction qui renvoie la liste des joeuurs d'une partie dont on connaît l'identifiant
        /// </summary>
        /// <param name="id">Identifiant de la partie de jeu</param>
        public List<Player> getPlayers(Guid id)
        {
            try
            {
                return Games[id].Players;
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        /// Fonction qui renvoie l'instance de jeu dont on connaît l'identifiant
        /// </summary>
        /// <param name="id">Identifiant de la partie de jeu</param>
        public OnlineGame getGame(Guid id)
        {
            try
            {
                return Games[id];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        /// Fonction qui permet d'ajouter une isntance de partie dans le dictionnaire
        /// </summary>
        /// <param name="game">Jeu à rajouter dans la liste</param>
        public Guid addGame(OnlineGame game)
        {
            try
            {
                Guid id = GenerateId();
                game.setId(id);
                Games.Add(id, game);
                Console.WriteLine("Game added with id : " + id);
                Console.WriteLine(Games.Count);
                return id;
            }
            catch (ArgumentException)
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Fonction qui vérifie si une partie de jeu est pleine
        /// </summary>
        /// <param name="idGame">Identifiant de la partie de jeu</param>
        public bool isFull(Guid idGame)
        {
            try
            {
                return Games[idGame].IsFull();
            }
            catch (ArgumentException)
            {
                return false;
            }

        }

        /// <summary>
        /// Ajoute un joueur dans une partie de jeu
        /// </summary>
        /// <param name="c">IJoueur à ajouter/param>
        /// <param name="id">Identifiant de la partie de jeu</param>
        public bool addPlayer(Player c, Guid id)
        {
            try
            {
                if (Games[id].AddPlayer(c))
                {
                    c.SetIdGame(id);
                    return true;
                }
                return false;

            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// Renvoie un identifiant de jeu aléatoire qui n'est pas complet
        /// </summary>
        public Guid RandomGame()
        {
            foreach (KeyValuePair<Guid, OnlineGame> kvp in Games)
            {
                if (!kvp.Value.IsFull()) return kvp.Key;
            }
            return Guid.Empty;
        }


        public Guid GenerateId()
        {
            var guid = Guid.NewGuid(); ;
            bool exist = false;
            while (!exist)
            {
                if (!Games.ContainsKey(guid))
                {
                    exist = true;
                    return guid;
                }
                guid = Guid.NewGuid();
            }
            return guid;
        }
    }
}
