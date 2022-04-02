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

    public void FixedUpdate()
    {
        if (this.slotType == SlotType.Collector)
        {
            foreach (var food in this.earth.foodList)
            {
                var dist = (Vector2)this.transform.position - food.pos;
                if (dist.magnitude < this.earth.collectorSuckDistance)
                {
                    food.velo += 0.005f * dist.normalized;
                    if (food.velo.magnitude < 0.02)
                    {
                        food.velo += 0.005f * dist.normalized;
                    }
                }
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("ok!!!");
        Debug.Log(other);
        if (this.slotType == SlotType.Collector)
        {
            // collect :)
            if (other.gameObject.TryGetComponent<FoodController>(out var ctrl))
            {
                this.earth.DestroyFood(ctrl);
            }
        }
    }
}
