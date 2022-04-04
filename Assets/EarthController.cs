using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Playables;
using UnityEngine.UI;
using Object = System.Object;
using Random = System.Random;
using Random2 = UnityEngine.Random;
using UnityEngine.SceneManagement;


public class EarthController : MonoBehaviour
{
    public static EarthController instance;

    public GameObject asteroidPrefab;
    public GameObject starPrefab;
    public ThrusterWrapperComponent thruster;
    public GameObject flame;
    public GameObject cdAnimation;
    public Animator CdAnimator;
    public GameObject athmosphere;
    public Text text_level;
    public int highscore;
    public int level = 1;
    public Vector2 pos;
    public float angle = 90;  // in deg
    public Vector2 velo;
    public Camera camera;
    public Random rnd = new Random();
    public int numEarthSlots = 6;
    public int numFlyingSlots = 6;
    public bool accelerating = false;
    public float flamesize = 0;
    public float timer;
    public float regenerationSpeed = 1f;
    public float[] balancing; //difficulty thresholds in seconds, doesnt work though, edited in Unity
    public float count; //timer for balancing
    public int changeLv = 1 ; //value to change difficulty
    public float shakeAmount;

    public GameObject slotPrefab;
    public GameObject[] slots;
    private SlotController thrusterSlot;
    private float[] regenerationSpeeds = new float[] { 0.5f, 0.9f, 1.3f };
    
    public List<AsteroidController> asteroidList = new List<AsteroidController>();
    
    public GameObject foodPrefab;
    public List<FoodController> foodList = new List<FoodController>();
    public int numFood = 0;
    public Text foodCounterText;
    public int numAsteroidsDodged = 0;
    public Text asteroidCounterText;

    public const float starDist = 30f;
    public const float starDistSpawn = 28f;
    public const int n_stars = 200;
    public List<PlanetController> planetList = new List<PlanetController>();

    public SlotController.SlotType[] tileTypes;
    public GameObject[] tileTypePrefabs;
    public int currentTileType;
    public int currentShopPrice;
    public bool isUpgrading = false;
    public bool isRemoving = false;
    public bool isBuilding = false;

    public float[] angleVelocity;
    public float[] speed;

    public bool paused = false;
    public GameObject pauseText;
    
    public GameObject planetPrefab;
    public int numPlanets;

    public GameObject asteroidTutorialScene;
    private GameObject currentScenePlaying = null;
    private bool currentScenePausing = false;
    public bool introAsteroidSpawned = false;
    public bool asteroidTutorialPlayed = false;
        
