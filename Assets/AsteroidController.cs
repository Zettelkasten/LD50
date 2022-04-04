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
    public GameObject schweif;
    public float criticalSpeed = 0.14f;
    public GameObject path;

    private void Start()
    {
        this.angle = Random.value * 360;
        
        var halo = (Behaviour)this.GetComponent("Halo");
        var dist_norm = Mathf.Sqrt(velo.SqrMagnitude());

        if (dist_norm > criticalSpeed)
        {
            halo.enabled = true;
        }
        else
        {
            halo.enabled = false;
        }
    }

    void Update()
    {
        this.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        this.transform.eulerAngles = new Vector3(0, 0, this.angle);
        this.schweif.transform.eulerAngles = new Vector3(0, 0, -Vector2.SignedAngle(velo, Vector2.right) + Mathf.PI);
        
        //var velo_angle = Mathf.Atan2(velo.x, velo.y) + angle;
        this.path.transform.eulerAngles = new Vector3(0, 0, -Vector2.SignedAngle(velo, Vector2.right));//new Vector3(velo_angle,0,0);
        
    }

    private void FixedUpdate()
    {
        var earth = EarthController.instance;
        if (earth.paused)
            return;
        this.pos += velo;
        var dist = earth.pos - pos;
        var dist_norm = dist.sqrMagnitude;
        this.velo += 2e-2f * dist / (0.01f + Mathf.Pow(dist_norm,2));
        // this.velo += 1e-3f *  (earth.pos - pos);
        //this.velo *= 1 - 1e-20f;
        // this.angle += 2;
    }
}
