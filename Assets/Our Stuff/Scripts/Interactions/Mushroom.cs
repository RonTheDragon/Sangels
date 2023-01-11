using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour, Iinteractable
{
    [SerializeField] protected string _info = "Press E to Wake Up Mushroom";
    string Iinteractable.Information { get { return _info; } set { _info = value; } }

    [SerializeField] protected Color _textColor = Color.white;
    Color Iinteractable.TextColor { get{ return _textColor; } set { _textColor = value; } }

    protected Mycelium _mycelium;

    [ReadOnly] public bool Awoken;

    public virtual bool CanUse()
    {
        return true;
    }

    public virtual void Use()
    {
        WakeUp();
        _mycelium.TryOpen();
    }

    public void WakeUp()
    {
        Awoken= true;
    }

    public void SetupMushroom(Mycelium mycelium)
    {
        _mycelium = mycelium;
    }
}
