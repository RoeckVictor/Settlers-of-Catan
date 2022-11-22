using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Noyau.View;
using Util.View;
using Network.Model;
using Network.View;
using ErrorEventArgs = Util.View.ErrorEventArgs;
using Player = Network.Model.Player;
using AI.View;
using AI.Model;
using AI.Controller;

namespace Network.Controller
{
    class NetworkManager
    {
        /// <summary>
        /// <para>La classe NetworkManager</para>
        /// <para>Contient l'ensemble des méthodes et évènements nécessaire à la communication entre le réseau du côté serveur et le noyau</para>
        /// </summary>
        public GameManager Games { get; private set; }
        private DataBase BDD;
        private bool registred;

        public NetworkManager()
        {
            Games = new GameManager();
            BDD = new DataBase(true);
            registred = false;
        }

        /// <summary>
        /// Appelée par les threads assignés à chaque client pour traiter l'évenement reçu depuis le jeu chez le client
        /// </summary>
        /// <param name="obj">Représente le TcpClient du client qui permet de faire une communication TCP</param>
        public void Communication(Object obj)
        {
            TcpClient tp = (TcpClient)obj;
            Client client = new Client(tp);

            string data;
            client.Send(allGames());

            while (!(data = client.Receive()).Equals(""))
            {
                EventType keyEvent = Callbacks.GetType(Serialization.ClassName(data));
                Console.WriteLine("Received an event: " + keyEvent.ToString());
                List<OnlineGame> games = BDD.GetAvailableGames();
                Games.Games.Clear();
                foreach (OnlineGame g in games)
                {
                    Games.Games.Add(g.InGame, g);
                }
                UpdateGame(data, keyEvent, client); // data is crypted
            }
        }

        /// <summary>
        /// Récupère toutes les parties de jeu stockées en base de données
        /// </summary>
        public string allGames()
        {
            List<OnlineGame> games = BDD.GetAvailableGames();
            List<GameLine> stringGames = new List<GameLine>();
            foreach (OnlineGame kvp in games)
            {
                stringGames.Add(new GameLine(kvp.InGame, kvp.NbPlayers, ClientManager.GetIAsInGame(kvp.InGame), kvp.Name, kvp.Access, kvp.IsFull(), kvp.nbConnected, kvp.nbReady));
            }
            return Serialization.Serialize(stringGames);
        }

