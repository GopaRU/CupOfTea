using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ManagerAssist : PhotonView {

    //здесь происходят все взаимодействия RPC 
    //скрипт висит на гейм менеджере


    public GameObject Spawn;
    private SpriteRenderer[] SpawnPoints;

    ///это ивент об окончании раунда алгоритм следующий
    ///1.на мастер клиенте остался в живых 1 игрок
    ///2.отправляется RPC на клинтские гейм-менеджеры по вызову ивента
    ///3.игрок киляется (+можно будет подписать противников) 
    ///(самое первое и быстрое что пришло мне в голову)

    public delegate void EndRoundAction();
    public static event EndRoundAction FinalizeRound;

    private void Start()
    {
        SpawnPoints = Spawn.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spr in SpawnPoints)
        {
            spr.enabled = false;
        }
    }

    [PunRPC]
    public void StartNewRound(int id)
    {
        GameManager.ImDead = false;
        PhotonNetwork.Instantiate("Player", SpawnPoints[id].transform.position, Quaternion.identity, 0);
    }


    [PunRPC]
    public void RpcFinalize()
    {
        GameManager.RoundGoingOn = false;
        GameManager.Awaiting = true;
        if (FinalizeRound != null)
        {
            FinalizeRound();
        }
    }
}
