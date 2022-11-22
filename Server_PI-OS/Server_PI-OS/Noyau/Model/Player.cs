using System;
using System.Collections.Generic;
using System.Text;
using Noyau.View;
using Util.View;

namespace Noyau.Model
{
    /// <summary>
    /// <para> La classe Player.</para>
    /// <para> Représente un joueur de la partie.</para>
    /// <para> Contient les méthodes et les propriétés nécessaires à l'implémentation d'un joueur.</para>
    /// </summary>
    public class Player : IPlayer
    {
        /// <inheritdoc/>
        public int Id { get; set; }

        /// <inheritdoc/>
        public bool IsIA { get; set; }

        /// <inheritdoc/>
        public int VictoryPoints { get { return RecalculatePoints(); } set { } }

        /// <inheritdoc/>
        public bool HasLongestRoad { get; set; }

        /// <inheritdoc/>
        public int KnightCardsPlayed { get { return knightCardsPlayed; } }
        public int knightCardsPlayed;

        /// <inheritdoc/>
        public bool HasGreatestArmy { get; set; }

        /// <inheritdoc/>
        public IDictionary<RessourceType, int> Ressources { get; set; }

        /// <inheritdoc/>
        public int TotalRessourceNumber
        {
            get
            {
                int totalCards = 0;
                foreach (var entry in Ressources)
                {
                    totalCards += entry.Value;
                }

                return totalCards;
            }
            set { }
        }

        /// <inheritdoc/>
        public IDictionary<CardType, int> Cards { get; set; }

        /// <inheritdoc/>
        public IList<CardType> CardsBoughtThisTurn { get; set; }

        /// <inheritdoc/>
        public IList<(Coordinate, ConstructionType)> Constructions { get; set; }

        /// <inheritdoc/>
        public int nbColony
        {
            get
            {
                int total = 0;
                foreach (var entry in Constructions)
                {
                    if (entry.Item2.Equals(ConstructionType.SETTLEMENT))
                        total++;
                }
                return total;
            }
            set { }
        }

        /// <inheritdoc/>
        public int nbRoad
        {
            get
            {
                int total = 0;
                foreach (var entry in Constructions)
                {
                    if (entry.Item2.Equals(ConstructionType.ROAD))
                        total++;
                }
                return total;
            }
            set { }
        }

        /// <inheritdoc/>
        public int nbCity
        {
            get
            {
                int total = 0;
                foreach (var entry in Constructions)
                {
                    if (entry.Item2.Equals(ConstructionType.CITY))
                        total++;
                }
                return total;
            }
            set { }
        }

        /// <summary>
        /// <para>Constructeur de Player</para>
        /// <para>Ne prend que l'identifiant du joueur, initialise le reste.</para>
        /// </summary>
        /// <param name="id">L'identifiant du player</param>
        public Player(int id, bool isIA)
        {
            Id = id;
            IsIA = isIA;
            HasLongestRoad = false;
            HasGreatestArmy = false;
            knightCardsPlayed = 0;
            Ressources = new Dictionary<RessourceType, int>();
            foreach (RessourceType r in Enum.GetValues(typeof(RessourceType)))
            {
                if (r != RessourceType.NONE)
                    Ressources.Add(r, 0);
            }
            Cards = new Dictionary<CardType, int>();
            foreach (CardType c in Enum.GetValues(typeof(CardType)))
            {
                Cards.Add(c, 0);
            }
            CardsBoughtThisTurn = new List<CardType>();
            Constructions = new List<(Coordinate, ConstructionType)>();
        }

        public Player()
        {
            Ressources = new Dictionary<RessourceType, int>();
            Cards = new Dictionary<CardType, int>();
            CardsBoughtThisTurn = new List<CardType>();
            Constructions = new List<(Coordinate, ConstructionType)>();
        }
        /// <summary>
        /// <para>Méthode permettant d'acheter une carte.</para>
        /// <para>Enlève la quantité de ressource nécessaire au joueur, et rajoute une carte développement</para>
        /// </summary>
        public void BuyCard()
        {
            if (Ressources[RessourceType.WHEAT] > 0 && Ressources[RessourceType.WOOL] > 0 && Ressources[RessourceType.ORE] > 0)
            {
                Ressources[RessourceType.WHEAT]--;
                Ressources[RessourceType.WOOL]--;
                Ressources[RessourceType.ORE]--;
                // Carte au hasard parmi les types de cartes
                Array values = Enum.GetValues(typeof(CardType));
                Random random = new Random();
                CardType randomCard = (CardType)values.GetValue(random.Next(0, values.Length));
                CardsBoughtThisTurn.Add(randomCard);

            }
        }

