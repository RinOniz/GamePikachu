using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;
    [SerializeField] private AudioClip startGameSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip winSound;

    [SerializeField] public GameObject resultPanel;
    [SerializeField] public GameObject board;

    private bool isGameOver;
    private bool isPause;

    private int score;

    private bool isSelected;
    private bool isResultShown = false;

    private GameObject firstTile;
    private GameObject secondTile;

    private int totalTiles;
    private int clearTiles;

    public void Start()
    {
        score = 0;
        isGameOver = false;
        isPause = false;

        clearTiles = 0;
        totalTiles = GetComponent<Board>().GetTotalTiles();

        if (audioSource != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }

        GameObject bgm = GameObject.Find("BackgroundMusic");

        if (bgm != null)
        {
            AudioSource bgmSource = bgm.GetComponent<AudioSource>();

            if (bgmSource != null)
            {
                bgmSource.Play(); 
            }
        }

        PlayStartSound();
    }

    private void Update()
    {
        if (!isPause && !isGameOver)
        {
            scoreText.text = score.ToString();
        }

        CheckWin();
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public GameObject GetFirstTile()
    {
        return firstTile;
    }

    public void SelectFirstTile(GameObject tile)
    {
        if (!isGameOver)
        {
            firstTile = tile;
            isSelected = true;
        }
    }

    public void SelectSecondTile(GameObject tile)
    {
        secondTile = tile;

        CompareTwoTiles();
        ResetTiles();
    }

    public void PlayStartSound()
    {
        audioSource.PlayOneShot(startGameSound);
    }

    private void CompareTwoTiles()
    {
        if (firstTile != null && secondTile != null)
        {
            Tile firstTileData = firstTile.GetComponent<Tile>();
            Tile secondTileData = secondTile.GetComponent<Tile>();

            Position firstPos = new Position(firstTileData.row, firstTileData.col);
            Position secondPos = new Position(secondTileData.row, secondTileData.col);

            Board board = GetComponent<Board>();

            bool isConnection = board.CheckConnection(firstPos, secondPos, true);

            if (firstTileData.id == secondTileData.id && isConnection)
            {
                Destroy(firstTile);
                Destroy(secondTile);

                board.Clear(firstPos, secondPos);

                AddScore();

                clearTiles += 2;
                audioSource.PlayOneShot(correctSound);

                if (clearTiles >= totalTiles)
                {
                    isGameOver = true;
                    audioSource.PlayOneShot(winSound);
                }
            }
            else
            {
                firstTile.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                secondTile.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                
                score -= 30;
                
                audioSource.PlayOneShot(incorrectSound);
            }
        }
    }

    private void ResetTiles()
    {
        firstTile = null;
        secondTile = null;
        isSelected = false;
    }

    private void AddScore()
    {
        int point = 70;

        score += point;
    }

    public void MinusChangeScore(int totalChanges)
    {
        score -= (50 * totalChanges);
    }

    public void SetGameOver()
    {
        isGameOver = true;
        audioSource.PlayOneShot(gameOverSound);

        ResetTiles();
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public bool IsPause()
    {
        return isPause;
    }

    public void SetPause(bool pause)
    {
        isPause = pause;
    }

    private void CheckWin()
    {
        if (isGameOver && !isResultShown)
        {
            isResultShown = true;
            board.SetActive(false);

            float remainTime = GetComponent<RoundCountdown>().GetRemainTime();

            Board b = GetComponent<Board>();
            float avgNodes = 0f;
            float avgTime = 0f;

            if (b.botMoveCount > 0)
            {
                avgNodes = (float)b.totalNodesBot / b.botMoveCount;
                avgTime = b.totalTimeBot / b.botMoveCount;
            }

            resultPanel.GetComponent<ResultPanel>().Show(clearTiles >= totalTiles, (this.score - 100), remainTime, avgNodes, avgTime);
        }
    }

    public bool GetHint(int totalGetHintTime)
    {
        Board board = GetComponent<Board>();

        bool hasHint = board.GetHint();

        if (hasHint)
        {
            score -= (10 * totalGetHintTime);
        }

        return hasHint;
    }
}