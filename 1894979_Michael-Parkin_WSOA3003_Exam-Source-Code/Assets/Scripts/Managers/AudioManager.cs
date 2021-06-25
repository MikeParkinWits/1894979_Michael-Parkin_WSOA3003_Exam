using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioSource footstepAudio, jumpAudio, inGameButtonAudio, backgroundAudio, wrongAudio, plugInAudio, completeAudio, loseAudio, puzzleCompleteAudio, playerRespawnAudio, startAudio;

    // Start is called before the first frame update
    void Awake()
    {
        var audioSources = GetComponents<AudioSource>();
        footstepAudio = audioSources[0];
        jumpAudio = audioSources[1];
        inGameButtonAudio = audioSources[2];
        backgroundAudio = audioSources[3];
        wrongAudio = audioSources[4];
        plugInAudio = audioSources[5];
        completeAudio = audioSources[6];
        loseAudio = audioSources[7];
        puzzleCompleteAudio = audioSources[8];
        playerRespawnAudio = audioSources[9];
        startAudio = audioSources[10];

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
