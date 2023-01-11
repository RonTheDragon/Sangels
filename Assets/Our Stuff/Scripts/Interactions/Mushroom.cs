using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour, Iinteractable
{
    [SerializeField] protected string _info = "Press [Interact Button] to Wake Up Mushroom";

    [SerializeField] protected string _alreadyUsedInfo = "Awakened Mushroom";
    string Iinteractable.Information { get { return _info; } set { _info = value; } }

    [SerializeField] protected Color _textColor = Color.white;
    Color Iinteractable.TextColor { get{ return _textColor; } set { _textColor = value; } }

    protected Mycelium _mycelium;

    [ReadOnly] public bool Awakened;

    public virtual bool CanUse()
    {
        if (Awakened) return false;
        return true;
    }

    public virtual void Use()
    {
        Awakened = true;
        _mycelium.TryOpen();
        _info = _alreadyUsedInfo;
    }

    public void SetupMushroom(Mycelium mycelium)
    {
        _mycelium = mycelium;
    }
}
