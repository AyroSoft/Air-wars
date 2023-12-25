using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BalloonMinigame : MonoBehaviour {
    [SerializeField]
    private GameObject[] balloons;
    [SerializeField]
    private GameObject win;
    [SerializeField]
    private Text txt;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        int balloonsPopped = 0;
        foreach(GameObject balloon in balloons)
        {
            if(balloon == null)
            {
                balloonsPopped++;
            }
        }
        txt.text = balloonsPopped + "/" + balloons.Length + "\n<size=12> Balloons Popped </size>";
        if(balloonsPopped == balloons.Length)
        {
            win.SetActive(true);
        }
	}
}
