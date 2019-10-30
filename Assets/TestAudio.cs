using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAudio : MonoBehaviour
{
    AudioSource audioSourceComponent;
    public AudioClip backgroundMusic;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSourceComponent = gameObject.GetComponent<AudioSource>();
        audioSourceComponent.PlayOneShot(backgroundMusic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
