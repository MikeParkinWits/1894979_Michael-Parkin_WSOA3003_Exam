using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject _startButton;

    [SerializeField] private GameObject hostTextGameObject;
    [SerializeField] private Text roomIDText;
    [SerializeField] private bool displayErrorMessage;
    [SerializeField] private Text errorMessageText;
    [SerializeField] private float errorMessageTimer;

    [SerializeField] private GameObject[] levels;
    [SerializeField] private GameObject[] lockedLevels;

    [SerializeField] private PhotonView photonView;


    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        displayErrorMessage = false;
    }

    // Start is called before the first frame update
    void Start()
    {

        hostTextGameObject.SetActive(true);
        roomIDText.text = "Room ID: " + PhotonNetwork.CurrentRoom.Name.ToString();


        LevelCheck();



    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        LevelCheck();
        Debug.LogError("JOINED");
    }

    private void LevelCheck()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PlayerPrefs.GetInt("Level 1 Complete") == 1)
            {
                photonView.RPC("LevelLockCheck", RpcTarget.All, true, false, false, false, false);
            }

            if (PlayerPrefs.GetInt("Level 2 Complete") == 1)
            {
                photonView.RPC("LevelLockCheck", RpcTarget.All, true, true, false, false, false);
            }

            if (PlayerPrefs.GetInt("Level 3 Complete") == 1)
            {
                photonView.RPC("LevelLockCheck", RpcTarget.All, true, true, true, false, false);
            }

            if (PlayerPrefs.GetInt("Level 4 Complete") == 1)
            {
                photonView.RPC("LevelLockCheck", RpcTarget.All, true, true, true, true, false);

            }

            if (PlayerPrefs.GetInt("Level 5 Complete") == 1)
            {
                photonView.RPC("LevelLockCheck", RpcTarget.All, true, true, true, true, true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //Debug.LogError("HOST");
            hostTextGameObject.SetActive(true);
        }
        else
        {
            hostTextGameObject.SetActive(false);
        }

        if (displayErrorMessage)
        {
            if (errorMessageTimer > 0)
            {
                errorMessageTimer -= Time.deltaTime;
            }
            else
            {
                errorMessageText.text = "";
            }
        }
    }

    public void LoadLevel(string level)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.PlayerList.Length == 2)
            {
                PhotonNetwork.LoadLevel(level);
            }
            else
            {
                displayErrorMessage = true;
                errorMessageText.text = "Other Player Has Not Joined";
                errorMessageTimer = 5;
                Debug.Log("NEED MORE PLAYERS");
            }
        }
        else
        {
            displayErrorMessage = true;
            errorMessageText.text = "Only Host Can Perform This Action";
            errorMessageTimer = 5;
        }
    }

    public void Disconnect()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Lobby");
    }

    [PunRPC]
    public void LevelLockCheck(bool levelOne, bool levelTwo, bool levelThree, bool levelFour, bool levelFive)
    {
        levels[1].SetActive(false);
        levels[2].SetActive(false);
        levels[3].SetActive(false);
        levels[4].SetActive(false);

        lockedLevels[1].SetActive(true);
        lockedLevels[2].SetActive(true);
        lockedLevels[3].SetActive(true);
        lockedLevels[4].SetActive(true);

        if (levelOne)
        {
            levels[1].SetActive(true);
            levels[2].SetActive(false);
            levels[3].SetActive(false);
            levels[4].SetActive(false);

            lockedLevels[1].SetActive(false);
            lockedLevels[2].SetActive(true);
            lockedLevels[3].SetActive(true);
            lockedLevels[4].SetActive(true);
        }

        if (levelTwo)
        {
            levels[1].SetActive(true);
            levels[2].SetActive(true);
            levels[3].SetActive(false);
            levels[4].SetActive(false);

            lockedLevels[1].SetActive(false);
            lockedLevels[2].SetActive(false);
            lockedLevels[3].SetActive(true);
            lockedLevels[4].SetActive(true);
        }

        if (levelThree)
        {
            levels[1].SetActive(true);
            levels[2].SetActive(true);
            levels[3].SetActive(true);
            levels[4].SetActive(false);

            lockedLevels[1].SetActive(false);
            lockedLevels[2].SetActive(false);
            lockedLevels[3].SetActive(false);
            lockedLevels[4].SetActive(true);
        }

        if (levelFour)
        {
            levels[1].SetActive(true);
            levels[2].SetActive(true);
            levels[3].SetActive(true);
            levels[4].SetActive(true);

            lockedLevels[1].SetActive(false);
            lockedLevels[2].SetActive(false);
            lockedLevels[3].SetActive(false);
            lockedLevels[4].SetActive(false);
        }





    }
}
