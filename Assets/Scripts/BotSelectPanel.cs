using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BotSelectPanel : MonoBehaviour
{
    [SerializeField] private Button bfsButton;
    [SerializeField] private Button dfsButton;
    [SerializeField] private Button aStarButton;
    [SerializeField] private Button backButton;

    [SerializeField] private ModeSelectPanel modeSelectPanelScript;

    [SerializeField] private Menu menuScript;

    private void Start()
    {
        bfsButton.onClick.AddListener(() => LoadBotGame("BFS"));
        dfsButton.onClick.AddListener(() => LoadBotGame("DFS"));
        aStarButton.onClick.AddListener(() => LoadBotGame("ASTAR"));

        backButton.onClick.AddListener(GoBackToModeSelect);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void LoadBotGame(string algorithmName)
    {
        menuScript.PlaySound();

        PlayerPrefs.SetInt("GameLevel", 1);

        PlayerPrefs.SetInt("GameMode", 2);

        PlayerPrefs.SetString("AI_Algorithm", algorithmName);

        SceneManager.LoadScene("GameScene");
    }

    private void GoBackToModeSelect()
    {
        Hide();

        modeSelectPanelScript.Show(); 
    }
}