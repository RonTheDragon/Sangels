using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class DeadPlayer : MonoBehaviour, Iinteractable
{
    [SerializeField] protected string _info = "Press [Interact Button] to Heal Player";
    [SerializeField] protected Color _textColor = Color.white;
    PlayerHealth _playerHealth=>GetComponent<PlayerHealth>();
    string Iinteractable.Information { get { return _info; } set { _info = value; } }
    Color Iinteractable.TextColor { get { return _textColor; } set { _textColor = value; } }

    public Action<string, Color> OnRevivingRange;


    public bool CanUse()
    {
        OnRevivingRange?.Invoke(_info, _textColor);
        return true;
    }

    public virtual void Use()
    {
        
        _playerHealth.RevivePlayer(30);
        Destroy(this);
    }
}
