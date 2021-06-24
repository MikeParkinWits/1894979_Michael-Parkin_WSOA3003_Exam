using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Test : MonoBehaviour
{

    public enum ObjectType { ButtonMovingPlatform, FreeMovingPlatform, RotatingPlatform, MovingDoor, CombinationPuzzleSwitch, LightPuzzleSwitch }
    public ObjectType objectType;

    public PhotonView photonView;
    private bool inTrigger = false;
    private PhotonView photonViewTrig;

    [Header("Moving Platform")]
    public GameObject platform;
    public GameObject positionTwo;
    public GameObject positionOne;
    private bool platformCanMove = false;
    private bool reachedSecondPoint = false;

    [Header("Rotating Platform")]
    private bool platformCanRotate = true;
    public int platformRotationSpeed = 65;

    [Header("Door")]
    public GameObject door;
    public GameObject doorPosOne;
    public GameObject doorPosTwo;
    private bool doorCanBeSwitched = true;
    private bool moveDoor = false;
    private bool moveToPosTwo = false;

    [Header("Puzzle")]
    public string puzzleSolutionPosition;
    public PuzzleManager puzzleManager;
    public bool alreadyClicked = false;
    public static bool stopPuzzleInput = false;

    public bool puzzleLevel;

    // Start is called before the first frame update
    void Start()
    {
        inTrigger = false;

        platformCanMove = false;

        if (objectType == ObjectType.LightPuzzleSwitch || objectType == ObjectType.CombinationPuzzleSwitch)
        {
            puzzleManager = GameObject.FindGameObjectWithTag("Puzzle Parent").GetComponent<PuzzleManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (moveDoor)
        {
            door.gameObject.SetActive(false);
        }

        if (inTrigger && photonViewTrig.IsMine)
        {


            if (objectType == ObjectType.CombinationPuzzleSwitch && !stopPuzzleInput)
            {
                if (Input.GetKeyDown(KeyCode.Return) && !alreadyClicked)
                {
                    photonView.RPC("PuzzleSwitch", RpcTarget.All);
                    photonView.RPC("PlayAudioButton", RpcTarget.All);
                }
            }



            if (objectType == ObjectType.LightPuzzleSwitch && !stopPuzzleInput)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    photonView.RPC("LightPuzzle", RpcTarget.All);
                    photonView.RPC("PlayAudioButton", RpcTarget.All);
                    puzzleManager.loseAudioPlayed = false;
                }
            }




            if (objectType == ObjectType.MovingDoor)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    photonView.RPC("MovingDoor", RpcTarget.All);
                    photonView.RPC("PlayAudioButton", RpcTarget.All);
                }
            }

            if (objectType == ObjectType.ButtonMovingPlatform)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    photonView.RPC("MovePlatform", RpcTarget.All);
                    photonView.RPC("PlayAudioButton", RpcTarget.All);
                }
            }

            if (objectType == ObjectType.RotatingPlatform)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    photonView.RPC("RotatePlatform", RpcTarget.All);
                    photonView.RPC("PlayAudioButton", RpcTarget.All);
                }
            }

        }

        if (objectType == ObjectType.FreeMovingPlatform)
        {
            photonView.RPC("FreeMovePlatform", RpcTarget.All);
        }

        //CHECKERS

        if (platformCanRotate && objectType == ObjectType.RotatingPlatform)
        {
            platform.transform.Rotate(Vector3.forward * platformRotationSpeed * Time.deltaTime);
        }


        if (platformCanMove && (objectType == ObjectType.ButtonMovingPlatform || objectType == ObjectType.FreeMovingPlatform))
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

            yield return new WaitForSeconds(1f);

            doorCanBeSwitched = true;
            moveDoor = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

            EnterTrigger();
            photonViewTrig = collision.gameObject.GetComponent<PlayerController>().photonView;
        

        

    }

    private void OnTriggerExit2D(Collider2D collision)
    {

            ExitTrigger();
        

    }

    [PunRPC]
    private void EnterTrigger()
    {
        Debug.Log("HELLO");
        inTrigger = true;
    }

    [PunRPC]
    private void ExitTrigger()
    {
        Debug.Log("BYE");
        inTrigger = false;
    }

    [PunRPC]
    private void MovePlatform()
    {
        if (platformCanMove)
        {
            platformCanMove = false;
        }
        else
        {
            platformCanMove = true;
        }
    }

    [PunRPC]
    private void FreeMovePlatform()
    {
            platformCanMove = true;
    }

    [PunRPC]
    private void RotatePlatform()
    {
        if (platformCanRotate)
        {
            platformCanRotate = false;
        }
        else
        {
            platformCanRotate = true;
        }
    }

    [PunRPC]
    private void MovingDoor()
    {
        moveDoor = true;
    }

    [PunRPC]
    private void PuzzleSwitch()
    {
        puzzleManager.puzzleInput.Add(puzzleSolutionPosition);
        GameManager.combinationString += puzzleSolutionPosition;
        alreadyClicked = true;
        puzzleManager.puzzleSwitches[puzzleManager.puzzleInput.Count - 1] = this.gameObject.GetComponent<Test>();
        puzzleManager.loseAudioPlayed = false;
    }

    [PunRPC]
    private void LightPuzzle()
    {
        puzzleManager.puzzleInput.Add(puzzleSolutionPosition);
        GameManager.combinationString += puzzleSolutionPosition;
    }

    [PunRPC]
    private void PlayAudioButton()
    {
        AudioManager.inGameButtonAudio.Play();
    }
}
