using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private Vector3 _rotating = new Vector3();

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(_rotating * Time.deltaTime);
    }
}
