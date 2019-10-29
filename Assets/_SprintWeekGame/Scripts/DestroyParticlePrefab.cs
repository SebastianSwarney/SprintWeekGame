using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticlePrefab : MonoBehaviour
{
    ParticleSystem partSyst;

    private void Start()
    {
        partSyst = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (partSyst)
        {
            if(!partSyst.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}
