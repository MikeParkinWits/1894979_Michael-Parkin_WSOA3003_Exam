using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerMechanicScript : MonoBehaviour
{

    public enum PlatformerObject { FreeMovingPlatform, ButtonMovingPlatform }

    public PlatformerObject platformerObject;

    [Header("Moving Platform")]
    public GameObject positionOne;
    public GameObject positionTwo;
    public GameObject platform;
    public bool reachedSecondPoint = false;
    public static bool buttonStopMovement = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (platformerObject == PlatformerObject.FreeMovingPlatform || platformerObject == PlatformerObject.ButtonMovingPlatform)
        {
            if (platformerObject == PlatformerObject.FreeMovingPlatform || (platformerObject == PlatformerObject.ButtonMovingPlatform && !buttonStopMovement))
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (platformerObject == PlatformerObject.FreeMovingPlatform)
        {
            if (collision.collider.tag == "Player")
            {
                collision.collider.transform.SetParent(transform);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (platformerObject == PlatformerObject.FreeMovingPlatform)
        {
            if (collision.collider.tag == "Player")
            {
                collision.collider.transform.SetParent(null);
            }
        }
    }
}
