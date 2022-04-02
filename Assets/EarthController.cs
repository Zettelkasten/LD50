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
    public GameObject flame;
    public Vector2 pos;
    public float angle = 90;  // in deg
    public Vector2 velo;
    public Camera camera;
    public Random rnd = new Random();
    public int numSlots = 6;
    public bool accelerating = false;
    public float flamesize = 0;

    public GameObject slotPrefab;
    public GameObject emptySlotPrefab;
    public GameObject collectorPrefab;
    public GameObject[] slots;

    public GameObject foodPrefab;
    public List<FoodController> foodList = new List<FoodController>();
    public int numFood = 0;
    public Text foodCounterText;

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
    }

    Vector2 RandomBorderPos()
    {
        var left_bottom = (Vector2)camera.ScreenToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        var left_top = (Vector2)camera.ScreenToWorldPoint(new Vector3(0, camera.pixelHeight, camera.nearClipPlane));    
        var right_top = (Vector2)camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight, camera.nearClipPlane));
        var right_bottom = (Vector2)camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, 0, camera.nearClipPlane));
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
                ast_pos.x = left_bottom.x + distance;
            }
            else
            {
                ast_pos.y = left_top.y;
                distance = (right_bottom.x - left_bottom.x) * (float) multiplier;
                ast_pos.x = left_top.x + distance;
            }
        }
        return ast_pos;
    }

    void SpawnAsteroid()
    {
        var ast_pos = RandomBorderPos();
        var asteroid = Instantiate(this.asteroidPrefab, ast_pos, Quaternion.identity);
        var ast_contr = asteroid.GetComponent<AsteroidController>();
        ast_contr.pos = ast_pos;
        ast_contr.velo = 0.05f * (this.pos - ast_pos).normalized;
        ast_contr.earth = this;
    }
    void SpawnFood()
    {
        var food = Instantiate(this.foodPrefab, this.pos, Quaternion.identity);
        var contr = food.GetComponent<FoodController>();
        var pos = RandomBorderPos();
        contr.pos = pos;
        contr.velo = 0.05f * (this.pos - pos).normalized;
        this.foodList.Add(contr);
    }

    public void DestroyFood(FoodController food)
    {
        this.foodList.Remove(food);
        Destroy(food.gameObject);
        this.numFood += 1;
    }
    
    public void DestroyAstroid(AsteroidController ast)
    {
        Destroy(ast.gameObject);
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
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<FoodController>(out var foodCtrl))
        {
            this.DestroyFood(foodCtrl);
        }
        else if (other.gameObject.TryGetComponent<AsteroidController>(out var astCtrl))
        {  
            this.DestroyAstroid(astCtrl);
        }
    }
    
    void Update()
    {
        this.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        this.thruster.transform.eulerAngles = new Vector3(0, 0, this.angle - 90);
        this.flame.transform.eulerAngles = new Vector3(0, 0, this.angle - 90);
        this.flame.transform.localScale = new Vector3(0.7f * flamesize, -0.7f * flamesize, 1f);
        this.camera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.camera.transform.position.z);
        this.foodCounterText.text = this.numFood + " Potatos";
    }

    private void FixedUpdate()
    {
        if (rnd.NextDouble() <= 0.01)
        {
            this.SpawnAsteroid();
        }
        if (rnd.NextDouble() <= 0.01)
        {
            this.SpawnFood();
        }
        
        
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
            accelerating = true;
        }
        else
        {
            accelerating = false;
        }
        this.pos += velo;

        if (accelerating)
        {
            if (flamesize < 0.8)
                flamesize += 0.2f;
            else
            {
                flamesize += 0.1f * (1 - flamesize) * Random2.value;
                flamesize += 0.2f * (Random2.value-0.5f);
            }
        }
        else
        {
            if (flamesize > 0.0)
                flamesize -= 0.12f;

        }
    }
}
