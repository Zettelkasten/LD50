using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class BlockCollider : MonoBehaviour
{
    public float angle;  // in deg
    public float[] angleSpeed;

    private void Start()
    {
        this.angle = this.transform.parent.eulerAngles.z + 90;
    }

    public void Update()
    {
        var earth = EarthController.instance;
        var pos = earth.transform.localScale * 0.8f * Util.Vector2FromAngle(Mathf.Deg2Rad * angle);
        this.transform.position = earth.transform.position + new Vector3(pos.x, pos.y, 0);
        this.transform.eulerAngles = new Vector3(0, 0, angle - 90);
    }

    public void FixedUpdate()
    {
        angle += this.angleSpeed[GetUpgradeLevel()];
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        var earth = EarthController.instance;
        if (other.gameObject.TryGetComponent<AsteroidController>(out var asteroid))
        {
            var normal = Util.Vector2FromAngle(Mathf.Deg2Rad * angle);
            var toEarth = Vector2.Dot(normal, asteroid.velo) < 0;
            asteroid.velo = normal * 0.2f + 0*(toEarth?1:-1) * Vector2.Reflect(asteroid.velo, normal) + 0.8f * earth.velo;
        }
    }
    private int GetUpgradeLevel()
    {
        return this.transform.parent.GetComponent<SlotController>().upgradeLevel - 1;
    }
}
