using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemComponent : MonoBehaviour
{
    public int tileType;
    public GameObject background;
    public GameObject selectedBackground;

    public bool selected = false;

    private void Update()
    {
        this.selectedBackground.SetActive(selected);
        this.background.SetActive(!selected);
    }

    private void OnMouseDown()
    {
        EarthController.instance.currentTileType = tileType;
        EarthController.instance.UnselectAllShopItems();
        this.selected = true;
        EarthController.instance.isBuilding = true;
    }
}
