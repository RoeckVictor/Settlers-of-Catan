using System;
using System.Collections;
using System.Collections.Generic;
using Noyau.View;
using Util.View;
using AI.Model;
using AI.View;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AI.Controller
{
    public class AIManager 
    {
        private Dictionary<Guid, Game> gamesWithIAs = new Dictionary<Guid, Game>();

        public static AIManager Instance { get { return iaManager.Value; } }
        private static readonly Lazy<AIManager> iaManager = new Lazy<AIManager>(() => new AIManager());

        private bool eventHandlersRegistered = false;
        public void RegisterEventHandlers()
        {
            if (eventHandlersRegistered)
                return;

            GameView.Instance.GameBegin += GameBeginHandler;
            GameView.Instance.GameDeleted += GameDeletedHandler;
            GameView.Instance.ConstructionDone += ConstructionDoneHandler;
            GameView.Instance.ConstructionPhaseBegin += ConstructionPhaseBeginHandler;
            GameView.Instance.HarvestPhaseBegin += HarvestPhaseBeginHandler;
            GameView.Instance.DiscardPhaseBegin += DiscardPhaseBeginHandler;
            GameView.Instance.ExchangePhaseBegin += ExchangePhaseBeginHandler;
            GameView.Instance.BanditMoveBegin += BanditMovePhaseBeginHandler;
            GameView.Instance.InitialConstructionFirstRound += InitialConstructHandler;

            eventHandlersRegistered = true;
        }

        public void UnregisterEventHandlers()
        {
            if (!eventHandlersRegistered)
                return;

            GameView.Instance.GameBegin -= GameBeginHandler;
            GameView.Instance.GameDeleted -= GameDeletedHandler;
            GameView.Instance.ConstructionDone -= ConstructionDoneHandler;
            GameView.Instance.ConstructionPhaseBegin -= ConstructionPhaseBeginHandler;
            GameView.Instance.HarvestPhaseBegin -= HarvestPhaseBeginHandler;
            GameView.Instance.DiscardPhaseBegin -= DiscardPhaseBeginHandler;
            GameView.Instance.ExchangePhaseBegin -= ExchangePhaseBeginHandler;
            GameView.Instance.BanditMoveBegin -= BanditMovePhaseBeginHandler;
            GameView.Instance.InitialConstructionFirstRound -= InitialConstructHandler;
            eventHandlersRegistered = false;
        }

        

        public void GameBeginHandler(object sender, GameStatusArgs e)
        {
            Dictionary<int, AbstractAI> iaPlayers = new Dictionary<int, AbstractAI>();

            for (var i = 0; i < e.Game.Players.Count; i++)
            {
                if (e.Game.Players[i].IsIA)
                    iaPlayers.Add(i, CreateAi(e.AIDifficulty));
            }

            Game newIAGame;
            if (iaPlayers.Count > 0)
            {
                newIAGame = new Game(iaPlayers);
                gamesWithIAs[e.GameId] = newIAGame;
            }
            else
                return;

            // Si le 1er joueur est une IA on construit directement
            IGame game = e.Game;
            IPlayer currPlayer = game.Players[game.CurrentPlayer];

        }

        public void ReplaceWithAi(IGame game, int playerId, int difficulty)
        {
            game.Players[playerId].IsIA = true;
            AbstractAI newIA = CreateAi(difficulty);
            newIA.InitAI(game);
            if (gamesWithIAs[game.Id].IAPlayers.ContainsKey(playerId))
            {
                System.Console.WriteLine("Player is already an AI!");
                return;
            }
            gamesWithIAs[game.Id].IAPlayers.Add(playerId, newIA);
        }

        internal AbstractAI CreateAi(int difficulty)
        {
            switch (difficulty)
            {
                case 0:
                    return new EasyAI();
                case 1:
                    return new MediumAI();
                case 2:
                    return new DifficultAI();
                default:
                    return new DifficultAI();
            }
        }

        // Appelée lors d'une demande de suppression de partie
        /// <summary>
        /// Handler de suppression de partie
        /// </summary>
        /// <param name="sender">Le sender de l'event</param>
        /// <param name="e">Les arguments de l'évènement</param>
        public void GameDeletedHandler(object sender, GameDeletedArgs e)
        {
            if (!gamesWithIAs.ContainsKey(e.GameId))
            {
                return;
            }

            gamesWithIAs.Remove(e.GameId);
        }

        //static int bloc = 0;
        public void ConstructionDoneHandler(object sender, ActionDoneInfoArgs e)
        {

            IGame game = e.Game;
            IPlayer currPlayer = game.Players[game.CurrentPlayer];

            if (!currPlayer.IsIA || !e.ActionSuccessful)
            {
                return;
            }

            
            IAI currIA = gamesWithIAs[e.GameId].IAPlayers[e.Game.CurrentPlayer];
            if (game.CurrentPhase == GamePhase.INITIAL_BUILDING_1)
            {

                if (currPlayer.VictoryPoints == 1)
                    DelayedFirstRoadConstruct(1, currIA, game);
                else
                    DelayedFirstColonyConstruct(1, currIA, game);

            }
            else if (game.CurrentPhase == GamePhase.INITIAL_BUILDING_2)
            {
                if (currPlayer.VictoryPoints == 2)
                    DelayedSecondRoadConstruct(1, currIA, game);
                else
                    DelayedSecondColonyConstruct(1, currIA, game);

            }

        }

        internal async void DelayedFirstRoadConstruct(int seconds, IAI ai, IGame game)
        {
            await Task.Delay(seconds * 1000);
            ai.PlaceFirstRoad(game);
        }
        internal async void DelayedSecondRoadConstruct(int seconds, IAI ai, IGame game)
        {
            await Task.Delay(seconds * 1000);
            ai.PlaceSecondRoad(game);
        }
        internal async void DelayedFirstColonyConstruct(int seconds, IAI ai, IGame game)
        {
            await Task.Delay(seconds * 1000);
            ai.PlaceFirstColony(game);
        }
        internal async void DelayedSecondColonyConstruct(int seconds, IAI ai, IGame game)
        {
            await Task.Delay(seconds * 1000);
            ai.PlaceSecondColony(game);
        }

        public void InitialConstructHandler(object sender, GameStatusArgs e)
        {
            gamesWithIAs[e.GameId].IAPlayers[e.Game.CurrentPlayer].PlaceFirstColony(GameView.Instance.GetGame(e.GameId));
        }

        public void ConstructionPhaseBeginHandler(object sender, GameStatusArgs e)
        {
            IGame game = e.Game;
            IPlayer currPlayer = game.Players[game.CurrentPlayer];
            if (!currPlayer.IsIA)
            {
                return;
            }

            //gamesWithIAs[e.GameId].IAPlayers[e.Game.CurrentPlayer].Construct(e.Game);
            DelayedConstruct(1, gamesWithIAs[e.GameId].IAPlayers[e.Game.CurrentPlayer], e.Game);
        }

        internal async void DelayedConstruct(int seconds, IAI ai, IGame game)
        {
            await Task.Delay(seconds * 1000);
            ai.Construct(game);
        }

        public void HarvestPhaseBeginHandler(object sender, GameStatusArgs e)
        {
            IGame game = e.Game;
            IPlayer currPlayer = game.Players[game.CurrentPlayer];
            if (!currPlayer.IsIA)
            {
                return;
            }
            DelayedDiceThrow(1, gamesWithIAs[e.GameId].IAPlayers[e.Game.CurrentPlayer], e.Game);

        }

        internal async void DelayedDiceThrow(int seconds, IAI ai, IGame game)
        {
            await Task.Delay(seconds * 1000);
            ai.HarvestBegin(game);
        }

        public void DiscardPhaseBeginHandler(object sender, GameStatusArgs e)
        {
            IGame game = e.Game;
            IPlayer currPlayer = game.Players[game.CurrentPlayer];


            foreach(IPlayer p in game.Players)
            {
                if (p.IsIA)
                {
                    gamesWithIAs[e.GameId].IAPlayers[p.Id].Discard(e.Game);
                }
                    
            }
        }

        public void ExchangePhaseBeginHandler(object sender, GameStatusArgs e)
        {
            IGame game = e.Game;
            IPlayer currPlayer = game.Players[game.CurrentPlayer];
            if (!currPlayer.IsIA)
            {
                return;
            }

            gamesWithIAs[e.GameId].IAPlayers[e.Game.CurrentPlayer].Exchange(e.Game);
        }

        public void BanditMovePhaseBeginHandler(object sender, GameStatusArgs e)
        {
            IGame game = e.Game;
            IPlayer currPlayer = game.Players[game.CurrentPlayer];
            if (!currPlayer.IsIA)
            {
                return;
            }

            gamesWithIAs[e.GameId].IAPlayers[e.Game.CurrentPlayer].MoveBandit(e.Game);
        }

    }

}
