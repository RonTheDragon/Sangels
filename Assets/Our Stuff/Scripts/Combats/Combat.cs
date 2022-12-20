using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Combat : MonoBehaviour
{
    [HideInInspector] public float DamageAmount;
    [HideInInspector] public float Knockback;
    [HideInInspector] public Vector2 Stagger;
    [HideInInspector] public float Fire;

    [HideInInspector] public LayerMask Attackable;
    protected GameManager GM => GameManager.instance;
    protected CombatManager attackManager => GetComponent<CombatManager>();
    
}
