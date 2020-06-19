using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapsProperties{

    /* this class is a way to put most of my hardcoding into one place, as well as explaining some of the terms that are used inside this program
     * Approach Rate (AR) how fast the target gets to the 300 value, you can see the math of how it is applied in the TargetComponents class
     * Overall Difficulty (OD) tells you how hard the map is in general, it is used to determine the leniancy of the timings of the various notes
     * Health drain (HP) how quickly you lose health, the game is constantly decrimenting your health and by hitting targets you are increasing it.
     * Map Length in seconds, pretty self explanatory, it is a component to the health loss algorithm // NOT YET
     * finally Sphere Size (CS) tells you how big each of the targets is going to get throughout the course of it's dilation, note this has a conversion system to dilation, also it has a range of [2-7]
     * 
     */

    /*
     *The elements in the Array are specific numbers affiliated with the map, note these are NOT final. 
     * 0, AR
     * 1, OD
     * 2, CS
     * 3, HP
     * 
     */
    public static float[] getMapProperties(Maps map, float time){
        //time is here in case I may want these numbers to change during certain points in the song, for example somewhere like a beat drop the AR may go up to increase the intensity of the song
        float[] mapProperties = new float[5];
        if (map == Maps.A_FOOL_MOON_NIGHT) { 
            mapProperties[0] = 9;
            mapProperties[1] = 7;
            mapProperties[2] = 4;
            mapProperties[3] = 5;
            mapProperties[4] = 234;
        }else if(map == Maps.HOLD) {
            mapProperties[0] = 9.5f;
            mapProperties[1] = 8;
            mapProperties[2] = 4.2f;
            mapProperties[3] = 7;
            mapProperties[4] = 125;
        }
        Debug.Assert(mapProperties[0] >= 1 && mapProperties[0] <= 10);
        Debug.Assert(mapProperties[1] >= 1 && mapProperties[1] <= 10);
        Debug.Assert(mapProperties[2] >= 1 && mapProperties[2] <= 10);
        Debug.Assert(mapProperties[3] >= 2 && mapProperties[3] <= 7);
        Debug.Assert(mapProperties[4] > 0); //may want to add to this after a little bit if I wawnt to actually implement an editor

        return mapProperties;
    }   

    /*
     * Given the CS I want to find the number to dilate to
     * 4, 1
     * 7, 2
     * 2, 0.33
     * using these points I did some math to find a line that fit them, came out to y = 1/3x - 1/3
     */
    public static float SphereSizeToScale(float CS)
    {
        return CS * (1f / 3f) - (1f / 3f);
    }
}
