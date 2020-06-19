using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusicPlayer : MonoBehaviour
{
    
    AudioSource backGroundMusic;
    // Start is called before the first frame update
    void Start()
    {
        backGroundMusic = gameObject.AddComponent<AudioSource>();
        backGroundMusic.PlayOneShot((AudioClip)Resources.Load(@"C:\Users\fizzi\Blossoming Beats\Assets\Playlist\TitleScreenMusic.MP3"));
        backGroundMusic.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
