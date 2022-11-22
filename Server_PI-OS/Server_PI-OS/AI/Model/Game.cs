using System;
using System.Collections.Generic;
using AI.Model;

namespace AI.Model
{
    class Game
    {
        public Dictionary<int, AbstractAI> IAPlayers;

        public Game(Dictionary<int, AbstractAI> iaPlayers)
        {
            IAPlayers = iaPlayers;
        }
    }
}
