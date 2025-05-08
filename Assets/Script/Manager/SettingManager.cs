using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;

public class SettingManager : MonoBehaviour
{
    public ToggleGroup levelToggleGroup;
    public ToggleGroup soundToggleGroup;
    public Button saveBtn;
    public Button closeBtn;

    private string filePath;

    void Start()
    {
        filePath = Application.persistentDataPath + "/puzzleSettings.json";
        saveBtn.onClick.AddListener(SaveSettings);
        closeBtn.onClick.AddListener(() => gameObject.SetActive(false));

        LoadSettings();
    }

    private void SaveSettings()
    {
        Toggle selectedLeveltoggle = levelToggleGroup.ActiveToggles().FirstOrDefault();
        Toggle seledcedSoundtoggle = soundToggleGroup.ActiveToggles().FirstOrDefault();

        if (selectedLeveltoggle == null || seledcedSoundtoggle == null)
        {
            return;
        }

        Level selectedLevel = (Level)System.Enum.Parse(typeof(Level), selectedLeveltoggle.name);
        bool isSoundOn = seledcedSoundtoggle.name == "OnToggle";

        PuzzleSettings puzzlesettings = new PuzzleSettings()
        {
            level = selectedLevel,
            isSound = isSoundOn,
        };

        string settingsjson = JsonUtility.ToJson(puzzlesettings, true);
        File.WriteAllText(filePath, settingsjson);
    }

    private void LoadSettings()
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        string  loadedJson = File.ReadAllText(filePath);
        PuzzleSettings settings = JsonUtility.FromJson<PuzzleSettings>(loadedJson);

        foreach (var toggle in levelToggleGroup.GetComponentsInChildren<Toggle>())
        {
            toggle.isOn = toggle.name == settings.level.ToString();
        }

        foreach (var toggle in soundToggleGroup.GetComponentsInChildren<Toggle>())
        {
            if (settings.isSound)
            {
                toggle.isOn = toggle.name == "OnToggle";
            }
            else
            {
                toggle.isOn = toggle.name == "OffToggle";
            }
        }
    }
}
