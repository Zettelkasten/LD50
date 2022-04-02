using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public Vector2 pos;
    public float angle;  // in deg
    public Vector2 velo;
    
    void Start()
    {
        this.velo = new Vector2(0.02f, 0.01f);
    }

    void Update()
    {
        this.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        this.transform.eulerAngles = new Vector3(0, 0, this.angle);
    }

    private void FixedUpdate()
    {
        this.pos += velo;
    }
}
