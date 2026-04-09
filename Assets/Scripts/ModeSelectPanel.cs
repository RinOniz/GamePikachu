using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModeSelectPanel : MonoBehaviour
{
    [SerializeField] public Button closePanelButton;
    [SerializeField] public GameObject panel;

    [SerializeField] public Button classicModeButton;
    [SerializeField] public Button funnyModeButton;
    [SerializeField] public Button botModeButton;

    [SerializeField] private LevelSelectPanel levelSelectPanel;
    [SerializeField] private BotSelectPanel botSelectPanel;

    [SerializeField] private Menu menuScript;

    private void Start()
    {
        closePanelButton.onClick.AddListener(Hide);
        classicModeButton.onClick.AddListener(SelectClassicMode);
        funnyModeButton.onClick.AddListener(SelectFunnyMode);
        botModeButton.onClick.AddListener(SelectBotMode);
    }

    public void Show()
    {
        panel.SetActive(true);
    }

    private void Hide()
    {
        panel.SetActive(false);
    }

    private void SelectClassicMode()
    {
        PlayerPrefs.SetInt("GameMode", 0);

        Hide();

        levelSelectPanel.Show();
    }

    private void SelectFunnyMode()
    {
        menuScript.PlaySound();

        SceneManager.LoadScene("GameScene");
        PlayerPrefs.SetInt("GameMode", 1);
    }

    private void SelectBotMode()
    {
        Hide();

        botSelectPanel.Show();
    }
}
