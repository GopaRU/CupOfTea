using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ButtonControl : MonoBehaviour {

    //тут все взаимодействия с UI. 
    //скрипт висит на гейм менеджере


    public GameObject MainMenu;
    public GameObject InGameMenu;
    public Slider MouseSlide;

    public GameObject WinAnnounce;
    public Text WinnerName;

    public GameObject Counter;
    public Text CounterText;

    float updatetime = 3f;

    public static bool Connected = false;
    bool Showedup = false;

    private void Start()
    {
        string value = PlayerPrefs.GetString("MyName");

        if (value == "")
        {
            PhotonNetwork.NickName = "Безымянный)))";
        } else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("MyName");
        }
        MouseSlide.value = PlayerPrefs.GetFloat("Sensy");
        if (MouseSlide.value == 0)
        {
            MouseSlide.value = 3f;
        }
        NewMouseSensitivity();
    }
    
    public void Update()
    {
        //это я ввел, что бы когда человек подключался видел, сколько человек на серве
        updatetime -= Time.deltaTime;
        if (updatetime < 0)
        {
            CounterText.text = "Игроков на сервере: " + PhotonNetwork.PlayerList.Length;
            updatetime = 3f;
        }


        if (Connected)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Showedup = !Showedup;
                InGameMenu.SetActive(Showedup);
                Cursor.visible = Showedup;
            }
        }
    }

    //вызывается умершим игроком
    public void SetWinner(string name)
    {
        WinnerName.text = name;
    }


    public void ConnectButton()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            RoomOptions newopt = new RoomOptions();
            newopt.MaxPlayers = 10;
            PhotonNetwork.JoinOrCreateRoom("Cup of tea game room", newopt, TypedLobby.Default, null);
            Cursor.visible = false;
            Debug.Log("+");
        }
        else
        {
            Debug.Log("-");
        }
    }

    public void Discon()
    {
        InGameMenu.SetActive(false);
        MainMenu.SetActive(true);
        Showedup = false;
        Connected = false;
    }

    public void ConnectedUI()
    {
        InGameMenu.SetActive(false);
        MainMenu.SetActive(false);
        Showedup = false;
        Connected = true;
    }

    public void NewMouseSensitivity()
    {
        PlayerScr.Sensitivity = MouseSlide.value;
        PlayerPrefs.SetFloat("Sensy", PlayerScr.Sensitivity);
    }

    public void ChangeMyName(Text newname)
    {
        PhotonNetwork.NickName = newname.text;
        PlayerPrefs.SetString("MyName",newname.text);
        PlayerPrefs.Save();
    }
    

    public void DisconnectButton()
    {
        Counter.SetActive(false);
        PhotonNetwork.LeaveRoom();
        InGameMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void ExitButton()
    {
        PlayerPrefs.Save();
        PhotonNetwork.Disconnect();
        Application.Quit();
    }
}
