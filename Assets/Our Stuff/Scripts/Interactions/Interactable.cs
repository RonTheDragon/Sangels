using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    public string Information { get; set; }
    public Color TextColor { get; set; }

    public void Use();

    public bool CanUse();
}
