using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroBackgroundComponent : MonoBehaviour
{

    public IntroComponent intro;
    
    private void OnMouseDown()
    {
        intro.MouseDown();
    }
}
