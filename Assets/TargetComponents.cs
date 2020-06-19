using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;
using System;
using System.Runtime.CompilerServices;

public class TargetComponents : MonoBehaviour
{

    
    static bool isOriginal = true;
    bool canDestroy;

    Vector3 expansionRate;
    Vector3 startPosition;
    GameObject thisTarget;
    int maxActiveFrames;
    float dilationRate;
    float currentTimeMillis;


    float comboNum;
    float nextComboNum;

    float difficultyMultiplier;
    float modMultiplier; //not implemented yet


    float threeHundredError;
    float oneHundredError;
    float fiftyError;
    float activeTimeMillis;


    //map properties components
    float AR;
    float OD;
    float CS;
    float HP;

    static bool cycleTerminate;
    private float startTimeMillis;
    Transform childTransform;

    Hand leftHand, rightHand;
    static TargetComponents currentTarget;

    SteamVR_Action_Boolean isPinched;
    


    /*
* AR up to 5	1800 - (AR x 120)
* AR above 5	1200 - ((AR - 5) x 150)
* osu! 300 window	79 - (OD x 6) + 0,5
* osu! 100 window	139 - (OD x 8) + 0,5
* osu! 50 window	199 - (OD x 10) + 0,5
* Circle Size (CS)	32 x (1 - 0,7 x (CS - 5) / 5)
*/

    /* https://osu.ppy.sh/help/wiki/Score/
     * reference for where the scoring system was taken from
     * 
     * The speed at which the HP drains is directly proportional to the value of the HP Drain, 
     * and how big the gaps between the notes are. Whereas it is inversely proportional to length of the map and the NC (New Combo) density.
     * 
     * 
     */



    private void Awake()
    {
        isPinched = SteamVR_Actions._default.GrabPinch;
        leftHand = Player.instance.leftHand;
        rightHand = Player.instance.rightHand;
    }

    

    // Start is called before the first frame update
    void Start()
    {
        canDestroy = !isOriginal;
        isOriginal = false;
        
        Maps currentMap = DelayManager.getInstanceDelayManager().map;
        AR = TargetSpawner.properties[0];
        OD = TargetSpawner.properties[1];
        CS = TargetSpawner.properties[2];
        HP = TargetSpawner.properties[3];

        thisTarget = this.gameObject;
        //childTransform = thisTarget.transform.GetChild(0);
        startPosition = transform.localPosition;

        TargetSpawner.modMultiplier = TargetSpawner.getModMultiplier();

        //Why the AR system works the way it does https://osu.ppy.sh/help/wiki/Beatmapping/Approach_rate
        if (AR <= 5) {
            activeTimeMillis = 1800 - (AR * 120);
        } else {
            activeTimeMillis = 1200 - ((AR - 5) * 150);
        }
        TargetSpawner.modMultiplier = TargetSpawner.getModMultiplier();
        getTimingIntervals();
        float size = MapsProperties.SphereSizeToScale(CS);
        float dilationRate = size / activeTimeMillis; //dilation rate per Milli
        float ratePerSecond = dilationRate * 1000f;
        thisTarget.transform.localScale = new Vector3(0, 0, 0);
        expansionRate = new Vector3(ratePerSecond, ratePerSecond, ratePerSecond);
        currentTarget = this;
        if (!canDestroy) {
            thisTarget.transform.localScale = new Vector3(0, 0, 0);
            expansionRate = new Vector3(0, 0, 0);
        }
        cycleTerminate = false;
        startTimeMillis = Time.timeSinceLevelLoad * 1000f;
        miss = false;
    }


    private void getTimingIntervals()
    {
        fiftyError = 198f - (OD * 10f);
        oneHundredError = 138f - (OD * 8f);
        threeHundredError = 78f - (OD * 6f);
        /* X = (78 - 6*OD) ms 300 error 
         * Y = (138 - 8*OD) ms 100 error
         * Z = (198 - 10*OD) ms 50 error
         * 
         * 
         */
    }

    bool miss;


    float elapsedTime = 0;
    int updateCounter;//debugging variable
    
