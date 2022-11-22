using UnityEngine;
using System.Collections;
using Util.View;
using Noyau.View;

namespace Assets.Resources.Scripts.Graphique
{
    class UIController : MonoBehaviour
    {
        public UIControllerScript theScript;

        public void Start()
        {
            RegisterEventHandlers();
        }

        //public UIController() { }
        public void RegisterEventHandlers()
        {
            UI.Instance.Start += OnStartHandler;
            UI.Instance.Ready += OnReadyPlayerHandler;
            UI.Instance.Message += OnMessageHandler;
            UI.Instance.Error += OnErrorHandler;
            UI.Instance.Exchange += OnExchangeRecievedHandler;
            UI.Instance.Accepted += OnExchangeAcceptedHandler;
            UI.Instance.Update += OnUpdateInfoHandler;
            UI.Instance.StatusReseau += OnBeginReseauHandler;
            UI.Instance.InitialConstructionFirstRound += OnInitialConstructionFirstRoundReseauHandler;
            UI.Instance.InitialConstructionSecondRound += OnInitialConstructionSecondRoundReseauHandler;
            UI.Instance.HarvestPhaseBegin += OnHarvestPhaseBeginReseauHandler;
            UI.Instance.BanditMoveBegin += OnBanditMoveBeginReseauHandler;
            UI.Instance.ConstructionPhaseBegin += OnConstructionPhaseBeginReseauHandler;


            UI.Instance.ConstructionDone += OnConstructionDoneReseauHandler;
            UI.Instance.ExchangeDone += OnExchangeDoneReseauHandler;
            UI.Instance.CardUsageDone += OnCardUsageDoneReseauHandler;

            UI.Instance.ExchangePhaseBegin += OnExchangePhaseBeginHandler;
            UI.Instance.DiscardPhaseBeginReseau += OnDiscardPhaseBeginHandler;
            UI.Instance.VictoryInfoReseau += OnVictoryInfoReseauHandler;

            UI.Instance.Timeout += OnTimeoutHandler;
            UI.Instance.ERefresh += OnRefreshHandler;
            UI.Instance.RetourArriere += OnRetourArriereHandler;
            UI.Instance.Deleted += OnDeleted;
        }



        public void OnTimeoutHandler(object sender, TimeOut e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(Timeout(sender, e));
        }
        public IEnumerator Timeout(object sender, TimeOut e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            theScript.OnTimeOutHandler(sender, e);
            yield return null;
        }

        public void OnDeleted(object sender, GameDeletedArgs e)
        {
            Debug.Log("Test 2");
            UnityMainThreadDispatcher.Instance().Enqueue(Deeleted(sender, e));
        }
        public IEnumerator Deeleted(object sender, GameDeletedArgs e)
        {
            Debug.Log("Test 1");
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            GameView.Instance.OnGameDeleted(e);
            theScript.PlayerLeft(sender,e);
            Debug.Log("Test 3");
            yield return null;
        }



        public void OnRefreshHandler(object sender, Refresh e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(Refresher(sender, e));
        }
        public IEnumerator Refresher(object sender, Refresh e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            theScript.OnRefreshMenu(sender, e);
            yield return null;
        }


        public void OnRetourArriereHandler(object sender, QuitLobby e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(RetourArr(sender, e));
        }
        public IEnumerator RetourArr(object sender, QuitLobby e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            theScript.OnLeaveOnlineLobby(sender, e);
            yield return null;
        }

        public void OnStartHandler(object sender, StartedGame e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(ExecuteStart(sender, e));
        }
        public IEnumerator ExecuteStart(object sender, StartedGame e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            theScript.OnStartHandler(sender, e);
            yield return null;
        }

        public void OnBeginReseauHandler(object sender, GameStatusArgs e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(BeginReseau(sender, e));
        }
        public IEnumerator BeginReseau(object sender, GameStatusArgs e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            GameView.Instance.OnGameBegin(e);
            yield return null;
        }


        public void OnInitialConstructionFirstRoundReseauHandler(object sender, GameStatusArgs e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(InitialConstructionFirstRoundReseau(sender, e));
        }
        public IEnumerator InitialConstructionFirstRoundReseau(object sender, GameStatusArgs e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            GameView.Instance.OnInitialConstructionFirstRound(e);
            yield return null;
        }

        public void OnInitialConstructionSecondRoundReseauHandler(object sender, GameStatusArgs e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(InitialConstructionSecondRoundReseau(sender, e));
        }
        public IEnumerator InitialConstructionSecondRoundReseau(object sender, GameStatusArgs e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            GameView.Instance.OnInitialConstructionSecondRound(e);
            yield return null;
        }

