using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomBarrier : MonoBehaviour , Iinteractable
{
    [SerializeField] protected string _info = "Wake up the Master Mushroom";
    string Iinteractable.Information { get { return _info; } set { _info = value; } }

    [SerializeField] protected Color _textColor = Color.white;
    Color Iinteractable.TextColor { get { return _textColor; } set { _textColor = value; } }

    public float UseTime { get => 0; set => throw new System.NotImplementedException(); }

    [SerializeField] protected GameObject _barrier;

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
