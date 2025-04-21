using UnityEngine;

namespace Interfaces
{
    public interface IMovable
    {
        public void Initialize();
        public void Move(Vector3 direction);
        public void SetAbilityToMove(bool canIt);
    }
}
