using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotController : MonoBehaviour
{
    private GameObject currentInner = null;
    private Vector3 currentInnerBaseScale = Vector3.zero;
    public GameObject marker;
    public EarthController earth;
    public SlotType slotType = SlotType.Empty;
    public bool flyingSlot = false;
    public int upgradeLevel = 0;
    public float[] upgradeScales = new float[] { 0.7f, 1.0f, 1.3f };
    public int upgradeMax = 3;

    private void Start()
    {
        earth = EarthController.instance;
    }

    private void Update()
    { ;
        marker.SetActive(CanBuildHere());
    }

    public bool CanBuildHere()
    {
        if (!earth.isBuilding)
            return false;
        if (earth.isUpgrading)
            return this.slotType != SlotType.Empty && this.upgradeLevel < upgradeMax;
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
                this.upgradeLevel = 1;
                SetInner(this.earth.tileTypePrefabs[this.earth.currentTileType], this.earth.tileTypes[this.earth.currentTileType]);
            }
            UpdateInner();
            EarthController.instance.UnselectAllShopItems();
            EarthController.instance.isBuilding = false;
        }
    }

    public void SetInner(GameObject innerPrefab, SlotType slotType)
    {
        if (this.currentInner != null)
        {
            Destroy(this.currentInner);
            earth.numFood -= 1;
        }
        this.currentInner = Instantiate(innerPrefab, this.transform);
        this.currentInnerBaseScale = this.currentInner.transform.localScale;
        this.slotType = slotType;
        UpdateInner();
    }

    private void UpdateInner()
    {
        if (this.currentInner == null)
            return;
        var scale = upgradeScales[Math.Min(upgradeLevel - 1, upgradeScales.Length - 1)];
        this.currentInner.transform.localScale = currentInnerBaseScale * scale;
    }
    
    public enum SlotType
    {
        Empty,
        Collector,
        Shooter,
        FlyingShield
    }
}
