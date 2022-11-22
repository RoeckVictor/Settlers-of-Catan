using System;
using Noyau.Controller;
using Util.View;

namespace Noyau.View
{
    /// <summary>
    /// <para>La classe GameView</para>
    /// <para>Contient l'ensemble des méthodes et évènements nécessaire à la communication entre le noyau et l'interface graphique</para>
    /// </summary>
    public class GameView
    {
        /* 
          Contient les evenements et interfaces publiques
          
          * Pour emettre un evenement:
          Appeler la methode GameView.Instance.On[EventName](); 
          et lui passer en parametre un objet EventArgs correspondant et initialise avec les bons parametres
          Exemple:
          GameView.Instance.OnEndPhase();
          (La classe est un singleton, elle est donc instanciee automatiquement de maniere unique,
           GameView.Instance permet de recuperer l'instance de celle-ci)

          *  Pour reagir a un evenement:
          Creer une methode correspondant avec un prototype de ce genre:
          public void MonEventHandler(object sender, MonEventArgs e)
          { // Gerer l'evenement en fonction des EventArgs}
          MonEventArgs doit correspondre au parametre defini dans cette classe pour l'event en question (type visible dans la methode OnMyEvent(...)
          Ensuite, enregistrer cette methode afin qu'elle soit appelee des que l'event est emis:
          Exemple:
          GameView.Instance.MonEvent += MonEventHandler;
        */

        /// <value>Instance de GameViews</value>
        public static GameView Instance { get { return gameView.Value; } }
        /// <value>Renvoie une game contenue dans le dictionnaire de partie pour le mode serveur</value>
        public IGame GetGame(Guid guid)
        {
            if (controller.Games.ContainsKey(guid))
                return controller.Games[guid];
            else return null;
        }
        /// <value>Instance de Game pour le mode local</value>
        public IGame Game { get; set; }
        /// <value>Grille de jeu modifiable</value>
        public static IHexGrid DefaultGrid => GameController.DefaultGrid;

        // EVENEMENTS
        // Evenements recus par le Noyau

        // Termine la phase de jeu actuelle. Passe au tour suivant si en derniere phase (Construction)
        public event EventHandler<BaseEventArgs> EndPhase;
        public virtual void OnEndPhase(BaseEventArgs e) => EndPhase?.Invoke(this, e);
        // Demande la creation d'une partie. Un evenement de confirmation de creation est renvoye par la suite,
        // contenant l'index de la partie dans la liste des parties en cours (toujours Guid.Empty en mode local)
        public event EventHandler<GameCreateEventArgs> GameCreate;
        public virtual void OnGameCreate(GameCreateEventArgs e) => GameCreate?.Invoke(this, e);
        // Demande la suppression d'une partie. Un evenement de confirmation de suppression est renvoye par la suite contenant un booléen.
        public event EventHandler<GameDeleteEventArgs> GameDelete;
        public virtual void OnGameDelete(GameDeleteEventArgs e) => GameDelete?.Invoke(this, e);
        // Demande la construction d'un batiment par un joueur. A utiliser en phase de construction
        public event EventHandler<ConstructEventArgs> Construct;
        public virtual void OnConstruct(ConstructEventArgs e) => Construct?.Invoke(this, e);
        // Demande un echange entre 2 joueurs. A utiliser en phase d'echange
        public event EventHandler<PlayerExchangeEventArgs> PlayerExchange;
        public virtual void OnPlayerExchange(PlayerExchangeEventArgs e) => PlayerExchange?.Invoke(this, e);
        // Demande un echange avec un port (par defaut, general ou specialise). A utiliser en phase d'echange
        // Necessite une colonie/ville sur un emplacement approprie dans les 2 derniers cas
        public event EventHandler<HarborExchangeEventArgs> HarborExchange;
        public virtual void OnHarborExchange(HarborExchangeEventArgs e) => HarborExchange?.Invoke(this, e);
        // Utilisation d'une carte chevalier par le joueur
        public event EventHandler<BanditMoveEventArgs> KnightCardUse;
        public virtual void OnKnightCardUse(BanditMoveEventArgs e) => KnightCardUse?.Invoke(this, e);
        // Utilisation d'une carte chevalier par le joueur
        public event EventHandler<BanditMoveEventArgs> BanditMove;
        public virtual void OnBanditMove(BanditMoveEventArgs e) => BanditMove?.Invoke(this, e);
        // Utilisation d'une carte Monopole par le joueur
        public event EventHandler<MonopolyCardUseEventArgs> MonopolyCardUse;
        public virtual void OnMonopolyCardUse(MonopolyCardUseEventArgs e) => MonopolyCardUse?.Invoke(this, e);
        // Utilisation d'une carte Paire de ressource par le joueur
        public event EventHandler<ResourcePairCardUseEventArgs> ResourcePairCardUse;
        public virtual void OnResourcePairCardUse(ResourcePairCardUseEventArgs e) => ResourcePairCardUse?.Invoke(this, e);
        // Utilisation d'une carte construction de route par le joueur
        public event EventHandler<RoadConstructionCardUseEventArgs> RoadConstructionCardUse;
        public virtual void OnRoadConstructionCardUse(RoadConstructionCardUseEventArgs e) => RoadConstructionCardUse?.Invoke(this, e);
        // Lance un de
        public event EventHandler<BaseEventArgs> DiceThrow;
        public virtual void OnDiceThrow(BaseEventArgs e) => DiceThrow?.Invoke(this, e);
        //Defausse de cartes pour joueur au dessus de 7 lors d'un jet a 7
        public event EventHandler<DiscardEventArgs> DiscardExtraRessources;
        public virtual void OnDiscardExtraRessources(DiscardEventArgs e) => DiscardExtraRessources?.Invoke(this, e);
        // Constructions initiales
        public event EventHandler<InitialConstructEventArgs> InitialColony;
        public virtual void OnInitialColony(InitialConstructEventArgs e) => InitialColony?.Invoke(this, e);

