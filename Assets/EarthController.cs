using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class EarthController : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public Vector2 velo;
    
    void Start()
    {
        
    }

    public float Rotation
    {
        get {
            return Mathf.Deg2Rad * this.transform.localRotation.z;
        }
        set {
            this.transform.localRotation = new Quaternion(0, 0, Mathf.Rad2Deg * value, 0);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("w"))
        {
            this.velo += 0.01f * Util.Vector2FromAngle(this.Rotation);
        }
        if (Input.GetKeyDown("a"))
        {
            this.Rotation = this.Rotation + 0.1f;
        }
    }

    private void FixedUpdate()
    {
        this.transform.localPosition += new Vector3(velo.x, velo.y, 0);
    }
}
