using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noyau.View;
using Util.View;

namespace Noyau.Model
{
    /// <summary>
    /// <para>La classe Game.</para>
    /// <para>Elle contient les propriétés et les méthodes nécessaires au bon fonctionnement d'une partie.</para>
    /// </summary>
    public class Game : IGame
    {
        /// <inheritdoc/>
        public Guid Id { get; set; }
        /// <inheritdoc/>
        public IList<IPlayer> Players { get; set; }

        /// <value>Liste des joueurs</value>
        /// <see cref="Players"/>
        public List<Player> players;

        /// <inheritdoc/>
        public IHexGrid GameGrid { get; set; }
        /// <value>Cast de GameGrid</value>
        /// <see cref="GameGrid"/>
        public HexGrid gameGrid => (HexGrid)GameGrid;
        /// <inheritdoc/>
        public int CurrentPlayer { get; set; }
        /// <inheritdoc/>
        public GamePhase CurrentPhase { get; set; }
        /// <inheritdoc/>
        public bool CardUsedThisTurn { get; set; }
        /// <inheritdoc/>
        public (int, int) lastDice { get; set; }

        /// <summary>
        /// Constructeur de Game
        /// </summary>
        /// <param name="gameId">L'identifiant de la partie</param>
        /// <param name="playerNumber">Le nombre de joueur de la partie</param>
        /// <param name="customGrid">La grille de jeu, si elle n'est pas donnée, le jeu en génère une automatiquement.</param>
        /// <seealso cref="View.IHexGrid"/>
        /// <seealso cref="View.gameId"/>
        public Game(Guid gameId, int playerNumber, int iaNumber, IHexGrid customGrid = null)
        {
            Id = gameId;
            Players = new List<IPlayer>();
            players = new List<Player>();
            int firstIAIndex = playerNumber - iaNumber;
            for (var i = 0; i < playerNumber; i++)
            {
                Player player = new Player(i, i >= firstIAIndex);
                players.Add(player);
                Players.Add(player);
            }
            if (customGrid == null)
                GameGrid = HexGrid.CreateRandomGrid();
            else
                GameGrid = customGrid;
            CurrentPhase = GamePhase.INITIAL_BUILDING_1;
            Random random = new Random();
            CurrentPlayer = random.Next(firstIAIndex);
        }

        public Game()
        {
            Players = new List<IPlayer>();
            players = new List<Player>();
        }
        /// <summary>
        /// Méthode qui produit et distribue les ressources appropriees au joueurs. En fonctions de leur batiments et du bandit
        /// </summary>
        /// <param name="diceRoll">La somme des deux dés.</param>
        public void RecoltAndDistribute(int diceRoll)
        {
            if (diceRoll == 7)
            {
                // Should use Players[currentPlayer].Steal(...) instead
                throw new InvalidOperationException();
            }

            foreach (var tt in GameGrid.TerrainTiles)
            {
                if (tt.Value.DiceProductionNumber == diceRoll && !tt.Value.ThiefIsPresent)
                {
                    RessourceType r = RessourceProducedByTerrain(tt.Value.Type);
                    if (r != RessourceType.NONE)
                    {
                        List<IIntersection> intersections = gameGrid.GetIntersectionsFromTile(tt.Key);
                        foreach (Intersection inter in intersections)
                        {
                            if (inter.Building == ConstructionType.SETTLEMENT)
                                Players[inter.PlayerId].AddRessource(r);
                            else if (inter.Building == ConstructionType.CITY)
                                Players[inter.PlayerId].AddRessource(r, 2);
                        }

                    }
                }

            }


        }

