using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class RandomSoundPlayer : MonoBehaviour
{
    private AudioSource[] m_audioSources;

    private void Start()
    {
        m_audioSources = GetComponentsInChildren<AudioSource>();
    }

    public void PlayRandomSound()
    {
        int randomInt = Random.Range(0, m_audioSources.Length);
        m_audioSources[randomInt].Play();
    }
}
