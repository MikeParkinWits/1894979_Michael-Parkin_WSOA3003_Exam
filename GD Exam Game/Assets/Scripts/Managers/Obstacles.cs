using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Obstacles : MonoBehaviour
{
    public enum ObstacleType { Spike , Wall, EndBox }

    private PlayerController _playerController;
    public PhotonView photonView;
    private Collider2D endCheckCollision;

    [Header("Obstacle Type")]
    public ObstacleType obstacleType;

    private bool endCheckAudioOnePlayed;
    private bool endCheckAudioTwoPlayed;

    // Start is called before the first frame update
    void Start()
    {

        endCheckAudioOnePlayed = false;
        endCheckAudioTwoPlayed = false;

        Setup();

        if (obstacleType == ObstacleType.Spike)
        {
            Spikes();
        }
    }

    private void Spikes()
    {

    }

    private void Setup()
    {
        _playerController = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.tag == "Player 1" || collision.gameObject.tag == "Player 2") && obstacleType == ObstacleType.Spike)
        {
            _playerController = GameObject.Find(collision.gameObject.name).GetComponent<PlayerController>();

            _playerController.PlayerDeath(collision.gameObject.tag);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (obstacleType == ObstacleType.EndBox && (collision.gameObject.tag == "End Trigger 1" || collision.gameObject.tag == "End Trigger 2"))
        {
            endCheckCollision = collision;
            photonView.RPC("EndCheck", RpcTarget.All);
        }
    }

    [PunRPC]
    private void EndCheck()
    {
        if (endCheckCollision.gameObject.tag == "End Trigger 1")
        {
            GameManager.endCheckOneComplete = true;
            Debug.LogError("1");

            if (!endCheckAudioOnePlayed)
            {
                AudioManager.plugInAudio.Play();
                endCheckAudioOnePlayed = true;
            }
        }

        if (endCheckCollision.gameObject.tag == "End Trigger 2")
        {
            GameManager.endCheckTwoComplete = true;
            Debug.LogError("2");

            if (!endCheckAudioTwoPlayed)
            {
                AudioManager.plugInAudio.Play();
                endCheckAudioTwoPlayed = true;
            }
        }
    }
}
