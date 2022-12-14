using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerRegistration : MonoBehaviour
{
    [HideInInspector] public LayerMask Attackable;
    private List<RegisteredDamaged> _registeredDamagedList = new List<RegisteredDamaged>();
    [SerializeField] private MeleeDamage _meleeDamage;

    private void Awake()
    {
        _meleeDamage.AddTrigger(this);
    }
    private void Update()
    {
        TimeManager();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (Attackable == (Attackable | (1 << other.gameObject.layer)))
        {
            if (!IsInList(other.gameObject))
            {
                Health mom = other.gameObject.GetComponent<Health>();
                if (mom != null)
                {
                _registeredDamagedList.Add(_meleeDamage.SubmitToRegisteredObjects(mom));
                }
            }
        }
    }


    private void TimeManager() 
    {
        if (_registeredDamagedList.Count > 0)
        {
            List<RegisteredDamaged> registeredDamagedList = new List<RegisteredDamaged>(_registeredDamagedList);
            foreach (RegisteredDamaged registeredDamaged in registeredDamagedList)
            {
                if (registeredDamaged.TimeLeft >= 0)
                    registeredDamaged.TimeLeft -= Time.deltaTime;
                else
                    _registeredDamagedList.Remove(registeredDamaged);
            }
        }
    }

    private bool IsInList(GameObject AttackedObject) 
    {
        if(_registeredDamagedList!=null)
        foreach (RegisteredDamaged attackedObject in _registeredDamagedList)
            if (attackedObject.AttackedObject == AttackedObject)
                return true;
        return false;
    }



}
