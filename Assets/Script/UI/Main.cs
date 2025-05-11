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
    private Button selectBtn;

    [SerializeField]
    private SettingManager settingManager;

    [SerializeField]
    private Transform titleText;

    private void Awake()
    {
        SettingManager.Instance.SetSettingValues();
    }

    void Start()
    {
        SoundManager.Instance.PlayBGM(SettingManager.Instance.bgmSlider.value);
        SoundManager.Instance.PlayClickSound(SettingManager.Instance.sfxSlider.value);

        startBtn.onClick.AddListener(GameStart);
        settingBtn.onClick.AddListener(OpenSettingPopup);

        AnimationTitle();
    }

    void GameStart()
    {
        SoundManager.Instance.PlayClickSound(SettingManager.Instance.sfxSlider.value);
        SceneManager.LoadScene("Game");
    }


    void OpenSettingPopup()
    {
        SoundManager.Instance.PlayClickSound(SettingManager.Instance.sfxSlider.value);
        SettingManager.Instance.OnOffPopup(true);
    }

    void AnimationTitle()
    {
        titleText.DOScale(1.2f, 1.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
