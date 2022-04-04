using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSceneBackgroundComponent : MonoBehaviour
{

    public TutorialSceneComponent intro;
    
    private void OnMouseDown()
    {
        intro.MouseDown();
    }
}
