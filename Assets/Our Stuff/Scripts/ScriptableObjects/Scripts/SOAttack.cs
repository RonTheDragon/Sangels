using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public abstract class SOAttack : ScriptableObject
{
    public float DamageAmount;
    public float Knockback;
    public Vector2 Stagger;
    public float MinDist = 0;
    public float MaxDist = 3;
    public string AnimationName;
    public float UsingTime;
    public float speedWhileUsing;
}
