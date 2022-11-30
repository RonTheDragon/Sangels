using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiHealth : Health
{

    public void Dead() 
    {
        if (CurrentHealth < 0) 
        {
            gameObject.SetActive(false);
        }
    }
    
}
