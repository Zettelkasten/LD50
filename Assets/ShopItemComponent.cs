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
    public Color tooExpansive;

    public bool upgrader = false;
    public bool remover = false;

    public bool selected = false;

    public AudioSource clickAudio;

    private void Update()
    {
        this.selectedBackground.SetActive(selected);
        this.background.SetActive(!selected);
        this.moneyText.text = "" + price;
        this.moneyText.color = EarthController.instance.numFood < this.price ? tooExpansive : Color.white;
    }

    private void OnMouseDown()
    {
        if (clickAudio != null)
            clickAudio.Play();
        if (EarthController.instance.numFood < this.price)
        {
            // haha u r broke
            return;
        }
        if (this.upgrader)
        {
            EarthController.instance.isUpgrading = true;
            EarthController.instance.isRemoving = false;
            EarthController.instance.currentTileType = -1;
        }
        else if (this.remover)
        {
            EarthController.instance.isUpgrading = false;
            EarthController.instance.isRemoving = true;
            EarthController.instance.currentTileType = -1;
        }
        else
        { // normal buy
            EarthController.instance.isUpgrading = false;
            EarthController.instance.isRemoving = false;
            EarthController.instance.currentTileType = tileType;
        }
        EarthController.instance.currentShopPrice = price;
        EarthController.instance.UnselectAllShopItems();
        EarthController.instance.isBuilding = true;
        EarthController.instance.paused = true;
        this.selected = true;
    }
}
