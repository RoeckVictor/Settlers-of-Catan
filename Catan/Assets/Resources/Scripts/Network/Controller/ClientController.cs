using Assets.Src.Network.Model;
using Util.View;
using System.Collections.Generic;
using System;
using Noyau.View;
using Assets.Resources.Scripts.Graphique;



namespace Network.Controller
{
    class ClientController
    {
        /// <summary>
        /// <para>classe CleintController</para>
        /// </summary>

        /// <summary>
        /// Instance de la classe client qui permet de se connecter au serveur
        /// </summary>
        public Client Client;
        /// <summary>
        /// Adresse IP du serveur host
        /// </summary>
        private string Host = "85.171.157.6"; //"127.0.0.1";
         /// <summary>
        /// Port de connexion au serveur
        /// </summary>
        private int Port = 6321;
        /// <summary>
        /// Informations du jeu en cours
        /// </summary>
        public GameLine infoGame { get; private set; }
        /// <summary>
        /// Les logins des joueurs connectés à la même partie que le client actuel et leur identifiant dans le jeu
        /// </summary>
        public List<(int, string)> infoPlayers { get; private set; }
        /// <summary>
        /// Les avatars des joueurs connectés à la même partie que le client actuel et leur identifiant dans le jeu
        /// </summary>
        public List<(int, int)> infoAvatar { get; private set; }
        /// <summary>
        /// Liste des jeux en ligne disponible obtenue lors de la connexion au serveur
        /// </summary>
        public List<GameLine> listJeux { get; set; }
        /// <summary>
        /// Indique si c'est la première fois qu'il reçoit l'étt de la partie ou pas, pour éviter de modifier son identifiant dans le jeu
        /// </summary>
        public bool First { get; set; }
        /// <summary>
        /// Indique si les événements on été enregistrés
        /// </summary>
        private bool eventHandlersRegistered = false;

        /// <summary>
        /// Constructeur de la classe
        /// </summary>
        public ClientController()
        {
            Client = new Client();
            listJeux = new List<GameLine>();
            infoPlayers = new List<(int, string)>();
            infoGame = new GameLine();

        }

        /// <summary>
        /// Identifiant du jeu en cours
        /// </summary>
        public Guid getIdGame()
        {
            return Client.Id;
        }
        /// <summary>
        /// Identifiant du joueur dans la partie
        /// </summary>
        public int getIdInGame()
        {
            return Client.IdInGame;
        }
        /// <summary>
        /// Permet d'obtenir le jeu auquel le client est connecté
        /// </summary>
        public GameLine getActualGame()
        {
            return Client.infoGame;
        }
        /// <summary>
        /// Les informations (logins) des joueurs de la partie auquelle appartient le client
        /// </summary>
        public List<(int, string)> getInfoPlayers()
        {
            return Client.infoPlayers;
        }
        /// <summary>
        /// Indique si c'est la première reception de l'état du jeu depuis le serveur
        /// </summary>
        public bool GetFirst()
        {
            return Client.First; 
        }
        /// <summary>
        /// Les informations (avatars) des joueurs de la partie auquelle appartient le client
        /// </summary>
        public List<(int, int)> getAvatar()
        {
            return Client.infoAvatar;
        }

        /// <summary>
        /// Obtient la liste des jeux en ligne
        /// </summary>
        public List<GameLine> getGames()
        {
            return listJeux;
        }

        /// <summary>
        /// Capture tous les événements à recevoir deuis l'UI
        /// </summary>
        internal void RegisterEventHandlers()
        {
            if (eventHandlersRegistered)
                return;

            View.NetworkView.Instance.ConnectEvent += ConnectEventHandler;
            View.NetworkView.Instance.DisconnectEvent += DisconnectEventHandler;
            View.NetworkView.Instance.JoinEvent += JoinEventHandler;
            View.NetworkView.Instance.Message += MessageHandler;
            View.NetworkView.Instance.Ready += ReadyHandler;
            View.NetworkView.Instance.Start += StartHandler;


            View.NetworkView.Instance.GameCreate += GameCreateHandler;
            View.NetworkView.Instance.GameDelete += GameDeleteHandler;
            View.NetworkView.Instance.EndPhase += EndPhaseHandler;
            View.NetworkView.Instance.Construct += ConstructHandler;
            View.NetworkView.Instance.PlayerExchange += PlayerExchangeHandler;
            View.NetworkView.Instance.PlayerAcceptExchange += PlayerAcceptExchangeHandler;
            View.NetworkView.Instance.HarborExchange += HarborExchangeHandler;
            View.NetworkView.Instance.KnightCardUse += KnightCardUseHandler;
            View.NetworkView.Instance.BanditMove += BanditMoveHandler;
            View.NetworkView.Instance.MonopolyCardUse += MonopolyCardUseHandler;
            View.NetworkView.Instance.ResourcePairCardUse += ResourcePairCardUseHandler;
            View.NetworkView.Instance.RoadConstructionCardUse += RoadConstructionCardUseHandler;
            View.NetworkView.Instance.DiceThrow += DiceThrowHandler;
            View.NetworkView.Instance.DiscardExtraRessources += DiscardExtraRessourcesHandler;
            View.NetworkView.Instance.InitialRoad += InitialRoadHandler;
            View.NetworkView.Instance.InitialColony += InitialColonyHandler;
            View.NetworkView.Instance.QuitLobby += QuitLobbyHandler;
            View.NetworkView.Instance.Refresh += RefreshHandler;

            eventHandlersRegistered = true;
        }

