using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveHealing : MonoBehaviour
{
    [HideInInspector] public GameObject Shooter;
    [SerializeField] private float _healRadius, _lureRadius, _healAmount, _duration;
    [SerializeField] private string _cloud;
    [HideInInspector] public LayerMask LureMask, HealMask;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != Shooter)
        {
            HealingCloud cloud = ObjectPooler.Instance.SpawnFromPool(_cloud, transform.position, transform.rotation).GetComponent<HealingCloud>();
            cloud.HealRadius = _healRadius;
            cloud.LureRadius = _lureRadius;
            cloud.HealAmount = _healAmount;
            cloud.Duration = _duration;
            cloud.HealMask = HealMask;
            cloud.LureMask= LureMask;
            cloud.Explode();
            gameObject.SetActive(false);
        }
    }

    
}

