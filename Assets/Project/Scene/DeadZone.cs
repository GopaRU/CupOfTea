using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class DeadZone : MonoBehaviour {

    public void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            other.gameObject.GetComponent<PlayerScr>().RPC("KillMe",RpcTarget.All);
        }
    }

}