        /// <summary>
        /// Méthode qui Echange les ressources de 2 joueurs
        /// </summary>
        /// <param name="p1">Les ressources du premier joueur, associant sont id à une liste de couple quantité;ressource</param>
        /// <param name="p2">>Les ressources du deuxième joueur, associant sont id à une liste de couple quantité;ressource</param>
        public void PlayerExchange((int playerId, IList<(RessourceType t, int num)> giftedRessources) p1, (int playerId, IList<(RessourceType t, int num)> giftedRessources) p2)
        {

            // Au moins 1 des 2 joueurs est celui dont etre celui dont c'est le tour
            if (!(p1.playerId == CurrentPlayer || p2.playerId == CurrentPlayer))
                throw new InvalidOperationException();

            // Check que les 2 joueurs ont bien les ressources disponibles
            //UnityEngine.Debug.Log("joueurA : "+p1.playerId);
            //UnityEngine.Debug.Log("Ressource : à échanger : valeur réelle");
            foreach (var res in p1.giftedRessources)
            {
                //UnityEngine.Debug.Log(res.t + " : " + res.num + " : " + players[p1.playerId].Ressources[res.t]);
                if (players[p1.playerId].Ressources[res.t] < res.num)
                    throw new InvalidOperationException();
            }


            //UnityEngine.Debug.Log("joueurB : " + p2.playerId);
            //UnityEngine.Debug.Log("Ressource : à échanger : valeur réelle");
            foreach (var res in p2.giftedRessources)
            {
                //UnityEngine.Debug.Log(res.t + " : " + res.num + " : " + players[p2.playerId].Ressources[res.t]);
                if (players[p2.playerId].Ressources[res.t] < res.num)
                    throw new InvalidOperationException();
            }

            // Echange
            foreach (var res in p1.giftedRessources)
            {
                players[p2.playerId].AddRessource(res.t, res.num);
                players[p1.playerId].RemoveRessource(res.t, res.num);
            }

            foreach (var res in p2.giftedRessources)
            {
                players[p1.playerId].AddRessource(res.t, res.num);
                players[p2.playerId].RemoveRessource(res.t, res.num);
            }

        }

        /// <summary>
        /// Méthode qui permet l'echange sans port, apres verification que le joueur a bien acces a celui-ci.
        /// </summary>
        /// <remarks>Taux de change 4:1</remarks>
        /// <param name="playerID">L'identifiant du joueur faisant l'echange</param>
        /// <param name="rToGive">Les ressources que le joueur abandonne</param>
        /// <param name="rToReceive">Les ressources que le joueur reçoit</param>
        public void NoHarborExchange(int playerID, RessourceType rToGive, RessourceType rToReceive)
        {
            if (rToGive == RessourceType.NONE || rToReceive == RessourceType.NONE)
                throw new InvalidOperationException();
            if (players[playerID].Ressources[rToGive] > 3)
            {
                players[playerID].RemoveRessource(rToGive, 4);
                players[playerID].AddRessource(rToReceive);
            }
        }

        /// <summary>
        /// Méthode qui permet l'echange avec un port généralisé, apres verification que le joueur a bien acces a celui-ci.
        /// </summary>
        /// <remarks>Taux de change 3:1</remarks>
        /// <param name="playerID">L'identifiant du joueur faisant l'echange</param>
        /// <param name="rToGive">Les ressources que le joueur abandonne</param>
        /// <param name="rToReceive">Les ressources que le joueur reçoit</param>
        public void GeneralHarborExchange(int playerID, RessourceType rToGive, RessourceType rToReceive)
        {
            if (rToGive == RessourceType.NONE || rToReceive == RessourceType.NONE)
                throw new InvalidOperationException();
            // Verification que le joueur dispose d'un port general
            bool canUseHarbor = false;
            foreach (var cons in Players[playerID].Constructions)
            {
                if (cons.Item2 == ConstructionType.SETTLEMENT || cons.Item2 == ConstructionType.CITY)
                {
                    Intersection inter = gameGrid.GetIntersection(cons.Item1);
                    if (inter.Harbor == HarborType.GENERAL)
                        canUseHarbor = true;
                }
            }
            if (!canUseHarbor)
                throw new InvalidOperationException();

            if (players[playerID].Ressources[rToGive] > 2)
            {
                players[playerID].RemoveRessource(rToGive, 3);
                players[playerID].AddRessource(rToReceive);
            }
        }