        /// <summary>
        /// Met à jour l'état d'une partie de jeu lors de la réception d'un événement
        /// </summary>
        /// <param name="uncryptedData">Représente l'événement sérialisé envoyé depuis le client</param>
        /// <param name="keyEvent">Le type de l'évenement extrait avant l'appel de cette fonction/param>
        /// <param name="client">Représente toutes la classe contenant toutes les caractéristiques d'un client TCP</param>
        public void UpdateGame(string uncryptedData, EventType keyEvent, Client client)
        {
            List<(int, string)> playerName = new List<(int, string)>();
            List<(int, int)> playerAvatar = new List<(int, int)>();
            string playerNames, playerAvatars;
            switch (keyEvent)
            {
                case EventType.CREATE:
                    CreateGame(uncryptedData, client);
                    break;


                case EventType.JOIN:
                    JoinGroup(uncryptedData, client);
                    break;


                case EventType.REGISTER:
                    RegisterEventArgs register = (RegisterEventArgs)Serialization.XMLDeSerialize(new RegisterEventArgs(), uncryptedData);
                    client.SetName(register.Name);
                    int retVal = BDD.InsertPlayer(client, Password.HashPassword(register.Password));
                    if (retVal != -1) client.SetIDDB(retVal);
                    else
                    {
                        ErrorEventArgs err = new Util.View.ErrorEventArgs(ErrorType.REGISTER_DB, BDD.LastErrorMessage);
                        client.Send(Serialization.XMLSerialize(err));
                    }
                    break;

                case EventType.ACCEPT:
                    AcceptExchange(uncryptedData);
                    break;

                case EventType.EXCHANGE:

                    ExchangeEvent ex = (ExchangeEvent)Serialization.XMLDeSerialize(new ExchangeEvent(), uncryptedData);
                    foreach (KeyValuePair<Client, Guid> mycli in ClientManager.TCPClients)
                    {
                        if (mycli.Value == ex.GameId) 
                        {
                            Console.WriteLine("1");
                            mycli.Key.Send(Serialization.XMLSerialize(ex));
                        }
                    }
                    break;

                case EventType.BASE:
                    BaseEventArgs rgs = (BaseEventArgs)Serialization.XMLDeSerialize(new BaseEventArgs(), uncryptedData);
                    if (rgs.FunctionName == "OnEndPhase") GameView.Instance.OnEndPhase(rgs);
                    if (rgs.FunctionName == "OnDiceThrow") GameView.Instance.OnDiceThrow(rgs);
                    break;

                case EventType.DELETE:
                    /*GameDeleteEventArgs delete = (GameDeleteEventArgs)Serialization.XMLDeSerialize(new GameDeleteEventArgs(), uncryptedData);
                    GameView.Instance.OnGameDelete(delete);
                    BDD.DeleteGame(delete.GameId);
                    Games.removeGame(delete.GameId);*/
                    break;

                case EventType.CONSTRUCT:
                    ConstructEventArgs constructEvent = (ConstructEventArgs)Serialization.XMLDeSerialize(new ConstructEventArgs(), uncryptedData);
                    GameView.Instance.OnConstruct(constructEvent);
                    break;

                case EventType.BANDIT:
                    BanditMoveEventArgs banditMove = (BanditMoveEventArgs)Serialization.XMLDeSerialize(new BanditMoveEventArgs(), uncryptedData);
                    IGame igame = GameView.Instance.GetGame(banditMove.GameId);
                    if (igame.CurrentPhase == GamePhase.BANDIT_MOVE)
                        GameView.Instance.OnBanditMove(banditMove);
                    else GameView.Instance.OnKnightCardUse(banditMove);
                    break;

                case EventType.HARBOR:
                    HarborExchangeEventArgs harborExchange = (HarborExchangeEventArgs)Serialization.XMLDeSerialize(new HarborExchangeEventArgs(), uncryptedData);
                    GameView.Instance.OnHarborExchange(harborExchange);
                    break;

                case EventType.RESOURCE:
                    ResourcePairCardUseEventArgs resourcePair = (ResourcePairCardUseEventArgs)Serialization.XMLDeSerialize(new ResourcePairCardUseEventArgs(), uncryptedData);
                    GameView.Instance.OnResourcePairCardUse(resourcePair);
                    break;

                case EventType.ROAD:
                    RoadConstructionCardUseEventArgs roadConstrcut = (RoadConstructionCardUseEventArgs)Serialization.XMLDeSerialize(new RoadConstructionCardUseEventArgs(), uncryptedData);
                    GameView.Instance.OnRoadConstructionCardUse(roadConstrcut);
                    break;

                case EventType.INITIAL:
                    InitialConstructEventArgs initial = (InitialConstructEventArgs)Serialization.XMLDeSerialize(new InitialConstructEventArgs(), uncryptedData);
                    if (initial.BuildingCoordinate.D == Direction.UP || initial.BuildingCoordinate.D == Direction.DOWN) GameView.Instance.OnInitialColony(initial);
                    else GameView.Instance.OnInitialRoad(initial);
                    break;

                case EventType.MONOPOLY:
                    MonopolyCardUseEventArgs monopoly = (MonopolyCardUseEventArgs)Serialization.XMLDeSerialize(new MonopolyCardUseEventArgs(), uncryptedData);
                    GameView.Instance.OnMonopolyCardUse(monopoly);
                    break;

                case EventType.DISCONNECT:
                    DisconnectEventArgs disconnect = (DisconnectEventArgs)Serialization.XMLDeSerialize(new DisconnectEventArgs(), uncryptedData);
                    OnlineGame gamee = BDD.GetGame(disconnect.GameId);
                    if(gamee!=null)
                    {
                        foreach (Player pp in gamee.Players)
                        {
                            BDD.Disconnect(pp.IdDB);
                        }
                    }
              

                    if (ClientManager.TCPClients.ContainsKey(client))
                        ClientManager.TCPClients.Remove(client);
                    foreach (KeyValuePair<Client, Guid> mycli in ClientManager.TCPClients)
                    {
                        if (mycli.Value == disconnect.GameId && mycli.Key.IdInGame!=disconnect.PlayerId)
                        {
                            mycli.Key.Send(Serialization.XMLSerialize(new GameDeletedArgs(disconnect.GameId,true,true)));
                            ClientManager.TCPClients.Remove(mycli.Key);
                        }
                    }
                    GameDeleteEventArgs del = new GameDeleteEventArgs(disconnect.GameId, true);
                    //GameView.Instance.OnGameDelete(del);
                    BDD.DeleteGame(disconnect.GameId);
                    Games.removeGame(disconnect.GameId);
                    break;

                case EventType.QUITLOBBY:
                    QuitLobby ql = (QuitLobby)Serialization.XMLDeSerialize(new QuitLobby(), uncryptedData);

                    OnlineGame game = BDD.GetGame(ql.GameId);
                    int idb = 0;
                    if(game!=null)
                    {
                        foreach (Player p in game.Players)
                        {
                            if (p.IdInGame == ql.PlayerId) idb = p.IdDB;
                        }

                        BDD.Disconnect(idb);
                    }

                    if(ClientManager.TCPClients.ContainsKey(client))
                        ClientManager.TCPClients.Remove(client);

                    game = BDD.GetGame(ql.GameId);
                    if(game!=null)
                    {
                        Console.WriteLine("id: " + ql.GameId);
                        if (game.nbConnected == 0) // Createur de la partie
                        {
                            BDD.DeleteGame(ql.GameId);
                            Games.removeGame(ql.GameId);
                            Console.WriteLine("remove bdd id: " + ql.GameId);
                        }
                        else
                        {
                            playerName.Clear();
                            playerAvatar.Clear();
                            foreach (Player p in game.Players)
                            {
                                playerName.Add((p.IdInGame, p.Name));
                                playerAvatar.Add((p.IdInGame, p.Avatar));
                            }
                            for (int k = 0; k < ClientManager.GetIAsInGame(ql.GameId); k++)
                            {
                                playerName.Add((game.NbPlayers - ClientManager.GetIAsInGame(ql.GameId), "Bot" + k));
                                playerAvatar.Add((game.NbPlayers - ClientManager.GetIAsInGame(ql.GameId), 0));
                            }
                            playerNames = Serialization.Serialize(playerName);
                            playerAvatars = Serialization.Serialize(playerAvatar);
                            GameLine gm2 = new GameLine(ql.GameId, game.NbPlayers, ClientManager.GetIAsInGame(ql.GameId), game.Name, game.Access, game.IsFull(), game.nbConnected, game.nbReady);
                            InfoGame ig2 = new InfoGame(ql.GameId, client.IdInGame, gm2, playerNames, playerAvatars, true);

                            foreach (KeyValuePair<Client, Guid> mycli in ClientManager.TCPClients)
                            {
                                if (mycli.Value == ql.GameId)
                                {
                                    mycli.Key.Send(Serialization.XMLSerialize(ig2));
                                }
                            }
                        }
                    }
                   
                    Refresh r = new Refresh(allGames());
                    client.Send(Serialization.XMLSerialize(r));
                    break;

                case EventType.REFRESH:
                    string agm = allGames();
                    Refresh refr= new Refresh(agm);
                    Console.WriteLine((Serialization.XMLSerialize(agm)));
                    client.Send("<Name>Util.View.Refresh</Name><Games>"+allGames()+"</Games>"); ;
                    break;

                case EventType.DISCARDNETWORK:
                    List<(RessourceType rType, int num)> list = new List<(RessourceType rType, int num)>();
                    DiscardEvent discard = (DiscardEvent)Serialization.XMLDeSerialize(new DiscardEvent(), uncryptedData);
                    Serialization.Deserialize(discard.list, list);
                    OnlineGame g = BDD.GetGame(discard.GameId);
                    ClientManager.addDiscrad(discard.GameId, discard.IdPlayer, list);
                    ClientManager.Increase(discard.GameId);
                    if (g.NbPlayers - ClientManager.GetIAsInGame(discard.GameId) == ClientManager.NBDiscrad(discard.GameId))
                    {
                        List<(int idPlayer, List<(RessourceType rType, int num)>)> listD = new List<(int idPlayer, List<(RessourceType rType, int num)>)>();
                        foreach (KeyValuePair<(Guid, int), List<(RessourceType rType, int num)>> entry in ClientManager.Discards)
                        {
                            listD.Add((entry.Key.Item2, entry.Value));
                        }
                        DiscardEventArgs dis = new DiscardEventArgs(discard.GameId, listD, true);
                        GameView.Instance.OnDiscardExtraRessources(dis);
                        ClientManager.Reset(discard.GameId);
                        ClientManager.RemoveDiscards(discard.GameId);
                    }
                    break;

                case EventType.MESSAGE:
                    foreach (KeyValuePair<Client, Guid> mycli in ClientManager.TCPClients)
                    {
                        if (mycli.Value == client.IdGame)
                        {
                            mycli.Key.Send(uncryptedData);
                        }
                            
                    }
                    break;
                case EventType.READY:
                    Ready(uncryptedData, client);
                    break;

                case EventType.START:
                    StartedGame sg = (StartedGame)Serialization.XMLDeSerialize(new StartedGame(), uncryptedData);
                    BDD.UpdateGameState(sg.GameId, DataBase.GameState.Started);
                    foreach (KeyValuePair<Client, Guid> mycli in ClientManager.TCPClients)
                    {
                        if (mycli.Value == sg.GameId)
                        {
                            mycli.Key.Send(Serialization.XMLSerialize(sg));
                        }
                    }
                    break;
                default:
                    foreach (KeyValuePair<Client, Guid> mycli in ClientManager.TCPClients)
                    {
                        if (mycli.Value == client.IdGame)
                        {
                            mycli.Key.Send(uncryptedData);
                        }
                           
                    }
                    break;
            }
        }