        public void OnHarvestPhaseBeginReseauHandler(object sender, GameStatusArgs e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(HarvestPhaseBeginReseau(sender, e));
        }
        public IEnumerator HarvestPhaseBeginReseau(object sender, GameStatusArgs e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            GameView.Instance.OnHarvestPhaseBegin(e);
            yield return null;
        }

        public void OnBanditMoveBeginReseauHandler(object sender, GameStatusArgs e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(BanditMoveBeginReseau(sender, e));
        }
        public IEnumerator BanditMoveBeginReseau(object sender, GameStatusArgs e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            GameView.Instance.OnBanditMoveBegin(e);
            yield return null;
        }

        public void OnConstructionPhaseBeginReseauHandler(object sender, GameStatusArgs e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(ConstructionPhaseBegin(sender, e));
        }
        public IEnumerator ConstructionPhaseBegin(object sender, GameStatusArgs e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            GameView.Instance.OnConstructionPhaseBegin(e);
            yield return null;
        }

        public void OnConstructionDoneReseauHandler(object sender, ActionDoneInfoArgs e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(ConstructionDone(sender, e));
        }
        public IEnumerator ConstructionDone(object sender, ActionDoneInfoArgs e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            GameView.Instance.OnConstructionDone(e);
            yield return null;
        }

        public void OnExchangeDoneReseauHandler(object sender, ActionDoneInfoArgs e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(ExchangeDone(sender, e));
        }
        public IEnumerator ExchangeDone(object sender, ActionDoneInfoArgs e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            GameView.Instance.OnExchangeDone(e);
            yield return null;
        }

        public void OnCardUsageDoneReseauHandler(object sender, ActionDoneInfoArgs e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(CardUsageDone(sender, e));
        }
        public IEnumerator CardUsageDone(object sender, ActionDoneInfoArgs e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            GameView.Instance.OnCardUsageDone(e);
            yield return null;
        }


        public void OnExchangePhaseBeginHandler(object sender, DiceResultsInfoArgs e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(ExchangePhaseBegin(sender, e));
        }
        public IEnumerator ExchangePhaseBegin(object sender, DiceResultsInfoArgs e)
        {
            GameView.Instance.OnExchangePhaseBegin(e);
            yield return null;
        }

        public void OnDiscardPhaseBeginHandler(object sender, DiceResultsInfoArgs e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(DiscardPhaseBegin(sender, e));
        }
        public IEnumerator DiscardPhaseBegin(object sender, DiceResultsInfoArgs e)
        {
            GameView.Instance.OnDiscardPhaseBegin(e);
            yield return null;
        }


        public void OnVictoryInfoReseauHandler(object sender, VictoryInfoArgs e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(VictoryInfo(sender, e));
        }
        public IEnumerator VictoryInfo(object sender, VictoryInfoArgs e)
        {
            GameView.Instance.OnVictory(e);
            yield return null;
        }

        public void OnUpdateInfoHandler(object sender, UpdateEvent e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(ExecuteUpdateInfo(sender, e));
        }
        public IEnumerator ExecuteUpdateInfo(object sender, UpdateEvent e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            theScript.OnUpdateInfoHandler(sender, e);
            yield return null;
        }

        public void OnReadyPlayerHandler(object sender, ReadyEvent e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(ExecuteReadyPlayer(sender, e));
        }
        public IEnumerator ExecuteReadyPlayer(object sender, ReadyEvent e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            theScript.OnReadyPlayerHandler(sender, e);
            yield return null;
        }

        public void OnMessageHandler(object sender, MessageEvent e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(ExecuteMessage(sender, e));
        }
        public IEnumerator ExecuteMessage(object sender, MessageEvent e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            //theScript.OnMessageHandler(sender, e);
            //theScript.SendMessageToChat(e.Message.ToString(), Message.MessageType.playerMessage);
            theScript.SendMessageToChat(e.Message, Message.MessageType.playerMessage);
            yield return null;
        }

        public void OnErrorHandler(object sender, ErrorEventArgs e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(ExecuteError(sender, e));
        }
        public IEnumerator ExecuteError(object sender, ErrorEventArgs e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            theScript.OnErrorHandler(sender, e);
            yield return null;
        }

        public void OnExchangeRecievedHandler(object sender, ExchangeEvent e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(ExecuteRecieve(sender, e));
        }
        public IEnumerator ExecuteRecieve(object sender, ExchangeEvent e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            theScript.OnExchangeRecievedHandler(sender, e);
            yield return null;
        }

        public void OnExchangeAcceptedHandler(object sender, AcceptExchange e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(ExecuteAccepted(sender, e));
        }
        public IEnumerator ExecuteAccepted(object sender, AcceptExchange e)
        {
            theScript = GameObject.Find("UIController").GetComponent<UIControllerScript>();
            theScript.OnExchangeAcceptedHandler(sender, e);
            yield return null;
        }
    }
}