    bool prevLeftHandState;
    bool prevRightHandState;
    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime * 1000f; //millis
        if (!canDestroy) {
            return;
        }
        updateCounter++;
        //Debug.Log("Expansion Rate: " + expansionRate);
        thisTarget.transform.localScale += expansionRate * Time.deltaTime;
        currentTimeMillis += Time.deltaTime * 1000f;
        if (currentTimeMillis > activeTimeMillis) {
            //miss
            miss = true;
            Debug.Log("slow miss");
            //Debug.Log(updateCounter);
            //expansionRate = new Vector3(0, 0, 0);
            //SendMessageUpwards("destroyTarget", thisTarget, SendMessageOptions.DontRequireReceiver);    
            if (canDestroy) {
                Destroy(thisTarget);
            }
            //enabled = false;
        }
        //Debug.Log("left: " + isPinched.GetState(SteamVR_Input_Sources.LeftHand));
        //Debug.Log("right: " + isPinched.GetState(SteamVR_Input_Sources.RightHand));

        bool leftHandState = isPinched.GetState(SteamVR_Input_Sources.LeftHand);
        bool rightHandState = isPinched.GetState(SteamVR_Input_Sources.RightHand);
        if ((leftHandState != prevLeftHandState && leftHandState && Player.instance.leftHand.hoveringInteractable != null) || (rightHandState != prevRightHandState && rightHandState && Player.instance.rightHand.hoveringInteractable != null)) {
            cycleTerminate = true;
            Debug.Log("Hit!");
            //Debug.Log(updateCounter);
            TargetSpawner.difficultyMultiplier = TargetSpawner.getDifficultyMultiplier();
            //enabled = false;
            //SendMessageUpwards("destroyTarget", thisTarget, SendMessageOptions.DontRequireReceiver);
            if (canDestroy) { 
            Destroy(thisTarget);
            }
        }
        //Debug.Log(cycleTerminate);
        //Debug.Log(updateCounter);
        prevLeftHandState = isPinched.GetState(SteamVR_Input_Sources.LeftHand);
        prevRightHandState = isPinched.GetState(SteamVR_Input_Sources.RightHand);

    }


    public void OnDestroy()
    {
        float error = Math.Abs((TargetSpawner.activeTargets.Dequeue().getDelay() * 1000f) - currentTimeMillis - startTimeMillis);
        Debug.Log("error" + error);
        float hitValue;
        if (miss) {
            //miss
            TargetSpawner.combo = 0;
            hitValue = 0;
            TargetSpawner.health -= 3 * HP;
        } else if (error <= threeHundredError) {
            //player has got a 300 perfect
            Debug.Log("300");
            TargetSpawner.combo++;
            TargetSpawner.health += 5;
            if (isComboEnd()) {
                TargetSpawner.health += 10;
            }
            TargetSpawner.netPercentDecimal += 1;
            hitValue = 300;
        } else if (error <= oneHundredError && error > threeHundredError) {
            //player has got a 100
            Debug.Log("100");
            TargetSpawner.combo++;
            if (isComboEnd()) {
                TargetSpawner.health += 5;
            }
            TargetSpawner.netPercentDecimal += (1f / 3f);
            hitValue = 100;
        } else if (error <= fiftyError && error > oneHundredError) {
            //player has got a 50
            Debug.Log("50");
            TargetSpawner.combo++;
            TargetSpawner.health -= HP;
            TargetSpawner.lowScoringHitsPerCombo++;
            TargetSpawner.netPercentDecimal += (1f / 6f);
            hitValue = 50;
        } else {
            //miss
            Debug.Log("fast miss");
            TargetSpawner.combo = 0;
            hitValue = 0;
            TargetSpawner.health -= 3 * HP;
        }
        if (isComboEnd()) {
            TargetSpawner.lowScoringHitsPerCombo = 0;
        }
        //Debug.Log("score: " + TargetSpawner.score);
        TargetSpawner.score += hitValue + (hitValue * ((Mathf.Max(TargetSpawner.combo - 1f, 0) * TargetSpawner.difficultyMultiplier * TargetSpawner.modMultiplier) / 25f));
        //Debug.Log("score: " + TargetSpawner.score);
    }

    


    private bool isComboEnd()
    {
        return nextComboNum == 1 || nextComboNum == -1;
    }

    public static TargetComponents getCurrentTarget()
    {
        return currentTarget;
    }


}