        public event EventHandler<BaseEventArgs> FirstPlayerAI;
        public virtual void OnFirstPlayerAI(BaseEventArgs e) => FirstPlayerAI?.Invoke(this, e);

        public event EventHandler<InitialConstructEventArgs> InitialRoad;
        public virtual void OnInitialRoad(InitialConstructEventArgs e) => InitialRoad?.Invoke(this, e);

        // Evenements Noyau => Exterieur

        // Demande la creation d'une partie. Un evenement de confirmation de creation est renvoye par la suite,
        // contenant l'index de la partie dans la liste des parties en cours (toujours 0 en mode local)
        public event EventHandler<GameStatusArgs> GameBegin;
        public virtual void OnGameBegin(GameStatusArgs e)
        {
            Game = e.Game;
            GameBegin?.Invoke(this, e);
        }

        // Très sûrement à modifier par la suite
        public event EventHandler<GameDeletedArgs> GameDeleted;
        public virtual void OnGameDeleted(GameDeletedArgs e)
        {
            GameDeleted?.Invoke(this, e);
        }
        // Notifie le debut du premier round de la phase de construction initiale en debut de partie
        public event EventHandler<GameStatusArgs> InitialConstructionFirstRound;
        public virtual void OnInitialConstructionFirstRound(GameStatusArgs e)
        {
            Game = e.Game;
            InitialConstructionFirstRound?.Invoke(this, e);
        }
        // Notifie le debut du second round de la phase de construction initiale en debut de partie
        public event EventHandler<GameStatusArgs> InitialConstructionSecondRound;
        public virtual void OnInitialConstructionSecondRound(GameStatusArgs e)
        {
            Game = e.Game;
            InitialConstructionSecondRound?.Invoke(this, e);
        }
        // Notifie le debut de la phase de recolte 
        //(si un jet de 7 a ete fait le jeu passera ensuite immediatement en phase deplacement du bandit)
        public event EventHandler<GameStatusArgs> HarvestPhaseBegin;
        public virtual void OnHarvestPhaseBegin(GameStatusArgs e)
        {
            Game = e.Game;
            HarvestPhaseBegin?.Invoke(this, e);
        }
        //// Notifie le début de la phase de défausse
        public event EventHandler<GameStatusArgs> DiscardPhaseBegin;
        public virtual void OnDiscardPhaseBegin(DiceResultsInfoArgs e)
        {
            Game = e.Game;
            DiscardPhaseBegin?.Invoke(this, e);
        }
        //// Notifie le debut de la phase de deplacement du bandit
        public event EventHandler<GameStatusArgs> BanditMoveBegin;
        public virtual void OnBanditMoveBegin(GameStatusArgs e)
        {
            Game = e.Game;
            BanditMoveBegin?.Invoke(this, e);
        }
        // Notifie le debut de la phase d'echange
        public event EventHandler<DiceResultsInfoArgs> ExchangePhaseBegin;
        public virtual void OnExchangePhaseBegin(DiceResultsInfoArgs e)
        {
            Game = e.Game;
            ExchangePhaseBegin?.Invoke(this, e);
        }
        // Debut de la phase de construction du tour
        public event EventHandler<GameStatusArgs> ConstructionPhaseBegin;
        public virtual void OnConstructionPhaseBegin(GameStatusArgs e)
        {
            Game = e.Game;
            ConstructionPhaseBegin?.Invoke(this, e);
        }
        // Apres une construction effectuee avec succes(!= Fin de la phase construction)
        public event EventHandler<ActionDoneInfoArgs> ConstructionDone;
        public virtual void OnConstructionDone(ActionDoneInfoArgs e)
        {
            Game = e.Game;
            ConstructionDone?.Invoke(this, e);
        }
        // Apres un echange fini
        public event EventHandler<ActionDoneInfoArgs> ExchangeDone;
        public virtual void OnExchangeDone(ActionDoneInfoArgs e)
        {
            Game = e.Game;
            ExchangeDone?.Invoke(this, e);
        }
        // Apres une carte utilisee
        public event EventHandler<ActionDoneInfoArgs> CardUsageDone;
        public virtual void OnCardUsageDone(ActionDoneInfoArgs e)
        {
            Game = e.Game;
            CardUsageDone?.Invoke(this, e);
        }
        // Apres qu'un joueur ait atteint la condition de victoire
        public event EventHandler<VictoryInfoArgs> Victory;
        public virtual void OnVictory(VictoryInfoArgs e)
        {
            Game = e.Game;
            Victory?.Invoke(this, e);
        }

        // Implementation interne (Non utilise par les autres modules)
        private static readonly Lazy<GameView> gameView = new Lazy<GameView>(() => new GameView());
        private GameController controller;

        private GameView()
        {
            controller = new GameController();
            // En mode local c'est le controleur qui doit reagir aux evenements, sinon le module reseau
        }

        public void RegisterControllerHandlers()
        {
            controller.RegisterEventHandlers();
        }

        public void UnregisterControllerHandlers()
        {
            controller.UnregisterEventHandlers();
        }
    }
}
