using System;
using Core.ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu]
    public class BaseBomberParameters : ActorBaseParams, INetworkSerializable
    {
        [SerializeField]
        private int _startPlayerHealth = 3;
        [SerializeField]
        private int _startSpeedMultiplier = 5;
        [SerializeField]
        private int _startBombsAtTime = 1;
        [SerializeField]
        private int _startBombsSpreading = 1;
        [SerializeField]
        private int _startBombsCountdown = 3;
        [SerializeField]
        private int _startBombsDamage = 1;
        
        [Space(20)]
        [Header("Current Values:")]
        [SerializeField]
        private int _speedMultiplier;
        [SerializeField]
        private int _bombsAtTime;
        [SerializeField]
        private int _bombsSpreading;
        [SerializeField]
        private int _bombsCountdown;
        [SerializeField]
        private int _bombsDamage;


        public Action<int> OnHealthChangedEvent;
        public Action<int> OnSpeedChangedEvent;
        public Action<int> OnDamageChangedEvent;
        public Action<int> OnBombsPerTimeChangedEvent;
        public Action<int> OnSpreadingChangedEvent;
        
        public int SpeedMultiplier 
        {
            get => _speedMultiplier;
            protected set => _speedMultiplier = value;
        }
        
        public int BombsAtTime 
        {
            get => _bombsAtTime;
            protected set => _bombsAtTime = value;
        }
        
        public int BombsSpreading
        {
            get => _bombsSpreading;
            protected set => _bombsSpreading = value;
        }
        
        public int BombsCountdown
        {
            get => _bombsCountdown;
            protected set => _bombsCountdown = value;
        }
        
        public int BombsDamage
        {
            get => _bombsDamage;
            protected set => _bombsDamage = value;
        }
        
        public void ResetValues()
        {
            ActorHealth = _startPlayerHealth;
            SpeedMultiplier = _startSpeedMultiplier;
            BombsAtTime = _startBombsAtTime;
            BombsSpreading = _startBombsSpreading;
            BombsCountdown = _startBombsCountdown;
            BombsDamage = _startBombsDamage;
        }

        public void SetActorHealth(byte newValue)
        {
            ActorHealth = newValue;
            OnHealthChangedEvent?.Invoke(ActorHealth);            
        }

        public void SetSpeedMultiplier(byte newValue)
        {
            SpeedMultiplier = newValue;
            OnSpeedChangedEvent?.Invoke(_speedMultiplier);            
        }

        public void SetBombsAtTime(byte newValue)
        {
            BombsAtTime = newValue;
            OnBombsPerTimeChangedEvent?.Invoke(BombsAtTime);            
        }

        public void SetBombsSpreading(byte newValue)
        {
            BombsSpreading = newValue;
            OnSpreadingChangedEvent?.Invoke(BombsSpreading);
        }

        public void SetBombsCountdown(byte newValue) => BombsCountdown = newValue;

        public void SetBombsDamage(byte newValue)
        {
            BombsDamage = newValue;
            OnDamageChangedEvent?.Invoke(_bombsDamage);
        }


        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _actorHealth);
            serializer.SerializeValue(ref _speedMultiplier);
            serializer.SerializeValue(ref _bombsAtTime);
            serializer.SerializeValue(ref _bombsSpreading);
            serializer.SerializeValue(ref _bombsCountdown);
            serializer.SerializeValue(ref _bombsDamage);
        }
    }
}
