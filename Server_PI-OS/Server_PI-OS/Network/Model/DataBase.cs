using System;
using System.Threading;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Data;
using Noyau.View;

namespace Network.Model
{
    public enum ClientType { Undefined = 0, RegisteredPlayer = 1, AnonymousPlayer = 2, EasyAI = 3, MediumAI = 4, DifficultAI = 5 };

    /// <summary>
    /// Classe offrant un ensemble de fonctions permettant de créer, d'écrire et de lire des infromations dans la base de données
    /// </summary>
    class DataBase
    {
        /// <summary>
        /// Type d'erreur rencontrée par la Base de Données
        /// </summary>
        public enum DB_Error { Ok = 0, Unknow, Temporary, Constraint, Intern, Fatal };

        /// <summary>
        /// Type d'erreur de contrainte rencontrée par la base de données
        /// </summary>
        public enum DB_ConstraintError
        {
            Ok = 0,
            // ID not found
            NonExistentID,
            // Insert Player
            NullPlayer, InvalidPlayerId, InvalidIdInGame, IdInGameAlreadyTaken, NullPseudonym, PseudonymAlreadyTaken, PseudonymTooShort, PseudonymTooLong, NullRegisteredPlayerPassword,
            // Insert Game
            NullGame, EmptyGameId, GameIdAlreadyTaken, NoPlayer, CreatorInAnotherGame, NonExistentCreator, NullGameName, GameNameTooShort, GameNameTooLong, InvalidNbPlayers,
            // Insert Message
            NullMessage, NonExistentReference, MessagePrimaryKey, NullMessageText, MessageTooShort, MessageTooLong,
            // Connect Client
            FullGame,
            // Test/Update Client/Game Password
            NullPassword
        }

        /// <summary>
        /// Etat de la partie
        /// </summary>
        public enum GameState { Waiting = 0, Started = 1 };

        /// <value>Taille minimum acceptable d'un pseudonyme de joueur</value>
        private const int MinLengthPseudonym = 0;
        /// <value>Taille maximum acceptable d'un pseudonyme de joueur</value>
        private const int MaxLengthPseudonym = int.MaxValue;
        /// <value>Taille minimum acceptable du nom d'une partie</value>
        private const int MinLengthGameName = 0;
        /// <value>Taille maximum acceptable du nom d'une partie</value>
        private const int MaxLengthGameName = int.MaxValue;
        /// <value>Taille minimum acceptable d'un message</value>
        private const int MinLengthMessage = 0;
        /// <value>Taille maximum acceptable d'un message</value>
        private const int MaxLengthMessage = int.MaxValue;
        /// <value>Taille minimum acceptable de joueurs dans une partie</value>
        private const int MinNbPlayers = 2;
        /// <value>Date à l'Epoch</value>
        private DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        /// <value>Chemin du fichier de la base de données</value>
        static private string DbPath = "database.db";
        /// <value>Connection à la base de données</value>
        static private SQLiteConnection connection = null;
        /// <value>E</value>
        static private bool InitialisedDatabase = false;
        /// <value>Mutex pour la création de la connexion et de la base de données</value>
        static private Mutex InitConnectionMutex = new Mutex();
        /// <value>Verrou lecture-écriture pour les accès concurrents à la base de donénes</value>
        static private ReaderWriterLockSlim ReaderWriterLock = new ReaderWriterLockSlim();
        /// <value>Liste des requêtes permettant d'accéder à la base de données en lecture ou en écriture</value>
        Dictionary<string, SQLiteCommand> Commands = new Dictionary<string, SQLiteCommand>();

        /// <value>Dernière erreur rencontrée, mise à jour après chaque appel de fonction (errno)</value>
        public DB_Error LastError { get; private set; } = DB_Error.Ok;
        /// <value>Dernière erreur de contrainte rencontrée, mise à jour après chaque appel de fonction (errno)</value>
        public DB_ConstraintError LastConstraintError = DB_ConstraintError.Ok;
        /// <value>Dernière message d'erreur, mise à jour après chaque appel de fonction (errno)</value>
        public string LastErrorMessage { get; private set; }

        /// <summary>
        /// Convertit un objet INTEGER de SQLite en int de C#
        /// </summary>
        /// <param name="integer">L'entier à convertir</param>
        /// <returns>L'entier converti en int, -1 si NULL</returns>
        private int DBIntToInt(object integer)
        {
            if (integer is DBNull)
            {
                return -1;
            }
            else
            {
                return (int)(long)integer;
            }
        }

        /// <summary>
        /// Convertit un objet INTEGER de SQLite en bool de C#
        /// </summary>
        /// <param name="boolean">L'entier à convertir</param>
        /// <returns>false si l'entier est null ou vaut 0, true sinon</returns>
        private bool DBIntToBool(object boolean)
        {
            return !(boolean is DBNull) && ((long)boolean != 0);
        }

        /// <summary>
        /// Convertit un objet TEXT de SQLite en string de C#
        /// </summary>
        /// <param name="text">L'objet à convertir</param>
        /// <returns>L'objet converti, null si NULL</returns>
        private string DBTextToString(object text)
        {
            if (text is DBNull)
            {
                return null;
            }
            else
            {
                return (string)text;
            }
        }

        /// <summary>
        /// Convertit un objet TEXT de SQLite contenant le nombre flottant de secondes écoulées depuis l'Epoch en DateTime de C#
        /// </summary>
        /// <param name="text">L'objet à convertir</param>
        /// <returns>L'objet convertie en date</returns>
        private DateTime DBTextToDate(object text)
        {
            return Epoch.AddSeconds(Convert.ToDouble((string)text));
        }

        /// <summary>
        /// Convertit un objet DateTime de C# en objet TEXT de SQLite contenant le nombre flottant de secondes écoulées depuis l'Epoch
        /// </summary>
        /// <param name="date">La date à convertir</param>
        /// <returns>La date convertie en chaîne</returns>
        private String DateToDBText(DateTime date)
        {
            return date.Subtract(Epoch).TotalSeconds.ToString();
        }

        /// <summary>
        /// Effectue une requête de lecture dans la base de données
        /// </summary>
        /// <param name="command">La requête à exécuter</param>
        /// <returns>Le résultat de la lecture. Doit être fermé avec la fonction SQLiteDataReader.Close()</returns>
        /// <remarks>
        /// Pose un verrou en lecture le temps de la récupération des données
        /// Met à jour les valeurs LastError et LastErrorMessage
        /// Return null si la lecture échoue
        /// Après lecture, le résultat doit être fermé avec la fonction SQLiteDataReader.Close()
        /// </remarks>
        private SQLiteDataReader ExecuteReadCommand(SQLiteCommand command)
        {
            return ExecuteReadCommand(command, true);
        }

