using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    public Sprite[] spriteList;
    
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = spriteList[(int)(Random.value * spriteList.Length)];
        this.transform.localScale *= 2.5f * Random.value - 0.5f;
        this.transform.position = EarthController.instance.GetWorldDiagonal().magnitude * Random.insideUnitCircle / 2;
    }
}