    void Start()
    {
        this.highscore = PlayerPrefs.GetInt ("highscore", highscore);
        
        cdAnimation.SetActive(false);
        Debug.Assert(tileTypePrefabs.Length == tileTypes.Length);
        instance = this;
        Physics.queriesHitTriggers = true;
        ScatterStars();
        this.slots = new GameObject[this.numEarthSlots + this.numFlyingSlots + 1];
        for (var i = 0; i < this.numEarthSlots; i++)
        {
            var angle = 360 * i / this.numEarthSlots; 
            var pos = this.transform.localScale * 0.37f * Util.Vector2FromAngle(Mathf.Deg2Rad * angle);
            this.slots[i] = Instantiate(this.slotPrefab, new Vector3(pos.x, pos.y, -1), Quaternion.identity, this.transform);
            this.slots[i].transform.eulerAngles = new Vector3(0, 0, angle - 90);
        }

        for (var i = this.numEarthSlots; i < this.numEarthSlots + this.numFlyingSlots; i++)
        {
            var angle = 360 * (i - this.numEarthSlots) / this.numFlyingSlots;
            var pos = this.transform.localScale * 0.7f * Util.Vector2FromAngle(Mathf.Deg2Rad * angle);
            this.slots[i] = Instantiate(this.slotPrefab, new Vector3(pos.x, pos.y, -1), Quaternion.identity, this.transform);
            this.slots[i].transform.eulerAngles = new Vector3(0, 0, angle - 90);
            var ctrl = this.slots[i].GetComponent<SlotController>();
            ctrl.flyingSlot = true;
        }

        var thrusterObj = Instantiate(this.slotPrefab, new Vector3(0, 0, -1), Quaternion.identity, this.transform);
        this.slots[this.numEarthSlots + this.numFlyingSlots] = thrusterObj;
        this.thrusterSlot = thrusterObj.GetComponent<SlotController>();
        this.thrusterSlot.thrusterSlot = true;
        this.thrusterSlot.upgradeLevel = 1;
        this.thrusterSlot.slotType = SlotController.SlotType.Thruster;
        
        // planets
        for (var i = 0; i < numPlanets; i++)
        {
            Instantiate(planetPrefab);
        }

       /* if (tutorialScenesPlayed)
        {
            introAsteroidSpawned = true;
        }*/
        if (!introAsteroidSpawned)
        {
            SpawnAsteroid();
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

    public Vector2 RandomBorderVelo(Vector2 borderPos)
    {
        var angle = Random2.value*Mathf.PI*2;
        var speed = Random2.value * 0.14f + 0.015f;
        var dist = this.pos - borderPos;
        var dist_norm = Mathf.Sqrt(dist.sqrMagnitude);
        var direction = (dist / dist_norm + Util.Vector2FromAngle(angle)).normalized;
        return speed * direction;
    }

    void SpawnAsteroid()
    {
        var ast_pos = RandomBorderPos();
        var asteroid = Instantiate(this.asteroidPrefab, ast_pos, Quaternion.identity);
        var ast_contr = asteroid.GetComponent<AsteroidController>();
        ast_contr.pos = ast_pos;
        ast_contr.velo = RandomBorderVelo(ast_pos);
        this.asteroidList.Add(ast_contr);
    }
    void SpawnFood()
    {   
        var pos = RandomBorderPos();
        var food = Instantiate(this.foodPrefab, pos, Quaternion.identity);
        var contr = food.GetComponent<FoodController>();
        contr.pos = pos;
        contr.velo = RandomBorderVelo(pos);
        this.foodList.Add(contr);
    }

    public void DestroyFood(FoodController food, bool collected = true)
    {
        if (!this.foodList.Contains(food))
            return;
        this.foodList.Remove(food);
        Destroy(food.gameObject);
        if (collected)
            this.numFood += 1;
    }
    
    public void DestroyAstroid(AsteroidController ast, bool dodged = false)
    {
        if (!this.asteroidList.Contains(ast))
            return;
        this.asteroidList.Remove(ast);
        if (ast.gameObject == null)
            return;
        Destroy(ast.gameObject);
        if (dodged)
            this.numAsteroidsDodged +=1;
    }
    
    void ScatterStars()
    {
        for (int i = 0; i < numPlanets; i++)
        {
            var starAngle = Random2.value * 2 * Mathf.PI;
            var starDist2 = Random2.value * starDistSpawn; // todo: even distribution
            var starPos = pos +  starDist2 * Util.Vector2FromAngle(starAngle);
            var star = Instantiate(this.planetPrefab, new Vector3(starPos.x, starPos.y, 4), Quaternion.identity);
            var comp = star.GetComponent<PlanetController>();
            planetList.Add(comp);
        }
    }
    
    void Update()
    {
        this.pauseText.SetActive(this.paused);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.paused = !this.paused;
            if (!this.paused)
            {
                this.isBuilding = false;
                this.UnselectAllShopItems();
            }
        }
        
        if (paused)
        {
            if (this.timer > 0)
                CdAnimator.speed = 0;
            return;
        }
        if (this.timer > 0)
        {
            CdAnimator.speed = regenerationSpeed;
            this.timer -= Time.deltaTime * regenerationSpeed;

            if (this.timer <= 0)
            {
                cdAnimation.SetActive(false); 
            }
            
            var animator = CdAnimator.GetComponent<Animator>();
            var clipinfo = animator.GetCurrentAnimatorClipInfo(0);
            if (clipinfo.Length > 0)  // if this is 0, that is odd, but ok, no clue whats going on! :)
            {
                var alpha = Mathf.Sqrt(Mathf.Sqrt(this.timer / clipinfo[0].clip.length)) * 0.7f;
                var athmoscolor = new Color(1,0.5f,0.5f,alpha);
                this.athmosphere.GetComponent<SpriteRenderer>().color = athmoscolor;
            }
            //var halo = new SerializedObject(this.GetComponent("Halo"));//= new Color(1,0.5f,0.5f,alpha);
            //halo.FindProperty("m_Color").colorValue = athmoscolor;
            var halo = (Behaviour)this.GetComponent("Halo");
            halo.enabled = true;
        }
        else
        {
            cdAnimation.SetActive(false);
            var halo = (Behaviour)this.GetComponent("Halo");
            halo.enabled = false;
        }
        //var animator = CdAnimator.GetComponent<Animator>();
        //var clipinfo = animator.GetCurrentAnimatorClipInfo(0);

        var thrusterfactor = thrusterSlot.upgradeScales[thrusterSlot.upgradeLevel-1];
        this.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        this.thruster.transform.eulerAngles = new Vector3(0, 0, this.angle - 90);
        this.flame.transform.localPosition = new Vector3(x: 0f, y: -0.42f * (thrusterfactor + 1)/2, 0);
        this.flame.transform.eulerAngles = new Vector3(0, 0, this.angle - 90);
        this.flame.transform.localScale = new Vector3(0.75f * flamesize * thrusterfactor, -0.75f * flamesize * thrusterfactor, 1f);
        this.camera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y,
            this.camera.transform.position.z);
        this.thruster.particles.enableEmission = flamesize > 0.001;
        if (this.timer > 0)
        { // shake effect.
            this.camera.transform.localPosition += ((Vector3) Random2.insideUnitCircle * shakeAmount);
        }
        this.foodCounterText.text = this.numFood + " Potatoes";
        this.asteroidCounterText.text = this.numAsteroidsDodged + " Asteroids dodged";
        UpdatePlanets();
        
        // move thruster slot to position
        var slotController = this.thrusterSlot;
        Vector2 thrusterPos = this.thruster.particles.transform.position;
        thrusterPos -= 0.9f * Util.Vector2FromAngle(Mathf.Deg2Rad * this.angle);
        slotController.transform.position = new Vector3(thrusterPos.x, thrusterPos.y, -1);
        slotController.transform.eulerAngles = this.thruster.thruster.transform.eulerAngles;
    }

