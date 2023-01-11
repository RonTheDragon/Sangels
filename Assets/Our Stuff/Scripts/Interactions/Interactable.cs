using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    protected string _information { get; set; }
    protected Color _color { get; set; }

    public void Use();

    public bool CanUse();
}
