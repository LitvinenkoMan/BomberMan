using UnityEngine;

namespace Interfaces
{
    public interface IBomb
    {
        public void PlaceBomb(Vector3 newPos);
        public void Ignite();
        public void Explode();
        public void Reset();
    }
}
