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
        return this.slotType == SlotType.Empty && earth.isBuilding && (earth.CurrentRequiresFlyingSlot() == flyingSlot);
    }

    private void OnMouseDown()
    {
        if (CanBuildHere() && earth.numFood >= 1)
        {
            SetInner(this.earth.tileTypePrefabs[this.earth.currentTileType], this.earth.tileTypes[this.earth.currentTileType]);
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
