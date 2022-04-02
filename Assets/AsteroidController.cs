using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidController : MonoBehaviour
{
    public Vector2 pos;
    public float angle;  // in deg
    public Vector2 velo;
    public EarthController earth;
    

    void Update()
    {
        this.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        this.transform.eulerAngles = new Vector3(0, 0, this.angle);
    }

    private void FixedUpdate()
    {
        this.pos += velo;
        //this.velo += 1e-3f *  (earth.pos - pos);
        //this.velo *= 1 - 1e-3f;
    }
}
