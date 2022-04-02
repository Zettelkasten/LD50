using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotController : MonoBehaviour
{
    private GameObject currentInner = null;
    public EarthController earth;
    public SlotType slotType = SlotType.Empty;

    private void OnMouseDown()
    {
        Debug.Log("WHY");
        if (this.earth == null)
        {
            return;  // no idea? wtf?
        }

        // SetInner(this.earth.collectorPrefab, SlotType.Collector);
        SetInner(this.earth.shooterPrefab, SlotType.Shooter);
    }

    public void SetInner(GameObject innerPrefab, SlotType slotType)
    {
        if (this.currentInner != null)
        {
            Destroy(this.currentInner);
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
