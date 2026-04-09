using System.Collections;
using UnityEngine;

public class AIBot : MonoBehaviour
{
    private Board board;
    private GameController gameController;

    private bool isBotActive = false;

    public AIStatsHUD aiHUD;

    public float moveDelay = 2.0f;
    public float pathDisplayTime = 0.5f;

    private void Start()
    {
        board = GetComponent<Board>();
        gameController = GetComponent<GameController>();

        int mode = PlayerPrefs.GetInt("GameMode", 0);

        if (mode == 2)
        {
            isBotActive = true;

            StartCoroutine(AutoPlay());
        }
    }

    IEnumerator AutoPlay()
    {
        yield return new WaitForSeconds(1.5f);

        while (isBotActive)
        {
            yield return new WaitUntil(() => gameController.IsPause() == false && gameController.IsGameOver() == false);

            if (aiHUD != null)
            {
                aiHUD.UpdateStatus("Scanning");
            }

            yield return new WaitForSeconds(0.8f);

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            Position[] pair = board.FindValidPairs();

            watch.Stop();

            float timeMs = (float)watch.Elapsed.TotalMilliseconds;

            int nodes = board.currNodesExpanded;

            if (pair != null)
            {
                board.totalNodesBot += nodes;
                board.totalTimeBot += timeMs;
                board.botMoveCount++;

                if (aiHUD != null) {
                    aiHUD.UpdateMetrics(nodes, timeMs, "Found");
                } 

                Position pos1 = pair[0];
                Position pos2 = pair[1];

                GameObject obj1 = board.GetTileObject(pos1.row, pos1.col);
                GameObject obj2 = board.GetTileObject(pos2.row, pos2.col);

                gameController.SelectFirstTile(obj1);
                gameController.SelectSecondTile(obj2);

                yield return new WaitForSeconds(moveDelay);
            }
            else
            {
                if (aiHUD != null) {
                    aiHUD.UpdateMetrics(nodes, timeMs, "Shuffling");
                }

                Debug.Log("Het duong, tu dong Change");

                yield return new WaitForSeconds(1.5f);
            }
        }
    }
}
