using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Network.Model;
using Noyau.View;

namespace Network.Controller
{
    public static class ClientManager
    {
        /// <summary>
        /// <para>La classe ClientManager</para>
        /// <para>Contient les clients connectés de façon statique</para>
        /// </summary>

        public static Dictionary<Client, Guid> TCPClients = new Dictionary<Client, Guid>();

        public static Dictionary<Guid, int> IAsInGames = new Dictionary<Guid, int>();

        public static Dictionary<Guid, int> DiscardCounter = new Dictionary<Guid, int>();

        public static Dictionary<(Guid, int), List<(RessourceType rType, int num)>> Discards = new System.Collections.Generic.Dictionary<(Guid, int), List<(RessourceType rType, int num)>>();
        public static void addClient(Guid val, Client key)
        {
            if (!TCPClients.ContainsKey(key))
                TCPClients.Add(key, val);
        }

        public static void addIA(Guid key, int val)
        {
            if (!IAsInGames.ContainsKey(key))
                IAsInGames.Add(key, val);
        }

        public static void Increase(Guid key)
        {
            if (!DiscardCounter.ContainsKey(key))
            {
                DiscardCounter.Add(key, 1);
            }

            else
            {
                DiscardCounter[key] = DiscardCounter[key] + 1;
            }
        }

        public static int GetIAsInGame(Guid key)
        {
            if (IAsInGames.ContainsKey(key)) return IAsInGames[key];
            else return 0;
        }

        public static void Reset(Guid key)
        {
            if (DiscardCounter.ContainsKey(key))
                DiscardCounter[key] = 0;

        }

        public static void addDiscrad(Guid id1, int id2, List<(RessourceType rType, int num)> discard)
        {
            if (!Discards.ContainsKey((id1, id2)))
            {
                Discards.Add((id1, id2), discard);
            }
        }

        public static void removeDiscrad(Guid id1, int id2)
        {
            if (Discards.ContainsKey((id1, id2)))
                Discards.Remove((id1, id2));
        }

        public static int NBDiscrad(Guid id1)
        {
            int cpt = 0;
            foreach (KeyValuePair<Guid, int> c in DiscardCounter)
            {
                if (c.Key == id1) cpt = c.Value;
            }
            return cpt;
        }

        public static List<(int idPlayer, List<(RessourceType rType, int num)>)> GetListDiscards(Guid id)
        {
            List<(int idPlayer, List<(RessourceType rType, int num)>)> list = new List<(int idPlayer, List<(RessourceType rType, int num)>)>();
            foreach (KeyValuePair<(Guid, int), List<(RessourceType rType, int num)>> c in Discards)
            {
                if (c.Key.Item1 == id)
                {
                    list.Add((c.Key.Item2, c.Value));
                }

            }
            return list;
        }

        public static void RemoveDiscards(Guid id)
        {
            foreach (KeyValuePair<(Guid, int), List<(RessourceType rType, int num)>> c in Discards)
            {
                if (c.Key.Item1 == id)
                {
                    removeDiscrad(id, c.Key.Item2);
                }

            }
        }
    }

}
