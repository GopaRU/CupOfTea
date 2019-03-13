using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deathscr : MonoBehaviour {

    void Destroyme()
    {
        Destroy(gameObject);
    }

    void OnEnable()
    {
        ManagerAssist.FinalizeRound += Destroyme;
    }


    void OnDisable()
    {
        ManagerAssist.FinalizeRound -= Destroyme;
    }
}