        /// <summary>
        /// Méthode qui permet l'echange avec un port spécialisé, apres verification que le joueur a bien acces a celui-ci.
        /// </summary>
        /// <remarks>Taux de change 2:1</remarks>
        /// <param name="playerID">L'identifiant du joueur faisant l'echange</param>
        /// <param name="rToGive">Les ressources que le joueur abandonne</param>
        /// <param name="rToReceive">Les ressources que le joueur reçoit</param>
        public void SpecializedHarborExchange(int playerID, RessourceType rToGive, RessourceType rToReceive)
        {
            // Verification que le joueur dispose d'un port specialise correspondant
            bool canUseHarbor = false;
            foreach (var cons in Players[playerID].Constructions)
            {
                if (cons.Item2 == ConstructionType.SETTLEMENT || cons.Item2 == ConstructionType.CITY)
                {
                    Intersection inter = gameGrid.GetIntersection(cons.Item1);
                    if (rToReceive == RessourceTradableByHarbor(inter.Harbor))
                        canUseHarbor = true;
                }
            }
            if (!canUseHarbor)
                throw new InvalidOperationException();

            if (players[playerID].Ressources[rToGive] > 1)
            {
                players[playerID].RemoveRessource(rToGive, 2);
                players[playerID].AddRessource(rToReceive);
            }
        }

        /// <summary>
        /// Méthode qui fait utiliser une carte monopôle.
        /// </summary>
        /// <remarks>La carte oblige tous les autres joueurs à donner leurs ressources de type <paramref name="r"/> aux joueur courant</remarks>
        /// <param name="r">Le type de ressource à monopoliser</param>
        public void UseMonopolyCard(RessourceType r)
        {
            if (!CardUsedThisTurn && Players[CurrentPlayer].Cards[CardType.RESSOURCE_MONOPOLY] > 0)
            {
                Players[CurrentPlayer].Cards[CardType.RESSOURCE_MONOPOLY]--;

                foreach (Player p in players)
                {
                    if (p.Id != CurrentPlayer)
                    {
                        players[CurrentPlayer].AddRessource(r, p.Ressources[r]);
                        p.SetRessource(r, 0);
                    }
                }
                CardUsedThisTurn = true;
            }
        }

        // Utilise une carte de developpement Construction de route
        /// <summary>
        /// Méthode qui fait utiliser une carte construction de route.
        /// </summary>
        /// <remarks>La carte permet au joueurs de créer deux routes à deux coordonnées de son choix (dans les emplacements disponibles)</remarks>
        /// <param name="road1">L'emplacement de la première route</param>
        /// <param name="road2">L'emplacement de la seconde route</param>
        public void UseRoadConstructionCard(Coordinate road1, Coordinate road2)
        {
            if (!CardUsedThisTurn && Players[CurrentPlayer].Cards[CardType.ROAD_BUILDING] > 0)
            {
                Players[CurrentPlayer].Cards[CardType.ROAD_BUILDING]--;

                gameGrid.ConstructRoad(players[CurrentPlayer], road1, true);
                gameGrid.ConstructRoad(players[CurrentPlayer], road2, true);

                CardUsedThisTurn = true;
            }

        }

        // Utilise une carte de developpement Choix de Ressource
        /// <summary>
        /// Méthode qui fait utiliser une carte de récupération de ressource
        /// </summary>
        /// <remarks>La carte permet au joueur de récuper deux ressources de son choix</remarks>
        /// <param name="r1">La première ressource à récuperer</param>
        /// <param name="r2">La seconde ressource à récuperer</param>
        public void UseRessourcePairCard(RessourceType r1, RessourceType r2)
        {
            if (!CardUsedThisTurn && Players[CurrentPlayer].Cards[CardType.RESSOURCE_PAIR] > 0)
            {
                Players[CurrentPlayer].Cards[CardType.RESSOURCE_PAIR]--;

                players[CurrentPlayer].AddRessource(r1);
                players[CurrentPlayer].AddRessource(r2);

                CardUsedThisTurn = true;
            }

        }

