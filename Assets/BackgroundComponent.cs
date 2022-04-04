using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundComponent : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (EarthController.instance.currentScenePlaying != null)
        {
            var currentScene = EarthController.instance.currentScenePlaying;
            currentScene.MouseDown();
            return;
        }
        EarthController.instance.paused = false;
        EarthController.instance.isBuilding = false;
        EarthController.instance.UnselectAllShopItems();
    }
}
