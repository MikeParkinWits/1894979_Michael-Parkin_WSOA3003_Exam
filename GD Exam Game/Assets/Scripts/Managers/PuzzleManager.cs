using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PuzzleManager : MonoBehaviour
{

    public List<int> puzzleSolution;
    public static List<int> puzzleInput = new List<int>();

    public static InteractableObjects[] puzzleSwitches;

    public bool isEqual;

    // Start is called before the first frame update
    void Start()
    {
        puzzleInput.Clear();
        puzzleSwitches = new InteractableObjects[puzzleSolution.Count()];
    }

    // Update is called once per frame
    void Update()
    {

            if (puzzleInput.Count > (puzzleSolution.Count - 1))
            {


                isEqual = Enumerable.SequenceEqual(puzzleSolution, puzzleInput);
                if (isEqual)
                {
                    Debug.Log("Lists are Equal");

                    foreach (var x in puzzleInput)
                    {
                        Debug.Log("Puzzle Input: " + x);
                    }
                }
                else
                {
                    Debug.Log("Lists are not Equal");
                    puzzleInput.Clear();
                    GameManager.combinationString = "";

                    foreach (var x in puzzleSwitches)
                    {
                        x.alreadyClicked = false;
                    }

                }
            }
        }


    
}
