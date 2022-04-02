using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotController : MonoBehaviour
{
    private GameObject currentInner = null;
    public EarthController earth;
    public SlotType slotType = SlotType.Empty;

    private GameObject mouth;

    private void OnMouseDown()
    {
        Debug.Log("down mouse and so");
        if (this.earth == null)
        {
            return;  // no idea? wtf?
        }

        SetInner(this.earth.collectorPrefab, SlotType.Collector);
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
        Collector
    }
}
