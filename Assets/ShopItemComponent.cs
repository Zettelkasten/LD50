using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemComponent : MonoBehaviour
{
    public int tileType;
    public GameObject background;
    public GameObject selectedBackground;
    public Text moneyText;
    public int price;

    public bool upgrader = false;

    public bool selected = false;

    private void Update()
    {
        this.selectedBackground.SetActive(selected);
        this.background.SetActive(!selected);
        this.moneyText.text = "" + price;
    }

    private void OnMouseDown()
    {
        if (EarthController.instance.numFood < this.price)
        {
            // haha u r broke
            return;
        }
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
        EarthController.instance.currentShopPrice = price;
        EarthController.instance.UnselectAllShopItems();
        EarthController.instance.isBuilding = true;
        this.selected = true;
    }
}
