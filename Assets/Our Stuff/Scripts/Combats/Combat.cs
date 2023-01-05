using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Combat : MonoBehaviour
{
    protected GameManager _gameManager => GameManager.instance;
    protected CombatManager _attackManager => GetComponent<CombatManager>();
    
}
