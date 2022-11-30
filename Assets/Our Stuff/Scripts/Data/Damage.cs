using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damage : MonoBehaviour 
{
    [SerializeField] float DamageAmount;
    [SerializeField] float Knockback;
    [HideInInspector] public LayerMask Attackable;
}
