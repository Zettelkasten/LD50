using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemComponent : MonoBehaviour
{
    private int tileType;

    private void OnMouseDown()
    {
        EarthController.instance.currentTileType = tileType;
    }
}
