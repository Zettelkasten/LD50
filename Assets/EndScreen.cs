using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
   public void ExitButton()
   {
      Application.Quit();
   }

   public void RestartGame()
   {
      SceneManager.LoadScene("SampleScene"); 
   }
}
