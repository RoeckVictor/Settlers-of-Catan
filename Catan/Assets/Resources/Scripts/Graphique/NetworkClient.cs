using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkView = Network.View.NetworkView;
using Util.View;
using System;
using Noyau.View;

public class NetworkClient : MonoBehaviour
{
    public static NetworkClient Instance;

    public bool isOnline { get; set; } = false;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        NetworkView.Instance.RegisterControllerHandlers();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void connectToNetwork()
    {
        ConnectEventArgs e = new ConnectEventArgs();
        NetworkView.Instance.OnConnectEvent(e);
        isOnline = true;
    }

    public void disconnect()
    {
        DisconnectEventArgs e = new DisconnectEventArgs(NetworkView.Instance.getIdGame(), NetworkView.Instance.getIdInGame());
        NetworkView.Instance.OnDisconnectEvent(e);
    }

    public void RandomConnect(string name, int avatar)
    {
        Guid id = new Guid();
        GameJoinEventArgs e = new GameJoinEventArgs(1, "", id, false, avatar, name);
        NetworkView.Instance.OnJoinEvent(e);
    }

    public void createServer(int total, int ia, IHexGrid grid, string name, bool access, string pwd, Guid id, int avatar, string playerName)
    {
        GameCreateEventArgs e = new GameCreateEventArgs(total, ia, grid, true, name, access, pwd, id, avatar, playerName);
        Debug.Log("Je suis dans le NetworkClient");
        NetworkView.Instance.OnGameCreate(e);
    }

    public void sendMessage(string text)
    {
        MessageEvent msg = new MessageEvent(text, NetworkView.Instance.getIdInGame(), NetworkView.Instance.getIdGame());
        NetworkView.Instance.OnMessageEvent(msg);
    }

    public void JoinGame(Guid id, bool access, string pwd, string name, int avatar)
    {
        GameJoinEventArgs e = new GameJoinEventArgs(0, pwd, id, access, avatar, name);
        NetworkView.Instance.OnJoinEvent(e);
    }

    public void GameCreate(int nbJoueursTotal, int nbOrdis, IHexGrid gameGrid, int avatar, string playerName)
    {
        GameCreateEventArgs e = new GameCreateEventArgs(nbJoueursTotal, nbOrdis, gameGrid, false, "", true, "", Guid.Empty, avatar, playerName);
        NetworkView.Instance.OnGameCreate(e);
        //GameView.Instance.OnGameCreate(e);
    }

    public void Ready()
    {
        ReadyEvent ready = new ReadyEvent(true, NetworkView.Instance.getIdGame(), NetworkView.Instance.getIdInGame());
        NetworkView.Instance.OnReadyEvent(ready);
    }

    public void StartGame()
    {
        StartedGame e = new StartedGame(NetworkView.Instance.getIdGame(), NetworkView.Instance.getIdInGame());
        NetworkView.Instance.OnStartEvent(e);
    }




}