        /// <summary>
        /// Crée la partie en base de donnée et lie le client l'ayant créé en BDD
        /// </summary>
        /// <param name="uncryptedData">Représente l'événement create sérialisé envoyé depuis le client</param>
        /// <param name="client">Représente toutes la classe contenant toutes les caractéristiques d'un client TCP</param>
        public void CreateGame(string uncryptedData, Client client)
        {
            Guid id;
            ErrorType errType = ErrorType.NONE;
            int max = BDD.MaxPlayerId(); int idp;
            List<(int, string)> playerName = new List<(int, string)>();
            List<(int, int)> playerAvatar = new List<(int, int)>();
            string playerNames, playerAvatars;

            GameCreate cg = (GameCreate)Serialization.XMLDeSerialize(new GameCreate(), uncryptedData);
            OnlineGame game = new OnlineGame(cg.TotalPlayerNumber, cg.IANumber, cg.GroupName, cg.Access, null, false, 0, 0);
            id = Games.addGame(game);
            ClientManager.addIA(id, cg.IANumber);
            ClientManager.addClient(id, client);
            if (id == Guid.Empty) { errType = ErrorType.CREATE_GAME; }
            else
            {
                Games.addPlayer(client, id);
                bool gameCreate;
                if (!cg.Access)
                {
                    if (!(gameCreate = BDD.InsertGame(game, cg.Password)))
                    {
                        errType = ErrorType.CREATE_GAME;
                    }
                }
                else
                {
                    if (!(gameCreate = BDD.InsertGame(game)))
                    {
                        errType = ErrorType.CREATE_GAME;
                    }
                }
                if (gameCreate)
                {
                    // Si le jeu a été créé, on le relie avec une clé étrangère pour client dans la BDD
                    client.SetName(cg.playerName + max.ToString());
                    client.SetAvatar(cg.Avatar);
                    client.Type = Model.ClientType.AnonymousPlayer;
                    client.SetIdGame(id);
                    client.SetReady(false);

                    idp = BDD.InsertPlayer(client);
                    if (idp != -1)
                    {
                        client.SetIDDB(idp);
                        if (!BDD.Connect(idp, id, client.IdInGame)) // Si la BDD n'a pas réussi à connecter la partie donc il faut détruire détruire
                        {
                            errType = ErrorType.CONNECT_GAME_PLAYER;
                            if (BDD.DeleteGame(id)) { errType = ErrorType.ADD_PLAYER_TO_GROUP; }
                        }
                    }
                    else
                    {
                        if (BDD.DeleteGame(id)) { errType = ErrorType.ADD_PLAYER_TO_GROUP; }
                        else { errType = ErrorType.REMOVE_GAME; }
                    }
                }
                else { errType = ErrorType.CREATE_GAME; }

            }
            if (errType != ErrorType.NONE)
            {
                ErrorEventArgs err = new ErrorEventArgs(errType, BDD.LastErrorMessage);
                client.Send(Serialization.XMLSerialize(err));
            }
            else
            {
                // Envoyer les informations de cette partie au client qui l'a créé
                game = BDD.GetGame(id);
                playerName.Clear();
                foreach (Player p in game.Players)
                {
                    playerName.Add((p.IdInGame, p.Name));
                    playerAvatar.Add((p.IdInGame, p.Avatar));
                }
                for (int k = 0; k < cg.IANumber; k++)
                {
                    playerName.Add((game.NbPlayers - cg.IANumber, "Bot" + k));
                    playerAvatar.Add((game.NbPlayers - cg.IANumber, 0));
                }
                playerNames = Serialization.Serialize(playerName);
                playerAvatars = Serialization.Serialize(playerAvatar);
                GameLine gm1 = new GameLine(id, game.NbPlayers, cg.IANumber, game.Name, game.Access, game.IsFull(), game.nbConnected, game.nbReady);
                InfoGame ig1 = new InfoGame(id, client.IdInGame, gm1, playerNames, playerAvatars, true);
                client.Send(Serialization.XMLSerialize(ig1));
            }
        }

