using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public bool creditsLoaded;
    public GameObject creditsScreen;

    // Start is called before the first frame update
    void Start()
    {
        creditsLoaded = false;
        creditsScreen.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    public void LoadCredits()
    {
        if (creditsLoaded)
        {
            creditsLoaded = false;
            creditsScreen.SetActive(false);
        }
        else if (!creditsLoaded)
        {
            creditsLoaded = true;
            creditsScreen.SetActive(true);
        }
    }
}
