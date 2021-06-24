using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviourPunCallbacks
{

    [SerializeField] private string _gameVersion = "0.1";
    [SerializeField] private GameObject _usernameMenu;
    [SerializeField] private GameObject _connectPanel;
    [SerializeField] private GameObject _connectingPanel;
    [SerializeField] private GameObject _connnectedSuccessPanel;
    [SerializeField] private float connectedTimer;
    [SerializeField] private bool connected;
    [SerializeField] private float noRoomIDTimer;
    [SerializeField] private GameObject failedToJoinText;
    [SerializeField] private Text failText;


    [SerializeField] private InputField _usernameInput;
    [SerializeField] private InputField _createGameInput;
    [SerializeField] private InputField _joinGameInput;
    [SerializeField] private GameObject _createGameButton;


    [SerializeField] private GameObject _startButton;


    private void Awake()
    {
        if (PhotonNetwork.NickName.Length == 0)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = _gameVersion;
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        connectedTimer = 2;
        noRoomIDTimer = 0;

        connected = false;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");
        Debug.Log(connectedTimer);

        connected = true;





    }

    // Start is called before the first frame update
    void Start()
    {
        _usernameMenu.SetActive(true);
        _connectingPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.NickName.Length == 0)
        {


            if (connected)
            {
                if (connectedTimer > 0)
                {
                    connectedTimer -= Time.deltaTime;
                    _connectingPanel.SetActive(false);
                    _connnectedSuccessPanel.SetActive(true);
                }
                else
                {
                    _connnectedSuccessPanel.SetActive(false);
                }
            }

        }
        else
        {
            _usernameMenu.SetActive(false);
            _connectingPanel.SetActive(false);
            _connnectedSuccessPanel.SetActive(false);
        }

        if (noRoomIDTimer > 0)
            {
                noRoomIDTimer -= Time.deltaTime;
                failedToJoinText.SetActive(true);
            }
            else
            {
                failedToJoinText.SetActive(false);
            }

        if (_createGameInput.text.Length <= 7)
        {
            _createGameButton.SetActive(true);
        }
        else
        {
            _createGameButton.SetActive(false);
        }

    }

    public void CheckUsernameInput()
    {
        if (_usernameInput.text.Length >= 3 && _usernameInput.text.Length <= 7)
        {
            _startButton.SetActive(true);
        }
        else
        {
            _startButton.SetActive(false);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.

        Debug.Log(returnCode + " " + message);

        if (returnCode == 32758)
        {
            failText.text = "Room Does Not Exist";
        }

        if (returnCode == 32765)
        {
            failText.text = "Room is Full";
        }

        noRoomIDTimer = 3;
    }

    public void SetUsernameInput()
    {
        _usernameMenu.SetActive(false);
        PhotonNetwork.NickName = _usernameInput.text;
        Debug.Log("USERNAME SET");
    }

    public void CreateGame()
    {

            PhotonNetwork.CreateRoom(_createGameInput.text.ToString().ToLower(), new RoomOptions() { MaxPlayers = 2 }, null);
        
    }

    public void JoinGame()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        
        PhotonNetwork.JoinRoom(_joinGameInput.text.ToString().ToLower());
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("LevelSelector");
    }

    public void DisconnectButton()
    {
        StartCoroutine(Disconnect());
    }

    public IEnumerator Disconnect()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.NickName = "";
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        SceneManager.LoadScene("MainMenu");
    }
}
