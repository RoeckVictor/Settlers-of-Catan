using Noyau.Model;
using Noyau.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;


namespace Util.View
{
    /// <summary>
    /// <para>La classe Seralization</para>
    /// <para>Contient l'ensemble des méthodes pour sérialiser les listes, dictionnaires ou classes envoyées entre le serveur et le client</para>
    /// </summary>

    static class Serialization
    {
        /// <summary>
        /// Sérialise les classes de base
        /// </summary>
        /// <param name="pObject">Objet à sérialiser</param>
        /// <returns>renvoie la chaîne de l'objet sérialisé</returns>
        public static string XMLSerialize(object pObject)
        {
            try
            {
                XmlSerializer objXMLSerilize = new XmlSerializer(pObject.GetType());
                using (StringWriter stream = new StringWriter())
                {
                    objXMLSerilize.Serialize(stream, pObject); stream.Flush();
                    return stream.ToString();
                }
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Désérialise les classes de bases
        /// </summary>
        /// <param name="pObject">L'objet dans lequel on désérialise</param>
        /// <param name="strXMLString">La chaîne à désérialiser</param>
        /// <returns>Renvoie l'objet désérialisé</returns>
        public static object XMLDeSerialize(object pObject, string strXMLString)
        {
            try
            {
                XmlSerializer objXMLSerilize = new XmlSerializer(pObject.GetType());
                using (StringReader stream = new StringReader(strXMLString))
                {
                    return objXMLSerilize.Deserialize(stream);
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Failed to create object from xml string ", ex);
            }
        }

        /// <summary>
        /// Récupère le nom de la classe
        /// </summary>
        /// <param name="serialized">Objet sérialisé</param>
        /// <returns>Le nom de la classe</returns>
        public static string ClassName(string serialized)
        {
            int pFrom = serialized.IndexOf("<Name>") + "<Name>".Length;
            int pTo = serialized.LastIndexOf("</Name>");
            return serialized.Substring(pFrom, pTo - pFrom);
        }
        /// <summary>
        /// Récupère l'étendue du boradcast
        /// </summary>
        /// <param name="serialized">L'objet sérialisé</param>
        /// <returns>Vrai si l'étendue est pour tous les joueurs de la partie, faux si c'est un évenement interne au serveur</returns>
        public static bool Scope(string serialized)
        {
            int pFrom = serialized.IndexOf("<Broadcast>") + "<Broadcast>".Length;
            int pTo = serialized.LastIndexOf("</Broadcast>");
            return (serialized.Substring(pFrom, pTo - pFrom).Equals("True") ? true : false);
        }


        /// <summary>
        /// Sérailise un dictionnaire de <string, int> 
        /// </summary>
        /// <param name="dictionary">Le dictionnaire à sérialiser</param>
        /// <returns>dictionnaire sérialisé</returns>
        public static string Serialize(Dictionary<string, int> dictionary)
        {
            if (dictionary.Count != 0)
            {
                List<Entry> entries = new List<Entry>(dictionary.Count);
                foreach (string key in dictionary.Keys)
                {
                    entries.Add(new Entry(key, dictionary[key]));
                }

                XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
                using (StringWriter stream = new StringWriter())
                {
                    serializer.Serialize(stream, entries);
                    return stream.ToString();
                }
            }
            return "NULL";
        }
        /// <summary>
        /// Sérialise les TerainTiles de la classe Hexgrid
        /// </summary>
        /// <param name="dictionary">Le dictionnaire à sérialiser</param>
        /// <returns>dictionnaire sérialisé</returns>
        public static string Serialize(Dictionary<Coordinate, TerrainTile> dictionary)
        {
            if (dictionary.Count != 0)
            {
                List<Entry> entries = new List<Entry>(dictionary.Count);
                foreach (Coordinate key in dictionary.Keys)
                {
                    entries.Add(new Entry(key, dictionary[key]));
                }

                XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
                using (StringWriter stream = new StringWriter())
                {
                    serializer.Serialize(stream, entries);
                    return stream.ToString();
                }
            }
            return "NULL";
        }
        /// <summary>
        /// Sérialise les Edges de HexGrid
        /// </summary>
        /// <param name="dictionary">Le dictionnaire à sérialiser</param>
        /// <returns>dictionnaire sérialisé</returns>
        public static string Serialize(Dictionary<Coordinate, Edge> dictionary)
        {
            if (dictionary.Count != 0)
            {
                List<Entry> entries = new List<Entry>(dictionary.Count);
                foreach (Coordinate key in dictionary.Keys)
                {
                    entries.Add(new Entry(key, dictionary[key]));
                }

                XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
                using (StringWriter stream = new StringWriter())
                {
                    serializer.Serialize(stream, entries);
                    return stream.ToString();
                }

            }
            return "NULL";
        }
        /// <summary>
        /// Sérialise les Intersections de HexGrid
        /// </summary>
        /// <param name="dictionary">Le dictionnaire à sérialiser</param>
        /// <returns>dictionnaire sérialisé</returns>
        public static string Serialize(Dictionary<Coordinate, Intersection> dictionary)
        {
            if (dictionary.Count != 0)
            {
                List<Entry> entries = new List<Entry>(dictionary.Count);
                foreach (Coordinate key in dictionary.Keys)
                {
                    entries.Add(new Entry(key, dictionary[key]));
                }

                XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
                using (StringWriter stream = new StringWriter())
                {
                    serializer.Serialize(stream, entries);
                    return stream.ToString();
                }
            }
            return "NULL";
        }

        /// <summary>
        /// Sérialise une liste de chaines
        /// </summary>
        /// <param name="list">la liste à sérialiser</param>
        /// <returns>la liste sérialisée</returns>
        public static string Serialize(List<string> list)
        {
            if (list.Count != 0)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
                using (StringWriter stream = new StringWriter())
                {
                    serializer.Serialize(stream, list);
                    return stream.ToString();
                }
            }
            return "NULL";
        }

        /// <summary>
        /// Sérialise une liste de couples d'entiers
        /// </summary>
        /// <param name="list">la liste à sérialiser</param>
        /// <returns>la liste sérialisée</returns>
        public static string Serialize(List<(int a, int b)> list)
        {
            if (list.Count != 0)
            {

                XmlSerializer serializer = new XmlSerializer(typeof(List<(int, int)>));
                using (StringWriter stream = new StringWriter())
                {
                    serializer.Serialize(stream, list);
                    return stream.ToString();
                }
            }
            return "NULL";
        }
        /// <summary>
        /// Sérialise une liste de couples d'entier et chaine
        /// </summary>
        /// <param name="list">la liste à sérialiser</param>
        /// <returns>la liste sérialisée</returns>
        public static string Serialize(List<(string a, int b)> list)
        {
            if (list.Count != 0)
            {

                XmlSerializer serializer = new XmlSerializer(typeof(List<(string, int)>));
                using (StringWriter stream = new StringWriter())
                {
                    serializer.Serialize(stream, list);
                    return stream.ToString();
                }
            }
            return "NULL";
        }

        /// <summary>
        /// Sérialise une liste de couples de coordonnées et chaine
        /// </summary>
        /// <param name="list">la liste à sérialiser</param>
        /// <returns>la liste sérialisée</returns>
        public static string Serialize(List<(Coordinate a, string b)> list)
        {
            if (list.Count != 0)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<(Coordinate, string)>));
                using (StringWriter stream = new StringWriter())
                {
                    serializer.Serialize(stream, list);
                    return stream.ToString();
                }
            }
            return "NULL";
        }

