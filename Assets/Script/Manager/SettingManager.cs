using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance
    {
        get;
        private set;
    }

    public ToggleGroup levelToggleGroup;
    public string level;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Button saveBtn;
    public Button closeBtn;

    public GameObject bg;

    private string filePath;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        filePath = Application.persistentDataPath + "/puzzleSettings.json";
        saveBtn.onClick.AddListener(SaveSettings);
        closeBtn.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void OnOffPopup(bool isOn)
    {
        bg.SetActive(isOn);
    }

    private void SaveSettings()
    {
        Toggle selectedLeveltoggle = levelToggleGroup.ActiveToggles().FirstOrDefault();

        if (selectedLeveltoggle == null)
        {
            return;
        }

        Level selectedLevel = (Level)System.Enum.Parse(typeof(Level), selectedLeveltoggle.name);

        PuzzleSettings puzzlesettings = new PuzzleSettings()
        {
            level = selectedLevel,
            bgmSound = bgmSlider.value,
            sfxSound = sfxSlider.value,
        };

        SoundManager.Instance.SetBGMVolume(bgmSlider.value);
        SoundManager.Instance.SetClickVolume(sfxSlider.value);

        string settingsjson = JsonUtility.ToJson(puzzlesettings, true);
        File.WriteAllText(filePath, settingsjson);
        OnOffPopup(false);
    }

    public void LoadSettings()
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        string  loadedJson = File.ReadAllText(filePath);
        PuzzleSettings settings = JsonUtility.FromJson<PuzzleSettings>(loadedJson);

        level = settings.level.ToString();

        SoundManager.Instance.SetBGMVolume(settings.bgmSound);
        SoundManager.Instance.SetClickVolume(settings.sfxSound);

        bgmSlider.value = settings.bgmSound;
        sfxSlider.value = settings.sfxSound;
    }

    public void SetSettingValues()
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        string loadedJson = File.ReadAllText(filePath);
        PuzzleSettings settings = JsonUtility.FromJson<PuzzleSettings>(loadedJson);

        SoundManager.Instance.SetBGMVolume(settings.bgmSound);
        SoundManager.Instance.SetClickVolume(settings.sfxSound);
    }
}
