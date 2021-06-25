using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjects : MonoBehaviour
{

    public enum ObjectType { DoorSwitch, CombinationPuzzleSwitch, LightPuzzleSwitch, PlatformSwitch, FreeMovingPlatform, ButtonMovingPlatform, RotatingPlatform }

    [Header("Object Type")]
    public ObjectType objectType;

    [Header("Door")]
    public GameObject door;
    public GameObject doorPosOne;
    public GameObject doorPosTwo;
    public bool doorCanBeSwitched = true;
    public bool moveDoor = false;
    private bool moveToPosTwo = false;
    private bool inTrigger = false;

    [Header("Puzzle")]
    public string puzzleSolutionPosition;
    public PuzzleManager puzzleManager;
    public bool alreadyClicked = false;

    [Header("Platformer")]
    public GameObject positionOne;
    public GameObject positionTwo;
    public GameObject platform;
    public Rigidbody2D platformRb;
    public bool reachedSecondPoint = false;
    public bool buttonStopMovement = false;
    public bool platformRotating = true;
    public int rotationSpeed;
    public PhotonView photonView;
    public GameObject currentPos;

    // Start is called before the first frame update
    void Start()
    {
        puzzleManager = GameObject.FindGameObjectWithTag("Puzzle Parent").GetComponent<PuzzleManager>();
    }

    [PunRPC]
    // Update is called once per frame
    void Update()
    {
        if (moveDoor)
        {
            StartCoroutine(DoorSwitch());

                door.gameObject.SetActive(false);
            
        }

        if (GameManager.canMove)
        {

            if (Trigger())
            {
                if (objectType == ObjectType.DoorSwitch)
                {
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        moveDoor = true;
                    }
                }

                if (objectType == ObjectType.CombinationPuzzleSwitch)
                {
                    if (Input.GetKeyDown(KeyCode.Return) && !alreadyClicked)
                    {
                        puzzleManager.puzzleInput.Add(puzzleSolutionPosition);
                        GameManager.combinationString += puzzleSolutionPosition;
                        alreadyClicked = true;
                        //PuzzleManager.puzzleSwitches[PuzzleManager.puzzleInput.Count - 1] = this.gameObject.GetComponent<InteractableObjects>();

                    }
                }

                if (objectType == ObjectType.LightPuzzleSwitch)
                {
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        puzzleManager.puzzleInput.Add(puzzleSolutionPosition);
                        GameManager.combinationString += puzzleSolutionPosition;

                    }
                }

                if (objectType == ObjectType.ButtonMovingPlatform)
                {
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        //photonView.RPC("CheckTrigger", RpcTarget.All);

                    }
                }

                if (objectType == ObjectType.RotatingPlatform)
                {
                    if (Input.GetKeyDown(KeyCode.Return))
                    {

                        if (platformRotating)
                        {
                            platformRotating = false;
                        }
                        else if (!platformRotating)
                        {
                            platformRotating = true;
                        }


                    }
                }


            }

            if (platformRotating && objectType == ObjectType.RotatingPlatform)
            {
                platform.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            }

            if (objectType == ObjectType.FreeMovingPlatform || objectType == ObjectType.ButtonMovingPlatform)
            {
                if (objectType == ObjectType.FreeMovingPlatform || (objectType == ObjectType.ButtonMovingPlatform && !buttonStopMovement))
                {

                    currentPos.transform.position = platform.transform.position;

                    if (!reachedSecondPoint)
                    {
                        platform.transform.position = Vector3.MoveTowards(platform.transform.position, positionTwo.transform.position, 0.01f);
                    }
                    else
                    {
                        platform.transform.position = Vector3.MoveTowards(platform.transform.position, positionOne.transform.position, 0.01f);
                    }

                    if (platform.transform.position == positionOne.transform.position)
                    {
                        reachedSecondPoint = false;
                    }

                    if (platform.transform.position == positionTwo.transform.position)
                    {
                        reachedSecondPoint = true;
                    }
                }
                else
                {
                    platform.transform.position = currentPos.transform.position;
                }

            }


        }
    }

    private IEnumerator DoorSwitch()
    {
        if (doorCanBeSwitched)
        {

            if (door.transform.position == doorPosOne.transform.position)
            {
                moveToPosTwo = true;
            }

            if (door.transform.position == doorPosTwo.transform.position)
            {
                moveToPosTwo = false;
            }



            doorCanBeSwitched = false;

            yield return new WaitForSeconds(0.4f);

            doorCanBeSwitched = true;
            moveDoor = false;
        }
    }

    [PunRPC]
    void CheckTrigger()
    {
        if (buttonStopMovement)
        {
            buttonStopMovement = false;
        }
        else if (!buttonStopMovement)
        {
            buttonStopMovement = true;
        }
    }

    [PunRPC]
    private void OnTriggerEnter2D(Collider2D collision)
    {

            inTrigger = true;
        
    }

    [PunRPC]
    private void OnTriggerExit2D(Collider2D collision)
    {
            inTrigger = false;
        
    }

    private bool Trigger()
    {
        return inTrigger;
    }

    //Platform Code
    [PunRPC]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetPhotonView().IsMine)
        {
            if (objectType == ObjectType.FreeMovingPlatform)
            {
                if (collision.collider.tag == "Player")
                {
                    collision.collider.transform.SetParent(transform);
                }
            }
        }
    }

    [PunRPC]
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetPhotonView().IsMine)
        {
            if (objectType == ObjectType.FreeMovingPlatform)
            {
                if (collision.collider.tag == "Player")
                {
                    collision.collider.transform.SetParent(null);
                }
            }
        }
    }

    [PunRPC]
    private void PlayAudioButton()
    {
        AudioManager.inGameButtonAudio.Play();
    }
}
