using System;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Util.View;
using EventType = Util.View.EventType;
using Noyau.View;
using System.Collections.Generic;
using Assets.Resources.Scripts.Graphique;
using AI.Controller;
using Noyau.Model;

namespace Assets.Src.Network.Model
{
    public class Client
    {
        /// <summary>
        /// Si le client est connecté au serveur ou pas
        /// </summary>
        public bool isConnected;
        /// <summary>
        /// Socket qui permet de se connecter au serveur
        /// </summary>
        public TcpClient Tcp { get; private set; }
        /// <summary>
        /// Flux maximum à envoyer au serveur
        /// </summary>
        private const int MAX = 1000000;
        /// <summary>
        /// Identifiant du client dans le jeu (entre 0 et 4)
        /// </summary>
        public int IdInGame { get; private set; }
        /// <summary>
        /// LIdentifiant de la partie auquel il est connecté
        /// </summary>
        public Guid Id { get; private set; }
        /// <summary>
        /// Informations de la partie de jeu (nombre de joueurs, nombre d'IAs etc)
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
        /// La grille sérializée
        /// </summary>
        private string Game;
        /// <summary>
        /// Indique si c'est la première fois qu'il reçoit l'étt de la partie ou pas, pour éviter de modifier son identifiant dans le jeu
        /// </summary>
        public bool First { get; set; }
        public Client()
        {
            isConnected = false;
            Tcp = new TcpClient();
            IdInGame = 0;
            Id = Guid.Empty;
            listJeux = new List<GameLine>();
            infoPlayers = new List<(int, string)>();
            infoGame = new GameLine();
            infoAvatar = new List<(int, int)>();

        }
        /// <summary>
        /// Permet de faire une connexion au serveur 
        /// </summary>
        /// <param name="host">L'adresse IP du serveur</param>
        /// <param name="port">Le port auquel on doit se connecter</param>
        public void Connect(string host, int port)
        {
            if (!isConnected)
            {
                Tcp = new TcpClient();
                Tcp.Connect(host, port);
                isConnected = true;
                First = true;
                byte[] games = Receive();
                string str = Encoding.ASCII.GetString(games);

                if (!str.Contains("NULL")) Serialization.Deserialize(str, listJeux);

                Thread t = new Thread(new ParameterizedThreadStart(HandleDevice));
                t.Start(Tcp);
            }
        }
        /// <summary>
        /// Ferme la socket de connexion avec le serveur
        /// </summary>
        public void Disconnect()
        {
            if (isConnected)
            {
                Tcp.GetStream().Close();
                Tcp.Close();
                isConnected = false;
                Debug.Log("Disconnect from server ");
            }
        }

        /// <summary>
        /// Permet de recevoir un message du serveur
        /// </summary>
        public byte[] Receive()
        {
            var stream = Tcp.GetStream();
            Byte[] bytes = new Byte[MAX];
            string data;
            int i = 0;
            try
            {
                i = stream.Read(bytes, 0, bytes.Length);
                data = Encoding.ASCII.GetString(bytes, 0, i);
                return bytes;
            }
            catch (Exception)
            {
                Console.WriteLine("Client disconnected");
                Tcp.Close();
                return null;
            }
        }

        /// <summary>
        /// Permet d'envoyer un message au serveur
        /// </summary>
        /// <param name="data">Le message à envoyer au serveur, dans notre cas ce sont les événements sérializés</param>
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
        /// Permet de traiter le message reçu dans un thread différent, pour éviter le blocage et effectuer des échanges asynchrones
        /// </summary>
        /// <param name="obj">Représnete la socket d'échange</param>
        private void HandleDevice(System.Object obj)
        {
            isConnected = true;
            TcpClient client = (TcpClient)obj;
            byte[] data;

            while ((data = Receive()) != null)
            {
                ProcessData(data);
            }
        }

