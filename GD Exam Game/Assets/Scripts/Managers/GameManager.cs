using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("Game Manager")]
    public static bool canMove = true;
    private bool isPaused = false;

    [Header("Timer")]
    public bool timerLevel;
    public float maxLevelTimerAmount = 0;

    [Header("Puzzle")]
    public bool combinationLevel = false;

    [Header("UI References")]
    public Text levelTimerText;
    public Text combinationPuzzleText;
    public static string combinationString;
    public GameObject levelWonScreen;
    public GameObject levelLostScreen;
    public GameObject pauseScreen;

    // Start is called before the first frame update
    void Start()
    {
        if (combinationLevel)
        {
            combinationPuzzleText.enabled = true;
        }
        else
        {
            combinationPuzzleText.enabled = false;
        }

        Time.timeScale = 1;

        levelWonScreen.SetActive(false);
        levelLostScreen.SetActive(false);
        pauseScreen.SetActive(false);

        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerLevel)
        {
            LevelTimer();
            levelTimerText.enabled = true;

            levelTimerText.text = maxLevelTimerAmount.ToString("0");
        }
        else
        {
            levelTimerText.enabled = false;
        }

        combinationPuzzleText.text = combinationString;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameWon();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameLost();
        }
    }

    private void LevelTimer()
    {
        if (maxLevelTimerAmount > 0)
        {
            maxLevelTimerAmount -= Time.deltaTime;
        }
        else
        {
            LeveFailed();
        }
    }

    public void LeveFailed()
    {
        Debug.Log("RESTART");
    }

    private void GameWon()
    {
        canMove = false;
        Time.timeScale = 0;
        levelWonScreen.SetActive(true);
    }

    private void GameLost()
    {
        canMove = false;
        Time.timeScale = 0;
        levelLostScreen.SetActive(true);
    }

    public void OnPause()
    {
        if (isPaused)
        {
            isPaused = false;
            pauseScreen.SetActive(false);
            canMove = true;
            Time.timeScale = 1;
        }
        else
        {
            pauseScreen.SetActive(true);
            canMove = false;
            isPaused = true;
            Time.timeScale = 0;
        }


    }
}
