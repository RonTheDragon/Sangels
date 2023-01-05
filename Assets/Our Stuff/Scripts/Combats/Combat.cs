using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Combat : MonoBehaviour
{
    protected GameManager _gameManager => GameManager.Instance;
    protected CombatManager _attackManager => GetComponent<CombatManager>();
    
}
