using System;
using System.Collections.Generic;
using System.Text;

namespace Noyau.View
{


    public enum ActionType
    {
        NONE,
        ROAD,
        SETTLEMENT,
        DEVELOPMENT_CARD,
        CITY,
        KNIGHT,
        VICTORY_POINT,
        ROAD_BUILDING,
        RESSOURCE_PAIR,
        RESSOURCE_MONOPOLY,
        PLAYER_EXCHANGE,
        HARBOR_EXCHANGE,
        ERROR
    }

    public enum CardType
    {
        KNIGHT,
        VICTORY_POINT,
        ROAD_BUILDING,
        RESSOURCE_PAIR,
        RESSOURCE_MONOPOLY
    }

    public enum RessourceType
    {
        NONE,
        BRICK,
        LUMBER,
        WHEAT,
        WOOL,
        ORE
    }

    public enum GamePhase
    {
        INITIAL_BUILDING_1,
        INITIAL_BUILDING_2,
        RECOLT,
        EXCHANGE,
        CONSTRUCTION,
        DISCARD,
        BANDIT_MOVE,
        ENDED
    }

    public enum ConstructionType
    {
        NONE,
        ROAD,
        SETTLEMENT,
        DEVELOPMENT_CARD,
        CITY
    }

    public enum HarborType
    {
        NONE,
        GENERAL,
        BRICK,
        LUMBER,
        WHEAT,
        WOOL,
        ORE
    }

    public enum TerrainType
    {
        NONE,
        HILLS,      // -> Bricks
        FOREST,     // -> Lumber
        FIELDS,     // -> Wheat
        PASTURE,    // -> Wool
        MOUNTAINS,   // -> Ore
        DESERT,
        SEA
    }
}
