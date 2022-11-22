using System;
using System.Collections.Generic;
using Noyau.View;
namespace Util.View
{

    /// <summary>
    /// <para>Rassemble tou le stypes d'énumérations utilisés dans le jeu</para>
    /// </summary>
    public enum ClientType { RegisteredPlayer = 0, AnonymousPlayer = 1, EasyAI = 2, MediumAI = 3, DifficultAI = 4 };

    public enum DB_ConstraintError
    {
        Ok = 0,
        // ID not found
        NonExistentID,
        // Insert Player
        NullPlayer, InvalidPlayerId, NullPseudonym, PseudonymAlreadyTaken, PseudonymTooShort, PseudonymTooLong, NullRegisteredPlayerPassword,
        // Insert Game
        NullGame, EmptyGameId, GameIdAlreadyTaken, NoPlayer, CreatorInAnotherGame, NonExistentCreator, NullGameName, GameNameTooShort, GameNameTooLong, InvalidNbPlayers,
        // Insert Message
        NullMessage, NonExistentReference, MessagePrimaryKey, NullMessageText, MessageTooShort, MessageTooLong,
        // Connect Client
        FullGame,
        // Test/Update Client/Game Password
        NullPassword
    }

    public enum EventType
    {
        CREATE,
        JOIN,
        CONSTRUCT,
        STATUS,
        INITIAL,
        EXCHANGE,
        HARBOR,
        BANDIT,
        RESOURCE,
        MONOPOLY,
        ROAD,
        INFO,
        DISCONNECT,
        DELETE,
        DISCARD,
        REGISTER,
        DICE,
        READY,
        START,
        MESSAGE,
        DELETED,
        VICTORY,
        JOINED,
        ERROR,
        MYINFO,
        ACCEPT,
        INFONETWORK,
        STATUSNETWORK,
        PLIST,
        BASE,
        DISCARDNETWORK,
        REFRESH,
        QUITLOBBY,
        NONE
    }


    public enum ErrorType
    {
        CREATE_GAME,
        ADD_PLAYER_TO_GROUP,
        JOIN_GROUP,
        JOIN_RANDOM_GROUP,
        REMOVE_GAME,
        CONNECT_GAME_PLAYER,
        REGISTER_DB,
        PASSWORD,
        NONE
    }

    public static class Callbacks
    {

        public static Dictionary<string, EventType> events = new Dictionary<string, EventType>(){
            {"Util.View.GameCreate", EventType.CREATE },
            {"Util.View.GameJoinEventArgs", EventType.JOIN },
            {"Util.View.GameDeleteEventArgs",EventType.DELETE },
            {"Util.View.GameDeletedArgs",EventType.DELETED },
            {"Util.View.GameStatus", EventType.STATUS },
            {"Util.View.BaseEventArgs", EventType.BASE },
            {"Util.View.GameStatusArgsNetwork", EventType.STATUSNETWORK },
            {"Util.View.ConstructEventArgs", EventType.CONSTRUCT },
            {"Util.View.InitialConstructEventArgs" , EventType.INITIAL},
            {"Util.View.ExchangeEvent" , EventType.EXCHANGE},
            {"Util.View.AcceptExchange" , EventType.ACCEPT},
            {"Util.View.HarborExchangeEventArgs", EventType.HARBOR },
            {"Util.View.BanditMoveEventArgs", EventType.BANDIT },
            {"Util.View.ResourcePairCardUseEventArgs", EventType.RESOURCE },
            {"Util.View.MonopolyCardUseEventArgs", EventType.MONOPOLY },
            {"Util.View.RoadConstructionCardUseEventArgs", EventType.ROAD },
            {"Util.View.ActionDoneInfoArgs", EventType.INFO },
            {"Util.View.ActionDoneNetwork", EventType.INFONETWORK },
            {"Util.View.QuitLobby", EventType.QUITLOBBY },
            {"Util.View.Refresh", EventType.REFRESH },
            {"Util.View.DisconnectEventArgs", EventType.DISCONNECT },
            {"Util.View.DiscardEventArgs", EventType.DISCARD },
            {"Util.View.DiscardEvent", EventType.DISCARDNETWORK },
            {"Util.View.DiceResultsInfoArgsNetwork", EventType.DICE },
            {"Util.View.RegisterEventArgs", EventType.REGISTER },
            {"Util.View.ReadyEvent", EventType.READY },
            {"Util.View.StartedGame", EventType.START},
            {"Util.View.MessageEvent", EventType.MESSAGE},
            {"Util.View.PlayerJoinedGame", EventType.JOINED},
            {"Util.View.ErrorEventArgs", EventType.ERROR},
            {"Util.View.InfoGame", EventType.MYINFO},
            {"Util.View.VictoryInfo", EventType.VICTORY},
            {"Util.View.PlayersListInGameEvent", EventType.PLIST}

        };

