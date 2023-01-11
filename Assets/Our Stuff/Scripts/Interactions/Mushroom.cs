using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour, Interactable
{
    [SerializeField] private string _info;
    string Interactable._information { get { return _info; } set { _info = value; } }

    [SerializeField] private Color _textColor;
    Color Interactable._color { get{ return _textColor; } set { _textColor = value; } }

    public bool CanUse()
    {
        throw new System.NotImplementedException();
    }

    public void Use()
    {
        throw new System.NotImplementedException();
    }

    public void WakeUp()
    {

    }
}
