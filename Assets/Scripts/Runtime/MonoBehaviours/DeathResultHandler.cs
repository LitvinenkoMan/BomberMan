using NetworkBehaviours;
using Runtime.NetworkBehaviours;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.MonoBehaviours
{
    public class DeathResultHandler : NetworkBehaviour
    {
        [SerializeField]
        private GameObject visuals;
        [SerializeField]
        private PlayerMovement playerMovement;
        [SerializeField]
        private BombDeployer bombDeployer;
        [SerializeField]
        private BomberParamsProvider bomberParamsProvider;

        [Space(20)]
        public UnityEvent OnPlayerDeath;


        public void OnZeroHealthResponse()
        {
            OnZeroHealthResponseRpc();
        }
        
        [Rpc(SendTo.Server)]
        public void OnZeroHealthResponseRpc()
        {
            EnableViewerMode();
            
            OnPlayerDeath?.Invoke();
        }


        private void EnableViewerMode()
        {
            NetworkObject.Despawn();
            playerMovement.enabled = false;
            bombDeployer.enabled = false;
            bomberParamsProvider.enabled = false;
            visuals.SetActive(false);
        }
    }
}
