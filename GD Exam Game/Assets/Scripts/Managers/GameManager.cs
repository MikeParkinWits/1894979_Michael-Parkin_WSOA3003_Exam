using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("Networking")]
    public GameObject playerPrefab;
    public GameObject gameCanvas;
    public GameObject sceneCamera;
    public float warningTime = 0;
    public GameObject warningTextWon;
    public GameObject warningTextPause;

    [Header("Game Manager")]
    public static bool canMove = true;
    private bool isPaused = false;
    public static bool startAudioPlaying = false;
    public float startAudioTimer;
    public GameObject skipStartAudioText;
    public static bool endCheckOneComplete;
    public static bool endCheckTwoComplete;

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
    public Text pingText;

    [Header("Player Light Values")]
    public float currentPlayerLightOuterValue;
    public float currentPlayerLightInnerValue;

    public float otherPlayerLightOuterValue;
    public float otherPlayerLightInnerValue;

    public PhotonView photonView;

    [Header("Feedback Variables")]
    public int playerDeathCount;
    public float playerDeathLightTimer;

    private bool winAudioPlayed;
    private bool loseAudioPlayed;

    private bool onHelpScreen;
    public GameObject helpScreen;

    public GameObject pauseButton;

    // Start is called before the first frame update
    void Awake()
    {

        winAudioPlayed = false;
        loseAudioPlayed = false;

        pauseButton.SetActive(true);

        startAudioPlaying = true;
        isPaused = false;
        onHelpScreen = false;
        helpScreen.SetActive(false);

        endCheckOneComplete = false;
        endCheckTwoComplete = false;
        if (combinationLevel)
        {
            combinationPuzzleText.enabled = true;
        }
        else
        {
            combinationPuzzleText.enabled = false;
        }

        playerDeathCount = 0;

        Time.timeScale = 1;

        levelWonScreen.SetActive(false);
        levelLostScreen.SetActive(false);
        pauseScreen.SetActive(false);

        isPaused = false;

        SpawnPlayer();

        canMove = true;

        PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 1;

        warningTextWon.SetActive(false);
        warningTextPause.SetActive(false);


    }

    private void Start()
    {
        AudioManager.startAudio.Play();
    }

    // Update is called once per frame
    void Update()
    {

        if (startAudioTimer > 0)
        {
            skipStartAudioText.SetActive(true);
            startAudioTimer -= Time.deltaTime;
            pauseButton.SetActive(false);
        }
        else
        {
            skipStartAudioText.SetActive(false);
            startAudioPlaying = false;
            AudioManager.startAudio.Stop();
            pauseButton.SetActive(true);
        }

        if (timerLevel)
        {
            if (!isPaused && canMove && !startAudioPlaying)
            {
                LevelTimer();
            }

            levelTimerText.enabled = true;

            levelTimerText.text = maxLevelTimerAmount.ToString("0");
        }
        else
        {
            levelTimerText.enabled = false;
        }

        combinationPuzzleText.text = combinationString;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!startAudioPlaying)
            {
                OnPause();
            }

        }

        pingText.text = "Ping: " + PhotonNetwork.GetPing();

        if (endCheckOneComplete && endCheckTwoComplete)
        {
            GameWon();
        }

        if (PhotonNetwork.PlayerList.Length < 2)
        {
            LoadMainMenu("MainMenu");
        }

        if (warningTime > 0)
        {
            warningTime -= Time.deltaTime;
            warningTextWon.SetActive(true);
        }

        if (Input.GetKey(KeyCode.Return) && startAudioPlaying)
        {
            photonView.RPC("SkipStartAudio", RpcTarget.All);
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
        GameLost();
        Debug.Log("RESTART");
    }

    private void GameWon()
    {
        canMove = false;
        levelWonScreen.SetActive(true);
        photonView.RPC("SaveLevel", RpcTarget.All);
        //Time.timeScale = 0;
    }

    private void GameLost()
    {
        canMove = false;
        levelLostScreen.SetActive(true);
        photonView.RPC("FailLevel", RpcTarget.All);
        //Time.timeScale = 0;
    }

    public void OnPause()
    {

        photonView.RPC("Pause", RpcTarget.All);


    }

    public void HelpButton()
    {
        photonView.RPC("Help", RpcTarget.All);
    }


    [PunRPC]
    private void SaveLevel()
    {
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + " Complete", 1);
        if (!winAudioPlayed)
        {
            winAudioPlayed = true;
            AudioManager.completeAudio.Play();
        }
        Debug.LogError("YAY");
        Debug.Log("YAY");
    }

    [PunRPC]
    private void FailLevel()
    {
        if (!loseAudioPlayed)
        {
            loseAudioPlayed = true;
            AudioManager.loseAudio.Play();
        }
    }

    [PunRPC]
    public void Pause()
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
            //Time.timeScale = 0;
        }

        warningTextWon.SetActive(false);
        warningTextPause.SetActive(false);
    }

    [PunRPC]
    public void Help()
    {
        if (onHelpScreen)
        {
            onHelpScreen = false;
            helpScreen.SetActive(false);
        }
        else
        {
            helpScreen.SetActive(true);
            onHelpScreen = true;
        }
    }

    [PunRPC]
    public void SkipStartAudio()
    {
        startAudioPlaying = false;
        startAudioTimer = -1;
    }

    public void SpawnPlayer()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector2(this.transform.position.x, this.transform.position.y), Quaternion.identity);
        sceneCamera.SetActive(false);
    }

    public void LoadScene(string scene)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ResetTime", RpcTarget.All);
            PhotonNetwork.LoadLevel(scene);
        }
        else
        {
            warningTime = 15;
        }

    }

    public void ResetScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ResetLevel", RpcTarget.All);
        }
        else
        {
            warningTime = 15;
        }

    }

    public void LoadMainMenu(string scene)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("ResetTime", RpcTarget.All);
            StartCoroutine(Disconnect());
            SceneManager.LoadScene(scene);
        }
        else
        {
            warningTime = 15;
        }

    }

    public void OnQuit()
    {
        photonView.RPC("QUIT", RpcTarget.All);
        PhotonNetwork.SendAllOutgoingCommands();
        Application.Quit();
    }

    public IEnumerator Disconnect()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.Disconnect();
        PhotonNetwork.NickName = "";
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        SceneManager.LoadScene("MainMenu");

    }

    [PunRPC]
    public void QUIT()
    {
        StartCoroutine(Disconnect());
    }

    [PunRPC]
    public void ResetTime()
    {
        Time.timeScale = 1;
    }

    [PunRPC]
    public void ResetLevel()
    {
        Time.timeScale = 1;
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name);
    }
}