        /// <summary>
        /// Ajoute un nombre <paramref name="num"/> de ressources du type <paramref name="r"/>
        /// </summary>
        /// <param name="r">Type de ressource</param>
        /// <param name="num">Nombre a rajouter</param>
        public void AddRessource(RessourceType r, int num = 1)
        {
            if (r != RessourceType.NONE)
                Ressources[r] += num;
        }


        /// <summary>
        /// Supprime un nombre <paramref name="num"/> de ressources du type <paramref name="r"/>
        /// </summary>
        /// <param name="r">Type de ressource</param>
        /// <param name="num">Nombre a rajouter</param>
        public void RemoveRessource(RessourceType r, int num = 1)
        {

            if (r != RessourceType.NONE)
                Ressources[r] -= num;
        }

        /// <summary>
        /// change un nombre <paramref name="num"/> de ressources du type <paramref name="r"/>
        /// </summary>
        /// <param name="r">Type de ressource</param>
        /// <param name="num">Nouveau nombre</param>
        public void SetRessource(RessourceType r, int num)
        {
            if (r != RessourceType.NONE)
                Ressources[r] = num;
        }

        /// <summary>
        /// Recalcule les points de victoire actuels du joueur (en fonction de ses constructions et cartes)
        /// </summary>
        /// <returns> Le nombre total de pts</returns>
        public int RecalculatePoints()
        {
            int totalScore = 0;
            totalScore += Cards[CardType.VICTORY_POINT];
            foreach (CardType ct in CardsBoughtThisTurn)
            {
                if (ct == CardType.VICTORY_POINT)
                    totalScore++;
            }
            foreach (var construct in Constructions)
            {
                if (construct.Item2 == ConstructionType.SETTLEMENT)
                    totalScore++;
                else if (construct.Item2 == ConstructionType.CITY)
                    totalScore += 2;
            }
            if (HasGreatestArmy)
                totalScore += 2;
            if (HasLongestRoad)
                totalScore += 2;

            return totalScore;
        }

        /// <inheritdoc/>
        public bool HasEnoughRessources(ConstructionType ct)
        {
            if (ct.Equals(ConstructionType.SETTLEMENT))
            {
                return Ressources[RessourceType.LUMBER] > 0 && Ressources[RessourceType.BRICK] > 0 &&
                    Ressources[RessourceType.WOOL] > 0 && Ressources[RessourceType.WHEAT] > 0;
            }
            else if (ct.Equals(ConstructionType.CITY))
            {
                return Ressources[RessourceType.WHEAT] > 1 && Ressources[RessourceType.ORE] > 2;
            }
            else if (ct.Equals(ConstructionType.ROAD))
            {
                return Ressources[RessourceType.LUMBER] > 0 && Ressources[RessourceType.BRICK] > 0;
            }
            else if (ct.Equals(ConstructionType.DEVELOPMENT_CARD))
            {
                bool canBuild = Ressources[RessourceType.WHEAT] > 0 && Ressources[RessourceType.ORE] > 0 && Ressources[RessourceType.WOOL] > 0;
                return canBuild;
            }
            else
            {
                throw new ArgumentException("Construction is not of valid type");
            }
        }

