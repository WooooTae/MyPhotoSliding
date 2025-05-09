using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class Main : MonoBehaviour
{
    [SerializeField]
    private Button startBtn;

    [SerializeField]
    private Button settingBtn;

    [SerializeField]
    private SettingManager settingManager;

    [SerializeField]
    private Transform titleText;

    void Start()
    {
        startBtn.onClick.AddListener(GameStart);
        settingBtn.onClick.AddListener(OpenSettingPopup);

        AnimationTitle();
    }

    void GameStart()
    {
        SoundManager.Instance.PlayBGM();
        SceneManager.LoadScene("Game");
    }


    void OpenSettingPopup()
    {
        SoundManager.Instance.PlayClickSound();
        settingManager.gameObject.SetActive(true);
    }

    void AnimationTitle()
    {
        titleText.DOScale(1.2f, 1.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