        /// <summary>
        /// Effectue une requête de lecture dans la base de données
        /// </summary>
        /// <param name="command">La requête à exécuter</param>
        /// <param name="readLock">Si readLock vaut true, un verrou en lecture est posé le temps de la récupération des données</param>
        /// <returns>Le résultat de la lecture. Doit être fermé avec la fonction SQLiteDataReader.Close()</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError et LastErrorMessage
        /// Return null si la lecture échoue
        /// Après lecture, le résultat doit être fermé avec la fonction SQLiteDataReader.Close()
        /// </remarks>
        private SQLiteDataReader ExecuteReadCommand(SQLiteCommand command, bool readLock)
        {
            if (readLock)
            {
                ReaderWriterLock.EnterReadLock();
            }

            SQLiteDataReader reader = null;
            try
            {
                reader = command.ExecuteReader();
                LastError = DB_Error.Ok;
                LastErrorMessage = null;
            }
            catch (SQLiteException e)
            {
                if (reader != null)
                {
                    if (!reader.IsClosed)
                        reader.Close();
                    reader = null;
                }
                Error(e);
            }
            finally
            {
                if (readLock)
                {
                    ReaderWriterLock.ExitReadLock();
                }
            }
            return reader;
        }

        /// <summary>
        /// Effectue une requête d'écriture dans la base de données
        /// </summary>
        /// <param name="command">La requête à exécuter</param>
        /// <returns>true si la commande s'est exécuté correctement, false sinon</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError et LastErrorMessage
        /// </remarks>
        private bool ExecuteNonQueryCommand(SQLiteCommand command)
        {
            bool ok;
            ReaderWriterLock.EnterWriteLock();
            try
            {
                ok = command.ExecuteNonQuery() != 0;
                LastError = DB_Error.Ok;
                LastErrorMessage = null;
                if (!ok)
                {
                    ConstraintError(DB_ConstraintError.NonExistentID, "id not found");
                }
            }
            catch (SQLiteException e)
            {
                ok = false;
                Error(e);
            }
            finally
            {
                ReaderWriterLock.ExitWriteLock();
            }
            return ok;
        }

        /// <summary>
        /// Effectue une requête d'insertion dans la base de données
        /// </summary>
        /// <param name="command">La requête à exécuter</param>
        /// <returns>L'id de la ligne insérée dans la base de données, -1 en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError et LastErrorMessage
        /// </remarks>
        private int ExecuteInsertCommand(SQLiteCommand command)
        {
            int id;
            SQLiteDataReader reader = null;
            ReaderWriterLock.EnterWriteLock();
            try
            {
                command.ExecuteNonQuery();
                LastError = DB_Error.Ok;
                LastErrorMessage = null;
                reader = ExecuteReadCommand(Commands["GET LAST INSERT ROW ID"], false);
                if (reader.Read())
                {
                    id = DBIntToInt(reader[0]);
                }
                else
                {
                    id = -1;
                }
                reader.Close();
            }
            catch (SQLiteException e)
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
                id = -1;
                Error(e);
            }
            finally
            {
                ReaderWriterLock.ExitWriteLock();
            }
            return id;
        }

        /// <summary>
        /// Traitement des exceptions générées par les fonctions SQLite
        /// </summary>
        /// <param name="e">L'exception levée par SQLite</param>
        /// <remarks>
        /// Met à jour les valeurs LastError et LastErrorMessage
        /// </remarks>
        private void Error(SQLiteException e)
        {
            LastErrorMessage = e.Message;
            switch (e.ErrorCode & 255)
            {
                case (0):
                case (100):
                case (101):
                    // Tout s'est bien passé
                    // Normalement on n'intercepte pas d'exception.
                    LastError = DB_Error.Ok;
                    LastErrorMessage = null;
                    break;
                case (1):
                    // Code d'erreur générique
                    // Je ne sais pas quoi faire.
                    LastError = DB_Error.Unknow;
                    Console.Error.WriteLine("SqliteException: {0}", e.Message);
                    break;
                case (4):
                case (5):
                case (6):
                case (9):
                    // Erreur due à un contexte temporaire (ex: accès concurrentiel, timeout)
                    // Réessayer plus tard ?
                    Console.Error.WriteLine("SqliteException: {0}", e.Message);
                    break;
                case (18): // Chaîne trop longue
                case (19): // Contrainte non respectée
                           // Erreur de données envoyées pour la requête
                           // C'est au serveur de gérer ça.
                    LastError = DB_Error.Constraint;
                    break;
                case (8):
                case (20):
                case (21):
                case (23):
                case (25):
                    // Antoine a mal fait la BDD
                    // Tapez-lui dessus.
                    LastError = DB_Error.Intern;
                    throw e;
                default:
                    // Erreur inconnue / Erreur fatale
                    // On relance l'exception.
                    LastError = DB_Error.Fatal;
                    throw e;
            }
        }

        /// <summary>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage pour enregistrer une erreur de contrainte
        /// </summary>
        /// <param name="error">L'erreur à enregistrer</param>
        /// <param name="message">Le message d'erreur à enregistrer</param>
        private void ConstraintError(DB_ConstraintError error, string message)
        {
            LastError = DB_Error.Constraint;
            LastConstraintError = error;
            LastErrorMessage = message;
            Console.Error.WriteLine("Database Constraint Error: " + message);
        }

        /// <summary>
        /// Crée un objet Database connecté à la base de données
        /// </summary>
        /// <param name="createIfNotExist">Si true, crée la base de données si le fichier n'existe pas</param>
        /// <returns>L'objet instancié</returns>
        public DataBase(bool createIfNotExist)
        {
            if (!InitialisedDatabase)
            {
                InitConnectionMutex.WaitOne();
                if (!InitialisedDatabase)
                {
                    InitConnection(createIfNotExist);
                    InitialisedDatabase = true;
                }

                InitConnectionMutex.ReleaseMutex();
            }

            InitCommands();
        }

