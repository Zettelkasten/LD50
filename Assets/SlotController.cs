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
    public bool thrusterSlot = false;
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
        if (earth.isRemoving)
            return this.slotType != SlotType.Empty && this.slotType != SlotType.Thruster;
        if (earth.isUpgrading)
            return this.slotType != SlotType.Empty && this.upgradeLevel < upgradeMax;
        else
            return this.slotType == SlotType.Empty && (earth.CurrentRequiresFlyingSlot() == flyingSlot) && !thrusterSlot;
    }

    private void OnMouseDown()
    {
        if (CanBuildHere() && earth.numFood >= earth.currentShopPrice)
        {
            EarthController.instance.paused = false;
            earth.numFood -= earth.currentShopPrice;
            if (earth.isUpgrading)
            {
                // upgrade.
                this.upgradeLevel++;
            }
            else if (earth.isRemoving)
            {
                earth.numFood += 5;
                Destroy(this.currentInner);
                this.currentInner = null;
                this.slotType = SlotType.Empty;
                return;
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
        }
        this.currentInner = Instantiate(innerPrefab, this.transform);
        this.currentInnerBaseScale = this.currentInner.transform.localScale;
        this.slotType = slotType;
        UpdateInner();
    }

    private void UpdateInner()
    {
        var scale = upgradeScales[Math.Min(upgradeLevel - 1, upgradeScales.Length - 1)];

        if (this.currentInner != null)
            this.currentInner.transform.localScale = currentInnerBaseScale * scale;
        if (this.slotType == SlotType.Thruster)
        {
            earth.thruster.GetComponent<ThrusterWrapperComponent>().thruster.transform.localScale = Vector3.one * scale;
            earth.flame.transform.localScale = Vector3.one * scale;
        }
    }
    
    public enum SlotType
    {
        Empty,
        Collector,
        Shooter,
        FlyingShield,
        Regenerator,
        Thruster
    }
}
