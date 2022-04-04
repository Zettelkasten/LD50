using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoodController : MonoBehaviour
{
    public Vector2 pos;
    public float angle;  // in deg
    public Vector2 velo;
    
    void Start()
    {
        this.velo = 0.05f * Util.Vector2FromAngle(Mathf.Deg2Rad * 360 * Random.value);
    }

    void Update()
    {
        this.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        this.transform.eulerAngles = new Vector3(0, 0, this.angle);
    }

    private void FixedUpdate()
    {
        if (EarthController.instance.paused)
            return;
        this.pos += velo;
        this.angle += 5;
    }
}
