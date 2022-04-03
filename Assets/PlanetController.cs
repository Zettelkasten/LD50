using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlanetController : MonoBehaviour
{
    private Vector2 origin;
    public Sprite[] spriteList;
    public float dist;
    
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = spriteList[(int)(Random.value * spriteList.Length)];
        this.transform.localScale *= 2.5f * Random.value - 0.5f;
        origin = EarthController.instance.GetWorldDiagonal().magnitude * Random.insideUnitCircle / 2;
        this.transform.position = origin;
        dist = 5 * Random.value + 5;
    }

    private void Update()
    {
        this.transform.localPosition = EarthController.instance.pos / 10 + origin;
    }
}
