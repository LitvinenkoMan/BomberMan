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
        [SerializeField]
        private BomberParamsProvider bomberParamsProvider;

        [Space(20)]
        public UnityEvent OnPlayerDeathUnityEvent;
        public Action<ulong> OnPlayerDeathAction;


        public void OnZeroHealthResponse()
        {
            OnZeroHealthResponseRpc();
        }
        
        [Rpc(SendTo.Server)]
        public void OnZeroHealthResponseRpc()
        {
            EnableViewerMode();
            
            OnPlayerDeathUnityEvent?.Invoke();
            OnPlayerDeathAction?.Invoke(NetworkObject.OwnerClientId);
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
