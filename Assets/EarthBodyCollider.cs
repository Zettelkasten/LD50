using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBodyCollider : MonoBehaviour
{
    public EarthController earth;
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<FoodController>(out var foodCtrl))
        {
            this.earth.DestroyFood(foodCtrl);
        }
        else if (other.gameObject.TryGetComponent<AsteroidController>(out var astCtrl))
        {  
            this.earth.DestroyAstroid(astCtrl);
        }
    }
}
