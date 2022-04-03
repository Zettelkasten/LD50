using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEditor.UIElements;
using UnityEngine;

public class LaserEyeController : MonoBehaviour
{
    public float shootCooldown;
    private float currentCooldown;
    public GameObject laserPrefab;
    public float maxRadius;

    private AsteroidController currentLaserTarget = null;
    private GameObject currentLaser = null;
    private float currentLaserTime = 0;
    public float currentLaserTimeMax = 0.5f;
    
    private void FixedUpdate()
    {
        if (currentLaser != null)
        {
            currentLaserTime -= Time.fixedDeltaTime;
            if (currentLaserTime <= 0)
            {
                Destroy(currentLaser);
                EarthController.instance.DestroyAstroid(currentLaserTarget);
                currentLaserTarget = null;
                currentLaser = null;
                currentLaserTime = 0;
            }
            else
            {
                Debug.Log("ok?");
                Debug.Log(currentLaserTime);
                var color = currentLaser.GetComponent<LineRenderer>().startColor;
                color = new Color(color.r, color.g, color.b, currentLaserTime / currentLaserTimeMax);
                currentLaser.GetComponent<LineRenderer>().startColor = color;
                currentLaser.GetComponent<LineRenderer>().endColor = color;
            }
        }
        
        currentCooldown -= Time.fixedDeltaTime;
        if (currentCooldown < 0)
        {
            // do shoot
            float closestDist = Mathf.Infinity;
            AsteroidController closest = null;
            foreach (var ast in EarthController.instance.asteroidList)
            {
                var newDist = ((Vector2)(ast.transform.position - this.transform.position)).magnitude;
                if (newDist < closestDist)
                {
                    closestDist = newDist;
                    closest = ast;
                }
            }

            if (closest != null && closestDist <= maxRadius)
            {
                // shoot it.
                currentCooldown = shootCooldown;
                currentLaser = Instantiate(laserPrefab, EarthController.instance.transform);
                currentLaser.GetComponent<LineRenderer>().SetPositions(new Vector3[] {this.transform.position, closest.transform.position});
                currentLaserTarget = closest;
                currentLaserTime = currentLaserTimeMax;
            }
        }
    }
}
