using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Noyau.View;
using pack = Network.View;
using Util.View;
using System;
using AI.Controller;
using UnityEngine.SceneManagement;

/* Ce script est utilise pour s'occuper de tout ce qui est "logistique" du jeu */
class UIControllerScript : MonoBehaviour
{
    [Header("Audio")]
    public int effetsSonores = 50; // Valeur par défaut des effets sonores
    public int musique = 50; // Valeur par défaut de la musique
    private AudioSource fxSource = null;
    private AudioSource musicSource = null;
    public AudioClip startMusic = null;
    public AudioClip cityBuild = null;
    public AudioClip colonyBuild = null;
    public AudioClip roadBuild = null;
    public AudioClip tradeClip = null;
    public AudioClip banditClip = null;
    public AudioClip victoryClip = null;
    public AudioClip exchangeMenuClip = null;

    [Header("Video")]
    public int langue = 0;
    public bool fullscreen = true;
    public int bright = 100;
    private bool isInFullscreen = false;

    [Header("Options")]
    private Transform world = null;
    private Slider sliderEffets = null;
    private Slider sliderMusic = null;
    private CheckBox checkbox_fullscreen = null;
    private RadioButton fr = null;
    private RadioButton en = null;

    [Header("Game")]
    private int nbJoueurs = 4;
    private int nbOrdis = 0;
    private int nbJoueursTotal = 4;
    private int gridSize = 5;
    //private int currentPlayerDisplay = 0;
    private IHexGrid gameGrid;
    private IGame currentGame;
    private Coordinate newBanditCoordinate;
    private bool playerDisplayDisabled = true;
    private int currentDisplayedPlayer = 0;
    private int harborExchangeValue = 0;
    private int[][] offeredRessources = new int[4][];
    private int[][] ressourcesDefausse = new int[4][];
    private int playerToExchangeTo = -1;
    private int playerToExchangeToInventory = -1;
    private int currentPlayerDiscard = 0;
    public TMP_ColorGradient[] PlayersColors = new TMP_ColorGradient[4];
    private bool devCardUsedThisTurn = false;
    private bool isWaitingForBandit = false;

    private bool isUsingDevCard = false;
    private bool isUningDevCard2 = false;
    private Coordinate firstRoad;
    public GameObject textObject = null;
    public GameObject serverObject = null;

    public int maxMessages = 30;
    private List<Message> messageList = new List<Message>();
    public Color playerMessage = new Color(), constructionMessage = new Color(), tradeMessage = new Color(), AIMessage = new Color(), banditMove = new Color(), generalMessage = new Color(), diceInfo = new Color(), devCardInfo = new Color();
    private List<Serveur> serveurList = new List<Serveur>();

    [Header("Attente Reseau")]
    private int nbJoueursConnecte = 0;
    private int nbJoueursPret = 0;
    private int playerNumber = 1;
    private Guid theServerIP;
    private List<(int, string)> infoPlayers;
    private bool gotID = false;

    [Header("Sprites")]
    public Sprite carriere = null;
    public Sprite champ = null;
    public Sprite foret = null;
    public Sprite montagne = null;
    public Sprite prairie = null;
    public Sprite desert = null;
    public Sprite baseBandit = null;
    public Sprite token2 = null;
    public Sprite token3 = null;
    public Sprite token4 = null;
    public Sprite token5 = null;
    public Sprite token6 = null;
    public Sprite token8 = null;
    public Sprite token9 = null;
    public Sprite token10 = null;
    public Sprite token11 = null;
    public Sprite token12 = null;
    public Sprite defaultAvatar0 = null;
    public Sprite defaultAvatar1 = null;
    public Sprite defaultAvatar2 = null;
    public Sprite defaultAvatar3 = null;
    public Sprite robotAvatar = null;
    public Sprite brickIcon = null;
    public Sprite lumberIcon = null;
    public Sprite wheatIcon = null;
    public Sprite woolIcon = null;
    public Sprite oreIcon = null;
    public Sprite nullSprite = null;
    public Sprite notGreatestArmy = null;
    public Sprite GreatestArmy = null;
    public Sprite notLongestRoad = null;
    public Sprite LongestRoad = null;
    public Sprite thiefSprite = null;
    public Sprite lockIcon = null;

    [Header("Autre")]
    public Texture2D defaultMouse = null; // texture correspondant au curseur de la souris
    private Transform dataSave = null;
    private Transform radioGroupNbJoueursLocal = null;
    private Transform radioGroupNbOrdisLocal = null;
    private Transform radioGroupNbJoueursReseau = null;
    private Transform radioGroupNbOrdisReseau = null;
    private Transform profilsJoueurs = null;
    private Transform radioButtonPersonalise = null;
    private Transform theGrid = null;
    private Transform launchButton = null;
    private Transform createButtonReseau = null;
    private Transform launchButtonReseau = null;
    private Transform serverName = null;
    private Transform checkboxPrivate = null;
    private Transform serverPassword = null;
    private Transform labelServerPassword = null;
    private Transform joueursConnecte = null;
    private Transform joueursPret = null;
    private Transform playersDisplay = null;
    private Transform gridDisplay = null;
    private Transform cancelBuildMode = null;
    private Transform exchangeMenu = null;
    private Transform discardMenu = null;
    private Transform chat = null;
    private Transform choosePlayer = null;
    private Transform victoryScreen = null;
    private Transform monopolyMenu = null;
    private Transform pairMenu = null;
    private Transform animatedTitle = null;
    private Transform errorMenu = null;
    private Transform wrongPasswordMenu = null;
    private Transform chatScrollView = null;
    private Transform requiredRessourcesDisplay = null;
    private Transform AIDifficulty = null;
    private Transform labelAlzheimer = null;
    private Transform playerDisconnect = null;
    private Transform errorTimeout = null;

    private Transform buildColony = null;
    private Transform buildCity = null;
    private Transform buildRoad = null;
    private Transform buyDevCard = null;
    private Transform endTurn = null;

    private Transform diceDisplay = null;
    private Transform die1 = null;
    private Transform die2 = null;
    private Transform endDieButton = null;
    private Transform rollDieButton = null;

    private Transform validateButton0 = null;
    private Transform validateButton1 = null;
    private Transform validateButton2 = null;
    private Transform validateButton3 = null;
    private Transform validateButton0Reseau = null;
    private Transform validateButton1Reseau = null;
    private Transform validateButton2Reseau = null;
    private Transform validateButton3Reseau = null;


    public Transform chatInput = null;
    private Transform onlineLobbyPlayersIcon = null;
    private Transform theServerList = null;

    private Transform inputFiledIP = null;
    private Transform joinGameButton = null;
    private Transform changeProfile = null;
    private Transform lobbyReseau = null;
    private Transform passwordMenu = null;

    private Transform menuTransition = null;
    private Transform sceneTransition = null;

    private bool eventInstantiated = false;

    private bool isOnline;

    public IHexGrid gridReseau=null;

    private AIManager aiManager = null;

    private bool needToRefresh = true;


