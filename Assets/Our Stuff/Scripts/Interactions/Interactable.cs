using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Iinteractable
{
    public string Information { get; set; }
    public Color TextColor { get; set; }

    public float UseTime { get; set; }

    public void Use();

    public bool CanUse();
}
