using System.Collections.Generic;
using UnityEngine;

namespace AsteraX.Application.Game.Asteroids
{
    public class AsteroidChildContainer : MonoBehaviour
    {
        private readonly List<AsteroidPhysicsBody> _children = new List<AsteroidPhysicsBody>();

        public void Add(AsteroidPhysicsBody child)
        {
            _children.Add(child);
        }

        public void DetachChildren()
        {
            foreach (var child in _children)
            {
                child.transform.SetParent(transform.parent);
                var childPosition = child.transform.position;
                childPosition.z = transform.position.z;
                child.transform.position = childPosition;
                var velocityDirection = (childPosition - transform.position).normalized;
                child.Enable();
                child.SetVelocity(velocityDirection);
            }
        }
    }
}