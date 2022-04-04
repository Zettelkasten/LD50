using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundComponent : MonoBehaviour
{
    private void OnMouseDown()
    {
        EarthController.instance.paused = false;
        EarthController.instance.isBuilding = false;
        EarthController.instance.UnselectAllShopItems();
    }
}
