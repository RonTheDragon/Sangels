using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damage : MonoBehaviour 
{
    public float DamageAmount;
    public float Knockback;
    public Vector2 Stagger;
    public float Fire;
    [HideInInspector] public LayerMask Attackable;

}
