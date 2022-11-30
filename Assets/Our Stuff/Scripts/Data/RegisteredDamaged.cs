using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisteredDamaged
{
    public float TimeLeft { get; set; }
    public GameObject AttackedObject { get; set; }



    public RegisteredDamaged(float timeLeft,GameObject attackedObject) 
    {
        TimeLeft = timeLeft;
        AttackedObject = attackedObject;
    }



}
