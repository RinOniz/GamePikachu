using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Menu : MonoBehaviour
{
    [SerializeField] public Button startButton;
    [SerializeField] public Button openModeSelectButton;
    [SerializeField] public Button howToPlayButton;
    [SerializeField] public Button closeButton;

    [SerializeField] public GameObject modeSelectPanel;
    [SerializeField] public GameObject howToPlayPanel;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip startGameSound;

    private void Start()
    {
        startButton.onClick.AddListener(QuickPlayClassic);
        openModeSelectButton.onClick.AddListener(OpenModeSelect);
        howToPlayButton.onClick.AddListener(OpenHowToPlayPanel);
        closeButton.onClick.AddListener(CloseGame);
    }

    private void QuickPlayClassic()
    {
        PlaySound();

        PlayerPrefs.SetInt("GameMode", 0);
        PlayerPrefs.SetInt("GameLevel", 1);

        SceneManager.LoadScene("GameScene");
    }

    private void OpenModeSelect()
    {
        modeSelectPanel.GetComponent<ModeSelectPanel>().Show();
    }

    private void OpenHowToPlayPanel()
    {
        howToPlayPanel.GetComponent<HowToPlayPanel>().Show();
    }

    private void CloseGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void PlaySound()
    {
        audioSource.PlayOneShot(startGameSound);
    }
}
