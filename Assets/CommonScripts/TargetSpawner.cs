using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class TargetSpawner : MonoBehaviour{
    
    [SerializeField]
    private GameObject Target;
    public static Queue<Target> activeTargets;
    AudioSource mapMusic;
       

    [SerializeField]
    AudioClip clip;
    [SerializeField]
    Maps map = Maps.HOLD;
    Transform worldTransform; //global transform 

    //values that the Target script needs to access
    public static float combo;
    public static float score;
    public static float health;
    public static float difficultyMultiplier;
    public static float modMultiplier;
    public static float newComboDensity;
    public static float lowScoringHitsPerCombo;
    public static float totalTargetsElapsed;
    public static float netPercentDecimal;

    float percent;

    float elapsedTime;
    bool isLevelLoaded;


    public static float[] properties;
    private static float AR, OD, CS, HP;
    private static float preempt; //value for determining when it should actually spawn

    private DelayManager delays;
    private static TargetSpawner targetSpawner;

    // Start is called before the first frame update
    void Start(){
        targetSpawner = this;
        activeTargets = new Queue<Target>();
        percent = 0;
        health = 100;
        properties = MapsProperties.getMapProperties(map, Time.time);
        AR = TargetSpawner.properties[0];
        OD = TargetSpawner.properties[1];
        CS = TargetSpawner.properties[2];
        HP = TargetSpawner.properties[3];
        delays = new DelayManager(map);
        if(AR <= 5) {
            preempt = 1200f + 600f * (5f - AR) / 5f;
        } else {
            preempt = 1200f - 750f * (AR - 5f) / 5f;
        }
        preempt = preempt / 1000f; //millis to seconds
        nextSpawnTime = delays.targets.Peek().getDelay() - preempt;
        newComboDensity = getNewComboDensity(); // may not be necessary
        worldTransform = transform.transform;
    }

    static float elapsedTimeStatic;
    float nextSpawnTime;
    // Update is called once per frame
    void Update(){
        if (isLevelLoaded) {
            elapsedTime += Time.deltaTime;
            elapsedTimeStatic = elapsedTime;
            properties = MapsProperties.getMapProperties(map, Time.time);
            float healthLoss = determineHealthLoss() * Time.deltaTime;
            percent = getPercent();
            health -= healthLoss;
            if(health <= 0) {
                //Fail
                //SceneManager.LoadScene(3);
            }
            //Debug.Log("Update");
            if (delays.targets.Count != 0) {
                //Debug.Log("non 0 ");
                if (elapsedTime > nextSpawnTime) {
                    spawn();
                }
            }
            
            
        }
        
    }

    float getPercent()
    {
        return netPercentDecimal / totalTargetsElapsed;
    }

    /* Start at 100 health, x is hp drain level, 
     * a miss reduces hp by 3x, 50 reduces by x, 100 doesn't 
     * change hp, 300 increases by 5, 300 combo by 10, 100 combo by 5.
     * I would probably also add a formula to calculate x within a combo 
     * being like: x = 12.5(0.08hp)^min(n/5,2) where n is the amount of misses/50s + 1 
     * in the current combo, this makes it unlikely that one fudged combo (like one tiny stream) 
     * will instantly kill you. Sliders could optionally be incorporated to give/drain upon tick/reverse arrow.(Sliders are a feature I plan to add in a future iteration of the project
     */

    private float determineHealthLoss()
    {
        float HP = 0.08f * properties[3]; // used value for the base of the formula, not literal HP but for the purposes of this method 
        return (12.5f * Mathf.Pow(HP, Mathf.Min((lowScoringHitsPerCombo + 1f) / 5f, 2f))) / 10f;
        
    }

    private float getNewComboDensity()
    {
        float totalObjectCount = 0;
        float newComboCount = 0;
        float previousCombo = 1;
        List<string> lines = DelayManager.getInstanceDelayManager().lines;
        foreach (string current in lines) {
            string[] words = current.Split(' ');
            float numCombo = float.Parse(words[4]);
            if(numCombo != 1.0 && numCombo != previousCombo + 1) {
                throw new MapsMetaDataException();
            } else if(numCombo == 1){
                newComboCount++;
            }
            totalObjectCount++;
            previousCombo = numCombo;

        }

        return newComboCount / totalObjectCount;
    }

    public static float getDifficultyMultiplier()
    {
        float difficultyComponents = OD + CS + HP;
        if (difficultyComponents >= 0 && difficultyComponents <= 5) {
            return 2;
        } else if (difficultyComponents >= 6 && difficultyComponents <= 12) {
            return 3;
        } else if (difficultyComponents >= 13 && difficultyComponents <= 17) {
            return 4;
        } else if (difficultyComponents >= 18 && difficultyComponents <= 24) {
            return 5;
        } else if (difficultyComponents >= 25 && difficultyComponents <= 30) {
            return 6;
        }
        return 1;
    }

    //going to add more here for now mods are not a thing, but they will be so I wanted to put them in my algorithm
    public static float getModMultiplier()
    {
        return 1;
    }

    public static Target current;
    private void spawn()
    {
        Debug.Log("Spawn gets here");
        current = delays.targets.Dequeue();
        activeTargets.Enqueue(current);
        Vector3 playerPosition = Player.instance.feetPositionGuess;
        Vector3 targetPosition = current.getPosition();
        Vector3 actualTargetPosition = new Vector3(targetPosition.x + playerPosition.x, targetPosition.y + playerPosition.y, targetPosition.z);
        Instantiate(Target, actualTargetPosition, Quaternion.identity, worldTransform);
        if(delays.targets.Count != 0) {
            nextSpawnTime = delays.targets.Peek().getDelay() - preempt;
        }
        

    }

    public static float getElapsedTimeEeconds()
    {
        return elapsedTimeStatic;
    }

 
   void OnEnable(){
    SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
    SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        mapMusic = gameObject.AddComponent<AudioSource>();
        mapMusic.clip = clip;
        mapMusic.PlayScheduled(0);
        isLevelLoaded = true;
        GameObject PlayerObject = GameObject.Find("Player"); 
        PlayerObject.transform.position = start_position_for_level.transform.position; 
        PlayerObject.transform.eulerAngles = start_position_for_level.transform.eulerAngles;
    }

    public void TargetDestroy(GameObject target)
    {
        Destroy(target.gameObject);
    }
    
}
