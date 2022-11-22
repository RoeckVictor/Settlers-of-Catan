using System;
using Noyau.Controller;
using UnityEngine;
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

        /// <summary>
        /// Active la souscription aux evenements des autres modules par le noyau.
        /// </summary>
        public void RegisterControllerHandlers()
        {
            controller.RegisterEventHandlers();
        }

        /// <summary>
        /// Annule la souscription aux evenements des autres modules par le noyau.
        /// </summary>
        public void UnregisterControllerHandlers()
        {
            Debug.Log("Cette fonction ne devrait pas etre appelée en mode en ligne");
            controller.UnregisterEventHandlers();
        }

        // Evenements recus par le Noyau

        /// <summary>
        /// Termine la phase de jeu actuelle. Passe au tour suivant si en derniere phase (Construction)
        /// </summary>
        public event EventHandler<BaseEventArgs> EndPhase;
        /// <summary>
        /// Fonction d'appel de l'evenement EndPhase.
        /// Normalement appelee par le modules exterieurs au noyau.
        /// </summary>
        /// <param name="e">Donnees indiquant la partie devant passer a la phase suivante</param>
        public virtual void OnEndPhase(BaseEventArgs e) => EndPhase?.Invoke(this, e);
        /// <summary>
        ///  Demande la creation d'une partie. Un evenement de confirmation de creation est renvoye par la suite,
        ///  contenant l'index de la partie dans la liste des parties en cours (toujours Guid.Empty en mode local)
        /// </summary>
        public event EventHandler<GameCreateEventArgs> GameCreate;
        /// <summary>
        /// Fonction d'appel de l'evenement GameCreate.
        /// Normalement appelee par le modules exterieurs au noyau.
        /// </summary>
        /// <param name="e">Donnees des parametres de configuration de la partie</param>
        public virtual void OnGameCreate(GameCreateEventArgs e) => GameCreate?.Invoke(this, e);
        /// <summary>
        /// Demande la suppression d'une partie. 
        /// Un evenement de confirmation de suppression est renvoye par la suite contenant un booléen.
        /// </summary>
        public event EventHandler<GameDeleteEventArgs> GameDelete;
        /// <summary>
        /// Fonction d'appel de l'evenement GameDelete.
        /// Normalement appelee par le modules exterieurs au noyau.
        /// </summary>
        /// <param name="e">Donnees indiquant la partie a supprimer</param>
        public virtual void OnGameDelete(GameDeleteEventArgs e) => GameDelete?.Invoke(this, e);
        /// <summary>
        /// Demande la construction d'un batiment par un joueur. A utiliser en phase de construction
        /// </summary>
        public event EventHandler<ConstructEventArgs> Construct;
        /// <summary>
        /// Fonction d'appel de l'evenement Construct.
        /// Normalement appelee par le modules exterieurs au noyau.
        /// </summary>
        /// <param name="e">Donnees indiquant la coordonnee ou construire</param>
        public virtual void OnConstruct(ConstructEventArgs e) => Construct?.Invoke(this, e);
        /// <summary>
        /// Demande un echange entre 2 joueurs. A utiliser en phase d'echange
        /// </summary>
        public event EventHandler<PlayerExchangeEventArgs> PlayerExchange;
        /// <summary>
        /// Fonction d'appel de l'evenement PlayerExchange.
        /// Normalement appelee par le modules exterieurs au noyau.
        /// </summary>
        /// <param name="e">Donnees indiquant les joueurs souhaitant echanger et les ressources concernees</param>
        public virtual void OnPlayerExchange(PlayerExchangeEventArgs e) => PlayerExchange?.Invoke(this, e);
        /// <summary>
        /// Demande un echange avec un port (par defaut, general ou specialise). A utiliser en phase d'echange
        /// Necessite une colonie/ville sur un emplacement approprie dans les 2 derniers cas
        /// </summary>
        public event EventHandler<HarborExchangeEventArgs> HarborExchange;
        /// <summary>
        /// Fonction d'appel de l'evenement HarborExchange.
        /// Normalement appelee par le modules exterieurs au noyau.
        /// </summary>
        /// <param name="e">Donnees indiquant le joueur souhaitant echanger et les ressources concernees</param>
        public virtual void OnHarborExchange(HarborExchangeEventArgs e) => HarborExchange?.Invoke(this, e);
        /// <summary>
        /// Evenement a appeler lors de l'utilisation d'une carte chevalier par le joueur
        /// </summary>
        public event EventHandler<BanditMoveEventArgs> KnightCardUse;
        /// <summary>
        /// Fonction d'appel de l'evenement KnightCardUse.
        /// Normalement appelee par le modules exterieurs au noyau.
        /// </summary>
        /// <param name="e">Donnees indiquant la coordonnee ou placer le bandit et quel joueur voler</param>
        public virtual void OnKnightCardUse(BanditMoveEventArgs e) => KnightCardUse?.Invoke(this, e);
        /// <summary>
        /// Evenement a appeler lors de l'utilisation d'une carte chevalier par le joueur
        /// </summary>
        public event EventHandler<BanditMoveEventArgs> BanditMove;
        /// <summary>
        /// Fonction d'appel de l'evenement BanditMove.
        /// Normalement appelee par le modules exterieurs au noyau.
        /// </summary>
        /// <param name="e">Donnees indiquant la coordonnee ou placer le bandit et quel joueur voler</param>
        public virtual void OnBanditMove(BanditMoveEventArgs e) => BanditMove?.Invoke(this, e);
        /// <summary>
        /// Evenement a appeler lors de l'utilisation d'une carte Monopole par le joueur
        /// </summary>
        public event EventHandler<MonopolyCardUseEventArgs> MonopolyCardUse;
        /// <summary>
        /// Fonction d'appel de l'evenement MonopolyCardUse.
        /// Normalement appelee par le modules exterieurs au noyau.
        /// </summary>
        /// <param name="e">Donnees indiquant la ressource voulue par le joueur</param>
        public virtual void OnMonopolyCardUse(MonopolyCardUseEventArgs e) => MonopolyCardUse?.Invoke(this, e);
        /// <summary>
        /// Evenement a appeler lors de l'utilisation d'une carte Paire de ressource par le joueur
        /// </summary>
        public event EventHandler<ResourcePairCardUseEventArgs> ResourcePairCardUse;
        /// <summary>
        /// Fonction d'appel de l'evenement ResourcePairCardUse.
        /// Normalement appelee par le modules exterieurs au noyau.
        /// </summary>
        /// <param name="e">Donnees indiquant les 2 ressources voulues par le joueur</param>
        public virtual void OnResourcePairCardUse(ResourcePairCardUseEventArgs e) => ResourcePairCardUse?.Invoke(this, e);
        /// <summary>
        /// Evenement a appeler lors de l'utilisation d'une carte construction de route par le joueur
        /// </summary>
        public event EventHandler<RoadConstructionCardUseEventArgs> RoadConstructionCardUse;
        /// <summary>
        /// Fonction d'appel de l'evenement RoadConstructionCardUse.
        /// Normalement appelee par le modules exterieurs au noyau.
        /// </summary>
        /// <param name="e">Donnees indiquant les coordonnees ou construire</param>
        public virtual void OnRoadConstructionCardUse(RoadConstructionCardUseEventArgs e) => RoadConstructionCardUse?.Invoke(this, e);
        /// <summary>
        /// Lance le de determinant les ressources recoltees (ou le passage en phase de defausse).
        /// </summary>
        public event EventHandler<BaseEventArgs> DiceThrow;
        /// <summary>
        /// Fonction d'appel de l'evenement DiceThrow.
        /// Normalement appelee par le modules exterieurs au noyau.
        /// </summary>
        /// <param name="e">Donnees indiquant la partie concernee</param>
        public virtual void OnDiceThrow(BaseEventArgs e) => DiceThrow?.Invoke(this, e);
        /// <summary>
        /// Evenement de defausses des cartes excedentaires des joueurs ayant 8 cartes ou plus apres un jet de 7 en phase de recolte.
        /// </summary>
        public event EventHandler<DiscardEventArgs> DiscardExtraRessources;
        /// <summary>
        /// Fonction d'appel de l'evenement DiscardExtraRessources.
        /// Normalement appelee par le modules exterieurs au noyau.
        /// </summary>
        /// <param name="e">Donnees indiquant les ressources a defausser de chaque joueur</param>
        public virtual void OnDiscardExtraRessources(DiscardEventArgs e) => DiscardExtraRessources?.Invoke(this, e);
        /// <summary>
        /// Evenement de construction d'une colonie durant la phase de construction initiale.
        /// </summary>
        public event EventHandler<InitialConstructEventArgs> InitialColony;
        /// <summary>
        /// Fonction d'appel de l'evenement InitialColony.
        /// Normalement appelee par le modules exterieurs au noyau.
        /// </summary>
        /// <param name="e">Donnees indiquant la coordonnee ou construire</param>
        public virtual void OnInitialColony(InitialConstructEventArgs e) => InitialColony?.Invoke(this, e);
        /// <summary>
        /// Evenement de gestion d'une IA au premier tour
        /// </summary>
        public event EventHandler<BaseEventArgs> FirstPlayerAI;
        /// <summary>
        /// Fonction d'appel de l'evenement FirstPlayerAI.
        /// Normalement appelee par le modules exterieurs au noyau.
        /// </summary>
        /// <param name="e">Donnees indiquant la partie concernee</param>
        public virtual void OnFirstPlayerAI(BaseEventArgs e) => FirstPlayerAI?.Invoke(this, e);
        /// <summary>
        /// Evenement permettant de construire une route durant la phase de construction initiale
        /// </summary>
        public event EventHandler<InitialConstructEventArgs> InitialRoad;
        /// <summary>
        /// Fonction d'appel de l'evenement InitialRoad.
        /// Normalement appelee par le modules exterieurs au noyau.
        /// </summary>
        /// <param name="e">Donnees indiquant la coordonnee ou construire</param>
        public virtual void OnInitialRoad(InitialConstructEventArgs e) => InitialRoad?.Invoke(this, e);

        // Evenements Noyau => Exterieur

        /// <summary>
        /// Evenement appele lors du debut d'une partie apres sa creation dans le noyau
        /// </summary>
        public event EventHandler<GameStatusArgs> GameBegin;
        /// <summary>
        /// Fonction d'appel de l'evenement GameBegin.
        /// Normalement appelee par le noyau.
        /// </summary>
        /// <param name="e">Donnees de l'etat de la partie ayant debute</param>
        public virtual void OnGameBegin(GameStatusArgs e)
        {
            Game = e.Game;
            GameBegin?.Invoke(this, e);
        }
        /// <summary>
        /// Evenement appele lors de la suppression d'une partie
        /// </summary>
        public event EventHandler<GameDeletedArgs> GameDeleted;
        /// <summary>
        /// Fonction d'appel de l'evenement GameDeleted.
        /// Normalement appelee par le noyau.
        /// </summary>
        /// <param name="e">Donnees du succes de la suppression</param>
        public virtual void OnGameDeleted(GameDeletedArgs e)
        {
            GameDeleted?.Invoke(this, e);
        }
        /// <summary>
        /// Evenement appele au debut du premier round de la phase de construction initiale en debut de partie
        /// </summary>
        public event EventHandler<GameStatusArgs> InitialConstructionFirstRound;
        /// <summary>
        /// Fonction d'appel de l'evenement InitialConstructionFirstRound.
        /// Normalement appelee par le noyau.
        /// </summary>
        /// <param name="e">Donnees de l'etat de la partie en cours</param>
        public virtual void OnInitialConstructionFirstRound(GameStatusArgs e)
        {
            Game = e.Game;
            InitialConstructionFirstRound?.Invoke(this, e);
        }
        /// <summary>
        /// Evenement appele au debut de second round de la phase de construction initiale
        /// </summary>
        public event EventHandler<GameStatusArgs> InitialConstructionSecondRound;
        /// <summary>
        /// Fonction d'appel de l'evenement InitialConstructionSecondRound.
        /// Normalement appelee par le noyau.
        /// </summary>
        /// <param name="e">Donnees de l'etat de la partie en cours</param>
        public virtual void OnInitialConstructionSecondRound(GameStatusArgs e)
        {
            Game = e.Game;
            InitialConstructionSecondRound?.Invoke(this, e);
        }
        /// <summary>
        /// Evenement appele lors du debut de la phase de recolte
        /// </summary>
        public event EventHandler<GameStatusArgs> HarvestPhaseBegin;
        /// <summary>
        /// Fonction d'appel de l'evenement HarvestPhaseBegin.
        /// Normalement appelee par le noyau.
        /// </summary>
        /// <param name="e">Donnees de l'etat de la partie en cours</param>
        public virtual void OnHarvestPhaseBegin(GameStatusArgs e)
        {
            Game = e.Game;
            HarvestPhaseBegin?.Invoke(this, e);
        }
        /// <summary>
        /// Evenement appele lors de la phase de defausse du bandit
        /// </summary>
        public event EventHandler<GameStatusArgs> DiscardPhaseBegin;
        /// <summary>
        /// Fonction d'appel de l'evenement DiscardPhaseBegin.
        /// Normalement appelee par le noyau.
        /// </summary>
        /// <param name="e">Donnees du resultats du jet de des</param>
        public virtual void OnDiscardPhaseBegin(DiceResultsInfoArgs e)
        {
            Game = e.Game;
            DiscardPhaseBegin?.Invoke(this, e);
        }
        /// <summary>
        /// Evenement appele lors du debut de la phase de deplacement du bandit
        /// </summary>
        public event EventHandler<GameStatusArgs> BanditMoveBegin;
        /// <summary>
        /// Fonction d'appel de l'evenement BanditMoveBegin.
        /// Normalement appelee par le noyau.
        /// </summary>
        /// <param name="e">Donnees de l'etat de la partie en cours</param>
        public virtual void OnBanditMoveBegin(GameStatusArgs e)
        {
            Game = e.Game;
            BanditMoveBegin?.Invoke(this, e);
        }
        /// <summary>
        /// Evenement appele lors du debut de la phase d'echange
        /// </summary>
        public event EventHandler<DiceResultsInfoArgs> ExchangePhaseBegin;
        /// <summary>
        /// Fonction d'appel de l'evenement ExchangePhaseBegin.
        /// Normalement appelee par le noyau.
        /// </summary>
        /// <param name="e">Donnees de l'etat de la partie en cours</param>
        public virtual void OnExchangePhaseBegin(DiceResultsInfoArgs e)
        {
            Game = e.Game;
            ExchangePhaseBegin?.Invoke(this, e);
        }
        /// <summary>
        /// Evenement appele au debut de la phase de construction du tour
        /// </summary
        public event EventHandler<GameStatusArgs> ConstructionPhaseBegin;
        /// <summary>
        /// Fonction d'appel de l'evenement ConstructionPhaseBegin.
        /// Normalement appelee par le noyau.
        /// </summary>
        /// <param name="e">Donnees de l'etat de la partie en cours</param>
        public virtual void OnConstructionPhaseBegin(GameStatusArgs e)
        {
            Game = e.Game;
            ConstructionPhaseBegin?.Invoke(this, e);
        }
        /// <summary>
        /// Evenement appele suite a une construction effectuee.
        /// A ne pas confondre avec la fin de la phase Construction.
        /// </summary>
        public event EventHandler<ActionDoneInfoArgs> ConstructionDone;
        /// <summary>
        /// Fonction d'appel de l'evenement ConstructionDone.
        /// Normalement appelee par le noyau.
        /// </summary>
        /// <param name="e">Donnees du succes de l'action effectuee</param>
        public virtual void OnConstructionDone(ActionDoneInfoArgs e)
        {
            Game = e.Game;
            ConstructionDone?.Invoke(this, e);
        }
        /// <summary>
        /// Evenement appele a la fin d'un echange effectue
        /// </summary>
        public event EventHandler<ActionDoneInfoArgs> ExchangeDone;
        /// <summary>
        /// Fonction d'appel de l'evenement ExchangeDone.
        /// Normalement appelee par le noyau.
        /// </summary>
        /// <param name="e">Donnees du succes de l'action effectuee</param>
        public virtual void OnExchangeDone(ActionDoneInfoArgs e)
        {
            Game = e.Game;
            ExchangeDone?.Invoke(this, e);
        }
        /// <summary>
        /// Evenement appelee apres l'utilisation d'une carte de developpement.
        /// </summary>
        public event EventHandler<ActionDoneInfoArgs> CardUsageDone;
        /// <summary>
        /// Fonction d'appel de l'evenement CardUsageDone.
        /// Normalement appelee par le noyau.
        /// </summary>
        /// <param name="e">Donnees du succes de l'action effectuee</param>
        public virtual void OnCardUsageDone(ActionDoneInfoArgs e)
        {
            Game = e.Game;
            CardUsageDone?.Invoke(this, e);
        }
        /// <summary>
        /// Evenement a appeler apres qu'un joueur ait atteint la condition de victoire.
        /// </summary>
        public event EventHandler<VictoryInfoArgs> Victory;
        /// <summary>
        /// Fonction d'appel de l'evenement Victory.
        /// Normalement appelee par le noyau.
        /// </summary>
        /// <param name="e">Donnees de classement des joueurs</param>
        public virtual void OnVictory(VictoryInfoArgs e)
        {
            Game = e.Game;
            Victory?.Invoke(this, e);
        }

        /// <summary>
        /// Instance interne de la classe permettant d'avoir une unique instance de celle-ci (Singleton)
        /// Implementation interne (Non utilise par les autres modules)
        /// </summary>
        private static readonly Lazy<GameView> gameView = new Lazy<GameView>(() => new GameView());
        private GameController controller;

        /// <summary>
        /// Constructeur interne permettant d'initialiser le reseau
        /// </summary>
        private GameView()
        {
            controller = new GameController(NetworkClient.Instance.isOnline);
            // En mode local c'est le controleur qui doit reagir aux evenements, sinon le module reseau
        }
    }
}
