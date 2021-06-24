using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.LWRP;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("Player Variables")]
    public GameObject playerCamera;
    public Camera playerCameraCam;
    public Text playerNickName;
    public PhotonView photonView;
    private Rigidbody2D _playerRB;
    public static GameObject player;

    [Header("Player Movement")]
    public float accelerationRate;
    public float accelerationRateForStopping;
    public float accelerationRateForTurining;
    public SpriteRenderer playerSprite;
    private float maxSpeed = 10f;
    private float horizontalMovement;
    private float stationaryTime = 0.3f;
    private float stationaryTimer;
    public static bool playerCanMove;

    [Header("Jump Variables")]
    public LayerMask groundLayers;
    public float radius;
    public float jumpForce;
    public float cliffDelay;
    public float jumpBuffer;
    private float jumpBufferTimer;
    private float cliffDelayTimer;
    public bool isGrounded;
    private float maxAirTime = 0.001f;
    private float airTimeTimer;
    private bool jumping = false;

    [Header("Spawn Variables")]
    public GameObject spawnPoint;
    public float timeTillRespawn;
    public GameObject spawnPointOne;
    public GameObject spawnPointTwo;
    public SpriteRenderer spriteRenderer;
    public Sprite playerOneSprite;
    public Sprite playerTwoSprite;
    public GameObject playerSpotlight;

    public static int currentLevelNum = 0;
    public GameObject currentLevel;
    public static GameObject currentLevelStatic;
    private float sceneSwitchTimer;
    private bool sceneSwitched = false;

    public GameObject currentPlayer;
    public GameObject otherPlayer;
    public Transform currentPlayerName;
    public Transform otherPlayerName;

    public bool loaded = false;

    public GameManager gameManager;

    public Animator playerAnimator;

    public RuntimeAnimatorController playerOneAnimCont;
    public RuntimeAnimatorController playerTwoAnimCont;

    public float startOtherPlayerOuterLight;
    public float startOtherPlayerInnerLight;
    public float startCurrentPlayerOuterLight;
    public float startCurrentPlayerInnerLight;

    private bool winAudioPlayed;

    public GameObject blueDeathParticle;
    public GameObject redDeathParticle;


    // Start is called before the first frame update
    void Awake()
    {

        winAudioPlayed = false;

        PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 1;


        spawnPointOne = GameObject.Find("Spawn Point One");
        spawnPointTwo = GameObject.Find("Spawn Point Two");

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        startOtherPlayerOuterLight = gameManager.otherPlayerLightOuterValue;
        startOtherPlayerInnerLight = gameManager.otherPlayerLightInnerValue;

        startCurrentPlayerOuterLight = gameManager.currentPlayerLightOuterValue;
        startCurrentPlayerInnerLight = gameManager.currentPlayerLightInnerValue;

        if (photonView.IsMine)
        {
            _playerRB = GetComponent<Rigidbody2D>();
            _playerRB.isKinematic = false;
            player = this.gameObject;
            playerCamera.SetActive(true);

            playerNickName.text = PhotonNetwork.NickName;
            playerNickName.color = Color.white;

            photonView.RPC("PlayerCheck", RpcTarget.All);

            playerSpotlight.SetActive(true);
            playerSpotlight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightInnerRadius = gameManager.currentPlayerLightInnerValue;
            playerSpotlight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightOuterRadius = gameManager.currentPlayerLightOuterValue;
        }
        else
        {
            playerNickName.text = photonView.Owner.NickName;
            playerNickName.color = Color.white;

            playerSpotlight.SetActive(true);
            playerSpotlight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightInnerRadius = gameManager.otherPlayerLightInnerValue;
            playerSpotlight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightOuterRadius = gameManager.otherPlayerLightOuterValue;
        }

        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("MASTER");
            transform.position = spawnPointOne.transform.position;
            spriteRenderer.sprite = playerOneSprite;
        }
        else
        {
            Debug.Log("CLIENT");
            transform.position = spawnPointTwo.transform.position;
            spriteRenderer.sprite = playerTwoSprite;
        }

        photonView.RPC("PlayerName", RpcTarget.All);
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (GameManager.canMove && !GameManager.startAudioPlaying)
            {
                Movement();
            }
        }

        
        if (loaded)
        {
            
            photonView.RPC("PlayerName", RpcTarget.All);
            currentPlayerName.gameObject.transform.position = playerCameraCam.WorldToScreenPoint(new Vector2(currentPlayer.transform.position.x, currentPlayer.transform.position.y + 1.5f));
            otherPlayerName.gameObject.transform.position = playerCameraCam.WorldToScreenPoint(new Vector2(otherPlayer.transform.position.x, otherPlayer.transform.position.y + 1.5f));
    }
           


    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (GameManager.canMove && !GameManager.startAudioPlaying)
            {
                Jump();
            }
        }

        if (photonView.IsMine)
        {
            if (gameManager.playerDeathLightTimer > 0)
            {
                playerSpotlight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightOuterRadius = gameManager.currentPlayerLightInnerValue;
                playerSpotlight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightOuterRadius = gameManager.currentPlayerLightOuterValue;

                gameManager.playerDeathLightTimer -= Time.deltaTime;


                if (gameManager.playerDeathLightTimer < 10)
                {
                    if (gameManager.currentPlayerLightInnerValue >= startCurrentPlayerInnerLight)
                    {
                        gameManager.currentPlayerLightInnerValue -= Time.deltaTime / 10;

                        playerSpotlight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightOuterRadius = gameManager.currentPlayerLightInnerValue;
                    }

                    if (gameManager.currentPlayerLightOuterValue >= startCurrentPlayerOuterLight)
                    {
                        gameManager.currentPlayerLightOuterValue -= Time.deltaTime / 10;
                        playerSpotlight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightOuterRadius = gameManager.currentPlayerLightOuterValue;
                    }

                }
            }
            else
            {
                gameManager.currentPlayerLightInnerValue = startCurrentPlayerInnerLight;
                gameManager.currentPlayerLightOuterValue = startCurrentPlayerOuterLight;
                playerSpotlight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightOuterRadius = gameManager.currentPlayerLightInnerValue;
                playerSpotlight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightOuterRadius = gameManager.currentPlayerLightOuterValue;
            }
        }


    }

    private void Movement()
    {
        horizontalMovement += Input.GetAxis("Horizontal");

        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.01f)
        {
            horizontalMovement *= Mathf.Pow(accelerationRateForStopping, Time.fixedDeltaTime * maxSpeed);
        }
        else if (Mathf.Sign(Input.GetAxisRaw("Horizontal")) != Mathf.Sign(horizontalMovement))
        {
            horizontalMovement *= Mathf.Pow(accelerationRateForTurining, Time.fixedDeltaTime * maxSpeed);
        }
        else
        {
            horizontalMovement *= Mathf.Pow(accelerationRate, Time.fixedDeltaTime * maxSpeed);
        }

        _playerRB.velocity = new UnityEngine.Vector2(horizontalMovement, _playerRB.velocity.y);

        playerAnimator.SetFloat("HorSpeed", Mathf.Abs(horizontalMovement));

        //FlipPlayer();

        photonView.RPC("FlipPlayerSprite", RpcTarget.All, horizontalMovement);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + new Vector3(0f, -1f, 0f), radius);

        Vector2 shootingCheckPosition = transform.position + new Vector3(1.25f, 0f, 0);
        Gizmos.DrawLine(shootingCheckPosition, shootingCheckPosition + new Vector2(2.25f, 0f));
    }

    private void Jump()
    {
        Vector2 floorCheckPosition = (Vector2)transform.position + new Vector2(0, -1f);
        isGrounded = Physics2D.OverlapCircle(floorCheckPosition, radius, groundLayers);

        //Creating a delay to allow the player to still jump even if not on ground
        if (isGrounded)
        {
            cliffDelayTimer = cliffDelay;
            jumping = false;
        }
        else
        {
            cliffDelayTimer -= Time.deltaTime;
        }

        //Creating buffer to allow player input slightly before hitting the floor
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = jumpBuffer;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (jumpBufferTimer > 0f && cliffDelayTimer > 0f)
        {
            jumping = true;

            _playerRB.velocity = new UnityEngine.Vector2(_playerRB.velocity.x, jumpForce);

            playerAnimator.SetTrigger("JumpStart");
        }
        else
        {
            jumping = false;
            playerAnimator.ResetTrigger("JumpStart");

        }

        //Creating variable jump height
        if (Input.GetButtonUp("Jump") && _playerRB.velocity.y > 0)
        {
            jumping = true;
            _playerRB.velocity = new UnityEngine.Vector2(_playerRB.velocity.x, _playerRB.velocity.y * 0.5f);
        }

        if (!isGrounded)
        {
            airTimeTimer = maxAirTime;
        }
        else
        {
            airTimeTimer -= Time.deltaTime;
        }

        if ((isGrounded || airTimeTimer < 0f) && _playerRB.velocity.y < 0.1f)
        {
            playerAnimator.SetTrigger("Land");
        }
        else
        {
            playerAnimator.ResetTrigger("Land");
        }
    }

    [PunRPC]
    private void FlipPlayerSprite(float horizontalMovementTest)
    {

        if (horizontalMovementTest > 0)
        {
            playerSprite.flipX = false;
        }
        else if (horizontalMovementTest < 0)
        {
            playerSprite.flipX = true;
        }
        else if (horizontalMovementTest == 0)
        {
            playerSprite.flipX = playerSprite.flipX;
        }
    }

    public void PlayerDeath(string playerTag)
    {


        //player.transform.localPosition = spawnPoint.transform.position;

        //EnemyController.onScreen();

        //Instantiate(currentLevelStatic, currentLevelStatic.gameObject.transform);

        Debug.LogError("DEATH: " + gameManager.playerDeathCount);

        photonView.RPC("PlayerDeathAudio", RpcTarget.All);


        if (photonView.IsMine)
        {
            gameManager.playerDeathCount++;

            Debug.LogError("DEATH: " + gameManager.playerDeathCount);
            if (gameManager.playerDeathCount > 3)
            {
                gameManager.playerDeathLightTimer = 20;
                gameManager.currentPlayerLightOuterValue += startCurrentPlayerOuterLight * 0.3f;
                gameManager.currentPlayerLightInnerValue += startCurrentPlayerInnerLight * 0.15f;
            }

        }

        if (player.tag == "Player 1")
        {
            PhotonNetwork.Instantiate(blueDeathParticle.name, new Vector2(this.transform.position.x, this.transform.position.y), Quaternion.identity);
            player.transform.localPosition = spawnPointOne.transform.position;
        }
        else if (player.tag == "Player 2")
        {
            PhotonNetwork.Instantiate(redDeathParticle.name, new Vector2(this.transform.position.x, this.transform.position.y), Quaternion.identity);
            player.transform.localPosition = spawnPointTwo.transform.position;
        }

        photonView.RPC("Respawn", RpcTarget.All, playerTag);

    }

    [PunRPC]
    private IEnumerator Respawn(string playerTag)
    {
        GameObject playerCol = GameObject.FindGameObjectWithTag(playerTag);
        playerCol.GetComponent<SpriteRenderer>().enabled = false;
        playerCol.GetComponentInChildren<Canvas>().enabled = false;
        playerCol.GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>().enabled = false;
        GameManager.canMove = false;

        yield return new WaitForSeconds(timeTillRespawn);

        playerCol.GetComponent<SpriteRenderer>().enabled = true;
        playerCol.GetComponentInChildren<Canvas>().enabled = true;
        playerCol.GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>().enabled = true;
        GameManager.canMove = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Checkpoint")
        {
            spawnPoint = collision.gameObject;
        }
    }

    [PunRPC]
    public void PlayerDeathAudio()
    {
        AudioManager.playerRespawnAudio.Play();
    }
    
    [PunRPC]
    private void PlayerCheck()
    {
        if (photonView.Owner.IsMasterClient)
        {
            spriteRenderer.sprite = playerOneSprite;
            this.tag = "Player 1";
            spawnPoint = spawnPointOne;

            playerAnimator.runtimeAnimatorController = playerOneAnimCont;
        }
        else
        {
            spriteRenderer.sprite = playerTwoSprite;
            this.tag = "Player 2";
            spawnPoint = spawnPointTwo;

            playerAnimator.runtimeAnimatorController = playerTwoAnimCont;
        }
    }

    [PunRPC]
    private void PlayerName()
    {
            StartCoroutine(LoadNickNames());
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

    public IEnumerator LoadNickNames()
    {
        yield return new WaitForSeconds(0.1f);

        currentPlayer = GameObject.FindGameObjectWithTag("Player 1");
        otherPlayer = GameObject.FindGameObjectWithTag("Player 2");
        currentPlayerName = currentPlayer.transform.Find("Canvas").transform.Find("NickName");
        otherPlayerName = otherPlayer.transform.Find("Canvas").transform.Find("NickName");
        loaded = true;
    }

    public void PlayFootstep()
    {
        if (GameManager.canMove)
        {
            AudioManager.footstepAudio.Play();
        }
    }
    public void PlayJump()
    {
        if (GameManager.canMove)
        {
            AudioManager.jumpAudio.Play();
        }
    }


}