        /// <summary>
        /// Fonction appelée quand un client veut rejoindre une partie aléatoire ou une partie déjà créée
        /// </summary>
        /// <param name="uncryptedData">Représente l'événement join sérialisé envoyé depuis le client</param>
        /// <param name="client">Représente toutes la classe contenant toutes les caractéristiques d'un client TCP</param>
        public void JoinGroup(string uncryptedData, Client client)
        {
            Guid id;
            ErrorType errType = ErrorType.NONE;
            int max = BDD.MaxPlayerId(); int idp;
            List<(int, string)> playerName = new List<(int, string)>();
            List<(int, int)> playerAvatar = new List<(int, int)>();
            string playerNames, playerAvatars;
            OnlineGame game = new OnlineGame();

            GameJoinEventArgs jg = (GameJoinEventArgs)Serialization.XMLDeSerialize(new GameJoinEventArgs(), uncryptedData);
            if (!jg.Random)
            {
                id = jg.GameId;
                if (id != Guid.Empty)
                {
                    ClientManager.addClient(id, client);
                    bool ac = BDD.GetGame(jg.GameId).Access;
                    if (!ac)
                    {
                        // Vérifier le mot de passe si c'est une partie privée
                        if (!BDD.VerifyGamePassword(id, jg.Password))
                        {
                            errType = ErrorType.PASSWORD;
                        }
                        else
                        {
                            if (!Games.addPlayer(client, id))
                            {
                                errType = ErrorType.ADD_PLAYER_TO_GROUP;
                            }
                        }
                    }
                    else
                    {
                        if (!Games.addPlayer(client, id))
                        {
                            errType = ErrorType.ADD_PLAYER_TO_GROUP;
                        }
                    }
                }
                else { errType = ErrorType.JOIN_GROUP; }
            }
            else
            {
                // Rejoindre une partie aléatoire
                id = Games.RandomGame();
                ClientManager.addClient(id, client);
                if (id != Guid.Empty)
                {
                    if (!Games.addPlayer(client, id))
                    {
                        Console.WriteLine("Error adding player to the random game");
                        errType = ErrorType.ADD_PLAYER_TO_GROUP;
                    }
                }
                else { errType = ErrorType.JOIN_RANDOM_GROUP; Console.WriteLine("Error joining random group"); } // TODO lui créer une partie standard
            }
            if (errType == ErrorType.NONE)
            {
                // Relier le client à la partie BDD
                client.SetName(jg.playerName + max.ToString());
                client.SetAvatar(jg.Avatar);
                client.SetReady(false);
                client.SetIdGame(id);
                idp = BDD.InsertPlayer(client);
                if (idp != -1)
                {
                    client.SetIDDB(idp);
                    if (!BDD.Connect(idp, id, client.IdInGame)) // Pas réussi à connecter avec la partie
                    {
                        errType = ErrorType.CONNECT_GAME_PLAYER;
                    }
                }
                else
                {
                    errType = ErrorType.JOIN_GROUP;
                }

                if (errType != ErrorType.NONE)
                {
                    ErrorEventArgs err = new ErrorEventArgs(errType, BDD.LastErrorMessage);
                    client.Send(Serialization.XMLSerialize(err));
                }
                else
                {
                    // Envoyer les informations de la partie au joueur et à tous les autres pour une mise à jour
                    game = BDD.GetGame(id);
                    playerName.Clear();
                    playerAvatar.Clear();
                    foreach (Player p in game.Players)
                    {
                        playerName.Add((p.IdInGame, p.Name));
                        playerAvatar.Add((p.IdInGame, p.Avatar));
                      
                    }
                    for (int k = 0; k < ClientManager.GetIAsInGame(id); k++)
                    {
                        playerName.Add((game.NbPlayers - ClientManager.GetIAsInGame(id), "Bot" + k));
                        playerAvatar.Add((game.NbPlayers - ClientManager.GetIAsInGame(id), 0));
                    }
                    playerNames = Serialization.Serialize(playerName);
                    playerAvatars = Serialization.Serialize(playerAvatar);
                    GameLine gm2 = new GameLine(id, game.NbPlayers, ClientManager.GetIAsInGame(jg.GameId), game.Name, game.Access, game.IsFull(), game.nbConnected, game.nbReady);
                    InfoGame ig2 = new InfoGame(id, client.IdInGame, gm2, playerNames, playerAvatars, true);
                    client.Send(Serialization.XMLSerialize(ig2));

                    PlayerJoinedGame join = new PlayerJoinedGame(jg.Name, client.IdInGame, id);
                    foreach (KeyValuePair<Client, Guid> mycli in ClientManager.TCPClients)
                    {
                        if (mycli.Value == jg.GameId)
                        {
                            mycli.Key.Send(Serialization.XMLSerialize(join));
                           
                        }
                    }

                }
                if (Games.isFull(id))
                {
                    // Si la partie est pleine envoyer un événement au noyau pour créer l'instance de la partie
                    OnlineGame gm3 = Games.getGame(id);
                    GameCreateEventArgs create = new GameCreateEventArgs(gm3.NbPlayers, ClientManager.GetIAsInGame(jg.GameId), gm3.Grid, true, gm3.Name, gm3.Access, jg.Password, id, 0, "Guest");
                    GameView.Instance.OnGameCreate(create);
                    Console.WriteLine("Game is full, ready to be started...");
                }

            }
            else
            {
                Console.WriteLine("An error has occured previously: " + errType);
                ErrorEventArgs err = new Util.View.ErrorEventArgs(errType, BDD.LastErrorMessage);
                client.Send(Serialization.XMLSerialize(err));
            }
            Console.WriteLine(errType);
        }

