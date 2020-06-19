using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DisplayScore : MonoBehaviour
{

    [SerializeField]
    Text scoreBox;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreBox.text = "Score: " + TargetSpawner.score + "\n" + "Combo: " + TargetSpawner.combo + "\n" + "Health: " + TargetSpawner.health ;
    }
}
