using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveMods : MonoBehaviour
{

    List<Mods> activeMods;
    // Start is called before the first frame update
    void Start()
    {
        activeMods = new List<Mods>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    enum Mods
    {
        DOUBLE_TIME, 
        HIDDEN,
        HARD_ROCK,
        FLASHLIGHT,


    }
}