        /// <summary>
        /// Fonction appelée quand un client accepte un échange de cartes avec un autre
        /// </summary>
        /// <param name="uncryptedData">Représente l'événement join sérialisé envoyé depuis le client</param>
        public void AcceptExchange(string uncryptedData)
        {
            AcceptExchange ee = (AcceptExchange)Serialization.XMLDeSerialize(new AcceptExchange(), uncryptedData);
            List<(RessourceType t, int num)> gifted = new List<(RessourceType t, int num)>();
            List<(RessourceType t, int num)> recieved = new List<(RessourceType t, int num)>();

            Serialization.Deserialize(ee.GifteRessources, gifted);
            Serialization.Deserialize(ee.RecievedRessources, recieved);
            PlayerExchangeEventArgs playerExchange = new PlayerExchangeEventArgs(ee.GameId, recieved, ee.OtherPlayerId, gifted, true);
            GameView.Instance.OnPlayerExchange(playerExchange);
        }
        /// <summary>
        /// Fonction appelée quand un client signale qu'il est prêt à démarrer la partie de jeu
        /// </summary>
        /// <param name="uncryptedData">Représente l'événement join sérialisé envoyé depuis le client</param>
        /// <param name="client">Représente toutes la classe contenant toutes les caractéristiques d'un client TCP</param>
        public void Ready(string uncryptedData, Client client)
        {
            List<(int, string)> playerName = new List<(int, string)>();
            List<(int, int)> playerAvatar = new List<(int, int)>();
            string playerNames, playerAvatars;
            OnlineGame game = new OnlineGame();
            ReadyEvent re = (ReadyEvent)Serialization.XMLDeSerialize(new ReadyEvent(), uncryptedData);
            BDD.SetPlayerReady(client.IdDB, true);
            OnlineGame mygame = BDD.GetGame(re.GameId);

            playerName = mygame.listPlayers();
            playerAvatar = mygame.listAvatar();
            playerNames = Serialization.Serialize(playerName);
            playerAvatars = Serialization.Serialize(playerAvatar);
            GameLine gm = new GameLine(re.GameId, mygame.NbPlayers, ClientManager.GetIAsInGame(mygame.InGame), mygame.Name, mygame.Access, mygame.IsFull(), mygame.nbConnected, mygame.nbReady);
            InfoGame ig = new InfoGame(re.GameId, client.IdInGame, gm, playerNames, playerAvatars, false);
            uncryptedData = Serialization.XMLSerialize(ig);
            foreach (KeyValuePair<Client, Guid> mycli in ClientManager.TCPClients)
            {
                if (mycli.Value == re.GameId)
                {
                    mycli.Key.Send(uncryptedData);
                }
                    
            }
        }


