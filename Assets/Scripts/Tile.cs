using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject tileBackground;
    public int row, col, id;

    public void OnMouseDown()
    {
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();

        if (gameController.IsPause() || gameController.IsGameOver())
        {
            return;
        }

        SpriteRenderer itemBackgroundSpriteRender = tileBackground.GetComponent<SpriteRenderer>();

        if (gameController.IsSelected())
        {
            if (gameController.GetFirstTile() != gameObject)
            {
                itemBackgroundSpriteRender.color = new Color(234f / 255f, 150f / 255f, 150f / 255f, 1.0f);
                gameController.SelectSecondTile(gameObject);
            }
        }
        else
        {
            itemBackgroundSpriteRender.color = new Color(234f / 255f, 150f / 255f, 150f / 255f, 1.0f);
            gameController.SelectFirstTile(gameObject);
        }
    }
}
