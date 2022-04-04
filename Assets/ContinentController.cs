using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinentController : MonoBehaviour
{
    // Start is called before the first frame update

    public Vector2 pos;
    public float angle = 0;  // in deg
    public float angleVelo;
    public Vector2 velo;
    public Sprite[] spriteList;
    public int i_continent;
    public float[] spawn_x = {-1, -1, 1, 1,};
    public float[] spawn_y = {1, -1, 1, -1,};
    
    void Start()
    {

    }

    public void setContinent(int i_continent2)
    {
        i_continent = i_continent2;
        GetComponent<SpriteRenderer>().sprite = spriteList[i_continent];
        if (i_continent < 4)
        {
            pos += new Vector2(spawn_x[i_continent], spawn_y[i_continent]);
            velo = (Vector2) Random.onUnitSphere * 0.1f;
        }else if (i_continent == 4)
        {
            pos += (Vector2) Random.onUnitSphere * 0.4f;
            velo = (Vector2) Random.onUnitSphere * 0.15f;
            
        }
        angleVelo = (float) Random.value * 0.9f;
    }

    void Update()
    {
        this.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        this.transform.eulerAngles = new Vector3(0, 0, this.angle);
        
    }

    private void FixedUpdate()
    {
        var earth = EarthController.instance;
        this.pos += velo;
        var dist = earth.pos - pos;
        var dist_norm = Mathf.Sqrt(dist.sqrMagnitude);
        this.velo += 5e-4f * dist / (0.01f + dist_norm);
    }
}
