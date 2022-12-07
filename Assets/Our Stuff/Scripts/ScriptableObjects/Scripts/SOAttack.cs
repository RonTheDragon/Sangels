using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public abstract class SOAttack : ScriptableObject
{
    public float MinDist = 0;
    public float MaxDist = 3;
    public string AnimationName;
}
