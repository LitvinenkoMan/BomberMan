using System;
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

        [Space(20)]
        public UnityEvent OnPlayerDeathUnityEvent;
        public event Action<ulong> OnPlayerDeathAction;


        public void OnZeroHealthResponse()
        {
            RiseDeathEventsRpc();
            EnableViewerMode();
        }

        [Rpc(SendTo.Everyone)]
        private void RiseDeathEventsRpc() 
        {
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
            playerMovement.enabled = false;         // this should be in movement
            bombDeployer.enabled = false;           // this should be in bomb deployer
            visuals.SetActive(false);               // this also should be not here
        }
    }
}
