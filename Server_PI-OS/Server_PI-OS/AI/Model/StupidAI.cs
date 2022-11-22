using Noyau.View;
using System;
using System.Collections.Generic;
using Util.View;

namespace AI.Model
{
    class StupidAI : AbstractAI
    {
        public override void PlaceFirstColony(IGame game)
        {
            Random r = new Random();
            Coordinate c;
            List<Coordinate> keyList = new List<Coordinate>(game.GameGrid.Intersections.Keys);
            do
            {
                c = keyList[r.Next(keyList.Count)];
            } while (game.GameGrid.Intersections[c].Building == ConstructionType.SETTLEMENT);

            GameView.Instance.OnInitialColony(new InitialConstructEventArgs(game.Id, c, false));
        }

        public override void PlaceFirstRoad(IGame game)
        {
            List<Coordinate> possibleRoads = game.GameGrid.PossibleRoads(game.Players[game.CurrentPlayer]);
            GameView.Instance.OnInitialRoad(new InitialConstructEventArgs(game.Id, possibleRoads[0], false));
        }

        public override void PlaceSecondColony(IGame game)
        {
            Random r = new Random();
            Coordinate c;
            List<Coordinate> keyList = new List<Coordinate>(game.GameGrid.Intersections.Keys);
            do
            {
                c = keyList[r.Next(keyList.Count)];
            } while (game.GameGrid.Intersections[c].Building == ConstructionType.SETTLEMENT);
            GameView.Instance.OnInitialColony(new InitialConstructEventArgs(game.Id, c, false));
        }

        public override void PlaceSecondRoad(IGame game)
        {
            List<Coordinate> possibleRoads = game.GameGrid.PossibleRoads(game.Players[game.CurrentPlayer]);
            GameView.Instance.OnInitialRoad(new InitialConstructEventArgs(game.Id, possibleRoads[0], false));
        }

        public override void HarvestBegin(IGame game)
        {
            // Do stuff first
            GameView.Instance.OnDiceThrow(new BaseEventArgs(game.Id, false));
        }

        public override void Construct(IGame game)
        {
            GameView.Instance.OnEndPhase(new BaseEventArgs(game.Id,false));
        }

        public override void Discard(IGame game)
        {
            GameView.Instance.OnDiscardExtraRessources(new DiscardEventArgs(game.Id, new List<(int idPlayer, List<(RessourceType rType, int num)>)>(), false));
        }

        public override void Exchange(IGame game)
        {
            GameView.Instance.OnEndPhase(new BaseEventArgs(game.Id,false));
        }

        public override void MoveBandit(IGame game)
        {
            GameView.Instance.OnBanditMove(new BanditMoveEventArgs(game.Id, game.GameGrid.CurrentThiefLocation, -1,false));
        }
    }
}
