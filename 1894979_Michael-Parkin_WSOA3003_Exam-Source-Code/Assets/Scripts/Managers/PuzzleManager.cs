using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PuzzleManager : MonoBehaviour
{

    public List<string> puzzleSolution;
    public List<string> puzzleInput = new List<string>();

    public Test[] puzzleSwitches;

    public bool isEqual;

    public GameObject doorOne;
    public GameObject doorTwo;

    public GameObject doorOnePos;
    public GameObject doorTwoPos;

    public bool loseAudioPlayed = false;
    public bool winAudioPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        puzzleInput.Clear();
        GameManager.combinationString = "";

        foreach (var x in puzzleSwitches)
        {
            x.alreadyClicked = false;
        }

        puzzleSwitches = new Test[puzzleSolution.Count];

        loseAudioPlayed = false;
        winAudioPlayed = false;

        Test.stopPuzzleInput = false;
    }

    // Update is called once per frame
    void Update()
    {

            if (puzzleInput.Count == (puzzleSolution.Count))
            {


                isEqual = Enumerable.SequenceEqual(puzzleSolution, puzzleInput);
                if (isEqual)
                {

                if (!winAudioPlayed)
                {
                    winAudioPlayed = true;
                    AudioManager.puzzleCompleteAudio.Play();
                }

                Test.stopPuzzleInput = true;

                    foreach (var x in puzzleInput)
                    {
                        Debug.Log("Puzzle Input: " + x);
                    }
                }
                else
                {

                if (!loseAudioPlayed)
                {
                    loseAudioPlayed = true;
                    AudioManager.wrongAudio.Play();
                }

                    GameManager.combinationString = "";

                    foreach (var x in puzzleSwitches)
                    {
                        x.alreadyClicked = false;
                    }

                puzzleInput.Clear();

            }
        }

            if (Test.stopPuzzleInput)
        {
            doorOne.gameObject.SetActive(false);
            doorTwo.gameObject.SetActive(false);
        }
    }


    
}