        public void DisconnectPlayer(string uncryptedData, Client client)
        {

            List<(int, string)> playerName = new List<(int, string)>();
            List<(int, int)> playerAvatar = new List<(int, int)>();
            string playerNames, playerAvatars;

            DisconnectEventArgs disconnect = (DisconnectEventArgs)Serialization.XMLDeSerialize(new DisconnectEventArgs(), uncryptedData);
            OnlineGame gm1 = BDD.GetGame(disconnect.GameId);
            Player pprim = new Player();
            foreach (Player p in gm1.Players)
            {
                if (p.IdInGame == disconnect.PlayerId) pprim = p;
            }
            BDD.DeletePlayer(pprim.IdDB);
            if (gm1.NbPlayers == 1)
            {
                BDD.DeleteGame(disconnect.GameId);
            }
            else
            {
               
                ClientManager.TCPClients.Remove(client);

                //remplacer par IA et informer les autres
                AIManager.Instance.ReplaceWithAi(GameView.Instance.GetGame(disconnect.GameId), disconnect.PlayerId, 2);

                playerName.Clear();
                playerAvatar.Clear();
                foreach (Player p in gm1.Players)
                {
                    playerName.Add((p.IdInGame, p.Name));
                    playerAvatar.Add((p.IdInGame, p.Avatar));
                }
                for (int k = 0; k < ClientManager.GetIAsInGame(disconnect.GameId); k++)
                {
                    playerName.Add((gm1.NbPlayers - ClientManager.GetIAsInGame(disconnect.GameId), "Bot" + k));
                    playerAvatar.Add((gm1.NbPlayers - ClientManager.GetIAsInGame(disconnect.GameId), 0));
                }
                playerNames = Serialization.Serialize(playerName);
                playerAvatars = Serialization.Serialize(playerAvatar);
                GameLine gm2 = new GameLine(disconnect.GameId, gm1.NbPlayers, ClientManager.GetIAsInGame(disconnect.GameId), gm1.Name, gm1.Access, gm1.IsFull(), gm1.nbConnected, gm1.nbReady);
                InfoGame ig2 = new InfoGame(disconnect.GameId, client.IdInGame, gm2, playerNames, playerAvatars, true);

                foreach (KeyValuePair<Client, Guid> mycli in ClientManager.TCPClients)
                {
                    if (mycli.Value == disconnect.GameId)
                    {
                        Console.WriteLine("7");
                        mycli.Key.Send(Serialization.XMLSerialize(ig2));
                    }
                        
                }

            }


        }


