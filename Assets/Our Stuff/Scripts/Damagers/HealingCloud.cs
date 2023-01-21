using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class HealingCloud : MonoBehaviour
{
    public float HealRadius, LureRadius, HealAmount, Duration;
    public LayerMask LureMask, HealMask;
    [SerializeField] private VisualEffect Petals;
    private bool Active;

    [SerializeField] float _lureCooldown=1, _healCooldown=1;
    private float _lureCD, _healCD;

    public virtual void Explode()
    {
        StartCoroutine("Poof");
    }

    private void Update()
    {
        if (Active)
        {
            if (_lureCD < 0)
            {
                _lureCD = _lureCooldown;
                Collider[] Lured = Physics.OverlapSphere(transform.position, LureRadius, LureMask);
                foreach (Collider c in Lured)
                {
                    AIController a = c.GetComponent<AIController>();
                    if (a != null)
                    {
                        a.SetLure(transform.position, _lureCooldown+1);
                    }
                }
            }
            if (_healCD<0)
            {
                _healCD = _healCooldown;
                Collider[] Healed = Physics.OverlapSphere(transform.position, HealRadius, HealMask);
                foreach (Collider c in Healed)
                {
                    CharacterHealth h = c.GetComponent<CharacterHealth>();
                    if (h != null)
                    {
                        h.Heal(HealAmount, _healCooldown);
                    }
                }
            }
            _lureCD -= Time.deltaTime;
            _healCD -= Time.deltaTime;
        }
    }

    private IEnumerator Poof()
    {
        Petals.SendEvent("OnPlay");
        Active = true;
        yield return new WaitForSeconds(Duration);
        Petals.SendEvent("OnStop");
        Active = false;
    }
}
