using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOff : MonoBehaviour
{
    private float _time;
 
    public void Use(float time = 0)
    {
        _time = time;
        StartCoroutine("Remove");
    }

    private IEnumerator Remove()
    {
        yield return new WaitForSeconds(_time);
        gameObject.SetActive(false);
    }
}
