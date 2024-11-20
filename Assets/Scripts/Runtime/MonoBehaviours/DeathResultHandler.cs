using System;
using NetworkBehaviours;
using Runtime.NetworkBehaviours;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

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

        [Space(20)]
        public UnityEvent OnPlayerDeathUnityEvent;
        public Action<ulong> OnPlayerDeathAction;


        public void OnZeroHealthResponse()
        {
            EnableViewerMode();
            
            OnPlayerDeathUnityEvent?.Invoke();
            OnPlayerDeathAction?.Invoke(NetworkObject.OwnerClientId);
        }
        
        [Rpc(SendTo.Server)]
        public void OnZeroHealthResponseRpc()
        {
            OnZeroHealthResponse();
        }

        private void EnableViewerMode()
        {
            NetworkObject.Despawn();
            playerMovement.enabled = false;
            bombDeployer.enabled = false;
            visuals.SetActive(false);
        }
    }
}
