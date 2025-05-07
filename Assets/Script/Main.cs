using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    [SerializeField]
    private Button startBtn;

    [SerializeField]
    private Button settingBtn;

    void Start()
    {
        startBtn.onClick.AddListener(GameStart);
        settingBtn.onClick.AddListener(OpenSettingPopup);
    }

    void GameStart()
    {
        SceneManager.LoadScene("Game");
    }

    void OpenSettingPopup()
    {

    }
}