        // Utilise une carte de developpement Chevalier (Si le joueur ne vole personne playerID a voler = 0)
        /// <summary>
        /// Méthode qui fait utiliser une carte chevalier
        /// </summary>
        /// <remarks>La carte permet de déplacer les brigands sur le terrain. Le joueur ayant joué la carte peut voler une ressource d'un adversaire adjacent aux brigands.</remarks>
        /// <param name="newThiefLocation">La nouvelle position des brigands</param>
        /// <param name="playerToStealFrom">L'identifiant du jouer à voler. Si elle vaut 0, le joueur ne volera personne.</param>
        public void UseKnightCard(Coordinate newThiefLocation, int playerToStealFrom = -1)
        {
            if (!CardUsedThisTurn && Players[CurrentPlayer].Cards[CardType.KNIGHT] > 0)
            {
                Player currentPlayer = players[CurrentPlayer];
                currentPlayer.Cards[CardType.KNIGHT]--;
                currentPlayer.knightCardsPlayed++;

                if (currentPlayer.knightCardsPlayed >= 3)
                {
                    bool foundGreatestArmy = false;
                    foreach (Player p in players)
                    {
                        if (p.HasGreatestArmy && p.knightCardsPlayed < currentPlayer.knightCardsPlayed)
                        {
                            p.HasGreatestArmy = false;
                            currentPlayer.HasGreatestArmy = true;

                            if (currentPlayer.VictoryPoints >= 10)
                            {
                                CalculateRankingsAndFinish();
                            }
                            foundGreatestArmy = true;
                        }
                    }

                    if (foundGreatestArmy == false)
                    {
                        currentPlayer.HasGreatestArmy = true;

                        if (currentPlayer.VictoryPoints >= 10)
                        {
                            CalculateRankingsAndFinish();
                        }
                    }
                }

                gameGrid.MoveThief(newThiefLocation);
                // Stealing
                if (playerToStealFrom > -1)
                    Steal(newThiefLocation, playerToStealFrom);

                CardUsedThisTurn = true;
            }

        }

        // TODO: Verifier que le joueur a voler ait au moins une carte

        /// <summary>
        /// Méthode qui fait déplacer les brigands, et voler les ressources d'un joueur.
        /// </summary>
        /// <param name="newThiefLocation">La nouvelle position des brigands</param>
        /// <param name="playerToStealFrom">>L'identifiant du jouer à voler.</param>
        public void Steal(Coordinate newThiefLocation, int playerToStealFrom = -1)
        {
            bool canSteal = false;
            List<IIntersection> intersects = gameGrid.GetIntersectionsFromTile(newThiefLocation);
            foreach (Intersection i in intersects)
            {
                if (i.PlayerId == playerToStealFrom)
                {
                    canSteal = true;
                    break;
                }
            }

            if (canSteal)
            {
                List<RessourceType> l = new List<RessourceType>();
                foreach (var RessourcesPerType in players[playerToStealFrom].Ressources)
                {
                    for (var i = 0; i < RessourcesPerType.Value; i++)
                    {
                        l.Add(RessourcesPerType.Key);
                    }
                }

                Random random = new Random();
                RessourceType randomRessource = l[random.Next(l.Count)];

                players[playerToStealFrom].RemoveRessource(randomRessource);
                players[CurrentPlayer].AddRessource(randomRessource);
            }
        }

        /// <summary>
        /// Méthode qui distribue les ressources en fonction d'une intersection.
        /// </summary>
        /// <remarks>Appelée uniquement lors de la première construction de l'initialisation de la partie</remarks>
        /// <param name="l">Une liste de case autour d'une intersection</param>
        public void DistributeInitialRessources(List<ITerrainTile> l)
        {
            foreach (ITerrainTile i in l)
            {
                Players[CurrentPlayer].AddRessource(RessourceProducedByTerrain(i.Type), 1);
            }
        }
        /// <summary>
        /// Méthode qui permet l'association d'un type de terrain à la ressource correspondante
        /// </summary>
        /// <param name="t">Le type de terrain</param>
        /// <returns>La ressource associée au terrain.</returns>
        private RessourceType RessourceProducedByTerrain(TerrainType t)
        {
            switch (t)
            {
                case TerrainType.PASTURE:
                    return RessourceType.WOOL;
                case TerrainType.MOUNTAINS:
                    return RessourceType.ORE;
                case TerrainType.FOREST:
                    return RessourceType.LUMBER;
                case TerrainType.FIELDS:
                    return RessourceType.WHEAT;
                case TerrainType.HILLS:
                    return RessourceType.BRICK;
                default:
                    return RessourceType.NONE;
            }
        }

        /// <inheritdoc/>
        public IList<(Coordinate, HarborType)> GetCurrentPlayerHarbors()
        {

            List<(Coordinate, HarborType)> res = new List<(Coordinate, HarborType)>();
            foreach (var c in Players[CurrentPlayer].Constructions)
            {
                if (c.Item1.D.Equals(Direction.UP) || c.Item1.D.Equals(Direction.DOWN))
                {
                    if (!GameGrid.Intersections[c.Item1].Harbor.Equals(HarborType.NONE))
                        res.Add((c.Item1, GameGrid.Intersections[c.Item1].Harbor));
                }
            }
            return res;
        }