        /// <summary>
        /// <para>Defausse des ressources d'un joueur, s'il en a 8 ou plus. </para>
        /// <para>A appeler en cas d'un jet de 7 durant la phase de recolte.</para>
        /// </summary>
        /// <param name="discard">List contenant le nombre de ressource à supprimer de chaque type</param>
        public void DiscardRessources(List<(RessourceType rType, int num)> discard)
        {

            int totalCards = TotalRessourceNumber;

            int totalDiscards = 0;
            // verification que le joueur a bien les ressources qu'il souhaite defausser
            foreach (var entry in discard)
            {
                totalDiscards += entry.num;
                if (Ressources[entry.rType] < entry.num)
                {
                    return;
                }

            }

            int expectedCardNumberAfterDiscard = totalCards / 2 + totalCards % 2;
            if (totalCards > 7 && (totalCards - totalDiscards) == expectedCardNumberAfterDiscard)
                foreach ((RessourceType r, int num) d in discard)
                {
                    Ressources[d.r] -= d.num;
                }
        }



        /// <summary>
        /// Méthode appelée à chaque fin de tour, rajoute les cartes achetés pendant le tour à la liste de cartes jouables.
        /// </summary>
        public void EndTurn()
        {
            while (CardsBoughtThisTurn.Count != 0)
            {
                Cards[CardsBoughtThisTurn[0]]++;
                CardsBoughtThisTurn.RemoveAt(0);
            }
        }

        public int LongestRoadLength()
        {
            int longest = 0;
            List<Coordinate> roads = new List<Coordinate>();
            foreach (var c in Constructions)
            {
                if (c.Item2 == ConstructionType.ROAD)
                    roads.Add(c.Item1);
            }

            foreach (var road in roads)
            {
                List<Coordinate> otherRoads = new List<Coordinate>(roads);
                otherRoads.Remove(road);
                int localLongest = 1;

                var begin = road;
                var end = road;
                bool beginModified = true;
                bool endModified = false;

                while (beginModified || endModified)
                {
                    beginModified = false;
                    endModified = false;
                    Coordinate roadAdded = null;
                    foreach (var otherRoad in otherRoads)
                    {
                        if (!beginModified && !endModified)
                        {
                            if (CanAppend(begin, otherRoad))
                            {
                                begin = otherRoad;
                                beginModified = true;
                                localLongest++;
                                roadAdded = otherRoad;
                            }
                            else if (CanAppend(end, otherRoad))
                            {
                                end = otherRoad;
                                endModified = true;
                                localLongest++;
                                roadAdded = otherRoad;
                            }
                        }
                    }
                    if (roadAdded != null)
                        otherRoads.Remove(roadAdded);
                }

                if (localLongest > longest)
                    longest = localLongest;
            }

            return longest;
        }

        public bool CanAppend(Coordinate firstRoad, Coordinate secondRoad)
        {
            if (firstRoad.D == Direction.NORTH_EAST)
            {
                if (firstRoad.X == secondRoad.X)
                {
                    if (firstRoad.Y == secondRoad.Y && firstRoad.Z == secondRoad.Z)
                        return (secondRoad.D == Direction.EAST);
                    else if (secondRoad.Y == (firstRoad.Y + 1) && secondRoad.Z == (firstRoad.Z - 1))
                    {
                        return (secondRoad.D == Direction.EAST || secondRoad.D == Direction.SOUTH_EAST);
                    }
                }
                else if (firstRoad.Y == secondRoad.Y)
                {
                    if (secondRoad.X == (firstRoad.X + 1) && secondRoad.Z == (firstRoad.Z - 1))
                    {
                        return (secondRoad.D == Direction.SOUTH_EAST);
                    }
                }
            }
            else if (firstRoad.D == Direction.EAST)
            {
                if (firstRoad.X == secondRoad.X && firstRoad.Y == secondRoad.Y && firstRoad.Z == secondRoad.Z)
                {
                    return (secondRoad.D == Direction.NORTH_EAST || secondRoad.D == Direction.SOUTH_EAST);
                }
                else if ((firstRoad.X + 1) == secondRoad.X && firstRoad.Y == secondRoad.Y && (firstRoad.Z - 1) == secondRoad.Z)
                {
                    return (secondRoad.D == Direction.SOUTH_EAST);
                }
                else if (firstRoad.X == secondRoad.X && (firstRoad.Y - 1) == secondRoad.Y && (firstRoad.Z + 1) == secondRoad.Z)
                {
                    return (secondRoad.D == Direction.NORTH_EAST);
                }
            }
            else if (firstRoad.D == Direction.SOUTH_EAST)
            {
                if (firstRoad.X == secondRoad.X && firstRoad.Y == secondRoad.Y && firstRoad.Z == secondRoad.Z)
                {
                    return (secondRoad.D == Direction.EAST);
                }
                else if ((firstRoad.X - 1) == secondRoad.X && firstRoad.Y == secondRoad.Y && (firstRoad.Z + 1) == secondRoad.Z)
                {
                    return (secondRoad.D == Direction.EAST || secondRoad.D == Direction.NORTH_EAST);
                }
                else if (firstRoad.X == secondRoad.X && (firstRoad.Y - 1) == secondRoad.Y && (firstRoad.Z + 1) == secondRoad.Z)
                {
                    return (secondRoad.D == Direction.NORTH_EAST);
                }
            }

            return false;
        }

