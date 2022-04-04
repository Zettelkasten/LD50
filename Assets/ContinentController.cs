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
    public float[] spawn_x = {-0.5f, -0.5f, 0.5f, 0.5f};
    public float[] spawn_y = {0.4f, -0.3f, 0.3f, -0.3f};
    public float lifetime = -1;
    
    void Start()
    {

    }

    public void setContinent(int i_continent2)
    {
        i_continent = i_continent2;
        GetComponent<SpriteRenderer>().sprite = spriteList[i_continent];
        float z;
        if (i_continent < 4)
        {
            pos += new Vector2(spawn_x[i_continent], spawn_y[i_continent])*0.65f;
            velo = (new Vector2(spawn_x[i_continent], spawn_y[i_continent]) +  0.3f * Random.insideUnitCircle) * 0.05f;
            z = -1f;
        }else if (i_continent == 4)
        {
            pos += (Vector2) Random.insideUnitCircle * 1.3f;
            velo = (Vector2) Random.insideUnitCircle * 0.12f*Random.value;
            z = 1f;

        }
        else if (i_continent == 5)
        {
            velo = (Vector2) Random.insideUnitCircle * 0.08f*Random.value;
            z = 3f;
        }
        else
        {
            velo += (Vector2) Random.insideUnitCircle * 0.08f*Random.value;
            this.transform.localScale = this.transform.localScale * 0.5f;
            this.lifetime = 3;
            pos += (Vector2) Random.insideUnitCircle * 0.3f;
            z = -1f;
        }
        var position = this.transform.position;
        this.transform.position = new Vector3(position.x, position.y,z);
        angleVelo = (float) (Random.value-0.5) * 2f;
    }

    void Update()
    {
        this.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        this.transform.eulerAngles = new Vector3(0, 0, this.angle);
        if (this.lifetime < 2f && this.lifetime > 0)
        {
            var color = this.GetComponent<SpriteRenderer>().color;
            this.GetComponent<SpriteRenderer>().color = new Color(color.r,color.g, color.b,this.lifetime / 2f);
        }
            
        
    }

    private void FixedUpdate()
    {
        var earth = EarthController.instance;
        if (earth.paused && i_continent > 5)
            return;
        
        this.pos += velo;
        this.angle += angleVelo;
        if (this.i_continent < 6)
        {
            var dist = earth.pos - pos;
            var dist_norm = Mathf.Sqrt(dist.sqrMagnitude);
            this.velo += 3e-4f * dist / (0.01f + dist_norm);
            if (dist_norm > 1.3f) // allow the earth to rebuild itself
                this.velo *= 0.995f;
            if(earth.apocalypse == false)
                Destroy(this);
        }
        

        if (lifetime > 0)
        {
            if (lifetime - Time.deltaTime < 0)
            {
                Destroy(this);
            }

            this.lifetime -= Time.deltaTime;
        }
    }
}
