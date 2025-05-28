using System;
using Core.ScriptableObjects;
using ScriptableObjects;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.MonoBehaviours
{
    public class BomberParamsProvider : NetworkBehaviour
    {
        [SerializeField]
        private TMP_Text PlayerName;
        [SerializeField]
        private BaseBomberParameters BaseBomberParams;
        
        private BomberParams _params;
        
        public event Action<BomberParams> OnBomberParamsChanged;



        private void Start()
        {
            if (!IsOwner)
            {
                enabled = false;
            }
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                _params = new BomberParams(BaseBomberParams.SpeedMultiplier, BaseBomberParams.BombsAtTime,
                    BaseBomberParams.BombsSpreading, BaseBomberParams.BombsCountdown, BaseBomberParams.BombsDamage);
            }
            
            
            
            name = $"P{GetComponent<NetworkObject>().OwnerClientId}";
            PlayerName.text = name;
        }

        public BomberParams GetBomberParams()
        {
            return _params;
        }

        protected override void OnOwnershipChanged(ulong previous, ulong current)
        {
            name = $"P{GetComponent<NetworkObject>().OwnerClientId}";
            PlayerName.text = name;
        }

        [ClientRpc]
        public void ResetLocalValuesClientRpc()
        {
            if (IsOwner)
            {
                BaseBomberParams.ResetValues();
            }
        }

        [Rpc(SendTo.Everyone)]
        private void RiseParamsChangedEventRpc(BomberParams bomberParams)
        {
            OnBomberParamsChanged?.Invoke(bomberParams);
        }
    }

    public struct BomberParams : INetworkSerializable 
    {
        private float _speedMultiplier;
        private float _bombsAtTime;
        private float _bombsSpreading;
        private float _bombsCountdown;
        private float _bombsDamage;

        public float SpeedMultiplier => _speedMultiplier;

        public float BombsAtTime => _bombsAtTime;

        public float BombsSpreading => _bombsSpreading;

        public float BombsCountdown => _bombsCountdown;

        public float BombsDamage => _bombsDamage;

        public BomberParams(float speedMultiplier, int bombsAtTime, int bombsSpreading, float bombsCountdown,
            int bombsDamage)
        {
            _speedMultiplier = speedMultiplier;
            _bombsAtTime = bombsAtTime;
            _bombsSpreading = bombsSpreading;
            _bombsCountdown = bombsCountdown;
            _bombsDamage = bombsDamage;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _speedMultiplier);
            serializer.SerializeValue(ref _bombsAtTime);
            serializer.SerializeValue(ref _bombsSpreading);
            serializer.SerializeValue(ref _bombsCountdown);
            serializer.SerializeValue(ref _bombsDamage);
        }
    }
}