        /// <summary>
        /// Méthode qui permet l'association d'un type de port à la ressource correspondante
        /// </summary>
        /// <param name="t">Le type de port</param>
        /// <returns>La ressource associée au port.</returns>
        private RessourceType RessourceTradableByHarbor(HarborType h)
        {
            switch (h)
            {
                case HarborType.WOOL:
                    return RessourceType.WOOL;
                case HarborType.ORE:
                    return RessourceType.ORE;
                case HarborType.LUMBER:
                    return RessourceType.LUMBER;
                case HarborType.WHEAT:
                    return RessourceType.WHEAT;
                case HarborType.BRICK:
                    return RessourceType.BRICK;
                default:
                    return RessourceType.NONE;
            }
        }

        /// <summary>
        /// Méthode ToString.
        /// </summary>
        /// <returns>Renvoie un string contenant la phase, le joueur courant et l'utilisation de carte.</returns>
        public override string ToString()
        {
            return "Etat de la partie:\nPhase:" + CurrentPhase + ", Tour du joueur:" + CurrentPlayer + ", Card Played:" + CardUsedThisTurn;
        }

        public void CalculateRankingsAndFinish()
        {
            List<(int, int)> playerPoints = new List<(int, int)>();
            for (var i = 0; i < players.Count; i++)
            {
                playerPoints.Add((i, players[i].VictoryPoints));
            }
            List<(int, int)> sorted = playerPoints.OrderByDescending(p => p.Item2).ToList();
            GameView.Instance.OnVictory(new VictoryInfoArgs(Id, this, sorted, false));
            this.CurrentPhase = GamePhase.ENDED;
        }

        public string Serialize()
        {
            /*List<string> localListPayers = new List<string>();
            foreach (Player player in Players)
            {
                localListPayers.Add(player.Serialize());
            }
            string localPlayers = Serialization.Serialize(localListPayers);

            StringGame game = new StringGame(this.Id, this.CurrentPlayer, this.CurrentPhase, this.CardUsedThisTurn);
            Console.WriteLine(this.Id.ToString());
            Console.WriteLine(CurrentPlayer.ToString());
            return "<Players>" + localPlayers + "</Players>" + "<GameGrid>" + this.GameGrid.Serialize() + "</GameGrid>" + Serialization.XMLSerialize(game) + game.GetStringGame();
            */
            StringGame game = new StringGame(this.Id, this.CurrentPlayer, this.CurrentPhase, this.CardUsedThisTurn, this.lastDice);
            string localPlayers = "<Players>";
            foreach (Player player in Players)
            {
                localPlayers += player.Serialize();
                localPlayers += "&";
            }
            localPlayers += "</Players>";

            return game.GetStringGame() + localPlayers+"<HexGrid>"+ gameGrid.Serialize() + "</HexGrid>";

        }

        public void Deserialize(string serializedClass)
        {
            int pFrom = serializedClass.IndexOf("<Players>") + "<Players>".Length;
            int pTo = serializedClass.LastIndexOf("</Players>");
            string subPlayers = serializedClass.Substring(pFrom, pTo - pFrom);

            char[] seps = { '&' };
            string[] parts = subPlayers.Split(seps);
            Players.Clear();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Length > 0)
                {
                    Player p = new Player();
                    p.Deserialize(parts[i]);
                    Players.Add(p);
                }
            }


            pFrom = serializedClass.IndexOf("<HexGrid>") + "<HexGrid>".Length;
            pTo = serializedClass.LastIndexOf("</HexGrid>");
            string subGrid = serializedClass.Substring(pFrom, pTo - pFrom);

            GameGrid.Deserialize(subGrid);
            /*List<string> localPlayers = new List<string>();
            Serialization.Deserialize(subPlayers, localPlayers);
            players.Clear();
            foreach (string player in localPlayers)
            {
                Player p = new Player();
                p.Deserialize(player);
                players.Add(p);
            }

            pFrom = serializedClass.IndexOf("<GameGrid>") + "<GameGrid>".Length;
            pTo = serializedClass.LastIndexOf("</GameGrid>");
            string subGrid = serializedClass.Substring(pFrom, pTo - pFrom);
            GameGrid.Deserialize(subGrid);*/


