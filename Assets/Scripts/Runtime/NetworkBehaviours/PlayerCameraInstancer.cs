using Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace MonoBehaviours.Network
{
    public class PlayerCameraInstancer : NetworkBehaviour
    {
        [SerializeField] 
        private GameObject CameraExample;
        
        
        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                GameObject camera = Instantiate(CameraExample, Vector3.zero, new Quaternion(0, 0, 0, 0));
                
                if (camera.GetComponentInChildren<CinemachineTargetGroup>())
                {
                    camera.GetComponentInChildren<CinemachineTargetGroup>().AddMember(transform, 1, 0);
                }                   
            }
            else
            {
                enabled = false;
            }
        }
    }
}
