using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour, Interactable
{
    [SerializeField] private string _info = "Press E to Wake Up Mushroom";
    string Interactable._information { get { return _info; } set { _info = value; } }

    [SerializeField] private Color _textColor = Color.white;
    Color Interactable._color { get{ return _textColor; } set { _textColor = value; } }

    private Mycelium _mycelium1;

    [ReadOnly] public bool Awoken;

    public virtual bool CanUse()
    {
        return true;
    }

    public virtual void Use()
    {
        WakeUp();
    }

    public void WakeUp()
    {
        Awoken= true;
    }

    public void SetupMushroom(Mycelium mycelium)
    {
        _mycelium1 = mycelium;
    }
}