            StringGame game = new StringGame();
            this.Id = game.getId(serializedClass);
            this.CurrentPhase = game.getCurPH(serializedClass);
            this.CurrentPlayer = game.getCurGP(serializedClass);
            this.CardUsedThisTurn = game.getCardUsed(serializedClass);
            this.lastDice = game.getLastDice(serializedClass);
        }
    }

    public class StringGame
    {
        public Guid StringGameId { get; set; }

        public int StringGameCurrentPlayer { get; set; }
        public GamePhase StringGameCurrentPhase { get; set; }
        public bool StringGameCardUsedThisTurn { get; set; }
        public (int,int) lastDice { get; set; }
        public StringGame()
        {
            StringGameId = Guid.Empty;
            StringGameCurrentPlayer = 0;
            StringGameCurrentPhase = GamePhase.INITIAL_BUILDING_1;
            StringGameCardUsedThisTurn = false;
        }
        public StringGame(Guid gameId, int currentPlayer, GamePhase currentPhase, bool cardUsedThisTurn,(int,int) last)
        {
            StringGameId = gameId;
            StringGameCurrentPlayer = currentPlayer;
            StringGameCurrentPhase = currentPhase;
            StringGameCardUsedThisTurn = cardUsedThisTurn;
            lastDice=last;
        }

        public string GetStringGame()
        {
            string chain = "";
            chain += "<GameId>" + StringGameId.ToString() + "</GameId>";
            chain += "<GameCurrentPlayer>" + StringGameCurrentPlayer.ToString() + "</GameCurrentPlayer>";
            chain += "<GameCurrentPhase>" + StringGameCurrentPhase.ToString() + "</GameCurrentPhase>";
            chain += "<GameCardUsedThisTurn>" + StringGameCardUsedThisTurn.ToString() + "</GameCardUsedThisTurn>";
            chain += "<lastDice>" + lastDice.Item1.ToString() + "+" + lastDice.Item2.ToString()+"</lastDice>";
            return chain;
        }

        public Guid getId(string serializedClass)
        {
            int pFrom = serializedClass.IndexOf("<GameId>") + "<GameId>".Length;
            int pTo = serializedClass.LastIndexOf("</GameId>");
            string id = serializedClass.Substring(pFrom, pTo - pFrom);
            if (id.Contains("00000000-0000-0000-0000-000000000000")) return Guid.Empty;
            return new Guid(id);
        }

        public int getCurGP(string serializedClass)
        {
            int pFrom = serializedClass.IndexOf("<GameCurrentPlayer>") + "<GameCurrentPlayer>".Length;
            int pTo = serializedClass.LastIndexOf("</GameCurrentPlayer>");
            string x = serializedClass.Substring(pFrom, pTo - pFrom);
            return Int32.Parse(x);
        }

        public GamePhase getCurPH(string serializedClass)
        {
            int pFrom = serializedClass.IndexOf("<GameCurrentPhase>") + "<GameCurrentPhase>".Length;
            int pTo = serializedClass.LastIndexOf("</GameCurrentPhase>");
            string x = serializedClass.Substring(pFrom, pTo - pFrom);
            return Callbacks.GetGamePhase(x);
        }

        public bool getCardUsed(string serializedClass)
        {
            int pFrom = serializedClass.IndexOf("<GameCardUsedThisTurn>") + "<GameCardUsedThisTurn>".Length;
            int pTo = serializedClass.LastIndexOf("</GameCardUsedThisTurn>");
            string x = serializedClass.Substring(pFrom, pTo - pFrom);
            Console.WriteLine(x);
            return bool.Parse(x);
        }

        public (int,int) getLastDice(string serializedClass)
        {
            int pFrom = serializedClass.IndexOf("<lastDice>") + "<lastDice>".Length;
            int pTo = serializedClass.LastIndexOf("</lastDice>");
            string x = serializedClass.Substring(pFrom, pTo - pFrom);
            char[] seps = { '+' };
            string[] parts = x.Split(seps);
            return (Int32.Parse(parts[0]),Int32.Parse(parts[1]));

        }
    }
}

