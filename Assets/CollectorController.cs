using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorController : MonoBehaviour
{
    public EarthController earth;
    public float[] suckStrength;
    public float[] suckDistance;
    
    public void Start()
    {
        this.earth = EarthController.instance;
    }

    public void FixedUpdate()
    {
        foreach (var food in this.earth.foodList)
        {
            var dist = (Vector2)this.transform.position - food.pos;
            if (dist.magnitude < suckDistance[GetUpgradeLevel()])
            {
                /*food.velo += suckStrength[GetUpgradeLevel()] * dist.normalized;
                if (food.velo.magnitude < 0.02)
                {
                    food.velo += suckStrength[GetUpgradeLevel()] * dist.normalized;
                }*/
                food.velo += suckStrength[GetUpgradeLevel()] * dist.normalized;
                var foodVelo = food.velo;
                var velo_para = foodVelo * Vector2.Dot(foodVelo,dist)/(foodVelo.sqrMagnitude * dist.sqrMagnitude);
                var velo_ort = food.velo - velo_para;
                food.velo = velo_para + velo_ort * 0.95f;

            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<FoodController>(out var ctrl))
        {
            this.earth.DestroyFood(ctrl);
        }
    }
    
    private int GetUpgradeLevel()
    {
        // double parent: eyes <- greenie <- slot
        return this.transform.parent.parent.GetComponent<SlotController>().upgradeLevel - 1;
    }
}