        public void QuitLobby(string uncryptedData, Client client)
        {
            QuitLobby qev = (QuitLobby)Serialization.XMLDeSerialize(new QuitLobby(), uncryptedData);
            OnlineGame gm = BDD.GetGame(qev.GameId);
            Player pprime = new Player();
            foreach(Player p in gm.Players)
            {
                if (p.IdInGame == qev.PlayerId) pprime = p;
            }
            ClientManager.TCPClients.Remove(client);
            pprime.SetIdGame(Guid.Empty);
            BDD.UpdatePlayer(pprime);
        }
        /// <summary>
        /// Fonction interne pour enregistrer les évenements envoyés par le noyau
        /// </summary>
        internal void RegisterEventHandlers()
        {
            if (registred) return;

            NetworkView.Instance.GameStatus += OnGameStatus;
            NetworkView.Instance.GameStatusCreate += OnGameStatusCreate;
            NetworkView.Instance.ActionDone += OnActionDoneInfo;
            NetworkView.Instance.Deleted += OnDeleted;
            NetworkView.Instance.DiceResults += OnDiceResultInfo;
            NetworkView.Instance.VictoryInfo += OnVictoryInfo;

            registred = true;
        }

        /// <summary>
        /// Handler de l'évenement état de la partie envoyé par le noyau
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement Etat d'une partie</param>
        /// 