        public string Serialize()
        {
            string chain = "";
            chain += "<IdPlayer>" + Id.ToString() + "</IdPlayer>";
            chain += "<IsIA>" + IsIA.ToString() + "</IsIA>";
            chain += "<VictoryPoints>" + VictoryPoints.ToString() + "</VictoryPoints>";
            chain += "<HasLongestRoad>" + HasLongestRoad.ToString() + "</HasLongestRoad>";
            chain += "<HasGreatestArmy>" + HasGreatestArmy.ToString() + "</HasGreatestArmy>";
            chain += "<TotalRessourceNumber>" + TotalRessourceNumber.ToString() + "</TotalRessourceNumber>";
            chain += "<nbColony>" + nbColony.ToString() + "</nbColony>";
            chain += "<nbRoad>" + nbRoad.ToString() + "</nbRoad>";
            chain += "<nbCity>" + nbCity.ToString() + "</nbCity>";
            chain += "<KnightCardsPlayed>" + knightCardsPlayed.ToString() + "</KnightCardsPlayed>";

            //Dictionnaire de Ressources:
            chain += "<Ressources>";
            foreach (KeyValuePair<RessourceType, int> entry in Ressources)
            {
                chain += entry.Key.ToString() + "#" + entry.Value.ToString() + "|";
            }
            chain += "</Ressources><Cards>";
            foreach (KeyValuePair<CardType, int> entry in Cards)
            {
                chain += entry.Key.ToString() + "#" + entry.Value.ToString() + "|";
            }
            chain += "</Cards><CardsBoughtThisTurn>";
            foreach (CardType entry in CardsBoughtThisTurn)
            {
                chain += entry.ToString() + "|";
            }
            chain += "</CardsBoughtThisTurn><Constructions>";
            foreach ((Coordinate, ConstructionType) entry in Constructions)
            {
                chain += entry.Item1.Serialize() + "#" + entry.Item2.ToString() + "|";
            }
            chain += "</Constructions>";
            return chain;
        }
        public void Deserialize(string serializedClass)
        {
            StringPlayer player = new StringPlayer();

            this.Id = player.GetId(serializedClass);
            this.IsIA = player.GetIsIA(serializedClass);
            this.VictoryPoints = player.GetVictoryPoints(serializedClass);
            this.HasLongestRoad = player.GetHasLongestRoad(serializedClass);
            this.HasGreatestArmy = player.GetHasGreatestArmy(serializedClass);
            this.TotalRessourceNumber = player.GetTotalRessourceNumber(serializedClass);
            this.nbColony = player.GetnbColony(serializedClass);
            this.nbRoad = player.GetnbRoad(serializedClass);
            this.nbCity = player.GetnbCity(serializedClass);
            this.knightCardsPlayed = player.GetKnighCardsPlayed(serializedClass);

            string str = player.GetRessources(serializedClass);
            char[] seps = { '|' };
            char[] subSeps = { '#' };
            string[] parts = str.Split(seps);
            Ressources.Clear();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Length > 0)
                {
                    string[] subParts = parts[i].Split(subSeps);
                    if (subParts.Length > 0)
                        if (!Ressources.ContainsKey(Callbacks.GetRessourceType(subParts[0])))
                            Ressources.Add(Callbacks.GetRessourceType(subParts[0]), Int32.Parse(subParts[1]));
                }
            }
            str = player.GetCards(serializedClass);
            parts = str.Split(seps);
            Cards.Clear();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Length > 0)
                {
                    string[] subParts = parts[i].Split(subSeps);
                    if (subParts.Length > 0)
                    {
                        if (!Cards.ContainsKey(Callbacks.GetCardType(subParts[0])))
                            Cards.Add(Callbacks.GetCardType(subParts[0]), Int32.Parse(subParts[1]));
                    }

                }
            }

            str = player.GetCardsBoughtThisTurn(serializedClass);
            parts = str.Split(seps);
            CardsBoughtThisTurn.Clear();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Length > 0)
                {
                    CardsBoughtThisTurn.Add(Callbacks.GetCardType(parts[i]));
                }
            }
            str = player.GetConstructions(serializedClass);
            parts = str.Split(seps);
            Constructions.Clear();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Length > 0)
                {
                    string[] subParts = parts[i].Split(subSeps);
                    if (subParts.Length > 0)
                    {
                        Coordinate c = new Coordinate();
                        c.Deserialize(subParts[0]);
                        Constructions.Add((c, Callbacks.GetConstructionype(subParts[1])));
                    }
                }
            }
        }

        private void Update(Dictionary<string, int> ressources, Dictionary<string, int> cards, List<string> cardsBoughtThisTurn, List<(Coordinate, string)> constructions)
        {
            Ressources.Clear();
            foreach (KeyValuePair<string, int> entry in ressources)
            {
                Ressources.Add(Callbacks.GetRessourceType(entry.Key), entry.Value);
            }
            Cards.Clear();
            foreach (KeyValuePair<string, int> entry in cards)
            {
                Cards.Add(Callbacks.GetCardType(entry.Key), entry.Value);
            }
            CardsBoughtThisTurn.Clear();
            foreach (string card in cardsBoughtThisTurn)
            {
                CardsBoughtThisTurn.Add(Callbacks.GetCardType(card));
            }
            Constructions.Clear();
            foreach ((Coordinate, string) couple in constructions)
            {
                Constructions.Add((couple.Item1, Callbacks.GetConstructionype(couple.Item2)));
            }

        }



    }
}


