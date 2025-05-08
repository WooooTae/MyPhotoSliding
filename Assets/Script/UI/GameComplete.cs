using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameComplete : MonoBehaviour
{
    [SerializeField]
    private Button retryBtn;

    [SerializeField]
    private Button mainBtn;

    public void OpenPopup()
    {
        gameObject.SetActive(true);
    }

    void Start()
    {
        retryBtn.onClick.AddListener(RetryGame);
        mainBtn.onClick.AddListener(() => SceneManager.LoadScene("Main"));
    }

    void RetryGame()
    {
        GameManager.Instance.Restart();
    }
}
