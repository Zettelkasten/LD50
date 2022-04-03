using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotController : MonoBehaviour
{
    private GameObject currentInner = null;
    public EarthController earth;
    public SlotType slotType = SlotType.Empty;
    public bool flyingSlot = false;
    public int upgradeLevel = 0;

    private void Start()
    {
        earth = EarthController.instance;
    }

    private void Update()
    {
        // also effects children, so always active when there is something built here.
        this.currentInner.GetComponent<SpriteRenderer>().enabled = this.slotType != SlotType.Empty || CanBuildHere();
    }

    public bool CanBuildHere()
    {
        if (!earth.isBuilding)
            return false;
        if (earth.isUpgrading)
            return this.slotType != SlotType.Empty;
        else
            return this.slotType == SlotType.Empty && (earth.CurrentRequiresFlyingSlot() == flyingSlot);
    }

    private void OnMouseDown()
    {
        if (CanBuildHere() && earth.numFood >= 1)
        {
            if (earth.isUpgrading)
            {
                // upgrade.
                this.upgradeLevel++;
            }
            else
            {
                // build.
                SetInner(this.earth.tileTypePrefabs[this.earth.currentTileType], this.earth.tileTypes[this.earth.currentTileType]);
                this.upgradeLevel = 1;
            }
            EarthController.instance.UnselectAllShopItems();
        }
    }

    public void SetInner(GameObject innerPrefab, SlotType slotType)
    {
        if (this.slotType != SlotType.Empty)
        {
            return; //do nothing, slot is taken already.
        }
        if (this.currentInner != null)
        {
            Destroy(this.currentInner);
            earth.numFood -= 1; 
        }
        this.currentInner = Instantiate(innerPrefab, this.transform);
        this.slotType = slotType;
        
    }
    
    public enum SlotType
    {
        Empty,
        Collector,
        Shooter,
        FlyingShield
    }
}
