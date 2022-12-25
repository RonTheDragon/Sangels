using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateUI : MonoBehaviour
{
    public static GameObject UIGameObject;

    // Method to toggle the game object
    public void ToggleGameObject()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
