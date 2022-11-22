using System;
using System.Collections.Generic;
using System.Text;

namespace Noyau.View
{

    /// <summary>
    /// Valeur correspondant aux types d'actions pouvant etre effectuees par un joueur ou une IA.
    /// </summary>
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

    /// <summary>
    /// Valeur correspondant aux types de cartes de developpement pouvant etre obtenues en cours de partie.
    /// Celle-ci peuvent ensuite etre utilisees au tour suivant par le joueur (IA ou humain).
    /// </summary>
    public enum CardType
    {
        KNIGHT,
        VICTORY_POINT,
        ROAD_BUILDING,
        RESSOURCE_PAIR,
        RESSOURCE_MONOPOLY
    }

    /// <summary>
    /// Valeur corresponant aux types de ressources pouvant etre obtenues par les joueurs.
    /// </summary>
    public enum RessourceType
    {
        NONE,
        BRICK,
        LUMBER,
        WHEAT,
        WOOL,
        ORE
    }

    /// <summary>
    /// Valeur correspondant aux differentes phases d'un tour de jeu standard, a l'exception de INITIAL_BUILDING_1 et _2.
    /// Celles-ci n'ont lieu qu'une fois en tout debut de partie.
    /// </summary>
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

    /// <summary>
    /// Valeur correspondant aux types de constructions qu'il est possible d'obtenir au cours de la phase de Construction.
    /// Les cartes de developpement sont considerees comme une construction.
    /// </summary>
    public enum ConstructionType
    {
        NONE,
        ROAD,
        SETTLEMENT,
        DEVELOPMENT_CARD,
        CITY
    }

    /// <summary>
    /// Les types de ports differents, nommes en fonctions des ressources disponibles.
    /// </summary>
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

    /// <summary>
    /// Les types de terrain possibles des differentes cases de HexGrid.
    /// </summary>
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
