using System;
using System.Net.Sockets;
using System.Text;
using Util.View;

namespace Network.Model
{
    public class Client : Player
    {
        /// <summary>
        /// <para>La classe Client qui représente un joueur TCP</para>
        /// <para>Contient l'ensemble des méthodes pour la réception et l'envoie de messages entre le serveur et le client connecté</para>
        /// </summary>


        /// <summary>
        /// La socket qui permet de se connecter au serveur
        /// </summary>
        public TcpClient Tcp { get; private set; } = new TcpClient();

        /// <summary>
        /// Indique si le client est enregistré en BDD avec un compte personnalisé
        /// </summary>
        public bool IsRegistred { get; private set; }

        /// <summary>
        /// Flux maximum à envoyer 
        /// </summary>
        const int MAX = 1000000;

        public Client() { }
        public Client(TcpClient t) { Tcp = t; IsReady = false; }

        public Client(int idDB, Guid idGame, int idInGame, ClientType type, string name, TcpClient tcp, int avatar, bool ready)
            : base(idDB, idGame, idInGame, type, name, avatar, ready) { Tcp = tcp; }

        /// <summary>
        /// Fonction qui permet la réception d'un message depuis le client
        /// </summary>
        public string Receive()
        {
            var stream = Tcp.GetStream();
            Byte[] bytes = new Byte[MAX];
            string data;
            int i = 0;
            try
            {
                i = stream.Read(bytes, 0, bytes.Length);
                data = Encoding.ASCII.GetString(bytes, 0, i);
                return data;
            }
            catch (Exception)
            {
                Console.WriteLine("Client disconnected");
                Tcp.Close();
                return "";
            }
        }
        /// <summary>
        /// Fonction qui permet d'envoyer un message à un client depuis le serveur
        /// </summary>
        /// <param name="data">Représente le message sous forme sérialisée</param>
        public void Send(string data)
        {
            try
            {
                Byte[] reply = System.Text.Encoding.ASCII.GetBytes(data);
                Tcp.GetStream().Write(reply, 0, reply.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
                Tcp.Close();
            }
        }

        /// <summary>
        /// Redéfinition de la fonction Equals pour l'interface Comparable
        /// </summary>
        /// <param name="other">instance de la classe client à comparer avec celle déjà instanciée</param>
        public bool Equals(Client other)
        {
            return (this.Tcp == other.Tcp);
        }


        /// <summary>
        /// Redéfinition de la fonction Equals pour l'interface Comparable
        /// </summary>
        /// <param name="obj">l'objet à comparer</param>
        public override bool Equals(object obj)
        {
            Client other = obj as Client;
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
        /// Le hash obtenu pour comparer les clients
        /// </summary>
        public override int GetHashCode()
        {
            return Tcp.GetHashCode();
        }
    }
}
