using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
   public static EndScreen instance;
   public Text highscore;
   public Text cur_score;
   
      
   public void Start()
   {
      
      highscore.text = "Your Highscore: " + EarthController.instance.highscore;
      cur_score.text = "Your Score: " + EarthController.instance.numAsteroidsDodged;
   }

   public void ExitButton()
   {
      // Use for app 
      // Application.Quit();  
      
      //Use for Webplayer
      SceneManager.LoadScene("Thanks_for_playing");
   }

   public void RestartGame()
   {
      SceneManager.LoadScene("SampleScene"); 
   }
}
