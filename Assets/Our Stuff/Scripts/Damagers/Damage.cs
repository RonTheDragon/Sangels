using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damage : MonoBehaviour 
{
    public float DamageAmount;
    public float Knockback;
    public Vector2 Stagger;
    [HideInInspector] public LayerMask Attackable;
    protected GameManager GM => GameManager.instance;
    protected AttackManager attackManager => GetComponent<AttackManager>();
}
