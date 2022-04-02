using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorController : MonoBehaviour
{
    public EarthController earth;
    
    public void Start()
    {
        this.earth = EarthController.instance;
    }

    public void FixedUpdate()
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

    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("ok!!!");
        Debug.Log(other);
        if (other.gameObject.TryGetComponent<FoodController>(out var ctrl))
        {
            this.earth.DestroyFood(ctrl);
        }
    }
}
