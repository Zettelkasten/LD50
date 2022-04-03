using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemComponent : MonoBehaviour
{
    public int tileType;
    public GameObject background;
    public GameObject selectedBackground;

    public bool upgrader = false;

    public bool selected = false;

    private void Update()
    {
        this.selectedBackground.SetActive(selected);
        this.background.SetActive(!selected);
    }

    private void OnMouseDown()
    {
        if (this.upgrader)
        {
            EarthController.instance.isUpgrading = true;
            EarthController.instance.currentTileType = -1;
        }
        else
        {
            EarthController.instance.isUpgrading = false;
            EarthController.instance.currentTileType = tileType;
        }
        EarthController.instance.UnselectAllShopItems();
        EarthController.instance.isBuilding = true;
        this.selected = true;
    }
}
