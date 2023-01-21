using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class HealingBlast : MonoBehaviour
{
    [SerializeField] private VisualEffect Petals;
    public float HealAmount;
    public float Radius;
    public LayerMask Healable;
    public void Explode()
    {
        Petals.SendEvent("OnPlay");
        Collider[] Healed = Physics.OverlapSphere(transform.position, Radius, Healable);
        foreach (Collider c in Healed)
        {
            CharacterHealth h = c.GetComponent<CharacterHealth>();
            if (h != null)
            {
                h.HealInstantly(HealAmount);
            }
        }
    }
}
