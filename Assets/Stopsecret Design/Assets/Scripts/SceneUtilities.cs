using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUtilities : MonoBehaviour {

    [SerializeField]
    private GameObject helicopter, jet;
    private void Start()
    {
        helicopter.SetActive(true);
        jet.SetActive(false);
    }
    void Update () {
		if(Input.GetKeyDown(KeyCode.R))
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            helicopter.SetActive(true);
            jet.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            helicopter.SetActive(false);
            jet.SetActive(true);
        }
    }
}
