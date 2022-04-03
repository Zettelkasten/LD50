using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemComponent : MonoBehaviour
{
    public int tileType;

    private void OnMouseDown()
    {
        EarthController.instance.currentTileType = tileType;
        EarthController.instance.isBuilding = true;
    }
}