        public static Dictionary<string, CardType> CardType = new Dictionary<string, CardType>()
        {
            {"KNIGHT", Noyau.View.CardType.KNIGHT },
            {"VICTORY_POINT",   Noyau.View.CardType.VICTORY_POINT },
            { "ROAD_BUILDING", Noyau.View.CardType.ROAD_BUILDING },
            {"RESSOURCE_PAIR", Noyau.View.CardType.RESSOURCE_PAIR },
            {"RESSOURCE_MONOPOLY", Noyau.View.CardType.RESSOURCE_MONOPOLY }
        };

        public static Dictionary<string, RessourceType> RessourceType = new Dictionary<string, RessourceType>()
        {
            {"NONE", Noyau.View.RessourceType.NONE },
            {"BRICK", Noyau.View.RessourceType.BRICK },
            {"LUMBER", Noyau.View.RessourceType.LUMBER },
            {"WHEAT", Noyau.View.RessourceType.WHEAT },
            {"WOOL", Noyau.View.RessourceType.WOOL },
            {"ORE", Noyau.View.RessourceType.ORE }
        };

        public static Dictionary<string, GamePhase> GamePhase = new Dictionary<string, GamePhase>()
        {
            {"INITIAL_BUILDING_1" , Noyau.View.GamePhase.INITIAL_BUILDING_1 },
            {"INITIAL_BUILDING_2", Noyau.View.GamePhase.INITIAL_BUILDING_2 },
            {"RECOLT", Noyau.View.GamePhase.RECOLT },
            {"EXCHANGE", Noyau.View.GamePhase.EXCHANGE },
            {"CONSTRUCTION", Noyau.View.GamePhase.CONSTRUCTION },
            {"DISCARD", Noyau.View.GamePhase.DISCARD },
            {"BANDIT_MOVE", Noyau.View.GamePhase.BANDIT_MOVE }
        };

        public static Dictionary<string, ConstructionType> ConstructionType = new Dictionary<string, ConstructionType>()
        {
            {"NONE", Noyau.View.ConstructionType.NONE },
            {"ROAD", Noyau.View.ConstructionType.ROAD },
            {"SETTLEMENT", Noyau.View.ConstructionType.SETTLEMENT },
            {"DEVELOPMENT_CARD", Noyau.View.ConstructionType.DEVELOPMENT_CARD },
            {"CITY", Noyau.View.ConstructionType.CITY }
        };

        public static Dictionary<string, HarborType> HarborType = new Dictionary<string, HarborType>()
        {
            {"NONE", Noyau.View.HarborType.NONE },
            {"GENERAL", Noyau.View.HarborType.GENERAL },
            {"BRICK", Noyau.View.HarborType.BRICK },
            {"LUMBER", Noyau.View.HarborType.LUMBER },
            {"WHEAT", Noyau.View.HarborType.WHEAT },
            {"WOOL", Noyau.View.HarborType.WOOL },
            {"ORE", Noyau.View.HarborType.ORE }
        };

        public static Dictionary<string, TerrainType> TerrainType = new Dictionary<string, TerrainType>()
        {
            {"NONE", Noyau.View.TerrainType.NONE },
            {"HILLS", Noyau.View.TerrainType.HILLS },
            {"FOREST", Noyau.View.TerrainType.FOREST },
            {"FIELDS", Noyau.View.TerrainType.FIELDS },
            {"PASTURE", Noyau.View.TerrainType.PASTURE },
            {"MOUNTAINS", Noyau.View.TerrainType.MOUNTAINS },
            {"DESERT", Noyau.View.TerrainType.DESERT },
            {"SEA", Noyau.View.TerrainType.SEA }
        };

