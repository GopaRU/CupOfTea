using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NameAim : MonoBehaviour {

    public Canvas MyName;
	
	void Update () {
        if (GameManager.ActiveCam != null) {
            MyName.transform.LookAt(GameManager.ActiveCam.transform);
        } else
        {
            GameManager.ActiveCam = Camera.main;
        }
	}
}
