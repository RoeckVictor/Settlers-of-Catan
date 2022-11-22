using System;
using System.Collections.Generic;
using Util.View;
using Network.Controller;

namespace Network.View
{
    public class NetworkView
    {

        /// <summary>
        /// <para>Classe NetworkView</para>
        /// <para>Contient l'ensemble des évenements échangés avec l'interface graphique</para>
        /// </summary>
        public static NetworkView Instance { get { return netView.Value; } }
        private static readonly Lazy<NetworkView> netView = new Lazy<NetworkView>(() => new NetworkView());
        private ClientController controller;

        private NetworkView()
        {
            controller = new ClientController();
        }

        public void RegisterControllerHandlers()
        {
            controller = new ClientController();
            controller.RegisterEventHandlers();
        }

        /// <summary>
        /// Obtient la liste des jeux en ligne
        /// </summary>
        public List<GameLine> getGamesNetwork()
        {
            return controller.getGames();
        }
        /// <summary>
        /// Permet d'obtenir le jeu auquel le client est connecté
        /// </summary>
        public GameLine getActualGame()
        {
            return controller.getActualGame();
        }
        /// <summary>
        /// Les informations (logins) des joueurs de la partie auquelle appartient le client
        /// </summary>
        public List<(int, string)> getInfoPlayers()
        {
            return controller.getInfoPlayers();
        }

        /// <summary>
        /// Les informations (avatars) des joueurs de la partie auquelle appartient le client
        /// </summary>
        public List<(int, int)> getAvatar()
        {
            return controller.getAvatar();
        }
        /// <summary>
        /// Indique si c'est la première reception de l'état du jeu depuis le serveur
        /// </summary>
        public bool GetFirst()
        {
            return controller.GetFirst();
        }
        /// <summary>
        /// Obtient le jeu actuel
        /// </summary>
        /// <param name="id">l'identifiant du jeu en cours</param>
        public GameLine getGame(Guid id)
        {
            if (controller.getGames().Count != 0)
            {
                GameLine g = new GameLine();
                foreach (GameLine game in controller.getGames())
                {
                    if (game.Id == id) g = game;
                }
                return g;
            }
            return null;
        }

        /// <summary>
        /// Identifiant du jeu en cours
        /// </summary>
        public Guid getIdGame()
        {
            return controller.getIdGame();
        }
        /// <summary>
        /// Identifiant du joueur dans la partie
        /// </summary>
        public int getIdInGame()
        {
            return controller.getIdInGame();
        }

        /// Evenements reçus par le client
        public event EventHandler<ConnectEventArgs> ConnectEvent;
        public virtual void OnConnectEvent(ConnectEventArgs e) => ConnectEvent?.Invoke(this, e);


        public event EventHandler<DisconnectEventArgs> DisconnectEvent;
        public virtual void OnDisconnectEvent(DisconnectEventArgs e) => DisconnectEvent?.Invoke(this, e);


        public event EventHandler<ReadyEvent> Ready;
        public virtual void OnReadyEvent(ReadyEvent e) => Ready?.Invoke(this, e);



        public event EventHandler<StartedGame> Start;
        public virtual void OnStartEvent(StartedGame e) => Start?.Invoke(this, e);


        public event EventHandler<GameJoinEventArgs> JoinEvent;
        public virtual void OnJoinEvent(GameJoinEventArgs e) => JoinEvent?.Invoke(this, e);


        public event EventHandler<MessageEvent> Message;
        public virtual void OnMessageEvent(MessageEvent e) => Message?.Invoke(this, e);


        public event EventHandler<BaseEventArgs> EndPhase;
        public virtual void OnEndPhase(BaseEventArgs e) => EndPhase?.Invoke(this, e);


        public event EventHandler<GameCreateEventArgs> GameCreate;
        public virtual void OnGameCreate(GameCreateEventArgs e) => GameCreate?.Invoke(this, e);


        public event EventHandler<GameDeleteEventArgs> GameDelete;
        public virtual void OnGameDelete(GameDeleteEventArgs e) => GameDelete?.Invoke(this, e);


        public event EventHandler<ConstructEventArgs> Construct;
        public virtual void OnConstruct(ConstructEventArgs e) => Construct?.Invoke(this, e);


        public event EventHandler<ExchangeEvent> PlayerExchange;
        public virtual void OnPlayerExchange(ExchangeEvent e) => PlayerExchange?.Invoke(this, e);


        public event EventHandler<AcceptExchange> PlayerAcceptExchange;
        public virtual void OnAcceptExchange(AcceptExchange e) => PlayerAcceptExchange?.Invoke(this, e);


        public event EventHandler<HarborExchangeEventArgs> HarborExchange;
        public virtual void OnHarborExchange(HarborExchangeEventArgs e) => HarborExchange?.Invoke(this, e);


        public event EventHandler<BanditMoveEventArgs> KnightCardUse;
        public virtual void OnKnightCardUse(BanditMoveEventArgs e) => KnightCardUse?.Invoke(this, e);


        public event EventHandler<BanditMoveEventArgs> BanditMove;
        public virtual void OnBanditMove(BanditMoveEventArgs e) => BanditMove?.Invoke(this, e);


        public event EventHandler<MonopolyCardUseEventArgs> MonopolyCardUse;
        public virtual void OnMonopolyCardUse(MonopolyCardUseEventArgs e) => MonopolyCardUse?.Invoke(this, e);


        public event EventHandler<ResourcePairCardUseEventArgs> ResourcePairCardUse;
        public virtual void OnResourcePairCardUse(ResourcePairCardUseEventArgs e) => ResourcePairCardUse?.Invoke(this, e);


        public event EventHandler<RoadConstructionCardUseEventArgs> RoadConstructionCardUse;
        public virtual void OnRoadConstructionCardUse(RoadConstructionCardUseEventArgs e) => RoadConstructionCardUse?.Invoke(this, e);


        public event EventHandler<BaseEventArgs> DiceThrow;
        public virtual void OnDiceThrow(BaseEventArgs e) => DiceThrow?.Invoke(this, e);


        public event EventHandler<DiscardEvent> DiscardExtraRessources;
        public virtual void OnDiscardExtraRessources(DiscardEvent e) => DiscardExtraRessources?.Invoke(this, e);


        public event EventHandler<InitialConstructEventArgs> InitialColony;
        public virtual void OnInitialColony(InitialConstructEventArgs e) => InitialColony?.Invoke(this, e);


        public event EventHandler<InitialConstructEventArgs> InitialRoad;
        public virtual void OnInitialRoad(InitialConstructEventArgs e) => InitialRoad?.Invoke(this, e);


        public event EventHandler<QuitLobby> QuitLobby;
        public virtual void OnQuitLobby(QuitLobby e) => QuitLobby?.Invoke(this, e);

        public event EventHandler<Refresh> Refresh;
        public virtual void OnRefresh(Refresh e) => Refresh?.Invoke(this, e);





    }
}
