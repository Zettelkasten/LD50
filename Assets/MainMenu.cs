using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ExitButton()
    {
        // Use for app 
        // Application.Quit();  
      
        //Use for Webplayer
        SceneManager.LoadScene("Thanks_for_playing");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene"); 
    }
    
    public void StartIntro()
    {
        SceneManager.LoadScene("Intro"); 
    }
}