        /// <summary>
        /// Connecte les instances de Database à la base de données
        /// </summary>
        /// <param name="createIfNotExist">Si true, crée la base de données si le fichier n'existe pas</param>
        private void InitConnection(bool createIfNotExist)
        {
            if (!File.Exists(DbPath))
            {
                if (createIfNotExist)
                {
                    Create();
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
            else
            {
                connection = new SQLiteConnection("Data Source=" + DbPath + ";Version=3;Open Mode=ReadWrite");
                connection.Open();

                ExecuteNonQueryCommand(new SQLiteCommand("PRAGMA foreign_keys = ON", connection));
            }
        }

        /// <summary>
        /// Initialise et prépare les commandes SQLite
        /// </summary>
        private void InitCommands()
        {
            // Instanciation des requêtes
            foreach (string table in new string[] { "Player", "Game", "Message" })
            {
                foreach (string cmdType in new string[] { "INSERT", "DELETE" })
                {
                    Commands[cmdType + " " + table] = new SQLiteCommand(null, connection);
                }
            }

            string[] otherCmd = new string[]
            {
                "GET LAST INSERT ROW ID",
                "GET Player", "GET CONNECTED PLAYERS", "GET LAST PLAYER ID",
                "GET Game", "GET AVAILABLE GAMES",
                "GET MESSAGES",
                "UPDATE Player", "UPDATE PLAYER PASSWORD", "WIN GAME", "LOSE GAME", "CONNECT",
                "RENAME GAME", "UPDATE GAME PASSWORD", "UPDATE GAME STATE", "SET PLAYER READY"
            };

            foreach (string cmd in otherCmd)
            {
                Commands[cmd] = new SQLiteCommand(null, connection);
            }


            // Création des requêtes
            Commands["GET LAST INSERT ROW ID"].CommandText = "SELECT last_insert_rowid()";
            Commands["GET Player"].CommandText = "SELECT * FROM Player WHERE player_id = @id";
            Commands["GET CONNECTED PLAYERS"].CommandText = "SELECT * FROM Player WHERE current_game_id = @game_id";
            Commands["GET LAST PLAYER ID"].CommandText = "SELECT max(player_id) AS max_id FROM Player";
            Commands["GET Game"].CommandText = "SELECT * FROM Game WHERE game_id = @id";
            Commands["GET AVAILABLE GAMES"].CommandText = "SELECT * FROM Game WHERE state = 0";
            Commands["GET MESSAGES"].CommandText = "SELECT * FROM Message WHERE game_id = @game_id ORDER BY date ASC";

            Commands["INSERT Player"].CommandText = @"INSERT INTO Player (type, pseudonym, password, avatar, games_won, games_played, current_game_id, id_in_game, is_ready)
                                                    VALUES (@type, @pseudonym, @password, @avatar, 0, 0, @current_game_id, @id_in_game, @is_ready)";
            Commands["INSERT Game"].CommandText = @"INSERT INTO Game (game_id, name, password, players_max, state)
                                                    VALUES (@game_id, @name, @password, @players_max, @state)";
            Commands["INSERT Message"].CommandText = @"INSERT INTO Message (game_id, sender_id, date, text)
                                                    VALUES (@game_id, @sender_id, @date, @text)";

            Commands["DELETE Player"].CommandText = "DELETE FROM Player WHERE player_id = @id";
            Commands["DELETE Game"].CommandText = "DELETE FROM Game WHERE game_id = @id";
            Commands["DELETE Message"].CommandText = "DELETE FROM Message WHERE game_id = @game_id AND sender_id = @sender_id AND date = @date";

            Commands["UPDATE Player"].CommandText = "UPDATE Player SET type = @type, pseudonym = @pseudonym, avatar = @avatar, current_game_id = @current_game_id, id_in_game = @id_in_game, is_ready = @is_ready WHERE player_id = @id";
            Commands["UPDATE PLAYER PASSWORD"].CommandText = "UPDATE Player SET password = @hashed_password WHERE player_id = @id";
            Commands["WIN GAME"].CommandText = "UPDATE Player SET (games_won, games_played) = (games_won + 1, games_played + 1) WHERE player_id = @id";
            Commands["LOSE GAME"].CommandText = "UPDATE Player SET games_played = games_played + 1 WHERE player_id = @id";
            Commands["CONNECT"].CommandText = "UPDATE Player SET current_game_id = @game_id, id_in_game = @id_in_game, is_ready = @is_ready WHERE player_id = @player_id";
            Commands["SET PLAYER READY"].CommandText = "UPDATE Player SET is_ready = @is_ready WHERE player_id = @player_id";

            Commands["RENAME GAME"].CommandText = "UPDATE Game SET name = @name WHERE game_id = @id";
            Commands["UPDATE GAME PASSWORD"].CommandText = "UPDATE Game SET password = @hashed_password WHERE game_id = @id";
            Commands["UPDATE GAME STATE"].CommandText = "UPDATE Game SET state = @state WHERE game_id = @id";



            // Compilation des requêtes
            foreach (SQLiteCommand command in Commands.Values)
            {
                command.Prepare();
            }

            // Ajout des arguments
            Commands["GET Player"].Parameters.Add(new SQLiteParameter("@id", null));
            Commands["GET CONNECTED PLAYERS"].Parameters.Add(new SQLiteParameter("@game_id", null));
            Commands["GET Game"].Parameters.Add(new SQLiteParameter("@id", null));
            Commands["GET MESSAGES"].Parameters.Add(new SQLiteParameter("@game_id", null));

            Commands["INSERT Player"].Parameters.Add(new SQLiteParameter("@type", null));
            Commands["INSERT Player"].Parameters.Add(new SQLiteParameter("@pseudonym", null));
            Commands["INSERT Player"].Parameters.Add(new SQLiteParameter("@password", null));
            Commands["INSERT Player"].Parameters.Add(new SQLiteParameter("@avatar", null));
            Commands["INSERT Player"].Parameters.Add(new SQLiteParameter("@current_game_id", null));
            Commands["INSERT Player"].Parameters.Add(new SQLiteParameter("@id_in_game", null));
            Commands["INSERT Player"].Parameters.Add(new SQLiteParameter("@is_ready", null));

            Commands["INSERT Game"].Parameters.Add(new SQLiteParameter("@game_id", null));
            Commands["INSERT Game"].Parameters.Add(new SQLiteParameter("@name", null));
            Commands["INSERT Game"].Parameters.Add(new SQLiteParameter("@password", null));
            Commands["INSERT Game"].Parameters.Add(new SQLiteParameter("@players_max", null));
            Commands["INSERT Game"].Parameters.Add(new SQLiteParameter("@state", null));

            Commands["INSERT Message"].Parameters.Add(new SQLiteParameter("@game_id", null));
            Commands["INSERT Message"].Parameters.Add(new SQLiteParameter("@sender_id", null));
            Commands["INSERT Message"].Parameters.Add(new SQLiteParameter("@date", null));
            Commands["INSERT Message"].Parameters.Add(new SQLiteParameter("@text", null));

            Commands["DELETE Player"].Parameters.Add(new SQLiteParameter("@id", null));
            Commands["DELETE Game"].Parameters.Add(new SQLiteParameter("@id", null));

            Commands["DELETE Message"].Parameters.Add(new SQLiteParameter("@game_id", null));
            Commands["DELETE Message"].Parameters.Add(new SQLiteParameter("@sender_id", null));
            Commands["DELETE Message"].Parameters.Add(new SQLiteParameter("@date", null));

            Commands["UPDATE Player"].Parameters.Add(new SQLiteParameter("@type", null));
            Commands["UPDATE Player"].Parameters.Add(new SQLiteParameter("@pseudonym", null));
            Commands["UPDATE Player"].Parameters.Add(new SQLiteParameter("@avatar", null));
            Commands["UPDATE Player"].Parameters.Add(new SQLiteParameter("@current_game_id", null));
            Commands["UPDATE Player"].Parameters.Add(new SQLiteParameter("@id_in_game", null));
            Commands["UPDATE Player"].Parameters.Add(new SQLiteParameter("@is_ready", null));
            Commands["UPDATE Player"].Parameters.Add(new SQLiteParameter("@id", null));

            Commands["UPDATE PLAYER PASSWORD"].Parameters.Add(new SQLiteParameter("@hashed_password", null));
            Commands["UPDATE PLAYER PASSWORD"].Parameters.Add(new SQLiteParameter("@id", null));

            Commands["WIN GAME"].Parameters.Add(new SQLiteParameter("@id", null));
            Commands["LOSE GAME"].Parameters.Add(new SQLiteParameter("@id", null));

            Commands["CONNECT"].Parameters.Add(new SQLiteParameter("@game_id", null));
            Commands["CONNECT"].Parameters.Add(new SQLiteParameter("@is_ready", null));
            Commands["CONNECT"].Parameters.Add(new SQLiteParameter("@id_in_game", null));
            Commands["CONNECT"].Parameters.Add(new SQLiteParameter("@player_id", null));

            Commands["SET PLAYER READY"].Parameters.Add(new SQLiteParameter("@is_ready", null));
            Commands["SET PLAYER READY"].Parameters.Add(new SQLiteParameter("@player_id", null));

            Commands["RENAME GAME"].Parameters.Add(new SQLiteParameter("@name", null));
            Commands["RENAME GAME"].Parameters.Add(new SQLiteParameter("@id", null));

            Commands["UPDATE GAME PASSWORD"].Parameters.Add(new SQLiteParameter("@hashed_password", null));
            Commands["UPDATE GAME PASSWORD"].Parameters.Add(new SQLiteParameter("@id", null));

            Commands["UPDATE GAME STATE"].Parameters.Add(new SQLiteParameter("@state", null));
            Commands["UPDATE GAME STATE"].Parameters.Add(new SQLiteParameter("@id", null));
        }

        /// <summary>
        /// Ferme la connection à la base de données
        /// </summary>
        public static void Close()
        {
            ReaderWriterLock.EnterWriteLock();
            InitialisedDatabase = false;
            try
            {
                connection.Close();
                connection = null;
            }
            finally
            {
                ReaderWriterLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Crée le fichier et la base de données
        /// </summary>
        private void Create()
        {
            SQLiteConnection.CreateFile("database.db");
            connection = new SQLiteConnection("Data Source=" + DbPath + ";Version=3;Open Mode=ReadWrite");
            connection.Open();

            string[] createCommands =
            {
                @"PRAGMA foreign_keys = ON",
                @"CREATE TABLE Game (
                    game_id TEXT PRIMARY KEY,
                    name TEXT NOT NULL,
                    password TEXT,
                    players_max INTEGER NOT NULL,
                    state INTEGER NOT NULL,
                    CHECK (length(name) >= " + MinLengthGameName + @"),
                    CHECK (length(name) <= " + MaxLengthGameName + @"),
                    CHECK (players_max >= " + MinNbPlayers + @")
                )",
                @"CREATE TABLE Player (
                    player_id INTEGER PRIMARY KEY,
                    type INTEGER NOT NULL, -- 0: Undefined, 1: Registered Player, 2: Anonymous Player, 3: EasyAI, 4: MediumAI, 5: DifficultAI
                    pseudonym TEXT UNIQUE,
                    password TEXT,
                    avatar INTEGER NOT NULL,
                    games_won INTEGER NOT NULL,
                    games_played INTEGER NOT NULL,
                    current_game_id TEXT
                        REFERENCES Game (game_id)
                            ON DELETE SET NULL
                            ON UPDATE CASCADE,
                    id_in_game INTEGER,
                    is_ready INTEGER NOT NULL,
                    CHECK (type >= 0),
                    CHECK (type <= 5),
                    CHECK (games_won >= 0),
                    CHECK (games_played >= games_won),
                    CHECK (length(pseudonym) >= " + MinLengthPseudonym + @"),
                    CHECK (length(pseudonym) <= " + MaxLengthPseudonym + @"),
                    UNIQUE(current_game_id, id_in_game)
                )",
                @"CREATE TABLE Message (
                    game_id TEXT NOT NULL
                        REFERENCES Game (game_id)
                            ON DELETE CASCADE
                            ON UPDATE CASCADE,
                    sender_id INTEGER
                        REFERENCES Player (player_id)
                            ON DELETE CASCADE
                            ON UPDATE CASCADE,
                    date TEXT NOT NULL,
                    text TEXT NOT NULL,
                    PRIMARY KEY (game_id, sender_id, date),
                    CHECK (length(text) >= " + MinLengthMessage + @")
                    CHECK (length(text) <= " + MaxLengthMessage + @")
                )",
                @"CREATE TRIGGER insert_player__max_id_in_game
                    BEFORE INSERT
                    ON Player
                    FOR EACH ROW
                    WHEN (NEW.id_in_game IS NOT NULL)
                        AND (NEW.current_game_id IS NOT NULL)
                        AND (NEW.id_in_game >= (SELECT players_max FROM Game WHERE game_id = NEW.current_game_id))
                BEGIN
                    SELECT RAISE(ABORT, 'Player.id_in_game >= Player.current_game.players_max');
                END",
                @"CREATE TRIGGER update_player__max_id_in_game
                    BEFORE UPDATE OF id_in_game
                    ON Player
                    FOR EACH ROW
                    WHEN (OLD.id_in_game IS NULL OR NEW.id_in_game <> OLD.id_in_game)
                        AND (NEW.id_in_game IS NOT NULL)
                        AND (NEW.current_game_id IS NOT NULL)
                        AND (NEW.id_in_game >= (SELECT players_max FROM Game WHERE game_id = NEW.current_game_id))
                BEGIN
                    SELECT RAISE(ABORT, 'Player.id_in_game >= Player.current_game.players_max');
                END"
            };
            ReaderWriterLock.EnterWriteLock();
            try
            {
                foreach (string command in createCommands)
                {
                    (new SQLiteCommand(command, connection)).ExecuteNonQuery();
                }
            }
            finally
            {
                ReaderWriterLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Supprime le fichier de la base de données
        /// </summary>
        /// <returns>true si la suppression est un succès, false sinon</returns>
        /// <remarks>
        /// Ferme préalablement la connection si celle-ci est ouverte
        /// </remarks>
        public static bool DeleteDatabase()
        {
            bool retValue = false;

            if (connection != null && connection.State == ConnectionState.Open)
            {
                Close();
            }

            if (File.Exists(DbPath))
            {
                try
                {
                    File.Delete(DbPath);
                    retValue = true;
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
            }
            return retValue;
        }




        // Getters

        /// <summary>
        /// Instancie un objet de type Player à partir de son id dans la base de données
        /// </summary>
        /// <param name="id">Id du joueur dans la base de données</param>
        /// <returns>L'objet instancié, null en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public Player GetPlayer(int id)
        {
            if (id < 0)
            {
                ConstraintError(DB_ConstraintError.InvalidPlayerId, "invalid player id");
                return null;
            }

            Player pl = null;
            Commands["GET Player"].Parameters["@id"].Value = id;
            SQLiteDataReader reader = ExecuteReadCommand(Commands["GET Player"]);
            if (reader != null && reader.Read())
            {
                Guid gameId;
                if (reader["current_game_id"] is DBNull)
                {
                    gameId = Guid.Empty;
                }
                else
                {
                    gameId = new Guid(DBTextToString(reader["current_game_id"]));
                }

                switch (DBIntToInt(reader["type"]))
                {
                    case (int)ClientType.EasyAI:
                        pl = new Easy(id, gameId, DBIntToInt(reader["id_in_game"]), DBTextToString(reader["pseudonym"]));
                        break;
                    case (int)ClientType.MediumAI:
                        pl = new Medium(id, gameId, DBIntToInt(reader["id_in_game"]), DBTextToString(reader["pseudonym"]));
                        break;
                    case (int)ClientType.DifficultAI:
                        pl = new Difficult(id, gameId, DBIntToInt(reader["id_in_game"]), DBTextToString(reader["pseudonym"]));
                        break;
                    default:
                        pl = new Client(id, gameId, DBIntToInt(reader["id_in_game"]), (ClientType)DBIntToInt(reader["type"]), DBTextToString(reader["pseudonym"]), null, DBIntToInt(reader["avatar"]), DBIntToBool(reader["is_ready"]));
                        break;
                }
            }
            reader.Close();
            return pl;
        }

        /// <summary>
        /// Instancie un objet de type OnlineGame à partir de son id dans la base de données
        /// </summary>
        /// <param name="id">Id de la partie dans la base de données</param>
        /// <returns>L'objet instancié, null en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public OnlineGame GetGame(Guid id)
        {
            if (id == Guid.Empty)
            {
                ConstraintError(DB_ConstraintError.EmptyGameId, "empty game id");
                return null;
            }

            Commands["GET Game"].Parameters["@id"].Value = id.ToString();
            SQLiteDataReader reader = ExecuteReadCommand(Commands["GET Game"]);
            if (reader != null && reader.Read())
            {
                List<Player> players = GetConnectedPlayers(id);
                int nbIA = 0;
                int nbReady = 0;
                foreach (Player player in players)
                {
                    if (player is IA)
                    {
                        nbIA++;
                    }

                    if (player.IsReady)
                    {
                        nbReady++;
                    }
                }
                OnlineGame g = new OnlineGame(DBIntToInt(reader["players_max"]), nbIA, DBTextToString(reader["name"]), reader["password"] is DBNull, GameView.DefaultGrid, DBIntToInt(reader["state"]) >= (int)GameState.Started, 0, nbReady);
                reader.Close();

                g.setId(id);
                foreach (Player player in players)
                {
                    g.AddPlayer(player);
                }

                return g;
            }
            reader.Close();
            return null;
        }

        /// <summary>
        /// Renvoie la liste des messages postés dans une partie
        /// </summary>
        /// <param name="id">Id de la partie dans la base de données</param>
        /// <returns>La liste de messages, null en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public List<Message> GetMessages(Guid id)
        {
            if (id == Guid.Empty)
            {
                ConstraintError(DB_ConstraintError.EmptyGameId, "empty game id");
                return null;
            }

            List<Message> messages = null;
            messages = new List<Message>();
            Commands["GET MESSAGES"].Parameters["@game_id"].Value = id.ToString(); ;
            SQLiteDataReader reader = ExecuteReadCommand(Commands["GET MESSAGES"]);
            while (reader.Read())
            {
                messages.Add(new Message(id, DBIntToInt(reader["sender_id"]), DBTextToDate(reader["date"]), DBTextToString(reader["text"])));
            }
            reader.Close();
            return messages;
        }

        /// <summary>
        /// Renvoie la liste de joueurs connectés à une partie
        /// </summary>
        /// <param name="id">Id de la partie dans la base de données</param>
        /// <returns>La liste de joueurs connectés sous forme de nouvelles instances</returns>1
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public List<Player> GetConnectedPlayers(Guid id)
        {
            if (id == Guid.Empty)
            {
                ConstraintError(DB_ConstraintError.EmptyGameId, "empty game id");
                return null;
            }

            List<Player> players = null;
            Commands["GET CONNECTED PLAYERS"].Parameters["@game_id"].Value = id.ToString();
            SQLiteDataReader reader = ExecuteReadCommand(Commands["GET CONNECTED PLAYERS"]);

            if (reader != null)
            {
                players = new List<Player>();
                while (reader.Read())
                {
                    switch (DBIntToInt(reader["type"]))
                    {
                        case (int)ClientType.EasyAI:
                            players.Add(new Easy(DBIntToInt(reader["player_id"]), id, DBIntToInt(reader["id_in_game"]), DBTextToString(reader["pseudonym"])));
                            break;
                        case (int)ClientType.MediumAI:
                            players.Add(new Medium(DBIntToInt(reader["player_id"]), id, DBIntToInt(reader["id_in_game"]), DBTextToString(reader["pseudonym"])));
                            break;
                        case (int)ClientType.DifficultAI:
                            players.Add(new Difficult(DBIntToInt(reader["player_id"]), id, DBIntToInt(reader["id_in_game"]), DBTextToString(reader["pseudonym"])));
                            break;
                        default:
                            players.Add(new Client(DBIntToInt(reader["player_id"]), id, DBIntToInt(reader["id_in_game"]), (ClientType)DBIntToInt(reader["type"]), DBTextToString(reader["pseudonym"]), null, DBIntToInt(reader["avatar"]), DBIntToBool(reader["is_ready"])));
                            break;
                    }
                }
                reader.Close();
            }
            return players;
        }

        /// <summary>
        /// Renvoie l'id maximum des joueurs de la base de données
        /// </summary>
        /// <returns>L'id maximal des joueurs de la base de données</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public int MaxPlayerId()
        {
            int retValue = -1;
            SQLiteDataReader reader = ExecuteReadCommand(Commands["GET LAST PLAYER ID"]);
            if (reader.Read() && !(reader["max_id"] is DBNull))
            {
                retValue = DBIntToInt(reader["max_id"]);
            }
            reader.Close();

            return retValue;
        }

        /// <summary>
        /// Renvoie la liste des parties en attente de joueurs
        /// </summary>
        /// <returns>La liste des parties en attente de joueurs</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public List<OnlineGame> GetAvailableGames()
        {
            List<OnlineGame> games = new List<OnlineGame>();
            SQLiteDataReader reader = ExecuteReadCommand(Commands["GET AVAILABLE GAMES"]);
            while (reader.Read())
            {
                Guid gameId = new Guid(DBTextToString(reader["game_id"]));
                List<Player> players = GetConnectedPlayers(gameId);
                int nbIA = 0;
                int nbReady = 0;
                foreach (Player player in players)
                {
                    if (player is IA)
                    {
                        nbIA++;
                    }

                    if (player.IsReady)
                    {
                        nbReady++;
                    }
                }
                OnlineGame g = new OnlineGame(DBIntToInt(reader["players_max"]), nbIA, DBTextToString(reader["name"]), reader["password"] is DBNull, GameView.DefaultGrid, DBIntToInt(reader["state"]) >= (int)GameState.Started, 0, nbReady);

                g.setId(gameId);
                foreach (Player player in players)
                {
                    g.AddPlayer(player);
                }

                games.Add(g);
            }
            reader.Close();
            return games;
        }

        /// <summary>
        /// Compare le mot de passe hashé avec celui contenu dans la base de données
        /// </summary>
        /// <param name="playerId">Id du joueur dans la base de données</param>
        /// <param name="password">Mot de passe hashé</param>
        /// <returns>true si le mot de passe correspond, false sinon</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public bool VerifyPlayerPassword(int playerId, string password)
        {
            bool retValue = false;
            if (playerId < 0)
            {
                ConstraintError(DB_ConstraintError.InvalidPlayerId, "invalid player id");
            }
            else
            {
                Commands["GET Player"].Parameters["@id"].Value = playerId;
                SQLiteDataReader reader = ExecuteReadCommand(Commands["GET Player"]);
                retValue = reader.Read() && (DBTextToString(reader["password"]) == password);
                reader.Close();
            }
            return retValue;
        }

        /// <summary>
        /// Compare le mot de passe hashé avec celui contenu dans la base de données
        /// </summary>
        /// <param name="id">Id de la partie dans la base de données</param>
        /// <param name="password">Mot de passe hashé</param>
        /// <returns>true si le mot de passe correspond, false sinon</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public bool VerifyGamePassword(Guid id, string password)
        {
            bool retValue = false;

            if (id == Guid.Empty)
            {
                ConstraintError(DB_ConstraintError.EmptyGameId, "empty game id");
            }
            else
            {
                Commands["GET Game"].Parameters["@id"].Value = id.ToString();
                SQLiteDataReader reader = ExecuteReadCommand(Commands["GET Game"]);
                retValue = reader.Read() && (DBTextToString(reader["password"]) == password);
                reader.Close();
            }
            return retValue;
        }




        // Insert

        /// <summary>
        /// Insère les informations d'un joueur dans une nouvelle entrée de la base de données
        /// </summary>
        /// <param name="pl">Joueur à insérer</param>
        /// <returns>L'id du joueur inséré dans la base de données, -1 en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public int InsertPlayer(Player pl)
        {
            return InsertPlayer(pl, null);
        }

        public int InsertPlayer(Player pl, string password)
        {
            int retValue = -1;
            if (pl == null)
            {
                ConstraintError(DB_ConstraintError.NullPlayer, "null player");
            }
            else if (pl.IdGame != Guid.Empty && pl.IdInGame < 0)
            {
                ConstraintError(DB_ConstraintError.InvalidIdInGame, "invalid id in game");
            }
            else
            {
                Commands["INSERT Player"].Parameters["@type"].Value = (int)pl.Type;
                Commands["INSERT Player"].Parameters["@pseudonym"].Value = pl.Name;
                Commands["INSERT Player"].Parameters["@password"].Value = password;
                Commands["INSERT Player"].Parameters["@avatar"].Value = pl.Avatar;

                if (pl.IdGame == Guid.Empty)
                {
                    Commands["INSERT Player"].Parameters["@current_game_id"].Value = null;
                    Commands["INSERT Player"].Parameters["@id_in_game"].Value = null;
                    Commands["INSERT Player"].Parameters["@is_ready"].Value = false;
                }
                else
                {
                    Commands["INSERT Player"].Parameters["@current_game_id"].Value = pl.IdGame.ToString();
                    Commands["INSERT Player"].Parameters["@id_in_game"].Value = pl.IdInGame;
                    Commands["INSERT Player"].Parameters["@is_ready"].Value = pl.IsReady;
                }

                retValue = ExecuteInsertCommand(Commands["INSERT Player"]);
                if (LastErrorMessage != null)
                {
                    if (LastErrorMessage == "constraint failed\r\nUNIQUE constraint failed: Player.pseudonym")
                    {
                        ConstraintError(DB_ConstraintError.PseudonymAlreadyTaken, "pseudonym already taken");
                    }
                    else if (LastErrorMessage == "Player.id_in_game >= Player.current_game.players_max")
                    {
                        ConstraintError(DB_ConstraintError.FullGame, "id in game greater or equal than game's max player count");
                    }
                    else if (LastErrorMessage == "constraint failed\r\nUNIQUE constraint failed: Player.current_game_id, Player.id_in_game")
                    {
                        ConstraintError(DB_ConstraintError.IdInGameAlreadyTaken, "id in game already taken");
                    }
                }
            }
            return retValue;
        }

        /// <summary>
        /// Insère les informations d'une partie dans une nouvelle entrée de la base de données
        /// </summary>
        /// <param name="g">Partie à insérer</param>
        /// <returns>true en cas de succès, false en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public bool InsertGame(OnlineGame g)
        {
            return InsertGame(g, null);
        }

        /// <summary>
        /// Insère les informations d'une partie dans une nouvelle entrée de la base de données
        /// </summary>
        /// <param name="g">Partie à insérer</param>
        /// <param name="password">Mot de passe de la partie</param>
        /// <returns>true en cas de succès, false en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public bool InsertGame(OnlineGame g, string password)
        {
            bool retValue = false;
            if (g == null)
            {
                ConstraintError(DB_ConstraintError.NullGame, "null game");
            }
            else if (g.InGame == Guid.Empty)
            {
                ConstraintError(DB_ConstraintError.EmptyGameId, "empty game id");
            }
            else if (g.Players == null)
            {
                ConstraintError(DB_ConstraintError.NoPlayer, "no player in this game");
            }
            else if (g.Name == null)
            {
                ConstraintError(DB_ConstraintError.NullGameName, "null game name");
            }
            else if (g.Name.Length < MinLengthGameName)
            {
                ConstraintError(DB_ConstraintError.GameNameTooShort, "game name too short (min " + MinLengthGameName + " characters)");
            }
            else if (g.Name.Length > MaxLengthGameName)
            {
                ConstraintError(DB_ConstraintError.GameNameTooLong, "game name too long (max " + MaxLengthGameName + " characters)");
            }
            else if (g.NbPlayers < MinNbPlayers)
            {
                ConstraintError(DB_ConstraintError.GameNameTooLong, "maximum players number too low (min " + MinNbPlayers + " players)");
            }
            else if (g.Players.Count > g.NbPlayers)
            {
                ConstraintError(DB_ConstraintError.FullGame, "too many players in this game");
            }
            else
            {
                Commands["INSERT Game"].Parameters["@game_id"].Value = g.InGame.ToString();
                Commands["INSERT Game"].Parameters["@name"].Value = g.Name;
                Commands["INSERT Game"].Parameters["@password"].Value = password;
                Commands["INSERT Game"].Parameters["@players_max"].Value = g.NbPlayers;
                Commands["INSERT Game"].Parameters["@state"].Value = (g.hasStarted) ? GameState.Started : GameState.Waiting;

                SQLiteTransaction transaction = connection.BeginTransaction();
                retValue = (ExecuteInsertCommand(Commands["INSERT Game"]) != -1);
                if (LastErrorMessage != null && LastErrorMessage == "constraint failed\r\nUNIQUE constraint failed: Game.game_id")
                {
                    ConstraintError(DB_ConstraintError.GameIdAlreadyTaken, "game id already taken");
                }

                if (retValue)
                {
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                }
            }
            return retValue;
        }

        /// <summary>
        /// Insère les informations d'un message dans une nouvelle entrée de la base de données
        /// </summary>
        /// <param name="m">Message à insérer</param>
        /// <returns>true en cas de succès, false en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public bool InsertMessage(Message m)
        {
            bool retValue = false;
            if (m == null)
            {
                ConstraintError(DB_ConstraintError.NullMessage, "null message");
            }
            else if (m.GameId == Guid.Empty)
            {
                ConstraintError(DB_ConstraintError.EmptyGameId, "empty game id");
            }
            else if (m.SenderId < -1)
            {
                ConstraintError(DB_ConstraintError.InvalidPlayerId, "invalid player id");
            }
            else if (m.Text == null)
            {
                ConstraintError(DB_ConstraintError.NullMessageText, "null text");
            }
            else if (m.Text.Length < MinLengthMessage)
            {
                ConstraintError(DB_ConstraintError.MessageTooShort, "message too short (min " + MinLengthMessage + " characters)");
            }
            else if (m.Text.Length > MaxLengthMessage)
            {
                ConstraintError(DB_ConstraintError.MessageTooLong, "message too long (max " + MaxLengthMessage + " characters)");
            }
            else
            {
                Commands["INSERT Message"].Parameters["@game_id"].Value = m.GameId.ToString();
                Commands["INSERT Message"].Parameters["@date"].Value = DateToDBText(m.Date);
                Commands["INSERT Message"].Parameters["@text"].Value = m.Text;

                if (m.SenderId == -1)
                {
                    Commands["INSERT Message"].Parameters["@sender_id"].Value = null;
                }
                else
                {
                    Commands["INSERT Message"].Parameters["@sender_id"].Value = m.SenderId;
                }

                retValue = (ExecuteInsertCommand(Commands["INSERT Message"]) != -1);

                if (LastErrorMessage != null)
                {
                    if (LastErrorMessage == "constraint failed\r\nUNIQUE constraint failed: Message.game_id, Message.sender_id, Message.date")
                    {
                        ConstraintError(DB_ConstraintError.MessagePrimaryKey, "cannot post two messages in a second");
                    }
                    else if (LastErrorMessage == "constraint failed\r\nFOREIGN KEY constraint failed")
                    {
                        ConstraintError(DB_ConstraintError.NonExistentReference, "non-existent game or sender");
                    }
                }
            }
            return retValue;
        }



        // Delete

        /// <summary>
        /// Supprime l'entrée d'un joueur de la base de données
        /// </summary>
        /// <param name="playerId">Id du joueur à supprimer</param>
        /// <returns>true en cas de succès, false en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public bool DeletePlayer(int playerId)
        {
            if (playerId < 0)
            {
                ConstraintError(DB_ConstraintError.InvalidPlayerId, "invalid player id");
                return false;
            }
            else
            {
                Commands["DELETE Player"].Parameters["@id"].Value = playerId;
                return ExecuteNonQueryCommand(Commands["DELETE Player"]);
            }
        }

        /// <summary>
        /// Supprime l'entrée d'une partie de la base de données
        /// </summary>
        /// <param name="id">Id de la partie à supprimer</param>
        /// <returns>true en cas de succès, false en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public bool DeleteGame(Guid id)
        {
            if (id == Guid.Empty)
            {
                ConstraintError(DB_ConstraintError.EmptyGameId, "empty game id");
                return false;
            }
            else
            {
                Commands["DELETE Game"].Parameters["@id"].Value = id.ToString();
                return ExecuteNonQueryCommand(Commands["DELETE Game"]);
            }
        }

        /// <summary>
        /// Supprime l'entrée d'un message de la base de données
        /// </summary>
        /// <param name="m">Message à supprimer</param>
        /// <returns>true en cas de succès, false en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public bool DeleteMessage(Message m)
        {
            if (m == null)
            {
                ConstraintError(DB_ConstraintError.NullMessage, "null message");
            }
            else if (m.GameId == Guid.Empty)
            {
                ConstraintError(DB_ConstraintError.EmptyGameId, "empty game id");
            }
            else if (m.SenderId < -1)
            {
                ConstraintError(DB_ConstraintError.InvalidPlayerId, "invalid sender id");
            }
            else
            {
                Commands["DELETE Message"].Parameters["@game_id"].Value = m.GameId.ToString();
                Commands["DELETE Message"].Parameters["@sender_id"].Value = m.SenderId;
                Commands["DELETE Message"].Parameters["@date"].Value = DateToDBText(m.Date);
                return ExecuteNonQueryCommand(Commands["DELETE Message"]);
            }
            return false;
        }




        // Actions

        /// <summary>
        /// Modifie l'entrée d'un joueur de la base de données
        /// </summary>
        /// <param name="pl">Joueur à mettre à jour</param>
        /// <returns>true en cas de succès, false en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public bool UpdatePlayer(Player pl)
        {
            bool retValue = false;
            if (pl == null)
            {
                ConstraintError(DB_ConstraintError.NullPlayer, "null player");
            }
            else if (pl.IdGame != Guid.Empty && pl.IdInGame < 0)
            {
                ConstraintError(DB_ConstraintError.InvalidIdInGame, "invalid id in game");
            }
            else
            {
                Commands["UPDATE Player"].Parameters["@type"].Value = (int)pl.Type;
                Commands["UPDATE Player"].Parameters["@pseudonym"].Value = pl.Name;
                Commands["UPDATE Player"].Parameters["@avatar"].Value = pl.Avatar;

                if (pl.IdGame == Guid.Empty)
                {
                    Commands["UPDATE Player"].Parameters["@current_game_id"].Value = null;
                    Commands["UPDATE Player"].Parameters["@id_in_game"].Value = null;
                    Commands["UPDATE Player"].Parameters["@is_ready"].Value = false;
                }
                else
                {
                    Commands["UPDATE Player"].Parameters["@current_game_id"].Value = pl.IdGame.ToString();
                    Commands["UPDATE Player"].Parameters["@id_in_game"].Value = pl.IdInGame;
                    Commands["UPDATE Player"].Parameters["@is_ready"].Value = pl.IsReady;
                }

                Commands["UPDATE Player"].Parameters["@id"].Value = pl.IdDB;

                retValue = ExecuteNonQueryCommand(Commands["UPDATE Player"]);
                if (LastErrorMessage != null)
                {
                    if (LastErrorMessage == "constraint failed\r\nUNIQUE constraint failed: Player.pseudonym")
                    {
                        ConstraintError(DB_ConstraintError.PseudonymAlreadyTaken, "pseudonym already taken");
                    }
                    else if (LastErrorMessage == "Player.id_in_game >= Player.current_game.players_max")
                    {
                        ConstraintError(DB_ConstraintError.FullGame, "id in game greater or equal than game's max player count");
                    }
                    else if (LastErrorMessage == "constraint failed\r\nUNIQUE constraint failed: Player.current_game_id, Player.id_in_game")
                    {
                        ConstraintError(DB_ConstraintError.IdInGameAlreadyTaken, "id in game already taken");
                    }
                }
            }
            return retValue;
        }

        /// <summary>
        /// Met à jour le mot de passe d'un joueur dans la base de données
        /// </summary>
        /// <param name="playerId">Id du joueur dans la base de données</param>
        /// <param name="newPassword">Nouveau mot de passe</param>
        /// <returns>true en cas de succès, false en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public bool UpdatePlayerPassword(int playerId, string newPassword)
        {
            bool retValue = false;
            if (playerId < 0)
            {
                ConstraintError(DB_ConstraintError.InvalidPlayerId, "invalid player id");
            }
            else
            {
                Commands["UPDATE PLAYER PASSWORD"].Parameters["@hashed_password"].Value = newPassword;
                Commands["UPDATE PLAYER PASSWORD"].Parameters["@id"].Value = playerId;
                retValue = ExecuteNonQueryCommand(Commands["UPDATE PLAYER PASSWORD"]);
                if (LastErrorMessage != null && LastErrorMessage == "Null password for registered player")
                {
                    ConstraintError(DB_ConstraintError.NullPassword, "null password");
                }
            }
            return retValue;
        }

        /// <summary>
        /// Incrémente les compteurs de parties jouées et gagnées dans la base de données
        /// </summary>
        /// <param name="playerId">Id du joueur dans la base de données</param>
        /// <returns>true en cas de succès, false en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public bool WinGame(int playerId)
        {
            if (playerId < 0)
            {
                ConstraintError(DB_ConstraintError.InvalidPlayerId, "invalid player id");
                return false;
            }
            else
            {
                Commands["WIN GAME"].Parameters["@id"].Value = playerId;
                return ExecuteNonQueryCommand(Commands["WIN GAME"]);
            }
        }

        /// <summary>
        /// Incrémente le compteur de parties jouées
        /// </summary>
        /// <param name="playerId">Id du joueur dans la base de données</param>
        /// <returns>true en cas de succès, false en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public bool LoseGame(int playerId)
        {
            if (playerId < 0)
            {
                ConstraintError(DB_ConstraintError.InvalidPlayerId, "invalid player id");
                return false;
            }
            else
            {
                Commands["LOSE GAME"].Parameters["@id"].Value = playerId;
                return ExecuteNonQueryCommand(Commands["LOSE GAME"]);
            }
        }

        /// <summary>
        /// Connecte un joueur à une partie
        /// </summary>
        /// <param name="playerId">Id du joueur dans la base de données</param>
        /// <param name="id">Id de la partie dans la base de données</param>
        /// <returns>true en cas de succès, false en cas d'erreur</returns>
        /// <remarks>
        /// Met la valeur de is_ready dans la table Player à false
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public bool Connect(int playerId, Guid id, int idInGame)
        {
            bool retValue = false;
            if (playerId < 0)
            {
                ConstraintError(DB_ConstraintError.InvalidPlayerId, "invalid player id");
            }
            else if (id == Guid.Empty)
            {
                ConstraintError(DB_ConstraintError.EmptyGameId, "empty game id");
            }

            else if (idInGame < 0)
            {
                ConstraintError(DB_ConstraintError.InvalidIdInGame, "invalid id in game");
            }
            else
            {
                Commands["CONNECT"].Parameters["@game_id"].Value = id.ToString();
                Commands["CONNECT"].Parameters["@id_in_game"].Value = idInGame;
                Commands["CONNECT"].Parameters["@is_ready"].Value = false;
                Commands["CONNECT"].Parameters["@player_id"].Value = playerId;
                retValue = ExecuteNonQueryCommand(Commands["CONNECT"]);
                if (LastErrorMessage != null)
                {
                    if (LastErrorMessage == "Player.id_in_game >= Player.current_game.players_max")
                    {
                        ConstraintError(DB_ConstraintError.FullGame, "id in game greater or equal than game's max player count");
                    }
                    else if (LastErrorMessage == "constraint failed\r\nUNIQUE constraint failed: Player.current_game_id, Player.id_in_game")
                    {
                        ConstraintError(DB_ConstraintError.IdInGameAlreadyTaken, "id in game already taken");
                    }
                }
            }
            return retValue;
        }

        /// <summary>
        /// Déconnecte un joueur de sa partie en cours
        /// </summary>
        /// <param name="playerId">Id du joueur dans la base de données</param>
        /// <returns>true en cas de succès, false en cas d'erreur</returns>
        /// <remarks>
        /// Met la valeur de is_ready dans la table Player à false
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public bool Disconnect(int playerId)
        {
            if (playerId < 0)
            {
                ConstraintError(DB_ConstraintError.InvalidPlayerId, "invalid player id");
                return false;
            }
            else
            {
                Commands["CONNECT"].Parameters["@game_id"].Value = null;
                Commands["CONNECT"].Parameters["@id_in_game"].Value = null;
                Commands["CONNECT"].Parameters["@is_ready"].Value = false;
                Commands["CONNECT"].Parameters["@player_id"].Value = playerId;
                return ExecuteNonQueryCommand(Commands["CONNECT"]);
            }
        }

        /// <summary>
        /// Met la valeur de IsReady d'un joueur dans la base de donées
        /// </summary>
        /// <param name="playerId">Id du joueur dans la base de données</param>
        /// <param name="isReady">Variable indiquant si le joueur est prêt</param>
        /// <returns>true en cas de succès, false en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public bool SetPlayerReady(int playerId, bool isReady)
        {
            bool retValue = false;
            if (playerId < 0)
            {
                ConstraintError(DB_ConstraintError.InvalidPlayerId, "invalid player id");
            }
            else
            {
                Commands["SET PLAYER READY"].Parameters["@is_ready"].Value = isReady;
                Commands["SET PLAYER READY"].Parameters["@player_id"].Value = playerId;
                retValue = ExecuteNonQueryCommand(Commands["SET PLAYER READY"]);
            }
            return retValue;
        }

        /// <summary>
        /// Renomme une partie dans la base de données
        /// </summary>
        /// <param name="id">Id de la partie dans la base de données</param>
        /// <param name="name">Nouveau nom</param>
        /// <returns>true en cas de succès, false en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public bool RenameGame(Guid id, string name)
        {
            if (id == Guid.Empty)
            {
                ConstraintError(DB_ConstraintError.EmptyGameId, "empty game id");
                return false;
            }
            else if (name == null)
            {
                ConstraintError(DB_ConstraintError.NullGameName, "null game name");
                return false;
            }
            else
            {
                Commands["RENAME GAME"].Parameters["@id"].Value = name;
                Commands["RENAME GAME"].Parameters["@name"].Value = id.ToString();
                return ExecuteNonQueryCommand(Commands["RENAME GAME"]);
            }
        }

        /// <summary>
        /// Met à jour le mot de passe d'une partie dans la base de données
        /// </summary>
        /// <param name="id">Id de la partie dans la base de données</param>
        /// <param name=" newPassword">Nouveau mot de passe</param>
        /// <returns>true en cas de succès, false en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public bool UpdateGamePassword(Guid id, string newPassword)
        {
            bool retValue = false;
            if (id == Guid.Empty)
            {
                ConstraintError(DB_ConstraintError.EmptyGameId, "empty game id");
            }
            else
            {
                Commands["UPDATE GAME PASSWORD"].Parameters["@hashed_password"].Value = newPassword;
                Commands["UPDATE GAME PASSWORD"].Parameters["@id"].Value = id.ToString();
                return ExecuteNonQueryCommand(Commands["UPDATE GAME PASSWORD"]);
            }
            return retValue;
        }

        /// <summary>
        /// Change l'état d'une partie dans la base de données
        /// </summary>
        /// <param name="id">Id de la partie dans la base de données</param>
        /// <param name="state">Nouvel état</param>
        /// <returns>true en cas de succès, false en cas d'erreur</returns>
        /// <remarks>
        /// Met à jour les valeurs LastError, LastConstraintError et LastErrorMessage
        /// </remarks>
        public bool UpdateGameState(Guid id, GameState state)
        {
            if (id == Guid.Empty)
            {
                ConstraintError(DB_ConstraintError.EmptyGameId, "empty game id");
                return false;
            }
            else
            {
                Commands["UPDATE GAME STATE"].Parameters["@state"].Value = state;
                Commands["UPDATE GAME STATE"].Parameters["@id"].Value = id.ToString();
                return ExecuteNonQueryCommand(Commands["UPDATE GAME STATE"]);
            }
        }
    }
}
