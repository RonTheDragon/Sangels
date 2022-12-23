using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

    public class TrackProjectile : ImpactProjectile
    {

        Vector3 _currentVelocity;
        quaternion _currentRotation;

        private void Update()
        {
            _currentVelocity = rb.velocity;
            _currentRotation = transform.rotation;
        }



        protected override void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);
            rb.velocity = _currentVelocity;
            transform.rotation = _currentRotation;
        }

    }
