using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoungeMusicPlayer : MonoBehaviour
{

    AudioSource backGroundMusic;
    List<string> paths;

    // Start is called before the first frame update
    void Start()
    {
        paths = new List<string>();
        paths.Add(@"Assets/Playlist/Marvin Divine - Pretty Lights.mp3");
        paths.Add(@"Assets/Playlist/Sun.mp3");
        backGroundMusic = gameObject.AddComponent<AudioSource>();
        float randValue = Random.value;
        if(randValue >= 0.5) {
            backGroundMusic.PlayOneShot((AudioClip)Resources.Load(@"C:\Users\fizzi\Blossoming Beats\Assets\Playlist\Marvin Divine - Pretty Lights.MP3"));
        } else {
            backGroundMusic.PlayOneShot((AudioClip)Resources.Load(@"C:\Users\fizzi\Blossoming Beats\Assets\Playlist\Sun.MP3"));
        }
       

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
