using System;
using Util.View;

namespace Network.Model
{
    public class Player: IEquatable<Player>
    {
        /// <summary>
        /// id dans la base de données
        /// </summary>
        public int IdDB { get; private set; }
        /// <summary>
        /// id du game auquel il appartient
        /// </summary>
        public Guid IdGame { get; private set; }
        /// <summary>
        /// id dans le jeu en cours
        /// </summary>
        public int IdInGame { get;  private set; }
        /// <summary>
        /// 0: IA
        /// 1: Joueur enregistré en BDD
        /// 2: Joueur fantome
        /// </summary>
        public ClientType Type { get; set; }
        /// <summary>
        /// Nom / pseudonyme du joueur
        /// </summary>
        public string Name { get; private set; } = "";

        public int Avatar { get; private set; }

        public bool IsReady { get; protected set; }




        public Player() { Name = ""; Type = ClientType.AnonymousPlayer; Avatar = 0; IsReady = false; }
        public Player(int idDB, Guid idGame, int idInGame, ClientType type, string name,int avatar,bool ready )
        {
            IdDB = idDB;
            IdGame = idGame;
            IdInGame = idInGame;
            Type = type;
            Name = name;
            Avatar = avatar;
            IsReady = ready;
        }

        public void SetName(string name) { Name = name; }
        public void SetIDDB(int id) { IdDB =id; }
        public void SetAvatar(int avatar) { Avatar = avatar; }
        public void SetIdInGame(int id) { IdInGame = id; }
        public void SetIdGame(Guid id) { IdGame = id; }
        public void SetReady(bool ready) { IsReady = ready; }
        

        public bool Equals(Player other)
        {
            return (this.IdDB == other.IdDB) && (this.IdGame==other.IdGame) 
                && (this.IdInGame==other.IdInGame) && other.Name.Equals(this.Name);
        }

        public override bool Equals(object obj)
        {
            Player other = obj as Player;
            if (other != null)
            {
                return Equals(other);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return IdDB.GetHashCode() ^ IdGame.GetHashCode() ^  IdInGame.GetHashCode() ^  Type.GetHashCode() ^ Name.GetHashCode();
        }


    }
}