public class StringPlayer
{
    public int Id { get; set; }
    public bool IsIA { get; set; }
    public int VictoryPoints { get; set; }
    public bool HasLongestRoad { get; set; }
    public bool HasGreatestArmy { get; set; }
    public int TotalRessourceNumber { get; set; }
    public int nbColony { get; set; }
    public int nbRoad { get; set; }
    public int nbCity { get; set; }
    public int KnighCardsPlayed { get; set; }

    public StringPlayer() { }
    public StringPlayer(int id, bool isIA, int victoryPoints, bool hasLongestRoad, bool hasGreatestArmy, int totalRessourceNumber, int nbcol, int nbroa, int nbci, int knight)
    {
        Id = id;
        IsIA = isIA;
        VictoryPoints = victoryPoints;
        HasLongestRoad = hasLongestRoad;
        HasGreatestArmy = hasGreatestArmy;

        TotalRessourceNumber = totalRessourceNumber;
        nbColony = nbcol;
        nbCity = nbci;
        nbRoad = nbroa;
        KnighCardsPlayed = knight;
    }
    public int GetId(string serializedClass)
    {
        int pFrom = serializedClass.IndexOf("<IdPlayer>") + "<IdPlayer>".Length;
        int pTo = serializedClass.LastIndexOf("</IdPlayer>");
        string id = serializedClass.Substring(pFrom, pTo - pFrom);
        return Int32.Parse(id);
    }

