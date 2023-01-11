using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    [SerializeField] private PlayerCombatManager _playerCombatManager;
    [SerializeField] private Transform _cam;
    private GameManager _gm => GameManager.Instance;
    private LayerMask _interactionMask;
    private Action _loop;
    private float _range,_radius,_radiusCooldown,radiusCD;

    private void Start()
    {
        _interactionMask = _gm.Everything;
        _loop += InteractableCheck;
    }
    private void Update()
    {
        _loop?.Invoke();
    }

    private void InteractableCheck()
    {
        if (radiusCD > 0) { radiusCD -= Time.deltaTime; }

        Interactable interacted;
        bool found = false;
        RaycastHit hit;
        if (Physics.Raycast(_cam.position, _cam.forward,out hit,_range, _interactionMask, QueryTriggerInteraction.Ignore)) 
        {
            interacted = hit.transform.GetComponent<Interactable>();
            if (interacted != null) found = true;
        }
        if (!found && radiusCD <= 0)
        {
            radiusCD = _radiusCooldown;
            Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);
            float dist = _radius + 1;
            //GameObject
           // for
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Icollectable Collectable = other.gameObject.GetComponent<Icollectable>();
        if (Collectable == null) return;

        if (Collectable is FruitCollectable)
        {
            FruitCollectable fruit = Collectable as FruitCollectable;
            if (_playerCombatManager.CollectFruit(fruit.Fruit, fruit.Amount))
            {
                Collectable.PickUp();
            }
        }
    }

}