    public void Start()
    {
        if (defaultMouse != null) Cursor.SetCursor(defaultMouse, Vector2.zero, CursorMode.Auto);
        currentGame = GameView.Instance.Game;

        GameView.Instance.RegisterControllerHandlers();
        InstantiateEvents();
        aiManager = GameObject.Find("AIManager").GetComponent<AIManager>();
        GameObject.DontDestroyOnLoad(GameObject.Find("AIManager"));
        aiManager.RegisterEventHandlers();

        SwitchSceneObjects();

        LoadOptions();
        SaveOptions();
        StartMusic();
        infoPlayers = new List<(int, string)>();
        isOnline = dataSave.GetComponent<UIData>().IsOnline;

        if (isOnline)
        {
            chatInput.gameObject.SetActive(true);
            aiManager.UnregisterEventHandlers();
        }

        if (playersDisplay != null)
            DisplayPlayerCard();

        offeredRessources[0] = new int[5] { 0, 0, 0, 0, 0 };
        offeredRessources[1] = new int[5] { 0, 0, 0, 0, 0 };
        offeredRessources[2] = new int[5] { 0, 0, 0, 0, 0 };
        offeredRessources[3] = new int[5] { 0, 0, 0, 0, 0 };

        ressourcesDefausse[0] = new int[5] { 0, 0, 0, 0, 0 };
        ressourcesDefausse[1] = new int[5] { 0, 0, 0, 0, 0 };
        ressourcesDefausse[2] = new int[5] { 0, 0, 0, 0, 0 };
        ressourcesDefausse[3] = new int[5] { 0, 0, 0, 0, 0 };

        infoPlayers = new List<(int, string)>();

        if (gridDisplay != null && currentGame.Players[currentGame.CurrentPlayer].IsIA)
        {
            BaseEventArgs ev = new BaseEventArgs(currentGame.Id, isOnline);
            //if (isOnline) Network.View.NetworkView.Instance.OnFirstPlayerAI(ev);
            GameView.Instance.OnFirstPlayerAI(ev);
        }

        if (isOnline)
        {
            if (gridDisplay != null && (dataSave.GetComponent<UIData>().selfInfo.Item3 - 1) == currentGame.CurrentPlayer)
            {
                ToggleColony(true);
                ToggleCity(false);
                ToggleRoads(false);
            }

            labelAlzheimer.GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().selfInfo.Item1;
        }
        else
        {
            ToggleColony(true);
            ToggleCity(false);
            ToggleRoads(false);
        }

        if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByBuildIndex(1)))
        {
            if (langue == 0)
                PopTitle("Phase d'Initialisation");
            else if (langue == 1)
                PopTitle("Initialisation Phase");
        }
    }

    public void Update()
    {
        if (chatInput != null)
        {
            //Gestion de l'envoie d'un message dans le chat
            TMP_InputField theInputField = chatInput.GetComponent<TMP_InputField>();
            //le message est contenu dans theInputField.text
            if (theInputField.text != "")
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    string theMessage = dataSave.GetComponent<UIData>().selfInfo.Item1 + ":" + theInputField.text;

                    //Afficher le message sur les machines des autres joueurs
                    if (isOnline) NetworkClient.Instance.sendMessage(theMessage);

                    //Clear le texte une fois qu'il est envoyé
                    theInputField.text = "";
                }
            }
        }
    }

    public void InstantiateEvents()
    {
        if (eventInstantiated)
            return;
        if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByBuildIndex(1)))
        {
            GameView.Instance.ConstructionPhaseBegin += ConstructionPhaseBeginHandler;
            GameView.Instance.HarvestPhaseBegin += HarvestPhaseBeginHandler;
            GameView.Instance.DiscardPhaseBegin += DiscardPhaseBeginHandler;
            GameView.Instance.ExchangePhaseBegin += ExchangePhaseBeginHandler;
            GameView.Instance.BanditMoveBegin += BanditMoveBeginHandler;
            GameView.Instance.ConstructionDone += ConstructionDoneHandler;
            GameView.Instance.ExchangeDone += ExchangeDoneHandler;
            GameView.Instance.BanditMove += BanditMoveHandler;
            GameView.Instance.Victory += VictoryHandler;

            GameView.Instance.KnightCardUse += KnightCardUseHandler;
            GameView.Instance.MonopolyCardUse += MonopolyCardUseHandler;
            GameView.Instance.ResourcePairCardUse += ResourcePairCardUseHandler;
            GameView.Instance.RoadConstructionCardUse += RoadConstructionCardUseHandler;

            eventInstantiated = true;
        }
    }

    public void UninstantiateEvents()
    {
        if (!eventInstantiated)
            return;
        if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByBuildIndex(1)))
        {
            GameView.Instance.ConstructionPhaseBegin -= ConstructionPhaseBeginHandler;
            GameView.Instance.HarvestPhaseBegin -= HarvestPhaseBeginHandler;
            GameView.Instance.DiscardPhaseBegin -= DiscardPhaseBeginHandler;
            GameView.Instance.ExchangePhaseBegin -= ExchangePhaseBeginHandler;
            GameView.Instance.BanditMoveBegin -= BanditMoveBeginHandler;
            GameView.Instance.ConstructionDone -= ConstructionDoneHandler;
            GameView.Instance.ExchangeDone -= ExchangeDoneHandler;
            GameView.Instance.BanditMove -= BanditMoveHandler;
            GameView.Instance.Victory -= VictoryHandler;

            GameView.Instance.KnightCardUse -= KnightCardUseHandler;
            GameView.Instance.MonopolyCardUse -= MonopolyCardUseHandler;
            GameView.Instance.ResourcePairCardUse -= ResourcePairCardUseHandler;
            GameView.Instance.RoadConstructionCardUse -= RoadConstructionCardUseHandler;

            eventInstantiated = false;
        }
    }

    public void StartMusic()
    {
        musicSource.loop = true;
        musicSource.clip = startMusic;
        musicSource.PlayDelayed(0.3f);
    }

    public void SetEF()
    {
        effetsSonores = (int)sliderEffets.value;
    }

    public void SetMusique()
    {
        musique = (int)sliderMusic.value;
    }

    public void SetLangue(int newLangue)
    {
        langue = newLangue;
    }

    public void SetScreen()
    {
        fullscreen = checkbox_fullscreen.isChecked;
    }

    public void LoadOptions()
    {
        dataSave.transform.GetComponent<UIData>().LoadOptions();
        effetsSonores = dataSave.transform.GetComponent<UIData>().effetsSonores;
        sliderEffets.value = effetsSonores;

        musique = dataSave.transform.GetComponent<UIData>().musique;
        sliderMusic.value = musique;

        langue = dataSave.transform.GetComponent<UIData>().langue;
        if (langue == 0)
        {
            fr.MouseEnter();
            fr.MouseClick();
        }
        else if (langue == 1)
        {
            en.MouseEnter();
            en.MouseClick();
        }

        fullscreen = dataSave.transform.GetComponent<UIData>().fullscreen;
        checkbox_fullscreen.isChecked = fullscreen;
    }

    public void SaveOptions()
    {
        dataSave.transform.GetComponent<UIData>().SaveOptions(effetsSonores, musique, langue, fullscreen);
        fxSource.volume = ((float)effetsSonores / 100);

        musicSource.volume = ((float)musique / 100);

        ChangeLangue(world, langue);

        SetFullscreen(fullscreen);
    }

    public void ResetOptions()
    {
        effetsSonores = 50;
        musique = 50;
        fullscreen = true;
        langue = 0;
        SaveOptions();
        LoadOptions();
    }


    public void ChangeLangue(Transform racine, int langue)
    {
        int i;
        for (i = 0; i < racine.childCount; i++)
        {
            /* Traitement */
            if (racine.GetChild(i).GetComponent<PushButton>() != null)
                racine.GetChild(i).GetComponent<PushButton>().ChangeLangue(langue);
            if (racine.GetChild(i).GetComponent<RadioButton>() != null)
                racine.GetChild(i).GetComponent<RadioButton>().ChangeLangue(langue);
            if (racine.GetChild(i).GetComponent<Label>() != null)
                racine.GetChild(i).GetComponent<Label>().ChangeLangue(langue);

            /* Parcours */
            if (racine.GetChild(i).childCount != 0 && racine.tag != "NoTranslate")
                ChangeLangue(racine.GetChild(i), langue);
        }
    }

    public void SetFullscreen(bool makeFullscreen)
    {
        if (makeFullscreen && !isInFullscreen)
        {
            Screen.SetResolution(2048, 1152, false);
            Screen.fullScreen = makeFullscreen;
            isInFullscreen = true;
        }
        else if (!makeFullscreen && isInFullscreen)
        {
            Screen.SetResolution(1024, 768, false);
            Screen.fullScreen = makeFullscreen;
            isInFullscreen = false;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        if (isOnline)
        {
            NetworkClient.Instance.disconnect();
        }
    }

    public void SetNBJoueurs(int nombre)
    {
        nbJoueurs = nombre;
        nbJoueursTotal = nbJoueurs + nbOrdis;
        ReajusteBoutonsNBJoueurs(true);
        nbJoueursTotal = nbJoueurs + nbOrdis;
        int i;
        for (i = 0; i < 4; i++)
        {
            profilsJoueurs.GetChild(i).GetChild(5).GetComponent<PushButton>().SetEnabled(true);
        }
        if (nbJoueurs != 4)
            profilsJoueurs.GetChild(nbJoueurs - 1).GetChild(5).GetComponent<PushButton>().SetEnabled(false);
    }

    public void SetNBOrdis(int nombre)
    {
        nbOrdis = nombre;
        nbJoueursTotal = nbJoueurs + nbOrdis;
        ReajusteBoutonsNBJoueurs(false);
        nbJoueursTotal = nbJoueurs + nbOrdis;
        int i;
        for (i = 0; i < 4; i++)
        {
            profilsJoueurs.GetChild(i).GetChild(5).GetComponent<PushButton>().SetEnabled(true);
        }
        profilsJoueurs.GetChild(nbJoueurs - 1).GetChild(5).GetComponent<PushButton>().SetEnabled(false);
    }

    public void ReajusteBoutonsNBJoueurs(bool estNBJoueurs)
    {
        if (nbJoueursTotal > 4)
        {
            if (estNBJoueurs)
            {
                nbOrdis = 4 - nbJoueurs;
                radioGroupNbOrdisLocal.GetChild(nbOrdis).GetComponent<RadioButton>().MouseEnter();
                radioGroupNbOrdisLocal.GetChild(nbOrdis).GetComponent<RadioButton>().MouseClick();

                radioGroupNbOrdisReseau.GetChild(nbOrdis).GetComponent<RadioButton>().MouseEnter();
                radioGroupNbOrdisReseau.GetChild(nbOrdis).GetComponent<RadioButton>().MouseClick();
            }
            else
            {
                nbJoueurs = 4 - nbOrdis;
                radioGroupNbJoueursLocal.GetChild(nbJoueurs - 1).GetComponent<RadioButton>().MouseEnter();
                radioGroupNbJoueursLocal.GetChild(nbJoueurs - 1).GetComponent<RadioButton>().MouseClick();

                radioGroupNbJoueursReseau.GetChild(nbJoueurs - 1).GetComponent<RadioButton>().MouseEnter();
                radioGroupNbJoueursReseau.GetChild(nbJoueurs - 1).GetComponent<RadioButton>().MouseClick();
            }
        }
        else if (nbJoueursTotal < 3)
        {
            if (estNBJoueurs)
            {
                nbOrdis = 3 - nbJoueurs;
                radioGroupNbOrdisLocal.GetChild(nbOrdis).GetComponent<RadioButton>().MouseEnter();
                radioGroupNbOrdisLocal.GetChild(nbOrdis).GetComponent<RadioButton>().MouseClick();

                radioGroupNbOrdisReseau.GetChild(nbOrdis).GetComponent<RadioButton>().MouseEnter();
                radioGroupNbOrdisReseau.GetChild(nbOrdis).GetComponent<RadioButton>().MouseClick();
            }
            else
            {
                nbJoueurs = 3 - nbOrdis;
                radioGroupNbJoueursLocal.GetChild(nbJoueurs - 1).GetComponent<RadioButton>().MouseEnter();
                radioGroupNbJoueursLocal.GetChild(nbJoueurs - 1).GetComponent<RadioButton>().MouseClick();

                radioGroupNbJoueursReseau.GetChild(nbJoueurs - 1).GetComponent<RadioButton>().MouseEnter();
                radioGroupNbJoueursReseau.GetChild(nbJoueurs - 1).GetComponent<RadioButton>().MouseClick();
            }
        }
    }

    public void TogglePasswordField()
    {
        if (checkboxPrivate.GetComponent<CheckBox>().isChecked)
        {
            serverPassword.gameObject.SetActive(true);
            labelServerPassword.gameObject.SetActive(true);
        }
        else
        {
            serverPassword.gameObject.SetActive(false);
            labelServerPassword.gameObject.SetActive(false);
        }
    }

    public void PrepareLaunchButtonLocal()
    {
        if (!CanStartGameLocal())
            launchButton.GetComponent<PushButton>().SetEnabled(false);
        else
            launchButton.GetComponent<PushButton>().SetEnabled(true);
    }

    public void PrepareCreateButtonReseau()
    {
        if (!CanCreateGameReseau())
            createButtonReseau.GetComponent<PushButton>().SetEnabled(false);
        else
            createButtonReseau.GetComponent<PushButton>().SetEnabled(true);
    }

    public void PrepareOnlineLobby()
    {
        nbJoueursConnecte = 1;
        nbJoueursPret = 0;
        playerNumber = 1;

        infoPlayers.Clear();

        string name = lobbyReseau.GetChild(11).GetComponentInChildren<TextMeshProUGUI>().text;
        int avatar = 0;
        if (changeProfile.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<RadioButton>().isChecked)
            avatar = 0;
        if (changeProfile.transform.GetChild(1).GetChild(3).GetChild(1).GetComponent<RadioButton>().isChecked)
            avatar = 1;
        if (changeProfile.transform.GetChild(1).GetChild(3).GetChild(2).GetComponent<RadioButton>().isChecked)
            avatar = 2;
        if (changeProfile.transform.GetChild(1).GetChild(3).GetChild(3).GetComponent<RadioButton>().isChecked)
            avatar = 3;

        infoPlayers.Add((avatar, name));

        for (int i = 0; i < 4; i++)
        {
            onlineLobbyPlayersIcon.transform.GetChild(i).GetChild(3).GetComponent<PushButton>().SetEnabled(true);
        }

        string nom = serverName.GetComponent<TMP_InputField>().text;
        bool access = checkboxPrivate.GetComponent<CheckBox>().isChecked;
        string pwd = serverPassword.GetComponent<TMP_InputField>().text;
        Debug.Log("Creation serveur d'accès: " + access);
        Debug.Log("Creation serveur de nom: " + nom);
        Debug.Log("Creation serveur pwd " + pwd);
        NetworkClient.Instance.createServer(nbJoueursTotal, nbOrdis,null, nom, !access, pwd, new Guid(), getAvatar(), getName());

        UpdateOnlineLobby();
        PrepareLaunchButtonLocal();
    }

    public void UpdateOnlineLobby()
    {
        joueursConnecte.transform.GetComponentInChildren<TextMeshProUGUI>().text = nbJoueursConnecte + "/" + nbJoueurs;
        joueursPret.transform.GetComponentInChildren<TextMeshProUGUI>().text = nbJoueursPret + "/" + nbJoueurs;

        if (playerNumber == 1)
            launchButtonReseau.gameObject.SetActive(true);
        else
            launchButtonReseau.gameObject.SetActive(false);

        for (int i = 0; i < nbJoueursConnecte; i++)
        {
            onlineLobbyPlayersIcon.transform.GetChild(i).gameObject.SetActive(true);

            if (infoPlayers[i].Item1 == 0)
                onlineLobbyPlayersIcon.transform.GetChild(i).GetChild(1).GetComponent<Image>().sprite = defaultAvatar0;
            else if (infoPlayers[i].Item1 == 1)
                onlineLobbyPlayersIcon.transform.GetChild(i).GetChild(1).GetComponent<Image>().sprite = defaultAvatar1;
            else if (infoPlayers[i].Item1 == 2)
                onlineLobbyPlayersIcon.transform.GetChild(i).GetChild(1).GetComponent<Image>().sprite = defaultAvatar2;
            else if (infoPlayers[i].Item1 == 3)
                onlineLobbyPlayersIcon.transform.GetChild(i).GetChild(1).GetComponent<Image>().sprite = defaultAvatar3;

            onlineLobbyPlayersIcon.transform.GetChild(i).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = infoPlayers[i].Item2;

            if (playerNumber - 1 == i)
                onlineLobbyPlayersIcon.transform.GetChild(i).GetChild(3).gameObject.SetActive(true);
            else
                onlineLobbyPlayersIcon.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);
        }
        for (int i = nbJoueursConnecte; i < 4; i++)
        {
            onlineLobbyPlayersIcon.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);
            onlineLobbyPlayersIcon.transform.GetChild(i).gameObject.SetActive(false);
        }

        PrepareLaunchButtonReseau();
    }

    public void SetServerListMenu()
    {
        Debug.Log("je suis appelée");
        //messageList.Remove(messageList[0]);
        serveurList.Clear();

        for (int i = 0; i < theServerList.childCount; i++)
        {
            //serveurList.Remove(serveurList[0]);
            Destroy(theServerList.GetChild(i).gameObject);
        }
        // trouver la liste de serveur et utiliser AddServerToList pour chaque serveur
        if (Network.View.NetworkView.Instance.getGamesNetwork() != null)
            foreach (GameLine gl in Network.View.NetworkView.Instance.getGamesNetwork())
            {
                Debug.Log("game nam" + gl.NameGame);
                AddServerToList(gl.NameGame, gl.Id);
            }
    }

    public void RefreshServerList()
    {
        Network.View.NetworkView.Instance.OnRefresh(new Refresh());
    }


    public void AddServerToList(string serverName, Guid serverIP)
    {

        Serveur newServer = new Serveur();
        newServer.serveurName = serverName;

        GameObject newElement = Instantiate(serverObject, theServerList.transform);


        GameLine game = Network.View.NetworkView.Instance.getGame(serverIP);
        if (game.nbConnected < (game.NbPlayers - game.NbIA))
        {
            if (game.Access)
            {
                newElement.transform.GetComponent<Button>().onClick.AddListener(() => ConnectToServer(serverIP, true, "", getName(), getAvatar()));
            }
            else
            {
                newElement.transform.GetComponent<Button>().onClick.AddListener(() => ShowPasswordMenu(serverIP));
                newElement.transform.GetChild(1).GetComponent<Image>().sprite = lockIcon;
            }
        }
        else
            newElement.transform.GetChild(0).GetComponent<TextMeshProUGUI>().colorGradientPreset = new TMP_ColorGradient(new Color(1, 0, 0));
        newElement.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newServer.serveurName + " [" + game.nbConnected + "/" + (game.NbPlayers - game.NbIA).ToString() + "]";

        serveurList.Add(newServer);
    }

    public void ShowPasswordMenu(Guid serverIP)
    {
        theServerIP = serverIP;
        passwordMenu.gameObject.SetActive(true);
    }

    public void PasswordServerConnect()
    {
        string password = passwordMenu.GetChild(2).GetComponent<TMP_InputField>().text;

        passwordMenu.GetChild(3).GetComponent<PushButton>().MouseExit();
        passwordMenu.gameObject.SetActive(false);

        ConnectToServer(theServerIP, false, password, getName(), getAvatar());
    }

    public void ConnectToServer(Guid ip, bool access, string pwd, string name, int avatar)
    {
        NetworkClient.Instance.JoinGame(ip, access, pwd, name, avatar);

        GameLine game = Network.View.NetworkView.Instance.getGame(ip);
        nbJoueurs = game.NbPlayers - game.NbIA;
        nbJoueursConnecte = game.nbConnected;
        nbJoueursPret = game.nbReady;
    }

    public bool CanStartGameLocal()
    {
        bool canStartGame = true;
        int i;
        for (i = 0; i < nbJoueurs; i++)
        {
            if (profilsJoueurs.GetChild(i).GetChild(1).GetComponent<TMP_InputField>().text == "")
            {
                canStartGame = false;
                break;
            }
        }
        if (radioButtonPersonalise.GetComponent<RadioButton>().isChecked)
        {
            if (!CheckTheGridLocal(theGrid, true))
                canStartGame = false;
        }
        return canStartGame;
    }

    public void SetChangeProfileButton()
    {
        if (changeProfile.transform.GetChild(1).GetChild(1).GetComponent<TMP_InputField>().text == "")
            changeProfile.transform.GetChild(2).GetComponent<PushButton>().SetEnabled(false);
        else
            changeProfile.transform.GetChild(2).GetComponent<PushButton>().SetEnabled(true);

    }

    public void UpdateProfile()
    {

        if (changeProfile.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<RadioButton>().isChecked)
        {
            lobbyReseau.GetChild(10).GetComponent<Image>().sprite = defaultAvatar0;
            dataSave.GetComponent<UIData>().selfInfo.Item2 = 0;
        }
        if (changeProfile.transform.GetChild(1).GetChild(3).GetChild(1).GetComponent<RadioButton>().isChecked)
        {
            lobbyReseau.GetChild(10).GetComponent<Image>().sprite = defaultAvatar1;
            dataSave.GetComponent<UIData>().selfInfo.Item2 = 1;
        }
        if (changeProfile.transform.GetChild(1).GetChild(3).GetChild(2).GetComponent<RadioButton>().isChecked)
        {
            lobbyReseau.GetChild(10).GetComponent<Image>().sprite = defaultAvatar2;
            dataSave.GetComponent<UIData>().selfInfo.Item2 = 2;
        }
        if (changeProfile.transform.GetChild(1).GetChild(3).GetChild(3).GetComponent<RadioButton>().isChecked)
        {
            lobbyReseau.GetChild(10).GetComponent<Image>().sprite = defaultAvatar3;
            dataSave.GetComponent<UIData>().selfInfo.Item2 = 3;
        }

        lobbyReseau.GetChild(11).GetComponentInChildren<TextMeshProUGUI>().text = changeProfile.transform.GetChild(1).GetChild(1).GetComponent<TMP_InputField>().text;
        dataSave.GetComponent<UIData>().selfInfo.Item1 = changeProfile.transform.GetChild(1).GetChild(1).GetComponent<TMP_InputField>().text;
    }

    public void ClickReadyButton(int playerNbr)
    {
        nbJoueursPret++;
        onlineLobbyPlayersIcon.transform.GetChild(playerNbr - 1).GetChild(3).GetComponent<PushButton>().MouseExit();
        onlineLobbyPlayersIcon.transform.GetChild(playerNbr - 1).GetChild(3).GetComponent<PushButton>().SetEnabled(false);

        //Envoyer au reseau le fait qu'il est prêt
        Network.View.NetworkView.Instance.OnReadyEvent(new ReadyEvent(true, Network.View.NetworkView.Instance.getIdGame(), Network.View.NetworkView.Instance.getIdInGame()));

        PrepareLaunchButtonReseau();

        UpdateOnlineLobby();
    }

    public bool CanCreateGameReseau()
    {
        bool canStartGame = true;
        if (serverName.GetComponent<TMP_InputField>().text == "")
            canStartGame = false;
        else if (checkboxPrivate.GetComponent<CheckBox>().isChecked && serverPassword.GetComponent<TMP_InputField>().text == "")
            canStartGame = false;
        return canStartGame;
    }

    public void PrepareLaunchButtonReseau()
    {
        if (nbJoueursConnecte == nbJoueurs && nbJoueursPret == nbJoueurs && playerNumber == 1)
            launchButtonReseau.transform.GetComponent<PushButton>().SetEnabled(true);
        else
            launchButtonReseau.transform.GetComponent<PushButton>().SetEnabled(false);
    }

    public bool CheckTheGridLocal(Transform racine, bool ret)
    {
        bool retValue = ret;
        int i;
        for (i = 0; i < racine.childCount; i++)
        {
            /* Traitement */
            if (racine.GetChild(i).GetComponent<ItemDropHandler>() != null)
            {
                retValue = racine.GetChild(i).GetComponent<ItemDropHandler>().IsFull();
            }
            /* Parcours */
            if (racine.GetChild(i).childCount != 0)
            {
                if (!CheckTheGridLocal(racine.GetChild(i), retValue))
                {
                    retValue = false;
                }
            }
        }
        return retValue;
    }

    public void CanJoinGameViaIP()
    {
        if (inputFiledIP.transform.GetComponent<TMP_InputField>().text == "")
            joinGameButton.GetComponent<PushButton>().SetEnabled(false);
        else
            joinGameButton.GetComponent<PushButton>().SetEnabled(true);
    }

    public void JoinGameViaIP()
    {
        string theIP = inputFiledIP.transform.GetComponent<TMP_InputField>().text;
        Guid theArg; //convertir la string en Guid

        // TODO a revoir
        ConnectToServer(theArg, true, "", getName(), getAvatar());
    }

    private int getAvatar()
    {
        int avatar = 0;
        if (changeProfile.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<RadioButton>().isChecked)
            avatar = 0;
        if (changeProfile.transform.GetChild(1).GetChild(3).GetChild(1).GetComponent<RadioButton>().isChecked)
            avatar = 1;
        if (changeProfile.transform.GetChild(1).GetChild(3).GetChild(2).GetComponent<RadioButton>().isChecked)
            avatar = 2;
        if (changeProfile.transform.GetChild(1).GetChild(3).GetChild(3).GetComponent<RadioButton>().isChecked)
            avatar = 3;

        return avatar;
    }

    private string getName()
    {
        return lobbyReseau.GetChild(11).GetComponentInChildren<TextMeshProUGUI>().text;
    }

    public void SetWaitingLobby()
    {
        joueursConnecte.GetChild(0).GetComponent<TextMeshProUGUI>().text = nbJoueursConnecte.ToString() + "/" + nbJoueurs.ToString();
        joueursPret.GetChild(0).GetComponent<TextMeshProUGUI>().text = nbJoueursPret.ToString() + "/" + nbJoueurs.ToString();
        if (nbJoueurs == nbJoueursConnecte && nbJoueurs == nbJoueursPret)
            launchButtonReseau.GetComponent<PushButton>().SetEnabled(true);
        else launchButtonReseau.GetComponent<PushButton>().SetEnabled(false);
    }

    public void GenerateGameDataLocal()
    {
        int i, j;
        for (i = 0; i < nbJoueursTotal; i++)
        {
            if (i < nbJoueurs)
            {
                string playerName = profilsJoueurs.GetChild(i).GetChild(1).GetComponent<TMP_InputField>().text;

                for (j = 0; j < 4; j++)
                    if (profilsJoueurs.GetChild(i).GetChild(4).GetChild(j).GetComponent<RadioButton>().isChecked)
                    {
                        dataSave.GetComponent<UIData>().players[i] = (playerName, j, false);
                        break;
                    }
            }
            else
                dataSave.GetComponent<UIData>().players[i] = ("Bot" + (i - nbJoueurs), 0, true);
        }

        gameGrid = null;
        if (radioButtonPersonalise.GetComponent<RadioButton>().isChecked)
        {
            Debug.Log("Test");
            gameGrid = GameView.DefaultGrid;
            ItemDragHandler tmpTile;
            ItemDragHandler tmpNumber;
            for (i = 0; i < gridSize; i++)
            {
                if (i <= gridSize / 2)
                {
                    for (j = 0; j < theGrid.GetChild(i).childCount; j++)
                    {
                        tmpTile = theGrid.GetChild(i).GetChild(j).GetChild(0).GetComponent<ItemDragHandler>();
                        gameGrid.TerrainTiles[new Coordinate(j - i, gridSize / 2 - j, i - gridSize / 2, Direction.NONE)].ChangeTerrainType(tmpTile.typeTile);
                        if (tmpTile.typeTile != TerrainType.DESERT)
                        {
                            tmpNumber = theGrid.GetChild(i).GetChild(j).GetChild(1).GetComponent<ItemDragHandler>();
                            gameGrid.TerrainTiles[new Coordinate(j - i, gridSize / 2 - j, i - gridSize / 2, Direction.NONE)].ChangeTerrainNumber(tmpNumber.number);

                        }
                        else
                        {
                            gameGrid.TerrainTiles[new Coordinate(j - i, gridSize / 2 - j, i - gridSize / 2, Direction.NONE)].ChangeTerrainNumber(0);
                            gameGrid.MoveThief(new Coordinate(j - i, gridSize / 2 - j, i - gridSize / 2, Direction.NONE));
                        }
                    }
                }
                else
                {
                    for (j = 0; j < theGrid.GetChild(i).childCount; j++)
                    {
                        tmpTile = theGrid.GetChild(i).GetChild(j).GetChild(0).GetComponent<ItemDragHandler>();
                        gameGrid.TerrainTiles[new Coordinate(j - gridSize / 2, (gridSize - 1) - i - j, i - gridSize / 2, Direction.NONE)].ChangeTerrainType(tmpTile.typeTile);
                        if (tmpTile.typeTile != TerrainType.DESERT)
                        {
                            tmpNumber = theGrid.GetChild(i).GetChild(j).GetChild(1).GetComponent<ItemDragHandler>();
                            gameGrid.TerrainTiles[new Coordinate(j - gridSize / 2, (gridSize - 1) - i - j, i - gridSize / 2, Direction.NONE)].ChangeTerrainNumber(tmpNumber.number);
                        }
                        else
                        {
                            gameGrid.TerrainTiles[new Coordinate(j - gridSize / 2, (gridSize / 2 - 1) - i - j, i - gridSize / 2, Direction.NONE)].ChangeTerrainNumber(0);
                            gameGrid.MoveThief(new Coordinate(j - gridSize / 2, (gridSize - 1) - i - j, i - gridSize / 2, Direction.NONE));
                        }
                    }
                }
            }

        }
        int aiDifficulty = 0;
        if (AIDifficulty.transform.GetChild(0).GetComponent<RadioButton>().isChecked)
            aiDifficulty = 0;
        else if(AIDifficulty.transform.GetChild(1).GetComponent<RadioButton>().isChecked)
            aiDifficulty = 1;
        else if(AIDifficulty.transform.GetChild(2).GetComponent<RadioButton>().isChecked)
            aiDifficulty = 2;

        GameCreateEventArgs e = new GameCreateEventArgs(nbJoueursTotal, nbOrdis, gameGrid, false, "", true, "", Guid.Empty, 0, "Guest", aiDifficulty);
        GameView.Instance.OnGameCreate(e);
    }



    public void GenerateGameDataReseau()
    {
        int i;
        for (i = 0; i < nbJoueursTotal; i++)
        {
            if (i < nbJoueurs)
            {
                string playerName = infoPlayers[i].Item2;
                int avatar = infoPlayers[i].Item1;
                dataSave.GetComponent<UIData>().players[i] = (playerName, avatar, false);
            }
            else
                dataSave.GetComponent<UIData>().players[i] = ("Bot" + (i - nbJoueurs), 0, true);
        }

        gameGrid = gridReseau;

        if (playerNumber == 1)
            NetworkClient.Instance.GameCreate(nbJoueursTotal, nbOrdis, null, getAvatar(), getName());


        /*
            GameCreateEventArgs e = new GameCreateEventArgs(nbJoueursTotal, nbOrdis, gameGrid, false, "", true, "", Guid.Empty);
            GameView.Instance.OnGameCreate(e);
        */
    }

    public void SwitchSceneObjects()
    {

        world = GameObject.Find("World").transform;
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            switch (go.name)
            {
                case "DataSave": dataSave = go.transform; break;
                case "SliderEffetsSonores": sliderEffets = go.GetComponent<Slider>(); break;
                case "SliderMusique": sliderMusic = go.GetComponent<Slider>(); break;
                case "CheckBoxFullscreen": checkbox_fullscreen = go.GetComponentInChildren<CheckBox>(); break;
                case "RadioButtonFr": fr = go.GetComponent<RadioButton>(); break;
                case "RadioButtonEn": en = go.GetComponent<RadioButton>(); break;
                case "FXSource": fxSource = go.GetComponent<AudioSource>(); break;
                case "MusicSource": musicSource = go.GetComponent<AudioSource>(); break;
                case "RadioGroupNbJoueursLocal": radioGroupNbJoueursLocal = go.transform; break;
                case "RadioGroupNbJoueursOnline": radioGroupNbJoueursReseau = go.transform; break;
                case "RadioGroupNbOrdisLocal": radioGroupNbOrdisLocal = go.transform; break;
                case "RadioGroupNbOrdisOnline": radioGroupNbOrdisReseau = go.transform; break;
                case "StackedWidgetProfilsJoueurs": profilsJoueurs = go.transform; break;
                case "RadioButtonPersonalise": radioButtonPersonalise = go.transform; break;
                case "TheGrid": theGrid = go.transform; break;
                case "PushButtonLancerLocal": launchButton = go.transform; break;
                case "PushButtonCreer": createButtonReseau = go.transform; break;
                case "PushButtonLancerOnline": launchButtonReseau = go.transform; break;
                case "InputFieldServerName": serverName = go.transform; break;
                case "CheckBoxMDP": checkboxPrivate = go.transform.GetChild(1); break;
                case "InputFieldServerMDP": serverPassword = go.transform; break;
                case "LabelServerMDP": labelServerPassword = go.transform; break;
                case "LabelJoueursConnecteTotal": joueursConnecte = go.transform; break;
                case "LabelJoueursPretTotal": joueursPret = go.transform; break;
                case "PlayersDisplay": { playersDisplay = go.transform; DisplayPlayerCard(); UpdateConstructionMap(); } break;
                case "GridDisplay": { gridDisplay = go.transform; DisplayBoard();} break;
                case "PushButtonCancel": cancelBuildMode = go.transform; break;
                case "ExchangeMenu": exchangeMenu = go.transform; break;
                case "DiscardMenu": discardMenu = go.transform; break;
                case "GameOver": victoryScreen = go.transform; break;
                case "AChat": chat = go.transform; break;
                case "PushButtonColony": buildColony = go.transform; break;
                case "PushButtonVille": buildCity = go.transform; break;
                case "PushButtonRoute": buildRoad = go.transform; break;
                case "PushButtonDeveloppement": buyDevCard = go.transform; break;
                case "PushButtonEndTurn": endTurn = go.transform; break;
                case "DiceDisplay": diceDisplay = go.transform; break;
                case "Die1": die1 = go.transform; break;
                case "Die2": die2 = go.transform; break;
                case "PushButtonDieRoll": rollDieButton = go.transform; break;
                case "EndDieRoll": endDieButton = go.transform; break;
                case "ChoosePlayer": choosePlayer = go.transform; InitiatePlayerChooseMenu(); break;
                case "PushButtonValider0": validateButton0 = go.transform; break;
                case "PushButtonValider1": validateButton1 = go.transform; break;
                case "PushButtonValider2": validateButton2 = go.transform; break;
                case "PushButtonValider3": validateButton3 = go.transform; break;
                case "PushButtonValider0Reseau": validateButton0Reseau = go.transform; break;
                case "PushButtonValider1Reseau": validateButton1Reseau = go.transform; break;
                case "PushButtonValider2Reseau": validateButton2Reseau = go.transform; break;
                case "PushButtonValider3Reseau": validateButton3Reseau = go.transform; break;
                case "MonopolyMenu": monopolyMenu = go.transform; break;
                case "PairMenu": pairMenu = go.transform; break;
                case "AnimatedTitle": animatedTitle = go.transform; break;
                case "BChatInput": chatInput = go.transform; break;
                case "LobbyPlayersIcon": onlineLobbyPlayersIcon = go.transform; break;
                case "ServerList": theServerList = go.transform; break;
                case "InputFieldIP": inputFiledIP = go.transform; break;
                case "PushButtonRejoindreServeur": joinGameButton = go.transform; break;
                case "ChangeProfile": changeProfile = go.transform; break;
                case "LobbyReseau": lobbyReseau = go.transform; break;
                case "PasswordMenu": passwordMenu = go.transform; break;
                case "MenuTransition": menuTransition = go.transform; break;
                case "SceneTransition": sceneTransition = go.transform; break;
                case "ErrorDisconnectMasterServer": errorMenu = go.transform; break;
                case "CChatScrollView": chatScrollView = go.transform; break;
                case "WrongPasswordMenu": wrongPasswordMenu = go.transform; break;
                case "RequiredRessourcesDisplay": requiredRessourcesDisplay = go.transform; break;
                case "RadioGroupDiffIA": AIDifficulty = go.transform; break;
                case "LabelAlzheimer": labelAlzheimer = go.transform; break;
                case "ErrorDisconnect": playerDisconnect = go.transform; break;
                case "ErrorTimeout": errorTimeout = go.transform; break;

                default: break;
            }
        }
    }

    public void ShowCurrentPlayerColor()
    {
        for (int i = 0; i < 4; i++)
        {
            playersDisplay.GetChild(1).GetChild(i).GetChild(0).gameObject.SetActive(false);
        }
        playersDisplay.GetChild(1).GetChild(currentGame.CurrentPlayer).GetChild(0).gameObject.SetActive(true);
    }

    public void DisplayPlayerCard()
    {
        nbJoueursTotal = currentGame.Players.Count;

        int i;
        for (i = 0; i < nbJoueursTotal; i++)
        {
            playersDisplay.GetChild(1).GetChild(i).gameObject.SetActive(true);

            ShowCurrentPlayerColor();

            playersDisplay.GetChild(1).GetChild(i).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[i].Item1;

            if (dataSave.GetComponent<UIData>().players[i].Item3 == false)
            {
                if (dataSave.GetComponent<UIData>().players[i].Item2 == 0)
                {
                    playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<Image>().sprite = defaultAvatar0;
                    playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<PushButton>().parDefaut = defaultAvatar0;
                    playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<PushButton>().onHover = defaultAvatar0;
                    playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<PushButton>().onClick = defaultAvatar0;
                }
                else if (dataSave.GetComponent<UIData>().players[i].Item2 == 1)
                {
                    playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<Image>().sprite = defaultAvatar1;
                    playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<PushButton>().parDefaut = defaultAvatar1;
                    playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<PushButton>().onHover = defaultAvatar1;
                    playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<PushButton>().onClick = defaultAvatar1;
                }
                else if (dataSave.GetComponent<UIData>().players[i].Item2 == 2)
                {
                    playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<Image>().sprite = defaultAvatar2;
                    playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<PushButton>().parDefaut = defaultAvatar2;
                    playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<PushButton>().onHover = defaultAvatar2;
                    playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<PushButton>().onClick = defaultAvatar2;
                }
                else
                {
                    playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<Image>().sprite = defaultAvatar3;
                    playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<PushButton>().parDefaut = defaultAvatar3;
                    playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<PushButton>().onHover = defaultAvatar3;
                    playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<PushButton>().onClick = defaultAvatar3;
                }
            }
            else
            {
                playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<Image>().sprite = robotAvatar;
                playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<PushButton>().parDefaut = robotAvatar;
                playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<PushButton>().onHover = robotAvatar;
                playersDisplay.GetChild(1).GetChild(i).GetChild(2).GetComponent<PushButton>().onClick = robotAvatar;
            }
        }
    }

    public void SwitchPlayerDisplay(int newDisplay)
    {
        if (dataSave.GetComponent<UIData>().players[newDisplay].Item3 == false)
        {
            if (dataSave.GetComponent<UIData>().players[newDisplay].Item2 == 0)
                playersDisplay.GetChild(0).GetChild(1).GetComponent<Image>().sprite = defaultAvatar0;
            else if (dataSave.GetComponent<UIData>().players[newDisplay].Item2 == 1)
                playersDisplay.GetChild(0).GetChild(1).GetComponent<Image>().sprite = defaultAvatar1;
            else if (dataSave.GetComponent<UIData>().players[newDisplay].Item2 == 2)
                playersDisplay.GetChild(0).GetChild(1).GetComponent<Image>().sprite = defaultAvatar2;
            else
                playersDisplay.GetChild(0).GetChild(1).GetComponent<Image>().sprite = defaultAvatar3;
        }
        else
            playersDisplay.GetChild(0).GetChild(1).GetComponent<Image>().sprite = robotAvatar;

        playersDisplay.GetChild(0).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[newDisplay].Item1;
        playersDisplay.GetChild(0).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().colorGradientPreset = PlayersColors[newDisplay];

        playersDisplay.GetChild(0).GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[newDisplay].VictoryPoints.ToString();

        playersDisplay.GetChild(0).GetChild(4).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[newDisplay].Ressources[RessourceType.WHEAT].ToString();
        playersDisplay.GetChild(0).GetChild(4).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[newDisplay].Ressources[RessourceType.BRICK].ToString();
        playersDisplay.GetChild(0).GetChild(4).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[newDisplay].Ressources[RessourceType.WOOL].ToString();
        playersDisplay.GetChild(0).GetChild(4).GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[newDisplay].Ressources[RessourceType.LUMBER].ToString();
        playersDisplay.GetChild(0).GetChild(4).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[newDisplay].Ressources[RessourceType.ORE].ToString();

        int route = 0;
        int colonie = 0;
        int ville = 0;

        foreach ((Coordinate, ConstructionType) construct in currentGame.Players[newDisplay].Constructions)
        {
            if (construct.Item2 == ConstructionType.ROAD)
                route++;
            else if (construct.Item2 == ConstructionType.SETTLEMENT)
                colonie++;
            else if (construct.Item2 == ConstructionType.CITY)
                ville++;
        }
        playersDisplay.GetChild(0).GetChild(5).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = route.ToString();
        playersDisplay.GetChild(0).GetChild(5).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = colonie.ToString();
        playersDisplay.GetChild(0).GetChild(5).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = ville.ToString();
        playersDisplay.GetChild(0).GetChild(5).GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[newDisplay].KnightCardsPlayed.ToString();

        if (currentGame.Players[newDisplay].HasGreatestArmy)
            playersDisplay.GetChild(0).GetChild(8).GetChild(0).GetComponent<Image>().sprite = GreatestArmy;
        else
            playersDisplay.GetChild(0).GetChild(8).GetChild(0).GetComponent<Image>().sprite = notGreatestArmy;

        if (currentGame.Players[newDisplay].HasLongestRoad)
            playersDisplay.GetChild(0).GetChild(8).GetChild(1).GetComponent<Image>().sprite = LongestRoad;
        else
            playersDisplay.GetChild(0).GetChild(8).GetChild(1).GetComponent<Image>().sprite = notLongestRoad;

        if (isOnline)
        {
            playersDisplay.GetChild(0).GetChild(6).GetChild(0).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "";
            playersDisplay.GetChild(0).GetChild(6).GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "";
            playersDisplay.GetChild(0).GetChild(6).GetChild(0).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = "";
            playersDisplay.GetChild(0).GetChild(6).GetChild(0).GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = "";
            playersDisplay.GetChild(0).GetChild(6).GetChild(0).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = "";
            for (int i = 0; i < 4; i++)
                playersDisplay.GetChild(0).GetChild(7).GetChild(i).GetComponent<PushButton>().SetEnabled(false);

            if (newDisplay == dataSave.GetComponent<UIData>().selfInfo.Item3 - 1)
            {
                playersDisplay.GetChild(0).GetChild(6).GetChild(0).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[newDisplay].Cards[CardType.KNIGHT].ToString();
                playersDisplay.GetChild(0).GetChild(6).GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[newDisplay].Cards[CardType.ROAD_BUILDING].ToString();
                playersDisplay.GetChild(0).GetChild(6).GetChild(0).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[newDisplay].Cards[CardType.RESSOURCE_PAIR].ToString();
                playersDisplay.GetChild(0).GetChild(6).GetChild(0).GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[newDisplay].Cards[CardType.RESSOURCE_MONOPOLY].ToString();
                playersDisplay.GetChild(0).GetChild(6).GetChild(0).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[newDisplay].Cards[CardType.VICTORY_POINT].ToString();

                if (newDisplay == currentGame.CurrentPlayer)
                {
                    if (currentGame.Players[newDisplay].Cards[CardType.KNIGHT] > 0 && !devCardUsedThisTurn)
                        playersDisplay.GetChild(0).GetChild(7).GetChild(0).GetComponent<PushButton>().SetEnabled(true);
                    if (currentGame.Players[newDisplay].Cards[CardType.ROAD_BUILDING] > 0 && !devCardUsedThisTurn)
                        playersDisplay.GetChild(0).GetChild(7).GetChild(1).GetComponent<PushButton>().SetEnabled(true);
                    if (currentGame.Players[newDisplay].Cards[CardType.RESSOURCE_PAIR] > 0 && !devCardUsedThisTurn)
                        playersDisplay.GetChild(0).GetChild(7).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                    if (currentGame.Players[newDisplay].Cards[CardType.RESSOURCE_MONOPOLY] > 0 && !devCardUsedThisTurn)
                        playersDisplay.GetChild(0).GetChild(7).GetChild(3).GetComponent<PushButton>().SetEnabled(true);
                }
            }
        }
        else
        {
            playersDisplay.GetChild(0).GetChild(6).GetChild(0).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[newDisplay].Cards[CardType.KNIGHT].ToString();
            playersDisplay.GetChild(0).GetChild(6).GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[newDisplay].Cards[CardType.ROAD_BUILDING].ToString();
            playersDisplay.GetChild(0).GetChild(6).GetChild(0).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[newDisplay].Cards[CardType.RESSOURCE_PAIR].ToString();
            playersDisplay.GetChild(0).GetChild(6).GetChild(0).GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[newDisplay].Cards[CardType.RESSOURCE_MONOPOLY].ToString();
            playersDisplay.GetChild(0).GetChild(6).GetChild(0).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[newDisplay].Cards[CardType.VICTORY_POINT].ToString();
            for (int i = 0; i < 4; i++)
                playersDisplay.GetChild(0).GetChild(7).GetChild(i).GetComponent<PushButton>().SetEnabled(false);


            if (newDisplay == currentGame.CurrentPlayer)
            {
                if (currentGame.Players[newDisplay].Cards[CardType.KNIGHT] > 0 && !devCardUsedThisTurn)
                    playersDisplay.GetChild(0).GetChild(7).GetChild(0).GetComponent<PushButton>().SetEnabled(true);
                if (currentGame.Players[newDisplay].Cards[CardType.ROAD_BUILDING] > 0 && !devCardUsedThisTurn)
                    playersDisplay.GetChild(0).GetChild(7).GetChild(1).GetComponent<PushButton>().SetEnabled(true);
                if (currentGame.Players[newDisplay].Cards[CardType.RESSOURCE_PAIR] > 0 && !devCardUsedThisTurn)
                    playersDisplay.GetChild(0).GetChild(7).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                if (currentGame.Players[newDisplay].Cards[CardType.RESSOURCE_MONOPOLY] > 0 && !devCardUsedThisTurn)
                    playersDisplay.GetChild(0).GetChild(7).GetChild(3).GetComponent<PushButton>().SetEnabled(true);
            }
        }

        if (playerDisplayDisabled)
            TogglePlayerDisplay();
        else if (newDisplay == currentDisplayedPlayer)
            TogglePlayerDisplay();

        currentDisplayedPlayer = newDisplay;
    }

    public void TogglePlayerDisplay()
    {
        if (playerDisplayDisabled)
        {
            playersDisplay.GetChild(0).GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
            playerDisplayDisabled = false;
        }
        else
        {
            playersDisplay.GetChild(0).GetComponent<Transform>().localScale = new Vector3(0, 0, 0);
            playerDisplayDisabled = true;
        }
    }

    public void DisablePlayerDisplay()
    {
        playersDisplay.GetChild(0).GetComponent<Transform>().localScale = new Vector3(0, 0, 0);
        playerDisplayDisabled = true;
    }

    public void DisplayBoard()
    {
        gameGrid = GameView.Instance.Game.GameGrid;
        int i, j;
        Dictionary<int, Sprite> numberToSprite = new Dictionary<int, Sprite>();
        numberToSprite.Add(2, token2); numberToSprite.Add(3, token3); numberToSprite.Add(4, token4);
        numberToSprite.Add(5, token5); numberToSprite.Add(6, token6); numberToSprite.Add(8, token8);
        numberToSprite.Add(9, token9); numberToSprite.Add(10, token10); numberToSprite.Add(11, token11);
        numberToSprite.Add(12, token12); numberToSprite.Add(0, nullSprite);

        Dictionary<TerrainType, Sprite> terrainTypeToSprite = new Dictionary<TerrainType, Sprite>();
        terrainTypeToSprite.Add(TerrainType.NONE, null); terrainTypeToSprite.Add(TerrainType.SEA, null);
        terrainTypeToSprite.Add(TerrainType.HILLS, carriere); terrainTypeToSprite.Add(TerrainType.FOREST, foret);
        terrainTypeToSprite.Add(TerrainType.FIELDS, champ); terrainTypeToSprite.Add(TerrainType.PASTURE, prairie);
        terrainTypeToSprite.Add(TerrainType.MOUNTAINS, montagne); terrainTypeToSprite.Add(TerrainType.DESERT, desert);

        for (i = 0; i < gridSize; i++)
        {
            if (i <= gridSize / 2)
            {
                for (j = 0; j < gridDisplay.GetChild(0).GetChild(i).childCount; j++)
                {
                    Image tmpCmp1 = gridDisplay.GetChild(0).GetChild(i).GetChild(j).GetComponent<Image>();
                    Image tmpCmp2 = gridDisplay.GetChild(1).GetChild(i).GetChild(j).GetComponent<Image>();
                    //Change le sprite du tile selon son type
                    tmpCmp1.sprite = terrainTypeToSprite[gameGrid.TerrainTiles[new Coordinate(j - i, gridSize / 2 - j, i - gridSize / 2, Direction.NONE)].Type];

                    //Change le sprite du nombre selon son nombre
                    tmpCmp2.sprite = numberToSprite[gameGrid.TerrainTiles[new Coordinate(j - i, gridSize / 2 - j, i - gridSize / 2, Direction.NONE)].DiceProductionNumber];
                }
            }
            else
            {
                for (j = 0; j < gridDisplay.GetChild(0).GetChild(i).childCount; j++)
                {
                    Image tmpCmp1 = gridDisplay.GetChild(0).GetChild(i).GetChild(j).GetComponent<Image>();
                    Image tmpCmp2 = gridDisplay.GetChild(1).GetChild(i).GetChild(j).GetComponent<Image>();
                    //Change le sprite du tile selon son type
                    tmpCmp1.sprite = terrainTypeToSprite[gameGrid.TerrainTiles[new Coordinate(j - gridSize / 2, (gridSize - 1) - i - j, i - gridSize / 2, Direction.NONE)].Type];

                    //Change le sprite du nombre selon son nombre
                    tmpCmp2.sprite = numberToSprite[gameGrid.TerrainTiles[new Coordinate(j - gridSize / 2, (gridSize - 1) - i - j, i - gridSize / 2, Direction.NONE)].DiceProductionNumber];
                }
            }
        }
    }

    public void BuildMode(int constructionType)
    {
        DisablePlayerDisplay();
        List<Coordinate> possibleSpot = new List<Coordinate>();
        int i;
        if (constructionType == -1)
        {
            cancelBuildMode.GetComponent<PushButton>().MouseExit();
            cancelBuildMode.GetComponent<PushButton>().SetEnabled(false);
            ToggleColony(false);
            ToggleCity(false);
            ToggleRoads(false);
        }
        else if (constructionType == 0)
        {
            cancelBuildMode.GetComponent<PushButton>().SetEnabled(true);
            ToggleColony(false);
            ToggleCity(false);
            ToggleRoads(false);
            possibleSpot = currentGame.GameGrid.PossibleColonies(currentGame.Players[currentGame.CurrentPlayer]);
            for (i = 0; i < possibleSpot.Count; i++)
            {
                foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                {
                    if (go.GetComponent<ConstructionButton>() != null && go.GetComponent<ConstructionButton>().constructionType == ConstructionType.SETTLEMENT)
                        if (go.GetComponent<ConstructionButton>().buttonCoordinate.Equals(possibleSpot[i]))
                            go.GetComponent<ConstructionButton>().ToggleButton(true);
                }
            }
        }
        else if (constructionType == 1)
        {
            cancelBuildMode.GetComponent<PushButton>().SetEnabled(true);
            ToggleColony(false);
            ToggleCity(false);
            ToggleRoads(false);
            possibleSpot = currentGame.GameGrid.PossibleCities(currentGame.Players[currentGame.CurrentPlayer]);
            for (i = 0; i < possibleSpot.Count; i++)
            {
                foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                {
                    if (go.GetComponent<ConstructionButton>() != null && go.GetComponent<ConstructionButton>().constructionType == ConstructionType.CITY)
                        if (go.GetComponent<ConstructionButton>().buttonCoordinate.Equals(possibleSpot[i]))
                            go.GetComponent<ConstructionButton>().ToggleButton(true);
                }
            }
        }
        else if (constructionType == 2)
        {
            cancelBuildMode.GetComponent<PushButton>().SetEnabled(true);
            ToggleColony(false);
            ToggleCity(false);
            ToggleRoads(false);
            possibleSpot = currentGame.GameGrid.PossibleRoads(currentGame.Players[currentGame.CurrentPlayer]);
            for (i = 0; i < possibleSpot.Count; i++)
            {
                foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                {
                    if (go.GetComponent<ConstructionButton>() != null && go.GetComponent<ConstructionButton>().constructionType == ConstructionType.ROAD)
                        if (go.GetComponent<ConstructionButton>().buttonCoordinate.Equals(possibleSpot[i]))
                            go.GetComponent<ConstructionButton>().ToggleButton(true);
                }
            }
        }
    }

    public void Construction(Coordinate coordinate, ConstructionType constructionType)
    {
        currentGame = GameView.Instance.Game;
        if (isUsingDevCard)
        {
            firstRoad = coordinate;
            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (go.GetComponent<ConstructionButton>() != null && go.GetComponent<ConstructionButton>().buttonCoordinate.Equals(coordinate))
                {
                    go.GetComponent<ConstructionButton>().ToggleButton(false);
                }
            }
            isUsingDevCard = false;
            isUningDevCard2 = true;
        }
        else if (isUningDevCard2)
        {
            ToggleRoads(false);
            isUningDevCard2 = false;
            RoadConstructionCardUseEventArgs ev = new RoadConstructionCardUseEventArgs(currentGame.Id, firstRoad, coordinate, isOnline);
            if (isOnline)
            {
                ev.GameId = Network.View.NetworkView.Instance.getIdGame();
                Network.View.NetworkView.Instance.OnRoadConstructionCardUse(ev);
            }
            else GameView.Instance.OnRoadConstructionCardUse(ev);

            UpdateConstructionMap();
        }
        else
        {
            if (currentGame.CurrentPhase == GamePhase.INITIAL_BUILDING_1 || currentGame.CurrentPhase == GamePhase.INITIAL_BUILDING_2)
            {
                if (constructionType == ConstructionType.SETTLEMENT)
                {

                    InitialConstructEventArgs e = new InitialConstructEventArgs(currentGame.Id ,coordinate, isOnline);

                    if (isOnline)
                    {
                        e.GameId = Network.View.NetworkView.Instance.getIdGame();
                        Network.View.NetworkView.Instance.OnInitialColony(e);
                    }
                    else GameView.Instance.OnInitialColony(e);
                    UpdateConstructionMap();
                    ToggleColony(false);
                    ToggleRoads(false);

                    if (!isOnline)
                    {
                        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                        {
                            List<Coordinate> possibleSpot = currentGame.GameGrid.PossibleRoads(currentGame.Players[currentGame.CurrentPlayer]);
                            for (int i = 0; i < possibleSpot.Count; i++)
                            {
                                if (go.GetComponent<ConstructionButton>() != null && go.GetComponent<ConstructionButton>().constructionType == ConstructionType.ROAD)
                                {

                                    if (go.GetComponent<ConstructionButton>().buttonCoordinate.Equals(possibleSpot[i]))
                                        go.GetComponent<ConstructionButton>().ToggleButton(true);
                                }
                            }
                        }
                    }
                }
                else
                {
                    InitialConstructEventArgs e = new InitialConstructEventArgs(currentGame.Id, coordinate, isOnline);
                    if (isOnline)
                    {
                        e.GameId = Network.View.NetworkView.Instance.getIdGame();
                        Network.View.NetworkView.Instance.OnInitialRoad(e);
                    }
                    else GameView.Instance.OnInitialRoad(e);
                    UpdateConstructionMap();
                    if (currentGame.CurrentPhase != GamePhase.INITIAL_BUILDING_1 && currentGame.CurrentPhase != GamePhase.INITIAL_BUILDING_2)
                    {
                        ToggleColony(false);
                        ToggleCity(false);
                        ToggleRoads(false);
                    }
                    else
                    {
                        ToggleColony(false);
                        ToggleCity(false);
                        ToggleRoads(false);

                        bool doTheThing = false;

                        /*
                        if (isOnline)
                        {
                            if (dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item3 == false && (dataSave.GetComponent<UIData>().selfInfo.Item3 - 1) == currentGame.CurrentPlayer)
                            {
                                doTheThing = true;
                            }
                        }


                        else */ if (!isOnline && dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item3 == false)
                        {
                            doTheThing = true;
                        }

                        if (doTheThing)
                        {
                            ToggleColony(true);
                            ToggleRoads(false);

                            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                            {
                                if (go.GetComponent<ConstructionButton>() != null && go.GetComponent<ConstructionButton>().constructionType == ConstructionType.SETTLEMENT)
                                {
                                    if (currentGame.GameGrid.Intersections[go.GetComponent<ConstructionButton>().buttonCoordinate].Building != ConstructionType.NONE)
                                    {
                                        //toggle l'intersection
                                        go.GetComponent<ConstructionButton>().ToggleButton(false);
                                        //toggle les intersections voisines
                                        Coordinate initialCoordinates = go.GetComponent<ConstructionButton>().buttonCoordinate;
                                        foreach (GameObject go2 in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                                        {
                                            if (initialCoordinates.D == Direction.UP)
                                            {
                                                if (go2.GetComponent<ConstructionButton>() != null && go2.GetComponent<ConstructionButton>().constructionType == ConstructionType.SETTLEMENT)
                                                {
                                                    if (go2.GetComponent<ConstructionButton>().buttonCoordinate.Equals(new Coordinate(initialCoordinates.X, initialCoordinates.Y + 1, initialCoordinates.Z - 1, Direction.DOWN))
                                                        || go2.GetComponent<ConstructionButton>().buttonCoordinate.Equals(new Coordinate(initialCoordinates.X + 1, initialCoordinates.Y, initialCoordinates.Z - 1, Direction.DOWN))
                                                        || go2.GetComponent<ConstructionButton>().buttonCoordinate.Equals(new Coordinate(initialCoordinates.X + 1, initialCoordinates.Y + 1, initialCoordinates.Z - 2, Direction.DOWN)))
                                                        go2.GetComponent<ConstructionButton>().ToggleButton(false);
                                                }
                                            }
                                            else
                                            {
                                                if (go2.GetComponent<ConstructionButton>() != null && go2.GetComponent<ConstructionButton>().constructionType == ConstructionType.SETTLEMENT)
                                                {
                                                    if (go2.GetComponent<ConstructionButton>().buttonCoordinate.Equals(new Coordinate(initialCoordinates.X - 1, initialCoordinates.Y, initialCoordinates.Z + 1, Direction.UP))
                                                       || go2.GetComponent<ConstructionButton>().buttonCoordinate.Equals(new Coordinate(initialCoordinates.X, initialCoordinates.Y - 1, initialCoordinates.Z + 1, Direction.UP))
                                                       || go2.GetComponent<ConstructionButton>().buttonCoordinate.Equals(new Coordinate(initialCoordinates.X - 1, initialCoordinates.Y - 1, initialCoordinates.Z + 2, Direction.UP)))
                                                        go2.GetComponent<ConstructionButton>().ToggleButton(false);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                ConstructEventArgs e = new ConstructEventArgs(currentGame.Id, constructionType, coordinate, isOnline);
                if (isOnline)
                {
                    e.GameId = Network.View.NetworkView.Instance.getIdGame();
                    Network.View.NetworkView.Instance.OnConstruct(e);
                }
                else GameView.Instance.OnConstruct(e);
                ToggleColony(false);
                ToggleCity(false);
                ToggleRoads(false);
                UpdateConstructionMap();
                cancelBuildMode.GetComponent<PushButton>().SetEnabled(false);

                if (!currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.SETTLEMENT))
                    buildColony.GetComponent<PushButton>().SetEnabled(false);
                if (!currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.CITY))
                    buildCity.GetComponent<PushButton>().SetEnabled(false);
                if (!currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.ROAD))
                    buildRoad.GetComponent<PushButton>().SetEnabled(false);
                if (!currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.DEVELOPMENT_CARD))
                    buyDevCard.GetComponent<PushButton>().SetEnabled(false);
            }
        }

        DisablePlayerDisplay();
        ShowCurrentPlayerColor();
    }

    public void ToggleColony(bool toggle)
    {
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.GetComponent<ConstructionButton>() != null && go.GetComponent<ConstructionButton>().constructionType == ConstructionType.SETTLEMENT)
            {
                go.GetComponent<Image>().enabled = toggle;
                go.GetComponent<Button>().enabled = toggle;
                if (toggle)
                    go.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                else
                    go.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            }
        }
    }
    public void ToggleCity(bool toggle)
    {
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.GetComponent<ConstructionButton>() != null && go.GetComponent<ConstructionButton>().constructionType == ConstructionType.CITY)
            {
                go.GetComponent<Image>().enabled = toggle;
                go.GetComponent<Button>().enabled = toggle;
                if (toggle)
                    go.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                else
                    go.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            }
        }
    }
    public void ToggleRoads(bool toggle)
    {
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.GetComponent<ConstructionButton>() != null && go.GetComponent<ConstructionButton>().constructionType == ConstructionType.ROAD)
            {
                go.GetComponent<Image>().enabled = toggle;
                go.GetComponent<Button>().enabled = toggle;
                if (toggle)
                    go.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                else
                    go.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            }
        }
    }

    public void ToggleBandits(bool toggle)
    {
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.GetComponent<BanditButton>() != null)
            {
                go.GetComponent<Image>().enabled = toggle;
                go.GetComponent<Button>().enabled = toggle;
                if (toggle)
                    go.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                else
                    go.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            }
        }
    }

    public void InitiatePlayerChooseMenu()
    {
        for (int i = 0; i < nbJoueursTotal; i++)
        {
            choosePlayer.GetChild(i + 1).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[i].Item1;
            choosePlayer.GetChild(i + 1).GetChild(1).GetComponent<Label>().traductions[0] = dataSave.GetComponent<UIData>().players[i].Item1;
            choosePlayer.GetChild(i + 1).GetChild(1).GetComponent<Label>().traductions[1] = dataSave.GetComponent<UIData>().players[i].Item1;

            if (dataSave.GetComponent<UIData>().players[i].Item3 == false)
            {
                if (dataSave.GetComponent<UIData>().players[i].Item2 == 0)
                {
                    choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<Image>().sprite = defaultAvatar0;
                    choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<PushButton>().parDefaut = defaultAvatar0;
                    choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<PushButton>().onHover = defaultAvatar0;
                    choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<PushButton>().onClick = defaultAvatar0;
                }
                else if (dataSave.GetComponent<UIData>().players[i].Item2 == 1)
                {
                    choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<Image>().sprite = defaultAvatar1;
                    choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<PushButton>().parDefaut = defaultAvatar1;
                    choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<PushButton>().onHover = defaultAvatar1;
                    choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<PushButton>().onClick = defaultAvatar1;
                }
                else if (dataSave.GetComponent<UIData>().players[i].Item2 == 2)
                {
                    choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<Image>().sprite = defaultAvatar2;
                    choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<PushButton>().parDefaut = defaultAvatar2;
                    choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<PushButton>().onHover = defaultAvatar2;
                    choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<PushButton>().onClick = defaultAvatar2;
                }
                else
                {
                    choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<Image>().sprite = defaultAvatar3;
                    choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<PushButton>().parDefaut = defaultAvatar3;
                    choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<PushButton>().onHover = defaultAvatar3;
                    choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<PushButton>().onClick = defaultAvatar3;
                }
            }
            else
            {
                choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<Image>().sprite = robotAvatar;
                choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<PushButton>().parDefaut = robotAvatar;
                choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<PushButton>().onHover = robotAvatar;
                choosePlayer.GetChild(i + 1).GetChild(0).GetComponent<PushButton>().onClick = robotAvatar;
            }
        }
    }

    public void UpdateConstructionMap()
    {
        currentGame = GameView.Instance.Game;
        ConstructionType ctype;
        int player;
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.GetComponent<ConstructionDisplay>() != null)
            {
                if (go.GetComponent<ConstructionDisplay>().constructionType != ConstructionType.NONE)
                {
                    if (go.GetComponent<ConstructionDisplay>().constructionType != ConstructionType.ROAD)
                    {
                        ctype = currentGame.GameGrid.Intersections[go.GetComponent<ConstructionDisplay>().displayCoordinate].Building;
                        player = currentGame.GameGrid.Intersections[go.GetComponent<ConstructionDisplay>().displayCoordinate].PlayerId;
                    }
                    else
                    {
                        ctype = currentGame.GameGrid.Edges[go.GetComponent<ConstructionDisplay>().displayCoordinate].Building;
                        player = currentGame.GameGrid.Edges[go.GetComponent<ConstructionDisplay>().displayCoordinate].PlayerId;
                    }
                    if (ctype == go.GetComponent<ConstructionDisplay>().constructionType)
                        go.GetComponent<ConstructionDisplay>().ChangeSprite(ctype, player);
                    else
                        go.GetComponent<ConstructionDisplay>().ChangeSprite(ConstructionType.NONE, -1);
                }
                else
                {
                    if (currentGame.GameGrid.CurrentThiefLocation.Equals(go.GetComponent<ConstructionDisplay>().displayCoordinate))
                        go.GetComponent<ConstructionDisplay>().ToggleBandit(true);
                    else
                    {
                        go.GetComponent<ConstructionDisplay>().ToggleBandit(false);
                    }
                }
            }

            int theNumber = -1;

            Dictionary<int, Sprite> numberToSprite = new Dictionary<int, Sprite>();
            numberToSprite.Add(2, token2); numberToSprite.Add(3, token3); numberToSprite.Add(4, token4);
            numberToSprite.Add(5, token5); numberToSprite.Add(6, token6); numberToSprite.Add(8, token8);
            numberToSprite.Add(9, token9); numberToSprite.Add(10, token10); numberToSprite.Add(11, token11);
            numberToSprite.Add(12, token12); numberToSprite.Add(0, baseBandit);

            if (go.GetComponent<BanditButton>() != null)
            {
                theNumber = gameGrid.TerrainTiles[go.GetComponent<BanditButton>().buttonCoordinate].DiceProductionNumber;
                go.GetComponent<Image>().sprite = numberToSprite[theNumber];
            }
        }
        DisablePlayerDisplay();
    }

    public void BuyDevCard()
    {
        ConstructEventArgs e = new ConstructEventArgs(currentGame.Id, ConstructionType.DEVELOPMENT_CARD, null, isOnline);
        if (isOnline)
        {
            e.GameId = Network.View.NetworkView.Instance.getIdGame();
            Network.View.NetworkView.Instance.OnConstruct(e);
        }
        else GameView.Instance.OnConstruct(e);

        DisablePlayerDisplay();

        if (!currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.SETTLEMENT))
            buildColony.GetComponent<PushButton>().SetEnabled(false);
        if (!currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.CITY))
            buildCity.GetComponent<PushButton>().SetEnabled(false);
        if (!currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.ROAD))
            buildRoad.GetComponent<PushButton>().SetEnabled(false);
        if (!currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.DEVELOPMENT_CARD))
        {
            buyDevCard.GetComponent<PushButton>().MouseExit();
            buyDevCard.GetComponent<PushButton>().SetEnabled(false);
        }
    }

    public void UseDevCard(int cardType)
    {
        Debug.Log("Waiting For Bandit: " + isWaitingForBandit);
        if (!isWaitingForBandit)
        {
            bool doTheThing = false;

            if (isOnline)
            {
                if (dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item3 == false && (dataSave.GetComponent<UIData>().selfInfo.Item3 - 1) == currentGame.CurrentPlayer)
                {
                    doTheThing = true;
                }
            }
            else if (!dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item3)
            {
                doTheThing = true;
            }

            if (doTheThing)
            {
                devCardUsedThisTurn = true;
                isUsingDevCard = true;
                DisablePlayerDisplay();
                if ((CardType)cardType == CardType.KNIGHT)
                {
                    ToggleColony(false);
                    ToggleCity(false);
                    ToggleRoads(false);
                    ToggleBandits(true);
                    foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                    {
                        if (go.GetComponent<BanditButton>() != null && go.GetComponent<BanditButton>().buttonCoordinate.Equals(currentGame.GameGrid.CurrentThiefLocation))
                        {
                            go.GetComponent<Image>().enabled = false;
                            go.GetComponent<Button>().enabled = false;
                            go.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
                        }
                    }
                }
                else if ((CardType)cardType == CardType.RESSOURCE_MONOPOLY)
                {
                    monopolyMenu.GetChild(8).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.BRICK].ToString();
                    monopolyMenu.GetChild(8).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.LUMBER].ToString();
                    monopolyMenu.GetChild(8).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.WHEAT].ToString();
                    monopolyMenu.GetChild(8).GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.WOOL].ToString();
                    monopolyMenu.GetChild(8).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.ORE].ToString();
                    monopolyMenu.gameObject.SetActive(true);
                }
                else if ((CardType)cardType == CardType.RESSOURCE_PAIR)
                {
                    pairMenu.gameObject.SetActive(true);
                }
                else if ((CardType)cardType == CardType.ROAD_BUILDING)
                {
                    BuildMode(2);
                }
                else if ((CardType)cardType == CardType.VICTORY_POINT)
                    isUsingDevCard = false;
            }
        }
    }

    public void TriggerMonopoly(int ressource)
    {
        MonopolyCardUseEventArgs e = new MonopolyCardUseEventArgs(currentGame.Id, (RessourceType)ressource, isOnline);
        if (isOnline)
        {
            e.GameId = Network.View.NetworkView.Instance.getIdGame();
            Network.View.NetworkView.Instance.OnMonopolyCardUse(e);
        }
        else GameView.Instance.OnMonopolyCardUse(e);

        //monopolyMenu.GetChild(ressource + 2).GetComponent<PushButton>().MouseExit();
        monopolyMenu.gameObject.SetActive(false);

        isUsingDevCard = false;
        UpdateConstructionMap();
        if (!dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item3)
        {
            if (!currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.SETTLEMENT))
                buildColony.GetComponent<PushButton>().SetEnabled(false);
            else
                buildColony.GetComponent<PushButton>().SetEnabled(true);
            if (!currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.CITY))
                buildCity.GetComponent<PushButton>().SetEnabled(false);
            else
                buildCity.GetComponent<PushButton>().SetEnabled(true);
            if (!currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.ROAD))
                buildRoad.GetComponent<PushButton>().SetEnabled(false);
            else
                buildRoad.GetComponent<PushButton>().SetEnabled(true);
            if (!currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.DEVELOPMENT_CARD))
                buyDevCard.GetComponent<PushButton>().SetEnabled(false);
            else
                buyDevCard.GetComponent<PushButton>().SetEnabled(true);
        }
    }

    public void TriggerPair()
    {
        int firstRessource = 0;
        int secondRessource = 0;

        for (int i = 0; i < 5; i++)
        {
            if (pairMenu.GetChild(3).GetChild(i).GetComponent<RadioButton>().isChecked)
                firstRessource = i + 1;
            if (pairMenu.GetChild(4).GetChild(i).GetComponent<RadioButton>().isChecked)
                secondRessource = i + 1;
        }
        ResourcePairCardUseEventArgs e = new ResourcePairCardUseEventArgs(currentGame.Id, (RessourceType)firstRessource, (RessourceType)secondRessource, isOnline);
        if (isOnline)
        {
            e.GameId = Network.View.NetworkView.Instance.getIdGame();
            Network.View.NetworkView.Instance.OnResourcePairCardUse(e);
        }
        else GameView.Instance.OnResourcePairCardUse(e);

        pairMenu.GetChild(7).GetComponent<PushButton>().MouseExit();
        pairMenu.gameObject.SetActive(false);

        isUsingDevCard = false;
        UpdateConstructionMap();

        if (!dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item3)
        {
            if (!currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.SETTLEMENT))
                buildColony.GetComponent<PushButton>().SetEnabled(false);
            else
                buildColony.GetComponent<PushButton>().SetEnabled(true);
            if (!currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.CITY))
                buildCity.GetComponent<PushButton>().SetEnabled(false);
            else
                buildCity.GetComponent<PushButton>().SetEnabled(true);
            if (!currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.ROAD))
                buildRoad.GetComponent<PushButton>().SetEnabled(false);
            else
                buildRoad.GetComponent<PushButton>().SetEnabled(true);
            if (!currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.DEVELOPMENT_CARD))
                buyDevCard.GetComponent<PushButton>().SetEnabled(false);
            else
                buyDevCard.GetComponent<PushButton>().SetEnabled(true);
        }
    }

    public void EndTurn()
    {
        Debug.Log("C'est ça: " + currentGame.CurrentPhase);
        if (currentGame.CurrentPhase == GamePhase.CONSTRUCTION)
        {
            ToggleColony(false);
            ToggleCity(false);
            ToggleRoads(false);
            buildColony.GetComponent<PushButton>().SetEnabled(false);
            buildCity.GetComponent<PushButton>().SetEnabled(false);
            buildRoad.GetComponent<PushButton>().SetEnabled(false);
            buyDevCard.GetComponent<PushButton>().SetEnabled(false);
            endTurn.GetComponent<PushButton>().MouseExit();
            endTurn.GetComponent<PushButton>().SetEnabled(false);
            BaseEventArgs e = new BaseEventArgs(currentGame.Id, isOnline);
            if (isOnline)
            {
                BaseEventArgs evBis = new BaseEventArgs(Network.View.NetworkView.Instance.getIdGame(), true, "OnEndPhase");
                Network.View.NetworkView.Instance.OnEndPhase(evBis);
            }
            else GameView.Instance.OnEndPhase(e);
            ShowCurrentPlayerColor();
            devCardUsedThisTurn = false;
        }
    }

    public void HarvestPhaseBeginHandler(object sender, GameStatusArgs e)
    {
        currentGame = GameView.Instance.Game;
        ToggleColony(false);
        ToggleCity(false);
        ToggleRoads(false);
        ToggleBandits(false);
        endTurn.GetComponent<PushButton>().SetEnabled(false);

        ShowCurrentPlayerColor();

        string text = "";
        if (langue == 0)
            text = "Début du tour de: " + dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item1;
        else if (langue == 1)
            text = "Turn of: " + dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item1;

        SendMessageToChat(text, Message.MessageType.generalMessage);

        bool doTheThing = false;

        if (isOnline)
        {
            if (dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item3 == false && (dataSave.GetComponent<UIData>().selfInfo.Item3 - 1) == currentGame.CurrentPlayer)
            {
                doTheThing = true;
            }
        }

        else if (!dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item3)
        {
            doTheThing = true;
        }

        if (doTheThing)
        {
            if (langue == 0)
                PopTitle("Tour de:" + dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item1);
            else if (langue == 1)
                PopTitle("Turn of:" + dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item1);

            BaseEventArgs ev = new BaseEventArgs(currentGame.Id, isOnline);
            if (isOnline)
            {
                BaseEventArgs evBis = new BaseEventArgs(Network.View.NetworkView.Instance.getIdGame(), true, "OnDiceThrow");
                Network.View.NetworkView.Instance.OnDiceThrow(evBis);
            }
            else GameView.Instance.OnDiceThrow(ev);
            diceDisplay.gameObject.SetActive(true);
            rollDieButton.gameObject.SetActive(true);
            //Debug.Log("Dices:" + GameView.Instance.Game.lastDice);

        }

        UpdateConstructionMap();
        DisablePlayerDisplay();
        if (isOnline && (dataSave.GetComponent<UIData>().selfInfo.Item3 - 1) == currentGame.CurrentPlayer)
        endTurn.GetComponent<PushButton>().SetEnabled(true);
    }

    public void RollDices()
    {
        die1.GetComponent<Dice>().finalSide = GameView.Instance.Game.lastDice.Item1;
        die2.GetComponent<Dice>().finalSide = GameView.Instance.Game.lastDice.Item2;
        rollDieButton.gameObject.SetActive(false);
        endDieButton.gameObject.SetActive(true);
        die1.GetComponent<Dice>().Roll();
        die2.GetComponent<Dice>().Roll();
    }

    public void EndDiceDisplay()
    {
        rollDieButton.gameObject.SetActive(true);
        endDieButton.gameObject.SetActive(false);
        diceDisplay.gameObject.SetActive(false);

        if (!isOnline)
        {
            if (langue == 0)
                SendMessageToChat("Résultat des dés: " + (GameView.Instance.Game.lastDice.Item1 + GameView.Instance.Game.lastDice.Item2).ToString(), Message.MessageType.diceInfo);
            else if (langue == 1)
                SendMessageToChat("Dice throw result: " + (GameView.Instance.Game.lastDice.Item1 + GameView.Instance.Game.lastDice.Item2).ToString(), Message.MessageType.diceInfo);
        }
        //else
        //{
            //Network.View.NetworkView.Instance.OnEndPhase(new BaseEventArgs(Network.View.NetworkView.Instance.getIdGame(), true, "OnEndPhase"));
        //}
    }

    public void ExchangePhaseBeginHandler(object sender, GameStatusArgs e)
    {
        ToggleColony(false);
        ToggleCity(false);
        ToggleRoads(false);

        if (isOnline)
        {
            if (dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item3 == false && (dataSave.GetComponent<UIData>().selfInfo.Item3 - 1) == currentGame.CurrentPlayer)
            {
                exchangeMenu.gameObject.SetActive(true);
            }
            else if(!dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item3)
            {
                exchangeMenu.gameObject.SetActive(true);
                exchangeMenu.GetChild(8).gameObject.SetActive(true);
                SetOnlineExchangeMenu();
            }
        }
        else if (!dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item3)
        {
            exchangeMenu.gameObject.SetActive(true);
        }
        else
        {
            if (langue == 0)
                SendMessageToChat("Résultat des dés: " + (GameView.Instance.Game.lastDice.Item1+GameView.Instance.Game.lastDice.Item2).ToString(), Message.MessageType.diceInfo);
            else if (langue == 1)
                SendMessageToChat("Dice throw result: " + (GameView.Instance.Game.lastDice.Item1 + GameView.Instance.Game.lastDice.Item2).ToString(), Message.MessageType.diceInfo);
        }

        /*
        fxSource.clip = exchangeMenuClip;
        fxSource.loop = false;
        fxSource.Play();
        */
        UpdateConstructionMap();
    }

    public void SetPlayerExchangeValues()
    {
        bool passedCurrentPlayer = false;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 5; j++)
                offeredRessources[i][j] = 0;
        }
        if (nbJoueursTotal < 4)
        {
            exchangeMenu.GetChild(5).GetChild(4).gameObject.SetActive(false);
        }

        for (int i = 0; i < nbJoueursTotal; i++)
        {
            if (isOnline)
            {
                /*
                int index = 0;

                if (currentGame.CurrentPlayer == (dataSave.GetComponent<UIData>().selfInfo.Item3 - 1))
                    index = 7;
                else
                    index = 8;

                if (!dataSave.GetComponent<UIData>().players[i].Item3)
                {
                    if (currentGame.Players[i].Equals(currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1]))
                    {
                        passedCurrentPlayer = true;

                        for (int j = 0; j < 5; j++)
                            exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(j).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "0";

                        exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(2).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(3).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(4).GetChild(0).GetComponent<PushButton>().SetEnabled(false);

                        if (currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[RessourceType.BRICK] > 0) exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(0).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(0).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                        if (currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[RessourceType.LUMBER] > 0) exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(1).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(1).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                        if (currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[RessourceType.WHEAT] > 0) exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(2).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(2).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                        if (currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[RessourceType.WOOL] > 0) exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(3).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(3).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                        if (currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[RessourceType.ORE] > 0) exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(4).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(4).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                    }  
                }
                */
            }
            else
            {
                if (!dataSave.GetComponent<UIData>().players[i].Item3)
                {
                    if (currentGame.Players[i].Equals(currentGame.Players[currentGame.CurrentPlayer]))
                    {
                        passedCurrentPlayer = true;

                        for (int j = 0; j < 5; j++)
                            exchangeMenu.GetChild(5).GetChild(1).GetChild(1).GetChild(j).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "0";

                        exchangeMenu.GetChild(5).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(5).GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(5).GetChild(1).GetChild(1).GetChild(2).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(5).GetChild(1).GetChild(1).GetChild(3).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(5).GetChild(1).GetChild(1).GetChild(4).GetChild(0).GetComponent<PushButton>().SetEnabled(false);

                        if (currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.BRICK] > 0) exchangeMenu.GetChild(5).GetChild(1).GetChild(1).GetChild(0).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(5).GetChild(1).GetChild(1).GetChild(0).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                        if (currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.LUMBER] > 0) exchangeMenu.GetChild(5).GetChild(1).GetChild(1).GetChild(1).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(5).GetChild(1).GetChild(1).GetChild(1).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                        if (currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.WHEAT] > 0) exchangeMenu.GetChild(5).GetChild(1).GetChild(1).GetChild(2).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(5).GetChild(1).GetChild(1).GetChild(2).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                        if (currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.WOOL] > 0) exchangeMenu.GetChild(5).GetChild(1).GetChild(1).GetChild(3).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(5).GetChild(1).GetChild(1).GetChild(3).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                        if (currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.ORE] > 0) exchangeMenu.GetChild(5).GetChild(1).GetChild(1).GetChild(4).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(5).GetChild(1).GetChild(1).GetChild(4).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                    }
                    else
                    {
                        if (passedCurrentPlayer)
                        {
                            if (dataSave.GetComponent<UIData>().players[i].Item3 == false)
                            {
                                if (dataSave.GetComponent<UIData>().players[i].Item2 == 0)
                                    exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(3).GetComponent<Image>().sprite = defaultAvatar0;
                                else if (dataSave.GetComponent<UIData>().players[i].Item2 == 1)
                                    exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(3).GetComponent<Image>().sprite = defaultAvatar1;
                                else if (dataSave.GetComponent<UIData>().players[i].Item2 == 2)
                                    exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(3).GetComponent<Image>().sprite = defaultAvatar2;
                                else
                                    exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(3).GetComponent<Image>().sprite = defaultAvatar3;
                            }
                            else
                                exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(3).GetComponent<Image>().sprite = robotAvatar;

                            exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[i].Item1;

                            for (int j = 0; j < 5; j++)
                                exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(j).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "0";

                            exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(0).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                            exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(1).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                            exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(2).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                            exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(3).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                            exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(4).GetChild(0).GetComponent<PushButton>().SetEnabled(false);

                            if (currentGame.Players[i].Ressources[RessourceType.BRICK] > 0) exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(0).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                            else exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(0).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                            if (currentGame.Players[i].Ressources[RessourceType.LUMBER] > 0) exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(1).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                            else exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(1).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                            if (currentGame.Players[i].Ressources[RessourceType.WHEAT] > 0) exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(2).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                            else exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(2).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                            if (currentGame.Players[i].Ressources[RessourceType.WOOL] > 0) exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(3).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                            else exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(3).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                            if (currentGame.Players[i].Ressources[RessourceType.ORE] > 0) exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(4).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                            else exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(4).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                        }
                        else
                        {
                            if (dataSave.GetComponent<UIData>().players[i].Item3 == false)
                            {
                                if (dataSave.GetComponent<UIData>().players[i].Item2 == 0)
                                    exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(3).GetComponent<Image>().sprite = defaultAvatar0;
                                else if (dataSave.GetComponent<UIData>().players[i].Item2 == 1)
                                    exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(3).GetComponent<Image>().sprite = defaultAvatar1;
                                else if (dataSave.GetComponent<UIData>().players[i].Item2 == 2)
                                    exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(3).GetComponent<Image>().sprite = defaultAvatar2;
                                else
                                    exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(3).GetComponent<Image>().sprite = defaultAvatar3;
                            }
                            else
                                exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(3).GetComponent<Image>().sprite = robotAvatar;

                            exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[i].Item1;

                            for (int j = 0; j < 5; j++)
                                exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(j).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "0";

                            exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(0).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                            exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(1).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                            exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(2).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                            exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(3).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                            exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(4).GetChild(0).GetComponent<PushButton>().SetEnabled(false);

                            if (currentGame.Players[i].Ressources[RessourceType.BRICK] > 0) exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(0).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                            else exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(0).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                            if (currentGame.Players[i].Ressources[RessourceType.LUMBER] > 0) exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(1).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                            else exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(1).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                            if (currentGame.Players[i].Ressources[RessourceType.WHEAT] > 0) exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(2).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                            else exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(2).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                            if (currentGame.Players[i].Ressources[RessourceType.WOOL] > 0) exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(3).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                            else exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(3).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                            if (currentGame.Players[i].Ressources[RessourceType.ORE] > 0) exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(4).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                            else exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(4).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                        }
                    }
                }
                else
                {
                    if (passedCurrentPlayer)
                    {
                        exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(3).GetComponent<Image>().sprite = robotAvatar;

                        exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[i].Item1;

                        for (int j = 0; j < 5; j++)
                            exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(j).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "0";

                        exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(0).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(1).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(2).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(3).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(5).GetChild(i + 1).GetChild(1).GetChild(4).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                    }
                    else
                    {
                        exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(3).GetComponent<Image>().sprite = robotAvatar;

                        exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[i].Item1;

                        for (int j = 0; j < 5; j++)
                            exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(j).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "0";

                        exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(0).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(1).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(2).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(3).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(5).GetChild(i + 2).GetChild(1).GetChild(4).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                    }
                }
            }
        }
    }

    public void SubstractOfferedRessource(int player, int ressource)
    {

        if (isOnline)
        {
            int index = 0;

            if (currentGame.CurrentPlayer == (dataSave.GetComponent<UIData>().selfInfo.Item3 - 1))
                index = 7;
            else
                index = 8;

            if (offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][ressource - 1] > 0)
                offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][ressource - 1]--;
            exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(ressource - 1).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][ressource - 1].ToString();

            if (offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][ressource - 1] > 0)
                exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(ressource - 1).GetChild(0).GetComponent<PushButton>().SetEnabled(true);
            else
            {
                exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(ressource - 1).GetChild(0).GetComponent<PushButton>().MouseExit();
                exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(ressource - 1).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
            }

            if (currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[(RessourceType)ressource] > offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][ressource - 1])
                exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(ressource - 1).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
            else
                exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(ressource - 1).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
        }
        else
        {
            int playerValue;

            if (player == 0)
                playerValue = currentGame.CurrentPlayer;
            else if (player <= currentGame.CurrentPlayer)
                playerValue = player - 1;
            else
                playerValue = player;

            if (offeredRessources[player][ressource - 1] > 0)
                offeredRessources[player][ressource - 1]--;
            exchangeMenu.GetChild(5).GetChild(player + 1).GetChild(1).GetChild(ressource - 1).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = offeredRessources[player][ressource - 1].ToString();

            if (offeredRessources[player][ressource - 1] > 0)
                exchangeMenu.GetChild(5).GetChild(player + 1).GetChild(1).GetChild(ressource - 1).GetChild(0).GetComponent<PushButton>().SetEnabled(true);
            else
            {
                exchangeMenu.GetChild(5).GetChild(player + 1).GetChild(1).GetChild(ressource - 1).GetChild(0).GetComponent<PushButton>().MouseExit();
                exchangeMenu.GetChild(5).GetChild(player + 1).GetChild(1).GetChild(ressource - 1).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
            }

            if (currentGame.Players[playerValue].Ressources[(RessourceType)ressource] > offeredRessources[player][ressource - 1])
                exchangeMenu.GetChild(5).GetChild(player + 1).GetChild(1).GetChild(ressource - 1).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
            else
                exchangeMenu.GetChild(5).GetChild(player + 1).GetChild(1).GetChild(ressource - 1).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
        }

        CancelValidation();
    }

    public void AddOfferedRessource(int player, int ressource)
    {
        if (isOnline)
        {
            int index = 0;

            if (currentGame.CurrentPlayer == (dataSave.GetComponent<UIData>().selfInfo.Item3 - 1))
                index = 7;
            else
                index = 8;

            if (offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][ressource - 1] < currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[(RessourceType)ressource])
                offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][ressource - 1]++;
            exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(ressource - 1).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][ressource - 1].ToString();

            if (offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][ressource - 1] > 0)
                exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(ressource - 1).GetChild(0).GetComponent<PushButton>().SetEnabled(true);
            else
                exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(ressource - 1).GetChild(0).GetComponent<PushButton>().SetEnabled(false);

            if (currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[(RessourceType)ressource] > offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][ressource - 1])
                exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(ressource - 1).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
            else
            {
                exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(ressource - 1).GetChild(2).GetComponent<PushButton>().MouseExit();
                exchangeMenu.GetChild(index).GetChild(1).GetChild(1).GetChild(ressource - 1).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
            }
        }
        else
        {
            int playerValue;

            if (player == 0)
                playerValue = currentGame.CurrentPlayer;
            else if (player <= currentGame.CurrentPlayer)
                playerValue = player - 1;
            else
                playerValue = player;

            if (offeredRessources[player][ressource - 1] < currentGame.Players[playerValue].Ressources[(RessourceType)ressource])
                offeredRessources[player][ressource - 1]++;
            exchangeMenu.GetChild(5).GetChild(player + 1).GetChild(1).GetChild(ressource - 1).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = offeredRessources[player][ressource - 1].ToString();

            if (offeredRessources[player][ressource - 1] > 0)
                exchangeMenu.GetChild(5).GetChild(player + 1).GetChild(1).GetChild(ressource - 1).GetChild(0).GetComponent<PushButton>().SetEnabled(true);
            else
                exchangeMenu.GetChild(5).GetChild(player + 1).GetChild(1).GetChild(ressource - 1).GetChild(0).GetComponent<PushButton>().SetEnabled(false);

            if (currentGame.Players[playerValue].Ressources[(RessourceType)ressource] > offeredRessources[player][ressource - 1])
                exchangeMenu.GetChild(5).GetChild(player + 1).GetChild(1).GetChild(ressource - 1).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
            else
            {
                exchangeMenu.GetChild(5).GetChild(player + 1).GetChild(1).GetChild(ressource - 1).GetChild(2).GetComponent<PushButton>().MouseExit();
                exchangeMenu.GetChild(5).GetChild(player + 1).GetChild(1).GetChild(ressource - 1).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
            }
        }

        CancelValidation();
    }

    public void ValidateExchange(int playerDisplayed)
    {
        if (isOnline)
        {
            if (playerDisplayed == 0)
                playerToExchangeTo = currentGame.CurrentPlayer;
            else if (playerDisplayed <= currentGame.CurrentPlayer)
                playerToExchangeTo = playerDisplayed - 1;
            else
                playerToExchangeTo = playerDisplayed;

            playerToExchangeToInventory = playerDisplayed;

            validateButton0Reseau.GetComponent<PushButton>().SetEnabled(true);

            if (playerDisplayed == 1)
            {
                validateButton1Reseau.GetComponent<PushButton>().MouseExit();
                validateButton1Reseau.GetComponent<PushButton>().SetEnabled(false);
                validateButton2Reseau.gameObject.SetActive(false);
                if (nbJoueursTotal == 4)
                    validateButton3Reseau.gameObject.SetActive(false);
            }
            else if (playerDisplayed == 2)
            {
                validateButton2Reseau.GetComponent<PushButton>().MouseExit();
                validateButton2Reseau.GetComponent<PushButton>().SetEnabled(false);
                validateButton1Reseau.gameObject.SetActive(false);
                if (nbJoueursTotal == 4)
                    validateButton3Reseau.gameObject.SetActive(false);
            }
            else if (playerDisplayed == 3)
            {
                validateButton3Reseau.GetComponent<PushButton>().MouseExit();
                validateButton3Reseau.GetComponent<PushButton>().SetEnabled(false);
                validateButton1Reseau.gameObject.SetActive(false);
                validateButton2Reseau.gameObject.SetActive(false);
            }
        }
        else
        {
            if (playerDisplayed == 0)
                playerToExchangeTo = currentGame.CurrentPlayer;
            else if (playerDisplayed <= currentGame.CurrentPlayer)
                playerToExchangeTo = playerDisplayed - 1;
            else
                playerToExchangeTo = playerDisplayed;

            playerToExchangeToInventory = playerDisplayed;

            validateButton0.GetComponent<PushButton>().SetEnabled(true);

            if (playerDisplayed == 1)
            {
                validateButton1.GetComponent<PushButton>().MouseExit();
                validateButton1.GetComponent<PushButton>().SetEnabled(false);
                validateButton2.gameObject.SetActive(false);
                if (nbJoueursTotal == 4)
                    validateButton3.gameObject.SetActive(false);
            }
            else if (playerDisplayed == 2)
            {
                validateButton2.GetComponent<PushButton>().MouseExit();
                validateButton2.GetComponent<PushButton>().SetEnabled(false);
                validateButton1.gameObject.SetActive(false);
                if (nbJoueursTotal == 4)
                    validateButton3.gameObject.SetActive(false);
            }
            else if (playerDisplayed == 3)
            {
                validateButton3.GetComponent<PushButton>().MouseExit();
                validateButton3.GetComponent<PushButton>().SetEnabled(false);
                validateButton1.gameObject.SetActive(false);
                validateButton2.gameObject.SetActive(false);
            }
        }
    }

    public void CancelValidation()
    {
        if (isOnline)
        {
            playerToExchangeTo = -1;
            playerToExchangeToInventory = -1;
            validateButton1Reseau.gameObject.SetActive(true);
            validateButton2Reseau.gameObject.SetActive(true);
            if (nbJoueursTotal == 4)
                validateButton3Reseau.gameObject.SetActive(true);

            validateButton1Reseau.GetComponent<PushButton>().SetEnabled(true);
            validateButton2Reseau.GetComponent<PushButton>().SetEnabled(true);
            if (nbJoueursTotal == 4)
                validateButton3Reseau.GetComponent<PushButton>().SetEnabled(true);

            validateButton0Reseau.GetComponent<PushButton>().SetEnabled(false);
        }
        else
        {
            playerToExchangeTo = -1;
            playerToExchangeToInventory = -1;
            validateButton1.gameObject.SetActive(true);
            validateButton2.gameObject.SetActive(true);
            if (nbJoueursTotal == 4)
                validateButton3.gameObject.SetActive(true);

            validateButton1.GetComponent<PushButton>().SetEnabled(true);
            validateButton2.GetComponent<PushButton>().SetEnabled(true);
            if (nbJoueursTotal == 4)
                validateButton3.GetComponent<PushButton>().SetEnabled(true);

            validateButton0.GetComponent<PushButton>().SetEnabled(false);
        }
    }

    public void ConfirmValidation()
    {

        (RessourceType, int)[] send = new (RessourceType, int)[5];
        (RessourceType, int)[] receive = new (RessourceType, int)[5];

        send[0] = (RessourceType.BRICK, offeredRessources[0][0]);
        send[1] = (RessourceType.LUMBER, offeredRessources[0][1]);
        send[2] = (RessourceType.WHEAT, offeredRessources[0][2]);
        send[3] = (RessourceType.WOOL, offeredRessources[0][3]);
        send[4] = (RessourceType.ORE, offeredRessources[0][4]);

        receive[0] = (RessourceType.BRICK, offeredRessources[playerToExchangeToInventory][0]);
        receive[1] = (RessourceType.LUMBER, offeredRessources[playerToExchangeToInventory][1]);
        receive[2] = (RessourceType.WHEAT, offeredRessources[playerToExchangeToInventory][2]);
        receive[3] = (RessourceType.WOOL, offeredRessources[playerToExchangeToInventory][3]);
        receive[4] = (RessourceType.ORE, offeredRessources[playerToExchangeToInventory][4]);

        PlayerExchangeEventArgs e = new PlayerExchangeEventArgs(currentGame.Id, receive, playerToExchangeTo, send, isOnline);

        if(!isOnline) GameView.Instance.OnPlayerExchange(e);

        CancelValidation();
        SetPlayerExchangeValues();

        fxSource.clip = tradeClip;
        fxSource.loop = false;
        fxSource.Play();
    }

    public void DisableHarborExchanges()
    {
        for (int i = 0; i < 5; i++)
        {
            if (exchangeMenu.GetChild(6).GetChild(2).GetChild(1).GetChild(i).GetChild(0).GetComponent<PushButton>().isHovered)
                exchangeMenu.GetChild(6).GetChild(2).GetChild(1).GetChild(i).GetChild(0).GetComponent<PushButton>().MouseExit();
            exchangeMenu.GetChild(6).GetChild(2).GetChild(1).GetChild(i).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
        }
    }

    public void SetHarborExchangeValues()
    {
        IList<(Coordinate, HarborType)> playerHarbors = currentGame.GetCurrentPlayerHarbors();
        bool hasGeneral = false;

        foreach ((Coordinate, HarborType) harb in playerHarbors)
            if (harb.Item2 == HarborType.GENERAL) hasGeneral = true;

        if (hasGeneral)
            for (int i = 0; i < 5; i++)
                exchangeMenu.GetChild(6).GetChild(1).GetChild(1).GetChild(i).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "3";
        else
            for (int i = 0; i < 5; i++)
                exchangeMenu.GetChild(6).GetChild(1).GetChild(1).GetChild(i).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "4";

        foreach ((Coordinate, HarborType) harb in playerHarbors)
        {
            if (harb.Item2 == HarborType.BRICK) exchangeMenu.GetChild(6).GetChild(1).GetChild(1).GetChild(3).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "2";
            if (harb.Item2 == HarborType.LUMBER) exchangeMenu.GetChild(6).GetChild(1).GetChild(1).GetChild(4).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "2";
            if (harb.Item2 == HarborType.WHEAT) exchangeMenu.GetChild(6).GetChild(1).GetChild(1).GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "2";
            if (harb.Item2 == HarborType.WOOL) exchangeMenu.GetChild(6).GetChild(1).GetChild(1).GetChild(1).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "2";
            if (harb.Item2 == HarborType.ORE) exchangeMenu.GetChild(6).GetChild(1).GetChild(1).GetChild(2).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "2";
        }

        exchangeMenu.GetChild(6).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.WHEAT].ToString();
        exchangeMenu.GetChild(6).GetChild(2).GetChild(1).GetChild(1).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.WOOL].ToString();
        exchangeMenu.GetChild(6).GetChild(2).GetChild(1).GetChild(2).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.ORE].ToString();
        exchangeMenu.GetChild(6).GetChild(2).GetChild(1).GetChild(3).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.BRICK].ToString();
        exchangeMenu.GetChild(6).GetChild(2).GetChild(1).GetChild(4).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.LUMBER].ToString();
    }

    public void ExchangeMode(int mode)
    {
        if (mode == 0)
        {
            if (!isOnline)
            {
                exchangeMenu.GetChild(5).gameObject.SetActive(true);
                CancelValidation();
                SetPlayerExchangeValues();
            }
            else
            {
                exchangeMenu.GetChild(7).gameObject.SetActive(true);
                SetOnlineExchangeMenu();
            }
        }
        else
        {
            exchangeMenu.GetChild(6).gameObject.SetActive(true);
            SetHarborExchangeValues();
        }
    }

    public void ExitExchangeMode()
    {
        exchangeMenu.GetChild(5).gameObject.SetActive(false);
        exchangeMenu.GetChild(6).gameObject.SetActive(false);
        exchangeMenu.GetChild(7).gameObject.SetActive(false);
        exchangeMenu.GetChild(8).gameObject.SetActive(false);
    }

    public void SetOnlineExchangeMenu()
    {
        int menuIndex = 0;

        if (currentGame.Players[currentGame.CurrentPlayer].Equals(currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1]))
            menuIndex = 7;
        else
            menuIndex = 8;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 5; j++)
                offeredRessources[i][j] = 0;
        }
        if (nbJoueursTotal < 4)
        {
            exchangeMenu.GetChild(menuIndex).GetChild(4).gameObject.SetActive(false);
        }

        //Si nous somme le joueur courant
        if(menuIndex == 7)
        {
            //passedPlayers est egal a 1 si on a initialise le joueur courant
            int passedPlayers = 1;

            for (int i = 0; i < nbJoueursTotal; i++)
            {
                //elimine les cas ou le joueur est un ordis
                if (!dataSave.GetComponent<UIData>().players[i].Item3)
                {
                    //Si on tombe sur le joueur courant dans la boucle
                    if (currentGame.CurrentPlayer == i)
                    {
                        passedPlayers = 0;
                        //alors il s'affiche dans le cadre 1
                        for (int j = 0; j < 5; j++)
                            exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(j).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "0";

                        exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(2).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(3).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(4).GetChild(0).GetComponent<PushButton>().SetEnabled(false);

                        if (currentGame.Players[i].Ressources[RessourceType.BRICK] > 0) exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(0).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(0).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                        if (currentGame.Players[i].Ressources[RessourceType.LUMBER] > 0) exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(1).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(1).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                        if (currentGame.Players[i].Ressources[RessourceType.WHEAT] > 0) exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(2).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(2).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                        if (currentGame.Players[i].Ressources[RessourceType.WOOL] > 0) exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(3).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(3).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                        if (currentGame.Players[i].Ressources[RessourceType.ORE] > 0) exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(4).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(4).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                    }
                    //si i n'est pas le joueur courant
                    else
                    {
                        //on l'affiche dans le cadre d'indice i+1+passedPlayers
                        if (dataSave.GetComponent<UIData>().players[i].Item2 == 0)
                            exchangeMenu.GetChild(menuIndex).GetChild(i + 1 + passedPlayers).GetChild(3).GetComponent<Image>().sprite = defaultAvatar0;
                        else if (dataSave.GetComponent<UIData>().players[i].Item2 == 1)
                            exchangeMenu.GetChild(menuIndex).GetChild(i + 1 + passedPlayers).GetChild(3).GetComponent<Image>().sprite = defaultAvatar1;
                        else if (dataSave.GetComponent<UIData>().players[i].Item2 == 2)
                            exchangeMenu.GetChild(menuIndex).GetChild(i + 1 + passedPlayers).GetChild(3).GetComponent<Image>().sprite = defaultAvatar2;
                        else
                            exchangeMenu.GetChild(menuIndex).GetChild(i + 1 + passedPlayers).GetChild(3).GetComponent<Image>().sprite = defaultAvatar3;

                        exchangeMenu.GetChild(menuIndex).GetChild(i + 1 + passedPlayers).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[i].Item1;

                        for (int j = 0; j < 5; j++)
                            exchangeMenu.GetChild(menuIndex).GetChild(i + 1 + passedPlayers).GetChild(1).GetChild(j).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "0";
                    }
                }
                //si c'est un ordi
                else
                {
                    //on l'affiche dans le cadre i+1+passedPlayers
                    exchangeMenu.GetChild(menuIndex).GetChild(i + 1 + passedPlayers).GetChild(3).GetComponent<Image>().sprite = robotAvatar;

                    exchangeMenu.GetChild(menuIndex).GetChild(i + 1 + passedPlayers).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[i].Item1;

                    for (int j = 0; j < 5; j++)
                        exchangeMenu.GetChild(menuIndex).GetChild(i + 1 + passedPlayers).GetChild(1).GetChild(j).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "0";
                }
            }
        }
        //si on est pas le joueur courant
        else
        {
            bool firstSlotTaken = false;

            for (int i = 0; i < nbJoueursTotal; i++)
            {
                //elimine les cas ou le joueur est un ordis
                if (!dataSave.GetComponent<UIData>().players[i].Item3)
                {
                    //Si on tombe sur le joueur courant dans la boucle
                    if (currentGame.CurrentPlayer == i)
                    {
                        //alors il s'affiche dans le cadre 3
                        if (dataSave.GetComponent<UIData>().players[i].Item2 == 0)
                            exchangeMenu.GetChild(menuIndex).GetChild(3).GetChild(3).GetComponent<Image>().sprite = defaultAvatar0;
                        else if (dataSave.GetComponent<UIData>().players[i].Item2 == 1)
                            exchangeMenu.GetChild(menuIndex).GetChild(3).GetChild(3).GetComponent<Image>().sprite = defaultAvatar1;
                        else if (dataSave.GetComponent<UIData>().players[i].Item2 == 2)
                            exchangeMenu.GetChild(menuIndex).GetChild(3).GetChild(3).GetComponent<Image>().sprite = defaultAvatar2;
                        else
                            exchangeMenu.GetChild(menuIndex).GetChild(3).GetChild(3).GetComponent<Image>().sprite = defaultAvatar3;

                        exchangeMenu.GetChild(menuIndex).GetChild(3).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[i].Item1;

                        for (int j = 0; j < 5; j++)
                            exchangeMenu.GetChild(menuIndex).GetChild(3).GetChild(1).GetChild(j).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "0";
                    }
                    //Si on tombe sur le soi meme
                    else if((dataSave.GetComponent<UIData>().selfInfo.Item3 - 1) == i)
                    {
                        //alors il s'affiche dans le cadre 1
                        for (int j = 0; j < 5; j++)
                            exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(j).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "0";

                        exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(2).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(3).GetChild(0).GetComponent<PushButton>().SetEnabled(false);
                        exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(4).GetChild(0).GetComponent<PushButton>().SetEnabled(false);

                        if (currentGame.Players[i].Ressources[RessourceType.BRICK] > 0) exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(0).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(0).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                        if (currentGame.Players[i].Ressources[RessourceType.LUMBER] > 0) exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(1).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(1).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                        if (currentGame.Players[i].Ressources[RessourceType.WHEAT] > 0) exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(2).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(2).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                        if (currentGame.Players[i].Ressources[RessourceType.WOOL] > 0) exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(3).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(3).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                        if (currentGame.Players[i].Ressources[RessourceType.ORE] > 0) exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(4).GetChild(2).GetComponent<PushButton>().SetEnabled(true);
                        else exchangeMenu.GetChild(menuIndex).GetChild(1).GetChild(1).GetChild(4).GetChild(2).GetComponent<PushButton>().SetEnabled(false);
                    }
                    //Les 2 cas restants
                    else
                    {
                        //si c'est le premier des 2 cas restant
                        if (!firstSlotTaken)
                        {
                            firstSlotTaken = true;
                            //alors il s'affiche dans le cadre 2
                            if (dataSave.GetComponent<UIData>().players[i].Item2 == 0)
                                exchangeMenu.GetChild(menuIndex).GetChild(2).GetChild(3).GetComponent<Image>().sprite = defaultAvatar0;
                            else if (dataSave.GetComponent<UIData>().players[i].Item2 == 1)
                                exchangeMenu.GetChild(menuIndex).GetChild(2).GetChild(3).GetComponent<Image>().sprite = defaultAvatar1;
                            else if (dataSave.GetComponent<UIData>().players[i].Item2 == 2)
                                exchangeMenu.GetChild(menuIndex).GetChild(2).GetChild(3).GetComponent<Image>().sprite = defaultAvatar2;
                            else
                                exchangeMenu.GetChild(menuIndex).GetChild(2).GetChild(3).GetComponent<Image>().sprite = defaultAvatar3;

                            exchangeMenu.GetChild(menuIndex).GetChild(2).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[i].Item1;

                            for (int j = 0; j < 5; j++)
                                exchangeMenu.GetChild(menuIndex).GetChild(2).GetChild(1).GetChild(j).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "0";
                        }
                        //si c'est le 2eme
                        else
                        {
                            //alors il s'affiche dans le cadre 4
                            if (dataSave.GetComponent<UIData>().players[i].Item2 == 0)
                                exchangeMenu.GetChild(menuIndex).GetChild(4).GetChild(3).GetComponent<Image>().sprite = defaultAvatar0;
                            else if (dataSave.GetComponent<UIData>().players[i].Item2 == 1)
                                exchangeMenu.GetChild(menuIndex).GetChild(4).GetChild(3).GetComponent<Image>().sprite = defaultAvatar1;
                            else if (dataSave.GetComponent<UIData>().players[i].Item2 == 2)
                                exchangeMenu.GetChild(menuIndex).GetChild(4).GetChild(3).GetComponent<Image>().sprite = defaultAvatar2;
                            else
                                exchangeMenu.GetChild(menuIndex).GetChild(4).GetChild(3).GetComponent<Image>().sprite = defaultAvatar3;

                            exchangeMenu.GetChild(menuIndex).GetChild(4).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[i].Item1;

                            for (int j = 0; j < 5; j++)
                                exchangeMenu.GetChild(menuIndex).GetChild(4).GetChild(1).GetChild(j).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "0";
                        }
                    }
                }
                //si c'est un ordi
                else
                {
                    //si c'est le premier des 2 cas restant
                    if (!firstSlotTaken)
                    {
                        firstSlotTaken = true;
                        //alors il s'affiche dans le cadre 2
                        exchangeMenu.GetChild(menuIndex).GetChild(2).GetChild(3).GetComponent<Image>().sprite = robotAvatar;

                        exchangeMenu.GetChild(menuIndex).GetChild(2).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[i].Item1;

                        for (int j = 0; j < 5; j++)
                            exchangeMenu.GetChild(menuIndex).GetChild(2).GetChild(1).GetChild(j).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "0";
                    }
                    //si c'est le 2eme
                    else
                    {
                        //alors il s'affiche dans le cadre 4
                        exchangeMenu.GetChild(menuIndex).GetChild(4).GetChild(3).GetComponent<Image>().sprite = robotAvatar;

                        exchangeMenu.GetChild(menuIndex).GetChild(4).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[i].Item1;

                        for (int j = 0; j < 5; j++)
                            exchangeMenu.GetChild(menuIndex).GetChild(4).GetChild(1).GetChild(j).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "0";
                    }
                }
            }
        }
    }

    public void EndExchanges()
    {
        DisableHarborExchanges();
        exchangeMenu.gameObject.SetActive(false);
        if (currentGame.CurrentPhase == GamePhase.EXCHANGE)
        {
            BaseEventArgs e = new BaseEventArgs(currentGame.Id, isOnline);
            if (isOnline)
            {
                BaseEventArgs evBis = new BaseEventArgs(Network.View.NetworkView.Instance.getIdGame(), true, "OnEndPhase");
                Network.View.NetworkView.Instance.OnEndPhase(evBis);
            }
            else GameView.Instance.OnEndPhase(e);
        }

        DisablePlayerDisplay();
    }

    public void HarborExchangeRessourceSelected(int ressource)
    {
        DisableHarborExchanges();

        for (int i = 0; i < 5; i++)
            exchangeMenu.GetChild(6).GetChild(2).GetChild(1).GetChild(i).GetChild(0).GetComponent<PushButton>().SetEnabled(false);

        IList<(Coordinate, HarborType)> playerHarbors = currentGame.GetCurrentPlayerHarbors();

        int generalNumber = 4;


        foreach ((Coordinate, HarborType) harb in playerHarbors)
        {
            if (harb.Item2 == HarborType.GENERAL) generalNumber = 3;
        }

        foreach ((Coordinate, HarborType) harb in playerHarbors)
        {
            if (harb.Item2 == HarborType.BRICK && (RessourceType)ressource == RessourceType.BRICK)
                generalNumber = 2;
            if (harb.Item2 == HarborType.LUMBER && (RessourceType)ressource == RessourceType.LUMBER)
                generalNumber = 2;
            if (harb.Item2 == HarborType.WHEAT && (RessourceType)ressource == RessourceType.WHEAT)
                generalNumber = 2;
            if (harb.Item2 == HarborType.WOOL && (RessourceType)ressource == RessourceType.WOOL)
                generalNumber = 2;
            if (harb.Item2 == HarborType.ORE && (RessourceType)ressource == RessourceType.ORE)
                generalNumber = 2;
        }

        if (currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.WHEAT] >= generalNumber)
            exchangeMenu.GetChild(6).GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetComponent<PushButton>().SetEnabled(true);
        if (currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.WOOL] >= generalNumber)
            exchangeMenu.GetChild(6).GetChild(2).GetChild(1).GetChild(1).GetChild(0).GetComponent<PushButton>().SetEnabled(true);
        if (currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.ORE] >= generalNumber)
            exchangeMenu.GetChild(6).GetChild(2).GetChild(1).GetChild(2).GetChild(0).GetComponent<PushButton>().SetEnabled(true);
        if (currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.BRICK] >= generalNumber)
            exchangeMenu.GetChild(6).GetChild(2).GetChild(1).GetChild(3).GetChild(0).GetComponent<PushButton>().SetEnabled(true);
        if (currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.LUMBER] >= generalNumber)
            exchangeMenu.GetChild(6).GetChild(2).GetChild(1).GetChild(4).GetChild(0).GetComponent<PushButton>().SetEnabled(true);

        harborExchangeValue = ressource;
    }

    public void HarborTradeRessources(int ressource)
    {
        DisableHarborExchanges();

        IList<(Coordinate, HarborType)> playerHarbors = currentGame.GetCurrentPlayerHarbors();
        Debug.Log(playerHarbors.Count);
        foreach (var h in playerHarbors)
        {
            Debug.Log(h.Item2);
        }

        HarborType ressourceToPay = HarborType.NONE;

        foreach ((Coordinate, HarborType) harb in playerHarbors)
            if (harb.Item2 == HarborType.GENERAL) ressourceToPay = HarborType.GENERAL;

        foreach ((Coordinate, HarborType) harb in playerHarbors)
        {
            if (harb.Item2 == HarborType.BRICK && (RessourceType)harborExchangeValue == RessourceType.BRICK) ressourceToPay = HarborType.BRICK;
            if (harb.Item2 == HarborType.LUMBER && (RessourceType)harborExchangeValue == RessourceType.LUMBER) ressourceToPay = HarborType.LUMBER;
            if (harb.Item2 == HarborType.WHEAT && (RessourceType)harborExchangeValue == RessourceType.WHEAT) ressourceToPay = HarborType.WHEAT;
            if (harb.Item2 == HarborType.WOOL && (RessourceType)harborExchangeValue == RessourceType.WOOL) ressourceToPay = HarborType.WOOL;
            if (harb.Item2 == HarborType.ORE && (RessourceType)harborExchangeValue == RessourceType.ORE) ressourceToPay = HarborType.ORE;
        }

        HarborExchangeEventArgs e = new HarborExchangeEventArgs(currentGame.Id, ressourceToPay, (RessourceType)ressource, (RessourceType)harborExchangeValue, isOnline);
        if (isOnline)
        {
            e.GameId = Network.View.NetworkView.Instance.getIdGame();
            Network.View.NetworkView.Instance.OnHarborExchange(e);
        }
        else GameView.Instance.OnHarborExchange(e);
        SetHarborExchangeValues();
    }

    public void UpdateDiscardScreen()
    {
        if (isOnline)
        {
            discardMenu.GetChild(3).gameObject.SetActive(false);
            discardMenu.GetChild(4).gameObject.SetActive(true);

            if (dataSave.GetComponent<UIData>().players[dataSave.GetComponent<UIData>().selfInfo.Item3-1].Item2 == 0)
                discardMenu.GetChild(1).GetComponent<Image>().sprite = defaultAvatar0;
            else if (dataSave.GetComponent<UIData>().players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Item2 == 1)
                discardMenu.GetChild(1).GetComponent<Image>().sprite = defaultAvatar1;
            else if (dataSave.GetComponent<UIData>().players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Item2 == 2)
                discardMenu.GetChild(1).GetComponent<Image>().sprite = defaultAvatar2;
            else
                discardMenu.GetChild(1).GetComponent<Image>().sprite = defaultAvatar3;

            discardMenu.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Item1;
            discardMenu.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().colorGradientPreset = PlayersColors[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1];

            for (int i = 0; i < 5; i++)
                {
                    discardMenu.GetChild(5).GetChild(0).GetChild(1).GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = ressourcesDefausse[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][i].ToString();
                    discardMenu.GetChild(5).GetChild(2).GetChild(1).GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = (currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[(RessourceType)(i + 1)] - ressourcesDefausse[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][i]).ToString();
                }

                int nbRessourcesDefausse = 0;

                if (currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].TotalRessourceNumber > 7)
                    nbRessourcesDefausse = currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].TotalRessourceNumber / 2;

                for (int i = 0; i < 5; i++)
                    nbRessourcesDefausse -= ressourcesDefausse[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][i];

                discardMenu.GetChild(5).GetChild(1).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = nbRessourcesDefausse.ToString();

                if (nbRessourcesDefausse > 0)
                {
                    discardMenu.GetChild(3).GetComponent<PushButton>().SetEnabled(false);
                    discardMenu.GetChild(4).GetComponent<PushButton>().SetEnabled(false);
                    for (int i = 0; i < 5; i++)
                    {
                        if (currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[(RessourceType)(i + 1)] - ressourcesDefausse[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][i] > 0)
                            discardMenu.GetChild(5).GetChild(2).GetChild(0).GetChild(i).GetComponent<PushButton>().SetEnabled(true);
                        else
                        {
                            if (discardMenu.GetChild(5).GetChild(2).GetChild(0).GetChild(i).GetComponent<PushButton>().isHovered)
                                discardMenu.GetChild(5).GetChild(2).GetChild(0).GetChild(i).GetComponent<PushButton>().MouseExit();
                            discardMenu.GetChild(5).GetChild(2).GetChild(0).GetChild(i).GetComponent<PushButton>().SetEnabled(false);
                        }
                    }
                }
                else
                {
                    discardMenu.GetChild(3).GetComponent<PushButton>().SetEnabled(true);
                    discardMenu.GetChild(4).GetComponent<PushButton>().SetEnabled(true);
                    for (int i = 0; i < 5; i++)
                    {
                        if (discardMenu.GetChild(5).GetChild(2).GetChild(0).GetChild(i).GetComponent<PushButton>().isHovered)
                            discardMenu.GetChild(5).GetChild(2).GetChild(0).GetChild(i).GetComponent<PushButton>().MouseExit();
                        discardMenu.GetChild(5).GetChild(2).GetChild(0).GetChild(i).GetComponent<PushButton>().SetEnabled(false);
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    if (ressourcesDefausse[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][i] <= 0)
                    {
                        if (discardMenu.GetChild(5).GetChild(0).GetChild(0).GetChild(i).GetComponent<PushButton>().isHovered)
                            discardMenu.GetChild(5).GetChild(0).GetChild(0).GetChild(i).GetComponent<PushButton>().MouseExit();
                        discardMenu.GetChild(5).GetChild(0).GetChild(0).GetChild(i).GetComponent<PushButton>().SetEnabled(false);
                    }
                    else
                        discardMenu.GetChild(5).GetChild(0).GetChild(0).GetChild(i).GetComponent<PushButton>().SetEnabled(true);
                }

        }
        else
        {
            if (dataSave.GetComponent<UIData>().players[currentPlayerDiscard].Item3)
            {
                BotNextPlayerDiscard();
            }
            else
            {
                if (currentPlayerDiscard + 1 == nbJoueursTotal)
                {
                    discardMenu.GetChild(3).GetComponent<PushButton>().MouseExit();
                    discardMenu.GetChild(3).gameObject.SetActive(false);
                    discardMenu.GetChild(4).gameObject.SetActive(true);
                }

                if (dataSave.GetComponent<UIData>().players[currentPlayerDiscard].Item3 == false)
                {
                    if (dataSave.GetComponent<UIData>().players[currentPlayerDiscard].Item2 == 0)
                        discardMenu.GetChild(1).GetComponent<Image>().sprite = defaultAvatar0;
                    else if (dataSave.GetComponent<UIData>().players[currentPlayerDiscard].Item2 == 1)
                        discardMenu.GetChild(1).GetComponent<Image>().sprite = defaultAvatar1;
                    else if (dataSave.GetComponent<UIData>().players[currentPlayerDiscard].Item2 == 2)
                        discardMenu.GetChild(1).GetComponent<Image>().sprite = defaultAvatar2;
                    else
                        discardMenu.GetChild(1).GetComponent<Image>().sprite = defaultAvatar3;
                }
                else
                    discardMenu.GetChild(1).GetComponent<Image>().sprite = robotAvatar;

                discardMenu.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[currentPlayerDiscard].Item1;
                discardMenu.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().colorGradientPreset = PlayersColors[currentPlayerDiscard];

                for (int i = 0; i < 5; i++)
                {
                    discardMenu.GetChild(5).GetChild(0).GetChild(1).GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = ressourcesDefausse[currentPlayerDiscard][i].ToString();
                    discardMenu.GetChild(5).GetChild(2).GetChild(1).GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = (currentGame.Players[currentPlayerDiscard].Ressources[(RessourceType)(i + 1)] - ressourcesDefausse[currentPlayerDiscard][i]).ToString();
                }

                int nbRessourcesDefausse = 0;

                if (currentGame.Players[currentPlayerDiscard].TotalRessourceNumber > 7)
                    nbRessourcesDefausse = currentGame.Players[currentPlayerDiscard].TotalRessourceNumber / 2;

                for (int i = 0; i < 5; i++)
                    nbRessourcesDefausse -= ressourcesDefausse[currentPlayerDiscard][i];

                discardMenu.GetChild(5).GetChild(1).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = nbRessourcesDefausse.ToString();

                if (nbRessourcesDefausse > 0)
                {
                    discardMenu.GetChild(3).GetComponent<PushButton>().SetEnabled(false);
                    discardMenu.GetChild(4).GetComponent<PushButton>().SetEnabled(false);
                    for (int i = 0; i < 5; i++)
                    {
                        if (currentGame.Players[currentPlayerDiscard].Ressources[(RessourceType)(i + 1)] - ressourcesDefausse[currentPlayerDiscard][i] > 0)
                            discardMenu.GetChild(5).GetChild(2).GetChild(0).GetChild(i).GetComponent<PushButton>().SetEnabled(true);
                        else
                        {
                            if (discardMenu.GetChild(5).GetChild(2).GetChild(0).GetChild(i).GetComponent<PushButton>().isHovered)
                                discardMenu.GetChild(5).GetChild(2).GetChild(0).GetChild(i).GetComponent<PushButton>().MouseExit();
                            discardMenu.GetChild(5).GetChild(2).GetChild(0).GetChild(i).GetComponent<PushButton>().SetEnabled(false);
                        }
                    }
                }
                else
                {
                    discardMenu.GetChild(3).GetComponent<PushButton>().SetEnabled(true);
                    discardMenu.GetChild(4).GetComponent<PushButton>().SetEnabled(true);
                    for (int i = 0; i < 5; i++)
                    {
                        if (discardMenu.GetChild(5).GetChild(2).GetChild(0).GetChild(i).GetComponent<PushButton>().isHovered)
                            discardMenu.GetChild(5).GetChild(2).GetChild(0).GetChild(i).GetComponent<PushButton>().MouseExit();
                        discardMenu.GetChild(5).GetChild(2).GetChild(0).GetChild(i).GetComponent<PushButton>().SetEnabled(false);
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    if (ressourcesDefausse[currentPlayerDiscard][i] <= 0)
                    {
                        if (discardMenu.GetChild(5).GetChild(0).GetChild(0).GetChild(i).GetComponent<PushButton>().isHovered)
                            discardMenu.GetChild(5).GetChild(0).GetChild(0).GetChild(i).GetComponent<PushButton>().MouseExit();
                        discardMenu.GetChild(5).GetChild(0).GetChild(0).GetChild(i).GetComponent<PushButton>().SetEnabled(false);
                    }
                    else
                        discardMenu.GetChild(5).GetChild(0).GetChild(0).GetChild(i).GetComponent<PushButton>().SetEnabled(true);
                }
            }
        }
    }

    public void AddDiscard(int ressource)
    {
        if (isOnline)
        {
            ressourcesDefausse[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][ressource - 1]++;
            UpdateDiscardScreen();
        }
        else
        {
            ressourcesDefausse[currentPlayerDiscard][ressource - 1]++;
            UpdateDiscardScreen();
        }
    }
    public void SubstractDiscard(int ressource)
    {
        if (isOnline)
        {
            ressourcesDefausse[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][ressource - 1]--;
            UpdateDiscardScreen();
        }
        else
        {
            ressourcesDefausse[currentPlayerDiscard][ressource - 1]--;
            UpdateDiscardScreen();
        }
    }

    public void DiscardPhaseBeginHandler(object sender, GameStatusArgs e)
    {
        Debug.Log("phase:" + currentGame.CurrentPhase);

        discardMenu.gameObject.SetActive(true);
        if (isOnline)
        {
            currentPlayerDiscard = 0;
            for (int i = 0; i < 5; i++)
                ressourcesDefausse[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][i] = 0;
            UpdateDiscardScreen();
        }
        else
        {
            currentPlayerDiscard = 0;
            for (int i = 0; i < 5; i++)
                ressourcesDefausse[currentPlayerDiscard][i] = 0;
            UpdateDiscardScreen();
        }
    }

    public void NextPlayerDiscard()
    {
        currentPlayerDiscard++;

        for (int i = 0; i < 5; i++)
            ressourcesDefausse[currentPlayerDiscard][i] = 0;
        UpdateDiscardScreen();
    }

    public void BotNextPlayerDiscard()
    {
        currentPlayerDiscard++;
        if (currentPlayerDiscard < nbJoueursTotal)
        {
            for (int i = 0; i < 5; i++)
                ressourcesDefausse[currentPlayerDiscard][i] = 0;
            UpdateDiscardScreen();
        }
        else
        {
            ValidateDiscard();
        }
    }

    public void ValidateDiscard()
    {
        if (isOnline)
        {
            discardMenu.GetChild(4).transform.GetComponent<PushButton>().SetEnabled(false);
            //TODO envoyer au noyau/reseau l'info de discard personel
            List<(RessourceType, int)> discardResult = new List<(RessourceType, int)>();
            for(int i=0; i<5; i++)
            {
                discardResult.Add(((RessourceType)(i + 1), ressourcesDefausse[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][i]));
                Debug.Log("Défausse ressource " + ((RessourceType)(i + 1)).ToString() + ": "+ ressourcesDefausse[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][i]);
            }
            DiscardEvent ev = new DiscardEvent(Network.View.NetworkView.Instance.getIdGame(), Serialization.Serialize(discardResult), Network.View.NetworkView.Instance.getIdInGame());
            Network.View.NetworkView.Instance.OnDiscardExtraRessources(ev);
        }
        else
        {
            List<(int, List<(RessourceType, int)>)> discardSender = new List<(int, List<(RessourceType, int)>)>();
            for (int i = 0; i < nbJoueursTotal; i++)
            {
                discardSender.Add((i, new List<(RessourceType, int)>()));
                for (int j = 0; j < 5; j++)
                {
                    discardSender[i].Item2.Add(((RessourceType)(j + 1), ressourcesDefausse[i][j]));
                }
            }
            GameView.Instance.OnDiscardExtraRessources(new DiscardEventArgs(currentGame.Id, discardSender, isOnline));

            discardMenu.GetChild(4).gameObject.SetActive(false);
            discardMenu.GetChild(3).gameObject.SetActive(true);
            discardMenu.gameObject.SetActive(false);
        }
    }

    public void BanditMoveBeginHandler(object sender, GameStatusArgs e)
    {
        Debug.Log("phase:" + currentGame.CurrentPhase);
        ToggleColony(false);
        ToggleCity(false);
        ToggleRoads(false);

        bool doTheThing = false;

        if (isOnline)
        {
            //Desactive les menu de discard
            discardMenu.GetChild(4).gameObject.SetActive(false);
            discardMenu.GetChild(3).gameObject.SetActive(true);
            discardMenu.gameObject.SetActive(false);

            if (dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item3 == false && (dataSave.GetComponent<UIData>().selfInfo.Item3 - 1) == currentGame.CurrentPlayer)
            {
                doTheThing = true;
            }
        }

        else if (!dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item3)
        {
            doTheThing = true;
        }

        if (doTheThing)
        {
            isWaitingForBandit = true;
            ToggleBandits(true);
            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (go.GetComponent<BanditButton>() != null && go.GetComponent<BanditButton>().buttonCoordinate.Equals(currentGame.GameGrid.CurrentThiefLocation))
                {
                    go.GetComponent<Image>().enabled = false;
                    go.GetComponent<Button>().enabled = false;
                    go.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
                }
            }
        }
    }

    public void MoveBandit(Coordinate coordinate)
    {
        ToggleBandits(false);
        choosePlayer.gameObject.SetActive(true);

        bool hasNeighbor = false;

        List<IIntersection> theIntersections = currentGame.GameGrid.GetIntersectionsFromTile(coordinate);
        foreach (IIntersection inter in theIntersections)
        {
            if (inter.Building != ConstructionType.NONE && inter.PlayerId != currentGame.CurrentPlayer)
            {
                choosePlayer.GetChild(inter.PlayerId + 1).gameObject.SetActive(true);
                hasNeighbor = true;
            }
        }

        newBanditCoordinate = coordinate;

        if (!hasNeighbor)
            SendBanditMovement(-1);
    }

    public void SendBanditMovement(int player)
    {
        isWaitingForBandit = false;

        for (int i = 0; i < nbJoueursTotal; i++)
            choosePlayer.GetChild(i + 1).gameObject.SetActive(false);
        choosePlayer.gameObject.SetActive(false);

        BanditMoveEventArgs e = new BanditMoveEventArgs(currentGame.Id, newBanditCoordinate, player, isOnline);
        if (isUsingDevCard)
        {
            if (isOnline)
            {
                e.GameId = Network.View.NetworkView.Instance.getIdGame();
                Network.View.NetworkView.Instance.OnKnightCardUse(e);
            }
            else GameView.Instance.OnKnightCardUse(e);
            isUsingDevCard = false;
        }
        else
        {
            if (isOnline)
            {
                e.GameId = Network.View.NetworkView.Instance.getIdGame();
                Network.View.NetworkView.Instance.OnBanditMove(e);
            }
            else GameView.Instance.OnBanditMove(e);
        }
    }

    public void ConstructionPhaseBeginHandler(object sender, GameStatusArgs e)
    {
        if (isOnline)
        {

            exchangeMenu.gameObject.SetActive(false);
            exchangeMenu.GetChild(8).gameObject.SetActive(false);

            if (langue == 0)
                SendMessageToChat("Résultat des dés: " + (GameView.Instance.Game.lastDice.Item1 + GameView.Instance.Game.lastDice.Item2).ToString(), Message.MessageType.diceInfo);
            else if (langue == 1)
                SendMessageToChat("Dice throw result: " + (GameView.Instance.Game.lastDice.Item1 + GameView.Instance.Game.lastDice.Item2).ToString(), Message.MessageType.diceInfo);

            if (!dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item3 && (dataSave.GetComponent<UIData>().selfInfo.Item3 - 1) == currentGame.CurrentPlayer)
            {
                if (currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.SETTLEMENT))
                    buildColony.GetComponent<PushButton>().SetEnabled(true);
                else
                    buildColony.GetComponent<PushButton>().SetEnabled(false);
                if (currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.CITY))
                    buildCity.GetComponent<PushButton>().SetEnabled(true);
                else
                    buildCity.GetComponent<PushButton>().SetEnabled(false);
                if (currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.ROAD))
                    buildRoad.GetComponent<PushButton>().SetEnabled(true);
                else
                    buildRoad.GetComponent<PushButton>().SetEnabled(false);
                if (currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.DEVELOPMENT_CARD))
                    buyDevCard.GetComponent<PushButton>().SetEnabled(true);
                else
                    buyDevCard.GetComponent<PushButton>().SetEnabled(false);
                endTurn.GetComponent<PushButton>().SetEnabled(true);
            }
        }

        else if (!dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item3)
        {
            if (currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.SETTLEMENT))
                buildColony.GetComponent<PushButton>().SetEnabled(true);
            else
                buildColony.GetComponent<PushButton>().SetEnabled(false);
            if (currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.CITY))
                buildCity.GetComponent<PushButton>().SetEnabled(true);
            else
                buildCity.GetComponent<PushButton>().SetEnabled(false);
            if (currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.ROAD))
                buildRoad.GetComponent<PushButton>().SetEnabled(true);
            else
                buildRoad.GetComponent<PushButton>().SetEnabled(false);
            if (currentGame.Players[currentGame.CurrentPlayer].HasEnoughRessources(ConstructionType.DEVELOPMENT_CARD))
                buyDevCard.GetComponent<PushButton>().SetEnabled(true);
            else
                buyDevCard.GetComponent<PushButton>().SetEnabled(false);
            endTurn.GetComponent<PushButton>().SetEnabled(true);
        }
    }

    public virtual void ConstructionDoneHandler(object sender, ActionDoneInfoArgs e)
    {
        if (currentGame.CurrentPhase == GamePhase.INITIAL_BUILDING_1 || currentGame.CurrentPhase == GamePhase.INITIAL_BUILDING_2)
        {
            bool doTheThing = false;

            if(isOnline)
            {
                if(dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item3 == false && (dataSave.GetComponent<UIData>().selfInfo.Item3 - 1) == currentGame.CurrentPlayer && e.Action == ActionType.ROAD)
                {
                    doTheThing = true;
                }
            }
            else if (dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item3 == false)
            {
                doTheThing = true;
            }

            if (doTheThing)
            {
                ToggleColony(true);
                ToggleRoads(false);

                foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                {
                    if (go.GetComponent<ConstructionButton>() != null && go.GetComponent<ConstructionButton>().constructionType == ConstructionType.SETTLEMENT)
                    {
                        if (currentGame.GameGrid.Intersections[go.GetComponent<ConstructionButton>().buttonCoordinate].Building != ConstructionType.NONE)
                        {
                            //toggle l'intersection
                            go.GetComponent<ConstructionButton>().ToggleButton(false);
                            //toggle les intersections voisines
                            Coordinate initialCoordinates = go.GetComponent<ConstructionButton>().buttonCoordinate;
                            foreach (GameObject go2 in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                            {
                                if (initialCoordinates.D == Direction.UP)
                                {
                                    if (go2.GetComponent<ConstructionButton>() != null && go2.GetComponent<ConstructionButton>().constructionType == ConstructionType.SETTLEMENT)
                                    {
                                        if (go2.GetComponent<ConstructionButton>().buttonCoordinate.Equals(new Coordinate(initialCoordinates.X, initialCoordinates.Y + 1, initialCoordinates.Z - 1, Direction.DOWN))
                                            || go2.GetComponent<ConstructionButton>().buttonCoordinate.Equals(new Coordinate(initialCoordinates.X + 1, initialCoordinates.Y, initialCoordinates.Z - 1, Direction.DOWN))
                                            || go2.GetComponent<ConstructionButton>().buttonCoordinate.Equals(new Coordinate(initialCoordinates.X + 1, initialCoordinates.Y + 1, initialCoordinates.Z - 2, Direction.DOWN)))
                                            go2.GetComponent<ConstructionButton>().ToggleButton(false);
                                    }
                                }
                                else
                                {
                                    if (go2.GetComponent<ConstructionButton>() != null && go2.GetComponent<ConstructionButton>().constructionType == ConstructionType.SETTLEMENT)
                                    {
                                        if (go2.GetComponent<ConstructionButton>().buttonCoordinate.Equals(new Coordinate(initialCoordinates.X - 1, initialCoordinates.Y, initialCoordinates.Z + 1, Direction.UP))
                                           || go2.GetComponent<ConstructionButton>().buttonCoordinate.Equals(new Coordinate(initialCoordinates.X, initialCoordinates.Y - 1, initialCoordinates.Z + 1, Direction.UP))
                                           || go2.GetComponent<ConstructionButton>().buttonCoordinate.Equals(new Coordinate(initialCoordinates.X - 1, initialCoordinates.Y - 1, initialCoordinates.Z + 2, Direction.UP)))
                                            go2.GetComponent<ConstructionButton>().ToggleButton(false);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (isOnline && dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item3 == false && (dataSave.GetComponent<UIData>().selfInfo.Item3 - 1) == currentGame.CurrentPlayer && e.Action == ActionType.SETTLEMENT)
            {
                UpdateConstructionMap();
                ToggleColony(false);
                ToggleRoads(false);

                foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                {
                    List<Coordinate> possibleSpot = currentGame.GameGrid.PossibleRoads(currentGame.Players[currentGame.CurrentPlayer]);
                    for (int i = 0; i < possibleSpot.Count; i++)
                    {
                        if (go.GetComponent<ConstructionButton>() != null && go.GetComponent<ConstructionButton>().constructionType == ConstructionType.ROAD)
                        {

                            if (go.GetComponent<ConstructionButton>().buttonCoordinate.Equals(possibleSpot[i]))
                                go.GetComponent<ConstructionButton>().ToggleButton(true);
                        }
                    }
                }
            }
        }

        ShowCurrentPlayerColor();

        if (e.Action == ActionType.CITY)
            fxSource.clip = cityBuild;
        else if (e.Action == ActionType.SETTLEMENT)
            fxSource.clip = colonyBuild;
        else if (e.Action == ActionType.ROAD)
            fxSource.clip = roadBuild;

        if (e.Action != ActionType.DEVELOPMENT_CARD)
        {
            fxSource.loop = false;
            fxSource.Play();
        }

            string text = "";
        string constructionChatMessage = "";

        if (langue == 0)
        {
            switch (e.Action)
            {
                case ActionType.CITY: constructionChatMessage = " a construit: Ville"; break;
                case ActionType.ROAD: constructionChatMessage = " a construit: Route"; break;
                case ActionType.SETTLEMENT: constructionChatMessage = " a construit: Colonie"; break;
                case ActionType.DEVELOPMENT_CARD: constructionChatMessage = " a acheté: Carte de Développement"; break;

                default: break;
            }
        }
        else if (langue == 1)
        {
            switch (e.Action)
            {
                case ActionType.CITY: constructionChatMessage = " built: City"; break;
                case ActionType.ROAD: constructionChatMessage = " built: Road"; break;
                case ActionType.SETTLEMENT: constructionChatMessage = " built: Settlement"; break;
                case ActionType.DEVELOPMENT_CARD: constructionChatMessage = " bought: Development Card"; break;

                default: break;
            }
        }

        if (langue == 0)
            text = dataSave.GetComponent<UIData>().players[e.PlayerID].Item1 + constructionChatMessage;
        else if (langue == 1)
            text = dataSave.GetComponent<UIData>().players[e.PlayerID].Item1 + constructionChatMessage;

        SendMessageToChat(text, Message.MessageType.constructionMessage);
        //if (isOnline) NetworkClient.Instance.sendMessage(text);

        UpdateConstructionMap();
        //Debug.Log("Hello am ConstructionDoneHandler");

        if (!e.ActionSuccessful)
            SendMessageToChat("Action Failed", Message.MessageType.banditMove);
    }

    public void BanditMoveHandler(object sender, BanditMoveEventArgs e)
    {
        isWaitingForBandit = false;
        string text = "";
        if (langue == 0)
            text = dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item1 + " a déplacé le bandit";
        else if (langue == 1)
            text = dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item1 + " moved the bandit";

        SendMessageToChat(text, Message.MessageType.banditMove);

        fxSource.clip = banditClip;
        fxSource.loop = false;
        fxSource.Play();

        UpdateConstructionMap();
    }

    public void ExchangeDoneHandler(object sender, ActionDoneInfoArgs e)
    {
        if(e.Action == ActionType.HARBOR_EXCHANGE)
        {
            string text = "";
            string ressourceEchange = "";
            string ressourcePaye = "";
            if (langue == 0)
            {
                switch (e.Resource1)
                {
                    case RessourceType.BRICK: ressourceEchange = "Brique"; break;
                    case RessourceType.LUMBER: ressourceEchange = "Bois"; break;
                    case RessourceType.WHEAT: ressourceEchange = "Ble"; break;
                    case RessourceType.WOOL: ressourceEchange = "Laine"; break;
                    case RessourceType.ORE: ressourceEchange = "Minerai"; break;

                    default: break;
                }
                switch (e.Resource2)
                {
                    case RessourceType.BRICK: ressourcePaye = "Brique"; break;
                    case RessourceType.LUMBER: ressourcePaye = "Bois"; break;
                    case RessourceType.WHEAT: ressourcePaye = "Ble"; break;
                    case RessourceType.WOOL: ressourcePaye = "Laine"; break;
                    case RessourceType.ORE: ressourcePaye = "Minerai"; break;

                    default: break;
                }
            }
            else if (langue == 1)
            {
                switch (e.Resource1)
                {
                    case RessourceType.BRICK: ressourceEchange = "Brick"; break;
                    case RessourceType.LUMBER: ressourceEchange = "Lumber"; break;
                    case RessourceType.WHEAT: ressourceEchange = "Wheat"; break;
                    case RessourceType.WOOL: ressourceEchange = "Wool"; break;
                    case RessourceType.ORE: ressourceEchange = "Ore"; break;

                    default: break;
                }
                switch (e.Resource2)
                {
                    case RessourceType.BRICK: ressourcePaye = "Brick"; break;
                    case RessourceType.LUMBER: ressourcePaye = "Lumber"; break;
                    case RessourceType.WHEAT: ressourcePaye = "Wheat"; break;
                    case RessourceType.WOOL: ressourcePaye = "Wool"; break;
                    case RessourceType.ORE: ressourcePaye = "Ore"; break;

                    default: break;
                }
            }

            if (langue == 0)
                text = dataSave.GetComponent<UIData>().players[e.PlayerID].Item1 + " a échangé: " + ressourceEchange + " contre: " + ressourcePaye;
            else if (langue == 1)
                text = dataSave.GetComponent<UIData>().players[e.PlayerID].Item1 + " traded: " + ressourceEchange + " for: " + ressourcePaye;

            SendMessageToChat(text, Message.MessageType.tradeMessage);
            //if (isOnline) NetworkClient.Instance.sendMessage(text);
        }

        fxSource.clip = tradeClip;
        fxSource.loop = false;
        fxSource.Play();
        UpdateConstructionMap();
        SetHarborExchangeValues();

        if (!e.ActionSuccessful)
            SendMessageToChat("Action Failed", Message.MessageType.banditMove);
    }

    public void VictoryHandler(object sender, VictoryInfoArgs e)
    {
        fxSource.clip = victoryClip;

        fxSource.loop = false;
        fxSource.Play();

        victoryScreen.gameObject.SetActive(true);

        if (langue == 0)
            victoryScreen.GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[e.PlayerRankings[0].Item1].Item1 + " a Gagne";
        else if (langue == 1)
            victoryScreen.GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[e.PlayerRankings[0].Item1].Item1 + " Won";
        
        for(int i=0; i<4; i++)
            victoryScreen.GetChild(4).GetChild(i).gameObject.SetActive(false);

        for (int i = 0; i < nbJoueursTotal; i++)
        {
            victoryScreen.GetChild(4).GetChild(i).gameObject.SetActive(true);

            if (dataSave.GetComponent<UIData>().players[e.PlayerRankings[i].Item1].Item3 == false)
            {
                if (dataSave.GetComponent<UIData>().players[e.PlayerRankings[i].Item1].Item2 == 0)
                    victoryScreen.GetChild(4).GetChild(i).GetChild(2).GetComponent<Image>().sprite = defaultAvatar0;
                else if (dataSave.GetComponent<UIData>().players[e.PlayerRankings[i].Item1].Item2 == 1)
                    victoryScreen.GetChild(4).GetChild(i).GetChild(2).GetComponent<Image>().sprite = defaultAvatar1;
                else if (dataSave.GetComponent<UIData>().players[e.PlayerRankings[i].Item1].Item2 == 2)
                    victoryScreen.GetChild(4).GetChild(i).GetChild(2).GetComponent<Image>().sprite = defaultAvatar2;
                else
                    victoryScreen.GetChild(4).GetChild(i).GetChild(2).GetComponent<Image>().sprite = defaultAvatar3;
            }
            else
                victoryScreen.GetChild(4).GetChild(i).GetChild(2).GetComponent<Image>().sprite = robotAvatar;


            victoryScreen.GetChild(4).GetChild(i).GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = dataSave.GetComponent<UIData>().players[e.PlayerRankings[i].Item1].Item1;
            victoryScreen.GetChild(4).GetChild(i).GetChild(3).GetComponentInChildren<TextMeshProUGUI>().colorGradientPreset = PlayersColors[e.PlayerRankings[i].Item1];

            victoryScreen.GetChild(4).GetChild(i).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = e.PlayerRankings[i].Item2.ToString();
        }
    }

    public void KnightCardUseHandler(object sender, BanditMoveEventArgs e)
    {
        string text = "";
        if (langue == 0)
            text = dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item1 + " a utilisé: Carte chevalier";
        else if (langue == 1)
            text = dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item1 + " used: Knight card";

        SendMessageToChat(text, Message.MessageType.devCardInfo);
        UpdateConstructionMap();
    }

    public void MonopolyCardUseHandler(object sender, MonopolyCardUseEventArgs e)
    {
        string text = "";
        if (langue == 0)
            text = dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item1 + " a utilisé: Carte monopole";
        else if (langue == 1)
            text = dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item1 + " used: Monopoly card";

        SendMessageToChat(text, Message.MessageType.devCardInfo);
        UpdateConstructionMap();
    }

    public void ResourcePairCardUseHandler(object sender, ResourcePairCardUseEventArgs e)
    {
        string text = "";
        if (langue == 0)
            text = dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item1 + " a utilisé: Carte paire de ressources";
        else if (langue == 1)
            text = dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item1 + " used: Resource pair card";

        SendMessageToChat(text, Message.MessageType.devCardInfo);
        UpdateConstructionMap();
    }

    public void RoadConstructionCardUseHandler(object sender, RoadConstructionCardUseEventArgs e)
    {
        string text = "";
        if (langue == 0)
            text = dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item1 + " a utilisé: Carte construction de routes";
        else if (langue == 1)
            text = dataSave.GetComponent<UIData>().players[currentGame.CurrentPlayer].Item1 + " used: Road construction card";

        SendMessageToChat(text, Message.MessageType.devCardInfo);
        UpdateConstructionMap();
    }

    public void KillLocalGame()
    {
        GameDeleteEventArgs e = new GameDeleteEventArgs(currentGame.Id, isOnline);
        if (isOnline)
        {
            e.GameId = Network.View.NetworkView.Instance.getIdGame();
            DisconnectEventArgs disco = new DisconnectEventArgs(Network.View.NetworkView.Instance.getIdGame(), Network.View.NetworkView.Instance.getIdInGame());
            Network.View.NetworkView.Instance.OnDisconnectEvent(disco);
        }
        else GameView.Instance.OnGameDelete(e);
        GameView.Instance.UnregisterControllerHandlers();
        UninstantiateEvents();
        //AIManager.Instance.UnregisterEventHandlers();
        aiManager.UnregisterEventHandlers();

        Destroy(dataSave.gameObject);
    }

    public void DisplayRessourcesInfo(int disp)
    {
        for (int i = 0; i < 4; i++)
        {
            requiredRessourcesDisplay.GetChild(i).gameObject.SetActive(false);
        }
        if (disp != -1)
        {
            if (isOnline)
            {
                requiredRessourcesDisplay.GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[RessourceType.WOOL].ToString() + "/1";
                requiredRessourcesDisplay.GetChild(0).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[RessourceType.WHEAT].ToString() + "/1";
                requiredRessourcesDisplay.GetChild(0).GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[RessourceType.BRICK].ToString() + "/1";
                requiredRessourcesDisplay.GetChild(0).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[RessourceType.LUMBER].ToString() + "/1";

                requiredRessourcesDisplay.GetChild(1).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[RessourceType.WHEAT].ToString() + "/2";
                requiredRessourcesDisplay.GetChild(1).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[RessourceType.ORE].ToString() + "/3";

                requiredRessourcesDisplay.GetChild(2).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[RessourceType.LUMBER].ToString() + "/1";
                requiredRessourcesDisplay.GetChild(2).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[RessourceType.BRICK].ToString() + "/1";

                requiredRessourcesDisplay.GetChild(3).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[RessourceType.WHEAT].ToString() + "/1";
                requiredRessourcesDisplay.GetChild(3).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[RessourceType.WOOL].ToString() + "/1";
                requiredRessourcesDisplay.GetChild(3).GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1].Ressources[RessourceType.ORE].ToString() + "/1";
            }
            else
            {
                requiredRessourcesDisplay.GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.WOOL].ToString() + "/1";
                requiredRessourcesDisplay.GetChild(0).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.WHEAT].ToString() + "/1";
                requiredRessourcesDisplay.GetChild(0).GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.BRICK].ToString() + "/1";
                requiredRessourcesDisplay.GetChild(0).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.LUMBER].ToString() + "/1";

                requiredRessourcesDisplay.GetChild(1).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.WHEAT].ToString() + "/2";
                requiredRessourcesDisplay.GetChild(1).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.ORE].ToString() + "/3";

                requiredRessourcesDisplay.GetChild(2).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.LUMBER].ToString() + "/1";
                requiredRessourcesDisplay.GetChild(2).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.BRICK].ToString() + "/1";

                requiredRessourcesDisplay.GetChild(3).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.WHEAT].ToString() + "/1";
                requiredRessourcesDisplay.GetChild(3).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.WOOL].ToString() + "/1";
                requiredRessourcesDisplay.GetChild(3).GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = currentGame.Players[currentGame.CurrentPlayer].Ressources[RessourceType.ORE].ToString() + "/1";
            }

            requiredRessourcesDisplay.GetChild(disp).gameObject.SetActive(true);               
        }
    }

    public void SendMessageToChat(string text, Message.MessageType messageType)
    {
        if (messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }

        Message newMessage = new Message();
        newMessage.text = text;

        GameObject newText = Instantiate(textObject, chat.transform);

        newMessage.textObject = newText.GetComponent<TextMeshProUGUI>();
        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = MessageTypeColor(messageType);

        messageList.Add(newMessage);

        Canvas.ForceUpdateCanvases();
        chatScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }

    private Color MessageTypeColor(Message.MessageType messageType)
    {
        Color color = playerMessage;

        switch (messageType)
        {
            case Message.MessageType.playerMessage:
                color = playerMessage;
                break;
            case Message.MessageType.constructionMessage:
                color = constructionMessage;
                break;
            case Message.MessageType.tradeMessage:
                color = tradeMessage;
                break;
            case Message.MessageType.AIMessage:
                color = AIMessage;
                break;
            case Message.MessageType.banditMove:
                color = banditMove;
                break;
            case Message.MessageType.generalMessage:
                color = generalMessage;
                break;
            case Message.MessageType.diceInfo:
                color = diceInfo;
                break;
            case Message.MessageType.devCardInfo:
                color = devCardInfo;
                break;
        }
        return color;
    }

    private void PopTitle(string text)
    {
        animatedTitle.parent.gameObject.SetActive(true);
        animatedTitle.GetComponentInChildren<TextMeshProUGUI>().text = text;
        animatedTitle.GetComponent<TitleAnimation>().PopTitle();
    }

    public void PopTitleFinished()
    {
        animatedTitle.parent.gameObject.SetActive(false);
    }

    public void LeaveOnlineLobby()
    {
        needToRefresh = true;
        QuitLobby e = new QuitLobby(Network.View.NetworkView.Instance.getIdGame(), Network.View.NetworkView.Instance.getIdInGame());
        Network.View.NetworkView.Instance.OnQuitLobby(e);
    }


    /************/

    public void OnUpdateInfoHandler(object sender, UpdateEvent e)
    {
        if (needToRefresh)
        {
            needToRefresh = false;
            menuTransition.GetComponent<MenuTransition>().FadeMenuTo(6);
        }
        /*
        if (Network.View.NetworkView.Instance.GetFirst())
        {
            //Mettre sur menu d'attente
            menuTransition.GetComponent<MenuTransition>().FadeMenuTo(6);
        }
        */

        nbJoueursConnecte = e.game.nbConnected;
        nbJoueursPret = e.game.nbReady;
        infoPlayers.Clear();
        for (int i = 0; i < Network.View.NetworkView.Instance.getInfoPlayers().Count; i++)
        {
            infoPlayers.Add((Network.View.NetworkView.Instance.getAvatar()[i].Item2, Network.View.NetworkView.Instance.getInfoPlayers()[i].Item2));
        }
        if (!gotID)
        {
            playerNumber = Network.View.NetworkView.Instance.getIdInGame() + 1;
            gotID = true;
        }

        int avatar = 0;
        if (changeProfile.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<RadioButton>().isChecked)
            avatar = 0;
        if (changeProfile.transform.GetChild(1).GetChild(3).GetChild(1).GetComponent<RadioButton>().isChecked)
            avatar = 1;
        if (changeProfile.transform.GetChild(1).GetChild(3).GetChild(2).GetComponent<RadioButton>().isChecked)
            avatar = 2;
        if (changeProfile.transform.GetChild(1).GetChild(3).GetChild(3).GetComponent<RadioButton>().isChecked)
            avatar = 3;
        dataSave.GetComponent<UIData>().selfInfo = (lobbyReseau.GetChild(11).GetComponentInChildren<TextMeshProUGUI>().text, avatar, playerNumber);
        Debug.Log("playerNumber:" + playerNumber);
        UpdateOnlineLobby();
    }

    public void OnStartHandler(object sender, StartedGame e)
    {
        // Afficher GUI
        gameGrid = GameView.Instance.Game.GameGrid;
        GenerateGameDataReseau();
        sceneTransition.GetComponent<SceneTransition>().StartGameLocal();
    }

    public void OnReadyPlayerHandler(object sender, ReadyEvent e)
    {

        // Afficher GUI
        /*
        nbJoueursPret++;
        UpdateOnlineLobby();
        */
    }

    public void OnMessageHandler(object sender, MessageEvent e)
    {
        // Afficher GUI
        Debug.Log("UI controller MSG reçu: " + e.Message);
        SendMessageToChat(e.Message, Message.MessageType.playerMessage);
    }

    public void OnErrorHandler(object sender, ErrorEventArgs e)
    {
        Debug.Log("Error xd:" + e.Type);
        // Afficher GUI
        if(e.Type == ErrorType.PASSWORD)
            wrongPasswordMenu.gameObject.SetActive(true);
        else
        {
            menuTransition.GetComponent<MenuTransition>().FadeMenuTo(0);
            errorMenu.gameObject.SetActive(true);
        }
    }

    public void OnExchangeRecievedHandler(object sender, ExchangeEvent e)
    {
        List<(RessourceType, int)> gifted = new List<(RessourceType, int)>();
        Serialization.Deserialize(e.GifteRessources, gifted);
        int senderId = e.PlayerId;

        int menuIndex = 0;

        if (currentGame.CurrentPlayer == (dataSave.GetComponent<UIData>().selfInfo.Item3 - 1))
            menuIndex = 7;
        else
            menuIndex = 8;

        //self elimine
        if(senderId != (dataSave.GetComponent<UIData>().selfInfo.Item3 - 1))
        {
            for (int j = 0; j < 5; j++)
                offeredRessources[senderId][j] = gifted[j].Item2;

            if(menuIndex == 7)
            {
                //self => exchangeMenu.GetChild(menuIndex).GetChild(1) elimine
                //rest 2 3 et 4 mis dans l'ordre d'apparition
                if(senderId < (dataSave.GetComponent<UIData>().selfInfo.Item3 - 1))
                {
                    //+2
                    for (int j = 0; j < 5; j++)
                        exchangeMenu.GetChild(menuIndex).GetChild(senderId + 2).GetChild(1).GetChild(j).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = offeredRessources[senderId][j].ToString();
                }
                else
                {
                    //+1
                    for (int j = 0; j < 5; j++)
                        exchangeMenu.GetChild(menuIndex).GetChild(senderId + 1).GetChild(1).GetChild(j).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = offeredRessources[senderId][j].ToString();
                }
            }
            else
            {
                //self => exchangeMenu.GetChild(menuIndex).GetChild(1) elimine
                //joueur courant => exchangeMenu.GetChild(menuIndex).GetChild(3)
                if (senderId == currentGame.CurrentPlayer)
                {
                    for (int j = 0; j < 5; j++)
                        exchangeMenu.GetChild(menuIndex).GetChild(3).GetChild(1).GetChild(j).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = offeredRessources[senderId][j].ToString();
                }
                //reste 2 et 4 mis dans l'ordre d'apparition
                else
                {
                    int courant = currentGame.CurrentPlayer;
                    int soiMeme = dataSave.GetComponent<UIData>().selfInfo.Item3 - 1;

                    bool[] tab = new bool[4];
                    for (int i = 0; i < 4; i++)
                        tab[i] = true;

                    tab[courant] = false;
                    tab[soiMeme] = false;

                    int theFirst = 0;

                    for(int i=0; i<4; i++)
                    {
                        if(tab[i])
                        {
                            theFirst = i;
                            break;
                        }
                    }
                    //2
                    if(senderId == theFirst)
                    {
                        for (int j = 0; j < 5; j++)
                            exchangeMenu.GetChild(menuIndex).GetChild(2).GetChild(1).GetChild(j).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = offeredRessources[senderId][j].ToString();
                    }
                    //4
                    else
                    {
                        for (int j = 0; j < 5; j++)
                            exchangeMenu.GetChild(menuIndex).GetChild(4).GetChild(1).GetChild(j).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = offeredRessources[senderId][j].ToString();
                    }
                }
            }
        }
    }

    public void OnExchangeAcceptedHandler(object sender, AcceptExchange e)
    {
        // Afficher GUI
        CancelValidation();
        SetOnlineExchangeMenu();

        fxSource.clip = tradeClip;
        fxSource.loop = false;
        fxSource.Play();
    }
    public void OnBeginReseauHandler(object sender, AcceptExchange e)
    {
        // Afficher GUI

    }

    public void SendExchangeOffer()
    {
        for (int i = 0; i < 5; i++)
            Debug.Log("JE VAIS ENVOYER RESSOURCE N" + i + " DE VALEUR:" + offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][i]);

        //envoie offre d'échange à tout les autres joueurs
        //offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3-1]; << le truc a envoyer
        //dataSave.GetComponent<UIData>().selfInfo.Item3-1; << son id de partie
        (RessourceType, int)[] send = new (RessourceType, int)[5];
        send[0] = (RessourceType.BRICK, offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][0]);
        send[1] = (RessourceType.LUMBER, offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][1]);
        send[2] = (RessourceType.WHEAT, offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][2]);
        send[3] = (RessourceType.WOOL, offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][3]);
        send[4] = (RessourceType.ORE, offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][4]);

        List<(RessourceType, int)> listSend = new List<(RessourceType, int)>();
        for (int i = 0; i < 5; i++)
        {
            listSend.Add(send[i]);
        }

        ExchangeEvent e = new ExchangeEvent(Serialization.Serialize(listSend), Network.View.NetworkView.Instance.getIdGame(), Network.View.NetworkView.Instance.getIdInGame());
        Network.View.NetworkView.Instance.OnPlayerExchange(e);
    }

    public void SendExchange()
    {
        //execute l'échange pour tout les joueurs
        //playerToExchangeToInventory << l'id de la personne qui fait l'échange avec le joueur courant
        //offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1] << ce que le joueur courant propose
        //offeredRessources[playerToExchangeToInventory]; << ce que l'autre joueur propose

        (RessourceType, int)[] send = new (RessourceType, int)[5];
        (RessourceType, int)[] receive = new (RessourceType, int)[5];

        send[0] = (RessourceType.BRICK, offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][0]);
        send[1] = (RessourceType.LUMBER, offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][1]);
        send[2] = (RessourceType.WHEAT, offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][2]);
        send[3] = (RessourceType.WOOL, offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][3]);
        send[4] = (RessourceType.ORE, offeredRessources[dataSave.GetComponent<UIData>().selfInfo.Item3 - 1][4]);

        receive[0] = (RessourceType.BRICK, offeredRessources[playerToExchangeToInventory][0]);
        receive[1] = (RessourceType.LUMBER, offeredRessources[playerToExchangeToInventory][1]);
        receive[2] = (RessourceType.WHEAT, offeredRessources[playerToExchangeToInventory][2]);
        receive[3] = (RessourceType.WOOL, offeredRessources[playerToExchangeToInventory][3]);
        receive[4] = (RessourceType.ORE, offeredRessources[playerToExchangeToInventory][4]);

        List<(RessourceType, int)> listSend = new List<(RessourceType, int)>();
        List<(RessourceType, int)> listReceive = new List<(RessourceType, int)>();
        for (int i = 0; i < 5; i++)
        {
            listSend.Add(send[i]);
            listReceive.Add(receive[i]);
        }

        AcceptExchange acc = new AcceptExchange(Serialization.Serialize(listSend), playerToExchangeTo, Serialization.Serialize(listReceive), Network.View.NetworkView.Instance.getIdGame());
        Network.View.NetworkView.Instance.OnAcceptExchange(acc);

        CancelValidation();
        SetOnlineExchangeMenu();

        fxSource.clip = tradeClip;
        fxSource.loop = false;
        fxSource.Play();
    }

    public void OnTimeOutHandler(object sender, TimeOut e)
    {
        errorTimeout.gameObject.SetActive(true);
    }

    public void PlayerLeft(object sender, GameDeletedArgs e)
    {
        Debug.Log("set active disconnect");
        playerDisconnect.gameObject.SetActive(true);
    }


    public void OnRefreshMenu(object sender, Refresh e)
    {
        SetServerListMenu();
    }


    public void OnLeaveOnlineLobby(object sender, QuitLobby e)
    {
        // retour vers le menu d'accueil
        OnRefreshMenu(sender,new Refresh());
    }
}
[System.Serializable]
public class Message
{
    public string text;
    public TextMeshProUGUI textObject;
    public MessageType messageType;

    public enum MessageType
    {
        playerMessage,
        constructionMessage,
        tradeMessage,
        AIMessage,
        banditMove,
        generalMessage,
        diceInfo,
        devCardInfo
    }
}

[System.Serializable]
public class Serveur
{
    public string serveurName;
    public TextMeshProUGUI textObject;
}
