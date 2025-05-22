using System;
using Core.ScriptableObjects;
using Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerHealthNet : NetworkBehaviour, IHealth
    {
        [SerializeField]
        private ActorBaseParams baseParams;
        
        public event Action<int> OnHealthChanged;
        public event Action OnHealthRunOut;

        public override void OnNetworkSpawn()
        {
            Initialize(0);
        }

        public void Initialize(float initialValue) { }

        public void AddHealth(int healthToAdd)
        {
            AddHealthRpc(healthToAdd, RpcTarget.Single(OwnerClientId, RpcTargetUse.Temp));
        }

        public void SubtractHealth(int healthToSubtract)
        {
            AddHealthRpc(-healthToSubtract, RpcTarget.Single(OwnerClientId, RpcTargetUse.Temp));
        }

        [Rpc(SendTo.SpecifiedInParams)]
        private void AddHealthRpc(int healthToAdd, RpcParams rpcParams = default)
        {
            baseParams.SetActorHealth(baseParams.ActorHealth + healthToAdd);

            OnHealthChangedRpc(healthToAdd);

            if (baseParams.ActorHealth <= 0)
            {
                OnHealthRunOutRpc();
            }
        }

        [Rpc(SendTo.Everyone)]
        private void OnHealthRunOutRpc()
        {
            OnHealthRunOut?.Invoke();
        }

        [Rpc(SendTo.Everyone)]
        private void OnHealthChangedRpc(int newHealth)
        {
            OnHealthChanged?.Invoke(newHealth);
        }

        public int GetHealth()
        {
            return baseParams.ActorHealth;
        }
    }
}
