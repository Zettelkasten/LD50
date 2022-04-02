using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class EarthController : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public Vector2 pos;
    public float angle;  // in deg
    public Vector2 velo;
    public Camera camera;
    

    void Start()
    {
        this.SpawnAsteroid();
    }

    void SpawnAsteroid()
    {
        for (var i = 0; i < 10; i++)
        {
            var asteroid = Instantiate(this.asteroidPrefab, this.pos, Quaternion.identity);
        }
    }

    void Update()
    {
        this.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        this.transform.eulerAngles = new Vector3(0, 0, this.angle);
        this.camera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.camera.transform.position.z);
    }

    private void FixedUpdate()
    {
        if (Input.GetKey("a"))
        {
            angle += 4f;
        } else if (Input.GetKey("d"))
        {
            angle -= 4f;
        }

        this.velo *= 0.9f;
        if (Input.GetKey("w"))
        {
            velo += 0.05f * Util.Vector2FromAngle(Mathf.Deg2Rad * this.angle);
        }
        this.pos += velo;
    }
}
