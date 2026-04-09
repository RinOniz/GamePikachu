using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;

    [SerializeField] public TextMeshProUGUI resultScoreText;
    [SerializeField] public TextMeshProUGUI resultTimePlayedText;
    [SerializeField] public TextMeshProUGUI playAgainText;

    [SerializeField] public GameObject stars;
    [SerializeField] public GameObject gameOverImage;

    [SerializeField] public Button playAgainButton;
    [SerializeField] public Button resultMenuButton;

    private void Start()
    {
        playAgainButton.onClick.AddListener(OnMainButtonClicked);
        resultMenuButton.onClick.AddListener(ReturnToMenu);
    }

    public void Show(bool winStatus, int score, float playTime, float avgNodes = 0f, float avgTime = 0f)
    {
        resultPanel.SetActive(true);

        stars.SetActive(winStatus);
        gameOverImage.SetActive(!winStatus);

        if (PlayerPrefs.GetInt("GameMode", 0) == 2)
        {
            resultScoreText.text = "AVG NODES: " + avgNodes.ToString("F0");
            resultTimePlayedText.text = "AVG TIME: " + avgTime.ToString("F2") + " ms";
        }
        else
        {
            resultScoreText.text = "Score: " + score;

            float minutes = Mathf.FloorToInt(playTime / 60);
            float seconds = Mathf.FloorToInt(playTime % 60);
            string timeStr = string.Format("{0:00}:{1:00}", minutes, seconds);

            resultTimePlayedText.text = "Time: " + timeStr;
        }

        if (winStatus)
        {
            int currentLevel = PlayerPrefs.GetInt("GameLevel", 1);
            if (currentLevel < 3) playAgainText.text = "NEXT LEVEL";
            else playAgainText.text = "YOU WIN!";
        }
        else
        {
            playAgainText.text = "PLAY AGAIN";
        }
    }

    private void OnMainButtonClicked()
    {
        if (playAgainText.text == "PLAY AGAIN")
        {
            SceneManager.LoadScene("GameScene");
        }
        else if (playAgainText.text == "NEXT LEVEL")
        {
            int currentLevel = PlayerPrefs.GetInt("GameLevel", 1);

            PlayerPrefs.SetInt("GameLevel", currentLevel + 1);

            SceneManager.LoadScene("GameScene");
        }
        else if (playAgainText.text == "YOU WIN!")
        {
            SceneManager.LoadScene("MenuScene");
        }
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
