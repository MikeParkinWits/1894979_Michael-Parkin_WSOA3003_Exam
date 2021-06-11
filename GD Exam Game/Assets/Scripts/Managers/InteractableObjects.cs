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
    public int puzzleSolutionPosition;
    public PuzzleManager puzzleManager;
    public bool alreadyClicked = false;

    [Header("Platformer")]
    public GameObject positionOne;
    public GameObject positionTwo;
    public GameObject platform;
    public bool reachedSecondPoint = false;
    public bool buttonStopMovement = false;
    public bool platformRotating = true;
    public int rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        puzzleManager = GameObject.FindGameObjectWithTag("Puzzle Parent").GetComponent<PuzzleManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveDoor)
        {
            StartCoroutine(DoorSwitch());
            if (moveToPosTwo)
            {
                door.gameObject.transform.position = Vector3.MoveTowards(door.gameObject.transform.position, doorPosTwo.transform.position, 0.1f);
            }
            else
            {
                door.gameObject.transform.position = Vector3.MoveTowards(door.gameObject.transform.position, doorPosOne.transform.position, 0.1f);
            }
        }

        Debug.Log(moveToPosTwo);


        if (GameManager.canMove)
        {


            if (inTrigger)
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
                        PuzzleManager.puzzleInput.Add(puzzleSolutionPosition);
                        GameManager.combinationString += puzzleSolutionPosition;
                        alreadyClicked = true;
                        PuzzleManager.puzzleSwitches[PuzzleManager.puzzleInput.Count - 1] = this.gameObject.GetComponent<InteractableObjects>();
                    }
                }

                if (objectType == ObjectType.LightPuzzleSwitch)
                {
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        PuzzleManager.puzzleInput.Add(puzzleSolutionPosition);
                        GameManager.combinationString += puzzleSolutionPosition;
                    }
                }

                if (objectType == ObjectType.ButtonMovingPlatform)
                {
                    if (Input.GetKeyDown(KeyCode.Return))
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        inTrigger = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        inTrigger = false;
    }

    //Platform Code

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (objectType == ObjectType.FreeMovingPlatform)
        {
            if (collision.collider.tag == "Player")
            {
                collision.collider.transform.SetParent(transform);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
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
