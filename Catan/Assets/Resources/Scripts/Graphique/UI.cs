using System;
using Util.View;

namespace Assets.Resources.Scripts.Graphique
{
    public class UI
    {
        public static UI Instance { get { return gameView.Value; } }
        private static readonly Lazy<UI> gameView = new Lazy<UI>(() => new UI());
        private UIController controller;

        private UI()
        {
            //controller = new UIController();
            //controller = GameObject.Find("UIControllerReseau").GetComponent<UIController>();
        }

        public void RegisterControllerHandlers()
        {
            //controller.RegisterEventHandlers();
        }

        public event EventHandler<PlayerJoinedGame> PlayerJoined;
        public virtual void OnJoinPlayer(PlayerJoinedGame e) => PlayerJoined?.Invoke(this, e);


        public event EventHandler<StartedGame> Start;
        public virtual void OnStart(StartedGame e) => Start?.Invoke(this, e);

        public event EventHandler<UpdateEvent> Update;
        public virtual void OnUpdate(UpdateEvent e) => Update?.Invoke(this, e);


        public event EventHandler<ReadyEvent> Ready;
        public virtual void OnReady(ReadyEvent e) => Ready?.Invoke(this, e);


        public event EventHandler<MessageEvent> Message;
        public virtual void OnMessage(MessageEvent e) => Message?.Invoke(this, e);


        public event EventHandler<ErrorEventArgs> Error;
        public virtual void OnError(ErrorEventArgs e) => Error?.Invoke(this, e);


        public event EventHandler<ExchangeEvent> Exchange;
        public virtual void OnExchange(ExchangeEvent e) => Exchange?.Invoke(this, e);


        public event EventHandler<AcceptExchange> Accepted;
        public virtual void OnAcceptExchange(AcceptExchange e) => Accepted?.Invoke(this, e);


        public event EventHandler<GameStatusArgs> StatusReseau;
        public virtual void OnBeginReseau(GameStatusArgs e) => StatusReseau?.Invoke(this, e);


        public event EventHandler<GameStatusArgs> InitialConstructionFirstRound;
        public virtual void OnInitialConstructionFirstRoundReseau(GameStatusArgs e) => InitialConstructionFirstRound?.Invoke(this, e);

        public event EventHandler<GameStatusArgs> InitialConstructionSecondRound;
        public virtual void OnInitialConstructionSecondRoundReseau(GameStatusArgs e) => InitialConstructionSecondRound?.Invoke(this, e);


        public event EventHandler<GameStatusArgs> HarvestPhaseBegin;
        public virtual void OnHarvestPhaseBeginReseau(GameStatusArgs e) => HarvestPhaseBegin?.Invoke(this, e);


        public event EventHandler<GameStatusArgs> BanditMoveBegin;
        public virtual void OnBanditMoveBeginReseau(GameStatusArgs e) => BanditMoveBegin?.Invoke(this, e);


        public event EventHandler<GameStatusArgs> ConstructionPhaseBegin;
        public virtual void OnConstructionPhaseBeginReseau(GameStatusArgs e) => ConstructionPhaseBegin?.Invoke(this, e);

        public event EventHandler<GameStatusArgs> DiscardPhaseBegin;
        public virtual void OnDiscardPhaseBeginReseau(GameStatusArgs e) => DiscardPhaseBegin?.Invoke(this, e);



        public event EventHandler<ActionDoneInfoArgs> ConstructionDone;
        public virtual void OnConstructionDoneReseau(ActionDoneInfoArgs e) => ConstructionDone?.Invoke(this, e);


        public event EventHandler<ActionDoneInfoArgs> ExchangeDone;
        public virtual void OnExchangeDoneReseau(ActionDoneInfoArgs e) => ExchangeDone?.Invoke(this, e);


        public event EventHandler<ActionDoneInfoArgs> CardUsageDone;
        public virtual void OnCardUsageDoneReseau(ActionDoneInfoArgs e) => CardUsageDone?.Invoke(this, e);


        public event EventHandler<DiceResultsInfoArgs> ExchangePhaseBegin;
        public virtual void OnExchangePhaseBeginReseau(DiceResultsInfoArgs e) => ExchangePhaseBegin?.Invoke(this, e);



        public event EventHandler<DiceResultsInfoArgs> DiscardPhaseBeginReseau;
        public virtual void OnDiscardPhaseBeginReseau(DiceResultsInfoArgs e) => DiscardPhaseBeginReseau?.Invoke(this, e);



        public event EventHandler<VictoryInfoArgs> VictoryInfoReseau;
        public virtual void OnVictoryInfoReseau(VictoryInfoArgs e) => VictoryInfoReseau?.Invoke(this, e);


        public event EventHandler<TimeOut> Timeout;
        public virtual void OnTimeout(TimeOut e) => Timeout?.Invoke(this, e);


        public event EventHandler<Refresh> ERefresh;
        public virtual void OnRefresh(Refresh e) => ERefresh?.Invoke(this, e);

        public event EventHandler<QuitLobby> RetourArriere;
        public virtual void OnRetourArriere(QuitLobby e) => RetourArriere?.Invoke(this, e);



        public event EventHandler<GameDeletedArgs> Deleted;
        public virtual void OnDeleted(GameDeletedArgs e) => Deleted?.Invoke(this, e);


    }
}
