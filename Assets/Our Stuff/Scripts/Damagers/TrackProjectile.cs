using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

    public class TrackProjectile : ImpactProjectile
    {

        private Vector3 _currentVelocity;
        private quaternion _currentRotation;

        private void Update()
        {
            _currentVelocity = _rigidBody.velocity;
            _currentRotation = transform.rotation;
        }



        protected override void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);
            _rigidBody.velocity = _currentVelocity;
            transform.rotation = _currentRotation;
        }

    }
