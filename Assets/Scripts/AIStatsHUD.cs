using UnityEngine;
using TMPro;

public class AIStatsHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI algorithmText;
    [SerializeField] private TextMeshProUGUI nodesText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI statusText;

    private void Start()
    {
        if (PlayerPrefs.GetInt("GameMode", 0) != 2)
        {
            gameObject.SetActive(false);
        }
        else
        {
            string currAlgorithm = PlayerPrefs.GetString("AI_Algorithm", "BFS");

            if (algorithmText != null)
            {
                algorithmText.text = "   ALGORITHM: " + currAlgorithm;
            }

            UpdateStatus("...");
        }
    }

    public void UpdateMetrics(int nodes, float timeMs, string status)
    {
        if (nodesText != null)
        {
            nodesText.text = "NODES EXPANDED: " + nodes;
        }

        if (timeText != null)
        {
            timeText.text = "PROCESSING TIME: " + timeMs.ToString("F2") + "ms";
        }

        UpdateStatus(status);
    }

    public void UpdateStatus(string status)
    {
        if (statusText != null)
        {
            statusText.text = "STATUS: " + status;
        }
    }
}