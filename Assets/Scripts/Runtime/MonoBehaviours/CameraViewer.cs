using Cinemachine;
using Interfaces;
using UnityEngine;

namespace Runtime.MonoBehaviours
{
    public class CameraViewer : MonoBehaviour, ICameraViewer
    {
        [SerializeField]
        private CinemachineTargetGroup targetGroup;

        public void AddToViewTarget(Transform targetTransform)
        {
            targetGroup.AddMember(targetTransform, 1, 0);
            Debug.Log($"Added {targetTransform} as target");
        }

        public void RemoveFromViewTarget(Transform targetTransform)
        {
            targetGroup.RemoveMember(targetTransform);
        }

        public void ClearTargetsList()
        {
            foreach (var targetGroupMTarget in targetGroup.m_Targets)
            {
                targetGroup.RemoveMember(targetGroupMTarget.target);
            }
            
            Debug.Log($"Cleared list");
        }
    }
}