    private void UpdatePlanets()
    {
        foreach (var star in planetList)
        {
            Vector2 starpos = star.transform.position;
            var dist = starpos - pos;
            var dist_norm = dist.SqrMagnitude();
            if (dist_norm > starDist * starDist)
            {
                var starangle = (Random2.value-0.5f) * Mathf.PI + angle * Mathf.Deg2Rad;
                var starpos_new = pos +  (Random2.value + starDistSpawn) * Util.Vector2FromAngle(starangle);
                star.transform.position = new Vector3(starpos_new.x,starpos_new.y,4);
            }
        }
    }

    public Vector2 GetWorldDiagonal()
    {
        var left_bottom = (Vector2)camera.ScreenToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        var right_top = (Vector2)camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight, camera.nearClipPlane));
        return right_top - left_bottom;
    }

    public void CalcRegenerationSpeed()
    {
        regenerationSpeed = 1f;
        foreach (var slot in (slots))
        {
            var component = slot.GetComponent<SlotController>();
            if (component.slotType == SlotController.SlotType.Regenerator)
            {
                regenerationSpeed += regenerationSpeeds[component.upgradeLevel - 1];
            }
        }
    }

    private void FixedUpdate()
    {
        if (paused)
            return;
        this.count += Time.deltaTime;
        
        //6 Levels design
        /* if (this.count >= balancing[changeLv] && changeLv < this.balancing.Length - 1)
         {
             this.balancing[0] += 0.165f;
             changeLv += 1;
         }*/
        
        //uncapped level design
        if (this.count >= balancing[1])
        {
            this.balancing[0] += 0.14f;//0.165f;
            this.count = 0;
            this.level += 1;
            this.text_level.text = "Level: " + this.level;
        }
        
        // spawn new
        if (asteroidTutorialPlayed && (currentScenePlaying == null || !currentScenePausing)) // default case.
        {
            if (rnd.NextDouble() <= 0.08 * this.balancing[0])
            {
                this.SpawnAsteroid();
                Debug.Log("spawn a");
            }
            if (rnd.NextDouble() <= 0.02)
            {
                this.SpawnFood();
            }
        }
        
        // delete old
        var worldDiagonal = GetWorldDiagonal().magnitude;
        var destroyAsteroidList = asteroidList.Where(asteroid => (asteroid.pos - this.pos).magnitude > worldDiagonal).ToList();
        foreach (var asteroid in destroyAsteroidList)
            DestroyAstroid(asteroid, true);
        var destroyFoodList = foodList.Where(food => (food.pos - this.pos).magnitude > worldDiagonal).ToList();
        foreach (var food in destroyFoodList)
        {
            DestroyFood(food, false);
        }
        
        // movement
        if (Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow))
        {
            angle += angleVelocity[thrusterSlot.upgradeLevel - 1];
        }
        else if (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow))
        {
            angle -= angleVelocity[thrusterSlot.upgradeLevel - 1];
        }

        if (Input.GetKeyDown("f"))
        {
            numFood += 10;
        }

        if (Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene("EndScene"); 
        }
        
        this.velo *= 0.9f;
        if (Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftShift))
        {
            velo += this.speed[thrusterSlot.upgradeLevel - 1] * Util.Vector2FromAngle(Mathf.Deg2Rad * this.angle);
            accelerating = true;
        }
        else
        {
            accelerating = false;
        }
        this.pos += velo;

        // flame
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

        // intro
        if (!asteroidTutorialPlayed && introAsteroidSpawned)
        {
            asteroidTutorialPlayed = true;
            if (asteroidList.Count == 0)
            {
                PlayScene(asteroidTutorialScene, true);
            }
        }
    }

    private void PlayScene(GameObject o, bool b)
    {
        currentScenePlaying = asteroidTutorialScene;
        currentScenePausing = true;
    }

    public void SpawnCooldown()
    {
        if (cdAnimation.activeSelf)
        {
            if (this.numAsteroidsDodged > this.highscore)
            {
                this.highscore = this.numAsteroidsDodged;
                PlayerPrefs.SetInt("highscore", highscore);
                PlayerPrefs.Save();
            }
            SceneManager.LoadScene("EndScene");
        }
        else
        {
            CalcRegenerationSpeed();
            CdAnimator.speed = regenerationSpeed;
            cdAnimation.SetActive(true);
            var animator = CdAnimator.GetComponent<Animator>();
            var clipinfo = animator.GetCurrentAnimatorClipInfo(0);
            this.timer = clipinfo[0].clip.length;
        }
    }

    public void UnselectAllShopItems()
    {
        foreach (var obj in FindObjectsOfType<ShopItemComponent>())
            obj.selected = false;
    }

    public bool CurrentRequiresFlyingSlot()
    {
        return this.tileTypes[this.currentTileType] == SlotController.SlotType.FlyingShield;
    }
}