        /// <summary>
        /// Permet de traiter les données reçues du serveur
        /// </summary>
        /// <param name="data">Le message reçu</param>
        private void ProcessData(byte[] data)
        {

            string uncryptedData = Encoding.ASCII.GetString(data);
            EventType keyEvent = Callbacks.GetType(Serialization.ClassName(uncryptedData));
            Debug.Log("Recieved event: " + keyEvent);

            switch (keyEvent)
            {
                /// Capture les informations de la partie de jeu en cours
                case EventType.MYINFO:
                    InfoGame ig = (InfoGame)Serialization.XMLDeSerialize(new InfoGame(), uncryptedData);
                    if (infoPlayers.Count != 0) infoPlayers.Clear();
                    if (infoAvatar.Count != 0) infoAvatar.Clear();
                    Serialization.Deserialize(ig.infoPlayers, infoPlayers);
                    Serialization.Deserialize(ig.infoAvatar, infoAvatar);
                    if (ig.First) IdInGame = ig.IdInGame;
                    Id = ig.IdGame;
                    infoGame = ig.game;
                    First = ig.First;
                    // Met à jour l'interface graphique avec les informations reçues
                    UpdateEvent up = new UpdateEvent(infoGame, infoPlayers, infoAvatar);
                    UI.Instance.OnUpdate(up);
                    break;


                case EventType.STATUS:
                    GameStatus gs = (GameStatus)Serialization.XMLDeSerialize(new GameStatus(), uncryptedData);
                    Game = gs.TheSerializedGame;
                    break;


                case EventType.DELETED:
                    GameDeletedArgs gd = (GameDeletedArgs)Serialization.XMLDeSerialize(new GameDeletedArgs(), uncryptedData);
                    UI.Instance.OnDeleted(gd);
                    break;


                case EventType.DICE:
                    DiceResultsInfoArgsNetwork dr = (DiceResultsInfoArgsNetwork)Serialization.XMLDeSerialize(new DiceResultsInfoArgsNetwork(), uncryptedData);
                    DiceResultsInfoArgs dice = new DiceResultsInfoArgs(dr.GameId, GameView.Instance.Game, dr.DiceRollResult, true);
                    GameView.Instance.Game.Deserialize(dr.Game);
                    GameView.Instance.Game.lastDice = (dr.DiceRollResult.Item1, dr.DiceRollResult.Item2);
                    if (dr.FunctionName == "OnExchangePhaseBegin") UI.Instance.OnExchangePhaseBeginReseau(dice);
                    if (dr.FunctionName == "OnDiscardPhaseBegin") UI.Instance.OnDiscardPhaseBeginReseau(dice);
                    
                    break;


                case EventType.VICTORY:
                    VictoryInfo vi = (VictoryInfo)Serialization.XMLDeSerialize(new VictoryInfo(), uncryptedData);
                    GameView.Instance.Game.Deserialize(vi.TheSerializedGame);
                    List<(int, int)> list = new List<(int, int)>();
                    Serialization.Deserialize(vi.PlayerRankings, list);
                    VictoryInfoArgs va = new VictoryInfoArgs(vi.GameId, GameView.Instance.Game, list, true);
                    UI.Instance.OnVictoryInfoReseau(va);
                    break;


                case EventType.ERROR:
                    ErrorEventArgs err = (ErrorEventArgs)Serialization.XMLDeSerialize(new ErrorEventArgs(), uncryptedData);
                    UI.Instance.OnError(err);
                    break;


                case EventType.MESSAGE:
                    MessageEvent msg = (MessageEvent)Serialization.XMLDeSerialize(new MessageEvent(), uncryptedData);
                    Debug.Log("Le message reçu: " + msg.Message);
                    UI.Instance.OnMessage(msg);
                    break;


                case EventType.READY:
                    ReadyEvent rdy = (ReadyEvent)Serialization.XMLDeSerialize(new ReadyEvent(), uncryptedData);
                    UI.Instance.OnReady(rdy);
                    break;


                case EventType.START:
                    StartedGame str = (StartedGame)Serialization.XMLDeSerialize(new StartedGame(), uncryptedData);
                    GameCreateEventArgs e = new GameCreateEventArgs(infoGame.NbPlayers, infoGame.NbIA, null, true, "", true, "", Id, 0, "");
                    GameView.Instance.OnGameCreate(e);
                    GameView.Instance.Game.Deserialize(Game);
                    UI.Instance.OnStart(str);
                    break;

                case EventType.STATUSNETWORK:
                    GameStatusArgsNetwork gsn = (GameStatusArgsNetwork)Serialization.XMLDeSerialize(new GameStatusArgsNetwork(), uncryptedData);
                    GameView.Instance.Game.Deserialize(gsn.Game);
                    GameStatusArgs gsargs = new GameStatusArgs(gsn.GameId, GameView.Instance.Game, true);

                    if (gsn.FunctionName == "OnGameBegin") UI.Instance.OnBeginReseau(gsargs);
                    if (gsn.FunctionName == "OnInitialConstructionFirstRound") UI.Instance.OnInitialConstructionFirstRoundReseau(gsargs);
                    if (gsn.FunctionName == "OnInitialConstructionSecondRound") UI.Instance.OnInitialConstructionSecondRoundReseau(gsargs);
                    if (gsn.FunctionName == "OnHarvestPhaseBegin") UI.Instance.OnHarvestPhaseBeginReseau(gsargs);
                    if (gsn.FunctionName == "OnBanditMoveBegin") UI.Instance.OnBanditMoveBeginReseau(gsargs);
                    if (gsn.FunctionName == "OnConstructionPhaseBegin") UI.Instance.OnConstructionPhaseBeginReseau(gsargs);
                    break;

                case EventType.INFONETWORK:
                    ActionDoneNetwork adi = (ActionDoneNetwork)Serialization.XMLDeSerialize(new ActionDoneNetwork(), uncryptedData);
                    GameView.Instance.Game.Deserialize(adi.Game);

                    ActionDoneInfoArgs acdni = new ActionDoneInfoArgs(adi.GameId, GameView.Instance.Game, Callbacks.GetActionType(adi.Action), adi.ActionSuccessful, adi.PlayerID, Callbacks.GetRessourceType(adi.Resource1), Callbacks.GetRessourceType(adi.Resource2), true);
                    if (adi.FunctionName == "OnConstructionDone") UI.Instance.OnConstructionDoneReseau(acdni);
                    if (adi.FunctionName == "OnExchangeDone") UI.Instance.OnExchangeDoneReseau(acdni);
                    if (adi.FunctionName == "OnCardUsageDone") UI.Instance.OnCardUsageDoneReseau(acdni);
                    break;
                // Envoie à l'UI pour qu'elle affiche les offres reçues
                case EventType.EXCHANGE:
                    ExchangeEvent exev = (ExchangeEvent)Serialization.XMLDeSerialize(new ExchangeEvent(), uncryptedData);
                    UI.Instance.OnExchange(exev);
                    break;

                case EventType.ACCEPT:
                    AcceptExchange acex = (AcceptExchange)Serialization.XMLDeSerialize(new AcceptExchange(), uncryptedData);
                    UI.Instance.OnAcceptExchange(acex);
                    break;

                case EventType.QUITLOBBY:
                    QuitLobby q = new QuitLobby();
                    UI.Instance.OnRetourArriere(q);
                    break;

                case EventType.REFRESH:
                    int pFrom = uncryptedData.IndexOf("<Games>") + "<Games>".Length;
                    int pTo = uncryptedData.LastIndexOf("</Games>");
                    uncryptedData.Substring(pFrom, pTo - pFrom);

                    listJeux.Clear();
                    Serialization.Deserialize(uncryptedData, listJeux);

                    Refresh refresh = new Refresh();
                    UI.Instance.OnRefresh(refresh);
                    break;
                

                default:
                    break;
            }

        }
    }
}