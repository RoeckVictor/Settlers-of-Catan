using System;
using System.Collections.Generic;
using Noyau.View;
using AI.View;
using static Util.View.Serialization;

namespace Util.View
{
    /// <summary>
    /// <para>Class Event</para>
    /// <para>Contient les informations de base de l'évenement, à savoir s'il est destiné à etre envoyé au serveur ou juste en partie locale</para>
    /// </summary>

    [Serializable]
    public abstract class Event : EventArgs
    {
        public string Name { get; set; }
        public bool Broadcast { get; set; }
        public bool IsOnline { get; set; }


        public Event() { Name = this.ToString(); Broadcast = false; }
        public Event(bool isOnline, bool isBroadcast)
        {
            Name = this.ToString();
            Broadcast = isBroadcast;
            IsOnline = isOnline;
        }
    }

    /// <summary>
    /// <para>Class ConnectEventArgs</para>
    /// <para>Déclenché si une connexion au serveur est souhaitée</para>
    /// </summary>
    public class ConnectEventArgs : Event
    {
        public ConnectEventArgs() : base(true, false) { }
    }

    /// <summary>
    /// <para>Class DiconnectEventArgs</para>
    /// <para>Déclenché si une déconnexion du serveur est souhaitée</para>
    /// </summary>
    public class DisconnectEventArgs : Event
    {
        public Guid GameId;

        public int PlayerId { get; set; }
        public DisconnectEventArgs() : base(true, true) { }
        public DisconnectEventArgs(Guid gameId, int idp) : base(true, true)
        {
            GameId = gameId;
            PlayerId = idp;
        }

    }

    /// <summary>
    /// <para>Class RegisterEventArgs</para>
    /// <para>Utillisé lorsqu'un client veut s'enregistrer en BDD du serveur</para>
    /// </summary>
    public class RegisterEventArgs : Event
    {
        public string PlayerName;
        public string Password;
        public string Login;
        public RegisterEventArgs() : base(true, false) { }

        public RegisterEventArgs(string playerName, string password, string login) : base(true, false)
        {
            PlayerName = playerName;
            Password = password;
            Login = login;
        }
    }

    /// <summary>
    /// <para>Class GameCreate</para>
    /// <para>Contient les informations nécessaire à la création de partie en ligne</para>
    /// </summary>

    [Serializable]
    public class GameCreate : Event
    {
        public int TotalPlayerNumber { get; set; }
        public int IANumber { get; set; }
        public string HexGrid { get; set; }
        public string GroupName { get; set; }
        public bool Access { get; set; }
        public string Password { get; set; }
        public Guid GroupId { get; set; }
        public int AIDifficulty { get; set; }

        public int Avatar { get; set; }
        public string playerName { get; set; }

        public GameCreate() { }
        public GameCreate(int totalPlayerNumber, int iaNumber, string hexGrid, bool isOnline, string groupName, bool access, string password, Guid groupId, int avatar, string name, int aiDifficulty = 2) : base(isOnline, false)
        {
            TotalPlayerNumber = totalPlayerNumber;
            IANumber = iaNumber;
            HexGrid = hexGrid;
            IsOnline = isOnline;
            GroupName = groupName;
            Access = access;
            Password = password;
            GroupId = groupId;
            Avatar = avatar;
            playerName = name;
            AIDifficulty = aiDifficulty;
        }
    }


    // Parametres des evenements Exterieur => Noyau

    /// <summary>
    /// <para>Class GameCreateEventArgs</para>
    /// <para>Contient les informations nécessaire à la création de partie</para>
    /// </summary>
    public class GameCreateEventArgs : Event
    {
        public int TotalPlayerNumber { get; set; }
        public int IANumber { get; set; }
        public IHexGrid Grid { get; set; }
        public string GroupName { get; set; }
        public bool Access { get; set; }
        public string Password { get; set; }
        public Guid GroupId { get; set; }

        public int Avatar { get; set; }
        public string playerName { get; set; }
        public int AIDifficulty { get; set; }

        public GameCreateEventArgs() { }
        public GameCreateEventArgs(int totalPlayerNumber, int iaNumber, IHexGrid grid, bool isOnline, string groupName, bool access, string password, Guid groupId, int avatar, string name, int aiDifficulty = 2) : base(isOnline, false)
        {
            TotalPlayerNumber = totalPlayerNumber;
            IANumber = iaNumber;
            Grid = grid;
            IsOnline = isOnline;
            GroupName = groupName;
            Access = access;
            Password = password;
            GroupId = groupId;
            Avatar = avatar;
            playerName = name;
            AIDifficulty = aiDifficulty;
        }
    }

