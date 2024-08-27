using Core.ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu]
    public class BaseBomberParameters : ActorBaseParams, INetworkSerializable
    {
        [SerializeField]
        public int _startPlayerHealth = 3;
        [SerializeField]
        public int _startSpeedMultiplier = 5;
        [SerializeField]
        public int _startBombsAtTime = 1;
        [SerializeField]
        public int _startBombsSpreading = 1;
        [SerializeField]
        public int _startBombsCountdown = 3;
        [SerializeField]
        public int _startBombsDamage = 1;
        
        [Space(20)]
        [Header("Current Values:")]
        [SerializeField]
        public int _speedMultiplier;
        [SerializeField]
        public int _bombsAtTime;
        [SerializeField]
        public int _bombsSpreading;
        [SerializeField]
        public int _bombsCountdown;
        [SerializeField]
        public int _bombsDamage;
        
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

        public void SetActorHealth(byte newValue) => ActorHealth = newValue;
        public void SetSpeedMultiplier(byte newValue) => SpeedMultiplier = newValue;
        public void SetBombsAtTime(byte newValue) => BombsAtTime = newValue;
        public void SetBombsSpreading(byte newValue) => BombsSpreading = newValue;
        public void SetBombsCountdown(byte newValue) => BombsCountdown = newValue;
        public void SetBombsDamage(byte newValue) => BombsDamage = newValue;

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
