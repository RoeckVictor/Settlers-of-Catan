using System;
using Network.Controller;
using Util.View;
namespace Network.View
{
    class NetworkView
    {

        /// <summary>
        /// <para>La classe NetworkView qui set de vue pour le package Network</para>
        /// <para>Contient l'ensemble des méthodes et évènements nécessaire à la communication entre le réseau du côté serveur et le noyau</para>
        /// </summary>
        public static NetworkView Instance { get { return netView.Value; } }
        private static readonly Lazy<NetworkView> netView = new Lazy<NetworkView>(() => new NetworkView());
        private NetworkManager controller;
        private NetworkView()
        {
            controller = new NetworkManager();
        }

        public void RegisterControllerHandlers()
        {
            controller.RegisterEventHandlers();
            Console.WriteLine("Event handler of Network registred");
        }

        /// Evenements reçus par le noyau
        public event EventHandler<GameStatusArgsNetwork> GameStatus;
        public virtual void OnGameStatus(GameStatusArgsNetwork e) => GameStatus?.Invoke(this, e);

        public event EventHandler<GameStatusArgsNetwork> GameStatusCreate;
        public virtual void OnGameStatusCreate(GameStatusArgsNetwork e) => GameStatusCreate?.Invoke(this, e);



        public event EventHandler<GameDeletedArgs> Deleted;
        public virtual void OnDeleted(GameDeletedArgs e) => Deleted?.Invoke(this, e);


        public event EventHandler<DiceResultsInfoArgsNetwork> DiceResults;
        public virtual void OnDiceResultInfo(DiceResultsInfoArgsNetwork e) => DiceResults?.Invoke(this, e);


        public event EventHandler<ActionDoneInfoArgsNetwork> ActionDone;
        public virtual void OnActionDoneInfo(ActionDoneInfoArgsNetwork e) => ActionDone?.Invoke(this, e);


        public event EventHandler<VictoryInfoArgs> VictoryInfo;
        public virtual void OnVictoryInfo(VictoryInfoArgs e) => VictoryInfo?.Invoke(this, e);
    }
}
