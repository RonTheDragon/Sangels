using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Combat : MonoBehaviour
{

    [HideInInspector] public LayerMask Attackable;
    protected GameManager GM => GameManager.instance;
    protected CombatManager attackManager => GetComponent<CombatManager>();
    
}
