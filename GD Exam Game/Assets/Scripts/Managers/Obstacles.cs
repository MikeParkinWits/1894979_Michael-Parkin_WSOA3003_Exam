using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    public enum ObstacleType { Spike , Wall }

    private PlayerController _playerController;

    [Header("Obstacle Type")]
    public ObstacleType obstacleType;

    // Start is called before the first frame update
    void Start()
    {

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
        _playerController = GameObject.Find("Player 1").GetComponent<PlayerController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && obstacleType == ObstacleType.Spike)
        {
            _playerController = GameObject.Find(collision.gameObject.name).GetComponent<PlayerController>();

            StartCoroutine(_playerController.PlayerDeath());
        }
    }
}
