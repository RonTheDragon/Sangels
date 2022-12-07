using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damage : MonoBehaviour 
{
    [SerializeField] protected float DamageAmount;
    [SerializeField] protected float Knockback;
    [HideInInspector] public LayerMask Attackable;
    protected GameManager GM => GameManager.instance;
    protected AttackManager attackManager => GetComponent<AttackManager>();
}
