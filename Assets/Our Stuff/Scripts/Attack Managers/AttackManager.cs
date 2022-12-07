using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackManager : MonoBehaviour
{
    [HideInInspector]
    public bool _melee;
    [HideInInspector]
    public Action Loop;
    public Animator anim => GetComponent<Animator>();

    [HideInInspector] public LayerMask Attackable;

}
