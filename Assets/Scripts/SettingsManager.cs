using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider mouseSlider;     
    public GameObject settingsPanel;

    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (mouseSlider != null)
        {
            mouseSlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        }
    }

    public void OnApplyButtonClicked()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);

        if (mouseSlider != null) 
            PlayerPrefs.SetFloat("MouseSensitivity", mouseSlider.value);

        PlayerPrefs.Save();

        ApplySettingsToGame();

        settingsPanel.SetActive(false);
    }

    private void ApplySettingsToGame()
    {
        if (BGMManager.instance != null)
        {
            BGMManager.instance.UpdateMusicVolume();
        }
    }
}