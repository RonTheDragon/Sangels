using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectsLight : MonoBehaviour
{
    private Light _light => GetComponent<Light>();
    [SerializeField] private ParticleSystem _particle;

    [SerializeField] private float _speedMultiplier = 1;
    [SerializeField] private float _lightMultiplier = 1;

    // Update is called once per frame
    private void Update()
    {
        if (_particle != null)
        {
            float emissionRate;
            if (_particle.isEmitting)
            {
                emissionRate = _particle.emission.rateOverTime.constant * _lightMultiplier;
            }
            else
            {
                emissionRate = 0;
            }
            // Set the intensity of the light based on the emission rate
            _light.intensity = Mathf.Lerp(_light.intensity, emissionRate, _speedMultiplier*Time.deltaTime);
        }
    }
}
