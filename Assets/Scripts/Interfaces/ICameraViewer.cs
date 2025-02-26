using UnityEngine;

namespace Interfaces
{
    public interface ICameraViewer
    {
        public void AddToViewTarget(Transform targetTransform);
        public void RemoveFromViewTarget(Transform targetTransform);
        public void ClearTargetsList();
    }
}
