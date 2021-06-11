using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Variables")]
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

    public static int currentLevelNum = 0;
    public GameObject currentLevel;
    public static GameObject currentLevelStatic;
    private float sceneSwitchTimer;
    private bool sceneSwitched = false;

    // Start is called before the first frame update
    void Start()
    {
        _playerRB = GetComponent<Rigidbody2D>();
        player = this.gameObject;

        spawnPoint = GameObject.Find("Spawn Point");
    }

    private void FixedUpdate()
    {
        if (GameManager.canMove)
        {
            Movement();
        }

        Debug.Log(currentLevelStatic);
    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.canMove)
        {
            Jump();
        }

        Debug.Log("Current Level: " + currentLevelNum);
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

        FlipPlayer();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + new Vector3(0f, -0.6f, 0f), radius);

        Vector2 shootingCheckPosition = transform.position + new Vector3(1.25f, 0f, 0);
        Gizmos.DrawLine(shootingCheckPosition, shootingCheckPosition + new Vector2(2.25f, 0f));
    }

    private void Jump()
    {
        Vector2 floorCheckPosition = (Vector2)transform.position + new Vector2(0, -0.6f);
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
        }
        else
        {
            jumping = false;
        }

        //Creating variable jump height
        if (Input.GetButtonUp("Jump") && _playerRB.velocity.y > 0)
        {
            jumping = true;
            _playerRB.velocity = new UnityEngine.Vector2(_playerRB.velocity.x, _playerRB.velocity.y * 0.5f);
        }
    }

    private void FlipPlayer()
    {
        if (horizontalMovement > 0)
        {
            playerSprite.flipX = false;
        }
        else if (horizontalMovement < 0)
        {
            playerSprite.flipX = true;
        }
        else if (horizontalMovement == 0)
        {
            playerSprite.flipX = playerSprite.flipX;
        }
    }

    public IEnumerator PlayerDeath()
    {
        //player.transform.localPosition = spawnPoint.transform.position;

        //EnemyController.onScreen();

        //Instantiate(currentLevelStatic, currentLevelStatic.gameObject.transform);

        player.transform.localPosition = spawnPoint.transform.position;
        player.SetActive(false);

        yield return new WaitForSeconds(timeTillRespawn);

        player.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Checkpoint")
        {
            spawnPoint = collision.gameObject;
        }
    }
}


