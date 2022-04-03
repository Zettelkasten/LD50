using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlanetController : MonoBehaviour
{
    public Sprite[] spriteList;
    public int numPlanets;
    
    void Start()
    {
        if (Random.value >= 0.7)  // not star, but planet or nebula.
        {
            int spriteNum;
            if (Random.value < 0.2)
                spriteNum = (int)(Random.value * numPlanets);
            else
                spriteNum = (int)(Random.value * (spriteList.Length - numPlanets)) + numPlanets;
            GetComponent<SpriteRenderer>().sprite = spriteList[spriteNum];
            this.transform.localScale = Vector3.one * (5.5f * Random.value + 0.5f);
        }
    }
}