        /// <summary>
        /// Rediffusion de l'événements au serveur
        /// </summary>
        /// <param name="sender">celui qui envoie l'event</param>
        /// <param name="e">les arguments de l'évenement</param>
        public void CatchEventHandler(object sender, Util.View.Event e)
        {
            if (e.IsOnline)
            {
                string serialized = Serialization.XMLSerialize(e);

                Client.Send(serialized);
            }
        }

        /// <summary>
        /// Retransmet l'évenement capturé au serveur
        /// </summary>
        /// <param name="e"></param>
        private void SendToServer(Event e)
        {
            string serialized = Serialization.XMLSerialize(e);
            Client.Send(serialized);
        }

        /// <summary>
        /// Capteur de l'évent de connexion, il permet de mettre à jour les logins et avatrs des clients déjà connectés à cette partie de jeu
        /// </summary>
        /// <param name="sender">celui qui envoie l'event</param>
        /// <param name="e">les arguments de l'évenement</param>
        public void ConnectEventHandler(object sender, ConnectEventArgs e)
        {
            try
            {
                Client.Connect(Host, Port);
                if (Client.isConnected)
                {
                    listJeux = Client.listJeux;
                    infoGame = Client.infoGame;
                    infoPlayers = Client.infoPlayers;
                    infoAvatar = Client.infoAvatar;
                }
            }
            catch(Exception)
            {
                UI.Instance.OnTimeout(new TimeOut());
                //pas d'inquietude c'est normal
                int[] tab = new int[2];
                int a = tab[3];
            }
        }

        public void DisconnectEventHandler(object sender, DisconnectEventArgs e)
        {
            string serialized = Serialization.XMLSerialize(e);
            Client.Send(serialized);
            Client.Disconnect();
        }


        public void RefreshHandler(object sender, Refresh e)
        {
            string serialized = Serialization.XMLSerialize(e);
            Client.Send(serialized);
        }

        public void QuitLobbyHandler(object sender, QuitLobby e)
        {
            string serialized = Serialization.XMLSerialize(e);
            Client.Send(serialized);
        }

        public void ReadyHandler(object sender, ReadyEvent e)
        {
            if (e.IsOnline)
            {
                SendToServer(e);
            }
        }

        public void StartHandler(object sender, StartedGame e)
        {
            if (e.IsOnline)
            {
                SendToServer(e);
            }
        }

        public void JoinEventHandler(object sender, GameJoinEventArgs e)
        {
            if (e.IsOnline)
            {
                SendToServer(e);
            }
        }

        public void MessageHandler(object sender, MessageEvent e)
        {
            if (e.IsOnline)
            {
                SendToServer(e);
            }
        }
        public void GameCreateHandler(object sender, GameCreateEventArgs e)
        {
            if (e.IsOnline)
            {
                GameCreate ev = new GameCreate(e.TotalPlayerNumber, e.IANumber,null, true, e.GroupName, e.Access, e.Password, e.GroupId, e.Avatar, e.playerName);
                SendToServer(ev);
            }
        }
        public void GameDeleteHandler(object sender, GameDeleteEventArgs e)
        {
            if (e.IsOnline)
            {
                SendToServer(e);
            }
        }

        public void EndPhaseHandler(object sender, BaseEventArgs e)
        {
            if (e.IsOnline)
            {
                SendToServer(e);
            }
        }

        public void DiscardExtraRessourcesHandler(object sender, DiscardEvent e)
        {
            if (e.IsOnline)
            {
                SendToServer(e);
            }
        }
        public void ConstructHandler(object sender, ConstructEventArgs e)
        {
            if (e.IsOnline)
            {
                SendToServer(e);
            }
        }
        public void PlayerExchangeHandler(object sender, ExchangeEvent e)
        {
            if (e.IsOnline)
            {
            
                /*List<(string, int)> gift = new List<(string, int)>();
                foreach ((RessourceType, int) couple in e.GiftedRessources)
                {
                    gift.Add((couple.Item1.ToString(), couple.Item2));
                }
                string gifted = Serialization.Serialize(gift);
                ExchangeEvent ev = new ExchangeEvent(gifted, e.GameId);*/
                SendToServer(e);
            }
        }


        public void PlayerAcceptExchangeHandler(object sender, AcceptExchange e)
        {
            if (e.IsOnline)
            {
                SendToServer(e);
            }
        }
        public void HarborExchangeHandler(object sender, HarborExchangeEventArgs e)
        {
            if (e.IsOnline)
            {
                SendToServer(e);
            }
        }
        public void KnightCardUseHandler(object sender, BanditMoveEventArgs e)
        {
            if (e.IsOnline)
            {
                SendToServer(e);
            }
        }
        public void BanditMoveHandler(object sender, BanditMoveEventArgs e)
        {
            if (e.IsOnline)
            {
                SendToServer(e);
            }
        }

        public void MonopolyCardUseHandler(object sender, MonopolyCardUseEventArgs e)
        {
            if (e.IsOnline)
            {
                SendToServer(e);
            }
        }
        public void ResourcePairCardUseHandler(object sender, ResourcePairCardUseEventArgs e)
        {
            if (e.IsOnline)
            {
                SendToServer(e);
            }
        }
        public void RoadConstructionCardUseHandler(object sender, RoadConstructionCardUseEventArgs e)
        {
            if (e.IsOnline)
            {
                SendToServer(e);
            }
        }
        public void DiceThrowHandler(object sender, BaseEventArgs e)
        {
            if (e.IsOnline)
            {
                SendToServer(e);
            }
        }
        public void InitialColonyHandler(object sender, InitialConstructEventArgs e)
        {
            if (e.IsOnline)
            {
                SendToServer(e);
            }
        }
        public void InitialRoadHandler(object sender, InitialConstructEventArgs e)
        {
            if (e.IsOnline)
            {
                SendToServer(e);
            }
        }

    }
}
