using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEyeController : MonoBehaviour
{
    public float shootCooldown;
    private float currentCooldown;

    private void FixedUpdate()
    {
        currentCooldown -= Time.fixedDeltaTime;
        if (currentCooldown < 0)
        {
            currentCooldown = shootCooldown;
            // DoShoot();
        }
    }
}