        public static Dictionary<string, ActionType> ActionType = new Dictionary<string, ActionType>()
        {

            {"NONE", Noyau.View.ActionType.NONE },
            {"ROAD", Noyau.View.ActionType.ROAD },
            {"SETTLEMENT", Noyau.View.ActionType.SETTLEMENT },
            {"DEVELOPMENT_CARD", Noyau.View.ActionType.DEVELOPMENT_CARD },
            {"CITY", Noyau.View.ActionType.CITY },
            {"KNIGHT", Noyau.View.ActionType.KNIGHT },
            {"VICTORY_POINT", Noyau.View.ActionType.VICTORY_POINT },
            {"ROAD_BUILDING", Noyau.View.ActionType.ROAD_BUILDING },
            {"RESSOURCE_PAIR", Noyau.View.ActionType.RESSOURCE_PAIR },
            {"RESSOURCE_MONOPOLY", Noyau.View.ActionType.RESSOURCE_MONOPOLY },
            {"PLAYER_EXCHANGE", Noyau.View.ActionType.PLAYER_EXCHANGE },
            {"HARBOR_EXCHANGE", Noyau.View.ActionType.HARBOR_EXCHANGE },
            {"ERROR", Noyau.View.ActionType.ERROR },

    };


        public static Dictionary<string, Action> MethodStatusDictionary(GameStatusArgs param) => new Dictionary<string, Action>
         {
            {"OnGameBegin" , () =>   GameView.Instance.OnGameBegin(param) },
            {"OnInitialConstructionFirstRound" , () =>   GameView.Instance.OnInitialConstructionFirstRound(param) },
            {"OnInitialConstructionSecondRound" , () =>  GameView.Instance.OnInitialConstructionSecondRound(param) },
            {"OnHarvestPhaseBegin" , () =>  GameView.Instance.OnHarvestPhaseBegin(param) },
            {"OnBanditMoveBegin",  () =>  GameView.Instance.OnBanditMoveBegin(param)},
            {"OnConstructionPhaseBegin",  () =>  GameView.Instance.OnConstructionPhaseBegin(param) },
            {"OnDiscardPhaseBegin", () => GameView.Instance.OnConstructionPhaseBegin(param) }
        };

        public static Dictionary<string, Action> MethodActionDictionary(ActionDoneInfoArgs param) => new Dictionary<string, Action>
        {
            {"OnConstructionDone", ()=> GameView.Instance.OnConstructionDone(param) },
            { "OnExchangeDone", ()=> GameView.Instance.OnExchangeDone(param) },
            {"OnCardUsageDone", ()=> GameView.Instance.OnCardUsageDone(param) }
        };


        public static Dictionary<string, Direction> Direction = new Dictionary<string, Direction>()
        {
            {"NONE", Noyau.View.Direction.NONE },
            {"UP", Noyau.View.Direction.UP },
            {"DOWN", Noyau.View.Direction.DOWN },
            {"NORTH_EAST", Noyau.View.Direction.NORTH_EAST },
            {"EAST", Noyau.View.Direction.EAST },
            {"SOUTH_EAST", Noyau.View.Direction.SOUTH_EAST }
        };
        public static EventType GetType(string key)
        {
            try
            {
                return events[key];
            }
            catch (KeyNotFoundException)
            {
                return EventType.NONE;
            }
        }

        public static CardType GetCardType(string key)
        {
            try
            {
                return CardType[key];
            }
            catch (KeyNotFoundException)
            {
                return Noyau.View.CardType.KNIGHT; // par défaut
            }
        }

        public static RessourceType GetRessourceType(string key)
        {
            try
            {
                return RessourceType[key];
            }
            catch (KeyNotFoundException)
            {
                return Noyau.View.RessourceType.BRICK;
            }
        }
        public static GamePhase GetGamePhase(string key)
        {
            try
            {
                return GamePhase[key];
            }
            catch (KeyNotFoundException)
            {
                return Noyau.View.GamePhase.BANDIT_MOVE;
            }
        }
        public static ConstructionType GetConstructionype(string key)
        {
            try
            {
                return ConstructionType[key];
            }
            catch (KeyNotFoundException)
            {
                return Noyau.View.ConstructionType.CITY;
            }
        }
        public static HarborType GetHarborType(string key)
        {
            try
            {
                return HarborType[key];
            }
            catch (KeyNotFoundException)
            {
                return Noyau.View.HarborType.BRICK;
            }
        }

        public static TerrainType GetTerrainType(string key)
        {
            try
            {
                return TerrainType[key];
            }
            catch (KeyNotFoundException)
            {
                return Noyau.View.TerrainType.DESERT;
            }
        }

        public static ActionType GetActionType(string key)
        {
            try
            {
                return ActionType[key];
            }
            catch (KeyNotFoundException)
            {
                return Noyau.View.ActionType.NONE;
            }
        }


        public static Direction GetDirection(string key)
        {
            try
            {
                return Direction[key];
            }
            catch (KeyNotFoundException)
            {
                return Noyau.View.Direction.NONE;
            }
        }
    }
}
