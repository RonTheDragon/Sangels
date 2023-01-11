using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomBarrier : MonoBehaviour , Interactable
{
    [SerializeField] private string _info = "Wake up the Master Mushroom";
    string Interactable._information { get { return _info; } set { _info = value; } }

    [SerializeField] private Color _textColor = Color.white;
    Color Interactable._color { get { return _textColor; } set { _textColor = value; } }

    [SerializeField] private GameObject _barrier;

    public bool CanUse()
    {
        return false;
    }

    public void Use()
    {
        throw new System.NotImplementedException();
    }

    public void Open()
    {
        _barrier.SetActive(false);
    }
}
