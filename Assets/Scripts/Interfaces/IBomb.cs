using UnityEngine;

namespace Interfaces
{
    interface IBomb
    {
        public void PlaceBomb(Vector3 newPos);
        public void Ignite();
        public void Explode();
        public void Reset();
    }
}
