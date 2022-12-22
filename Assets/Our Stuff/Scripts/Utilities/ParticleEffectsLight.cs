using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectsLight : MonoBehaviour
{
    Light _light => GetComponent<Light>();
    [SerializeField] private ParticleSystem particle;

    [SerializeField] private float speedMultiplier = 1;
    [SerializeField] private float LightMultiplier = 1;

    // Update is called once per frame
    void Update()
    {
        if (particle != null)
        {
            float emissionRate;
            if (particle.isEmitting)
            {
                emissionRate = particle.emission.rateOverTime.constant * LightMultiplier;
            }
            else
            {
                emissionRate = 0;
            }
            // Set the intensity of the light based on the emission rate
            _light.intensity = Mathf.Lerp(_light.intensity, emissionRate, speedMultiplier*Time.deltaTime);
        }
    }
}
