using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Experimental.Playables;
using UnityEngine.UI;
using Random = System.Random;
using Random2 = UnityEngine.Random;

public class EarthController : MonoBehaviour
{
    public static EarthController instance;
    
    public GameObject asteroidPrefab;
    public GameObject starPrefab;
    public GameObject thruster;
    public Vector2 pos;
    public float angle = 90;  // in deg
    public Vector2 velo;
    public Camera camera;
    public Random rnd = new Random();
    public int numSlots = 6;

    public GameObject slotPrefab;
    public GameObject emptySlotPrefab;
    public GameObject collectorPrefab;
    public GameObject[] slots;

    public GameObject foodPrefab;
    public List<FoodController> foodList = new List<FoodController>();

    public float collectorSuckDistance;

    void Start()
    {
        instance = this;
        Physics.queriesHitTriggers = true;
        this.slots = new GameObject[this.numSlots];
        ScatterStars();
        for (var i = 0; i < this.numSlots; i++)
        {
            var angle = 360 * i / this.numSlots; 
            var pos = this.transform.localScale * 0.37f * Util.Vector2FromAngle(Mathf.Deg2Rad * angle);
            this.slots[i] = Instantiate(this.slotPrefab, new Vector3(pos.x, pos.y, -1), Quaternion.identity, this.transform);
            this.slots[i].transform.eulerAngles = new Vector3(0, 0, angle - 90);
            var ctrl = this.slots[i].GetComponent<SlotController>();
            ctrl.earth = this;
            ctrl.SetInner(emptySlotPrefab, SlotController.SlotType.Empty);
        }

        for (var i = 0; i < 30; i++)
        {
            this.SpawnAsteroid();
        }
        for (var i = 0; i < 30; i++)
        {
            var obj = Instantiate(this.foodPrefab);
            var ctrl = obj.GetComponent<FoodController>();
            ctrl.pos = this.pos + new Vector2(5, 5);
            this.foodList.Add(ctrl);
        }
    }

    void SpawnAsteroid()
    {
        var left_bottom = (Vector2)camera.ScreenToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        var left_top = (Vector2)camera.ScreenToWorldPoint(new Vector3(0, camera.pixelHeight, camera.nearClipPlane));    
        var right_top = (Vector2)camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight, camera.nearClipPlane));
        var right_bottom = (Vector2)camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, 0, camera.nearClipPlane));
        var asteroid = Instantiate(this.asteroidPrefab, this.pos, Quaternion.identity);
        var ast_contr = asteroid.GetComponent<AsteroidController>();
        var ast_pos = new Vector2(0, 0);
        int numX = rnd.Next(0, 2);
        int numY = rnd.Next(0, 2);
        var multiplier = rnd.NextDouble();
        float distance;
        if (numX == 1)
        {
            if (numY == 1)
            {
                ast_pos.x = left_bottom.x;
                distance = (left_top.y - left_bottom.y) * (float)multiplier;
                ast_pos.y = left_bottom.y + distance;
            }
            else
            {   
                ast_pos.x = right_bottom.x;
                distance = (left_top.y - left_bottom.y) * (float)multiplier;
                ast_pos.y = right_bottom.y + distance;
            }
        }
        else
        {
            if (numY == 1)
            {
                ast_pos.y = left_bottom.y;
                distance = (right_bottom.x - left_bottom.x) * (float) multiplier;
                ast_pos.x = left_bottom.y + distance;
            }
            else
            {
                ast_pos.y = left_top.y;
                distance = (right_bottom.x - left_bottom.x) * (float) multiplier;
                ast_pos.x = left_top.y + distance;
            }

        }
        Debug.Log(numY);
        ast_contr.pos = ast_pos;
        ast_contr.velo = 0.05f * (this.pos - ast_pos).normalized;
        ast_contr.earth = this;
    }

    void ScatterStars()
    {
        var bounds = 100f;
        var n_stars = 300;
        for (int i = 0; i < n_stars; i++)
        {
            var starPos = new Vector3(Random2.Range(-bounds, bounds), Random2.Range(-bounds, bounds),2);
            var star = Instantiate(this.starPrefab, starPos, Quaternion.identity);
        }
            
    }

    void Update()
    {
        this.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        this.thruster.transform.eulerAngles = new Vector3(0, 0, this.angle - 90);
        this.camera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.camera.transform.position.z);
    }

    private void FixedUpdate()
    {
        if (Input.GetKey("a"))
        {
            angle += 4f;
        }
        else if (Input.GetKey("d"))
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

    public void DestroyFood(FoodController food)
    {
        this.foodList.Remove(food);
        Destroy(food.gameObject);
    }
}