    public bool GetIsIA(string serializedClass)
    {
        int pFrom = serializedClass.IndexOf("<IsIA>") + "<IsIA>".Length;
        int pTo = serializedClass.LastIndexOf("</IsIA>");
        string x = serializedClass.Substring(pFrom, pTo - pFrom);
        return bool.Parse(x);
    }
    public int GetVictoryPoints(string serializedClass)
    {
        int pFrom = serializedClass.IndexOf("<VictoryPoints>") + "<VictoryPoints>".Length;
        int pTo = serializedClass.LastIndexOf("</VictoryPoints>");
        string x = serializedClass.Substring(pFrom, pTo - pFrom);
        return Int32.Parse(x);
    }
    public bool GetHasLongestRoad(string serializedClass)
    {
        int pFrom = serializedClass.IndexOf("<HasLongestRoad>") + "<HasLongestRoad>".Length;
        int pTo = serializedClass.LastIndexOf("</HasLongestRoad>");
        string x = serializedClass.Substring(pFrom, pTo - pFrom);
        return bool.Parse(x);
    }
    public bool GetHasGreatestArmy(string serializedClass)
    {
        int pFrom = serializedClass.IndexOf("<HasGreatestArmy>") + "<HasGreatestArmy>".Length;
        int pTo = serializedClass.LastIndexOf("</HasGreatestArmy>");
        string x = serializedClass.Substring(pFrom, pTo - pFrom);
        return bool.Parse(x);
    }
    public int GetTotalRessourceNumber(string serializedClass)
    {
        int pFrom = serializedClass.IndexOf("<TotalRessourceNumber>") + "<TotalRessourceNumber>".Length;
        int pTo = serializedClass.LastIndexOf("</TotalRessourceNumber>");
        string x = serializedClass.Substring(pFrom, pTo - pFrom);
        return Int32.Parse(x);
    }
    public int GetnbColony(string serializedClass)
    {
        int pFrom = serializedClass.IndexOf("<nbColony>") + "<nbColony>".Length;
        int pTo = serializedClass.LastIndexOf("</nbColony>");
        string x = serializedClass.Substring(pFrom, pTo - pFrom);
        return Int32.Parse(x);
    }
    public int GetnbRoad(string serializedClass)
    {
        int pFrom = serializedClass.IndexOf("<nbRoad>") + "<nbRoad>".Length;
        int pTo = serializedClass.LastIndexOf("</nbRoad>");
        string x = serializedClass.Substring(pFrom, pTo - pFrom);
        return Int32.Parse(x);
    }
    public int GetnbCity(string serializedClass)
    {
        int pFrom = serializedClass.IndexOf("<nbCity>") + "<nbCity>".Length;
        int pTo = serializedClass.LastIndexOf("</nbCity>");
        string x = serializedClass.Substring(pFrom, pTo - pFrom);
        return Int32.Parse(x);
    }
    public int GetKnighCardsPlayed(string serializedClass)
    {
        int pFrom = serializedClass.IndexOf("<KnightCardsPlayed>") + "<KnightCardsPlayed>".Length;
        int pTo = serializedClass.LastIndexOf("</KnightCardsPlayed>");
        string x = serializedClass.Substring(pFrom, pTo - pFrom);
        return Int32.Parse(x);
    }

    public string GetRessources(string serializedClass)
    {
        int pFrom = serializedClass.IndexOf("<Ressources>") + "<Ressources>".Length;
        int pTo = serializedClass.LastIndexOf("</Ressources>");
        string x = serializedClass.Substring(pFrom, pTo - pFrom);
        return x;
    }

    public string GetCards(string serializedClass)
    {
        int pFrom = serializedClass.IndexOf("<Cards>") + "<Cards>".Length;
        int pTo = serializedClass.LastIndexOf("</Cards>");
        string x = serializedClass.Substring(pFrom, pTo - pFrom);
        return x;
    }

    public string GetCardsBoughtThisTurn(string serializedClass)
    {
        int pFrom = serializedClass.IndexOf("<CardsBoughtThisTurn>") + "<CardsBoughtThisTurn>".Length;
        int pTo = serializedClass.LastIndexOf("</CardsBoughtThisTurn>");
        string x = serializedClass.Substring(pFrom, pTo - pFrom);
        return x;
    }

    public string GetConstructions(string serializedClass)
    {
        int pFrom = serializedClass.IndexOf("<Constructions>") + "<Constructions>".Length;
        int pTo = serializedClass.LastIndexOf("</Constructions>");
        string x = serializedClass.Substring(pFrom, pTo - pFrom);
        return x;
    }



}