        public void OnGameStatusCreate(Object sender, GameStatusArgsNetwork e)
        {
            GameStatus gs = new GameStatus(e.GameId, e.Game, e.NbIA, true);
            foreach (KeyValuePair<Client, Guid> entry in ClientManager.TCPClients)
            {
                if (entry.Value == e.GameId)
                {
                    entry.Key.Send(Serialization.XMLSerialize(gs));
                }
            }

        }
        public void OnGameStatus(Object sender, GameStatusArgsNetwork e)
        {
            //GameStatus gs = new GameStatus(e.GameId, serializedGame, e.NbIA, true);
            //string cryptedData = Serialization.XMLSerialize(gs);
            OnlineGame g = BDD.GetGame(e.GameId);
            if (g.hasStarted)
            {
                foreach (KeyValuePair<Client, Guid> entry in ClientManager.TCPClients)
                {
                    if (entry.Value == e.GameId)
                    {
                        entry.Key.Send(Serialization.XMLSerialize(e));
                    }
                }
            }
        }

        /// <summary>
        /// Handler de l'évenement pour supprimer une partie
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement Suppression de partie</param>
        public void OnDeleted(Object sender, GameDeletedArgs e)
        {
            /*string cryptedData = Serialization.XMLSerialize(e);
            OnlineGame game = BDD.GetGame(e.GameId);
            if(game!= null)
            {
                foreach (Client mycli in game.Players)
                {
                    if(mycli.Tcp.Connected)
                    mycli.Send(cryptedData);
                }
            } */           
        }

        /// <summary>
        /// Handler de l'évenement qui capture le résultat du lancé de dés
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement du lancé de dés</param>
        public void OnDiceResultInfo(Object sender, DiceResultsInfoArgsNetwork e)
        {
            // Envoie à tous les joueurs le résultat obtenu
            OnlineGame game = BDD.GetGame(e.GameId);
            if (game.hasStarted)
            {
                foreach (KeyValuePair<Client, Guid> entry in ClientManager.TCPClients)
                {
                    if (entry.Value == e.GameId)
                    {
                        entry.Key.Send(Serialization.XMLSerialize(e));
                    }
                }
            }
        }
        /// <summary>
        /// Handler de l'évenement d'une action faite
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement Action faite/param>
        public void OnActionDoneInfo(Object sender, ActionDoneInfoArgsNetwork e)
        {
            string serializedGame = e.Game.Serialize();
            OnlineGame game = BDD.GetGame(e.GameId);
            if (game.hasStarted)
            {
                foreach (KeyValuePair<Client, Guid> entry in ClientManager.TCPClients)
                {
                    if (entry.Value == e.GameId)
                    {
                        //entry.Key.Send(crypted);
                        Console.WriteLine("9");
                        ActionDoneNetwork action = new ActionDoneNetwork("Util.View.ActionDoneNetwork", e.GameId, e.Action.ToString(), e.ActionSuccessful, e.PlayerID, e.Resource1.ToString(), e.Resource2.ToString(), e.FunctionName, serializedGame);
                        entry.Key.Send(Serialization.XMLSerialize(action));
                    }
                }
            }
        }
        /// <summary>
        /// Handler de l'évenement de victoire
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement de victoire</param>
        public void OnVictoryInfo(Object sender, VictoryInfoArgs e)
        {

            string serializedGame = e.Game.Serialize();

            GameStatus gs = new GameStatus(e.GameId, serializedGame, ClientManager.GetIAsInGame(e.GameId), true);
            string crypted = Serialization.XMLSerialize(gs);

            OnlineGame game = BDD.GetGame(e.GameId);
            if (game.hasStarted)
            {
                foreach (KeyValuePair<Client, Guid> entry in ClientManager.TCPClients)
                {
                    if (entry.Value == e.GameId)
                    {
                        string g = e.Game.Serialize();
                        string rankings = Serialization.Serialize(e.PlayerRankings);
                        int iaNum = ClientManager.GetIAsInGame(e.GameId);
                        VictoryInfo ev = new VictoryInfo(e.GameId, g, rankings, iaNum, true);
                        entry.Key.Send(crypted);
                        entry.Key.Send(Serialization.XMLSerialize(ev));

                    }
                }
            }
        }
    }
}
