using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlanetController : MonoBehaviour
{
    public bool isPlanet;
    public Vector2 origin;
    public Sprite[] spriteList;
    public float dist;
    
    void Start()
    {
        if (Random.value >= 0.9)  // not star, but planet.
        {
            GetComponent<SpriteRenderer>().sprite = spriteList[(int)(Random.value * spriteList.Length)];
            this.transform.localScale = Vector3.one * (5.5f * Random.value + 0.5f);
        }
        dist = 5 * Random.value + 0.2f;
        dist = 1;  // fuck this
    }

    private void Update()
    {
        var earth = EarthController.instance;
        var cam = earth.camera.transform.position;
        var pos = -earth.pos / dist + origin;
        // this.transform.position = pos; // fuck this too
    }
}
