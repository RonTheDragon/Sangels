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

    public bool CanUse()
    {
        if(_playerHealth.IsDead)
        return true;
        Debug.Log("");
        return false;

    }

    public virtual void Use()
    {
        _playerHealth.RevivePlayer(30);
        Debug.Log("RevivingPlayer hahahash");
    }
}
