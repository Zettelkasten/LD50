using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotController : MonoBehaviour
{
    private GameObject currentInner = null;
    public EarthController earth;
    public SlotType slotType = SlotType.Empty;

    private void Update()
    {
        this.currentInner.GetComponent<SpriteRenderer>().enabled = this.slotType != SlotType.Empty || this.earth.isBuilding;
    }

    private void OnMouseDown()
    {
        Debug.Log("WHY");
        if (this.earth == null)
        {
            return;  // no idea? wtf?
        }

        if (earth.isBuilding && earth.numFood >= 1)
        {
            SetInner(this.earth.tileTypePrefabs[this.earth.currentTileType], this.earth.tileTypes[this.earth.currentTileType]);
            earth.isBuilding = false;
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
        Shooter
    }
}
