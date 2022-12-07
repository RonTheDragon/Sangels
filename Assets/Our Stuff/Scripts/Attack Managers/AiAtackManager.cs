using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AiAtackManager : AttackManager
{

    [ReadOnly] public Transform Target;


    public void AttackTarget()
    {
        OverrideToAttack();


    }
    void OverrideToAttack()
    {

    }
}