    /// <summary>
    /// <para>Class GameJoinEventArgs</para>
    /// <para>Contient les informations nécessaire pour rejoindre une partie</para>
    /// </summary>
    public class GameJoinEventArgs : Event
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public bool Random { get; set; }
        public Guid GameId { get; set; }
        public int Avatar { get; set; }
        public string playerName { get; set; }



        public GameJoinEventArgs() : base() { }
        public GameJoinEventArgs(int id, string password, Guid gameId, bool random, int avatar, string name) : base(true, false)
        {
            Id = id;
            Password = password;
            Random = random;
            GameId = gameId;
            Avatar = avatar;
            playerName = name;
        }
    }

    /// <summary>
    /// <para>Class BaseEventArgs</para>
    /// <para>Contient le minimum d'information nécessaire pour les events, utilisé par les autres eventArgs.</para>
    /// </summary>

    public class BaseEventArgs : Event
    {
        public Guid GameId { get; set; }
        public string FunctionName { get; set; }
        public BaseEventArgs() { }
        public BaseEventArgs(Guid gameId, bool isOnline) : base(isOnline, true)
        {
            GameId = gameId;
            FunctionName = "";
        }

        public BaseEventArgs(Guid gameId, bool isOnline, string name) : base(isOnline, true)
        {
            GameId = gameId;
            FunctionName = name;
        }

    }

    /// <summary>
    /// <para>Class GameDeletEventArgs</para>
    /// <para>Contient les informations nécessaire à la suppression de partie</para>
    /// </summary>
    public class GameDeleteEventArgs : BaseEventArgs
    {
        public GameDeleteEventArgs() { }
        public GameDeleteEventArgs(Guid gameId, bool isOnline) : base(gameId, isOnline) { Broadcast = false; } //  no broadcast
    }

    /// <summary>
    /// <para>Class ConstructEventArgs</para>
    /// <para>Contient les informations nécessaire à la création d'aménagement</para>
    /// </summary>
    public class ConstructEventArgs : BaseEventArgs
    {
        public ConstructionType ConstructionType { get; set; }
        public Coordinate ConstructionLocation { get; set; }

        public ConstructEventArgs() { }
        public ConstructEventArgs(Guid gameId, ConstructionType cType, Coordinate location, bool isOnline) : base(gameId, isOnline)
        {
            ConstructionType = cType;
            ConstructionLocation = location;
        }
    }

    /// <summary>
    /// <para>Class InitialConstructEventArgs</para>
    /// <para>Contient les informations nécessaire à la création d'aménagements intial</para>
    /// </summary>
    public class InitialConstructEventArgs : BaseEventArgs
    {
        public Coordinate BuildingCoordinate { get; set; }

        public InitialConstructEventArgs() { }
        public InitialConstructEventArgs(Guid gameId, Coordinate bLocation, bool isOnline) : base(gameId, isOnline)
        {
            BuildingCoordinate = bLocation;
        }
    }


    /// <summary>
    /// <para>Class PlayerExchangeEventArgs</para>
    /// <para>Contient les informations nécessaire à l'échange entre deux joueurs</para>
    /// </summary>
    public class PlayerExchangeEventArgs : BaseEventArgs
    {
        public IList<(RessourceType t, int num)> GiftedRessources { get; set; }
        public int OtherPlayerId { get; set; }
        public IList<(RessourceType t, int num)> ReceivedRessources { get; set; }

        public PlayerExchangeEventArgs() { }
        public PlayerExchangeEventArgs(Guid gameId, IList<(RessourceType t, int num)> giftedRessources, int otherPlayerId, IList<(RessourceType t, int num)> receivedRessources, bool isOnline) : base(gameId, isOnline)
        {
            GiftedRessources = giftedRessources;
            OtherPlayerId = otherPlayerId;
            ReceivedRessources = receivedRessources;
        }
    }

    /// <summary>
    /// <para>Class HarborExchangeEventArgs</para>
    /// <para>Contient les informations nécessaire à l'échange avec les ports</para>
    /// </summary>
    public class HarborExchangeEventArgs : BaseEventArgs
    {
        public HarborType Harbor { get; set; }
        public RessourceType RessourceToGive { get; set; }
        public RessourceType RessourceToReceive { get; set; }

        public HarborExchangeEventArgs() { }
        public HarborExchangeEventArgs(Guid gameId, HarborType harbor, RessourceType ressourceToGive, RessourceType ressourceToReceive, bool isOnline) : base(gameId, isOnline)
        {
            Harbor = harbor;
            RessourceToGive = ressourceToGive;
            RessourceToReceive = ressourceToReceive;
        }
    }


    /// <summary>
    /// <para>Class BanditMoveEventArgs</para>
    /// <para>Contient les informations nécessaire au déplacement des brigands</para>
    /// </summary>
    public class BanditMoveEventArgs : BaseEventArgs
    {
        public Coordinate NewThiefLocation { get; set; }
        public int PlayerToStealFrom { get; set; }

        public BanditMoveEventArgs() { }
        public BanditMoveEventArgs(Guid gameId, Coordinate newThiefLocation, int playerToStealFrom, bool isOnline) : base(gameId, isOnline)
        {
            NewThiefLocation = newThiefLocation;
            PlayerToStealFrom = playerToStealFrom;
        }
    }
    /// <summary>
    /// <para>Class MonopolyCardUseEventArgs</para>
    /// <para>Contient les informations nécessaire à l'utilisation de la carte monopôle</para>
    /// </summary>
    public class MonopolyCardUseEventArgs : BaseEventArgs
    {
        public RessourceType RessourceToMonopolize { get; set; }

        public MonopolyCardUseEventArgs() { }
        public MonopolyCardUseEventArgs(Guid gameId, RessourceType ressourceToMonopolize, bool isOnline) : base(gameId, isOnline)
            => RessourceToMonopolize = ressourceToMonopolize;
    }

    /// <summary>
    /// <para>Class ResourcePairCardUseEventArgs</para>
    /// <para>Contient les informations nécessaire à l'utilisation de la carte récupération de ressources</para>
    /// </summary>
    public class ResourcePairCardUseEventArgs : BaseEventArgs
    {
        public RessourceType RessourceToGet1 { get; set; }
        public RessourceType RessourceToGet2 { get; set; }

        public ResourcePairCardUseEventArgs() { }
        public ResourcePairCardUseEventArgs(Guid gameId, RessourceType rToGet1, RessourceType rToGet2, bool isOnline) : base(gameId, isOnline)
        {
            RessourceToGet1 = rToGet1;
            RessourceToGet2 = rToGet2;
        }
    }

    /// <summary>
    /// <para>Class RoadConstructionCardUseEventArgs</para>
    /// <para>Contient les informations nécessaire à l'utilisation de la carte construction de routes </para>
    /// </summary>
    public class RoadConstructionCardUseEventArgs : BaseEventArgs
    {
        public Coordinate FirstRoadLocation { get; set; }
        public Coordinate SecondRoadLocation { get; set; }

        public RoadConstructionCardUseEventArgs() { }
        public RoadConstructionCardUseEventArgs(Guid gameId, Coordinate firstRoadLoc, Coordinate secondRoadLoc, bool isOnline) : base(gameId, isOnline)
        {
            FirstRoadLocation = firstRoadLoc;
            SecondRoadLocation = secondRoadLoc;
        }
    }

    /// <summary>
    /// <para>Class DiscardEventArgs</para>
    /// <para>Contient les informations nécessaire à la défausse de carte (lorsque le dés affiche 7)</para>
    /// </summary>
    public class DiscardEventArgs : BaseEventArgs
    {
        public List<(int idPlayer, List<(RessourceType rType, int num)>)> Discards;

        public DiscardEventArgs() { }
        public DiscardEventArgs(Guid gameId, List<(int idPlayer, List<(RessourceType rType, int num)>)> discards, bool isOnline) : base(gameId, isOnline)
        {
            Discards = discards;
        }
    }

    /// <summary>
    /// <para>Class DiscardEvent</para>
    /// <para>Contient les informations nécessaire à la défausse de carte (lorsque le dés affiche 7) en ligne</para>
    /// </summary>
    public class DiscardEvent : BaseEventArgs
    {
        public string list;

        public int IdPlayer;

        public DiscardEvent() { }
        public DiscardEvent(Guid gameId, string discards, int idp) : base(gameId, true)
        {
            list = discards;
            IdPlayer = idp;
        }
    }


    // Parametres des evenements Noyau => Exterieur

    /// <summary>
    /// <para>Class GamestatusEventArgs</para>
    /// <para>Contient le status de la partie </para>
    /// </summary>
    public class GameStatusArgs : Event
    {
        public Guid GameId { get; set; }
        public IGame Game { get; set; }
        public int AIDifficulty { get; set; }

        public GameStatusArgs() { }
        public GameStatusArgs(Guid gameId, IGame game, bool isOnline, int aiDifficulty = 2) : base(isOnline, true)
        {
            GameId = gameId;
            Game = game;
            AIDifficulty = aiDifficulty;
        }
    }

    /// <summary>
    /// <para>Class GamestatusArgsNetwork</para>
    /// <para>Contient le status de la partie reçu depuis le réseau</para>
    /// </summary>
    public class GameStatusArgsNetwork : Event
    {
        public Guid GameId { get; set; }
        public string Game { get; set; }
        public int NbIA { get; set; }
        public string FunctionName { get; set; }

        public GameStatusArgsNetwork() { }
        public GameStatusArgsNetwork(Guid gameId, string game, bool isOnline, int nbia, string name) : base(isOnline, true)
        {
            GameId = gameId;
            Game = game;
            NbIA = nbia;
            FunctionName = name;
        }
    }



    /// <summary>
    /// <para>Class GameDeletedArgs</para>
    /// <para>Contient les informations renvoyé par l'évènement de suppression de partie </para>
    /// </summary>
    public class GameDeletedArgs : Event
    {
        public Guid GameId { get; set; }
        public bool isSuccess { get; set; }

        public GameDeletedArgs() { }
        public GameDeletedArgs(Guid gameId, bool success, bool isOnline) : base(isOnline, false)
        {
            GameId = gameId;
            isSuccess = success;
        }
    }
    /// <summary>
    /// <para>Class DiceResultsInfoArgs</para>
    /// <para>Contient les informations d'un lancé de dés</para>
    /// </summary>
    public class DiceResultsInfoArgs : GameStatusArgs
    {
        public (int, int) DiceRollResult { get; set; }

        public DiceResultsInfoArgs() { }
        public DiceResultsInfoArgs(Guid gameId, IGame game, (int, int) diceRoll, bool isOnline) : base(gameId, game, isOnline)
        {
            DiceRollResult = diceRoll;
        }
    }

    /// <summary>
    /// <para>Class DiceResultsInfoArgsNetwork</para>
    /// <para>Contient les informations d'un lancé de dés en mode réseau</para>
    /// </summary>
    public class DiceResultsInfoArgsNetwork
    {
        public Guid GameId { get; set; }
        public (int, int) DiceRollResult { get; set; }

        public string Game { get; set; }

        public string Name { get; set; }
        public string FunctionName { get; set; }

        public DiceResultsInfoArgsNetwork() { }
        public DiceResultsInfoArgsNetwork(Guid gameId, string game, (int, int) diceRoll, string func)
        {
            Name = this.ToString();
            GameId = gameId;
            DiceRollResult = diceRoll;
            Game = game;
            FunctionName = func;
        }
    }
    /// <summary>
    /// <para>Class ActionDoneInfoArgs</para>
    /// <para>Indique si une action a été un succès ou un echec</para>
    /// </summary>
    public class ActionDoneInfoArgs : GameStatusArgs
    {
        public bool ActionSuccessful { get; set; }
        public ActionType Action { get; set; }
        public int PlayerID { get; set; }
        public RessourceType Resource1 { get; set; }
        public RessourceType Resource2 { get; set; }
        public ActionDoneInfoArgs() { }
        public ActionDoneInfoArgs(Guid gameId, IGame game, ActionType action, bool actionSuccessful, int playerID, RessourceType resource1, RessourceType resource2, bool isOnline) : base(gameId, game, isOnline)
        {
            ActionSuccessful = actionSuccessful;
            Action = action;
            PlayerID = playerID;
            Resource1 = resource1;
            Resource2 = resource2;
        }
    }

    /// <summary>
    /// <para>Class ActionDoneInfoArgs</para>
    /// <para>Indique si une action a été un succès ou un echec</para>
    /// </summary>
    public class ActionDoneInfoArgsNetwork : GameStatusArgs
    {
        public bool ActionSuccessful { get; set; }
        public ActionType Action { get; set; }
        public int PlayerID { get; set; }
        public RessourceType Resource1 { get; set; }
        public RessourceType Resource2 { get; set; }
        public string FunctionName { get; set; }
        public ActionDoneInfoArgsNetwork() { }
        public ActionDoneInfoArgsNetwork(Guid gameId, IGame game, ActionType action, bool actionSuccessful, int playerID, RessourceType resource1, RessourceType resource2, bool isOnline, string name) : base(gameId, game, isOnline)
        {
            ActionSuccessful = actionSuccessful;
            Action = action;
            PlayerID = playerID;
            Resource1 = resource1;
            Resource2 = resource2;
            FunctionName = name;
        }
    }



    /// <summary>
    /// <para>Class ActionDoneInfoArgs</para>
    /// <para>Indique si une action a été un succès ou un echec</para>
    /// </summary>
    public class ActionDoneNetwork
    {
        public string Name { get; set; }
        public Guid GameId { get; set; }
        public bool ActionSuccessful { get; set; }
        public string Action { get; set; }
        public int PlayerID { get; set; }
        public string Resource1 { get; set; }
        public string Resource2 { get; set; }
        public string FunctionName { get; set; }

        public string Game { get; set; }
        public ActionDoneNetwork() { }
        public ActionDoneNetwork(string n, Guid gameId, string action, bool actionSuccessful, int playerID, string resource1, string resource2, string name, string grid)
        {
            Name = n;
            ActionSuccessful = actionSuccessful;
            Action = action;
            PlayerID = playerID;
            Resource1 = resource1;
            Resource2 = resource2;
            FunctionName = name;
            GameId = gameId;
            Game = grid;
        }
    }

    /// <summary>
    /// <para>Class VictoryInfoArgs</para>
    /// <para>Indique si une action a été un succès ou un echec</para>
    /// </summary>
    public class VictoryInfoArgs : GameStatusArgs
    {
        // Liste de tuples (indexJoueur, pointsJoueur) classes par nombre de points descendants
        public List<(int, int)> PlayerRankings { get; set; }

        public VictoryInfoArgs(Guid gameId, IGame game, List<(int, int)> playerRankings, bool isOnline) : base(gameId, game, isOnline)
        {
            PlayerRankings = playerRankings;
        }
    }

    /// <summary>
    /// <para>Class Gamestatus</para>
    /// <para>Contient le status de la partie reçu depuis le réseau</para>
    /// </summary>
    public class GameStatus : Event
    {
        public Guid GameId { get; set; }
        public string TheSerializedGame { get; set; }

        public int IaNumber { get; set; }

        public GameStatus() { }
        public GameStatus(Guid gameId, string game, int iaNumber, bool isOnline) : base(isOnline, true)
        {
            GameId = gameId;
            TheSerializedGame = game;
            IaNumber = iaNumber;
        }
    }

    /// <summary>
    /// <para>Class VictoryInfo</para>
    /// <para>Indique si une action a été un succès ou un echec en ligne</para>
    /// </summary>

    public class VictoryInfo : GameStatus
    {
        // Liste de tuples (indexJoueur, pointsJoueur) classes par nombre de points descendants
        public string PlayerRankings { get; set; }

        public VictoryInfo(Guid gameId, string game, string playerRankings, int iaNum, bool isOnline) : base(gameId, game, iaNum, isOnline)
        {
            PlayerRankings = playerRankings;
            Broadcast = true;
        }
        public VictoryInfo() { Broadcast = true; }
    }

    /// <summary>
    /// <para>Class ErrorEventArgs</para>
    /// <para>Indique s'il y a eu erreur depuis le serveur</para>
    /// </summary>
    public class ErrorEventArgs : Event
    {
        public ErrorType Type { get; set; }

        public string Message { get; set; }

        public ErrorEventArgs() { }
        public ErrorEventArgs(ErrorType errorType, string message)
        {
            Type = errorType;
            Message = message;
        }
    }

    /// <summary>
    /// <para>Class PlayerJoinGame</para>
    /// <para>Indique qu'un joueur a rejont la partie en cours</para>
    /// </summary>
    public class PlayerJoinedGame : BaseEventArgs
    {
        public string PlayerName { get; set; }
        public int Id { get; set; }

        public PlayerJoinedGame() { }
        public PlayerJoinedGame(string name, int id, Guid idGame) : base(idGame, true)
        {
            PlayerName = name;
            Id = id;
        }
    }
    /// <summary>
    /// <para>Class ReadyEvent</para>
    /// <para>Indique qu'un joueur est prêt</para>
    /// </summary>
    public class ReadyEvent : BaseEventArgs
    {
        public bool ready { get; set; }

        public int IdInGame { get; set; }

        public ReadyEvent() { Broadcast = true; }
        public ReadyEvent(bool isReady, Guid id, int idInGame) : base(id, true)
        {
            ready = isReady;
            Broadcast = true;
            IdInGame = idInGame;
        }
    }
    /// <summary>
    /// <para>Class StartedGame</para>
    /// <para>Indique que le jeu a commencé</para>
    /// </summary>
    public class StartedGame : BaseEventArgs
    {

        public int IdInGame { get; set; }
        public StartedGame() { Broadcast = true; IdInGame = 0; }
        public StartedGame(Guid id, int idg) : base(id, true) { Broadcast = true; IdInGame = idg; }
    }

    /// <summary>
    /// <para>Class MessageEvent</para>
    /// <para>Utilisé pour la réception de messages dans le chat en ligne</para>
    /// </summary>
    public class MessageEvent : BaseEventArgs
    {
        public string Message { get; set; }
        public int idSender { get; set; }


        public MessageEvent() { Broadcast = true; }
        public MessageEvent(string msg, int idsender, Guid id) : base(id, true)
        {
            Message = msg;
            idSender = idsender;
            Broadcast = true;
        }
    }

    /// <summary>
    /// <para>Class ExchangeEvent</para>
    /// <para>Contient les informations nécessaire à l'échange entre deux joueurs en ligne</para>
    /// </summary>
    public class ExchangeEvent : BaseEventArgs
    {

        public string GifteRessources { get; set; } // 5 cases 
        public int PlayerId { get; set; }

        public ExchangeEvent() { Broadcast = true; }
        public ExchangeEvent(string gifted, Guid id, int idp) : base(id, true)
        {
            GifteRessources = gifted;
            Broadcast = true;
            PlayerId = idp;
        }
    }

    /// <summary>
    /// <para>Class AcceptExchange</para>
    /// <para>Indique si l'échange a été validé par les 2 joueurs concernés en ligne</para>
    /// </summary>
    public class AcceptExchange : BaseEventArgs
    {
        public string GifteRessources { get; set; }
        public string RecievedRessources { get; set; }
        public int OtherPlayerId { get; set; }

        public AcceptExchange() { Broadcast = true; }

        public AcceptExchange(string gifted, int other, string recieved, Guid id) : base(id, true)
        {
            GifteRessources = gifted;
            OtherPlayerId = other;
            RecievedRessources = recieved;
            Broadcast = true;
        }
    }

    /// <summary>
    /// <para>Class InfoGame</para>
    /// <para>Contient les informations nde l'état du jeu (map, joueurs)</para>
    /// </summary>
    public class InfoGame : Event
    {
        public Guid IdGame;
        public int IdInGame;
        public GameLine game;
        public string infoPlayers;
        public string infoAvatar;
        public bool First;

        public InfoGame() { game = new GameLine(); Broadcast = true; First = true; }
        public InfoGame(Guid id, int idp, GameLine gm, string info, string infoa, bool first) : base(true, false)
        {
            game = new GameLine();
            IdGame = id;
            IdInGame = idp;
            infoPlayers = info;
            game = gm;
            Broadcast = true;
            infoAvatar = infoa;
            First = first;
        }
    }

    /// <summary>
    /// <para>Class UpdateEvent</para>
    /// <para>Met à jour les informations des joueurs</para>
    /// </summary>
    public class UpdateEvent
    {
        public GameLine game;
        public List<(int, string)> infoPlayers;
        public List<(int, int)> infoAvatar;

        public UpdateEvent()
        {
            game = new GameLine();
            infoPlayers = new List<(int, string)>();
            infoAvatar = new List<(int, int)>();
        }
        public UpdateEvent(GameLine g, List<(int, string)> players, List<(int, int)> avatar)
        {
            game = new GameLine();
            infoPlayers = new List<(int, string)>();
            infoAvatar = new List<(int, int)>();

            game = g;
            infoPlayers = players;
            infoAvatar = avatar;
        }
    }

    public class QuitLobby
    {
        public string Name { get; set; }
        public Guid GameId { get; set; }
        public int PlayerId { get; set; }

        public QuitLobby() { Name = this.ToString(); }
        public QuitLobby(Guid idg, int idp)
        {
            GameId = idg;
            PlayerId = idp;
            Name = this.ToString();
        }
    }

    public class Refresh
    {
        public string Name { get; set; }
        public string Games { get; set; }
        public Refresh() { Name = this.ToString(); }
        public Refresh(string games)
        {
            Games = games;
            Name = this.ToString();
        }
    }

    public class StopGame
    {
        public string Name { get; set; }
        public StopGame() { Name = this.ToString(); }
    }

    public class TimeOut
    {
        public string Name { get; set; }
        public TimeOut() { Name = this.ToString(); }
    }
}
