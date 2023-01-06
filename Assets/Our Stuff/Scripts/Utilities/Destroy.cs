using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    [SerializeField] private float _timer;
    // Start is called before the first frame update
    private void Start()
    {
        Destroy(gameObject, _timer);
    }
}
