using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitPanel : MonoBehaviour
{
    [SerializeField] private Button exitBtn;

    private void OnEnable()
    {
        exitBtn.onClick.AddListener(ExitClickHandle);
    }

    private void ExitClickHandle()
    {
        SceneManager.LoadScene("Example");
    }

    private void OnDisable()
    {
        exitBtn.onClick.RemoveListener(ExitClickHandle);
    }
}
