using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks{

    //скрипт висит на гейм менеджере

        
    public static Camera ActiveCam;
    public List<GameObject> AllCams;
    public List<GameObject> Players;
    
    ButtonControl Bcontrol;
    PhotonView Assistant;
    public static int PlayersAlive;
    public static int PlayersConnected;
    public static bool ImDead = true;
    public static bool RoundGoingOn = false;
    public static bool Awaiting = true;

    internal bool AbleToConnect { get; set; }
    float timer = 4f;

    void Start () {
        Cursor.lockState = CursorLockMode.Confined;
        ActiveCam = Camera.main;
        Bcontrol = GetComponent<ButtonControl>();
        Assistant = GetComponent<PhotonView>();
        PhotonNetwork.ConnectUsingSettings();
	}
	
    //Механизм раундов, тут все очень плохо, делал в конце, заделывал говном и палками.
	void Update () {

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Is Master");
            if (RoundGoingOn)
            {
                Debug.Log("ROUND GOING     ::" + PlayersAlive + " " + !Awaiting + "::");
                if (PlayersAlive < 2 && !Awaiting)
                {
                    Debug.Log("Players < 2");
                    
                    Assistant.RPC("RpcFinalize",RpcTarget.All);
                }
            } else
            {
                Debug.Log("ROUND ended");
                if (PhotonNetwork.PlayerList.Length > 1)
                {
                    Debug.Log("Connections > 1");
                    timer -= Time.deltaTime;
                    Debug.Log(timer);
                    Debug.Log(Awaiting);
                    if (timer < 0 && Awaiting)
                    {
                        Debug.Log(" TIMER + AWAITING");
                        //awaiting false
                        NewRound();
                        timer = 5f;
                    }
                } 
            }
        }
	}
    
    //тупо для дебага была 
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        AbleToConnect = true;
        Debug.Log("Online");
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log("Ливнул");
        StartCoroutine(DelayedCheck());
    }


    public override void OnJoinedRoom()
    {
        Bcontrol.Counter.SetActive(true);
        Bcontrol.ConnectedUI();
        PlayersAlive = GameObject.FindGameObjectsWithTag("Player").Length;
    }

    //если отпало соединение то выходим в меню
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Bcontrol.Discon();
    }

    //
    private void NewRound()
    {
        int spawnID = 0;
        foreach (Player pl in PhotonNetwork.PlayerList)
        {
            Debug.Log(pl);
            Assistant.RPC("StartNewRound",pl,spawnID);
            Bcontrol.WinAnnounce.SetActive(false);
            spawnID++;
        }
        StartCoroutine(DelayedStart());
        Awaiting = false;
    }

    //костыль. что бы когда первая чашка спавнится, ее не убило условиями конца раунда
    //править в отделе раундов Roundgoingon = false;
    public IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(1f);
        RoundGoingOn = true;
    }

    //ищем всех живыех игроков
    IEnumerator DelayedCheck()
    {
        yield return new WaitForSeconds(1f);
        PlayersAlive = GameObject.FindGameObjectsWithTag("Player").Length;
    }
}
