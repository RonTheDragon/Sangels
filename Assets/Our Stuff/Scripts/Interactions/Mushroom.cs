using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.VFX;

public class Mushroom : MonoBehaviour, Iinteractable
{
    [SerializeField] protected string _info = "Press [Use] to Wake Up Mushroom";

    [SerializeField] protected string _alreadyUsedInfo = "Awakened Mushroom";

    [SerializeField] VisualEffect _mushroomVFX;

    private string _asleepInfo => _info;
    string Iinteractable.Information { get { return _info; } set { _info = value; } }

    [SerializeField] protected Color _textColor = Color.white;
    Color Iinteractable.TextColor { get{ return _textColor; } set { _textColor = value; } }
    public float UseTime { get => 0; set => throw new System.NotImplementedException(); }
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
        _mushroomVFX.SendEvent("OnPlay");
    }

    public void UnUse() //for future Puzzle use
    {
        Awakened = false;
        _mycelium.TryOpen();
        _info = _asleepInfo;
        _mushroomVFX.SendEvent("OnStop");
    }

    public void SetupMushroom(Mycelium mycelium)
    {
        _mycelium = mycelium;
    }
}
