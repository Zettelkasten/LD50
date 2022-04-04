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
    public GameObject continentPrefab;
    public ThrusterWrapperComponent thruster;
    public GameObject flame;
    public GameObject cdAnimation;
    public Animator CdAnimator;
    public GameObject athmosphere;
    public GameObject w_button;
    public GameObject a_button;
    public GameObject d_button;
    public GameObject space_button;
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

    public TutorialSceneComponent asteroidTutorialScene;
    public TutorialSceneComponent firstUpgradeTutorialScene;
    public TutorialSceneComponent firstHitTutorialScene;
    public TutorialSceneComponent manyDinosTutorialScene;

    public TutorialSceneComponent currentScenePlaying = null;
    public bool currentScenePausing = false;
    public bool introAsteroidSpawned = false;
    public bool introAsteroidWasVisible = false;

    public float deathTotalWaitingTime;
    public float deathRemainingWaitingTime = 0;

    public AudioSource mainMusic;
    public AudioSource panicMusic;

    public float currentScreenshakeTime = 0;

    public string[] godLines;
    public TutorialSceneComponent godLineScene;

    public AudioSource deathSound;

    void Start()
    {
        this.highscore = PlayerPrefs.GetInt ("highscore", highscore);
        w_button.SetActive(false);
        a_button.SetActive(false);
        d_button.SetActive(false);
        space_button.SetActive(false);
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
            var pos = this.transform.localScale * 0.8f * Util.Vector2FromAngle(Mathf.Deg2Rad * angle);
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
        
        asteroidTutorialScene.gameObject.SetActive(false);
        firstUpgradeTutorialScene.gameObject.SetActive(false);
        firstHitTutorialScene.gameObject.SetActive(false);
        manyDinosTutorialScene.gameObject.SetActive(false);
        godLineScene.gameObject.SetActive(false);
        if (!Util.godLinesShuffled)
        {
            for (int i = 0; i < godLines.Length; i++) {
                int rnd = Random2.Range(0, godLines.Length);
                (godLines[rnd], godLines[i]) = (godLines[i], godLines[rnd]);
            }
            Util.godLinesShuffled = true;
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
        if (deathRemainingWaitingTime > 0)
        {
            deathRemainingWaitingTime -= Time.deltaTime;
            if (deathRemainingWaitingTime <= 0)
            {
                // actually dead.
                if (this.numAsteroidsDodged > this.highscore)
                {
                    this.highscore = this.numAsteroidsDodged;
                    PlayerPrefs.SetInt("highscore", highscore);
                    PlayerPrefs.Save();
                }
                SceneManager.LoadScene("EndScene");
            }
        }
        
        this.mainMusic.volume = this.timer <= 0 ? 1 : 0.000001f;
        this.panicMusic.volume = this.timer > 0 ? 1 : 0.000001f;
        this.mainMusic.volume = this.timer <= 0 ? 0.007f : 0.000000f;
        this.panicMusic.volume = this.timer > 0 ? 0.007f : 0.000000f;
        if (this.currentScenePlaying != null)
        {
            this.mainMusic.volume = 0f;
            this.panicMusic.volume = 0f;
        }

        this.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        this.camera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y,
            this.camera.transform.position.z);
        if (this.timer > 0 || this.currentScreenshakeTime > 0)
        { // shake effect.
            this.camera.transform.localPosition += (Vector3) Random2.insideUnitCircle * shakeAmount;
            if (this.currentScreenshakeTime > 0)
                this.currentScreenshakeTime -= Time.deltaTime;
        }
        
        this.pauseText.SetActive(this.paused);
        
        if (this.timer > 0)
        {
            CdAnimator.speed = regenerationSpeed;
            if (!this.paused && (this.currentScenePlaying == null || !this.currentScenePausing))
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

        var thrusterfactor = thrusterSlot.upgradeScales[thrusterSlot.upgradeLevel-1];
        this.thruster.transform.eulerAngles = new Vector3(0, 0, this.angle - 90);
        this.flame.transform.localPosition = new Vector3(x: 0f, y: -0.42f * (thrusterfactor + 1)/2, 0);
        this.flame.transform.eulerAngles = new Vector3(0, 0, this.angle - 90);
        this.flame.transform.localScale = new Vector3(0.75f * flamesize * thrusterfactor, -0.75f * flamesize * thrusterfactor, 1f);
        this.thruster.particles.enableEmission = flamesize > 0.001;

        this.foodCounterText.text = this.numFood + " Potatoes";
        this.asteroidCounterText.text = this.numAsteroidsDodged + " Asteroids dodged";
        UpdatePlanets();
        
        // move thruster slot to position: ok apparently we dont want this >(
        // var slotController = this.thrusterSlot;
        // Vector2 thrusterPos = this.thruster.particles.transform.position;
        // thrusterPos -= 0.9f * Util.Vector2FromAngle(Mathf.Deg2Rad * this.angle);
        // slotController.transform.position = new Vector3(thrusterPos.x, thrusterPos.y, -1);
        // slotController.transform.eulerAngles = this.thruster.thruster.transform.eulerAngles;

        if (paused || (currentScenePlaying != null && currentScenePausing))
            if (this.timer > 0)
                CdAnimator.speed = 0;
        
        if (currentScenePlaying == null || !currentScenePausing)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.paused = !this.paused;
                if (!this.paused)
                {
                    this.isBuilding = false;
                    this.UnselectAllShopItems();
                }
            }
        }
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
        
        if (Util.asteroidTutorialPlayed == false)
        {
            w_button.SetActive(true);
            a_button.SetActive(true);
            d_button.SetActive(true);
            space_button.SetActive(true);
            if (this.count >= 2)
            {
                w_button.SetActive(false);
                a_button.SetActive(false);
                d_button.SetActive(false);
                space_button.SetActive(false);
            }
            else
            {
                if (this.count < 0.5f)
                {
                    return;
                }
            }
        }


        UpdateTutorialsAndScenes();
        if (this.currentScenePlaying != null && this.currentScenePausing)
            return;
        
        //6 Levels design
        /* if (this.count >= balancing[changeLv] && changeLv < this.balancing.Length - 1)
         {
             this.balancing[0] += 0.165f;
             changeLv += 1;
         }*/
        
        //uncapped level design
        if (this.count >= balancing[1])
        {
            this.balancing[0] += 0.13f;//0.165f;
            this.count = 0;
            this.level += 1;
            this.text_level.text = "Level: " + this.level;
            this.PlayNextGodScene();
        }
        
        // spawn new
        if (Util.asteroidTutorialPlayed && (currentScenePlaying == null || !currentScenePausing)) // default case.
        {
            if (rnd.NextDouble() <= 0.08 * this.balancing[0])
            {
                this.SpawnAsteroid();
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

        if (Input.GetKeyDown("z"))
        {
            apocalpse();
        }
        
        // movement
        if (deathRemainingWaitingTime <= 0)
        {
            if (Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow))
            {
                angle += angleVelocity[thrusterSlot.upgradeLevel - 1];
            }
            else if (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow))
            {
                angle -= angleVelocity[thrusterSlot.upgradeLevel - 1];
            }
        }

        if (Input.GetKeyDown("f") && Input.GetKey(KeyCode.LeftShift))
        {
            numFood += 10;
        }

        if (Input.GetKeyDown("r") && Input.GetKey(KeyCode.LeftShift))
        {
            SceneManager.LoadScene("EndScene"); 
        }

        if (deathRemainingWaitingTime <= 0)
        {
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
        }

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
    }

    private void PlayNextGodScene()
    {
        if (this.currentScenePlaying != null)
            return;
        PlayScene(this.godLineScene, false);
        this.currentScenePlaying.ResetDialogue(new string[] { "God: " + this.godLines[Util.godLineCounter] });
        this.currentScenePlaying.autoContinue = 7f;
        this.currentScenePlaying.autoContinueEnabled = true;
        Util.godLineCounter = (Util.godLineCounter + 1) % godLines.Length;
    }

    public void UpdateTutorialsAndScenes()
    {
        if (currentScenePlaying != null)
        {
            if (currentScenePlaying.isDone)
            {
                currentScenePlaying.gameObject.SetActive(false);
                currentScenePlaying = null;
                currentScenePausing = false;
                this.paused = false;
                return;
            }
        }
        if (!introAsteroidSpawned)
        {
            // spawn asteroid
            var left_bottom = (Vector2)camera.ScreenToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
            var right_bottom = (Vector2)camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, 0, camera.nearClipPlane));
            var ast_pos = 0.9f * left_bottom + 0.2f * right_bottom;
            var asteroid = Instantiate(this.asteroidPrefab, ast_pos, Quaternion.identity);
            var ast_contr = asteroid.GetComponent<AsteroidController>();
            ast_contr.pos = ast_pos;
            ast_contr.velo = Util.Vector2FromAngle(Mathf.Deg2Rad * 70) * (1.4f * 0.15f + 0.03f);
            this.asteroidList.Add(ast_contr);
            
            introAsteroidSpawned = true;
            introAsteroidWasVisible = false;
            return;
        }
        if (!Util.asteroidTutorialPlayed && introAsteroidSpawned)
        {
            introAsteroidWasVisible |= asteroidList.Count > 0 && asteroidList[0].GetComponent<Renderer>().isVisible;
            if (asteroidList.Count == 0 || (introAsteroidWasVisible && !asteroidList[0].GetComponent<Renderer>().isVisible)) // wait for asteroid to despawn.
            {
                Util.asteroidTutorialPlayed = true;
                PlayScene(asteroidTutorialScene, true);
            }
        }

        if (!Util.firstUpgradeTutorialPlayed && this.numFood >= 8)
        {
            Util.firstUpgradeTutorialPlayed = true;
            PlayScene(firstUpgradeTutorialScene, true);
        }

        if (!Util.firstHitTutorialPlayed && this.timer > 0)
        {
            Util.firstHitTutorialPlayed = true;
            PlayScene(firstHitTutorialScene, true);
        }

        var numPlaced = -1; // thruster
        foreach (var slot in slots)
        {
            numPlaced += slot.GetComponent<SlotController>().slotType != SlotController.SlotType.Empty ? 1 : 0;
        }
        if (!Util.manyDinosTutorialPlayed && numPlaced >= 3)
        {
            Util.manyDinosTutorialPlayed = true;
            PlayScene(manyDinosTutorialScene, true);
        }
    }
    
    private void PlayScene(TutorialSceneComponent scene, bool pausing)
    {
        this.currentScreenshakeTime = 0.5f;
        currentScenePlaying = scene;
        currentScenePausing = pausing;
        currentScenePlaying.gameObject.SetActive(true);
    }

    public void SpawnCooldown()
    {
        if (cdAnimation.activeSelf)
        {
            deathRemainingWaitingTime = deathTotalWaitingTime;
            apocalpse();
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


    public void apocalpse()
    {
        this.deathSound.Play();
        this.transform.localScale = new Vector3(0, 0, 0);
        for (int i = 0; i < 4; i++)
        {
            var continent = Instantiate(this.continentPrefab, this.pos, Quaternion.identity);
            continent.GetComponent<ContinentController>().pos = this.pos;
            continent.GetComponent<ContinentController>().setContinent(i);
        }
        
        for (int i = 0; i < 500; i++)
        {
            var continent = Instantiate(this.continentPrefab, this.pos, Quaternion.identity);
            continent.GetComponent<ContinentController>().pos = this.pos;
            continent.GetComponent<ContinentController>().setContinent(4);
        }
    }
}
