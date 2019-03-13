using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    
    public float AttackPower;

    void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerScr>().SetPush(transform.forward * AttackPower);
        Debug.Log("ХУЯК");
    }
}
