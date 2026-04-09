using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] public GameObject pointPrefab;

    private GameObject[,] paths;

    private const int totalCols = 18;
    private const int totalRows = 10;

    private float startX = -9.2f;
    private float startY = 4.5f;
    private float timer = 0.0f;

    public void Start()
    {
        paths = new GameObject[totalRows, totalCols];

        Spawn();
    }

    private void Spawn()
    {
        for (int row = 0; row < totalRows; row++)
        {
            for (int col = 0; col < totalCols; col++)
            {
                Vector3 position = new Vector3(startX + col, startY - row, 0.0f);
                GameObject point = Instantiate(pointPrefab, position, Quaternion.identity) as GameObject;
                point.name = "Point[" + row + ", " + col + "]";

                SpriteRenderer spriteRender = point.GetComponent<SpriteRenderer>();
                spriteRender.enabled = false;
                spriteRender.sortingOrder = 3;

                point.transform.SetParent(gameObject.transform);

                paths[row, col] = point;
            }
        }
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            timer = timer > 0 ? timer : 0.0f;

            if (timer == 0.0f)
            {
                for (int row = 0; row < totalRows; row++)
                {
                    for (int col = 0; col < totalCols; col++)
                    {
                        GameObject point = paths[row, col];
                        SpriteRenderer spriteRenderer = point.GetComponent<SpriteRenderer>();

                        if (spriteRenderer.enabled)
                        {
                            spriteRenderer.enabled = false;
                        }
                    }
                }
            }
        }
    }

    public void DrawPath(List<Position> positions)
    {
        foreach (Position position in positions)
        {
            int row = position.row;
            int col = position.col;

            GameObject point = paths[row, col];
            SpriteRenderer spriteRenderer = point.GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = true;
        }

        timer = 0.7f;
    }
}