        /// <summary>
        /// Sérialise une liste de jeux en ligne
        /// </summary>
        /// <param name="list">la liste à sérialiser</param>
        /// <returns>la liste sérialisée</returns>
        public static string Serialize(List<GameLine> list)
        {
            if (list.Count != 0)
            {

                XmlSerializer serializer = new XmlSerializer(typeof(List<GameLine>));
                using (StringWriter stream = new StringWriter())
                {
                    serializer.Serialize(stream, list);
                    return stream.ToString();
                }
            }
            return "NULL";
        }

        /// <summary>
        /// Désérialise une liste de couples d'entiers
        /// </summary>
        /// <param name="strXMLString">La liste sous format sérialisé</param>
        /// <param name="list">la liste désérialisée</param>
        public static void Deserialize(string strXMLString, List<(int a, int b)> list)
        {
            if (!strXMLString.Contains("NULL"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<(int, int)>));
                using (StringReader stream = new StringReader(strXMLString))
                {
                    List<(int, int)> listy = (List<(int, int)>)serializer.Deserialize(stream);
                    foreach ((int, int) entry in listy)
                    {
                        list.Add(entry);
                    }
                }
            }
        }
        /// <summary>
        /// Sérialise une liste de couple d'entier et chaine
        /// </summary>
        /// <param name="list">la liste à sérialiser</param>
        /// <returns>la liste sérialisée</returns>
        public static string Serialize(List<(int a, string b)> list)
        {
            if (list.Count != 0)
            {

                XmlSerializer serializer = new XmlSerializer(typeof(List<(int, string)>));
                using (StringWriter stream = new StringWriter())
                {
                    serializer.Serialize(stream, list);
                    return stream.ToString();
                }
            }
            return "NULL";
        }

        /// <summary>
        /// Désérialise une liste de couples d'entier et de chaine
        /// </summary>
        /// <param name="strXMLString">La liste sous format sérialisé</param>
        /// <param name="list">la liste désérialisée</param>
        public static void Deserialize(string strXMLString, List<(int a, string b)> list)
        {

            if (!strXMLString.Contains("NULL"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<(int, string)>));
                using (StringReader stream = new StringReader(strXMLString))
                {
                    List<(int, string)> listy = (List<(int, string)>)serializer.Deserialize(stream);
                    foreach ((int, string) entry in listy)
                    {
                        list.Add(entry);
                    }
                }
            }
        }

        /// <summary>
        /// Désérialise une liste de couples de chaine et d'entier
        /// </summary>
        /// <param name="strXMLString">La liste sous format sérialisé</param>
        /// <param name="list">la liste désérialisée</param>
        public static void Deserialize(string strXMLString, List<(string a, int b)> list)
        {

            if (!strXMLString.Contains("NULL"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<(string, int)>));
                using (StringReader stream = new StringReader(strXMLString))
                {
                    List<(string, int)> listy = (List<(string, int)>)serializer.Deserialize(stream);
                    foreach ((string, int) entry in listy)
                    {
                        list.Add(entry);
                    }
                }
            }
        }
        /// <summary>
        /// Désérialise une liste de couples de coordonnées et de chaine
        /// </summary>
        /// <param name="strXMLString">La liste sous format sérialisé</param>
        /// <param name="list">la liste désérialisée</param>

        public static void Deserialize(string strXMLString, List<(Coordinate a, string b)> list)
        {

            if (!strXMLString.Contains("NULL"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<(Coordinate, string)>));
                using (StringReader stream = new StringReader(strXMLString))
                {
                    List<(Coordinate, string)> listy = (List<(Coordinate, string)>)serializer.Deserialize(stream);
                    foreach ((Coordinate, string) entry in listy)
                    {
                        list.Add(entry);
                    }
                }
            }
        }
        /// <summary>
        /// Désérialise une liste de jeux en ligne
        /// </summary>
        /// <param name="strXMLString">La liste sous format sérialisé</param>
        /// <param name="list">la liste désérialisée</param>
        public static void Deserialize(string strXMLString, List<GameLine> list)
        {
            if (!strXMLString.Contains("NULL"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<GameLine>));
                using (StringReader stream = new StringReader(strXMLString))
                {
                    List<GameLine> listy = (List<GameLine>)serializer.Deserialize(stream);
                    foreach (GameLine entry in listy)
                    {
                        list.Add(entry);
                    }
                }
            }
        }
        /// <summary>
        /// Désérialise les Intersections dans HexGrid
        /// </summary>
        /// <param name="strXMLString">Le dictionnaire sérialisé</param>
        /// <param name="dictionary">Le dictionnaire désérialisé</param>
        public static void Deserialize(string strXMLString, Dictionary<Coordinate, Intersection> dictionary)
        {
            if (!strXMLString.Contains("NULL"))
            {
                dictionary.Clear();
                XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));

                using (StringReader stream = new StringReader(strXMLString))
                {
                    List<Entry> list = (List<Entry>)serializer.Deserialize(stream);
                    foreach (Entry entry in list)
                    {
                        Coordinate key = (Coordinate)Serialization.XMLDeSerialize(new Coordinate(), entry.Key);
                        Intersection value = (Intersection)Serialization.XMLDeSerialize(new Intersection(), entry.Value);
                        dictionary[key] = value;
                    }
                }
            }
        }
        /// <summary>
        /// Désérialise les Edgesdans HexGrid
        /// </summary>
        /// <param name="strXMLString">Le dictionnaire sérialisé</param>
        /// <param name="dictionary">Le dictionnaire désérialisé</param>
        public static void Deserialize(string strXMLString, Dictionary<Coordinate, Edge> dictionary)
        {
            if (!strXMLString.Contains("NULL"))
            {
                dictionary.Clear();
                XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));

                using (StringReader stream = new StringReader(strXMLString))
                {
                    List<Entry> list = (List<Entry>)serializer.Deserialize(stream);
                    foreach (Entry entry in list)
                    {
                        Coordinate key = (Coordinate)Serialization.XMLDeSerialize(new Coordinate(), entry.Key);
                        Edge value = (Edge)Serialization.XMLDeSerialize(new Edge(), entry.Value);
                        dictionary[key] = value;
                    }
                }
            }
        }

        /// <summary>
        /// Désérialise les TerrainTiles dans HexGrid
        /// </summary>
        /// <param name="strXMLString">Le dictionnaire sérialisé</param>
        /// <param name="dictionary">Le dictionnaire désérialisé</param>
        public static void Deserialize(string strXMLString, Dictionary<Coordinate, TerrainTile> dictionary)
        {
            if (!strXMLString.Contains("NULL"))
            {
                dictionary.Clear();
                XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));

                using (StringReader stream = new StringReader(strXMLString))
                {
                    List<Entry> list = (List<Entry>)serializer.Deserialize(stream);
                    foreach (Entry entry in list)
                    {
                        Coordinate key = (Coordinate)Serialization.XMLDeSerialize(new Coordinate(), entry.Key);
                        TerrainTile value = (TerrainTile)Serialization.XMLDeSerialize(new TerrainTile(), entry.Value);
                        dictionary[key] = value;
                    }
                }
            }
        }

        /// <summary>
        /// Désérialise une liste de chaines
        /// </summary>
        /// <param name="strXMLString">La liste sous format sérialisé</param>
        /// <param name="list">la liste désérialisée</param>
        public static void Deserialize(string strXMLString, List<string> list)
        {
            if (!strXMLString.Contains("NULL"))
            {
                list.Clear();
                XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
                using (StringReader stream = new StringReader(strXMLString))
                {
                    list = (List<string>)serializer.Deserialize(stream);
                }
            }

        }


        /// <summary>
        /// Désérialise le dictionnaire de string et int
        /// </summary>
        /// <param name="strXMLString">Le dictionnaire sérialisé</param>
        /// <param name="dictionary">Le dictionnaire désérialisé</param>
        public static void Deserialize(string strXMLString, Dictionary<string, int> dictionary)
        {
            if (!strXMLString.Contains("NULL"))
            {
                dictionary.Clear();
                XmlSerializer serializer = new XmlSerializer(typeof(List<SimpleEntry>));

                using (StringReader stream = new StringReader(strXMLString))
                {
                    List<SimpleEntry> list = (List<SimpleEntry>)serializer.Deserialize(stream);
                    foreach (SimpleEntry entry in list)
                    {
                        dictionary[(string)entry.Key] = (int)entry.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Désérialise une liste de cples d'entier et de Liste de couple de ressources et d'entiers
        /// </summary>
        /// <param name="strXMLString">liste à désérialiser</param>
        /// <param name="discards">liste désérialisée</param>
        public static void Deserialize(string str, List<(int idPlayer, List<(RessourceType rType, int num)>)> discards)
        {
            /*if (!strXMLString.Contains("NULL"))
            {
                discards.Clear();
                List<string> list = new List<string>();
                Deserialize(strXMLString, list);
                foreach (string val in list)
                {
                    SimpleEntry myval = new SimpleEntry();
                    XMLDeSerialize(myval, val);

                    List<string> subval = new List<string>();
                    Deserialize((string)myval.Value, subval);

                    List<(RessourceType rType, int num)> mylist = new List<(RessourceType rType, int num)>();
                    foreach (string loc_list in subval)
                    {
                        SimpleEntry spent = new SimpleEntry();
                        XMLDeSerialize(spent, loc_list);
                        mylist.Add(((RessourceType)spent.Key, (int)spent.Value));
                    }
                    discards.Add(((int)myval.Key, mylist));
                }
            }*/

            char[] seps = { '|' };
            char[] subSeps = { '+' };
            char[] couples = { '&' };
            char[] subCouples = { '#' };
            string[] parts = str.Split(seps);
            discards.Clear();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Length > 0)
                {
                    string[] subParts = parts[i].Split(subSeps);
                    if (subParts.Length > 0)
                    {
                        int x = Int32.Parse(subParts[0]);
                        List<(RessourceType, int)> list = new List<(RessourceType, int)>();
                        string[] subC = subParts[1].Split(couples);
                        if (subC.Length > 0)
                        {
                            for (int j = 0; j < subC.Length; j++)
                            {
                                string[] finalSplit = subC[i].Split(subCouples);
                                RessourceType t = Callbacks.GetRessourceType(finalSplit[0]);
                                int y = Int32.Parse(finalSplit[1]);
                                list.Add((t, y));
                            }
                        }
                        discards.Add((x, list));
                    }
                }
            }
        }



        ///****************************************************************************
        ///
        public static string Serialize(List<(int idPlayer, List<(RessourceType rType, int num)>)> discards)
        {
            string serialize = "NULL";
            if (discards.Count != 0)
            {
                serialize = "";
                foreach ((int idPlayer, List<(RessourceType rType, int num)>) couple in discards)
                {
                    serialize += couple.idPlayer.ToString() + "+";
                    foreach ((RessourceType, int) c in couple.Item2)
                    {
                        serialize += c.Item1.ToString() + "#" + c.Item2.ToString() + "&";
                    }
                    serialize += "|";
                }
            }
            return serialize;
        }


        public static string Serialize(List<(RessourceType rType, int num)> discards)
        {
            string serialize = "NULL";
            if (discards.Count != 0)
            {
                serialize = "";
                foreach ((RessourceType rType, int num) couple in discards)
                {
                    serialize += couple.rType.ToString() + "+" + couple.num.ToString() + "|";
                }
            }
            return serialize;
        }


        
        public static void Deserialize(string str, List<(RessourceType rType, int num)> discards)
        {
            if (str!="NULL")
            {
                char[] seps = { '|' };
                char[] subSeps = { '+' };
                string[] parts = str.Split(seps);
                discards.Clear();

                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].Length > 0)
                    {
                        string[] subParts = parts[i].Split(subSeps);
                        if (subParts.Length > 0)
                        {
                            RessourceType t = Callbacks.GetRessourceType(subParts[0]);
                            int x = Int32.Parse(subParts[1]);
                            discards.Add((t, x));
                        }
                    }
                }

            }

        }


    }


    ///****************************************************************************



    /// <summary>
    /// <para>Classe complémentaire pour sérialiser des couples ou des valeurs du dictionnaire</para>
    /// </summary>
    public class Entry
    {
        /// <summary>
        /// Première valeur du couple ou clé du dictionnaire
        /// </summary>
        public string Key;
        /// <summary>
        /// Deuxième valeur du couple ou clé du dictionnaire
        /// </summary>
        public string Value;

        public Entry()
        {
        }

        /// <summary>
        /// Constructeur de la classe Entry
        /// </summary>
        /// <param name="key">Première valeur du couple ou clé du dictionnaire</param>
        /// <param name="value">Deuxième valeur du couple ou clé du dictionnaire</param>
        public Entry(object key, object value)
        {
            Key = Serialization.XMLSerialize(key);
            Value = Serialization.XMLSerialize(value);
        }
    }

    /// <summary>
    /// <para>Classe complémentaire pour sérialiser des couples ou des valeurs du dictionnaire de façon plus générique</para>
    /// </summary>
    public class SimpleEntry
    {
        /// <summary>
        /// Première valeur du couple ou clé du dictionnaire
        /// </summary>
        public object Key;
        /// <summary>
        /// Deuxième valeur du couple ou clé du dictionnaire
        /// </summary>
        public object Value;

        public SimpleEntry() { }
        /// <summary>
        /// Constructeur de la classe SimpleEntry
        /// </summary>
        /// <param name="key">Première valeur du couple ou clé du dictionnaire</param>
        /// <param name="value">Deuxième valeur du couple ou clé du dictionnaire</param>
        public SimpleEntry(object key, object value)
        {
            Key = key;
            Value = value;
        }
    }

    /// <summary>
    /// Classe simplifiée d'une instance de jeu
    /// </summary>
    public class GameLine
    {
        /// <summary>
        ///Identifiant de la partie
        /// </summary>
        public Guid Id;
        /// <summary>
        /// Nombre de joueurs total
        /// </summary>
        public int NbPlayers;
        /// <summary>
        /// Nombre d'IAs
        /// </summary>
        public int NbIA;
        /// <summary>
        /// Nom du jeu
        /// </summary>
        public string NameGame;
        /// <summary>
        /// Accès public ou privé
        /// </summary>
        public bool Access;
        /// <summary>
        /// Partie pleine ou pas
        /// </summary>
        public bool isFull;
        /// <summary>
        /// Nombre de clients connectés au jeu
        /// </summary>
        public int nbConnected;
        /// <summary>
        /// Nombre de clients prêts dans le jeu
        /// </summary>
        public int nbReady;

        public GameLine() { }

        /// <summary>
        /// Constructeur de la classe GameLine
        /// </summary>
        /// <param name="id">Identifiant de la partie</param>
        /// <param name="nbPlayers">DNombre de joueurs total</param>
        /// <param name="nbIA">Nombre d'IAs</param>
        /// <param name="name">Nom du jeu</param>
        /// <param name="access">Accès public ou privé</param>
        /// <param name="full">Partie pleine ou pas</param>
        /// <param name="nbco">Nombre de clients connectés au jeu</param>
        /// <param name="nbre">Nombre de clients prêts dans le jeu</param>
        public GameLine(Guid id, int nbPlayers, int nbIA, string name, bool access, bool full, int nbco, int nbre)
        {
            Id = id;
            NbPlayers = nbPlayers;
            NbIA = nbIA;
            NameGame = name;
            Access = access;
            isFull = full;
            nbConnected = nbco;
            nbReady = nbre;
        }
    }
}
