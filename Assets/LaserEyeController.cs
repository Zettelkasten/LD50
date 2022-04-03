using UnityEngine;

public class LaserEyeController : MonoBehaviour
{
    public float[] shootCooldown = new float[] { 4, 3, 2 };
    private float currentCooldown;
    public GameObject laserPrefab;
    public float[] maxRadius = new float[] { 10, 15, 20 };

    private AsteroidController currentLaserTarget = null;
    private GameObject currentLaser = null;
    private float currentLaserTime = 0;
    public float[] currentLaserTimeMax = new float[] { 1f, 0.5f, 0.3f };
    
    private void FixedUpdate()
    {
        if (currentLaser != null)
        {
            currentLaserTime -= Time.fixedDeltaTime;
            if (currentLaserTime <= 0)
            {
                Destroy(currentLaser);
                if (EarthController.instance.asteroidList.Contains(currentLaserTarget))
                    EarthController.instance.DestroyAstroid(currentLaserTarget);
                currentLaserTarget = null;
                currentLaser = null;
                currentLaserTime = 0;
            }
            else
            {
                var color = currentLaser.GetComponent<LineRenderer>().startColor;
                color = new Color(color.r, color.g, color.b, currentLaserTime / currentLaserTimeMax[GetUpgradeLevel()]);
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

            if (closest != null && closestDist <= maxRadius[GetUpgradeLevel()])
            {
                // shoot it.
                currentCooldown = shootCooldown[GetUpgradeLevel()];
                currentLaser = Instantiate(laserPrefab, EarthController.instance.transform);
                currentLaser.GetComponent<LineRenderer>().SetPositions(new Vector3[] {this.transform.position, closest.transform.position});
                currentLaserTarget = closest;
                currentLaserTime = currentLaserTimeMax[GetUpgradeLevel()];
            }
        }
    }

    private int GetUpgradeLevel()
    {
        // double parent: eyes <- guac <- slot
        return this.transform.parent.parent.GetComponent<SlotController>().upgradeLevel - 1;
    }
}
